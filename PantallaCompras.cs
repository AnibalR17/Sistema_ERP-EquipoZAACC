using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sistema_ERP.Vistas
{
    public partial class PantallaCompras : Form
    {
        private DataTable dtCarrito;

        public PantallaCompras()
        {
            InitializeComponent();
            ConfigurarPresentacion();
        }

        private void PantallaCompras_Load(object sender, EventArgs e)
        {
            ConfigurarEstiloGrid();
            InicializarCarrito();
        }

        private void InicializarCarrito()
        {
            dtCarrito = new DataTable();
            dtCarrito.Columns.Add("Codigo", typeof(string));
            dtCarrito.Columns.Add("Producto", typeof(string));
            dtCarrito.Columns.Add("Cantidad", typeof(int));
            dtCarrito.Columns.Add("Precio Unitario", typeof(decimal));
            dtCarrito.Columns.Add("Subtotal", typeof(decimal), "Cantidad * [Precio Unitario]");

            dgvDetalleCompra.DataSource = dtCarrito;
        }

        private void ConfigurarEstiloGrid()
        {
            dgvDetalleCompra.RowHeadersVisible = false;
            dgvDetalleCompra.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvDetalleCompra.GridColor = Color.FromArgb(235, 235, 235);
            dgvDetalleCompra.EnableHeadersVisualStyles = false;
            dgvDetalleCompra.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvDetalleCompra.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dgvDetalleCompra.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(100, 100, 100);
            dgvDetalleCompra.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvDetalleCompra.ColumnHeadersHeight = 40;

            dgvDetalleCompra.DefaultCellStyle.BackColor = Color.White;
            dgvDetalleCompra.DefaultCellStyle.ForeColor = Color.Black;
            dgvDetalleCompra.DefaultCellStyle.SelectionBackColor = Color.FromArgb(240, 240, 240);
            dgvDetalleCompra.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvDetalleCompra.RowTemplate.Height = 40;
            dgvDetalleCompra.AllowUserToAddRows = false;
            dgvDetalleCompra.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void pnlCard_Paint(object sender, PaintEventArgs e)
        {
            if (sender is Panel pnl)
            {
                DibujarPanelRedondeado(e.Graphics, pnl.ClientRectangle, 12, pnl.BackColor, Color.FromArgb(235, 235, 235));
            }
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
    }
}