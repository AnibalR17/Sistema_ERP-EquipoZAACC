using System.Data;
using Microsoft.Data.SqlClient;

namespace Sistema_ERP.Clases
{
    public static class Borrar
    {
        public static bool Producto(int idProducto, string motivo)
        {
            if (idProducto <= 0) throw new ArgumentOutOfRangeException(nameof(idProducto));
            ArgumentException.ThrowIfNullOrWhiteSpace(motivo);
            if (motivo.Trim().Length > 300)
                throw new ArgumentException("El motivo admite hasta 300 caracteres.", nameof(motivo));
            const string sql = @"UPDATE dbo.Producto
                                 SET desactivado_en = SYSUTCDATETIME(), motivo_desactivacion = @motivo,
                                     actualizado_en = SYSUTCDATETIME()
                                 WHERE id_producto = @id AND activo = 1;
                                 SELECT CONVERT(int, @@ROWCOUNT);";
            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = idProducto;
            comando.Parameters.Add("@motivo", SqlDbType.NVarChar, 300).Value = motivo.Trim();
            return (int)comando.ExecuteScalar() > 0;
        }

        public static bool Proveedor(int idProveedor, string motivo)
        {
            if (idProveedor <= 0) throw new ArgumentOutOfRangeException(nameof(idProveedor));
            ArgumentException.ThrowIfNullOrWhiteSpace(motivo);
            if (motivo.Trim().Length > 300)
                throw new ArgumentException("El motivo admite hasta 300 caracteres.", nameof(motivo));
            const string sql = @"UPDATE dbo.Proveedor
                                 SET desactivado_en = SYSUTCDATETIME(), motivo_desactivacion = @motivo,
                                     actualizado_en = SYSUTCDATETIME()
                                 WHERE id_proveedor = @id AND activo = 1;
                                 SELECT CONVERT(int, @@ROWCOUNT);";
            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = idProveedor;
            comando.Parameters.Add("@motivo", SqlDbType.NVarChar, 300).Value = motivo.Trim();
            return (int)comando.ExecuteScalar() > 0;
        }

        public static bool Categoria(int id_categoria, string motivo)
        {
            if (id_categoria <= 0) throw new ArgumentOutOfRangeException(nameof(id_categoria));
            ArgumentException.ThrowIfNullOrWhiteSpace(motivo);
            if (motivo.Trim().Length > 300)
                throw new ArgumentException("El motivo admite hasta 300 caracteres.", nameof(motivo));

            const string sql = @"UPDATE dbo.Categoria
                                 SET desactivado_en = SYSUTCDATETIME(),
                                     motivo_desactivacion = @motivo,
                                     actualizado_en = SYSUTCDATETIME()
                                 WHERE id_categoria = @id AND activo = 1;
                                 SELECT CONVERT(int, @@ROWCOUNT);";
            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = id_categoria;
            comando.Parameters.Add("@motivo", SqlDbType.NVarChar, 300).Value = motivo.Trim();
            return (int)comando.ExecuteScalar() > 0;
        }
    }
}
