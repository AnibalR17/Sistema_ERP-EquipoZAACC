using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sistema_ERP;

internal static class TemaAplicacion
{
    private enum FondoControl { Heredado, Transparente, Fondo, Superficie, Lateral, Suave, Campo, Borde, Original }
    private sealed class Preparacion(FondoControl fondo) { internal FondoControl Fondo = fondo; }
    private static readonly ConditionalWeakTable<Control, Preparacion> preparados = new();
    internal static TemaVisual Actual => ProductoEstilo.Oscuro ? TemaVisual.Oscuro : TemaVisual.Claro;

    internal static void Inicializar(string? ruta = null)
    {
        ProductoEstilo.Oscuro = PreferenciasApariencia.Cargar(ruta ?? PreferenciasApariencia.Ruta) == TemaVisual.Oscuro;
        AplicarModoWindows();
    }

    internal static void Cambiar(TemaVisual tema, bool guardar = true)
    {
        if (!Enum.IsDefined(tema)) throw new ArgumentOutOfRangeException(nameof(tema));
        // Mantiene la selección anterior si no se puede guardar la preferencia.
        if (guardar) PreferenciasApariencia.Guardar(PreferenciasApariencia.Ruta, tema);
        ProductoEstilo.Oscuro = tema == TemaVisual.Oscuro;
        AplicarModoWindows();
        var raices = preparados.Select(p => p.Key).Where(c => !c.IsDisposed && c.Parent is null)
            .Concat(Application.OpenForms.OfType<FormularioTematico>()).Distinct().ToArray();
        foreach (var raiz in raices)
        {
            raiz.SuspendLayout();
            try { Aplicar(raiz); }
            finally { raiz.ResumeLayout(false); raiz.Invalidate(true); }
        }
    }

    private static void AplicarModoWindows() =>
        Application.SetColorMode(ProductoEstilo.Oscuro ? SystemColorMode.Dark : SystemColorMode.Classic);

    internal static void Aplicar(Control control)
    {
        if (control.IsDisposed) return;
        if (!preparados.TryGetValue(control, out var preparacion))
        {
            preparacion = new Preparacion(ClasificarFondo(control));
            preparados.Add(control, preparacion);
            control.ControlAdded += (_, e) => { if (e.Control is not null) Aplicar(e.Control); };
            if (control is TextBoxBase entrada) entrada.ReadOnlyChanged += (_, _) => AplicarColores(entrada, preparacion);
            if (control is ComboBox combo)
            {
                combo.DrawMode = DrawMode.OwnerDrawFixed;
                combo.DrawItem += PintarOpcion;
            }
        }

        if (control is Button boton)
        {
            if (!EstiloBotones.EstaAplicado(boton))
            {
                var fondo = boton.BackColor;
                ProductoEstilo.ColoresBoton(boton, fondo.R < 70 && fondo.G < 70 && fondo.B < 70);
            }
            else EstiloBotones.Actualizar(boton);
        }
        else AplicarColores(control, preparacion);
        foreach (Control hijo in control.Controls) Aplicar(hijo);
        control.Invalidate();
    }

    private static FondoControl ClasificarFondo(Control control)
    {
        if (control is TextBoxBase or ComboBox or UpDownBase or DateTimePicker or ListBox) return FondoControl.Campo;
        if (control is Form or UserControl or MdiClient or TabControl or TabPage) return FondoControl.Fondo;
        if (control.Name is "header" or "panelContenedor" or "pnlIzquierdo") return FondoControl.Lateral;
        Color color = control.BackColor;
        if (color.A == 0) return FondoControl.Transparente;
        if (TypeDescriptor.GetProperties(control)[nameof(Control.BackColor)]?.ShouldSerializeValue(control) == false)
            return FondoControl.Heredado;
        int rgb = color.ToArgb() & 0xFFFFFF;
        if (rgb is 0xFFFFFF or 0x252526) return FondoControl.Superficie;
        if (rgb == 0x181818) return FondoControl.Lateral;
        if (rgb is 0xF4F7FB or 0x1F1F1F || color.R == color.G && color.G == color.B && color.R >= 250)
            return FondoControl.Fondo;
        if (rgb is 0xE8EEF5 or 0x2B2B2B || color.R == color.G && color.G == color.B && color.R >= 240)
            return FondoControl.Suave;
        if (color.R == color.G && color.G == color.B && color.R >= 180) return FondoControl.Borde;
        return FondoControl.Original;
    }

