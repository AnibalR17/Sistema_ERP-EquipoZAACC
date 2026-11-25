using System.Text.Json;
using Fido2NetLib;
using Fido2NetLib.Objects;

namespace Sistema_ERP.Clases;

internal static class Passkeys
{
    internal const string rpId = "erp.equipozaacc.local";
    internal const string rpNombre = "Sistema ERP · Equipo ZAACC";
    private const string origen = "https://erp.equipozaacc.local";

    private static readonly Fido2 servidor = new(new Fido2Configuration
    {
        RPID = rpId,
        RPName = rpNombre,
        Origins = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { origen }
    });

    internal static RegistroPasskey Registrar(IntPtr propietario, IdentidadPasskey identidad,
        IReadOnlyList<CredencialPasskey> existentes)
    {
        byte[] userHandle = identidad.UserHandle ?? System.Security.Cryptography.RandomNumberGenerator.GetBytes(32);
        var usuario = new Fido2User { Id = userHandle, Name = identidad.Usuario, DisplayName = identidad.Nombre };
        var opciones = servidor.RequestNewCredential(new RequestNewCredentialParams
        {
            User = usuario,
            ExcludeCredentials = Descriptores(existentes),
            AuthenticatorSelection = new AuthenticatorSelection
            {
                ResidentKey = ResidentKeyRequirement.Required,
                UserVerification = UserVerificationRequirement.Required
            },
            AttestationPreference = AttestationConveyancePreference.None,
            PubKeyCredParams = [new PubKeyCredParam(COSE.Algorithm.ES256, PublicKeyCredentialType.PublicKey)]
        });
        byte[] clientData = CrearDatosCliente("webauthn.create", opciones.Challenge);
        var nativa = WebAuthnWindows.CrearCredencial(propietario, rpId, rpNombre, identidad.Usuario, identidad.Nombre,
            userHandle, opciones.Challenge, clientData, existentes.Select(c => c.Id));
        var respuesta = new AuthenticatorAttestationRawResponse
        {
            Id = Base64Url(nativa.IdCredencial),
            RawId = nativa.IdCredencial,
            Type = PublicKeyCredentialType.PublicKey,
            Response = new AuthenticatorAttestationRawResponse.AttestationResponse
            {
                AttestationObject = nativa.ObjetoAtestacion,
                ClientDataJson = nativa.DatosCliente,
                Transports = []
            },
            ClientExtensionResults = new AuthenticationExtensionsClientOutputs()
        };
        RegisteredPublicKeyCredential registro = servidor.MakeNewCredentialAsync(new MakeNewCredentialParams
        {
            AttestationResponse = respuesta,
            OriginalOptions = CredentialCreateOptions.FromJson(opciones.ToJson()),
            IsCredentialIdUniqueToUserCallback = (args, _) => Task.FromResult(PasskeyDatos.CredencialDisponible(args.CredentialId))
        }).GetAwaiter().GetResult();

        return new RegistroPasskey(registro.Id, registro.PublicKey, registro.User.Id, registro.SignCount);
    }

    internal static ResultadoAcceso Ingresar(string usuario, IntPtr propietario)
    {
        CuentaPasskey? cuenta = PasskeyDatos.CargarParaLogin(usuario);
        if (cuenta is null || !cuenta.Activo || !cuenta.EmpleadoActivo || cuenta.CambiarClave)
            return ResultadoAcceso.Rechazado("Usuario o passkey incorrectos, o cuenta no disponible. Revisa tus datos e inténtalo de nuevo.");
        if (cuenta.BloqueadoHasta > DateTime.UtcNow)
            return ResultadoAcceso.Rechazado("La cuenta está bloqueada temporalmente. Espera 15 minutos antes de volver a intentarlo.");
        if (cuenta.Credenciales.Count == 0)
            return ResultadoAcceso.Rechazado("Esta cuenta todavía no tiene un passkey registrado.");

        var opciones = servidor.GetAssertionOptions(new GetAssertionOptionsParams
        {
            AllowedCredentials = Descriptores(cuenta.Credenciales),
            UserVerification = UserVerificationRequirement.Required
        });
        byte[] clientData = CrearDatosCliente("webauthn.get", opciones.Challenge);
        RespuestaAutenticacionNativa afirmacion;
        try
        {
            afirmacion = WebAuthnWindows.ObtenerAfirmacion(propietario, rpId, opciones.Challenge, clientData,
                cuenta.Credenciales.Select(c => c.Id));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (InvalidOperationException)
        {
            return ResultadoAcceso.Rechazado("No se pudo completar el acceso con passkey. Comprueba Windows Hello, el teléfono o la llave de seguridad e inténtalo de nuevo.");
        }
        catch (PlatformNotSupportedException)
        {
            return ResultadoAcceso.Rechazado("Los passkeys requieren Windows 10 versión 1903 o posterior.");
        }

        CredencialPasskey? credencial = cuenta.Credenciales.FirstOrDefault(c => c.Id.AsSpan().SequenceEqual(afirmacion.IdCredencial));
        if (credencial is null)
        {
            PasskeyDatos.RegistrarFalloLogin(cuenta.Id, cuenta.Usuario);
            return ResultadoAcceso.Rechazado("Usuario o passkey incorrectos, o cuenta no disponible. Revisa tus datos e inténtalo de nuevo.");
        }

        var respuesta = new AuthenticatorAssertionRawResponse
        {
            Id = Base64Url(afirmacion.IdCredencial),
            RawId = afirmacion.IdCredencial,
            Type = PublicKeyCredentialType.PublicKey,
            Response = new AuthenticatorAssertionRawResponse.AssertionResponse
            {
                AuthenticatorData = afirmacion.DatosAutenticador,
                Signature = afirmacion.Firma,
                ClientDataJson = afirmacion.DatosCliente,
                UserHandle = afirmacion.IdentificadorUsuario ?? credencial.UserHandle
            },
            ClientExtensionResults = new AuthenticationExtensionsClientOutputs()
        };
        VerifyAssertionResult verificada;
        try
        {
            verificada = servidor.MakeAssertionAsync(new MakeAssertionParams
            {
                AssertionResponse = respuesta,
                OriginalOptions = AssertionOptions.FromJson(opciones.ToJson()),
                StoredPublicKey = credencial.PublicKey,
                StoredSignatureCounter = credencial.SignCount,
                IsUserHandleOwnerOfCredentialIdCallback = (args, _) => Task.FromResult(
                    PasskeyDatos.UserHandlePertenece(args.CredentialId, args.UserHandle))
            }).GetAwaiter().GetResult();
        }
        catch (Fido2VerificationException)
        {
            PasskeyDatos.RegistrarFalloLogin(cuenta.Id, cuenta.Usuario);
            return ResultadoAcceso.Rechazado("Usuario o passkey incorrectos, o cuenta no disponible. Revisa tus datos e inténtalo de nuevo.");
        }

        return PasskeyDatos.FinalizarLogin(cuenta, credencial, verificada.SignCount);
    }

    private static IReadOnlyList<PublicKeyCredentialDescriptor> Descriptores(IEnumerable<CredencialPasskey> credenciales) =>
        credenciales.Select(credencial => new PublicKeyCredentialDescriptor(PublicKeyCredentialType.PublicKey,
            credencial.Id, [])).ToArray();

    private static byte[] CrearDatosCliente(string tipo, byte[] challenge) => JsonSerializer.SerializeToUtf8Bytes(new
    {
        type = tipo,
        challenge = Base64Url(challenge),
        origin = origen,
        crossOrigin = false
    });

    private static string Base64Url(byte[] bytes) => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
