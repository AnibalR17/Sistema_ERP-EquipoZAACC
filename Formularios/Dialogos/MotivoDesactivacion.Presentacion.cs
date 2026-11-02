namespace Sistema_ERP
{
    public sealed partial class MotivoDesactivacion
    {
        internal static MotivoDesactivacion ParaAnularCompra(int id, bool confirmada)
        {
            var dialogo = new MotivoDesactivacion($"compra #{id}", "compra");
            dialogo.Text = dialogo.lblTitulo.Text = $"Anular compra #{id}";
            dialogo.descripcion.Text = confirmada
                ? "Se revertirá la entrada de inventario. Se requiere stock disponible suficiente. El documento y su historial se conservarán."
                : "Se anulará el borrador sin mover inventario. El documento y su historial se conservarán.";
            dialogo.txtMotivo.MaxLength = 500;
            dialogo.lblMotivo.Text = "Motivo de anulación * (hasta 500 caracteres)";
            dialogo.confirmar.Text = "Anular compra";
            return dialogo;
        }

        private void ConfigurarPresentacion(string nombre, string entidad)
        {
            Text = $"Desactivar {entidad}";
            lblTitulo.Text = Text;
            descripcion.Text = $"Se desactivara {nombre}. Se conservaran sus registros asociados e historial.";
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            DialogosMaqueta.Redondear(this);
        }
    }
}
