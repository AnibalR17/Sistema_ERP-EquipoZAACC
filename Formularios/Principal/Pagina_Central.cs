using Sistema_ERP.Clases;
using System.Drawing.Drawing2D;

namespace Sistema_ERP
{
    public partial class Pagina_Central : Form
    {
        private readonly Dictionary<string, Form> pantallas = new();
        private readonly Dictionary<Form, Button> navegacion = new();
        private Form? inicio;

        public Pagina_Central()
        {
            InitializeComponent();
            ConfigurarMdi();
            homeButton.Click += (_, _) => MostrarInicio();
            inventoryButton.Click += (_, _) => MostrarInventario();
            transferButton.Click += (_, _) => MostrarTraspaso();
            validationButton.Click += (_, _) => MostrarValidacion();
            bulkButton.Click += (_, _) => MostrarPantalla("Carga masiva", bulkButton, () => new CargaMasiva());
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
            // El diseno puede generar este evento antes de que termine InitializeComponent.
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
                using var conexion = Conexion.CrearConexion();
                await conexion.OpenAsync();
                MessageBox.Show(this, "Conexion a la base de datos exitosa.",
                    "Conexion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                MessageBox.Show(this,
                    "No se pudo conectar a la base de datos. Verifique la conexion a Internet y la configuracion de acceso.",
                    "Error de conexion", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void ConfigurarMdi()
        {
            SuspendLayout();
            IsMdiContainer = true;
            header.Parent = this;
            main.Controls.Remove(content);
            Controls.Remove(main);
            main.Dispose();
            // El area MDI ocupa el espacio disponible junto al encabezado y al menu lateral compartidos.
            Controls.SetChildIndex(header, 1);
            panelContenedor.BringToFront();
            inicio = CrearPantalla(content, "Inicio");
            pantallas.Add("Inicio", inicio);
            navegacion.Add(inicio, homeButton);
            MdiChildActivate += (_, _) => ActualizarNavegacion();

            var proveedoresButton = new Button
            {
                Name = "proveedoresButton", Text = "Proveedores",
                Location = new Point(inventoryButton.Left, inventoryButton.Bottom), Size = catalogButton.Size,
                FlatStyle = FlatStyle.Flat, BackColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft, Padding = catalogButton.Padding,
                Cursor = Cursors.Hand, Font = new Font(Font, FontStyle.Regular),
                TabIndex = inventoryButton.TabIndex + 1
            };
            proveedoresButton.FlatAppearance.BorderSize = 0;
            proveedoresButton.Click += (_, _) => MostrarPantalla("Proveedores", proveedoresButton,
                () => new Vistas.PantallaProveedores());
            panelContenedor.Controls.Add(proveedoresButton);
            foreach (var boton in new[] { transferButton, validationButton, bulkButton, catalogButton,
                returnsButton, rolesButton, investorsButton, settingsButton })
            {
                boton.Top += proveedoresButton.Height;
                boton.TabIndex++;
            }

            // Permite desplazar el menu de forma independiente para acceder a todas las opciones en ventanas pequenas.
            var menu = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            var pie = new Panel { Dock = DockStyle.Bottom, Height = 130 };
            foreach (Control control in panelContenedor.Controls.Cast<Control>().ToArray())
            {
                if (control == userLabel || control == roleLabel || control == logoutButton)
                    pie.Controls.Add(control);
                else
                    menu.Controls.Add(control);
            }
            userLabel.Anchor = roleLabel.Anchor = logoutButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            userLabel.Top = 8;
            roleLabel.Top = 40;
            logoutButton.Top = 70;
            panelContenedor.Controls.Add(menu);
            panelContenedor.Controls.Add(pie);
            ResumeLayout(true);
            Shown += (_, _) => MostrarInicio();
        }

        private Form CrearPantalla(Control contenido, string titulo)
        {
            var pantalla = new Form { Text = titulo, AutoScaleMode = AutoScaleMode.None };
            contenido.Dock = DockStyle.Fill;
            pantalla.Controls.Add(contenido);
            return pantalla;
        }

        private void MostrarPantalla(string clave, Button boton, Func<Form> crear)
        {
            if (!pantallas.TryGetValue(clave, out var pantalla) || pantalla.IsDisposed)
            {
                pantalla = crear();
                pantallas[clave] = pantalla;
                navegacion[pantalla] = boton;
                pantalla.FormClosed += (_, _) =>
                {
                    pantallas.Remove(clave);
                    navegacion.Remove(pantalla);
                };
            }
            if (pantalla.MdiParent == null)
            {
                pantalla.WindowState = FormWindowState.Normal;
                pantalla.FormBorderStyle = FormBorderStyle.None;
                pantalla.ControlBox = false;
                pantalla.ShowInTaskbar = false;
                pantalla.MinimumSize = Size.Empty;
                pantalla.MdiParent = this;
                pantalla.Dock = DockStyle.Fill;
            }
            pantalla.Show();
            pantalla.BringToFront();
            pantalla.Activate();
            ActualizarNavegacion();
        }

        private void ActualizarNavegacion()
        {
            foreach (var boton in navegacion.Values.Distinct())
            {
                bool seleccionado = ActiveMdiChild != null &&
                    navegacion.TryGetValue(ActiveMdiChild, out var activo) && activo == boton;
                var estilo = seleccionado ? FontStyle.Bold : FontStyle.Regular;
                if (boton.Font.Style != estilo) boton.Font = new Font(boton.Font, estilo);
                boton.BackColor = seleccionado ? Color.FromArgb(243, 243, 243) : Color.White;
            }
            if (ActiveMdiChild != null) Text = $"Sistema ERP - {ActiveMdiChild.Text}";
        }

        private void catalogButton_Click(object? sender, EventArgs e) =>
            MostrarPantalla("Catalogo", catalogButton, () => CrearPantalla(new Catalogo_Productos(), "Catalogo"));

        private void MostrarInicio() =>
            MostrarPantalla("Inicio", homeButton, () => inicio!);

        private void MostrarInventario() =>
            MostrarPantalla("Compras", inventoryButton, () => new Vistas.PantallaCompras());

        private void MostrarTraspaso() =>
            MostrarPantalla("Traspasos", transferButton, () => CrearPantalla(new Traspaso_Tiendas(), "Traspasos"));

        private void MostrarValidacion() =>
            MostrarPantalla("Validacion", validationButton, () => CrearPantalla(new Validacion_Traspasos(), "Validacion"));

        private void returnsButton_Click(object sender, EventArgs e) =>
            MostrarPantalla("Apartados y devoluciones", returnsButton, () => new ApartadosDevoluciones());

        private void rolesButton_Click(object sender, EventArgs e) =>
            MostrarPantalla("Directorio y roles", rolesButton, () => new DirectorioRoles());

        private void investorsButton_Click(object sender, EventArgs e) =>
            MostrarPantalla("Control de inversores", investorsButton, () => new ControlInversores());

        private void settingsButton_Click(object sender, EventArgs e) =>
            MostrarPantalla("Configuraci\u00f3n y auditor\u00eda", settingsButton, () => new ConfiguracionAuditoria());

        private void logoutButton_Click(object sender, EventArgs e)
        {
            logoutButton.Enabled = false;
            using var login = new LoginERP.FrmLogin();
            Hide();
            try
            {
                login.ShowDialog();
            }
            finally
            {
                Close();
            }
        }

        private void salesButton_Click(object sender, EventArgs e) =>
            MostrarPantalla("Punto de Venta", salesButton, () => new PuntoVenta());

        private void notificationButton_Click(object sender, EventArgs e) =>
            DialogosMaqueta.Mostrar(this, new Notificaciones());
    }
}
