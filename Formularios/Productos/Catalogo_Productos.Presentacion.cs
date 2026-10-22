namespace Sistema_ERP
{
    public partial class Catalogo_Productos
    {
        internal void UsarNavegacionPrincipal()
        {
            VistaContenido.Preparar(this, content);
        }

        private void ConfigurarPresentacion()
        {
            SuspendLayout();
            Font = new Font("Segoe UI", 11F);
            ForeColor = ProductoEstilo.Texto;
            content.BackColor = ProductoEstilo.Fondo;
            // El disenador contiene dos copias del bloque de usuario.
            label2.Visible = label3.Visible = button2.Visible = false;
            homeButton.Font = new Font(Font, FontStyle.Regular);
            catalogButton.Font = new Font(Font, FontStyle.Bold);
            catalogButton.BackColor = ProductoEstilo.Suave;
            subtitleLabel.Text = "Administra tus productos, categorias y proveedores.";
            label1.Text = "El stock se actualiza con los movimientos de inventario.";
            label1.BackColor = ProductoEstilo.Suave;
            label1.Padding = new Padding(12, 0, 8, 0);
            label1.Font = new Font("Segoe UI", 10F);
            txtBuscar.PlaceholderText = "Buscar por nombre o codigo de barras...";
            txtBuscar.BorderStyle = BorderStyle.FixedSingle;
            ProductoEstilo.Boton(btnNuevoProducto, true);
            ProductoEstilo.Boton(btnEditar);
            ProductoEstilo.Boton(btnDesactivar);
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
            acciones.Controls.AddRange(new Control[] { btnNuevoProducto, btnEditar, btnDesactivar });
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
}
