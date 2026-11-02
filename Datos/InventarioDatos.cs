using System.Data;
using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;

namespace Sistema_ERP
{
    public static class InventarioDatos
    {
        public static DataTable Existencias(int tienda, string buscar)
        {
            using var conexion = Conexion.AbrirConexion();
            SesionActual.Validar(conexion);
            using var adaptador = new SqlDataAdapter("""
                SELECT p.id_producto, p.codigo_barras AS Codigo, p.nombre AS Producto,
                       ISNULL(i.existencia,0) AS Existencia, ISNULL(i.reservado,0) AS Reservado,
                       ISNULL(i.existencia-i.reservado,0) AS Disponible, ISNULL(i.stock_minimo,0) AS Minimo,
                       p.activo AS Activo, i.version_fila
                FROM dbo.Producto p LEFT JOIN dbo.InventarioTienda i ON i.id_producto=p.id_producto AND i.id_tienda=@tienda
                WHERE (p.activo=1 OR i.id_producto IS NOT NULL)
                AND (@buscar=N'' OR CHARINDEX(@buscar,p.nombre)>0 OR CHARINDEX(@buscar,p.codigo_barras)>0)
                ORDER BY p.nombre
                """, conexion);
            adaptador.SelectCommand.Parameters.Add("@tienda", SqlDbType.Int).Value = tienda;
            adaptador.SelectCommand.Parameters.Add("@buscar", SqlDbType.NVarChar, 150).Value = buscar.Trim();
            var tabla = new DataTable();
            adaptador.Fill(tabla);
            return tabla;
        }

        public static DataTable Movimientos(int tienda, string buscar, DateTime desde, DateTime hasta)
        {
            if (desde.Date > hasta.Date || hasta.Date == DateTime.MaxValue.Date) throw new ArgumentException("Revisa el intervalo de fechas.");
            using var conexion = Conexion.AbrirConexion();
            SesionActual.Validar(conexion);
            using var adaptador = new SqlDataAdapter("""
                SELECT m.id_movimiento AS Id, m.fecha AS FechaUTC, p.nombre AS Producto, m.tipo AS Tipo,
                       m.cantidad_delta AS Cantidad, u.nombre_usuario AS Usuario, m.motivo AS Motivo,
                       dt.id_traspaso AS Traspaso, m.id_movimiento_revertido AS ReversoDe,
                       CONVERT(bit,CASE WHEN r.id_movimiento IS NULL THEN 0 ELSE 1 END) AS Revertido
                FROM dbo.MovimientoInventario m JOIN dbo.Producto p ON p.id_producto=m.id_producto
                JOIN dbo.Usuario u ON u.id_usuario=m.id_usuario
                LEFT JOIN dbo.DetalleTraspaso dt ON dt.id_detalle_traspaso=m.id_detalle_traspaso
                LEFT JOIN dbo.MovimientoInventario r ON r.id_movimiento_revertido=m.id_movimiento
                WHERE m.id_tienda=@tienda AND m.fecha>=@desde AND m.fecha<@hasta
                AND (@buscar=N'' OR CHARINDEX(@buscar,p.nombre)>0 OR CHARINDEX(@buscar,p.codigo_barras)>0
                     OR CHARINDEX(@buscar,m.tipo)>0 OR CHARINDEX(@buscar,ISNULL(m.motivo,N''))>0)
                ORDER BY m.id_movimiento DESC
                """, conexion);
            adaptador.SelectCommand.Parameters.Add("@tienda", SqlDbType.Int).Value = tienda;
            adaptador.SelectCommand.Parameters.Add("@buscar", SqlDbType.NVarChar, 150).Value = buscar.Trim();
            adaptador.SelectCommand.Parameters.Add("@desde", SqlDbType.DateTime2).Value = desde.Date;
            adaptador.SelectCommand.Parameters.Add("@hasta", SqlDbType.DateTime2).Value = hasta.Date.AddDays(1);
            var tabla = new DataTable();
            adaptador.Fill(tabla);
            return tabla;
        }

        public static void Ajustar(int tienda, int producto, int cantidad, string motivo, byte[]? version)
        {
            if (tienda <= 0 || producto <= 0 || cantidad == 0 || cantidad == int.MinValue)
                throw new ArgumentException("Selecciona tienda y producto e indica una cantidad distinta de cero dentro del rango permitido.");
            ValidarMotivo(motivo);
            using var conexion = Conexion.AbrirConexion();
            using var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
            int usuario = Preparar(conexion, transaccion, motivo.Trim());
            ComprobarVersion(conexion, transaccion, tienda, producto, version);
            using var comando = new SqlCommand("""
                IF NOT EXISTS(SELECT 1 FROM dbo.Tienda WITH(HOLDLOCK) WHERE id_tienda=@tienda AND activo=1)
                    OR NOT EXISTS(SELECT 1 FROM dbo.Producto WITH(HOLDLOCK) WHERE id_producto=@producto AND activo=1)
                    THROW 50001, 'La tienda y el producto deben estar activos.', 1;
                INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,motivo)
                VALUES(@tienda,@producto,'AJUSTE',@cantidad,@usuario,@motivo);
                """, conexion, transaccion);
            comando.Parameters.Add("@tienda", SqlDbType.Int).Value = tienda;
            comando.Parameters.Add("@producto", SqlDbType.Int).Value = producto;
            comando.Parameters.Add("@cantidad", SqlDbType.Int).Value = cantidad;
            comando.Parameters.Add("@usuario", SqlDbType.Int).Value = usuario;
            comando.Parameters.Add("@motivo", SqlDbType.NVarChar, 500).Value = motivo.Trim();
            comando.ExecuteNonQuery();
            transaccion.Commit();
        }

