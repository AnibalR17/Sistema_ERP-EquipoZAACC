namespace Sistema_ERP;

internal static class ImpuestosVenta
{
    internal static decimal Isv(decimal subtotal, decimal tasa)
    {
        if (tasa is not (0m or 15m or 18m)) throw new ArgumentException("Selecciona ISV 15 %, 18 % o exento.");
        return decimal.Round(subtotal * tasa / 100m, 2, MidpointRounding.AwayFromZero);
    }
}
