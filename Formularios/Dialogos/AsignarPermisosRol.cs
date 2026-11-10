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
        internal PlantillaPermisos? PlantillaAplicada { get; private set; }

        public AsignarPermisosRol() : this("", new DataTable(), []) { }

        public AsignarPermisosRol(string nombreRol, DataTable catalogo, IEnumerable<int> seleccion, int? idRol = null, string codigoRol = "")
        {
            seleccionInicial = seleccion.ToHashSet();
            seleccionados = [];
            InitializeComponent();
            string nombre = string.IsNullOrWhiteSpace(nombreRol) ? "Nuevo rol" : nombreRol.Trim();
            Text = $"Permisos por módulo · {nombre}";
            titulo.Text = $"Permisos de {nombre}";
            listaModulos.FormattingEnabled = true;
            listaModulos.Format += (_, e) => e.Value = CatalogoPermisos.NombrePantalla((string)e.ListItem!);
            foreach (var plantilla in PlantillaPermisos.Todas) listaPresets.Items.Add(plantilla);
            listaPresets.SelectedIndexChanged += (_, _) => DescribirPreset();
            listaPresets.SelectedIndex = 0;
            aplicarPreset.Click += (_, _) => AplicarPreset();
            quitarTodos.Click += (_, _) =>
            {
                seleccionados.Clear(); PlantillaAplicada = null;
                MostrarModulo();
            };
            avisoAdministrador.Visible = string.Equals(codigoRol.Trim(), "ADMINISTRADOR", StringComparison.OrdinalIgnoreCase);
            avisoAdministrador.Text = "ADMINISTRADOR siempre tiene acceso total. Para limitar pantallas, usa otro rol y retira este del usuario.";
            listaModulos.SelectedIndexChanged += (_, _) => MostrarModulo();
            listaPermisos.ItemCheck += CambiarPermiso;
            seleccionarModulo.Click += (_, _) => SeleccionarModulo();
            quitarModulo.Click += (_, _) =>
            {
                seleccionados.ExceptWith(PermisosDelModulo().Select(p => p.Id));
                MostrarModulo();
            };
            RecargaManual.Agregar(acciones, () =>
            {
                using var actuales = SeguridadDatos.CatalogoPermisosParaRol(idRol);
                CargarCatalogo(actuales, seleccionados.ToArray());
            });
            CargarCatalogo(catalogo, seleccionInicial);
        }

        private void CargarCatalogo(DataTable catalogo, IEnumerable<int> seleccion)
        {
            string? moduloAnterior = listaModulos.SelectedItem as string;
            var ids = seleccion.ToHashSet();
            permisos.Clear(); seleccionados.Clear();
            foreach (DataRow fila in catalogo.Rows)
                permisos.Add(new Permiso((int)fila["Id"], (string)fila["Modulo"], (string)fila["Accion"], (string)fila["Descripcion"], (bool)fila["activo"]));
            seleccionados.UnionWith(permisos.Where(p => ids.Contains(p.Id)).Select(p => p.Id));
            listaModulos.Items.Clear();
            foreach (string modulo in permisos.Select(p => p.Modulo).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(CatalogoPermisos.NombrePantalla))
                listaModulos.Items.Add(modulo);
            if (moduloAnterior is not null && listaModulos.Items.Contains(moduloAnterior)) listaModulos.SelectedItem = moduloAnterior;
            else if (listaModulos.Items.Count > 0) listaModulos.SelectedIndex = 0;
            else MostrarModulo();
            aplicarPreset.Enabled = permisos.Any(p => p.Activo);
        }

        private void DescribirPreset()
        {
            if (listaPresets.SelectedItem is PlantillaPermisos plantilla)
                descripcionPreset.Text = "Aplicar reemplaza toda la selección. " + plantilla.Descripcion;
        }

        private void AplicarPreset()
        {
            if (listaPresets.SelectedItem is not PlantillaPermisos plantilla) return;
            var activos = permisos.Where(p => p.Activo).ToArray();
            var consultas = activos.Where(p => EsAccion(p, "CONSULTAR")).Select(p => p.Modulo).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var elegidos = activos.Where(p => plantilla.Incluye(p.Modulo, p.Accion) &&
                (!EsAccion(p, "GESTIONAR") || consultas.Contains(p.Modulo))).ToArray();
            seleccionados.Clear();
            seleccionados.UnionWith(elegidos.Select(p => p.Id));
            PlantillaAplicada = plantilla;
            MostrarModulo();
            int faltantes = plantilla.Permisos.Count(p => !elegidos.Any(e =>
                string.Equals(e.Modulo, p.Modulo, StringComparison.OrdinalIgnoreCase) && EsAccion(e, p.Accion)));
            mensaje.Text = $"Preset {plantilla.Nombre} aplicado. Puedes ajustar sus pantallas antes de aceptar." +
                (faltantes > 0 ? $" Faltan {faltantes} permisos activos requeridos; revísalos en Roles → Crear / editar permisos." : "");
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
                string? modulo = listaModulos.SelectedItem as string;
                var pantalla = modulo is null ? null : CatalogoPermisos.Buscar(modulo);
                alcanceModulo.Text = pantalla is null
                    ? "Selecciona los permisos del catálogo para este módulo."
                    : $"Consultar: {pantalla.Consulta}{Environment.NewLine}Gestionar: {pantalla.Gestion}";
                seleccionarModulo.Enabled = PermisosDelModulo().Any(p => p.Activo);
                quitarModulo.Enabled = PermisosDelModulo().Any(p => seleccionados.Contains(p.Id));
                listaPermisos.Enabled = listaPermisos.Items.Count > 0;
                mensaje.Text = listaModulos.Items.Count == 0
                    ? "No hay permisos disponibles. Revisa los inactivos en Roles → Crear / editar permisos."
                    : "Sin consulta no hay acceso; los permisos inactivos no conceden acceso.";
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
            resumen.Text = $"{seleccionados.Count} permisos · {modulos} módulos";
            quitarTodos.Enabled = seleccionados.Count > 0;
        }

        private static bool EsAccion(Permiso permiso, string accion) => string.Equals(permiso.Accion, accion, StringComparison.OrdinalIgnoreCase);
    }
}
