namespace Sistema_ERP;

internal static class AnularDevolucionValidacion
{
    internal static void Estado(string? estado)
    {
        if (estado == "ANULADA") throw new InvalidOperationException("La devolución ya está anulada.");
    }
}
