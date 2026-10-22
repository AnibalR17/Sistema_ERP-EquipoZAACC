using System.Data;
using Microsoft.Data.SqlClient;

namespace Sistema_ERP
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string CodigoBarras { get; set; } = "";
        public string Nombre { get; set; } = "";
        public decimal PrecioUnitario { get; set; }
        public int StockActual { get; set; }
        public int IdCategoria { get; set; }
        public int IdProveedor { get; set; }
    }

    public static class ProductoDatos
    {

        public static DataTable Listar()
        {
            const string sql = @"SELECT p.id_producto, p.codigo_barras, p.nombre,
                                        c.nombre AS categoria, p.stock_actual, p.precio_unitario,
                                        p.id_categoria, p.id_proveedor
                                 FROM Producto p
                                 INNER JOIN Categoria c ON c.id_categoria = p.id_categoria
                                 WHERE p.activo = 1
                                 ORDER BY p.nombre";

            using SqlConnection conexion = ConexionDB.AbrirConexion();
            using var adaptador = new SqlDataAdapter(sql, conexion);
            var tabla = new DataTable();
            adaptador.Fill(tabla);
            return tabla;
        }

        public static bool ExisteCodigo(string codigoBarras, int? excluirId = null)
        {
            const string sql = @"SELECT COUNT(1) FROM Producto
                                 WHERE codigo_barras = @codigo
                                   AND (@excluirId IS NULL OR id_producto <> @excluirId)";

            using SqlConnection conexion = ConexionDB.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@codigo", SqlDbType.NVarChar, 50).Value = codigoBarras.Trim();
            comando.Parameters.Add("@excluirId", SqlDbType.Int).Value = (object?)excluirId ?? DBNull.Value;
            return (int)comando.ExecuteScalar() > 0;
        }

        public static int Agregar(Producto p)
        {
            const string sql = @"INSERT INTO Producto
                                    (codigo_barras, nombre, precio_unitario, stock_actual, id_categoria, id_proveedor)
                                 OUTPUT INSERTED.id_producto
                                 VALUES (@codigo, @nombre, @precio, 0, @idCategoria, @idProveedor)";

            using SqlConnection conexion = ConexionDB.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            AgregarParametros(comando, p);
            return (int)comando.ExecuteScalar();
        }

        public static bool Editar(Producto p)
        {
            const string sql = @"UPDATE Producto
                                 SET codigo_barras = @codigo, nombre = @nombre, precio_unitario = @precio,
                                     id_categoria = @idCategoria, id_proveedor = @idProveedor
                                 WHERE id_producto = @id AND activo = 1";

            using SqlConnection conexion = ConexionDB.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            AgregarParametros(comando, p);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = p.IdProducto;
            return comando.ExecuteNonQuery() > 0;
        }

        public static bool Desactivar(int id_producto)
        {
            const string sql = "UPDATE Producto SET activo = 0 WHERE id_producto = @id AND activo = 1";
            using SqlConnection conexion = ConexionDB.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = id_producto;
            return comando.ExecuteNonQuery() > 0;
        }

        private static void AgregarParametros(SqlCommand comando, Producto p)
        {
            comando.Parameters.Add("@codigo", SqlDbType.NVarChar, 50).Value = p.CodigoBarras.Trim();
            comando.Parameters.Add("@nombre", SqlDbType.NVarChar, 150).Value = p.Nombre.Trim();
            var precio = comando.Parameters.Add("@precio", SqlDbType.Decimal);
            precio.Precision = 10;
            precio.Scale = 2;
            precio.Value = p.PrecioUnitario;
            comando.Parameters.Add("@idCategoria", SqlDbType.Int).Value = p.IdCategoria;
            comando.Parameters.Add("@idProveedor", SqlDbType.Int).Value = p.IdProveedor;
        }
    }
}
