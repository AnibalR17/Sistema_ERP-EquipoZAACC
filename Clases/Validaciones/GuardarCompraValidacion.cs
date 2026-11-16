using Sistema_ERP.Clases;

namespace Sistema_ERP;

internal static class GuardarCompraValidacion
{
    internal static void Estado(string estado)
    {
        if (estado != "BORRADOR") throw new InvalidOperationException("Solo se pueden editar compras en BORRADOR.");
    }
    internal static void Version(int? id, byte[]? version)
    {
        if (id.HasValue && (id.Value <= 0 || version is not { Length: 32 })) throw new ArgumentException("Recarga la compra antes de actualizarla.");
    }
    internal static (int Proveedor, int Tienda) Captura(bool lineaPendiente, int? proveedor, int? tienda)
    {
        if (lineaPendiente) throw new ArgumentException("Agrega o actualiza la línea en edición antes de guardar el borrador.");
        if (proveedor is null || tienda is null) throw new ArgumentException("Selecciona proveedor y tienda.");
        return (proveedor.Value, tienda.Value);
    }
    private const decimal maximoImporte = 9999999999999999.99m;
    internal static void Validar(int proveedor, int tienda, string factura, string observaciones, IReadOnlyCollection<LineaCompra> lineas)
    {
        if (proveedor <= 0 || tienda <= 0) throw new ArgumentException("Selecciona proveedor y tienda.");
        if (string.IsNullOrWhiteSpace(factura) || factura.Trim().Length > 50)
            throw new ArgumentException("Introduce un número de factura de hasta 50 caracteres.");
        ValidacionDatos.NoSoloCeros(factura, "Número de factura");
        ValidacionDatos.NoSoloCeros(observaciones, "Observaciones");
        if (observaciones.Length > 500) throw new ArgumentException("Las observaciones admiten hasta 500 caracteres.");
        if (lineas.Count == 0 || lineas.Any(l => l.Producto <= 0 || l.Cantidad <= 0 || l.Costo <= 0
            || l.Costo > maximoImporte || decimal.Round(l.Costo, 2) != l.Costo)
            || lineas.Select(l => l.Producto).Distinct().Count() != lineas.Count)
            throw new ArgumentException("Agrega productos sin repetir, cantidades enteras positivas y costos positivos con hasta dos decimales.");
        decimal total = 0;
        foreach (var linea in lineas)
        {
            decimal subtotal = linea.Cantidad * linea.Costo;
            if (subtotal > maximoImporte || total > maximoImporte - subtotal)
                throw new ArgumentException("El subtotal o total supera el importe permitido (decimal 18,2).");
            total += subtotal;
        }
    }
}
