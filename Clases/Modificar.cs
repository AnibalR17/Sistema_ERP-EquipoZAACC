using System.Data;
using Microsoft.Data.SqlClient;

namespace Sistema_ERP.Clases
{
    public static class Modificar
    {
        public static bool ReactivarProducto(int idProducto)
            => Reactivar("Producto", "id_producto", idProducto);

        public static bool ReactivarProveedor(int idProveedor)
            => Reactivar("Proveedor", "id_proveedor", idProveedor);

        public static bool ReactivarCategoria(int idCategoria)
            => Reactivar("Categoria", "id_categoria", idCategoria);

        private static bool Reactivar(string tabla, string columnaId, int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            // Los identificadores provienen exclusivamente de los métodos de esta clase.
            string sql = $@"UPDATE dbo.{tabla}
                            SET desactivado_en = NULL, desactivado_por = NULL,
                                motivo_desactivacion = NULL, actualizado_en = SYSUTCDATETIME()
                            WHERE {columnaId} = @id AND activo = 0;
                            SELECT CONVERT(int, @@ROWCOUNT);";

            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
            return (int)comando.ExecuteScalar() > 0;
        }

        public static bool Proveedor(Proveedor proveedor)
        {
            proveedor.Validar();
            if (proveedor.IdProveedor <= 0) throw new ArgumentOutOfRangeException(nameof(proveedor.IdProveedor));
            const string sql = @"UPDATE dbo.Proveedor
                                 SET nombre_empresa = @nombre, rtn = @rtn, contacto = @contacto,
                                     telefono = @telefono, correo = @correo, direccion = @direccion,
                                     actualizado_en = SYSUTCDATETIME()
                                 WHERE id_proveedor = @id AND activo = 1;
                                 SELECT CONVERT(int, @@ROWCOUNT);";
            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            ProveedorDatos.AgregarParametros(comando, proveedor);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = proveedor.IdProveedor;
            return (int)comando.ExecuteScalar() > 0;
        }

        public static bool Categoria(int idCategoria, string nombre, string? descripcion)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
            if (idCategoria <= 0) throw new ArgumentOutOfRangeException(nameof(idCategoria));
            if (nombre.Trim().Length > 100) throw new ArgumentException("El nombre admite hasta 100 caracteres.", nameof(nombre));
            if (descripcion?.Trim().Length > 300) throw new ArgumentException("La descripción admite hasta 300 caracteres.", nameof(descripcion));

            const string sql = @"UPDATE dbo.Categoria SET nombre = @nombre, descripcion = @descripcion,
                                 actualizado_en = SYSUTCDATETIME()
                                 WHERE id_categoria = @id AND activo = 1;
                                 SELECT CONVERT(int, @@ROWCOUNT);";

            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = nombre.Trim();
            comando.Parameters.Add("@id", SqlDbType.Int).Value = idCategoria;
            comando.Parameters.Add("@descripcion", SqlDbType.NVarChar, 300).Value =
                string.IsNullOrWhiteSpace(descripcion) ? DBNull.Value : descripcion.Trim();
            return (int)comando.ExecuteScalar() > 0;
        }
    }
}
