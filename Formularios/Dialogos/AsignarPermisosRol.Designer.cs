#nullable enable
namespace Sistema_ERP
{
    public sealed partial class AsignarPermisosRol
    {
        private System.ComponentModel.IContainer? components;
        private Label titulo = null!, resumen = null!, mensaje = null!;
        private ListBox listaModulos = null!;
        private CheckedListBox listaPermisos = null!;
        private Button seleccionarModulo = null!, quitarModulo = null!, aceptar = null!, cancelar = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            titulo = new Label(); resumen = new Label(); mensaje = new Label();
            listaModulos = new ListBox(); listaPermisos = new CheckedListBox();
            seleccionarModulo = new Button(); quitarModulo = new Button(); aceptar = new Button(); cancelar = new Button();
            SuspendLayout();
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 11F);
            BackColor = Color.White; ForeColor = ProductoEstilo.Texto;
            ClientSize = new Size(960, 640); MinimumSize = new Size(820, 580);
            Padding = new Padding(24);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false; MaximizeBox = false; MinimizeBox = false;
            Name = "AsignarPermisosRol"; Text = "Permisos por módulo";

            var contenido = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 7, Margin = Padding.Empty };
            contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 7; i++) contenido.RowStyles.Add(new RowStyle(i == 2 ? SizeType.Percent : SizeType.AutoSize, i == 2 ? 100 : 0));
            titulo.Dock = DockStyle.Fill; titulo.AutoSize = true;
            titulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            titulo.MaximumSize = new Size(0, 80); titulo.AutoEllipsis = true;
            titulo.Margin = new Padding(0, 0, 0, 10);
            contenido.Controls.Add(titulo, 0, 0);
            contenido.Controls.Add(new Label
            {
                Text = "Selecciona un módulo y marca sus permisos. Al marcar GESTIONAR se incluye CONSULTAR del mismo módulo; al quitar CONSULTAR también se retira GESTIONAR.",
                Dock = DockStyle.Fill, AutoSize = true, ForeColor = ProductoEstilo.TextoSecundario,
                Margin = new Padding(0, 0, 0, 16)
            }, 0, 1);

            var columnas = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Margin = Padding.Empty };
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68));
            columnas.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            columnas.Controls.Add(new Label { Text = "Módulos", AutoSize = true, Margin = new Padding(0, 0, 0, 8) }, 0, 0);
            columnas.Controls.Add(new Label { Text = "Permisos del módulo", AutoSize = true, Margin = new Padding(0, 0, 0, 8) }, 1, 0);
            listaModulos.Name = "listaModulos";
            listaModulos.Dock = DockStyle.Fill; listaModulos.IntegralHeight = false;
            listaModulos.HorizontalScrollbar = true; listaModulos.BorderStyle = BorderStyle.FixedSingle;
            listaModulos.Margin = new Padding(0, 0, 16, 0);
            listaPermisos.Name = "listaPermisos";
            listaPermisos.Dock = DockStyle.Fill; listaPermisos.IntegralHeight = false;
            listaPermisos.CheckOnClick = true; listaPermisos.HorizontalScrollbar = true;
            listaPermisos.BorderStyle = BorderStyle.FixedSingle; listaPermisos.Margin = Padding.Empty;
            columnas.Controls.Add(listaModulos, 0, 1); columnas.Controls.Add(listaPermisos, 1, 1);
            contenido.Controls.Add(columnas, 0, 2);

            var accionesModulo = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, Margin = new Padding(0, 8, 0, 0) };
            ProductoEstilo.Boton(seleccionarModulo); ProductoEstilo.Boton(quitarModulo);
            seleccionarModulo.Name = "seleccionarModulo"; seleccionarModulo.Text = "Seleccionar módulo"; seleccionarModulo.Width = 210;
            quitarModulo.Name = "quitarModulo"; quitarModulo.Text = "Quitar del módulo"; quitarModulo.Width = 210;
            accionesModulo.Controls.AddRange(new Control[] { seleccionarModulo, quitarModulo });
            contenido.Controls.Add(accionesModulo, 0, 3);

            var estadoSeleccion = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 1, RowCount = 2, Margin = Padding.Empty };
            estadoSeleccion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            mensaje.Name = "mensaje"; mensaje.AutoSize = true; mensaje.Dock = DockStyle.Fill;
            mensaje.ForeColor = ProductoEstilo.TextoSecundario; mensaje.Margin = new Padding(0, 8, 0, 0);
            resumen.Name = "resumen"; resumen.AutoSize = true; resumen.Margin = new Padding(0, 8, 0, 8);
            estadoSeleccion.Controls.Add(mensaje, 0, 0); estadoSeleccion.Controls.Add(resumen, 0, 1);
            contenido.Controls.Add(estadoSeleccion, 0, 4);
            contenido.Controls.Add(new Label
            {
                Text = "Aceptar aplica los permisos al formulario. Pulsa Guardar o Actualizar para conservarlos con el rol.",
                AutoSize = true, Dock = DockStyle.Fill, ForeColor = ProductoEstilo.TextoSecundario, Margin = new Padding(0, 0, 0, 12)
            }, 0, 5);
            var acciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false, Margin = Padding.Empty
            };
            ProductoEstilo.Boton(aceptar, true); ProductoEstilo.Boton(cancelar);
            aceptar.Name = "aceptar"; aceptar.Text = "Aceptar"; aceptar.DialogResult = DialogResult.OK;
            cancelar.Name = "cancelar"; cancelar.Text = "Cancelar"; cancelar.DialogResult = DialogResult.Cancel;
            acciones.Controls.AddRange(new Control[] { aceptar, cancelar });
            contenido.Controls.Add(acciones, 0, 6);
            Controls.Add(contenido); AcceptButton = aceptar; CancelButton = cancelar;
            ResumeLayout(true);
        }
    }
}
