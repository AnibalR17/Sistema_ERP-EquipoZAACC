using System.Data;
using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;

namespace Sistema_ERP
{
    public enum TipoSeguridad { Usuarios, Roles, Permisos }

    public static class SeguridadDatos
    {
        private static (string Tabla, string Id) Entidad(TipoSeguridad tipo) => tipo switch
        {
            TipoSeguridad.Usuarios => ("Usuario", "id_usuario"),
            TipoSeguridad.Roles => ("Rol", "id_rol"),
            TipoSeguridad.Permisos => ("Permiso", "id_permiso"),
            _ => throw new ArgumentOutOfRangeException(nameof(tipo))
        };

        public static DataTable Listar(TipoSeguridad tipo, bool inactivos)
        {
            var (tabla, id) = Entidad(tipo);
            string campos = tipo switch
            {
                TipoSeguridad.Usuarios => "nombre_usuario AS Primero, CONVERT(nvarchar(20),id_empleado) AS Segundo, id_empleado, CAST('' AS nvarchar(300)) AS Descripcion",
                TipoSeguridad.Roles => "codigo AS Primero, nombre AS Segundo, descripcion AS Descripcion",
                _ => "modulo AS Primero, accion AS Segundo, descripcion AS Descripcion"
            };
            using var conexion = Conexion.AbrirConexion();
            using var adaptador = new SqlDataAdapter($"SELECT {id} AS Id, {campos}, activo, motivo_desactivacion, version_fila FROM dbo.{tabla} WHERE activo=1 OR @inactivos=1 ORDER BY Primero", conexion);
            adaptador.SelectCommand.Parameters.Add("@inactivos", SqlDbType.Bit).Value = inactivos;
            var resultado = new DataTable();
            adaptador.Fill(resultado);
            return resultado;
        }

        public static DataTable Empleados()
        {
            using var conexion = Conexion.AbrirConexion();
            using var adaptador = new SqlDataAdapter("SELECT id_empleado AS Id, nombre AS Nombre, activo FROM dbo.Empleado ORDER BY nombre", conexion);
            var tabla = new DataTable();
            adaptador.Fill(tabla);
            return tabla;
        }

        public static DataTable Asignaciones(TipoSeguridad tipo, int? id)
        {
            if (tipo == TipoSeguridad.Permisos) return new DataTable();
            bool usuario = tipo == TipoSeguridad.Usuarios;
            string entidad = usuario ? "Rol" : "Permiso";
            string clave = usuario ? "id_rol" : "id_permiso";
            string relacion = usuario ? "UsuarioRol" : "RolPermiso";
            string propietario = usuario ? "id_usuario" : "id_rol";
            string nombre = usuario ? "e.nombre" : "e.codigo";
            using var conexion = Conexion.AbrirConexion();
            using var adaptador = new SqlDataAdapter($"""
                SELECT e.{clave} AS Id, {nombre} + CASE WHEN e.activo=0 THEN N' (inactivo)' ELSE N'' END AS Nombre,
                       e.activo, CONVERT(bit,CASE WHEN r.activo=1 THEN 1 ELSE 0 END) AS Asignado
                FROM dbo.{entidad} e LEFT JOIN dbo.{relacion} r ON r.{clave}=e.{clave} AND r.{propietario}=@id
                WHERE e.activo=1 OR r.activo=1 ORDER BY Nombre
                """, conexion);
            adaptador.SelectCommand.Parameters.Add("@id", SqlDbType.Int).Value = (object?)id ?? DBNull.Value;
            var tabla = new DataTable();
            adaptador.Fill(tabla);
            return tabla;
        }

