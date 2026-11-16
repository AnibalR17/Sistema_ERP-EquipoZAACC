using System.Data;

namespace Sistema_ERP;

internal sealed class SeleccionarClienteApartado : FormularioTematico
{
    private readonly DataGridView clientes = PantallaMaqueta.Tabla();
    internal int? IdCliente { get; private set; }

    internal SeleccionarClienteApartado(DataTable datos)
    {
        Text = "Seleccionar cliente existente"; Name = "seleccionarClienteApartado";
        Size = new Size(940, 580); MinimumSize = new Size(740, 420);
        StartPosition = FormStartPosition.CenterParent; Padding = new Padding(16); Font = new Font("Segoe UI", 10);
        clientes.Name = "clientesExistentes"; clientes.MultiSelect = false; clientes.AutoGenerateColumns = false;
        foreach (var (campo, titulo) in new[] { ("id_cliente", "Código"), ("nombre", "Nombre"), ("telefono", "Teléfono"), ("correo", "Correo"), ("documento_fiscal", "RTN / documento fiscal") })
            clientes.Columns.Add(new DataGridViewTextBoxColumn { Name = campo, DataPropertyName = campo, HeaderText = titulo });
        clientes.DataSource = datos;
        var elegir = PantallaMaqueta.Boton("Usar cliente seleccionado", true); elegir.Name = "usarCliente";
        void Seleccionar()
        {
            if (clientes.CurrentRow?.DataBoundItem is not DataRowView fila) return;
            IdCliente = (int)fila["id_cliente"]; DialogResult = DialogResult.OK;
        }
        elegir.Click += (_, _) => Seleccionar();
        clientes.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) Seleccionar(); };
        clientes.SelectionChanged += (_, _) => elegir.Enabled = clientes.CurrentRow is not null;
        var cerrar = PantallaMaqueta.Boton("Cancelar"); cerrar.DialogResult = DialogResult.Cancel; CancelButton = cerrar;
        var acciones = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true }; acciones.Controls.AddRange([elegir, cerrar]);
        var ayuda = new Label { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(0, 0, 0, 12),
            Text = datos.Rows.Count == 0 ? "No hay clientes activos. Registra o reactiva un cliente en el directorio antes de crear el apartado."
                : "Busca por nombre, teléfono, correo, RTN o código y selecciona un cliente activo del directorio." };
        Controls.Add(BusquedaTabla.Crear(clientes)); Controls.Add(acciones); Controls.Add(ayuda);
        elegir.Enabled = datos.Rows.Count > 0;
    }
}
