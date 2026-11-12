using System.Data;
using System.Globalization;
using System.Reflection;
using ClosedXML.Excel;
using Sistema_ERP;

internal static partial class Program
{
    private static int ProbarRegresionesIssues()
    {
        try
        {
            ProbarLecturaPrecios();
            ProbarValidacionCeros();
            ProbarValidacionContactos();
            ProbarDesplegableRoles();
            ProbarTemasVisuales();
            ProbarDisposicion();
            ProbarCierreSesion();
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }

    private static void ProbarLecturaPrecios()
    {
        int comprobaciones = 0;
        void Comprobar(bool condicion, string contexto) { Igual(true, condicion, contexto); comprobaciones++; }
        var leerPrecio = typeof(CargaMasiva).GetMethod("TryParseDecimal", estaticos)!;
        var leerCosto = app.GetType("Sistema_ERP.Vistas.PantallaCompras")!.GetMethod("LeerCosto", estaticos | BindingFlags.Public)!;
        CultureInfo original = CultureInfo.CurrentCulture;
        try
        {
            foreach (string cultura in new[] { "en-US", "es-HN", "es-ES", "de-DE" })
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultura);
                foreach (var (texto, esperado) in new (string, decimal)[]
                {
                    ("12.50", 12.50m), ("12,50", 12.50m), ("1000", 1000m),
                    ("99999999.99", 99999999.99m), (" 12,5 ", 12.5m), (".50", .50m)
                })
                {
                    object[] argumentos = [texto, 0m];
                    Comprobar((bool)leerPrecio.Invoke(null, argumentos)! && (decimal)argumentos[1] == esperado,
                        $"Importación interpreta {texto} en {cultura}");
                    Comprobar((decimal)leerCosto.Invoke(null, [texto])! == esperado, $"Compras interpreta {texto} en {cultura}");
                }
                foreach (string texto in new[] { "", ".", ",", "1,000", "1.000", "1,234.56", "1.234,56", "L 12.50", "$12.50", "1 000,50", "1e3", "12.5000", "12.", "12,", "999999999999999999999999999999" })
                {
                    Comprobar(!(bool)leerPrecio.Invoke(null, [texto, 0m])!, $"Importación rechaza {texto} en {cultura}");
                    bool rechazado = false;
                    try { leerCosto.Invoke(null, [texto]); }
                    catch (TargetInvocationException ex) when (ex.InnerException is ArgumentException) { rechazado = true; }
                    Comprobar(rechazado, $"Compras rechaza {texto} en {cultura}");
                }
            }
        }
        finally { CultureInfo.CurrentCulture = original; }

