using System.Data;
using System.Reflection;
using Sistema_ERP;

internal static partial class Program
{
    private static void ProbarGenerarPermisosUsuario()
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
        void Marcar(CheckedListBox lista, string nombre, bool marcar)
        {
            int indice = Enumerable.Range(0, lista.Items.Count).Single(i => lista.Items[i].ToString() == nombre);
            lista.SetItemChecked(indice, marcar);
        }
        var inicial = Tabla($"""
            SET NOCOUNT ON;
            INSERT dbo.Rol(codigo,nombre) VALUES('VENTANA_LECTURA',N'Ventana lectura'); DECLARE @lectura int=SCOPE_IDENTITY();
            INSERT dbo.Rol(codigo,nombre) VALUES('VENTANA_GESTION',N'Ventana gestión'); DECLARE @gestion int=SCOPE_IDENTITY();
            INSERT dbo.Rol(codigo,nombre) VALUES('VENTANA_VACIO',N'Ventana sin permisos'); DECLARE @vacio int=SCOPE_IDENTITY();
            INSERT dbo.Rol(codigo,nombre,desactivado_en) VALUES('VENTANA_INACTIVO',N'Ventana inactivo',SYSUTCDATETIME()); DECLARE @inactivo int=SCOPE_IDENTITY();
            INSERT dbo.RolPermiso(id_rol,id_permiso)
                SELECT @lectura,id_permiso FROM dbo.Permiso WHERE modulo='PRODUCTOS' AND accion='CONSULTAR';
            INSERT dbo.RolPermiso(id_rol,id_permiso)
                SELECT @gestion,id_permiso FROM dbo.Permiso WHERE modulo='PRODUCTOS' AND accion IN ('CONSULTAR','GESTIONAR');
            INSERT dbo.RolPermiso(id_rol,id_permiso)
                SELECT @inactivo,id_permiso FROM dbo.Permiso WHERE modulo='PRODUCTOS' AND accion='GESTIONAR';
            INSERT dbo.Permiso(modulo,accion,descripcion,desactivado_en)
                VALUES('VENTANA_PRUEBA','CONSULTAR',N'Permiso inactivo',SYSUTCDATETIME());
            INSERT dbo.RolPermiso(id_rol,id_permiso) VALUES(@lectura,CONVERT(int,SCOPE_IDENTITY()));
            INSERT dbo.RolPermiso(id_rol,id_permiso,activo)
                SELECT @lectura,id_permiso,0 FROM dbo.Permiso WHERE modulo='VENTAS' AND accion='CONSULTAR';
            INSERT dbo.Empleado(codigo,nombre,id_tienda) VALUES('VENTANA',N'Empleado ventana',{tienda});
            SELECT CONVERT(int,SCOPE_IDENTITY()) AS Empleado,@lectura AS Lectura,@gestion AS Gestion,@vacio AS Vacio,@inactivo AS Inactivo,
                (SELECT id_rol FROM dbo.Rol WHERE codigo='ADMINISTRADOR') AS Administrador;
            """).Rows[0];
        int lectura = (int)inicial["Lectura"], gestion = (int)inicial["Gestion"], empleado = (int)inicial["Empleado"],
            administrador = (int)inicial["Administrador"], inactivo = (int)inicial["Inactivo"];
        using var roles = SeguridadDatos.Asignaciones(TipoSeguridad.Usuarios, null);
        using var permisos = SeguridadDatos.PermisosPorRol();
        Comprobar(!permisos.AsEnumerable().Any(f => (int)f["IdRol"] == inactivo), "Un rol inactivo no aporta permisos a la ventana");
        var permisosLectura = permisos.AsEnumerable().Where(f => (int)f["IdRol"] == lectura && !f.IsNull("IdPermiso")).ToArray();
        Comprobar(permisosLectura.Length == 1 && (string)permisosLectura[0]["Modulo"] == "PRODUCTOS",
            "Se excluyen permisos inactivos y relaciones desactivadas");
        using (var dialogo = new GenerarPermisosUsuario("nuevo.ventana", roles, permisos, [lectura]))
        {
            Mostrar(dialogo);
            var lista = dialogo.Controls.Find("listaRoles", true).OfType<CheckedListBox>().Single();
            var tabla = dialogo.Controls.Find("tablaPermisos", true).OfType<DataGridView>().Single();
            var mensaje = dialogo.Controls.Find("mensajePermisos", true).Single();
            Comprobar(dialogo.RolesSeleccionados.SequenceEqual([lectura]) && tabla.Visible && tabla.Rows.Count == 1 && !mensaje.Visible,
                "La ventana precarga los roles y muestra sus permisos al abrirse");
            Marcar(lista, "Ventana gestión", true);
            Comprobar(tabla.Rows.Count == 2, "Los permisos de varios roles se combinan sin duplicados");
            Marcar(lista, "Ventana lectura", false);
            Comprobar(tabla.Rows.Count == 2, "Retirar un rol conserva los permisos aportados por otro");
            Marcar(lista, "Ventana gestión", false);
            Comprobar(tabla.Rows.Count == 0 && mensaje.Visible, "Quitar todos los roles limpia la vista de permisos");
            Marcar(lista, "Ventana sin permisos", true);
            Comprobar(mensaje.Visible && tabla.Rows.Count == 0, "Un rol sin permisos muestra el estado vacío");
            Marcar(lista, "Ventana sin permisos", false);
            Marcar(lista, "Ventana lectura", true);
            // Se usa el código del rol, aunque su nombre visible sea distinto.
            string nombreAdmin = roles.AsEnumerable().Single(f => (int)f["Id"] == administrador).Field<string>("Nombre")!;
            Marcar(lista, nombreAdmin, true);
            Comprobar(!tabla.Visible && mensaje.Text.Contains("Acceso total"), "ADMINISTRADOR muestra acceso total");
            Marcar(lista, nombreAdmin, false);
            Comprobar(tabla.Visible && tabla.Rows.Count == 1 && !mensaje.Visible, "Retirar ADMINISTRADOR recupera la vista de permisos");
            dialogo.Size = dialogo.MinimumSize;
            dialogo.PerformLayout();
            Comprobar(tabla.Height > 100 && dialogo.Controls.Find("aceptar", true).Single().Bottom <= dialogo.ClientSize.Height,
                "La ventana conserva espacio para permisos y acciones en su tamaño mínimo");
            dialogo.Controls.Find("cancelar", true).OfType<Button>().Single().PerformClick();
            Comprobar(dialogo.DialogResult == DialogResult.Cancel && SeguridadDatos.Listar(TipoSeguridad.Usuarios, false).AsEnumerable().All(f => (string)f["Primero"] != "nuevo.ventana"),
                "Cancelar la ventana no crea usuarios ni asignaciones");
        }

