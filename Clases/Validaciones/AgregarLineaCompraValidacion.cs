using System.Data;
using System.Globalization;

namespace Sistema_ERP;

internal static class AgregarLineaCompraValidacion
{
    private const decimal maximoImporte = 9999999999999999.99m;

    internal static decimal Costo(string texto)
    {
        if (!ImporteMonetario.TryParse(texto, out decimal costo) || costo <= 0 || costo > maximoImporte || decimal.Round(costo, 2) != costo)
            throw new ArgumentException("Indica un costo positivo como 12.50 o 12,50, con hasta dos decimales, sin moneda ni separadores de miles.");
        return costo;
    }

    internal static (DataRowView Producto, int Cantidad, decimal Costo) Validar(DataRowView? producto, string unidades, string precio,
        DataTable carrito, DataRow? seleccionada, bool actualizar)
    {
        if (producto is null) throw new ArgumentException("Selecciona un producto activo.");
        if (!int.TryParse(unidades.Trim(), NumberStyles.None, CultureInfo.CurrentCulture, out int cantidad) || cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser un entero positivo.");
        decimal costo = Costo(precio), subtotal = cantidad * costo;
        if (subtotal > maximoImporte) throw new ArgumentException("El subtotal supera el importe permitido.");
        if (actualizar && seleccionada is null) throw new ArgumentException("Selecciona una línea para actualizarla.");
        int id = (int)producto["id_producto"];
        if (carrito.AsEnumerable().Any(f => f != seleccionada && f.Field<int>("id_producto") == id))
            throw new ArgumentException("El producto ya está en el detalle. Selecciona su línea y pulsa Actualizar línea.");
        decimal total = carrito.AsEnumerable().Where(f => f != seleccionada).Sum(f => f.Field<decimal>("Subtotal")) + subtotal;
        if (total > maximoImporte) throw new ArgumentException("El total supera el importe permitido.");
        return (producto, cantidad, costo);
    }
}
