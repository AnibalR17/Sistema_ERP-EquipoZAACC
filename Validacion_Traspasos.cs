namespace Sistema_ERP
{
    public partial class Validacion_Traspasos : UserControl
    {
        internal void UsarNavegacionPrincipal()
        {
            sidebar.Visible = false;
            header.Visible = false;
            MinimumSize = Size.Empty;
        }

        public Validacion_Traspasos()
        {
            InitializeComponent();
        }
    }
}