        public static void GuardarMinimo(int tienda, int producto, int minimo, byte[]? version)
        {
            if (tienda <= 0 || producto <= 0 || minimo < 0) throw new ArgumentException("Selecciona tienda y producto e indica un mínimo no negativo.");
            using var conexion = Conexion.AbrirConexion();
            using var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
            Preparar(conexion, transaccion, "Cambio de stock mínimo");
            ComprobarVersion(conexion, transaccion, tienda, producto, version);
            using var comando = new SqlCommand("""
                IF NOT EXISTS(SELECT 1 FROM dbo.Tienda WITH(HOLDLOCK) WHERE id_tienda=@tienda AND activo=1)
                    OR NOT EXISTS(SELECT 1 FROM dbo.Producto WITH(HOLDLOCK) WHERE id_producto=@producto AND activo=1)
                    THROW 50001, 'La tienda y el producto deben estar activos.', 1;
                IF EXISTS(SELECT 1 FROM dbo.InventarioTienda WHERE id_tienda=@tienda AND id_producto=@producto)
                    UPDATE dbo.InventarioTienda SET stock_minimo=@minimo WHERE id_tienda=@tienda AND id_producto=@producto;
                ELSE INSERT dbo.InventarioTienda(id_tienda,id_producto,stock_minimo) VALUES(@tienda,@producto,@minimo);
                """, conexion, transaccion);
            comando.Parameters.Add("@tienda", SqlDbType.Int).Value = tienda;
            comando.Parameters.Add("@producto", SqlDbType.Int).Value = producto;
            comando.Parameters.Add("@minimo", SqlDbType.Int).Value = minimo;
            comando.ExecuteNonQuery();
            transaccion.Commit();
        }

        public static void RevertirAjuste(long movimiento, string motivo)
        {
            if (movimiento <= 0) throw new ArgumentException("Selecciona un ajuste.");
            ValidarMotivo(motivo);
            using var conexion = Conexion.AbrirConexion();
            using var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
            int usuario = Preparar(conexion, transaccion, motivo.Trim());
            using var comando = new SqlCommand("""
                IF NOT EXISTS(SELECT 1 FROM dbo.MovimientoInventario WITH(UPDLOCK,HOLDLOCK) WHERE id_movimiento=@id AND tipo='AJUSTE')
                    THROW 50001, 'Solo se pueden revertir ajustes manuales.', 1;
                IF EXISTS(SELECT 1 FROM dbo.MovimientoInventario WITH(UPDLOCK,HOLDLOCK) WHERE id_movimiento_revertido=@id)
                    THROW 50001, 'El ajuste ya fue revertido. Actualiza el listado.', 1;
                INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,motivo,id_movimiento_revertido)
                SELECT id_tienda,id_producto,'REVERSO',-cantidad_delta,@usuario,@motivo,id_movimiento
                FROM dbo.MovimientoInventario WHERE id_movimiento=@id;
                """, conexion, transaccion);
            comando.Parameters.Add("@id", SqlDbType.BigInt).Value = movimiento;
            comando.Parameters.Add("@usuario", SqlDbType.Int).Value = usuario;
            comando.Parameters.Add("@motivo", SqlDbType.NVarChar, 500).Value = motivo.Trim();
            comando.ExecuteNonQuery();
            transaccion.Commit();
        }

        private static void ValidarMotivo(string motivo)
        {
            if (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length > 500)
                throw new ArgumentException("Introduce un motivo de hasta 500 caracteres.");
        }

        private static void ComprobarVersion(SqlConnection conexion, SqlTransaction transaccion, int tienda, int producto, byte[]? version)
        {
            using var comando = new SqlCommand("SELECT version_fila FROM dbo.InventarioTienda WITH(UPDLOCK,HOLDLOCK) WHERE id_tienda=@tienda AND id_producto=@producto", conexion, transaccion);
            comando.Parameters.Add("@tienda", SqlDbType.Int).Value = tienda;
            comando.Parameters.Add("@producto", SqlDbType.Int).Value = producto;
            byte[]? actual = comando.ExecuteScalar() as byte[];
            if ((actual is null) != (version is null) || (actual != null && !actual.SequenceEqual(version!)))
                throw new InvalidOperationException("El inventario cambió. Actualiza la pantalla antes de guardar.");
        }

        private static int Preparar(SqlConnection conexion, SqlTransaction transaccion, string motivo)
        {
            int usuario = SesionActual.Validar(conexion, transaccion);
            using var comando = new SqlCommand("""
                IF EXISTS(SELECT requerido.nombre FROM (VALUES (N'trg_MovimientoInventario_aplicar'),
                    (N'trg_MovimientoInventario_validar'),(N'trg_MovimientoInventario_inmutable')) requerido(nombre)
                    WHERE NOT EXISTS(SELECT 1 FROM sys.triggers t WHERE t.object_id=OBJECT_ID(N'dbo.'+requerido.nombre) AND t.is_disabled=0))
                    THROW 50004, 'Instale Triggers2.sql antes de registrar movimientos.', 1;
                EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@usuario;
                EXEC sys.sp_set_session_context @key=N'motivo',@value=@motivo;
                """, conexion, transaccion);
            comando.Parameters.Add("@usuario", SqlDbType.Int).Value = usuario;
            comando.Parameters.Add("@motivo", SqlDbType.NVarChar, 500).Value = motivo;
            comando.ExecuteNonQuery();
            return usuario;
        }
    }
}
