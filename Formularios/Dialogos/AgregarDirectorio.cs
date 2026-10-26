using Microsoft.Data.SqlClient;

namespace Sistema_ERP
{
    public sealed partial class AgregarDirectorio : Form
    {
        private readonly TipoDirectorio tipo;
        public AgregarDirectorio() : this(TipoDirectorio.Clientes) { }

        public AgregarDirectorio(TipoDirectorio tipo)
        {
            this.tipo = tipo;
            InitializeComponent();
            ConfigurarPresentacion();
            guardar.Click += (_, _) => Guardar();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (ModoDiseno.Activo(this) || tipo != TipoDirectorio.Empleados) return;
            try
            {
                tienda.DisplayMember = "nombre"; tienda.ValueMember = "id_tienda";
                tienda.DataSource = DirectorioDatos.Tiendas(); tienda.SelectedIndex = -1;
                if (tienda.Items.Count == 0) { estado.Text = "Registra una tienda activa antes de agregar empleados."; guardar.Enabled = false; }
            }
            catch (Exception ex) { estado.Text = "No se pudieron cargar las tiendas. " + ex.Message; guardar.Enabled = false; }
        }

        private void Guardar()
        {
            guardar.Enabled = false;
            try
            {
                DirectorioDatos.Agregar(tipo, nombre.Text, telefono.Text, correo.Text, identificador.Text,
                    direccion.Text, contacto.Text, tienda.SelectedValue is int id ? id : null);
                DialogResult = DialogResult.OK;
            }
            catch (SqlException ex) when (ex.Number is 2601 or 2627)
            { estado.Text = "Ya existe un registro con ese código, RTN o documento fiscal."; }
            catch (ArgumentException ex) { estado.Text = ex.Message; }
            catch (Exception ex) { estado.Text = "No se pudo guardar el registro. " + ex.Message; }
            finally { guardar.Enabled = true; }
        }
    }
}
