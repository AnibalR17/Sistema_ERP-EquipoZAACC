using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;

namespace Sistema_ERP.Vistas
{
    public partial class PantallaCompras : Form
    {
        private const decimal MaximoImporte = 9999999999999999.99m;
        private readonly DataTable dtCarrito = CrearCarrito();
        private bool ocupado, cargandoCampos, catalogosListos, historialListo, modificado, lineaPendiente;
        private int? compraId;
        private byte[]? versionCompra;
        private DataRow? lineaEnEdicion;
        private string estadoCompra = "NUEVA";

        public PantallaCompras()
        {
            InitializeComponent();
            if (ModoDiseno.Activo(this)) return;
            ConfigurarPresentacion();
            PermisosInterfaz.Configurar(this, Modulos.Compras, btnGuardar, btnConfirmar, btnAnular, btnAgregarCarrito, btnActualizarLinea, btnQuitarLinea);
            dgvDetalleCompra.DataBindingComplete += (_, _) =>
            {
                if (dgvDetalleCompra.Columns["id_producto"] is DataGridViewColumn columna) columna.Visible = false;
            };
            dgvDetalleCompra.DataSource = dtCarrito;
            btnAgregarCarrito.Click += (_, _) => EditarLinea(false);
            btnActualizarLinea.Click += (_, _) => EditarLinea(true);
            btnQuitarLinea.Click += (_, _) => QuitarLinea();
            btnGuardar.Click += async (_, _) => await GuardarCompra();
            btnCancelar.Click += (_, _) => { if (PuedeDescartar()) NuevaCompra(); };
            btnConfirmar.Click += async (_, _) => await ConfirmarCompra();
            btnAnular.Click += async (_, _) => await AnularCompra();
            btnRecargar.Click += async (_, _) => { if (PuedeDescartar()) await CargarDatos(); };
            RecargaManual.Preparar(btnRecargar);
            btnConsultar.Click += async (_, _) => await ConsultarHistorial();
            btnAbrirCompra.Click += async (_, _) => await AbrirCompra();
            dgvCompras.CellDoubleClick += async (_, e) => { if (e.RowIndex >= 0) await AbrirCompra(); };
            dgvCompras.SelectionChanged += (_, _) => ActualizarAcciones();
            dgvDetalleCompra.SelectionChanged += (_, _) => SeleccionarLinea();
            txtBuscar.KeyDown += async (_, e) =>
            {
                if (e.KeyCode != Keys.Enter) return;
                e.SuppressKeyPress = true;
                await ConsultarHistorial();
            };
            pestanasCompras.SelectedIndexChanged += async (_, _) =>
            {
                if (pestanasCompras.SelectedIndex == 1 && !historialListo && catalogosListos && !ocupado)
                    await ConsultarHistorial();
            };
            foreach (Control campo in new Control[] { cmbProveedor, cmbTienda, txtFactura, txtObservaciones, dtpFecha })
            {
                if (campo is ComboBox combo) combo.SelectedIndexChanged += (_, _) => CambiarCabecera();
                else if (campo is DateTimePicker fecha) fecha.ValueChanged += (_, _) => CambiarCabecera();
                else campo.TextChanged += (_, _) => CambiarCabecera();
            }
            cmbProducto.SelectedIndexChanged += (_, _) => CambiarLinea();
            txtCantidad.TextChanged += (_, _) => CambiarLinea();
            txtPrecio.TextChanged += (_, _) => CambiarLinea();
            NuevaCompra();
        }

        private static DataTable CrearCarrito()
        {
            var tabla = new DataTable();
            tabla.Columns.Add("id_producto", typeof(int));
            tabla.Columns.Add("Codigo", typeof(string));
            tabla.Columns.Add("Producto", typeof(string));
            tabla.Columns.Add("Cantidad", typeof(int));
            tabla.Columns.Add("Costo", typeof(decimal));
            tabla.Columns.Add("Subtotal", typeof(decimal), "Cantidad * Costo");
            return tabla;
        }

        private bool Editable => estadoCompra is "NUEVA" or "BORRADOR";
        private bool TieneCambios => modificado || lineaPendiente;
        private DataRow? LineaSeleccionada => (dgvDetalleCompra.CurrentRow?.DataBoundItem as DataRowView)?.Row;

