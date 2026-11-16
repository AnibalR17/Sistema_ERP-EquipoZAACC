namespace Sistema_ERP;

internal sealed class FiltroFechas : UserControl
{
    private sealed record Campo(string Nombre, string Titulo) { public override string ToString() => Titulo; }
    private readonly ComboBox campo = new() { Name = "campoFecha", DropDownStyle = ComboBoxStyle.DropDownList, Width = 180 };
    private readonly DateTimePicker desde = Fecha("desdeFecha"), hasta = Fecha("hastaFecha");
    private bool actualizando;
    internal event EventHandler? Cambio;
    internal DateTime? Desde => desde.Checked ? desde.Value.Date : null;
    internal DateTime? Hasta => hasta.Checked ? hasta.Value.Date : null;
    internal string? Columna => (campo.SelectedItem as Campo)?.Nombre;
    internal RangoFechas Rango => new(Desde, Hasta);

    internal FiltroFechas(string? columna = null)
    {
        Name = "filtroFechas"; Dock = DockStyle.Fill; AutoSize = true; AutoSizeMode = AutoSizeMode.GrowAndShrink; Margin = Padding.Empty;
        var barra = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true, Margin = Padding.Empty };
        Label Etiqueta(string texto) => new() { Text = texto, AutoSize = true, Margin = new Padding(3, 7, 3, 0) };
        var limpiar = new Button { Name = "limpiarFechas", Text = "Quitar fechas", AutoSize = true };
        barra.Controls.AddRange([campo, Etiqueta("Desde"), desde, Etiqueta("Hasta"), hasta, limpiar]);
        Controls.Add(barra);
        desde.Value = DateTime.UtcNow.Date.AddDays(-30); hasta.Value = DateTime.UtcNow.Date;
        desde.Checked = hasta.Checked = false;
        desde.AccessibleName = "Fecha desde"; hasta.AccessibleName = "Fecha hasta"; campo.AccessibleName = "Fecha del historial que se filtrará";
        desde.ValueChanged += (_, _) => Notificar(); hasta.ValueChanged += (_, _) => Notificar();
        campo.SelectedIndexChanged += (_, _) => Notificar();
        limpiar.Click += (_, _) =>
        {
            actualizando = true;
            try { desde.Checked = hasta.Checked = false; }
            finally { actualizando = false; }
            Notificar();
        };
        if (columna is not null) ConfigurarCampos([(columna, columna)]);
    }

    private static DateTimePicker Fecha(string nombre) => new()
    { Name = nombre, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy", ShowCheckBox = true, Width = 145 };

    private void Notificar() { if (!actualizando) Cambio?.Invoke(this, EventArgs.Empty); }

    internal void ConfigurarCampos(IEnumerable<(string Nombre, string Titulo)> columnas)
    {
        var opciones = columnas.Select(c => new Campo(c.Nombre, c.Titulo)).ToArray();
        if (campo.Items.Cast<Campo>().SequenceEqual(opciones)) return;
        string? anterior = Columna;
        actualizando = true;
        try
        {
            campo.Items.Clear(); campo.Items.AddRange(opciones);
            campo.SelectedItem = campo.Items.Cast<Campo>().FirstOrDefault(c => c.Nombre == anterior) ?? opciones.FirstOrDefault();
            campo.Visible = opciones.Length > 1;
        }
        finally { actualizando = false; }
    }
}
