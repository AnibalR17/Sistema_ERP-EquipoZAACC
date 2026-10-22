namespace Sistema_ERP
{
    public partial class Nuevo_Producto
    {
        private void ConfigurarPresentacion()
        {
            SuspendLayout();
            Font = new Font("Segoe UI", 11F);
            ForeColor = ProductoEstilo.Texto;
            BackColor = ProductoEstilo.Fondo;
            ClientSize = new Size(620, 650);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            titleLabel.Text = "Nuevo producto";
            titleLabel.Font = new Font("Segoe UI", 27F, FontStyle.Bold);
            titleLabel.SetBounds(32, 16, 556, 72);

            Label[] etiquetas = { subtitleLabel, label1, label2, label3, label4, label5 };
            Control[] campos = { txtCodigo, txtNombre, nudPrecio, txtStock, cmbCategoria, cmbProveedor };
            string[] textos = { "Código de barras *", "Nombre *", "Precio unitario (L.) *", "Stock actual · solo lectura", "Categoría *", "Proveedor *" };
            for (int i = 0; i < campos.Length; i++)
            {
                int y = 100 + i * 76;
                etiquetas[i].Text = textos[i];
                etiquetas[i].Font = new Font("Segoe UI", 11F);
                etiquetas[i].ForeColor = Color.FromArgb(100, 100, 100);
                etiquetas[i].SetBounds(32, y, 556, 26);
                campos[i].SetBounds(32, y + 28, 556, 30);
                campos[i].TabIndex = i;
            }
            txtStock.BackColor = ProductoEstilo.Suave;
            txtStock.TabStop = false;
            cmbCategoria.DropDownStyle = cmbProveedor.DropDownStyle = ComboBoxStyle.DropDownList;
            ProductoEstilo.Boton(btnGuardar, true);
            ProductoEstilo.Boton(btnCancelar);
            btnGuardar.Text = "Guardar producto";
            btnCancelar.SetBounds(236, 582, 170, 44);
            btnGuardar.SetBounds(418, 582, 170, 44);
            btnGuardar.TabIndex = 6;
            btnCancelar.TabIndex = 7;
            ResumeLayout(true);
        }
    }
}
