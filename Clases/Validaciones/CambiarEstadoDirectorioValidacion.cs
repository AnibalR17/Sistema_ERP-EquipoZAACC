namespace Sistema_ERP;

internal static class CambiarEstadoDirectorioValidacion
{
    internal static void Validar(int id, bool activar, string motivo)
    {
        ValidacionDatos.IdPositivo(id);
        if (!activar && (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length > 300))
            throw new ArgumentException("Introduce un motivo de entre 1 y 300 caracteres.");
        if (!activar) ValidacionDatos.NoSoloCeros(motivo, "Motivo");
    }
}
