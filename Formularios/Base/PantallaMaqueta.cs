using System.Drawing.Drawing2D;

namespace Sistema_ERP
{
    public class PantallaMaqueta : Form
    {
        protected readonly TableLayoutPanel Pagina;
        protected readonly Panel Cuerpo;

        public PantallaMaqueta() : this("Pantalla", "", "") { }

        protected PantallaMaqueta(string titulo, string subtitulo, string aviso)
        {
            Text = titulo;
            Font = new Font("Segoe UI", 11F);
            ForeColor = ProductoEstilo.Texto;
            BackColor = ProductoEstilo.Fondo;
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            Padding = new Padding(32, 24, 32, 28);
            Pagina = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, MinimumSize = new Size(680, 600) };
            Pagina.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            foreach (var alto in new[] { 58F, 36F, 62F }) Pagina.RowStyles.Add(new RowStyle(SizeType.Absolute, alto));
            Pagina.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Pagina.Controls.Add(Etiqueta(titulo, 27, true), 0, 0);
            var descripcion = Etiqueta(subtitulo); descripcion.ForeColor = Color.Gray;
            Pagina.Controls.Add(descripcion, 0, 1);
            var info = new PanelDisenoInventario { Dock = DockStyle.Fill, Radio = 18, MostrarSombra = false, BackColor = ProductoEstilo.Suave, Padding = new Padding(16, 0, 12, 0), Margin = new Padding(0, 6, 0, 14) };
            var mensaje = Etiqueta("ℹ " + aviso, 10); mensaje.ForeColor = Color.Gray;
            info.Controls.Add(mensaje); Pagina.Controls.Add(info, 0, 2);
            Cuerpo = new Panel { Dock = DockStyle.Fill, Margin = Padding.Empty };
            Pagina.Controls.Add(Cuerpo, 0, 3); Controls.Add(Pagina);
        }

        internal static Label Etiqueta(string texto, float tamano = 11, bool negrita = false) => new()
        {
            Text = texto,
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font("Segoe UI", tamano, negrita ? FontStyle.Bold : FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = Padding.Empty
        };

        internal static Button Boton(string texto, bool principal = false)
        {
            var boton = new Button { Text = texto, Margin = new Padding(0, 4, 10, 4) };
            ProductoEstilo.Boton(boton, principal); return boton;
        }

        internal static PanelDisenoInventario Tarjeta(int alto) => new()
        {
            Dock = DockStyle.Top,
            Height = alto,
            Padding = new Padding(28, 20, 28, 30),
            Margin = Padding.Empty
        };

        internal static TextBox Buscar(string texto = "Buscar...") => new()
        { PlaceholderText = texto, BorderStyle = BorderStyle.FixedSingle, Width = 300, Margin = new Padding(0, 10, 0, 0) };

        internal static DataGridView Tabla(params string[] columnas)
        {
            var tabla = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = ProductoEstilo.Suave,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ColumnHeadersHeight = 36
            };
            tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.Gray;
            tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F);
            foreach (var columna in columnas) tabla.Columns.Add(columna, columna);
            return tabla;
        }

        internal static TableLayoutPanel Filas(params float[] alturas)
        {
            var tabla = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = alturas.Length, Margin = Padding.Empty };
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            foreach (float altura in alturas) tabla.RowStyles.Add(new RowStyle(altura < 0 ? SizeType.Percent : SizeType.Absolute, Math.Abs(altura)));
            return tabla;
        }
    }
}
