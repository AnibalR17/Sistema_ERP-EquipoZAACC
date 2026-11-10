#nullable enable
namespace Sistema_ERP;

public sealed partial class EditorPermisosRol
{
    private System.ComponentModel.IContainer? components;
    private Label titulo = null!, estado = null!;
    private ComboBox modulo = null!, accion = null!;
    private TextBox descripcion = null!, buscar = null!;
    private CheckBox inactivos = null!;
    private DataGridView tabla = null!;
    private Button nuevo = null!, guardar = null!, actualizar = null!, desactivar = null!, reactivar = null!;
    private FlowLayoutPanel acciones = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        titulo = new Label(); estado = new Label(); modulo = new ComboBox(); accion = new ComboBox();
        descripcion = new TextBox(); buscar = new TextBox(); inactivos = new CheckBox(); tabla = new DataGridView();
        nuevo = new Button(); guardar = new Button(); actualizar = new Button(); desactivar = new Button(); reactivar = new Button();
        acciones = new FlowLayoutPanel();
        SuspendLayout();
        Name = "EditorPermisosRol"; Text = "Crear / editar permisos del rol";
        Font = new Font("Segoe UI", 11F); BackColor = Color.White; ForeColor = ProductoEstilo.Texto;
        AutoScaleMode = AutoScaleMode.Font; ClientSize = new Size(1100, 700); MinimumSize = new Size(940, 660);
        Padding = new Padding(24); StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false; MinimizeBox = MaximizeBox = false;
        var contenido = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 8, Margin = Padding.Empty };
        contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (int i = 0; i < 8; i++) contenido.RowStyles.Add(new RowStyle(i == 6 ? SizeType.Percent : SizeType.AutoSize, i == 6 ? 100 : 0));
        titulo.AutoSize = true; titulo.Dock = DockStyle.Fill; titulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        titulo.AutoEllipsis = true; titulo.MaximumSize = new Size(0, 80); titulo.Margin = new Padding(0, 0, 0, 12);
        contenido.Controls.Add(titulo, 0, 0);
        contenido.Controls.Add(new Label
        {
            Text = "Guardar permiso actualiza el catálogo compartido por los roles. La asignación al rol se conserva al volver y pulsar Guardar o Actualizar en Roles. Consultar y Gestionar controlan los accesos actuales; otras acciones requieren validación en su pantalla.",
            AutoSize = true, Dock = DockStyle.Fill, ForeColor = ProductoEstilo.TextoSecundario, Margin = new Padding(0, 0, 0, 14)
        }, 0, 1);
        var campos = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 3, RowCount = 2, Margin = Padding.Empty };
        campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40)); campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20)); campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        foreach (var (texto, columna) in new[] { ("Pantalla / módulo *", 0), ("Acción *", 1), ("Descripción *", 2) })
            campos.Controls.Add(new Label { Text = texto, AutoSize = true, Margin = new Padding(0, 0, 0, 8) }, columna, 0);
        modulo.Name = "modulo"; modulo.MaxLength = 50; modulo.DropDownWidth = 600;
        accion.Name = "accion"; accion.MaxLength = 30; descripcion.Name = "descripcion"; descripcion.MaxLength = 300;
        modulo.AccessibleName = "Pantalla o código de módulo"; accion.AccessibleName = "Acción del permiso"; descripcion.AccessibleName = "Descripción del permiso";
        foreach (var control in new Control[] { modulo, accion, descripcion }) { control.Dock = DockStyle.Fill; control.Margin = new Padding(0, 0, 12, 12); }
        campos.Controls.Add(modulo, 0, 1); campos.Controls.Add(accion, 1, 1); campos.Controls.Add(descripcion, 2, 1);
        contenido.Controls.Add(campos, 0, 2);
        acciones.AutoSize = true; acciones.Dock = DockStyle.Fill; acciones.Margin = Padding.Empty;
        foreach (var (boton, nombre, texto) in new[] { (nuevo, "nuevo", "Nuevo"), (guardar, "guardar", "Guardar permiso"),
            (actualizar, "actualizar", "Actualizar"), (desactivar, "desactivar", "Desactivar"), (reactivar, "reactivar", "Reactivar") })
        {
            ProductoEstilo.Boton(boton, boton == guardar); boton.Name = nombre; boton.Text = texto;
            boton.Width = boton == guardar ? 160 : 120; acciones.Controls.Add(boton);
        }
        contenido.Controls.Add(acciones, 0, 3);
        estado.Name = "estado"; estado.AutoSize = true; estado.Dock = DockStyle.Fill; estado.Margin = new Padding(0, 8, 0, 8);
        contenido.Controls.Add(estado, 0, 4);
        var filtros = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 8) };
        buscar.Name = "buscar"; buscar.Width = 360; buscar.PlaceholderText = "Buscar módulo, acción o descripción…";
        inactivos.Name = "inactivos"; inactivos.Text = "Incluir inactivos"; inactivos.AutoSize = true;
        filtros.Controls.AddRange([buscar, inactivos]); contenido.Controls.Add(filtros, 0, 5);
        tabla.Name = "tabla"; tabla.Dock = DockStyle.Fill; tabla.ReadOnly = true;
        tabla.AllowUserToAddRows = tabla.AllowUserToDeleteRows = false; tabla.AutoGenerateColumns = false;
        tabla.MultiSelect = false; tabla.RowHeadersVisible = false; tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        tabla.BackgroundColor = Color.White; tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        foreach (var (propiedad, texto, peso) in new[] { ("Primero", "Pantalla / módulo", 30), ("Segundo", "Acción", 15),
            ("Descripcion", "Descripción", 35), ("motivo_desactivacion", "Motivo de desactivación", 20) })
            tabla.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = propiedad, HeaderText = texto, FillWeight = peso });
        tabla.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "activo", HeaderText = "Activo", FillWeight = 8 });
        contenido.Controls.Add(tabla, 0, 6);
        var pie = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Margin = new Padding(0, 12, 0, 0) };
        var cerrar = new Button { Name = "cerrar", Text = "Volver a Roles", DialogResult = DialogResult.OK };
        ProductoEstilo.Boton(cerrar); cerrar.Width = 190; pie.Controls.Add(cerrar);
        contenido.Controls.Add(pie, 0, 7); Controls.Add(contenido); CancelButton = cerrar;
        ResumeLayout(true);
    }
}
