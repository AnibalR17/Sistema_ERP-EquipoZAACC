using System.Data;

namespace Sistema_ERP
{
    public sealed partial class AsignarPermisosRol : FormularioTematico
    {
        private sealed record Permiso(int Id, string Modulo, string Accion, string Descripcion, bool Activo) : PermisoAsignable(Id, Modulo, Accion, Descripcion, Activo)
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
            buscarPermisos.TextChanged += (_, _) => FiltrarModulos();
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
            TemaAplicacion.Aplicar(this);
        }

        private void CargarCatalogo(DataTable catalogo, IEnumerable<int> seleccion)
        {
            var ids = seleccion.ToHashSet();
            permisos.Clear(); seleccionados.Clear();
            foreach (DataRow fila in catalogo.Rows)
                permisos.Add(new Permiso((int)fila["Id"], (string)fila["Modulo"], (string)fila["Accion"], (string)fila["Descripcion"], (bool)fila["activo"]));
            seleccionados.UnionWith(permisos.Where(p => ids.Contains(p.Id)).Select(p => p.Id));
            FiltrarModulos();
            aplicarPreset.Enabled = permisos.Any(p => p.Activo);
        }

        private void FiltrarModulos()
        {
            string? anterior = listaModulos.SelectedItem as string;
            string texto = buscarPermisos.Text.Trim();
            bool Coincide(string valor) => valor.Contains(texto, StringComparison.CurrentCultureIgnoreCase);
            listaModulos.BeginUpdate();
            try
            {
                listaModulos.Items.Clear();
                foreach (string modulo in permisos.Where(p => Coincide(p.Modulo) || Coincide(CatalogoPermisos.NombrePantalla(p.Modulo))
                    || Coincide(p.Accion) || Coincide(p.Descripcion)).Select(p => p.Modulo).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(CatalogoPermisos.NombrePantalla))
                    listaModulos.Items.Add(modulo);
                if (anterior is not null && listaModulos.Items.Contains(anterior)) listaModulos.SelectedItem = anterior;
                else if (listaModulos.Items.Count > 0) listaModulos.SelectedIndex = 0;
                else MostrarModulo();
            }
            finally { listaModulos.EndUpdate(); }
        }

        private void DescribirPreset()
        {
            if (listaPresets.SelectedItem is PlantillaPermisos plantilla)
                descripcionPreset.Text = "Aplicar reemplaza toda la selección. " + plantilla.Descripcion;
        }

        private void AplicarPreset()
        {
            if (listaPresets.SelectedItem is not PlantillaPermisos plantilla) return;
            var (elegidos, faltantes) = AplicarPresetPermisosValidacion.Seleccionar(plantilla, permisos);
            seleccionados.Clear();
            seleccionados.UnionWith(elegidos.Select(p => p.Id));
            PlantillaAplicada = plantilla;
            MostrarModulo();
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
                    ? buscarPermisos.Text.Trim().Length > 0 ? "No hay coincidencias. Cambia o borra la búsqueda." : "No hay permisos disponibles. Revisa los inactivos en Roles → Crear / editar permisos."
                    : "Sin consulta no hay acceso; los permisos inactivos no conceden acceso.";
                ActualizarResumen();
            }
            finally { listaPermisos.EndUpdate(); cargando = false; }
        }

        private void CambiarPermiso(object? sender, ItemCheckEventArgs e)
        {
            if (cargando) return;
            var permiso = (Permiso)listaPermisos.Items[e.Index];
            try
            {
                AsignarPermisoRolValidacion.Cambiar(permiso, e.NewValue == CheckState.Checked, PermisosDelModulo().ToArray(), seleccionados, seleccionInicial);
            }
            catch (InvalidOperationException ex) { e.NewValue = e.CurrentValue; mensaje.Text = ex.Message; return; }
            SincronizarSeleccion(e.Index);
            quitarModulo.Enabled = PermisosDelModulo().Any(p => seleccionados.Contains(p.Id));
            ActualizarResumen();
        }

        private void SeleccionarModulo()
        {
            string? advertencia = AsignarPermisoRolValidacion.SeleccionarModulo(PermisosDelModulo(), seleccionados);
            MostrarModulo();
            if (advertencia is not null) mensaje.Text = advertencia;
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

    }
}
