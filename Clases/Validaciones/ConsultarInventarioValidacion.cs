namespace Sistema_ERP;

internal static class ConsultarInventarioValidacion
{
    internal static int Tienda(int? tienda) => tienda ?? throw new ArgumentException("Registra una tienda antes de consultar inventario.");
    internal static void Fechas(DateTime desde, DateTime hasta)
    {
        if (desde.Date > hasta.Date || hasta.Date == DateTime.MaxValue.Date) throw new ArgumentException("Revisa el intervalo de fechas.");
    }
}
