namespace LoginERP
{
    partial class FrmLogin
    {
        /// <summary>
        /// Variable del disenador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se esten usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codigo generado por el Disenador de Windows Forms

        /// <summary>
        /// Metodo necesario para admitir el Disenador. No se puede modificar
        /// el contenido de este metodo con el editor de codigo.
        /// </summary>
        private void InitializeComponent()
        {
            pnlIzquierdo = new Panel();
            pnlTarjeta3 = new Panel();
            lblTarjeta3Desc = new Label();
            lblTarjeta3Titulo = new Label();
            pnlTarjeta2 = new Panel();
            lblTarjeta2Desc = new Label();
            lblTarjeta2Titulo = new Label();
            pnlTarjeta1 = new Panel();
            lblTarjeta1Desc = new Label();
            lblTarjeta1Titulo = new Label();
            lblPie = new Label();
            lblSubtitulo = new Label();
            lblTitulo = new Label();
            lblMarca = new Label();
            pnlLogin = new Panel();
            btnIngresar = new Button();
            cmbRol = new ComboBox();
            lblRol = new Label();
            lineaRol = new Panel();
            lineaClave = new Panel();
            lineaUsuario = new Panel();
            txtClave = new TextBox();
            txtUsuario = new TextBox();
            lblClave = new Label();
            lblUsuario = new Label();
            lblLoginSub = new Label();
            lblLoginTitulo = new Label();
            txtConfirmacion = new TextBox();
            lblEstado = new Label();
            pnlIzquierdo.SuspendLayout();
            pnlTarjeta3.SuspendLayout();
            pnlTarjeta2.SuspendLayout();
            pnlTarjeta1.SuspendLayout();
            pnlLogin.SuspendLayout();
            SuspendLayout();
            // 
            // pnlIzquierdo
            // 
            pnlIzquierdo.BackColor = Color.FromArgb(242, 242, 242);
            pnlIzquierdo.Controls.Add(pnlTarjeta3);
            pnlIzquierdo.Controls.Add(pnlTarjeta2);
            pnlIzquierdo.Controls.Add(pnlTarjeta1);
            pnlIzquierdo.Controls.Add(lblPie);
            pnlIzquierdo.Controls.Add(lblSubtitulo);
            pnlIzquierdo.Controls.Add(lblTitulo);
            pnlIzquierdo.Controls.Add(lblMarca);
            pnlIzquierdo.Dock = DockStyle.Left;
            pnlIzquierdo.Location = new Point(0, 0);
            pnlIzquierdo.Margin = new Padding(3, 4, 3, 4);
            pnlIzquierdo.Name = "pnlIzquierdo";
            pnlIzquierdo.Size = new Size(540, 812);
            pnlIzquierdo.TabIndex = 0;
            // 
            // pnlTarjeta3
            // 
            pnlTarjeta3.BackColor = Color.White;
            pnlTarjeta3.Controls.Add(lblTarjeta3Desc);
            pnlTarjeta3.Controls.Add(lblTarjeta3Titulo);
            pnlTarjeta3.Location = new Point(40, 650);
            pnlTarjeta3.Margin = new Padding(3, 4, 3, 4);
            pnlTarjeta3.Name = "pnlTarjeta3";
            pnlTarjeta3.Size = new Size(400, 68);
            pnlTarjeta3.TabIndex = 5;
            // 
            // lblTarjeta3Desc
            // 
            lblTarjeta3Desc.AutoSize = true;
            lblTarjeta3Desc.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTarjeta3Desc.ForeColor = Color.FromArgb(110, 110, 110);
            lblTarjeta3Desc.Location = new Point(14, 36);
            lblTarjeta3Desc.Name = "lblTarjeta3Desc";
            lblTarjeta3Desc.Size = new Size(196, 17);
            lblTarjeta3Desc.TabIndex = 1;
            lblTarjeta3Desc.Text = "Cajero, Gerente y Administrador";
            // 
            // lblTarjeta3Titulo
            // 
            lblTarjeta3Titulo.AutoSize = true;
            lblTarjeta3Titulo.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTarjeta3Titulo.ForeColor = Color.FromArgb(17, 17, 17);
            lblTarjeta3Titulo.Location = new Point(14, 10);
            lblTarjeta3Titulo.Name = "lblTarjeta3Titulo";
            lblTarjeta3Titulo.Size = new Size(146, 23);
            lblTarjeta3Titulo.TabIndex = 0;
            lblTarjeta3Titulo.Text = "Control por roles";
            // 
            // pnlTarjeta2
            // 
            pnlTarjeta2.BackColor = Color.White;
            pnlTarjeta2.Controls.Add(lblTarjeta2Desc);
            pnlTarjeta2.Controls.Add(lblTarjeta2Titulo);
            pnlTarjeta2.Location = new Point(40, 569);
            pnlTarjeta2.Margin = new Padding(3, 4, 3, 4);
            pnlTarjeta2.Name = "pnlTarjeta2";
            pnlTarjeta2.Size = new Size(400, 68);
            pnlTarjeta2.TabIndex = 5;
            // 
            // lblTarjeta2Desc
            // 
            lblTarjeta2Desc.AutoSize = true;
            lblTarjeta2Desc.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTarjeta2Desc.ForeColor = Color.FromArgb(110, 110, 110);
            lblTarjeta2Desc.Location = new Point(14, 36);
            lblTarjeta2Desc.Name = "lblTarjeta2Desc";
            lblTarjeta2Desc.Size = new Size(216, 17);
            lblTarjeta2Desc.TabIndex = 1;
            lblTarjeta2Desc.Text = "Solicitud, aprobación y seguimiento";
            // 
            // lblTarjeta2Titulo
            // 
            lblTarjeta2Titulo.AutoSize = true;
            lblTarjeta2Titulo.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTarjeta2Titulo.ForeColor = Color.FromArgb(17, 17, 17);
            lblTarjeta2Titulo.Location = new Point(14, 10);
            lblTarjeta2Titulo.Name = "lblTarjeta2Titulo";
            lblTarjeta2Titulo.Size = new Size(196, 23);
            lblTarjeta2Titulo.TabIndex = 0;
            lblTarjeta2Titulo.Text = "Traspasos entre tiendas";
            // 
            // pnlTarjeta1
            // 
            pnlTarjeta1.BackColor = Color.White;
            pnlTarjeta1.Controls.Add(lblTarjeta1Desc);
            pnlTarjeta1.Controls.Add(lblTarjeta1Titulo);
            pnlTarjeta1.Location = new Point(40, 488);
            pnlTarjeta1.Margin = new Padding(3, 4, 3, 4);
            pnlTarjeta1.Name = "pnlTarjeta1";
            pnlTarjeta1.Size = new Size(400, 68);
            pnlTarjeta1.TabIndex = 4;
            // 
            // lblTarjeta1Desc
            // 
            lblTarjeta1Desc.AutoSize = true;
            lblTarjeta1Desc.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTarjeta1Desc.ForeColor = Color.FromArgb(110, 110, 110);
            lblTarjeta1Desc.Location = new Point(14, 36);
            lblTarjeta1Desc.Name = "lblTarjeta1Desc";
            lblTarjeta1Desc.Size = new Size(246, 17);
            lblTarjeta1Desc.TabIndex = 1;
            lblTarjeta1Desc.Text = "Facturación rápida con control de pagos";
            lblTarjeta1Desc.Click += lblTarjeta1Desc_Click;
            // 
            // lblTarjeta1Titulo
            // 
            lblTarjeta1Titulo.AutoSize = true;
            lblTarjeta1Titulo.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTarjeta1Titulo.ForeColor = Color.FromArgb(17, 17, 17);
            lblTarjeta1Titulo.Location = new Point(14, 10);
            lblTarjeta1Titulo.Name = "lblTarjeta1Titulo";
            lblTarjeta1Titulo.Size = new Size(132, 23);
            lblTarjeta1Titulo.TabIndex = 0;
            lblTarjeta1Titulo.Text = "Punto de Venta";
            // 
            // lblPie
            // 
            lblPie.AutoSize = true;
            lblPie.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPie.ForeColor = Color.FromArgb(110, 110, 110);
            lblPie.Location = new Point(40, 762);
            lblPie.Name = "lblPie";
            lblPie.Size = new Size(161, 17);
            lblPie.TabIndex = 3;
            lblPie.Text = "© 2026 Sistema ERP · v1.0";
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSubtitulo.ForeColor = Color.FromArgb(110, 110, 110);
            lblSubtitulo.Location = new Point(40, 425);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(344, 23);
            lblSubtitulo.TabIndex = 2;
            lblSubtitulo.Text = "Administra todas tus tiendas desde la nube ";
            // 
            // lblTitulo
            // 
            lblTitulo.Font = new Font("Segoe UI", 25.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitulo.ForeColor = Color.FromArgb(17, 17, 17);
            lblTitulo.Location = new Point(40, 188);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(460, 238);
            lblTitulo.TabIndex = 1;
            lblTitulo.Text = "Ventas, inventario y traspasos en un solo lugar ";
            // 
            // lblMarca
            // 
            lblMarca.AutoSize = true;
            lblMarca.Font = new Font("Segoe UI", 13.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblMarca.ForeColor = Color.FromArgb(17, 17, 17);
            lblMarca.Location = new Point(40, 50);
            lblMarca.Name = "lblMarca";
            lblMarca.Size = new Size(140, 30);
            lblMarca.TabIndex = 0;
            lblMarca.Text = "Sistema ERP";
            // 
            // pnlLogin
            // 
            pnlLogin.BackColor = Color.White;
            pnlLogin.Controls.Add(btnIngresar);
            pnlLogin.Controls.Add(cmbRol);
            pnlLogin.Controls.Add(lblRol);
            pnlLogin.Controls.Add(lineaRol);
            pnlLogin.Controls.Add(lineaClave);
            pnlLogin.Controls.Add(lineaUsuario);
            pnlLogin.Controls.Add(txtClave);
            pnlLogin.Controls.Add(txtUsuario);
            pnlLogin.Controls.Add(lblClave);
            pnlLogin.Controls.Add(lblUsuario);
            pnlLogin.Controls.Add(lblLoginSub);
            pnlLogin.Controls.Add(lblLoginTitulo);
            pnlLogin.Controls.Add(txtConfirmacion);
            pnlLogin.Controls.Add(lblEstado);
            pnlLogin.Location = new Point(620, 156);
            pnlLogin.Margin = new Padding(3, 4, 3, 4);
            pnlLogin.Name = "pnlLogin";
            pnlLogin.Size = new Size(400, 612);
            pnlLogin.TabIndex = 1;
            pnlLogin.Paint += pnlLogin_Paint;
            // 
            // btnIngresar
            // 
            btnIngresar.BackColor = Color.FromArgb(17, 17, 17);
            btnIngresar.Cursor = Cursors.Hand;
            btnIngresar.FlatAppearance.BorderSize = 0;
            btnIngresar.FlatStyle = FlatStyle.Flat;
            btnIngresar.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnIngresar.ForeColor = Color.White;
            btnIngresar.Location = new Point(32, 408);
            btnIngresar.Margin = new Padding(3, 4, 3, 4);
            btnIngresar.Name = "btnIngresar";
            btnIngresar.Size = new Size(336, 58);
            btnIngresar.TabIndex = 10;
            btnIngresar.Text = "Ingresar";
            btnIngresar.UseVisualStyleBackColor = false;
            btnIngresar.Click += btnIngresar_Click;
            // 
            // cmbRol
            // 
            cmbRol.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRol.FlatStyle = FlatStyle.Flat;
            cmbRol.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbRol.FormattingEnabled = true;
            cmbRol.Location = new Point(32, 330);
            cmbRol.Margin = new Padding(3, 4, 3, 4);
            cmbRol.Name = "cmbRol";
            cmbRol.Size = new Size(336, 33);
            cmbRol.TabIndex = 9;
            cmbRol.Visible = false;
            cmbRol.SelectedIndexChanged += cmbRol_SelectedIndexChanged;
            // 
            // lblRol
            // 
            lblRol.AutoSize = true;
            lblRol.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblRol.ForeColor = Color.FromArgb(110, 110, 110);
            lblRol.Location = new Point(32, 305);
            lblRol.Name = "lblRol";
            lblRol.Size = new Size(172, 17);
            lblRol.TabIndex = 8;
            lblRol.Text = "Confirmar nueva contraseña";
            lblRol.Visible = false;
            // 
            // lineaRol
            // 
            lineaRol.BackColor = Color.FromArgb(210, 210, 210);
            lineaRol.Location = new Point(32, 375);
            lineaRol.Margin = new Padding(3, 4, 3, 4);
            lineaRol.Name = "lineaRol";
            lineaRol.Size = new Size(336, 1);
            lineaRol.TabIndex = 7;
            lineaRol.Visible = false;
            // 
            // lineaClave
            // 
            lineaClave.BackColor = Color.FromArgb(210, 210, 210);
            lineaClave.Location = new Point(32, 282);
            lineaClave.Margin = new Padding(3, 4, 3, 4);
            lineaClave.Name = "lineaClave";
            lineaClave.Size = new Size(336, 1);
            lineaClave.TabIndex = 6;
            // 
            // lineaUsuario
            // 
            lineaUsuario.BackColor = Color.FromArgb(210, 210, 210);
            lineaUsuario.Location = new Point(32, 198);
            lineaUsuario.Margin = new Padding(3, 4, 3, 4);
            lineaUsuario.Name = "lineaUsuario";
            lineaUsuario.Size = new Size(336, 1);
            lineaUsuario.TabIndex = 5;
            // 
            // txtClave
            // 
            txtClave.BorderStyle = BorderStyle.None;
            txtClave.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtClave.Location = new Point(32, 248);
            txtClave.Margin = new Padding(3, 4, 3, 4);
            txtClave.MaxLength = 128;
            txtClave.Name = "txtClave";
            txtClave.Size = new Size(336, 24);
            txtClave.TabIndex = 4;
            txtClave.UseSystemPasswordChar = true;
            // 
            // txtUsuario
            // 
            txtUsuario.BorderStyle = BorderStyle.None;
            txtUsuario.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtUsuario.Location = new Point(32, 162);
            txtUsuario.Margin = new Padding(3, 4, 3, 4);
            txtUsuario.MaxLength = 50;
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(336, 24);
            txtUsuario.TabIndex = 2;
            // 
            // lblClave
            // 
            lblClave.AutoSize = true;
            lblClave.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblClave.ForeColor = Color.FromArgb(110, 110, 110);
            lblClave.Location = new Point(32, 220);
            lblClave.Name = "lblClave";
            lblClave.Size = new Size(74, 17);
            lblClave.TabIndex = 3;
            lblClave.Text = "Contraseña";
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblUsuario.ForeColor = Color.FromArgb(110, 110, 110);
            lblUsuario.Location = new Point(32, 135);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(53, 17);
            lblUsuario.TabIndex = 2;
            lblUsuario.Text = "Usuario";
            // 
            // lblLoginSub
            // 
            lblLoginSub.AutoSize = true;
            lblLoginSub.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblLoginSub.ForeColor = Color.FromArgb(110, 110, 110);
            lblLoginSub.Location = new Point(32, 85);
            lblLoginSub.Name = "lblLoginSub";
            lblLoginSub.Size = new Size(266, 20);
            lblLoginSub.TabIndex = 1;
            lblLoginSub.Text = "Ingresa tus credenciales para continuar";
            // 
            // lblLoginTitulo
            // 
            lblLoginTitulo.AutoSize = true;
            lblLoginTitulo.Font = new Font("Segoe UI", 19.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLoginTitulo.ForeColor = Color.FromArgb(17, 17, 17);
            lblLoginTitulo.Location = new Point(32, 32);
            lblLoginTitulo.Name = "lblLoginTitulo";
            lblLoginTitulo.Size = new Size(221, 45);
            lblLoginTitulo.TabIndex = 0;
            lblLoginTitulo.Text = "Iniciar sesión";
            // 
            // txtConfirmacion
            // 
            txtConfirmacion.Font = new Font("Segoe UI", 11F);
            txtConfirmacion.Location = new Point(32, 330);
            txtConfirmacion.Margin = new Padding(3, 4, 3, 4);
            txtConfirmacion.MaxLength = 128;
            txtConfirmacion.Name = "txtConfirmacion";
            txtConfirmacion.Size = new Size(336, 32);
            txtConfirmacion.TabIndex = 9;
            txtConfirmacion.UseSystemPasswordChar = true;
            txtConfirmacion.Visible = false;
            // 
            // lblEstado
            // 
            lblEstado.Font = new Font("Segoe UI", 10F);
            lblEstado.ForeColor = Color.FromArgb(178, 34, 34);
            lblEstado.Location = new Point(32, 478);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(336, 112);
            lblEstado.TabIndex = 11;
            // 
            // FrmLogin
            // 
            AcceptButton = btnIngresar;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 250, 250);
            ClientSize = new Size(1098, 812);
            Controls.Add(pnlLogin);
            Controls.Add(pnlIzquierdo);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            Name = "FrmLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Iniciar Sesion";
            Load += FrmLogin_Load;
            pnlIzquierdo.ResumeLayout(false);
            pnlIzquierdo.PerformLayout();
            pnlTarjeta3.ResumeLayout(false);
            pnlTarjeta3.PerformLayout();
            pnlTarjeta2.ResumeLayout(false);
            pnlTarjeta2.PerformLayout();
            pnlTarjeta1.ResumeLayout(false);
            pnlTarjeta1.PerformLayout();
            pnlLogin.ResumeLayout(false);
            pnlLogin.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlIzquierdo;
        private System.Windows.Forms.Label lblMarca;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblPie;
        private System.Windows.Forms.Panel pnlTarjeta1;
        private System.Windows.Forms.Label lblTarjeta1Titulo;
        private System.Windows.Forms.Label lblTarjeta1Desc;
        private System.Windows.Forms.Panel pnlTarjeta3;
        private System.Windows.Forms.Label lblTarjeta3Desc;
        private System.Windows.Forms.Label lblTarjeta3Titulo;
        private System.Windows.Forms.Panel pnlTarjeta2;
        private System.Windows.Forms.Label lblTarjeta2Desc;
        private System.Windows.Forms.Label lblTarjeta2Titulo;
        private System.Windows.Forms.Panel pnlLogin;
        private System.Windows.Forms.Label lblLoginTitulo;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.Label lblLoginSub;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label lblClave;
        private System.Windows.Forms.TextBox txtClave;
        private System.Windows.Forms.Panel lineaUsuario;
        private System.Windows.Forms.Panel lineaRol;
        private System.Windows.Forms.Panel lineaClave;
        private System.Windows.Forms.Label lblRol;
        private System.Windows.Forms.ComboBox cmbRol;
        private System.Windows.Forms.Button btnIngresar;
        private System.Windows.Forms.TextBox txtConfirmacion;
        private System.Windows.Forms.Label lblEstado;
    }
}

