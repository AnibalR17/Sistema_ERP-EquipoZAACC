namespace Sistema_ERP
{
    public sealed partial class MotivoDesactivacion : FormularioTematico
    {
        public string Motivo { get; private set; } = "";
        public MotivoDesactivacion() : this("Registro") { }

        public MotivoDesactivacion(string nombre, string entidad = "categoria")
        {
            InitializeComponent();
            ConfigurarPresentacion(nombre, entidad);
            confirmar.Click += (_, _) => Confirmar();
            Shown += (_, _) => txtMotivo.Focus();
        }

        private void Confirmar()
        {
            try { Motivo = DesactivarRegistroValidacion.MotivoCapturado(txtMotivo.Text); DialogResult = DialogResult.OK; }
            catch (ArgumentException ex) { error.Text = ex.Message; txtMotivo.Focus(); }
        }
    }
}
