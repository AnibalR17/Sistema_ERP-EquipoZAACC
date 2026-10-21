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
        public PantallaProveedores()
        {
            InitializeComponent();
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
      
            dgvProveedores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void CargarDatos()
        {
            try
            {
                using (SqlConnection conn = ConexionDB.AbrirConexion())
                {
                    string query = "SELECT * FROM Proveedor";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvProveedores.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message);
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