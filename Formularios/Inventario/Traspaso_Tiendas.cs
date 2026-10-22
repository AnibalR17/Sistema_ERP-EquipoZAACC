namespace Sistema_ERP
{
    public partial class Traspaso_Tiendas : UserControl
    {
        internal void UsarNavegacionPrincipal()
        {
            VistaContenido.Preparar(this, content);
        }

        public Traspaso_Tiendas()
        {
            InitializeComponent();
            UsarNavegacionPrincipal();
        }
    }
}