namespace Sistema_ERP;

internal static class DesactivarRegistroValidacion
{
    internal static string MotivoCapturado(string texto)
    {
        string motivo = texto.Trim();
        if (motivo.Length == 0) throw new ArgumentException("Introduce el motivo para continuar.");
        if (ValidacionDatos.SoloCeros(motivo)) throw new ArgumentException("El motivo no puede contener solo ceros.");
        return motivo;
    }

    internal static void Validar(int id, string motivo)
    {
        ValidacionDatos.IdPositivo(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(motivo);
        ValidacionDatos.NoSoloCeros(motivo, "Motivo");
        if (motivo.Trim().Length > 300) throw new ArgumentException("El motivo admite hasta 300 caracteres.", nameof(motivo));
    }

    internal static void Cliente(string motivo)
    {
        if (string.IsNullOrWhiteSpace(motivo)) throw new ArgumentException("El motivo de desactivacion es obligatorio.", nameof(motivo));
        ValidacionDatos.NoSoloCeros(motivo, "Motivo");
    }
}
