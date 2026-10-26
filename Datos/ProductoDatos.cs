using Sistema_ERP.Clases;
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
        public bool Activo { get; set; } = true;
    }

    public static class ProductoDatos
    {

        public static DataTable Listar(bool incluirInactivos = false)
        {
            const string sql = @"SELECT p.id_producto, p.codigo_barras, p.nombre,
                                        c.nombre AS categoria, p.stock_actual, p.precio_unitario,
                                        p.id_categoria, p.id_proveedor, p.activo, p.motivo_desactivacion
                                 FROM dbo.vw_ProductoStock p
                                 INNER JOIN dbo.Categoria c ON c.id_categoria = p.id_categoria
                                 WHERE p.activo = 1 OR @incluirInactivos = 1
                                 ORDER BY p.nombre";

            using SqlConnection conexion = Conexion.AbrirConexion();
            using var adaptador = new SqlDataAdapter(sql, conexion);
            adaptador.SelectCommand.Parameters.Add("@incluirInactivos", SqlDbType.Bit).Value = incluirInactivos;
            var tabla = new DataTable();
            adaptador.Fill(tabla);
            return tabla;
        }

        public static bool ExisteCodigo(string codigoBarras, int? excluirId = null)
        {
            const string sql = @"SELECT COUNT(1) FROM dbo.Producto
                                 WHERE codigo_barras = @codigo
                                   AND (@excluirId IS NULL OR id_producto <> @excluirId)";

            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@codigo", SqlDbType.NVarChar, 50).Value = codigoBarras.Trim();
            comando.Parameters.Add("@excluirId", SqlDbType.Int).Value = (object?)excluirId ?? DBNull.Value;
            return (int)comando.ExecuteScalar() > 0;
        }

        public static int Agregar(Producto producto)
        {
            const string sql = @"DECLARE @insertados TABLE (id_producto int);
                                 INSERT INTO dbo.Producto
                                    (codigo_barras, nombre, precio_unitario, id_categoria, id_proveedor)
                                 OUTPUT INSERTED.id_producto INTO @insertados
                                 VALUES (@codigo, @nombre, @precio, @idCategoria, @idProveedor);
                                 SELECT id_producto FROM @insertados;";

            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            AgregarParametros(comando, producto);
            return (int)comando.ExecuteScalar();
        }

        public static bool Editar(Producto producto)
        {
            const string sql = @"UPDATE dbo.Producto
                                 SET codigo_barras = @codigo, nombre = @nombre, precio_unitario = @precio,
                                     id_categoria = @idCategoria, id_proveedor = @idProveedor
                                 WHERE id_producto = @id AND activo = 1;
                                 SELECT CONVERT(int, @@ROWCOUNT);";

            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            AgregarParametros(comando, producto);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = producto.IdProducto;
            return (int)comando.ExecuteScalar() > 0;
        }

        public static bool Desactivar(int idProducto, string motivo) => Borrar.Producto(idProducto, motivo);
        public static bool Reactivar(int idProducto) => Modificar.ReactivarProducto(idProducto);

        private static void AgregarParametros(SqlCommand comando, Producto producto)
        {
            comando.Parameters.Add("@codigo", SqlDbType.NVarChar, 50).Value = producto.CodigoBarras.Trim();
            comando.Parameters.Add("@nombre", SqlDbType.NVarChar, 150).Value = producto.Nombre.Trim();
            var precio = comando.Parameters.Add("@precio", SqlDbType.Decimal);
            precio.Precision = 10;
            precio.Scale = 2;
            precio.Value = producto.PrecioUnitario;
            comando.Parameters.Add("@idCategoria", SqlDbType.Int).Value = producto.IdCategoria;
            comando.Parameters.Add("@idProveedor", SqlDbType.Int).Value = producto.IdProveedor;
        }
    }
}
