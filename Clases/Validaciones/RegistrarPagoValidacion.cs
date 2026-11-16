namespace Sistema_ERP;

internal static class RegistrarPagoValidacion
{
    internal static void LongitudReferencia(string referencia)
    {
        if (referencia.Length > 100) throw new ArgumentException("La referencia admite hasta 100 caracteres.");
    }

    internal static void ReferenciaObligatoria(bool efectivo, string referencia)
    {
        if (!efectivo && string.IsNullOrWhiteSpace(referencia)) throw new ArgumentException("Introduce la referencia del pago.");
    }

    internal static void MetodoActivo(bool activo)
    {
        if (!activo) throw new ArgumentException("Selecciona un método de pago activo.");
    }

    internal static void ReferenciaManual(string referencia) => ValidacionDatos.NoSoloCeros(referencia, "Referencia del pago");
}
