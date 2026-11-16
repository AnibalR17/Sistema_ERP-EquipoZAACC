namespace Sistema_ERP;

internal class CatalogosImportacion
{
    public HashSet<string> CodigosBD { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> CodigosArchivo { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> Categorias { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> Proveedores { get; init; } = new(StringComparer.OrdinalIgnoreCase);
}