        string ruta = Path.Combine(Path.GetTempPath(), "ERP-precios-" + Guid.NewGuid().ToString("N") + ".xlsx");
        try
        {
            using (var libro = new XLWorkbook())
            {
                var hoja = libro.AddWorksheet("Productos");
                string[] columnas = ["CodigoBarras", "Nombre", "Precio", "StockInicial", "NombreCategoria", "RTNProveedor"];
                for (int c = 0; c < columnas.Length; c++) hoja.Cell(1, c + 1).Value = columnas[c];
                for (int f = 2; f <= 10; f++)
                {
                    hoja.Cell(f, 1).Value = "PRECIO-" + f; hoja.Cell(f, 2).Value = "Producto " + f;
                    hoja.Cell(f, 4).Value = 0; hoja.Cell(f, 5).Value = "Categoría";
                    hoja.Cell(f, 6).Value = "08019999999999";
                }
                hoja.Cell(2, 3).Value = 1234.56m; hoja.Cell(2, 3).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(3, 3).Value = 12.5m; hoja.Cell(3, 3).Style.NumberFormat.Format = "\"L\" #,##0.00";
                hoja.Cell(4, 3).Value = "12,50";
                hoja.Cell(5, 3).Value = "1,000";
                hoja.Cell(6, 3).Value = "1.000";
                hoja.Cell(7, 3).Value = "1,234.56";
                hoja.Cell(8, 3).Value = "-2";
                hoja.Cell(9, 3).Value = "0";
                hoja.Cell(10, 3).Value = "100000000";
                libro.SaveAs(ruta);
            }
            var carga = typeof(CargaMasiva);
            using var tabla = (DataTable)carga.GetMethod("LeerArchivo", estaticos)!.Invoke(null, [ruta])!;
            var cat = carga.GetNestedType("Catalogos", BindingFlags.NonPublic)!;
            object catalogos = Activator.CreateInstance(cat)!;
            cat.GetProperty("Categorias")!.SetValue(catalogos, new Dictionary<string, int> { ["Categoría"] = 1 });
            cat.GetProperty("Proveedores")!.SetValue(catalogos, new Dictionary<string, int> { ["08019999999999"] = 1 });
            foreach (DataRow fila in tabla.Rows)
            {
                bool valido = (bool)carga.GetMethod("ValidarFila", estaticos)!.Invoke(null, [fila, catalogos])!;
                int numero = int.Parse(((string)fila["CodigoBarras"])[7..], CultureInfo.InvariantCulture);
                Comprobar(valido == (numero <= 4), "Validación del archivo Excel: " + fila["CodigoBarras"]);
            }
            var productos = (List<ProductoImportado>)carga.GetMethod("ConstruirProductosValidos", estaticos)!.Invoke(null, [tabla, catalogos])!;
            Comprobar(productos.Select(p => p.Precio).SequenceEqual([1234.56m, 12.5m, 12.5m]),
                "Solo se importan precios válidos y se conserva el valor numérico de Excel");
            Comprobar(((string)tabla.Rows[3]["Observacion"]).Contains("separadores de miles"),
                "El precio ambiguo explica cómo corregir el formato");
        }
        finally { File.Delete(ruta); }
        Console.WriteLine($"OK: {comprobaciones} comprobaciones de precios, culturas, Excel y validación sin SQL.");
    }

    private static void ProbarDesplegableRoles()
    {
        const BindingFlags instancia = BindingFlags.Instance | BindingFlags.NonPublic;
        int comprobaciones = 0;
        void Comprobar(bool condicion, string contexto) { Igual(true, condicion, contexto); comprobaciones++; }
        using var roles = new DataTable();
        roles.Columns.Add("Id", typeof(int)); roles.Columns.Add("Nombre", typeof(string)); roles.Columns.Add("activo", typeof(bool));
        roles.Rows.Add(1, "Lectura", true); roles.Rows.Add(2, "Gestión", true);
        roles.Rows.Add(3, "Administrador", true); roles.Rows.Add(4, "Anterior (inactivo)", false);
        using var permisos = new DataTable();
        permisos.Columns.Add("IdRol", typeof(int)); permisos.Columns.Add("CodigoRol", typeof(string));
        permisos.Columns.Add("IdPermiso", typeof(int)); permisos.Columns.Add("Modulo", typeof(string));
        permisos.Columns.Add("Accion", typeof(string)); permisos.Columns.Add("Descripcion", typeof(string));
        permisos.Rows.Add(1, "LECTURA", 10, "PRODUCTOS", "CONSULTAR", "Consultar productos");
        permisos.Rows.Add(2, "GESTION", 10, "PRODUCTOS", "CONSULTAR", "Consultar productos");
        permisos.Rows.Add(2, "GESTION", 11, "PRODUCTOS", "GESTIONAR", "Gestionar productos");
        permisos.Rows.Add(3, "ADMINISTRADOR", DBNull.Value, "", "", "");
        int[] originales = [1, 4];
        using var dialogo = new GenerarPermisosUsuario("Prueba", roles, permisos, originales)
        {
            ShowInTaskbar = false, Opacity = 0, StartPosition = FormStartPosition.Manual, Location = new Point(-5000, -5000)
        };
        dialogo.Show();
        var selector = dialogo.Controls.Find("listaRoles", true).Single();
        var disponibles = selector.Controls.Find("rolesDisponibles", true).OfType<ComboBox>().Single();
        var asignados = selector.Controls.Find("rolesAsignados", true).OfType<ListBox>().Single();
        var agregar = selector.Controls.Find("agregarRol", true).OfType<Button>().Single();
        var quitar = selector.Controls.Find("quitarRol", true).OfType<Button>().Single();
        var tabla = dialogo.Controls.Find("tablaPermisos", true).OfType<DataGridView>().Single();
        Comprobar(disponibles.DropDownStyle == ComboBoxStyle.DropDownList && dialogo.RolesSeleccionados.SequenceEqual([1, 4]),
            "El desplegable conserva varios roles existentes, incluidos los inactivos");
        Comprobar(tabla.Rows.Count == 1 && disponibles.Items.Count == 2, "Solo se ofrecen roles activos sin asignar");
        disponibles.SelectedItem = disponibles.Items.Cast<object>().Single(r => r.ToString() == "Gestión"); agregar.PerformClick();
        Comprobar(dialogo.RolesSeleccionados.ToHashSet().SetEquals([1, 2, 4]) && tabla.Rows.Count == 2, "Agregar combina permisos sin duplicarlos");
        Comprobar(disponibles.Items.Cast<object>().All(r => r.ToString() != "Gestión"), "Un rol asignado no puede duplicarse");
        asignados.SelectedItem = asignados.Items.Cast<object>().Single(r => r.ToString() == "Lectura"); quitar.PerformClick();
        Comprobar(tabla.Rows.Count == 2 && dialogo.RolesSeleccionados.ToHashSet().SetEquals([2, 4]), "Quitar conserva los demás roles y sus permisos");
        disponibles.SelectedItem = disponibles.Items.Cast<object>().Single(r => r.ToString() == "Administrador"); agregar.PerformClick();
        Comprobar(!tabla.Visible && dialogo.Controls.Find("mensajePermisos", true).Single().Text.Contains("Acceso total"), "ADMINISTRADOR muestra acceso total");
        quitar.PerformClick();
        Comprobar(tabla.Visible && tabla.Rows.Count == 2, "Quitar ADMINISTRADOR recupera los permisos combinados");
        asignados.SelectedItem = asignados.Items.Cast<object>().Single(r => r.ToString() == "Anterior (inactivo)"); quitar.PerformClick();
        Comprobar(!dialogo.RolesSeleccionados.Contains(4) && disponibles.Items.Cast<object>().All(r => !r.ToString()!.Contains("inactivo")),
            "Un rol inactivo retirado no se ofrece para reasignación");
        roles.Rows.Add(5, "Nuevo", true);
        typeof(GenerarPermisosUsuario).GetMethod("CargarOpciones", instancia)!.Invoke(dialogo, [roles, permisos, dialogo.RolesSeleccionados]);
        Comprobar(dialogo.RolesSeleccionados.SequenceEqual([2]) && disponibles.Items.Cast<object>().Any(r => r.ToString() == "Nuevo"), "Recargar conserva la selección y actualiza el desplegable");
        dialogo.Size = dialogo.MinimumSize; dialogo.PerformLayout();
        Comprobar(tabla.Height > 100 && disponibles.Width > 80 && asignados.Height > 80,
            $"El selector y los permisos caben en el tamaño mínimo (tabla={tabla.Size}, desplegable={disponibles.Size}, asignados={asignados.Size})");
        dialogo.Controls.Find("cancelar", true).OfType<Button>().Single().PerformClick();
        Comprobar(dialogo.DialogResult == DialogResult.Cancel && originales.SequenceEqual([1, 4]), "Cancelar conserva la selección original");

        using var vacio = new GenerarPermisosUsuario("", new DataTable(), new DataTable(), []);
        Comprobar(!vacio.Controls.Find("agregarRol", true).Single().Enabled && !vacio.Controls.Find("quitarRol", true).Single().Enabled,
            "Sin roles, Agregar y Quitar quedan deshabilitados");

        using var pantalla = new UsuariosRolesPermisos();
        typeof(UsuariosRolesPermisos).GetMethod("AplicarAsignaciones", instancia)!.Invoke(pantalla, [roles, new[] { 1, 2 }]);
        Comprobar(((int[])typeof(UsuariosRolesPermisos).GetMethod("SeleccionAsignaciones", instancia)!.Invoke(pantalla, null)!).ToHashSet().SetEquals([1, 2]),
            "El formulario principal conserva todos los roles para guardar");
        Console.WriteLine($"OK: {comprobaciones} comprobaciones del desplegable, roles, permisos y cancelación sin SQL.");
    }
}
