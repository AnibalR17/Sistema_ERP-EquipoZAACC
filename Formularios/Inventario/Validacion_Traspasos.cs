namespace Sistema_ERP
{
    public partial class Validacion_Traspasos : UserControl
    {
        internal void UsarNavegacionPrincipal()
        {
            VistaContenido.Preparar(this, content);
        }

        public Validacion_Traspasos()
        {
            InitializeComponent();
            UsarNavegacionPrincipal();
        }
    }
}