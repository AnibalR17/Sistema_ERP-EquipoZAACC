namespace Sistema_ERP;

internal static class AgregarProductoVentaValidacion
{
    internal static int Cantidad(int unidades, int existentes, int disponible)
    {
        long total = (long)unidades + existentes;
        if (total > disponible) throw new ArgumentException("La cantidad supera la existencia disponible.");
        return (int)total;
    }
}
