namespace Sistema_ERP.Vistas
{
    public partial class PantallaProductos
    {
        private void ConfigurarPresentacion()
        {
            EstiloFormularios.Aplicar(this, mainLayout, lblTitulo);
            EstiloFormularios.Acciones(pnlAcciones, txtBuscar, btnNuevo, btnEditar, btnDesactivar);
        }
    }
}
