using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Sistema_ERP.Clases;

internal sealed record RespuestaRegistroNativa
{
    internal byte[] IdCredencial { get; init; }
    internal byte[] ObjetoAtestacion { get; init; }
    internal byte[] DatosCliente { get; init; }

    internal RespuestaRegistroNativa(byte[] idCredencial, byte[] objetoAtestacion, byte[] datosCliente)
        => (IdCredencial, ObjetoAtestacion, DatosCliente) = (idCredencial, objetoAtestacion, datosCliente);
}

internal sealed record RespuestaAutenticacionNativa
{
    internal byte[] IdCredencial { get; init; }
    internal byte[] DatosAutenticador { get; init; }
    internal byte[] Firma { get; init; }
    internal byte[]? IdentificadorUsuario { get; init; }
    internal byte[] DatosCliente { get; init; }

    internal RespuestaAutenticacionNativa(byte[] idCredencial, byte[] datosAutenticador, byte[] firma,
        byte[]? identificadorUsuario, byte[] datosCliente)
        => (IdCredencial, DatosAutenticador, Firma, IdentificadorUsuario, DatosCliente) =
            (idCredencial, datosAutenticador, firma, identificadorUsuario, datosCliente);
}

internal static class WebAuthnWindows
{
    private const uint versionEstructura = 1;
    private const uint tiempoEspera = 120_000;
    private const uint autenticadorPlataformaCruzada = 2;
    private const uint verificacionUsuarioRequerida = 1;
    private const uint atestacionNinguna = 1;
    private const int cancelado = unchecked((int)0x80090036);
    private const int abortado = unchecked((int)0x80004004);
    private const int maximoCredencial = 1024;
    private const int maximoRespuesta = 1024 * 1024;

    internal static RespuestaRegistroNativa CrearCredencial(IntPtr propietario, string rpId, string rpNombre,
        string nombreUsuario, string nombreVisible, byte[] userHandle, byte[] challenge, byte[] clientDataJson,
        IEnumerable<byte[]> credencialesExistentes)
    {
        ExigirWindows();
        using var excluidas = new ListaCredencialesNativa(credencialesExistentes);
        using var algoritmos = new ListaAlgoritmosNativa([-7]);
        using var idFijado = new BytesFijados(userHandle);
        using var datosCliente = new BytesFijados(clientDataJson);

        var rp = new EntidadRp { version = versionEstructura, id = rpId, nombre = rpNombre };
        var usuario = new EntidadUsuario
        {
            version = versionEstructura,
            id = idFijado.Direccion,
            longitudId = checked((uint)userHandle.Length),
            nombre = nombreUsuario,
            nombreVisible = nombreVisible
        };
        var parametros = new ParametrosCredenciales
        {
            cantidad = algoritmos.Cantidad,
            elementos = algoritmos.Direccion
        };
        var cliente = new DatosCliente
        {
            version = versionEstructura,
            longitudJson = checked((uint)clientDataJson.Length),
            json = datosCliente.Direccion,
            algoritmoHash = "SHA-256"
        };
        var opciones = new OpcionesCrear
        {
            version = versionEstructura,
            tiempoEsperaMs = tiempoEspera,
            credenciales = excluidas.Lista,
            adjuncion = autenticadorPlataformaCruzada,
            requiereCredencialResidente = 1,
            verificacionUsuario = verificacionUsuarioRequerida,
            preferenciaAtestacion = atestacionNinguna
        };

        int resultado = WebAuthNAuthenticatorMakeCredential(propietario, ref rp, ref usuario, ref parametros,
            ref cliente, ref opciones, out IntPtr salida);
        ComprobarResultado(resultado, "No se pudo crear el passkey");
        try
        {
            if (salida == IntPtr.Zero) throw new InvalidOperationException("Windows devolvió una respuesta de passkey vacía.");
            var atestacion = Marshal.PtrToStructure<AtestacionNativa>(salida);
            if (atestacion.objetoAtestacion == IntPtr.Zero || atestacion.longitudObjetoAtestacion is 0 or > maximoRespuesta)
                throw new InvalidOperationException("Windows devolvió una respuesta de passkey incompleta.");
            if (atestacion.idCredencial == IntPtr.Zero || atestacion.longitudIdCredencial is 0 or > maximoCredencial)
                throw new InvalidOperationException("Windows devolvió un identificador de passkey inválido.");

            return new RespuestaRegistroNativa(
                Copiar(atestacion.idCredencial, atestacion.longitudIdCredencial),
                Copiar(atestacion.objetoAtestacion, atestacion.longitudObjetoAtestacion),
                clientDataJson.ToArray());
        }
        finally { WebAuthNFreeCredentialAttestation(salida); }
    }

