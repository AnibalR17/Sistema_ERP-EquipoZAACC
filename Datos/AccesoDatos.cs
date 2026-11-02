using System.Data;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;

namespace Sistema_ERP
{
    internal sealed record AccesoUsuario(int Id, string Nombre, string Roles, bool CambiarClave, byte[] Version, long? Sesion);
    internal sealed record ResultadoAcceso(AccesoUsuario? Usuario, string? Error)
    {
        internal static ResultadoAcceso Rechazado(string mensaje) => new(null, mensaje);
    }

    internal static class AccesoDatos
    {
        // Evita una respuesta inmediata cuando el usuario no existe.
        private static readonly string hashSimulado = ClaveSegura.Crear(Convert.ToBase64String(RandomNumberGenerator.GetBytes(24)));

        internal static ResultadoAcceso Ingresar(string usuario, string clave)
        {
            usuario = usuario.Trim();
            if (usuario.Length is < 1 or > 50 || clave.Length is < 1 or > 128)
                return ResultadoAcceso.Rechazado("Introduce un usuario y una contraseña válidos.");
            using var conexion = Conexion.AbrirConexion();
            using var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
            CrearAdministradorTemporal(conexion, transaccion);
            using var consultar = new SqlCommand("""
                SELECT u.id_usuario,e.nombre,u.contrasena_hash,u.activo,e.activo AS empleado_activo,
                u.bloqueado_hasta,u.debe_cambiar_contrasena,u.version_fila,u.intentos_fallidos
                FROM dbo.Usuario u WITH (UPDLOCK,HOLDLOCK) JOIN dbo.Empleado e ON e.id_empleado=u.id_empleado
                WHERE u.nombre_usuario_normalizado=UPPER(LTRIM(RTRIM(@usuario)))
                """, conexion, transaccion);
            consultar.Parameters.Add("@usuario", SqlDbType.NVarChar, 50).Value = usuario;
            int? id = null;
            string nombre = "", hash = hashSimulado;
            bool activo = false, cambiar = false;
            byte[] version = [];
            DateTime? bloqueo = null;
            using (var lector = consultar.ExecuteReader())
            {
                if (lector.Read())
                {
                    id = lector.GetInt32(0); nombre = lector.GetString(1); hash = lector.GetString(2);
                    activo = lector.GetBoolean(3) && lector.GetBoolean(4);
                    bloqueo = lector.IsDBNull(5) ? null : lector.GetDateTime(5);
                    cambiar = lector.GetBoolean(6); version = (byte[])lector[7];
                }
            }
            bool correcta = ClaveSegura.Verificar(clave, hash);
            // SQL Server es la referencia de tiempo tanto para bloqueo como para sesiones.
            using var reloj = new SqlCommand("SELECT SYSUTCDATETIME()", conexion, transaccion);
            DateTime ahora = (DateTime)reloj.ExecuteScalar()!;
            string resultado = !id.HasValue || !correcta ? "INVALIDO" : !activo ? "INACTIVO" : bloqueo > ahora ? "BLOQUEADO" : "EXITOSO";
            if (id.HasValue && activo && bloqueo > ahora) resultado = "BLOQUEADO";
            if (id.HasValue && activo && resultado == "INVALIDO")
            {
                using var fallo = new SqlCommand("""
                    UPDATE dbo.Usuario SET intentos_fallidos=CASE WHEN bloqueado_hasta IS NOT NULL AND bloqueado_hasta<=SYSUTCDATETIME() THEN 1 ELSE intentos_fallidos+1 END,
                    bloqueado_hasta=CASE WHEN bloqueado_hasta IS NOT NULL AND bloqueado_hasta<=SYSUTCDATETIME() THEN NULL
                        WHEN intentos_fallidos>=4 THEN DATEADD(minute,15,SYSUTCDATETIME()) ELSE NULL END
                    WHERE id_usuario=@id
                    """, conexion, transaccion);
                fallo.Parameters.Add("@id", SqlDbType.Int).Value = id.Value;
                fallo.ExecuteNonQuery();
            }
            using var evento = new SqlCommand("INSERT dbo.EventoAcceso(id_usuario,nombre_intentado,resultado,equipo) VALUES(@id,@usuario,@resultado,@equipo)", conexion, transaccion);
            evento.Parameters.Add("@id", SqlDbType.Int).Value = (object?)id ?? DBNull.Value;
            evento.Parameters.Add("@usuario", SqlDbType.NVarChar, 50).Value = usuario;
            evento.Parameters.Add("@resultado", SqlDbType.VarChar, 30).Value = resultado;
            evento.Parameters.Add("@equipo", SqlDbType.NVarChar, 150).Value = Environment.MachineName;
            evento.ExecuteNonQuery();
            if (resultado != "EXITOSO")
            {
                transaccion.Commit();
                return ResultadoAcceso.Rechazado(resultado == "BLOQUEADO"
                    ? "La cuenta está bloqueada temporalmente. Espera 15 minutos antes de volver a intentarlo."
                    : "Usuario o contraseña incorrectos, o cuenta no disponible. Revisa tus datos e inténtalo de nuevo.");
            }
            long? sesion = null;
            string roles;
            using (var consultarRoles = new SqlCommand("SELECT r.nombre FROM dbo.UsuarioRol ur JOIN dbo.Rol r ON r.id_rol=ur.id_rol WHERE ur.id_usuario=@id AND ur.activo=1 AND r.activo=1 ORDER BY r.nombre", conexion, transaccion))
            {
                consultarRoles.Parameters.Add("@id", SqlDbType.Int).Value = id!.Value;
                using var lector = consultarRoles.ExecuteReader();
                var nombres = new List<string>();
                while (lector.Read()) nombres.Add(lector.GetString(0));
                roles = nombres.Count == 0 ? "Sin roles asignados" : string.Join(", ", nombres);
            }
            if (!cambiar)
            {
                using var crear = new SqlCommand("""
                    UPDATE dbo.Usuario SET intentos_fallidos=0,bloqueado_hasta=NULL,ultimo_acceso_en=SYSUTCDATETIME() WHERE id_usuario=@id;
                    INSERT dbo.SesionUsuario(id_usuario,token_hash,version_seguridad,expira_en,equipo)
                    SELECT id_usuario,@token,version_seguridad,DATEADD(hour,8,SYSUTCDATETIME()),@equipo FROM dbo.Usuario WHERE id_usuario=@id;
                    SELECT CONVERT(bigint,SCOPE_IDENTITY());
                    """, conexion, transaccion);
                crear.Parameters.Add("@id", SqlDbType.Int).Value = id!.Value;
                crear.Parameters.Add("@token", SqlDbType.Binary, 32).Value = SHA256.HashData(RandomNumberGenerator.GetBytes(32));
                crear.Parameters.Add("@equipo", SqlDbType.NVarChar, 150).Value = Environment.MachineName;
                sesion = Convert.ToInt64(crear.ExecuteScalar());
            }
            transaccion.Commit();
            return new ResultadoAcceso(new AccesoUsuario(id!.Value, nombre, roles, cambiar, version, sesion), null);
        }

