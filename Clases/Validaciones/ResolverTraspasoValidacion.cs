namespace Sistema_ERP;

internal static class ResolverTraspasoValidacion
{
    internal static void Validar(int id, byte[]? version, string estado, string motivo)
    {
        ValidacionDatos.IdPositivo(id);
        ArgumentNullException.ThrowIfNull(version);
        if (estado is not ("CANCELADO" or "APROBADO" or "RECHAZADO" or "EN_TRANSITO" or "RECIBIDO")) throw new ArgumentException("Estado inválido.");
        if (motivo.Length > 500 || (estado is "CANCELADO" or "RECHAZADO" && string.IsNullOrWhiteSpace(motivo)))
            throw new ArgumentException("Indique un motivo de hasta 500 caracteres.");
        ValidacionDatos.NoSoloCeros(motivo, "Motivo");
    }

    internal static void Transicion(string anterior, string siguiente)
    {
        bool permitido = siguiente switch
        {
            "EN_TRANSITO" => anterior == "APROBADO",
            "RECIBIDO" => anterior == "EN_TRANSITO",
            "CANCELADO" => anterior is "PENDIENTE" or "APROBADO",
            _ => anterior == "PENDIENTE"
        };
        if (!permitido) throw new InvalidOperationException("El estado actual no permite esta operación.");
    }

    internal static void Pendiente(string estado)
    {
        if (estado != "PENDIENTE") throw new InvalidOperationException("Solo se pueden modificar solicitudes pendientes.");
    }
}
