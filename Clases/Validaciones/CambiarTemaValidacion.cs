namespace Sistema_ERP;

internal static class CambiarTemaValidacion
{
    internal static void Validar(TemaVisual tema)
    {
        if (!Enum.IsDefined(tema)) throw new ArgumentOutOfRangeException(nameof(tema));
    }
}
