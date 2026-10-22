namespace Sistema_ERP.Vistas
{
    internal static class EstiloFormularios
    {
        internal static void Aplicar(Form formulario, TableLayoutPanel layout, Label titulo)
        {
            formulario.SuspendLayout();
            formulario.Font = new Font("Segoe UI", 11F);
            formulario.ForeColor = ProductoEstilo.Texto;
            formulario.BackColor = ProductoEstilo.Fondo;
            formulario.Padding = new Padding(32, 20, 32, 28);
            formulario.AutoScroll = true;
            layout.MinimumSize = new Size(680, 540);
            titulo.Font = new Font("Segoe UI", 27F, FontStyle.Bold);
            titulo.Dock = DockStyle.Fill;
            layout.RowStyles[0].Height = 72;
            formulario.ResumeLayout(true);
        }

        internal static void Acciones(FlowLayoutPanel acciones, TextBox buscar, params Button[] botones)
        {
            acciones.WrapContents = false;
            acciones.AutoScroll = true;
            foreach (var boton in botones)
            {
                ProductoEstilo.Boton(boton, boton == botones[0]);
                boton.Width = 110;
                boton.Margin = new Padding(0, 4, 8, 0);
            }
            buscar.Width = 260;
            buscar.Margin = new Padding(0, 8, 0, 0);
        }

        internal static void Campos(Panel panel, Label titulo, params (Label etiqueta, Control campo)[] campos)
        {
            var tabla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = campos.Length, RowCount = 3,
                Padding = new Padding(16, 8, 16, 8)
            };
            tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            for (int i = 0; i < campos.Length; i++)
            {
                tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / campos.Length));
                var (etiqueta, campo) = campos[i];
                etiqueta.Dock = DockStyle.Fill;
                etiqueta.AutoSize = false;
                etiqueta.Margin = new Padding(0, 0, 12, 0);
                campo.Dock = DockStyle.Top;
                campo.Margin = new Padding(0, 0, 12, 0);
                tabla.Controls.Add(etiqueta, i, 1);
                tabla.Controls.Add(campo, i, 2);
            }
            titulo.Dock = DockStyle.Fill;
            tabla.Controls.Add(titulo, 0, 0);
            tabla.SetColumnSpan(titulo, campos.Length);
            panel.Controls.Add(tabla);
        }
    }

    public partial class PantallaProductos
    {
        private void ConfigurarPresentacion()
        {
            EstiloFormularios.Aplicar(this, mainLayout, lblTitulo);
            EstiloFormularios.Acciones(pnlAcciones, txtBuscar, btnNuevo, btnEditar, btnEliminar);
        }
    }

    public partial class PantallaProveedores
    {
        private void ConfigurarPresentacion()
        {
            EstiloFormularios.Aplicar(this, mainLayout, lblTitulo);
            EstiloFormularios.Acciones(pnlAcciones, txtBuscar, btnNuevo, btnEditar, btnEliminar);
            // Reserva una fila completa para los botones y otra para la búsqueda.
            pnlAcciones.Controls.Remove(txtBuscar);
            pnlAcciones.AutoScroll = false;
            pnlAcciones.Margin = Padding.Empty;
            var acciones = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2,
                Margin = new Padding(0, 0, 0, 8)
            };
            acciones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            acciones.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
            acciones.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.Controls.Remove(pnlAcciones);
            acciones.Controls.Add(pnlAcciones, 0, 0);
            txtBuscar.Dock = DockStyle.Fill;
            txtBuscar.Margin = new Padding(0, 4, 0, 4);
            acciones.Controls.Add(txtBuscar, 0, 1);
            mainLayout.RowStyles[2].Height = 104;
            mainLayout.Controls.Add(acciones, 0, 2);
        }
    }

    public partial class PantallaCompras
    {
        private void ConfigurarPresentacion()
        {
            EstiloFormularios.Aplicar(this, mainLayout, lblTitulo);
            EstiloFormularios.Campos(pnlProveedor, lblSeccionProv,
                (lblProveedor, cmbProveedor), (lblFecha, dtpFecha), (lblFactura, txtFactura));
            EstiloFormularios.Campos(pnlProducto, lblSeccionProd,
                (lblProducto, cmbProducto), (lblCantidad, txtCantidad), (lblPrecio, txtPrecio));
            // Coloca la acci?n del carrito en una fila propia para evitar que se superponga al precio.
            mainLayout.Controls.Remove(pnlProducto);
            var productos = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            productos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            productos.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            productos.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            productos.Controls.Add(pnlProducto, 0, 0);
            ProductoEstilo.Boton(btnAgregarCarrito, true);
            btnAgregarCarrito.Text = "Agregar al carrito";
            btnAgregarCarrito.Width = 190;
            productos.Controls.Add(btnAgregarCarrito, 0, 1);
            mainLayout.Controls.Add(productos, 0, 2);
            mainLayout.RowStyles[2].Height = 163;

            var pie = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            pie.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            pie.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            pie.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            lblTotal.Dock = DockStyle.Fill;
            lblTotal.TextAlign = ContentAlignment.MiddleLeft;
            pie.Controls.Add(lblTotal, 0, 0);
            ProductoEstilo.Boton(btnGuardar, true);
            ProductoEstilo.Boton(btnCancelar);
            btnGuardar.Dock = btnCancelar.Dock = DockStyle.Fill;
            pie.Controls.Add(btnGuardar, 1, 0);
            pie.Controls.Add(btnCancelar, 2, 0);
            pnlPie.Controls.Add(pie);
        }
    }
}
