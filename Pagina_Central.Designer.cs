#nullable enable
namespace Sistema_ERP
{
    partial class Pagina_Central
    {
        private System.ComponentModel.IContainer? components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            main = new Panel();
            content = new Panel();
            dashboardLayout = new TableLayoutPanel();
            headingPanel = new Panel();
            titleLabel = new Label();
            subtitleLabel = new Label();
            btnprobarconexion = new Button();
            infoPanel = new Panel();
            infoLabel = new Label();
            cardsLayout = new TableLayoutPanel();
            cardPanel0 = new Panel();
            cardTitle0 = new Label();
            cardValue0 = new Label();
            cardDetail0 = new Label();
            cardPanel1 = new Panel();
            cardTitle1 = new Label();
            cardValue1 = new Label();
            cardDetail1 = new Label();
            cardPanel2 = new Panel();
            cardTitle2 = new Label();
            cardValue2 = new Label();
            cardDetail2 = new Label();
            cardPanel3 = new Panel();
            cardTitle3 = new Label();
            cardValue3 = new Label();
            cardDetail3 = new Label();
            chartPanel = new Panel();
            chartTitleLabel = new Label();
            header = new Panel();
            toggleButton = new Button();
            searchBox = new TextBox();
            accountLabel = new Label();
            notificationButton = new Button();
            logoutButton = new Button();
            roleLabel = new Label();
            userLabel = new Label();
            settingsButton = new Button();
            investorsButton = new Button();
            rolesButton = new Button();
            returnsButton = new Button();
            catalogButton = new Button();
            bulkButton = new Button();
            validationButton = new Button();
            transferButton = new Button();
            inventoryButton = new Button();
            adminLabel = new Label();
            salesButton = new Button();
            homeButton = new Button();
            discoverLabel = new Label();
            brandLabel = new Label();
            panelContenedor = new Panel();
            main.SuspendLayout();
            content.SuspendLayout();
            dashboardLayout.SuspendLayout();
            headingPanel.SuspendLayout();
            infoPanel.SuspendLayout();
            cardsLayout.SuspendLayout();
            cardPanel0.SuspendLayout();
            cardPanel1.SuspendLayout();
            cardPanel2.SuspendLayout();
            cardPanel3.SuspendLayout();
            chartPanel.SuspendLayout();
            header.SuspendLayout();
            panelContenedor.SuspendLayout();
            SuspendLayout();
            // 
            // main
            // 
            main.BackColor = Color.FromArgb(253, 253, 253);
            main.Controls.Add(content);
            main.Controls.Add(header);
            main.Dock = DockStyle.Fill;
            main.Location = new Point(265, 0);
            main.Name = "main";
            main.Size = new Size(1235, 900);
            main.TabIndex = 0;
            // 
            // content
            // 
            content.Controls.Add(dashboardLayout);
            content.Dock = DockStyle.Fill;
            content.Location = new Point(0, 64);
            content.Name = "content";
            content.Padding = new Padding(32, 34, 32, 36);
            content.Size = new Size(1235, 836);
            content.TabIndex = 0;
            // 
            // dashboardLayout
            // 
            dashboardLayout.ColumnCount = 1;
            dashboardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            dashboardLayout.Controls.Add(headingPanel, 0, 0);
            dashboardLayout.Controls.Add(infoPanel, 0, 1);
            dashboardLayout.Controls.Add(cardsLayout, 0, 3);
            dashboardLayout.Controls.Add(chartPanel, 0, 4);
            dashboardLayout.Dock = DockStyle.Fill;
            dashboardLayout.Location = new Point(32, 34);
            dashboardLayout.Margin = new Padding(0);
            dashboardLayout.Name = "dashboardLayout";
            dashboardLayout.RowCount = 5;
            dashboardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 86F));
            dashboardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            dashboardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            dashboardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 144F));
            dashboardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            dashboardLayout.Size = new Size(1171, 766);
            dashboardLayout.TabIndex = 0;
            // 
            // headingPanel
            // 
            headingPanel.Controls.Add(titleLabel);
            headingPanel.Controls.Add(subtitleLabel);
            headingPanel.Controls.Add(btnprobarconexion);
            headingPanel.Dock = DockStyle.Fill;
            headingPanel.Location = new Point(0, 0);
            headingPanel.Margin = new Padding(0);
            headingPanel.Name = "headingPanel";
            headingPanel.Size = new Size(1171, 86);
            headingPanel.TabIndex = 0;
            headingPanel.Paint += headingPanel_Paint;
            // 
            // titleLabel
            // 
            titleLabel.BackColor = Color.Transparent;
            titleLabel.Font = new Font("Segoe UI", 27F, FontStyle.Bold);
            titleLabel.Location = new Point(0, -1);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(400, 48);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "Inicio";
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // subtitleLabel
            // 
            subtitleLabel.BackColor = Color.Transparent;
            subtitleLabel.Font = new Font("Segoe UI", 12F);
            subtitleLabel.ForeColor = Color.FromArgb(125, 125, 125);
            subtitleLabel.Location = new Point(0, 48);
            subtitleLabel.Name = "subtitleLabel";
            subtitleLabel.Size = new Size(500, 28);
            subtitleLabel.TabIndex = 1;
            subtitleLabel.Text = "Resumen general del negocio";
            subtitleLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnprobarconexion
            // 
            btnprobarconexion.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnprobarconexion.Location = new Point(971, 16);
            btnprobarconexion.Name = "btnprobarconexion";
            btnprobarconexion.Size = new Size(180, 40);
            btnprobarconexion.TabIndex = 2;
            btnprobarconexion.Text = "Probar conexión";
            btnprobarconexion.UseVisualStyleBackColor = true;
            btnprobarconexion.Click += btnprobarconexion_Click;
            // 
            // infoPanel
            // 
            infoPanel.BackColor = Color.FromArgb(243, 243, 243);
            infoPanel.Controls.Add(infoLabel);
            infoPanel.Dock = DockStyle.Fill;
            infoPanel.Location = new Point(0, 86);
            infoPanel.Margin = new Padding(0);
            infoPanel.Name = "infoPanel";
            infoPanel.Size = new Size(1171, 42);
            infoPanel.TabIndex = 1;
            infoPanel.Paint += RoundedPanel_Paint;
            // 
            // infoLabel
            // 
            infoLabel.BackColor = Color.Transparent;
            infoLabel.Dock = DockStyle.Fill;
            infoLabel.Font = new Font("Segoe UI", 11F);
            infoLabel.ForeColor = Color.FromArgb(125, 125, 125);
            infoLabel.Location = new Point(0, 0);
            infoLabel.Name = "infoLabel";
            infoLabel.Padding = new Padding(16, 0, 8, 0);
            infoLabel.Size = new Size(1171, 42);
            infoLabel.TabIndex = 0;
            infoLabel.Text = "ℹ Vista rápida de ventas, traspasos, stock e inversiones. Los datos son de ejemplo.";
            infoLabel.TextAlign = ContentAlignment.MiddleLeft;
            infoLabel.Click += infoLabel_Click;
            // 
            // cardsLayout
            // 
            cardsLayout.ColumnCount = 4;
            cardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            cardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            cardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            cardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            cardsLayout.Controls.Add(cardPanel0, 0, 0);
            cardsLayout.Controls.Add(cardPanel1, 1, 0);
            cardsLayout.Controls.Add(cardPanel2, 2, 0);
            cardsLayout.Controls.Add(cardPanel3, 3, 0);
            cardsLayout.Dock = DockStyle.Fill;
            cardsLayout.Location = new Point(0, 148);
            cardsLayout.Margin = new Padding(0);
            cardsLayout.Name = "cardsLayout";
            cardsLayout.RowCount = 1;
            cardsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            cardsLayout.Size = new Size(1171, 144);
            cardsLayout.TabIndex = 2;
            // 
            // cardPanel0
            // 
            cardPanel0.BackColor = Color.White;
            cardPanel0.Controls.Add(cardTitle0);
            cardPanel0.Controls.Add(cardValue0);
            cardPanel0.Controls.Add(cardDetail0);
            cardPanel0.Dock = DockStyle.Fill;
            cardPanel0.Location = new Point(0, 0);
            cardPanel0.Margin = new Padding(0, 0, 10, 20);
            cardPanel0.Name = "cardPanel0";
            cardPanel0.Size = new Size(282, 124);
            cardPanel0.TabIndex = 0;
            cardPanel0.Paint += RoundedPanel_Paint;
            // 
            // cardTitle0
            // 
            cardTitle0.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cardTitle0.BackColor = Color.Transparent;
            cardTitle0.Font = new Font("Segoe UI", 10F);
            cardTitle0.ForeColor = Color.FromArgb(125, 125, 125);
            cardTitle0.Location = new Point(24, 18);
            cardTitle0.Name = "cardTitle0";
            cardTitle0.Size = new Size(234, 26);
            cardTitle0.TabIndex = 0;
            cardTitle0.Text = "Ventas del día";
            cardTitle0.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cardValue0
            // 
            cardValue0.BackColor = Color.Transparent;
            cardValue0.Font = new Font("Segoe UI", 25F, FontStyle.Bold);
            cardValue0.Location = new Point(24, 44);
            cardValue0.Name = "cardValue0";
            cardValue0.Size = new Size(220, 38);
            cardValue0.TabIndex = 1;
            cardValue0.Text = "L. 0.00";
            cardValue0.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cardDetail0
            // 
            cardDetail0.BackColor = Color.Transparent;
            cardDetail0.Font = new Font("Segoe UI", 10F);
            cardDetail0.ForeColor = Color.FromArgb(125, 125, 125);
            cardDetail0.Location = new Point(24, 82);
            cardDetail0.Name = "cardDetail0";
            cardDetail0.Size = new Size(220, 24);
            cardDetail0.TabIndex = 2;
            cardDetail0.Text = "—";
            cardDetail0.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cardPanel1
            // 
            cardPanel1.BackColor = Color.White;
            cardPanel1.Controls.Add(cardTitle1);
            cardPanel1.Controls.Add(cardValue1);
            cardPanel1.Controls.Add(cardDetail1);
            cardPanel1.Dock = DockStyle.Fill;
            cardPanel1.Location = new Point(302, 0);
            cardPanel1.Margin = new Padding(10, 0, 10, 20);
            cardPanel1.Name = "cardPanel1";
            cardPanel1.Size = new Size(272, 124);
            cardPanel1.TabIndex = 1;
            cardPanel1.Paint += RoundedPanel_Paint;
            // 
            // cardTitle1
            // 
            cardTitle1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cardTitle1.BackColor = Color.Transparent;
            cardTitle1.Font = new Font("Segoe UI", 10F);
            cardTitle1.ForeColor = Color.FromArgb(125, 125, 125);
            cardTitle1.Location = new Point(24, 18);
            cardTitle1.Name = "cardTitle1";
            cardTitle1.Size = new Size(224, 26);
            cardTitle1.TabIndex = 0;
            cardTitle1.Text = "Traspasos pendientes";
            cardTitle1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cardValue1
            // 
            cardValue1.BackColor = Color.Transparent;
            cardValue1.Font = new Font("Segoe UI", 25F, FontStyle.Bold);
            cardValue1.Location = new Point(24, 44);
            cardValue1.Name = "cardValue1";
            cardValue1.Size = new Size(220, 38);
            cardValue1.TabIndex = 1;
            cardValue1.Text = "0";
            cardValue1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cardDetail1
            // 
            cardDetail1.BackColor = Color.Transparent;
            cardDetail1.Font = new Font("Segoe UI", 10F);
            cardDetail1.ForeColor = Color.FromArgb(125, 125, 125);
            cardDetail1.Location = new Point(24, 82);
            cardDetail1.Name = "cardDetail1";
            cardDetail1.Size = new Size(220, 24);
            cardDetail1.TabIndex = 2;
            cardDetail1.Text = "Esperan aprobación";
            cardDetail1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cardPanel2
            // 
            cardPanel2.BackColor = Color.White;
            cardPanel2.Controls.Add(cardTitle2);
            cardPanel2.Controls.Add(cardValue2);
            cardPanel2.Controls.Add(cardDetail2);
            cardPanel2.Dock = DockStyle.Fill;
            cardPanel2.Location = new Point(594, 0);
            cardPanel2.Margin = new Padding(10, 0, 10, 20);
            cardPanel2.Name = "cardPanel2";
            cardPanel2.Size = new Size(272, 124);
            cardPanel2.TabIndex = 2;
            cardPanel2.Paint += RoundedPanel_Paint;
            // 
            // cardTitle2
            // 
            cardTitle2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cardTitle2.BackColor = Color.Transparent;
            cardTitle2.Font = new Font("Segoe UI", 10F);
            cardTitle2.ForeColor = Color.FromArgb(125, 125, 125);
            cardTitle2.Location = new Point(24, 18);
            cardTitle2.Name = "cardTitle2";
            cardTitle2.Size = new Size(224, 26);
            cardTitle2.TabIndex = 0;
            cardTitle2.Text = "Alertas de stock";
            cardTitle2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cardValue2
            // 
            cardValue2.BackColor = Color.Transparent;
            cardValue2.Font = new Font("Segoe UI", 25F, FontStyle.Bold);
            cardValue2.Location = new Point(24, 44);
            cardValue2.Name = "cardValue2";
            cardValue2.Size = new Size(220, 38);
            cardValue2.TabIndex = 1;
            cardValue2.Text = "0";
            cardValue2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cardDetail2
            // 
            cardDetail2.BackColor = Color.Transparent;
            cardDetail2.Font = new Font("Segoe UI", 10F);
            cardDetail2.ForeColor = Color.FromArgb(125, 125, 125);
            cardDetail2.Location = new Point(24, 82);
            cardDetail2.Name = "cardDetail2";
            cardDetail2.Size = new Size(220, 24);
            cardDetail2.TabIndex = 2;
            cardDetail2.Text = "Bajo mínimo";
            cardDetail2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cardPanel3
            // 
            cardPanel3.BackColor = Color.White;
            cardPanel3.Controls.Add(cardTitle3);
            cardPanel3.Controls.Add(cardValue3);
            cardPanel3.Controls.Add(cardDetail3);
            cardPanel3.Dock = DockStyle.Fill;
            cardPanel3.Location = new Point(886, 0);
            cardPanel3.Margin = new Padding(10, 0, 0, 20);
            cardPanel3.Name = "cardPanel3";
            cardPanel3.Size = new Size(285, 124);
            cardPanel3.TabIndex = 3;
            cardPanel3.Paint += RoundedPanel_Paint;
            // 
            // cardTitle3
            // 
            cardTitle3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cardTitle3.BackColor = Color.Transparent;
            cardTitle3.Font = new Font("Segoe UI", 10F);
            cardTitle3.ForeColor = Color.FromArgb(125, 125, 125);
            cardTitle3.Location = new Point(24, 18);
            cardTitle3.Name = "cardTitle3";
            cardTitle3.Size = new Size(237, 26);
            cardTitle3.TabIndex = 0;
            cardTitle3.Text = "Inversiones por vencer";
            cardTitle3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cardValue3
            // 
            cardValue3.BackColor = Color.Transparent;
            cardValue3.Font = new Font("Segoe UI", 25F, FontStyle.Bold);
            cardValue3.Location = new Point(24, 44);
            cardValue3.Name = "cardValue3";
            cardValue3.Size = new Size(220, 38);
            cardValue3.TabIndex = 1;
            cardValue3.Text = "0";
            cardValue3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cardDetail3
            // 
            cardDetail3.BackColor = Color.Transparent;
            cardDetail3.Font = new Font("Segoe UI", 10F);
            cardDetail3.ForeColor = Color.FromArgb(125, 125, 125);
            cardDetail3.Location = new Point(24, 82);
            cardDetail3.Name = "cardDetail3";
            cardDetail3.Size = new Size(220, 24);
            cardDetail3.TabIndex = 2;
            cardDetail3.Text = "En 30 días";
            cardDetail3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // chartPanel
            // 
            chartPanel.BackColor = Color.White;
            chartPanel.Controls.Add(chartTitleLabel);
            chartPanel.Dock = DockStyle.Fill;
            chartPanel.Location = new Point(0, 292);
            chartPanel.Margin = new Padding(0);
            chartPanel.Name = "chartPanel";
            chartPanel.Size = new Size(1171, 474);
            chartPanel.TabIndex = 3;
            chartPanel.Paint += ChartPanel_Paint;
            // 
            // chartTitleLabel
            // 
            chartTitleLabel.BackColor = Color.Transparent;
            chartTitleLabel.Font = new Font("Segoe UI", 12F);
            chartTitleLabel.Location = new Point(26, 14);
            chartTitleLabel.Name = "chartTitleLabel";
            chartTitleLabel.Size = new Size(300, 32);
            chartTitleLabel.TabIndex = 0;
            chartTitleLabel.Text = "Ventas de la semana";
            chartTitleLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // header
            // 
            header.BackColor = Color.White;
            header.Controls.Add(toggleButton);
            header.Controls.Add(searchBox);
            header.Controls.Add(accountLabel);
            header.Controls.Add(notificationButton);
            header.Dock = DockStyle.Top;
            header.Location = new Point(0, 0);
            header.Name = "header";
            header.Size = new Size(1235, 64);
            header.TabIndex = 1;
            header.Paint += Header_Paint;
            header.Resize += Header_Resize;
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
            toggleButton.Click += ToggleButton_Click;
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
            // logoutButton
            // 
            logoutButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            logoutButton.BackColor = Color.FromArgb(242, 242, 242);
            logoutButton.Cursor = Cursors.Hand;
            logoutButton.FlatAppearance.BorderSize = 0;
            logoutButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            logoutButton.FlatStyle = FlatStyle.Flat;
            logoutButton.Location = new Point(24, 835);
            logoutButton.Name = "logoutButton";
            logoutButton.Padding = new Padding(10, 0, 0, 0);
            logoutButton.Size = new Size(216, 38);
            logoutButton.TabIndex = 16;
            logoutButton.Text = "Cerrar sesión";
            logoutButton.TextAlign = ContentAlignment.MiddleLeft;
            logoutButton.UseVisualStyleBackColor = false;
            // 
            // roleLabel
            // 
            roleLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            roleLabel.BackColor = Color.Transparent;
            roleLabel.Font = new Font("Segoe UI", 9F);
            roleLabel.ForeColor = Color.FromArgb(125, 125, 125);
            roleLabel.Location = new Point(24, 806);
            roleLabel.Name = "roleLabel";
            roleLabel.Size = new Size(225, 22);
            roleLabel.TabIndex = 15;
            roleLabel.Text = "Administrador";
            roleLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // userLabel
            // 
            userLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            userLabel.BackColor = Color.Transparent;
            userLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            userLabel.Location = new Point(24, 778);
            userLabel.Name = "userLabel";
            userLabel.Size = new Size(225, 30);
            userLabel.TabIndex = 14;
            userLabel.Text = "Usuario Adm-01";
            userLabel.TextAlign = ContentAlignment.MiddleLeft;
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
            catalogButton.Size = new Size(226, 40);
            catalogButton.TabIndex = 9;
            catalogButton.Text = "Catálogo de productos";
            catalogButton.TextAlign = ContentAlignment.MiddleLeft;
            catalogButton.UseVisualStyleBackColor = false;
            catalogButton.Click += catalogButton_Click;
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
            // panelContenedor
            // 
            panelContenedor.BackColor = Color.White;
            panelContenedor.Controls.Add(brandLabel);
            panelContenedor.Controls.Add(discoverLabel);
            panelContenedor.Controls.Add(homeButton);
            panelContenedor.Controls.Add(salesButton);
            panelContenedor.Controls.Add(adminLabel);
            panelContenedor.Controls.Add(inventoryButton);
            panelContenedor.Controls.Add(transferButton);
            panelContenedor.Controls.Add(validationButton);
            panelContenedor.Controls.Add(bulkButton);
            panelContenedor.Controls.Add(catalogButton);
            panelContenedor.Controls.Add(returnsButton);
            panelContenedor.Controls.Add(rolesButton);
            panelContenedor.Controls.Add(investorsButton);
            panelContenedor.Controls.Add(settingsButton);
            panelContenedor.Controls.Add(userLabel);
            panelContenedor.Controls.Add(roleLabel);
            panelContenedor.Controls.Add(logoutButton);
            panelContenedor.Dock = DockStyle.Left;
            panelContenedor.Location = new Point(0, 0);
            panelContenedor.Name = "panelContenedor";
            panelContenedor.Size = new Size(265, 900);
            panelContenedor.TabIndex = 1;
            panelContenedor.Paint += Sidebar_Paint;
            // 
            // Pagina_Central
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1500, 900);
            Controls.Add(main);
            Controls.Add(panelContenedor);
            Font = new Font("Segoe UI", 11F);
            MinimumSize = new Size(1050, 720);
            Name = "Pagina_Central";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sistema ERP — Inicio";
            main.ResumeLayout(false);
            content.ResumeLayout(false);
            dashboardLayout.ResumeLayout(false);
            headingPanel.ResumeLayout(false);
            infoPanel.ResumeLayout(false);
            cardsLayout.ResumeLayout(false);
            cardPanel0.ResumeLayout(false);
            cardPanel1.ResumeLayout(false);
            cardPanel2.ResumeLayout(false);
            cardPanel3.ResumeLayout(false);
            chartPanel.ResumeLayout(false);
            header.ResumeLayout(false);
            header.PerformLayout();
            panelContenedor.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel main = null!;
        private System.Windows.Forms.Panel content = null!;
        private System.Windows.Forms.Panel header = null!;
        private System.Windows.Forms.Button toggleButton = null!;
        private System.Windows.Forms.TextBox searchBox = null!;
        private System.Windows.Forms.Label accountLabel = null!;
        private System.Windows.Forms.Button notificationButton = null!;
        private System.Windows.Forms.TableLayoutPanel dashboardLayout = null!;
        private System.Windows.Forms.Panel headingPanel = null!;
        private System.Windows.Forms.Label titleLabel = null!;
        private System.Windows.Forms.Label subtitleLabel = null!;
        private System.Windows.Forms.Button btnprobarconexion = null!;
        private System.Windows.Forms.Panel infoPanel = null!;
        private System.Windows.Forms.Label infoLabel = null!;
        private System.Windows.Forms.TableLayoutPanel cardsLayout = null!;
        private System.Windows.Forms.Panel cardPanel0 = null!;
        private System.Windows.Forms.Label cardTitle0 = null!;
        private System.Windows.Forms.Label cardValue0 = null!;
        private System.Windows.Forms.Label cardDetail0 = null!;
        private System.Windows.Forms.Panel cardPanel1 = null!;
        private System.Windows.Forms.Label cardTitle1 = null!;
        private System.Windows.Forms.Label cardValue1 = null!;
        private System.Windows.Forms.Label cardDetail1 = null!;
        private System.Windows.Forms.Panel cardPanel2 = null!;
        private System.Windows.Forms.Label cardTitle2 = null!;
        private System.Windows.Forms.Label cardValue2 = null!;
        private System.Windows.Forms.Label cardDetail2 = null!;
        private System.Windows.Forms.Panel cardPanel3 = null!;
        private System.Windows.Forms.Label cardTitle3 = null!;
        private System.Windows.Forms.Label cardValue3 = null!;
        private System.Windows.Forms.Label cardDetail3 = null!;
        private System.Windows.Forms.Panel chartPanel = null!;
        private System.Windows.Forms.Label chartTitleLabel = null!;
        private Button logoutButton;
        private Label roleLabel;
        private Label userLabel;
        private Button settingsButton;
        private Button investorsButton;
        private Button rolesButton;
        private Button returnsButton;
        private Button catalogButton;
        private Button bulkButton;
        private Button validationButton;
        private Button transferButton;
        private Button inventoryButton;
        private Label adminLabel;
        private Button salesButton;
        private Button homeButton;
        private Label discoverLabel;
        private Label brandLabel;
        private Panel panelContenedor;
    }
}
