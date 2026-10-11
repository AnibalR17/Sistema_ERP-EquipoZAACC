using System.Drawing.Drawing2D;

namespace Sistema_ERP
{
    public partial class Pagina_Central : Form
    {
        public Pagina_Central()
        {
            InitializeComponent();
            Header_Resize(this, EventArgs.Empty);
            foreach (Panel panel in new[] { infoPanel, cardPanel0, cardPanel1, cardPanel2, cardPanel3, chartPanel })
            {
                panel.Resize += DashboardPanel_Resize;
                UpdateRoundedRegion(panel);
            }
        }

        private void ToggleButton_Click(object? sender, EventArgs e)
        {
            sidebar.Visible = !sidebar.Visible;
        }

        private void Header_Resize(object? sender, EventArgs e)
        {
            // Layout can raise this event before InitializeComponent finishes.
            if (searchBox == null || accountLabel == null || notificationButton == null) return;
            searchBox.Width = Math.Max(160, Math.Min(420, header.Width - 390));
            accountLabel.Left = searchBox.Right + 18;
            notificationButton.Left = accountLabel.Right + 12;
        }

        private void Sidebar_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.LightGray, sidebar.Width - 1, 0, sidebar.Width - 1, sidebar.Height);
        }

        private void Header_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.LightGray, 0, header.Height - 1, header.Width, header.Height - 1);
        }

        private static GraphicsPath RoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
            path.AddArc(rect.Left, rect.Top, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Top, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void UpdateRoundedRegion(Panel panel)
        {
            if (panel.Width < 2 || panel.Height < 2) return;
            using var path = RoundedPath(new Rectangle(0, 0, panel.Width, panel.Height), panel == infoPanel ? 18 : 30);
            var previous = panel.Region;
            panel.Region = new Region(path);
            previous?.Dispose();
        }

        private void DashboardPanel_Resize(object? sender, EventArgs e)
        {
            if (sender is Panel panel)
            {
                UpdateRoundedRegion(panel);
                panel.Invalidate();
            }
        }

        private void RoundedPanel_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel panel || panel.Width < 2 || panel.Height < 2) return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var path = RoundedPath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), panel == infoPanel ? 18 : 30);
            using var border = new Pen(Color.FromArgb(247, 247, 247));
            e.Graphics.DrawPath(border, path);
        }

        private void ChartPanel_Paint(object? sender, PaintEventArgs e)
        {
            RoundedPanel_Paint(sender, e);
            if (chartPanel.Width < 120 || chartPanel.Height < 160) return;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var dayFont = new Font("Segoe UI", 18F);
            using var ink = new SolidBrush(Color.FromArgb(18, 18, 18));
            float left = 54, right = chartPanel.Width - 54, baseline = chartPanel.Height - 110;
            float plotHeight = Math.Max(30, baseline - 95);
            using var line = new Pen(Color.FromArgb(18, 18, 18), 8);
            g.DrawLine(line, left, baseline, right, baseline);
            float[] levels = { 0, .30f, .15f, .60f, .45f, .80f, .70f };
            string[] days = { "L", "M", "X", "J", "V", "S", "D" };
            using var centered = new StringFormat { Alignment = StringAlignment.Center };
            for (int i = 0; i < days.Length; i++)
            {
                float x = left + (right - left) * i / 6;
                float y = baseline - plotHeight * levels[i];
                g.FillEllipse(ink, x - 11, y - 11, 22, 22);
                g.DrawString(days[i], dayFont, ink, new PointF(x, baseline + 42), centered);
            }
        }

        private async void btnprobarconexion_Click(object? sender, System.EventArgs e)
        {
            if (sender is not Button button) return;
            button.Enabled = false;

            try
            {
                using var conexion = ConexionDB.CrearConexion();
                await conexion.OpenAsync();
                MessageBox.Show(this, "Conexión a la base de datos exitosa.",
                    "Conexión", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                MessageBox.Show(this,
                    "No se pudo conectar a la base de datos. Verifique la conexión a Internet y la configuración de acceso.",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                button.Enabled = true;
            }
        }

        private void headingPanel_Paint(object? sender, System.Windows.Forms.PaintEventArgs e)
        {
        }
    }
}
