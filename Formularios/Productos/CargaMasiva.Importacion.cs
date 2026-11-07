using System.Data;
using System.Globalization;
using ClosedXML.Excel;
using Sistema_ERP.Datos;

namespace Sistema_ERP
{
    public partial class CargaMasiva
    {
        private static readonly string[] ColumnasEsperadas =
            { "CodigoBarras", "Nombre", "Precio", "StockInicial", "NombreCategoria", "RTNProveedor" };

        private const string ColObservacion = "Observacion";

        private DataTable? _tablaPrevia;
        private Catalogos? _catalogos;

        private sealed class Catalogos
        {
            public HashSet<string> CodigosBD { get; init; } = new(StringComparer.OrdinalIgnoreCase);
            public HashSet<string> CodigosArchivo { get; } = new(StringComparer.OrdinalIgnoreCase);
            public Dictionary<string, int> Categorias { get; init; } = new(StringComparer.OrdinalIgnoreCase);
            public Dictionary<string, int> Proveedores { get; init; } = new(StringComparer.OrdinalIgnoreCase);
        }

        private void ConfigurarPrevisualizacion()
        {
            dgvPrevia.ReadOnly = true;
            dgvPrevia.AllowUserToAddRows = false;
            dgvPrevia.RowHeadersVisible = false;
            dgvPrevia.EnableHeadersVisualStyles = false;
            dgvPrevia.BackgroundColor = Color.White;
            dgvPrevia.BorderStyle = BorderStyle.None;
            dgvPrevia.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvPrevia.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvPrevia.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvPrevia.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(125, 125, 125);
            dgvPrevia.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvPrevia.CellFormatting += (_, e) =>
            {
                if (e.RowIndex < 0 || dgvPrevia.Rows[e.RowIndex].DataBoundItem is not DataRowView fila) return;
                if (!string.IsNullOrEmpty(Convert.ToString(fila[ColObservacion])))
                {
                    e.CellStyle.BackColor = Color.FromArgb(255, 199, 199);
                    e.CellStyle.ForeColor = Color.FromArgb(110, 0, 0);
                    e.CellStyle.SelectionBackColor = Color.FromArgb(235, 150, 150);
                }
            };

            btnImportar.Enabled = false;
            btnImportar.Click += (_, _) => ImportarDatos();
        }

