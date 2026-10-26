using System.Data;
using System.Net.Mail;
using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;

namespace Sistema_ERP
{
    public static class DirectorioDatos
    {
        // Los identificadores SQL provienen exclusivamente de esta lista fija.
        private static (string Tabla, string Id, string Nombre, string Identificador) Entidad(TipoDirectorio tipo) => tipo switch
        {
            TipoDirectorio.Clientes => ("Cliente", "id_cliente", "nombre", "documento_fiscal"),
            TipoDirectorio.Proveedores => ("Proveedor", "id_proveedor", "nombre_empresa", "rtn"),
            TipoDirectorio.Empleados => ("Empleado", "id_empleado", "nombre", "codigo"),
            _ => throw new ArgumentOutOfRangeException(nameof(tipo))
        };

        public static DataTable Listar(TipoDirectorio tipo, bool incluirInactivos = false)
        {
            var e = Entidad(tipo);
            string extras = tipo switch
            {
                TipoDirectorio.Clientes => "p.direccion AS Direccion, CAST(NULL AS nvarchar(150)) AS Contacto, CAST(NULL AS int) AS IdTienda, CAST(NULL AS nvarchar(100)) AS Tienda",
                TipoDirectorio.Proveedores => "p.direccion AS Direccion, p.contacto AS Contacto, CAST(NULL AS int) AS IdTienda, CAST(NULL AS nvarchar(100)) AS Tienda",
                _ => "CAST(NULL AS nvarchar(300)) AS Direccion, CAST(NULL AS nvarchar(150)) AS Contacto, p.id_tienda AS IdTienda, t.nombre AS Tienda"
            };
            string join = tipo == TipoDirectorio.Empleados ? "JOIN dbo.Tienda t ON t.id_tienda=p.id_tienda" : "";
            string sql = $@"SELECT p.{e.Id} AS Id, p.{e.Nombre} AS Nombre, p.telefono AS Telefono, p.correo AS Correo,
                p.{e.Identificador} AS Identificador, {extras}, p.activo, p.motivo_desactivacion
                FROM dbo.{e.Tabla} p {join} WHERE p.activo=1 OR @incluir=1 ORDER BY p.{e.Nombre}";
            using var conexion = Conexion.AbrirConexion();
            using var adaptador = new SqlDataAdapter(sql, conexion);
            adaptador.SelectCommand.Parameters.Add("@incluir", SqlDbType.Bit).Value = incluirInactivos;
            var tabla = new DataTable(); adaptador.Fill(tabla); return tabla;
        }

        public static DataTable Tiendas()
        {
            using var conexion = Conexion.AbrirConexion();
            using var adaptador = new SqlDataAdapter("SELECT id_tienda, nombre FROM dbo.Tienda WHERE activo=1 ORDER BY nombre", conexion);
            var tabla = new DataTable(); adaptador.Fill(tabla); return tabla;
        }

        public static void Agregar(TipoDirectorio tipo, string nombre, string telefono, string correo,
            string identificador, string direccion, string contacto, int? tienda)
            => Guardar(tipo, null, nombre, telefono, correo, identificador, direccion, contacto, tienda);

        public static bool Editar(TipoDirectorio tipo, int id, string nombre, string telefono, string correo,
            string identificador, string direccion, string contacto, int? tienda)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            return Guardar(tipo, id, nombre, telefono, correo, identificador, direccion, contacto, tienda);
        }

