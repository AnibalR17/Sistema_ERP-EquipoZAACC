using System.Globalization;

namespace Sistema_ERP;

internal static class ImporteMonetario
{
    // Los textos no admiten miles: 1,000 es ambiguo y nunca debe convertirse en 1.
    internal static bool TryParse(string texto, out decimal valor)
    {
        valor = 0;
        string entrada = texto.Trim();
        int separador = entrada.IndexOfAny(['.', ',']);
        if (separador >= 0 && entrada.Length - separador - 1 is not (1 or 2)) return false;

        return decimal.TryParse(entrada.Replace(',', '.'),
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out valor);
    }
}
