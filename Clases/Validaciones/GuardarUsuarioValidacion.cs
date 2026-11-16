namespace Sistema_ERP;

internal static class GuardarUsuarioValidacion
{
    internal static void EmpleadoActivo(bool activo)
    {
        if (!activo) throw new ArgumentException("Selecciona un empleado activo para una nueva asignación.");
    }
    internal static bool Validar(int? id, string usuario, int? empleado, string clave)
    {
        ValidacionDatos.Texto(usuario, 50, "El primer campo", true);
        ValidacionDatos.Nombre(usuario, "Usuario");
        if (empleado is null or <= 0) throw new ArgumentException("Selecciona un empleado.");
        bool requiereClave = !id.HasValue || clave.Length > 0;
        if (requiereClave) CrearClaveValidacion.Validar(clave);
        return requiereClave;
    }

    internal static void Confirmacion(string clave, string confirmacion)
    {
        if (clave != confirmacion) throw new ArgumentException("Las contraseñas no coinciden.");
    }
}
