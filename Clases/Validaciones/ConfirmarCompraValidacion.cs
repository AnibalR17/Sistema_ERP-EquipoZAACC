namespace Sistema_ERP;

internal static class ConfirmarCompraValidacion
{
    internal static void Estado(string estado)
    {
        if (estado != "BORRADOR") throw new InvalidOperationException("Solo se puede confirmar una compra en BORRADOR.");
    }
}
