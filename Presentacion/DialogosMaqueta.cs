namespace Sistema_ERP
{
    internal static class DialogosMaqueta
    {
        internal static void Mostrar(Form propietario, Form dialogo)
        {
            using (dialogo)
            {
                using var sombra = new Form
                {
                    FormBorderStyle = FormBorderStyle.None,
                    StartPosition = FormStartPosition.Manual,
                    Bounds = propietario.Bounds,
                    BackColor = Color.Black,
                    Opacity = .4,
                    ShowInTaskbar = false
                };
                sombra.Show(propietario);
                try
                {
                    dialogo.ShowDialog(sombra);
                }
                finally
                {
                    sombra.Close();
                }
            }
        }
    }
}
