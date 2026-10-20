namespace Sistema_ERP
{
    public partial class Traspaso_Tiendas : UserControl
    {
        internal void UsarNavegacionPrincipal()
        {
            sidebar.Visible = false;
            header.Visible = false;
            MinimumSize = Size.Empty;
        }

        public Traspaso_Tiendas()
        {
            InitializeComponent();
        }
    }
}