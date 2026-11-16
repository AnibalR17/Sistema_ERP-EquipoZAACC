using System.Data;

namespace Sistema_ERP;

internal static class ConsultarDevolucionValidacion
{
    internal static void Existe(int filas)
    {
        if (filas != 1) throw new InvalidOperationException("La devolución ya no existe.");
    }

    internal static DataRow VersionActual(DataTable datos, byte[] version)
    {
        if (datos.Rows.Count != 1 || !((byte[])datos.Rows[0]["version_fila"]).SequenceEqual(version))
            throw new InvalidOperationException("La devolución cambió. Actualiza la lista antes de continuar.");
        return datos.Rows[0];
    }
}
