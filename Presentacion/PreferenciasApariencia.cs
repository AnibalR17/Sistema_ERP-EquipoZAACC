using System.Text.Json;

namespace Sistema_ERP;

internal enum TemaVisual { Claro, Oscuro }

internal static class PreferenciasApariencia
{
    internal static string Ruta => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Sistema_ERP", "apariencia.json");

    internal static TemaVisual Cargar(string ruta)
    {
        try
        {
            using var datos = JsonDocument.Parse(File.ReadAllText(ruta));
            return datos.RootElement.ValueKind == JsonValueKind.Object
                && datos.RootElement.TryGetProperty("tema", out var tema)
                && tema.ValueKind == JsonValueKind.String && tema.GetString() == "oscuro"
                ? TemaVisual.Oscuro : TemaVisual.Claro;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        { return TemaVisual.Claro; }
    }

    internal static void Guardar(string ruta, TemaVisual tema)
    {
        if (!Enum.IsDefined(tema)) throw new ArgumentOutOfRangeException(nameof(tema));
        string destino = Path.GetFullPath(ruta);
        Directory.CreateDirectory(Path.GetDirectoryName(destino)!);
        string temporal = destino + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllText(temporal, JsonSerializer.Serialize(new { tema = tema == TemaVisual.Oscuro ? "oscuro" : "claro" }));
            File.Move(temporal, destino, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporal)) File.Delete(temporal);
        }
    }
}
