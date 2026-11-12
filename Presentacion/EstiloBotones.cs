using System.Runtime.CompilerServices;

namespace Sistema_ERP;

internal static class EstiloBotones
{
    internal enum Tipo { Principal, Secundario, Navegacion, NavegacionSeleccionada }

    private sealed class Colores
    {
        internal Tipo Tipo;
    }

    private static readonly ConditionalWeakTable<Button, Colores> estilos = new();

    internal static bool EstaAplicado(Button boton) => estilos.TryGetValue(boton, out _);

    internal static void Aplicar(Button boton, Tipo tipo)
    {
        if (!estilos.TryGetValue(boton, out var colores))
        {
            colores = new Colores();
            estilos.Add(boton, colores);
            boton.EnabledChanged += ActualizarEstado;
            boton.Paint += PintarDeshabilitado;
        }
        colores.Tipo = tipo;
        boton.FlatStyle = FlatStyle.Flat;
        boton.UseVisualStyleBackColor = false;
        ActualizarEstado(boton, EventArgs.Empty);
    }

    internal static void Actualizar(Button boton) => ActualizarEstado(boton, EventArgs.Empty);

    private static void ActualizarEstado(object? sender, EventArgs e)
    {
        if (sender is not Button boton || !estilos.TryGetValue(boton, out var colores)) return;
        bool principal = colores.Tipo == Tipo.Principal;
        bool seleccionado = colores.Tipo == Tipo.NavegacionSeleccionada;
        bool navegacion = colores.Tipo is Tipo.Navegacion or Tipo.NavegacionSeleccionada;
        Color fondo = principal ? ProductoEstilo.Acento : seleccionado ? ProductoEstilo.Seleccion
            : navegacion ? ProductoEstilo.Lateral : ProductoEstilo.Superficie;
        Color texto = principal ? Color.White : seleccionado ? ProductoEstilo.TextoSeleccion
            : navegacion || ProductoEstilo.Oscuro ? ProductoEstilo.Texto : ProductoEstilo.Acento;
        boton.BackColor = boton.Enabled ? fondo : ProductoEstilo.FondoDeshabilitado;
        boton.ForeColor = boton.Enabled ? texto : ProductoEstilo.TextoDeshabilitado;
        boton.FlatAppearance.BorderColor = ProductoEstilo.Borde;
        boton.FlatAppearance.BorderSize = boton.Enabled && (principal || navegacion) ? 0 : 1;
        boton.FlatAppearance.MouseOverBackColor = principal ? ProductoEstilo.AcentoHover : ProductoEstilo.Seleccion;
        boton.FlatAppearance.MouseDownBackColor = boton.FlatAppearance.MouseOverBackColor;
        boton.Cursor = boton.Enabled ? Cursors.Hand : Cursors.Default;
        boton.Invalidate();
    }

    private static void PintarDeshabilitado(object? sender, PaintEventArgs e)
    {
        if (sender is not Button { Enabled: false } boton) return;

        // WinForms ignora ForeColor al deshabilitar un botón. Dibuja su texto
        // explícitamente para conservar el contraste sin habilitar la acción.
        e.Graphics.Clear(ProductoEstilo.FondoDeshabilitado);
        ControlPaint.DrawBorder(e.Graphics, boton.ClientRectangle, ProductoEstilo.Borde, ButtonBorderStyle.Solid);
        var area = new Rectangle(boton.Padding.Left + 4, boton.Padding.Top + 3,
            Math.Max(0, boton.ClientSize.Width - boton.Padding.Horizontal - 8),
            Math.Max(0, boton.ClientSize.Height - boton.Padding.Vertical - 6));
        var formato = TextFormatFlags.WordBreak | TextFormatFlags.PreserveGraphicsClipping;
        formato |= boton.TextAlign switch
        {
            ContentAlignment.TopCenter or ContentAlignment.MiddleCenter or ContentAlignment.BottomCenter => TextFormatFlags.HorizontalCenter,
            ContentAlignment.TopRight or ContentAlignment.MiddleRight or ContentAlignment.BottomRight => TextFormatFlags.Right,
            _ => TextFormatFlags.Left
        };
        formato |= boton.TextAlign switch
        {
            ContentAlignment.MiddleLeft or ContentAlignment.MiddleCenter or ContentAlignment.MiddleRight => TextFormatFlags.VerticalCenter,
            ContentAlignment.BottomLeft or ContentAlignment.BottomCenter or ContentAlignment.BottomRight => TextFormatFlags.Bottom,
            _ => TextFormatFlags.Top
        };
        if (boton.AutoEllipsis) formato |= TextFormatFlags.EndEllipsis;
        formato |= boton.UseMnemonic ? TextFormatFlags.HidePrefix : TextFormatFlags.NoPrefix;
        if (boton.RightToLeft == RightToLeft.Yes) formato |= TextFormatFlags.RightToLeft;
        TextRenderer.DrawText(e.Graphics, boton.Text, boton.Font, area, ProductoEstilo.TextoDeshabilitado, formato);
    }
}
