namespace Sistema_ERP;

internal static class PermisosInterfaz
{
    internal static void Configurar(Control vista, string modulo, params Control[] escrituras)
        => AlCargar(vista, permisos =>
        {
            if (!permisos.Puede(modulo)) throw new UnauthorizedAccessException("No tienes permiso para abrir este contenido.");
            foreach (var control in escrituras) control.Visible = permisos.Puede(modulo, gestionar: true);
        });

    internal static void ConfigurarAcciones(Control vista, params (Control Control, string Modulo)[] escrituras)
        => AlCargar(vista, permisos =>
        {
            foreach (var (control, modulo) in escrituras) control.Visible = permisos.Puede(modulo, gestionar: true);
        });

    private static void AlCargar(Control vista, Action<PermisosUsuario> aplicar)
    {
        void Cargar(object? sender, EventArgs e)
        {
            if (ModoDiseno.Activo(vista)) return;
            try { aplicar(AutorizacionDatos.Obtener()); }
            catch (Exception ex)
            {
                vista.Enabled = false;
                MessageBox.Show(vista, "No se pudo autorizar el acceso. " + ex.Message, "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (vista is Form formulario) formulario.Close(); else vista.Hide();
            }
        }
        if (vista is Form formulario) formulario.Load += Cargar;
        else if (vista is UserControl control) control.Load += Cargar;
        else throw new ArgumentException("La vista debe ser un formulario o control de usuario.", nameof(vista));
    }
}
