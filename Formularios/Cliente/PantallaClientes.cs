using System.Data;
using Sistema_ERP.Clases;
using Sistema_ERP.Datos;

namespace Sistema_ERP.Vistas
{

    public partial class PantallaClientes
    {
        private int? _idEditando;

        public PantallaClientes()
        {
            ConstruirPresentacion();
            ConfigurarGrid();
            EnlazarEventos();
            CargarClientes();
            ActualizarBotones();
        }



        private void ConfigurarGrid()
        {
            dgvClientes.AutoGenerateColumns = false;
            dgvClientes.ReadOnly = true;
            dgvClientes.AllowUserToAddRows = false;
            dgvClientes.AllowUserToDeleteRows = false;
            dgvClientes.MultiSelect = false;
            dgvClientes.RowHeadersVisible = false;
            dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvClientes.EnableHeadersVisualStyles = false;
            dgvClientes.BorderStyle = BorderStyle.None;
            dgvClientes.BackgroundColor = Color.White;
            dgvClientes.GridColor = Color.FromArgb(211, 211, 211);
            dgvClientes.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvClientes.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvClientes.ColumnHeadersHeight = 40;
            dgvClientes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvClientes.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvClientes.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(125, 125, 125);
            dgvClientes.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;
            dgvClientes.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvClientes.DefaultCellStyle.BackColor = Color.White;
            dgvClientes.DefaultCellStyle.ForeColor = Color.FromArgb(18, 18, 18);
            dgvClientes.DefaultCellStyle.Font = new Font("Segoe UI", 11F);
            dgvClientes.DefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 243, 243);
            dgvClientes.DefaultCellStyle.SelectionForeColor = Color.FromArgb(18, 18, 18);
            dgvClientes.RowTemplate.Height = 38;

            dgvClientes.Columns.Clear();
            dgvClientes.Columns.Add(Columna("id_cliente", "Id", 0, visible: false));
            dgvClientes.Columns.Add(Columna("nombre", "Nombre", 30));
            dgvClientes.Columns.Add(Columna("telefono", "Teléfono", 16));
            dgvClientes.Columns.Add(Columna("correo", "Correo", 26));
            dgvClientes.Columns.Add(Columna("activo", "Activo", 8));
            var baja = Columna("desactivado_en", "Desactivado en", 16);
            baja.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            dgvClientes.Columns.Add(baja);
            dgvClientes.Columns.Add(Columna("motivo_desactivacion", "Motivo", 24));
        }

        private static DataGridViewColumn Columna(string campo, string titulo, float peso, bool visible = true)
        {
            DataGridViewColumn col = campo == "activo"
                ? new DataGridViewCheckBoxColumn()
                : new DataGridViewTextBoxColumn();
            col.DataPropertyName = campo;
            col.Name = campo;
            col.HeaderText = titulo;
            col.FillWeight = peso;
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            col.Visible = visible;
            return col;
        }

        private void CargarClientes()
        {
            try
            {
                dgvClientes.DataSource = ClienteDatos.Listar(chkClientesInactivos.Checked).DefaultView;
                dgvClientes.ClearSelection();
                ActualizarBotones();
            }
            catch (Exception ex)
            {
                MostrarError("No se pudo cargar la lista de clientes.\n" + ex.Message);
            }
        }



