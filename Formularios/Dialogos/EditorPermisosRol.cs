using System.Data;
using Microsoft.Data.SqlClient;

namespace Sistema_ERP;

public sealed partial class EditorPermisosRol : FormularioTematico
{
    private sealed record Modulo(string Codigo, string Nombre)
    {
        public override string ToString() => $"{Nombre} ({Codigo})";
    }

    private readonly HashSet<int> creados = [];
    private DataView? vista;
    private int? idSeleccionado;
    private byte[]? version;
    private bool activo, cargando, disponible;

    public int[] PermisosCreados => creados.Order().ToArray();

    public EditorPermisosRol() : this("") { }

    public EditorPermisosRol(string nombreRol)
    {
        InitializeComponent();
        TemaAplicacion.Aplicar(this);
        titulo.Text = string.IsNullOrWhiteSpace(nombreRol) ? "Permisos del nuevo rol" : $"Permisos de {nombreRol.Trim()}";
        foreach (var pantalla in CatalogoPermisos.Pantallas) modulo.Items.Add(new Modulo(pantalla.Modulo, pantalla.Nombre));
        accion.Items.AddRange(["CONSULTAR", "GESTIONAR", "EXPORTAR"]);
        nuevo.Click += (_, _) => Limpiar();
        guardar.Click += (_, _) => Guardar(false);
        actualizar.Click += (_, _) => Guardar(true);
        desactivar.Click += (_, _) =>
        {
            if (!disponible || !idSeleccionado.HasValue || !activo) return;
            using var motivo = new MotivoDesactivacion($"{CodigoModulo()}.{accion.Text}", "permiso");
            if (motivo.ShowDialog(this) == DialogResult.OK) CambiarEstado(false, motivo.Motivo);
        };
        reactivar.Click += (_, _) => CambiarEstado(true, "");
        RecargaManual.Agregar(acciones, () => Cargar(idSeleccionado));
        tabla.SelectionChanged += (_, _) => { if (!cargando) Seleccionar(); };
        tabla.CellFormatting += (_, e) =>
        {
            if (tabla.Columns[e.ColumnIndex].DataPropertyName == "Primero" && e.Value is string codigo)
                e.Value = CatalogoPermisos.NombrePantalla(codigo);
        };
        buscar.TextChanged += (_, _) => Filtrar();
        inactivos.CheckedChanged += (_, _) => Cargar(idSeleccionado);
        PermisosInterfaz.Configurar(this, Modulos.Seguridad, guardar, actualizar, nuevo, desactivar, reactivar);
        Botones();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!ModoDiseno.Activo(this)) Cargar();
    }

    private void Cargar(int? seleccionar = null) => Ejecutar(() =>
    {
        disponible = false;
        cargando = true;
        try
        {
            vista = SeguridadDatos.Listar(TipoSeguridad.Permisos, inactivos.Checked).DefaultView;
            tabla.DataSource = vista;
            disponible = true;
            Filtrar();
        }
        catch
        {
            vista = null; tabla.DataSource = null;
            throw;
        }
        finally { cargando = false; Limpiar(); }
        if (seleccionar.HasValue)
        {
            var fila = tabla.Rows.Cast<DataGridViewRow>().FirstOrDefault(f =>
                (int)((DataRowView)f.DataBoundItem!)["Id"] == seleccionar.Value);
            if (fila is not null) { tabla.CurrentCell = fila.Cells[0]; fila.Selected = true; Seleccionar(); }
        }
    });

    private string CodigoModulo() => modulo.SelectedItem is Modulo seleccionado ? seleccionado.Codigo : modulo.Text.Trim();

    private void Limpiar()
    {
        cargando = true;
        try
        {
            tabla.ClearSelection(); idSeleccionado = null; version = null; activo = false;
            modulo.SelectedIndex = accion.SelectedIndex = -1;
            modulo.Text = accion.Text = ""; descripcion.Clear(); estado.Text = "";
        }
        finally { cargando = false; Botones(); }
    }

    private void Seleccionar()
    {
        if (tabla.SelectedRows.Count != 1 || tabla.SelectedRows[0].DataBoundItem is not DataRowView fila) { Limpiar(); return; }
        idSeleccionado = (int)fila["Id"]; version = (byte[])fila["version_fila"]; activo = (bool)fila["activo"];
        string codigo = (string)fila["Primero"];
        modulo.SelectedItem = modulo.Items.Cast<Modulo>().FirstOrDefault(m => string.Equals(m.Codigo, codigo, StringComparison.OrdinalIgnoreCase));
        if (modulo.SelectedIndex < 0) modulo.Text = codigo;
        accion.Text = (string)fila["Segundo"]; descripcion.Text = (string)fila["Descripcion"];
        estado.Text = ""; Botones();
    }

    private void Guardar(bool editar)
    {
        if (!disponible || (editar && (!idSeleccionado.HasValue || !activo))) return;
        Ejecutar(() =>
        {
            if (string.IsNullOrWhiteSpace(CodigoModulo())) throw new ArgumentException("Selecciona o escribe el módulo del permiso.");
            if (string.IsNullOrWhiteSpace(accion.Text)) throw new ArgumentException("Selecciona o escribe la acción del permiso.");
            if (string.IsNullOrWhiteSpace(descripcion.Text)) throw new ArgumentException("Escribe la descripción del permiso.");
            int id = SeguridadDatos.Guardar(TipoSeguridad.Permisos, editar ? idSeleccionado : null, editar ? version : null,
                CodigoModulo().ToUpperInvariant(), accion.Text.Trim().ToUpperInvariant(), descripcion.Text, null, "", []);
            if (!editar) creados.Add(id);
            Cargar(id);
            if (disponible) Mensaje(editar ? "Permiso actualizado en el catálogo." : "Permiso creado. Al volver, se preparará su asignación al rol.");
        });
    }

    private void CambiarEstado(bool activar, string motivo)
    {
        if (!disponible || !idSeleccionado.HasValue || version is null || activo == activar) return;
        Ejecutar(() =>
        {
            int id = idSeleccionado.Value;
            SeguridadDatos.CambiarEstado(TipoSeguridad.Permisos, id, version, activar, motivo);
            Cargar(id);
            if (disponible) Mensaje(activar ? "Permiso reactivado." : "Permiso desactivado; sus relaciones se conservan.");
        });
    }

    private void Filtrar()
    {
        if (vista is null) return;
        string texto = buscar.Text.Trim().Replace("'", "''").Replace("[", "[[]").Replace("%", "[%]").Replace("*", "[*]");
        vista.RowFilter = $"Primero LIKE '%{texto}%' OR Segundo LIKE '%{texto}%' OR Descripcion LIKE '%{texto}%'";
    }

    private void Botones()
    {
        guardar.Enabled = nuevo.Enabled = disponible;
        guardar.Enabled &= !idSeleccionado.HasValue;
        actualizar.Enabled = desactivar.Enabled = disponible && idSeleccionado.HasValue && activo;
        reactivar.Enabled = disponible && idSeleccionado.HasValue && !activo;
        modulo.Enabled = accion.Enabled = disponible && (!idSeleccionado.HasValue || activo);
        descripcion.ReadOnly = !modulo.Enabled;
    }

    private void Ejecutar(Action operacion)
    {
        try { operacion(); }
        catch (SqlException ex) when (ex.Number is 2601 or 2627) { Mensaje("Ese módulo y acción ya existen. Incluye los inactivos para revisarlos.", true); }
        catch (Exception ex) { Mensaje(ex.Message, true); }
        finally { Botones(); }
    }

    private void Mensaje(string texto, bool error = false)
    {
        estado.Text = texto; estado.ForeColor = error ? ProductoEstilo.Error : ProductoEstilo.Exito;
    }
}
