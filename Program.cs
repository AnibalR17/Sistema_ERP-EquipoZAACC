namespace Sistema_ERP
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            while (true)
            {
                using var login = new LoginERP.FrmLogin();
                if (login.ShowDialog() != DialogResult.OK || login.Acceso is not AccesoUsuario acceso) break;
                SesionActual.Usuario = acceso;
                using var principal = new Pagina_Central(acceso);
                Application.Run(principal);
                SesionActual.Usuario = null;
                try { AccesoDatos.Cerrar(acceso); }
                catch (Exception)
                {
                    MessageBox.Show("No se pudo confirmar el cierre de la sesión en la base de datos. La sesión caducará automáticamente.", "Sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                if (!principal.VolverAlAcceso) break;
            }
        }
    }
}
