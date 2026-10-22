using System.Data;
using Microsoft.Data.SqlClient;

namespace Sistema_ERP
{
    public static class ProveedorDatos
    {
        public static DataTable ListarParaCombo()
        {
            const string sql = "SELECT id_proveedor, nombre_empresa FROM Proveedor WHERE activo = 1 ORDER BY nombre_empresa";

            using SqlConnection conexion = ConexionDB.AbrirConexion();
            using var adaptador = new SqlDataAdapter(sql, conexion);
            var tabla = new DataTable();
            adaptador.Fill(tabla);
            return tabla;
        }
        public static bool Desactivar(int idProveedor)
        {
            const string sql = "UPDATE Proveedor SET activo = 0 WHERE id_proveedor = @id AND activo = 1";
            using SqlConnection conexion = ConexionDB.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = idProveedor;
            return comando.ExecuteNonQuery() > 0;
        }

    }
}