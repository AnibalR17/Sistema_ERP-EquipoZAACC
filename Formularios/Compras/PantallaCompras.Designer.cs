namespace Sistema_ERP.Vistas
{
    partial class PantallaCompras
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel mainLayout;
        private System.Windows.Forms.Label lblTitulo;

        // Panel 1: Datos de Proveedor y Factura
        private System.Windows.Forms.Panel pnlProveedor;
        private System.Windows.Forms.Label lblSeccionProv;
        private System.Windows.Forms.Label lblProveedor;
        private System.Windows.Forms.ComboBox cmbProveedor;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Label lblFactura;
        private System.Windows.Forms.TextBox txtFactura;

        // Panel 2: Seleccion de Productos
        private System.Windows.Forms.Panel pnlProducto;
        private System.Windows.Forms.Label lblSeccionProd;
        private System.Windows.Forms.Label lblProducto;
        private System.Windows.Forms.ComboBox cmbProducto;
        private System.Windows.Forms.Label lblCantidad;
        private System.Windows.Forms.TextBox txtCantidad;
        private System.Windows.Forms.Label lblPrecio;
        private System.Windows.Forms.TextBox txtPrecio;
        private System.Windows.Forms.Button btnAgregarCarrito;

        // Panel 3: Grid de Detalle
        private System.Windows.Forms.Panel pnlContenedorGrid;
        private System.Windows.Forms.DataGridView dgvDetalleCompra;

        // Panel 4: Pie (Total y Botones Guardar/Cancelar)
        private System.Windows.Forms.Panel pnlPie;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            mainLayout = new TableLayoutPanel();
            lblTitulo = new Label();
            pnlProveedor = new Panel();
            lblSeccionProv = new Label();
            lblProveedor = new Label();
            cmbProveedor = new ComboBox();
            lblFecha = new Label();
            dtpFecha = new DateTimePicker();
            lblFactura = new Label();
            txtFactura = new TextBox();
            pnlProducto = new Panel();
            lblSeccionProd = new Label();
            lblProducto = new Label();
            cmbProducto = new ComboBox();
            lblCantidad = new Label();
            txtCantidad = new TextBox();
            lblPrecio = new Label();
            txtPrecio = new TextBox();
            btnAgregarCarrito = new Button();
            pnlContenedorGrid = new Panel();
            dgvDetalleCompra = new DataGridView();
            pnlPie = new Panel();
            lblTotal = new Label();
            btnGuardar = new Button();
            btnCancelar = new Button();
            mainLayout.SuspendLayout();
            pnlProveedor.SuspendLayout();
            pnlProducto.SuspendLayout();
            pnlContenedorGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDetalleCompra).BeginInit();
            pnlPie.SuspendLayout();
            SuspendLayout();
            // 
            // mainLayout
            // 
            mainLayout.ColumnCount = 1;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.Controls.Add(lblTitulo, 0, 0);
            mainLayout.Controls.Add(pnlProveedor, 0, 1);
            mainLayout.Controls.Add(pnlProducto, 0, 2);
            mainLayout.Controls.Add(pnlContenedorGrid, 0, 3);
            mainLayout.Controls.Add(pnlPie, 0, 4);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Location = new Point(32, 34);
            mainLayout.Name = "mainLayout";
            mainLayout.RowCount = 5;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 115F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 115F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 65F));
            mainLayout.Size = new Size(1436, 830);
            mainLayout.TabIndex = 0;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblTitulo.Location = new Point(3, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(416, 50);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Registro de Compras";
            lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlProveedor
            // 
            pnlProveedor.BackColor = Color.White;
            pnlProveedor.Controls.Add(lblSeccionProv);
            pnlProveedor.Controls.Add(lblProveedor);
            pnlProveedor.Controls.Add(cmbProveedor);
            pnlProveedor.Controls.Add(lblFecha);
            pnlProveedor.Controls.Add(dtpFecha);
            pnlProveedor.Controls.Add(lblFactura);
            pnlProveedor.Controls.Add(txtFactura);
            pnlProveedor.Dock = DockStyle.Fill;
            pnlProveedor.Location = new Point(0, 50);
            pnlProveedor.Margin = new Padding(0, 0, 0, 10);
            pnlProveedor.Name = "pnlProveedor";
            pnlProveedor.Size = new Size(1436, 105);
            pnlProveedor.TabIndex = 1;
            pnlProveedor.Paint += pnlCard_Paint;
            // 
            // lblSeccionProv
            // 
            lblSeccionProv.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblSeccionProv.ForeColor = Color.FromArgb(64, 64, 64);
            lblSeccionProv.Location = new Point(16, 12);
            lblSeccionProv.Name = "lblSeccionProv";
            lblSeccionProv.Size = new Size(300, 25);
            lblSeccionProv.TabIndex = 0;
            lblSeccionProv.Text = "Datos del Proveedor y Factura";
            // 
            // lblProveedor
            // 
            lblProveedor.Font = new Font("Segoe UI", 10F);
            lblProveedor.Location = new Point(16, 48);
            lblProveedor.Name = "lblProveedor";
            lblProveedor.Size = new Size(90, 23);
            lblProveedor.TabIndex = 1;
            lblProveedor.Text = "Proveedor:";
            // 
            // cmbProveedor
            // 
            cmbProveedor.Font = new Font("Segoe UI", 10F);
            cmbProveedor.Location = new Point(110, 45);
            cmbProveedor.Name = "cmbProveedor";
            cmbProveedor.Size = new Size(320, 31);
            cmbProveedor.TabIndex = 2;
            // 
            // lblFecha
            // 
            lblFecha.Font = new Font("Segoe UI", 10F);
            lblFecha.Location = new Point(480, 48);
            lblFecha.Name = "lblFecha";
            lblFecha.Size = new Size(60, 23);
            lblFecha.TabIndex = 3;
            lblFecha.Text = "Fecha:";
            // 
            // dtpFecha
            // 
            dtpFecha.Font = new Font("Segoe UI", 10F);
            dtpFecha.Location = new Point(545, 45);
            dtpFecha.Name = "dtpFecha";
            dtpFecha.Size = new Size(260, 30);
            dtpFecha.TabIndex = 4;
            // 
            // lblFactura
            // 
            lblFactura.Font = new Font("Segoe UI", 10F);
            lblFactura.Location = new Point(16, 78);
            lblFactura.Name = "lblFactura";
            lblFactura.Size = new Size(90, 23);
            lblFactura.TabIndex = 5;
            lblFactura.Text = "N° Factura:";
            // 
            // txtFactura
            // 
            txtFactura.Font = new Font("Segoe UI", 10F);
            txtFactura.Location = new Point(110, 75);
            txtFactura.Name = "txtFactura";
            txtFactura.Size = new Size(320, 30);
            txtFactura.TabIndex = 6;
            // 
            // pnlProducto
            // 
            pnlProducto.BackColor = Color.White;
            pnlProducto.Controls.Add(lblSeccionProd);
            pnlProducto.Controls.Add(lblProducto);
            pnlProducto.Controls.Add(cmbProducto);
            pnlProducto.Controls.Add(lblCantidad);
            pnlProducto.Controls.Add(txtCantidad);
            pnlProducto.Controls.Add(lblPrecio);
            pnlProducto.Controls.Add(txtPrecio);
            pnlProducto.Controls.Add(btnAgregarCarrito);
            pnlProducto.Dock = DockStyle.Fill;
            pnlProducto.Location = new Point(0, 165);
            pnlProducto.Margin = new Padding(0, 0, 0, 10);
            pnlProducto.Name = "pnlProducto";
            pnlProducto.Size = new Size(1436, 105);
            pnlProducto.TabIndex = 2;
            pnlProducto.Paint += pnlCard_Paint;
            // 
            // lblSeccionProd
            // 
            lblSeccionProd.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblSeccionProd.ForeColor = Color.FromArgb(64, 64, 64);
            lblSeccionProd.Location = new Point(16, 12);
            lblSeccionProd.Name = "lblSeccionProd";
            lblSeccionProd.Size = new Size(300, 25);
            lblSeccionProd.TabIndex = 0;
            lblSeccionProd.Text = "Seleccionar Productos";
            // 
            // lblProducto
            // 
            lblProducto.Font = new Font("Segoe UI", 10F);
            lblProducto.Location = new Point(16, 48);
            lblProducto.Name = "lblProducto";
            lblProducto.Size = new Size(90, 23);
            lblProducto.TabIndex = 1;
            lblProducto.Text = "Producto:";
            // 
            // cmbProducto
            // 
            cmbProducto.Font = new Font("Segoe UI", 10F);
            cmbProducto.Location = new Point(110, 45);
            cmbProducto.Name = "cmbProducto";
            cmbProducto.Size = new Size(320, 31);
            cmbProducto.TabIndex = 2;
            // 
            // lblCantidad
            // 
            lblCantidad.Font = new Font("Segoe UI", 10F);
            lblCantidad.Location = new Point(480, 48);
            lblCantidad.Name = "lblCantidad";
            lblCantidad.Size = new Size(65, 23);
            lblCantidad.TabIndex = 3;
            lblCantidad.Text = "Cantidad:";
            // 
            // txtCantidad
            // 
            txtCantidad.Font = new Font("Segoe UI", 10F);
            txtCantidad.Location = new Point(545, 45);
            txtCantidad.Name = "txtCantidad";
            txtCantidad.Size = new Size(260, 30);
            txtCantidad.TabIndex = 4;
            // 
            // lblPrecio
            // 
            lblPrecio.Font = new Font("Segoe UI", 10F);
            lblPrecio.Location = new Point(16, 78);
            lblPrecio.Name = "lblPrecio";
            lblPrecio.Size = new Size(90, 23);
            lblPrecio.TabIndex = 5;
            lblPrecio.Text = "Precio:";
            // 
            // txtPrecio
            // 
            txtPrecio.Font = new Font("Segoe UI", 10F);
            txtPrecio.Location = new Point(110, 75);
            txtPrecio.Name = "txtPrecio";
            txtPrecio.Size = new Size(320, 30);
            txtPrecio.TabIndex = 6;
            // 
            // btnAgregarCarrito
            // 
            btnAgregarCarrito.BackColor = Color.FromArgb(0, 122, 204);
            btnAgregarCarrito.Cursor = Cursors.Hand;
            btnAgregarCarrito.FlatAppearance.BorderSize = 0;
            btnAgregarCarrito.FlatStyle = FlatStyle.Flat;
            btnAgregarCarrito.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAgregarCarrito.ForeColor = Color.White;
            btnAgregarCarrito.Location = new Point(830, 43);
            btnAgregarCarrito.Name = "btnAgregarCarrito";
            btnAgregarCarrito.Size = new Size(160, 36);
            btnAgregarCarrito.TabIndex = 7;
            btnAgregarCarrito.Text = "Agregar al carrito \U0001f6d2";
            btnAgregarCarrito.UseVisualStyleBackColor = false;
            // 
            // pnlContenedorGrid
            // 
            pnlContenedorGrid.BackColor = Color.White;
            pnlContenedorGrid.Controls.Add(dgvDetalleCompra);
            pnlContenedorGrid.Dock = DockStyle.Fill;
            pnlContenedorGrid.Location = new Point(0, 280);
            pnlContenedorGrid.Margin = new Padding(0, 0, 0, 10);
            pnlContenedorGrid.Name = "pnlContenedorGrid";
            pnlContenedorGrid.Padding = new Padding(15);
            pnlContenedorGrid.Size = new Size(1436, 475);
            pnlContenedorGrid.TabIndex = 3;
            pnlContenedorGrid.Paint += pnlCard_Paint;
            // 
            // dgvDetalleCompra
            // 
            dgvDetalleCompra.BackgroundColor = Color.White;
            dgvDetalleCompra.BorderStyle = BorderStyle.None;
            dgvDetalleCompra.ColumnHeadersHeight = 29;
            dgvDetalleCompra.Dock = DockStyle.Fill;
            dgvDetalleCompra.Location = new Point(15, 15);
            dgvDetalleCompra.Name = "dgvDetalleCompra";
            dgvDetalleCompra.RowHeadersWidth = 51;
            dgvDetalleCompra.Size = new Size(1406, 445);
            dgvDetalleCompra.TabIndex = 0;
            // 
            // pnlPie
            // 
            pnlPie.BackColor = Color.Transparent;
            pnlPie.Controls.Add(lblTotal);
            pnlPie.Controls.Add(btnGuardar);
            pnlPie.Controls.Add(btnCancelar);
            pnlPie.Dock = DockStyle.Fill;
            pnlPie.Location = new Point(3, 768);
            pnlPie.Name = "pnlPie";
            pnlPie.Size = new Size(1430, 59);
            pnlPie.TabIndex = 4;
            // 
            // lblTotal
            // 
            lblTotal.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTotal.Location = new Point(920, 12);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(250, 35);
            lblTotal.TabIndex = 0;
            lblTotal.Text = "Total: L. 0.00";
            lblTotal.TextAlign = ContentAlignment.MiddleRight;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = Color.FromArgb(40, 167, 69);
            btnGuardar.Cursor = Cursors.Hand;
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnGuardar.ForeColor = Color.White;
            btnGuardar.Location = new Point(1305, 12);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(110, 38);
            btnGuardar.TabIndex = 1;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            // 
            // btnCancelar
            // 
            btnCancelar.BackColor = Color.FromArgb(220, 53, 69);
            btnCancelar.Cursor = Cursors.Hand;
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnCancelar.ForeColor = Color.White;
            btnCancelar.Location = new Point(1185, 12);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(110, 38);
            btnCancelar.TabIndex = 2;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            // 
            // PantallaCompras
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(253, 253, 253);
            ClientSize = new Size(1500, 900);
            Controls.Add(mainLayout);
            Font = new Font("Segoe UI", 11F);
            Name = "PantallaCompras";
            Padding = new Padding(32, 34, 32, 36);
            Text = "Registro de Compras";
            WindowState = FormWindowState.Maximized;
            Load += PantallaCompras_Load;
            mainLayout.ResumeLayout(false);
            mainLayout.PerformLayout();
            pnlProveedor.ResumeLayout(false);
            pnlProveedor.PerformLayout();
            pnlProducto.ResumeLayout(false);
            pnlProducto.PerformLayout();
            pnlContenedorGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvDetalleCompra).EndInit();
            pnlPie.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}