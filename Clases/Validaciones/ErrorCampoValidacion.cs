namespace Sistema_ERP;

internal sealed class ErrorCampoValidacion(string campo, string mensaje) : ArgumentException(mensaje)
{
    internal string Campo { get; } = campo;
}
