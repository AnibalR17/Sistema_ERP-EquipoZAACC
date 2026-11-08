using System.Drawing.Drawing2D;
using System.Data;

namespace Sistema_ERP;

public sealed partial class CargaMasiva : PantallaMaqueta
{
    private DataGridView dgvPrevia = new();
    private Button btnImportar = new();
    private Label lblResumen = new();
    private readonly ComboBox cmbTiendaImportacion = new() { Name = "tiendaStockInicial", DropDownStyle = ComboBoxStyle.DropDownList, Width = 300 };

    public CargaMasiva() : base("Carga masiva", "Importacion desde Excel", "Sube un .xlsx, revisa la previsualizacion (rojo = error) y resuelve duplicados antes de importar.")
    {
        var filas = Filas(120, 90, -100);

        var tarjetaArchivo = Tarjeta(120);
        var archivo = new Button
        {
            Dock = DockStyle.Fill,
            Text = "Arrastra tu archivo .xlsx aqui o haz clic para seleccionar",
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.Gray,
            BackColor = Color.White,
            AllowDrop = true,
            Cursor = Cursors.Hand
        };
        archivo.FlatAppearance.BorderSize = 0;
        archivo.Paint += (_, e) => {
            using var borde = new Pen(Color.LightGray, 2) { DashStyle = DashStyle.Dash };
            e.Graphics.DrawRectangle(borde, 2, 2, archivo.Width - 5, archivo.Height - 5);
        };

        void Seleccionar(string ruta)
        {
            if (Path.GetExtension(ruta).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                archivo.Text = Path.GetFileName(ruta);
                CargarArchivo(ruta);
            }
        }

        archivo.Click += (_, _) => {
            using var selector = new OpenFileDialog { Filter = "Libro de Excel (*.xlsx)|*.xlsx" };
            if (selector.ShowDialog(this) == DialogResult.OK) Seleccionar(selector.FileName);
        };
        archivo.DragEnter += (_, e) => {
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] rutas && rutas.Length == 1 && Path.GetExtension(rutas[0]).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                e.Effect = DragDropEffects.Copy;
        };
        archivo.DragDrop += (_, e) => {
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] rutas && rutas.Length == 1)
                Seleccionar(rutas[0]);
        };
        tarjetaArchivo.Controls.Add(archivo);

        var opciones = Filas(44, 40);
        var barra = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true };
        barra.Controls.Add(new Label { Text = "Tienda para stock inicial", AutoSize = true, Margin = new Padding(0, 8, 10, 0) });
        barra.Controls.Add(cmbTiendaImportacion);
        var plantilla = Boton("Descargar plantilla"); plantilla.AutoSize = true;
        plantilla.Click += (_, _) =>
        {
            using var guardar = new SaveFileDialog { Filter = "Libro de Excel (*.xlsx)|*.xlsx", FileName = "Plantilla_carga_masiva.xlsx" };
            if (guardar.ShowDialog(this) != DialogResult.OK) return;
            try { CargaMasivaPlantilla.Crear(guardar.FileName); MessageBox.Show(this, "Plantilla creada. Completa la primera hoja y conserva sus encabezados."); }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Plantilla", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        };
        barra.Controls.Add(plantilla); opciones.Controls.Add(barra, 0, 0);
        opciones.Controls.Add(Etiqueta("Primera hoja: CodigoBarras, Nombre, Precio, StockInicial, NombreCategoria, RTNProveedor. Códigos y RTN como texto.", 10), 0, 1);
        var tarjetaTabla = Tarjeta(300); tarjetaTabla.Dock = DockStyle.Fill;
        var layoutTabla = Filas(-100, 40, 40);

        dgvPrevia.Dock = DockStyle.Fill;
        layoutTabla.Controls.Add(dgvPrevia, 0, 0);

        lblResumen.Text = "";
        lblResumen.Dock = DockStyle.Fill;
        lblResumen.TextAlign = ContentAlignment.MiddleLeft;
        layoutTabla.Controls.Add(lblResumen, 0, 1);

        btnImportar.Text = "Importar datos válidos";
        ProductoEstilo.Boton(btnImportar, true);
        layoutTabla.Controls.Add(btnImportar, 0, 2);

        tarjetaTabla.Controls.Add(layoutTabla);

        filas.Controls.Add(tarjetaArchivo, 0, 0);
        filas.Controls.Add(opciones, 0, 1);
        filas.Controls.Add(tarjetaTabla, 0, 2);
        Cuerpo.Controls.Add(filas);

        ConfigurarPrevisualizacion();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (ModoDiseno.Activo(this)) return;
        try
        {
            cmbTiendaImportacion.DisplayMember = "nombre"; cmbTiendaImportacion.ValueMember = "id_tienda";
            cmbTiendaImportacion.DataSource = VentaDatos.Consultar("SELECT id_tienda,nombre FROM dbo.Tienda WHERE activo=1 ORDER BY nombre");
            cmbTiendaImportacion.SelectedIndex = -1;
        }
        catch (Exception ex) { lblResumen.Text = "No se pudieron cargar las tiendas. " + ex.Message; }
    }
}
