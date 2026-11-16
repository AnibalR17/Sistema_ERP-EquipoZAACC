using System.Data;

namespace Sistema_ERP;

internal static class AgregarProductoApartadoValidacion
{
    internal static (DataRowView Producto, int Unidades) Validar(DataRowView? producto, int cantidad, int existentes)
    {
        if (producto is null) throw new ArgumentException("Selecciona un producto.");
        long unidades = (long)cantidad + existentes;
        if (unidades > (int)producto["Disponible"]) throw new ArgumentException("La cantidad supera el stock disponible.");
        return (producto, (int)unidades);
    }
}
