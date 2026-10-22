namespace Sistema_ERP
{
    public partial class Inventario : UserControl
    {
        internal void UsarNavegacionPrincipal()
        {
            VistaContenido.Preparar(this, content);
        }

        public Inventario()
        {
            InitializeComponent();
            UsarNavegacionPrincipal();
        }
    }
}