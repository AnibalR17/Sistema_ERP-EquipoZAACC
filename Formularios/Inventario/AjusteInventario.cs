namespace Sistema_ERP
{
    public sealed partial class AjusteInventario : Form
    {
        private bool guardando;
        public AjusteInventario() { InitializeComponent(); }
        internal AjusteInventario(string nombre, Action<int, string> operacion, bool reverso = false) : this()
        {
            Text = titulo.Text = nombre;
            if (reverso)
            {
                indicacion.Text = "Se registrará un movimiento opuesto al ajuste seleccionado. El historial original se conserva. Indica el motivo para confirmar.";
                cantidad.Visible = etiquetaCantidad.Visible = false;
            }
            guardar.Click += async (_, _) =>
            {
                if (guardando) return;
                guardando = true; guardar.Enabled = cancelar.Enabled = false;
                cantidad.Enabled = motivo.Enabled = false;
                try
                {
                    int unidades = (int)cantidad.Value; string texto = motivo.Text;
                    await Task.Run(() => operacion(unidades, texto));
                    DialogResult = DialogResult.OK; Close();
                }
                catch (Exception ex) { error.Text = ex.Message; }
                finally
                {
                    guardando = false;
                    if (!IsDisposed) guardar.Enabled = cancelar.Enabled = cantidad.Enabled = motivo.Enabled = true;
                }
            };
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (guardando && DialogResult != DialogResult.OK) e.Cancel = true;
            base.OnFormClosing(e);
        }
    }
}
