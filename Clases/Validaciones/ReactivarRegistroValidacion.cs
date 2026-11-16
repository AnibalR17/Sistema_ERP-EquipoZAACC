namespace Sistema_ERP;

internal static class ReactivarRegistroValidacion
{
    internal static void Validar(int id, string parametro = "id") => ValidacionDatos.IdPositivo(id, parametro);
}
