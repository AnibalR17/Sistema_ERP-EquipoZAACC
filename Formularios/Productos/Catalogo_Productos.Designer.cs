namespace Sistema_ERP
{
    partial class Catalogo_Productos
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            sidebar = new System.Windows.Forms.Panel();
            brandLabel = new System.Windows.Forms.Label();
            discoverLabel = new System.Windows.Forms.Label();
            homeButton = new System.Windows.Forms.Button();
            salesButton = new System.Windows.Forms.Button();
            adminLabel = new System.Windows.Forms.Label();
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
            header = new System.Windows.Forms.Panel();
            toggleButton = new System.Windows.Forms.Button();
            searchBox = new System.Windows.Forms.TextBox();
            accountLabel = new System.Windows.Forms.Label();
            notificationButton = new System.Windows.Forms.Button();
            content = new System.Windows.Forms.Panel();
            titleLabel = new System.Windows.Forms.Label();
            subtitleLabel = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            btnNuevoProducto = new System.Windows.Forms.Button();
            txtBuscar = new System.Windows.Forms.TextBox();
            dgvProductos = new System.Windows.Forms.DataGridView();
            Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            button2 = new System.Windows.Forms.Button();
            btnEditar = new System.Windows.Forms.Button();
            btnDesactivar = new System.Windows.Forms.Button();
            sidebar.SuspendLayout();
            header.SuspendLayout();
            content.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProductos).BeginInit();
            SuspendLayout();
            // 
            // sidebar
            // 
            sidebar.BackColor = System.Drawing.Color.White;
            sidebar.Controls.Add(label2);
            sidebar.Controls.Add(label3);
            sidebar.Controls.Add(button2);
            sidebar.Controls.Add(brandLabel);
            sidebar.Controls.Add(discoverLabel);
            sidebar.Controls.Add(homeButton);
            sidebar.Controls.Add(salesButton);
            sidebar.Controls.Add(adminLabel);
            sidebar.Controls.Add(inventoryButton);
            sidebar.Controls.Add(transferButton);
            sidebar.Controls.Add(validationButton);
            sidebar.Controls.Add(bulkButton);
            sidebar.Controls.Add(catalogButton);
            sidebar.Controls.Add(returnsButton);
            sidebar.Controls.Add(rolesButton);
            sidebar.Controls.Add(investorsButton);
            sidebar.Controls.Add(settingsButton);
            sidebar.Controls.Add(userLabel);
            sidebar.Controls.Add(roleLabel);
            sidebar.Controls.Add(logoutButton);
            sidebar.Dock = System.Windows.Forms.DockStyle.Left;
            sidebar.Location = new System.Drawing.Point(0, 0);
            sidebar.Name = "sidebar";
            sidebar.Size = new System.Drawing.Size(265, 796);
            sidebar.TabIndex = 2;
            // 
            // brandLabel
            // 
            brandLabel.BackColor = System.Drawing.Color.Transparent;
            brandLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            brandLabel.Location = new System.Drawing.Point(24, 16);
            brandLabel.Name = "brandLabel";
            brandLabel.Size = new System.Drawing.Size(225, 40);
            brandLabel.TabIndex = 0;
            brandLabel.Text = "Sistema ERP";
            brandLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // discoverLabel
            // 
            discoverLabel.BackColor = System.Drawing.Color.Transparent;
            discoverLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            discoverLabel.Location = new System.Drawing.Point(24, 80);
            discoverLabel.Name = "discoverLabel";
            discoverLabel.Size = new System.Drawing.Size(220, 28);
            discoverLabel.TabIndex = 1;
            discoverLabel.Text = "Descubrir";
            discoverLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // homeButton
            // 
            homeButton.BackColor = System.Drawing.Color.White;
            homeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            homeButton.FlatAppearance.BorderSize = 0;
            homeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            homeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            homeButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            homeButton.Location = new System.Drawing.Point(14, 112);
            homeButton.Name = "homeButton";
            homeButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            homeButton.Size = new System.Drawing.Size(236, 40);
            homeButton.TabIndex = 2;
            homeButton.Text = "Inicio";
            homeButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            homeButton.UseVisualStyleBackColor = false;
            // 
            // salesButton
            // 
            salesButton.BackColor = System.Drawing.Color.White;
            salesButton.Cursor = System.Windows.Forms.Cursors.Hand;
            salesButton.FlatAppearance.BorderSize = 0;
            salesButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            salesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            salesButton.Location = new System.Drawing.Point(14, 154);
            salesButton.Name = "salesButton";
            salesButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            salesButton.Size = new System.Drawing.Size(236, 40);
            salesButton.TabIndex = 3;
            salesButton.Text = "Punto de Venta";
            salesButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            salesButton.UseVisualStyleBackColor = false;
            // 
            // adminLabel
            // 
            adminLabel.BackColor = System.Drawing.Color.Transparent;
            adminLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            adminLabel.Location = new System.Drawing.Point(24, 205);
            adminLabel.Name = "adminLabel";
            adminLabel.Size = new System.Drawing.Size(220, 28);
            adminLabel.TabIndex = 4;
            adminLabel.Text = "Administracion";
            adminLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // inventoryButton
            // 
            inventoryButton.BackColor = System.Drawing.Color.White;
            inventoryButton.Cursor = System.Windows.Forms.Cursors.Hand;
            inventoryButton.FlatAppearance.BorderSize = 0;
            inventoryButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            inventoryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            inventoryButton.Location = new System.Drawing.Point(14, 236);
            inventoryButton.Name = "inventoryButton";
            inventoryButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            inventoryButton.Size = new System.Drawing.Size(236, 40);
            inventoryButton.TabIndex = 5;
            inventoryButton.Text = "Inventario y movimientos";
            inventoryButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            inventoryButton.UseVisualStyleBackColor = false;
            // 
            // transferButton
            // 
            transferButton.BackColor = System.Drawing.Color.White;
            transferButton.Cursor = System.Windows.Forms.Cursors.Hand;
            transferButton.FlatAppearance.BorderSize = 0;
            transferButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            transferButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            transferButton.Location = new System.Drawing.Point(14, 276);
            transferButton.Name = "transferButton";
            transferButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            transferButton.Size = new System.Drawing.Size(236, 40);
            transferButton.TabIndex = 6;
            transferButton.Text = "Traspaso entre Tiendas";
            transferButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            transferButton.UseVisualStyleBackColor = false;
            // 
            // validationButton
            // 
            validationButton.BackColor = System.Drawing.Color.White;
            validationButton.Cursor = System.Windows.Forms.Cursors.Hand;
            validationButton.FlatAppearance.BorderSize = 0;
            validationButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            validationButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            validationButton.Location = new System.Drawing.Point(14, 316);
            validationButton.Name = "validationButton";
            validationButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            validationButton.Size = new System.Drawing.Size(236, 40);
            validationButton.TabIndex = 7;
            validationButton.Text = "Validacion traspaso";
            validationButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            validationButton.UseVisualStyleBackColor = false;
            // 
            // bulkButton
            // 
            bulkButton.BackColor = System.Drawing.Color.White;
            bulkButton.Cursor = System.Windows.Forms.Cursors.Hand;
            bulkButton.FlatAppearance.BorderSize = 0;
            bulkButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            bulkButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            bulkButton.Location = new System.Drawing.Point(14, 356);
            bulkButton.Name = "bulkButton";
            bulkButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            bulkButton.Size = new System.Drawing.Size(236, 40);
            bulkButton.TabIndex = 8;
            bulkButton.Text = "Carga masiva";
            bulkButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            bulkButton.UseVisualStyleBackColor = false;
            // 
            // catalogButton
            // 
            catalogButton.BackColor = System.Drawing.Color.White;
            catalogButton.Cursor = System.Windows.Forms.Cursors.Hand;
            catalogButton.FlatAppearance.BorderSize = 0;
            catalogButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            catalogButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            catalogButton.Location = new System.Drawing.Point(14, 396);
            catalogButton.Name = "catalogButton";
            catalogButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            catalogButton.Size = new System.Drawing.Size(236, 40);
            catalogButton.TabIndex = 9;
            catalogButton.Text = "Catalogo de productos";
            catalogButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            catalogButton.UseVisualStyleBackColor = false;
            // 
            // returnsButton
            // 
            returnsButton.BackColor = System.Drawing.Color.White;
            returnsButton.Cursor = System.Windows.Forms.Cursors.Hand;
            returnsButton.FlatAppearance.BorderSize = 0;
            returnsButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            returnsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            returnsButton.Location = new System.Drawing.Point(14, 436);
            returnsButton.Name = "returnsButton";
            returnsButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            returnsButton.Size = new System.Drawing.Size(236, 40);
            returnsButton.TabIndex = 10;
            returnsButton.Text = "Apartados y devoluciones";
            returnsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            returnsButton.UseVisualStyleBackColor = false;
            // 
            // rolesButton
            // 
            rolesButton.BackColor = System.Drawing.Color.White;
            rolesButton.Cursor = System.Windows.Forms.Cursors.Hand;
            rolesButton.FlatAppearance.BorderSize = 0;
            rolesButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            rolesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            rolesButton.Location = new System.Drawing.Point(14, 476);
            rolesButton.Name = "rolesButton";
            rolesButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            rolesButton.Size = new System.Drawing.Size(236, 40);
            rolesButton.TabIndex = 11;
            rolesButton.Text = "Directorio y roles";
            rolesButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            rolesButton.UseVisualStyleBackColor = false;
            // 
            // investorsButton
            // 
            investorsButton.BackColor = System.Drawing.Color.White;
            investorsButton.Cursor = System.Windows.Forms.Cursors.Hand;
            investorsButton.FlatAppearance.BorderSize = 0;
            investorsButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            investorsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            investorsButton.Location = new System.Drawing.Point(14, 516);
            investorsButton.Name = "investorsButton";
            investorsButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            investorsButton.Size = new System.Drawing.Size(236, 40);
            investorsButton.TabIndex = 12;
            investorsButton.Text = "Control de inversores";
            investorsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            investorsButton.UseVisualStyleBackColor = false;
            // 
            // settingsButton
            // 
            settingsButton.BackColor = System.Drawing.Color.White;
            settingsButton.Cursor = System.Windows.Forms.Cursors.Hand;
            settingsButton.FlatAppearance.BorderSize = 0;
            settingsButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            settingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            settingsButton.Location = new System.Drawing.Point(14, 556);
            settingsButton.Name = "settingsButton";
            settingsButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            settingsButton.Size = new System.Drawing.Size(236, 40);
            settingsButton.TabIndex = 13;
            settingsButton.Text = "Configuracion y auditoria";
            settingsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            settingsButton.UseVisualStyleBackColor = false;
            // 
            // userLabel
            // 
            userLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            userLabel.BackColor = System.Drawing.Color.Transparent;
            userLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            userLabel.Location = new System.Drawing.Point(24, 1474);
            userLabel.Name = "userLabel";
            userLabel.Size = new System.Drawing.Size(225, 30);
            userLabel.TabIndex = 14;
            userLabel.Text = "Usuario Adm-01";
            userLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // roleLabel
            // 
            roleLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            roleLabel.BackColor = System.Drawing.Color.Transparent;
            roleLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            roleLabel.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            roleLabel.Location = new System.Drawing.Point(24, 1502);
            roleLabel.Name = "roleLabel";
            roleLabel.Size = new System.Drawing.Size(225, 22);
            roleLabel.TabIndex = 15;
            roleLabel.Text = "Administrador";
            roleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // logoutButton
            // 
            logoutButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            logoutButton.BackColor = System.Drawing.Color.FromArgb(242, 242, 242);
            logoutButton.Cursor = System.Windows.Forms.Cursors.Hand;
            logoutButton.FlatAppearance.BorderSize = 0;
            logoutButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            logoutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            logoutButton.Location = new System.Drawing.Point(24, 1531);
            logoutButton.Name = "logoutButton";
            logoutButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            logoutButton.Size = new System.Drawing.Size(216, 38);
            logoutButton.TabIndex = 16;
            logoutButton.Text = "Cerrar sesion";
            logoutButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            logoutButton.UseVisualStyleBackColor = false;
            // 
            // header
            // 
            header.BackColor = System.Drawing.Color.White;
            header.Controls.Add(toggleButton);
            header.Controls.Add(searchBox);
            header.Controls.Add(accountLabel);
            header.Controls.Add(notificationButton);
            header.Dock = System.Windows.Forms.DockStyle.Top;
            header.Location = new System.Drawing.Point(265, 0);
            header.Name = "header";
            header.Size = new System.Drawing.Size(1296, 64);
            header.TabIndex = 3;
            // 
            // toggleButton
            // 
            toggleButton.BackColor = System.Drawing.Color.White;
            toggleButton.Cursor = System.Windows.Forms.Cursors.Hand;
            toggleButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            toggleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            toggleButton.Location = new System.Drawing.Point(32, 18);
            toggleButton.Name = "toggleButton";
            toggleButton.Size = new System.Drawing.Size(36, 30);
            toggleButton.TabIndex = 0;
            toggleButton.Text = "☰";
            toggleButton.UseVisualStyleBackColor = false;
            // 
            // searchBox
            // 
            searchBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            searchBox.Font = new System.Drawing.Font("Segoe UI", 12F);
            searchBox.Location = new System.Drawing.Point(82, 23);
            searchBox.Name = "searchBox";
            searchBox.PlaceholderText = "🔍 Buscador global...";
            searchBox.Size = new System.Drawing.Size(420, 27);
            searchBox.TabIndex = 1;
            // 
            // accountLabel
            // 
            accountLabel.BackColor = System.Drawing.Color.Transparent;
            accountLabel.Font = new System.Drawing.Font("Segoe UI", 11F);
            accountLabel.Location = new System.Drawing.Point(520, 16);
            accountLabel.Name = "accountLabel";
            accountLabel.Size = new System.Drawing.Size(145, 34);
            accountLabel.TabIndex = 2;
            accountLabel.Text = "Administrador ⌄";
            accountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // notificationButton
            // 
            notificationButton.BackColor = System.Drawing.Color.White;
            notificationButton.Cursor = System.Windows.Forms.Cursors.Hand;
            notificationButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            notificationButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            notificationButton.Location = new System.Drawing.Point(680, 18);
            notificationButton.Name = "notificationButton";
            notificationButton.Size = new System.Drawing.Size(54, 30);
            notificationButton.TabIndex = 3;
            notificationButton.Text = "🔔";
            notificationButton.UseVisualStyleBackColor = false;
            // 
            // content
            // 
            content.Controls.Add(btnDesactivar);
            content.Controls.Add(btnEditar);
            content.Controls.Add(dgvProductos);
            content.Controls.Add(txtBuscar);
            content.Controls.Add(btnNuevoProducto);
            content.Controls.Add(label1);
            content.Controls.Add(subtitleLabel);
            content.Controls.Add(titleLabel);
            content.Dock = System.Windows.Forms.DockStyle.Fill;
            content.Location = new System.Drawing.Point(265, 64);
            content.Name = "content";
            content.Padding = new System.Windows.Forms.Padding(32, 34, 32, 36);
            content.Size = new System.Drawing.Size(1296, 732);
            content.TabIndex = 4;
            // 
            // titleLabel
            // 
            titleLabel.BackColor = System.Drawing.Color.Transparent;
            titleLabel.Font = new System.Drawing.Font("Segoe UI", 27F, System.Drawing.FontStyle.Bold);
            titleLabel.Location = new System.Drawing.Point(32, 16);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new System.Drawing.Size(516, 72);
            titleLabel.TabIndex = 1;
            titleLabel.Text = "Catalogo de productos";
            titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // subtitleLabel
            // 
            subtitleLabel.BackColor = System.Drawing.Color.Transparent;
            subtitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            subtitleLabel.Location = new System.Drawing.Point(35, 88);
            subtitleLabel.Name = "subtitleLabel";
            subtitleLabel.Size = new System.Drawing.Size(331, 28);
            subtitleLabel.TabIndex = 2;
            subtitleLabel.Text = "Producto, Categorias y Proveedores.";
            subtitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            label1.BackColor = System.Drawing.Color.Transparent;
            label1.Font = new System.Drawing.Font("Segoe UI", 12F);
            label1.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            label1.Location = new System.Drawing.Point(35, 130);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(776, 28);
            label1.TabIndex = 3;
            label1.Text = "ℹ El stock actual es solo lectura: Cambia unicamente con moviminentos de inventario.";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnNuevoProducto
            // 
            btnNuevoProducto.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            btnNuevoProducto.ForeColor = System.Drawing.SystemColors.Window;
            btnNuevoProducto.Location = new System.Drawing.Point(35, 183);
            btnNuevoProducto.Name = "btnNuevoProducto";
            btnNuevoProducto.Size = new System.Drawing.Size(153, 56);
            btnNuevoProducto.TabIndex = 4;
            btnNuevoProducto.Text = "+ Nuevo Producto";
            btnNuevoProducto.UseVisualStyleBackColor = false;
            // 
            // txtBuscar
            // 
            txtBuscar.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtBuscar.Font = new System.Drawing.Font("Segoe UI", 12F);
            txtBuscar.Location = new System.Drawing.Point(35, 255);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.PlaceholderText = "🔍 Buscar...";
            txtBuscar.Size = new System.Drawing.Size(420, 27);
            txtBuscar.TabIndex = 5;
            // 
            // dgvProductos
            // 
            dgvProductos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProductos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Column1, Column2, Column3, Column4 });
            dgvProductos.Location = new System.Drawing.Point(35, 304);
            dgvProductos.Name = "dgvProductos";
            dgvProductos.RowHeadersWidth = 51;
            dgvProductos.Size = new System.Drawing.Size(540, 188);
            dgvProductos.TabIndex = 6;
            // 
            // Column1
            // 
            Column1.HeaderText = "Codigo";
            Column1.MinimumWidth = 6;
            Column1.Name = "Column1";
            Column1.Width = 125;
            // 
            // Column2
            // 
            Column2.HeaderText = "Nombre";
            Column2.MinimumWidth = 6;
            Column2.Name = "Column2";
            Column2.Width = 125;
            // 
            // Column3
            // 
            Column3.HeaderText = "Stock";
            Column3.MinimumWidth = 6;
            Column3.Name = "Column3";
            Column3.Width = 125;
            // 
            // Column4
            // 
            Column4.HeaderText = "Precio";
            Column4.MinimumWidth = 6;
            Column4.Name = "Column4";
            Column4.Width = 125;
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label2.BackColor = System.Drawing.Color.Transparent;
            label2.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            label2.Location = new System.Drawing.Point(24, 689);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(225, 30);
            label2.TabIndex = 17;
            label2.Text = "Usuario Adm-01";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label3.BackColor = System.Drawing.Color.Transparent;
            label3.Font = new System.Drawing.Font("Segoe UI", 9F);
            label3.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            label3.Location = new System.Drawing.Point(24, 717);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(225, 22);
            label3.TabIndex = 18;
            label3.Text = "Administrador";
            label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button2
            // 
            button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            button2.BackColor = System.Drawing.Color.FromArgb(242, 242, 242);
            button2.Cursor = System.Windows.Forms.Cursors.Hand;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button2.Location = new System.Drawing.Point(24, 746);
            button2.Name = "button2";
            button2.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            button2.Size = new System.Drawing.Size(216, 38);
            button2.TabIndex = 19;
            button2.Text = "Cerrar sesion";
            button2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            button2.UseVisualStyleBackColor = false;
            // 
            // btnEditar
            // 
            btnEditar.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            btnEditar.ForeColor = System.Drawing.SystemColors.Window;
            btnEditar.Location = new System.Drawing.Point(194, 183);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new System.Drawing.Size(153, 56);
            btnEditar.TabIndex = 7;
            btnEditar.Text = "Editar";
            btnEditar.UseVisualStyleBackColor = false;
            // 
            // btnDesactivar
            // 
            btnDesactivar.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            btnDesactivar.ForeColor = System.Drawing.SystemColors.Window;
            btnDesactivar.Location = new System.Drawing.Point(353, 183);
            btnDesactivar.Name = "btnDesactivar";
            btnDesactivar.Size = new System.Drawing.Size(153, 56);
            btnDesactivar.TabIndex = 8;
            btnDesactivar.Text = "Desactivar";
            btnDesactivar.UseVisualStyleBackColor = false;
            // 
            // Catalogo_Productos
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.Window;
            ClientSize = new System.Drawing.Size(1561, 796);
            Controls.Add(content);
            Controls.Add(header);
            Controls.Add(sidebar);
            Name = "Catalogo_Productos";
            Text = "Catalogo_Productos";
            sidebar.ResumeLayout(false);
            header.ResumeLayout(false);
            header.PerformLayout();
            content.ResumeLayout(false);
            content.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProductos).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel sidebar;
        private System.Windows.Forms.Label brandLabel;
        private System.Windows.Forms.Label discoverLabel;
        private System.Windows.Forms.Button homeButton;
        private System.Windows.Forms.Button salesButton;
        private System.Windows.Forms.Label adminLabel;
        private System.Windows.Forms.Button inventoryButton;
        private System.Windows.Forms.Button transferButton;
        private System.Windows.Forms.Button validationButton;
        private System.Windows.Forms.Button bulkButton;
        private System.Windows.Forms.Button catalogButton;
        private System.Windows.Forms.Button returnsButton;
        private System.Windows.Forms.Button rolesButton;
        private System.Windows.Forms.Button investorsButton;
        private System.Windows.Forms.Button settingsButton;
        private System.Windows.Forms.Label userLabel;
        private System.Windows.Forms.Label roleLabel;
        private System.Windows.Forms.Button logoutButton;
        private System.Windows.Forms.Panel header;
        private System.Windows.Forms.Button toggleButton;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Label accountLabel;
        private System.Windows.Forms.Button notificationButton;
        private System.Windows.Forms.Panel content;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnDesactivar;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.DataGridView dgvProductos;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Button btnNuevoProducto;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label subtitleLabel;
        private System.Windows.Forms.Label titleLabel;
    }
}