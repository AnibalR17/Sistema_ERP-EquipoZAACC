namespace Sistema_ERP;

internal static class CrearClaveValidacion
{
    internal static void Validar(string clave)
    {
        if (clave.Length is < 12 or > 128) throw new ArgumentException("La contraseña debe tener entre 12 y 128 caracteres.");
        ValidacionDatos.NoSoloCeros(clave, "Contraseña");
    }
}
