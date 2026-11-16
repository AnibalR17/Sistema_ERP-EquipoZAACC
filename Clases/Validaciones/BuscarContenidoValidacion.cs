namespace Sistema_ERP;

internal static class BuscarContenidoValidacion
{
    internal static void Validar(string texto, int? tienda)
    {
        if (texto.Length > 150) throw new ArgumentException("La búsqueda admite hasta 150 caracteres.");
        if (tienda is <= 0) throw new ArgumentException("Selecciona una tienda válida.");
    }

    internal static void Detalle(int id)
    {
        if (id <= 0) throw new ArgumentException("Selecciona un registro para ver su detalle.");
    }
}
