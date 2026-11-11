using System.Data;

namespace Sistema_ERP
{
    public sealed partial class GenerarPermisosUsuario : Form
    {
        private sealed record Permiso(int RolId, int Id, string Modulo, string Accion, string Descripcion);
        private readonly List<Permiso> permisosPorRol = [];
        private readonly HashSet<int> administradores = [];
        public int[] RolesSeleccionados => listaRoles.IdsSeleccionados;

        public GenerarPermisosUsuario() : this("", new DataTable(), new DataTable(), []) { }

        public GenerarPermisosUsuario(string usuario, DataTable roles, DataTable permisos, IEnumerable<int> seleccionados, int? idUsuario = null)
        {
            InitializeComponent();
            string nombre = string.IsNullOrWhiteSpace(usuario) ? "Nuevo usuario" : usuario.Trim();
            Text = $"Generar permisos · {nombre}";
            titulo.Text = $"Permisos de {nombre}";

            CargarOpciones(roles, permisos, seleccionados);
            listaRoles.SeleccionCambiada += (_, _) => ActualizarPermisos();
            RecargaManual.Agregar(acciones, () =>
            {
                int[] actual = RolesSeleccionados;
                using var rolesActuales = SeguridadDatos.Asignaciones(TipoSeguridad.Usuarios, idUsuario);
                using var permisosActuales = SeguridadDatos.PermisosPorRol();
                CargarOpciones(rolesActuales, permisosActuales, actual);
            });
        }

        private void CargarOpciones(DataTable roles, DataTable permisos, IEnumerable<int> seleccionados)
        {
            permisosPorRol.Clear(); administradores.Clear();
            foreach (DataRow fila in permisos.Rows)
            {
                int rol = (int)fila["IdRol"];
                if (string.Equals((string)fila["CodigoRol"], "ADMINISTRADOR", StringComparison.OrdinalIgnoreCase))
                    administradores.Add(rol);
                if (!fila.IsNull("IdPermiso"))
                    permisosPorRol.Add(new Permiso(rol, (int)fila["IdPermiso"], (string)fila["Modulo"], (string)fila["Accion"], (string)fila["Descripcion"]));
            }
            listaRoles.Cargar(roles, seleccionados);
            ActualizarPermisos();
        }

        private void ActualizarPermisos()
        {
            var seleccion = listaRoles.IdsActivosSeleccionados.ToHashSet();
            bool administrador = seleccion.Overlaps(administradores);
            var permisos = permisosPorRol.Where(p => seleccion.Contains(p.RolId))
                .DistinctBy(p => p.Id).OrderBy(p => p.Modulo).ThenBy(p => p.Accion).ToArray();
            tablaPermisos.DataSource = permisos.Select(p => new { Modulo = CatalogoPermisos.NombrePantalla(p.Modulo), p.Accion, p.Descripcion }).ToArray();
            bool mostrarTabla = !administrador && permisos.Length > 0;
            tablaPermisos.Visible = mostrarTabla;
            mensajePermisos.Visible = !mostrarTabla;
            mensajePermisos.Text = administrador
                ? "Acceso total. El rol ADMINISTRADOR permite consultar y gestionar todos los módulos."
                : !listaRoles.TieneRoles
                    ? "No hay roles disponibles. Crea un rol y asígnale permisos en la pestaña Roles."
                    : "Los roles seleccionados no conceden permisos activos. Selecciona un rol con permisos o configúralos en la pestaña Roles.";
            resumen.Text = administrador
                ? $"{RolesSeleccionados.Length} roles seleccionados · Acceso total"
                : $"{RolesSeleccionados.Length} roles seleccionados · {permisos.Length} permisos activos";
        }
    }
}
