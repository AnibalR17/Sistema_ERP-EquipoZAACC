namespace Sistema_ERP;

internal static class RegistrarAbonoApartadoValidacion
{
    internal static void Validar(int metodo, decimal monto, decimal saldo, bool efectivo, string referencia)
    {
        if (metodo <= 0 || monto <= 0) throw new ErrorCampoValidacion("Monto", "Selecciona método de pago y un monto positivo.");
        if (saldo > 0 && !efectivo && string.IsNullOrWhiteSpace(referencia))
            throw new ErrorCampoValidacion("Referencia", "Introduce la referencia del pago.");
    }
}
