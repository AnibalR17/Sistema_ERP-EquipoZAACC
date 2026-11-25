using System.Data;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;

namespace Sistema_ERP;

internal sealed record CredencialPasskey
{
    internal byte[] Id { get; init; }
    internal byte[] PublicKey { get; init; }
    internal byte[] UserHandle { get; init; }
    internal uint SignCount { get; init; }
    internal string Nombre { get; init; }

    internal CredencialPasskey(byte[] id, byte[] publicKey, byte[] userHandle, uint signCount, string nombre)
        => (Id, PublicKey, UserHandle, SignCount, Nombre) = (id, publicKey, userHandle, signCount, nombre);
}

internal sealed record CuentaPasskey
{
    internal int Id { get; init; }
    internal string Usuario { get; init; }
    internal string Nombre { get; init; }
    internal bool Activo { get; init; }
    internal bool EmpleadoActivo { get; init; }
    internal bool CambiarClave { get; init; }
    internal DateTime? BloqueadoHasta { get; init; }
    internal IReadOnlyList<CredencialPasskey> Credenciales { get; init; }

    internal CuentaPasskey(int id, string usuario, string nombre, bool activo, bool empleadoActivo,
        bool cambiarClave, DateTime? bloqueadoHasta, IReadOnlyList<CredencialPasskey> credenciales)
        => (Id, Usuario, Nombre, Activo, EmpleadoActivo, CambiarClave, BloqueadoHasta, Credenciales) =
            (id, usuario, nombre, activo, empleadoActivo, cambiarClave, bloqueadoHasta, credenciales);
}

internal sealed record PasskeyResumen
{
    internal byte[] Id { get; init; }
    internal string Nombre { get; init; }
    internal DateTime CreadaEn { get; init; }
    internal DateTime? UltimoUsoEn { get; init; }

    internal PasskeyResumen(byte[] id, string nombre, DateTime creadaEn, DateTime? ultimoUsoEn)
        => (Id, Nombre, CreadaEn, UltimoUsoEn) = (id, nombre, creadaEn, ultimoUsoEn);
}

internal sealed record RegistroPasskey
{
    internal byte[] Id { get; init; }
    internal byte[] PublicKey { get; init; }
    internal byte[] UserHandle { get; init; }
    internal uint SignCount { get; init; }

    internal RegistroPasskey(byte[] id, byte[] publicKey, byte[] userHandle, uint signCount)
        => (Id, PublicKey, UserHandle, SignCount) = (id, publicKey, userHandle, signCount);
}

internal sealed record IdentidadPasskey
{
    internal string Usuario { get; init; }
    internal string Nombre { get; init; }
    internal byte[]? UserHandle { get; init; }

    internal IdentidadPasskey(string usuario, string nombre, byte[]? userHandle)
        => (Usuario, Nombre, UserHandle) = (usuario, nombre, userHandle);
}

internal static class PasskeyDatos
{
    internal static CuentaPasskey? CargarParaLogin(string nombreUsuario)
    {
        using var conexion = Conexion.AbrirConexion();
        using var comando = new SqlCommand("""
            SELECT u.id_usuario,u.nombre_usuario,e.nombre,u.activo,e.activo AS empleado_activo,
                u.debe_cambiar_contrasena,u.bloqueado_hasta
            FROM dbo.Usuario u JOIN dbo.Empleado e ON e.id_empleado=u.id_empleado
            WHERE u.nombre_usuario_normalizado=UPPER(LTRIM(RTRIM(@usuario)))
            """, conexion);
        comando.Parameters.Add("@usuario", SqlDbType.NVarChar, 50).Value = nombreUsuario.Trim();
        using var lector = comando.ExecuteReader();
        if (!lector.Read()) return null;

        int id = lector.GetInt32(0);
        string usuario = lector.GetString(1), nombre = lector.GetString(2);
        bool activo = lector.GetBoolean(3), empleadoActivo = lector.GetBoolean(4), cambiar = lector.GetBoolean(5);
        DateTime? bloqueo = lector.IsDBNull(6) ? null : lector.GetDateTime(6);
        lector.Close();

        var credenciales = new List<CredencialPasskey>();
        using var listar = new SqlCommand("""
            SELECT id_credencial,clave_publica,user_handle,contador_firma,nombre
            FROM dbo.PasskeyUsuario WHERE id_usuario=@id AND revocada_en IS NULL ORDER BY creada_en
            """, conexion);
        listar.Parameters.Add("@id", SqlDbType.Int).Value = id;
        using (var credencial = listar.ExecuteReader())
            while (credencial.Read())
                credenciales.Add(new CredencialPasskey((byte[])credencial[0], (byte[])credencial[1], (byte[])credencial[2],
                    checked((uint)credencial.GetInt64(3)), credencial.GetString(4)));

        return new CuentaPasskey(id, usuario, nombre, activo, empleadoActivo, cambiar, bloqueo, credenciales);
    }

