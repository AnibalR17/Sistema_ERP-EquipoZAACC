namespace Sistema_ERP.Vistas
{
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
