namespace Sistema_ERP;

internal static class GuardarMinimoInventarioValidacion
{
    internal static void VersionActual(byte[]? actual, byte[]? version)
    {
        if ((actual is null) != (version is null) || (actual != null && !actual.SequenceEqual(version!)))
            throw new InvalidOperationException("El inventario cambió. Actualiza la pantalla antes de guardar.");
    }
    internal static void Validar(int tienda, int producto, int minimo)
    {
        if (tienda <= 0 || producto <= 0 || minimo < 0) throw new ArgumentException("Selecciona tienda y producto e indica un mínimo no negativo.");
    }
}
