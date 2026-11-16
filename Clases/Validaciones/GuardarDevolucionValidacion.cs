using System.Data;

namespace Sistema_ERP;

internal static class GuardarDevolucionValidacion
{
    internal static void VentaConfirmada(bool confirmada)
    {
        if (!confirmada) throw new InvalidOperationException("Selecciona una venta confirmada.");
    }

    internal static void Editable(string? estado, int original, int venta)
    {
        if (estado != "BORRADOR" || original != venta) throw new InvalidOperationException("Solo puedes editar el borrador y no puedes cambiar su factura.");
    }
    internal static ArgumentException ErrorCantidad() => new("La cantidad a devolver debe ser un número entero.");
    internal static bool CantidadValida(string? texto) => int.TryParse(texto, out int n) && n >= 0;

    internal static void EdicionTerminada(bool edicionTerminada)
    {
        if (!edicionTerminada) throw new ArgumentException("Corrige la cantidad a devolver antes de guardar.");
    }

    internal static (int Venta, LineaDevolucion[] Lineas) Captura(int? venta, DataTable lineas)
    {
        if (venta is null) throw new ArgumentException("Selecciona una factura confirmada.");
        var seleccionadas = new List<LineaDevolucion>();
        foreach (DataRow linea in lineas.Rows)
        {
            int cantidad = Convert.ToInt32(linea["Devolver"]);
            if (cantidad < 0 || cantidad > (int)linea["Disponible"])
                throw new ArgumentException("La cantidad a devolver debe estar entre cero y la cantidad disponible.");
            if (cantidad > 0) seleccionadas.Add(new((int)linea["Id"], cantidad, (bool)linea["Reintegrar"]));
        }
        return (venta.Value, seleccionadas.ToArray());
    }

    internal static void Validar(IReadOnlyCollection<LineaDevolucion> lineas)
    {
        if (lineas.Count == 0 || lineas.Any(l => l.DetalleVenta <= 0 || l.Cantidad <= 0)
            || lineas.Select(l => l.DetalleVenta).Distinct().Count() != lineas.Count)
            throw new ArgumentException("Selecciona al menos un producto y una cantidad positiva a devolver, sin repetir líneas.");
    }
}
