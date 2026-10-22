using System.Drawing.Drawing2D;

namespace Sistema_ERP
{
    public sealed class CargaMasiva : PantallaMaqueta
    {
        public CargaMasiva() : base("Carga masiva", "Importacion desde Excel", "Sube un .xlsx, revisa la previsualizacion (rojo = error) y resuelve duplicados antes de importar.")
        {
            var tarjeta = Tarjeta(190);
            var archivo = new Button { Dock = DockStyle.Fill, Text = "Arrastra tu archivo .xlsx aqui o haz clic para seleccionar", FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray, BackColor = Color.White, AllowDrop = true, Cursor = Cursors.Hand };
            archivo.FlatAppearance.BorderSize = 0;
            archivo.Paint += (_, e) => { using var borde = new Pen(Color.LightGray, 2) { DashStyle = DashStyle.Dash }; e.Graphics.DrawRectangle(borde, 2, 2, archivo.Width - 5, archivo.Height - 5); };
            void Seleccionar(string ruta) { if (Path.GetExtension(ruta).Equals(".xlsx", StringComparison.OrdinalIgnoreCase)) archivo.Text = Path.GetFileName(ruta); }
            archivo.Click += (_, _) => { using var selector = new OpenFileDialog { Filter = "Libro de Excel (*.xlsx)|*.xlsx" }; if (selector.ShowDialog(this) == DialogResult.OK) Seleccionar(selector.FileName); };
            archivo.DragEnter += (_, e) => { if (e.Data?.GetData(DataFormats.FileDrop) is string[] rutas && rutas.Length == 1 && Path.GetExtension(rutas[0]).Equals(".xlsx", StringComparison.OrdinalIgnoreCase)) e.Effect = DragDropEffects.Copy; };
            archivo.DragDrop += (_, e) => { if (e.Data?.GetData(DataFormats.FileDrop) is string[] rutas && rutas.Length == 1) Seleccionar(rutas[0]); };
            tarjeta.Controls.Add(archivo); Cuerpo.Controls.Add(tarjeta);
        }
    }
}