    internal static IdentidadPasskey IdentidadActual(AccesoUsuario acceso)
    {
        using var conexion = Conexion.AbrirConexion();
        SesionActual.Validar(conexion);
        using var comando = new SqlCommand("""
            SELECT u.nombre_usuario,e.nombre,
                (SELECT TOP(1) p.user_handle FROM dbo.PasskeyUsuario p WHERE p.id_usuario=u.id_usuario ORDER BY p.creada_en)
            FROM dbo.Usuario u JOIN dbo.Empleado e ON e.id_empleado=u.id_empleado
            WHERE u.id_usuario=@id AND u.activo=1 AND e.activo=1 AND u.debe_cambiar_contrasena=0
            """, conexion);
        comando.Parameters.Add("@id", SqlDbType.Int).Value = acceso.Id;
        using var lector = comando.ExecuteReader();
        if (!lector.Read()) throw new InvalidOperationException("La cuenta ya no está disponible para registrar passkeys.");
        return new IdentidadPasskey(lector.GetString(0), lector.GetString(1), lector.IsDBNull(2) ? null : (byte[])lector[2]);
    }

    internal static IReadOnlyList<CredencialPasskey> CredencialesActuales(AccesoUsuario acceso)
    {
        using var conexion = Conexion.AbrirConexion();
        SesionActual.Validar(conexion);
        return LeerCredenciales(conexion, acceso.Id);
    }

    internal static IReadOnlyList<PasskeyResumen> Listar(AccesoUsuario acceso)
    {
        using var conexion = Conexion.AbrirConexion();
        SesionActual.Validar(conexion);
        using var comando = new SqlCommand("""
            SELECT id_credencial,nombre,creada_en,ultimo_uso_en FROM dbo.PasskeyUsuario
            WHERE id_usuario=@id AND revocada_en IS NULL ORDER BY creada_en DESC
            """, conexion);
        comando.Parameters.Add("@id", SqlDbType.Int).Value = acceso.Id;
        using var lector = comando.ExecuteReader();
        var filas = new List<PasskeyResumen>();
        while (lector.Read()) filas.Add(new PasskeyResumen((byte[])lector[0], lector.GetString(1), lector.GetDateTime(2),
            lector.IsDBNull(3) ? null : lector.GetDateTime(3)));
        return filas;
    }

