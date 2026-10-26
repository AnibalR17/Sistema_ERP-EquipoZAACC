using System.Drawing.Drawing2D;

namespace Sistema_ERP
{
    internal static class DialogosMaqueta
    {
        internal static void Mostrar(Form propietario, Form dialogo, bool oscurecerFondo = true)
        {
            using (dialogo)
            {
                if (!oscurecerFondo)
                {
                    dialogo.ShowDialog(propietario);
                    return;
                }
                using var sombra = new Form { FormBorderStyle = FormBorderStyle.None, StartPosition = FormStartPosition.Manual, Bounds = propietario.Bounds, BackColor = Color.Black, Opacity = .4, ShowInTaskbar = false };
                sombra.Show(propietario);
                try { dialogo.ShowDialog(sombra); }
                finally { sombra.Close(); }
            }
        }
    }
}