        public static void Guardar(TipoSeguridad tipo, int? id, byte[]? version, string primero,
            string segundo, string descripcion, int? empleado, string contrasena, int[] asignaciones)
        {
            var (tabla, clave) = Entidad(tipo);
            primero = primero.Trim(); segundo = segundo.Trim(); descripcion = descripcion.Trim();
            ValidarTexto(primero, tipo == TipoSeguridad.Roles ? 30 : 50, "El primer campo", true, tipo != TipoSeguridad.Usuarios);
            if (tipo != TipoSeguridad.Usuarios)
            {
                ValidarTexto(segundo, tipo == TipoSeguridad.Roles ? 80 : 30, "El segundo campo", true, tipo == TipoSeguridad.Permisos);
                ValidarTexto(descripcion, 300, "La descripción", tipo == TipoSeguridad.Permisos);
            }
            string? hash = null;
            if (tipo == TipoSeguridad.Usuarios)
            {
                if (empleado is null or <= 0) throw new ArgumentException("Selecciona un empleado.");
                if (!id.HasValue || contrasena.Length > 0)
                {
                    if (contrasena.Length < 12 || contrasena.Length > 128)
                        throw new ArgumentException("La contraseña debe tener entre 12 y 128 caracteres.");
                    hash = ClaveSegura.Crear(contrasena);
                }
            }
            using var conexion = Conexion.AbrirConexion();
            using var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
            if (tipo == TipoSeguridad.Usuarios)
            {
                using var comprobar = new SqlCommand("""
                    SELECT COUNT(*) FROM dbo.Empleado e WITH (UPDLOCK,HOLDLOCK) WHERE e.id_empleado=@empleado
                    AND (e.activo=1 OR EXISTS(SELECT 1 FROM dbo.Usuario u WHERE u.id_usuario=@id AND u.id_empleado=e.id_empleado))
                    """, conexion, transaccion);
                comprobar.Parameters.Add("@empleado", SqlDbType.Int).Value = empleado!.Value;
                comprobar.Parameters.Add("@id", SqlDbType.Int).Value = (object?)id ?? DBNull.Value;
                if (Convert.ToInt32(comprobar.ExecuteScalar()) != 1) throw new ArgumentException("Selecciona un empleado activo para una nueva asignación.");
            }
            string columnas = tipo switch
            {
                TipoSeguridad.Usuarios => "nombre_usuario,id_empleado,contrasena_hash",
                TipoSeguridad.Roles => "codigo,nombre,descripcion",
                _ => "modulo,accion,descripcion"
            };
            string valores = tipo == TipoSeguridad.Usuarios ? "@primero,@empleado,@hash" : "@primero,@segundo,@descripcion";
            string cambios = tipo switch
            {
                TipoSeguridad.Usuarios => """
                    nombre_usuario=@primero,id_empleado=@empleado,
                    contrasena_hash=COALESCE(@hash,contrasena_hash),
                    debe_cambiar_contrasena=CASE WHEN @hash IS NULL THEN debe_cambiar_contrasena ELSE 1 END,
                    contrasena_actualizada_en=CASE WHEN @hash IS NULL THEN contrasena_actualizada_en ELSE SYSUTCDATETIME() END
                    """,
                TipoSeguridad.Roles => "codigo=@primero,nombre=@segundo,descripcion=@descripcion",
                _ => "modulo=@primero,accion=@segundo,descripcion=@descripcion"
            };
            using var comando = new SqlCommand(id.HasValue
                ? $"UPDATE dbo.{tabla} SET {cambios},actualizado_en=SYSUTCDATETIME() WHERE {clave}=@id AND activo=1 AND version_fila=@version; SELECT CONVERT(int,@@ROWCOUNT);"
                : $"INSERT dbo.{tabla}({columnas}) VALUES({valores}); SELECT CONVERT(int,SCOPE_IDENTITY());", conexion, transaccion);
            comando.Parameters.Add("@primero", tipo == TipoSeguridad.Usuarios ? SqlDbType.NVarChar : SqlDbType.VarChar, tipo == TipoSeguridad.Roles ? 30 : 50).Value = primero;
            comando.Parameters.Add("@segundo", tipo == TipoSeguridad.Permisos ? SqlDbType.VarChar : SqlDbType.NVarChar, tipo == TipoSeguridad.Roles ? 80 : 30).Value = segundo;
            comando.Parameters.Add("@descripcion", SqlDbType.NVarChar, 300).Value = descripcion;
            comando.Parameters.Add("@empleado", SqlDbType.Int).Value = (object?)empleado ?? DBNull.Value;
            comando.Parameters.Add("@hash", SqlDbType.NVarChar, 512).Value = (object?)hash ?? DBNull.Value;
            comando.Parameters.Add("@id", SqlDbType.Int).Value = (object?)id ?? DBNull.Value;
            comando.Parameters.Add("@version", SqlDbType.Timestamp, 8).Value = (object?)version ?? DBNull.Value;
            int resultado = Convert.ToInt32(comando.ExecuteScalar());
            if (id.HasValue && resultado != 1) throw new InvalidOperationException("El registro cambió o fue desactivado. Recarga la pestaña antes de continuar.");
            int registro = id ?? resultado;
            if (tipo != TipoSeguridad.Permisos) GuardarAsignaciones(conexion, transaccion, tipo, registro, asignaciones);
            transaccion.Commit();
        }

