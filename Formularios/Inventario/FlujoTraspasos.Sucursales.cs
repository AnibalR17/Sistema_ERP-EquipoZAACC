using Microsoft.Data.SqlClient;

namespace Sistema_ERP
{
    internal sealed partial class FlujoTraspasos
    {
        private static bool AgregarSucursal(IWin32Window propietario)
        {
            using var ventana = new Form
            {
                Text = "Agregar sucursal",
                ClientSize = new Size(530, 540),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false,
                Font = new Font("Segoe UI", 11F),
                ForeColor = ProductoEstilo.Texto,
                BackColor = Color.White,
                Padding = new Padding(24),
                AutoScaleMode = AutoScaleMode.Font
            };

            var codigo = new TextBox { Name = "txtCodigoSucursal", MaxLength = 20, TabIndex = 0 };
            var nombre = new TextBox { Name = "txtNombreSucursal", MaxLength = 100, TabIndex = 1 };
            var direccion = new TextBox { Name = "txtDireccionSucursal", MaxLength = 300, TabIndex = 2 };
            var telefono = new TextBox { Name = "txtTelefonoSucursal", MaxLength = 30, TabIndex = 3 };
            var contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Margin = Padding.Empty
            };
            contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            contenido.Controls.Add(new Label
            {
                Text = "Agregar sucursal", Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold), Margin = Padding.Empty
            }, 0, 0);
            contenido.Controls.Add(new Label
            {
                Text = "Completa los datos. Los campos con * son obligatorios.",
                Dock = DockStyle.Fill, ForeColor = ProductoEstilo.TextoSecundario,
                Margin = Padding.Empty
            }, 0, 1);

            var campos = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 8, Margin = Padding.Empty };
            campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            int fila = 0;
            foreach (var (etiqueta, campo) in new[]
            {
                ("Código *", codigo), ("Nombre *", nombre),
                ("Dirección", direccion), ("Teléfono", telefono)
            })
            {
                campos.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
                campos.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
                campos.Controls.Add(new Label { Text = etiqueta, Dock = DockStyle.Fill, Margin = Padding.Empty }, 0, fila++);
                campo.Dock = DockStyle.Top;
                campo.Margin = new Padding(0, 0, 0, 8);
                campo.BorderStyle = BorderStyle.FixedSingle;
                campos.Controls.Add(campo, 0, fila++);
            }
            contenido.Controls.Add(campos, 0, 2);
            var estado = new Label { Dock = DockStyle.Fill, ForeColor = ProductoEstilo.Error, Margin = Padding.Empty };
            contenido.Controls.Add(estado, 0, 3);

            var guardar = new Button { Text = "Guardar sucursal", Name = "btnGuardarSucursal" };
            ProductoEstilo.Boton(guardar, true);
            guardar.Margin = new Padding(0, 4, 0, 4);
            guardar.Click += (_, _) =>
            {
                guardar.Enabled = false;
                estado.Text = "";
                try
                {
                    TiendaDatos.Agregar(codigo.Text, nombre.Text, direccion.Text, telefono.Text);
                    ventana.DialogResult = DialogResult.OK;
                }
                catch (SqlException ex) when (ex.Number is 2601 or 2627)
                {
                    estado.Text = "Ya existe una sucursal con ese código.";
                    codigo.Focus();
                }
                catch (ArgumentException ex) { estado.Text = ex.Message; }
                catch (Exception ex) { estado.Text = "No se pudo guardar la sucursal. " + ex.Message; }
                finally { guardar.Enabled = true; }
            };

            var cancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel };
            ProductoEstilo.Boton(cancelar);
            cancelar.Margin = new Padding(0, 4, 10, 4);
            var acciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false, Margin = Padding.Empty
            };
            acciones.Controls.Add(guardar);
            acciones.Controls.Add(cancelar);
            contenido.Controls.Add(acciones, 0, 4);
            ventana.Controls.Add(contenido);
            ventana.AcceptButton = guardar;
            ventana.CancelButton = cancelar;
            ventana.Shown += (_, _) => codigo.Focus();
            return ventana.ShowDialog(propietario) == DialogResult.OK;
        }
    }
}
