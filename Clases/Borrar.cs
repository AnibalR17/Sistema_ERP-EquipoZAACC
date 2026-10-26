using System.Data;
using Microsoft.Data.SqlClient;

namespace Sistema_ERP.Clases
{
    public static class Borrar
    {
        public static bool Producto(int idProducto, string motivo)
            => Desactivar("Producto", "id_producto", idProducto, motivo);

        public static bool Proveedor(int idProveedor, string motivo)
            => Desactivar("Proveedor", "id_proveedor", idProveedor, motivo);

        public static bool Categoria(int idCategoria, string motivo)
            => Desactivar("Categoria", "id_categoria", idCategoria, motivo);

        public static bool Tienda(int idTienda, string motivo)
            => Desactivar("Tienda", "id_tienda", idTienda, motivo);

        private static bool Desactivar(string tabla, string columnaId, int id, string motivo)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            ArgumentException.ThrowIfNullOrWhiteSpace(motivo);
            if (motivo.Trim().Length > 300)
                throw new ArgumentException("El motivo admite hasta 300 caracteres.", nameof(motivo));

            // Los identificadores provienen exclusivamente de los métodos de esta clase.
            string sql = $@"UPDATE dbo.{tabla}
                            SET desactivado_en = SYSUTCDATETIME(), motivo_desactivacion = @motivo,
                                actualizado_en = SYSUTCDATETIME()
                            WHERE {columnaId} = @id AND activo = 1;
                            SELECT CONVERT(int, @@ROWCOUNT);";

            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
            comando.Parameters.Add("@motivo", SqlDbType.NVarChar, 300).Value = motivo.Trim();
            return (int)comando.ExecuteScalar() > 0;
        }
    }
}
