namespace Sistema_ERP;

internal static class CrearApartadoValidacion
{
    internal static (int Tienda, int Cliente) Seleccion(int? tienda, int? cliente)
    {
        if (tienda is null) throw new ErrorCampoValidacion("Tienda", "Selecciona una tienda.");
        if (cliente is null) throw new ErrorCampoValidacion("Cliente", "Selecciona un cliente existente y activo. Puedes usar Buscar cliente.");
        return (tienda.Value, cliente.Value);
    }

    internal static void Validar(int cliente, IReadOnlyCollection<LineaVenta> lineas)
    {
        if (cliente <= 0) throw new ArgumentException("Selecciona un cliente existente y activo para crear el apartado.");
        if (lineas.Count == 0 || lineas.Any(l => l.Producto <= 0 || l.Cantidad <= 0) || lineas.Select(l => l.Producto).Distinct().Count() != lineas.Count)
            throw new ArgumentException("Agrega productos sin repetir y con cantidades positivas.");
    }
}
