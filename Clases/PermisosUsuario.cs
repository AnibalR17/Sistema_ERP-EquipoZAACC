namespace Sistema_ERP;

internal static class Modulos
{
    internal const string ventas = "VENTAS", inventario = "INVENTARIO", compras = "COMPRAS",
        proveedores = "PROVEEDORES", traspasos = "TRASPASOS", validacion = "VALIDACION_TRASPASOS",
        cargaMasiva = "CARGA_MASIVA", productos = "PRODUCTOS", apartados = "APARTADOS_DEVOLUCIONES",
        directorio = "DIRECTORIO", seguridad = "SEGURIDAD", configuracion = "CONFIGURACION";
}

internal sealed class PermisosUsuario(bool administrador, IEnumerable<string> codigos)
{
    private readonly HashSet<string> permisos = new(codigos, StringComparer.OrdinalIgnoreCase);

    internal bool Puede(string modulo, bool gestionar = false) => administrador ||
        (permisos.Contains(modulo + ".CONSULTAR") && (!gestionar || permisos.Contains(modulo + ".GESTIONAR")));

    internal bool PuedeAlguno(params string[] modulos) => modulos.Any(m => Puede(m));
}
