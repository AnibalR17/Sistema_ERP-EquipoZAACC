using Sistema_ERP.Clases;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Sistema_ERP
{

    public static class CategoriaDatos
    {
        public static DataTable Listar(bool incluirInactivas = false)
        {
            const string sql = @"SELECT id_categoria, nombre, descripcion, activo FROM dbo.Categoria
                                 WHERE activo = 1 OR @incluirInactivas = 1 ORDER BY nombre";

            using SqlConnection conexion = Conexion.AbrirConexion();
            using var adaptador = new SqlDataAdapter(sql, conexion);
            adaptador.SelectCommand.Parameters.Add("@incluirInactivas", SqlDbType.Bit).Value = incluirInactivas;
            var tabla = new DataTable();
            adaptador.Fill(tabla);
            return tabla;
        }

        public static bool ExisteNombre(string nombre, int? excluirId = null)
        {
            const string sql = @"SELECT COUNT(1) FROM dbo.Categoria
                                 WHERE nombre = @nombre
                                   AND (@excluirId IS NULL OR id_categoria <> @excluirId)";

            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = nombre.Trim();
            comando.Parameters.Add("@excluirId", SqlDbType.Int).Value = (object?)excluirId ?? DBNull.Value;
            return (int)comando.ExecuteScalar() > 0;
        }

        public static int Agregar(string nombre, string? descripcion = null)
            => Ingresar.Categoria(nombre, descripcion);

        public static bool Editar(int idCategoria, string nombre, string? descripcion)
            => Modificar.Categoria(idCategoria, nombre, descripcion);

        public static bool Desactivar(int idCategoria, string motivo)
            => Borrar.Categoria(idCategoria, motivo);

        public static bool Reactivar(int idCategoria)
            => Modificar.ReactivarCategoria(idCategoria);
    }
}
