using System.Data;
using Microsoft.Data.SqlClient;

namespace Sistema_ERP.Clases
{
    public static class Modificar
    {
        public static bool ReactivarProducto(int idProducto)
        {
            if (idProducto <= 0) throw new ArgumentOutOfRangeException(nameof(idProducto));
            const string sql = @"UPDATE dbo.Producto
                                 SET desactivado_en = NULL, desactivado_por = NULL,
                                     motivo_desactivacion = NULL, actualizado_en = SYSUTCDATETIME()
                                 WHERE id_producto = @id AND activo = 0;
                                 SELECT CONVERT(int, @@ROWCOUNT);";
            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = idProducto;
            return (int)comando.ExecuteScalar() > 0;
        }

        public static bool ReactivarProveedor(int idProveedor)
        {
            if (idProveedor <= 0) throw new ArgumentOutOfRangeException(nameof(idProveedor));
            const string sql = @"UPDATE dbo.Proveedor
                                 SET desactivado_en = NULL, desactivado_por = NULL,
                                     motivo_desactivacion = NULL, actualizado_en = SYSUTCDATETIME()
                                 WHERE id_proveedor = @id AND activo = 0;
                                 SELECT CONVERT(int, @@ROWCOUNT);";
            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = idProveedor;
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

        public static bool ReactivarCategoria(int idCategoria)
        {
            if (idCategoria <= 0) throw new ArgumentOutOfRangeException(nameof(idCategoria));
            const string sql = @"UPDATE dbo.Categoria
                                 SET desactivado_en = NULL, desactivado_por = NULL,
                                     motivo_desactivacion = NULL, actualizado_en = SYSUTCDATETIME()
                                 WHERE id_categoria = @id AND activo = 0";
            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = idCategoria;
            return comando.ExecuteNonQuery() > 0;
        }

        public static bool Categoria(int idCategoria, string nombre, string? descripcion)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
            if (idCategoria <= 0) throw new ArgumentOutOfRangeException(nameof(idCategoria));
            if (nombre.Trim().Length > 100) throw new ArgumentException("El nombre admite hasta 100 caracteres.", nameof(nombre));
            if (descripcion?.Trim().Length > 300) throw new ArgumentException("La descripción admite hasta 300 caracteres.", nameof(descripcion));

            const string sql = @"UPDATE dbo.Categoria SET nombre = @nombre, descripcion = @descripcion,
                                 actualizado_en = SYSUTCDATETIME()
                                 WHERE id_categoria = @id AND activo = 1";

            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = nombre.Trim();
            comando.Parameters.Add("@id", SqlDbType.Int).Value = idCategoria;
            comando.Parameters.Add("@descripcion", SqlDbType.NVarChar, 300).Value =
                string.IsNullOrWhiteSpace(descripcion) ? DBNull.Value : descripcion.Trim();
            return comando.ExecuteNonQuery() > 0;
        }
    }
}
