using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace Sistema_ERP.Datos;

internal static class PreparacionBaseDatos
{
    private const string recursoEsquema = "Sistema_ERP.Setup.Esquema.sql";
    private const string recursoTriggers = "Sistema_ERP.Setup.Triggers.sql";
    private const string recursoBitacora = "Sistema_ERP.Setup.Bitacora.sql";
    private const string recursoPasskeys = "Sistema_ERP.Setup.Passkeys.sql";

    internal static bool Asegurar(Action<string> informar)
    {
        using var configuracion = Clases.Conexion.CrearConexion();
        var cadena = new SqlConnectionStringBuilder(configuracion.ConnectionString);
        string baseDatos = cadena.InitialCatalog.Trim();
        if (baseDatos.Length == 0)
            throw new InvalidOperationException("La cadena de conexión debe indicar la base de datos del ERP, por ejemplo Database=EquipoZAACC.");
        if (baseDatos.Length > 128 || new[] { "master", "model", "msdb", "tempdb" }.Contains(baseDatos, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("La cadena de conexión debe apuntar a una base de datos del ERP, no a una base del sistema.");

        cadena.InitialCatalog = "master";
        using var conexion = new SqlConnection(cadena.ConnectionString);
        conexion.Open();

        bool existe = ExisteBase(conexion, baseDatos);
        if (!existe)
        {
            EjecutarRecurso(conexion, recursoEsquema, baseDatos, "esquema de la base de datos", informar);
            conexion.ChangeDatabase(baseDatos);
            AplicarObjetosFaltantes(conexion, baseDatos, informar);
            VerificarPreparacion(conexion);
            return true;
        }

        conexion.ChangeDatabase(baseDatos);
        var estado = LeerEstado(conexion);
        if (estado.objetos == 0)
        {
            EjecutarRecurso(conexion, recursoEsquema, baseDatos, "esquema de la base de datos", informar);
            conexion.ChangeDatabase(baseDatos);
            AplicarObjetosFaltantes(conexion, baseDatos, informar);
            VerificarPreparacion(conexion);
            return true;
        }

        if (!estado.usuario)
            throw new InvalidOperationException($"La base «{baseDatos}» ya contiene objetos, pero no tiene dbo.Usuario. No se ejecutó el esquema para proteger esos datos. Seleccione la base ERP correcta o migre esa base por separado.");
        if (!estado.inventarioTienda || !estado.auditoria)
            throw new InvalidOperationException($"La base «{baseDatos}» tiene un esquema incompleto o anterior. Se requiere dbo.InventarioTienda y dbo.Auditoria; no se modificaron sus datos.");

        bool cambios = AplicarObjetosFaltantes(conexion, baseDatos, informar);
        VerificarPreparacion(conexion);
        return cambios;
    }

    private static void VerificarPreparacion(SqlConnection conexion)
    {
        var estado = LeerEstado(conexion);
        if (!estado.usuario || !estado.inventarioTienda || !estado.auditoria ||
            !estado.triggersNegocio || !estado.bitacora || !estado.passkeys)
            throw new InvalidOperationException("La preparación terminó sin dejar instalados el esquema, los triggers y la tabla de passkeys requeridos.");
    }

    private static bool AplicarObjetosFaltantes(SqlConnection conexion, string baseDatos, Action<string> informar)
    {
        bool cambios = false;
        var estado = LeerEstado(conexion);
        if (!estado.triggersNegocio)
        {
            EjecutarRecurso(conexion, recursoTriggers, baseDatos, "triggers de inventario, ventas y seguridad", informar);
            cambios = true;
            estado = LeerEstado(conexion);
        }
        if (!estado.bitacora)
        {
            EjecutarRecurso(conexion, recursoBitacora, baseDatos, "triggers de bitácora", informar);
            cambios = true;
            estado = LeerEstado(conexion);
        }
        if (!estado.passkeys)
        {
            EjecutarRecurso(conexion, recursoPasskeys, baseDatos, "activación de passkeys", informar);
            cambios = true;
        }
        return cambios;
    }

    private static bool ExisteBase(SqlConnection conexion, string baseDatos)
    {
        using var comando = new SqlCommand("SELECT CASE WHEN DB_ID(@base) IS NULL THEN 0 ELSE 1 END;", conexion);
        comando.Parameters.Add("@base", SqlDbType.NVarChar, 128).Value = baseDatos;
        return Convert.ToInt32(comando.ExecuteScalar()) == 1;
    }

    private static (int objetos, bool usuario, bool inventarioTienda, bool auditoria, bool triggersNegocio,
        bool bitacora, bool passkeys) LeerEstado(SqlConnection conexion)
    {
        using var comando = new SqlCommand("""
            SELECT
                (SELECT COUNT(*) FROM sys.objects WHERE is_ms_shipped=0),
                OBJECT_ID(N'dbo.Usuario',N'U'),
                OBJECT_ID(N'dbo.InventarioTienda',N'U'),
                OBJECT_ID(N'dbo.Auditoria',N'U'),
                (SELECT COUNT(*) FROM sys.triggers WHERE name IN
                    (N'trg_MovimientoInventario_aplicar',N'trg_Venta_contabilizar',N'trg_Usuario_seguridad',
                     N'trg_Auditoria_inmutable',N'trg_DetalleApartado_reservas')),
                CASE WHEN EXISTS(SELECT 1 FROM sys.triggers
                    WHERE parent_id=OBJECT_ID(N'dbo.Usuario') AND name=N'trg_Bitacora_Usuario') THEN 1 ELSE 0 END,
                CASE WHEN OBJECT_ID(N'dbo.PasskeyUsuario',N'U') IS NOT NULL
                    AND EXISTS(SELECT 1 FROM sys.triggers
                        WHERE parent_id=OBJECT_ID(N'dbo.PasskeyUsuario') AND name=N'trg_Bitacora_PasskeyUsuario')
                    AND OBJECT_DEFINITION(OBJECT_ID(N'dbo.trg_Bitacora_PasskeyUsuario',N'TR')) LIKE N'%Se registró un passkey%'
                    AND EXISTS(SELECT 1 FROM sys.key_constraints
                        WHERE parent_object_id=OBJECT_ID(N'dbo.PasskeyUsuario') AND name=N'PK_PasskeyUsuario' AND type=N'PK')
                    AND NOT EXISTS(SELECT 1 FROM sys.indexes
                        WHERE object_id=OBJECT_ID(N'dbo.PasskeyUsuario') AND name=N'PK_PasskeyUsuario' AND type=1)
                    AND NOT EXISTS(SELECT 1 FROM sys.indexes
                        WHERE object_id=OBJECT_ID(N'dbo.PasskeyUsuario') AND name=N'UX_PasskeyUsuario_user_handle')
                    THEN 1 ELSE 0 END
            """, conexion);
        using var lector = comando.ExecuteReader();
        if (!lector.Read()) throw new InvalidOperationException("No se pudo consultar el estado del esquema ERP.");
        return (lector.GetInt32(0), !lector.IsDBNull(1), !lector.IsDBNull(2), !lector.IsDBNull(3),
            lector.GetInt32(4) == 5, lector.GetInt32(5) == 1, lector.GetInt32(6) == 1);
    }

    private static void EjecutarRecurso(SqlConnection conexion, string nombreRecurso, string baseDatos,
        string nombrePaso, Action<string> informar)
    {
        informar($"Aplicando {nombrePaso}…");
        using Stream? recurso = typeof(PreparacionBaseDatos).Assembly.GetManifestResourceStream(nombreRecurso);
        if (recurso is null) throw new InvalidOperationException($"No se encontró el script integrado {nombreRecurso}.");
        using var lector = new StreamReader(recurso, detectEncodingFromByteOrderMarks: true);
        string script = AdaptarBaseDatos(lector.ReadToEnd(), baseDatos);
        string[] lotes = Regex.Split(script, @"(?im)^[ \t]*GO[ \t]*(?:--[^\r\n]*)?\r?$");
        int numeroLote = 0;
        foreach (string lote in lotes)
        {
            if (string.IsNullOrWhiteSpace(lote)) continue;
            numeroLote++;
            try
            {
                using var comando = new SqlCommand(lote, conexion) { CommandTimeout = 180 };
                comando.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"Falló «{nombrePaso}», lote {numeroLote}: {ex.Message}", ex);
            }
        }
    }

    private static string AdaptarBaseDatos(string script, string baseDatos)
    {
        string literal = baseDatos.Replace("'", "''", StringComparison.Ordinal);
        string identificador = new SqlCommandBuilder().QuoteIdentifier(baseDatos);
        script = script.Replace("DB_ID(N'EquipoZAACC')", $"DB_ID(N'{literal}')", StringComparison.OrdinalIgnoreCase);
        script = script.Replace("CREATE DATABASE [EquipoZAACC]", $"CREATE DATABASE {identificador}", StringComparison.OrdinalIgnoreCase);
        return Regex.Replace(script, @"(?im)^([ \t]*USE[ \t]+)EquipoZAACC(?=[ \t]*;?[ \t]*\r?$)",
            match => match.Groups[1].Value + identificador);
    }
}
