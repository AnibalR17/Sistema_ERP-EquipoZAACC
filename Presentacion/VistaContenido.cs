namespace Sistema_ERP
{
    internal static class VistaContenido
    {
        internal static void Preparar(UserControl vista, Control contenido)
        {
            vista.SuspendLayout();
            vista.Controls.Add(contenido);
            foreach (Control control in vista.Controls.Cast<Control>().ToArray())
            {
                if (control == contenido) continue;
                vista.Controls.Remove(control);
                control.Dispose();
            }
            vista.MinimumSize = Size.Empty;
            contenido.Dock = DockStyle.Fill;
            vista.Load -= PrepararDisposicion;
            vista.Load += PrepararDisposicion;
            vista.ResumeLayout(true);
        }

        private static void PrepararDisposicion(object? sender, EventArgs e)
        {
            if (sender is Control vista && !ModoDiseno.Activo(vista)) DisposicionControles.Preparar(vista);
        }
    }
}
