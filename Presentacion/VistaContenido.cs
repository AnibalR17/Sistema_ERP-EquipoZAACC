namespace Sistema_ERP
{
    internal static class VistaContenido
    {
        // Conserva el contenido del disenador; la navegacion compartida pertenece al formulario MDI principal.
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
            vista.ResumeLayout(true);
        }
    }
}
