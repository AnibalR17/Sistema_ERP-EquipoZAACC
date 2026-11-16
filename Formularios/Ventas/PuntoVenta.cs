using System.Data;

namespace Sistema_ERP;

public sealed class PuntoVenta : PantallaMaqueta
{
    private readonly ComboBox tienda = Combo(), cliente = Combo(), metodo = Combo();
    private readonly ComboBox tasaIsv = new() { Name = "tasaISV", DropDownStyle = ComboBoxStyle.DropDownList, Width = 120, AccessibleName = "ISV del producto que se agregará" };
    private readonly TextBox buscar = Buscar("Buscar por cualquier dato del producto"), referencia = new() { Name = "referenciaPago", MaxLength = 100 }, observaciones = new() { MaxLength = 500 };
    private readonly TextBox rtnCliente = new() { Name = "rtnCliente", MaxLength = 30, PlaceholderText = "14 dígitos (opcional)", AccessibleName = "RTN del cliente (opcional)" };
    private readonly NumericUpDown cantidad = new() { Minimum = 1, Maximum = 100000, Value = 1 }, recibido = new() { DecimalPlaces = 2, Maximum = 9999999999999999.99m, ThousandsSeparator = true };
    private readonly DataGridView productos = Tabla(), carrito = Tabla();
    private readonly Label resumen = Etiqueta("Total: L 0.00", 12, true), estado = Etiqueta("");
    private readonly Button procesar = Boton("Emitir factura", true);
    private readonly Button refrescarStock = Boton("Refrescar stock");
    private readonly Button exportarPdf = Boton("Exportar recibo PDF");
    private DataTable? catalogo;
    private readonly DataTable detalle = new();
    private FacturaEmitida? ultima;
    private string comprobante = "";
    private bool refrescarAlActivar;
    private bool cargandoCatalogos;

