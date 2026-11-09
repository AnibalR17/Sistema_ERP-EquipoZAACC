using System.Data;

namespace Sistema_ERP
{
    public sealed partial class GenerarPermisosUsuario : Form
    {
        private sealed record Rol(int Id, string Nombre, bool Activo)
        {
            public override string ToString() => Nombre;
        }

        private sealed record Permiso(int RolId, int Id, string Modulo, string Accion, string Descripcion);
        private readonly List<Permiso> permisosPorRol = [];
        private readonly HashSet<int> administradores = [];
        private readonly HashSet<int> seleccionInicial;

        public int[] RolesSeleccionados => listaRoles.CheckedItems.Cast<Rol>().Select(r => r.Id).ToArray();

        public GenerarPermisosUsuario() : this("", new DataTable(), new DataTable(), []) { }

        public GenerarPermisosUsuario(string usuario, DataTable roles, DataTable permisos, IEnumerable<int> seleccionados)
        {
            seleccionInicial = seleccionados.ToHashSet();
            InitializeComponent();
            string nombre = string.IsNullOrWhiteSpace(usuario) ? "Nuevo usuario" : usuario.Trim();
            Text = $"Generar permisos · {nombre}";
            titulo.Text = $"Permisos de {nombre}";

            foreach (DataRow fila in permisos.Rows)
            {
                int rol = (int)fila["IdRol"];
                if (string.Equals((string)fila["CodigoRol"], "ADMINISTRADOR", StringComparison.OrdinalIgnoreCase))
                    administradores.Add(rol);
                if (!fila.IsNull("IdPermiso"))
                    permisosPorRol.Add(new Permiso(rol, (int)fila["IdPermiso"], (string)fila["Modulo"], (string)fila["Accion"], (string)fila["Descripcion"]));
            }
            foreach (DataRow fila in roles.Rows)
            {
                var rol = new Rol((int)fila["Id"], (string)fila["Nombre"], (bool)fila["activo"]);
                listaRoles.Items.Add(rol, seleccionInicial.Contains(rol.Id));
            }
            listaRoles.ItemCheck += (_, e) =>
            {
                var rol = (Rol)listaRoles.Items[e.Index];
                if (!rol.Activo && e.NewValue == CheckState.Checked && !seleccionInicial.Contains(rol.Id))
                    e.NewValue = e.CurrentValue;
                ActualizarPermisos(e.Index, e.NewValue);
            };
            ActualizarPermisos();
        }

        private void ActualizarPermisos(int indice = -1, CheckState nuevoEstado = CheckState.Unchecked)
        {
            var indices = listaRoles.CheckedIndices.Cast<int>().ToHashSet();
            if (indice >= 0)
            {
                if (nuevoEstado == CheckState.Checked) indices.Add(indice);
                else indices.Remove(indice);
            }
            var seleccion = indices.Select(i => (Rol)listaRoles.Items[i]).Where(r => r.Activo).Select(r => r.Id).ToHashSet();
            bool administrador = seleccion.Overlaps(administradores);
            var permisos = permisosPorRol.Where(p => seleccion.Contains(p.RolId))
                .DistinctBy(p => p.Id).OrderBy(p => p.Modulo).ThenBy(p => p.Accion).ToArray();
            tablaPermisos.DataSource = permisos.Select(p => new { p.Modulo, p.Accion, p.Descripcion }).ToArray();
            bool mostrarTabla = !administrador && permisos.Length > 0;
            tablaPermisos.Visible = mostrarTabla;
            mensajePermisos.Visible = !mostrarTabla;
            mensajePermisos.Text = administrador
                ? "Acceso total. El rol ADMINISTRADOR permite consultar y gestionar todos los módulos."
                : listaRoles.Items.Count == 0
                    ? "No hay roles disponibles. Crea un rol y asígnale permisos en la pestaña Roles."
                    : "Los roles seleccionados no conceden permisos activos. Selecciona un rol con permisos o configúralos en la pestaña Roles.";
            resumen.Text = administrador
                ? $"{indices.Count} roles seleccionados · Acceso total"
                : $"{indices.Count} roles seleccionados · {permisos.Length} permisos activos";
        }
    }
}
