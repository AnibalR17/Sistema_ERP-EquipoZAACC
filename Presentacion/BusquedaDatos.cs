using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Sistema_ERP;

internal static class BusquedaDatos
{
    internal static string Expresion(DataTable tabla, string texto, IEnumerable<string>? campos = null)
    {
        texto = texto.Trim();
        if (texto.Length == 0) return "";
        string literal = Regex.Replace(texto, @"[%*\[\]]", "[$0]").Replace("'", "''");
        var permitidos = campos?.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var condiciones = new List<string>();
        foreach (DataColumn columna in tabla.Columns)
        {
            if (permitidos is not null && !permitidos.Contains(columna.ColumnName)) continue;
            Type tipo = columna.DataType;
            if (tipo != typeof(string) && tipo != typeof(DateTime) && tipo != typeof(bool)
                && tipo != typeof(int) && tipo != typeof(long) && tipo != typeof(short)
                && tipo != typeof(decimal) && tipo != typeof(double) && tipo != typeof(float)) continue;
            string campo = "[" + columna.ColumnName.Replace("\\", "\\\\").Replace("]", "\\]") + "]";
            condiciones.Add($"CONVERT({campo}, 'System.String') LIKE '%{literal}%'");
            if (tipo == typeof(DateTime) && DateTime.TryParseExact(texto, ["d/M/yyyy", "dd/MM/yyyy", "yyyy-MM-dd"],
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha) && fecha.Date < DateTime.MaxValue.Date)
                condiciones.Add($"({campo} >= #{fecha:MM/dd/yyyy}# AND {campo} < #{fecha.AddDays(1):MM/dd/yyyy}#)");
            if (tipo == typeof(bool))
            {
                if (texto.Equals("sí", StringComparison.OrdinalIgnoreCase) || texto.Equals("si", StringComparison.OrdinalIgnoreCase)) condiciones.Add($"{campo} = true");
                if (texto.Equals("no", StringComparison.OrdinalIgnoreCase)) condiciones.Add($"{campo} = false");
            }
        }
        return condiciones.Count == 0 ? "1 = 0" : "(" + string.Join(" OR ", condiciones) + ")";
    }

    internal static void Filtrar(DataView vista, string texto, DataGridView tabla)
        => vista.RowFilter = Expresion(vista.Table!, texto, Campos(tabla));

    internal static IEnumerable<string> Campos(DataGridView tabla) => tabla.Columns.Cast<DataGridViewColumn>()
        .Where(c => c.Visible && !string.IsNullOrEmpty(c.DataPropertyName)).Select(c => c.DataPropertyName);
}
