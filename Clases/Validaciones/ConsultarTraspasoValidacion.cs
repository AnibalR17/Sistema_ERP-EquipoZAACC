using System.Data;

namespace Sistema_ERP;

internal static class ConsultarTraspasoValidacion
{
    internal static DataRowView Seleccion(DataRowView? fila) => fila ?? throw new InvalidOperationException("Seleccione una solicitud.");
}
