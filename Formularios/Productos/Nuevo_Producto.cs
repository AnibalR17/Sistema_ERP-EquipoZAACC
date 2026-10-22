using Microsoft.Data.SqlClient;

namespace Sistema_ERP
{
    public partial class Nuevo_Producto : Form
    {
        private readonly Producto? _editando;

        public Nuevo_Producto(Producto? producto = null)
        {
            InitializeComponent();
            ConfigurarPresentacion();
            _editando = producto;

            Load += FormProducto_Load;
            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += (_, _) => DialogResult = DialogResult.Cancel;
            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;
        }

        private void FormProducto_Load(object? sender, EventArgs e)
        {
            // Stock: solo lectura, valor por defecto 0
            txtStock.ReadOnly = true;
            txtStock.TabStop = false;
            txtStock.Text = "0";

            nudPrecio.DecimalPlaces = 2;
            nudPrecio.Minimum = 0;
            nudPrecio.Maximum = 99999999.99m;
            nudPrecio.ThousandsSeparator = true;

            try
            {
                ConfigurarCombo(cmbCategoria, CategoriaDatos.Listar(), "nombre", "id_categoria");
                ConfigurarCombo(cmbProveedor, ProveedorDatos.ListarParaCombo(), "nombre_empresa", "id_proveedor");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "No se pudieron cargar las categorias y proveedores.\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                return;
            }

            if (_editando is not null)
            {
                Text = "Editar producto";
                titleLabel.Text = "Editar producto";
                btnGuardar.Text = "Guardar cambios";
                txtCodigo.Text = _editando.CodigoBarras;
                txtNombre.Text = _editando.Nombre;
                nudPrecio.Value = Math.Min(_editando.PrecioUnitario, nudPrecio.Maximum);
                txtStock.Text = _editando.StockActual.ToString();
                cmbCategoria.SelectedValue = _editando.IdCategoria;
                cmbProveedor.SelectedValue = _editando.IdProveedor;
            }
            else
            {
                Text = "Nuevo producto";
            }
        }

        private static void ConfigurarCombo(ComboBox combo, System.Data.DataTable datos, string mostrar, string valor)
        {
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.DisplayMember = mostrar;
            combo.ValueMember = valor;
            combo.DataSource = datos;
            combo.SelectedIndex = -1;
        }

        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (!Validar(out int idCategoria, out int idProveedor)) return;

            var producto = new Producto
            {
                IdProducto = _editando?.IdProducto ?? 0,
                CodigoBarras = txtCodigo.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                PrecioUnitario = nudPrecio.Value,
                IdCategoria = idCategoria,
                IdProveedor = idProveedor
            };

            try
            {
                if (ProductoDatos.ExisteCodigo(producto.CodigoBarras, _editando?.IdProducto))
                {
                    Error("Ya existe un producto con ese codigo de barras.", txtCodigo);
                    return;
                }

                if (_editando is null) ProductoDatos.Agregar(producto);
                else ProductoDatos.Editar(producto);

                DialogResult = DialogResult.OK;
            }
            catch (SqlException)
            {
                MessageBox.Show(this, "No se pudo guardar el producto. Verifique la conexion a la base de datos.",
                    "Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool Validar(out int idCategoria, out int idProveedor)
        {
            idCategoria = idProveedor = 0;

            if (string.IsNullOrWhiteSpace(txtCodigo.Text)) { Error("El codigo de barras es obligatorio.", txtCodigo); return false; }
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { Error("El nombre es obligatorio.", txtNombre); return false; }
            if (nudPrecio.Value <= 0) { Error("El precio unitario debe ser un numero mayor a cero.", nudPrecio); return false; }
            if (cmbCategoria.SelectedValue is not int cat) { Error("Seleccione una categoria.", cmbCategoria); return false; }
            if (cmbProveedor.SelectedValue is not int prov) { Error("Seleccione un proveedor.", cmbProveedor); return false; }

            idCategoria = cat;
            idProveedor = prov;
            return true;
        }

        private void Error(string mensaje, Control foco)
        {
            MessageBox.Show(this, mensaje, "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
            foco.Focus();
        }
    }
}
