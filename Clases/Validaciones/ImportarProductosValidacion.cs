using System.Data;
using System.Globalization;
using ClosedXML.Excel;
using Sistema_ERP.Clases;

namespace Sistema_ERP;

internal static class ImportarProductosValidacion
{
    internal static void ProveedorUnico(bool unico)
    {
        if (!unico) throw new InvalidOperationException("Hay proveedores activos con el mismo RTN normalizado. Corrige sus RTN antes de importar.");
    }

    internal static void EncabezadoPlantilla(string actual, string esperado)
    {
        if (!string.Equals(actual, esperado, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Los encabezados de la plantilla de ejemplo no coinciden con el formato de carga.");
    }
    internal static bool ArchivoAdmitido(string[]? rutas) => rutas is { Length: 1 } && Path.GetExtension(rutas[0]).Equals(".xlsx", StringComparison.OrdinalIgnoreCase);
    internal static void TipoArchivo(string ruta)
    {
        if (!string.Equals(Path.GetExtension(ruta), ".xlsx", StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Solo se admiten archivos .xlsx.");
    }

    internal static void AntesDeImportar(IReadOnlyCollection<ProductoImportado> productos, int? tienda, string ruta, byte[]? hash)
    {
        if (productos.Any(p => p.StockInicial > 0) && tienda is null) throw new InvalidOperationException("Selecciona la tienda para el stock inicial.");
        using var original = File.OpenRead(ruta);
        if (hash is null || !System.Security.Cryptography.SHA256.HashData(original).SequenceEqual(hash))
            throw new InvalidOperationException("El archivo cambió desde la previsualización. Vuelve a seleccionarlo.");
    }
    private static readonly string[] columnasEsperadas = CargaMasivaPlantilla.columnas;
    private const string colObservacion = "Observacion";
    internal static bool TryParseDecimal(string texto, out decimal valor) => ImporteMonetario.TryParse(texto, out valor);
    internal static DataTable LeerArchivo(string ruta)
    {
        TipoArchivo(ruta);
        using var flujo = new FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var libro = new XLWorkbook(flujo);
        IXLWorksheet hoja = libro.Worksheets.First();

        IXLRow? encabezado = hoja.FirstRowUsed();
        if (encabezado is null) throw new InvalidDataException("La hoja está vacía.");

        var columnas = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (IXLCell celda in encabezado.CellsUsed())
        {
            string nombre = celda.GetString().Trim();
            if (!columnas.TryAdd(nombre, celda.Address.ColumnNumber)) throw new InvalidDataException("Hay columnas repetidas: " + nombre);
        }

        string[] faltantes = columnasEsperadas.Where(c => !columnas.ContainsKey(c)).ToArray();
        if (faltantes.Length > 0)
            throw new InvalidDataException("Faltan columnas en el archivo: " + string.Join(", ", faltantes) + ".");

        var tabla = new DataTable();
        foreach (string c in columnasEsperadas) tabla.Columns.Add(c, typeof(string));
        tabla.Columns.Add(colObservacion, typeof(string));

        foreach (IXLRow filaExcel in hoja.RowsUsed().Skip(1))
        {
            DataRow fila = tabla.NewRow();
            foreach (string c in columnasEsperadas) fila[c] = TextoCelda(filaExcel.Cell(columnas[c]));
            fila[colObservacion] = "";
            tabla.Rows.Add(fila);
        }

        if (tabla.Rows.Count == 0) throw new InvalidDataException("El archivo no tiene filas de datos.");
        return tabla;
    }
    private static string TextoCelda(IXLCell celda)
    {
        if (celda.IsEmpty()) return "";
        if (celda.DataType == XLDataType.Number)
            return ((decimal)celda.GetDouble()).ToString(CultureInfo.InvariantCulture);
        return celda.GetString().Trim();
    }
    internal static bool ValidarFila(DataRow fila, CatalogosImportacion cat)
    {
        var errores = new List<string>();
        string Texto(string col) => Convert.ToString(fila[col])?.Trim() ?? "";

        string[] obligatorios = { "CodigoBarras", "Nombre", "Precio", "NombreCategoria", "RTNProveedor" };
        string[] vacios = obligatorios.Where(c => Texto(c).Length == 0).ToArray();
        if (vacios.Length > 0) errores.Add("Falta: " + string.Join(", ", vacios));
        foreach (string campo in new[] { "CodigoBarras", "Nombre", "NombreCategoria", "RTNProveedor" })
            if (ValidacionDatos.SoloCeros(Texto(campo))) errores.Add($"{campo} no puede contener solo ceros");
        foreach (string campo in new[] { "Nombre", "NombreCategoria" })
            if (Texto(campo).Length > 0 && !ValidacionDatos.SoloCeros(Texto(campo)) && !ValidacionDatos.NombreValido(Texto(campo)))
                errores.Add($"{campo} debe contener al menos 3 letras");
        if (Texto("CodigoBarras").Length > 50) errores.Add("El código admite hasta 50 caracteres");
        if (Texto("Nombre").Length > 150) errores.Add("El nombre admite hasta 150 caracteres");

        if (Texto("Precio").Length > 0)
        {
            if (!TryParseDecimal(Texto("Precio"), out decimal precio)) errores.Add("Precio no válido: usa 12.50 o 12,50, hasta dos decimales, sin moneda ni separadores de miles");
            else if (precio <= 0) errores.Add("El precio debe ser mayor a cero");
            else if (precio > 99999999.99m || decimal.Round(precio, 2) != precio) errores.Add("El precio admite hasta 99999999.99 y dos decimales");
        }

        if (Texto("StockInicial").Length > 0 &&
            (!int.TryParse(Texto("StockInicial"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int stock) || stock < 0))
            errores.Add("El stock inicial debe ser un entero mayor o igual a cero");

        string codigo = Texto("CodigoBarras");
        if (codigo.Length > 0)
        {
            if (cat.CodigosBD.Contains(codigo)) errores.Add("El código de barras ya existe en la base de datos");
            else if (!cat.CodigosArchivo.Add(codigo)) errores.Add("Código de barras repetido en el archivo");
        }

        if (Texto("NombreCategoria").Length > 0 && !cat.Categorias.ContainsKey(Texto("NombreCategoria")))
            errores.Add($"La categoría '{Texto("NombreCategoria")}' no existe");
        string rtn = CargaMasivaDatos.NormalizarRtn(Texto("RTNProveedor"));
        if (rtn.Length > 0 && !cat.Proveedores.ContainsKey(rtn))
            errores.Add($"No existe un proveedor con RTN '{Texto("RTNProveedor")}'");

        fila[colObservacion] = string.Join("; ", errores);
        return errores.Count == 0;
    }
    internal static List<ProductoImportado> ConstruirProductosValidos(DataTable tabla, CatalogosImportacion cat)
    {
        var lista = new List<ProductoImportado>();
        foreach (DataRow fila in tabla.Rows)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(fila[colObservacion]))) continue;

            string Texto(string col) => Convert.ToString(fila[col])?.Trim() ?? "";
            if (!TryParseDecimal(Texto("Precio"), out decimal precio))
                throw new InvalidOperationException($"Revisa el precio de la fila {tabla.Rows.IndexOf(fila) + 2} antes de importar.");
            int.TryParse(Texto("StockInicial"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int stock);

            lista.Add(new ProductoImportado(
                Texto("CodigoBarras"),
                Texto("Nombre"),
                Math.Round(precio, 2),
                stock,
                cat.Categorias[Texto("NombreCategoria")],
                cat.Proveedores[CargaMasivaDatos.NormalizarRtn(Texto("RTNProveedor"))], tabla.Rows.IndexOf(fila) + 2));
        }
        return lista;
    }
    internal static void Validar(IReadOnlyCollection<ProductoImportado> productos, int? tienda)
    {
        foreach (var producto in productos)
        {
            ValidacionDatos.NoSoloCeros(producto.CodigoBarras, "Código de barras");
            ValidacionDatos.Nombre(producto.Nombre, "Nombre del producto");
        }

        if (productos.Any(p => string.IsNullOrWhiteSpace(p.CodigoBarras) || p.CodigoBarras.Length > 50 || string.IsNullOrWhiteSpace(p.Nombre) || p.Nombre.Length > 150
            || p.Precio <= 0 || p.Precio > 99999999.99m || decimal.Round(p.Precio, 2) != p.Precio || p.StockInicial < 0)
            || productos.Select(p => p.CodigoBarras.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count() != productos.Count)
            throw new ArgumentException("Revisa códigos únicos, nombres, precios positivos de hasta dos decimales y stock entero no negativo.");
        if (productos.Any(p => p.StockInicial > 0) && tienda is not > 0) throw new ArgumentException("Selecciona la tienda donde se registrará el stock inicial.");


    }
}
