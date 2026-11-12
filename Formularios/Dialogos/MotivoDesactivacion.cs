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
            string motivo = txtMotivo.Text.Trim();
            if (motivo.Length == 0 || ValidacionDatos.SoloCeros(motivo))
            {
                error.Text = motivo.Length == 0 ? "Introduce el motivo para continuar." : "El motivo no puede contener solo ceros.";
                txtMotivo.Focus();
                return;
            }
            Motivo = motivo;
            DialogResult = DialogResult.OK;
        }
    }
}
