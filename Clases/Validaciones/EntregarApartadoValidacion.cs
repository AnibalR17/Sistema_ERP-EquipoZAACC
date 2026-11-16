namespace Sistema_ERP;

internal static class EntregarApartadoValidacion
{
    internal static void Captura(int metodo, decimal monto, decimal saldo, bool efectivo, string referencia, decimal recibido)
    {
        if (saldo > 0)
        {
            RegistrarAbonoApartadoValidacion.Validar(metodo, monto, saldo, efectivo, referencia);
            if (efectivo && recibido < saldo) throw new ArgumentException("El monto recibido es insuficiente.");
        }
    }

    internal static void Importe(bool efectivo, decimal recibido, decimal saldo)
    {
        if (efectivo && (recibido < saldo || decimal.Round(recibido, 2) != recibido))
            throw new ArgumentException("El monto recibido no cubre el saldo.");
    }
}