        private static bool Guardar(TipoDirectorio tipo, int? id, string nombre, string telefono, string correo,
            string identificador, string direccion, string contacto, int? tienda)
        {
            var e = Entidad(tipo);
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Introduce el nombre.");
            if (nombre.Trim().Length > 150 || telefono.Trim().Length > 30 || correo.Trim().Length > 254 ||
                identificador.Trim().Length > 30 || direccion.Trim().Length > 300 || contacto.Trim().Length > 150)
                throw new ArgumentException("Uno de los campos supera la longitud permitida.");
            if (!string.IsNullOrWhiteSpace(correo) &&
                (!MailAddress.TryCreate(correo.Trim(), out var email) || email.Address != correo.Trim()))
                throw new ArgumentException("Introduce un correo electronico valido.");
            if (tipo == TipoDirectorio.Proveedores)
            {
                var proveedor = new Proveedor
                {
                    IdProveedor = id ?? 0,
                    NombreEmpresa = nombre,
                    Telefono = telefono,
                    Correo = correo,
                    Rtn = identificador,
                    Direccion = direccion,
                    Contacto = contacto
                };
                proveedor.Validar();
                if (id.HasValue) return ProveedorDatos.Editar(proveedor);
                ProveedorDatos.Agregar(proveedor); return true;
            }
            if (tipo == TipoDirectorio.Empleados && (string.IsNullOrWhiteSpace(identificador) || tienda is null || tienda <= 0))
                throw new ArgumentException("Introduce el codigo y selecciona la tienda del empleado.");
            if (tipo == TipoDirectorio.Clientes && identificador.Any(c => c > 127))
                throw new ArgumentException("El documento fiscal debe contener caracteres ASCII.");

            string extraColumna = tipo == TipoDirectorio.Empleados ? "id_tienda" : "direccion";
            string extraParametro = tipo == TipoDirectorio.Empleados ? "@tienda" : "@direccion";
            string sql = id.HasValue
                ? $@"UPDATE dbo.{e.Tabla} SET {e.Nombre}=@nombre, telefono=@telefono, correo=@correo,
                    {e.Identificador}=@identificador, {extraColumna}={extraParametro}, actualizado_en=SYSUTCDATETIME()
                    WHERE {e.Id}=@id AND activo=1; SELECT CONVERT(int, @@ROWCOUNT);"
                : $@"INSERT INTO dbo.{e.Tabla}({e.Nombre},telefono,correo,{e.Identificador},{extraColumna})
                    VALUES(@nombre,@telefono,@correo,@identificador,{extraParametro}); SELECT CONVERT(int, @@ROWCOUNT);";
            // Permite conservar la tienda anterior aunque haya sido desactivada; las nuevas asignaciones requieren una activa.
            if (tipo == TipoDirectorio.Empleados)
                sql = @"IF NOT EXISTS(SELECT 1 FROM dbo.Tienda WHERE id_tienda=@tienda AND activo=1)
                    AND NOT EXISTS(SELECT 1 FROM dbo.Empleado WHERE id_empleado=@id AND id_tienda=@tienda)
                    THROW 50001, 'Selecciona una tienda activa para el empleado.', 1; " + sql;
            using var conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            foreach (var (campo, largo, valor) in new[] { ("@nombre",150,nombre), ("@telefono",30,telefono),
                ("@correo",254,correo), ("@identificador",30,identificador), ("@direccion",300,direccion) })
                comando.Parameters.Add(campo, campo == "@identificador" && tipo == TipoDirectorio.Clientes ? SqlDbType.VarChar : SqlDbType.NVarChar, largo).Value =
                    string.IsNullOrWhiteSpace(valor) ? DBNull.Value : valor.Trim();
            comando.Parameters.Add("@tienda", SqlDbType.Int).Value = (object?)tienda ?? DBNull.Value;
            comando.Parameters.Add("@id", SqlDbType.Int).Value = (object?)id ?? DBNull.Value;
            return (int)comando.ExecuteScalar() > 0;
        }

        public static bool Desactivar(TipoDirectorio tipo, int id, string motivo)
            => CambiarEstado(tipo, id, false, motivo);
        public static bool Reactivar(TipoDirectorio tipo, int id) => CambiarEstado(tipo, id, true, "");

        private static bool CambiarEstado(TipoDirectorio tipo, int id, bool activar, string motivo)
        {
            var e = Entidad(tipo);
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (!activar && (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length > 300))
                throw new ArgumentException("Introduce un motivo de entre 1 y 300 caracteres.");
            string valores = activar
                ? "desactivado_en=NULL, desactivado_por=NULL, motivo_desactivacion=NULL"
                : "desactivado_en=SYSUTCDATETIME(), motivo_desactivacion=@motivo";
            using var conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand($@"UPDATE dbo.{e.Tabla} SET {valores}, actualizado_en=SYSUTCDATETIME()
                WHERE {e.Id}=@id AND activo=@anterior; SELECT CONVERT(int, @@ROWCOUNT);", conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
            comando.Parameters.Add("@anterior", SqlDbType.Bit).Value = !activar;
            comando.Parameters.Add("@motivo", SqlDbType.NVarChar, 300).Value = motivo.Trim();
            return (int)comando.ExecuteScalar() > 0;
        }
    }
}
