namespace Sistema_ERP.Vistas
{
    public partial class PantallaProveedores
    {
        private readonly TextBox txtEmpresaProveedor = new() { Name = "txtEmpresaProveedor", MaxLength = 150 };
        private readonly TextBox txtRtnProveedor = new() { Name = "txtRtnProveedor", MaxLength = 30 };
        private readonly TextBox txtContactoProveedor = new() { Name = "txtContactoProveedor", MaxLength = 150 };
        private readonly TextBox txtTelefonoProveedor = new() { Name = "txtTelefonoProveedor", MaxLength = 30 };
        private readonly TextBox txtCorreoProveedor = new() { Name = "txtCorreoProveedor", MaxLength = 254 };
        private readonly TextBox txtDireccionProveedor = new() { Name = "txtDireccionProveedor", MaxLength = 300 };
        private readonly Button btnGuardarProveedor = new() { Name = "btnGuardarProveedor", Text = "Guardar" };
        private readonly Button btnDesactivarProveedor = new() { Name = "btnDesactivarProveedor", Text = "Desactivar", Enabled = false };
        private readonly Button btnReactivarProveedor = new() { Name = "btnReactivarProveedor", Text = "Reactivar", Enabled = false };
        private readonly CheckBox chkProveedoresInactivos = new()
        {
            Name = "chkProveedoresInactivos",
            Text = "Mostrar también proveedores inactivos",
            AutoSize = true
        };

        private void ConfigurarPresentacion()
        {
            EstiloFormularios.Aplicar(this, mainLayout, lblTitulo);
            SuspendLayout();
            mainLayout.SuspendLayout();
            mainLayout.Controls.Clear();
            mainLayout.RowStyles.Clear();
            mainLayout.RowCount = 7;
            mainLayout.MinimumSize = new Size(680, 620);
            foreach (float altura in new[] { 72F, 56F, 156F, 76F, 36F, 44F })
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, altura));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var campos = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2,
                Margin = Padding.Empty
            };
            for (int columna = 0; columna < 3; columna++)
                campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / 3));
            campos.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            campos.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            var entradas = new (string Etiqueta, TextBox Campo)[]
            {
                ("Empresa *", txtEmpresaProveedor), ("RTN (opcional)", txtRtnProveedor),
                ("Contacto", txtContactoProveedor), ("Teléfono", txtTelefonoProveedor),
                ("Correo", txtCorreoProveedor), ("Dirección", txtDireccionProveedor)
            };
            for (int i = 0; i < entradas.Length; i++)
            {
                var celda = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 2,
                    Margin = new Padding(0, 0, 12, 8)
                };
                celda.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
                celda.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                celda.Controls.Add(new Label { Text = entradas[i].Etiqueta, Dock = DockStyle.Fill }, 0, 0);
                entradas[i].Campo.Dock = DockStyle.Top;
                entradas[i].Campo.BorderStyle = BorderStyle.FixedSingle;
                celda.Controls.Add(entradas[i].Campo, 0, 1);
                campos.Controls.Add(celda, i % 3, i / 3);
            }

            pnlAcciones.Controls.Clear();
            btnNuevo.Text = "Nuevo";
            btnEditar.Text = "Actualizar";
            btnEditar.Enabled = false;
            EstiloFormularios.Acciones(pnlAcciones, txtBuscar, btnGuardarProveedor, btnEditar,
                btnDesactivarProveedor, btnReactivarProveedor, btnNuevo);
            pnlAcciones.Controls.AddRange(new Control[] { btnGuardarProveedor, btnEditar,
                btnDesactivarProveedor, btnReactivarProveedor, btnNuevo });
            pnlAcciones.Margin = Padding.Empty;
            lblInfo.Text = "El nombre de la empresa es obligatorio. Selecciona una fila para actualizar sus datos.";
            lblInfo.AutoEllipsis = true;
            txtBuscar.PlaceholderText = "Buscar proveedor por nombre o RTN...";
            txtBuscar.Dock = DockStyle.Fill;
            txtBuscar.Margin = new Padding(0, 4, 0, 4);
            mainLayout.Controls.Add(lblTitulo, 0, 0);
            mainLayout.Controls.Add(pnlInfo, 0, 1);
            mainLayout.Controls.Add(campos, 0, 2);
            mainLayout.Controls.Add(pnlAcciones, 0, 3);
            mainLayout.Controls.Add(chkProveedoresInactivos, 0, 4);
            mainLayout.Controls.Add(txtBuscar, 0, 5);
            mainLayout.Controls.Add(pnlContenedorGrid, 0, 6);
            mainLayout.ResumeLayout(true);
            ResumeLayout(true);
        }
    }
}
