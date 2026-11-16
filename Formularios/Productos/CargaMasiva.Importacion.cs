using System.Data;

namespace Sistema_ERP
{
    public partial class CargaMasiva
    {
        private const string colObservacion = "Observacion";

        private DataTable? tablaPrevia;
        private Catalogos? catalogos;
        private string? rutaOriginal;
        private byte[]? hashOriginal;

        private sealed class Catalogos : CatalogosImportacion { }

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
                if (!string.IsNullOrEmpty(Convert.ToString(fila[colObservacion])))
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
            try { ImportarProductosValidacion.TipoArchivo(ruta); }
            catch (InvalidDataException ex)
            {
                MessageBox.Show(this, ex.Message, "Carga masiva", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                ReiniciarPrevisualizacion();
                DataTable tabla = LeerArchivo(ruta);
                catalogos = new Catalogos
                {
                    CodigosBD = CargaMasivaDatos.CodigosExistentes(),
                    Categorias = CargaMasivaDatos.MapaCategorias(),
                    Proveedores = CargaMasivaDatos.MapaProveedoresPorRtn()
                };

                int validas = 0;
                foreach (DataRow fila in tabla.Rows)
                    if (ValidarFila(fila, catalogos)) validas++;

                tablaPrevia = tabla;
                rutaOriginal = ruta;
                using (var original = File.OpenRead(ruta)) hashOriginal = System.Security.Cryptography.SHA256.HashData(original);
                dgvPrevia.DataSource = tabla;
                dgvPrevia.Columns[colObservacion]!.FillWeight = 200;

                int conError = tabla.Rows.Count - validas;
                lblResumen.Text = $"{tabla.Rows.Count} filas: {validas} válidas, {conError} con error.";
                btnImportar.Enabled = validas > 0;
                archivo.Text = Path.GetFileName(ruta);
                btnCancelarCarga.Enabled = true;
                pestanasCarga.SelectedTab = tabPrevisualizacion;
            }
            catch (InvalidDataException ex)
            {
                ReiniciarPrevisualizacion();
                MessageBox.Show(this, ex.Message, "Archivo no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                ReiniciarPrevisualizacion();
                MessageBox.Show(this, "No se pudo leer el archivo.\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static DataTable LeerArchivo(string ruta)
        { return ImportarProductosValidacion.LeerArchivo(ruta); }



        private static bool ValidarFila(DataRow fila, Catalogos cat)
        { return ImportarProductosValidacion.ValidarFila(fila, cat); }

        private static bool TryParseDecimal(string texto, out decimal valor) =>
            ImportarProductosValidacion.TryParseDecimal(texto, out valor);

        private void ImportarDatos()
        {
            if (tablaPrevia is null || catalogos is null) return;

            List<ProductoImportado> validos = ConstruirProductosValidos(tablaPrevia, catalogos);
            int omitidas = tablaPrevia.Rows.Count - validos.Count;
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
                ImportarProductosValidacion.AntesDeImportar(validos, cmbTiendaImportacion.SelectedValue as int?, rutaOriginal!, hashOriginal);
                int insertados = CargaMasivaDatos.ImportarProductos(validos,
                    cmbTiendaImportacion.SelectedValue is int tienda ? tienda : null, Path.GetFileName(rutaOriginal), hashOriginal);
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
        { return ImportarProductosValidacion.ConstruirProductosValidos(tabla, cat); }

        private void ReiniciarPrevisualizacion()
        {
            tablaPrevia = null;
            catalogos = null;
            rutaOriginal = null;
            hashOriginal = null;
            dgvPrevia.DataSource = null;
            lblResumen.Text = "";
            btnImportar.Enabled = false;
            btnCancelarCarga.Enabled = false;
            archivo.Text = textoSeleccionArchivo;
        }
    }
}
