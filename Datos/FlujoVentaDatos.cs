using System.Data;
using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;

namespace Sistema_ERP;

internal static class FlujoVentaDatos
{
    internal static DataTable Tabla(SqlConnection cn, SqlTransaction? tx, string sql, params (string Nombre, object? Valor)[] parametros)
    {
        using var cmd = Comando(cn, tx, sql, parametros);
        using var ad = new SqlDataAdapter(cmd);
        var tabla = new DataTable(); ad.Fill(tabla); return tabla;
    }

    internal static SqlCommand Comando(SqlConnection cn, SqlTransaction? tx, string sql, params (string Nombre, object? Valor)[] parametros)
    {
        var cmd = new SqlCommand(sql, cn, tx);
        foreach (var (nombre, valor) in parametros)
        {
            if (valor is decimal importe)
            { var p = cmd.Parameters.Add(nombre, SqlDbType.Decimal); p.Precision = 18; p.Scale = 2; p.Value = importe; }
            else cmd.Parameters.AddWithValue(nombre, valor ?? DBNull.Value);
        }
        return cmd;
    }

    internal static int Ejecutar(SqlConnection cn, SqlTransaction tx, string sql, params (string Nombre, object? Valor)[] parametros)
    { using var cmd = Comando(cn, tx, sql, parametros); return cmd.ExecuteNonQuery(); }

    internal static int Id(SqlConnection cn, SqlTransaction tx, string sql, params (string Nombre, object? Valor)[] parametros)
    { using var cmd = Comando(cn, tx, sql, parametros); return Convert.ToInt32(cmd.ExecuteScalar()); }

    internal static int Preparar(SqlConnection cn, SqlTransaction tx, string motivo)
    {
        int usuario = AutorizacionDatos.Exigir(cn, Modulos.apartados, gestionar: true, transaccion: tx);
        Ejecutar(cn, tx, """
            IF (SELECT value_in_use FROM sys.configurations WHERE name='nested triggers')<>1
                OR DATABASEPROPERTYEX(DB_NAME(),'IsRecursiveTriggersEnabled')=1
                THROW 50001,'Se requieren nested triggers habilitado y RECURSIVE_TRIGGERS OFF.',1;
            IF EXISTS(SELECT 1 FROM (VALUES('trg_DetalleApartado_reservas'),('trg_DetalleApartado_bloqueo'),
                ('trg_Apartado_cierre'),('trg_Apartado_estado'),('trg_Apartado_cabecera'),('trg_Apartado_alta_baja'),
                ('trg_AbonoApartado_proteger'),('trg_Apartado_auditoria'),('trg_Venta_contabilizar'),('trg_Venta_estado'),
                ('trg_Venta_alta_baja'),('trg_Venta_cabecera'),('trg_Venta_auditoria'),('trg_DetalleVenta_bloqueo'),('trg_PagoVenta_bloqueo'),
                ('trg_DetalleDevolucion_validar'),('trg_DetalleDevolucion_bloqueo'),('trg_Devolucion_estado'),
                ('trg_Devolucion_alta_baja'),('trg_Devolucion_cabecera'),('trg_Devolucion_contabilizar'),('trg_Devolucion_auditoria'),
                ('trg_Reembolso_validar'),('trg_Reembolso_concurrencia'),('trg_Bitacora_AbonoApartado'),('trg_Bitacora_Reembolso'),
                ('trg_MovimientoInventario_validar'),('trg_MovimientoInventario_aplicar'),('trg_MovimientoInventario_documento'),
                ('trg_MovimientoInventario_inmutable')) r(nombre)
                WHERE NOT EXISTS(SELECT 1 FROM sys.triggers WHERE object_id=OBJECT_ID('dbo.'+r.nombre) AND is_disabled=0))
                THROW 50001,'Instala Triggers2.sql y Migraciones/002_bitacora_sistema.sql antes de operar apartados y devoluciones.',1;
            EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@usuario;
            EXEC sys.sp_set_session_context @key=N'motivo',@value=@motivo;
            """, ("@usuario", usuario), ("@motivo", motivo));
        return usuario;
    }

    internal static void Importe(decimal importe)
    { RegistrarImporteValidacion.Validar(importe); }

    internal static string Motivo(string motivo)
    { return RegistrarMotivoValidacion.Validar(motivo); }

    internal static bool ValidarPago(SqlConnection cn, SqlTransaction tx, int metodo, string referencia)
    {
        RegistrarPagoValidacion.LongitudReferencia(referencia);
        var tabla = Tabla(cn, tx, "SELECT codigo FROM dbo.MetodoPago WITH(HOLDLOCK) WHERE id_metodo_pago=@id AND activo=1", ("@id", metodo));
        RegistrarPagoValidacion.MetodoActivo(tabla.Rows.Count == 1);
        bool efectivo = string.Equals(Convert.ToString(tabla.Rows[0][0]), "EFECTIVO", StringComparison.OrdinalIgnoreCase);
        RegistrarPagoValidacion.ReferenciaObligatoria(efectivo, referencia);
        return efectivo;
    }

    internal static string ReferenciaPago(bool efectivo, string referencia)
    {
        if (efectivo) return "EFE-" + Guid.NewGuid().ToString("N");
        RegistrarPagoValidacion.ReferenciaManual(referencia);
        return referencia.Trim();
    }

    internal static void Evento(SqlConnection cn, SqlTransaction tx, int usuario, string modulo, string accion, int id, string descripcion)
        => Ejecutar(cn, tx, """
            INSERT dbo.Auditoria(id_usuario,modulo,accion,entidad,id_registro,descripcion,equipo)
            VALUES(@usuario,@modulo,@accion,@entidad,@id,@texto,@equipo)
            """, ("@usuario", usuario), ("@modulo", modulo), ("@accion", accion),
            ("@entidad", modulo == "APARTADOS" ? "Apartado" : "Devolucion"), ("@id", id.ToString()), ("@texto", descripcion), ("@equipo", Environment.MachineName));
}