        private async void PantallaCompras_Load(object? sender, EventArgs e)
        {
            if (!ModoDiseno.Activo(this)) await CargarDatos();
        }

        private async Task CargarDatos()
        {
            if (ocupado) return;
            ocupado = true;
            catalogosListos = historialListo = false;
            ActualizarAcciones();
            Mensaje("Consultando proveedores, tiendas y productos...");
            try
            {
                SesionActual.Exigir();
                var datos = await Task.Run(() => (CompraDatos.Proveedores(), CompraDatos.Tiendas(), CompraDatos.Productos()));
                if (IsDisposed) return;
                cargandoCampos = true;
                try
                {
                    Enlazar(cmbProveedor, datos.Item1, "id_proveedor");
                    Enlazar(cmbTienda, datos.Item2, "id_tienda");
                    Enlazar(cmbProducto, datos.Item3, "id_producto");
                    catalogosListos = true;
                    NuevaCompra();
                }
                finally { cargandoCampos = false; }
                Mensaje(datos.Item1.Rows.Count == 0 || datos.Item2.Rows.Count == 0 || datos.Item3.Rows.Count == 0
                    ? "Registra proveedores, tiendas y productos activos; después pulsa Reiniciar."
                    : "Agrega productos y guarda el borrador. Confirma la compra para ingresar las unidades al inventario.");
            }
            catch (Exception ex) { if (!IsDisposed) Mensaje(ErrorOperacion(ex), true); }
            finally { if (!IsDisposed) { ocupado = false; ActualizarAcciones(); } }
            if (!IsDisposed && catalogosListos) await ConsultarHistorial();
        }

        private static void Enlazar(ComboBox combo, DataTable datos, string identificador)
        {
            combo.DisplayMember = "nombre";
            combo.ValueMember = identificador;
            combo.DataSource = datos;
        }

        private void NuevaCompra()
        {
            cargandoCampos = true;
            try
            {
                compraId = null;
                versionCompra = null;
                estadoCompra = "NUEVA";
                dtCarrito.Clear();
                txtFactura.Clear();
                txtObservaciones.Clear();
                dtpFecha.Value = DateTime.Today;
                QuitarReferenciasHistoricas(cmbProveedor);
                QuitarReferenciasHistoricas(cmbTienda);
                SeleccionarPrimeroActivo(cmbProveedor);
                SeleccionarPrimeroActivo(cmbTienda);
                LimpiarLinea();
                modificado = lineaPendiente = false;
                pestanasCompras.SelectedIndex = 0;
                Mensaje("Nueva compra. Selecciona proveedor y tienda e indica el número de factura.");
            }
            finally { cargandoCampos = false; }
            ActualizarTotal();
            ActualizarAcciones();
        }

        private static void SeleccionarPrimeroActivo(ComboBox combo)
        {
            combo.SelectedIndex = -1;
            for (int i = 0; i < combo.Items.Count; i++)
                if (combo.Items[i] is DataRowView fila && (!fila.Row.Table.Columns.Contains("historico") || !fila.Row.Field<bool>("historico")))
                { combo.SelectedIndex = i; break; }
        }

        private static void QuitarReferenciasHistoricas(ComboBox combo)
        {
            if (combo.DataSource is not DataTable tabla || !tabla.Columns.Contains("historico")) return;
            foreach (DataRow fila in tabla.AsEnumerable().Where(f => f.Field<bool>("historico")).ToArray())
                tabla.Rows.Remove(fila);
        }

        private void CambiarCabecera()
        {
            if (cargandoCampos || ocupado || !Editable) return;
            modificado = true;
            ActualizarAcciones();
        }

        private void CambiarLinea()
        {
            if (cargandoCampos || ocupado || !Editable) return;
            lineaPendiente = true;
            ActualizarAcciones();
        }

        internal static decimal LeerCosto(string texto)
        {
            string entrada = texto.Trim();
            var estilo = NumberStyles.AllowDecimalPoint;
            bool valido = decimal.TryParse(entrada, estilo, CultureInfo.CurrentCulture, out decimal costo);
            if (!valido && CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator != ".")
                valido = decimal.TryParse(entrada, estilo, CultureInfo.InvariantCulture, out costo);
            if (!valido || costo <= 0 || costo > MaximoImporte || decimal.Round(costo, 2) != costo)
                throw new ArgumentException("Indica un costo positivo con hasta dos decimales, sin separadores de miles.");
            return costo;
        }

