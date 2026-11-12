using Microsoft.Data.SqlClient;
using System.Data;

namespace Sistema_ERP
{
    public sealed partial class ConfiguracionAuditoria : FormularioTematico
    {
        private DataView? vista;
        private int? idSeleccionado;
        private bool seleccionActiva;
        private DataTable? eventos;
        private readonly Stack<long> paginas = new();
        private long antes = long.MaxValue;
        private readonly System.Windows.Forms.Timer filtroAuditoria = new() { Interval = 350 };

        public ConfiguracionAuditoria()
        {
            InitializeComponent();
            PrepararAuditoria();
            PrepararFacturacion();
            ConectarEventos();
            RecargaManual.Agregar(accionesTienda, CargarTiendas);
            ActualizarBotones();
            PermisosInterfaz.Configurar(this, Modulos.Configuracion, guardar, actualizar, nuevo, desactivar, reactivar);
        }

        private void ConectarEventos()
        {
            guardar.Click += (_, _) => Guardar(false);
            actualizar.Click += (_, _) => Guardar(true);
            nuevo.Click += (_, _) => Limpiar();
            desactivar.Click += (_, _) => CambiarEstado(false);
            reactivar.Click += (_, _) => CambiarEstado(true);
            tiendas.SelectionChanged += (_, _) => Seleccionar();
            inactivas.CheckedChanged += (_, _) => CargarTiendas();
            buscar.TextChanged += (_, _) => Filtrar();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!ModoDiseno.Activo(this)) CargarTiendas();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (!ModoDiseno.Activo(this)) ReiniciarAuditoria();
        }

        private void PrepararAuditoria()
        {
            tablaAuditoria.Columns.Clear();
            tablaAuditoria.AutoGenerateColumns = true;
            tablaAuditoria.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            tablaAuditoria.ScrollBars = ScrollBars.Both;
            buscarAuditoria.MaxLength = 150;
            var acciones = new FlowLayoutPanel { Dock = DockStyle.Fill };
            var refrescar = new Button { Text = "Actualizar", AutoSize = true };
            RecargaManual.Preparar(refrescar);
            refrescar.Name = "reiniciarAuditoria";
            var anterior = new Button { Text = "Anteriores", AutoSize = true };
            var siguiente = new Button { Text = "Siguientes", AutoSize = true };
            layoutAuditoria.Controls.Remove(contadorAuditoria);
            contadorAuditoria.AutoSize = true;
            acciones.Controls.AddRange([refrescar, anterior, siguiente, contadorAuditoria]);
            layoutAuditoria.RowStyles[3].Height = 38;
            layoutAuditoria.Controls.Add(acciones, 0, 3);
            refrescar.Click += (_, _) => ReiniciarAuditoria();
            anterior.Click += (_, _) => { if (paginas.Count > 0) { antes = paginas.Pop(); CargarAuditoria(); } };
            siguiente.Click += (_, _) =>
            {
                if (eventos?.Rows.Count != 200) return;
                paginas.Push(antes); antes = Convert.ToInt64(eventos.Rows[^1]["Id"]); CargarAuditoria();
            };
            buscarAuditoria.TextChanged += (_, _) => { filtroAuditoria.Stop(); filtroAuditoria.Start(); };
            filtroAuditoria.Tick += (_, _) => { filtroAuditoria.Stop(); ReiniciarAuditoria(); };
            Disposed += (_, _) => filtroAuditoria.Dispose();
            layoutConfiguracion.Dock = DockStyle.Fill;
            layoutConfiguracion.RowStyles[2].Height = 120;
            layoutConfiguracion.RowStyles[6].SizeType = SizeType.Percent;
            layoutConfiguracion.RowStyles[6].Height = 45;
            layoutConfiguracion.RowStyles[8].SizeType = SizeType.Percent;
            layoutConfiguracion.RowStyles[8].Height = 55;
            layoutConfiguracion.MinimumSize = new Size(0, 690);
        }

        private void ReiniciarAuditoria() { paginas.Clear(); antes = long.MaxValue; CargarAuditoria(); }
        private void CargarAuditoria()
        {
            if (ModoDiseno.Activo(this)) return;
            try
            {
                eventos = AuditoriaDatos.Listar(buscarAuditoria.Text, antes);
                tablaAuditoria.DataSource = eventos;
                tablaAuditoria.Columns["Id"]!.Visible = false;
                tablaAuditoria.Columns["Fecha UTC"]!.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                contadorAuditoria.Text = $"Página {paginas.Count + 1}: {eventos.Rows.Count} eventos (máximo 200)";
            }
            catch (Exception ex) { eventos = null; tablaAuditoria.DataSource = null; contadorAuditoria.Text = "No se pudo cargar la bitácora: " + ex.Message; }
        }

