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
                if (boton == btnAgregarCarrito || boton == btnGuardar)
                {
                    boton.EnabledChanged += (_, _) => AplicarEstadoAccion(boton);
                    AplicarEstadoAccion(boton);
                }
            }
        }

        private static void AplicarEstadoAccion(Button boton)
        {
            boton.BackColor = boton.Enabled ? ProductoEstilo.Texto : ProductoEstilo.Suave;
            boton.ForeColor = boton.Enabled ? Color.White : ProductoEstilo.TextoSecundario;
        }
    }
}
