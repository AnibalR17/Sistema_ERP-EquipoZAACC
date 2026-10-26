using Microsoft.Data.SqlClient;

namespace Sistema_ERP.Clases
{
    public static class Conexion
    {
        private static string ConnectionString
        {
            get
            {
                string? configured = Environment.GetEnvironmentVariable("SISTEMA_ERP_CONNECTION_STRING");
                if (!string.IsNullOrWhiteSpace(configured)) return configured;

                string path = Path.Combine(AppContext.BaseDirectory, "connection.local.txt");
                if (File.Exists(path))
                {
                    string local = File.ReadAllText(path).Trim();
                    if (!string.IsNullOrWhiteSpace(local)) return local;
                }

                throw new InvalidOperationException(
                    "Configure SISTEMA_ERP_CONNECTION_STRING o connection.local.txt para conectar a la base de datos.");
            }
        }

        public static SqlConnection CrearConexion() => new(ConnectionString);

        public static SqlConnection AbrirConexion()
        {
            SqlConnection conexion = CrearConexion();

            try
            {
                conexion.Open();
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