    internal static bool ValidarClaveActual(AccesoUsuario acceso, string clave)
    {
        if (string.IsNullOrWhiteSpace(clave)) return false;
        using var conexion = Conexion.AbrirConexion();
        using var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
        SesionActual.Validar(conexion, transaccion);
        using var consultar = new SqlCommand("""
            SELECT u.contrasena_hash,u.activo,e.activo,u.debe_cambiar_contrasena,u.bloqueado_hasta,u.intentos_fallidos,u.nombre_usuario
            FROM dbo.Usuario u WITH(UPDLOCK,HOLDLOCK) JOIN dbo.Empleado e WITH(HOLDLOCK) ON e.id_empleado=u.id_empleado
            WHERE u.id_usuario=@id
            """, conexion, transaccion);
        consultar.Parameters.Add("@id", SqlDbType.Int).Value = acceso.Id;
        string? hash = null, usuario = null;
        bool activo = false, empleadoActivo = false, cambiar = true;
        DateTime? bloqueo = null;
        int intentos = 0;
        using (var lector = consultar.ExecuteReader())
        {
            if (lector.Read())
            {
                hash = lector.GetString(0); activo = lector.GetBoolean(1); empleadoActivo = lector.GetBoolean(2);
                cambiar = lector.GetBoolean(3); bloqueo = lector.IsDBNull(4) ? null : lector.GetDateTime(4);
                intentos = lector.GetInt32(5); usuario = lector.GetString(6);
            }
        }
        if (hash is null || !activo || !empleadoActivo || cambiar)
        {
            transaccion.Commit();
            return false;
        }
        using var reloj = new SqlCommand("SELECT SYSUTCDATETIME()", conexion, transaccion);
        DateTime ahora = (DateTime)reloj.ExecuteScalar()!;
        if (bloqueo > ahora)
        {
            RegistrarEvento(conexion, transaccion, acceso.Id, usuario!, "BLOQUEADO");
            transaccion.Commit();
            return false;
        }

        bool correcta = ClaveSegura.Verificar(clave, hash);
        if (!correcta)
        {
            using var fallo = new SqlCommand("""
                UPDATE dbo.Usuario SET
                    intentos_fallidos=CASE WHEN bloqueado_hasta IS NOT NULL AND bloqueado_hasta<=SYSUTCDATETIME() THEN 1 ELSE intentos_fallidos+1 END,
                    bloqueado_hasta=CASE WHEN bloqueado_hasta IS NOT NULL AND bloqueado_hasta<=SYSUTCDATETIME() THEN NULL
                        WHEN intentos_fallidos>=4 THEN DATEADD(minute,15,SYSUTCDATETIME()) ELSE NULL END
                WHERE id_usuario=@id
                """, conexion, transaccion);
            fallo.Parameters.Add("@id", SqlDbType.Int).Value = acceso.Id;
            fallo.ExecuteNonQuery();
            RegistrarEvento(conexion, transaccion, acceso.Id, usuario!, "INVALIDO");
            transaccion.Commit();
            return false;
        }

        RegistrarEvento(conexion, transaccion, acceso.Id, usuario!, "EXITOSO");
        transaccion.Commit();
        return true;
    }

    internal static void Guardar(AccesoUsuario acceso, RegistroPasskey registro, string nombre)
    {
        if (registro.Id.Length is 0 or > 1024 || registro.UserHandle.Length is 0 or > 64 || registro.PublicKey.Length is 0 or > 4096)
            throw new InvalidOperationException("El autenticador devolvió datos de passkey no válidos.");
        using var conexion = Conexion.AbrirConexion();
        using var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
        SesionActual.Validar(conexion, transaccion);
        using var comprobar = new SqlCommand("""
            SELECT COUNT(*) FROM dbo.Usuario u WITH(UPDLOCK,HOLDLOCK) JOIN dbo.Empleado e WITH(HOLDLOCK) ON e.id_empleado=u.id_empleado
            WHERE u.id_usuario=@id AND u.activo=1 AND e.activo=1 AND u.debe_cambiar_contrasena=0
            """, conexion, transaccion);
        comprobar.Parameters.Add("@id", SqlDbType.Int).Value = acceso.Id;
        if (Convert.ToInt32(comprobar.ExecuteScalar()) != 1)
            throw new InvalidOperationException("La cuenta ya no está disponible para registrar passkeys.");

        using (var comprobarHandle = new SqlCommand("""
            SELECT TOP(1) user_handle FROM dbo.PasskeyUsuario WITH(UPDLOCK,HOLDLOCK)
            WHERE id_usuario=@id ORDER BY creada_en
            """, conexion, transaccion))
        {
            comprobarHandle.Parameters.Add("@id", SqlDbType.Int).Value = acceso.Id;
            if (comprobarHandle.ExecuteScalar() is byte[] handle &&
                !CryptographicOperations.FixedTimeEquals(handle, registro.UserHandle))
                throw new InvalidOperationException("El identificador WebAuthn de la cuenta ya no coincide.");
        }

        using var insertar = new SqlCommand("""
            INSERT dbo.PasskeyUsuario(id_credencial,id_usuario,user_handle,clave_publica,contador_firma,nombre)
            VALUES(@credencial,@id,@handle,@publica,@contador,@nombre)
            """, conexion, transaccion);
        insertar.Parameters.Add("@credencial", SqlDbType.VarBinary, 1024).Value = registro.Id;
        insertar.Parameters.Add("@id", SqlDbType.Int).Value = acceso.Id;
        insertar.Parameters.Add("@handle", SqlDbType.VarBinary, 64).Value = registro.UserHandle;
        insertar.Parameters.Add("@publica", SqlDbType.VarBinary, 4096).Value = registro.PublicKey;
        insertar.Parameters.Add("@contador", SqlDbType.BigInt).Value = registro.SignCount;
        insertar.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = nombre.Trim();
        insertar.ExecuteNonQuery();
        transaccion.Commit();
    }

