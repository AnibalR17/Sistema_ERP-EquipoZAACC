namespace Sistema_ERP.Vistas
{
    partial class PantallaProveedores
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel mainLayout;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel pnlInfo;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.FlowLayoutPanel pnlAcciones;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Panel pnlContenedorGrid;
        private System.Windows.Forms.DataGridView dgvProveedores;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            mainLayout = new System.Windows.Forms.TableLayoutPanel();
            lblTitulo = new System.Windows.Forms.Label();
            pnlInfo = new System.Windows.Forms.Panel();
            lblInfo = new System.Windows.Forms.Label();
            pnlAcciones = new System.Windows.Forms.FlowLayoutPanel();
            btnNuevo = new System.Windows.Forms.Button();
            btnEditar = new System.Windows.Forms.Button();
            txtBuscar = new System.Windows.Forms.TextBox();
            pnlContenedorGrid = new System.Windows.Forms.Panel();
            dgvProveedores = new System.Windows.Forms.DataGridView();
            mainLayout.SuspendLayout();
            pnlInfo.SuspendLayout();
            pnlAcciones.SuspendLayout();
            pnlContenedorGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProveedores).BeginInit();
            SuspendLayout();
            //
            // mainLayout
            //
            mainLayout.ColumnCount = 1;
            mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            mainLayout.Controls.Add(lblTitulo, 0, 0);
            mainLayout.Controls.Add(pnlInfo, 0, 1);
            mainLayout.Controls.Add(pnlAcciones, 0, 2);
            mainLayout.Controls.Add(pnlContenedorGrid, 0, 3);
            mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            mainLayout.Location = new System.Drawing.Point(32, 34);
            mainLayout.Name = "mainLayout";
            mainLayout.RowCount = 4;
            mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            mainLayout.Size = new System.Drawing.Size(1436, 830);
            mainLayout.TabIndex = 0;
            //
            // lblTitulo
            //
            lblTitulo.AutoSize = true;
            lblTitulo.Dock = System.Windows.Forms.DockStyle.Fill;
            lblTitulo.Font = new System.Drawing.Font("Segoe UI", 27F, System.Drawing.FontStyle.Bold);
            lblTitulo.Location = new System.Drawing.Point(0, 0);
            lblTitulo.Margin = new System.Windows.Forms.Padding(0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new System.Drawing.Size(1436, 60);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Proveedores";
            lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlInfo
            //
            pnlInfo.BackColor = System.Drawing.Color.FromArgb(243, 243, 243);
            pnlInfo.Controls.Add(lblInfo);
            pnlInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlInfo.Location = new System.Drawing.Point(0, 60);
            pnlInfo.Margin = new System.Windows.Forms.Padding(0, 0, 0, 15);
            pnlInfo.Name = "pnlInfo";
            pnlInfo.Size = new System.Drawing.Size(1436, 45);
            pnlInfo.TabIndex = 1;
            pnlInfo.Paint += pnlInfo_Paint;
            //
            // lblInfo
            //
            lblInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            lblInfo.Font = new System.Drawing.Font("Segoe UI", 11F);
            lblInfo.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            lblInfo.Location = new System.Drawing.Point(0, 0);
            lblInfo.Name = "lblInfo";
            lblInfo.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            lblInfo.Size = new System.Drawing.Size(1436, 45);
            lblInfo.TabIndex = 0;
            lblInfo.Text = "ℹ Gestion centralizada del directorio de proveedores y contactos activos.";
            lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlAcciones
            //
            pnlAcciones.Controls.Add(btnNuevo);
            pnlAcciones.Controls.Add(btnEditar);
            pnlAcciones.Controls.Add(txtBuscar);
            pnlAcciones.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlAcciones.Location = new System.Drawing.Point(0, 120);
            pnlAcciones.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            pnlAcciones.Name = "pnlAcciones";
            pnlAcciones.Size = new System.Drawing.Size(1436, 40);
            pnlAcciones.TabIndex = 2;
            //
            // btnNuevo
            //
            btnNuevo.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            btnNuevo.Cursor = System.Windows.Forms.Cursors.Hand;
            btnNuevo.FlatAppearance.BorderSize = 0;
            btnNuevo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnNuevo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            btnNuevo.Location = new System.Drawing.Point(0, 0);
            btnNuevo.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new System.Drawing.Size(120, 38);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "➕ Nuevo";
            btnNuevo.UseVisualStyleBackColor = false;
            btnNuevo.MouseEnter += Btn_MouseEnter;
            btnNuevo.MouseLeave += Btn_MouseLeave;
            //
            // btnEditar
            //
            btnEditar.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            btnEditar.Cursor = System.Windows.Forms.Cursors.Hand;
            btnEditar.FlatAppearance.BorderSize = 0;
            btnEditar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnEditar.Font = new System.Drawing.Font("Segoe UI", 11F);
            btnEditar.Location = new System.Drawing.Point(130, 0);
            btnEditar.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new System.Drawing.Size(120, 38);
            btnEditar.TabIndex = 1;
            btnEditar.Text = "✏ Editar";
            btnEditar.UseVisualStyleBackColor = false;
            btnEditar.MouseEnter += Btn_MouseEnter;
            btnEditar.MouseLeave += Btn_MouseLeave;
            //
            // txtBuscar
            //
            txtBuscar.Font = new System.Drawing.Font("Segoe UI", 12F);
            txtBuscar.Location = new System.Drawing.Point(400, 4);
            txtBuscar.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.PlaceholderText = "🔍 Buscar proveedor por nombre o RTN...";
            txtBuscar.Size = new System.Drawing.Size(400, 34);
            txtBuscar.TabIndex = 3;
            //
            // pnlContenedorGrid
            //
            pnlContenedorGrid.BackColor = System.Drawing.Color.White;
            pnlContenedorGrid.Controls.Add(dgvProveedores);
            pnlContenedorGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlContenedorGrid.Location = new System.Drawing.Point(0, 170);
            pnlContenedorGrid.Margin = new System.Windows.Forms.Padding(0);
            pnlContenedorGrid.Name = "pnlContenedorGrid";
            pnlContenedorGrid.Padding = new System.Windows.Forms.Padding(20);
            pnlContenedorGrid.Size = new System.Drawing.Size(1436, 660);
            pnlContenedorGrid.TabIndex = 3;
            pnlContenedorGrid.Paint += pnlContenedorGrid_Paint;
            //
            // dgvProveedores
            //
            dgvProveedores.BackgroundColor = System.Drawing.Color.White;
            dgvProveedores.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dgvProveedores.ColumnHeadersHeight = 29;
            dgvProveedores.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvProveedores.Location = new System.Drawing.Point(20, 20);
            dgvProveedores.Name = "dgvProveedores";
            dgvProveedores.RowHeadersWidth = 51;
            dgvProveedores.Size = new System.Drawing.Size(1396, 620);
            dgvProveedores.TabIndex = 0;
            //
            // PantallaProveedores
            //
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(253, 253, 253);
            ClientSize = new System.Drawing.Size(1500, 900);
            Controls.Add(mainLayout);
            Font = new System.Drawing.Font("Segoe UI", 11F);
            Name = "PantallaProveedores";
            Padding = new System.Windows.Forms.Padding(32, 34, 32, 36);
            Text = "Proveedores";
            WindowState = FormWindowState.Maximized;
            Load += PantallaProveedores_Load;
            mainLayout.ResumeLayout(false);
            mainLayout.PerformLayout();
            pnlInfo.ResumeLayout(false);
            pnlAcciones.ResumeLayout(false);
            pnlAcciones.PerformLayout();
            pnlContenedorGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvProveedores).EndInit();
            ResumeLayout(false);
        }
    }
}
