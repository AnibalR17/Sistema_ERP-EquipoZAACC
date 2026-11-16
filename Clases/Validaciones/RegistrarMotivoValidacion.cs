
namespace Sistema_ERP;

internal static class RegistrarMotivoValidacion
{
    internal static bool CapturaValida(string motivo) => !string.IsNullOrWhiteSpace(motivo);

    internal static string Validar(string motivo)
    {
        motivo = motivo.Trim();
        if (motivo.Length is < 1 or > 500) throw new ArgumentException("Introduce un motivo de hasta 500 caracteres.");
        ValidacionDatos.NoSoloCeros(motivo, "Motivo");
        return motivo;
    }
}
