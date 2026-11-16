using Sistema_ERP.Clases;

namespace Sistema_ERP;

internal static class GuardarClienteValidacion
{

    internal static void Validar(Cliente cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.Nombre))
            throw new ArgumentException("El nombre del cliente es obligatorio.", nameof(cliente.Nombre));

        cliente.Nombre = cliente.Nombre.Trim();
        cliente.Telefono = string.IsNullOrWhiteSpace(cliente.Telefono) ? null : cliente.Telefono.Trim();
        cliente.Correo = string.IsNullOrWhiteSpace(cliente.Correo) ? null : cliente.Correo.Trim();
        ValidacionDatos.NoSoloCeros(cliente.Nombre, "Nombre del cliente");
        ValidacionDatos.NoSoloCeros(cliente.Telefono, "Teléfono");
        ValidacionDatos.NoSoloCeros(cliente.Correo, "Correo");
        ValidacionDatos.Nombre(cliente.Nombre, "Nombre del cliente");
        ValidacionDatos.Correo(cliente.Correo, maximo: 150);
    }
}
