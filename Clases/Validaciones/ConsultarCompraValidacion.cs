namespace Sistema_ERP;

internal static class ConsultarCompraValidacion
{
    internal static void Existe(int filas)
    {
        if (filas != 1) throw new InvalidOperationException("La compra ya no existe. Recarga el listado.");
    }

    internal static void VersionActual(bool existe, byte[]? actual, byte[] esperada)
    {
        if (!existe || actual is null || !actual.SequenceEqual(esperada))
            throw new InvalidOperationException("La compra cambió en otra ventana. Recárgala antes de guardar o cambiar su estado.");
    }
    internal static void Seleccion(int id)
    {
        if (id <= 0) throw new ArgumentException("Selecciona una compra.");
    }

    internal static void Version(int id, byte[]? version)
    {
        if (id <= 0 || version is not { Length: 32 }) throw new ArgumentException("Selecciona una compra y recárgala antes de operar.");
    }
}
