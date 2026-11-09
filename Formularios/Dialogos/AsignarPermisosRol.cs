using System.Data;

namespace Sistema_ERP
{
    public sealed partial class AsignarPermisosRol : Form
    {
        private sealed record Permiso(int Id, string Modulo, string Accion, string Descripcion, bool Activo)
        {
            public override string ToString() => $"{Accion}{(Activo ? "" : " (inactivo)")} · {Descripcion}";
        }

        private readonly List<Permiso> permisos = [];
        private readonly HashSet<int> seleccionados;
        private readonly HashSet<int> seleccionInicial;
        private bool cargando;

        public int[] PermisosSeleccionados => seleccionados.Order().ToArray();

        public AsignarPermisosRol() : this("", new DataTable(), []) { }

        public AsignarPermisosRol(string nombreRol, DataTable catalogo, IEnumerable<int> seleccion)
        {
            seleccionInicial = seleccion.ToHashSet();
            seleccionados = [];
            InitializeComponent();
            string nombre = string.IsNullOrWhiteSpace(nombreRol) ? "Nuevo rol" : nombreRol.Trim();
            Text = $"Permisos por módulo · {nombre}";
            titulo.Text = $"Permisos de {nombre}";
            foreach (DataRow fila in catalogo.Rows)
                permisos.Add(new Permiso((int)fila["Id"], (string)fila["Modulo"], (string)fila["Accion"], (string)fila["Descripcion"], (bool)fila["activo"]));
            seleccionados.UnionWith(permisos.Where(p => seleccionInicial.Contains(p.Id)).Select(p => p.Id));
            foreach (string modulo in permisos.Select(p => p.Modulo).Distinct(StringComparer.OrdinalIgnoreCase).Order())
                listaModulos.Items.Add(modulo);
            listaModulos.SelectedIndexChanged += (_, _) => MostrarModulo();
            listaPermisos.ItemCheck += CambiarPermiso;
            seleccionarModulo.Click += (_, _) => SeleccionarModulo();
            quitarModulo.Click += (_, _) =>
            {
                seleccionados.ExceptWith(PermisosDelModulo().Select(p => p.Id));
                MostrarModulo();
            };
            if (listaModulos.Items.Count > 0) listaModulos.SelectedIndex = 0;
            else MostrarModulo();
        }

        private IEnumerable<Permiso> PermisosDelModulo() => permisos.Where(p =>
            string.Equals(p.Modulo, listaModulos.SelectedItem as string, StringComparison.OrdinalIgnoreCase));

        private void MostrarModulo()
        {
            cargando = true;
            listaPermisos.BeginUpdate();
            try
            {
                listaPermisos.Items.Clear();
                foreach (var permiso in PermisosDelModulo().OrderBy(p => p.Accion))
                    listaPermisos.Items.Add(permiso, seleccionados.Contains(permiso.Id));
                seleccionarModulo.Enabled = PermisosDelModulo().Any(p => p.Activo);
                quitarModulo.Enabled = PermisosDelModulo().Any(p => seleccionados.Contains(p.Id));
                listaPermisos.Enabled = listaPermisos.Items.Count > 0;
                mensaje.Text = listaModulos.Items.Count == 0
                    ? "No hay permisos disponibles. Crea los permisos en la pestaña Permisos."
                    : "Los permisos inactivos asignados se conservan y pueden retirarse. No conceden acceso.";
                ActualizarResumen();
            }
            finally { listaPermisos.EndUpdate(); cargando = false; }
        }

        private void CambiarPermiso(object? sender, ItemCheckEventArgs e)
        {
            if (cargando) return;
            var permiso = (Permiso)listaPermisos.Items[e.Index];
            if (e.NewValue == CheckState.Checked)
            {
                if (!permiso.Activo && !seleccionInicial.Contains(permiso.Id))
                {
                    e.NewValue = e.CurrentValue;
                    mensaje.Text = "No se pueden asignar permisos inactivos.";
                    return;
                }
                if (permiso.Activo && EsAccion(permiso, "GESTIONAR"))
                {
                    var consulta = PermisosDelModulo().FirstOrDefault(p => p.Activo && EsAccion(p, "CONSULTAR"));
                    if (consulta is null)
                    {
                        e.NewValue = e.CurrentValue;
                        mensaje.Text = "Este módulo necesita un permiso CONSULTAR activo para asignar GESTIONAR.";
                        return;
                    }
                    seleccionados.Add(consulta.Id);
                }
                seleccionados.Add(permiso.Id);
            }
            else
            {
                seleccionados.Remove(permiso.Id);
                if (EsAccion(permiso, "CONSULTAR"))
                    seleccionados.ExceptWith(PermisosDelModulo().Where(p => EsAccion(p, "GESTIONAR")).Select(p => p.Id));
            }
            SincronizarSeleccion(e.Index);
            quitarModulo.Enabled = PermisosDelModulo().Any(p => seleccionados.Contains(p.Id));
            ActualizarResumen();
        }

        private void SeleccionarModulo()
        {
            var activos = PermisosDelModulo().Where(p => p.Activo).ToArray();
            bool consulta = activos.Any(p => EsAccion(p, "CONSULTAR"));
            seleccionados.UnionWith(activos.Where(p => !EsAccion(p, "GESTIONAR") || consulta).Select(p => p.Id));
            MostrarModulo();
            if (!consulta && activos.Any(p => EsAccion(p, "GESTIONAR")))
                mensaje.Text = "No se asignó GESTIONAR porque este módulo no tiene un permiso CONSULTAR activo.";
        }

        private void SincronizarSeleccion(int indiceActual)
        {
            cargando = true;
            try
            {
                for (int i = 0; i < listaPermisos.Items.Count; i++)
                    if (i != indiceActual)
                        listaPermisos.SetItemChecked(i, seleccionados.Contains(((Permiso)listaPermisos.Items[i]).Id));
            }
            finally { cargando = false; }
        }

        private void ActualizarResumen()
        {
            int modulos = permisos.Where(p => seleccionados.Contains(p.Id)).Select(p => p.Modulo).Distinct(StringComparer.OrdinalIgnoreCase).Count();
            resumen.Text = $"{seleccionados.Count} permisos seleccionados en {modulos} módulos";
        }

        private static bool EsAccion(Permiso permiso, string accion) => string.Equals(permiso.Accion, accion, StringComparison.OrdinalIgnoreCase);
    }
}
