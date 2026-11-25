using Microsoft.Data.SqlClient;

namespace Sistema_ERP.Clases;

internal static class ValidadorConexion
{
    internal static string Verificar(string cadena)
    {
        SqlConnectionStringBuilder destino = new(cadena);
        string baseDatos = destino.InitialCatalog.Trim();
        if (string.IsNullOrWhiteSpace(destino.DataSource))
            throw new InvalidOperationException("Escribe el servidor o la instancia de SQL Server.");
        if (baseDatos.Length == 0)
            throw new InvalidOperationException("Escribe el nombre de la base de datos del ERP.");
        if (baseDatos.Length > 128 || new[] { "master", "model", "msdb", "tempdb" }
                .Contains(baseDatos, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("El nombre debe ser una base de datos del ERP, no una base del sistema.");

        var administrador = new SqlConnectionStringBuilder(destino.ConnectionString)
        {
            InitialCatalog = "master",
            ConnectTimeout = 8
        };

        int? idBase;
        bool puedeCrearBase;
        using (var conexion = new SqlConnection(administrador.ConnectionString))
        {
            conexion.Open();
            using var comando = new SqlCommand("""
                SELECT DB_ID(@base),
                       CONVERT(int, CASE WHEN
                           HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'CREATE DATABASE') = 1 OR
                           HAS_PERMS_BY_NAME(NULL, 'SERVER', 'CREATE ANY DATABASE') = 1 OR
                           HAS_PERMS_BY_NAME(NULL, 'SERVER', 'ALTER ANY DATABASE') = 1
                           THEN 1 ELSE 0 END);
                """, conexion);
            comando.Parameters.Add("@base", System.Data.SqlDbType.NVarChar, 128).Value = baseDatos;
            using var lector = comando.ExecuteReader();
            if (!lector.Read()) throw new InvalidOperationException("No se pudo verificar el acceso al servidor.");
            // DB_ID puede llegar como Int16 según el proveedor/servidor; GetInt32
            // exige que el tipo CLR ya sea exactamente Int32 y no hace conversión.
            idBase = lector.IsDBNull(0) ? null : Convert.ToInt32(lector.GetValue(0));
            puedeCrearBase = lector.GetInt32(1) == 1;
        }

        if (idBase.HasValue)
        {
            destino.InitialCatalog = baseDatos;
            destino.ConnectTimeout = 8;
            using var conexionBase = new SqlConnection(destino.ConnectionString);
            conexionBase.Open();
            return $"Conexión correcta. La base de datos «{baseDatos}» existe y es accesible.";
        }

        if (!puedeCrearBase)
            throw new InvalidOperationException(
                $"El servidor responde, pero la base «{baseDatos}» no existe y esta cuenta no tiene permiso para crearla.");

        return $"Conexión correcta. La base «{baseDatos}» se creará al iniciar el ERP.";
    }
}
