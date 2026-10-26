using Microsoft.Data.SqlClient;
using System.Data;

namespace Sistema_ERP
{
    public sealed partial class DirectorioRoles : Form
    {
        private TipoDirectorio tipoActual = TipoDirectorio.Clientes;
        private DataView? vista;
        private int? idSeleccionado;
        private bool seleccionActiva;
        private bool cambiandoPestana;

        public DirectorioRoles()
        {
            InitializeComponent();
            ConfigurarPresentacion();
            ConectarEventos();
            ActualizarBotones();
        }

        private void ConectarEventos()
        {
            tabClientes.Click += (_, _) => CambiarPestana(TipoDirectorio.Clientes, tabClientes, pestanas);
            tabProveedores.Click += (_, _) => CambiarPestana(TipoDirectorio.Proveedores, tabProveedores, pestanas);
            tabEmpleados.Click += (_, _) => CambiarPestana(TipoDirectorio.Empleados, tabEmpleados, pestanas);
            guardar.Click += (_, _) => Guardar(false);
            actualizar.Click += (_, _) => Guardar(true);
            nuevo.Click += (_, _) => Limpiar();
            desactivar.Click += (_, _) => CambiarEstado(false);
            reactivar.Click += (_, _) => CambiarEstado(true);
            tabla.SelectionChanged += (_, _) => Seleccionar();
            buscar.TextChanged += (_, _) => Filtrar();
            inactivos.CheckedChanged += (_, _) => { if (!cambiandoPestana) Cargar(); };
        }

        private void CambiarPestana(TipoDirectorio tipo, Button tab, FlowLayoutPanel tabs)
        {
            cambiandoPestana = true;
            try
            {
                vista = null; tabla.DataSource = null; tipoActual = tipo;
                foreach (Button otro in tabs.Controls) { ProductoEstilo.Boton(otro, otro == tab); otro.Width = 140; }
                buscar.Clear(); inactivos.Checked = false; ConfigurarCampos(); Limpiar(false);
            }
            finally { cambiandoPestana = false; }
            Cargar();
        }

        protected override void OnLoad(EventArgs e) { base.OnLoad(e); if (!ModoDiseno.Activo(this)) Cargar(); }

        private void Cargar()
        {
            if (ModoDiseno.Activo(this)) return;
            try
            {
                if (tipoActual == TipoDirectorio.Empleados)
                {
                    tienda.DisplayMember = "nombre"; tienda.ValueMember = "id_tienda"; tienda.DataSource = DirectorioDatos.Tiendas(); tienda.SelectedIndex = -1;
                }
                vista = DirectorioDatos.Listar(tipoActual, inactivos.Checked).DefaultView;
                tabla.DataSource = vista; Filtrar();
                if (tipoActual == TipoDirectorio.Empleados && tienda.Items.Count == 0)
                    Mensaje("Registra una tienda activa en Configuracion antes de agregar empleados.", true);
            }
            catch (Exception ex)
            {
                vista = null; tabla.DataSource = null; Limpiar(false); contador.Text = "No se pudieron cargar los registros";
                Mensaje("No se pudo cargar el directorio. " + ex.Message, true);
            }
        }

        private void ActualizarBotones()
        {
            guardar.Enabled = idSeleccionado is null;
            actualizar.Enabled = desactivar.Enabled = idSeleccionado.HasValue && seleccionActiva;
            reactivar.Enabled = idSeleccionado.HasValue && !seleccionActiva;
            foreach (TextBox campo in new[] { nombre, identificador, telefono, correo, direccion, contacto }) campo.ReadOnly = idSeleccionado.HasValue && !seleccionActiva;
            tienda.Enabled = idSeleccionado is null || seleccionActiva;
        }

        private void Limpiar(bool enfocar = true)
        {
            tabla.ClearSelection(); idSeleccionado = null; seleccionActiva = false;
            foreach (TextBox campo in new[] { nombre, identificador, telefono, correo, direccion, contacto }) campo.Clear();
            tienda.SelectedIndex = -1; estado.Text = ""; ActualizarBotones(); if (enfocar) nombre.Focus();
        }

