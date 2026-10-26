namespace Sistema_ERP
{
    public partial class Catalogo_Productos
    {
        private readonly Button btnReactivarProducto = new() { Name = "btnReactivarProducto", Text = "Reactivar", Enabled = false };
        private readonly CheckBox chkProductosInactivos = new()
        {
            Name = "chkProductosInactivos",
            Text = "Mostrar también productos inactivos",
            AutoSize = true
        };
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
            ProductoEstilo.Boton(btnReactivarProducto);
            btnEditar.Enabled = btnDesactivar.Enabled = false;
            btnNuevoProducto.Text = "+ Nuevo producto";

            dgvProductos.BackgroundColor = Color.White;
            dgvProductos.BorderStyle = BorderStyle.None;
            dgvProductos.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvProductos.GridColor = ProductoEstilo.Suave;
            dgvProductos.EnableHeadersVisualStyles = false;
            dgvProductos.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvProductos.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = ProductoEstilo.Suave,
                ForeColor = ProductoEstilo.Texto,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Padding = new Padding(8),
                SelectionBackColor = ProductoEstilo.Suave,
                SelectionForeColor = ProductoEstilo.Texto
            };
            dgvProductos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvProductos.ColumnHeadersHeight = 48;
            dgvProductos.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = ProductoEstilo.Texto,
                SelectionBackColor = Color.FromArgb(228, 232, 237),
                SelectionForeColor = ProductoEstilo.Texto,
                Padding = new Padding(8, 4, 8, 4)
            };
            dgvProductos.RowTemplate.Height = 44;
            dgvProductos.AllowUserToResizeRows = false;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(32, 20, 32, 28),
                BackColor = ProductoEstilo.Fondo
            };
            foreach (float height in new[] { 72F, 40F, 56F, 52F })
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var acciones = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, AutoScroll = true };
            acciones.Controls.AddRange(new Control[] { btnNuevoProducto, btnEditar, btnDesactivar, btnReactivarProducto });
            foreach (Button boton in acciones.Controls) boton.Margin = new Padding(0, 10, 12, 0);
            var productos = PantallaMaqueta.Filas(76, 36, 48, -100);
            acciones.Dock = txtBuscar.Dock = dgvProductos.Dock = DockStyle.Fill;
            productos.Controls.Add(acciones, 0, 0);
            productos.Controls.Add(chkProductosInactivos, 0, 1);
            productos.Controls.Add(txtBuscar, 0, 2);
            productos.Controls.Add(dgvProductos, 0, 3);
            var categorias = CrearPanelCategorias();
            categorias.Visible = false;
            var cuerpo = new Panel { Dock = DockStyle.Fill, Margin = Padding.Empty };
            cuerpo.Controls.Add(productos);
            cuerpo.Controls.Add(categorias);
            var tabs = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            var tabProductos = PantallaMaqueta.Boton("Productos", true);
            var tabCategorias = PantallaMaqueta.Boton("Categorías");
            tabProductos.Width = tabCategorias.Width = 130;
            tabs.Controls.AddRange(new Control[] { tabProductos, tabCategorias });
            void Seleccionar(bool mostrarCategorias)
            {
                productos.Visible = !mostrarCategorias;
                categorias.Visible = mostrarCategorias;
                foreach (Button boton in tabs.Controls)
                {
                    bool seleccionado = boton == (mostrarCategorias ? tabCategorias : tabProductos);
                    boton.BackColor = seleccionado ? ProductoEstilo.Texto : ProductoEstilo.Suave;
                    boton.ForeColor = seleccionado ? Color.White : ProductoEstilo.Texto;
                }
                if (mostrarCategorias) CargarCategorias();
            }
            tabProductos.Click += (_, _) => Seleccionar(false);
            tabCategorias.Click += (_, _) => Seleccionar(true);
            tabCategorias.BackColor = ProductoEstilo.Suave;
            Control[] filas = { titleLabel, subtitleLabel, label1, tabs, cuerpo };
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
