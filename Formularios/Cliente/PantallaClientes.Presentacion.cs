namespace Sistema_ERP.Vistas
{

    public partial class PantallaClientes : Form
    {
        private readonly TableLayoutPanel mainLayout = new();
        private readonly Label lblTitulo = new() { Text = "Clientes" };

        private readonly TextBox txtNombreCliente = new();
        private readonly TextBox txtTelefonoCliente = new();
        private readonly TextBox txtCorreoCliente = new();

        private readonly Button btnNuevo = new() { Text = "Nuevo" };
        private readonly Button btnEditar = new() { Text = "Editar" };
        private readonly Button btnGuardarCliente = new() { Text = "Guardar" };
        private readonly Button btnDesactivarCliente = new() { Text = "Desactivar" };
        private readonly Button btnReactivarCliente = new() { Text = "Reactivar" };

        private readonly CheckBox chkClientesInactivos = new() { Text = "Mostrar clientes inactivos", AutoSize = true };
        private readonly DataGridView dgvClientes = new();
        private readonly FlowLayoutPanel panelAcciones = new();

        private void ConstruirPresentacion()
        {
            SuspendLayout();

            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 1;
            mainLayout.RowCount = 5;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 86));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            lblTitulo.Dock = DockStyle.Fill;
            lblTitulo.TextAlign = ContentAlignment.MiddleLeft;

            mainLayout.Controls.Add(lblTitulo, 0, 0);
            mainLayout.Controls.Add(ConstruirFormulario(), 0, 1);

            panelAcciones.Dock = DockStyle.Fill;
            panelAcciones.FlowDirection = FlowDirection.LeftToRight;
            panelAcciones.Controls.AddRange(new Control[]
            {
                btnNuevo, btnEditar, btnGuardarCliente, btnDesactivarCliente, btnReactivarCliente
            });
            mainLayout.Controls.Add(panelAcciones, 0, 2);

            chkClientesInactivos.Dock = DockStyle.Fill;
            mainLayout.Controls.Add(chkClientesInactivos, 0, 3);

            dgvClientes.Dock = DockStyle.Fill;
            mainLayout.Controls.Add(dgvClientes, 0, 4);

            Controls.Add(mainLayout);
            Dock = DockStyle.Fill;


            EstiloFormularios.Aplicar(this, mainLayout, lblTitulo);
            EstiloFormularios.Acciones(panelAcciones, (TextBox)null!);

            ResumeLayout(false);
        }

        private Control ConstruirFormulario()
        {
            var formulario = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 3,
                RowCount = 2
            };
            for (int i = 0; i < 3; i++) formulario.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            formulario.Controls.Add(Etiqueta("Nombre *"), 0, 0);
            formulario.Controls.Add(Etiqueta("Telefono"), 1, 0);
            formulario.Controls.Add(Etiqueta("Correo"), 2, 0);

            foreach (TextBox t in new[] { txtNombreCliente, txtTelefonoCliente, txtCorreoCliente })
            {
                t.Dock = DockStyle.Top;
                t.Margin = new Padding(3, 3, 16, 3);
            }
            txtNombreCliente.MaxLength = 150;
            txtTelefonoCliente.MaxLength = 30;
            txtCorreoCliente.MaxLength = 150;

            formulario.Controls.Add(txtNombreCliente, 0, 1);
            formulario.Controls.Add(txtTelefonoCliente, 1, 1);
            formulario.Controls.Add(txtCorreoCliente, 2, 1);
            return formulario;
        }

        private static Label Etiqueta(string texto) => new()
        {
            Text = texto,
            AutoSize = true,
            ForeColor = Color.FromArgb(125, 125, 125),
            Margin = new Padding(3, 0, 3, 0)
        };
    }
}