        private void Seleccionar()
        {
            if (tabla.SelectedRows.Count != 1 || tabla.SelectedRows[0].DataBoundItem is not DataRowView fila) return;
            idSeleccionado = Convert.ToInt32(fila["Id"]); seleccionActiva = Convert.ToBoolean(fila["activo"]);
            foreach (var (campo, control) in new[] { ("Nombre", nombre), ("Identificador", identificador), ("Telefono", telefono), ("Correo", correo), ("Direccion", direccion), ("Contacto", contacto) })
                control.Text = Convert.ToString(fila[campo]);
            if (tipoActual == TipoDirectorio.Empleados && tienda.DataSource is DataTable tiendas)
            {
                int idTienda = Convert.ToInt32(fila["IdTienda"]);
                if (!tiendas.AsEnumerable().Any(r => r.Field<int>("id_tienda") == idTienda))
                    tiendas.Rows.Add(idTienda, Convert.ToString(fila["Tienda"]) + " (inactiva)");
                tienda.SelectedValue = idTienda;
            }
            estado.Text = ""; ActualizarBotones();
        }

        private void Guardar(bool editar)
        {
            if (editar && (idSeleccionado is null || !seleccionActiva)) return;
            guardar.Enabled = actualizar.Enabled = false;
            try
            {
                int? idTienda = tienda.SelectedValue is int id ? id : null;
                if (editar)
                {
                    if (!DirectorioDatos.Editar(tipoActual, idSeleccionado!.Value, nombre.Text, telefono.Text, correo.Text, identificador.Text, direccion.Text, contacto.Text, idTienda))
                    { Cargar(); Mensaje("El registro ya no esta disponible para actualizar.", true); return; }
                }
                else DirectorioDatos.Agregar(tipoActual, nombre.Text, telefono.Text, correo.Text, identificador.Text, direccion.Text, contacto.Text, idTienda);
                Cargar(); Mensaje(editar ? "Registro actualizado correctamente." : "Registro guardado correctamente.");
            }
            catch (SqlException ex) when (ex.Number is 2601 or 2627) { Mensaje("Ya existe un registro con ese documento, RTN o codigo, incluso si esta inactivo.", true); identificador.Focus(); }
            catch (ArgumentException ex) { Mensaje(ex.Message, true); }
            catch (Exception ex) { Mensaje("No se pudo guardar. " + ex.Message, true); }
            finally { ActualizarBotones(); }
        }

        private void CambiarEstado(bool activar)
        {
            if (idSeleccionado is not int id || seleccionActiva == activar) return;
            string motivo = "";
            if (activar)
            {
                if (MessageBox.Show(this, $"Reactivar a {nombre.Text}?", "Reactivar registro", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
            }
            else
            {
                string entidad = tipoActual switch { TipoDirectorio.Clientes => "cliente", TipoDirectorio.Proveedores => "proveedor", _ => "empleado" };
                using var dialogo = new MotivoDesactivacion(nombre.Text, entidad);
                if (dialogo.ShowDialog(this) != DialogResult.OK) return;
                motivo = dialogo.Motivo;
            }
            desactivar.Enabled = reactivar.Enabled = false;
            try
            {
                bool cambiado = activar ? DirectorioDatos.Reactivar(tipoActual, id) : DirectorioDatos.Desactivar(tipoActual, id, motivo);
                Cargar(); Mensaje(cambiado ? (activar ? "Registro reactivado correctamente." : "Registro desactivado correctamente.") : "El registro ya cambio de estado.");
            }
            catch (Exception ex) { Mensaje("No se pudo cambiar el estado. " + ex.Message, true); }
            finally { ActualizarBotones(); }
        }

        private void Filtrar()
        {
            if (vista is null) return;
            string texto = System.Text.RegularExpressions.Regex.Replace(buscar.Text.Trim(), @"[%*\[\]]", "[$0]").Replace("'", "''");
            vista.RowFilter = texto.Length == 0 ? "" : $"Nombre LIKE '%{texto}%' OR Identificador LIKE '%{texto}%' OR Telefono LIKE '%{texto}%' OR Correo LIKE '%{texto}%'";
            Limpiar(false); contador.Text = $"Mostrando {vista.Count} registros";
        }

        private void Mensaje(string texto, bool error = false) { estado.Text = texto; estado.ForeColor = error ? ProductoEstilo.Error : ProductoEstilo.Exito; }

    }
}
