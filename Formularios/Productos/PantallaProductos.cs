using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sistema_ERP.Vistas
{
    public partial class PantallaProductos : Form
    {
        public PantallaProductos()
        {
            InitializeComponent();
            ConfigurarPresentacion();
            btnDesactivar.Click += BtnDesactivar_Click;
        }

        private void PantallaProductos_Load(object sender, EventArgs e)
        {
            ConfigurarEstiloGrid();
            CargarDatos();
        }

        private void ConfigurarEstiloGrid()
        {
            dgvProductos.RowHeadersVisible = false;
            dgvProductos.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvProductos.GridColor = Color.FromArgb(235, 235, 235);
            dgvProductos.EnableHeadersVisualStyles = false;
            dgvProductos.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvProductos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dgvProductos.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(100, 100, 100);
            dgvProductos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvProductos.ColumnHeadersHeight = 40;

            dgvProductos.DefaultCellStyle.BackColor = Color.White;
            dgvProductos.DefaultCellStyle.ForeColor = Color.Black;
            dgvProductos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(240, 240, 240);
            dgvProductos.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvProductos.RowTemplate.Height = 45;
            dgvProductos.AllowUserToAddRows = false;
            dgvProductos.AllowUserToDeleteRows = false;
            dgvProductos.ReadOnly = true;
            dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void CargarDatos()
        {
            try
            {
                using (SqlConnection conn = ConexionDB.AbrirConexion())
                {
                    string query = "SELECT * FROM Producto WHERE activo = 1";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvProductos.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message);
            }
        }

        private void BtnDesactivar_Click(object? sender, EventArgs e)
        {
            if (dgvProductos.CurrentRow?.DataBoundItem is not DataRowView fila)
            {
                MessageBox.Show(this, "Seleccione un registro de la tabla.", "Desactivar");
                return;
            }
            if (MessageBox.Show(this, "¿Desactivar el registro seleccionado? Se conservara su historial.",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                if (!ProductoDatos.Desactivar(Convert.ToInt32(fila["id_producto"])))
                    MessageBox.Show(this, "El registro ya esta inactivo o no esta disponible.", "Desactivar");
                CargarDatos();
            }
            catch (Exception)
            {
                MessageBox.Show(this, "No se pudo desactivar el registro.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d - 1, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d - 1, bounds.Bottom - d - 1, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d - 1, d, d, 90, 90);
            path.CloseFigure();

            using (SolidBrush brush = new SolidBrush(backColor))
            {
                g.FillPath(brush, path);
            }

            if (borderColor != Color.Transparent)
            {
                using (Pen pen = new Pen(borderColor, 1))
                {
                    g.DrawPath(pen, path);
                }
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