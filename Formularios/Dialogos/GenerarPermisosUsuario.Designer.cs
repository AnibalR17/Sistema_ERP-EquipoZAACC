#nullable enable
namespace Sistema_ERP
{
    public sealed partial class GenerarPermisosUsuario
    {
        private System.ComponentModel.IContainer? components;
        private Label titulo = null!, resumen = null!, mensajePermisos = null!;
        private SelectorRoles listaRoles = null!;
        private DataGridView tablaPermisos = null!;
        private Button aceptar = null!, cancelar = null!;
        private FlowLayoutPanel acciones = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            titulo = new Label(); resumen = new Label(); mensajePermisos = new Label();
            listaRoles = new SelectorRoles(); tablaPermisos = new DataGridView();
            aceptar = new Button(); cancelar = new Button();
            SuspendLayout();
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 11F);
            BackColor = Color.White;
            ForeColor = ProductoEstilo.Texto;
            ClientSize = new Size(940, 620);
            MinimumSize = new Size(800, 560);
            Padding = new Padding(24);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false; MaximizeBox = false; MinimizeBox = false;
            Text = "Generar permisos";
            Name = "GenerarPermisosUsuario";

            var contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 6, Margin = Padding.Empty
            };
            contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            contenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            contenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            contenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            titulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            titulo.Dock = DockStyle.Fill; titulo.AutoSize = true;
            titulo.AutoEllipsis = true; titulo.MaximumSize = new Size(0, 80);
            titulo.Margin = new Padding(0, 0, 0, 10);
            contenido.Controls.Add(titulo, 0, 0);
            var indicacion = new Label
            {
                Text = "Agrega roles desde el desplegable o quítalos de la lista. Los roles inactivos no conceden permisos. Gestionar requiere también Consultar del mismo módulo.",
                Dock = DockStyle.Fill, AutoSize = true, ForeColor = ProductoEstilo.TextoSecundario,
                Margin = new Padding(0, 0, 0, 16)
            };
            contenido.Controls.Add(indicacion, 0, 1);

            var columnas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Margin = Padding.Empty
            };
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68));
            columnas.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            columnas.Controls.Add(new Label { Text = "Roles del usuario", AutoSize = true, Margin = new Padding(0, 0, 0, 8) }, 0, 0);
            columnas.Controls.Add(new Label { Text = "Permisos de los roles seleccionados", AutoSize = true, Margin = new Padding(0, 0, 0, 8) }, 1, 0);
            listaRoles.Name = "listaRoles";
            listaRoles.Dock = DockStyle.Fill;
            listaRoles.Margin = new Padding(0, 0, 16, 0);
            columnas.Controls.Add(listaRoles, 0, 1);

            var vistaPermisos = new Panel { Dock = DockStyle.Fill, Margin = Padding.Empty };
            tablaPermisos.Name = "tablaPermisos";
            tablaPermisos.Dock = DockStyle.Fill; tablaPermisos.ReadOnly = true;
            tablaPermisos.AllowUserToAddRows = tablaPermisos.AllowUserToDeleteRows = false;
            tablaPermisos.AutoGenerateColumns = false;
            tablaPermisos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tablaPermisos.RowHeadersVisible = false; tablaPermisos.BackgroundColor = Color.White;
            tablaPermisos.BorderStyle = BorderStyle.FixedSingle;
            tablaPermisos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tablaPermisos.MultiSelect = false; tablaPermisos.RowTemplate.Height = 34;
            tablaPermisos.EnableHeadersVisualStyles = false;
            tablaPermisos.ColumnHeadersDefaultCellStyle.BackColor = ProductoEstilo.Suave;
            tablaPermisos.DefaultCellStyle.SelectionBackColor = ProductoEstilo.Suave;
            tablaPermisos.DefaultCellStyle.SelectionForeColor = ProductoEstilo.Texto;
            tablaPermisos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Modulo", HeaderText = "Pantalla / módulo", FillWeight = 30 });
            tablaPermisos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Accion", HeaderText = "Acción", FillWeight = 25 });
            tablaPermisos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Descripcion", HeaderText = "Descripción", FillWeight = 45 });
            mensajePermisos.Name = "mensajePermisos";
            mensajePermisos.Dock = DockStyle.Fill; mensajePermisos.TextAlign = ContentAlignment.MiddleCenter;
            mensajePermisos.Padding = new Padding(20); mensajePermisos.BackColor = ProductoEstilo.Suave;
            vistaPermisos.Controls.Add(tablaPermisos); vistaPermisos.Controls.Add(mensajePermisos);
            columnas.Controls.Add(vistaPermisos, 1, 1);
            contenido.Controls.Add(columnas, 0, 2);
            resumen.Name = "resumen";
            resumen.AutoSize = true; resumen.Margin = new Padding(0, 12, 0, 8);
            contenido.Controls.Add(resumen, 0, 3);
            contenido.Controls.Add(new Label
            {
                Text = "Al aceptar, la selección se aplica al formulario. Pulsa Guardar o Actualizar para conservarla con el usuario.",
                AutoSize = true, Dock = DockStyle.Fill, ForeColor = ProductoEstilo.TextoSecundario,
                Margin = new Padding(0, 0, 0, 12)
            }, 0, 4);
            acciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false, Margin = Padding.Empty
            };
            ProductoEstilo.Boton(aceptar, true); ProductoEstilo.Boton(cancelar);
            aceptar.Name = "aceptar"; aceptar.Text = "Aceptar"; aceptar.DialogResult = DialogResult.OK;
            cancelar.Name = "cancelar"; cancelar.Text = "Cancelar"; cancelar.DialogResult = DialogResult.Cancel;
            acciones.Controls.AddRange(new Control[] { aceptar, cancelar });
            contenido.Controls.Add(acciones, 0, 5);
            Controls.Add(contenido);
            AcceptButton = aceptar; CancelButton = cancelar;
            ResumeLayout(true);
        }
    }
}
