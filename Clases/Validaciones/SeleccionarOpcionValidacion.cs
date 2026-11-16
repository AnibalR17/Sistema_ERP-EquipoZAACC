namespace Sistema_ERP;

internal static class SeleccionarOpcionValidacion
{
    internal static int Validar(string texto, IReadOnlyList<string> opciones, int actual)
    {
        bool Coincide(string opcion) => string.Equals(opcion, texto, StringComparison.CurrentCultureIgnoreCase);
        if (actual >= 0 && actual < opciones.Count && Coincide(opciones[actual])) return actual;
        for (int i = 0; i < opciones.Count; i++) if (Coincide(opciones[i])) return i;
        if (texto.Length == 0) return -1;
        throw new ArgumentException("Selecciona una opción existente de la lista. Puedes escribir para buscarla.");
    }
}
