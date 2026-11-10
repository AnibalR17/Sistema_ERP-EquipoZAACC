using System.Drawing.Drawing2D;
using System.Data;

namespace Sistema_ERP;

public sealed partial class CargaMasiva : PantallaMaqueta
{
    private const string TextoSeleccionArchivo = "Arrastra tu archivo .xlsx aqui o haz clic para seleccionar";
    private DataGridView dgvPrevia = new();
    private Button btnImportar = new();
    private readonly Button btnCancelarCarga = new() { Name = "cancelarCarga", Text = "Cancelar carga", Enabled = false };
    private readonly Button archivo = new()
    {
        Name = "seleccionarArchivoCarga",
        Dock = DockStyle.Fill,
        Text = TextoSeleccionArchivo,
        FlatStyle = FlatStyle.Flat,
        ForeColor = Color.Gray,
        BackColor = Color.White,
        AllowDrop = true,
        Cursor = Cursors.Hand
    };
    private Label lblResumen = new();
    private readonly TabControl pestanasCarga = new() { Name = "pestanasCargaMasiva", Dock = DockStyle.Fill };
    private readonly TabPage tabEjemplo = new("Ejemplo de plantilla") { Padding = new Padding(12), BackColor = Color.White };
    private readonly TabPage tabPrevisualizacion = new("Vista previa del archivo") { BackColor = Color.White };
    private readonly ComboBox cmbTiendaImportacion = new() { Name = "tiendaStockInicial", DropDownStyle = ComboBoxStyle.DropDownList, Width = 300 };

    public CargaMasiva() : base("Carga masiva", "Importacion desde Excel", "Sube un .xlsx, revisa la previsualizacion (rojo = error) y resuelve duplicados antes de importar.")
    {
        var filas = Filas(120, 90, -100);

        var tarjetaArchivo = Tarjeta(120);
        archivo.FlatAppearance.BorderSize = 0;
        archivo.Paint += (_, e) => {
            using var borde = new Pen(Color.LightGray, 2) { DashStyle = DashStyle.Dash };
            e.Graphics.DrawRectangle(borde, 2, 2, archivo.Width - 5, archivo.Height - 5);
        };

        void Seleccionar(string ruta)
        {
            if (Path.GetExtension(ruta).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
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
        RecargaManual.Agregar(barra, () =>
        {
            CargarTiendas();
            if (rutaOriginal is string ruta) CargarArchivo(ruta);
        });
        opciones.Controls.Add(Etiqueta("Consulta el ejemplo de plantilla. Usa un .xlsx con los encabezados en la primera fila de la primera hoja.", 10), 0, 1);
        var tarjetaTabla = Tarjeta(300); tarjetaTabla.Dock = DockStyle.Fill;
        var layoutTabla = Filas(-100, 40, 48);

        dgvPrevia.Dock = DockStyle.Fill;
        layoutTabla.Controls.Add(dgvPrevia, 0, 0);

        lblResumen.Text = "";
        lblResumen.Dock = DockStyle.Fill;
        lblResumen.TextAlign = ContentAlignment.MiddleLeft;
        layoutTabla.Controls.Add(lblResumen, 0, 1);

        btnImportar.Text = "Importar datos válidos";
        ProductoEstilo.Boton(btnImportar, true);
        ProductoEstilo.Boton(btnCancelarCarga);
        btnImportar.Dock = btnCancelarCarga.Dock = DockStyle.Fill;
        btnCancelarCarga.Click += (_, _) =>
        {
            ReiniciarPrevisualizacion();
            lblResumen.Text = "Carga cancelada. Selecciona otro archivo para continuar.";
        };
        var accionesCarga = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = Padding.Empty };
        accionesCarga.ColumnStyles.Add(new(SizeType.Percent, 100)); accionesCarga.ColumnStyles.Add(new(SizeType.Absolute, 180));
        accionesCarga.RowStyles.Add(new(SizeType.Percent, 100));
        accionesCarga.Controls.Add(btnImportar, 0, 0); accionesCarga.Controls.Add(btnCancelarCarga, 1, 0);
        layoutTabla.Controls.Add(accionesCarga, 0, 2);

        tarjetaTabla.Controls.Add(layoutTabla);

        tabPrevisualizacion.Controls.Add(tarjetaTabla);
        pestanasCarga.TabPages.AddRange([tabEjemplo, tabPrevisualizacion]);
        filas.Controls.Add(tarjetaArchivo, 0, 0);
        filas.Controls.Add(opciones, 0, 1);
        filas.Controls.Add(pestanasCarga, 0, 2);
        Cuerpo.Controls.Add(filas);

        ConfigurarPrevisualizacion();
        PermisosInterfaz.Configurar(this, Modulos.CargaMasiva, btnImportar);
        MostrarEjemploPlantilla();
    }

    private void MostrarEjemploPlantilla()
    {
        var contenido = Filas(62, -100, 44);
        var descripcion = Etiqueta("Ejemplo del formato de Excel. Completa NombreCategoria y RTNProveedor con los datos de tu base. Puedes copiar las filas o usar Descargar plantilla.", 10);
        var ejemplo = Tabla(); ejemplo.Name = "tablaEjemploPlantilla";
        ejemplo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        ejemplo.ScrollBars = ScrollBars.Both;
        ejemplo.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
        var ayuda = Etiqueta("Precio: número con hasta dos decimales. StockInicial: entero no negativo. CodigoBarras y RTNProveedor: texto.", 10);
        contenido.Controls.Add(descripcion, 0, 0); contenido.Controls.Add(ejemplo, 0, 1); contenido.Controls.Add(ayuda, 0, 2);
        tabEjemplo.Controls.Add(contenido);
        try
        {
            ejemplo.DataSource = CargaMasivaPlantilla.LeerEjemplo();
            ejemplo.Columns["Precio"]!.DefaultCellStyle.Format = "0.00";
            foreach (DataGridViewColumn columna in ejemplo.Columns) columna.MinimumWidth = 115;
        }
        catch (Exception ex) { descripcion.Text = "No se pudo mostrar el ejemplo. " + ex.Message; }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        CargarTiendas();
    }

    private void CargarTiendas()
    {
        if (ModoDiseno.Activo(this)) return;
        try
        {
            object? anterior = cmbTiendaImportacion.SelectedValue;
            cmbTiendaImportacion.DisplayMember = "nombre"; cmbTiendaImportacion.ValueMember = "id_tienda";
            cmbTiendaImportacion.DataSource = VentaDatos.Consultar("SELECT id_tienda,nombre FROM dbo.Tienda WHERE activo=1 ORDER BY nombre", modulo: Modulos.CargaMasiva);
            cmbTiendaImportacion.SelectedIndex = -1;
            if (anterior is int id) cmbTiendaImportacion.SelectedValue = id;
        }
        catch (Exception ex) { lblResumen.Text = "No se pudieron cargar las tiendas. " + ex.Message; }
    }
}
