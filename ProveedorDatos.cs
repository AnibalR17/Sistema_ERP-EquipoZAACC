using System.Data;
using Microsoft.Data.SqlClient;

namespace Sistema_ERP
{
    public static class ProveedorDatos
    {
        public static DataTable ListarParaCombo()
        {
            const string sql = "SELECT id_proveedor, nombre_empresa FROM Proveedor ORDER BY nombre_empresa";

            using SqlConnection conexion = ConexionDB.AbrirConexion();
            using var adaptador = new SqlDataAdapter(sql, conexion);
            var tabla = new DataTable();
            adaptador.Fill(tabla);
            return tabla;
        }
    }
}