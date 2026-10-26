using Microsoft.Data.SqlClient;
using System.Data;

namespace Sistema_ERP
{
    public partial class Catalogo_Productos
    {
        private readonly TextBox txtNombreCategoria = new()
        {
            Name = "txtNombreCategoria", Dock = DockStyle.Fill, MaxLength = 100,
            PlaceholderText = "Nombre de la categoría"
        };
        private readonly TextBox txtDescripcionCategoria = new()
        {
            Name = "txtDescripcionCategoria", Dock = DockStyle.Fill, MaxLength = 300,
            Multiline = true, ScrollBars = ScrollBars.Vertical,
            PlaceholderText = "Descripción de la categoría (opcional)"
        };
        private readonly DataGridView dgvCategorias = PantallaMaqueta.Tabla("Nombre", "Descripción");
        private readonly Button btnGuardarCategoria = PantallaMaqueta.Boton("Guardar categoría", true);
        private readonly Button btnActualizarCategoria = PantallaMaqueta.Boton("Actualizar");
        private readonly Button btnDesactivarCategoria = PantallaMaqueta.Boton("Desactivar");
        private readonly Button btnReactivarCategoria = PantallaMaqueta.Boton("Reactivar");
        private readonly CheckBox chkCategoriasInactivas = new()
        {
            Text = "Mostrar también categorías inactivas", AutoSize = true
        };
        private int? idCategoriaSeleccionada;

