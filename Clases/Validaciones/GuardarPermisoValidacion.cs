namespace Sistema_ERP;

internal static class GuardarPermisoValidacion
{
    internal static void Validar(string modulo, string accion, string descripcion)
    {
        ValidacionDatos.Texto(modulo, 50, "El primer campo", true, true);
        ValidacionDatos.Texto(accion, 30, "El segundo campo", true, true);
        ValidacionDatos.Texto(descripcion, 300, "La descripción", true);
    }

    internal static void Captura(string modulo, string accion, string descripcion)
    {
        if (string.IsNullOrWhiteSpace(modulo)) throw new ArgumentException("Selecciona o escribe el módulo del permiso.");
        if (string.IsNullOrWhiteSpace(accion)) throw new ArgumentException("Selecciona o escribe la acción del permiso.");
        if (string.IsNullOrWhiteSpace(descripcion)) throw new ArgumentException("Escribe la descripción del permiso.");
    }
}