        private void EditarLinea(bool actualizar)
        {
            if (ocupado || !catalogosListos || !Editable) return;
            try
            {
                if (cmbProducto.SelectedItem is not DataRowView producto)
                    throw new ArgumentException("Selecciona un producto activo.");
                if (!int.TryParse(txtCantidad.Text.Trim(), NumberStyles.None, CultureInfo.CurrentCulture, out int cantidad) || cantidad <= 0)
                    throw new ArgumentException("La cantidad debe ser un entero positivo.");
                decimal costo = LeerCosto(txtPrecio.Text);
                decimal subtotal = cantidad * costo;
                if (subtotal > MaximoImporte) throw new ArgumentException("El subtotal supera el importe permitido.");
                int id = (int)producto["id_producto"];
                DataRow? seleccionada = actualizar ? LineaSeleccionada : null;
                if (actualizar && seleccionada == null) throw new ArgumentException("Selecciona una línea para actualizarla.");
                if (dtCarrito.AsEnumerable().Any(f => f != seleccionada && f.Field<int>("id_producto") == id))
                    throw new ArgumentException("El producto ya está en el detalle. Selecciona su línea y pulsa Actualizar línea.");
                decimal total = dtCarrito.AsEnumerable().Where(f => f != seleccionada).Sum(f => f.Field<decimal>("Subtotal")) + subtotal;
                if (total > MaximoImporte) throw new ArgumentException("El total supera el importe permitido.");
                cargandoCampos = true;
                try
                {
                    if (seleccionada == null)
                        dtCarrito.Rows.Add(id, producto["codigo_barras"], producto["nombre"], cantidad, costo);
                    else
                    {
                        seleccionada.BeginEdit();
                        seleccionada["id_producto"] = id;
                        seleccionada["Codigo"] = producto["codigo_barras"];
                        seleccionada["Producto"] = producto["nombre"];
                        seleccionada["Cantidad"] = cantidad;
                        seleccionada["Costo"] = costo;
                        seleccionada.EndEdit();
                    }
                }
                finally { cargandoCampos = false; }
                modificado = true;
                LimpiarLinea();
                ActualizarTotal();
                Mensaje(actualizar ? "Línea actualizada. Guarda el borrador para conservar los cambios." : "Producto agregado al detalle.");
            }
            catch (Exception ex) when (ex is ArgumentException or OverflowException) { Mensaje(ex.Message, true); }
            ActualizarAcciones();
        }

