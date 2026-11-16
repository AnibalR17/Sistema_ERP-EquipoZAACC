namespace Sistema_ERP;

internal static class ReembolsarDevolucionValidacion
{
    internal static void Estado(string? estado)
    {
        if (estado != "CONFIRMADA") throw new InvalidOperationException("Primero confirma la devolución.");
    }

    internal static void Pendiente(decimal pendiente)
    {
        if (pendiente <= 0) throw new InvalidOperationException("La devolución ya está reembolsada.");
    }

    internal static void Aplicado(decimal pendiente)
    {
        if (pendiente > 0) throw new InvalidOperationException("Los pagos de la venta no tienen suficiente saldo reembolsable.");
    }
}
