namespace Sistema_ERP.Vistas
{
    public partial class PantallaCompras
    {
        private void ConfigurarPresentacion()
        {
            foreach (var boton in new[] { btnAgregarCarrito, btnActualizarLinea, btnQuitarLinea, btnGuardar, btnConfirmar, btnAnular, btnCancelar, btnConsultar, btnAbrirCompra, btnRecargar })
            {
                var medida = boton.Size;
                ProductoEstilo.Boton(boton, boton == btnAgregarCarrito || boton == btnGuardar);
                boton.Size = medida;
            }
        }
    }
}
