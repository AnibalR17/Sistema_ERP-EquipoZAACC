namespace Sistema_ERP;

internal static class ConfirmarDevolucionValidacion
{
    internal static void Estado(string? estado)
    {
        if (estado != "BORRADOR") throw new InvalidOperationException("Solo se confirma una devolución en borrador.");
    }
}
