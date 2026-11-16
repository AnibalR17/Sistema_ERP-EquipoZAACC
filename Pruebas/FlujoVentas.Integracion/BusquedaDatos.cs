using System.Data;
using System.Reflection;
using Sistema_ERP;

internal static partial class Program
{
    private static void ProbarBusquedaDatos()
    {
        var expresion = app.GetType("Sistema_ERP.BusquedaDatos")!.GetMethod("Expresion", estaticos)!;
        using var datos = new DataTable();
        datos.Columns.Add("Nombre"); datos.Columns.Add("Teléfono"); datos.Columns.Add("Importe", typeof(decimal));
        datos.Columns.Add("Fecha", typeof(DateTime)); datos.Columns.Add("Activo", typeof(bool));
        datos.Columns.Add("Interno"); datos.Columns.Add("Versión", typeof(byte[]));
        datos.Rows.Add("Ana O'Brien [%]*", "9988-1122", 125.50m, new DateTime(2026, 11, 15, 14, 30, 0), true, "secreto", new byte[] { 1 });
        datos.Rows.Add("María", DBNull.Value, 30m, new DateTime(2026, 11, 16), true, "otro", new byte[] { 2 });
        datos.Rows.Add("Ana", "9988-3344", 25m, new DateTime(2026, 10, 1), false, "reservado", new byte[] { 3 });
        int comprobaciones = 0;
        void Comprobar(bool condicion, string contexto) { Igual(true, condicion, contexto); comprobaciones++; }
        void Buscar(string texto, int esperado, string[]? campos = null)
        {
            using var vista = new DataView(datos);
            vista.RowFilter = (string)expresion.Invoke(null, [datos, texto, campos])!;
            Comprobar(vista.Count == esperado, $"Búsqueda literal «{texto}»: {vista.Count} filas");
        }
        Buscar("ana", 2); Buscar("MARÍA", 1); Buscar("9988-1122", 1); Buscar("125", 1);
        Buscar("15/11/2026", 1); Buscar("2026-11-16", 1); Buscar("sí", 2); Buscar("no", 1, ["Activo"]);
        foreach (string texto in new[] { "O'Brien", "[", "]", "%", "*", "[%]*" }) Buscar(texto, 1);
        Buscar("' OR true", 0); Buscar("", 3); Buscar("inexistente", 0); Buscar("Ana", 0, ["Teléfono"]);
        Buscar("secreto", 0, ["Nombre", "Teléfono"]); Buscar("1", 0, ["Versión"]);

        using var tabla = new DataGridView { AutoGenerateColumns = true, AllowUserToAddRows = false, DataSource = datos };
        tabla.DataBindingComplete += (_, _) =>
        {
            foreach (string nombre in new[] { "Interno", "Versión" })
                if (tabla.Columns[nombre] is { } columna) columna.Visible = false;
        };
        using var control = (Control)app.GetType("Sistema_ERP.BusquedaTabla")!.GetMethod("Crear", estaticos)!.Invoke(null, [tabla, false])!;
        using var ventana = new FormularioTematico { Size = new Size(900, 500), ShowInTaskbar = false,
            StartPosition = FormStartPosition.Manual, Location = new Point(-10000, -10000), Opacity = 0 };
        ventana.Controls.Add(control); ventana.Show(); Application.DoEvents();
        tabla.Columns["Interno"]!.Visible = tabla.Columns["Versión"]!.Visible = false;
        var buscar = control.Controls.Find("buscarDatos", true).OfType<TextBox>().Single();
        var campo = control.Controls.Find("campoBusqueda", true).OfType<ComboBox>().Single();
        buscar.Text = "Ana"; Comprobar(tabla.Rows.Count == 2 && datos.Rows.Count == 3, "Filtrar conserva todas las filas para guardar y totalizar");
        campo.SelectedIndex = campo.Items.Cast<object>().Select((item, indice) => (item, indice)).Single(p => p.item.ToString() == "Teléfono").indice;
        Comprobar(tabla.Rows.Count == 0, "El campo elegido limita la búsqueda");
        buscar.Text = "9988-1122"; Comprobar(tabla.Rows.Count == 1, "Busca por teléfono en el campo elegido");
        campo.SelectedIndex = 0; buscar.Text = "secreto";
        Comprobar(tabla.Rows.Count == 0, "Los datos ocultos no participan en la búsqueda");
        control.Controls.Find("limpiarBusqueda", true).OfType<Button>().Single().PerformClick();
        Comprobar(tabla.Rows.Count == 3, "Limpiar recupera todas las filas");
        buscar.Text = "María";
        using var recarga = datos.Copy(); recarga.Rows.Add("María segunda", "9999-0000", 10m, DateTime.Today, true, "", new byte[] { 4 });
        tabla.DataSource = recarga;
        Comprobar(tabla.Rows.Count == 2, "Recargar conserva y vuelve a aplicar la búsqueda");
        buscar.Clear();
        var baseActivos = new DataView(datos) { RowFilter = "Activo = true" };
        tabla.DataSource = baseActivos; buscar.Text = "Ana";
        Comprobar(tabla.Rows.Count == 1, "La búsqueda respeta el filtro de estado previo");
        buscar.Clear(); Comprobar(tabla.Rows.Count == 2, "Limpiar mantiene el filtro de estado previo");

        using var clientes = new DataTable(); clientes.Columns.Add("id_cliente", typeof(int));
        foreach (string nombre in new[] { "nombre", "telefono", "correo", "documento_fiscal" }) clientes.Columns.Add(nombre);
        clientes.Rows.Add(7, "Cliente repetido", "9988-7777", "uno@ejemplo.hn", "08011999123456");
        clientes.Rows.Add(9, "Cliente repetido", "9988-9999", "dos@ejemplo.hn", "08011999654321");
        using var selector = (Form)Activator.CreateInstance(app.GetType("Sistema_ERP.SeleccionarClienteApartado")!,
            BindingFlags.Instance | BindingFlags.NonPublic, null, [clientes], null)!;
        selector.ShowInTaskbar = false; selector.StartPosition = FormStartPosition.Manual; selector.Location = new Point(-10000, -10000); selector.Opacity = 0;
        selector.Show(); Application.DoEvents();
        selector.Controls.Find("buscarDatos", true).OfType<TextBox>().Single().Text = "dos@ejemplo.hn";
        var lista = selector.Controls.Find("clientesExistentes", true).OfType<DataGridView>().Single();
        Comprobar(lista.ReadOnly && !lista.AllowUserToAddRows && lista.Rows.Count == 1, "Seleccionar cliente permite buscar homónimos por contacto sin crear registros");
        selector.Controls.Find("usarCliente", true).OfType<Button>().Single().PerformClick();
        Comprobar((int?)selector.GetType().GetProperty("IdCliente", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(selector) == 9,
            "El selector devuelve el identificador del cliente existente elegido");
        Fallar(() => Invocar("ApartadoDatos", "Crear", 1, 0, DateTime.UtcNow.AddDays(1), Array.CreateInstance(app.GetType("Sistema_ERP.LineaVenta")!, 0)));
        Console.WriteLine($"OK: {comprobaciones} comprobaciones de búsquedas, campos, recargas y selección de clientes existentes, sin SQL.");
    }
}