        using (var dialogo = new GenerarPermisosUsuario("nuevo.ventana", roles, permisos, [lectura, gestion]))
        {
            Mostrar(dialogo);
            dialogo.Controls.Find("aceptar", true).OfType<Button>().Single().PerformClick();
            Comprobar(dialogo.DialogResult == DialogResult.OK, "Aceptar devuelve la selección al formulario");
            SeguridadDatos.Guardar(TipoSeguridad.Usuarios, null, null, "nuevo.ventana", "", "", empleado, "Temporal-ventana-123!", dialogo.RolesSeleccionados);
        }
        var creado = SeguridadDatos.Listar(TipoSeguridad.Usuarios, false).AsEnumerable().Single(f => (string)f["Primero"] == "nuevo.ventana");
        int idUsuario = (int)creado["Id"];
        using var asignados = SeguridadDatos.Asignaciones(TipoSeguridad.Usuarios, idUsuario);
        Comprobar(asignados.AsEnumerable().Where(f => (bool)f["Asignado"]).Select(f => (int)f["Id"]).ToHashSet().SetEquals([lectura, gestion]),
            "Crear el usuario persiste los roles elegidos en la ventana");
        using (var dialogo = new GenerarPermisosUsuario("nuevo.ventana", asignados, permisos, [lectura, gestion]))
        {
            var lista = dialogo.Controls.Find("listaRoles", true).OfType<CheckedListBox>().Single();
            Marcar(lista, "Ventana gestión", false);
            SeguridadDatos.Guardar(TipoSeguridad.Usuarios, idUsuario, (byte[])creado["version_fila"], "nuevo.ventana", "", "", empleado, "", dialogo.RolesSeleccionados);
        }
        Comprobar((int)Tabla($"SELECT COUNT(*) FROM dbo.UsuarioRol WHERE id_usuario={idUsuario} AND id_rol={gestion} AND activo=0").Rows[0][0] == 1,
            "Editar el usuario retira el rol conservando la relación histórica");
        Ejecutar($"INSERT dbo.UsuarioRol(id_usuario,id_rol) VALUES({idUsuario},{inactivo})");
        using var conInactivos = SeguridadDatos.Asignaciones(TipoSeguridad.Usuarios, idUsuario);
        using (var dialogo = new GenerarPermisosUsuario("nuevo.ventana", conInactivos, permisos, [lectura, inactivo]))
        {
            Mostrar(dialogo);
            var lista = dialogo.Controls.Find("listaRoles", true).OfType<CheckedListBox>().Single();
            Comprobar(dialogo.RolesSeleccionados.Contains(inactivo) && dialogo.Controls.Find("tablaPermisos", true).OfType<DataGridView>().Single().Rows.Count == 1,
                "Editar conserva un rol inactivo asignado sin mostrar sus permisos");
            Marcar(lista, "Ventana inactivo (inactivo)", false);
            Comprobar(!dialogo.RolesSeleccionados.Contains(inactivo), "Puede retirarse un rol inactivo");
        }
        using (var dialogo = new GenerarPermisosUsuario("", new DataTable(), new DataTable(), []))
        {
            Mostrar(dialogo);
            Comprobar(dialogo.Controls.Find("mensajePermisos", true).Single().Text.Contains("No hay roles disponibles"), "Sin roles disponibles se explica cómo crearlos");
        }