        private void SeleccionarLinea()
        {
            if (cargandoCampos || ocupado) return;
            if (lineaPendiente && MessageBox.Show(this,
                "La línea en edición tiene cambios pendientes. ¿Descartarlos para seleccionar otra línea?",
                "Detalle de compra", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                cargandoCampos = true;
                try
                {
                    var anterior = dgvDetalleCompra.Rows.Cast<DataGridViewRow>().FirstOrDefault(f =>
                        (f.DataBoundItem as DataRowView)?.Row == lineaEnEdicion);
                    dgvDetalleCompra.ClearSelection();
                    dgvDetalleCompra.CurrentCell = anterior?.Cells["Producto"];
                    if (anterior != null) anterior.Selected = true;
                }
                finally { cargandoCampos = false; }
                ActualizarAcciones();
                return;
            }
            cargandoCampos = true;
            try
            {
                if (LineaSeleccionada is DataRow fila)
                {
                    cmbProducto.SelectedValue = fila["id_producto"];
                    txtCantidad.Text = Convert.ToString(fila["Cantidad"], CultureInfo.CurrentCulture);
                    txtPrecio.Text = ((decimal)fila["Costo"]).ToString("F2", CultureInfo.CurrentCulture);
                }
                else { cmbProducto.SelectedIndex = -1; txtCantidad.Clear(); txtPrecio.Clear(); }
                lineaEnEdicion = LineaSeleccionada;
                lineaPendiente = false;
            }
            finally { cargandoCampos = false; }
            ActualizarAcciones();
        }

        private void LimpiarLinea()
        {
            bool anterior = cargandoCampos;
            cargandoCampos = true;
            try
            {
                dgvDetalleCompra.ClearSelection();
                dgvDetalleCompra.CurrentCell = null;
                cmbProducto.SelectedIndex = -1;
                txtCantidad.Clear();
                txtPrecio.Clear();
                lineaPendiente = false;
                lineaEnEdicion = null;
            }
            finally { cargandoCampos = anterior; }
        }

        private void QuitarLinea()
        {
            if (ocupado || !Editable || LineaSeleccionada is not DataRow fila) return;
            cargandoCampos = true;
            try { dtCarrito.Rows.Remove(fila); }
            finally { cargandoCampos = false; }
            modificado = true;
            LimpiarLinea();
            ActualizarTotal();
            ActualizarAcciones();
            Mensaje("Línea retirada. Guarda el borrador para conservar los cambios.");
        }

        private void ActualizarTotal() => lblTotal.Text = $"Total: L. {dtCarrito.AsEnumerable().Sum(f => f.Field<decimal>("Subtotal")):N2}";

        private async Task GuardarCompra()
        {
            if (ocupado || !catalogosListos || !Editable || (compraId != null && versionCompra == null)) return;
            bool guardada = false;
            try
            {
                if (lineaPendiente) throw new ArgumentException("Agrega o actualiza la línea en edición antes de guardar el borrador.");
                if (cmbProveedor.SelectedValue is not int proveedor || cmbTienda.SelectedValue is not int tienda)
                    throw new ArgumentException("Selecciona proveedor y tienda.");
                string factura = txtFactura.Text.Trim(), notas = txtObservaciones.Text.Trim();
                DateTime fecha = dtpFecha.Value.Date;
                var lineas = dtCarrito.AsEnumerable().Select(f => new LineaCompra(f.Field<int>("id_producto"), f.Field<int>("Cantidad"), f.Field<decimal>("Costo"))).ToArray();
                ocupado = true; ActualizarAcciones();
                int id = await Task.Run(() => CompraDatos.Guardar(compraId, versionCompra, proveedor, tienda, factura, fecha, notas, lineas));
                if (IsDisposed) return;
                compraId = id; estadoCompra = "BORRADOR"; versionCompra = null; modificado = false;
                guardada = true;
                InvalidarHistorial();
                var datos = await Task.Run(() => CompraDatos.Obtener(id));
                if (IsDisposed) return;
                MostrarCompra(datos);
                Mensaje($"Compra #{id} guardada como borrador. Confirma para ingresar las unidades al inventario.");
            }
            catch (Exception ex)
            {
                if (!IsDisposed) Mensaje(guardada
                    ? $"La compra #{compraId} se guardó, pero no se pudo volver a cargar. Consúltala en Historial antes de continuar. {ErrorOperacion(ex)}"
                    : ErrorOperacion(ex), true);
            }
            finally { if (!IsDisposed) { ocupado = false; ActualizarAcciones(); } }
        }

        private async Task ConsultarHistorial()
        {
            if (ocupado) return;
            ocupado = true; historialListo = false; ActualizarAcciones();
            try
            {
                string texto = txtBuscar.Text, estado = cmbEstadoFiltro.SelectedIndex <= 0 ? "" : cmbEstadoFiltro.Text;
                DateTime desde = dtpDesde.Value.Date, hasta = dtpHasta.Value.Date;
                var compras = await Task.Run(() => CompraDatos.Listar(texto, estado, desde, hasta));
                if (IsDisposed) return;
                dgvCompras.DataSource = compras;
                ConfigurarColumnasHistorial();
                historialListo = true;
                if (pestanasCompras.SelectedIndex == 1) Mensaje($"{compras.Rows.Count} compras. Abre una fila para consultar su detalle.");
            }
            catch (Exception ex)
            {
                if (!IsDisposed) { dgvCompras.DataSource = null; Mensaje(ErrorOperacion(ex), true); }
            }
            finally { if (!IsDisposed) { ocupado = false; ActualizarAcciones(); } }
        }

        private void InvalidarHistorial()
        {
            historialListo = false;
            dgvCompras.DataSource = null;
        }

        private async Task AbrirCompra()
        {
            if (ocupado || !historialListo || dgvCompras.CurrentRow?.DataBoundItem is not DataRowView fila || !PuedeDescartar()) return;
            int id = (int)fila["id_compra"];
            ocupado = true; ActualizarAcciones();
            try
            {
                var datos = await Task.Run(() => CompraDatos.Obtener(id));
                if (IsDisposed) return;
                MostrarCompra(datos);
                pestanasCompras.SelectedIndex = 0;
                Mensaje(estadoCompra == "BORRADOR"
                    ? "Borrador cargado. Proveedor y tienda se conservan; puedes editar la factura y sus líneas."
                    : estadoCompra == "ANULADA" ? $"Compra anulada. Motivo: {datos.Tables[0].Rows[0]["motivo_anulacion"]}" : "Compra confirmada. El detalle es de solo lectura.");
            }
            catch (Exception ex) { if (!IsDisposed) Mensaje(ErrorOperacion(ex), true); }
            finally { if (!IsDisposed) { ocupado = false; ActualizarAcciones(); } }
        }

        private void MostrarCompra(DataSet datos)
        {
            DataRow cabecera = datos.Tables[0].Rows[0];
            cargandoCampos = true;
            try
            {
                dtCarrito.Clear();
                foreach (DataRow fila in datos.Tables[1].Rows)
                    dtCarrito.Rows.Add(fila["id_producto"], fila["Codigo"], fila["Producto"], fila["cantidad"], fila["costo_unitario"]);
                compraId = (int)cabecera["id_compra"];
                versionCompra = (byte[])cabecera["version_fila"];
                estadoCompra = (string)cabecera["estado"];
                SeleccionarHistorico(cmbProveedor, "id_proveedor", (int)cabecera["id_proveedor"], (string)cabecera["Proveedor"]);
                SeleccionarHistorico(cmbTienda, "id_tienda", (int)cabecera["id_tienda"], (string)cabecera["Tienda"]);
                txtFactura.Text = (string)cabecera["numero_factura"];
                txtObservaciones.Text = Convert.ToString(cabecera["observaciones"]);
                dtpFecha.Value = (DateTime)cabecera["fecha_factura"];
                LimpiarLinea();
                modificado = lineaPendiente = false;
            }
            finally { cargandoCampos = false; }
            ActualizarTotal();
        }

        private static void SeleccionarHistorico(ComboBox combo, string campo, int id, string nombre)
        {
            if (combo.DataSource is not DataTable tabla)
            {
                tabla = new DataTable(); tabla.Columns.Add(campo, typeof(int)); tabla.Columns.Add("nombre", typeof(string));
                Enlazar(combo, tabla, campo);
            }
            if (!tabla.Columns.Contains("historico"))
            { var columna = tabla.Columns.Add("historico", typeof(bool)); columna.DefaultValue = false; foreach (DataRow fila in tabla.Rows) fila[columna] = false; }
            if (!tabla.AsEnumerable().Any(f => f.Field<int>(campo) == id))
            {
                var fila = tabla.NewRow(); fila[campo] = id; fila["nombre"] = nombre + " (inactivo)"; fila["historico"] = true; tabla.Rows.Add(fila);
            }
            combo.SelectedValue = id;
        }

        private async Task ConfirmarCompra()
        {
            if (ocupado || estadoCompra != "BORRADOR" || compraId is not int id || versionCompra == null || TieneCambios) return;
            if (MessageBox.Show(this, $"¿Confirmar la compra #{id}? Se ingresarán las unidades del detalle a {cmbTienda.Text}. El documento quedará de solo lectura.", "Confirmar compra", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            await CambiarEstado(() => CompraDatos.Confirmar(id, versionCompra), "CONFIRMADA", "Compra confirmada. El inventario fue actualizado.");
        }

        private async Task AnularCompra()
        {
            if (ocupado || compraId is not int id || versionCompra == null || estadoCompra == "ANULADA" || TieneCambios) return;
            using var dialogo = MotivoDesactivacion.ParaAnularCompra(id, estadoCompra == "CONFIRMADA");
            if (dialogo.ShowDialog(this) != DialogResult.OK) return;
            await CambiarEstado(() => CompraDatos.Anular(id, versionCompra, dialogo.Motivo), "ANULADA", "Compra anulada. Se conservaron el documento y su historial.");
        }

        private async Task CambiarEstado(Action operacion, string nuevoEstado, string mensaje)
        {
            ocupado = true; ActualizarAcciones();
            bool aplicada = false;
            try
            {
                await Task.Run(operacion);
                if (IsDisposed) return;
                estadoCompra = nuevoEstado; versionCompra = null;
                aplicada = true;
                InvalidarHistorial();
                var datos = await Task.Run(() => CompraDatos.Obtener(compraId!.Value));
                if (IsDisposed) return;
                MostrarCompra(datos);
                Mensaje(mensaje);
            }
            catch (Exception ex)
            {
                if (!IsDisposed) Mensaje(aplicada
                    ? $"La compra #{compraId} quedó {nuevoEstado}, pero no se pudo volver a cargar. Consúltala en Historial. {ErrorOperacion(ex)}"
                    : ErrorOperacion(ex), true);
            }
            finally { if (!IsDisposed) { ocupado = false; ActualizarAcciones(); } }
        }

        private void ActualizarAcciones()
        {
            bool editar = !ocupado && catalogosListos && Editable && (compraId == null || versionCompra != null);
            cmbProveedor.Enabled = cmbTienda.Enabled = editar && compraId == null;
            txtFactura.ReadOnly = txtObservaciones.ReadOnly = !editar;
            dtpFecha.Enabled = editar;
            cmbProducto.Enabled = txtCantidad.Enabled = txtPrecio.Enabled = editar;
            btnAgregarCarrito.Enabled = editar && cmbProducto.Items.Count > 0;
            btnActualizarLinea.Enabled = btnQuitarLinea.Enabled = editar && LineaSeleccionada != null;
            btnGuardar.Enabled = editar && modificado && dtCarrito.Rows.Count > 0;
            btnConfirmar.Enabled = !ocupado && catalogosListos && estadoCompra == "BORRADOR" && versionCompra != null && !TieneCambios;
            btnAnular.Enabled = !ocupado && compraId != null && versionCompra != null && estadoCompra != "ANULADA" && !TieneCambios;
            btnCancelar.Enabled = btnRecargar.Enabled = btnConsultar.Enabled = !ocupado;
            txtBuscar.Enabled = cmbEstadoFiltro.Enabled = dtpDesde.Enabled = dtpHasta.Enabled = !ocupado;
            btnAbrirCompra.Enabled = !ocupado && historialListo && dgvCompras.CurrentRow != null;
            dgvDetalleCompra.Enabled = dgvCompras.Enabled = !ocupado;
            lblEstadoCompra.Text = compraId is int id ? $"Compra #{id} · {estadoCompra}{(TieneCambios ? " · Cambios sin guardar" : "")}" : "Nueva compra · Sin guardar";
        }

        private bool PuedeDescartar() => !TieneCambios || MessageBox.Show(this,
            "Hay cambios sin guardar. ¿Descartarlos y continuar?", "Compras", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (ocupado || (e.CloseReason == CloseReason.UserClosing && !PuedeDescartar())) e.Cancel = true;
            base.OnFormClosing(e);
        }

        private void Mensaje(string texto, bool error = false)
        {
            lblMensaje.Text = texto;
            lblMensaje.ForeColor = error ? ProductoEstilo.Error : ProductoEstilo.TextoSecundario;
        }

        private static string ErrorOperacion(Exception ex) => ex is SqlException sql && sql.Number is 2601 or 2627
            ? "Ya existe una compra con ese número de factura para el proveedor." : ex.Message;

        private void ConfigurarColumnasHistorial()
        {
            foreach (DataGridViewColumn columna in dgvCompras.Columns)
            {
                columna.Visible = columna.Name is "id_compra" or "Proveedor" or "Tienda" or "numero_factura" or "fecha_factura" or "estado" or "Total" or "Usuario";
                columna.HeaderText = columna.Name switch { "id_compra" => "Compra", "numero_factura" => "Factura", "fecha_factura" => "Fecha factura", "estado" => "Estado", _ => columna.Name };
                if (columna.Name == "Total") columna.DefaultCellStyle.Format = "N2";
                if (columna.Name == "fecha_factura") columna.DefaultCellStyle.Format = "dd/MM/yyyy";
            }
        }
    }
}
