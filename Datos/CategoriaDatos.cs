using System.Data;
using Microsoft.Data.SqlClient;

namespace Sistema_ERP
{
    
    public static class CategoriaDatos
    {

        public static DataTable Listar()
        {
            const string sql = "SELECT id_categoria, nombre FROM Categoria WHERE activo = 1 ORDER BY nombre";

            using SqlConnection conexion = ConexionDB.AbrirConexion();
            using var adaptador = new SqlDataAdapter(sql, conexion);
            var tabla = new DataTable();
            adaptador.Fill(tabla);
            return tabla;
        }

        public static bool ExisteNombre(string nombre, int? excluirId = null)
        {
            const string sql = @"SELECT COUNT(1) FROM Categoria
                                 WHERE nombre = @nombre
                                   AND (@excluirId IS NULL OR id_categoria <> @excluirId)";

            using SqlConnection conexion = ConexionDB.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = nombre.Trim();
            comando.Parameters.Add("@excluirId", SqlDbType.Int).Value = (object?)excluirId ?? DBNull.Value;
            return (int)comando.ExecuteScalar() > 0;
        }

        public static int Agregar(string nombre)
        {
            const string sql = @"INSERT INTO Categoria (nombre)
                                 OUTPUT INSERTED.id_categoria
                                 VALUES (@nombre)";

            using SqlConnection conexion = ConexionDB.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = nombre.Trim();
            return (int)comando.ExecuteScalar();
        }

        public static bool Editar(int idCategoria, string nombre)
        {
            const string sql = "UPDATE Categoria SET nombre = @nombre WHERE id_categoria = @id";

            using SqlConnection conexion = ConexionDB.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = nombre.Trim();
            comando.Parameters.Add("@id", SqlDbType.Int).Value = idCategoria;
            return comando.ExecuteNonQuery() > 0;
        }

        public static bool Desactivar(int id_categoria)
        {
            const string sql = "UPDATE Categoria SET activo = 0 WHERE id_categoria = @id AND activo = 1";
            using SqlConnection conexion = ConexionDB.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = id_categoria;
            return comando.ExecuteNonQuery() > 0;
        }

    }
}
