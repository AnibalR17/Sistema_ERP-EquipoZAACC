using System.Drawing.Drawing2D;

namespace Sistema_ERP
{
    public partial class Pagina_Central : Form
    {
        private Catalogo_Productos? catalogo;
        private Inventario? inventario;
        private Traspaso_Tiendas? traspaso;
        private Validacion_Traspasos? validacion;

        public Pagina_Central()
        {
            InitializeComponent();
            homeButton.Click += (_, _) => MostrarInicio();
            inventoryButton.Click += (_, _) => MostrarInventario();
            transferButton.Click += (_, _) => MostrarTraspaso();
            validationButton.Click += (_, _) => MostrarValidacion();
            Header_Resize(this, EventArgs.Empty);
            foreach (Panel panel in new[] { infoPanel, cardPanel0, cardPanel1, cardPanel2, cardPanel3, chartPanel })
            {
                panel.Resize += DashboardPanel_Resize;
                UpdateRoundedRegion(panel);
            }
        }

        private void ToggleButton_Click(object? sender, EventArgs e)
        {
            panelContenedor.Visible = !panelContenedor.Visible;
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
            e.Graphics.DrawLine(Pens.LightGray, panelContenedor.Width - 1, 0, panelContenedor.Width - 1, panelContenedor.Height);
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

        private void infoLabel_Click(object sender, EventArgs e)
        {

        }

        private void catalogButton_Click(object? sender, EventArgs e)
        {
            if (catalogo == null)
            {
                catalogo = new Catalogo_Productos { Dock = DockStyle.Fill };
                catalogo.UsarNavegacionPrincipal();
                catalogo.HomeRequested += (_, _) => MostrarInicio();
                main.Controls.Add(catalogo);
            }

            MostrarVista(catalogo, catalogButton);
        }

        private void MostrarInicio()
        {
            MostrarVista(content, homeButton);
        }

        private void MostrarInventario()
        {
            if (inventario == null)
            {
                inventario = new Inventario { Dock = DockStyle.Fill };
                inventario.UsarNavegacionPrincipal();
                main.Controls.Add(inventario);
            }
            MostrarVista(inventario, inventoryButton);
        }

        private void MostrarTraspaso()
        {
            if (traspaso == null)
            {
                traspaso = new Traspaso_Tiendas { Dock = DockStyle.Fill };
                traspaso.UsarNavegacionPrincipal();
                main.Controls.Add(traspaso);
            }
            MostrarVista(traspaso, transferButton);
        }

        private void MostrarValidacion()
        {
            if (validacion == null)
            {
                validacion = new Validacion_Traspasos { Dock = DockStyle.Fill };
                validacion.UsarNavegacionPrincipal();
                main.Controls.Add(validacion);
            }
            MostrarVista(validacion, validationButton);
        }

        private void MostrarVista(Control vista, Button seleccionado)
        {
            foreach (Control? pantalla in new Control?[] { content, catalogo, inventario, traspaso, validacion })
                if (pantalla != null) pantalla.Visible = pantalla == vista;

            vista.BringToFront();
            foreach (Button boton in new[] { homeButton, catalogButton, inventoryButton, transferButton, validationButton })
            {
                FontStyle estilo = boton == seleccionado ? FontStyle.Bold : FontStyle.Regular;
                if (boton.Font.Style != estilo)
                {
                    boton.Font = new Font(boton.Font, estilo);
                }
                boton.BackColor = boton == seleccionado ? Color.FromArgb(243, 243, 243) : Color.White;
            }
        }

        private void returnsButton_Click(object sender, EventArgs e)
        {

        }

        private void rolesButton_Click(object sender, EventArgs e)
        {

        }

        private void investorsButton_Click(object sender, EventArgs e)
        {

        }

        private void settingsButton_Click(object sender, EventArgs e)
        {

        }

        private void logoutButton_Click(object sender, EventArgs e)
        {

        }

        private void salesButton_Click(object sender, EventArgs e)
        {

        }

        private void notificationButton_Click(object sender, EventArgs e)
        {

        }
    }
}
