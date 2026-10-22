using System;
using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Sistema_ERP
{
    public partial class Catalogo_Productos : UserControl
    {
        private DataView? _vista;

        public event EventHandler? HomeRequested;

        public Catalogo_Productos()
        {
            InitializeComponent();
            ConfigurarPresentacion();
            UsarNavegacionPrincipal();

            homeButton.Click += (_, _) => HomeRequested?.Invoke(this, EventArgs.Empty);
            toggleButton.Click += (_, _) => sidebar.Visible = !sidebar.Visible;

            Load += Catalogo_Productos_Load;
            txtBuscar.TextChanged += TxtBuscar_TextChanged;
            btnNuevoProducto.Click += BtnNuevoProducto_Click;
            btnEditar.Click += BtnEditar_Click;
            btnDesactivar.Click += BtnDesactivar_Click;
            dgvProductos.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) BtnEditar_Click(this, EventArgs.Empty); };
        }

        private void Catalogo_Productos_Load(object? sender, EventArgs e)
        {
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
                _vista = ProductoDatos.Listar().DefaultView;
                dgvProductos.DataSource = _vista;
                AplicarFiltro(); 
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
            if (_vista is null) return;

            string texto = txtBuscar.Text.Trim();
            if (texto.Length == 0)
            {
                _vista.RowFilter = "";
                return;
            }

            // Escapa caracteres especiales de LIKE y comillas para que RowFilter no falle.
            string seguro = Regex.Replace(texto, @"[%*\[\]]", "[$0]").Replace("'", "''");
            _vista.RowFilter = $"nombre LIKE '%{seguro}%' OR codigo_barras LIKE '%{seguro}%'";
        }

        private void BtnNuevoProducto_Click(object? sender, EventArgs e)
        {
            using var formulario = new Nuevo_Producto();
            if (formulario.ShowDialog(FindForm()) == DialogResult.OK) CargarProductos();
        }

        private void BtnEditar_Click(object? sender, EventArgs e)
        {
            Producto? producto = ProductoSeleccionado();
            if (producto is null)
            {
                MessageBox.Show(this, "Seleccione un producto de la tabla.", "Editar",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var formulario = new Nuevo_Producto(producto);
            if (formulario.ShowDialog(FindForm()) == DialogResult.OK) CargarProductos();
        }

        private void BtnDesactivar_Click(object? sender, EventArgs e)
        {
            Producto? producto = ProductoSeleccionado();
            if (producto is null)
            {
                MessageBox.Show(this, "Seleccione un producto de la tabla.", "Desactivar",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirmar = MessageBox.Show(this, $"¿Desactivar el producto \"{producto.Nombre}\"?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmar != DialogResult.Yes) return;

            try
            {
                if (!ProductoDatos.Desactivar(producto.IdProducto))
                    MessageBox.Show(this, "El producto ya esta inactivo o no esta disponible.", "Desactivar");
                CargarProductos();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(this, ex.Message, "No se puede desactivar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show(this, "No se pudo desactivar el producto.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Producto? ProductoSeleccionado()
        {
            if (dgvProductos.CurrentRow?.DataBoundItem is not DataRowView fila) return null;

            return new Producto
            {
                IdProducto = Convert.ToInt32(fila["id_producto"]),
                CodigoBarras = Convert.ToString(fila["codigo_barras"]) ?? "",
                Nombre = Convert.ToString(fila["nombre"]) ?? "",
                PrecioUnitario = Convert.ToDecimal(fila["precio_unitario"]),
                StockActual = Convert.ToInt32(fila["stock_actual"]),
                IdCategoria = Convert.ToInt32(fila["id_categoria"]),
                IdProveedor = Convert.ToInt32(fila["id_proveedor"])
            };
        }
    }
}
