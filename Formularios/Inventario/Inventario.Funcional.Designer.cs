#nullable enable
namespace Sistema_ERP
{
    public partial class Inventario
    {
        private TableLayoutPanel contenidoInventario = null!;
        private ComboBox tiendaInventario = null!;
        private TextBox buscarInventario = null!;
        private DateTimePicker desdeInventario = null!, hastaInventario = null!;
        private Button consultarInventario = null!, guardarMinimo = null!;
        private NumericUpDown minimoInventario = null!;
        private Label estadoInventario = null!, resumenInventario = null!;
        private DataGridView existenciasInventario = null!, historialInventario = null!;
        private TabControl pestanasInventario = null!;

        private void InitializeInventarioFuncional()
        {
            contenidoInventario = new TableLayoutPanel();
            tiendaInventario = new ComboBox(); buscarInventario = new TextBox();
            desdeInventario = new DateTimePicker(); hastaInventario = new DateTimePicker();
            consultarInventario = new Button(); guardarMinimo = new Button();
            minimoInventario = new NumericUpDown(); estadoInventario = new Label(); resumenInventario = new Label();
            existenciasInventario = new DataGridView(); historialInventario = new DataGridView(); pestanasInventario = new TabControl();
            contenidoInventario.Dock = DockStyle.Fill;
            contenidoInventario.Padding = new Padding(32, 34, 32, 36);
            contenidoInventario.ColumnCount = 1; contenidoInventario.RowCount = 6;
            contenidoInventario.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 5; i++) contenidoInventario.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            contenidoInventario.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var tituloInventario = new Label { Text = "Inventario y movimientos", AutoSize = true, Font = new Font("Segoe UI", 27F, FontStyle.Bold), Margin = new Padding(0, 0, 0, 14) };
            contenidoInventario.Controls.Add(tituloInventario, 0, 0);
            var filtrosInventario = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            tiendaInventario.Width = 220; tiendaInventario.DropDownStyle = ComboBoxStyle.DropDownList;
            buscarInventario.Width = 250; buscarInventario.MaxLength = 150; buscarInventario.PlaceholderText = "Producto, código o movimiento...";
            desdeInventario.Format = hastaInventario.Format = DateTimePickerFormat.Short;
            desdeInventario.Width = hastaInventario.Width = 145;
            desdeInventario.Value = DateTime.UtcNow.Date.AddDays(-30); hastaInventario.Value = DateTime.UtcNow.Date;
            filtrosInventario.Controls.Add(TraspasoEstilo.Campo("Tienda *", tiendaInventario));
            filtrosInventario.Controls.Add(TraspasoEstilo.Campo("Buscar", buscarInventario));
            filtrosInventario.Controls.Add(TraspasoEstilo.Campo("Movimientos desde (UTC)", desdeInventario));
            filtrosInventario.Controls.Add(TraspasoEstilo.Campo("Hasta (UTC)", hastaInventario));
            consultarInventario.Text = "Consultar"; ProductoEstilo.Boton(consultarInventario); consultarInventario.Width = 110;
            filtrosInventario.Controls.Add(TraspasoEstilo.Campo("Actualizar datos", consultarInventario));
            contenidoInventario.Controls.Add(filtrosInventario, 0, 1);
            var accionesInventario = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            guardarMinimo.Text = "Guardar mínimo";
            ProductoEstilo.Boton(guardarMinimo);
            minimoInventario.Maximum = int.MaxValue; minimoInventario.Width = 130;
            accionesInventario.Controls.Add(TraspasoEstilo.Campo("Umbral de stock mínimo", minimoInventario));
            accionesInventario.Controls.Add(guardarMinimo);
            contenidoInventario.Controls.Add(accionesInventario, 0, 2);
            estadoInventario.AutoSize = true; estadoInventario.Margin = new Padding(0, 6, 0, 10);
            contenidoInventario.Controls.Add(estadoInventario, 0, 3);
            resumenInventario.AutoSize = true; resumenInventario.ForeColor = ProductoEstilo.TextoSecundario;
            resumenInventario.Margin = new Padding(0, 0, 0, 10);
            contenidoInventario.Controls.Add(resumenInventario, 0, 4);
            pestanasInventario.Dock = DockStyle.Fill;
            var paginaExistencias = new TabPage("Existencias"); var paginaMovimientos = new TabPage("Movimientos");
            foreach (var (pagina, grid) in new[] { (paginaExistencias, existenciasInventario), (paginaMovimientos, historialInventario) })
            {
                pagina.BackColor = Color.White; pagina.Padding = new Padding(8);
                grid.Dock = DockStyle.Fill; grid.ReadOnly = true; grid.AllowUserToAddRows = false;
                grid.AllowUserToDeleteRows = false; grid.MultiSelect = false; grid.RowHeadersVisible = false;
                grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                TraspasoEstilo.Tabla(grid);
                grid.RowTemplate.Height = 42; grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                var tarjetaInventario = new PanelDisenoInventario { Dock = DockStyle.Fill, Radio = 30, MostrarSombra = false, ColorBorde = Color.FromArgb(247, 247, 247), Padding = new Padding(16) };
                tarjetaInventario.Controls.Add(grid); pagina.Controls.Add(tarjetaInventario); pestanasInventario.TabPages.Add(pagina);
            }
            contenidoInventario.Controls.Add(pestanasInventario, 0, 5);
            content.Font = new Font("Segoe UI", 11F); content.BackColor = Color.White;
            content.Padding = Padding.Empty;
            content.Controls.Add(contenidoInventario);
        }
    }
}
