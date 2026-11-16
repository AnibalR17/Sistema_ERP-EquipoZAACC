namespace Sistema_ERP;

internal static class GuardarRegistroSeguridadValidacion
{
    internal static void Resultado(int? id, int resultado)
    {
        if (id.HasValue && resultado != 1) throw new InvalidOperationException("El registro cambió o fue desactivado. Recarga la pestaña antes de continuar.");
    }
}
