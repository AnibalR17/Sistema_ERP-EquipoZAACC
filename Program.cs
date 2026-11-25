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
            TemaAplicacion.Inicializar();
            if (!ConfigurarConexionInicial()) return;
            if (!PrepararBaseDatos()) return;
            while (true)
            {
                using var login = new LoginERP.FrmLogin();
                TemaAplicacion.Aplicar(login);
                if (login.ShowDialog() != DialogResult.OK || login.Acceso is not AccesoUsuario acceso) break;
                SesionActual.Usuario = acceso;
                using var principal = new Pagina_Central(acceso);
                TemaAplicacion.Aplicar(principal);
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

        private static bool ConfigurarConexionInicial()
        {
            string? cadena = null;
            Exception? error = null;
            try
            {
                cadena = Clases.Conexion.LeerCadenaConfigurada();
                if (!string.IsNullOrWhiteSpace(cadena))
                {
                    Clases.ValidadorConexion.Verificar(cadena);
                    return true;
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            using var configuracion = new ConfiguracionConexionInicial(
                cadena, error is null ? null : Clases.DiagnosticoConexion.Describir(error));
            return configuracion.ShowDialog() == DialogResult.OK;
        }

        private static bool PrepararBaseDatos()
        {
            using var ventana = new Form
            {
                Text = "Preparación de la base ERP",
                ClientSize = new Size(520, 145),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                ShowInTaskbar = false,
                ControlBox = false
            };
            var estado = new Label
            {
                AutoSize = false,
                Location = new Point(24, 22),
                Size = new Size(472, 42),
                Text = "Verificando la base de datos…",
                TextAlign = ContentAlignment.MiddleLeft
            };
            var progreso = new ProgressBar
            {
                Location = new Point(24, 82),
                Size = new Size(472, 22),
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 28
            };
            ventana.Controls.Add(estado);
            ventana.Controls.Add(progreso);

            Exception? error = null;
            bool cambios = false;
            ventana.Shown += async (_, _) =>
            {
                try
                {
                    cambios = await Task.Run(() => Datos.PreparacionBaseDatos.Asegurar(mensaje =>
                        ventana.BeginInvoke((Action)(() => estado.Text = mensaje))));
                    ventana.DialogResult = DialogResult.OK;
                }
                catch (Exception ex)
                {
                    error = ex;
                    ventana.DialogResult = DialogResult.Abort;
                }
            };

            if (ventana.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show(error?.Message ?? "No se pudo verificar la base de datos.",
                    "Preparación de la base ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (cambios)
                MessageBox.Show("La base ERP quedó preparada. Se aplicaron el esquema o los scripts que hacían falta.",
                    "Preparación completada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }
    }
}
