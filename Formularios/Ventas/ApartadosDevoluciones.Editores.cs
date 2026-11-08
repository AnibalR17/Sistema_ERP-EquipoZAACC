using System.Data;

namespace Sistema_ERP;

public sealed partial class ApartadosDevoluciones
{
    private static ComboBox Combo(DataTable datos, string id, string texto = "nombre") => new()
    { DataSource = datos, ValueMember = id, DisplayMember = texto, DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
    private static NumericUpDown Dinero(decimal maximo = 9999999999999999.99m) => new()
    { DecimalPlaces = 2, Maximum = Math.Max(0, maximo), ThousandsSeparator = true, Dock = DockStyle.Fill };
    private static void Error(Form propietario, Exception ex) => MessageBox.Show(propietario, ex.Message, "Revisa los datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    private static void ColumnaOculta(DataGridView tabla, string nombre) { if (tabla.Columns[nombre] is { } c) c.Visible = false; }

    private void NuevoApartado()
    {
        using var ventana = Ventana("Nuevo apartado");
        var tienda = Combo(VentaDatos.Consultar("SELECT id_tienda,nombre FROM dbo.Tienda WHERE activo=1 ORDER BY nombre"), "id_tienda");
        var cliente = Combo(VentaDatos.Consultar("SELECT id_cliente,nombre FROM dbo.Cliente WHERE activo=1 ORDER BY nombre"), "id_cliente");
        var vence = new DateTimePicker { Value = InicioDatos.AhoraHonduras.AddDays(7), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm", Dock = DockStyle.Fill };
        var campos = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 2 };
        for (int i = 0; i < 3; i++) campos.ColumnStyles.Add(new(SizeType.Percent, 100F / 3));
        campos.RowStyles.Add(new(SizeType.Absolute, 28)); campos.RowStyles.Add(new(SizeType.Percent, 100));
        foreach (var c in new[] { ("Tienda", (Control)tienda), ("Cliente", (Control)cliente), ("Vence (hora de Honduras)", (Control)vence) })
        { int indice = campos.Controls.Count / 2; campos.Controls.Add(Etiqueta(c.Item1), indice, 0); campos.Controls.Add(c.Item2, indice, 1); }
        var producto = new ComboBox { Width = 430, DropDownStyle = ComboBoxStyle.DropDownList, DisplayMember = "Mostrar", ValueMember = "id_producto" };
        var cantidad = new NumericUpDown { Minimum = 1, Maximum = 100000, Value = 1, Width = 80 };
        var tabla = Tabla(); var detalle = new DataTable();
        detalle.Columns.Add("Id", typeof(int)); detalle.Columns.Add("Producto", typeof(string)); detalle.Columns.Add("Cantidad", typeof(int)); detalle.Columns.Add("Precio", typeof(decimal));
        detalle.Columns.Add("Subtotal", typeof(decimal), "Cantidad*Precio"); detalle.Columns.Add("ISV", typeof(decimal)); detalle.Columns.Add("Importe", typeof(decimal));
        var total = Etiqueta("Total L 0.00"); var barra = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true };
        void Totales()
        {
            foreach (DataRow r in detalle.Rows) { r["ISV"] = ImpuestosVenta.Isv((decimal)r["Subtotal"], 15m); r["Importe"] = (decimal)r["Subtotal"] + (decimal)r["ISV"]; }
            total.Text = $"Total con ISV 15%: L {detalle.AsEnumerable().Sum(r => r.Field<decimal>("Importe")):N2}";
        }
        var agregar = Accion("Agregar", () =>
        {
            try
            {
                if (producto.SelectedItem is not DataRowView p) throw new ArgumentException("Selecciona un producto.");
                var existente = detalle.AsEnumerable().FirstOrDefault(r => r.Field<int>("Id") == (int)p["id_producto"]);
                int unidades = (int)cantidad.Value + (existente?.Field<int>("Cantidad") ?? 0);
                if (unidades > (int)p["Disponible"]) throw new ArgumentException("La cantidad supera el stock disponible.");
                if (existente is null) detalle.Rows.Add(p["id_producto"], p["Producto"], unidades, p["Precio"]); else existente["Cantidad"] = unidades;
                Totales();
            }
            catch (Exception ex) { Error(ventana, ex); }
        });
        var quitar = Accion("Quitar", () => { if (tabla.CurrentRow?.DataBoundItem is DataRowView r) detalle.Rows.Remove(r.Row); Totales(); });
        barra.Controls.AddRange([producto, cantidad, agregar, quitar]);
        var botones = new FlowLayoutPanel { Dock = DockStyle.Fill }; int? creado = null;
        var guardar = Accion("Guardar apartado", () =>
        {
            try
            {
                if (tienda.SelectedValue is not int t || cliente.SelectedValue is not int c) throw new ArgumentException("Selecciona tienda y cliente.");
                creado = ApartadoDatos.Crear(t, c, DateTime.SpecifyKind(vence.Value, DateTimeKind.Unspecified).AddHours(6),
                    detalle.AsEnumerable().Select(r => new LineaVenta(r.Field<int>("Id"), r.Field<int>("Cantidad"), r.Field<decimal>("Precio"))).ToArray());
                ventana.DialogResult = DialogResult.OK;
            }
            catch (Exception ex) { Error(ventana, ex); }
        });
        var cancelar = Accion("Cerrar", () => ventana.DialogResult = DialogResult.Cancel); botones.Controls.AddRange([guardar, cancelar]);
        var filas = Filas(72, 76, -100, 32, 44); filas.Controls.Add(campos, 0, 0); filas.Controls.Add(barra, 0, 1); filas.Controls.Add(tabla, 0, 2); filas.Controls.Add(total, 0, 3); filas.Controls.Add(botones, 0, 4); ventana.Controls.Add(filas);
        tabla.DataSource = detalle; ColumnaOculta(tabla, "Id");
        void CargarProductos()
        {
            try
            {
                detalle.Clear(); total.Text = "Total L 0.00"; producto.DataSource = null;
                if (tienda.SelectedValue is not int id) return;
                var catalogo = VentaDatos.Productos(id); catalogo.Columns.Add("Mostrar", typeof(string), "Producto + ' · Disponible: ' + Convert(Disponible, 'System.String')"); producto.DataSource = catalogo;
            }
            catch (Exception ex) { Error(ventana, ex); }
        }
        tienda.SelectedIndexChanged += (_, _) => CargarProductos(); CargarProductos();
        if (ventana.ShowDialog(this) == DialogResult.OK) { CargarListas(); mensaje.Text = $"Apartado {creado} creado. Las unidades quedaron reservadas."; }
    }

    private void EditarDevolucion(bool editar)
    {
        var seleccion = Seleccion(devoluciones); if (editar && seleccion is null) return;
        int? id = editar ? Numero(seleccion!) : null;
        using var ventana = Ventana(editar ? "Editar borrador de devolución" : "Nueva devolución");
        var venta = Combo(DevolucionDatos.Ventas(), "id_venta");
        var motivo = new TextBox { MaxLength = 500, Dock = DockStyle.Fill, PlaceholderText = "Motivo de la devolución", Multiline = true };
        var ayuda = Etiqueta("Escribe la cantidad en Devolver (0 para omitir) y marca Reintegrar solo si la unidad vuelve al inventario.", 10);
        var tabla = Tabla(); tabla.ReadOnly = false; tabla.EditMode = DataGridViewEditMode.EditOnEnter;
        DataTable lineas = new(); byte[]? version = null;
        if (editar)
        {
            using var datos = DevolucionDatos.Obtener(id!.Value); var d = datos.Tables[0].Rows[0];
            venta.SelectedValue = d["id_venta"]; venta.Enabled = false; motivo.Text = Convert.ToString(d["Motivo"]); version = (byte[])d["version_fila"];
        }
        bool errorCelda = false;
        var guardar = Accion("Guardar borrador", () =>
        {
            try
            {
                if (!tabla.EndEdit() || errorCelda) throw new ArgumentException("Corrige la cantidad a devolver antes de guardar.");
                if (tabla.DataSource is DataTable fuente) ventana.BindingContext?[fuente]?.EndCurrentEdit();
                if (venta.SelectedValue is not int v) throw new ArgumentException("Selecciona una factura confirmada.");
                var seleccionadas = new List<LineaDevolucion>();
                foreach (DataRow l in lineas.Rows)
                {
                    int cantidad = Convert.ToInt32(l["Devolver"]);
                    if (cantidad < 0 || cantidad > (int)l["Disponible"]) throw new ArgumentException("La cantidad a devolver debe estar entre cero y la cantidad disponible.");
                    if (cantidad > 0) seleccionadas.Add(new((int)l["Id"], cantidad, (bool)l["Reintegrar"]));
                }
                DevolucionDatos.Guardar(id, version, v, motivo.Text, seleccionadas); ventana.DialogResult = DialogResult.OK;
            }
            catch (Exception ex) { Error(ventana, ex); }
        });
        var botones = new FlowLayoutPanel { Dock = DockStyle.Fill }; botones.Controls.Add(guardar); botones.Controls.Add(Accion("Cerrar", () => ventana.DialogResult = DialogResult.Cancel));
        var filas = Filas(28, 42, 28, 62, 48, -100, 44);
        filas.Controls.Add(Etiqueta("Factura original"), 0, 0); filas.Controls.Add(venta, 0, 1); filas.Controls.Add(Etiqueta("Motivo"), 0, 2); filas.Controls.Add(motivo, 0, 3); filas.Controls.Add(ayuda, 0, 4); filas.Controls.Add(tabla, 0, 5); filas.Controls.Add(botones, 0, 6); ventana.Controls.Add(filas);
        tabla.DataError += (_, e) => { errorCelda = true; e.ThrowException = false; Error(ventana, new ArgumentException("La cantidad a devolver debe ser un número entero.")); };
        tabla.CellValidating += (_, e) =>
        {
            if (e.ColumnIndex >= 0 && tabla.Columns[e.ColumnIndex].Name == "Devolver")
            { errorCelda = !int.TryParse(Convert.ToString(e.FormattedValue), out int n) || n < 0; e.Cancel = errorCelda; }
        };
        void Cargar()
        {
            try
            {
                lineas = venta.SelectedValue is int v ? DevolucionDatos.LineasVenta(v, id) : new(); tabla.DataSource = lineas;
                foreach (DataGridViewColumn c in tabla.Columns) c.ReadOnly = c.Name is not ("Devolver" or "Reintegrar");
                ColumnaOculta(tabla, "Id");
            }
            catch (Exception ex) { Error(ventana, ex); }
        }
        venta.SelectedIndexChanged += (_, _) => Cargar(); Cargar();
        if (ventana.ShowDialog(this) == DialogResult.OK) { CargarListas(); mensaje.Text = "Borrador guardado. Confirma la devolución para aplicar el inventario."; }
    }

    private sealed class CobroDialogo : Form
    {
        private readonly ComboBox metodo;
        private readonly NumericUpDown monto, recibido;
        private readonly TextBox referencia = new() { Dock = DockStyle.Fill, MaxLength = 100 };
        internal int Metodo => metodo.SelectedValue is int id ? id : 0;
        internal decimal Monto => monto.Value;
        internal decimal Recibido => recibido.Value;
        internal string Referencia => referencia.Text;
        internal CobroDialogo(string titulo, decimal saldo, bool abono)
        {
            Text = titulo; Size = new Size(550, 470); MinimumSize = Size; StartPosition = FormStartPosition.CenterParent; Padding = new Padding(20); Font = new Font("Segoe UI", 10);
            metodo = Combo(VentaDatos.Consultar("SELECT id_metodo_pago,nombre,codigo FROM dbo.MetodoPago WHERE activo=1 ORDER BY nombre"), "id_metodo_pago");
            monto = Dinero(saldo); monto.Value = saldo; monto.Enabled = abono;
            recibido = Dinero(); recibido.Value = saldo;
            bool Efectivo() => metodo.SelectedItem is DataRowView r && string.Equals(Convert.ToString(r["codigo"]), "EFECTIVO", StringComparison.OrdinalIgnoreCase);
            var aceptar = Accion(abono ? "Registrar abono" : "Confirmar entrega y factura", () =>
            {
                if ((abono || saldo > 0) && (Metodo <= 0 || Monto <= 0)) { Error(this, new ArgumentException("Selecciona método de pago y un monto positivo.")); return; }
                if (saldo > 0 && !Efectivo() && string.IsNullOrWhiteSpace(Referencia)) { referencia.Focus(); Error(this, new ArgumentException("Introduce la referencia del pago.")); return; }
                if (!abono && saldo > 0 && Efectivo() && Recibido < saldo) { Error(this, new ArgumentException("El monto recibido es insuficiente.")); return; }
                DialogResult = DialogResult.OK;
            });
            var filas = Filas(32, 28, 38, 28, 38, 28, 38, 28, 38, 48);
            filas.Controls.Add(Etiqueta($"Saldo pendiente: L {saldo:N2}", 12, true), 0, 0);
            filas.Controls.Add(Etiqueta(abono ? "Monto del abono" : "Saldo a cobrar"), 0, 1); filas.Controls.Add(monto, 0, 2);
            filas.Controls.Add(Etiqueta("Método de pago"), 0, 3); filas.Controls.Add(metodo, 0, 4);
            filas.Controls.Add(Etiqueta("Referencia"), 0, 5); filas.Controls.Add(referencia, 0, 6);
            filas.Controls.Add(Etiqueta("Recibido (efectivo, solo entrega)"), 0, 7); filas.Controls.Add(recibido, 0, 8); filas.Controls.Add(aceptar, 0, 9); Controls.Add(filas);
            void Actualizar() { metodo.Enabled = saldo > 0; referencia.Enabled = saldo > 0; recibido.Enabled = !abono && saldo > 0 && Efectivo(); }
            metodo.SelectedIndexChanged += (_, _) => Actualizar(); Actualizar();
        }
    }
}
