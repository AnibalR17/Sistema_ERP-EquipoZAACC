using Microsoft.Data.SqlClient;

namespace Sistema_ERP.Clases
{
    public static class Conexion
    {
        private static string ObtenerCadenaConexion()
        {
            string? configurada = Environment.GetEnvironmentVariable("SISTEMA_ERP_CONNECTION_STRING");
            if (!string.IsNullOrWhiteSpace(configurada)) return configurada;

            string ruta = Path.Combine(AppContext.BaseDirectory, "connection.local.txt");
            if (File.Exists(ruta))
            {
                string local = File.ReadAllText(ruta).Trim();
                if (!string.IsNullOrWhiteSpace(local)) return local;
            }

            throw new InvalidOperationException(
                "Configure SISTEMA_ERP_CONNECTION_STRING o connection.local.txt para conectar a la base de datos.");
        }

        public static SqlConnection CrearConexion() => new(ObtenerCadenaConexion());

        public static SqlConnection AbrirConexion()
        {
            SqlConnection conexion = CrearConexion();

            try
            {
                conexion.Open();
                using var contexto = new SqlCommand("EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@usuario; EXEC sys.sp_set_session_context @key=N'motivo',@value=NULL;", conexion);
                contexto.Parameters.Add("@usuario", System.Data.SqlDbType.Int).Value = (object?)SesionActual.Usuario?.Id ?? DBNull.Value;
                contexto.ExecuteNonQuery();
                return conexion;
            }
            catch
            {
                conexion.Dispose();
                throw;
            }
        }
    }
}
