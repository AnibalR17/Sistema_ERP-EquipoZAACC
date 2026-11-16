namespace Sistema_ERP;

internal static class CalcularIsvValidacion
{
    internal static void Validar(decimal tasa)
    {
        if (tasa is not (0m or 15m or 18m)) throw new ArgumentException("Selecciona ISV 15 %, 18 % o exento.");
    }
}
