namespace Sistema_ERP
{
    public sealed partial class MotivoDesactivacion
    {
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
