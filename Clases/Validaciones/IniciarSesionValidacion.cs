namespace Sistema_ERP;

internal static class IniciarSesionValidacion
{
    internal static bool Credenciales(string usuario, string clave) => usuario.Length is >= 1 and <= 50 && clave.Length is >= 1 and <= 128;
}
