using System.Data;

namespace Sistema_ERP
{
    public partial class Inventario : UserControl
    {
        private bool cargandoInventario;
        internal void UsarNavegacionPrincipal()
        {
            VistaContenido.Preparar(this, content);
        }

        public Inventario()
        {
            InitializeComponent();
            if (Sistema_ERP.ModoDiseno.Activo(this)) return;
            UsarNavegacionPrincipal();
            foreach (Control control in content.Controls.Cast<Control>().ToArray()) control.Dispose();
            InitializeInventarioFuncional();
            consultarInventario.Click += async (_, _) => await CargarInventario(true);
            tiendaInventario.SelectedIndexChanged += async (_, _) => { if (!cargandoInventario) await CargarInventario(false); };
            buscarInventario.KeyDown += async (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; await CargarInventario(false); } };
            existenciasInventario.SelectionChanged += (_, _) => SeleccionarExistencia();
            pestanasInventario.SelectedIndexChanged += (_, _) => ActualizarAccionesInventario();
            guardarMinimo.Click += async (_, _) => await CambiarMinimo();
            Load += async (_, _) => await CargarInventario(true);
            ActualizarAccionesInventario();
        }

        private DataRowView? ExistenciaSeleccionada => existenciasInventario.CurrentRow?.DataBoundItem as DataRowView;

        private async Task CargarInventario(bool recargarTiendas)
        {
            if (cargandoInventario || ModoDiseno.Activo(this)) return;
            cargandoInventario = true; ActualizarAccionesInventario();
            estadoInventario.Text = "Consultando inventario...";
            existenciasInventario.DataSource = historialInventario.DataSource = null;
            try
            {
                SesionActual.Exigir();
                if (recargarTiendas)
                {
                    object? anterior = tiendaInventario.SelectedValue;
                    var tiendas = await Task.Run(() => TiendaDatos.Listar(true));
                    if (IsDisposed) return;
                    tiendaInventario.DisplayMember = "Nombre"; tiendaInventario.ValueMember = "id_tienda";
                    tiendaInventario.DataSource = tiendas;
                    if (anterior is int id && tiendas.AsEnumerable().Any(t => t.Field<int>("id_tienda") == id)) tiendaInventario.SelectedValue = id;
                }
                if (tiendaInventario.SelectedValue is not int tienda) throw new ArgumentException("Registra una tienda antes de consultar inventario.");
                string texto = buscarInventario.Text;
                DateTime desde = desdeInventario.Value, hasta = hastaInventario.Value;
                var resultado = await Task.Run(() => (InventarioDatos.Existencias(tienda, texto), InventarioDatos.Movimientos(tienda, texto, desde, hasta)));
                if (IsDisposed) return;
                existenciasInventario.DataSource = resultado.Item1;
                historialInventario.DataSource = resultado.Item2;
                foreach (string columna in new[] { "id_producto", "version_fila" })
                    if (existenciasInventario.Columns[columna] is DataGridViewColumn oculta) oculta.Visible = false;
                resumenInventario.Text = $"{resultado.Item1.Rows.Count} productos · {resultado.Item2.Rows.Count} movimientos en el intervalo. Las existencias son actuales.";
                MensajeInventario("Datos actualizados. Las existencias son de solo lectura. Puedes cambiar el umbral de stock mínimo para avisos.");
            }
            catch (Exception ex) { if (!IsDisposed) { resumenInventario.Text = "Sin datos cargados"; MensajeInventario(ex.Message, true); } }
            finally { if (!IsDisposed) { cargandoInventario = false; SeleccionarExistencia(); ActualizarAccionesInventario(); } }
        }

        private void SeleccionarExistencia()
        {
            minimoInventario.Value = ExistenciaSeleccionada is DataRowView fila ? Convert.ToInt32(fila["Minimo"]) : 0;
            ActualizarAccionesInventario();
        }

        private void ActualizarAccionesInventario()
        {
            bool activo = !cargandoInventario && ExistenciaSeleccionada is DataRowView fila && (bool)fila["Activo"]
                && tiendaInventario.SelectedItem is DataRowView tienda && (bool)tienda["activo"];
            guardarMinimo.Enabled = minimoInventario.Enabled = activo && pestanasInventario.SelectedIndex == 0;
            consultarInventario.Enabled = tiendaInventario.Enabled = buscarInventario.Enabled = desdeInventario.Enabled = hastaInventario.Enabled = !cargandoInventario;
        }

        private async Task CambiarMinimo()
        {
            if (ExistenciaSeleccionada is not DataRowView fila || tiendaInventario.SelectedValue is not int tienda) return;
            int producto = (int)fila["id_producto"], minimo = (int)minimoInventario.Value;
            byte[]? version = fila["version_fila"] as byte[];
            cargandoInventario = true; ActualizarAccionesInventario();
            try { await Task.Run(() => InventarioDatos.GuardarMinimo(tienda, producto, minimo, version)); }
            catch (Exception ex) { if (!IsDisposed) MensajeInventario(ex.Message, true); return; }
            finally { if (!IsDisposed) { cargandoInventario = false; ActualizarAccionesInventario(); } }
            if (IsDisposed) return;
            await CargarInventario(false);
        }

        private void MensajeInventario(string mensaje, bool error = false)
        {
            estadoInventario.Text = mensaje;
            estadoInventario.ForeColor = error ? ProductoEstilo.Error : ProductoEstilo.Exito;
        }
    }
}
