
namespace Sistema_ERP;

internal static class GuardarTiendaValidacion
{
    internal static void Edicion(int id) => ValidacionDatos.IdPositivo(id, "idTienda");

    internal static void Validar(string codigo, string nombre, string direccion, string telefono)
    {
        if (string.IsNullOrWhiteSpace(codigo)) throw new ArgumentException("Introduce el codigo de la tienda.");
        if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Introduce el nombre de la tienda.");
        foreach (var (valor, campo) in new[]
        {
                (codigo, "Código de la tienda"), (nombre, "Nombre de la tienda"),
                (direccion, "Dirección"), (telefono, "Teléfono")
            }) ValidacionDatos.NoSoloCeros(valor, campo);
        ValidacionDatos.Nombre(nombre, "Nombre de la tienda");
        if (codigo.Trim().Length > 20 || nombre.Trim().Length > 100 || direccion.Trim().Length > 300 || telefono.Trim().Length > 30)
            throw new ArgumentException("Uno de los campos supera la longitud permitida.");
        if (codigo.Any(c => c > 127)) throw new ArgumentException("El codigo debe contener caracteres ASCII.");
    }
}
