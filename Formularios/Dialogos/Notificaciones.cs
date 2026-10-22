using System.Drawing.Drawing2D;

namespace Sistema_ERP
{
    public sealed class Notificaciones : DialogoMaqueta
    {
        public Notificaciones() : base("Notificaciones", 260)
        {
            var avisos = PantallaMaqueta.Filas(50, 50);
            foreach (var (texto, color, fila) in new[] { ("⚠ Alerta de stock", Color.DarkOrange, 0), ("● Inversion proxima a vencer", Color.Firebrick, 1) })
            {
                var aviso = new PanelDisenoInventario { Dock = DockStyle.Fill, MostrarSombra = false, Radio = 16, BackColor = ProductoEstilo.Suave, Padding = new Padding(16, 0, 8, 0), Margin = new Padding(0, 0, 0, 8) };
                aviso.Controls.Add(PantallaMaqueta.Etiqueta(texto)); aviso.Paint += (_, e) => { using var borde = new Pen(color, 4); e.Graphics.DrawLine(borde, 2, 10, 2, aviso.Height - 10); }; avisos.Controls.Add(aviso, 0, fila);
            }
            Contenido.Controls.Add(avisos, 0, 1);
            var acciones = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            var cerrar = PantallaMaqueta.Boton("Cerrar", true); cerrar.Width = 100; cerrar.DialogResult = DialogResult.Cancel;
            acciones.Controls.Add(cerrar); Contenido.Controls.Add(acciones, 0, 2); CancelButton = cerrar;
        }
    }
}
