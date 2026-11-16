namespace Sistema_ERP;

internal static class CambiarClaveValidacion
{
    internal static void CuentaActualizada(int filas)
    {
        if (filas != 1) throw new InvalidOperationException("La cuenta cambió. Cierra esta ventana e inicia sesión de nuevo.");
    }
    internal static void Confirmacion(string clave, string confirmacion)
    {
        if (ValidacionDatos.SoloCeros(clave)) throw new ArgumentException("La contraseña no puede contener solo ceros.");
        if (clave != confirmacion) throw new ArgumentException("Las contraseñas no coinciden.");
    }

    internal static void Validar(bool pendiente, string anterior, string nueva)
    {
        if (!pendiente) throw new InvalidOperationException("No hay un cambio de contraseña pendiente.");
        if (anterior == nueva) throw new ArgumentException("La contraseña nueva debe ser diferente de la anterior.");
    }
}
