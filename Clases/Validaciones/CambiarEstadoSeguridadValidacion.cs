namespace Sistema_ERP;

internal static class CambiarEstadoSeguridadValidacion
{
    internal static void Motivo(bool activar, string motivo)
    {
        if (!activar) ValidacionDatos.Texto(motivo.Trim(), 300, "El motivo", true);
    }

    internal static void Resultado(int filas)
    {
        if (filas != 1) throw new InvalidOperationException("El registro cambió. Recarga la pestaña antes de continuar.");
    }
}