    internal static void Revocar(AccesoUsuario acceso, byte[] idCredencial)
    {
        using var conexion = Conexion.AbrirConexion();
        using var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
        SesionActual.Validar(conexion, transaccion);
        using var comando = new SqlCommand("""
            UPDATE dbo.PasskeyUsuario SET revocada_en=SYSUTCDATETIME()
            WHERE id_usuario=@id AND id_credencial=@credencial AND revocada_en IS NULL;
            SELECT CONVERT(int,@@ROWCOUNT);
            """, conexion, transaccion);
        comando.Parameters.Add("@id", SqlDbType.Int).Value = acceso.Id;
        comando.Parameters.Add("@credencial", SqlDbType.VarBinary, 1024).Value = idCredencial;
        int actualizadas = Convert.ToInt32(comando.ExecuteScalar());
        if (actualizadas != 1) throw new InvalidOperationException("Ese passkey ya no está activo. Recarga la lista.");
        transaccion.Commit();
    }

    internal static bool CredencialDisponible(byte[] idCredencial)
    {
        using var conexion = Conexion.AbrirConexion();
        using var comando = new SqlCommand("SELECT COUNT(*) FROM dbo.PasskeyUsuario WHERE id_credencial=@id", conexion);
        comando.Parameters.Add("@id", SqlDbType.VarBinary, 1024).Value = idCredencial;
        return Convert.ToInt32(comando.ExecuteScalar()) == 0;
    }

    internal static bool UserHandlePertenece(byte[] idCredencial, byte[] userHandle)
    {
        using var conexion = Conexion.AbrirConexion();
        using var comando = new SqlCommand("""
            SELECT COUNT(*) FROM dbo.PasskeyUsuario
            WHERE id_credencial=@credencial AND user_handle=@handle AND revocada_en IS NULL
            """, conexion);
        comando.Parameters.Add("@credencial", SqlDbType.VarBinary, 1024).Value = idCredencial;
        comando.Parameters.Add("@handle", SqlDbType.VarBinary, 64).Value = userHandle;
        return Convert.ToInt32(comando.ExecuteScalar()) == 1;
    }

