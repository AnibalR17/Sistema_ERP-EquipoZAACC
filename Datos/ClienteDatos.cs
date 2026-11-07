using System.Data;
using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;

namespace Sistema_ERP.Datos
{

    public static class ClienteDatos
    {
        private const int UniqueIndex = 2601;
        private const int UniqueConstraint = 2627;


        public static DataTable Listar(bool incluirInactivos)
        {
            const string sql = @"SELECT id_cliente, nombre, telefono, correo,
                                        CAST(CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END AS bit) AS activo,
                                        desactivado_en, motivo_desactivacion, actualizado_en
                                 FROM Cliente
                                 WHERE (@incluir = 1 OR desactivado_en IS NULL)
                                 ORDER BY nombre";

            using SqlConnection conexion = ConexionDB.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@incluir", SqlDbType.Bit).Value = incluirInactivos;
            using var adaptador = new SqlDataAdapter(comando);
            var tabla = new DataTable();
            adaptador.Fill(tabla);
            return tabla;
        }


        public static int Agregar(Cliente cliente)
        {
            cliente.Validar();
            const string sql = @"INSERT INTO Cliente (nombre, telefono, correo)
                                 OUTPUT INSERTED.id_cliente
                                 VALUES (@nombre, @telefono, @correo)";
            try
            {
                using SqlConnection conexion = ConexionDB.AbrirConexion();
                using var comando = new SqlCommand(sql, conexion);
                AgregarParametros(comando, cliente);
                return (int)comando.ExecuteScalar();
            }
            catch (SqlException ex) when (EsDuplicado(ex))
            {
                throw new InvalidOperationException("Ya existe un cliente con ese correo.", ex);
            }
        }

        public static bool Editar(Cliente cliente)
        {
            cliente.Validar();
            const string sql = @"UPDATE Cliente
                                 SET nombre = @nombre, telefono = @telefono, correo = @correo,
                                     actualizado_en = SYSDATETIME()
                                 WHERE id_cliente = @id";
            try
            {
                using SqlConnection conexion = ConexionDB.AbrirConexion();
                using var comando = new SqlCommand(sql, conexion);
                AgregarParametros(comando, cliente);
                comando.Parameters.Add("@id", SqlDbType.Int).Value = cliente.IdCliente;
                return comando.ExecuteNonQuery() > 0;
            }
            catch (SqlException ex) when (EsDuplicado(ex))
            {
                throw new InvalidOperationException("Ya existe otro cliente con ese correo.", ex);
            }
        }


        public static bool Desactivar(int idCliente, string motivo, int? desactivadoPor = null)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("El motivo de desactivacion es obligatorio.", nameof(motivo));

            const string sql = @"UPDATE Cliente
                                 SET desactivado_en = SYSDATETIME(), desactivado_por = @por,
                                     motivo_desactivacion = @motivo, actualizado_en = SYSDATETIME()
                                 WHERE id_cliente = @id AND desactivado_en IS NULL";

            using SqlConnection conexion = ConexionDB.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = idCliente;
            comando.Parameters.Add("@motivo", SqlDbType.NVarChar, 250).Value = motivo.Trim();
            comando.Parameters.Add("@por", SqlDbType.Int).Value = (object?)desactivadoPor ?? DBNull.Value;
            return comando.ExecuteNonQuery() > 0;
        }

        public static bool Reactivar(int idCliente)
        {
            const string sql = @"UPDATE Cliente
                                 SET desactivado_en = NULL, desactivado_por = NULL,
                                     motivo_desactivacion = NULL, actualizado_en = SYSDATETIME()
                                 WHERE id_cliente = @id AND desactivado_en IS NOT NULL";

            using SqlConnection conexion = ConexionDB.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = idCliente;
            return comando.ExecuteNonQuery() > 0;
        }

        private static void AgregarParametros(SqlCommand comando, Cliente c)
        {
            comando.Parameters.Add("@nombre", SqlDbType.NVarChar, 150).Value = c.Nombre;
            comando.Parameters.Add("@telefono", SqlDbType.NVarChar, 30).Value = (object?)c.Telefono ?? DBNull.Value;
            comando.Parameters.Add("@correo", SqlDbType.NVarChar, 150).Value = (object?)c.Correo ?? DBNull.Value;
        }

        private static bool EsDuplicado(SqlException ex) => ex.Number is UniqueIndex or UniqueConstraint;
    }
}