        private static void CrearAdministradorTemporal(SqlConnection conexion, SqlTransaction transaccion)
        {
            using var comprobar = new SqlCommand("SELECT COUNT(*) FROM dbo.Usuario WITH (UPDLOCK,HOLDLOCK)", conexion, transaccion);
            if (Convert.ToInt32(comprobar.ExecuteScalar()) != 0) return;

            using var crear = new SqlCommand("""
                DECLARE @tienda int, @empleado int, @rol int, @usuario int;
                SELECT @rol=id_rol FROM dbo.Rol WITH (UPDLOCK,HOLDLOCK) WHERE codigo='ADMINISTRADOR' AND activo=1;
                IF @rol IS NULL
                BEGIN
                    IF EXISTS(SELECT 1 FROM dbo.Rol WHERE codigo='ADMINISTRADOR')
                        THROW 50001, 'El rol ADMINISTRADOR esta inactivo. Reactivelo mediante administracion de la base.', 1;
                    INSERT dbo.Rol(codigo,nombre,descripcion) VALUES('ADMINISTRADOR',N'Administrador',N'Administracion del sistema');
                    SET @rol=CONVERT(int,SCOPE_IDENTITY());
                END;
                SELECT @empleado=id_empleado FROM dbo.Empleado WITH (UPDLOCK,HOLDLOCK) WHERE codigo='ADMIN-INICIAL' AND activo=1;
                IF @empleado IS NULL
                BEGIN
                    IF EXISTS(SELECT 1 FROM dbo.Empleado WHERE codigo='ADMIN-INICIAL')
                        THROW 50002, 'El empleado ADMIN-INICIAL esta inactivo. Revise la configuracion inicial.', 1;
                    SELECT TOP(1) @tienda=id_tienda FROM dbo.Tienda WITH (UPDLOCK,HOLDLOCK) WHERE activo=1 ORDER BY id_tienda;
                    IF @tienda IS NULL
                    BEGIN
                        IF EXISTS(SELECT 1 FROM dbo.Tienda WHERE codigo='INICIAL')
                            THROW 50003, 'La tienda INICIAL esta inactiva. Revise la configuracion inicial.', 1;
                        INSERT dbo.Tienda(codigo,nombre) VALUES('INICIAL',N'Tienda inicial');
                        SET @tienda=CONVERT(int,SCOPE_IDENTITY());
                    END;
                    INSERT dbo.Empleado(codigo,nombre,id_tienda) VALUES('ADMIN-INICIAL',N'Administrador temporal',@tienda);
                    SET @empleado=CONVERT(int,SCOPE_IDENTITY());
                END;
                INSERT dbo.Usuario(id_empleado,nombre_usuario,contrasena_hash,debe_cambiar_contrasena)
                    VALUES(@empleado,N'admin',@hash,1);
                SET @usuario=CONVERT(int,SCOPE_IDENTITY());
                INSERT dbo.UsuarioRol(id_usuario,id_rol) VALUES(@usuario,@rol);
                """, conexion, transaccion);
            crear.Parameters.Add("@hash", SqlDbType.NVarChar, 512).Value = ClaveSegura.CrearTemporalAdministrador();
            crear.ExecuteNonQuery();
        }

