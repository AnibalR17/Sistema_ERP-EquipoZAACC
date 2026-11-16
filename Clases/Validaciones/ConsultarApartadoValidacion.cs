using System.Data;

namespace Sistema_ERP;

internal static class ConsultarApartadoValidacion
{
    internal static void Existe(int filas)
    {
        if (filas != 1) throw new InvalidOperationException("El apartado ya no existe.");
    }

    internal static DataRow VersionActual(DataTable datos, byte[] version, bool activo)
    {
        if (datos.Rows.Count != 1 || !((byte[])datos.Rows[0]["version_fila"]).SequenceEqual(version))
            throw new InvalidOperationException("El apartado cambió. Actualiza la lista antes de continuar.");
        var fila = datos.Rows[0];
        if (activo && Convert.ToString(fila["estado"]) != "ACTIVO") throw new InvalidOperationException("El apartado ya no está activo.");
        return fila;
    }
}