        using (var pantalla = new UsuariosRolesPermisos())
        {
            Mostrar(pantalla);
            var boton = pantalla.Controls.Find("generarPermisos", true).Single();
            Comprobar(boton.Visible && boton.Enabled, "La opción está disponible para un usuario nuevo");
            var tabla = Campo<DataGridView>(pantalla, "tabla");
            tabla.ClearSelection();
            tabla.Rows.Cast<DataGridViewRow>().Single(f => (int)((DataRowView)f.DataBoundItem!)["Id"] == idUsuario).Selected = true;
            Comprobar(boton.Visible && boton.Enabled && Campo<int?>(pantalla, "idSeleccionado") == idUsuario,
                $"La opción está disponible al editar un usuario activo (visible={boton.Visible}, habilitado={boton.Enabled}, id={Campo<int?>(pantalla, "idSeleccionado")}, esperado={idUsuario}, filas={tabla.SelectedRows.Count}, estado={Campo<Label>(pantalla, "estado").Text})");
            typeof(UsuariosRolesPermisos).GetMethod("CambiarPestana", instancia)!.Invoke(pantalla, [TipoSeguridad.Roles]);
            Comprobar(!boton.Visible, "La opción se muestra solo en la pestaña Usuarios");
        }

        var sesion = app.GetType("Sistema_ERP.SesionActual")!.GetProperty("Usuario", estaticos)!;
        object? original = sesion.GetValue(null);
        try
        {
            Ejecutar($"""
                UPDATE dbo.Usuario SET debe_cambiar_contrasena=0 WHERE id_usuario={idUsuario};
                UPDATE dbo.UsuarioRol SET activo=0 WHERE id_usuario={idUsuario};
                INSERT dbo.RolPermiso(id_rol,id_permiso)
                    SELECT {(int)inicial["Vacio"]},id_permiso FROM dbo.Permiso WHERE modulo='SEGURIDAD' AND accion='CONSULTAR';
                INSERT dbo.UsuarioRol(id_usuario,id_rol) VALUES({idUsuario},{(int)inicial["Vacio"]});
                """);
            long idSesion = (long)Tabla($"""
                SET NOCOUNT ON;
                INSERT dbo.SesionUsuario(id_usuario,token_hash,version_seguridad,expira_en)
                    SELECT id_usuario,CRYPT_GEN_RANDOM(32),version_seguridad,DATEADD(hour,1,SYSUTCDATETIME()) FROM dbo.Usuario WHERE id_usuario={idUsuario};
                SELECT CONVERT(bigint,SCOPE_IDENTITY());
                """).Rows[0][0];
            sesion.SetValue(null, Activator.CreateInstance(app.GetType("Sistema_ERP.AccesoUsuario")!,
                [idUsuario, "Ventana", "Consulta", false, Array.Empty<byte>(), idSesion]));
            using var consultaAutorizada = SeguridadDatos.Listar(TipoSeguridad.Usuarios, false);
            using var pantalla = new UsuariosRolesPermisos();
            Mostrar(pantalla);
            var boton = pantalla.Controls.Find("generarPermisos", true).Single();
            Comprobar(!boton.Visible, "Una cuenta de consulta no ve Generar permisos");
            typeof(UsuariosRolesPermisos).GetMethod("CambiarPestana", instancia)!.Invoke(pantalla, [TipoSeguridad.Roles]);
            Comprobar(!pantalla.Controls.Find("asignarPermisosModulo", true).Single().Visible, "Una cuenta de consulta no ve Permisos por módulo");
            Comprobar(!pantalla.Controls.Find("editarPermisosRol", true).Single().Visible, "Una cuenta de consulta no puede crear ni editar permisos desde Roles");
            typeof(UsuariosRolesPermisos).GetMethod("CambiarPestana", instancia)!.Invoke(pantalla, [TipoSeguridad.Usuarios]);
            Comprobar(!boton.Visible, "Cambiar de pestaña no revela la opción a una cuenta de consulta");
        }
        finally { sesion.SetValue(null, original); }
        Console.WriteLine($"OK: {comprobaciones} comprobaciones de la ventana de permisos, roles, creación, edición y consulta.");
    }
}
