using System.Data;

namespace Sistema_ERP;

public sealed class SelectorRoles : UserControl
{
    private sealed record Rol(int Id, string Nombre, bool Activo)
    {
        public override string ToString() => Nombre;
    }

    private readonly ComboBox disponibles = new()
    {
        Name = "rolesDisponibles", DropDownStyle = ComboBoxStyle.DropDownList,
        Dock = DockStyle.Fill, DropDownWidth = 360
    };
    private readonly ListBox asignados = new()
    {
        Name = "rolesAsignados", Dock = DockStyle.Fill,
        IntegralHeight = false, HorizontalScrollbar = true
    };
    private readonly Button agregar = new() { Name = "agregarRol", Text = "Agregar", AutoSize = true };
    private readonly Button quitar = new() { Name = "quitarRol", Text = "Quitar", AutoSize = true };
    private Rol[] opciones = [];

    internal int[] IdsSeleccionados => asignados.Items.Cast<Rol>().Select(r => r.Id).ToArray();
    internal int[] IdsActivosSeleccionados => asignados.Items.Cast<Rol>().Where(r => r.Activo).Select(r => r.Id).ToArray();
    internal bool TieneRoles => opciones.Length > 0;
    internal Control[] ControlesEdicion => [disponibles, agregar, quitar];
    internal event EventHandler? SeleccionCambiada;

    public SelectorRoles()
    {
        MinimumSize = new Size(210, 120);
        Size = new Size(400, 140);
        var contenido = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Margin = Padding.Empty
        };
        contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        contenido.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        contenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        disponibles.Margin = new Padding(0, 3, 8, 8);
        asignados.Margin = new Padding(0, 0, 8, 0);
        ProductoEstilo.Boton(agregar); ProductoEstilo.Boton(quitar);
        agregar.AutoSizeMode = quitar.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        agregar.Padding = quitar.Padding = new Padding(8, 4, 8, 4);
        agregar.Margin = new Padding(0, 0, 0, 8); quitar.Margin = Padding.Empty;
        contenido.Controls.Add(disponibles, 0, 0); contenido.Controls.Add(agregar, 1, 0);
        contenido.Controls.Add(asignados, 0, 1); contenido.Controls.Add(quitar, 1, 1);
        Controls.Add(contenido);
        disponibles.SelectedIndexChanged += (_, _) => ActualizarBotones();
        asignados.SelectedIndexChanged += (_, _) => ActualizarBotones();
        agregar.Click += (_, _) =>
        {
            if (disponibles.SelectedItem is not Rol rol || !rol.Activo) return;
            asignados.Items.Add(rol);
            asignados.SelectedItem = rol;
            ActualizarDisponibles();
            SeleccionCambiada?.Invoke(this, EventArgs.Empty);
        };
        quitar.Click += (_, _) =>
        {
            int indice = asignados.SelectedIndex;
            if (indice < 0) return;
            asignados.Items.RemoveAt(indice);
            if (asignados.Items.Count > 0) asignados.SelectedIndex = Math.Min(indice, asignados.Items.Count - 1);
            ActualizarDisponibles();
            SeleccionCambiada?.Invoke(this, EventArgs.Empty);
        };
        ActualizarBotones();
    }

    internal void Cargar(DataTable roles, IEnumerable<int> seleccionados)
    {
        var elegidos = seleccionados.ToHashSet();
        opciones = roles.AsEnumerable().Select(f => new Rol((int)f["Id"], (string)f["Nombre"], (bool)f["activo"])).ToArray();
        asignados.BeginUpdate();
        try
        {
            asignados.Items.Clear();
            foreach (var rol in opciones.Where(r => elegidos.Contains(r.Id))) asignados.Items.Add(rol);
        }
        finally { asignados.EndUpdate(); }
        ActualizarDisponibles();
        SeleccionCambiada?.Invoke(this, EventArgs.Empty);
    }

    private void ActualizarDisponibles()
    {
        int? seleccionado = (disponibles.SelectedItem as Rol)?.Id;
        var elegidos = IdsSeleccionados.ToHashSet();
        disponibles.BeginUpdate();
        try
        {
            disponibles.Items.Clear();
            foreach (var rol in opciones.Where(r => r.Activo && !elegidos.Contains(r.Id))) disponibles.Items.Add(rol);
            disponibles.SelectedItem = disponibles.Items.Cast<Rol>().FirstOrDefault(r => r.Id == seleccionado);
            if (disponibles.SelectedIndex < 0 && disponibles.Items.Count > 0) disponibles.SelectedIndex = 0;
        }
        finally { disponibles.EndUpdate(); }
        ActualizarBotones();
    }

    private void ActualizarBotones()
    {
        disponibles.Enabled = disponibles.Items.Count > 0;
        agregar.Enabled = disponibles.SelectedItem is Rol;
        quitar.Enabled = asignados.SelectedIndex >= 0;
    }
}