    internal static RespuestaAutenticacionNativa ObtenerAfirmacion(IntPtr propietario, string rpId,
        byte[] challenge, byte[] clientDataJson, IEnumerable<byte[]> credencialesPermitidas)
    {
        ExigirWindows();
        using var permitidas = new ListaCredencialesNativa(credencialesPermitidas);
        using var datosCliente = new BytesFijados(clientDataJson);

        var cliente = new DatosCliente
        {
            version = versionEstructura,
            longitudJson = checked((uint)clientDataJson.Length),
            json = datosCliente.Direccion,
            algoritmoHash = "SHA-256"
        };
        var opciones = new OpcionesAfirmar
        {
            version = versionEstructura,
            tiempoEsperaMs = tiempoEspera,
            credenciales = permitidas.Lista,
            adjuncion = autenticadorPlataformaCruzada,
            verificacionUsuario = verificacionUsuarioRequerida
        };

        int resultado = WebAuthNAuthenticatorGetAssertion(propietario, rpId, ref cliente, ref opciones, out IntPtr salida);
        ComprobarResultado(resultado, "No se pudo usar el passkey");
        try
        {
            if (salida == IntPtr.Zero) throw new InvalidOperationException("Windows devolvió una respuesta de passkey vacía.");
            var afirmacion = Marshal.PtrToStructure<AfirmacionNativa>(salida);
            if (afirmacion.credencial.id == IntPtr.Zero || afirmacion.credencial.longitudId is 0 or > maximoCredencial ||
                afirmacion.datosAutenticador == IntPtr.Zero || afirmacion.longitudDatosAutenticador is 0 or > maximoRespuesta ||
                afirmacion.firma == IntPtr.Zero || afirmacion.longitudFirma is 0 or > maximoRespuesta)
                throw new InvalidOperationException("Windows devolvió una respuesta de passkey incompleta.");

            byte[]? userHandle = afirmacion.identificadorUsuario != IntPtr.Zero && afirmacion.longitudIdentificadorUsuario is > 0 and <= 64
                ? Copiar(afirmacion.identificadorUsuario, afirmacion.longitudIdentificadorUsuario)
                : null;

            return new RespuestaAutenticacionNativa(
                Copiar(afirmacion.credencial.id, afirmacion.credencial.longitudId),
                Copiar(afirmacion.datosAutenticador, afirmacion.longitudDatosAutenticador),
                Copiar(afirmacion.firma, afirmacion.longitudFirma),
                userHandle,
                clientDataJson.ToArray());
        }
        finally { WebAuthNFreeAssertion(salida); }
    }

