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
            if (Sistema_ERP.ModoDiseno.Activo(this)) return;
            UsarNavegacionPrincipal();
        }
    }
}