        internal static void CambiarClave(AccesoUsuario acceso, string anterior, string nueva)
        {
            if (!acceso.CambiarClave) throw new InvalidOperationException("No hay un cambio de contraseña pendiente.");
            if (anterior == nueva) throw new ArgumentException("La contraseña nueva debe ser diferente de la anterior.");
            string hash = ClaveSegura.Crear(nueva);
            using var conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand("""
                UPDATE u SET contrasena_hash=@hash,debe_cambiar_contrasena=0,intentos_fallidos=0,bloqueado_hasta=NULL,
                contrasena_actualizada_en=SYSUTCDATETIME(),actualizado_en=SYSUTCDATETIME()
                FROM dbo.Usuario u JOIN dbo.Empleado e ON e.id_empleado=u.id_empleado
                WHERE u.id_usuario=@id AND u.version_fila=@version AND u.activo=1 AND e.activo=1 AND u.debe_cambiar_contrasena=1;
                SELECT CONVERT(int,@@ROWCOUNT);
                """, conexion);
            comando.Parameters.Add("@hash", SqlDbType.NVarChar, 512).Value = hash;
            comando.Parameters.Add("@id", SqlDbType.Int).Value = acceso.Id;
            comando.Parameters.Add("@version", SqlDbType.Timestamp, 8).Value = acceso.Version;
            if (Convert.ToInt32(comando.ExecuteScalar()) != 1) throw new InvalidOperationException("La cuenta cambió. Cierra esta ventana e inicia sesión de nuevo.");
        }

        internal static bool Vigente(AccesoUsuario acceso)
        {
            using var conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand("""
                SELECT COUNT(*) FROM dbo.SesionUsuario s JOIN dbo.Usuario u ON u.id_usuario=s.id_usuario
                JOIN dbo.Empleado e ON e.id_empleado=u.id_empleado
                WHERE s.id_sesion=@sesion AND s.id_usuario=@id AND s.revocada_en IS NULL AND s.expira_en>SYSUTCDATETIME()
                AND s.version_seguridad=u.version_seguridad AND u.activo=1 AND e.activo=1 AND u.debe_cambiar_contrasena=0
                """, conexion);
            comando.Parameters.Add("@sesion", SqlDbType.BigInt).Value = (object?)acceso.Sesion ?? DBNull.Value;
            comando.Parameters.Add("@id", SqlDbType.Int).Value = acceso.Id;
            return Convert.ToInt32(comando.ExecuteScalar()) == 1;
        }

        internal static void Cerrar(AccesoUsuario acceso)
        {
            using var conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand("UPDATE dbo.SesionUsuario SET revocada_en=SYSUTCDATETIME() WHERE id_sesion=@sesion AND id_usuario=@id AND revocada_en IS NULL", conexion);
            comando.Parameters.Add("@sesion", SqlDbType.BigInt).Value = (object?)acceso.Sesion ?? DBNull.Value;
            comando.Parameters.Add("@id", SqlDbType.Int).Value = acceso.Id;
            comando.ExecuteNonQuery();
        }
    }
}
