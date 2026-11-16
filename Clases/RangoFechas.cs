using System.Globalization;

namespace Sistema_ERP;

internal readonly record struct RangoFechas
{
    internal DateTime? Desde { get; }
    internal DateTime? HastaExclusivo { get; }

    internal RangoFechas(DateTime? desde, DateTime? hasta)
    {
        ConsultarHistorialValidacion.Fechas(desde, hasta);
        Desde = desde?.Date;
        HastaExclusivo = hasta?.Date.AddDays(1);
    }

    internal string Expresion(string campo)
    {
        string columna = "[" + campo.Replace("\\", "\\\\").Replace("]", "\\]") + "]";
        var filtros = new List<string>();
        if (Desde is { } desde) filtros.Add($"{columna} >= #{desde.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}#");
        if (HastaExclusivo is { } hasta) filtros.Add($"{columna} < #{hasta.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}#");
        return string.Join(" AND ", filtros);
    }
}
