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
            main = new System.Windows.Forms.Panel();
            content = new System.Windows.Forms.Panel();
            dashboardLayout = new System.Windows.Forms.TableLayoutPanel();
            headingPanel = new System.Windows.Forms.Panel();
            titleLabel = new System.Windows.Forms.Label();
            subtitleLabel = new System.Windows.Forms.Label();
            btnprobarconexion = new System.Windows.Forms.Button();
            infoPanel = new System.Windows.Forms.Panel();
            infoLabel = new System.Windows.Forms.Label();
            cardsLayout = new System.Windows.Forms.TableLayoutPanel();
            cardPanel0 = new System.Windows.Forms.Panel();
            cardTitle0 = new System.Windows.Forms.Label();
            cardValue0 = new System.Windows.Forms.Label();
            cardDetail0 = new System.Windows.Forms.Label();
            cardPanel1 = new System.Windows.Forms.Panel();
            cardTitle1 = new System.Windows.Forms.Label();
            cardValue1 = new System.Windows.Forms.Label();
            cardDetail1 = new System.Windows.Forms.Label();
            cardPanel2 = new System.Windows.Forms.Panel();
            cardTitle2 = new System.Windows.Forms.Label();
            cardValue2 = new System.Windows.Forms.Label();
            cardDetail2 = new System.Windows.Forms.Label();
            cardPanel3 = new System.Windows.Forms.Panel();
            cardTitle3 = new System.Windows.Forms.Label();
            cardValue3 = new System.Windows.Forms.Label();
            cardDetail3 = new System.Windows.Forms.Label();
            chartPanel = new System.Windows.Forms.Panel();
            chartTitleLabel = new System.Windows.Forms.Label();
            header = new System.Windows.Forms.Panel();
            toggleButton = new System.Windows.Forms.Button();
            searchBox = new System.Windows.Forms.TextBox();
            accountLabel = new System.Windows.Forms.Label();
            notificationButton = new System.Windows.Forms.Button();
            logoutButton = new System.Windows.Forms.Button();
            roleLabel = new System.Windows.Forms.Label();
            userLabel = new System.Windows.Forms.Label();
            settingsButton = new System.Windows.Forms.Button();
            investorsButton = new System.Windows.Forms.Button();
            rolesButton = new System.Windows.Forms.Button();
            returnsButton = new System.Windows.Forms.Button();
            catalogButton = new System.Windows.Forms.Button();
            bulkButton = new System.Windows.Forms.Button();
            validationButton = new System.Windows.Forms.Button();
            transferButton = new System.Windows.Forms.Button();
            inventoryButton = new System.Windows.Forms.Button();
            adminLabel = new System.Windows.Forms.Label();
            salesButton = new System.Windows.Forms.Button();
            homeButton = new System.Windows.Forms.Button();
            discoverLabel = new System.Windows.Forms.Label();
            brandLabel = new System.Windows.Forms.Label();
            panelContenedor = new System.Windows.Forms.Panel();
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
            main.BackColor = System.Drawing.Color.FromArgb(253, 253, 253);
            main.Controls.Add(content);
            main.Controls.Add(header);
            main.Dock = System.Windows.Forms.DockStyle.Fill;
            main.Location = new System.Drawing.Point(265, 0);
            main.Name = "main";
            main.Size = new System.Drawing.Size(1235, 900);
            main.TabIndex = 0;
            // 
            // content
            // 
            content.Controls.Add(dashboardLayout);
            content.Dock = System.Windows.Forms.DockStyle.Fill;
            content.Location = new System.Drawing.Point(0, 64);
            content.Name = "content";
            content.Padding = new System.Windows.Forms.Padding(32, 34, 32, 36);
            content.Size = new System.Drawing.Size(1235, 836);
            content.TabIndex = 0;
            // 
            // dashboardLayout
            // 
            dashboardLayout.ColumnCount = 1;
            dashboardLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            dashboardLayout.Controls.Add(headingPanel, 0, 0);
            dashboardLayout.Controls.Add(infoPanel, 0, 1);
            dashboardLayout.Controls.Add(cardsLayout, 0, 3);
            dashboardLayout.Controls.Add(chartPanel, 0, 4);
            dashboardLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            dashboardLayout.Location = new System.Drawing.Point(32, 34);
            dashboardLayout.Margin = new System.Windows.Forms.Padding(0);
            dashboardLayout.Name = "dashboardLayout";
            dashboardLayout.RowCount = 5;
            dashboardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 86F));
            dashboardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            dashboardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            dashboardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 144F));
            dashboardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            dashboardLayout.Size = new System.Drawing.Size(1171, 766);
            dashboardLayout.TabIndex = 0;
            // 
            // headingPanel
            // 
            headingPanel.Controls.Add(titleLabel);
            headingPanel.Controls.Add(subtitleLabel);
            headingPanel.Controls.Add(btnprobarconexion);
            headingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            headingPanel.Location = new System.Drawing.Point(0, 0);
            headingPanel.Margin = new System.Windows.Forms.Padding(0);
            headingPanel.Name = "headingPanel";
            headingPanel.Size = new System.Drawing.Size(1171, 86);
            headingPanel.TabIndex = 0;
            headingPanel.Paint += headingPanel_Paint;
            // 
            // titleLabel
            // 
            titleLabel.BackColor = System.Drawing.Color.Transparent;
            titleLabel.Font = new System.Drawing.Font("Segoe UI", 27F, System.Drawing.FontStyle.Bold);
            titleLabel.Location = new System.Drawing.Point(0, -1);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new System.Drawing.Size(400, 48);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "Inicio";
            titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // subtitleLabel
            // 
            subtitleLabel.BackColor = System.Drawing.Color.Transparent;
            subtitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            subtitleLabel.Location = new System.Drawing.Point(0, 48);
            subtitleLabel.Name = "subtitleLabel";
            subtitleLabel.Size = new System.Drawing.Size(500, 28);
            subtitleLabel.TabIndex = 1;
            subtitleLabel.Text = "Resumen general del negocio";
            subtitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnprobarconexion
            // 
            btnprobarconexion.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnprobarconexion.Location = new System.Drawing.Point(971, 16);
            btnprobarconexion.Name = "btnprobarconexion";
            btnprobarconexion.Size = new System.Drawing.Size(180, 40);
            btnprobarconexion.TabIndex = 2;
            btnprobarconexion.Text = "Probar conexion";
            btnprobarconexion.UseVisualStyleBackColor = true;
            btnprobarconexion.Click += btnprobarconexion_Click;
            // 
            // infoPanel
            // 
            infoPanel.BackColor = System.Drawing.Color.FromArgb(243, 243, 243);
            infoPanel.Controls.Add(infoLabel);
            infoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            infoPanel.Location = new System.Drawing.Point(0, 86);
            infoPanel.Margin = new System.Windows.Forms.Padding(0);
            infoPanel.Name = "infoPanel";
            infoPanel.Size = new System.Drawing.Size(1171, 42);
            infoPanel.TabIndex = 1;
            infoPanel.Paint += RoundedPanel_Paint;
            // 
            // infoLabel
            // 
            infoLabel.BackColor = System.Drawing.Color.Transparent;
            infoLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            infoLabel.Font = new System.Drawing.Font("Segoe UI", 11F);
            infoLabel.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            infoLabel.Location = new System.Drawing.Point(0, 0);
            infoLabel.Name = "infoLabel";
            infoLabel.Padding = new System.Windows.Forms.Padding(16, 0, 8, 0);
            infoLabel.Size = new System.Drawing.Size(1171, 42);
            infoLabel.TabIndex = 0;
            infoLabel.Text = "ℹ Vista rapida de ventas, traspasos, stock e inversiones. Los datos son de ejemplo.";
            infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            infoLabel.Click += infoLabel_Click;
            // 
            // cardsLayout
            // 
            cardsLayout.ColumnCount = 4;
            cardsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            cardsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            cardsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            cardsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            cardsLayout.Controls.Add(cardPanel0, 0, 0);
            cardsLayout.Controls.Add(cardPanel1, 1, 0);
            cardsLayout.Controls.Add(cardPanel2, 2, 0);
            cardsLayout.Controls.Add(cardPanel3, 3, 0);
            cardsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            cardsLayout.Location = new System.Drawing.Point(0, 148);
            cardsLayout.Margin = new System.Windows.Forms.Padding(0);
            cardsLayout.Name = "cardsLayout";
            cardsLayout.RowCount = 1;
            cardsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            cardsLayout.Size = new System.Drawing.Size(1171, 144);
            cardsLayout.TabIndex = 2;
            // 
            // cardPanel0
            // 
            cardPanel0.BackColor = System.Drawing.Color.White;
            cardPanel0.Controls.Add(cardTitle0);
            cardPanel0.Controls.Add(cardValue0);
            cardPanel0.Controls.Add(cardDetail0);
            cardPanel0.Dock = System.Windows.Forms.DockStyle.Fill;
            cardPanel0.Location = new System.Drawing.Point(0, 0);
            cardPanel0.Margin = new System.Windows.Forms.Padding(0, 0, 10, 20);
            cardPanel0.Name = "cardPanel0";
            cardPanel0.Size = new System.Drawing.Size(282, 124);
            cardPanel0.TabIndex = 0;
            cardPanel0.Paint += RoundedPanel_Paint;
            // 
            // cardTitle0
            // 
            cardTitle0.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            cardTitle0.BackColor = System.Drawing.Color.Transparent;
            cardTitle0.Font = new System.Drawing.Font("Segoe UI", 10F);
            cardTitle0.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            cardTitle0.Location = new System.Drawing.Point(24, 18);
            cardTitle0.Name = "cardTitle0";
            cardTitle0.Size = new System.Drawing.Size(234, 26);
            cardTitle0.TabIndex = 0;
            cardTitle0.Text = "Ventas del dia";
            cardTitle0.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cardValue0
            // 
            cardValue0.BackColor = System.Drawing.Color.Transparent;
            cardValue0.Font = new System.Drawing.Font("Segoe UI", 25F, System.Drawing.FontStyle.Bold);
            cardValue0.Location = new System.Drawing.Point(24, 44);
            cardValue0.Name = "cardValue0";
            cardValue0.Size = new System.Drawing.Size(220, 38);
            cardValue0.TabIndex = 1;
            cardValue0.Text = "L. 0.00";
            cardValue0.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cardDetail0
            // 
            cardDetail0.BackColor = System.Drawing.Color.Transparent;
            cardDetail0.Font = new System.Drawing.Font("Segoe UI", 10F);
            cardDetail0.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            cardDetail0.Location = new System.Drawing.Point(24, 82);
            cardDetail0.Name = "cardDetail0";
            cardDetail0.Size = new System.Drawing.Size(220, 24);
            cardDetail0.TabIndex = 2;
            cardDetail0.Text = "—";
            cardDetail0.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cardPanel1
            // 
            cardPanel1.BackColor = System.Drawing.Color.White;
            cardPanel1.Controls.Add(cardTitle1);
            cardPanel1.Controls.Add(cardValue1);
            cardPanel1.Controls.Add(cardDetail1);
            cardPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            cardPanel1.Location = new System.Drawing.Point(302, 0);
            cardPanel1.Margin = new System.Windows.Forms.Padding(10, 0, 10, 20);
            cardPanel1.Name = "cardPanel1";
            cardPanel1.Size = new System.Drawing.Size(272, 124);
            cardPanel1.TabIndex = 1;
            cardPanel1.Paint += RoundedPanel_Paint;
            // 
            // cardTitle1
            // 
            cardTitle1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            cardTitle1.BackColor = System.Drawing.Color.Transparent;
            cardTitle1.Font = new System.Drawing.Font("Segoe UI", 10F);
            cardTitle1.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            cardTitle1.Location = new System.Drawing.Point(24, 18);
            cardTitle1.Name = "cardTitle1";
            cardTitle1.Size = new System.Drawing.Size(224, 26);
            cardTitle1.TabIndex = 0;
            cardTitle1.Text = "Traspasos pendientes";
            cardTitle1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cardValue1
            // 
            cardValue1.BackColor = System.Drawing.Color.Transparent;
            cardValue1.Font = new System.Drawing.Font("Segoe UI", 25F, System.Drawing.FontStyle.Bold);
            cardValue1.Location = new System.Drawing.Point(24, 44);
            cardValue1.Name = "cardValue1";
            cardValue1.Size = new System.Drawing.Size(220, 38);
            cardValue1.TabIndex = 1;
            cardValue1.Text = "0";
            cardValue1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cardDetail1
            // 
            cardDetail1.BackColor = System.Drawing.Color.Transparent;
            cardDetail1.Font = new System.Drawing.Font("Segoe UI", 10F);
            cardDetail1.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            cardDetail1.Location = new System.Drawing.Point(24, 82);
            cardDetail1.Name = "cardDetail1";
            cardDetail1.Size = new System.Drawing.Size(220, 24);
            cardDetail1.TabIndex = 2;
            cardDetail1.Text = "Esperan aprobacion";
            cardDetail1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cardPanel2
            // 
            cardPanel2.BackColor = System.Drawing.Color.White;
            cardPanel2.Controls.Add(cardTitle2);
            cardPanel2.Controls.Add(cardValue2);
            cardPanel2.Controls.Add(cardDetail2);
            cardPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            cardPanel2.Location = new System.Drawing.Point(594, 0);
            cardPanel2.Margin = new System.Windows.Forms.Padding(10, 0, 10, 20);
            cardPanel2.Name = "cardPanel2";
            cardPanel2.Size = new System.Drawing.Size(272, 124);
            cardPanel2.TabIndex = 2;
            cardPanel2.Paint += RoundedPanel_Paint;
            // 
            // cardTitle2
            // 
            cardTitle2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            cardTitle2.BackColor = System.Drawing.Color.Transparent;
            cardTitle2.Font = new System.Drawing.Font("Segoe UI", 10F);
            cardTitle2.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            cardTitle2.Location = new System.Drawing.Point(24, 18);
            cardTitle2.Name = "cardTitle2";
            cardTitle2.Size = new System.Drawing.Size(224, 26);
            cardTitle2.TabIndex = 0;
            cardTitle2.Text = "Alertas de stock";
            cardTitle2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cardValue2
            // 
            cardValue2.BackColor = System.Drawing.Color.Transparent;
            cardValue2.Font = new System.Drawing.Font("Segoe UI", 25F, System.Drawing.FontStyle.Bold);
            cardValue2.Location = new System.Drawing.Point(24, 44);
            cardValue2.Name = "cardValue2";
            cardValue2.Size = new System.Drawing.Size(220, 38);
            cardValue2.TabIndex = 1;
            cardValue2.Text = "0";
            cardValue2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cardDetail2
            // 
            cardDetail2.BackColor = System.Drawing.Color.Transparent;
            cardDetail2.Font = new System.Drawing.Font("Segoe UI", 10F);
            cardDetail2.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            cardDetail2.Location = new System.Drawing.Point(24, 82);
            cardDetail2.Name = "cardDetail2";
            cardDetail2.Size = new System.Drawing.Size(220, 24);
            cardDetail2.TabIndex = 2;
            cardDetail2.Text = "Bajo minimo";
            cardDetail2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cardPanel3
            // 
            cardPanel3.BackColor = System.Drawing.Color.White;
            cardPanel3.Controls.Add(cardTitle3);
            cardPanel3.Controls.Add(cardValue3);
            cardPanel3.Controls.Add(cardDetail3);
            cardPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            cardPanel3.Location = new System.Drawing.Point(886, 0);
            cardPanel3.Margin = new System.Windows.Forms.Padding(10, 0, 0, 20);
            cardPanel3.Name = "cardPanel3";
            cardPanel3.Size = new System.Drawing.Size(285, 124);
            cardPanel3.TabIndex = 3;
            cardPanel3.Paint += RoundedPanel_Paint;
            // 
            // cardTitle3
            // 
            cardTitle3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            cardTitle3.BackColor = System.Drawing.Color.Transparent;
            cardTitle3.Font = new System.Drawing.Font("Segoe UI", 10F);
            cardTitle3.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            cardTitle3.Location = new System.Drawing.Point(24, 18);
            cardTitle3.Name = "cardTitle3";
            cardTitle3.Size = new System.Drawing.Size(237, 26);
            cardTitle3.TabIndex = 0;
            cardTitle3.Text = "Inversiones por vencer";
            cardTitle3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cardValue3
            // 
            cardValue3.BackColor = System.Drawing.Color.Transparent;
            cardValue3.Font = new System.Drawing.Font("Segoe UI", 25F, System.Drawing.FontStyle.Bold);
            cardValue3.Location = new System.Drawing.Point(24, 44);
            cardValue3.Name = "cardValue3";
            cardValue3.Size = new System.Drawing.Size(220, 38);
            cardValue3.TabIndex = 1;
            cardValue3.Text = "0";
            cardValue3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cardDetail3
            // 
            cardDetail3.BackColor = System.Drawing.Color.Transparent;
            cardDetail3.Font = new System.Drawing.Font("Segoe UI", 10F);
            cardDetail3.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            cardDetail3.Location = new System.Drawing.Point(24, 82);
            cardDetail3.Name = "cardDetail3";
            cardDetail3.Size = new System.Drawing.Size(220, 24);
            cardDetail3.TabIndex = 2;
            cardDetail3.Text = "En 30 dias";
            cardDetail3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chartPanel
            // 
            chartPanel.BackColor = System.Drawing.Color.White;
            chartPanel.Controls.Add(chartTitleLabel);
            chartPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            chartPanel.Location = new System.Drawing.Point(0, 292);
            chartPanel.Margin = new System.Windows.Forms.Padding(0);
            chartPanel.Name = "chartPanel";
            chartPanel.Size = new System.Drawing.Size(1171, 474);
            chartPanel.TabIndex = 3;
            chartPanel.Paint += ChartPanel_Paint;
            // 
            // chartTitleLabel
            // 
            chartTitleLabel.BackColor = System.Drawing.Color.Transparent;
            chartTitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            chartTitleLabel.Location = new System.Drawing.Point(26, 14);
            chartTitleLabel.Name = "chartTitleLabel";
            chartTitleLabel.Size = new System.Drawing.Size(300, 32);
            chartTitleLabel.TabIndex = 0;
            chartTitleLabel.Text = "Ventas de la semana";
            chartTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // header
            // 
            header.BackColor = System.Drawing.Color.White;
            header.Controls.Add(toggleButton);
            header.Controls.Add(searchBox);
            header.Controls.Add(accountLabel);
            header.Controls.Add(notificationButton);
            header.Dock = System.Windows.Forms.DockStyle.Top;
            header.Location = new System.Drawing.Point(0, 0);
            header.Name = "header";
            header.Size = new System.Drawing.Size(1235, 64);
            header.TabIndex = 1;
            header.Paint += Header_Paint;
            header.Resize += Header_Resize;
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
            toggleButton.Click += ToggleButton_Click;
            // 
            // searchBox
            // 
            searchBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            searchBox.Font = new System.Drawing.Font("Segoe UI", 12F);
            searchBox.Location = new System.Drawing.Point(82, 23);
            searchBox.Name = "searchBox";
            searchBox.PlaceholderText = "🔍 Buscador global...";
            searchBox.Size = new System.Drawing.Size(420, 22);
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
            notificationButton.Click += notificationButton_Click;
            // 
            // logoutButton
            // 
            logoutButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            logoutButton.BackColor = System.Drawing.Color.FromArgb(242, 242, 242);
            logoutButton.Cursor = System.Windows.Forms.Cursors.Hand;
            logoutButton.FlatAppearance.BorderSize = 0;
            logoutButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            logoutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            logoutButton.Location = new System.Drawing.Point(24, 835);
            logoutButton.Name = "logoutButton";
            logoutButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            logoutButton.Size = new System.Drawing.Size(216, 38);
            logoutButton.TabIndex = 16;
            logoutButton.Text = "Cerrar sesion";
            logoutButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            logoutButton.UseVisualStyleBackColor = false;
            logoutButton.Click += logoutButton_Click;
            // 
            // roleLabel
            // 
            roleLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            roleLabel.BackColor = System.Drawing.Color.Transparent;
            roleLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            roleLabel.ForeColor = System.Drawing.Color.FromArgb(125, 125, 125);
            roleLabel.Location = new System.Drawing.Point(24, 806);
            roleLabel.Name = "roleLabel";
            roleLabel.Size = new System.Drawing.Size(225, 22);
            roleLabel.TabIndex = 15;
            roleLabel.Text = "Administrador";
            roleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // userLabel
            // 
            userLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            userLabel.BackColor = System.Drawing.Color.Transparent;
            userLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            userLabel.Location = new System.Drawing.Point(24, 778);
            userLabel.Name = "userLabel";
            userLabel.Size = new System.Drawing.Size(225, 30);
            userLabel.TabIndex = 14;
            userLabel.Text = "Usuario Adm-01";
            userLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            settingsButton.Click += settingsButton_Click;
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
            investorsButton.Click += investorsButton_Click;
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
            rolesButton.Click += rolesButton_Click;
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
            returnsButton.Click += returnsButton_Click;
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
            catalogButton.Size = new System.Drawing.Size(226, 40);
            catalogButton.TabIndex = 9;
            catalogButton.Text = "Catalogo de productos";
            catalogButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            catalogButton.UseVisualStyleBackColor = false;
            catalogButton.Click += catalogButton_Click;
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
            salesButton.Click += salesButton_Click;
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
            // panelContenedor
            // 
            panelContenedor.BackColor = System.Drawing.Color.White;
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
            panelContenedor.Dock = System.Windows.Forms.DockStyle.Left;
            panelContenedor.Location = new System.Drawing.Point(0, 0);
            panelContenedor.Name = "panelContenedor";
            panelContenedor.Size = new System.Drawing.Size(265, 900);
            panelContenedor.TabIndex = 1;
            panelContenedor.Paint += Sidebar_Paint;
            // 
            // Pagina_Central
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(1500, 900);
            Controls.Add(main);
            Controls.Add(panelContenedor);
            Font = new System.Drawing.Font("Segoe UI", 11F);
            MinimumSize = new System.Drawing.Size(1050, 720);
            Name = "Pagina_Central";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
        private System.Windows.Forms.Button logoutButton = null!;
        private System.Windows.Forms.Label roleLabel = null!;
        private System.Windows.Forms.Label userLabel = null!;
        private System.Windows.Forms.Button settingsButton = null!;
        private System.Windows.Forms.Button investorsButton = null!;
        private System.Windows.Forms.Button rolesButton = null!;
        private System.Windows.Forms.Button returnsButton = null!;
        private System.Windows.Forms.Button catalogButton = null!;
        private System.Windows.Forms.Button bulkButton = null!;
        private System.Windows.Forms.Button validationButton = null!;
        private System.Windows.Forms.Button transferButton = null!;
        private System.Windows.Forms.Button inventoryButton = null!;
        private System.Windows.Forms.Label adminLabel = null!;
        private System.Windows.Forms.Button salesButton = null!;
        private System.Windows.Forms.Button homeButton = null!;
        private System.Windows.Forms.Label discoverLabel = null!;
        private System.Windows.Forms.Label brandLabel = null!;
        private System.Windows.Forms.Panel panelContenedor = null!;
    }
}
