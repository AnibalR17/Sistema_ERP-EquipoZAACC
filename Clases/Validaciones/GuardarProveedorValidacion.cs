using Sistema_ERP.Clases;

namespace Sistema_ERP;

internal static class GuardarProveedorValidacion
{
    internal static void Edicion(int id) => ValidacionDatos.IdPositivo(id, "IdProveedor");
    internal static void RtnDisponible(bool existe)
    {
        if (existe) throw new ErrorCampoValidacion("RTN", "Ya existe un proveedor con ese RTN.");
    }

    internal static void Validar(Proveedor proveedor)
    {
        if (string.IsNullOrWhiteSpace(proveedor.NombreEmpresa))
            throw new ArgumentException("Introduzca el nombre de la empresa.");
        foreach (var (valor, limite, campo) in new[]
        {
                (proveedor.NombreEmpresa, 150, "Nombre de empresa"), (proveedor.Rtn, 30, "RTN"),
                (proveedor.Contacto, 150, "Contacto"), (proveedor.Telefono, 30, "Teléfono"),
                (proveedor.Correo, 254, "Correo"), (proveedor.Direccion, 300, "Dirección")
            })
        {
            ValidacionDatos.NoSoloCeros(valor, campo);
            if (valor.Trim().Length > limite)
                throw new ArgumentException($"{campo} admite hasta {limite} caracteres.");
        }
        if (proveedor.Rtn.Any(c => c > 127))
            throw new ArgumentException("El RTN debe contener caracteres ASCII.");
        ValidacionDatos.Nombre(proveedor.NombreEmpresa, "Nombre de empresa");
        ValidacionDatos.Nombre(proveedor.Contacto, "Contacto", opcional: true);
        ValidacionDatos.Correo(proveedor.Correo);
    }
}
