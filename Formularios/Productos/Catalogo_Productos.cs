using System;
using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Sistema_ERP
{
    public partial class Catalogo_Productos : UserControl
    {
        private DataView? vistaProductos;

        public event EventHandler? HomeRequested;

        public Catalogo_Productos()
        {
            InitializeComponent();
            if (Sistema_ERP.ModoDiseno.Activo(this)) return;
            ConfigurarPresentacion();
            UsarNavegacionPrincipal();

            homeButton.Click += (_, _) => HomeRequested?.Invoke(this, EventArgs.Empty);
            toggleButton.Click += (_, _) => sidebar.Visible = !sidebar.Visible;

            Load += Catalogo_Productos_Load;
            txtBuscar.TextChanged += TxtBuscar_TextChanged;
            btnNuevoProducto.Click += BtnNuevoProducto_Click;
            btnEditar.Click += BtnEditar_Click;
            btnDesactivar.Click += BtnDesactivar_Click;
            btnReactivarProducto.Click += (_, _) => CambiarEstadoProducto(true);
            chkProductosInactivos.CheckedChanged += (_, _) => CargarProductos();
            dgvProductos.SelectionChanged += (_, _) => ActualizarAccionesProducto();
            dgvProductos.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) BtnEditar_Click(this, EventArgs.Empty); };
        }

        private void Catalogo_Productos_Load(object? sender, EventArgs e)
        {
            if (Sistema_ERP.ModoDiseno.Activo(this)) return;
            ConfigurarGrid();
            CargarProductos();
        }

        private void ConfigurarGrid()
        {
            dgvProductos.AutoGenerateColumns = false;
            dgvProductos.ReadOnly = true;
            dgvProductos.AllowUserToAddRows = false;
            dgvProductos.AllowUserToDeleteRows = false;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.MultiSelect = false;
            dgvProductos.RowHeadersVisible = false;

            dgvProductos.Columns.Clear();
            dgvProductos.Columns.Add(Columna("codigo_barras", "Codigo", 18));
            dgvProductos.Columns.Add(Columna("nombre", "Nombre", 34));
            dgvProductos.Columns.Add(Columna("categoria", "Categoria", 20));
            var stock = Columna("stock_actual", "Stock", 10);
            stock.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvProductos.Columns.Add(stock);
            var precio = Columna("precio_unitario", "Precio", 18);
            precio.DefaultCellStyle.Format = "'L. 'N2";
            precio.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvProductos.Columns.Add(precio);
            dgvProductos.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "activo",
                DataPropertyName = "activo",
                HeaderText = "Activo",
                Width = 65
            });
            dgvProductos.Columns.Add(Columna("motivo_desactivacion", "Motivo de desactivación", 24));

            foreach (string oculta in new[] { "id_producto", "id_categoria", "id_proveedor" })
                dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = oculta, Name = oculta, Visible = false });
        }

        private static DataGridViewTextBoxColumn Columna(string campo, string titulo, float peso) => new()
        {
            DataPropertyName = campo,
            Name = campo,
            HeaderText = titulo,
            FillWeight = peso,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        };

        private void CargarProductos()
        {
            try
            {
                vistaProductos = ProductoDatos.Listar(chkProductosInactivos.Checked).DefaultView;
                dgvProductos.DataSource = vistaProductos;
                AplicarFiltro();
                dgvProductos.ClearSelection();
                ActualizarAccionesProducto();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "No se pudo cargar el catalogo de productos.\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtBuscar_TextChanged(object? sender, EventArgs e) => AplicarFiltro();

        private void AplicarFiltro()
        {
            if (vistaProductos is null) return;

            string texto = txtBuscar.Text.Trim();
            if (texto.Length == 0)
            {
                vistaProductos.RowFilter = "";
                return;
            }

            // Escapa caracteres especiales de LIKE y comillas para que RowFilter no falle.
            string seguro = Regex.Replace(texto, @"[%*\[\]]", "[$0]").Replace("'", "''");
            vistaProductos.RowFilter = $"nombre LIKE '%{seguro}%' OR codigo_barras LIKE '%{seguro}%'";
        }

        private void BtnNuevoProducto_Click(object? sender, EventArgs e)
        {
            using var formulario = new Nuevo_Producto();
            if (formulario.ShowDialog(FindForm()) == DialogResult.OK) CargarProductos();
        }

        private void BtnEditar_Click(object? sender, EventArgs e)
        {
            Producto? producto = ProductoSeleccionado();
            if (producto is null || !producto.Activo)
            {
                MessageBox.Show(this, "Seleccione un producto activo de la tabla.", "Editar",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var formulario = new Nuevo_Producto(producto);
            if (formulario.ShowDialog(FindForm()) == DialogResult.OK) CargarProductos();
        }

        private void BtnDesactivar_Click(object? sender, EventArgs e)
            => CambiarEstadoProducto(false);

        private void ActualizarAccionesProducto()
        {
            Producto? producto = ProductoSeleccionado();
            btnEditar.Enabled = btnDesactivar.Enabled = producto?.Activo == true;
            btnReactivarProducto.Enabled = producto is not null && !producto.Activo;
        }

        private void CambiarEstadoProducto(bool reactivar)
        {
            Producto? producto = ProductoSeleccionado();
            if (producto is null || producto.Activo == reactivar) return;
            string motivo = "";
            if (reactivar)
            {
                if (MessageBox.Show(this, $"¿Reactivar el producto «{producto.Nombre}»?", "Reactivar producto",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
                    != DialogResult.Yes) return;
            }
            else
            {
                using var dialogo = new MotivoDesactivacion(producto.Nombre, "producto");
                if (dialogo.ShowDialog(this) != DialogResult.OK) return;
                motivo = dialogo.Motivo;
            }
            btnDesactivar.Enabled = btnReactivarProducto.Enabled = false;
            try
            {
                bool cambiado = reactivar ? ProductoDatos.Reactivar(producto.IdProducto)
                    : ProductoDatos.Desactivar(producto.IdProducto, motivo);
                CargarProductos();
                MessageBox.Show(this, cambiado
                    ? (reactivar ? "Producto reactivado correctamente." : "Producto desactivado correctamente.")
                    : "El producto ya cambió de estado o no está disponible.",
                    "Productos", MessageBoxButtons.OK, cambiado ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(this, ex.Message, "Productos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show(this, "No se pudo cambiar el estado del producto.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ActualizarAccionesProducto();
            }
        }

        private Producto? ProductoSeleccionado()
        {
            if (dgvProductos.SelectedRows.Count == 0 ||
                dgvProductos.SelectedRows[0].DataBoundItem is not DataRowView fila) return null;

            return new Producto
            {
                IdProducto = Convert.ToInt32(fila["id_producto"]),
                CodigoBarras = Convert.ToString(fila["codigo_barras"]) ?? "",
                Nombre = Convert.ToString(fila["nombre"]) ?? "",
                PrecioUnitario = Convert.ToDecimal(fila["precio_unitario"]),
                StockActual = Convert.ToInt32(fila["stock_actual"]),
                IdCategoria = Convert.ToInt32(fila["id_categoria"]),
                IdProveedor = Convert.ToInt32(fila["id_proveedor"]),
                Activo = Convert.ToBoolean(fila["activo"])
            };
        }
    }
}