    internal static ResultadoAcceso FinalizarLogin(CuentaPasskey cuenta, CredencialPasskey credencial, uint contadorNuevo)
    {
        using var conexion = Conexion.AbrirConexion();
        using var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
        using var consultar = new SqlCommand("""
            SELECT u.nombre_usuario,e.nombre,u.activo,e.activo,u.debe_cambiar_contrasena,u.version_fila,
                u.version_seguridad,u.bloqueado_hasta,p.user_handle,p.contador_firma
            FROM dbo.PasskeyUsuario p WITH(UPDLOCK,HOLDLOCK)
            JOIN dbo.Usuario u WITH(UPDLOCK,HOLDLOCK) ON u.id_usuario=p.id_usuario
            JOIN dbo.Empleado e WITH(HOLDLOCK) ON e.id_empleado=u.id_empleado
            WHERE p.id_credencial=@credencial AND p.id_usuario=@id AND p.revocada_en IS NULL
            """, conexion, transaccion);
        consultar.Parameters.Add("@credencial", SqlDbType.VarBinary, 1024).Value = credencial.Id;
        consultar.Parameters.Add("@id", SqlDbType.Int).Value = cuenta.Id;
        string usuario = "", nombre = "";
        bool activa = false, empleadoActivo = false, cambiar = true;
        byte[] version = [], userHandle = [];
        Guid versionSeguridad = Guid.Empty;
        DateTime? bloqueo = null;
        long contadorActual = 0;
        using (var lector = consultar.ExecuteReader())
        {
            if (lector.Read())
            {
                usuario = lector.GetString(0); nombre = lector.GetString(1); activa = lector.GetBoolean(2); empleadoActivo = lector.GetBoolean(3);
                cambiar = lector.GetBoolean(4); version = (byte[])lector[5]; versionSeguridad = lector.GetGuid(6);
                bloqueo = lector.IsDBNull(7) ? null : lector.GetDateTime(7); userHandle = (byte[])lector[8]; contadorActual = lector.GetInt64(9);
            }
        }
        using var reloj = new SqlCommand("SELECT SYSUTCDATETIME()", conexion, transaccion);
        DateTime ahora = (DateTime)reloj.ExecuteScalar()!;
        if (version.Length == 0 || !activa || !empleadoActivo || cambiar || bloqueo > ahora ||
            !CryptographicOperations.FixedTimeEquals(userHandle, credencial.UserHandle))
        {
            RegistrarEvento(conexion, transaccion, cuenta.Id, cuenta.Usuario, bloqueo > ahora ? "BLOQUEADO" : "INACTIVO");
            transaccion.Commit();
            return ResultadoAcceso.Rechazado(bloqueo > ahora
                ? "La cuenta está bloqueada temporalmente. Espera 15 minutos antes de volver a intentarlo."
                : "Usuario o passkey incorrectos, o cuenta no disponible. Revisa tus datos e inténtalo de nuevo.");
        }

        using (var actualizarCredencial = new SqlCommand("""
            UPDATE dbo.PasskeyUsuario SET contador_firma=CASE WHEN @contador>@actual THEN @contador ELSE @actual END,
                ultimo_uso_en=SYSUTCDATETIME()
            WHERE id_credencial=@credencial AND id_usuario=@id AND revocada_en IS NULL
            """, conexion, transaccion))
        {
            actualizarCredencial.Parameters.Add("@contador", SqlDbType.BigInt).Value = contadorNuevo;
            actualizarCredencial.Parameters.Add("@actual", SqlDbType.BigInt).Value = contadorActual;
            actualizarCredencial.Parameters.Add("@credencial", SqlDbType.VarBinary, 1024).Value = credencial.Id;
            actualizarCredencial.Parameters.Add("@id", SqlDbType.Int).Value = cuenta.Id;
            if (actualizarCredencial.ExecuteNonQuery() != 1)
            {
                transaccion.Rollback();
                return ResultadoAcceso.Rechazado("Usuario o passkey incorrectos, o cuenta no disponible. Revisa tus datos e inténtalo de nuevo.");
            }
        }

        using (var actualizarUsuario = new SqlCommand("""
            UPDATE dbo.Usuario SET intentos_fallidos=0,bloqueado_hasta=NULL,ultimo_acceso_en=SYSUTCDATETIME()
            WHERE id_usuario=@id
            """, conexion, transaccion))
        {
            actualizarUsuario.Parameters.Add("@id", SqlDbType.Int).Value = cuenta.Id;
            actualizarUsuario.ExecuteNonQuery();
        }
        RegistrarEvento(conexion, transaccion, cuenta.Id, usuario, "EXITOSO");
        string roles;
        using (var consultarRoles = new SqlCommand("""
            SELECT r.nombre FROM dbo.UsuarioRol ur JOIN dbo.Rol r ON r.id_rol=ur.id_rol
            WHERE ur.id_usuario=@id AND ur.activo=1 AND r.activo=1 ORDER BY r.nombre
            """, conexion, transaccion))
        {
            consultarRoles.Parameters.Add("@id", SqlDbType.Int).Value = cuenta.Id;
            using var lector = consultarRoles.ExecuteReader();
            var nombres = new List<string>();
            while (lector.Read()) nombres.Add(lector.GetString(0));
            roles = nombres.Count == 0 ? "Sin roles asignados" : string.Join(", ", nombres);
        }
        long sesion;
        using (var crear = new SqlCommand("""
            INSERT dbo.SesionUsuario(id_usuario,token_hash,version_seguridad,expira_en,equipo)
            VALUES(@id,@token,@seguridad,DATEADD(hour,8,SYSUTCDATETIME()),@equipo);
            SELECT CONVERT(bigint,SCOPE_IDENTITY());
            """, conexion, transaccion))
        {
            crear.Parameters.Add("@id", SqlDbType.Int).Value = cuenta.Id;
            crear.Parameters.Add("@token", SqlDbType.Binary, 32).Value = SHA256.HashData(RandomNumberGenerator.GetBytes(32));
            crear.Parameters.Add("@seguridad", SqlDbType.UniqueIdentifier).Value = versionSeguridad;
            crear.Parameters.Add("@equipo", SqlDbType.NVarChar, 150).Value = Environment.MachineName;
            sesion = Convert.ToInt64(crear.ExecuteScalar());
        }
        transaccion.Commit();
        return new ResultadoAcceso(new AccesoUsuario(cuenta.Id, nombre, roles, false, version, sesion), null);
    }

