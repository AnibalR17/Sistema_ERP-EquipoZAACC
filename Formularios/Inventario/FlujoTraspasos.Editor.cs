using System.Data;
using Sistema_ERP.Clases;

namespace Sistema_ERP
{
    internal sealed partial class FlujoTraspasos
    {
        private sealed class EditorTraspaso : UserControl
        {
            internal event EventHandler? Guardado;

            internal EditorTraspaso(DataRow? fila, Func<int> obtenerUsuario, bool mostrarCabecera = true)
            {
                Text = fila == null ? "Solicitar traspaso" : $"Editar traspaso #{fila["id_traspaso"]}";
                Dock = DockStyle.Fill;
                TraspasoEstilo.Superficie(this);
                if (!mostrarCabecera) Padding = Padding.Empty;
                var cabecera = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true };
                var origen = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
                var destino = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
                var notas = new TextBox { Width = 260, MaxLength = 500 };
                cabecera.Controls.AddRange(new[] { TraspasoEstilo.Campo("Tienda de origen", origen), TraspasoEstilo.Campo("Tienda de destino", destino), TraspasoEstilo.Campo("Observaciones", notas) });
                var lineas = Tabla();
                lineas.ReadOnly = false;
                lineas.AllowUserToAddRows = true;
                lineas.AllowUserToDeleteRows = true;
                lineas.AutoGenerateColumns = false;
                var datos = new DataTable();
                datos.Columns.Add("id_producto", typeof(int));
                datos.Columns.Add("cantidad", typeof(int));
                datos.Columns.Add("precio_referencia", typeof(decimal));
                var productos = new DataTable();
                productos.Columns.Add("id_producto", typeof(int));
                productos.Columns.Add("nombre", typeof(string));
                productos.Columns.Add("precio_unitario", typeof(decimal));
                productos.Columns.Add("disponible", typeof(int));
                lineas.Columns.Add(new DataGridViewComboBoxColumn { HeaderText = "Producto", DataPropertyName = "id_producto", DataSource = productos, ValueMember = "id_producto", DisplayMember = "nombre" });
                lineas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cantidad", DataPropertyName = "cantidad" });
                lineas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Precio de referencia", DataPropertyName = "precio_referencia" });
                lineas.Columns.Add(new DataGridViewTextBoxColumn { Name = "disponible", HeaderText = "Disponible en origen", ReadOnly = true });

                void ActualizarProducto(DataGridViewRow renglon, bool actualizarPrecio)
                {
                    if (renglon.IsNewRow || renglon.Cells[0].Value is not int idProducto) return;
                    var producto = productos.AsEnumerable().FirstOrDefault(p => p.Field<int>("id_producto") == idProducto);
                    if (producto == null) return;
                    renglon.Cells["disponible"].Value = producto["disponible"];
                    if (actualizarPrecio)
                        renglon.Cells[2].Value = producto["precio_unitario"];
                }

