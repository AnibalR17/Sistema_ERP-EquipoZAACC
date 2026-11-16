using Sistema_ERP.Clases;

namespace Sistema_ERP;

internal static class EmitirVentaValidacion
{
    internal static void PrecioActual(object? precio, decimal esperado)
    {
        if (precio is null || precio == DBNull.Value) throw new InvalidOperationException("Un producto no está activo o no tiene existencias suficientes. Recarga el catálogo.");
        if (Convert.ToDecimal(precio) != esperado) throw new InvalidOperationException("El precio de un producto cambió. Actualiza los catálogos y vuelve a agregar los productos.");
    }
    internal static (int Tienda, int Cliente, int Metodo) Captura(int? tienda, int? cliente, int? metodo, int productos)
    {
        if (tienda is null || cliente is null || metodo is null || productos == 0)
            throw new ArgumentException("Selecciona tienda, cliente, pago y agrega productos.");
        return (tienda.Value, cliente.Value, metodo.Value);
    }

    internal static void Total(decimal total, bool efectivo, decimal recibido)
    {
        if (total <= 0 || total > 9999999999999999.99m) throw new ArgumentException("El total de la venta está fuera del rango permitido.");
        if (efectivo && recibido < total) throw new ArgumentException("El monto recibido es insuficiente.");
    }
    internal static void Validar(int tienda, int cliente, int metodo, decimal recibido, string referencia, string observaciones, IReadOnlyCollection<LineaVenta> lineas)
    {
        if (tienda <= 0 || cliente <= 0 || metodo <= 0 || lineas.Count == 0
                    || lineas.Any(l => l.Producto <= 0 || l.Cantidad <= 0 || l.Precio <= 0)
                    || lineas.Select(l => l.Producto).Distinct().Count() != lineas.Count)
            throw new ArgumentException("Selecciona tienda, cliente, método de pago y productos con cantidades positivas.");
        if (referencia.Length > 100 || observaciones.Length > 500 || recibido < 0)
            throw new ArgumentException("Revisa el monto, la referencia y las observaciones.");
        ValidacionDatos.NoSoloCeros(observaciones, "Observaciones");

    }
}
