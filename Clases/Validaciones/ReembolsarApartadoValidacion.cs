namespace Sistema_ERP;

internal static class ReembolsarApartadoValidacion
{
    internal static void Estado(string? estado)
    {
        if (estado is not ("CANCELADO" or "VENCIDO")) throw new InvalidOperationException("Primero cancela el apartado o márcalo como vencido.");
    }

    internal static void Total(decimal total)
    {
        if (total == 0) throw new InvalidOperationException("No hay abonos pendientes de reembolso.");
    }
}
