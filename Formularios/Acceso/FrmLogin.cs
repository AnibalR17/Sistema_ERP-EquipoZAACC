using Sistema_ERP;

namespace LoginERP
{
    public partial class FrmLogin : FormularioTematico
    {
        internal AccesoUsuario? Acceso { get; private set; }
        private AccesoUsuario? pendiente;
        private string claveAnterior = "";
        private bool trabajando;
        private readonly Func<string, string, Task<ResultadoAcceso>> autenticar;

        public FrmLogin() : this((usuario, clave) => Task.Run(() => AccesoDatos.Ingresar(usuario, clave))) { }
        internal FrmLogin(Func<string, string, Task<ResultadoAcceso>> autenticar)
        {
            this.autenticar = autenticar;
            InitializeComponent();
        }
        private void FrmLogin_Load(object sender, EventArgs e) { if (!ModoDiseno.Activo(this)) txtUsuario.Focus(); }
        private void pnlLogin_Paint(object sender, PaintEventArgs e) { }
        private void cmbRol_SelectedIndexChanged(object sender, EventArgs e) { }

        private async void btnIngresar_Click(object sender, EventArgs e)
        {
            if (trabajando) return;
            string usuario = txtUsuario.Text, clave = txtClave.Text;
            trabajando = true;
            btnIngresar.Enabled = txtUsuario.Enabled = txtClave.Enabled = txtConfirmacion.Enabled = false;
            lblEstado.Text = "Validando acceso...";
            try
            {
                if (pendiente != null)
                {
                    if (ValidacionDatos.SoloCeros(clave))
                    {
                        MostrarError("La contraseña no puede contener solo ceros.");
                        return;
                    }
                    if (clave != txtConfirmacion.Text) throw new ArgumentException("Las contraseñas no coinciden.");
                    var cambio = pendiente;
                    await Task.Run(() => AccesoDatos.CambiarClave(cambio, claveAnterior, clave));
                    pendiente = null; claveAnterior = "";
                    txtConfirmacion.Visible = false; lblRol.Visible = false;
                    txtUsuario.ReadOnly = false; lblClave.Text = "Contraseña";
                    btnIngresar.Text = "Ingresar";
                }
                var respuesta = await autenticar(usuario, clave);
                if (respuesta.Usuario is not AccesoUsuario resultado)
                {
                    MostrarError(respuesta.Error ?? "No se pudo iniciar sesión. Revisa tus datos.");
                    return;
                }
                if (resultado.CambiarClave)
                {
                    pendiente = resultado; claveAnterior = clave;
                    txtUsuario.ReadOnly = true;
                    txtClave.Clear(); txtConfirmacion.Clear();
                    txtConfirmacion.Visible = lblRol.Visible = true;
                    lblRol.Text = "Confirmar nueva contraseña";
                    lblClave.Text = "Nueva contraseña (12–128 caracteres)";
                    btnIngresar.Text = "Cambiar contraseña e ingresar";
                    lblEstado.Text = "Debes cambiar tu contraseña antes de entrar.";
                }
                else
                {
                    Acceso = resultado;
                    txtClave.Clear(); txtConfirmacion.Clear(); claveAnterior = "";
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (ArgumentException ex) { MostrarError(ex.Message); }
            catch (InvalidOperationException ex) { MostrarError(ex.Message); }
            catch (Exception) { MostrarError("No se pudo conectar al sistema. Revisa la configuración de conexión e inténtalo nuevamente."); }
            finally
            {
                trabajando = false;
                if (!IsDisposed)
                {
                    btnIngresar.Enabled = txtUsuario.Enabled = txtClave.Enabled = txtConfirmacion.Enabled = true;
                    txtClave.Focus();
                }
            }
        }

        private void MostrarError(string mensaje)
        {
            lblEstado.ForeColor = ProductoEstilo.Error;
            lblEstado.Text = mensaje;
            txtClave.Clear();
            txtConfirmacion.Clear();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (trabajando && DialogResult != DialogResult.OK) { e.Cancel = true; return; }
            claveAnterior = "";
            base.OnFormClosing(e);
        }
    }
}
