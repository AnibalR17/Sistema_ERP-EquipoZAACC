namespace Sistema_ERP
{
    internal static class DialogosMaqueta
    {
        internal static void Redondear(Form dialogo)
        {
            int d = Math.Min(48, Math.Min(dialogo.Width, dialogo.Height));
            if (d <= 0) return;
            using var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(0, 0, d, d, 180, 90);
            path.AddArc(dialogo.Width - d, 0, d, d, 270, 90);
            path.AddArc(dialogo.Width - d, dialogo.Height - d, d, d, 0, 90);
            path.AddArc(0, dialogo.Height - d, d, d, 90, 90);
            path.CloseFigure();
            var anterior = dialogo.Region;
            dialogo.Region = new Region(path);
            anterior?.Dispose();
        }

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
