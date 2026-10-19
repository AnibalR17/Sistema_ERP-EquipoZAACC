namespace Sistema_ERP
{
    internal static class ProductoEstilo
    {
        internal static readonly Color Fondo = Color.FromArgb(253, 253, 253);
        internal static readonly Color Suave = Color.FromArgb(243, 243, 243);
        internal static readonly Color Texto = Color.FromArgb(24, 24, 24);

        internal static void Boton(Button boton, bool principal = false)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = principal ? 0 : 1;
            boton.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
            boton.FlatAppearance.MouseOverBackColor = principal ? Color.FromArgb(55, 55, 55) : Suave;
            boton.BackColor = principal ? Texto : Color.White;
            boton.ForeColor = principal ? Color.White : Texto;
            boton.Font = new Font("Segoe UI", 11F);
            boton.Cursor = Cursors.Hand;
            boton.UseVisualStyleBackColor = false;
            boton.Size = new Size(170, 44);
        }
    }

    public partial class Catalogo_Productos
    {
        internal void UsarNavegacionPrincipal()
        {
            // Conserva el encabezado y la navegación de Pagina_Central al abrir el catálogo.
            sidebar.Visible = false;
            header.Visible = false;
        }

        private void ConfigurarPresentacion()
        {
            SuspendLayout();
            Font = new Font("Segoe UI", 11F);
            ForeColor = ProductoEstilo.Texto;
            content.BackColor = ProductoEstilo.Fondo;
            // El diseñador contiene dos copias del bloque de usuario.
            label2.Visible = label3.Visible = button2.Visible = false;
            homeButton.Font = new Font(Font, FontStyle.Regular);
            catalogButton.Font = new Font(Font, FontStyle.Bold);
            catalogButton.BackColor = ProductoEstilo.Suave;
            subtitleLabel.Text = "Administra tus productos, categorías y proveedores.";
            label1.Text = "El stock se actualiza con los movimientos de inventario.";
            label1.BackColor = ProductoEstilo.Suave;
            label1.Padding = new Padding(12, 0, 8, 0);
            label1.Font = new Font("Segoe UI", 10F);
            txtBuscar.PlaceholderText = "Buscar por nombre o código de barras...";
            txtBuscar.BorderStyle = BorderStyle.FixedSingle;
            ProductoEstilo.Boton(btnNuevoProducto, true);
            ProductoEstilo.Boton(btnEditar);
            ProductoEstilo.Boton(btnEliminar);
            btnNuevoProducto.Text = "+ Nuevo producto";

            dgvProductos.BackgroundColor = Color.White;
            dgvProductos.BorderStyle = BorderStyle.None;
            dgvProductos.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvProductos.GridColor = ProductoEstilo.Suave;
            dgvProductos.EnableHeadersVisualStyles = false;
            dgvProductos.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvProductos.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = ProductoEstilo.Suave, ForeColor = ProductoEstilo.Texto,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Padding = new Padding(8), SelectionBackColor = ProductoEstilo.Suave,
                SelectionForeColor = ProductoEstilo.Texto
            };
            dgvProductos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvProductos.ColumnHeadersHeight = 48;
            dgvProductos.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White, ForeColor = ProductoEstilo.Texto,
                SelectionBackColor = Color.FromArgb(228, 232, 237),
                SelectionForeColor = ProductoEstilo.Texto, Padding = new Padding(8, 4, 8, 4)
            };
            dgvProductos.RowTemplate.Height = 44;
            dgvProductos.AllowUserToResizeRows = false;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 6,
                Padding = new Padding(32, 20, 32, 28), BackColor = ProductoEstilo.Fondo
            };
            foreach (float height in new[] { 72F, 40F, 56F, 68F, 48F })
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var acciones = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            acciones.Controls.AddRange(new Control[] { btnNuevoProducto, btnEditar, btnEliminar });
            foreach (Button boton in acciones.Controls) boton.Margin = new Padding(0, 10, 12, 0);
            Control[] filas = { titleLabel, subtitleLabel, label1, acciones, txtBuscar, dgvProductos };
            for (int i = 0; i < filas.Length; i++)
            {
                filas[i].Dock = DockStyle.Fill;
                filas[i].Margin = new Padding(0, 0, 0, i == 2 ? 8 : 0);
                layout.Controls.Add(filas[i], 0, i);
            }
            content.Padding = Padding.Empty;
            content.Controls.Add(layout);
            header.Resize += (_, _) => AjustarCabecera();
            AjustarCabecera();
            ResumeLayout(true);
        }

        private void AjustarCabecera()
        {
            searchBox.Width = Math.Max(100, Math.Min(420, header.Width - 340));
            accountLabel.Left = searchBox.Right + 18;
            notificationButton.Left = accountLabel.Right + 12;
        }
    }

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