        private static void GuardarAsignaciones(SqlConnection conexion, SqlTransaction transaccion, TipoSeguridad tipo, int id, int[] seleccion)
        {
            bool usuario = tipo == TipoSeguridad.Usuarios;
            string tabla = usuario ? "UsuarioRol" : "RolPermiso";
            string propietario = usuario ? "id_usuario" : "id_rol";
            string destino = usuario ? "id_rol" : "id_permiso";
            string entidad = usuario ? "Rol" : "Permiso";
            var seleccionados = seleccion.ToHashSet();
            var existentes = new Dictionary<int, bool>();
            using (var leer = new SqlCommand($"SELECT {destino},activo FROM dbo.{tabla} WITH (UPDLOCK,HOLDLOCK) WHERE {propietario}=@id", conexion, transaccion))
            {
                leer.Parameters.Add("@id", SqlDbType.Int).Value = id;
                using var lector = leer.ExecuteReader();
                while (lector.Read()) existentes.Add(lector.GetInt32(0), lector.GetBoolean(1));
            }
            foreach (int relacionado in existentes.Keys.Union(seleccionados))
            {
                bool activar = seleccionados.Contains(relacionado);
                if (existentes.TryGetValue(relacionado, out bool previo) && previo == activar) continue;
                using var comando = new SqlCommand($"""
                    IF @activo=1 AND NOT EXISTS(SELECT 1 FROM dbo.{entidad} WITH (UPDLOCK,HOLDLOCK) WHERE {destino}=@destino AND activo=1)
                        THROW 50001, 'No se pueden agregar asignaciones a registros inactivos.', 1;
                    IF EXISTS(SELECT 1 FROM dbo.{tabla} WHERE {propietario}=@id AND {destino}=@destino)
                        UPDATE dbo.{tabla} SET activo=@activo,asignado_en=SYSUTCDATETIME(),asignado_por=NULL WHERE {propietario}=@id AND {destino}=@destino;
                    ELSE
                        INSERT dbo.{tabla}({propietario},{destino},activo) VALUES(@id,@destino,@activo);
                    """, conexion, transaccion);
                comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
                comando.Parameters.Add("@destino", SqlDbType.Int).Value = relacionado;
                comando.Parameters.Add("@activo", SqlDbType.Bit).Value = activar;
                comando.ExecuteNonQuery();
            }
        }

        public static void CambiarEstado(TipoSeguridad tipo, int id, byte[] version, bool activar, string motivo)
        {
            if (!activar) ValidarTexto(motivo.Trim(), 300, "El motivo", true);
            var (tabla, clave) = Entidad(tipo);
            using var conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand($"""
                UPDATE dbo.{tabla} SET desactivado_en=CASE WHEN @activar=1 THEN NULL ELSE SYSUTCDATETIME() END,
                desactivado_por=NULL,motivo_desactivacion=CASE WHEN @activar=1 THEN NULL ELSE @motivo END,
                actualizado_en=SYSUTCDATETIME() WHERE {clave}=@id AND version_fila=@version AND activo<>@activar;
                SELECT CONVERT(int,@@ROWCOUNT);
                """, conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
            comando.Parameters.Add("@version", SqlDbType.Timestamp, 8).Value = version;
            comando.Parameters.Add("@activar", SqlDbType.Bit).Value = activar;
            comando.Parameters.Add("@motivo", SqlDbType.NVarChar, 300).Value = motivo.Trim();
            if (Convert.ToInt32(comando.ExecuteScalar()) != 1)
                throw new InvalidOperationException("El registro cambió. Recarga la pestaña antes de continuar.");
        }

        private static void ValidarTexto(string texto, int maximo, string campo, bool obligatorio, bool ascii = false)
        {
            if (obligatorio && string.IsNullOrWhiteSpace(texto)) throw new ArgumentException(campo + " es obligatorio.");
            if (texto.Length > maximo) throw new ArgumentException($"{campo} admite hasta {maximo} caracteres.");
            if (ascii && texto.Any(c => c > 127 || char.IsControl(c))) throw new ArgumentException(campo + " debe usar caracteres ASCII imprimibles.");
        }
    }
}
