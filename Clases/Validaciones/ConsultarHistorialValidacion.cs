namespace Sistema_ERP;

internal static class ConsultarHistorialValidacion
{
    internal static void Fechas(DateTime? desde, DateTime? hasta)
    {
        if (desde?.Date > hasta?.Date) throw new ArgumentException("La fecha desde no puede ser posterior a la fecha hasta.");
        if (hasta?.Date == DateTime.MaxValue.Date) throw new ArgumentException("Selecciona una fecha final anterior al 31/12/9999.");
    }

    internal static void Compras(string estado, DateTime? desde, DateTime? hasta)
    {
        if (estado.Length > 0 && estado is not ("BORRADOR" or "CONFIRMADA" or "ANULADA")) throw new ArgumentException("Selecciona un estado de compra válido.");
        if (desde.HasValue && hasta.HasValue && desde.Value.Date > hasta.Value.Date) throw new ArgumentException("La fecha inicial no puede ser posterior a la final.");
    }
}