                lineas.CurrentCellDirtyStateChanged += (_, _) =>
                {
                    if (lineas.CurrentCell is DataGridViewComboBoxCell && lineas.IsCurrentCellDirty)
                        lineas.CommitEdit(DataGridViewDataErrorContexts.Commit);
                };
                bool cargandoDatos = true;
                lineas.CellValueChanged += (_, e) =>
                {
                    if (!cargandoDatos && e.RowIndex >= 0 && e.ColumnIndex == 0)
                        ActualizarProducto(lineas.Rows[e.RowIndex], true);
                };
                lineas.DataBindingComplete += (_, _) =>
                {
                    foreach (DataGridViewRow renglon in lineas.Rows)
                        ActualizarProducto(renglon, false);
                };
                origen.SelectedIndexChanged += (_, _) => Ejecutar(() =>
                {
                    if (cargandoDatos) return;
                    datos.Clear();
                    productos.Clear();
                    productos.Merge(TraspasoDatos.Productos(origen.SelectedValue is int seleccion ? seleccion : 0));
                });
                bool errorEdicion = false;
                lineas.DataError += (_, e) =>
                {
                    errorEdicion = true;
                    e.ThrowException = false;
                    MessageBox.Show("Revise el producto, la cantidad entera y el precio numérico.");
                };
                lineas.DataSource = datos;
                bool limpiandoFilas = false;
                void QuitarFilasVacias()
                {
                    if (cargandoDatos || limpiandoFilas || lineas.IsDisposed) return;
                    limpiandoFilas = true;
                    try
                    {
                        if (!lineas.EndEdit()) return;
                        if (BindingContext?[datos] is CurrencyManager administrador)
                            administrador.EndCurrentEdit();
                        foreach (DataRow renglon in datos.Rows.Cast<DataRow>()
                            .Where(r => r.RowState != DataRowState.Deleted &&
                                datos.Columns.Cast<DataColumn>().All(c =>
                                    r.IsNull(c) || r[c] is string texto && string.IsNullOrWhiteSpace(texto)))
                            .ToArray())
                            datos.Rows.Remove(renglon);
                    }
                    finally { limpiandoFilas = false; }
                }
                lineas.RowLeave += (_, _) =>
                {
                    if (!limpiandoFilas && !cargandoDatos && IsHandleCreated)
                        BeginInvoke(new Action(QuitarFilasVacias));
                };
                lineas.Leave += (_, _) =>
                {
                    QuitarFilasVacias();
                    if (!lineas.IsCurrentCellInEditMode)
                        lineas.AllowUserToAddRows = false;
                };
                lineas.Enter += (_, _) => lineas.AllowUserToAddRows = true;
                if (fila != null)
                {
                    notas.Text = Convert.ToString(fila["observaciones"]);
                }
                var guardar = new Button
                {
                    Name = "btnGuardarSolicitud",
                    Text = fila == null ? "Guardar solicitud" : "Guardar cambios",
                    Enabled = false
                };
                void ActualizarTiendas()
                {
                    if (cargandoDatos || fila != null) return;
                    object? origenAnterior = origen.SelectedValue;
                    object? destinoAnterior = destino.SelectedValue;
                    var tiendas = TiendaDatos.Listar();
                    cargandoDatos = true;
                    try
                    {
                        Enlazar(origen, tiendas, "id_tienda", "Nombre");
                        Enlazar(destino, tiendas.Copy(), "id_tienda", "Nombre");
                        origen.SelectedIndex = destino.SelectedIndex = -1;
                        if (origenAnterior is int o) origen.SelectedValue = o;
                        if (destinoAnterior is int d) destino.SelectedValue = d;
                    }
                    finally { cargandoDatos = false; }
                }
                origen.DropDown += (_, _) => Ejecutar(ActualizarTiendas);
                destino.DropDown += (_, _) => Ejecutar(ActualizarTiendas);
                guardar.Click += (_, _) => Ejecutar(() =>
                {
                    int usuario = obtenerUsuario();
                    errorEdicion = false;
                    if (!lineas.EndEdit() || errorEdicion)
                        return;
                    if (BindingContext?[datos] is CurrencyManager administrador)
                        administrador.EndCurrentEdit();
                    QuitarFilasVacias();
                    var detalle = datos.Rows.Cast<DataRow>().Where(r => r.RowState != DataRowState.Deleted)
                        .Select(r => new LineaTraspaso(Convert.ToInt32(r["id_producto"]), Convert.ToInt32(r["cantidad"]), Convert.ToDecimal(r["precio_referencia"]))).ToArray();
                    int tiendaOrigen = origen.SelectedValue is int o ? o : 0;
                    int tiendaDestino = destino.SelectedValue is int d ? d : 0;
                    if (fila == null)
                    {
                        int numero = Ingresar.Traspaso(tiendaOrigen, tiendaDestino, usuario, notas.Text.Trim(), detalle);
                        datos.Clear();
                        origen.SelectedIndex = -1;
                        destino.SelectedIndex = -1;
                        notas.Clear();
                        MessageBox.Show($"Traspaso #{numero} creado. Estado: PENDIENTE.", "Traspasos",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                        Modificar.Traspaso((int)fila["id_traspaso"], (byte[])fila["version_fila"],
                            tiendaOrigen, tiendaDestino, usuario, notas.Text.Trim(), detalle);
                    Guardado?.Invoke(this, EventArgs.Empty);
                });
                var layout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3,
                    Margin = Padding.Empty
                };
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
                layout.Controls.Add(cabecera, 0, 0);
                layout.Controls.Add(TraspasoEstilo.Tarjeta(lineas), 0, 1);
                Controls.Add(layout);
                if (mostrarCabecera)
                    Controls.Add(TraspasoEstilo.Cabecera(Text, fila == null
                        ? "Selecciona las tiendas y agrega los productos que deseas traspasar."
                        : "Edita productos y observaciones. Las tiendas y el solicitante se conservan."));
                TraspasoEstilo.Boton(guardar, true);
                guardar.Dock = DockStyle.None;
                var acciones = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft,
                    WrapContents = false, Margin = Padding.Empty, Padding = new Padding(0, 8, 0, 0)
                };
                acciones.Controls.Add(guardar);
                if (mostrarCabecera)
                {
                    var cerrar = new Button { Text = "Cerrar" };
                    TraspasoEstilo.Boton(cerrar);
                    cerrar.Click += (_, _) => FindForm()?.Close();
                    acciones.Controls.Add(cerrar);
                }
                layout.Controls.Add(acciones, 0, 2);
                Load += (_, _) => Ejecutar(() =>
                {
                    Enlazar(origen, TiendaDatos.Listar(fila != null), "id_tienda", "Nombre");
                    Enlazar(destino, TiendaDatos.Listar(fila != null), "id_tienda", "Nombre");
                    if (fila != null)
                    {
                        origen.SelectedValue = fila["id_tienda_origen"];
                        destino.SelectedValue = fila["id_tienda_destino"];
                        origen.Enabled = false;
                        destino.Enabled = false;
                    }
                    productos.Merge(TraspasoDatos.Productos(origen.SelectedValue is int tienda ? tienda : 0, fila != null));
                    if (fila != null)
                        datos.Merge(TraspasoDatos.Detalle((int)fila["id_traspaso"]));
                    cargandoDatos = false;
                    guardar.Enabled = true;
                });
            }
        }
    }

}
