using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Sistema_ERP
{
    // Visual surface only: rounded corners and the soft card shadow in the mockups.
    public class PanelDisenoInventario : Panel
    {
        private int radio = 30;
        private bool mostrarSombra = true;
        private Color colorBorde = Color.Empty;

        public PanelDisenoInventario()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = Color.White;
        }

        [DefaultValue(30)]
        public int Radio
        {
            get => radio;
            set { radio = Math.Max(1, value); Invalidate(); }
        }

        [DefaultValue(true)]
        public bool MostrarSombra
        {
            get => mostrarSombra;
            set { mostrarSombra = value; Invalidate(); }
        }

        [DefaultValue(typeof(Color), "Empty")]
        public Color ColorBorde
        {
            get => colorBorde;
            set { colorBorde = value; Invalidate(); }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(Parent?.BackColor ?? Color.White);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 3 || Height < 3) return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var bounds = MostrarSombra
                ? new Rectangle(8, 2, Width - 16, Height - 18)
                : new Rectangle(0, 0, Width - 1, Height - 1);
            if (bounds.Width < 1 || bounds.Height < 1) return;
            if (MostrarSombra)
            {
                for (int spread = 10; spread >= 1; spread--)
                {
                    var shadow = bounds;
                    shadow.Offset(0, 7);
                    shadow.Inflate(spread / 2, spread / 2);
                    using var shadowPath = Rounded(shadow, Radio);
                    using var brush = new SolidBrush(Color.FromArgb(3, 0, 0, 0));
                    e.Graphics.FillPath(brush, shadowPath);
                }
            }
            using var path = Rounded(bounds, Radio);
            using var fill = new SolidBrush(BackColor);
            e.Graphics.FillPath(fill, path);
            if (!ColorBorde.IsEmpty)
            {
                using var borde = new Pen(ColorBorde);
                e.Graphics.DrawPath(borde, path);
            }
            base.OnPaint(e);
        }

        private static GraphicsPath Rounded(Rectangle bounds, int radius)
        {
            int diameter = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));
            var path = new GraphicsPath();
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
