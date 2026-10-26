using Sistema_ERP.Clases;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Sistema_ERP
{
    public static class ProveedorDatos
    {
        public static DataTable ListarParaCombo()
        {
            const string sql = "SELECT id_proveedor, nombre_empresa FROM dbo.Proveedor WHERE activo = 1 ORDER BY nombre_empresa";

            using SqlConnection conexion = Conexion.AbrirConexion();
            using var adaptador = new SqlDataAdapter(sql, conexion);
            var tabla = new DataTable();
            adaptador.Fill(tabla);
            return tabla;
        }
        public static DataTable Listar(bool incluirInactivos = false)
        {
            const string sql = @"SELECT id_proveedor, nombre_empresa, rtn, contacto, telefono, correo, direccion,
                                        activo, motivo_desactivacion
                                 FROM dbo.Proveedor WHERE activo = 1 OR @incluirInactivos = 1 ORDER BY nombre_empresa";
            using SqlConnection conexion = Conexion.AbrirConexion();
            using var adaptador = new SqlDataAdapter(sql, conexion);
            adaptador.SelectCommand.Parameters.Add("@incluirInactivos", SqlDbType.Bit).Value = incluirInactivos;
            var tabla = new DataTable();
            adaptador.Fill(tabla);
            return tabla;
        }

        public static bool ExisteRtn(string rtn, int? excluirId = null)
        {
            if (string.IsNullOrWhiteSpace(rtn)) return false;
            const string sql = @"SELECT COUNT(*) FROM dbo.Proveedor
                                 WHERE rtn = @rtn AND (@id IS NULL OR id_proveedor <> @id)";
            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@rtn", SqlDbType.VarChar, 30).Value = rtn.Trim();
            comando.Parameters.Add("@id", SqlDbType.Int).Value = (object?)excluirId ?? DBNull.Value;
            return (int)comando.ExecuteScalar() > 0;
        }

        public static int Agregar(Proveedor proveedor) => Ingresar.Proveedor(proveedor);
        public static bool Editar(Proveedor proveedor) => Modificar.Proveedor(proveedor);
        public static bool Desactivar(int idProveedor, string motivo) => Borrar.Proveedor(idProveedor, motivo);
        public static bool Reactivar(int idProveedor) => Modificar.ReactivarProveedor(idProveedor);

        internal static void AgregarParametros(SqlCommand comando, Proveedor proveedor)
        {
            comando.Parameters.Add("@nombre", SqlDbType.NVarChar, 150).Value = proveedor.NombreEmpresa.Trim();
            foreach (var (parametro, tipo, limite, valor) in new[]
            {
                ("@rtn", SqlDbType.VarChar, 30, proveedor.Rtn),
                ("@contacto", SqlDbType.NVarChar, 150, proveedor.Contacto),
                ("@telefono", SqlDbType.NVarChar, 30, proveedor.Telefono),
                ("@correo", SqlDbType.NVarChar, 254, proveedor.Correo),
                ("@direccion", SqlDbType.NVarChar, 300, proveedor.Direccion)
            })
                comando.Parameters.Add(parametro, tipo, limite).Value =
                    string.IsNullOrWhiteSpace(valor) ? DBNull.Value : valor.Trim();
        }
    }
}
