#nullable enable
namespace Sistema_ERP
{
    public sealed partial class AsignarPermisosRol
    {
        private System.ComponentModel.IContainer? components;
        private Label titulo = null!, resumen = null!, mensaje = null!, alcanceModulo = null!, descripcionPreset = null!, avisoAdministrador = null!;
        private ListBox listaModulos = null!;
        private CheckedListBox listaPermisos = null!;
        private ComboBox listaPresets = null!;
        private Button seleccionarModulo = null!, quitarModulo = null!, aceptar = null!, cancelar = null!, aplicarPreset = null!, quitarTodos = null!;
        private FlowLayoutPanel acciones = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            titulo = new Label(); resumen = new Label(); mensaje = new Label();
            alcanceModulo = new Label(); descripcionPreset = new Label(); avisoAdministrador = new Label();
            listaModulos = new ListBox(); listaPermisos = new CheckedListBox();
            listaPresets = new ComboBox(); aplicarPreset = new Button(); quitarTodos = new Button();
            seleccionarModulo = new Button(); quitarModulo = new Button(); aceptar = new Button(); cancelar = new Button();
            SuspendLayout();
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 11F);
            BackColor = Color.White; ForeColor = ProductoEstilo.Texto;
            ClientSize = new Size(1100, 700); MinimumSize = new Size(940, 660);
            Padding = new Padding(24);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false; MaximizeBox = false; MinimizeBox = false;
            Name = "AsignarPermisosRol"; Text = "Permisos por módulo";

