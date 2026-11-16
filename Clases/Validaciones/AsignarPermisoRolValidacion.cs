using System.Data;

namespace Sistema_ERP;

internal record PermisoAsignable(int Id, string Modulo, string Accion, string Descripcion, bool Activo);

internal static class AsignarPermisoRolValidacion
{
    internal static HashSet<int> IncluirCreados(DataTable catalogo, IEnumerable<int> seleccion, IEnumerable<int> creados)
    {
        var elegidos = seleccion.ToHashSet();
        var nuevos = creados.ToHashSet();
        foreach (var permiso in catalogo.AsEnumerable().Where(f => f.Field<bool>("activo") && nuevos.Contains(f.Field<int>("Id"))))
        {
            if (string.Equals(permiso.Field<string>("Accion"), "GESTIONAR", StringComparison.OrdinalIgnoreCase))
            {
                var consulta = catalogo.AsEnumerable().FirstOrDefault(f => f.Field<bool>("activo") &&
                    string.Equals(f.Field<string>("Modulo"), permiso.Field<string>("Modulo"), StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(f.Field<string>("Accion"), "CONSULTAR", StringComparison.OrdinalIgnoreCase));
                if (consulta is null) continue;
                elegidos.Add(consulta.Field<int>("Id"));
            }
            elegidos.Add(permiso.Field<int>("Id"));
        }
        return elegidos;
    }
    private static bool Accion(PermisoAsignable permiso, string accion) => string.Equals(permiso.Accion, accion, StringComparison.OrdinalIgnoreCase);

    internal static void Cambiar(PermisoAsignable permiso, bool marcar, IReadOnlyCollection<PermisoAsignable> modulo,
        HashSet<int> seleccionados, IReadOnlySet<int> iniciales)
    {
        if (marcar)
        {
            if (!permiso.Activo && !iniciales.Contains(permiso.Id)) throw new InvalidOperationException("No se pueden asignar permisos inactivos.");
            if (permiso.Activo && Accion(permiso, "GESTIONAR"))
            {
                var consulta = modulo.FirstOrDefault(p => p.Activo && Accion(p, "CONSULTAR"));
                if (consulta is null) throw new InvalidOperationException("Este módulo necesita un permiso CONSULTAR activo para asignar GESTIONAR.");
                seleccionados.Add(consulta.Id);
            }
            seleccionados.Add(permiso.Id);
        }
        else
        {
            seleccionados.Remove(permiso.Id);
            if (Accion(permiso, "CONSULTAR")) seleccionados.ExceptWith(modulo.Where(p => Accion(p, "GESTIONAR")).Select(p => p.Id));
        }
    }

    internal static string? SeleccionarModulo(IEnumerable<PermisoAsignable> permisos, HashSet<int> seleccionados)
    {
        var activos = permisos.Where(p => p.Activo).ToArray();
        bool consulta = activos.Any(p => Accion(p, "CONSULTAR"));
        seleccionados.UnionWith(activos.Where(p => !Accion(p, "GESTIONAR") || consulta).Select(p => p.Id));
        return !consulta && activos.Any(p => Accion(p, "GESTIONAR"))
            ? "No se asignó GESTIONAR porque este módulo no tiene un permiso CONSULTAR activo." : null;
    }
}
