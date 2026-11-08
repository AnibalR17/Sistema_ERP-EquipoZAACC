using System.Data;

namespace Sistema_ERP;

public sealed partial class ApartadosDevoluciones : PantallaMaqueta
{
    private readonly DataGridView apartados = Tabla(), devoluciones = Tabla();
    private readonly TextBox buscarApartado = Buscar("Cliente, tienda o número de apartado"), buscarDevolucion = Buscar("Factura, cliente o número de devolución");
    private readonly ComboBox estadoApartado = Estados("ACTIVO", "ENTREGADO", "CANCELADO", "VENCIDO"), estadoDevolucion = Estados("BORRADOR", "CONFIRMADA", "ANULADA");
    private readonly Label mensaje = Etiqueta("");
    private readonly Dictionary<string, Button> accionesApartado = new(), accionesDevolucion = new();
    private bool ocupado;
    private static DataRowView? Seleccion(DataGridView tabla) => tabla.CurrentRow?.DataBoundItem as DataRowView;
    private static byte[] Version(DataRowView fila) => (byte[])fila["version_fila"];
    private static int Numero(DataRowView fila) => (int)fila["Id"];

    public ApartadosDevoluciones() : base("Apartados y devoluciones", "Reservas, abonos, entregas y reembolsos", "Los apartados reservan existencias. Las devoluciones usan la factura original y respetan lo vendido y los pagos recibidos.")
    {
        var raiz = Filas(-100, 48); var pestanas = new TabControl { Dock = DockStyle.Fill };
        TabPage Pagina(string titulo, DataGridView tabla, TextBox buscar, ComboBox estados, Dictionary<string, Button> botones, (string Texto, Action Accion)[] opciones)
        {
            var pagina = new TabPage(titulo) { Padding = new Padding(10), BackColor = Color.White };
            var contenido = Filas(42, 100, -100);
            var filtros = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, Margin = Padding.Empty };
            filtros.ColumnStyles.Add(new(SizeType.Percent, 100)); filtros.ColumnStyles.Add(new(SizeType.Absolute, 170)); filtros.ColumnStyles.Add(new(SizeType.Absolute, 110));
            buscar.Dock = estados.Dock = DockStyle.Fill; buscar.Margin = new Padding(0, 0, 10, 4);
            var consultar = Accion("Consultar", Recargar); consultar.Dock = DockStyle.Fill;
            filtros.Controls.Add(buscar, 0, 0); filtros.Controls.Add(estados, 1, 0); filtros.Controls.Add(consultar, 2, 0);
            var barra = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true };
            foreach (var opcion in opciones) { var boton = Accion(opcion.Texto, () => Ejecutar(opcion.Accion)); botones.Add(opcion.Texto, boton); barra.Controls.Add(boton); }
            contenido.Controls.Add(filtros, 0, 0); contenido.Controls.Add(barra, 0, 1); contenido.Controls.Add(tabla, 0, 2); pagina.Controls.Add(contenido);
            buscar.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Recargar(); } };
            estados.SelectedIndexChanged += (_, _) => Recargar(); tabla.SelectionChanged += (_, _) => ActualizarAcciones();
            return pagina;
        }
        pestanas.TabPages.Add(Pagina("Apartados", apartados, buscarApartado, estadoApartado, accionesApartado,
        [ ("Nuevo apartado", NuevoApartado), ("Ver detalle", () => VerDetalle(true)), ("Registrar abono", Abonar), ("Entregar y facturar", Entregar),
          ("Cancelar apartado", () => CerrarApartado(false)), ("Marcar vencido", () => CerrarApartado(true)), ("Reembolsar abonos", ReembolsarApartado) ]));
        pestanas.TabPages.Add(Pagina("Devoluciones", devoluciones, buscarDevolucion, estadoDevolucion, accionesDevolucion,
        [ ("Nueva devolución", () => EditarDevolucion(false)), ("Editar borrador", () => EditarDevolucion(true)), ("Ver detalle", () => VerDetalle(false)),
          ("Confirmar devolución", ConfirmarDevolucion), ("Anular devolución", AnularDevolucion), ("Reembolsar", ReembolsarDevolucion) ]));
        raiz.Controls.Add(pestanas, 0, 0); raiz.Controls.Add(mensaje, 0, 1); Cuerpo.Controls.Add(raiz);
        apartados.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) Ejecutar(() => VerDetalle(true)); };
        devoluciones.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) Ejecutar(() => VerDetalle(false)); };
        ActualizarAcciones();
    }

    private static ComboBox Estados(params string[] estados)
    { var combo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList }; combo.Items.Add("Todos los estados"); combo.Items.AddRange(estados); combo.SelectedIndex = 0; return combo; }
    private static Button Accion(string texto, Action ejecutar)
    {
        var boton = Boton(texto); boton.AutoSize = true; boton.Height = 36; boton.Font = new Font("Segoe UI", 10);
        boton.Margin = new Padding(0, 2, 8, 4); boton.Click += (_, _) => ejecutar(); return boton;
    }
    protected override void OnActivated(EventArgs e) { base.OnActivated(e); if (!ModoDiseno.Activo(this)) Recargar(); }
    private void Recargar()
    {
        if (ocupado || ModoDiseno.Activo(this)) return;
        Ejecutar(() => { CargarListas(); mensaje.Text = $"{apartados.Rows.Count} apartados · {devoluciones.Rows.Count} devoluciones. Fechas mostradas en UTC."; });
    }
    private void CargarListas()
    {
        void Mostrar(DataGridView tabla, DataTable datos)
        {
            int? seleccionado = Seleccion(tabla) is { } fila ? Numero(fila) : null;
            tabla.DataSource = datos;
            foreach (DataGridViewColumn col in tabla.Columns)
            {
                col.Visible = col.Name is not ("version_fila" or "id_tienda" or "id_cliente" or "id_venta");
                if (col.ValueType == typeof(decimal)) col.DefaultCellStyle.Format = "'L 'N2";
                if (col.ValueType == typeof(DateTime)) col.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                col.MinimumWidth = col.ValueType == typeof(DateTime) ? 140 : 80;
            }
            tabla.MultiSelect = false; tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            if (seleccionado.HasValue)
                foreach (DataGridViewRow r in tabla.Rows) if (r.DataBoundItem is DataRowView f && Numero(f) == seleccionado.Value) { tabla.CurrentCell = r.Cells["Id"]; break; }
        }
        Mostrar(apartados, ApartadoDatos.Listar(buscarApartado.Text, estadoApartado.SelectedIndex == 0 ? "" : estadoApartado.Text));
        Mostrar(devoluciones, DevolucionDatos.Listar(buscarDevolucion.Text, estadoDevolucion.SelectedIndex == 0 ? "" : estadoDevolucion.Text));
    }
    private void Ejecutar(Action accion)
    {
        if (ocupado) return;
        ocupado = true; UseWaitCursor = true;
        try { accion(); mensaje.ForeColor = ProductoEstilo.Exito; }
        catch (Exception ex) { mensaje.ForeColor = ProductoEstilo.Error; mensaje.Text = ex.Message; MessageBox.Show(this, ex.Message, "Apartados y devoluciones", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        finally { ocupado = false; UseWaitCursor = false; ActualizarAcciones(); }
    }
    private void ActualizarAcciones()
    {
        foreach (var b in accionesApartado.Values.Concat(accionesDevolucion.Values)) b.Enabled = !ocupado;
        var a = Seleccion(apartados); var d = Seleccion(devoluciones);
        void Habilitar(Dictionary<string, Button> botones, string texto, bool habilitado) { if (botones.TryGetValue(texto, out var b)) b.Enabled = !ocupado && habilitado; }
        foreach (string texto in new[] { "Ver detalle", "Registrar abono", "Entregar y facturar", "Cancelar apartado", "Marcar vencido", "Reembolsar abonos" }) Habilitar(accionesApartado, texto, a != null);
        bool activo = a != null && Convert.ToString(a["Estado"]) == "ACTIVO";
        foreach (string texto in new[] { "Registrar abono", "Entregar y facturar", "Cancelar apartado", "Marcar vencido" }) Habilitar(accionesApartado, texto, activo);
        Habilitar(accionesApartado, "Reembolsar abonos", a != null && (Convert.ToString(a["Estado"]) is "CANCELADO" or "VENCIDO") && (decimal)a["Reembolsable"] > 0);
        if (activo) Habilitar(accionesApartado, "Registrar abono", (decimal)a!["Saldo"] > 0 && (DateTime)a["VenceUTC"] > DateTime.UtcNow);
        foreach (string texto in new[] { "Editar borrador", "Ver detalle", "Confirmar devolución", "Anular devolución", "Reembolsar" }) Habilitar(accionesDevolucion, texto, d != null);
        Habilitar(accionesDevolucion, "Editar borrador", d != null && Convert.ToString(d["Estado"]) == "BORRADOR");
        Habilitar(accionesDevolucion, "Confirmar devolución", d != null && Convert.ToString(d["Estado"]) == "BORRADOR");
        Habilitar(accionesDevolucion, "Anular devolución", d != null && Convert.ToString(d["Estado"]) != "ANULADA" && (decimal)d["Reembolsado"] == 0);
        Habilitar(accionesDevolucion, "Reembolsar", d != null && Convert.ToString(d["Estado"]) == "CONFIRMADA" && (decimal)d["Pendiente"] > 0);
    }
    private void VerDetalle(bool apartado)
    {
        var fila = Seleccion(apartado ? apartados : devoluciones); if (fila is null) return;
        using var ds = apartado ? ApartadoDatos.Obtener(Numero(fila)) : DevolucionDatos.Obtener(Numero(fila));
        using var ventana = Ventana((apartado ? "Apartado " : "Devolución ") + Numero(fila));
        var tabs = new TabControl { Dock = DockStyle.Fill }; string[] nombres = apartado ? ["Resumen", "Productos", "Abonos", "Reembolsos"] : ["Resumen", "Productos", "Reembolsos"];
        for (int i = 0; i < ds.Tables.Count; i++)
        {
            var tab = new TabPage(nombres[i]); var tabla = Tabla(); tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells; tabla.DataSource = ds.Tables[i];
            foreach (string c in new[] { "version_fila", "id_tienda", "id_cliente", "id_venta" }) if (tabla.Columns[c] is { } col) col.Visible = false;
            tab.Controls.Add(tabla); tabs.TabPages.Add(tab);
        }
        ventana.Controls.Add(tabs); ventana.ShowDialog(this);
    }
    private void Abonar()
    {
        var a = Seleccion(apartados); if (a is null) return;
        using var datos = ApartadoDatos.Obtener(Numero(a)); decimal saldo = (decimal)datos.Tables[0].Rows[0]["Saldo"];
        using var cobro = new CobroDialogo("Abonar al apartado " + Numero(a), saldo, true);
        if (cobro.ShowDialog(this) != DialogResult.OK) return;
        ApartadoDatos.Abonar(Numero(a), Version(a), cobro.Metodo, cobro.Monto, cobro.Referencia); CargarListas(); mensaje.Text = "Abono registrado correctamente.";
    }
    private void Entregar()
    {
        var a = Seleccion(apartados); if (a is null) return;
        using var datos = ApartadoDatos.Obtener(Numero(a)); decimal saldo = (decimal)datos.Tables[0].Rows[0]["Saldo"];
        using var cobro = new CobroDialogo("Entregar apartado " + Numero(a) + " y facturar", saldo, false);
        if (cobro.ShowDialog(this) != DialogResult.OK) return;
        var factura = ApartadoDatos.Entregar(Numero(a), Version(a), cobro.Metodo, cobro.Recibido, cobro.Referencia);
        CargarListas(); mensaje.Text = $"Factura {factura.Numero} · Total L {factura.Total:N2} · Cambio L {factura.Cambio:N2}";
        MostrarComprobante(VentaDatos.Comprobante(factura.Id), factura.Numero, factura.Id);
    }
    private void CerrarApartado(bool vencido)
    {
        var a = Seleccion(apartados); if (a is null) return;
        string? motivo = PedirMotivo(vencido ? "Marcar apartado como vencido" : "Cancelar apartado"); if (motivo is null) return;
        ApartadoDatos.Cerrar(Numero(a), Version(a), vencido, motivo); CargarListas(); mensaje.Text = "Apartado cerrado. La reserva se liberó; los abonos pueden reembolsarse por separado.";
    }
    private void ReembolsarApartado()
    {
        var a = Seleccion(apartados); if (a is null) return;
        string? motivo = PedirMotivo($"Reembolsar abonos: L {a["Reembolsable"]:N2}"); if (motivo is null) return;
        decimal total = ApartadoDatos.Reembolsar(Numero(a), Version(a), motivo); CargarListas(); mensaje.Text = $"Reembolso registrado: L {total:N2}, usando los métodos de los abonos originales.";
    }
    private void ConfirmarDevolucion()
    {
        var d = Seleccion(devoluciones); if (d is null) return;
        if (MessageBox.Show(this, "¿Confirmar la devolución y reintegrar las unidades marcadas al inventario?", "Confirmar devolución", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        DevolucionDatos.Confirmar(Numero(d), Version(d)); CargarListas(); mensaje.Text = "Devolución confirmada. Puedes registrar el reembolso.";
    }
    private void AnularDevolucion()
    {
        var d = Seleccion(devoluciones); if (d is null) return;
        string? motivo = PedirMotivo("Anular devolución y revertir su inventario"); if (motivo is null) return;
        DevolucionDatos.Anular(Numero(d), Version(d), motivo); CargarListas(); mensaje.Text = "Devolución anulada correctamente.";
    }
    private void ReembolsarDevolucion()
    {
        var d = Seleccion(devoluciones); if (d is null) return;
        string? motivo = PedirMotivo($"Reembolsar devolución: L {d["Pendiente"]:N2}"); if (motivo is null) return;
        decimal total = DevolucionDatos.Reembolsar(Numero(d), Version(d), motivo); CargarListas(); mensaje.Text = $"Reembolso registrado: L {total:N2}, aplicado a los pagos originales de la factura.";
    }
    private static Form Ventana(string titulo) => new() { Text = titulo, Size = new Size(1000, 680), MinimumSize = new Size(760, 540), StartPosition = FormStartPosition.CenterParent, Font = new Font("Segoe UI", 10), Padding = new Padding(16), BackColor = Color.White };
    private void MostrarComprobante(string texto, string numero, int venta)
    {
        using var ventana = Ventana("Factura " + numero);
        ventana.Controls.Add(new TextBox { Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Both, Text = texto.Replace("\n", Environment.NewLine) });
        var pdf = Accion("Guardar PDF", async () => await DialogosExportacion.ReciboPdf(ventana, venta, numero)); pdf.Dock = DockStyle.Bottom;
        ventana.Controls.Add(pdf);
        ventana.ShowDialog(this);
    }
    private string? PedirMotivo(string titulo)
    {
        using var ventana = new Form { Text = titulo, Size = new Size(570, 290), StartPosition = FormStartPosition.CenterParent, Padding = new Padding(16) };
        var texto = new TextBox { Multiline = true, MaxLength = 500, Dock = DockStyle.Fill, PlaceholderText = "Escribe el motivo de la operación" };
        var aceptar = Accion("Confirmar", () => { if (!string.IsNullOrWhiteSpace(texto.Text)) ventana.DialogResult = DialogResult.OK; else texto.Focus(); }); aceptar.Dock = DockStyle.Bottom;
        ventana.Controls.Add(texto); ventana.Controls.Add(aceptar); return ventana.ShowDialog(this) == DialogResult.OK ? texto.Text.Trim() : null;
    }
}
