using Sistema_ERP.Clases;

namespace Sistema_ERP;

internal static class GuardarDirectorioValidacion
{
    internal static void Edicion(int id) => ValidacionDatos.IdPositivo(id);
    internal static void Validar(TipoDirectorio tipo, string nombre, string telefono, string correo, string identificador, string direccion, string contacto, int? tienda)
    {
        if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Introduce el nombre.");
        foreach (var (valor, campo) in new[]
        {
                (nombre, "Nombre"), (telefono, "Teléfono"), (correo, "Correo"),
                (identificador, "Identificador"), (direccion, "Dirección"), (contacto, "Contacto")
            }) ValidacionDatos.NoSoloCeros(valor, campo);
        if (nombre.Trim().Length > 150 || telefono.Trim().Length > 30 || correo.Trim().Length > 254 ||
            identificador.Trim().Length > 30 || direccion.Trim().Length > 300 || contacto.Trim().Length > 150)
            throw new ArgumentException("Uno de los campos supera la longitud permitida.");
        ValidacionDatos.Nombre(nombre, "Nombre");
        if (tipo == TipoDirectorio.Proveedores) ValidacionDatos.Nombre(contacto, "Contacto", opcional: true);
        ValidacionDatos.Correo(correo);
        if (tipo == TipoDirectorio.Proveedores)
        {
            var proveedor = new Proveedor
            {
                NombreEmpresa = nombre,
                Telefono = telefono,
                Correo = correo,
                Rtn = identificador,
                Direccion = direccion,
                Contacto = contacto
            };
            GuardarProveedorValidacion.Validar(proveedor);
        }
        if (tipo == TipoDirectorio.Empleados && (string.IsNullOrWhiteSpace(identificador) || tienda is null || tienda <= 0))
            throw new ArgumentException("Introduce el codigo y selecciona la tienda del empleado.");
        if (tipo == TipoDirectorio.Clientes && identificador.Any(c => c > 127))
            throw new ArgumentException("El documento fiscal debe contener caracteres ASCII.");


    }
}
