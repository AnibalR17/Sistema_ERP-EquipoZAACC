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
            mainLayout = new System.Windows.Forms.TableLayoutPanel();
            lblTitulo = new System.Windows.Forms.Label();
            pnlProveedor = new System.Windows.Forms.Panel();
            lblSeccionProv = new System.Windows.Forms.Label();
            lblProveedor = new System.Windows.Forms.Label();
            cmbProveedor = new System.Windows.Forms.ComboBox();
            lblFecha = new System.Windows.Forms.Label();
            dtpFecha = new System.Windows.Forms.DateTimePicker();
            lblFactura = new System.Windows.Forms.Label();
            txtFactura = new System.Windows.Forms.TextBox();
            pnlProducto = new System.Windows.Forms.Panel();
            lblSeccionProd = new System.Windows.Forms.Label();
            lblProducto = new System.Windows.Forms.Label();
            cmbProducto = new System.Windows.Forms.ComboBox();
            lblCantidad = new System.Windows.Forms.Label();
            txtCantidad = new System.Windows.Forms.TextBox();
            lblPrecio = new System.Windows.Forms.Label();
            txtPrecio = new System.Windows.Forms.TextBox();
            btnAgregarCarrito = new System.Windows.Forms.Button();
            pnlContenedorGrid = new System.Windows.Forms.Panel();
            dgvDetalleCompra = new System.Windows.Forms.DataGridView();
            pnlPie = new System.Windows.Forms.Panel();
            lblTotal = new System.Windows.Forms.Label();
            btnGuardar = new System.Windows.Forms.Button();
            btnCancelar = new System.Windows.Forms.Button();
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
            mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            mainLayout.Controls.Add(lblTitulo, 0, 0);
            mainLayout.Controls.Add(pnlProveedor, 0, 1);
            mainLayout.Controls.Add(pnlProducto, 0, 2);
            mainLayout.Controls.Add(pnlContenedorGrid, 0, 3);
            mainLayout.Controls.Add(pnlPie, 0, 4);
            mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            mainLayout.Location = new System.Drawing.Point(32, 34);
            mainLayout.Name = "mainLayout";
            mainLayout.RowCount = 5;
            mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 115F));
            mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 115F));
            mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            mainLayout.Size = new System.Drawing.Size(1436, 830);
            mainLayout.TabIndex = 0;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            lblTitulo.Location = new System.Drawing.Point(3, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new System.Drawing.Size(416, 50);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Registro de Compras";
            lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlProveedor
            // 
            pnlProveedor.BackColor = System.Drawing.Color.White;
            pnlProveedor.Controls.Add(lblSeccionProv);
            pnlProveedor.Controls.Add(lblProveedor);
            pnlProveedor.Controls.Add(cmbProveedor);
            pnlProveedor.Controls.Add(lblFecha);
            pnlProveedor.Controls.Add(dtpFecha);
            pnlProveedor.Controls.Add(lblFactura);
            pnlProveedor.Controls.Add(txtFactura);
            pnlProveedor.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlProveedor.Location = new System.Drawing.Point(0, 50);
            pnlProveedor.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            pnlProveedor.Name = "pnlProveedor";
            pnlProveedor.Size = new System.Drawing.Size(1436, 105);
            pnlProveedor.TabIndex = 1;
            pnlProveedor.Paint += pnlCard_Paint;
            // 
            // lblSeccionProv
            // 
            lblSeccionProv.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            lblSeccionProv.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            lblSeccionProv.Location = new System.Drawing.Point(16, 12);
            lblSeccionProv.Name = "lblSeccionProv";
            lblSeccionProv.Size = new System.Drawing.Size(300, 25);
            lblSeccionProv.TabIndex = 0;
            lblSeccionProv.Text = "Datos del Proveedor y Factura";
            // 
            // lblProveedor
            // 
            lblProveedor.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblProveedor.Location = new System.Drawing.Point(16, 48);
            lblProveedor.Name = "lblProveedor";
            lblProveedor.Size = new System.Drawing.Size(90, 23);
            lblProveedor.TabIndex = 1;
            lblProveedor.Text = "Proveedor:";
            // 
            // cmbProveedor
            // 
            cmbProveedor.Font = new System.Drawing.Font("Segoe UI", 10F);
            cmbProveedor.Location = new System.Drawing.Point(110, 45);
            cmbProveedor.Name = "cmbProveedor";
            cmbProveedor.Size = new System.Drawing.Size(320, 31);
            cmbProveedor.TabIndex = 2;
            // 
            // lblFecha
            // 
            lblFecha.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblFecha.Location = new System.Drawing.Point(480, 48);
            lblFecha.Name = "lblFecha";
            lblFecha.Size = new System.Drawing.Size(60, 23);
            lblFecha.TabIndex = 3;
            lblFecha.Text = "Fecha:";
            // 
            // dtpFecha
            // 
            dtpFecha.Font = new System.Drawing.Font("Segoe UI", 10F);
            dtpFecha.Location = new System.Drawing.Point(545, 45);
            dtpFecha.Name = "dtpFecha";
            dtpFecha.Size = new System.Drawing.Size(260, 30);
            dtpFecha.TabIndex = 4;
            // 
            // lblFactura
            // 
            lblFactura.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblFactura.Location = new System.Drawing.Point(16, 78);
            lblFactura.Name = "lblFactura";
            lblFactura.Size = new System.Drawing.Size(90, 23);
            lblFactura.TabIndex = 5;
            lblFactura.Text = "N° Factura:";
            // 
            // txtFactura
            // 
            txtFactura.Font = new System.Drawing.Font("Segoe UI", 10F);
            txtFactura.Location = new System.Drawing.Point(110, 75);
            txtFactura.Name = "txtFactura";
            txtFactura.Size = new System.Drawing.Size(320, 30);
            txtFactura.TabIndex = 6;
            // 
            // pnlProducto
            // 
            pnlProducto.BackColor = System.Drawing.Color.White;
            pnlProducto.Controls.Add(lblSeccionProd);
            pnlProducto.Controls.Add(lblProducto);
            pnlProducto.Controls.Add(cmbProducto);
            pnlProducto.Controls.Add(lblCantidad);
            pnlProducto.Controls.Add(txtCantidad);
            pnlProducto.Controls.Add(lblPrecio);
            pnlProducto.Controls.Add(txtPrecio);
            pnlProducto.Controls.Add(btnAgregarCarrito);
            pnlProducto.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlProducto.Location = new System.Drawing.Point(0, 165);
            pnlProducto.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            pnlProducto.Name = "pnlProducto";
            pnlProducto.Size = new System.Drawing.Size(1436, 105);
            pnlProducto.TabIndex = 2;
            pnlProducto.Paint += pnlCard_Paint;
            // 
            // lblSeccionProd
            // 
            lblSeccionProd.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            lblSeccionProd.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            lblSeccionProd.Location = new System.Drawing.Point(16, 12);
            lblSeccionProd.Name = "lblSeccionProd";
            lblSeccionProd.Size = new System.Drawing.Size(300, 25);
            lblSeccionProd.TabIndex = 0;
            lblSeccionProd.Text = "Seleccionar Productos";
            // 
            // lblProducto
            // 
            lblProducto.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblProducto.Location = new System.Drawing.Point(16, 48);
            lblProducto.Name = "lblProducto";
            lblProducto.Size = new System.Drawing.Size(90, 23);
            lblProducto.TabIndex = 1;
            lblProducto.Text = "Producto:";
            // 
            // cmbProducto
            // 
            cmbProducto.Font = new System.Drawing.Font("Segoe UI", 10F);
            cmbProducto.Location = new System.Drawing.Point(110, 45);
            cmbProducto.Name = "cmbProducto";
            cmbProducto.Size = new System.Drawing.Size(320, 31);
            cmbProducto.TabIndex = 2;
            // 
            // lblCantidad
            // 
            lblCantidad.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblCantidad.Location = new System.Drawing.Point(480, 48);
            lblCantidad.Name = "lblCantidad";
            lblCantidad.Size = new System.Drawing.Size(65, 23);
            lblCantidad.TabIndex = 3;
            lblCantidad.Text = "Cantidad:";
            // 
            // txtCantidad
            // 
            txtCantidad.Font = new System.Drawing.Font("Segoe UI", 10F);
            txtCantidad.Location = new System.Drawing.Point(545, 45);
            txtCantidad.Name = "txtCantidad";
            txtCantidad.Size = new System.Drawing.Size(260, 30);
            txtCantidad.TabIndex = 4;
            // 
            // lblPrecio
            // 
            lblPrecio.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblPrecio.Location = new System.Drawing.Point(16, 78);
            lblPrecio.Name = "lblPrecio";
            lblPrecio.Size = new System.Drawing.Size(90, 23);
            lblPrecio.TabIndex = 5;
            lblPrecio.Text = "Precio:";
            // 
            // txtPrecio
            // 
            txtPrecio.Font = new System.Drawing.Font("Segoe UI", 10F);
            txtPrecio.Location = new System.Drawing.Point(110, 75);
            txtPrecio.Name = "txtPrecio";
            txtPrecio.Size = new System.Drawing.Size(320, 30);
            txtPrecio.TabIndex = 6;
            // 
            // btnAgregarCarrito
            // 
            btnAgregarCarrito.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
            btnAgregarCarrito.Cursor = System.Windows.Forms.Cursors.Hand;
            btnAgregarCarrito.FlatAppearance.BorderSize = 0;
            btnAgregarCarrito.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnAgregarCarrito.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btnAgregarCarrito.ForeColor = System.Drawing.Color.White;
            btnAgregarCarrito.Location = new System.Drawing.Point(830, 43);
            btnAgregarCarrito.Name = "btnAgregarCarrito";
            btnAgregarCarrito.Size = new System.Drawing.Size(160, 36);
            btnAgregarCarrito.TabIndex = 7;
            btnAgregarCarrito.Text = "Agregar al carrito \U0001f6d2";
            btnAgregarCarrito.UseVisualStyleBackColor = false;
            // 
            // pnlContenedorGrid
            // 
            pnlContenedorGrid.BackColor = System.Drawing.Color.White;
            pnlContenedorGrid.Controls.Add(dgvDetalleCompra);
            pnlContenedorGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlContenedorGrid.Location = new System.Drawing.Point(0, 280);
            pnlContenedorGrid.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            pnlContenedorGrid.Name = "pnlContenedorGrid";
            pnlContenedorGrid.Padding = new System.Windows.Forms.Padding(15);
            pnlContenedorGrid.Size = new System.Drawing.Size(1436, 475);
            pnlContenedorGrid.TabIndex = 3;
            pnlContenedorGrid.Paint += pnlCard_Paint;
            // 
            // dgvDetalleCompra
            // 
            dgvDetalleCompra.BackgroundColor = System.Drawing.Color.White;
            dgvDetalleCompra.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dgvDetalleCompra.ColumnHeadersHeight = 29;
            dgvDetalleCompra.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvDetalleCompra.Location = new System.Drawing.Point(15, 15);
            dgvDetalleCompra.Name = "dgvDetalleCompra";
            dgvDetalleCompra.RowHeadersWidth = 51;
            dgvDetalleCompra.Size = new System.Drawing.Size(1406, 445);
            dgvDetalleCompra.TabIndex = 0;
            // 
            // pnlPie
            // 
            pnlPie.BackColor = System.Drawing.Color.Transparent;
            pnlPie.Controls.Add(lblTotal);
            pnlPie.Controls.Add(btnGuardar);
            pnlPie.Controls.Add(btnCancelar);
            pnlPie.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlPie.Location = new System.Drawing.Point(3, 768);
            pnlPie.Name = "pnlPie";
            pnlPie.Size = new System.Drawing.Size(1430, 59);
            pnlPie.TabIndex = 4;
            // 
            // lblTotal
            // 
            lblTotal.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            lblTotal.Location = new System.Drawing.Point(920, 12);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new System.Drawing.Size(250, 35);
            lblTotal.TabIndex = 0;
            lblTotal.Text = "Total: L. 0.00";
            lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = System.Drawing.Color.FromArgb(40, 167, 69);
            btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnGuardar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            btnGuardar.ForeColor = System.Drawing.Color.White;
            btnGuardar.Location = new System.Drawing.Point(1305, 12);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new System.Drawing.Size(110, 38);
            btnGuardar.TabIndex = 1;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            // 
            // btnCancelar
            // 
            btnCancelar.BackColor = System.Drawing.Color.FromArgb(220, 53, 69);
            btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnCancelar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            btnCancelar.ForeColor = System.Drawing.Color.White;
            btnCancelar.Location = new System.Drawing.Point(1185, 12);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new System.Drawing.Size(110, 38);
            btnCancelar.TabIndex = 2;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            // 
            // PantallaCompras
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(253, 253, 253);
            ClientSize = new System.Drawing.Size(1500, 900);
            Controls.Add(mainLayout);
            Font = new System.Drawing.Font("Segoe UI", 11F);
            Name = "PantallaCompras";
            Padding = new System.Windows.Forms.Padding(32, 34, 32, 36);
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