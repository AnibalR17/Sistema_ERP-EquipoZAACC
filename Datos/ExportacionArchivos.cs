using System.Data;
using ClosedXML.Excel;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Sistema_ERP;

internal static class ExportacionArchivos
{
    internal static void CatalogoExcel(DataTable productos, string ruta, string tienda, string filtro, bool incluirInactivos)
    {
        if (productos.Rows.Count == 0) throw new ArgumentException("No hay productos para exportar con los filtros actuales.");
        GuardarArchivo(ruta, temporal =>
        {
            using var libro = new XLWorkbook();
            var hoja = libro.Worksheets.Add("Catalogo");
            string[] titulos = ["Código", "Nombre", "Categoría", "Stock", "Precio base (L)", "Estado", "Motivo de desactivación"];
            for (int i = 0; i < titulos.Length; i++) hoja.Cell(1, i + 1).Value = titulos[i];
            hoja.Column(1).Style.NumberFormat.Format = "@";
            int fila = 2;
            foreach (DataRow p in productos.Rows)
            {
                hoja.Cell(fila, 1).Value = Convert.ToString(p["codigo_barras"]) ?? "";
                hoja.Cell(fila, 2).Value = Convert.ToString(p["nombre"]) ?? "";
                hoja.Cell(fila, 3).Value = Convert.ToString(p["categoria"]) ?? "";
                hoja.Cell(fila, 4).Value = Convert.ToInt32(p["stock_actual"]);
                hoja.Cell(fila, 5).Value = Convert.ToDecimal(p["precio_unitario"]);
                hoja.Cell(fila, 6).Value = Convert.ToBoolean(p["activo"]) ? "Activo" : "Inactivo";
                hoja.Cell(fila, 7).Value = Convert.ToString(p["motivo_desactivacion"]) ?? "";
                fila++;
            }
            hoja.Range(1, 1, fila - 1, titulos.Length).CreateTable().Theme = XLTableTheme.TableStyleMedium2;
            hoja.SheetView.FreezeRows(1); hoja.Columns().AdjustToContents();
            hoja.Column(2).Width = 40; hoja.Column(7).Width = 45;
            hoja.Column(5).Style.NumberFormat.Format = "\"L \"#,##0.00";
            var informacion = libro.Worksheets.Add("Información");
            (string Campo, string Valor)[] datos = [
                ("Catálogo de productos", "Sistema ERP"), ("Tienda", tienda), ("Búsqueda", filtro.Length == 0 ? "Todos los productos" : filtro),
                ("Incluir inactivos", incluirInactivos ? "Sí" : "No"), ("Productos exportados", productos.Rows.Count.ToString()),
                ("Generado (Honduras)", InicioDatos.AhoraHonduras.ToString("dd/MM/yyyy HH:mm:ss")),
                ("Precio", "Precio base antes de ISV"), ("Stock", "Existencia de la tienda seleccionada, o stock total de todas las tiendas") ];
            for (int i = 0; i < datos.Length; i++) { informacion.Cell(i + 1, 1).Value = datos[i].Campo; informacion.Cell(i + 1, 2).Value = datos[i].Valor; }
            informacion.Column(1).Width = 28; informacion.Column(1).Style.Font.Bold = true;
            informacion.Column(2).Width = 90; informacion.Column(2).Style.Alignment.WrapText = true; informacion.Rows().AdjustToContents();
            libro.SaveAs(temporal);
        });
    }

    internal static void ReciboPdf(string ruta, string numero, string contenido)
    {
        if (string.IsNullOrWhiteSpace(contenido)) throw new ArgumentException("El recibo no tiene contenido para exportar.");
        GuardarArchivo(ruta, temporal =>
        {
            using var documento = new PdfDocument();
            documento.Info.Title = "Recibo de venta " + numero; documento.Info.Author = "Sistema ERP";
            var fuente = new XFont("Segoe UI", 10, XFontStyleEx.Regular);
            var titulo = new XFont("Segoe UI", 15, XFontStyleEx.Bold);
            var pie = new XFont("Segoe UI", 9, XFontStyleEx.Regular);
            PdfPage? pagina = null; XGraphics? grafico = null;
            double y = 0; const double margen = 40, altoLinea = 16;
            void NuevaPagina()
            {
                grafico?.Dispose(); pagina = documento.AddPage(); pagina.Size = PdfSharp.PageSize.A4;
                grafico = XGraphics.FromPdfPage(pagina);
                grafico.DrawString("Sistema ERP · Recibo de venta", titulo, XBrushes.Black, new XRect(margen, 28, pagina.Width.Point - 2 * margen, 25), XStringFormats.TopLeft);
                grafico.DrawLine(XPens.LightGray, margen, 60, pagina.Width.Point - margen, 60);
                grafico.DrawString("Página " + documento.PageCount, pie, XBrushes.Gray, new XRect(margen, pagina.Height.Point - 35, pagina.Width.Point - 2 * margen, 20), XStringFormats.TopLeft);
                y = 76;
            }
            try
            {
                NuevaPagina();
                foreach (string linea in contenido.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
                {
                    string texto = new(linea.Replace("\t", "    ").Where(c => !char.IsControl(c)).ToArray());
                    foreach (string renglon in AjustarLinea(texto, grafico!, fuente, pagina!.Width.Point - 2 * margen).ToArray())
                    {
                        if (y + altoLinea > pagina!.Height.Point - 52) NuevaPagina();
                        if (renglon.Length > 0) grafico!.DrawString(renglon, fuente, XBrushes.Black,
                            new XRect(margen, y, pagina!.Width.Point - 2 * margen, altoLinea), XStringFormats.TopLeft);
                        y += altoLinea;
                    }
                }
            }
            finally { grafico?.Dispose(); }
            documento.Save(temporal);
        });
    }

    private static IEnumerable<string> AjustarLinea(string texto, XGraphics grafico, XFont fuente, double ancho)
    {
        if (texto.Length == 0) { yield return ""; yield break; }
        while (texto.Length > 0)
        {
            if (grafico.MeasureString(texto, fuente).Width <= ancho) { yield return texto; yield break; }
            int inicio = 1, fin = texto.Length, caben = 1;
            while (inicio <= fin)
            {
                int mitad = inicio + (fin - inicio) / 2;
                if (grafico.MeasureString(texto[..mitad], fuente).Width <= ancho) { caben = mitad; inicio = mitad + 1; }
                else fin = mitad - 1;
            }
            int espacio = texto.LastIndexOf(' ', Math.Min(caben - 1, texto.Length - 1));
            int corte = espacio > 0 ? espacio : caben;
            if (corte < texto.Length && char.IsHighSurrogate(texto[corte - 1]) && char.IsLowSurrogate(texto[corte])) corte = corte > 1 ? corte - 1 : 2;
            yield return texto[..corte].TrimEnd(); texto = texto[corte..].TrimStart();
        }
    }

    private static void GuardarArchivo(string ruta, Action<string> escribir)
    {
        string destino = Path.GetFullPath(ruta);
        string temporal = Path.Combine(Path.GetDirectoryName(destino)!, "." + Path.GetFileNameWithoutExtension(destino) + "." + Guid.NewGuid().ToString("N") + Path.GetExtension(destino));
        try { escribir(temporal); File.Move(temporal, destino, true); }
        finally { if (File.Exists(temporal)) File.Delete(temporal); }
    }
}
