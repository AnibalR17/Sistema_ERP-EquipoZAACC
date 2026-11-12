namespace Sistema_ERP;

internal static class DisposicionControles
{
    internal static void Boton(Button boton)
    {
        int Escalar(int valor) => (int)Math.Ceiling(valor * boton.DeviceDpi / 96d);
        boton.AutoSize = true;
        boton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        boton.Padding = new Padding(Escalar(12), Escalar(4), Escalar(12), Escalar(4));
        boton.MinimumSize = new Size(Escalar(120), Math.Max(Escalar(44), boton.Font.Height + boton.Padding.Vertical + Escalar(8)));
    }

    internal static void Preparar(Control raiz)
    {
        raiz.SuspendLayout();
        try
        {
            foreach (Control hijo in raiz.Controls) Preparar(hijo);

            if (raiz is Label etiqueta && etiqueta.Parent is TableLayoutPanel) etiqueta.AutoSize = true;

            if (raiz is Button boton && boton.Parent is FlowLayoutPanel or TableLayoutPanel)
                Boton(boton);

            int altoCampo = raiz switch
            {
                TextBoxBase { Multiline: false } entrada => entrada.PreferredHeight,
                UpDownBase numero => numero.PreferredHeight,
                _ => 0
            };
            if (altoCampo > 0) raiz.MinimumSize = new Size(raiz.MinimumSize.Width, altoCampo);

            if (raiz is FlowLayoutPanel barra && barra.Controls.OfType<Button>().Any())
            {
                barra.AutoSize = true;
                barra.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                barra.WrapContents = true;
                barra.AutoScroll = false;
            }

            if (raiz is TableLayoutPanel tabla) PrepararFilas(tabla);
        }
        finally { raiz.ResumeLayout(true); }
    }

    private static bool EsContenido(Control control) => control is
        Label or Button or TextBoxBase or ComboBox or NumericUpDown or DateTimePicker or CheckBox or RadioButton
        || control is FlowLayoutPanel { AutoSize: true }
        || control is TableLayoutPanel { AutoSize: true };

    private static void PrepararFilas(TableLayoutPanel tabla)
    {
        bool soloContenido = tabla.Controls.Count > 0 && tabla.Controls.Cast<Control>().All(EsContenido);
        // Las filas de datos conservan el espacio disponible; los campos y las
        // acciones reservan su altura preferida, incluidos fuente y márgenes.
        foreach (Control control in tabla.Controls)
        {
            if (tabla.GetRowSpan(control) != 1 || !EsContenido(control)) continue;
            int fila = tabla.GetPositionFromControl(control).Row;
            if (fila < 0 || fila >= tabla.RowStyles.Count) continue;
            if (tabla.RowStyles[fila] is { SizeType: SizeType.Absolute, Height: 0 }) continue;
            if (soloContenido || tabla.RowStyles[fila].SizeType == SizeType.Absolute)
                tabla.RowStyles[fila].SizeType = SizeType.AutoSize;

            if (control is Button)
            {
                int columna = tabla.GetPositionFromControl(control).Column;
                if (tabla.GetColumnSpan(control) == 1 && columna >= 0 && columna < tabla.ColumnStyles.Count
                    && tabla.ColumnStyles[columna].SizeType == SizeType.Absolute)
                    tabla.ColumnStyles[columna].SizeType = SizeType.AutoSize;
            }
        }

        if (soloContenido)
        {
            tabla.AutoSize = true;
            tabla.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        // ComboBox no admite MinimumSize y, antes de crear su identificador,
        // su tamaño preferido puede ser menor que su altura nativa real.
        foreach (var combo in tabla.Controls.OfType<ComboBox>())
        {
            int fila = tabla.GetPositionFromControl(combo).Row;
            if (tabla.GetRowSpan(combo) != 1 || fila < 0 || fila >= tabla.RowStyles.Count) continue;
            if (tabla.RowStyles[fila] is { SizeType: SizeType.Absolute, Height: 0 }) continue;
            int alto = tabla.Controls.Cast<Control>()
                .Where(c => tabla.GetPositionFromControl(c).Row == fila && tabla.GetRowSpan(c) == 1 && EsContenido(c))
                .Select(c => (c is ComboBox lista ? Math.Max(lista.Height, lista.PreferredHeight) : c.PreferredSize.Height) + c.Margin.Vertical)
                .DefaultIfEmpty().Max();
            tabla.RowStyles[fila].SizeType = SizeType.Absolute;
            tabla.RowStyles[fila].Height = alto;
        }
    }
}
