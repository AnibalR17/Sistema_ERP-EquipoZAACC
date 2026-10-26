using System.Data;
using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;

namespace Sistema_ERP
{
    public static class TiendaDatos
    {
        public static DataTable Listar(bool incluirInactivas = false)
        {
            using var conexion = Conexion.AbrirConexion();
            using var adaptador = new SqlDataAdapter("SELECT id_tienda, codigo AS Codigo, nombre AS Nombre, direccion AS Direccion, telefono AS Telefono, activo, motivo_desactivacion FROM dbo.Tienda WHERE activo=1 OR @incluirInactivas=1 ORDER BY nombre", conexion);
            adaptador.SelectCommand.Parameters.Add("@incluirInactivas", SqlDbType.Bit).Value = incluirInactivas;
            var tabla = new DataTable(); adaptador.Fill(tabla); return tabla;
        }

        public static bool Desactivar(int idTienda, string motivo) => Borrar.Tienda(idTienda, motivo);

        public static bool Reactivar(int idTienda)
        {
            if (idTienda <= 0) throw new ArgumentOutOfRangeException(nameof(idTienda));
            using var conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(@"UPDATE dbo.Tienda SET desactivado_en=NULL, desactivado_por=NULL,
                motivo_desactivacion=NULL, actualizado_en=SYSUTCDATETIME() WHERE id_tienda=@id AND activo=0;
                SELECT CONVERT(int, @@ROWCOUNT);", conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = idTienda;
            return (int)comando.ExecuteScalar() > 0;
        }

        public static void Agregar(string codigo, string nombre, string direccion, string telefono)
        {
            Validar(codigo, nombre, direccion, telefono);
            using var conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand("INSERT INTO dbo.Tienda(codigo,nombre,direccion,telefono) VALUES(@codigo,@nombre,@direccion,@telefono)", conexion);
            Parametros(comando, codigo, nombre, direccion, telefono);
            comando.ExecuteNonQuery();
        }

        public static bool Editar(int idTienda, string codigo, string nombre, string direccion, string telefono)
        {
            if (idTienda <= 0) throw new ArgumentOutOfRangeException(nameof(idTienda));
            Validar(codigo, nombre, direccion, telefono);
            using var conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(@"UPDATE dbo.Tienda SET codigo=@codigo, nombre=@nombre,
                direccion=@direccion, telefono=@telefono, actualizado_en=SYSUTCDATETIME()
                WHERE id_tienda=@id AND activo=1; SELECT CONVERT(int, @@ROWCOUNT);", conexion);
            Parametros(comando, codigo, nombre, direccion, telefono);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = idTienda;
            return (int)comando.ExecuteScalar() > 0;
        }

        private static void Validar(string codigo, string nombre, string direccion, string telefono)
        {
            if (string.IsNullOrWhiteSpace(codigo)) throw new ArgumentException("Introduce el codigo de la tienda.");
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Introduce el nombre de la tienda.");
            if (codigo.Trim().Length > 20 || nombre.Trim().Length > 100 || direccion.Trim().Length > 300 || telefono.Trim().Length > 30)
                throw new ArgumentException("Uno de los campos supera la longitud permitida.");
            if (codigo.Any(c => c > 127)) throw new ArgumentException("El codigo debe contener caracteres ASCII.");
        }

        private static void Parametros(SqlCommand comando, string codigo, string nombre, string direccion, string telefono)
        {
            comando.Parameters.Add("@codigo", SqlDbType.VarChar, 20).Value = codigo.Trim();
            comando.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = nombre.Trim();
            comando.Parameters.Add("@direccion", SqlDbType.NVarChar, 300).Value = string.IsNullOrWhiteSpace(direccion) ? DBNull.Value : direccion.Trim();
            comando.Parameters.Add("@telefono", SqlDbType.NVarChar, 30).Value = string.IsNullOrWhiteSpace(telefono) ? DBNull.Value : telefono.Trim();
        }
    }
}
