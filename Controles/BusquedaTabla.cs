using System.Data;

namespace Sistema_ERP;

// Filtra la vista, conservando las filas originales para los totales y el guardado.
internal sealed class BusquedaTabla : UserControl
{
    private readonly DataGridView tabla;
    private readonly TextBox buscar = new() { Name = "buscarDatos", PlaceholderText = "Buscar en los datos cargados…", Dock = DockStyle.Fill };
    private readonly ComboBox campo = new() { Name = "campoBusqueda", DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
    private readonly Label resultado = new() { AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };
    private readonly FiltroFechas? fechas;
    private DataView? vista;
    private string filtroBase = "";
    private bool actualizando;
    private sealed record Campo(string Nombre, string Titulo) { public override string ToString() => Titulo; }

    internal static Control Crear(DataGridView tabla, bool conFechas = false) => new BusquedaTabla(tabla, conFechas);

    private BusquedaTabla(DataGridView tabla, bool conFechas)
    {
        this.tabla = tabla;
        Name = "busquedaTabla"; Dock = DockStyle.Fill; Margin = Padding.Empty;
        var filas = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = conFechas ? 3 : 2, Margin = Padding.Empty };
        filas.ColumnStyles.Add(new(SizeType.Percent, 100));
        filas.RowStyles.Add(new(SizeType.AutoSize));
        if (conFechas) filas.RowStyles.Add(new(SizeType.AutoSize));
        filas.RowStyles.Add(new(SizeType.Percent, 100));
        var barra = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 4, Margin = new Padding(0, 0, 0, 6) };
        barra.ColumnStyles.Add(new(SizeType.Percent, 40)); barra.ColumnStyles.Add(new(SizeType.Percent, 60));
        barra.ColumnStyles.Add(new(SizeType.AutoSize)); barra.ColumnStyles.Add(new(SizeType.AutoSize));
        var limpiar = new Button { Name = "limpiarBusqueda", Text = "Limpiar", AutoSize = true };
        campo.AccessibleName = "Campo por el que buscar"; buscar.AccessibleName = "Buscar en los datos cargados";
        limpiar.Click += (_, _) => { buscar.Clear(); buscar.Focus(); };
        barra.Controls.Add(campo, 0, 0); barra.Controls.Add(buscar, 1, 0); barra.Controls.Add(limpiar, 2, 0); barra.Controls.Add(resultado, 3, 0);
        tabla.Dock = DockStyle.Fill; tabla.Margin = Padding.Empty;
        filas.Controls.Add(barra, 0, 0);
        if (conFechas)
        {
            fechas = new FiltroFechas(); fechas.Visible = false;
            fechas.Cambio += (_, _) => Aplicar(); filas.Controls.Add(fechas, 0, 1);
        }
        filas.Controls.Add(tabla, 0, conFechas ? 2 : 1); Controls.Add(filas);
        buscar.TextChanged += (_, _) => Aplicar(); campo.SelectedIndexChanged += (_, _) => Aplicar();
        tabla.DataSourceChanged += (_, _) => Enlazar();
        tabla.DataBindingComplete += (_, _) => { ActualizarCampos(); Aplicar(); };
        tabla.ColumnAdded += (_, _) => ActualizarCampos();
        tabla.ColumnRemoved += (_, _) => ActualizarCampos();
        tabla.ColumnStateChanged += (_, e) => { if ((e.StateChanged & DataGridViewElementStates.Visible) != 0) { ActualizarCampos(); Aplicar(); } };
        tabla.KeyDown += (_, e) => { if (e.Control && e.KeyCode == Keys.F) { e.SuppressKeyPress = true; buscar.Focus(); } };
        Enlazar();
    }

    private void Enlazar()
    {
        if (actualizando) return;
        if (vista is not null) vista.ListChanged -= VistaCambiada;
        vista = tabla.DataSource switch { DataTable datos => datos.DefaultView, DataView datos => datos, _ => null };
        filtroBase = vista?.RowFilter ?? "";
        if (vista is not null) vista.ListChanged += VistaCambiada;
        ActualizarCampos(); Aplicar();
    }

    private void ActualizarCampos()
    {
        if (actualizando) return;
        var campos = tabla.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible && !string.IsNullOrEmpty(c.DataPropertyName)
            && c.ValueType != typeof(byte[])).Select(c => new Campo(c.DataPropertyName, c.HeaderText)).ToArray();
        if (fechas is not null)
        {
            var columnasFecha = tabla.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible && vista?.Table?.Columns[c.DataPropertyName]?.DataType == typeof(DateTime))
                .Select(c => (c.DataPropertyName, c.HeaderText)).ToArray();
            fechas.ConfigurarCampos(columnasFecha); fechas.Visible = columnasFecha.Length > 0;
        }
        if (campo.Items.Count > 0 && campo.Items.Cast<Campo>().Skip(1).SequenceEqual(campos)) return;
        string? seleccionado = (campo.SelectedItem as Campo)?.Nombre;
        actualizando = true;
        try
        {
            campo.Items.Clear(); campo.Items.Add(new Campo("", "Todos los campos")); campo.Items.AddRange(campos);
            campo.SelectedItem = campo.Items.Cast<Campo>().FirstOrDefault(c => c.Nombre == seleccionado) ?? campo.Items[0];
        }
        finally { actualizando = false; }
    }

    private void Aplicar()
    {
        if (actualizando || vista is null) { if (vista is null) resultado.Text = ""; return; }
        if (!tabla.EndEdit()) return;
        actualizando = true;
        try
        {
            var seleccionado = campo.SelectedItem as Campo;
            IEnumerable<string> campos = seleccionado is { Nombre.Length: > 0 } ? [seleccionado.Nombre] : BusquedaDatos.Campos(tabla);
            string filtro = BusquedaDatos.Expresion(vista.Table!, buscar.Text, campos);
            if (fechas?.Columna is { } fecha)
            {
                try
                {
                    string rango = fechas.Rango.Expresion(fecha);
                    if (rango.Length > 0) filtro = filtro.Length == 0 ? rango : $"({filtro}) AND ({rango})";
                }
                catch (ArgumentException ex) { vista.RowFilter = "1 = 0"; resultado.Text = ex.Message; return; }
            }
            vista.RowFilter = filtroBase.Length == 0 ? filtro : filtro.Length == 0 ? filtroBase : $"({filtroBase}) AND ({filtro})";
            resultado.Text = $"{vista.Count} filas";
        }
        finally { actualizando = false; }
    }

    private void VistaCambiada(object? sender, System.ComponentModel.ListChangedEventArgs e)
    {
        if (!actualizando && vista is not null) resultado.Text = $"{vista.Count} filas";
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && vista is not null) vista.ListChanged -= VistaCambiada;
        base.Dispose(disposing);
    }
}
