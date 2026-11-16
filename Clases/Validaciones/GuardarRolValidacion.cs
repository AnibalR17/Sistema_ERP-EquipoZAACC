namespace Sistema_ERP;

internal static class GuardarRolValidacion
{
    internal static void Validar(string codigo, string nombre, string descripcion)
    {
        ValidacionDatos.Texto(codigo, 30, "El primer campo", true, true);
        ValidacionDatos.Texto(nombre, 80, "El segundo campo", true);
        ValidacionDatos.Texto(descripcion, 300, "La descripción", false);
        ValidacionDatos.Nombre(nombre, "Nombre del rol");
    }
}
