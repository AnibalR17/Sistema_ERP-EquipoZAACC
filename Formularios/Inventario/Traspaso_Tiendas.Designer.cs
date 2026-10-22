#nullable enable

namespace Sistema_ERP
{
    partial class Traspaso_Tiendas
    {
        private System.ComponentModel.IContainer? components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            sidebar = new System.Windows.Forms.Panel();
            sidebarDivider = new System.Windows.Forms.Panel();
            brandLabel = new System.Windows.Forms.Label();
            discoverLabel = new System.Windows.Forms.Label();
            adminLabel = new System.Windows.Forms.Label();
            homeButton = new System.Windows.Forms.Button();
            salesButton = new System.Windows.Forms.Button();
            inventoryButton = new System.Windows.Forms.Button();
            transferButton = new System.Windows.Forms.Button();
            validationButton = new System.Windows.Forms.Button();
            bulkButton = new System.Windows.Forms.Button();
            catalogButton = new System.Windows.Forms.Button();
            returnsButton = new System.Windows.Forms.Button();
            rolesButton = new System.Windows.Forms.Button();
            investorsButton = new System.Windows.Forms.Button();
            settingsButton = new System.Windows.Forms.Button();
            userLabel = new System.Windows.Forms.Label();
            roleLabel = new System.Windows.Forms.Label();
            logoutButton = new System.Windows.Forms.Button();
            main = new System.Windows.Forms.Panel();
            header = new System.Windows.Forms.Panel();
            headerDivider = new System.Windows.Forms.Panel();
            toggleButton = new System.Windows.Forms.Button();
            searchBox = new System.Windows.Forms.TextBox();
            searchDivider = new System.Windows.Forms.Panel();
            accountLabel = new System.Windows.Forms.Label();
            notificationButton = new System.Windows.Forms.Button();
            content = new System.Windows.Forms.Panel();
            titulo = new System.Windows.Forms.Label();
            subtitulo = new System.Windows.Forms.Label();
            avisoPanel = new Sistema_ERP.PanelDisenoInventario();
            aviso = new System.Windows.Forms.Label();
            tiendasPanel = new Sistema_ERP.PanelDisenoInventario();
            pasoTiendas = new System.Windows.Forms.Label();
            tiendasLayout = new System.Windows.Forms.TableLayoutPanel();
            origenLabel = new System.Windows.Forms.Label();
            origen = new System.Windows.Forms.ComboBox();
            destinoLabel = new System.Windows.Forms.Label();
            destino = new System.Windows.Forms.ComboBox();
            productosPanel = new Sistema_ERP.PanelDisenoInventario();
            pasoProductos = new System.Windows.Forms.Label();
            productos = new System.Windows.Forms.DataGridView();
            productosColumna0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            productosColumna1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            productosColumna2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            productosColumna3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            resumenPanel = new Sistema_ERP.PanelDisenoInventario();
            pasoInventario = new System.Windows.Forms.Label();
            resumen = new System.Windows.Forms.Label();
            cancelar = new System.Windows.Forms.Button();
            registrar = new System.Windows.Forms.Button();
            sidebar.SuspendLayout();
            sidebarDivider.SuspendLayout();
            main.SuspendLayout();
            header.SuspendLayout();
            headerDivider.SuspendLayout();
            searchDivider.SuspendLayout();
            content.SuspendLayout();
            avisoPanel.SuspendLayout();
            tiendasPanel.SuspendLayout();
            tiendasLayout.SuspendLayout();
            productosPanel.SuspendLayout();
            resumenPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)productos).BeginInit();
            this.SuspendLayout();
            this.Size = new System.Drawing.Size(1500, 944);
            // sidebar
            sidebar.Name = "sidebar";
            sidebar.Location = new System.Drawing.Point(0, 0);
            sidebar.Size = new System.Drawing.Size(265, 944);
            sidebar.Dock = System.Windows.Forms.DockStyle.Left;
            sidebar.BackColor = System.Drawing.Color.White;

            // sidebarDivider
            sidebarDivider.Name = "sidebarDivider";
            sidebarDivider.Location = new System.Drawing.Point(264, 0);
            sidebarDivider.Size = new System.Drawing.Size(1, 944);
            sidebarDivider.Dock = System.Windows.Forms.DockStyle.Right;
            sidebarDivider.BackColor = System.Drawing.Color.FromArgb(225, 225, 225);
            sidebar.Controls.Add(sidebarDivider);
            // brandLabel
            brandLabel.Name = "brandLabel";
            brandLabel.Location = new System.Drawing.Point(24, 14);
            brandLabel.Size = new System.Drawing.Size(220, 40);
            brandLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            brandLabel.ForeColor = System.Drawing.Color.FromArgb(18, 18, 18);
            brandLabel.BackColor = System.Drawing.Color.Transparent;
            brandLabel.Text = "Sistema ERP";
            brandLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            sidebar.Controls.Add(brandLabel);
            // discoverLabel
            discoverLabel.Name = "discoverLabel";
            discoverLabel.Location = new System.Drawing.Point(24, 80);
            discoverLabel.Size = new System.Drawing.Size(220, 30);
            discoverLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            discoverLabel.ForeColor = System.Drawing.Color.FromArgb(18, 18, 18);
            discoverLabel.BackColor = System.Drawing.Color.Transparent;
            discoverLabel.Text = "Descubrir";
            discoverLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            sidebar.Controls.Add(discoverLabel);
            // adminLabel
            adminLabel.Name = "adminLabel";
            adminLabel.Location = new System.Drawing.Point(24, 194);
            adminLabel.Size = new System.Drawing.Size(220, 28);
            adminLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            adminLabel.ForeColor = System.Drawing.Color.FromArgb(18, 18, 18);
            adminLabel.BackColor = System.Drawing.Color.Transparent;
            adminLabel.Text = "Administracion";
            adminLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            sidebar.Controls.Add(adminLabel);
            // homeButton
            homeButton.Name = "homeButton";
            homeButton.Location = new System.Drawing.Point(14, 112);
            homeButton.Size = new System.Drawing.Size(236, 40);
            homeButton.BackColor = System.Drawing.Color.White;
            homeButton.Font = new System.Drawing.Font("Segoe UI", 11F);
            homeButton.Text = "Inicio";
            homeButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            homeButton.UseVisualStyleBackColor = false;
            homeButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            homeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            sidebar.Controls.Add(homeButton);
            homeButton.FlatAppearance.BorderSize = 0;
            // salesButton
            salesButton.Name = "salesButton";
            salesButton.Location = new System.Drawing.Point(14, 152);
            salesButton.Size = new System.Drawing.Size(236, 40);
            salesButton.BackColor = System.Drawing.Color.White;
            salesButton.Font = new System.Drawing.Font("Segoe UI", 11F);
            salesButton.Text = "Punto de Venta";
            salesButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            salesButton.UseVisualStyleBackColor = false;
            salesButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            salesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            sidebar.Controls.Add(salesButton);
            salesButton.FlatAppearance.BorderSize = 0;
            // inventoryButton
            inventoryButton.Name = "inventoryButton";
            inventoryButton.Location = new System.Drawing.Point(14, 230);
            inventoryButton.Size = new System.Drawing.Size(236, 40);
            inventoryButton.BackColor = System.Drawing.Color.White;
            inventoryButton.Font = new System.Drawing.Font("Segoe UI", 11F);
            inventoryButton.Text = "Inventario y movimientos";
            inventoryButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            inventoryButton.UseVisualStyleBackColor = false;
            inventoryButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            inventoryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            sidebar.Controls.Add(inventoryButton);
            inventoryButton.FlatAppearance.BorderSize = 0;
            // transferButton
            transferButton.Name = "transferButton";
            transferButton.Location = new System.Drawing.Point(14, 270);
            transferButton.Size = new System.Drawing.Size(236, 40);
            transferButton.BackColor = System.Drawing.Color.FromArgb(243, 243, 243);
            transferButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            transferButton.Text = "Traspaso entre Tiendas";
            transferButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            transferButton.UseVisualStyleBackColor = false;
            transferButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            transferButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            sidebar.Controls.Add(transferButton);
            transferButton.FlatAppearance.BorderSize = 0;
            // validationButton
            validationButton.Name = "validationButton";
            validationButton.Location = new System.Drawing.Point(14, 310);
            validationButton.Size = new System.Drawing.Size(236, 40);
            validationButton.BackColor = System.Drawing.Color.White;
            validationButton.Font = new System.Drawing.Font("Segoe UI", 11F);
            validationButton.Text = "Validacion traspaso";
            validationButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            validationButton.UseVisualStyleBackColor = false;
            validationButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            validationButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            sidebar.Controls.Add(validationButton);
            validationButton.FlatAppearance.BorderSize = 0;
            // bulkButton
            bulkButton.Name = "bulkButton";
            bulkButton.Location = new System.Drawing.Point(14, 350);
            bulkButton.Size = new System.Drawing.Size(236, 40);
            bulkButton.BackColor = System.Drawing.Color.White;
            bulkButton.Font = new System.Drawing.Font("Segoe UI", 11F);
            bulkButton.Text = "Carga masiva";
            bulkButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            bulkButton.UseVisualStyleBackColor = false;
            bulkButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            bulkButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            sidebar.Controls.Add(bulkButton);
            bulkButton.FlatAppearance.BorderSize = 0;
            // catalogButton
            catalogButton.Name = "catalogButton";
            catalogButton.Location = new System.Drawing.Point(14, 390);
            catalogButton.Size = new System.Drawing.Size(236, 40);
            catalogButton.BackColor = System.Drawing.Color.White;
            catalogButton.Font = new System.Drawing.Font("Segoe UI", 11F);
            catalogButton.Text = "Catalogo de productos";
            catalogButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            catalogButton.UseVisualStyleBackColor = false;
            catalogButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            catalogButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            sidebar.Controls.Add(catalogButton);
            catalogButton.FlatAppearance.BorderSize = 0;
            // returnsButton
            returnsButton.Name = "returnsButton";
            returnsButton.Location = new System.Drawing.Point(14, 430);
            returnsButton.Size = new System.Drawing.Size(236, 40);
            returnsButton.BackColor = System.Drawing.Color.White;
            returnsButton.Font = new System.Drawing.Font("Segoe UI", 11F);
            returnsButton.Text = "Apartados y devoluciones";
            returnsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            returnsButton.UseVisualStyleBackColor = false;
            returnsButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            returnsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            sidebar.Controls.Add(returnsButton);
            returnsButton.FlatAppearance.BorderSize = 0;
            // rolesButton
            rolesButton.Name = "rolesButton";
            rolesButton.Location = new System.Drawing.Point(14, 470);
            rolesButton.Size = new System.Drawing.Size(236, 40);
            rolesButton.BackColor = System.Drawing.Color.White;
            rolesButton.Font = new System.Drawing.Font("Segoe UI", 11F);
            rolesButton.Text = "Directorio y roles";
            rolesButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            rolesButton.UseVisualStyleBackColor = false;
            rolesButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            rolesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            sidebar.Controls.Add(rolesButton);
            rolesButton.FlatAppearance.BorderSize = 0;
            // investorsButton
            investorsButton.Name = "investorsButton";
            investorsButton.Location = new System.Drawing.Point(14, 510);
            investorsButton.Size = new System.Drawing.Size(236, 40);
            investorsButton.BackColor = System.Drawing.Color.White;
            investorsButton.Font = new System.Drawing.Font("Segoe UI", 11F);
            investorsButton.Text = "Control de inversores";
            investorsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            investorsButton.UseVisualStyleBackColor = false;
            investorsButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            investorsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            sidebar.Controls.Add(investorsButton);
            investorsButton.FlatAppearance.BorderSize = 0;
            // settingsButton
            settingsButton.Name = "settingsButton";
            settingsButton.Location = new System.Drawing.Point(14, 550);
            settingsButton.Size = new System.Drawing.Size(236, 40);
            settingsButton.BackColor = System.Drawing.Color.White;
            settingsButton.Font = new System.Drawing.Font("Segoe UI", 11F);
            settingsButton.Text = "Configuracion y auditoria";
            settingsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            settingsButton.UseVisualStyleBackColor = false;
            settingsButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            settingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            sidebar.Controls.Add(settingsButton);
            settingsButton.FlatAppearance.BorderSize = 0;
            // userLabel
            userLabel.Name = "userLabel";
            userLabel.Location = new System.Drawing.Point(24, 818);
            userLabel.Size = new System.Drawing.Size(220, 28);
            userLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            userLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            userLabel.ForeColor = System.Drawing.Color.FromArgb(18, 18, 18);
            userLabel.BackColor = System.Drawing.Color.Transparent;
            userLabel.Text = "Usuario Adm-01";
            userLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            sidebar.Controls.Add(userLabel);
            // roleLabel
            roleLabel.Name = "roleLabel";
            roleLabel.Location = new System.Drawing.Point(24, 846);
            roleLabel.Size = new System.Drawing.Size(220, 22);
            roleLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            roleLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            roleLabel.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            roleLabel.BackColor = System.Drawing.Color.Transparent;
            roleLabel.Text = "Administrador";
            roleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            sidebar.Controls.Add(roleLabel);
            // logoutButton
            logoutButton.Name = "logoutButton";
            logoutButton.Location = new System.Drawing.Point(24, 876);
            logoutButton.Size = new System.Drawing.Size(216, 38);
            logoutButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            logoutButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            logoutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            logoutButton.UseVisualStyleBackColor = false;
            logoutButton.BackColor = System.Drawing.Color.FromArgb(242, 242, 242);
            logoutButton.Text = "Cerrar sesion";
            sidebar.Controls.Add(logoutButton);
            logoutButton.FlatAppearance.BorderSize = 0;
            // main
            main.Name = "main";
            main.Location = new System.Drawing.Point(265, 0);
            main.Size = new System.Drawing.Size(1235, 944);
            main.Dock = System.Windows.Forms.DockStyle.Fill;
            main.BackColor = System.Drawing.Color.White;

            // header
            header.Name = "header";
            header.Location = new System.Drawing.Point(0, 0);
            header.Size = new System.Drawing.Size(1235, 64);
            header.Dock = System.Windows.Forms.DockStyle.Top;
            header.BackColor = System.Drawing.Color.White;

            // headerDivider
            headerDivider.Name = "headerDivider";
            headerDivider.Location = new System.Drawing.Point(0, 63);
            headerDivider.Size = new System.Drawing.Size(1235, 1);
            headerDivider.Dock = System.Windows.Forms.DockStyle.Bottom;
            headerDivider.BackColor = System.Drawing.Color.FromArgb(225, 225, 225);
            header.Controls.Add(headerDivider);
            // toggleButton
            toggleButton.Name = "toggleButton";
            toggleButton.Location = new System.Drawing.Point(32, 18);
            toggleButton.Size = new System.Drawing.Size(36, 30);
            toggleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            toggleButton.UseVisualStyleBackColor = false;
            toggleButton.BackColor = System.Drawing.Color.White;
            toggleButton.Text = "≡";
            header.Controls.Add(toggleButton);
            // searchBox
            searchBox.Name = "searchBox";
            searchBox.Location = new System.Drawing.Point(80, 22);
            searchBox.Size = new System.Drawing.Size(420, 24);
            searchBox.Font = new System.Drawing.Font("Segoe UI", 11F);
            searchBox.PlaceholderText = "🔍 Buscador global...";
            searchBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            header.Controls.Add(searchBox);
            // searchDivider
            searchDivider.Name = "searchDivider";
            searchDivider.Location = new System.Drawing.Point(80, 48);
            searchDivider.Size = new System.Drawing.Size(420, 1);
            searchDivider.BackColor = System.Drawing.Color.FromArgb(220, 220, 220);
            header.Controls.Add(searchDivider);
            // accountLabel
            accountLabel.Name = "accountLabel";
            accountLabel.Location = new System.Drawing.Point(518, 18);
            accountLabel.Size = new System.Drawing.Size(130, 30);
            accountLabel.Font = new System.Drawing.Font("Segoe UI", 11F);
            accountLabel.ForeColor = System.Drawing.Color.FromArgb(18, 18, 18);
            accountLabel.BackColor = System.Drawing.Color.Transparent;
            accountLabel.Text = "Administrador⌄";
            accountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            header.Controls.Add(accountLabel);
            // notificationButton
            notificationButton.Name = "notificationButton";
            notificationButton.Location = new System.Drawing.Point(660, 18);
            notificationButton.Size = new System.Drawing.Size(54, 30);
            notificationButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            notificationButton.UseVisualStyleBackColor = false;
            notificationButton.BackColor = System.Drawing.Color.White;
            notificationButton.Text = "🔔 1";
            header.Controls.Add(notificationButton);
            // content
            content.Name = "content";
            content.Location = new System.Drawing.Point(0, 64);
            content.Size = new System.Drawing.Size(1235, 880);
            content.BackColor = System.Drawing.Color.White;
            content.AutoScroll = true;
            content.Dock = System.Windows.Forms.DockStyle.Fill;

            // titulo
            titulo.Name = "titulo";
            titulo.Location = new System.Drawing.Point(32, 36);
            titulo.Size = new System.Drawing.Size(1163, 48);
            titulo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            titulo.Font = new System.Drawing.Font("Segoe UI", 27F, System.Drawing.FontStyle.Bold);
            titulo.ForeColor = System.Drawing.Color.FromArgb(18, 18, 18);
            titulo.BackColor = System.Drawing.Color.Transparent;
            titulo.Text = "Traspaso de Tiendas";
            titulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            content.Controls.Add(titulo);
            // subtitulo
            subtitulo.Name = "subtitulo";
            subtitulo.Location = new System.Drawing.Point(32, 84);
            subtitulo.Size = new System.Drawing.Size(1163, 28);
            subtitulo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            subtitulo.Font = new System.Drawing.Font("Segoe UI", 11F);
            subtitulo.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            subtitulo.BackColor = System.Drawing.Color.Transparent;
            subtitulo.Text = "Registro traspaso entre tiendas...";
            subtitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            content.Controls.Add(subtitulo);
            // avisoPanel
            avisoPanel.Name = "avisoPanel";
            avisoPanel.Location = new System.Drawing.Point(24, 122);
            avisoPanel.Size = new System.Drawing.Size(1187, 42);
            avisoPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            avisoPanel.Radio = 16;
            avisoPanel.BackColor = System.Drawing.Color.FromArgb(243, 243, 243);
            avisoPanel.MostrarSombra = false;
            content.Controls.Add(avisoPanel);
            // aviso
            aviso.Name = "aviso";
            aviso.Location = new System.Drawing.Point(16, 0);
            aviso.Size = new System.Drawing.Size(1155, 42);
            aviso.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            aviso.Font = new System.Drawing.Font("Segoe UI", 10F);
            aviso.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            aviso.BackColor = System.Drawing.Color.Transparent;
            aviso.Text = "ℹ Sigue los 3 pasos. El traspaso quedara Pendiente hasta que un Gerente lo apruebe en \"Validacion traspaso\".";
            aviso.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            avisoPanel.Controls.Add(aviso);
            // tiendasPanel
            tiendasPanel.Name = "tiendasPanel";
            tiendasPanel.Location = new System.Drawing.Point(24, 180);
            tiendasPanel.Size = new System.Drawing.Size(1187, 152);
            tiendasPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tiendasPanel.BackColor = System.Drawing.Color.White;
            content.Controls.Add(tiendasPanel);
            // pasoTiendas
            pasoTiendas.Name = "pasoTiendas";
            pasoTiendas.Location = new System.Drawing.Point(28, 16);
            pasoTiendas.Size = new System.Drawing.Size(1131, 30);
            pasoTiendas.Font = new System.Drawing.Font("Segoe UI", 11F);
            pasoTiendas.ForeColor = System.Drawing.Color.FromArgb(18, 18, 18);
            pasoTiendas.BackColor = System.Drawing.Color.Transparent;
            pasoTiendas.Text = "Paso 1 — Selecciona tiendas";
            pasoTiendas.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tiendasPanel.Controls.Add(pasoTiendas);
            // tiendasLayout
            tiendasLayout.Name = "tiendasLayout";
            tiendasLayout.Location = new System.Drawing.Point(28, 62);
            tiendasLayout.Size = new System.Drawing.Size(1131, 64);
            tiendasLayout.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tiendasLayout.RowCount = 2;
            tiendasLayout.ColumnCount = 2;
            tiendasPanel.Controls.Add(tiendasLayout);
            tiendasLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tiendasLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tiendasLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            tiendasLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            // origenLabel
            origenLabel.Name = "origenLabel";
            origenLabel.Location = new System.Drawing.Point(0, 0);
            origenLabel.Size = new System.Drawing.Size(450, 24);
            origenLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            origenLabel.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            origenLabel.BackColor = System.Drawing.Color.Transparent;
            origenLabel.Text = "Tienda Origen";
            origenLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tiendasLayout.Controls.Add(origenLabel);
            tiendasLayout.SetCellPosition(origenLabel, new System.Windows.Forms.TableLayoutPanelCellPosition(0, 0));
            // origen
            origen.Name = "origen";
            origen.Location = new System.Drawing.Point(0, 0);
            origen.Size = new System.Drawing.Size(450, 30);
            origen.BackColor = System.Drawing.Color.White;
            origen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            origen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            origen.AccessibleName = "Tienda Origen";
            origen.Dock = System.Windows.Forms.DockStyle.Fill;
            tiendasLayout.Controls.Add(origen);
            tiendasLayout.SetCellPosition(origen, new System.Windows.Forms.TableLayoutPanelCellPosition(0, 1));
            origen.Items.AddRange(new object[] { "Tienda Centro", "Tienda Norte" });
            origen.SelectedIndex = 0;
            // destinoLabel
            destinoLabel.Name = "destinoLabel";
            destinoLabel.Location = new System.Drawing.Point(0, 0);
            destinoLabel.Size = new System.Drawing.Size(450, 24);
            destinoLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            destinoLabel.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            destinoLabel.BackColor = System.Drawing.Color.Transparent;
            destinoLabel.Text = "Tienda Destino";
            destinoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tiendasLayout.Controls.Add(destinoLabel);
            tiendasLayout.SetCellPosition(destinoLabel, new System.Windows.Forms.TableLayoutPanelCellPosition(1, 0));
            // destino
            destino.Name = "destino";
            destino.Location = new System.Drawing.Point(0, 0);
            destino.Size = new System.Drawing.Size(450, 30);
            destino.BackColor = System.Drawing.Color.White;
            destino.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            destino.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            destino.AccessibleName = "Tienda Destino";
            destino.Dock = System.Windows.Forms.DockStyle.Fill;
            tiendasLayout.Controls.Add(destino);
            tiendasLayout.SetCellPosition(destino, new System.Windows.Forms.TableLayoutPanelCellPosition(1, 1));
            destino.Items.AddRange(new object[] { "Tienda Centro", "Tienda Norte" });
            destino.SelectedIndex = 1;
            // productosPanel
            productosPanel.Name = "productosPanel";
            productosPanel.Location = new System.Drawing.Point(24, 344);
            productosPanel.Size = new System.Drawing.Size(1187, 128);
            productosPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            productosPanel.BackColor = System.Drawing.Color.White;
            content.Controls.Add(productosPanel);
            // pasoProductos
            pasoProductos.Name = "pasoProductos";
            pasoProductos.Location = new System.Drawing.Point(28, 16);
            pasoProductos.Size = new System.Drawing.Size(1131, 30);
            pasoProductos.Font = new System.Drawing.Font("Segoe UI", 11F);
            pasoProductos.ForeColor = System.Drawing.Color.FromArgb(18, 18, 18);
            pasoProductos.BackColor = System.Drawing.Color.Transparent;
            pasoProductos.Text = "Paso 2 — Ingresa Productos";
            pasoProductos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            productosPanel.Controls.Add(pasoProductos);
            // productos
            productos.Name = "productos";
            productos.Location = new System.Drawing.Point(28, 58);
            productos.Size = new System.Drawing.Size(1131, 42);
            productos.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            productos.AllowUserToAddRows = false;
            productos.AllowUserToResizeRows = false;
            productos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            productos.GridColor = System.Drawing.Color.FromArgb(230, 230, 230);
            productos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            productos.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            productos.ReadOnly = true;
            productos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            productos.AllowUserToDeleteRows = false;
            productos.RowHeadersVisible = false;
            productos.ColumnHeadersHeight = 36;
            productos.MultiSelect = false;
            productos.BackgroundColor = System.Drawing.Color.White;
            productos.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            productos.EnableHeadersVisualStyles = false;
            productos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            productosPanel.Controls.Add(productos);
            productos.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.White;
            productos.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.Gray;
            productos.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F);
            productos.ColumnHeadersDefaultCellStyle.SelectionBackColor = System.Drawing.Color.White;
            productos.ColumnHeadersDefaultCellStyle.SelectionForeColor = System.Drawing.Color.Gray;
            productosColumna0.Name = "productosColumna0";
            productosColumna0.HeaderText = "Producto";
            productosColumna1.Name = "productosColumna1";
            productosColumna1.HeaderText = "Precio";
            productosColumna2.Name = "productosColumna2";
            productosColumna2.HeaderText = "Disponible";
            productosColumna3.Name = "productosColumna3";
            productosColumna3.HeaderText = "Cantidad";
            productos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { productosColumna0, productosColumna1, productosColumna2, productosColumna3 });
            // resumenPanel
            resumenPanel.Name = "resumenPanel";
            resumenPanel.Location = new System.Drawing.Point(24, 484);
            resumenPanel.Size = new System.Drawing.Size(1187, 104);
            resumenPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            resumenPanel.BackColor = System.Drawing.Color.White;
            content.Controls.Add(resumenPanel);
            // pasoInventario
            pasoInventario.Name = "pasoInventario";
            pasoInventario.Location = new System.Drawing.Point(28, 16);
            pasoInventario.Size = new System.Drawing.Size(1131, 30);
            pasoInventario.Font = new System.Drawing.Font("Segoe UI", 11F);
            pasoInventario.ForeColor = System.Drawing.Color.FromArgb(18, 18, 18);
            pasoInventario.BackColor = System.Drawing.Color.Transparent;
            pasoInventario.Text = "Paso 3 — Actualiza inventario";
            pasoInventario.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            resumenPanel.Controls.Add(pasoInventario);
            // resumen
            resumen.Name = "resumen";
            resumen.Location = new System.Drawing.Point(28, 50);
            resumen.Size = new System.Drawing.Size(1131, 26);
            resumen.Font = new System.Drawing.Font("Segoe UI", 9F);
            resumen.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            resumen.BackColor = System.Drawing.Color.Transparent;
            resumen.Text = "Inventario origen despues del traspaso";
            resumen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            resumenPanel.Controls.Add(resumen);
            // cancelar
            cancelar.Name = "cancelar";
            cancelar.Location = new System.Drawing.Point(32, 608);
            cancelar.Size = new System.Drawing.Size(110, 44);
            cancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cancelar.UseVisualStyleBackColor = false;
            cancelar.BackColor = System.Drawing.Color.White;
            cancelar.Text = "Cancelar";
            content.Controls.Add(cancelar);
            // registrar
            registrar.Name = "registrar";
            registrar.Location = new System.Drawing.Point(150, 608);
            registrar.Size = new System.Drawing.Size(190, 44);
            registrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            registrar.ForeColor = System.Drawing.Color.White;
            registrar.BackColor = System.Drawing.Color.Gray;
            registrar.Text = "Registrar Traspaso";
            registrar.UseVisualStyleBackColor = false;
            content.Controls.Add(registrar);
            registrar.FlatAppearance.BorderSize = 0;
            main.Controls.Add(content);
            main.Controls.Add(header);
            this.Controls.Add(main);
            this.Controls.Add(sidebar);
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.MinimumSize = new System.Drawing.Size(1050, 720);
            this.Name = "Traspaso_Tiendas";
            this.Text = "Sistema ERP — Traspaso de Tiendas";
            sidebar.ResumeLayout(false);
            sidebar.PerformLayout();
            sidebarDivider.ResumeLayout(false);
            sidebarDivider.PerformLayout();
            main.ResumeLayout(false);
            main.PerformLayout();
            header.ResumeLayout(false);
            header.PerformLayout();
            headerDivider.ResumeLayout(false);
            headerDivider.PerformLayout();
            searchDivider.ResumeLayout(false);
            searchDivider.PerformLayout();
            content.ResumeLayout(false);
            content.PerformLayout();
            avisoPanel.ResumeLayout(false);
            avisoPanel.PerformLayout();
            tiendasPanel.ResumeLayout(false);
            tiendasPanel.PerformLayout();
            tiendasLayout.ResumeLayout(false);
            tiendasLayout.PerformLayout();
            productosPanel.ResumeLayout(false);
            productosPanel.PerformLayout();
            resumenPanel.ResumeLayout(false);
            resumenPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)productos).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Panel sidebar = null!;
        private System.Windows.Forms.Panel sidebarDivider = null!;
        private System.Windows.Forms.Label brandLabel = null!;
        private System.Windows.Forms.Label discoverLabel = null!;
        private System.Windows.Forms.Label adminLabel = null!;
        private System.Windows.Forms.Button homeButton = null!;
        private System.Windows.Forms.Button salesButton = null!;
        private System.Windows.Forms.Button inventoryButton = null!;
        private System.Windows.Forms.Button transferButton = null!;
        private System.Windows.Forms.Button validationButton = null!;
        private System.Windows.Forms.Button bulkButton = null!;
        private System.Windows.Forms.Button catalogButton = null!;
        private System.Windows.Forms.Button returnsButton = null!;
        private System.Windows.Forms.Button rolesButton = null!;
        private System.Windows.Forms.Button investorsButton = null!;
        private System.Windows.Forms.Button settingsButton = null!;
        private System.Windows.Forms.Label userLabel = null!;
        private System.Windows.Forms.Label roleLabel = null!;
        private System.Windows.Forms.Button logoutButton = null!;
        private System.Windows.Forms.Panel main = null!;
        private System.Windows.Forms.Panel header = null!;
        private System.Windows.Forms.Panel headerDivider = null!;
        private System.Windows.Forms.Button toggleButton = null!;
        private System.Windows.Forms.TextBox searchBox = null!;
        private System.Windows.Forms.Panel searchDivider = null!;
        private System.Windows.Forms.Label accountLabel = null!;
        private System.Windows.Forms.Button notificationButton = null!;
        private System.Windows.Forms.Panel content = null!;
        private System.Windows.Forms.Label titulo = null!;
        private System.Windows.Forms.Label subtitulo = null!;
        private Sistema_ERP.PanelDisenoInventario avisoPanel = null!;
        private System.Windows.Forms.Label aviso = null!;
        private Sistema_ERP.PanelDisenoInventario tiendasPanel = null!;
        private System.Windows.Forms.Label pasoTiendas = null!;
        private System.Windows.Forms.TableLayoutPanel tiendasLayout = null!;
        private System.Windows.Forms.Label origenLabel = null!;
        private System.Windows.Forms.ComboBox origen = null!;
        private System.Windows.Forms.Label destinoLabel = null!;
        private System.Windows.Forms.ComboBox destino = null!;
        private Sistema_ERP.PanelDisenoInventario productosPanel = null!;
        private System.Windows.Forms.Label pasoProductos = null!;
        private System.Windows.Forms.DataGridView productos = null!;
        private System.Windows.Forms.DataGridViewTextBoxColumn productosColumna0 = null!;
        private System.Windows.Forms.DataGridViewTextBoxColumn productosColumna1 = null!;
        private System.Windows.Forms.DataGridViewTextBoxColumn productosColumna2 = null!;
        private System.Windows.Forms.DataGridViewTextBoxColumn productosColumna3 = null!;
        private Sistema_ERP.PanelDisenoInventario resumenPanel = null!;
        private System.Windows.Forms.Label pasoInventario = null!;
        private System.Windows.Forms.Label resumen = null!;
        private System.Windows.Forms.Button cancelar = null!;
        private System.Windows.Forms.Button registrar = null!;
    }
}