    internal static void RegistrarFalloLogin(int? id, string usuario)
    {
        using var conexion = Conexion.AbrirConexion();
        using var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
        if (id.HasValue)
        {
            using var fallo = new SqlCommand("""
                UPDATE dbo.Usuario SET intentos_fallidos=CASE WHEN bloqueado_hasta IS NOT NULL AND bloqueado_hasta<=SYSUTCDATETIME() THEN 1 ELSE intentos_fallidos+1 END,
                    bloqueado_hasta=CASE WHEN bloqueado_hasta IS NOT NULL AND bloqueado_hasta<=SYSUTCDATETIME() THEN NULL
                        WHEN intentos_fallidos>=4 THEN DATEADD(minute,15,SYSUTCDATETIME()) ELSE NULL END
                WHERE id_usuario=@id AND activo=1
                """, conexion, transaccion);
            fallo.Parameters.Add("@id", SqlDbType.Int).Value = id.Value;
            fallo.ExecuteNonQuery();
        }
        RegistrarEvento(conexion, transaccion, id, usuario, "INVALIDO");
        transaccion.Commit();
    }

    private static IReadOnlyList<CredencialPasskey> LeerCredenciales(SqlConnection conexion, int idUsuario)
    {
        using var comando = new SqlCommand("""
            SELECT id_credencial,clave_publica,user_handle,contador_firma,nombre FROM dbo.PasskeyUsuario
            WHERE id_usuario=@id AND revocada_en IS NULL ORDER BY creada_en
            """, conexion);
        comando.Parameters.Add("@id", SqlDbType.Int).Value = idUsuario;
        using var lector = comando.ExecuteReader();
        var credenciales = new List<CredencialPasskey>();
        while (lector.Read()) credenciales.Add(new CredencialPasskey((byte[])lector[0], (byte[])lector[1],
            (byte[])lector[2], checked((uint)lector.GetInt64(3)), lector.GetString(4)));
        return credenciales;
    }

    private static void RegistrarEvento(SqlConnection conexion, SqlTransaction transaccion, int? id, string usuario, string resultado)
    {
        using var evento = new SqlCommand("""
            INSERT dbo.EventoAcceso(id_usuario,nombre_intentado,resultado,equipo)
            VALUES(@id,@usuario,@resultado,@equipo)
            """, conexion, transaccion);
        evento.Parameters.Add("@id", SqlDbType.Int).Value = (object?)id ?? DBNull.Value;
        evento.Parameters.Add("@usuario", SqlDbType.NVarChar, 50).Value = usuario;
        evento.Parameters.Add("@resultado", SqlDbType.VarChar, 30).Value = resultado;
        evento.Parameters.Add("@equipo", SqlDbType.NVarChar, 150).Value = Environment.MachineName;
        evento.ExecuteNonQuery();
    }
}