        private void CargarArchivo(string ruta)
        {
            if (!string.Equals(Path.GetExtension(ruta), ".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(this, "Solo se admiten archivos .xlsx.", "Carga masiva",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataTable tabla = LeerArchivo(ruta);
                _catalogos = new Catalogos
                {
                    CodigosBD = CargaMasivaDatos.CodigosExistentes(),
                    Categorias = CargaMasivaDatos.MapaCategorias(),
                    Proveedores = CargaMasivaDatos.MapaProveedoresPorRtn()
                };

                int validas = 0;
                foreach (DataRow fila in tabla.Rows)
                    if (ValidarFila(fila, _catalogos)) validas++;

                _tablaPrevia = tabla;
                dgvPrevia.DataSource = tabla;
                dgvPrevia.Columns[ColObservacion]!.FillWeight = 200;

                int conError = tabla.Rows.Count - validas;
                lblResumen.Text = $"{tabla.Rows.Count} filas: {validas} válidas, {conError} con error.";
                btnImportar.Enabled = validas > 0;
            }
            catch (InvalidDataException ex)
            {
                MessageBox.Show(this, ex.Message, "Archivo no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "No se pudo leer el archivo.\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static DataTable LeerArchivo(string ruta)
        {
            using var flujo = new FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var libro = new XLWorkbook(flujo);
            IXLWorksheet hoja = libro.Worksheets.First();

            IXLRow? encabezado = hoja.FirstRowUsed();
            if (encabezado is null) throw new InvalidDataException("La hoja está vacía.");

            var columnas = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (IXLCell celda in encabezado.CellsUsed())
                columnas[celda.GetString().Trim()] = celda.Address.ColumnNumber;

            string[] faltantes = ColumnasEsperadas.Where(c => !columnas.ContainsKey(c)).ToArray();
            if (faltantes.Length > 0)
                throw new InvalidDataException("Faltan columnas en el archivo: " + string.Join(", ", faltantes) + ".");

            var tabla = new DataTable();
            foreach (string c in ColumnasEsperadas) tabla.Columns.Add(c, typeof(string));
            tabla.Columns.Add(ColObservacion, typeof(string));

            foreach (IXLRow filaExcel in hoja.RowsUsed().Skip(1))
            {
                DataRow fila = tabla.NewRow();
                foreach (string c in ColumnasEsperadas) fila[c] = TextoCelda(filaExcel.Cell(columnas[c]));
                fila[ColObservacion] = "";
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

        private static bool ValidarFila(DataRow fila, Catalogos cat)
        {
            var errores = new List<string>();
            string Texto(string col) => Convert.ToString(fila[col])?.Trim() ?? "";

            string[] obligatorios = { "CodigoBarras", "Nombre", "Precio", "NombreCategoria", "RTNProveedor" };
            string[] vacios = obligatorios.Where(c => Texto(c).Length == 0).ToArray();
            if (vacios.Length > 0) errores.Add("Falta: " + string.Join(", ", vacios));

            if (Texto("Precio").Length > 0)
            {
                if (!TryParseDecimal(Texto("Precio"), out decimal precio)) errores.Add("El precio no es un número válido");
                else if (precio <= 0) errores.Add("El precio debe ser mayor a cero");
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

            fila[ColObservacion] = string.Join("; ", errores);
            return errores.Count == 0;
        }

        private static bool TryParseDecimal(string texto, out decimal valor) =>
            decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out valor) ||
            decimal.TryParse(texto, NumberStyles.Number, CultureInfo.CurrentCulture, out valor);

        private void ImportarDatos()
        {
            if (_tablaPrevia is null || _catalogos is null) return;

            List<ProductoImportado> validos = ConstruirProductosValidos(_tablaPrevia, _catalogos);
            int omitidas = _tablaPrevia.Rows.Count - validos.Count;
            if (validos.Count == 0)
            {
                MessageBox.Show(this, "No hay filas válidas para importar.", "Carga masiva",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string aviso = omitidas > 0 ? $"\nSe omitirán {omitidas} filas con error." : "";
            var confirmar = MessageBox.Show(this, $"Se importarán {validos.Count} productos.{aviso}\n¿Continuar?",
                "Confirmar importación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmar != DialogResult.Yes) return;

            try
            {
                int insertados = CargaMasivaDatos.ImportarProductos(validos);
                MessageBox.Show(this, $"Importación completada: {insertados} productos registrados.", "Carga masiva",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReiniciarPrevisualizacion();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(this, ex.Message, "Importación cancelada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show(this, "No se pudo importar. No se guardó ningún producto.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static List<ProductoImportado> ConstruirProductosValidos(DataTable tabla, Catalogos cat)
        {
            var lista = new List<ProductoImportado>();
            foreach (DataRow fila in tabla.Rows)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(fila[ColObservacion]))) continue;

                string Texto(string col) => Convert.ToString(fila[col])?.Trim() ?? "";
                TryParseDecimal(Texto("Precio"), out decimal precio);
                int.TryParse(Texto("StockInicial"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int stock);

                lista.Add(new ProductoImportado(
                    Texto("CodigoBarras"),
                    Texto("Nombre"),
                    Math.Round(precio, 2),
                    stock,
                    cat.Categorias[Texto("NombreCategoria")],
                    cat.Proveedores[CargaMasivaDatos.NormalizarRtn(Texto("RTNProveedor"))]));
            }
            return lista;
        }

        private void ReiniciarPrevisualizacion()
        {
            _tablaPrevia = null;
            _catalogos = null;
            dgvPrevia.DataSource = null;
            lblResumen.Text = "";
            btnImportar.Enabled = false;
        }
    }
}