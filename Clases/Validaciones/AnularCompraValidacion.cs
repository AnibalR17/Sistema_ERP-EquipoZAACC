namespace Sistema_ERP;

internal static class AnularCompraValidacion
{
    internal static void Estado(string estado)
    {
        if (estado is not ("BORRADOR" or "CONFIRMADA")) throw new InvalidOperationException("La compra ya está anulada.");
    }
    internal static void Motivo(string motivo)
    {
        if (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length > 500)
            throw new ArgumentException("Introduce un motivo de anulación de hasta 500 caracteres.");
        ValidacionDatos.NoSoloCeros(motivo, "Motivo");
    }
}
