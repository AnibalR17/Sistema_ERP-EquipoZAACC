using System.Data;

namespace Sistema_ERP
{
    public partial class Pagina_Central
    {
        private readonly ComboBox selectorTienda = new()
        {
            Name = "selectorTienda",
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 11F),
            Top = 18,
            AccessibleName = "Tienda para consultar el catálogo",
            TabIndex = 2
        };

        internal int? IdTiendaSeleccionada =>
            selectorTienda.SelectedValue is int id && id > 0 ? id : null;

        internal event EventHandler? TiendaSeleccionadaChanged;

        private void ConfigurarSelectorTienda()
        {
            header.Controls.Add(selectorTienda);
            Shown += (_, _) => CargarTiendasCabecera();
            selectorTienda.DropDown += (_, _) => CargarTiendasCabecera();
            selectorTienda.SelectedIndexChanged += (_, _) =>
                TiendaSeleccionadaChanged?.Invoke(this, EventArgs.Empty);
        }

        private void CargarTiendasCabecera()
        {
            int? seleccionAnterior = IdTiendaSeleccionada;
            var tiendas = new DataTable();
            tiendas.Columns.Add("id_tienda", typeof(int));
            tiendas.Columns.Add("Nombre", typeof(string));
            tiendas.Rows.Add(0, "Todas las tiendas (stock total)");
            try
            {
                foreach (DataRow tienda in TiendaDatos.Listar().Rows)
                    tiendas.Rows.Add(tienda["id_tienda"], tienda["Nombre"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "No se pudo cargar la lista de tiendas.\n" + ex.Message,
                    "Tiendas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            selectorTienda.DisplayMember = "Nombre";
            selectorTienda.ValueMember = "id_tienda";
            selectorTienda.DataSource = tiendas;
            selectorTienda.SelectedValue = seleccionAnterior.HasValue && tiendas.AsEnumerable()
                .Any(t => t.Field<int>("id_tienda") == seleccionAnterior.Value)
                ? seleccionAnterior.Value : 0;
        }
    }
}
