namespace Sistema_ERP;

internal static class GuardarCategoriaValidacion
{
    internal static void NombreConsultable(string nombre) => ValidacionDatos.Nombre(nombre, "Nombre de la categoría");
    internal static void NombreDisponible(bool existe)
    {
        if (existe) throw new ErrorCampoValidacion("Nombre", "Ya existe una categoría con ese nombre, posiblemente inactiva.");
    }
    internal static void Validar(string nombre, string? descripcion, int? id = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
        ValidacionDatos.Nombre(nombre, "Nombre de la categoría");
        ValidacionDatos.NoSoloCeros(descripcion, "Descripción");
        if (id is <= 0) throw new ArgumentOutOfRangeException("idCategoria");
        if (nombre.Trim().Length > 100) throw new ArgumentException("El nombre admite hasta 100 caracteres.", nameof(nombre));
        if (descripcion?.Trim().Length > 300) throw new ArgumentException("La descripción admite hasta 300 caracteres.", nameof(descripcion));
    }

    internal static void Captura(string nombre, string descripcion)
    {
        if (string.IsNullOrWhiteSpace(nombre)) throw new ErrorCampoValidacion("Nombre", "Introduzca el nombre de la categoría.");
        Validar(nombre, descripcion);
    }
}
