using System.Data;
using System.Reflection;
using Sistema_ERP;

internal static partial class Program
{
    private static void ProbarPermisosRolPorModulo()
    {
        const BindingFlags instancia = BindingFlags.Instance | BindingFlags.NonPublic;
        int comprobaciones = 0;
        void Comprobar(bool condicion, string contexto) { Igual(true, condicion, contexto); comprobaciones++; }
        T Campo<T>(object formulario, string nombre) => (T)formulario.GetType().GetField(nombre, instancia)!.GetValue(formulario)!;
        void Mostrar(Form formulario)
        {
            formulario.ShowInTaskbar = false; formulario.Opacity = 0;
            formulario.StartPosition = FormStartPosition.Manual; formulario.Location = new Point(-5000, -5000);
            formulario.Show();
        }
        T Control<T>(Form formulario, string nombre) where T : Control => formulario.Controls.Find(nombre, true).OfType<T>().Single();
        void Modulo(AsignarPermisosRol dialogo, string modulo) => Control<ListBox>(dialogo, "listaModulos").SelectedItem = modulo;
        void Marcar(AsignarPermisosRol dialogo, string accion, bool marcado)
        {
            var lista = Control<CheckedListBox>(dialogo, "listaPermisos");
            int indice = Enumerable.Range(0, lista.Items.Count).Single(i => lista.Items[i].ToString()!.StartsWith(accion + " ", StringComparison.Ordinal));
            lista.SetItemChecked(indice, marcado);
        }
        Ejecutar("""
            INSERT dbo.Permiso(modulo,accion,descripcion) VALUES
                ('MODULO_PRUEBA','CONSULTAR',N'Consulta personalizada'),
                ('MODULO_PRUEBA','GESTIONAR',N'Gestión personalizada'),
                ('MODULO_PRUEBA','EXPORTAR',N'Exportación personalizada'),
                ('MODULO_SIN_CONSULTA','GESTIONAR',N'Gestión sin consulta disponible');
            INSERT dbo.Permiso(modulo,accion,descripcion,desactivado_en)
                VALUES('MODULO_PRUEBA','ARCHIVAR',N'Permiso inactivo',SYSUTCDATETIME());
            """);
        using var catalogo = SeguridadDatos.Asignaciones(TipoSeguridad.Roles, null);
        int Id(string modulo, string accion) => catalogo.AsEnumerable().Single(f => (string)f["Modulo"] == modulo && (string)f["Accion"] == accion).Field<int>("Id");
        int consultaProductos = Id("PRODUCTOS", "CONSULTAR"), gestionProductos = Id("PRODUCTOS", "GESTIONAR"),
            consultaVentas = Id("VENTAS", "CONSULTAR"), exportar = Id("MODULO_PRUEBA", "EXPORTAR");
        Comprobar(catalogo.AsEnumerable().All(f => !f.IsNull("Modulo") && !f.IsNull("Accion") && !f.IsNull("Descripcion")),
            "El catálogo aporta módulo, acción y descripción para cada permiso");
        using (var dialogo = new AsignarPermisosRol("Rol por módulos", catalogo, []))
        {
            Mostrar(dialogo);
            var modulos = Control<ListBox>(dialogo, "listaModulos");
            Comprobar(modulos.Items.Cast<string>().Distinct(StringComparer.OrdinalIgnoreCase).Count() == modulos.Items.Count,
                "Cada módulo aparece una sola vez");
            Modulo(dialogo, "PRODUCTOS"); Marcar(dialogo, "GESTIONAR", true);
            Comprobar(dialogo.PermisosSeleccionados.ToHashSet().SetEquals([consultaProductos, gestionProductos]) &&
                Control<CheckedListBox>(dialogo, "listaPermisos").CheckedItems.Count == 2, "Gestionar incluye Consultar y actualiza ambas casillas");
            Modulo(dialogo, "VENTAS"); Marcar(dialogo, "CONSULTAR", true);
            Modulo(dialogo, "PRODUCTOS");
            Comprobar(dialogo.PermisosSeleccionados.Length == 3 && Control<CheckedListBox>(dialogo, "listaPermisos").CheckedItems.Count == 2,
                "Cambiar de módulo conserva las selecciones anteriores");
            Marcar(dialogo, "CONSULTAR", false);
            Comprobar(dialogo.PermisosSeleccionados.SequenceEqual([consultaVentas]) && Control<CheckedListBox>(dialogo, "listaPermisos").CheckedItems.Count == 0,
                "Quitar Consultar retira Gestionar solo del mismo módulo");
            Control<Button>(dialogo, "seleccionarModulo").PerformClick();
            Comprobar(dialogo.PermisosSeleccionados.ToHashSet().SetEquals([consultaProductos, gestionProductos, consultaVentas]),
                "Seleccionar módulo agrega sus permisos activos sin afectar otros módulos");
            Control<Button>(dialogo, "quitarModulo").PerformClick();
            Comprobar(dialogo.PermisosSeleccionados.SequenceEqual([consultaVentas]), "Quitar del módulo conserva los permisos de los demás módulos");
            Modulo(dialogo, "MODULO_PRUEBA"); Marcar(dialogo, "EXPORTAR", true);
            Comprobar(dialogo.PermisosSeleccionados.Contains(exportar), "Se admiten acciones adicionales del catálogo");
            Modulo(dialogo, "MODULO_SIN_CONSULTA"); Marcar(dialogo, "GESTIONAR", true);
            Comprobar(dialogo.PermisosSeleccionados.Length == 2 && Control<Label>(dialogo, "mensaje").Text.Contains("CONSULTAR activo"),
                "No se asigna Gestionar si falta un permiso Consultar activo");
            Control<Button>(dialogo, "seleccionarModulo").PerformClick();
            Comprobar(dialogo.PermisosSeleccionados.Length == 2, "Seleccionar módulo también respeta la dependencia de consulta");
            dialogo.Size = dialogo.MinimumSize; dialogo.PerformLayout();
            Comprobar(Control<CheckedListBox>(dialogo, "listaPermisos").Height > 80, "El tamaño mínimo conserva espacio para seleccionar permisos");
            Control<Button>(dialogo, "cancelar").PerformClick();
            Comprobar(dialogo.DialogResult == DialogResult.Cancel && SeguridadDatos.Listar(TipoSeguridad.Roles, false).AsEnumerable().All(f => (string)f["Primero"] != "MODULOS_ROL"),
                "Cancelar no crea el rol ni guarda permisos");
        }

        using (var pantalla = new UsuariosRolesPermisos())
        {
            Mostrar(pantalla);
            var boton = Control<Button>(pantalla, "asignarPermisosModulo");
            Comprobar(!boton.Visible, "La opción por módulo no se muestra en Usuarios");
            typeof(UsuariosRolesPermisos).GetMethod("CambiarPestana", instancia)!.Invoke(pantalla, [TipoSeguridad.Roles]);
            Comprobar(boton.Visible && boton.Enabled && !Campo<int?>(pantalla, "idSeleccionado").HasValue,
                "La opción está disponible al crear un rol nuevo");
            Campo<TextBox>(pantalla, "primero").Text = "MODULOS_ROL";
            Campo<TextBox>(pantalla, "segundo").Text = "Rol por módulos";
            using (var dialogo = new AsignarPermisosRol("Rol por módulos", catalogo, []))
            {
                Mostrar(dialogo);
                Modulo(dialogo, "PRODUCTOS"); Control<Button>(dialogo, "seleccionarModulo").PerformClick();
                Modulo(dialogo, "VENTAS"); Marcar(dialogo, "CONSULTAR", true);
                Modulo(dialogo, "MODULO_PRUEBA"); Marcar(dialogo, "EXPORTAR", true);
                Control<Button>(dialogo, "aceptar").PerformClick();
                Comprobar(dialogo.DialogResult == DialogResult.OK, "Aceptar devuelve los permisos seleccionados");
                typeof(UsuariosRolesPermisos).GetMethod("AplicarAsignaciones", instancia)!.Invoke(pantalla, [catalogo, dialogo.PermisosSeleccionados]);
            }
            Comprobar(Campo<CheckedListBox>(pantalla, "asignaciones").CheckedItems.Count == 4, "La selección se aplica al formulario del rol");
            Campo<Button>(pantalla, "guardar").PerformClick();
            var rol = SeguridadDatos.Listar(TipoSeguridad.Roles, false).AsEnumerable().Single(f => (string)f["Primero"] == "MODULOS_ROL");
            int idRol = (int)rol["Id"];
            using var guardados = SeguridadDatos.Asignaciones(TipoSeguridad.Roles, idRol);
            Comprobar(guardados.AsEnumerable().Where(f => (bool)f["Asignado"]).Select(f => (int)f["Id"]).ToHashSet()
                .SetEquals([consultaProductos, gestionProductos, consultaVentas, exportar]), "Guardar crea el rol con los permisos de todos los módulos elegidos");

            var tabla = Campo<DataGridView>(pantalla, "tabla");
            tabla.ClearSelection();
            tabla.Rows.Cast<DataGridViewRow>().Single(f => (int)((DataRowView)f.DataBoundItem!)["Id"] == idRol).Selected = true;
            Comprobar(boton.Enabled && Campo<int?>(pantalla, "idSeleccionado") == idRol, "La opción también permite editar un rol activo");
            using (var dialogo = new AsignarPermisosRol("Rol por módulos", guardados, [consultaProductos, gestionProductos, consultaVentas, exportar]))
            {
                Modulo(dialogo, "VENTAS"); Marcar(dialogo, "CONSULTAR", false);
                typeof(UsuariosRolesPermisos).GetMethod("AplicarAsignaciones", instancia)!.Invoke(pantalla, [guardados, dialogo.PermisosSeleccionados]);
            }
            Campo<Button>(pantalla, "actualizar").PerformClick();
            Comprobar((int)Tabla($"SELECT COUNT(*) FROM dbo.RolPermiso WHERE id_rol={idRol} AND id_permiso={consultaVentas} AND activo=0").Rows[0][0] == 1,
                "Actualizar retira el permiso y conserva la relación histórica");

            int inactivo = (int)Tabla("SELECT id_permiso FROM dbo.Permiso WHERE modulo='MODULO_PRUEBA' AND accion='ARCHIVAR'").Rows[0][0];
            Ejecutar($"INSERT dbo.RolPermiso(id_rol,id_permiso) VALUES({idRol},{inactivo})");
            using var conInactivos = SeguridadDatos.Asignaciones(TipoSeguridad.Roles, idRol);
            using (var dialogo = new AsignarPermisosRol("Rol por módulos", conInactivos, [inactivo, exportar]))
            {
                Modulo(dialogo, "MODULO_PRUEBA");
                Comprobar(dialogo.PermisosSeleccionados.Contains(inactivo), "Se conservan los permisos inactivos ya asignados");
                Marcar(dialogo, "ARCHIVAR", false);
                Comprobar(!dialogo.PermisosSeleccionados.Contains(inactivo) && dialogo.PermisosSeleccionados.Contains(exportar),
                    "Se puede retirar un permiso inactivo sin perder otras acciones");
            }
            Ejecutar($"UPDATE dbo.Rol SET desactivado_en=SYSUTCDATETIME() WHERE id_rol={idRol}");
            Campo<CheckBox>(pantalla, "inactivos").Checked = true;
            tabla.ClearSelection();
            tabla.Rows.Cast<DataGridViewRow>().Single(f => (int)((DataRowView)f.DataBoundItem!)["Id"] == idRol).Selected = true;
            Comprobar(!boton.Enabled, "Un rol inactivo no permite modificar permisos por módulo");
        }
        using (var dialogo = new AsignarPermisosRol())
        {
            Mostrar(dialogo);
            Comprobar(!Control<Button>(dialogo, "seleccionarModulo").Enabled && Control<Label>(dialogo, "mensaje").Text.Contains("No hay permisos disponibles"),
                "Un catálogo vacío se informa sin habilitar asignaciones");
        }
        Console.WriteLine($"OK: {comprobaciones} comprobaciones de permisos por módulo, dependencias, creación, edición e inactivos.");
    }
}
