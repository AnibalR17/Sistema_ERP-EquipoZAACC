using Microsoft.Data.SqlClient;

namespace Sistema_ERP.Clases
{
    public static class Conexion
    {
        private static string ObtenerCadenaConexion()
        {
            string? configurada = LeerCadenaConfigurada();
            if (!string.IsNullOrWhiteSpace(configurada)) return configurada;

            throw new InvalidOperationException(
                "Configure la conexión a SQL Server para poder iniciar el sistema.");
        }

        internal static string? LeerCadenaConfigurada()
        {
            string? protegida = ConfiguracionConexion.Leer();
            if (!string.IsNullOrWhiteSpace(protegida)) return protegida;

            string? variable = Environment.GetEnvironmentVariable("SISTEMA_ERP_CONNECTION_STRING");
            if (!string.IsNullOrWhiteSpace(variable)) return variable;

            string ruta = Path.Combine(AppContext.BaseDirectory, "connection.local.txt");
            if (!File.Exists(ruta)) return null;
            string local = File.ReadAllText(ruta).Trim();
            return string.IsNullOrWhiteSpace(local) ? null : local;
        }

        internal static void GuardarCadenaConexion(string cadena) => ConfiguracionConexion.Guardar(cadena);

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
