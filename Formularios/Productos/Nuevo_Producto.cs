using Microsoft.Data.SqlClient;

namespace Sistema_ERP
{
    public partial class Nuevo_Producto : FormularioTematico
    {
        private readonly Producto? editando;

        public Nuevo_Producto() : this(null) { }

        public Nuevo_Producto(Producto? producto)
        {
            InitializeComponent();
            if (Sistema_ERP.ModoDiseno.Activo(this)) return;
            ConfigurarPresentacion();
            editando = producto;
            var reiniciar = RecargaManual.Agregar(this, CargarReferencias);
            reiniciar.SetBounds(32, 582, 120, 44);
            reiniciar.TabIndex = 8;
            PermisosInterfaz.Configurar(this, Modulos.productos, btnGuardar);

            Load += FormProducto_Load;
            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += (_, _) => DialogResult = DialogResult.Cancel;
            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;
        }

        private void FormProducto_Load(object? sender, EventArgs e)
        {
            if (Sistema_ERP.ModoDiseno.Activo(this)) return;
            txtStock.ReadOnly = true;
            txtStock.TabStop = false;
            txtStock.Text = "0";

            nudPrecio.DecimalPlaces = 2;
            nudPrecio.Minimum = 0;
            nudPrecio.Maximum = 99999999.99m;
            nudPrecio.ThousandsSeparator = true;

            try
            {
                CargarReferencias();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "No se pudieron cargar las categorias y proveedores.\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                return;
            }

            if (editando is not null)
            {
                Text = "Editar producto";
                titleLabel.Text = "Editar producto";
                btnGuardar.Text = "Guardar cambios";
                txtCodigo.Text = editando.CodigoBarras;
                txtNombre.Text = editando.Nombre;
                nudPrecio.Value = Math.Min(editando.PrecioUnitario, nudPrecio.Maximum);
                txtStock.Text = editando.StockActual.ToString();
                cmbCategoria.SelectedValue = editando.IdCategoria;
                cmbProveedor.SelectedValue = editando.IdProveedor;
            }
            else
            {
                Text = "Nuevo producto";
            }
        }

        private void CargarReferencias()
        {
            object? categoria = cmbCategoria.SelectedValue, proveedor = cmbProveedor.SelectedValue;
            var categorias = CategoriaDatos.Listar();
            var proveedores = ProveedorDatos.ListarParaCombo();
            ConfigurarCombo(cmbCategoria, categorias, "nombre", "id_categoria");
            ConfigurarCombo(cmbProveedor, proveedores, "nombre_empresa", "id_proveedor");
            if (categoria is int c) cmbCategoria.SelectedValue = c;
            if (proveedor is int p) cmbProveedor.SelectedValue = p;
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
                IdProducto = editando?.IdProducto ?? 0,
                CodigoBarras = txtCodigo.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                PrecioUnitario = nudPrecio.Value,
                IdCategoria = idCategoria,
                IdProveedor = idProveedor
            };

            try
            {
                GuardarProductoValidacion.CodigoDisponible(ProductoDatos.ExisteCodigo(producto.CodigoBarras, editando?.IdProducto));

                if (editando is null) ProductoDatos.Agregar(producto);
                else ProductoDatos.Editar(producto);

                DialogResult = DialogResult.OK;
            }
            catch (ArgumentException ex)
            {
                Error(ex.Message, txtCodigo);
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

            try
            {
                (idCategoria, idProveedor) = GuardarProductoValidacion.Captura(txtCodigo.Text, txtNombre.Text, nudPrecio.Value,
                    cmbCategoria.SelectedValue as int?, cmbProveedor.SelectedValue as int?);
                return true;
            }
            catch (ErrorCampoValidacion ex)
            {
                Control foco = ex.Campo switch { "Nombre" => txtNombre, "PrecioUnitario" => nudPrecio,
                    "IdCategoria" => cmbCategoria, "IdProveedor" => cmbProveedor, _ => txtCodigo };
                Error(ex.Message, foco); return false;
            }
        }

        private void Error(string mensaje, Control foco)
        {
            MessageBox.Show(this, mensaje, "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
            foco.Focus();
        }
    }
}