        private Control CrearPanelCategorias()
        {
            var panel = PantallaMaqueta.Filas(36, 48, 36, 88, 76, 36, -100);
            panel.Controls.Add(PantallaMaqueta.Etiqueta("Nombre de la categoría (máximo 100 caracteres)"), 0, 0);
            panel.Controls.Add(txtNombreCategoria, 0, 1);
            panel.Controls.Add(PantallaMaqueta.Etiqueta("Descripción (opcional, máximo 300 caracteres)"), 0, 2);
            panel.Controls.Add(txtDescripcionCategoria, 0, 3);
            var acciones = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, AutoScroll = true };
            btnGuardarCategoria.AutoSize = true;
            btnGuardarCategoria.Click += GuardarCategoria;
            acciones.Controls.Add(btnGuardarCategoria);
            btnActualizarCategoria.Enabled = false;
            btnActualizarCategoria.Click += ActualizarCategoria;
            acciones.Controls.Add(btnActualizarCategoria);
            btnDesactivarCategoria.Enabled = false;
            btnDesactivarCategoria.Click += DesactivarCategoria;
            acciones.Controls.Add(btnDesactivarCategoria);
            btnReactivarCategoria.Enabled = false;
            btnReactivarCategoria.Click += ReactivarCategoria;
            acciones.Controls.Add(btnReactivarCategoria);
            var btnNuevaCategoria = PantallaMaqueta.Boton("Nueva categoría");
            btnNuevaCategoria.Click += (_, _) => LimpiarCategoria();
            acciones.Controls.Add(btnNuevaCategoria);
            panel.Controls.Add(acciones, 0, 4);
            chkCategoriasInactivas.CheckedChanged += (_, _) => CargarCategorias();
            panel.Controls.Add(chkCategoriasInactivas, 0, 5);
            dgvCategorias.Name = "dgvCategorias";
            dgvCategorias.AutoGenerateColumns = false;
            dgvCategorias.ReadOnly = true;
            dgvCategorias.AllowUserToAddRows = false;
            dgvCategorias.AllowUserToDeleteRows = false;
            dgvCategorias.MultiSelect = false;
            dgvCategorias.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCategorias.DefaultCellStyle.SelectionBackColor = ProductoEstilo.Suave;
            dgvCategorias.DefaultCellStyle.SelectionForeColor = ProductoEstilo.Texto;
            dgvCategorias.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;
            dgvCategorias.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Gray;
            dgvCategorias.SelectionChanged += (_, _) => SeleccionarCategoria();
            dgvCategorias.Columns[0].DataPropertyName = "nombre";
            dgvCategorias.Columns[1].DataPropertyName = "descripcion";
            dgvCategorias.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "activo", DataPropertyName = "activo", HeaderText = "Activa"
            });
            panel.Controls.Add(dgvCategorias, 0, 6);
            return panel;
        }

        private void CargarCategorias()
        {
            try
            {
                dgvCategorias.DataSource = CategoriaDatos.Listar(chkCategoriasInactivas.Checked);
                LimpiarCategoria();
            }
            catch (Exception)
            {
                MessageBox.Show(this, "No se pudieron cargar las categorías. Verifique la conexión e intente nuevamente.",
                    "Categorías", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SeleccionarCategoria()
        {
            if (dgvCategorias.SelectedRows.Count == 0 ||
                dgvCategorias.SelectedRows[0].DataBoundItem is not DataRowView fila) return;
            idCategoriaSeleccionada = Convert.ToInt32(fila["id_categoria"]);
            txtNombreCategoria.Text = Convert.ToString(fila["nombre"]) ?? "";
            txtDescripcionCategoria.Text = Convert.ToString(fila["descripcion"]) ?? "";
            bool activa = Convert.ToBoolean(fila["activo"]);
            btnActualizarCategoria.Enabled = activa;
            btnDesactivarCategoria.Enabled = activa;
            btnReactivarCategoria.Enabled = !activa;
            txtNombreCategoria.ReadOnly = txtDescripcionCategoria.ReadOnly = !activa;
            btnGuardarCategoria.Enabled = false;
        }

        private void LimpiarCategoria()
        {
            dgvCategorias.ClearSelection();
            idCategoriaSeleccionada = null;
            txtNombreCategoria.Clear();
            txtDescripcionCategoria.Clear();
            btnActualizarCategoria.Enabled = false;
            btnDesactivarCategoria.Enabled = false;
            btnReactivarCategoria.Enabled = false;
            txtNombreCategoria.ReadOnly = txtDescripcionCategoria.ReadOnly = false;
            btnGuardarCategoria.Enabled = true;
            txtNombreCategoria.Focus();
        }

        private void ReactivarCategoria(object? sender, EventArgs e)
        {
            if (idCategoriaSeleccionada is not int id ||
                dgvCategorias.SelectedRows.Count == 0 ||
                dgvCategorias.SelectedRows[0].DataBoundItem is not DataRowView fila ||
                Convert.ToBoolean(fila["activo"])) return;
            string nombre = Convert.ToString(fila["nombre"]) ?? "";
            if (MessageBox.Show(this, $"¿Reactivar la categoría \"{nombre}\"?",
                "Reactivar categoría", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
            btnReactivarCategoria.Enabled = false;
            try
            {
                bool reactivada = CategoriaDatos.Reactivar(id);
                LimpiarCategoria();
                CargarCategorias();
                MessageBox.Show(this,
                    reactivada ? "Categoría reactivada correctamente." : "La categoría ya está activa o no está disponible.",
                    "Categorías", MessageBoxButtons.OK,
                    reactivada ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show(this, "No se pudo reactivar la categoría. Verifique la conexión e intente nuevamente.",
                    "Categorías", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnReactivarCategoria.Enabled = idCategoriaSeleccionada.HasValue;
            }
        }

        private void DesactivarCategoria(object? sender, EventArgs e)
        {
            if (idCategoriaSeleccionada is not int id ||
                dgvCategorias.SelectedRows.Count == 0 ||
                dgvCategorias.SelectedRows[0].DataBoundItem is not DataRowView fila) return;

            string nombre = Convert.ToString(fila["nombre"]) ?? "";
            using var dialogo = new MotivoDesactivacion(nombre);
            if (FindForm() is Form propietario)
                DialogosMaqueta.Mostrar(propietario, dialogo, oscurecerFondo: false);
            else
                dialogo.ShowDialog(this);
            if (dialogo.DialogResult != DialogResult.OK) return;

            btnDesactivarCategoria.Enabled = false;
            try
            {
                bool desactivada = CategoriaDatos.Desactivar(id, dialogo.Motivo);
                LimpiarCategoria();
                CargarCategorias();
                MessageBox.Show(this,
                    desactivada ? "Categoría desactivada correctamente." : "La categoría ya está inactiva o no está disponible.",
                    "Categorías", MessageBoxButtons.OK,
                    desactivada ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show(this, "No se pudo desactivar la categoría. Verifique la conexión e intente nuevamente.",
                    "Categorías", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnDesactivarCategoria.Enabled = idCategoriaSeleccionada.HasValue;
            }
        }

        private void ActualizarCategoria(object? sender, EventArgs e)
        {
            if (idCategoriaSeleccionada is not int id) return;
            string nombre = txtNombreCategoria.Text.Trim();
            if (nombre.Length == 0)
            {
                MessageBox.Show(this, "Introduzca el nombre de la categoría.", "Categorías",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombreCategoria.Focus();
                return;
            }
            btnActualizarCategoria.Enabled = false;
            try
            {
                if (CategoriaDatos.ExisteNombre(nombre, id))
                {
                    MostrarCategoriaDuplicada();
                    return;
                }
                if (!CategoriaDatos.Editar(id, nombre, txtDescripcionCategoria.Text))
                {
                    MessageBox.Show(this, "La categoría ya no está activa o no está disponible.", "Categorías",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    CargarCategorias();
                    return;
                }
                MessageBox.Show(this, "Categoría actualizada correctamente.", "Categorías",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarCategorias();
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                MostrarCategoriaDuplicada();
            }
            catch (Exception)
            {
                MessageBox.Show(this, "No se pudo actualizar la categoría. Verifique la conexión e intente nuevamente.",
                    "Categorías", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnActualizarCategoria.Enabled = idCategoriaSeleccionada.HasValue;
            }
        }

        private void GuardarCategoria(object? sender, EventArgs e)
        {
            string nombre = txtNombreCategoria.Text.Trim();
            if (nombre.Length == 0)
            {
                MessageBox.Show(this, "Introduzca el nombre de la categoría.", "Categorías",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombreCategoria.Focus();
                return;
            }

            btnGuardarCategoria.Enabled = false;
            try
            {
                if (CategoriaDatos.ExisteNombre(nombre))
                {
                    MostrarCategoriaDuplicada();
                    return;
                }
                CategoriaDatos.Agregar(nombre, txtDescripcionCategoria.Text);
                txtNombreCategoria.Clear();
                txtDescripcionCategoria.Clear();
                MessageBox.Show(this, "Categoría guardada correctamente.", "Categorías",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarCategorias();
                txtNombreCategoria.Focus();
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                MostrarCategoriaDuplicada();
            }
            catch (Exception)
            {
                MessageBox.Show(this, "No se pudo guardar la categoría. Verifique la conexión e intente nuevamente.",
                    "Categorías", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnGuardarCategoria.Enabled = true;
            }
        }

        private void MostrarCategoriaDuplicada()
        {
            MessageBox.Show(this, "Ya existe una categoría con ese nombre, posiblemente inactiva.",
                "Categorías", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtNombreCategoria.Focus();
        }
    }
}
