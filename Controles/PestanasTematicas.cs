namespace Sistema_ERP;

public class PestanasTematicas : TabControl
{
    public PestanasTematicas()
    {
        DrawMode = TabDrawMode.OwnerDrawFixed;
        Padding = new Point(16, 6);
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        if (e.Index >= 0 && e.Index < TabCount) PintarPestana(e.Graphics, e.Index);
        base.OnDrawItem(e);
    }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        // TabControl no utiliza BackColor para la franja vacía ni para su borde.
        // Pinta esas zonas también al capturarlo con DrawToBitmap (WM_PRINTCLIENT).
        if (Alignment != TabAlignment.Top || !IsHandleCreated || Width < 2 || Height < 2) return;
        if (m.Msg == 0x000F)
        {
            using var g = Graphics.FromHwnd(Handle);
            PintarMarco(g);
        }
        else if (m.Msg == 0x0318 && m.WParam != IntPtr.Zero)
        {
            using var g = Graphics.FromHdc(m.WParam);
            PintarMarco(g);
        }
    }

    private void PintarMarco(Graphics g)
    {
        Rectangle area = DisplayRectangle;
        using var fondo = new SolidBrush(ProductoEstilo.Lateral);
        g.FillRectangle(fondo, 0, 0, Width, Math.Max(0, area.Top));
        g.FillRectangle(fondo, 0, area.Top, area.Left, Math.Max(0, Height - area.Top));
        g.FillRectangle(fondo, area.Right, area.Top, Math.Max(0, Width - area.Right), Math.Max(0, Height - area.Top));
        g.FillRectangle(fondo, 0, area.Bottom, Width, Math.Max(0, Height - area.Bottom));
        using var borde = new Pen(ProductoEstilo.Borde);
        if (area.Width > 0 && area.Height > 0)
            g.DrawRectangle(borde, area.Left - 1, area.Top - 1, area.Width + 1, area.Height + 1);
        for (int i = 0; i < TabCount; i++) PintarPestana(g, i);
    }

    private void PintarPestana(Graphics g, int indice)
    {
        Rectangle bounds = GetTabRect(indice);
        bool seleccionada = indice == SelectedIndex;
        using var fondo = new SolidBrush(seleccionada ? ProductoEstilo.Fondo : ProductoEstilo.Lateral);
        g.FillRectangle(fondo, bounds);
        TextRenderer.DrawText(g, TabPages[indice].Text, Font, bounds,
            seleccionada ? ProductoEstilo.Texto : ProductoEstilo.TextoSecundario,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        if (seleccionada)
        {
            using var acento = new Pen(ProductoEstilo.Acento, 2);
            g.DrawLine(acento, bounds.Left, bounds.Top + 1, bounds.Right, bounds.Top + 1);
            if (Focused && ShowFocusCues) ControlPaint.DrawFocusRectangle(g, Rectangle.Inflate(bounds, -4, -4), ProductoEstilo.Texto, ProductoEstilo.Fondo);
        }
    }
}