        private void ActualizarBotones()
        {
            guardar.Enabled = idSeleccionado is null;
            actualizar.Enabled = desactivar.Enabled = idSeleccionado.HasValue && seleccionActiva;
            reactivar.Enabled = idSeleccionado.HasValue && !seleccionActiva;
            foreach (var campo in new[] { codigo, nombre, direccion, telefono }) campo.ReadOnly = idSeleccionado.HasValue && !seleccionActiva;
        }

        private void Limpiar(bool enfocar = true)
        {
            tiendas.ClearSelection(); idSeleccionado = null; seleccionActiva = false;
            codigo.Clear(); nombre.Clear(); direccion.Clear(); telefono.Clear(); estado.Text = "";
            ActualizarBotones(); if (enfocar) codigo.Focus();
        }

        private void Seleccionar()
        {
            if (tiendas.SelectedRows.Count != 1 || tiendas.SelectedRows[0].DataBoundItem is not DataRowView fila) return;
            idSeleccionado = Convert.ToInt32(fila["id_tienda"]); seleccionActiva = Convert.ToBoolean(fila["activo"]);
            codigo.Text = Convert.ToString(fila["Codigo"]); nombre.Text = Convert.ToString(fila["Nombre"]);
            direccion.Text = Convert.ToString(fila["Direccion"]); telefono.Text = Convert.ToString(fila["Telefono"]);
            estado.Text = ""; ActualizarBotones();
        }

        private void Guardar(bool editar)
        {
            if (editar && (idSeleccionado is null || !seleccionActiva)) return;
            guardar.Enabled = actualizar.Enabled = false;
            try
            {
                if (editar)
                {
                    if (!TiendaDatos.Editar(idSeleccionado!.Value, codigo.Text, nombre.Text, direccion.Text, telefono.Text))
                    { CargarTiendas(); Mensaje("La tienda ya no esta disponible para actualizar.", true); return; }
                }
                else TiendaDatos.Agregar(codigo.Text, nombre.Text, direccion.Text, telefono.Text);
                CargarTiendas(); ReiniciarAuditoria(); Mensaje(editar ? "Tienda actualizada correctamente." : "Tienda guardada correctamente.");
            }
            catch (SqlException ex) when (ex.Number is 2601 or 2627) { Mensaje("Ya existe una tienda con ese codigo, incluso si esta inactiva.", true); codigo.Focus(); }
            catch (ArgumentException ex) { Mensaje(ex.Message, true); }
            catch (Exception ex) { Mensaje("No se pudo guardar la tienda. " + ex.Message, true); }
            finally { ActualizarBotones(); }
        }

        private void CambiarEstado(bool activar)
        {
            if (idSeleccionado is not int id || seleccionActiva == activar) return;
            string motivo = "";
            if (activar)
            {
                if (MessageBox.Show(this, $"Reactivar la tienda {nombre.Text}?", "Reactivar tienda", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
            }
            else
            {
                using var dialogo = new MotivoDesactivacion(nombre.Text, "tienda");
                if (dialogo.ShowDialog(this) != DialogResult.OK) return;
                motivo = dialogo.Motivo;
            }
            desactivar.Enabled = reactivar.Enabled = false;
            try
            {
                bool cambiado = activar ? TiendaDatos.Reactivar(id) : TiendaDatos.Desactivar(id, motivo);
                CargarTiendas(); ReiniciarAuditoria(); Mensaje(cambiado ? (activar ? "Tienda reactivada correctamente." : "Tienda desactivada correctamente.") : "La tienda ya cambio de estado.");
            }
            catch (Exception ex) { Mensaje("No se pudo cambiar el estado. " + ex.Message, true); }
            finally { ActualizarBotones(); }
        }

        private void Mensaje(string mensaje, bool error = false)
        { estado.ForeColor = error ? ProductoEstilo.Error : ProductoEstilo.Exito; estado.Text = mensaje; }

        private void CargarTiendas()
        {
            if (ModoDiseno.Activo(this)) return;
            try
            {
                vista = TiendaDatos.Listar(inactivas.Checked).DefaultView;
                tiendas.DataSource = vista; Filtrar();
            }
            catch (Exception ex)
            {
                vista = null; tiendas.DataSource = null; Limpiar(false);
                estadoLista.Text = "No se pudieron cargar los registros";
                Mensaje("No se pudieron cargar las tiendas. " + ex.Message, true);
            }
        }

        private void Filtrar()
        {
            if (vista is null) return;
            string texto = System.Text.RegularExpressions.Regex.Replace(buscar.Text.Trim(), @"[%*\[\]]", "[$0]").Replace("'", "''");
            vista.RowFilter = texto.Length == 0 ? "" : $"Nombre LIKE '%{texto}%' OR Codigo LIKE '%{texto}%'";
            Limpiar(false); estadoLista.Text = $"Mostrando {vista.Count} registros";
        }
    }
}
