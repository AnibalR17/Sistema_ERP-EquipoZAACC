namespace Sistema_ERP;

internal static class ExportarCatalogoValidacion
{
    internal static void Validar(int filas)
    {
        if (filas == 0) throw new ArgumentException("No hay productos para exportar con los filtros actuales.");
    }
}
