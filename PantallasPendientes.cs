using System.Drawing.Drawing2D;

namespace Sistema_ERP
{
    // Pantallas de dise?o: sin consultas, escrituras, importaciones ni transacciones en la base de datos.
    public class PantallaMaqueta : Form
    {
        protected readonly TableLayoutPanel Pagina;
        protected readonly Panel Cuerpo;

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
            Text = texto, Dock = DockStyle.Fill, AutoSize = false,
            Font = new Font("Segoe UI", tamano, negrita ? FontStyle.Bold : FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft, Margin = Padding.Empty
        };

        internal static Button Boton(string texto, bool principal = false)
        {
            var boton = new Button { Text = texto, Margin = new Padding(0, 4, 10, 4) };
            ProductoEstilo.Boton(boton, principal); return boton;
        }

        internal static PanelDisenoInventario Tarjeta(int alto) => new()
        {
            Dock = DockStyle.Top, Height = alto, Padding = new Padding(28, 20, 28, 30), Margin = Padding.Empty
        };

        internal static TextBox Buscar(string texto = "Buscar...") => new()
        { PlaceholderText = texto, BorderStyle = BorderStyle.FixedSingle, Width = 300, Margin = new Padding(0, 10, 0, 0) };

        internal static DataGridView Tabla(params string[] columnas)
        {
            var tabla = new DataGridView
            {
                Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None,
                ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false, ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal, GridColor = ProductoEstilo.Suave,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, ColumnHeadersHeight = 36
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

    public sealed class CargaMasiva : PantallaMaqueta
    {
        public CargaMasiva() : base("Carga masiva", "Importación desde Excel", "Sube un .xlsx, revisa la previsualización (rojo = error) y resuelve duplicados antes de importar.")
        {
            var tarjeta = Tarjeta(190);
            var archivo = new Button { Dock = DockStyle.Fill, Text = "Arrastra tu archivo .xlsx aquí o haz clic para seleccionar", FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray, BackColor = Color.White, AllowDrop = true, Cursor = Cursors.Hand };
            archivo.FlatAppearance.BorderSize = 0;
            archivo.Paint += (_, e) => { using var borde = new Pen(Color.LightGray, 2) { DashStyle = DashStyle.Dash }; e.Graphics.DrawRectangle(borde, 2, 2, archivo.Width - 5, archivo.Height - 5); };
            void Seleccionar(string ruta) { if (Path.GetExtension(ruta).Equals(".xlsx", StringComparison.OrdinalIgnoreCase)) archivo.Text = Path.GetFileName(ruta); }
            archivo.Click += (_, _) => { using var selector = new OpenFileDialog { Filter = "Libro de Excel (*.xlsx)|*.xlsx" }; if (selector.ShowDialog(this) == DialogResult.OK) Seleccionar(selector.FileName); };
            archivo.DragEnter += (_, e) => { if (e.Data?.GetData(DataFormats.FileDrop) is string[] rutas && rutas.Length == 1 && Path.GetExtension(rutas[0]).Equals(".xlsx", StringComparison.OrdinalIgnoreCase)) e.Effect = DragDropEffects.Copy; };
            archivo.DragDrop += (_, e) => { if (e.Data?.GetData(DataFormats.FileDrop) is string[] rutas && rutas.Length == 1) Seleccionar(rutas[0]); };
            tarjeta.Controls.Add(archivo); Cuerpo.Controls.Add(tarjeta);
        }
    }

    public sealed class PuntoVenta : PantallaMaqueta
    {
        public PuntoVenta() : base("Punto de Venta", "Facturación", "Busca un producto y haz clic para agregarlo al carrito. Luego elige cliente, método de pago y procesa.")
        {
            var columnas = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var izquierda = Filas(108, 140, -100);
            var busqueda = Tarjeta(108); var buscar = Buscar("Buscar por nombre o código de barras..."); buscar.Dock = DockStyle.Top; busqueda.Controls.Add(buscar);
            var carrito = Tarjeta(140); var vacio = Filas(32, -100); vacio.Controls.Add(Etiqueta("Carrito"), 0, 0); var texto = Etiqueta("El carrito está vacío."); texto.ForeColor = Color.Gray; vacio.Controls.Add(texto, 0, 1); carrito.Controls.Add(vacio);
            izquierda.Controls.Add(busqueda, 0, 0); izquierda.Controls.Add(carrito, 0, 1);
            var resumen = Tarjeta(410); resumen.Padding = new Padding(24, 16, 24, 28);
            var filas = Filas(28, 40, 32, 30, 40, 26, 40, 26, 36, 54);
            ComboBox Opciones(params string[] valores) { var combo = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList }; combo.Items.AddRange(valores); combo.SelectedIndex = 0; return combo; }
            filas.Controls.Add(Etiqueta("Cliente"), 0, 0); filas.Controls.Add(Opciones("Cliente general"), 0, 1);
            filas.Controls.Add(Etiqueta("Resumen"), 0, 2); filas.Controls.Add(Etiqueta("Subtotal                    L. 0.00", 11, true), 0, 3);
            filas.Controls.Add(Etiqueta("Total                 L. 0.00", 16, true), 0, 4);
            filas.Controls.Add(Etiqueta("Método de pago", 10), 0, 5); filas.Controls.Add(Opciones("Efectivo", "Tarjeta", "Transferencia"), 0, 6);
            filas.Controls.Add(Etiqueta("Monto recibido", 10), 0, 7);
            filas.Controls.Add(new NumericUpDown { Dock = DockStyle.Top, DecimalPlaces = 2, Maximum = 1000000, ThousandsSeparator = true }, 0, 8);
            var procesar = Boton("Procesar Venta", true); procesar.Name = "procesarVenta"; procesar.Dock = DockStyle.Fill;
            procesar.Click += (_, _) => DialogosMaqueta.Mostrar(FindForm()?.MdiParent ?? this, new ConfirmarVenta());
            filas.Controls.Add(procesar, 0, 9); resumen.Controls.Add(filas);
            columnas.Controls.Add(izquierda, 0, 0); columnas.Controls.Add(resumen, 1, 0); Cuerpo.Controls.Add(columnas);
        }
    }

    public sealed class DirectorioRoles : PantallaMaqueta
    {
        public DirectorioRoles() : base("Directorio y roles", "Clientes, proveedores y empleados", "El rol del empleado define qué módulos puede ver. Cambia de rol arriba para probarlo.")
        {
            var filas = Filas(52, 190, -100);
            var tabs = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            var tarjeta = Tarjeta(190); var contenido = Filas(54, -100, 24);
            var acciones = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            var agregar = Boton("+ Agregar", true); agregar.Width = 125; acciones.Controls.Add(agregar); acciones.Controls.Add(Buscar());
            var tabla = Tabla("Nombre", "Teléfono", "Correo");
            contenido.Controls.Add(acciones, 0, 0); contenido.Controls.Add(tabla, 0, 1); contenido.Controls.Add(Etiqueta("Mostrando 0 registros", 9), 0, 2);
            foreach (var nombre in new[] { "Clientes", "Proveedores", "Empleados" })
            {
                var tab = Boton(nombre, nombre == "Clientes"); tab.Width = 130;
                tab.Click += (_, _) => {
                    foreach (Button otro in tabs.Controls) { otro.BackColor = otro == tab ? ProductoEstilo.Texto : ProductoEstilo.Suave; otro.ForeColor = otro == tab ? Color.White : ProductoEstilo.Texto; }
                    tabla.Columns.Clear(); foreach (var campo in nombre == "Empleados" ? new[] { "Nombre", "Teléfono", "Correo", "Rol" } : new[] { "Nombre", "Teléfono", "Correo" }) tabla.Columns.Add(campo, campo);
                };
                tabs.Controls.Add(tab);
            }
            tarjeta.Controls.Add(contenido); filas.Controls.Add(tabs, 0, 0); filas.Controls.Add(tarjeta, 0, 1); Cuerpo.Controls.Add(filas);
            // La captura y el almacenamiento de datos quedan fuera del alcance de esta pantalla de dise?o.
            agregar.Enabled = false;
            agregar.BackColor = ProductoEstilo.Suave;
        }
    }

    public sealed class ConfiguracionAuditoria : PantallaMaqueta
    {
        public ConfiguracionAuditoria() : base("Configuración y auditoría", "Bitácora del sistema", "Registro de acciones: quién, cuándo y qué módulo.")
        {
            var tarjeta = Tarjeta(165); var filas = Filas(44, -100, 24);
            var buscar = Buscar(); buscar.Dock = DockStyle.Left;
            filas.Controls.Add(buscar, 0, 0); filas.Controls.Add(Tabla("Fecha/Hora", "Usuario", "Módulo", "Descripción"), 0, 1); filas.Controls.Add(Etiqueta("Mostrando 0 registros", 9), 0, 2);
            tarjeta.Controls.Add(filas); Cuerpo.Controls.Add(tarjeta);
        }
    }

    public sealed class ApartadosDevoluciones : PantallaMaqueta
    {
        public ApartadosDevoluciones() : base("Apartados y devoluciones", "Seguimiento de apartados y devoluciones", "Consulta los apartados y las devoluciones registradas.")
        {
            var tarjeta = Tarjeta(210); var filas = Filas(52, -100, 24);
            var tabs = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            var tabla = Tabla("Cliente", "Producto", "Fecha", "Estado");
            foreach (var nombre in new[] { "Apartados", "Devoluciones" }) {
                var tab = Boton(nombre, nombre == "Apartados");
                tab.Click += (_, _) => { foreach (Button otro in tabs.Controls) { otro.BackColor = otro == tab ? ProductoEstilo.Texto : ProductoEstilo.Suave; otro.ForeColor = otro == tab ? Color.White : ProductoEstilo.Texto; } };
                tabs.Controls.Add(tab);
            }
            filas.Controls.Add(tabs, 0, 0); filas.Controls.Add(tabla, 0, 1); filas.Controls.Add(Etiqueta("Mostrando 0 registros", 9), 0, 2); tarjeta.Controls.Add(filas); Cuerpo.Controls.Add(tarjeta);
        }
    }

    internal static class DialogosMaqueta
    {
        internal static void Mostrar(Form propietario, Form dialogo)
        {
            using (dialogo)
            using (var sombra = new Form { FormBorderStyle = FormBorderStyle.None, StartPosition = FormStartPosition.Manual, Bounds = propietario.Bounds, BackColor = Color.Black, Opacity = .4, ShowInTaskbar = false })
            {
                sombra.Show(propietario);
                try { dialogo.ShowDialog(sombra); }
                finally { sombra.Close(); }
            }
        }
    }

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
            CancelButton = cerrar; // La tecla Escape tambi?n cierra los di?logos sin bordes.
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            using var path = new GraphicsPath(); int d = 48;
            path.AddArc(0, 0, d, d, 180, 90); path.AddArc(Width-d, 0, d, d, 270, 90); path.AddArc(Width-d, Height-d, d, d, 0, 90); path.AddArc(0, Height-d, d, d, 90, 90); path.CloseFigure();
            Region = new Region(path);
        }
    }

    public sealed class ConfirmarVenta : DialogoMaqueta
    {
        public ConfirmarVenta() : base("¿Procesar venta?", 210)
        {
            Contenido.Controls.Add(PantallaMaqueta.Etiqueta("Total: L. 0.00\nCambio: L. 0.00"), 0, 1);
            var acciones = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            var confirmar = PantallaMaqueta.Boton("Confirmar", true); confirmar.Width = 125; confirmar.DialogResult = DialogResult.OK;
            var cancelar = PantallaMaqueta.Boton("Cancelar"); cancelar.Width = 115; cancelar.DialogResult = DialogResult.Cancel;
            acciones.Controls.Add(confirmar); acciones.Controls.Add(cancelar); Contenido.Controls.Add(acciones, 0, 2); CancelButton = cancelar;
        }
    }

    public sealed class Notificaciones : DialogoMaqueta
    {
        public Notificaciones() : base("Notificaciones", 260)
        {
            var avisos = PantallaMaqueta.Filas(50, 50);
            foreach (var (texto, color, fila) in new[] { ("⚠ Alerta de stock", Color.DarkOrange, 0), ("● Inversión próxima a vencer", Color.Firebrick, 1) })
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
