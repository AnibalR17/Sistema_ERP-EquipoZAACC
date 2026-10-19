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
            titleLabel = new System.Windows.Forms.Label();
            subtitleLabel = new System.Windows.Forms.Label();
            txtCodigo = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            txtStock = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            cmbCategoria = new System.Windows.Forms.ComboBox();
            cmbProveedor = new System.Windows.Forms.ComboBox();
            label5 = new System.Windows.Forms.Label();
            btnGuardar = new System.Windows.Forms.Button();
            btnCancelar = new System.Windows.Forms.Button();
            nudPrecio = new System.Windows.Forms.NumericUpDown();
            txtNombre = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)nudPrecio).BeginInit();
            SuspendLayout();
            // 
            // titleLabel
            // 
            titleLabel.BackColor = System.Drawing.Color.Transparent;
            titleLabel.Font = new System.Drawing.Font("Segoe UI", 27F, System.Drawing.FontStyle.Bold);
            titleLabel.Location = new System.Drawing.Point(12, 9);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new System.Drawing.Size(400, 62);
            titleLabel.TabIndex = 1;
            titleLabel.Text = "Nuevo Producto ";
            titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // subtitleLabel
            // 
            subtitleLabel.BackColor = System.Drawing.Color.Transparent;
            subtitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            subtitleLabel.Location = new System.Drawing.Point(12, 83);
            subtitleLabel.Name = "subtitleLabel";
            subtitleLabel.Size = new System.Drawing.Size(167, 28);
            subtitleLabel.TabIndex = 2;
            subtitleLabel.Text = "Codigo de barras ";
            subtitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtCodigo
            // 
            txtCodigo.Location = new System.Drawing.Point(12, 114);
            txtCodigo.Name = "txtCodigo";
            txtCodigo.Size = new System.Drawing.Size(413, 27);
            txtCodigo.TabIndex = 3;
            // 
            // label1
            // 
            label1.BackColor = System.Drawing.Color.Transparent;
            label1.Font = new System.Drawing.Font("Segoe UI", 12F);
            label1.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            label1.Location = new System.Drawing.Point(12, 154);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(93, 28);
            label1.TabIndex = 4;
            label1.Text = "Nombre ";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            label2.BackColor = System.Drawing.Color.Transparent;
            label2.Font = new System.Drawing.Font("Segoe UI", 12F);
            label2.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            label2.Location = new System.Drawing.Point(12, 230);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(144, 28);
            label2.TabIndex = 6;
            label2.Text = "Precio Unitario ";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtStock
            // 
            txtStock.Location = new System.Drawing.Point(12, 334);
            txtStock.Name = "txtStock";
            txtStock.ReadOnly = true;
            txtStock.Size = new System.Drawing.Size(413, 27);
            txtStock.TabIndex = 9;
            // 
            // label3
            // 
            label3.BackColor = System.Drawing.Color.Transparent;
            label3.Font = new System.Drawing.Font("Segoe UI", 12F);
            label3.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            label3.Location = new System.Drawing.Point(12, 303);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(243, 28);
            label3.TabIndex = 8;
            label3.Text = "Stock actual (Solo lectura)";
            label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            label4.BackColor = System.Drawing.Color.Transparent;
            label4.Font = new System.Drawing.Font("Segoe UI", 12F);
            label4.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            label4.Location = new System.Drawing.Point(12, 384);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(151, 28);
            label4.TabIndex = 10;
            label4.Text = "Categoria*";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbCategoria
            // 
            cmbCategoria.FormattingEnabled = true;
            cmbCategoria.Location = new System.Drawing.Point(12, 415);
            cmbCategoria.Name = "cmbCategoria";
            cmbCategoria.Size = new System.Drawing.Size(151, 28);
            cmbCategoria.TabIndex = 11;
            // 
            // cmbProveedor
            // 
            cmbProveedor.FormattingEnabled = true;
            cmbProveedor.Location = new System.Drawing.Point(12, 481);
            cmbProveedor.Name = "cmbProveedor";
            cmbProveedor.Size = new System.Drawing.Size(151, 28);
            cmbProveedor.TabIndex = 13;
            // 
            // label5
            // 
            label5.BackColor = System.Drawing.Color.Transparent;
            label5.Font = new System.Drawing.Font("Segoe UI", 12F);
            label5.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            label5.Location = new System.Drawing.Point(12, 450);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(151, 28);
            label5.TabIndex = 12;
            label5.Text = "Proveedor*";
            label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new System.Drawing.Point(387, 567);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new System.Drawing.Size(94, 29);
            btnGuardar.TabIndex = 14;
            btnGuardar.Text = "Aceptar";
            btnGuardar.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new System.Drawing.Point(259, 567);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new System.Drawing.Size(94, 29);
            btnCancelar.TabIndex = 15;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            // 
            // nudPrecio
            // 
            nudPrecio.Location = new System.Drawing.Point(13, 261);
            nudPrecio.Name = "nudPrecio";
            nudPrecio.Size = new System.Drawing.Size(150, 27);
            nudPrecio.TabIndex = 16;
            // 
            // txtNombre
            // 
            txtNombre.Location = new System.Drawing.Point(13, 185);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new System.Drawing.Size(412, 27);
            txtNombre.TabIndex = 17;
            // 
            // Nuevo_Producto
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.Window;
            ClientSize = new System.Drawing.Size(493, 608);
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

        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label subtitleLabel;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtStock;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbCategoria;
        private System.Windows.Forms.ComboBox cmbProveedor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.NumericUpDown nudPrecio;
        private System.Windows.Forms.TextBox txtNombre;
    }
}