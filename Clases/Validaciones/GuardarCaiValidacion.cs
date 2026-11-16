using System.Data;
using System.Text.RegularExpressions;

namespace Sistema_ERP;

internal static class GuardarCaiValidacion
{
    internal static void VersionActual(DataTable anterior, byte[]? version)
    {
        if ((anterior.Rows.Count == 0 && version != null) || (anterior.Rows.Count == 1 && (version is null || !((byte[])anterior.Rows[0][0]).SequenceEqual(version))))
            throw new InvalidOperationException("El CAI cambió en otra ventana. Actualiza la configuración antes de guardar.");
    }
    internal static int Tienda(int? tienda) => tienda ?? throw new ArgumentException("Selecciona una tienda.");

    internal static string? Normalizar(string? cai)
    {
        string valor = Regex.Replace(cai ?? "", @"\s", "").ToUpperInvariant();
        if (valor.Length == 0) return null;
        ValidacionDatos.NoSoloCeros(valor, "CAI");
        if (Regex.IsMatch(valor, "^[A-Z0-9]{32}$")) valor = string.Join("-", Enumerable.Range(0, 5).Select(i => valor.Substring(i * 6, 6)).Append(valor[30..]));
        if (!Regex.IsMatch(valor, "^[A-Z0-9]{6}(?:-[A-Z0-9]{6}){4}-[A-Z0-9]{2}$"))
            throw new ArgumentException("Introduce el CAI autorizado con formato XXXXXX-XXXXXX-XXXXXX-XXXXXX-XXXXXX-XX (37 caracteres).");
        return valor;
    }
}
