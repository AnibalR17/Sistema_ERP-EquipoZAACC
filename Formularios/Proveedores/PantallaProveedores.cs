using Sistema_ERP.Clases;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sistema_ERP.Vistas
{
    public partial class PantallaProveedores : Form
    {
        private DataView? vistaProveedores;
        private int? idProveedorSeleccionado;
        private bool proveedorSeleccionadoActivo;

        public PantallaProveedores()
        {
            InitializeComponent();
            ConfigurarPresentacion();
            btnNuevo.Click += (_, _) => LimpiarProveedor();
            btnGuardarProveedor.Click += (_, _) => GuardarProveedor(false);
            btnEditar.Click += (_, _) => GuardarProveedor(true);
            btnDesactivarProveedor.Click += (_, _) => CambiarEstadoProveedor(false);
            btnReactivarProveedor.Click += (_, _) => CambiarEstadoProveedor(true);
            chkProveedoresInactivos.CheckedChanged += (_, _) => CargarDatos();
            dgvProveedores.SelectionChanged += (_, _) => SeleccionarProveedor();
            txtBuscar.TextChanged += (_, _) => FiltrarProveedores();
        }

        private void PantallaProveedores_Load(object sender, EventArgs e)
        {
            ConfigurarEstiloGrid();
            CargarDatos();
        }

        private void ConfigurarEstiloGrid()
        {
            dgvProveedores.RowHeadersVisible = false;

            dgvProveedores.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvProveedores.GridColor = Color.FromArgb(235, 235, 235);

            dgvProveedores.EnableHeadersVisualStyles = false;
            dgvProveedores.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvProveedores.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dgvProveedores.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(100, 100, 100);
            dgvProveedores.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvProveedores.ColumnHeadersHeight = 40;

            dgvProveedores.DefaultCellStyle.SelectionBackColor = Color.FromArgb(240, 240, 240);
            dgvProveedores.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvProveedores.RowTemplate.Height = 45;
            dgvProveedores.AllowUserToAddRows = false;
            dgvProveedores.AllowUserToDeleteRows = false;
            dgvProveedores.ReadOnly = true;
            dgvProveedores.MultiSelect = false;
            dgvProveedores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProveedores.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 250, 250);
            dgvProveedores.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(100, 100, 100);
            dgvProveedores.AutoGenerateColumns = false;
            dgvProveedores.Columns.Clear();
            foreach (var (campo, titulo) in new[]
            {
                ("nombre_empresa", "Empresa"), ("rtn", "RTN"), ("contacto", "Contacto"),
                ("telefono", "Teléfono"), ("correo", "Correo"), ("direccion", "Dirección")
            })
                dgvProveedores.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = campo,
                    DataPropertyName = campo,
                    HeaderText = titulo,
                    MinimumWidth = campo == "telefono" ? 90 : 110
                });

            dgvProveedores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProveedores.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "activo",
                DataPropertyName = "activo",
                HeaderText = "Activo",
                MinimumWidth = 60
            });
            dgvProveedores.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "motivo_desactivacion",
                DataPropertyName = "motivo_desactivacion",
                HeaderText = "Motivo de desactivación",
                MinimumWidth = 140
            });
        }

        private void CargarDatos()
        {
            try
            {
                vistaProveedores = ProveedorDatos.Listar(chkProveedoresInactivos.Checked).DefaultView;
                dgvProveedores.DataSource = vistaProveedores;
                FiltrarProveedores();
                LimpiarProveedor();
            }
            catch (Exception ex)
            {
                MostrarErrorProveedor("cargar los proveedores", ex);
            }
        }

        private void SeleccionarProveedor()
        {
            if (dgvProveedores.SelectedRows.Count == 0 ||
                dgvProveedores.SelectedRows[0].DataBoundItem is not DataRowView fila) return;
            idProveedorSeleccionado = Convert.ToInt32(fila["id_proveedor"]);
            foreach (var (campo, texto) in CamposProveedor())
                texto.Text = Convert.ToString(fila[campo]) ?? "";
            proveedorSeleccionadoActivo = Convert.ToBoolean(fila["activo"]);
            foreach (var (_, texto) in CamposProveedor()) texto.ReadOnly = !proveedorSeleccionadoActivo;
            ActualizarBotonesProveedor();
        }

        private (string Campo, TextBox Texto)[] CamposProveedor() => new[]
        {
            ("nombre_empresa", txtEmpresaProveedor), ("rtn", txtRtnProveedor),
            ("contacto", txtContactoProveedor), ("telefono", txtTelefonoProveedor),
            ("correo", txtCorreoProveedor), ("direccion", txtDireccionProveedor)
        };

        private void LimpiarProveedor(bool enfocarNombre = true)
        {
            dgvProveedores.ClearSelection();
            idProveedorSeleccionado = null;
            proveedorSeleccionadoActivo = false;
            foreach (var (_, texto) in CamposProveedor())
            {
                texto.Clear();
                texto.ReadOnly = false;
            }
            ActualizarBotonesProveedor();
            if (enfocarNombre) txtEmpresaProveedor.Focus();
        }

        private void FiltrarProveedores()
        {
            if (vistaProveedores is null) return;
            string texto = txtBuscar.Text.Trim();
            string seguro = System.Text.RegularExpressions.Regex.Replace(texto, @"[%*\[\]]", "[$0]").Replace("'", "''");
            vistaProveedores.RowFilter = texto.Length == 0 ? "" :
                $"nombre_empresa LIKE '%{seguro}%' OR rtn LIKE '%{seguro}%'";
            LimpiarProveedor(enfocarNombre: false);
        }

        private void GuardarProveedor(bool actualizar)
        {
            if (actualizar && (idProveedorSeleccionado is null || !proveedorSeleccionadoActivo)) return;
            var proveedor = new Proveedor
            {
                IdProveedor = idProveedorSeleccionado ?? 0,
                NombreEmpresa = txtEmpresaProveedor.Text,
                Rtn = txtRtnProveedor.Text,
                Contacto = txtContactoProveedor.Text,
                Telefono = txtTelefonoProveedor.Text,
                Correo = txtCorreoProveedor.Text,
                Direccion = txtDireccionProveedor.Text
            };
            btnGuardarProveedor.Enabled = btnEditar.Enabled = false;
            try
            {
                proveedor.Validar();
                if (ProveedorDatos.ExisteRtn(proveedor.Rtn, actualizar ? proveedor.IdProveedor : null))
                {
                    MostrarRtnDuplicado();
                    return;
                }
                if (actualizar)
                {
                    if (!ProveedorDatos.Editar(proveedor))
                    {
                        MessageBox.Show(this, "El proveedor ya no está disponible para actualizar.", "Proveedores",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        CargarDatos();
                        return;
                    }
                }
                else
                {
                    ProveedorDatos.Agregar(proveedor);
                }
                MessageBox.Show(this, actualizar ? "Proveedor actualizado correctamente." : "Proveedor guardado correctamente.",
                    "Proveedores", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtBuscar.Clear();
                CargarDatos();
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                MostrarRtnDuplicado();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(this, ex.Message, "Proveedores", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MostrarErrorProveedor(actualizar ? "actualizar el proveedor" : "guardar el proveedor", ex);
            }
            finally
            {
                ActualizarBotonesProveedor();
            }
        }

        private void ActualizarBotonesProveedor()
        {
            bool seleccionado = idProveedorSeleccionado.HasValue;
            btnGuardarProveedor.Enabled = !seleccionado;
            btnEditar.Enabled = btnDesactivarProveedor.Enabled = seleccionado && proveedorSeleccionadoActivo;
            btnReactivarProveedor.Enabled = seleccionado && !proveedorSeleccionadoActivo;
        }

        private void CambiarEstadoProveedor(bool reactivar)
        {
            if (idProveedorSeleccionado is not int id || proveedorSeleccionadoActivo == reactivar ||
                dgvProveedores.SelectedRows.Count == 0 ||
                dgvProveedores.SelectedRows[0].DataBoundItem is not DataRowView fila) return;
            string nombre = Convert.ToString(fila["nombre_empresa"]) ?? "";
            string motivo = "";
            if (reactivar)
            {
                if (MessageBox.Show(this, $"¿Reactivar el proveedor «{nombre}»?", "Reactivar proveedor",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
                    != DialogResult.Yes) return;
            }
            else
            {
                using var dialogo = new MotivoDesactivacion(nombre, "proveedor");
                if (dialogo.ShowDialog(this) != DialogResult.OK) return;
                motivo = dialogo.Motivo;
            }
            btnDesactivarProveedor.Enabled = btnReactivarProveedor.Enabled = false;
            try
            {
                bool cambiado = reactivar ? ProveedorDatos.Reactivar(id) : ProveedorDatos.Desactivar(id, motivo);
                CargarDatos();
                MessageBox.Show(this, cambiado
                    ? (reactivar ? "Proveedor reactivado correctamente." : "Proveedor desactivado correctamente.")
                    : "El proveedor ya cambió de estado o no está disponible.",
                    "Proveedores", MessageBoxButtons.OK, cambiado ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MostrarErrorProveedor(reactivar ? "reactivar el proveedor" : "desactivar el proveedor", ex);
            }
            finally
            {
                ActualizarBotonesProveedor();
            }
        }

        private void MostrarRtnDuplicado()
        {
            MessageBox.Show(this, "Ya existe un proveedor con ese RTN.", "Proveedores",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtRtnProveedor.Focus();
        }

        private void MostrarErrorProveedor(string accion, Exception ex)
        {
            string detalle = ex is SqlException sql ? $"Código SQL: {sql.Number}." : ex.Message;
            MessageBox.Show(this, $"No se pudo {accion}. {detalle}", "Proveedores",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void pnlInfo_Paint(object sender, PaintEventArgs e)
        {
            DibujarPanelRedondeado(e.Graphics, pnlInfo.ClientRectangle, 12, pnlInfo.BackColor, Color.Transparent);
        }

        private void pnlContenedorGrid_Paint(object sender, PaintEventArgs e)
        {
            DibujarPanelRedondeado(e.Graphics, pnlContenedorGrid.ClientRectangle, 18, pnlContenedorGrid.BackColor, Color.FromArgb(235, 235, 235));
        }

        private void DibujarPanelRedondeado(Graphics g, Rectangle bounds, int radius, Color backColor, Color borderColor)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using GraphicsPath path = new GraphicsPath();
            int d = radius * 2;

            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d - 1, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d - 1, bounds.Bottom - d - 1, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d - 1, d, d, 90, 90);
            path.CloseFigure();

            using (SolidBrush brush = new SolidBrush(backColor))
                g.FillPath(brush, path);

            if (borderColor != Color.Transparent)
            {
                using (Pen pen = new Pen(borderColor, 1))
                    g.DrawPath(pen, path);
            }
        }

        private void Btn_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Button btn) btn.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void Btn_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Button btn) btn.BackColor = Color.White;
        }
    }
}
