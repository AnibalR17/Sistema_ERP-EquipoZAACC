using System.Data;
using Microsoft.Data.SqlClient;

namespace Sistema_ERP.Clases
{
    public static class Ingresar
    {
        public static int Proveedor(Proveedor proveedor)
        {
            proveedor.Validar();
            const string sql = @"DECLARE @insertados TABLE (id_proveedor int);
                                 INSERT INTO dbo.Proveedor
                                     (nombre_empresa, rtn, contacto, telefono, correo, direccion)
                                 OUTPUT INSERTED.id_proveedor INTO @insertados
                                 VALUES (@nombre, @rtn, @contacto, @telefono, @correo, @direccion);
                                 SELECT id_proveedor FROM @insertados;";
            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            ProveedorDatos.AgregarParametros(comando, proveedor);
            return (int)comando.ExecuteScalar();
        }

        public static int Categoria(string nombre, string? descripcion = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
            if (nombre.Trim().Length > 100) throw new ArgumentException("El nombre admite hasta 100 caracteres.", nameof(nombre));
            if (descripcion?.Trim().Length > 300) throw new ArgumentException("La descripción admite hasta 300 caracteres.", nameof(descripcion));

            // OUTPUT debe usar INTO cuando la tabla tiene triggers habilitados.
            const string sql = @"DECLARE @categoriasInsertadas TABLE (id_categoria int);
                                 INSERT INTO dbo.Categoria (nombre, descripcion)
                                 OUTPUT INSERTED.id_categoria INTO @categoriasInsertadas
                                 VALUES (@nombre, @descripcion);
                                 SELECT id_categoria FROM @categoriasInsertadas;";

            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = nombre.Trim();
            comando.Parameters.Add("@descripcion", SqlDbType.NVarChar, 300).Value =
                string.IsNullOrWhiteSpace(descripcion) ? DBNull.Value : descripcion.Trim();
            return (int)comando.ExecuteScalar();
        }
    }
}