    public PuntoVenta() : base("Facturación de ventas", "Productos, cliente y pago", "Selecciona una tienda, agrega productos y confirma la factura para registrar el pago y descontar inventario.")
    {
        var columnas = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
        columnas.ColumnStyles.Add(new(SizeType.Percent, 100)); columnas.ColumnStyles.Add(new(SizeType.Absolute, 310));
        var izquierda = Filas(48, -50, 44, -50, 54);
        var busqueda = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = Padding.Empty };
        busqueda.ColumnStyles.Add(new(SizeType.Percent, 100)); busqueda.ColumnStyles.Add(new(SizeType.Absolute, 180));
        busqueda.RowStyles.Add(new(SizeType.Percent, 100));
        buscar.Dock = DockStyle.Fill; buscar.Margin = new Padding(0, 4, 10, 4);
        refrescarStock.Name = "refrescarStock"; refrescarStock.Dock = DockStyle.Fill; refrescarStock.Margin = new Padding(0, 2, 0, 2);
        refrescarStock.AccessibleDescription = "Carga desde la base de datos todo el stock de la tienda seleccionada y limpia la búsqueda.";
        busqueda.Controls.Add(buscar, 0, 0); busqueda.Controls.Add(refrescarStock, 1, 0);
        izquierda.Controls.Add(busqueda, 0, 0); izquierda.Controls.Add(productos, 0, 1);
        var acciones = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, WrapContents = false };
        var agregar = Boton("Agregar"); var quitar = Boton("Quitar"); var vaciar = Boton("Vaciar");
        PermisosInterfaz.Configurar(this, Modulos.ventas, procesar, agregar, quitar, vaciar);
        cantidad.Width = 70; tasaIsv.Items.AddRange(["Exento (0%)", "ISV 15%", "ISV 18%"]); tasaIsv.SelectedIndex = 1;
        acciones.Controls.AddRange([cantidad, tasaIsv, agregar, quitar, vaciar]); izquierda.Controls.Add(acciones, 0, 2);
        izquierda.Controls.Add(BusquedaTabla.Crear(carrito), 0, 3); izquierda.Controls.Add(estado, 0, 4);
        var lateral = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(12, 0, 0, 0) };
        var campos = Filas(26, 36, 26, 36, 26, 36, 26, 36, 26, 36, 26, 36, 26, 36, 112, 42, 42, 42, 42, 42);
        campos.Dock = DockStyle.Top; campos.Height = 774;
        Control[] controles = [Etiqueta("Tienda"), tienda, Etiqueta("Cliente"), cliente, Etiqueta("RTN del cliente (opcional)"), rtnCliente, Etiqueta("Método de pago"), metodo,
            Etiqueta("Monto recibido (efectivo)"), recibido, Etiqueta("Referencia del pago"), referencia, Etiqueta("Observaciones"), observaciones, resumen, procesar];
        for (int i = 0; i < controles.Length; i++) { controles[i].Dock = DockStyle.Fill; campos.Controls.Add(controles[i], 0, i); }
        var recargar = Boton("Actualizar catálogos"); var imprimir = Boton("Ver / imprimir última factura");
        RecargaManual.Preparar(recargar); recargar.Name = "btnReiniciar";
        recargar.Dock = imprimir.Dock = DockStyle.Fill; campos.Controls.Add(recargar, 0, 16); campos.Controls.Add(imprimir, 0, 17);
        exportarPdf.Name = "exportarReciboPDF"; exportarPdf.Enabled = false; exportarPdf.Dock = DockStyle.Fill; campos.Controls.Add(exportarPdf, 0, 18);
        exportarPdf.Click += async (_, _) => { if (ultima is { } f) await DialogosExportacion.ReciboPdf(this, f.Id, f.Numero); };
        var historial = Boton("Facturas registradas"); historial.Dock = DockStyle.Fill; campos.Controls.Add(historial, 0, 19);
        historial.Click += (_, _) => Historial();
        lateral.Controls.Add(campos); columnas.Controls.Add(izquierda, 0, 0); columnas.Controls.Add(lateral, 1, 0); cuerpo.Controls.Add(columnas);
        detalle.Columns.Add("Id", typeof(int)); detalle.Columns.Add("Producto", typeof(string)); detalle.Columns.Add("Cantidad", typeof(int)); detalle.Columns.Add("Precio", typeof(decimal));
        detalle.Columns.Add("Importe", typeof(decimal), "Cantidad * Precio"); detalle.Columns.Add("TasaISV", typeof(decimal)).DefaultValue = 15m;
        detalle.Columns.Add("ISV", typeof(decimal)); detalle.Columns.Add("Total", typeof(decimal));
        carrito.DataSource = detalle; carrito.Columns["Id"]!.Visible = false;
        carrito.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        carrito.Columns["Importe"]!.HeaderText = "Subtotal";
        foreach (string columna in new[] { "Precio", "Importe", "ISV", "Total" }) carrito.Columns[columna]!.DefaultCellStyle.Format = "'L '#,##0.00";
        agregar.Click += (_, _) => Agregar(); productos.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) Agregar(); };
        quitar.Click += (_, _) => { if (carrito.CurrentRow?.DataBoundItem is DataRowView r) { detalle.Rows.Remove(r.Row); Totales(); } };
        vaciar.Click += (_, _) => { detalle.Clear(); Totales(); };
        buscar.TextChanged += (_, _) => Filtrar(); recibido.ValueChanged += (_, _) => Totales();
        tienda.SelectedIndexChanged += (_, _) => { if (!cargandoCatalogos) { detalle.Clear(); CargarProductos(); Totales(); } };
        cliente.SelectedIndexChanged += (_, _) => { if (!cargandoCatalogos) rtnCliente.Text = cliente.SelectedItem is DataRowView fila ? VentaDatos.RtnSugerido(fila["documento_fiscal"]) : ""; };
        metodo.SelectedIndexChanged += (_, _) => { if (!cargandoCatalogos) ActualizarPago(); };
        recargar.Click += (_, _) => Cargar(); procesar.Click += (_, _) => Emitir();
        refrescarStock.Click += (_, _) => CargarProductos(limpiarBusqueda: true);
        imprimir.Click += (_, _) => Imprimir();
        ActualizarPago();
    }
    private static ComboBox Combo() => new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private bool EsEfectivo => metodo.SelectedItem is DataRowView r && string.Equals(Convert.ToString(r["codigo"]), "EFECTIVO", StringComparison.OrdinalIgnoreCase);
    private void ActualizarPago()
    {
        bool efectivo = EsEfectivo;
        recibido.Enabled = efectivo;
        referencia.ReadOnly = efectivo;
        if (efectivo) referencia.Clear();
        referencia.PlaceholderText = efectivo ? "Se genera al registrar el pago" : "Introduce la referencia del pago";
        Totales();
    }
    protected override void OnLoad(EventArgs e) { base.OnLoad(e); if (!ModoDiseno.Activo(this)) Cargar(); }
    protected override void OnDeactivate(EventArgs e) { base.OnDeactivate(e); refrescarAlActivar = true; }
    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);
        if (!refrescarAlActivar || ModoDiseno.Activo(this)) return;
        refrescarAlActivar = false; CargarProductos();
    }
    private void Cargar()
    {
        try
        {
            var clientes = VentaDatos.Consultar("SELECT id_cliente,nombre,documento_fiscal FROM dbo.Cliente WHERE activo=1 ORDER BY nombre");
            var metodos = VentaDatos.Consultar("SELECT id_metodo_pago,nombre,codigo FROM dbo.MetodoPago WHERE activo=1 ORDER BY nombre");
            var tiendas = VentaDatos.Consultar("SELECT id_tienda,nombre FROM dbo.Tienda WHERE activo=1 ORDER BY nombre");
            object? tiendaAnterior = tienda.SelectedValue, clienteAnterior = cliente.SelectedValue;
            cargandoCatalogos = true;
            void Enlazar(ComboBox combo, DataTable datos, string id)
            {
                object? anterior = combo.SelectedValue;
                combo.DisplayMember = "nombre"; combo.ValueMember = id; combo.DataSource = datos;
                if (anterior is int valor && datos.AsEnumerable().Any(r => r.Field<int>(id) == valor)) combo.SelectedValue = valor;
            }
            Enlazar(cliente, clientes, "id_cliente"); Enlazar(metodo, metodos, "id_metodo_pago"); Enlazar(tienda, tiendas, "id_tienda");
            if (!Equals(tiendaAnterior, tienda.SelectedValue)) detalle.Clear();
            if (!Equals(clienteAnterior, cliente.SelectedValue))
                rtnCliente.Text = cliente.SelectedItem is DataRowView fila ? VentaDatos.RtnSugerido(fila["documento_fiscal"]) : "";
            CargarProductos(); ActualizarPago();
        }
        catch (Exception ex) { estado.Text = "No se pudieron cargar los catálogos."; MessageBox.Show(this, ex.Message, "Facturación", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        finally { cargandoCatalogos = false; }
    }
    private void CargarProductos(bool limpiarBusqueda = false)
    {
        refrescarStock.Enabled = false;
        try
        {
            if (tienda.SelectedValue is not int id)
            {
                catalogo = null; productos.DataSource = null;
                estado.Text = "Selecciona una tienda para cargar sus existencias."; return;
            }
            int? seleccionado = (productos.CurrentRow?.DataBoundItem as DataRowView)?["id_producto"] as int?;
            var actualizado = VentaDatos.Productos(id);
            catalogo = actualizado; productos.DataSource = catalogo.DefaultView;
            productos.Columns["id_producto"]!.Visible = false;
            productos.Columns["Precio"]!.DefaultCellStyle.Format = "'L '#,##0.00";
            foreach (string columna in new[] { "Existencia", "Reservado", "Disponible" })
            { productos.Columns[columna]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; productos.Columns[columna]!.DefaultCellStyle.Format = "N0"; }
            if (limpiarBusqueda) buscar.Clear();
            Filtrar();
            if (seleccionado.HasValue)
                foreach (DataGridViewRow fila in productos.Rows)
                    if (fila.DataBoundItem is DataRowView producto && (int)producto["id_producto"] == seleccionado.Value)
                    { productos.CurrentCell = fila.Cells["Producto"]; break; }
            long disponible = catalogo.AsEnumerable().Sum(p => (long)p.Field<int>("Disponible"));
            estado.Text = $"Stock de {tienda.Text}: {catalogo.Rows.Count} productos, {disponible:N0} unidades disponibles. Actualizado {DateTime.Now:HH:mm:ss}.";
        }
        catch (Exception ex) { catalogo = null; productos.DataSource = null; estado.Text = "No se pudo actualizar el stock. " + ex.Message; }
        finally { refrescarStock.Enabled = true; }
    }
    private void Filtrar()
    {
        if (catalogo is null) return;
        BusquedaDatos.Filtrar(catalogo.DefaultView, buscar.Text, productos);
    }
    private void Agregar()
    {
        if (productos.CurrentRow?.DataBoundItem is not DataRowView p) return;
        int id = (int)p["id_producto"], unidades = (int)cantidad.Value;
        var fila = detalle.AsEnumerable().FirstOrDefault(r => r.Field<int>("Id") == id);
        int total;
        try { total = AgregarProductoVentaValidacion.Cantidad(unidades, fila?.Field<int>("Cantidad") ?? 0, Convert.ToInt32(p["Disponible"])); }
        catch (ArgumentException ex) { estado.Text = ex.Message; return; }
        decimal tasa = tasaIsv.SelectedIndex switch { 0 => 0m, 2 => 18m, _ => 15m };
        if (fila is null) fila = detalle.Rows.Add(id, p["Producto"], total, p["Precio"]); else fila["Cantidad"] = total;
        fila["TasaISV"] = tasa;
        Totales(); estado.Text = "Producto agregado.";
    }
    private decimal Total => detalle.AsEnumerable().Sum(r => r.Field<decimal>("Total"));
    private void Totales()
    {
        foreach (DataRow fila in detalle.Rows)
        { fila["ISV"] = ImpuestosVenta.Isv((decimal)fila["Importe"], (decimal)fila["TasaISV"]); fila["Total"] = (decimal)fila["Importe"] + (decimal)fila["ISV"]; }
        decimal subtotal = detalle.AsEnumerable().Sum(r => r.Field<decimal>("Importe"));
        decimal isv15 = detalle.AsEnumerable().Where(r => r.Field<decimal>("TasaISV") == 15).Sum(r => r.Field<decimal>("ISV"));
        decimal isv18 = detalle.AsEnumerable().Where(r => r.Field<decimal>("TasaISV") == 18).Sum(r => r.Field<decimal>("ISV"));
        resumen.Text = $"Subtotal: L {subtotal:N2}\nISV 15%: L {isv15:N2}\nISV 18%: L {isv18:N2}\nTotal: L {Total:N2}\nCambio: L {(EsEfectivo ? Math.Max(0, recibido.Value - Total) : 0):N2}";
    }
    private void Emitir()
    {
        int t, c, m;
        try { (t, c, m) = EmitirVentaValidacion.Captura(tienda.SelectedValue as int?, cliente.SelectedValue as int?, metodo.SelectedValue as int?, detalle.Rows.Count); }
        catch (ArgumentException ex) { estado.Text = ex.Message; return; }
        string? rtn;
        try { rtn = VentaDatos.NormalizarRtn(rtnCliente.Text); }
        catch (ArgumentException ex) { estado.Text = ex.Message; rtnCliente.Focus(); return; }
        if (MessageBox.Show(this, $"¿Confirmar la factura por L {Total:N2}?", "Emitir factura", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        procesar.Enabled = false;
        try
        {
            var factura = VentaDatos.Emitir(t, c, m, recibido.Value, referencia.Text, observaciones.Text,
                detalle.AsEnumerable().Select(r => new LineaVenta(r.Field<int>("Id"), r.Field<int>("Cantidad"), r.Field<decimal>("Precio")) { TasaIsv = r.Field<decimal>("TasaISV") }).ToArray(), rtn);
            ultima = factura;
            exportarPdf.Enabled = true;
            comprobante = "";
            detalle.Clear(); recibido.Value = 0; referencia.Clear(); observaciones.Clear(); rtnCliente.Clear(); Totales(); CargarProductos();
            estado.Text = $"Factura registrada: {factura.Numero}";
            try { comprobante = VentaDatos.Comprobante(factura.Id); Imprimir(); }
            catch (Exception ex) { MessageBox.Show(this, "La factura se guardó. No se pudo mostrar el recibo: " + ex.Message, "Recibo"); }
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "No se pudo emitir la factura", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        finally { procesar.Enabled = true; }
    }
    private void Imprimir()
    {
        if (ultima is null) { estado.Text = "Primero emite una factura."; return; }
        try { if (string.IsNullOrWhiteSpace(comprobante)) comprobante = VentaDatos.Comprobante(ultima.Id); }
        catch (Exception ex) { estado.Text = "No se pudo cargar el recibo. " + ex.Message; return; }
        using var ventana = new FormularioTematico { Text = "Factura " + ultima.Numero, Size = new Size(720, 650), StartPosition = FormStartPosition.CenterParent };
        var texto = new TextBox { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Both, Dock = DockStyle.Fill, Text = comprobante.Replace("\n", Environment.NewLine) };
        var boton = Boton("Imprimir");
        var pdf = Boton("Guardar PDF"); pdf.Click += async (_, _) => await DialogosExportacion.ReciboPdf(ventana, ultima.Id, ultima.Numero);
        boton.Click += (_, _) =>
        {
            try
            {
                using var documento = new System.Drawing.Printing.PrintDocument();
                int posicion = 0;
                documento.BeginPrint += (_, _) => posicion = 0;
                documento.PrintPage += (_, e) =>
                {
                    using var fuente = new Font("Segoe UI", 10);
                    string pendiente = comprobante[posicion..];
                    e.Graphics!.MeasureString(pendiente, fuente, e.MarginBounds.Size, StringFormat.GenericDefault, out int caracteres, out int lineas);
                    e.Graphics!.DrawString(pendiente[..caracteres], fuente, Brushes.Black, e.MarginBounds);
                    posicion += caracteres; e.HasMorePages = posicion < comprobante.Length;
                };
                using var dialogo = new PrintDialog { Document = documento, UseEXDialog = true };
                if (dialogo.ShowDialog(ventana) == DialogResult.OK) documento.Print();
            }
            catch (Exception ex) { MessageBox.Show(ventana, ex.Message, "Impresión"); }
        };
        var botones = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 54 }; botones.Controls.AddRange([boton, pdf]);
        RecargaManual.Agregar(botones, () =>
        {
            comprobante = VentaDatos.Comprobante(ultima.Id);
            texto.Text = comprobante.Replace("\n", Environment.NewLine);
        });
        ventana.Controls.Add(texto); ventana.Controls.Add(botones); ventana.ShowDialog(this);
    }

    private void Historial()
    {
        try
        {
            using var ventana = new FormularioTematico { Text = "Facturas registradas (hasta 500 por consulta)", Size = new Size(1000, 650), StartPosition = FormStartPosition.CenterParent };
            var tabla = Tabla();
            var fechas = new FiltroFechas("Fecha UTC");
            var estadoHistorial = Etiqueta("", 10);
            DataTable CargarHistorial() => VentaDatos.Historial(fechas.Desde, fechas.Hasta);
            var datos = CargarHistorial();
            tabla.DataSource = datos.DefaultView;
            tabla.MultiSelect = false;
            var filtro = Buscar("Buscar por cualquier dato de la factura"); filtro.Dock = DockStyle.Top;
            void FiltrarHistorial()
            {
                BusquedaDatos.Filtrar(datos.DefaultView, filtro.Text, tabla);
                if (tabla.DataSource is not null) estadoHistorial.Text = $"{datos.DefaultView.Count} facturas · Fechas UTC · Hasta 500 resultados; ajusta las fechas para consultar otros períodos.";
            }
            filtro.TextChanged += (_, _) => FiltrarHistorial();
            void RecargarHistorial()
            {
                try { datos = CargarHistorial(); tabla.DataSource = datos.DefaultView; FiltrarHistorial(); }
                catch (Exception ex) { tabla.DataSource = null; estadoHistorial.Text = ex.Message; }
            }
            fechas.Cambio += (_, _) => RecargarHistorial();
            var ver = Boton("Ver / imprimir factura");
            var pdf = Boton("Exportar PDF");
            pdf.Click += async (_, _) => { if (tabla.CurrentRow?.DataBoundItem is DataRowView r) await DialogosExportacion.ReciboPdf(ventana, (int)r["Id"], Convert.ToString(r["Numero"])!); };
            ver.Click += (_, _) =>
            {
                if (tabla.CurrentRow?.DataBoundItem is not DataRowView r) return;
                try
                {
                    string contenido = VentaDatos.Comprobante((int)r["Id"]);
                    ultima = new((int)r["Id"], Convert.ToString(r["Numero"])!, Convert.ToDecimal(r["Total"]), 0);
                    exportarPdf.Enabled = true;
                    comprobante = contenido; Imprimir();
                }
                catch (Exception ex) { MessageBox.Show(ventana, ex.Message, "Factura"); }
            };
            var botones = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true }; botones.Controls.AddRange([ver, pdf]);
            RecargaManual.Agregar(botones, RecargarHistorial);
            var filas = Filas(38, 48, -100, 44, 54);
            filas.RowStyles[1].SizeType = SizeType.AutoSize;
            filas.Controls.Add(filtro, 0, 0); filas.Controls.Add(fechas, 0, 1); filas.Controls.Add(tabla, 0, 2);
            filas.Controls.Add(estadoHistorial, 0, 3); filas.Controls.Add(botones, 0, 4);
            ventana.Controls.Add(filas); FiltrarHistorial(); ventana.ShowDialog(this);
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Historial de ventas"); }
    }
}
