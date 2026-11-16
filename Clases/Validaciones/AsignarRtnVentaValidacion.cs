
namespace Sistema_ERP;

internal static class AsignarRtnVentaValidacion
{

    internal static string? Normalizar(string? rtn)
    {
        string valor = new((rtn ?? "").Where(c => c != '-' && !char.IsWhiteSpace(c)).ToArray());
        if (valor.Length == 0) return null;
        ValidacionDatos.NoSoloCeros(valor, "RTN");
        if (valor.Length != 14 || valor.Any(c => c is < '0' or > '9'))
            throw new ArgumentException("El RTN es opcional. Si lo ingresas, debe contener 14 dígitos; se permiten espacios y guiones.");
        return valor;
    }
}
