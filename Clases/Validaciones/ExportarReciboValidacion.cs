namespace Sistema_ERP;

internal static class ExportarReciboValidacion
{
    internal static void Validar(string contenido)
    {
        if (string.IsNullOrWhiteSpace(contenido)) throw new ArgumentException("El recibo no tiene contenido para exportar.");
    }
}
