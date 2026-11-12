using System.Globalization;
using System.Net.Mail;

namespace Sistema_ERP;

internal static class ValidacionDatos
{
    internal static bool SoloCeros(string? valor)
    {
        bool contieneCero = false;
        foreach (char caracter in valor ?? "")
        {
            if (char.IsDigit(caracter) && char.GetNumericValue(caracter) == 0)
                contieneCero = true;
            else if (!char.IsWhiteSpace(caracter) && caracter is not ('-' or '+' or '(' or ')' or '.' or ',' or '/' or '\\'))
                return false;
        }
        return contieneCero;
    }

    internal static void NoSoloCeros(string? valor, string campo)
    {
        if (SoloCeros(valor)) throw new ArgumentException($"El campo «{campo}» no puede contener solo ceros.");
    }

    internal static bool NombreValido(string? valor, bool opcional = false) =>
        (opcional && string.IsNullOrWhiteSpace(valor)) || (valor?.Count(char.IsLetter) ?? 0) >= 3;

    internal static void Nombre(string? valor, string campo, bool opcional = false)
    {
        NoSoloCeros(valor, campo);
        if (!NombreValido(valor, opcional))
            throw new ArgumentException($"El campo «{campo}» debe contener al menos 3 letras.");
    }

    internal static bool CorreoValido(string? valor, int maximo = 254)
    {
        if (string.IsNullOrWhiteSpace(valor)) return true;
        string texto = valor.Trim();
        if (texto.Length > maximo || texto.Any(c => char.IsWhiteSpace(c) || char.IsControl(c))
            || !MailAddress.TryCreate(texto, out var correo) || correo.Address != texto
            || correo.User.Length > 64 || correo.User.StartsWith('.') || correo.User.EndsWith('.') || correo.User.Contains(".."))
            return false;
        try
        {
            string dominio = new IdnMapping { UseStd3AsciiRules = true }.GetAscii(correo.Host);
            string[] partes = dominio.Split('.');
            return dominio.Length <= 253 && partes.Length >= 2
                && partes.All(p => p.Length is >= 1 and <= 63 && p[0] != '-' && p[^1] != '-'
                    && p.All(c => char.IsAsciiLetterOrDigit(c) || c == '-'))
                && partes[^1].Length >= 2 && partes[^1].Any(char.IsAsciiLetter);
        }
        catch (ArgumentException) { return false; }
    }

    internal static void Correo(string? valor, string campo = "Correo", int maximo = 254)
    {
        NoSoloCeros(valor, campo);
        if (!CorreoValido(valor, maximo))
            throw new ArgumentException($"El campo «{campo}» debe contener un correo válido, como usuario@dominio.com, de hasta {maximo} caracteres.");
    }
}
