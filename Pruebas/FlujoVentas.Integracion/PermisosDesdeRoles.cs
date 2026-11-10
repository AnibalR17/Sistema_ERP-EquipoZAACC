using System.Data;
using System.Reflection;
using Sistema_ERP;

internal static partial class Program
{
    private static void ProbarPermisosDesdeRoles()
    {
        const BindingFlags instancia = BindingFlags.Instance | BindingFlags.NonPublic;
        int comprobaciones = 0;
        void Comprobar(bool condicion, string contexto) { Igual(true, condicion, contexto); comprobaciones++; }
        T Campo<T>(object formulario, string nombre) => (T)formulario.GetType().GetField(nombre, instancia)!.GetValue(formulario)!;
        void Mostrar(Form formulario)
        {
            formulario.ShowInTaskbar = false; formulario.Opacity = 0;
            formulario.StartPosition = FormStartPosition.Manual; formulario.Location = new Point(-5000, -5000); formulario.Show();
        }
        using var pantalla = new UsuariosRolesPermisos(); Mostrar(pantalla);
        Comprobar(Campo<FlowLayoutPanel>(pantalla, "pestanas").Controls.Cast<Button>().Select(b => b.Text).SequenceEqual(["Usuarios", "Roles"]),
            "La pantalla ofrece únicamente Usuarios y Roles");
        var abrir = Campo<Button>(pantalla, "editarPermisosRol");
        Comprobar(!abrir.Visible, "La edición de permisos no se muestra en Usuarios");
        typeof(UsuariosRolesPermisos).GetMethod("CambiarPestana", instancia)!.Invoke(pantalla, [TipoSeguridad.Roles]);
        Comprobar(abrir.Visible && abrir.Enabled, "Los permisos se crean directamente desde Roles");
        Campo<TextBox>(pantalla, "primero").Text = "PERMISOS_EN_ROL";
        Campo<TextBox>(pantalla, "segundo").Text = "Rol con permisos propios";
        using var opciones = SeguridadDatos.Asignaciones(TipoSeguridad.Roles, null);
        int ventas = opciones.AsEnumerable().Single(f => (string)f["Modulo"] == "VENTAS" && (string)f["Accion"] == "CONSULTAR").Field<int>("Id");
        typeof(UsuariosRolesPermisos).GetMethod("AplicarAsignaciones", instancia)!.Invoke(pantalla, [opciones, new[] { ventas }]);

        using var editor = new EditorPermisosRol("Rol con permisos propios"); Mostrar(editor);
        void Crear(string modulo, string accion, string descripcion)
        {
            Campo<Button>(editor, "nuevo").PerformClick();
            Campo<ComboBox>(editor, "modulo").Text = modulo;
            Campo<ComboBox>(editor, "accion").Text = accion;
            Campo<TextBox>(editor, "descripcion").Text = descripcion;
            Campo<Button>(editor, "guardar").PerformClick();
        }
        int Id(string modulo, string accion) => (int)Tabla($"SELECT id_permiso FROM dbo.Permiso WHERE modulo='{modulo}' AND accion='{accion}'").Rows[0][0];
        void Seleccionar(int id)
        {
            var tabla = Campo<DataGridView>(editor, "tabla"); tabla.ClearSelection();
            var fila = tabla.Rows.Cast<DataGridViewRow>().Single(f => (int)((DataRowView)f.DataBoundItem!)["Id"] == id);
            tabla.CurrentCell = fila.Cells[0]; fila.Selected = true;
        }
        Campo<Button>(editor, "guardar").PerformClick();
        Comprobar(Campo<Label>(editor, "estado").Text.Contains("módulo") && editor.PermisosCreados.Length == 0,
            $"Se validan los campos antes de crear un permiso (guardar={Campo<Button>(editor, "guardar").Enabled}, id={Campo<int?>(editor, "idSeleccionado")}, estado={Campo<Label>(editor, "estado").Text})");
        Crear("ROL_PROPIO", "CONSULTAR", "Consultar módulo propio");
        int consulta = Id("ROL_PROPIO", "CONSULTAR");
        Comprobar(editor.PermisosCreados.SequenceEqual([consulta]), "Guardar permiso crea un registro y devuelve su identificador");
        Crear("ROL_PROPIO", "CONSULTAR", "Duplicado");
        Comprobar(Campo<Label>(editor, "estado").Text.Contains("ya existen") && editor.PermisosCreados.Length == 1,
            "No se duplica una combinación de módulo y acción");
        Crear("ROL_PROPIO", "GESTIONAR", "Gestionar módulo propio");
        int gestion = Id("ROL_PROPIO", "GESTIONAR");
        Crear("ROL_SIN_CONSULTA", "GESTIONAR", "Gestión sin consulta");
        int incompleto = Id("ROL_SIN_CONSULTA", "GESTIONAR");
        Comprobar(editor.PermisosCreados.Length == 3, "Se pueden crear varios permisos sin abandonar el rol");
        Seleccionar(consulta);
        Campo<TextBox>(editor, "descripcion").Text = "Descripción actualizada desde Roles";
        Campo<Button>(editor, "actualizar").PerformClick();
        Comprobar((string)Tabla($"SELECT descripcion FROM dbo.Permiso WHERE id_permiso={consulta}").Rows[0][0] == "Descripción actualizada desde Roles",
            "El editor conserva la actualización de permisos del catálogo");
        Ejecutar($"UPDATE dbo.Permiso SET descripcion=N'Cambio concurrente' WHERE id_permiso={consulta}");
        Campo<TextBox>(editor, "descripcion").Text = "Sobrescribir cambio concurrente";
        Campo<Button>(editor, "actualizar").PerformClick();
        Comprobar(Campo<Label>(editor, "estado").Text.Contains("cambió") &&
            (string)Tabla($"SELECT descripcion FROM dbo.Permiso WHERE id_permiso={consulta}").Rows[0][0] == "Cambio concurrente", "Se rechaza actualizar un permiso con una versión anterior");
        typeof(EditorPermisosRol).GetMethod("Cargar", instancia)!.Invoke(editor, [consulta]);
        typeof(EditorPermisosRol).GetMethod("CambiarEstado", instancia)!.Invoke(editor, [false, "Prueba desde Roles"]);
        Campo<CheckBox>(editor, "inactivos").Checked = true; Seleccionar(consulta);
        Comprobar(!Campo<Button>(editor, "actualizar").Enabled && Campo<Button>(editor, "reactivar").Enabled && !Campo<ComboBox>(editor, "modulo").Enabled,
            "Los permisos inactivos se consultan sin permitir edición");
        Campo<Button>(editor, "reactivar").PerformClick();
        Comprobar((bool)Tabla($"SELECT activo FROM dbo.Permiso WHERE id_permiso={consulta}").Rows[0][0], "Reactivar conserva el permiso y su identificador");
        editor.Size = editor.MinimumSize; editor.PerformLayout();
        Comprobar(Campo<DataGridView>(editor, "tabla").Height > 100, "El editor conserva el listado visible en su tamaño mínimo");
        Campo<TextBox>(editor, "buscar").Text = "MÓDULO_INEXISTENTE_ESP%[";
        Comprobar(Campo<DataGridView>(editor, "tabla").Rows.Count == 0, "La búsqueda admite texto literal y resultados vacíos");
        Campo<TextBox>(editor, "buscar").Clear();
        editor.Controls.Find("cerrar", true).OfType<Button>().Single().PerformClick();

        typeof(UsuariosRolesPermisos).GetMethod("AplicarPermisosCreados", instancia)!.Invoke(pantalla, [new[] { ventas }, editor.PermisosCreados]);
        var seleccionados = Campo<CheckedListBox>(pantalla, "asignaciones").CheckedItems.Cast<object>()
            .Select(o => (int)o.GetType().GetProperty("Id")!.GetValue(o)!).ToHashSet();
        Comprobar(seleccionados.SetEquals([ventas, consulta, gestion]) && !seleccionados.Contains(incompleto),
            "Los nuevos permisos se seleccionan conservando lo anterior y sin conceder gestión sin consulta");
        Comprobar(Campo<TextBox>(pantalla, "primero").Text == "PERMISOS_EN_ROL" && Campo<TextBox>(pantalla, "segundo").Text == "Rol con permisos propios",
            "Volver del editor conserva los datos sin guardar del rol");
        Comprobar((int)Tabla($"SELECT COUNT(*) FROM dbo.RolPermiso WHERE id_permiso IN ({consulta},{gestion},{incompleto})").Rows[0][0] == 0,
            "Crear permisos no los asigna a ningún rol antes de guardar");
        Campo<Button>(pantalla, "guardar").PerformClick();
        int idRol = SeguridadDatos.Listar(TipoSeguridad.Roles, false).AsEnumerable().Single(f => (string)f["Primero"] == "PERMISOS_EN_ROL").Field<int>("Id");
        using var guardados = SeguridadDatos.Asignaciones(TipoSeguridad.Roles, idRol);
        Comprobar(guardados.AsEnumerable().Where(f => f.Field<bool>("Asignado")).Select(f => f.Field<int>("Id")).ToHashSet().SetEquals([ventas, consulta, gestion]),
            "Guardar el rol conserva los permisos creados y seleccionados desde Roles");
        Console.WriteLine($"OK: {comprobaciones} comprobaciones de creación, edición, estados y asignación de permisos desde Roles.");
    }
}