        private void EnlazarEventos()
        {
            btnNuevo.Click += (_, _) => Nuevo();
            btnEditar.Click += (_, _) => Editar();
            btnGuardarCliente.Click += (_, _) => Guardar();
            btnDesactivarCliente.Click += (_, _) => Desactivar();
            btnReactivarCliente.Click += (_, _) => Reactivar();
            chkClientesInactivos.CheckedChanged += (_, _) => CargarClientes();
            dgvClientes.SelectionChanged += (_, _) => ActualizarBotones();
            dgvClientes.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) Editar(); };

            dgvClientes.CellFormatting += (_, e) =>
            {
                if (e.RowIndex < 0 || dgvClientes.Rows[e.RowIndex].DataBoundItem is not DataRowView fila) return;
                if (fila["activo"] is bool activo && !activo)
                    e.CellStyle.ForeColor = Color.FromArgb(125, 125, 125);
            };
        }

        private void Nuevo()
        {
            _idEditando = null;
            LimpiarFormulario();
            dgvClientes.ClearSelection();
            txtNombreCliente.Focus();
        }

        private void Editar()
        {
            DataRowView? fila = FilaSeleccionada();
            if (fila is null) { MostrarAviso("Seleccione un cliente de la tabla."); return; }

            _idEditando = Convert.ToInt32(fila["id_cliente"]);
            txtNombreCliente.Text = Convert.ToString(fila["nombre"]) ?? "";
            txtTelefonoCliente.Text = Convert.ToString(fila["telefono"]) ?? "";
            txtCorreoCliente.Text = Convert.ToString(fila["correo"]) ?? "";
            txtNombreCliente.Focus();
        }

        private void Guardar()
        {
            var cliente = new Cliente
            {
                IdCliente = _idEditando ?? 0,
                Nombre = txtNombreCliente.Text,
                Telefono = txtTelefonoCliente.Text,
                Correo = txtCorreoCliente.Text
            };

            try
            {
                cliente.Validar();

                if (_idEditando is null) ClienteDatos.Agregar(cliente);
                else ClienteDatos.Editar(cliente);

                _idEditando = null;
                LimpiarFormulario();
                CargarClientes();
            }
            catch (ArgumentException ex)
            {
                MostrarError(ex.Message);
                txtNombreCliente.Focus();
            }
            catch (InvalidOperationException ex)
            {
                MostrarError(ex.Message);
                txtCorreoCliente.Focus();
            }
            catch (Exception)
            {
                MostrarError("No se pudo guardar el cliente. Verifique la conexión a la base de datos.");
            }
        }

        private void Desactivar()
        {
            DataRowView? fila = FilaSeleccionada();
            if (fila is null) { MostrarAviso("Seleccione un cliente de la tabla."); return; }
            if (fila["activo"] is bool activo && !activo) { MostrarAviso("El cliente ya está inactivo."); return; }

            int id = Convert.ToInt32(fila["id_cliente"]);
            string nombre = Convert.ToString(fila["nombre"]) ?? "";

            using var dialogo = new MotivoDesactivacion(nombre, "cliente");
            if (dialogo.ShowDialog(this) != DialogResult.OK) return;

            try
            {

                ClienteDatos.Desactivar(id, dialogo.Motivo);
                CargarClientes();
            }
            catch (ArgumentException ex) { MostrarError(ex.Message); }
            catch (Exception) { MostrarError("No se pudo desactivar el cliente."); }
        }

        private void Reactivar()
        {
            DataRowView? fila = FilaSeleccionada();
            if (fila is null) { MostrarAviso("Seleccione un cliente de la tabla."); return; }
            if (fila["activo"] is bool activo && activo) { MostrarAviso("El cliente ya está activo."); return; }

            string nombre = Convert.ToString(fila["nombre"]) ?? "";
            var confirmar = MessageBox.Show(this, $"¿Reactivar al cliente \"{nombre}\"?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmar != DialogResult.Yes) return;

            try
            {
                ClienteDatos.Reactivar(Convert.ToInt32(fila["id_cliente"]));
                CargarClientes();
            }
            catch (Exception) { MostrarError("No se pudo reactivar el cliente."); }
        }



        private DataRowView? FilaSeleccionada() => dgvClientes.CurrentRow?.Selected == true
            ? dgvClientes.CurrentRow.DataBoundItem as DataRowView
            : null;

        private void ActualizarBotones()
        {
            DataRowView? fila = FilaSeleccionada();
            bool hay = fila is not null;
            bool activo = hay && fila!["activo"] is bool a && a;

            btnEditar.Enabled = hay;
            btnDesactivarCliente.Enabled = hay && activo;
            btnReactivarCliente.Enabled = hay && !activo;
        }

        private void LimpiarFormulario()
        {
            txtNombreCliente.Clear();
            txtTelefonoCliente.Clear();
            txtCorreoCliente.Clear();
        }

        private void MostrarError(string mensaje) =>
            MessageBox.Show(this, mensaje, "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void MostrarAviso(string mensaje) =>
            MessageBox.Show(this, mensaje, "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