            var contenido = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 10, Margin = Padding.Empty };
            contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 10; i++) contenido.RowStyles.Add(new RowStyle(i == 5 ? SizeType.Percent : SizeType.AutoSize, i == 5 ? 100 : 0));
            titulo.Dock = DockStyle.Fill; titulo.AutoSize = true;
            titulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            titulo.MaximumSize = new Size(0, 80); titulo.AutoEllipsis = true;
            titulo.Margin = new Padding(0, 0, 0, 10);
            contenido.Controls.Add(titulo, 0, 0);
            contenido.Controls.Add(new Label
            {
                Text = "Elige una pantalla: CONSULTAR permite abrirla y GESTIONAR modificarla (incluye consulta).",
                Dock = DockStyle.Fill, AutoSize = true, ForeColor = ProductoEstilo.TextoSecundario,
                Margin = new Padding(0, 0, 0, 16)
            }, 0, 1);

            var presets = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 3, RowCount = 1, Margin = Padding.Empty };
            presets.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            presets.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            presets.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            presets.Controls.Add(new Label { Text = "Preset de permisos", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 0, 12, 0) }, 0, 0);
            listaPresets.Name = "listaPresets"; listaPresets.Dock = DockStyle.Fill;
            listaPresets.DropDownStyle = ComboBoxStyle.DropDownList; listaPresets.AccessibleName = "Preset de permisos";
            ProductoEstilo.Boton(aplicarPreset); aplicarPreset.Name = "aplicarPreset";
            aplicarPreset.Text = "Aplicar preset"; aplicarPreset.Width = 160;
            presets.Controls.Add(listaPresets, 1, 0); presets.Controls.Add(aplicarPreset, 2, 0);
            contenido.Controls.Add(presets, 0, 2);
            descripcionPreset.Name = "descripcionPreset"; descripcionPreset.AutoSize = true; descripcionPreset.Dock = DockStyle.Fill;
            descripcionPreset.ForeColor = ProductoEstilo.TextoSecundario; descripcionPreset.Margin = new Padding(0, 4, 0, 10);
            contenido.Controls.Add(descripcionPreset, 0, 3);
            avisoAdministrador.Name = "avisoAdministrador"; avisoAdministrador.AutoSize = true; avisoAdministrador.Dock = DockStyle.Fill;
            avisoAdministrador.ForeColor = ProductoEstilo.Error; avisoAdministrador.Margin = new Padding(0, 0, 0, 10);
            avisoAdministrador.Visible = false;
            contenido.Controls.Add(avisoAdministrador, 0, 4);

            var columnas = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Margin = Padding.Empty };
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 56));
            columnas.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            columnas.Controls.Add(new Label { Text = "Pantallas / módulos", AutoSize = true, Margin = new Padding(0, 0, 0, 8) }, 0, 0);
            columnas.Controls.Add(new Label { Text = "Permisos del módulo", AutoSize = true, Margin = new Padding(0, 0, 0, 8) }, 1, 0);
            listaModulos.Name = "listaModulos";
            listaModulos.Dock = DockStyle.Fill; listaModulos.IntegralHeight = false;
            listaModulos.HorizontalScrollbar = true; listaModulos.BorderStyle = BorderStyle.FixedSingle;
            listaModulos.Margin = new Padding(0, 0, 16, 0);
            listaPermisos.Name = "listaPermisos";
            listaPermisos.Dock = DockStyle.Fill; listaPermisos.IntegralHeight = false;
            listaPermisos.CheckOnClick = true; listaPermisos.HorizontalScrollbar = true;
            listaPermisos.BorderStyle = BorderStyle.FixedSingle; listaPermisos.Margin = Padding.Empty;
            var detalle = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = Padding.Empty };
            detalle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            detalle.RowStyles.Add(new RowStyle(SizeType.AutoSize)); detalle.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            alcanceModulo.Name = "alcanceModulo"; alcanceModulo.AutoSize = true; alcanceModulo.Dock = DockStyle.Fill;
            alcanceModulo.ForeColor = ProductoEstilo.TextoSecundario; alcanceModulo.Margin = new Padding(0, 0, 0, 8);
            detalle.Controls.Add(alcanceModulo, 0, 0); detalle.Controls.Add(listaPermisos, 0, 1);
            columnas.Controls.Add(listaModulos, 0, 1); columnas.Controls.Add(detalle, 1, 1);
            contenido.Controls.Add(columnas, 0, 5);

            var accionesModulo = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, Margin = new Padding(0, 8, 0, 0) };
            ProductoEstilo.Boton(seleccionarModulo); ProductoEstilo.Boton(quitarModulo);
            seleccionarModulo.Name = "seleccionarModulo"; seleccionarModulo.Text = "Seleccionar módulo"; seleccionarModulo.Width = 210;
            quitarModulo.Name = "quitarModulo"; quitarModulo.Text = "Quitar del módulo"; quitarModulo.Width = 210;
            ProductoEstilo.Boton(quitarTodos); quitarTodos.Name = "quitarTodos"; quitarTodos.Text = "Quitar todos"; quitarTodos.Width = 160;
            accionesModulo.Controls.AddRange(new Control[] { seleccionarModulo, quitarModulo, quitarTodos });
            contenido.Controls.Add(accionesModulo, 0, 6);

            var estadoSeleccion = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 2, RowCount = 1, Margin = Padding.Empty };
            estadoSeleccion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 72));
            estadoSeleccion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));
            mensaje.Name = "mensaje"; mensaje.AutoSize = true; mensaje.Dock = DockStyle.Fill;
            mensaje.ForeColor = ProductoEstilo.TextoSecundario; mensaje.Margin = new Padding(0, 8, 0, 0);
            resumen.Name = "resumen"; resumen.AutoSize = true; resumen.Dock = DockStyle.Fill;
            resumen.TextAlign = ContentAlignment.TopRight; resumen.Margin = new Padding(12, 8, 0, 8);
            estadoSeleccion.Controls.Add(mensaje, 0, 0); estadoSeleccion.Controls.Add(resumen, 1, 0);
            contenido.Controls.Add(estadoSeleccion, 0, 7);
            contenido.Controls.Add(new Label
            {
                Text = "Aceptar aplica los permisos al formulario. Pulsa Guardar o Actualizar para conservarlos con el rol.",
                AutoSize = true, Dock = DockStyle.Fill, ForeColor = ProductoEstilo.TextoSecundario, Margin = new Padding(0, 0, 0, 12)
            }, 0, 8);
            acciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false, Margin = Padding.Empty
            };
            ProductoEstilo.Boton(aceptar, true); ProductoEstilo.Boton(cancelar);
            aceptar.Name = "aceptar"; aceptar.Text = "Aceptar"; aceptar.DialogResult = DialogResult.OK;
            cancelar.Name = "cancelar"; cancelar.Text = "Cancelar"; cancelar.DialogResult = DialogResult.Cancel;
            acciones.Controls.AddRange(new Control[] { aceptar, cancelar });
            contenido.Controls.Add(acciones, 0, 9);
            Controls.Add(contenido); AcceptButton = aceptar; CancelButton = cancelar;
            ResumeLayout(true);
        }
    }
}
