#nullable enable
namespace Sistema_ERP.Vistas
{
    partial class PantallaCompras
    {
        private System.ComponentModel.IContainer? components;
        private TableLayoutPanel mainLayout = null!, encabezado = null!, capturaLayout = null!, datosCompra = null!, datosProducto = null!, historialLayout = null!, filtrosHistorial = null!;
        private TabControl pestanasCompras = null!;
        private TabPage tabCaptura = null!, tabHistorial = null!;
        private FlowLayoutPanel accionesDetalle = null!, accionesCompra = null!, accionesHistorial = null!;
        private Label lblTitulo = null!, lblEstadoCompra = null!, lblMensaje = null!, lblTotal = null!;
        private Label lblSeccionProv = null!, lblProveedor = null!, lblTienda = null!, lblFecha = null!, lblFactura = null!, lblObservaciones = null!;
        private Label lblSeccionProd = null!, lblProducto = null!, lblCantidad = null!, lblPrecio = null!;
        private Label lblBuscar = null!, lblEstadoFiltro = null!, lblDesde = null!, lblHasta = null!;
        private ComboBox cmbProveedor = null!, cmbTienda = null!, cmbProducto = null!, cmbEstadoFiltro = null!;
        private DateTimePicker dtpFecha = null!, dtpDesde = null!, dtpHasta = null!;
        private TextBox txtFactura = null!, txtObservaciones = null!, txtCantidad = null!, txtPrecio = null!, txtBuscar = null!;
        private Button btnAgregarCarrito = null!, btnActualizarLinea = null!, btnQuitarLinea = null!, btnGuardar = null!, btnCancelar = null!, btnConfirmar = null!, btnAnular = null!;
        private Button btnRecargar = null!, btnConsultar = null!, btnAbrirCompra = null!;
        private PanelDisenoInventario pnlContenedorGrid = null!, pnlListado = null!;
        private DataGridView dgvDetalleCompra = null!, dgvCompras = null!;
        private DataGridViewTextBoxColumn columnaProductoId = null!, columnaCodigo = null!, columnaProducto = null!, columnaCantidad = null!, columnaCosto = null!, columnaSubtotal = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            mainLayout = new TableLayoutPanel(); encabezado = new TableLayoutPanel(); capturaLayout = new TableLayoutPanel();
            datosCompra = new TableLayoutPanel(); datosProducto = new TableLayoutPanel(); historialLayout = new TableLayoutPanel(); filtrosHistorial = new TableLayoutPanel();
            pestanasCompras = new Sistema_ERP.PestanasTematicas(); tabCaptura = new TabPage(); tabHistorial = new TabPage();
            accionesDetalle = new FlowLayoutPanel(); accionesCompra = new FlowLayoutPanel(); accionesHistorial = new FlowLayoutPanel();
            lblTitulo = new Label(); lblEstadoCompra = new Label(); lblMensaje = new Label(); lblTotal = new Label();
            lblSeccionProv = new Label(); lblProveedor = new Label(); lblTienda = new Label(); lblFecha = new Label(); lblFactura = new Label(); lblObservaciones = new Label();
            lblSeccionProd = new Label(); lblProducto = new Label(); lblCantidad = new Label(); lblPrecio = new Label();
            lblBuscar = new Label(); lblEstadoFiltro = new Label(); lblDesde = new Label(); lblHasta = new Label();
            cmbProveedor = new ComboBox(); cmbTienda = new ComboBox(); cmbProducto = new ComboBox(); cmbEstadoFiltro = new ComboBox();
            dtpFecha = new DateTimePicker(); dtpDesde = new DateTimePicker(); dtpHasta = new DateTimePicker();
            txtFactura = new TextBox(); txtObservaciones = new TextBox(); txtCantidad = new TextBox(); txtPrecio = new TextBox(); txtBuscar = new TextBox();
            btnAgregarCarrito = new Button(); btnActualizarLinea = new Button(); btnQuitarLinea = new Button(); btnGuardar = new Button(); btnCancelar = new Button(); btnConfirmar = new Button(); btnAnular = new Button();
            btnRecargar = new Button(); btnConsultar = new Button(); btnAbrirCompra = new Button();
            pnlContenedorGrid = new PanelDisenoInventario(); pnlListado = new PanelDisenoInventario();
            dgvDetalleCompra = new DataGridView(); dgvCompras = new DataGridView();
            columnaProductoId = new DataGridViewTextBoxColumn(); columnaCodigo = new DataGridViewTextBoxColumn(); columnaProducto = new DataGridViewTextBoxColumn();
            columnaCantidad = new DataGridViewTextBoxColumn(); columnaCosto = new DataGridViewTextBoxColumn(); columnaSubtotal = new DataGridViewTextBoxColumn();
            SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDetalleCompra).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvCompras).BeginInit();
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 11F);
            BackColor = Color.White; ForeColor = Color.FromArgb(24, 24, 24);
            Name = "PantallaCompras"; Text = "Compras y detalles";
            ClientSize = new Size(1180, 900);
            Padding = new Padding(32, 34, 32, 36); AutoScroll = true;
            mainLayout.Name = "mainLayout"; mainLayout.Dock = DockStyle.Fill;
            mainLayout.MinimumSize = new Size(690, 440);
            mainLayout.ColumnCount = 1; mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            mainLayout.RowCount = 3;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            encabezado.Dock = DockStyle.Fill; encabezado.ColumnCount = 2;
            encabezado.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            encabezado.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 134));
            encabezado.RowCount = 1; encabezado.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            encabezado.Margin = Padding.Empty;
            lblTitulo.Name = "lblTitulo"; lblTitulo.Text = "Compras y detalles";
            lblTitulo.Font = new Font("Segoe UI", 27F, FontStyle.Bold);
            lblTitulo.Dock = DockStyle.Fill; lblTitulo.TextAlign = ContentAlignment.MiddleLeft; lblTitulo.Margin = Padding.Empty;
            btnRecargar.Name = "btnRecargar"; btnRecargar.Text = "Recargar";
            btnRecargar.Anchor = AnchorStyles.Right; btnRecargar.Size = new Size(126, 44);
            encabezado.Controls.Add(lblTitulo, 0, 0); encabezado.Controls.Add(btnRecargar, 1, 0);
            mainLayout.Controls.Add(encabezado, 0, 0);
            pestanasCompras.Name = "pestanasCompras"; pestanasCompras.Dock = DockStyle.Fill; pestanasCompras.Margin = Padding.Empty;
            pestanasCompras.TabPages.AddRange(new[] { tabCaptura, tabHistorial });
            tabCaptura.Name = "tabCaptura"; tabCaptura.Text = "Captura";
            tabHistorial.Name = "tabHistorial"; tabHistorial.Text = "Historial";
            tabCaptura.BackColor = tabHistorial.BackColor = Color.White;
            tabCaptura.AutoScroll = tabHistorial.AutoScroll = true;
            tabCaptura.Padding = tabHistorial.Padding = new Padding(12, 14, 12, 12);
            mainLayout.Controls.Add(pestanasCompras, 0, 1);
            capturaLayout.Dock = DockStyle.Top; capturaLayout.AutoSize = true;
            capturaLayout.ColumnCount = 1; capturaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            capturaLayout.RowCount = 8;
            capturaLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            capturaLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
            capturaLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 96));
            capturaLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            capturaLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 210));
            capturaLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
            capturaLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            capturaLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 12));
            lblEstadoCompra.Name = "lblEstadoCompra"; lblEstadoCompra.Text = "Nueva compra · Borrador";
            lblEstadoCompra.Dock = DockStyle.Fill; lblEstadoCompra.Font = new Font("Segoe UI", 11F, FontStyle.Bold); lblEstadoCompra.Margin = Padding.Empty;
            capturaLayout.Controls.Add(lblEstadoCompra, 0, 0);
            datosCompra.Dock = DockStyle.Fill; datosCompra.Margin = Padding.Empty; datosCompra.ColumnCount = 3;
            datosCompra.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.334F));
            datosCompra.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            datosCompra.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            datosCompra.RowCount = 5;
            datosCompra.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            datosCompra.RowStyles.Add(new RowStyle(SizeType.Absolute, 23));
            datosCompra.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            datosCompra.RowStyles.Add(new RowStyle(SizeType.Absolute, 23));
            datosCompra.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            lblSeccionProv.Text = "Proveedor y factura"; lblSeccionProv.Font = new Font("Segoe UI", 11F, FontStyle.Bold); lblSeccionProv.Dock = DockStyle.Fill;
            datosCompra.Controls.Add(lblSeccionProv, 0, 0); datosCompra.SetColumnSpan(lblSeccionProv, 3);
            lblProveedor.Text = "Proveedor *"; lblTienda.Text = "Tienda de ingreso *"; lblFecha.Text = "Fecha de factura *";
            lblFactura.Text = "Número de factura *"; lblObservaciones.Text = "Observaciones";
            datosCompra.Controls.Add(lblProveedor, 0, 1); datosCompra.Controls.Add(cmbProveedor, 0, 2);
            datosCompra.Controls.Add(lblTienda, 1, 1); datosCompra.Controls.Add(cmbTienda, 1, 2);
            datosCompra.Controls.Add(lblFecha, 2, 1); datosCompra.Controls.Add(dtpFecha, 2, 2);
            datosCompra.Controls.Add(lblFactura, 0, 3); datosCompra.Controls.Add(txtFactura, 0, 4);
            datosCompra.Controls.Add(lblObservaciones, 1, 3); datosCompra.SetColumnSpan(lblObservaciones, 2);
            datosCompra.Controls.Add(txtObservaciones, 1, 4); datosCompra.SetColumnSpan(txtObservaciones, 2);
            capturaLayout.Controls.Add(datosCompra, 0, 1);
            datosProducto.Dock = DockStyle.Fill; datosProducto.Margin = new Padding(0, 8, 0, 0); datosProducto.ColumnCount = 3;
            datosProducto.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            datosProducto.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            datosProducto.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            datosProducto.RowCount = 3;
            datosProducto.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            datosProducto.RowStyles.Add(new RowStyle(SizeType.Absolute, 23));
            datosProducto.RowStyles.Add(new RowStyle(SizeType.Absolute, 37));
            lblSeccionProd.Text = "Detalle de productos"; lblSeccionProd.Font = new Font("Segoe UI", 11F, FontStyle.Bold); lblSeccionProd.Dock = DockStyle.Fill;
            datosProducto.Controls.Add(lblSeccionProd, 0, 0); datosProducto.SetColumnSpan(lblSeccionProd, 3);
            lblProducto.Text = "Producto *"; lblCantidad.Text = "Cantidad entera *"; lblPrecio.Text = "Costo unitario (L.) *";
            datosProducto.Controls.Add(lblProducto, 0, 1); datosProducto.Controls.Add(cmbProducto, 0, 2);
            datosProducto.Controls.Add(lblCantidad, 1, 1); datosProducto.Controls.Add(txtCantidad, 1, 2);
            datosProducto.Controls.Add(lblPrecio, 2, 1); datosProducto.Controls.Add(txtPrecio, 2, 2);
            capturaLayout.Controls.Add(datosProducto, 0, 2);
            foreach (var etiqueta in new[] { lblProveedor, lblTienda, lblFecha, lblFactura, lblObservaciones, lblProducto, lblCantidad, lblPrecio, lblBuscar, lblEstadoFiltro, lblDesde, lblHasta })
            {
                etiqueta.Dock = DockStyle.Fill; etiqueta.Font = new Font("Segoe UI", 10F); etiqueta.Margin = new Padding(0, 0, 12, 0);
            }
            foreach (Control campo in new Control[] { cmbProveedor, cmbTienda, dtpFecha, txtFactura, txtObservaciones, cmbProducto, txtCantidad, txtPrecio, txtBuscar, cmbEstadoFiltro, dtpDesde, dtpHasta })
            {
                campo.Dock = DockStyle.Top; campo.Margin = new Padding(0, 0, 12, 8);
            }
            cmbProveedor.Name = "cmbProveedor"; cmbTienda.Name = "cmbTienda"; cmbProducto.Name = "cmbProducto"; cmbEstadoFiltro.Name = "cmbEstadoFiltro";
            cmbProveedor.DropDownStyle = cmbTienda.DropDownStyle = cmbProducto.DropDownStyle = cmbEstadoFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProveedor.DropDownWidth = cmbTienda.DropDownWidth = cmbProducto.DropDownWidth = 400;
            dtpFecha.Name = "dtpFecha"; dtpDesde.Name = "dtpDesde"; dtpHasta.Name = "dtpHasta";
            dtpFecha.Format = dtpDesde.Format = dtpHasta.Format = DateTimePickerFormat.Short;
            dtpDesde.Value = DateTime.Today.AddDays(-30); dtpHasta.Value = DateTime.Today;
            cmbEstadoFiltro.Items.AddRange(new object[] { "Todos", "BORRADOR", "CONFIRMADA", "ANULADA" });
            cmbEstadoFiltro.SelectedIndex = 0;
            txtFactura.Name = "txtFactura"; txtObservaciones.Name = "txtObservaciones";
            txtCantidad.Name = "txtCantidad"; txtPrecio.Name = "txtPrecio"; txtBuscar.Name = "txtBuscar";
            txtFactura.MaxLength = 50; txtObservaciones.MaxLength = 500; txtCantidad.MaxLength = 10; txtPrecio.MaxLength = 19; txtBuscar.MaxLength = 150;
            txtCantidad.Text = "1"; txtPrecio.PlaceholderText = "0.00";
            txtObservaciones.PlaceholderText = "Opcional, hasta 500 caracteres";
            accionesDetalle.Dock = DockStyle.Fill; accionesDetalle.AutoSize = true; accionesDetalle.Margin = new Padding(0, 0, 0, 10);
            btnAgregarCarrito.Name = "btnAgregarCarrito"; btnAgregarCarrito.Text = "Agregar producto";
            btnActualizarLinea.Name = "btnActualizarLinea"; btnActualizarLinea.Text = "Actualizar línea";
            btnQuitarLinea.Name = "btnQuitarLinea"; btnQuitarLinea.Text = "Quitar línea";
            accionesDetalle.Controls.AddRange(new Control[] { btnAgregarCarrito, btnActualizarLinea, btnQuitarLinea });
            capturaLayout.Controls.Add(accionesDetalle, 0, 3);
            pnlContenedorGrid.Name = "pnlContenedorGrid"; pnlListado.Name = "pnlListado";
            foreach (var panel in new[] { pnlContenedorGrid, pnlListado })
            {
                panel.Dock = DockStyle.Fill; panel.Radio = 30; panel.MostrarSombra = false; panel.ColorBorde = Color.FromArgb(247, 247, 247);
                panel.Padding = new Padding(16); panel.Margin = Padding.Empty;
            }
            dgvDetalleCompra.Name = "dgvDetalleCompra"; dgvCompras.Name = "dgvCompras";
            foreach (var tabla in new[] { dgvDetalleCompra, dgvCompras })
            {
                tabla.Dock = DockStyle.Fill; tabla.ReadOnly = true; tabla.AllowUserToAddRows = false; tabla.AllowUserToDeleteRows = false;
                tabla.MultiSelect = false; tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                tabla.RowHeadersVisible = false; tabla.BorderStyle = BorderStyle.None; tabla.BackgroundColor = Color.White;
                tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; tabla.RowTemplate.Height = 42; tabla.ColumnHeadersHeight = 42;
                tabla.EnableHeadersVisualStyles = false; tabla.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                tabla.GridColor = Color.FromArgb(247, 247, 247); tabla.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                tabla.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.White, ForeColor = Color.FromArgb(24, 24, 24), Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
                tabla.DefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 243, 243); tabla.DefaultCellStyle.SelectionForeColor = Color.FromArgb(24, 24, 24);
            }
            dgvDetalleCompra.AutoGenerateColumns = false;
            columnaProductoId.Name = columnaProductoId.DataPropertyName = "id_producto"; columnaProductoId.Visible = false;
            columnaCodigo.Name = columnaCodigo.DataPropertyName = "Codigo"; columnaCodigo.HeaderText = "Código"; columnaCodigo.MinimumWidth = 90; columnaCodigo.FillWeight = 90;
            columnaProducto.Name = columnaProducto.DataPropertyName = "Producto"; columnaProducto.HeaderText = "Producto"; columnaProducto.MinimumWidth = 170; columnaProducto.FillWeight = 200;
            columnaCantidad.Name = columnaCantidad.DataPropertyName = "Cantidad"; columnaCantidad.HeaderText = "Cantidad"; columnaCantidad.MinimumWidth = 75; columnaCantidad.FillWeight = 65;
            columnaCosto.Name = columnaCosto.DataPropertyName = "Costo"; columnaCosto.HeaderText = "Costo unitario (L.)"; columnaCosto.MinimumWidth = 120;
            columnaSubtotal.Name = columnaSubtotal.DataPropertyName = "Subtotal"; columnaSubtotal.HeaderText = "Subtotal (L.)"; columnaSubtotal.MinimumWidth = 110;
            columnaCantidad.DefaultCellStyle.Alignment = columnaCosto.DefaultCellStyle.Alignment = columnaSubtotal.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            columnaCantidad.DefaultCellStyle.Format = "N0"; columnaCosto.DefaultCellStyle.Format = columnaSubtotal.DefaultCellStyle.Format = "N2";
            dgvDetalleCompra.Columns.AddRange(new DataGridViewColumn[] { columnaProductoId, columnaCodigo, columnaProducto, columnaCantidad, columnaCosto, columnaSubtotal });
            columnaProductoId.Visible = false;
            pnlContenedorGrid.Controls.Add(dgvDetalleCompra); capturaLayout.Controls.Add(pnlContenedorGrid, 0, 4);
            lblTotal.Name = "lblTotal"; lblTotal.Text = "Total: L. 0.00"; lblTotal.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTotal.Dock = DockStyle.Fill; lblTotal.TextAlign = ContentAlignment.MiddleRight;
            capturaLayout.Controls.Add(lblTotal, 0, 5);
            accionesCompra.Dock = DockStyle.Fill; accionesCompra.AutoSize = true; accionesCompra.Margin = Padding.Empty;
            btnGuardar.Name = "btnGuardar"; btnGuardar.Text = "Guardar borrador";
            btnConfirmar.Name = "btnConfirmar"; btnConfirmar.Text = "Confirmar compra";
            btnAnular.Name = "btnAnular"; btnAnular.Text = "Anular compra";
            btnCancelar.Name = "btnCancelar"; btnCancelar.Text = "Nuevo";
            accionesCompra.Controls.AddRange(new Control[] { btnGuardar, btnConfirmar, btnAnular, btnCancelar });
            capturaLayout.Controls.Add(accionesCompra, 0, 6); tabCaptura.Controls.Add(capturaLayout);
            historialLayout.Dock = DockStyle.Fill; historialLayout.MinimumSize = new Size(650, 480);
            historialLayout.ColumnCount = 1; historialLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); historialLayout.RowCount = 3;
            historialLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
            historialLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            historialLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            filtrosHistorial.Dock = DockStyle.Fill; filtrosHistorial.ColumnCount = 4; filtrosHistorial.RowCount = 2;
            filtrosHistorial.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            filtrosHistorial.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
            filtrosHistorial.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
            filtrosHistorial.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
            filtrosHistorial.RowStyles.Add(new RowStyle(SizeType.Absolute, 24)); filtrosHistorial.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            lblBuscar.Text = "Buscar factura o proveedor"; lblEstadoFiltro.Text = "Estado"; lblDesde.Text = "Factura desde"; lblHasta.Text = "Factura hasta";
            txtBuscar.PlaceholderText = "Factura, proveedor, tienda...";
            filtrosHistorial.Controls.Add(lblBuscar, 0, 0); filtrosHistorial.Controls.Add(txtBuscar, 0, 1);
            filtrosHistorial.Controls.Add(lblEstadoFiltro, 1, 0); filtrosHistorial.Controls.Add(cmbEstadoFiltro, 1, 1);
            filtrosHistorial.Controls.Add(lblDesde, 2, 0); filtrosHistorial.Controls.Add(dtpDesde, 2, 1);
            filtrosHistorial.Controls.Add(lblHasta, 3, 0); filtrosHistorial.Controls.Add(dtpHasta, 3, 1);
            historialLayout.Controls.Add(filtrosHistorial, 0, 0);
            accionesHistorial.Dock = DockStyle.Fill; accionesHistorial.AutoSize = true; accionesHistorial.Margin = new Padding(0, 0, 0, 12);
            btnConsultar.Name = "btnConsultar"; btnConsultar.Text = "Consultar";
            btnAbrirCompra.Name = "btnAbrirCompra"; btnAbrirCompra.Text = "Abrir detalle";
            accionesHistorial.Controls.AddRange(new Control[] { btnConsultar, btnAbrirCompra }); historialLayout.Controls.Add(accionesHistorial, 0, 1);
            pnlListado.Controls.Add(dgvCompras); historialLayout.Controls.Add(pnlListado, 0, 2); tabHistorial.Controls.Add(historialLayout);
            foreach (var boton in new[] { btnAgregarCarrito, btnActualizarLinea, btnQuitarLinea, btnGuardar, btnConfirmar, btnAnular, btnCancelar, btnConsultar, btnAbrirCompra })
            {
                boton.Size = new Size(156, 44); boton.Margin = new Padding(0, 4, 8, 0); boton.FlatStyle = FlatStyle.Flat;
                boton.BackColor = Color.White; boton.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
                boton.Cursor = Cursors.Hand; boton.UseVisualStyleBackColor = false;
            }
            btnAgregarCarrito.BackColor = btnGuardar.BackColor = Color.FromArgb(24, 24, 24);
            btnAgregarCarrito.ForeColor = btnGuardar.ForeColor = Color.White;
            lblMensaje.Name = "lblMensaje"; lblMensaje.Text = "Recarga los catálogos para comenzar.";
            lblMensaje.ForeColor = Color.FromArgb(125, 125, 125); lblMensaje.Dock = DockStyle.Fill;
            lblMensaje.TextAlign = ContentAlignment.MiddleLeft; lblMensaje.Margin = new Padding(0, 8, 0, 0);
            mainLayout.Controls.Add(lblMensaje, 0, 2); Controls.Add(mainLayout);
            encabezado.TabIndex = 0; pestanasCompras.TabIndex = 1; lblMensaje.TabIndex = 2;
            lblTitulo.TabStop = false; btnRecargar.TabIndex = 0;
            tabCaptura.TabIndex = 0; tabHistorial.TabIndex = 1;
            lblEstadoCompra.TabStop = false; datosCompra.TabIndex = 0; datosProducto.TabIndex = 1;
            accionesDetalle.TabIndex = 2; pnlContenedorGrid.TabIndex = 3; accionesCompra.TabIndex = 4;
            cmbProveedor.TabIndex = 0; cmbTienda.TabIndex = 1; dtpFecha.TabIndex = 2;
            txtFactura.TabIndex = 3; txtObservaciones.TabIndex = 4;
            cmbProducto.TabIndex = 0; txtCantidad.TabIndex = 1; txtPrecio.TabIndex = 2;
            btnAgregarCarrito.TabIndex = 0; btnActualizarLinea.TabIndex = 1; btnQuitarLinea.TabIndex = 2;
            dgvDetalleCompra.TabIndex = 0;
            btnGuardar.TabIndex = 0; btnConfirmar.TabIndex = 1; btnAnular.TabIndex = 2; btnCancelar.TabIndex = 3;
            filtrosHistorial.TabIndex = 0; accionesHistorial.TabIndex = 1; pnlListado.TabIndex = 2;
            txtBuscar.TabIndex = 0; cmbEstadoFiltro.TabIndex = 1; dtpDesde.TabIndex = 2; dtpHasta.TabIndex = 3;
            btnConsultar.TabIndex = 0; btnAbrirCompra.TabIndex = 1; dgvCompras.TabIndex = 0;
            Load += PantallaCompras_Load;
            ((System.ComponentModel.ISupportInitialize)dgvDetalleCompra).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvCompras).EndInit();
            ResumeLayout(true);
        }
    }
}
