namespace Sistema_ERP.Vistas
{
    public partial class PantallaProveedores
    {
        private void ConfigurarPresentacion()
        {
            EstiloFormularios.Aplicar(this, mainLayout, lblTitulo);
            EstiloFormularios.Acciones(pnlAcciones, txtBuscar, btnNuevo, btnEditar, btnDesactivar);
            // Reserva una fila completa para los botones y otra para la busqueda.
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
}
