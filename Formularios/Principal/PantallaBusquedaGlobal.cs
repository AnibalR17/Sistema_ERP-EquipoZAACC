using System.Data;

namespace Sistema_ERP;

internal sealed class PantallaBusquedaGlobal : PantallaMaqueta
{
    private readonly DataGridView resultados = Tabla();
    private readonly Label estado = Etiqueta("Escribe al menos 2 caracteres en la barra superior para buscar.");
    private readonly Button detalle = Boton("Ver detalle", true), modulo = Boton("Abrir módulo");
    internal event Action<ResultadoBusqueda>? VerDetalle;
    internal event Action<ResultadoBusqueda>? AbrirModulo;
    internal event Func<Task>? ReiniciarSolicitado;

    internal PantallaBusquedaGlobal() : base("Búsqueda global", "Encuentra contenido en todos tus módulos",
        "Busca nombres, códigos, contactos o números de documentos. Solo se muestra el contenido que puedes consultar.")
    {
        Name = "pantallaBusquedaGlobal";
        resultados.Name = "resultadosBusquedaGlobal";
        resultados.AutoGenerateColumns = false;
        resultados.MultiSelect = false;
        foreach (var (campo, titulo, peso) in new[] { ("Tipo", "Tipo", 16F), ("Nombre", "Resultado", 34F), ("Detalle", "Información", 50F) })
            resultados.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = campo, Name = campo, HeaderText = titulo, FillWeight = peso });
        var filas = Filas(52, -100, 56);
        estado.Name = "estadoBusquedaGlobal";
        filas.Controls.Add(estado, 0, 0); filas.Controls.Add(resultados, 0, 1);
        var botones = new FlowLayoutPanel { Dock = DockStyle.Fill };
        detalle.Name = "verDetalleBusqueda"; modulo.Name = "abrirModuloBusqueda";
        botones.Controls.AddRange([detalle, modulo]); filas.Controls.Add(botones, 0, 2); Cuerpo.Controls.Add(filas);
        RecargaManual.Agregar(botones, async () => { if (ReiniciarSolicitado is { } recargar) await recargar(); });
        detalle.Click += (_, _) => AbrirSeleccion();
        modulo.Click += (_, _) => { if (Seleccion is { } resultado) AbrirModulo?.Invoke(resultado); };
        resultados.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) AbrirSeleccion(); };
        resultados.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; AbrirSeleccion(); } };
        resultados.SelectionChanged += (_, _) => ActualizarAcciones();
        ActualizarAcciones();
    }

    private ResultadoBusqueda? Seleccion => resultados.CurrentRow?.DataBoundItem as ResultadoBusqueda;
    private bool ocupado;

    private void AbrirSeleccion()
    {
        if (ocupado || Seleccion is not { } resultado) return;
        if (resultado.Id == 0) AbrirModulo?.Invoke(resultado); else VerDetalle?.Invoke(resultado);
    }

    internal void MostrarEstado(string texto)
    {
        resultados.DataSource = null; estado.Text = texto; ActualizarAcciones();
    }

    internal void MostrarResultados(string texto, string tienda, IReadOnlyList<ResultadoBusqueda> encontrados)
    {
        resultados.DataSource = encontrados.ToArray();
        estado.Text = encontrados.Count == 0 ? $"Sin coincidencias para «{texto}» en el contenido permitido. {tienda}."
            : $"{encontrados.Count} resultados para «{texto}» · {tienda} · Hasta {BusquedaGlobalDatos.LimitePorTipo} por tipo; afina la búsqueda para ver otros.";
        ActualizarAcciones();
    }

    internal void MarcarOcupado(bool valor) { ocupado = valor; ActualizarAcciones(); }
    private void ActualizarAcciones()
    {
        detalle.Enabled = !ocupado && Seleccion is { Id: > 0 };
        modulo.Enabled = !ocupado && Seleccion is not null;
    }
}

internal sealed class DetalleBusquedaGlobal : FormularioTematico
{
    internal ResultadoBusqueda Resultado { get; }

    internal DetalleBusquedaGlobal(ResultadoBusqueda resultado, DataSet datos)
    {
        Resultado = resultado;
        Text = $"{resultado.Tipo}: {resultado.Nombre}";
        Size = new Size(1050, 650); MinimumSize = new Size(700, 400);
        StartPosition = FormStartPosition.CenterParent; ShowInTaskbar = false;
        Font = new Font("Segoe UI", 11F); Padding = new Padding(16); BackColor = Color.White;
        var tabs = new Sistema_ERP.PestanasTematicas { Dock = DockStyle.Fill };
        string[] titulos = resultado.Tipo switch
        {
            "Apartado" => ["Datos generales", "Productos", "Abonos", "Reembolsos"],
            "Devolución" => ["Datos generales", "Productos", "Reembolsos"],
            "Venta" => ["Datos generales", "Productos", "Pagos"],
            "Traspaso" => ["Datos generales", "Productos", "Historial"],
            _ => ["Datos generales", "Productos"]
        };
        void MostrarDatos(DataSet actuales)
        {
            int seleccion = tabs.SelectedIndex;
            foreach (TabPage anterior in tabs.TabPages.Cast<TabPage>().ToArray()) anterior.Dispose();
            tabs.TabPages.Clear();
            for (int i = 0; i < actuales.Tables.Count; i++)
            {
                var pagina = new TabPage(i < titulos.Length ? titulos[i] : $"Detalle {i + 1}") { Padding = new Padding(8) };
                var tabla = PantallaMaqueta.Tabla();
                tabla.AutoGenerateColumns = false;
                tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                foreach (DataColumn dato in actuales.Tables[i].Columns)
                {
                    if (dato.DataType == typeof(byte[]) || dato.ColumnName.StartsWith("id_", StringComparison.OrdinalIgnoreCase)) continue;
                    var columna = new DataGridViewTextBoxColumn
                    { Name = dato.ColumnName, DataPropertyName = dato.ColumnName, HeaderText = dato.ColumnName.Replace('_', ' '), ValueType = dato.DataType };
                    if (columna.ValueType == typeof(DateTime)) columna.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    else if (columna.ValueType == typeof(decimal)) columna.DefaultCellStyle.Format = "N2";
                    tabla.Columns.Add(columna);
                }
                tabla.DataSource = actuales.Tables[i];
                pagina.Controls.Add(tabla); tabs.TabPages.Add(pagina);
            }
            if (seleccion >= 0 && seleccion < tabs.TabCount) tabs.SelectedIndex = seleccion;
        }
        MostrarDatos(datos);
        var acciones = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 56 };
        var abrir = PantallaMaqueta.Boton("Abrir módulo", true); abrir.DialogResult = DialogResult.OK;
        var cerrar = PantallaMaqueta.Boton("Cerrar"); cerrar.DialogResult = DialogResult.Cancel;
        acciones.Controls.AddRange([abrir, cerrar]); Controls.Add(tabs); Controls.Add(acciones); CancelButton = cerrar;
        RecargaManual.Agregar(acciones, async () =>
        {
            using var actuales = await Task.Run(() => BusquedaGlobalDatos.ObtenerDetalle(Resultado));
            if (!IsDisposed) MostrarDatos(actuales);
        });
    }
}
