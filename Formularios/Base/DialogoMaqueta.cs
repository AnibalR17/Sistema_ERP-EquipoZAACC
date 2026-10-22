using System.Drawing.Drawing2D;

namespace Sistema_ERP
{
    public class DialogoMaqueta : Form
    {
        protected readonly TableLayoutPanel Contenido;
        protected DialogoMaqueta(string titulo, int alto)
        {
            Text = titulo; ClientSize = new Size(500, alto); Font = new Font("Segoe UI", 11F);
            FormBorderStyle = FormBorderStyle.None; StartPosition = FormStartPosition.CenterParent; ShowInTaskbar = false;
            BackColor = Color.White; Padding = new Padding(28);
            Contenido = PantallaMaqueta.Filas(40, -100, 54);
            Contenido.Controls.Add(PantallaMaqueta.Etiqueta(titulo, 16, true), 0, 0); Controls.Add(Contenido);
            var cerrar = new Button { Text = "Cerrar", DialogResult = DialogResult.Cancel };
            CancelButton = cerrar; // La tecla Escape tambien cierra los dialogos sin bordes.
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            using var path = new GraphicsPath(); int d = 48;
            path.AddArc(0, 0, d, d, 180, 90); path.AddArc(Width-d, 0, d, d, 270, 90); path.AddArc(Width-d, Height-d, d, d, 0, 90); path.AddArc(0, Height-d, d, d, 90, 90); path.CloseFigure();
            Region = new Region(path);
        }
    }
}
