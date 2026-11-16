
namespace Sistema_ERP;

internal static class RegistrarImporteValidacion
{

    internal static void Validar(decimal importe)
    {
        if (importe <= 0 || importe > 9999999999999999.99m || decimal.Round(importe, 2) != importe)
            throw new ArgumentException("El importe debe ser positivo, con hasta dos decimales y dentro del rango permitido.");
    }
}
