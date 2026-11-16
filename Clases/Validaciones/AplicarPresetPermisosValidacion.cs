namespace Sistema_ERP;

internal static class AplicarPresetPermisosValidacion
{
    internal static (PermisoAsignable[] Elegidos, int Faltantes) Seleccionar(PlantillaPermisos plantilla, IEnumerable<PermisoAsignable> permisos)
    {
        var activos = permisos.Where(p => p.Activo).ToArray();
        var consultas = activos.Where(p => EsAccion(p, "CONSULTAR")).Select(p => p.Modulo).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var elegidos = activos.Where(p => plantilla.Incluye(p.Modulo, p.Accion) && (!EsAccion(p, "GESTIONAR") || consultas.Contains(p.Modulo))).ToArray();
        int faltantes = plantilla.Permisos.Count(p => !elegidos.Any(e => string.Equals(e.Modulo, p.Modulo, StringComparison.OrdinalIgnoreCase) && EsAccion(e, p.Accion)));
        return (elegidos, faltantes);
    }

    private static bool EsAccion(PermisoAsignable permiso, string accion) => string.Equals(permiso.Accion, accion, StringComparison.OrdinalIgnoreCase);
}
