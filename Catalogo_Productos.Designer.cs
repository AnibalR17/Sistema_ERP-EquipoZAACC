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
            sidebar = new Panel();
            brandLabel = new Label();
            discoverLabel = new Label();
            homeButton = new Button();
            salesButton = new Button();
            adminLabel = new Label();
            inventoryButton = new Button();
            transferButton = new Button();
            validationButton = new Button();
            bulkButton = new Button();
            catalogButton = new Button();
            returnsButton = new Button();
            rolesButton = new Button();
            investorsButton = new Button();
            settingsButton = new Button();
            userLabel = new Label();
            roleLabel = new Label();
            logoutButton = new Button();
            header = new Panel();
            toggleButton = new Button();
            searchBox = new TextBox();
            accountLabel = new Label();
            notificationButton = new Button();
            content = new Panel();
            titleLabel = new Label();
            subtitleLabel = new Label();
            label1 = new Label();
            btnNuevoProducto = new Button();
            txtBuscar = new TextBox();
            dgvProductos = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            Column3 = new DataGridViewTextBoxColumn();
            Column4 = new DataGridViewTextBoxColumn();
            label2 = new Label();
            label3 = new Label();
            button2 = new Button();
            btnEditar = new Button();
            btnEliminar = new Button();
            sidebar.SuspendLayout();
            header.SuspendLayout();
            content.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProductos).BeginInit();
            SuspendLayout();
            // 
            // sidebar
            // 
            sidebar.BackColor = Color.White;
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
            sidebar.Dock = DockStyle.Left;
            sidebar.Location = new Point(0, 0);
            sidebar.Name = "sidebar";
            sidebar.Size = new Size(265, 796);
            sidebar.TabIndex = 2;
            // 
            // brandLabel
            // 
            brandLabel.BackColor = Color.Transparent;
            brandLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            brandLabel.Location = new Point(24, 16);
            brandLabel.Name = "brandLabel";
            brandLabel.Size = new Size(225, 40);
            brandLabel.TabIndex = 0;
            brandLabel.Text = "Sistema ERP";
            brandLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // discoverLabel
            // 
            discoverLabel.BackColor = Color.Transparent;
            discoverLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            discoverLabel.Location = new Point(24, 80);
            discoverLabel.Name = "discoverLabel";
            discoverLabel.Size = new Size(220, 28);
            discoverLabel.TabIndex = 1;
            discoverLabel.Text = "Descubrir";
            discoverLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // homeButton
            // 
            homeButton.BackColor = Color.White;
            homeButton.Cursor = Cursors.Hand;
            homeButton.FlatAppearance.BorderSize = 0;
            homeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            homeButton.FlatStyle = FlatStyle.Flat;
            homeButton.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            homeButton.Location = new Point(14, 112);
            homeButton.Name = "homeButton";
            homeButton.Padding = new Padding(10, 0, 0, 0);
            homeButton.Size = new Size(236, 40);
            homeButton.TabIndex = 2;
            homeButton.Text = "Inicio";
            homeButton.TextAlign = ContentAlignment.MiddleLeft;
            homeButton.UseVisualStyleBackColor = false;
            // 
            // salesButton
            // 
            salesButton.BackColor = Color.White;
            salesButton.Cursor = Cursors.Hand;
            salesButton.FlatAppearance.BorderSize = 0;
            salesButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            salesButton.FlatStyle = FlatStyle.Flat;
            salesButton.Location = new Point(14, 154);
            salesButton.Name = "salesButton";
            salesButton.Padding = new Padding(10, 0, 0, 0);
            salesButton.Size = new Size(236, 40);
            salesButton.TabIndex = 3;
            salesButton.Text = "Punto de Venta";
            salesButton.TextAlign = ContentAlignment.MiddleLeft;
            salesButton.UseVisualStyleBackColor = false;
            // 
            // adminLabel
            // 
            adminLabel.BackColor = Color.Transparent;
            adminLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            adminLabel.Location = new Point(24, 205);
            adminLabel.Name = "adminLabel";
            adminLabel.Size = new Size(220, 28);
            adminLabel.TabIndex = 4;
            adminLabel.Text = "Administración";
            adminLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // inventoryButton
            // 
            inventoryButton.BackColor = Color.White;
            inventoryButton.Cursor = Cursors.Hand;
            inventoryButton.FlatAppearance.BorderSize = 0;
            inventoryButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            inventoryButton.FlatStyle = FlatStyle.Flat;
            inventoryButton.Location = new Point(14, 236);
            inventoryButton.Name = "inventoryButton";
            inventoryButton.Padding = new Padding(10, 0, 0, 0);
            inventoryButton.Size = new Size(236, 40);
            inventoryButton.TabIndex = 5;
            inventoryButton.Text = "Inventario y movimientos";
            inventoryButton.TextAlign = ContentAlignment.MiddleLeft;
            inventoryButton.UseVisualStyleBackColor = false;
            // 
            // transferButton
            // 
            transferButton.BackColor = Color.White;
            transferButton.Cursor = Cursors.Hand;
            transferButton.FlatAppearance.BorderSize = 0;
            transferButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            transferButton.FlatStyle = FlatStyle.Flat;
            transferButton.Location = new Point(14, 276);
            transferButton.Name = "transferButton";
            transferButton.Padding = new Padding(10, 0, 0, 0);
            transferButton.Size = new Size(236, 40);
            transferButton.TabIndex = 6;
            transferButton.Text = "Traspaso entre Tiendas";
            transferButton.TextAlign = ContentAlignment.MiddleLeft;
            transferButton.UseVisualStyleBackColor = false;
            // 
            // validationButton
            // 
            validationButton.BackColor = Color.White;
            validationButton.Cursor = Cursors.Hand;
            validationButton.FlatAppearance.BorderSize = 0;
            validationButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            validationButton.FlatStyle = FlatStyle.Flat;
            validationButton.Location = new Point(14, 316);
            validationButton.Name = "validationButton";
            validationButton.Padding = new Padding(10, 0, 0, 0);
            validationButton.Size = new Size(236, 40);
            validationButton.TabIndex = 7;
            validationButton.Text = "Validación traspaso";
            validationButton.TextAlign = ContentAlignment.MiddleLeft;
            validationButton.UseVisualStyleBackColor = false;
            // 
            // bulkButton
            // 
            bulkButton.BackColor = Color.White;
            bulkButton.Cursor = Cursors.Hand;
            bulkButton.FlatAppearance.BorderSize = 0;
            bulkButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            bulkButton.FlatStyle = FlatStyle.Flat;
            bulkButton.Location = new Point(14, 356);
            bulkButton.Name = "bulkButton";
            bulkButton.Padding = new Padding(10, 0, 0, 0);
            bulkButton.Size = new Size(236, 40);
            bulkButton.TabIndex = 8;
            bulkButton.Text = "Carga masiva";
            bulkButton.TextAlign = ContentAlignment.MiddleLeft;
            bulkButton.UseVisualStyleBackColor = false;
            // 
            // catalogButton
            // 
            catalogButton.BackColor = Color.White;
            catalogButton.Cursor = Cursors.Hand;
            catalogButton.FlatAppearance.BorderSize = 0;
            catalogButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            catalogButton.FlatStyle = FlatStyle.Flat;
            catalogButton.Location = new Point(14, 396);
            catalogButton.Name = "catalogButton";
            catalogButton.Padding = new Padding(10, 0, 0, 0);
            catalogButton.Size = new Size(236, 40);
            catalogButton.TabIndex = 9;
            catalogButton.Text = "Catálogo de productos";
            catalogButton.TextAlign = ContentAlignment.MiddleLeft;
            catalogButton.UseVisualStyleBackColor = false;
            // 
            // returnsButton
            // 
            returnsButton.BackColor = Color.White;
            returnsButton.Cursor = Cursors.Hand;
            returnsButton.FlatAppearance.BorderSize = 0;
            returnsButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            returnsButton.FlatStyle = FlatStyle.Flat;
            returnsButton.Location = new Point(14, 436);
            returnsButton.Name = "returnsButton";
            returnsButton.Padding = new Padding(10, 0, 0, 0);
            returnsButton.Size = new Size(236, 40);
            returnsButton.TabIndex = 10;
            returnsButton.Text = "Apartados y devoluciones";
            returnsButton.TextAlign = ContentAlignment.MiddleLeft;
            returnsButton.UseVisualStyleBackColor = false;
            // 
            // rolesButton
            // 
            rolesButton.BackColor = Color.White;
            rolesButton.Cursor = Cursors.Hand;
            rolesButton.FlatAppearance.BorderSize = 0;
            rolesButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            rolesButton.FlatStyle = FlatStyle.Flat;
            rolesButton.Location = new Point(14, 476);
            rolesButton.Name = "rolesButton";
            rolesButton.Padding = new Padding(10, 0, 0, 0);
            rolesButton.Size = new Size(236, 40);
            rolesButton.TabIndex = 11;
            rolesButton.Text = "Directorio y roles";
            rolesButton.TextAlign = ContentAlignment.MiddleLeft;
            rolesButton.UseVisualStyleBackColor = false;
            // 
            // investorsButton
            // 
            investorsButton.BackColor = Color.White;
            investorsButton.Cursor = Cursors.Hand;
            investorsButton.FlatAppearance.BorderSize = 0;
            investorsButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            investorsButton.FlatStyle = FlatStyle.Flat;
            investorsButton.Location = new Point(14, 516);
            investorsButton.Name = "investorsButton";
            investorsButton.Padding = new Padding(10, 0, 0, 0);
            investorsButton.Size = new Size(236, 40);
            investorsButton.TabIndex = 12;
            investorsButton.Text = "Control de inversores";
            investorsButton.TextAlign = ContentAlignment.MiddleLeft;
            investorsButton.UseVisualStyleBackColor = false;
            // 
            // settingsButton
            // 
            settingsButton.BackColor = Color.White;
            settingsButton.Cursor = Cursors.Hand;
            settingsButton.FlatAppearance.BorderSize = 0;
            settingsButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            settingsButton.FlatStyle = FlatStyle.Flat;
            settingsButton.Location = new Point(14, 556);
            settingsButton.Name = "settingsButton";
            settingsButton.Padding = new Padding(10, 0, 0, 0);
            settingsButton.Size = new Size(236, 40);
            settingsButton.TabIndex = 13;
            settingsButton.Text = "Configuración y auditoría";
            settingsButton.TextAlign = ContentAlignment.MiddleLeft;
            settingsButton.UseVisualStyleBackColor = false;
            // 
            // userLabel
            // 
            userLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            userLabel.BackColor = Color.Transparent;
            userLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            userLabel.Location = new Point(24, 1474);
            userLabel.Name = "userLabel";
            userLabel.Size = new Size(225, 30);
            userLabel.TabIndex = 14;
            userLabel.Text = "Usuario Adm-01";
            userLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // roleLabel
            // 
            roleLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            roleLabel.BackColor = Color.Transparent;
            roleLabel.Font = new Font("Segoe UI", 9F);
            roleLabel.ForeColor = Color.FromArgb(125, 125, 125);
            roleLabel.Location = new Point(24, 1502);
            roleLabel.Name = "roleLabel";
            roleLabel.Size = new Size(225, 22);
            roleLabel.TabIndex = 15;
            roleLabel.Text = "Administrador";
            roleLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // logoutButton
            // 
            logoutButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            logoutButton.BackColor = Color.FromArgb(242, 242, 242);
            logoutButton.Cursor = Cursors.Hand;
            logoutButton.FlatAppearance.BorderSize = 0;
            logoutButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            logoutButton.FlatStyle = FlatStyle.Flat;
            logoutButton.Location = new Point(24, 1531);
            logoutButton.Name = "logoutButton";
            logoutButton.Padding = new Padding(10, 0, 0, 0);
            logoutButton.Size = new Size(216, 38);
            logoutButton.TabIndex = 16;
            logoutButton.Text = "Cerrar sesión";
            logoutButton.TextAlign = ContentAlignment.MiddleLeft;
            logoutButton.UseVisualStyleBackColor = false;
            // 
            // header
            // 
            header.BackColor = Color.White;
            header.Controls.Add(toggleButton);
            header.Controls.Add(searchBox);
            header.Controls.Add(accountLabel);
            header.Controls.Add(notificationButton);
            header.Dock = DockStyle.Top;
            header.Location = new Point(265, 0);
            header.Name = "header";
            header.Size = new Size(1296, 64);
            header.TabIndex = 3;
            // 
            // toggleButton
            // 
            toggleButton.BackColor = Color.White;
            toggleButton.Cursor = Cursors.Hand;
            toggleButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            toggleButton.FlatStyle = FlatStyle.Flat;
            toggleButton.Location = new Point(32, 18);
            toggleButton.Name = "toggleButton";
            toggleButton.Size = new Size(36, 30);
            toggleButton.TabIndex = 0;
            toggleButton.Text = "☰";
            toggleButton.UseVisualStyleBackColor = false;
            // 
            // searchBox
            // 
            searchBox.BorderStyle = BorderStyle.None;
            searchBox.Font = new Font("Segoe UI", 12F);
            searchBox.Location = new Point(82, 23);
            searchBox.Name = "searchBox";
            searchBox.PlaceholderText = "🔍 Buscador global...";
            searchBox.Size = new Size(420, 27);
            searchBox.TabIndex = 1;
            // 
            // accountLabel
            // 
            accountLabel.BackColor = Color.Transparent;
            accountLabel.Font = new Font("Segoe UI", 11F);
            accountLabel.Location = new Point(520, 16);
            accountLabel.Name = "accountLabel";
            accountLabel.Size = new Size(145, 34);
            accountLabel.TabIndex = 2;
            accountLabel.Text = "Administrador ⌄";
            accountLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // notificationButton
            // 
            notificationButton.BackColor = Color.White;
            notificationButton.Cursor = Cursors.Hand;
            notificationButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            notificationButton.FlatStyle = FlatStyle.Flat;
            notificationButton.Location = new Point(680, 18);
            notificationButton.Name = "notificationButton";
            notificationButton.Size = new Size(54, 30);
            notificationButton.TabIndex = 3;
            notificationButton.Text = "🔔";
            notificationButton.UseVisualStyleBackColor = false;
            // 
            // content
            // 
            content.Controls.Add(btnEliminar);
            content.Controls.Add(btnEditar);
            content.Controls.Add(dgvProductos);
            content.Controls.Add(txtBuscar);
            content.Controls.Add(btnNuevoProducto);
            content.Controls.Add(label1);
            content.Controls.Add(subtitleLabel);
            content.Controls.Add(titleLabel);
            content.Dock = DockStyle.Fill;
            content.Location = new Point(265, 64);
            content.Name = "content";
            content.Padding = new Padding(32, 34, 32, 36);
            content.Size = new Size(1296, 732);
            content.TabIndex = 4;
            // 
            // titleLabel
            // 
            titleLabel.BackColor = Color.Transparent;
            titleLabel.Font = new Font("Segoe UI", 27F, FontStyle.Bold);
            titleLabel.Location = new Point(32, 16);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(516, 72);
            titleLabel.TabIndex = 1;
            titleLabel.Text = "Catálogo de productos";
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // subtitleLabel
            // 
            subtitleLabel.BackColor = Color.Transparent;
            subtitleLabel.Font = new Font("Segoe UI", 12F);
            subtitleLabel.ForeColor = Color.FromArgb(125, 125, 125);
            subtitleLabel.Location = new Point(35, 88);
            subtitleLabel.Name = "subtitleLabel";
            subtitleLabel.Size = new Size(331, 28);
            subtitleLabel.TabIndex = 2;
            subtitleLabel.Text = "Producto, Categorías y Proveedores.";
            subtitleLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 12F);
            label1.ForeColor = Color.FromArgb(125, 125, 125);
            label1.Location = new Point(35, 130);
            label1.Name = "label1";
            label1.Size = new Size(776, 28);
            label1.TabIndex = 3;
            label1.Text = "ℹ El stock actual es solo lectura: Cambia unicamente con moviminentos de inventario.";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnNuevoProducto
            // 
            btnNuevoProducto.BackColor = SystemColors.ActiveCaptionText;
            btnNuevoProducto.ForeColor = SystemColors.Window;
            btnNuevoProducto.Location = new Point(35, 183);
            btnNuevoProducto.Name = "btnNuevoProducto";
            btnNuevoProducto.Size = new Size(153, 56);
            btnNuevoProducto.TabIndex = 4;
            btnNuevoProducto.Text = "+ Nuevo Producto";
            btnNuevoProducto.UseVisualStyleBackColor = false;
            // 
            // txtBuscar
            // 
            txtBuscar.BorderStyle = BorderStyle.None;
            txtBuscar.Font = new Font("Segoe UI", 12F);
            txtBuscar.Location = new Point(35, 255);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.PlaceholderText = "🔍 Buscar...";
            txtBuscar.Size = new Size(420, 27);
            txtBuscar.TabIndex = 5;
            // 
            // dgvProductos
            // 
            dgvProductos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProductos.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2, Column3, Column4 });
            dgvProductos.Location = new Point(35, 304);
            dgvProductos.Name = "dgvProductos";
            dgvProductos.RowHeadersWidth = 51;
            dgvProductos.Size = new Size(540, 188);
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
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            label2.Location = new Point(24, 689);
            label2.Name = "label2";
            label2.Size = new Size(225, 30);
            label2.TabIndex = 17;
            label2.Text = "Usuario Adm-01";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Segoe UI", 9F);
            label3.ForeColor = Color.FromArgb(125, 125, 125);
            label3.Location = new Point(24, 717);
            label3.Name = "label3";
            label3.Size = new Size(225, 22);
            label3.TabIndex = 18;
            label3.Text = "Administrador";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button2.BackColor = Color.FromArgb(242, 242, 242);
            button2.Cursor = Cursors.Hand;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            button2.FlatStyle = FlatStyle.Flat;
            button2.Location = new Point(24, 746);
            button2.Name = "button2";
            button2.Padding = new Padding(10, 0, 0, 0);
            button2.Size = new Size(216, 38);
            button2.TabIndex = 19;
            button2.Text = "Cerrar sesión";
            button2.TextAlign = ContentAlignment.MiddleLeft;
            button2.UseVisualStyleBackColor = false;
            // 
            // btnEditar
            // 
            btnEditar.BackColor = SystemColors.ActiveCaptionText;
            btnEditar.ForeColor = SystemColors.Window;
            btnEditar.Location = new Point(194, 183);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new Size(153, 56);
            btnEditar.TabIndex = 7;
            btnEditar.Text = "Editar";
            btnEditar.UseVisualStyleBackColor = false;
            // 
            // btnEliminar
            // 
            btnEliminar.BackColor = SystemColors.ActiveCaptionText;
            btnEliminar.ForeColor = SystemColors.Window;
            btnEliminar.Location = new Point(353, 183);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(153, 56);
            btnEliminar.TabIndex = 8;
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseVisualStyleBackColor = false;
            // 
            // Catalogo_Productos
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(1561, 796);
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

        private Panel sidebar;
        private Label brandLabel;
        private Label discoverLabel;
        private Button homeButton;
        private Button salesButton;
        private Label adminLabel;
        private Button inventoryButton;
        private Button transferButton;
        private Button validationButton;
        private Button bulkButton;
        private Button catalogButton;
        private Button returnsButton;
        private Button rolesButton;
        private Button investorsButton;
        private Button settingsButton;
        private Label userLabel;
        private Label roleLabel;
        private Button logoutButton;
        private Panel header;
        private Button toggleButton;
        private TextBox searchBox;
        private Label accountLabel;
        private Button notificationButton;
        private Panel content;
        private Label label2;
        private Label label3;
        private Button button2;
        private Button btnEliminar;
        private Button btnEditar;
        private DataGridView dgvProductos;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewTextBoxColumn Column4;
        private TextBox txtBuscar;
        private Button btnNuevoProducto;
        private Label label1;
        private Label subtitleLabel;
        private Label titleLabel;
    }
}