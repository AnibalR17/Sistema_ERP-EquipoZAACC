namespace Sistema_ERP;

internal static class ImpuestosVenta
{
    internal static decimal Isv(decimal subtotal, decimal tasa)
    {
        CalcularIsvValidacion.Validar(tasa);
        return decimal.Round(subtotal * tasa / 100m, 2, MidpointRounding.AwayFromZero);
    }
}
