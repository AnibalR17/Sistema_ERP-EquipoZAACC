using ClosedXML.Excel;

namespace Sistema_ERP;

internal static class CargaMasivaPlantilla
{
    internal static readonly string[] Columnas = ["CodigoBarras", "Nombre", "Precio", "StockInicial", "NombreCategoria", "RTNProveedor"];

    internal static void Crear(string ruta)
    {
        var categorias = CargaMasivaDatos.MapaCategorias(); var proveedores = CargaMasivaDatos.MapaProveedoresPorRtn();
        var existentes = CargaMasivaDatos.CodigosExistentes();
        using var libro = new XLWorkbook();
        var productos = libro.Worksheets.Add("Productos");
        for (int i = 0; i < Columnas.Length; i++) productos.Cell(1, i + 1).Value = Columnas[i];
        productos.Column(1).Style.NumberFormat.Format = "@"; productos.Column(6).Style.NumberFormat.Format = "@";
        productos.Column(3).Style.NumberFormat.Format = "0.00";
        string categoria = categorias.Keys.OrderBy(c => c).FirstOrDefault() ?? "COMPLETAR_CATEGORIA_EXISTENTE";
        string rtn = proveedores.Keys.OrderBy(c => c).FirstOrDefault() ?? "COMPLETAR_RTN_PROVEEDOR";
        int secuencia = 1;
        for (int fila = 2; fila <= 11; fila++)
        {
            string codigo; do { codigo = "000990" + secuencia++.ToString("D6"); } while (existentes.Contains(codigo));
            productos.Cell(fila, 1).Value = codigo; productos.Cell(fila, 2).Value = "Producto de prueba " + (fila - 1).ToString("D2");
            productos.Cell(fila, 3).Value = 10m + (fila - 1) * 2.5m; productos.Cell(fila, 4).Value = fila % 3 == 0 ? 0 : 5;
            productos.Cell(fila, 5).Value = categoria; productos.Cell(fila, 6).Value = rtn;
        }
        productos.Range(1, 1, 11, 6).CreateTable().Theme = XLTableTheme.TableStyleMedium2;
        productos.SheetView.FreezeRows(1); productos.Columns().AdjustToContents(); productos.Column(2).Width = 36;
        var catalogos = libro.Worksheets.Add("Catalogos");
        catalogos.Cell(1, 1).Value = "NombreCategoria"; catalogos.Cell(1, 3).Value = "RTNProveedor";
        catalogos.Column(3).Style.NumberFormat.Format = "@";
        int r = 2; foreach (string c in categorias.Keys.OrderBy(c => c)) catalogos.Cell(r++, 1).Value = c;
        r = 2; foreach (string p in proveedores.Keys.OrderBy(p => p)) catalogos.Cell(r++, 3).Value = p;
        catalogos.Range("A1:C1").Style.Font.Bold = true; catalogos.Columns().AdjustToContents();
        var instrucciones = libro.Worksheets.Add("Instrucciones");
        string[] notas = ["PLANTILLA DE CARGA MASIVA · PRODUCTOS", "Edita únicamente los datos de la primera hoja Productos; conserva la fila de encabezados.",
            "CodigoBarras: texto obligatorio, único, máximo 50 caracteres. Conserva los ceros iniciales.", "Nombre: obligatorio, máximo 150 caracteres.",
            "Precio: celda numérica positiva, hasta dos decimales y máximo 99999999.99; sin símbolo de moneda ni separadores de miles.",
            "StockInicial: entero no negativo; una celda vacía equivale a cero. Selecciona en la pantalla la tienda donde se cargará el stock.",
            "NombreCategoria: nombre de una categoría activa, exactamente como aparece en Catalogos; no uses su ID.",
            "RTNProveedor: RTN de un proveedor activo existente, guardado como texto; no uses el ID ni el nombre de empresa.",
            "Se admiten guiones y espacios en el RTN; el sistema los elimina para comparar.",
            "Catalogos refleja la base de datos al generar el archivo; actualiza la plantilla si cambian categorías o proveedores.",
            "Si ves COMPLETAR en algún ejemplo, primero registra o corrige la categoría o el RTN del proveedor y sustituye ese valor.",
            "La vista previa marca las filas con error. Importar datos válidos omite las filas inválidas tras tu confirmación.",
            "Los diez productos son datos ficticios de prueba. Importarlos sí los guarda y registra su stock y auditoría."];
        for (int i = 0; i < notas.Length; i++) instrucciones.Cell(i + 1, 1).Value = notas[i];
        instrucciones.Column(1).Width = 110; instrucciones.Column(1).Style.Alignment.WrapText = true; instrucciones.Row(1).Style.Font.Bold = true; instrucciones.Rows().AdjustToContents();
        libro.SaveAs(ruta);
    }
}
