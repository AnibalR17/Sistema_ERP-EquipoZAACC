namespace Sistema_ERP.Vistas
{
    partial class PantallaProductos
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel mainLayout;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel pnlInfo;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.FlowLayoutPanel pnlAcciones;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnDesactivar;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Panel pnlContenedorGrid;
        private System.Windows.Forms.DataGridView dgvProductos;

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
            pnlInfo = new Panel();
            lblInfo = new Label();
            pnlAcciones = new FlowLayoutPanel();
            btnNuevo = new Button();
            btnEditar = new Button();
            btnDesactivar = new Button();
            txtBuscar = new TextBox();
            pnlContenedorGrid = new Panel();
            dgvProductos = new DataGridView();
            mainLayout.SuspendLayout();
            pnlInfo.SuspendLayout();
            pnlAcciones.SuspendLayout();
            pnlContenedorGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProductos).BeginInit();
            SuspendLayout();
            //
            // mainLayout
            //
            mainLayout.ColumnCount = 1;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.Controls.Add(lblTitulo, 0, 0);
            mainLayout.Controls.Add(pnlInfo, 0, 1);
            mainLayout.Controls.Add(pnlAcciones, 0, 2);
            mainLayout.Controls.Add(pnlContenedorGrid, 0, 3);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Location = new Point(32, 34);
            mainLayout.Name = "mainLayout";
            mainLayout.RowCount = 4;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.Size = new Size(1436, 830);
            mainLayout.TabIndex = 0;
            //
            // lblTitulo
            //
            lblTitulo.AutoSize = true;
            lblTitulo.Dock = DockStyle.Fill;
            lblTitulo.Font = new Font("Segoe UI", 27F, FontStyle.Bold);
            lblTitulo.Location = new Point(0, 0);
            lblTitulo.Margin = new Padding(0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(1436, 60);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Catalogo de Productos";
            lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
            //
            // pnlInfo
            //
            pnlInfo.BackColor = Color.FromArgb(243, 243, 243);
            pnlInfo.Controls.Add(lblInfo);
            pnlInfo.Dock = DockStyle.Fill;
            pnlInfo.Location = new Point(0, 60);
            pnlInfo.Margin = new Padding(0, 0, 0, 15);
            pnlInfo.Name = "pnlInfo";
            pnlInfo.Size = new Size(1436, 45);
            pnlInfo.TabIndex = 1;
            pnlInfo.Paint += pnlInfo_Paint;
            //
            // lblInfo
            //
            lblInfo.Dock = DockStyle.Fill;
            lblInfo.Font = new Font("Segoe UI", 11F);
            lblInfo.ForeColor = Color.FromArgb(125, 125, 125);
            lblInfo.Location = new Point(0, 0);
            lblInfo.Name = "lblInfo";
            lblInfo.Padding = new Padding(16, 0, 0, 0);
            lblInfo.Size = new Size(1436, 45);
            lblInfo.TabIndex = 0;
            lblInfo.Text = "Gestion de articulos, precios de venta, costos y control de stock actual.";
            lblInfo.TextAlign = ContentAlignment.MiddleLeft;
            //
            // pnlAcciones
            //
            pnlAcciones.Controls.Add(btnNuevo);
            pnlAcciones.Controls.Add(btnEditar);
            pnlAcciones.Controls.Add(btnDesactivar);
            pnlAcciones.Controls.Add(txtBuscar);
            pnlAcciones.Dock = DockStyle.Fill;
            pnlAcciones.Location = new Point(0, 120);
            pnlAcciones.Margin = new Padding(0, 0, 0, 10);
            pnlAcciones.Name = "pnlAcciones";
            pnlAcciones.Size = new Size(1436, 40);
            pnlAcciones.TabIndex = 2;
            //
            // btnNuevo
            //
            btnNuevo.BackColor = Color.FromArgb(255, 255, 255);
            btnNuevo.Cursor = Cursors.Hand;
            btnNuevo.FlatAppearance.BorderSize = 0;
            btnNuevo.FlatStyle = FlatStyle.Flat;
            btnNuevo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnNuevo.Location = new Point(0, 0);
            btnNuevo.Margin = new Padding(0, 0, 10, 0);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(120, 38);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo";
            btnNuevo.UseVisualStyleBackColor = false;
            btnNuevo.MouseEnter += Btn_MouseEnter;
            btnNuevo.MouseLeave += Btn_MouseLeave;
            //
            // btnEditar
            //
            btnEditar.BackColor = Color.FromArgb(255, 255, 255);
            btnEditar.Cursor = Cursors.Hand;
            btnEditar.FlatAppearance.BorderSize = 0;
            btnEditar.FlatStyle = FlatStyle.Flat;
            btnEditar.Font = new Font("Segoe UI", 11F);
            btnEditar.Location = new Point(130, 0);
            btnEditar.Margin = new Padding(0, 0, 10, 0);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new Size(120, 38);
            btnEditar.TabIndex = 1;
            btnEditar.Text = "Editar";
            btnEditar.UseVisualStyleBackColor = false;
            btnEditar.MouseEnter += Btn_MouseEnter;
            btnEditar.MouseLeave += Btn_MouseLeave;
            //
            // btnDesactivar
            //
            btnDesactivar.BackColor = Color.FromArgb(255, 255, 255);
            btnDesactivar.Cursor = Cursors.Hand;
            btnDesactivar.FlatAppearance.BorderSize = 0;
            btnDesactivar.FlatStyle = FlatStyle.Flat;
            btnDesactivar.Font = new Font("Segoe UI", 11F);
            btnDesactivar.Location = new Point(260, 0);
            btnDesactivar.Margin = new Padding(0, 0, 20, 0);
            btnDesactivar.Name = "btnDesactivar";
            btnDesactivar.Size = new Size(120, 38);
            btnDesactivar.TabIndex = 2;
            btnDesactivar.Text = "Desactivar";
            btnDesactivar.UseVisualStyleBackColor = false;
            btnDesactivar.MouseEnter += Btn_MouseEnter;
            btnDesactivar.MouseLeave += Btn_MouseLeave;
            //
            // txtBuscar
            //
            txtBuscar.Font = new Font("Segoe UI", 12F);
            txtBuscar.Location = new Point(400, 4);
            txtBuscar.Margin = new Padding(0, 4, 0, 0);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.PlaceholderText = "Buscar producto por codigo o nombre...";
            txtBuscar.Size = new Size(400, 34);
            txtBuscar.TabIndex = 3;
            //
            // pnlContenedorGrid
            //
            pnlContenedorGrid.BackColor = Color.White;
            pnlContenedorGrid.Controls.Add(dgvProductos);
            pnlContenedorGrid.Dock = DockStyle.Fill;
            pnlContenedorGrid.Location = new Point(0, 170);
            pnlContenedorGrid.Margin = new Padding(0);
            pnlContenedorGrid.Name = "pnlContenedorGrid";
            pnlContenedorGrid.Padding = new Padding(20);
            pnlContenedorGrid.Size = new Size(1436, 660);
            pnlContenedorGrid.TabIndex = 3;
            pnlContenedorGrid.Paint += pnlContenedorGrid_Paint;
            //
            // dgvProductos
            //
            dgvProductos.BackgroundColor = Color.White;
            dgvProductos.BorderStyle = BorderStyle.None;
            dgvProductos.ColumnHeadersHeight = 29;
            dgvProductos.Dock = DockStyle.Fill;
            dgvProductos.Location = new Point(20, 20);
            dgvProductos.Name = "dgvProductos";
            dgvProductos.RowHeadersWidth = 51;
            dgvProductos.Size = new Size(1396, 620);
            dgvProductos.TabIndex = 0;
            //
            // PantallaProductos
            //
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(253, 253, 253);
            ClientSize = new Size(1500, 900);
            Controls.Add(mainLayout);
            Font = new Font("Segoe UI", 11F);
            Name = "PantallaProductos";
            Padding = new Padding(32, 34, 32, 36);
            Text = "Catalogo de Productos";
            WindowState = FormWindowState.Maximized;
            Load += PantallaProductos_Load;
            mainLayout.ResumeLayout(false);
            mainLayout.PerformLayout();
            pnlInfo.ResumeLayout(false);
            pnlAcciones.ResumeLayout(false);
            pnlAcciones.PerformLayout();
            pnlContenedorGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvProductos).EndInit();
            ResumeLayout(false);
        }
    }
}