    private static void ExigirWindows()
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 18362))
            throw new PlatformNotSupportedException("Los passkeys requieren Windows 10 versión 1903 o posterior.");
    }

    private static void ComprobarResultado(int resultado, string mensaje)
    {
        if (resultado == 0) return;
        if (resultado is cancelado or abortado) throw new OperationCanceledException("La operación de passkey fue cancelada.");
        Exception? detalle = Marshal.GetExceptionForHR(resultado);
        throw new InvalidOperationException($"{mensaje} (código de Windows 0x{resultado:X8})" +
            (detalle is null ? "." : $": {detalle.Message}"));
    }

    private static byte[] Copiar(IntPtr direccion, uint longitud)
    {
        if (longitud > int.MaxValue) throw new InvalidOperationException("Windows devolvió datos de passkey demasiado grandes.");
        var bytes = new byte[(int)longitud];
        Marshal.Copy(direccion, bytes, 0, bytes.Length);
        return bytes;
    }

    [DllImport("webauthn.dll", ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
    private static extern int WebAuthNAuthenticatorMakeCredential(IntPtr propietario,
        ref EntidadRp rp, ref EntidadUsuario usuario, ref ParametrosCredenciales parametros,
        ref DatosCliente datosCliente, ref OpcionesCrear opciones, out IntPtr atestacion);

    [DllImport("webauthn.dll", ExactSpelling = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode)]
    private static extern int WebAuthNAuthenticatorGetAssertion(IntPtr propietario, string rpId,
        ref DatosCliente datosCliente, ref OpcionesAfirmar opciones, out IntPtr afirmacion);

    [DllImport("webauthn.dll", ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
    private static extern void WebAuthNFreeCredentialAttestation(IntPtr atestacion);

    [DllImport("webauthn.dll", ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
    private static extern void WebAuthNFreeAssertion(IntPtr afirmacion);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct EntidadRp
    {
        internal uint version;
        [MarshalAs(UnmanagedType.LPWStr)] internal string id;
        [MarshalAs(UnmanagedType.LPWStr)] internal string nombre;
        [MarshalAs(UnmanagedType.LPWStr)] internal string? icono;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct EntidadUsuario
    {
        internal uint version;
        internal uint longitudId;
        internal IntPtr id;
        [MarshalAs(UnmanagedType.LPWStr)] internal string nombre;
        [MarshalAs(UnmanagedType.LPWStr)] internal string? icono;
        [MarshalAs(UnmanagedType.LPWStr)] internal string nombreVisible;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DatosCliente
    {
        internal uint version;
        internal uint longitudJson;
        internal IntPtr json;
        [MarshalAs(UnmanagedType.LPWStr)] internal string algoritmoHash;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ParametroCredencial
    {
        internal uint version;
        internal IntPtr tipoCredencial;
        internal int algoritmo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ParametrosCredenciales
    {
        internal uint cantidad;
        internal IntPtr elementos;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Credencial
    {
        internal uint version;
        internal uint longitudId;
        internal IntPtr id;
        internal IntPtr tipoCredencial;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Credenciales
    {
        internal uint cantidad;
        internal IntPtr elementos;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Extensiones
    {
        internal uint cantidad;
        internal IntPtr elementos;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct OpcionesCrear
    {
        internal uint version;
        internal uint tiempoEsperaMs;
        internal Credenciales credenciales;
        internal Extensiones extensiones;
        internal uint adjuncion;
        internal int requiereCredencialResidente;
        internal uint verificacionUsuario;
        internal uint preferenciaAtestacion;
        internal uint flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct OpcionesAfirmar
    {
        internal uint version;
        internal uint tiempoEsperaMs;
        internal Credenciales credenciales;
        internal Extensiones extensiones;
        internal uint adjuncion;
        internal uint verificacionUsuario;
        internal uint flags;
        internal IntPtr u2fAppId;
        internal IntPtr appIdUsado;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AtestacionNativa
    {
        internal uint version;
        internal IntPtr formato;
        internal uint longitudDatosAutenticador;
        internal IntPtr datosAutenticador;
        internal uint longitudAtestacion;
        internal IntPtr atestacion;
        internal uint tipoDecodificacion;
        internal IntPtr datosDecodificados;
        internal uint longitudObjetoAtestacion;
        internal IntPtr objetoAtestacion;
        internal uint longitudIdCredencial;
        internal IntPtr idCredencial;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AfirmacionNativa
    {
        internal uint version;
        internal uint longitudDatosAutenticador;
        internal IntPtr datosAutenticador;
        internal uint longitudFirma;
        internal IntPtr firma;
        internal Credencial credencial;
        internal uint longitudIdentificadorUsuario;
        internal IntPtr identificadorUsuario;
    }

    private sealed class BytesFijados : IDisposable
    {
        private GCHandle bloque;
        internal IntPtr Direccion => bloque.AddrOfPinnedObject();
        internal BytesFijados(byte[] bytes)
        {
            if (bytes.Length == 0) throw new ArgumentException("Los datos WebAuthn no pueden estar vacíos.");
            bloque = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        }
        public void Dispose() { if (bloque.IsAllocated) bloque.Free(); }
    }

    private sealed class ListaCredencialesNativa : IDisposable
    {
        private readonly List<IntPtr> bloques = [];
        internal Credenciales Lista { get; }

        internal ListaCredencialesNativa(IEnumerable<byte[]> ids)
        {
            byte[][] credenciales = ids.ToArray();
            if (credenciales.Length == 0) { Lista = new Credenciales(); return; }
            int tamano = Marshal.SizeOf<Credencial>();
            IntPtr elementos = Marshal.AllocHGlobal(checked(tamano * credenciales.Length));
            bloques.Add(elementos);
            for (int i = 0; i < credenciales.Length; i++)
            {
                byte[] id = credenciales[i];
                if (id.Length is 0 or > maximoCredencial) throw new InvalidOperationException("Hay un identificador de passkey inválido en la base de datos.");
                IntPtr idNativo = Marshal.AllocHGlobal(id.Length);
                bloques.Add(idNativo);
                Marshal.Copy(id, 0, idNativo, id.Length);
                IntPtr tipo = Marshal.StringToHGlobalUni("public-key");
                bloques.Add(tipo);
                var credencial = new Credencial { version = versionEstructura, longitudId = checked((uint)id.Length), id = idNativo, tipoCredencial = tipo };
                Marshal.StructureToPtr(credencial, IntPtr.Add(elementos, checked(i * tamano)), false);
            }
            Lista = new Credenciales { cantidad = checked((uint)credenciales.Length), elementos = elementos };
        }

        public void Dispose() { foreach (IntPtr bloque in bloques) Marshal.FreeHGlobal(bloque); }
    }

    private sealed class ListaAlgoritmosNativa : IDisposable
    {
        private readonly IntPtr elementos;
        private readonly IntPtr tipo;
        internal uint Cantidad { get; }
        internal IntPtr Direccion => elementos;

        internal ListaAlgoritmosNativa(IEnumerable<int> algoritmos)
        {
            int[] valores = algoritmos.Distinct().ToArray();
            if (valores.Length == 0) throw new ArgumentException("Debe indicarse al menos un algoritmo WebAuthn.");
            int tamano = Marshal.SizeOf<ParametroCredencial>();
            elementos = Marshal.AllocHGlobal(checked(tamano * valores.Length));
            tipo = Marshal.StringToHGlobalUni("public-key");
            for (int i = 0; i < valores.Length; i++)
            {
                var parametro = new ParametroCredencial { version = versionEstructura, tipoCredencial = tipo, algoritmo = valores[i] };
                Marshal.StructureToPtr(parametro, IntPtr.Add(elementos, checked(i * tamano)), false);
            }
            Cantidad = checked((uint)valores.Length);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(tipo);
            Marshal.FreeHGlobal(elementos);
        }
    }
}
