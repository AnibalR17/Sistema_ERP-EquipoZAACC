using Sistema_ERP.Clases;

namespace Sistema_ERP;

internal static class GuardarProductoValidacion
{
    internal static void CodigoConsultable(string codigo) => ValidacionDatos.NoSoloCeros(codigo, "Código de barras");
    internal static (int Categoria, int Proveedor) Captura(string codigo, string nombre, decimal precio, int? categoria, int? proveedor)
    {
        if (string.IsNullOrWhiteSpace(codigo)) throw new ErrorCampoValidacion("CodigoBarras", "El codigo de barras es obligatorio.");
        if (string.IsNullOrWhiteSpace(nombre)) throw new ErrorCampoValidacion("Nombre", "El nombre es obligatorio.");
        if (ValidacionDatos.SoloCeros(codigo)) throw new ErrorCampoValidacion("CodigoBarras", "El código de barras no puede contener solo ceros.");
        if (ValidacionDatos.SoloCeros(nombre)) throw new ErrorCampoValidacion("Nombre", "El nombre no puede contener solo ceros.");
        if (!ValidacionDatos.NombreValido(nombre)) throw new ErrorCampoValidacion("Nombre", "El nombre del producto debe contener al menos 3 letras.");
        if (precio <= 0) throw new ErrorCampoValidacion("PrecioUnitario", "El precio unitario debe ser un numero mayor a cero.");
        if (categoria is null) throw new ErrorCampoValidacion("IdCategoria", "Seleccione una categoria.");
        if (proveedor is null) throw new ErrorCampoValidacion("IdProveedor", "Seleccione un proveedor.");
        return (categoria.Value, proveedor.Value);
    }

    internal static void CodigoDisponible(bool existe)
    {
        if (existe) throw new ErrorCampoValidacion("CodigoBarras", "Ya existe un producto con ese codigo de barras.");
    }

    internal static void Validar(Producto producto)
    {
        ValidacionDatos.NoSoloCeros(producto.CodigoBarras, "Código de barras");
        ValidacionDatos.Nombre(producto.Nombre, "Nombre del producto");
    }
}
