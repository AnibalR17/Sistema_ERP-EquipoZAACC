namespace Sistema_ERP
{
    partial class Nuevo_Producto
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            titleLabel = new Label();
            subtitleLabel = new Label();
            txtCodigo = new TextBox();
            label1 = new Label();
            label2 = new Label();
            txtStock = new TextBox();
            label3 = new Label();
            label4 = new Label();
            cmbCategoria = new ComboBox();
            cmbProveedor = new ComboBox();
            label5 = new Label();
            btnGuardar = new Button();
            btnCancelar = new Button();
            nudPrecio = new NumericUpDown();
            txtNombre = new TextBox();
            ((System.ComponentModel.ISupportInitialize)nudPrecio).BeginInit();
            SuspendLayout();
            // 
            // titleLabel
            // 
            titleLabel.BackColor = Color.Transparent;
            titleLabel.Font = new Font("Segoe UI", 27F, FontStyle.Bold);
            titleLabel.Location = new Point(12, 9);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(400, 62);
            titleLabel.TabIndex = 1;
            titleLabel.Text = "Nuevo Producto ";
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // subtitleLabel
            // 
            subtitleLabel.BackColor = Color.Transparent;
            subtitleLabel.Font = new Font("Segoe UI", 12F);
            subtitleLabel.ForeColor = Color.FromArgb(125, 125, 125);
            subtitleLabel.Location = new Point(12, 83);
            subtitleLabel.Name = "subtitleLabel";
            subtitleLabel.Size = new Size(167, 28);
            subtitleLabel.TabIndex = 2;
            subtitleLabel.Text = "Codigo de barras ";
            subtitleLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtCodigo
            // 
            txtCodigo.Location = new Point(12, 114);
            txtCodigo.Name = "txtCodigo";
            txtCodigo.Size = new Size(413, 27);
            txtCodigo.TabIndex = 3;
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 12F);
            label1.ForeColor = Color.FromArgb(125, 125, 125);
            label1.Location = new Point(12, 154);
            label1.Name = "label1";
            label1.Size = new Size(93, 28);
            label1.TabIndex = 4;
            label1.Text = "Nombre ";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Segoe UI", 12F);
            label2.ForeColor = Color.FromArgb(125, 125, 125);
            label2.Location = new Point(12, 230);
            label2.Name = "label2";
            label2.Size = new Size(144, 28);
            label2.TabIndex = 6;
            label2.Text = "Precio Unitario ";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtStock
            // 
            txtStock.Location = new Point(12, 334);
            txtStock.Name = "txtStock";
            txtStock.ReadOnly = true;
            txtStock.Size = new Size(413, 27);
            txtStock.TabIndex = 9;
            // 
            // label3
            // 
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Segoe UI", 12F);
            label3.ForeColor = Color.FromArgb(125, 125, 125);
            label3.Location = new Point(12, 303);
            label3.Name = "label3";
            label3.Size = new Size(243, 28);
            label3.TabIndex = 8;
            label3.Text = "Stock actual (Solo lectura)";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            label4.BackColor = Color.Transparent;
            label4.Font = new Font("Segoe UI", 12F);
            label4.ForeColor = Color.FromArgb(125, 125, 125);
            label4.Location = new Point(12, 384);
            label4.Name = "label4";
            label4.Size = new Size(151, 28);
            label4.TabIndex = 10;
            label4.Text = "Categoria*";
            label4.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cmbCategoria
            // 
            cmbCategoria.FormattingEnabled = true;
            cmbCategoria.Location = new Point(12, 415);
            cmbCategoria.Name = "cmbCategoria";
            cmbCategoria.Size = new Size(151, 28);
            cmbCategoria.TabIndex = 11;
            // 
            // cmbProveedor
            // 
            cmbProveedor.FormattingEnabled = true;
            cmbProveedor.Location = new Point(12, 481);
            cmbProveedor.Name = "cmbProveedor";
            cmbProveedor.Size = new Size(151, 28);
            cmbProveedor.TabIndex = 13;
            // 
            // label5
            // 
            label5.BackColor = Color.Transparent;
            label5.Font = new Font("Segoe UI", 12F);
            label5.ForeColor = Color.FromArgb(125, 125, 125);
            label5.Location = new Point(12, 450);
            label5.Name = "label5";
            label5.Size = new Size(151, 28);
            label5.TabIndex = 12;
            label5.Text = "Proveedor*";
            label5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(387, 567);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(94, 29);
            btnGuardar.TabIndex = 14;
            btnGuardar.Text = "Aceptar";
            btnGuardar.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(259, 567);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(94, 29);
            btnCancelar.TabIndex = 15;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            // 
            // nudPrecio
            // 
            nudPrecio.Location = new Point(13, 261);
            nudPrecio.Name = "nudPrecio";
            nudPrecio.Size = new Size(150, 27);
            nudPrecio.TabIndex = 16;
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(13, 185);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(412, 27);
            txtNombre.TabIndex = 17;
            // 
            // Nuevo_Producto
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(493, 608);
            Controls.Add(txtNombre);
            Controls.Add(nudPrecio);
            Controls.Add(btnCancelar);
            Controls.Add(btnGuardar);
            Controls.Add(cmbProveedor);
            Controls.Add(label5);
            Controls.Add(cmbCategoria);
            Controls.Add(label4);
            Controls.Add(txtStock);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtCodigo);
            Controls.Add(subtitleLabel);
            Controls.Add(titleLabel);
            Name = "Nuevo_Producto";
            Text = "Nuevo_Producto";
            ((System.ComponentModel.ISupportInitialize)nudPrecio).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label titleLabel;
        private Label subtitleLabel;
        private TextBox txtCodigo;
        private TextBox textBox2;
        private Label label1;
        private Label label2;
        private TextBox txtStock;
        private Label label3;
        private Label label4;
        private ComboBox cmbCategoria;
        private ComboBox cmbProveedor;
        private Label label5;
        private Button btnGuardar;
        private Button btnCancelar;
        private NumericUpDown nudPrecio;
        private TextBox txtNombre;
    }
}