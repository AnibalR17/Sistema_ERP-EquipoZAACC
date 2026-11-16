using System.Data;

namespace Sistema_ERP;

internal static class GuardarTraspasoValidacion
{
    internal static void Edicion(int id, byte[]? version)
    {
        ValidacionDatos.IdPositivo(id);
        ArgumentNullException.ThrowIfNull(version);
    }

    internal static void UsuarioActivo(bool activo)
    {
        if (!activo) throw new ArgumentException("Seleccione un usuario activo.");
    }
    internal static bool FilaVacia(DataRow fila) => fila.Table.Columns.Cast<DataColumn>().All(c => fila.IsNull(c) || fila[c] is string texto && string.IsNullOrWhiteSpace(texto));
    internal static void Validar(int origen, int destino, string observaciones, IReadOnlyCollection<LineaTraspaso> lineas)
    {
        if (origen <= 0 || destino <= 0 || origen == destino)
            throw new ArgumentException("Seleccione dos tiendas distintas.");
        if (observaciones.Length > 500)
            throw new ArgumentException("Las observaciones admiten hasta 500 caracteres.");
        ValidacionDatos.NoSoloCeros(observaciones, "Observaciones");
        if (lineas.Count == 0 || lineas.Any(l => l.Producto <= 0 || l.Cantidad <= 0 || l.Precio < 0 || l.Precio > 9999999999999999.99m)
            || lineas.Select(l => l.Producto).Distinct().Count() != lineas.Count)
            throw new ArgumentException("Agregue productos sin repetir, con cantidades enteras positivas y precios válidos.");

    }
}
