namespace Sistema_ERP;

internal static class Modulos
{
    internal const string Ventas = "VENTAS", Inventario = "INVENTARIO", Compras = "COMPRAS",
        Proveedores = "PROVEEDORES", Traspasos = "TRASPASOS", Validacion = "VALIDACION_TRASPASOS",
        CargaMasiva = "CARGA_MASIVA", Productos = "PRODUCTOS", Apartados = "APARTADOS_DEVOLUCIONES",
        Directorio = "DIRECTORIO", Seguridad = "SEGURIDAD", Configuracion = "CONFIGURACION";
}

internal sealed class PermisosUsuario(bool administrador, IEnumerable<string> codigos)
{
    private readonly HashSet<string> permisos = new(codigos, StringComparer.OrdinalIgnoreCase);

    internal bool Puede(string modulo, bool gestionar = false) => administrador ||
        (permisos.Contains(modulo + ".CONSULTAR") && (!gestionar || permisos.Contains(modulo + ".GESTIONAR")));

    internal bool PuedeAlguno(params string[] modulos) => modulos.Any(m => Puede(m));
}