    private static void AplicarColores(Control control, Preparacion preparacion)
    {
        control.ForeColor = TextoNeutral(control.ForeColor);
        control.BackColor = preparacion.Fondo switch
        {
            FondoControl.Heredado => Color.Empty,
            FondoControl.Transparente => Color.Transparent,
            FondoControl.Fondo => ProductoEstilo.Fondo,
            FondoControl.Superficie => ProductoEstilo.Superficie,
            FondoControl.Lateral => ProductoEstilo.Lateral,
            FondoControl.Suave => ProductoEstilo.Suave,
            FondoControl.Borde => ProductoEstilo.Borde,
            FondoControl.Campo => control is TextBoxBase { ReadOnly: true } ? ProductoEstilo.Suave : ProductoEstilo.Campo,
            _ => control.BackColor
        };
        if (control is PanelDisenoInventario tarjeta) tarjeta.ColorBorde = ProductoEstilo.Borde;
        if (control is DataGridView tabla) AplicarTabla(tabla);
        if (control is ComboBox combo) combo.FlatStyle = FlatStyle.Flat;
        if (control is TabPage pagina) pagina.UseVisualStyleBackColor = false;
        if (control is DateTimePicker fecha)
        {
            fecha.CalendarMonthBackground = ProductoEstilo.Campo;
            fecha.CalendarForeColor = ProductoEstilo.Texto;
            fecha.CalendarTitleBackColor = ProductoEstilo.Suave;
            fecha.CalendarTitleForeColor = ProductoEstilo.Texto;
        }
    }

    private static Color TextoNeutral(Color color)
    {
        int rgb = color.ToArgb() & 0xFFFFFF;
        if (rgb is 0x006400 or 0x89D185) return ProductoEstilo.Exito;
        if (rgb is 0xB22222 or 0xF85149) return ProductoEstilo.Error;
        if (rgb is 0x1D4ED8 or 0x0078D4 or 0x4DAAFC) return ProductoEstilo.TextoAcento;
        if (rgb is 0x475569 or 0x9D9D9D || color.R == color.G && color.G == color.B && color.R is >= 90 and <= 180)
            return ProductoEstilo.TextoSecundario;
        if (rgb == 0x1F2937 || color.R == color.G && color.G == color.B) return ProductoEstilo.Texto;
        return color;
    }

    private static void AplicarTabla(DataGridView tabla)
    {
        tabla.EnableHeadersVisualStyles = false;
        tabla.BackgroundColor = ProductoEstilo.Superficie;
        tabla.GridColor = ProductoEstilo.Borde;
        tabla.ColumnHeadersDefaultCellStyle.BackColor = ProductoEstilo.Suave;
        tabla.ColumnHeadersDefaultCellStyle.ForeColor = ProductoEstilo.Texto;
        tabla.ColumnHeadersDefaultCellStyle.SelectionBackColor = ProductoEstilo.Suave;
        tabla.ColumnHeadersDefaultCellStyle.SelectionForeColor = ProductoEstilo.Texto;
        tabla.DefaultCellStyle.BackColor = ProductoEstilo.Superficie;
        tabla.DefaultCellStyle.ForeColor = ProductoEstilo.Texto;
        tabla.DefaultCellStyle.SelectionBackColor = ProductoEstilo.Seleccion;
        tabla.DefaultCellStyle.SelectionForeColor = ProductoEstilo.TextoSeleccion;
        tabla.AlternatingRowsDefaultCellStyle.BackColor = ProductoEstilo.Fondo;
        tabla.AlternatingRowsDefaultCellStyle.ForeColor = ProductoEstilo.Texto;
        tabla.RowHeadersDefaultCellStyle.BackColor = ProductoEstilo.Suave;
        tabla.RowHeadersDefaultCellStyle.ForeColor = ProductoEstilo.Texto;
    }

    private static void PintarOpcion(object? sender, DrawItemEventArgs e)
    {
        if (sender is not ComboBox combo) return;
        bool seleccionada = (e.State & DrawItemState.Selected) != 0;
        Color fondo = seleccionada ? ProductoEstilo.Seleccion : ProductoEstilo.Campo;
        Color texto = !combo.Enabled ? ProductoEstilo.TextoDeshabilitado
            : seleccionada ? ProductoEstilo.TextoSeleccion : ProductoEstilo.Texto;
        using var pincel = new SolidBrush(fondo);
        e.Graphics.FillRectangle(pincel, e.Bounds);
        string valor = (e.Index >= 0 && e.Index < combo.Items.Count ? combo.GetItemText(combo.Items[e.Index]) : combo.Text) ?? "";
        var formato = TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;
        if (combo.RightToLeft == RightToLeft.Yes) formato |= TextFormatFlags.RightToLeft | TextFormatFlags.Right;
        TextRenderer.DrawText(e.Graphics, valor, e.Font ?? combo.Font, e.Bounds, texto, formato);
        if ((e.State & DrawItemState.Focus) != 0) ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds, texto, fondo);
    }

}
