using System.Data;
using System.Reflection;
using Sistema_ERP;

internal static partial class Program
{
    private static void ProbarPresetsPermisos()
    {
        const BindingFlags instancia = BindingFlags.Instance | BindingFlags.NonPublic;
        int comprobaciones = 0;
        void Comprobar(bool condicion, string contexto) { Igual(true, condicion, contexto); comprobaciones++; }
        T Campo<T>(object formulario, string nombre) => (T)formulario.GetType().GetField(nombre, instancia)!.GetValue(formulario)!;
        T Control<T>(Form formulario, string nombre) where T : Control => formulario.Controls.Find(nombre, true).OfType<T>().Single();
        void Mostrar(Form formulario)
        {
            formulario.ShowInTaskbar = false; formulario.Opacity = 0;
            formulario.StartPosition = FormStartPosition.Manual; formulario.Location = new Point(-5000, -5000);
            formulario.Show();
        }
        void Aplicar(AsignarPermisosRol dialogo, string nombre)
        {
            var presets = Control<ComboBox>(dialogo, "listaPresets");
            presets.SelectedItem = presets.Items.Cast<object>().Single(p => p.ToString() == nombre);
            Control<Button>(dialogo, "aplicarPreset").PerformClick();
        }
        object? Plantilla(AsignarPermisosRol dialogo) => typeof(AsignarPermisosRol).GetProperty("PlantillaAplicada", instancia)!.GetValue(dialogo);
        using var catalogo = SeguridadDatos.Asignaciones(TipoSeguridad.Roles, null);
        int cantidadRolesInicial = SeguridadDatos.Listar(TipoSeguridad.Roles, true).Rows.Count;
        DataRow[] Elegidos(AsignarPermisosRol dialogo) => catalogo.AsEnumerable().Where(f => dialogo.PermisosSeleccionados.Contains(f.Field<int>("Id"))).ToArray();
        int Id(string modulo, string accion) => catalogo.AsEnumerable().Single(f => (string)f["Modulo"] == modulo && (string)f["Accion"] == accion).Field<int>("Id");
        using (var dialogo = new AsignarPermisosRol("Prueba presets", catalogo, [Id("VENTAS", "CONSULTAR")]))
        {
            Mostrar(dialogo);
            var presets = Control<ComboBox>(dialogo, "listaPresets");
            Comprobar(presets.Items.Cast<object>().Select(p => p.ToString()).SequenceEqual(["Administrador", "Cajero", "Gerente", "Analista de datos"]),
                "Se ofrecen los cuatro presets solicitados");
            presets.SelectedIndex = 3;
            Comprobar(dialogo.PermisosSeleccionados.SequenceEqual([Id("VENTAS", "CONSULTAR")]), "Elegir un preset no cambia permisos hasta pulsar Aplicar");
            var modulos = Control<ListBox>(dialogo, "listaModulos");
            Comprobar(modulos.GetItemText("PRODUCTOS") == "Catálogo de productos y categorías", "Los módulos muestran los nombres de sus pantallas");
            modulos.SelectedItem = "DIRECTORIO";
            Comprobar(Control<Label>(dialogo, "alcanceModulo").Text.Contains("empleados"), "El alcance del directorio indica que incluye empleados");
            Aplicar(dialogo, "Cajero");
            Comprobar(Elegidos(dialogo).Length == 6 && Elegidos(dialogo).Where(f => (string)f["Accion"] == "GESTIONAR")
                .Select(f => (string)f["Modulo"]).ToHashSet().SetEquals(["VENTAS", "APARTADOS_DEVOLUCIONES"]), "Cajero gestiona ventas y apartados con consulta de productos e inventario");
            modulos.SelectedItem = "INVENTARIO";
            var lista = Control<CheckedListBox>(dialogo, "listaPermisos");
            int gestion = Enumerable.Range(0, lista.Items.Count).Single(i => lista.Items[i].ToString()!.StartsWith("GESTIONAR "));
            lista.SetItemChecked(gestion, true);
            Comprobar(dialogo.PermisosSeleccionados.Contains(Id("INVENTARIO", "GESTIONAR")), "Se pueden personalizar las pantallas después de aplicar un preset");
            Aplicar(dialogo, "Analista de datos");
            Comprobar(Elegidos(dialogo).Length == 8 && Elegidos(dialogo).All(f => (string)f["Accion"] == "CONSULTAR"), "Analista reemplaza la selección anterior y no concede escrituras");
            Comprobar(!Elegidos(dialogo).Any(f => new[] { "SEGURIDAD", "CONFIGURACION", "DIRECTORIO", "CARGA_MASIVA" }.Contains((string)f["Modulo"])),
                "Analista no incluye administración, empleados ni importaciones");
            Aplicar(dialogo, "Gerente");
            Comprobar(Elegidos(dialogo).Length == 20 && !Elegidos(dialogo).Any(f => new[] { "SEGURIDAD", "CONFIGURACION" }.Contains((string)f["Modulo"])),
                "Gerente gestiona operaciones sin administrar la seguridad ni la configuración");
            Aplicar(dialogo, "Administrador");
            Comprobar(Elegidos(dialogo).Length == 24 && !Elegidos(dialogo).Any(f => ((string)f["Modulo"]).StartsWith("MODULO_")),
                "Administrador concede las pantallas registradas sin incorporar permisos personalizados desconocidos");
            dialogo.Size = dialogo.MinimumSize; dialogo.PerformLayout();
            Comprobar(lista.Height > 80 && Control<Button>(dialogo, "aceptar").PointToScreen(Point.Empty).Y + Control<Button>(dialogo, "aceptar").Height
                <= dialogo.PointToScreen(new Point(0, dialogo.ClientSize.Height)).Y, "El tamaño mínimo conserva permisos y botones accesibles");
            Control<Button>(dialogo, "quitarTodos").PerformClick();
            Comprobar(dialogo.PermisosSeleccionados.Length == 0 && !Control<Button>(dialogo, "quitarTodos").Enabled, "Quitar todos elimina la selección de todas las pantallas");
            Control<Button>(dialogo, "cancelar").PerformClick();
            Comprobar(dialogo.DialogResult == DialogResult.Cancel && SeguridadDatos.Listar(TipoSeguridad.Roles, true).Rows.Count == cantidadRolesInicial,
                "Aplicar y cancelar presets no guarda roles ni relaciones");
        }
        using (var incompleto = catalogo.Copy())
        {
            incompleto.AsEnumerable().Single(f => (string)f["Modulo"] == "VENTAS" && (string)f["Accion"] == "CONSULTAR")["activo"] = false;
            using var dialogo = new AsignarPermisosRol("Catálogo incompleto", incompleto, []);
            Mostrar(dialogo); Aplicar(dialogo, "Cajero");
            Comprobar(!dialogo.PermisosSeleccionados.Contains(Id("VENTAS", "CONSULTAR")) && !dialogo.PermisosSeleccionados.Contains(Id("VENTAS", "GESTIONAR"))
                && Control<Label>(dialogo, "mensaje").Text.Contains("Faltan 2 permisos"), "Un permiso de consulta inactivo impide conceder también su gestión e informa el preset incompleto");
        }
        using (var dialogo = new AsignarPermisosRol())
        {
            Mostrar(dialogo);
            Comprobar(!Control<Button>(dialogo, "aplicarPreset").Enabled, "Sin catálogo no se pueden aplicar presets");
        }
        using (var dialogo = new AsignarPermisosRol("Administrador", catalogo, [], codigoRol: "administrador"))
        {
            Mostrar(dialogo);
            Comprobar(Control<Label>(dialogo, "avisoAdministrador").Visible && Control<Label>(dialogo, "avisoAdministrador").Text.Contains("siempre tiene acceso total"),
                "El rol reservado ADMINISTRADOR informa que sus permisos no pueden limitarse con casillas");
            dialogo.Size = dialogo.MinimumSize;
            Control<ListBox>(dialogo, "listaModulos").SelectedItem = "TRASPASOS";
            dialogo.PerformLayout();
            Comprobar(Control<CheckedListBox>(dialogo, "listaPermisos").Height > 80, "La advertencia administrativa y las descripciones extensas dejan espacio para seleccionar permisos");
        }

        var rolesGuardados = new Dictionary<string, int>();
        foreach (var (nombre, codigo, cantidad) in new[] { ("Administrador", "ADMIN_PERSONALIZADO", 24), ("Cajero", "CAJERO", 6), ("Gerente", "GERENTE", 20), ("Analista de datos", "ANALISTA_DATOS", 8) })
        {
            using var pantalla = new UsuariosRolesPermisos(); Mostrar(pantalla);
            typeof(UsuariosRolesPermisos).GetMethod("CambiarPestana", instancia)!.Invoke(pantalla, [TipoSeguridad.Roles]);
            using var dialogo = new AsignarPermisosRol("Nuevo rol", catalogo, []); Mostrar(dialogo); Aplicar(dialogo, nombre);
            Control<Button>(dialogo, "aceptar").PerformClick();
            typeof(UsuariosRolesPermisos).GetMethod("AplicarAsignaciones", instancia)!.Invoke(pantalla, [catalogo, dialogo.PermisosSeleccionados]);
            typeof(UsuariosRolesPermisos).GetMethod("CompletarRolDesdePlantilla", instancia)!.Invoke(pantalla, [Plantilla(dialogo)]);
            string codigoGuardado = Campo<TextBox>(pantalla, "primero").Text;
            Comprobar(codigoGuardado.StartsWith(codigo) && !string.IsNullOrWhiteSpace(Campo<TextBox>(pantalla, "segundo").Text), $"{nombre} prepara un código disponible y el nombre del nuevo rol");
            Campo<Button>(pantalla, "guardar").PerformClick();
            var rol = SeguridadDatos.Listar(TipoSeguridad.Roles, false).AsEnumerable().Single(f => (string)f["Primero"] == codigoGuardado);
            int id = rol.Field<int>("Id"); rolesGuardados.Add(codigo, id);
            using var asignaciones = SeguridadDatos.Asignaciones(TipoSeguridad.Roles, id);
            Comprobar(asignaciones.AsEnumerable().Count(f => f.Field<bool>("Asignado")) == cantidad, $"{nombre} persiste los permisos al guardar");
            Campo<TextBox>(pantalla, "primero").Text = "CODIGO_PROPIO";
            Campo<TextBox>(pantalla, "segundo").Text = "Nombre propio";
            typeof(UsuariosRolesPermisos).GetMethod("CompletarRolDesdePlantilla", instancia)!.Invoke(pantalla, [Plantilla(dialogo)]);
            Comprobar(Campo<TextBox>(pantalla, "primero").Text == "CODIGO_PROPIO" && Campo<TextBox>(pantalla, "segundo").Text == "Nombre propio", "Los presets respetan los datos escritos por el usuario");
        }

        var sesion = app.GetType("Sistema_ERP.SesionActual")!.GetProperty("Usuario", estaticos)!;
        object? original = sesion.GetValue(null);
        try
        {
            foreach (var (codigo, idRol) in rolesGuardados)
            {
                long idSesion = (long)Tabla($"""
                    SET NOCOUNT ON;
                    INSERT dbo.Empleado(codigo,nombre,id_tienda) VALUES('P_{codigo}',N'Prueba preset',{tienda});
                    DECLARE @e int=SCOPE_IDENTITY();
                    INSERT dbo.Usuario(id_empleado,nombre_usuario,contrasena_hash,debe_cambiar_contrasena) VALUES(@e,N'P_{codigo}',N'NO-AUTENTICABLE',0);
                    DECLARE @u int=SCOPE_IDENTITY();
                    INSERT dbo.UsuarioRol(id_usuario,id_rol) VALUES(@u,{idRol});
                    INSERT dbo.SesionUsuario(id_usuario,token_hash,version_seguridad,expira_en)
                        SELECT @u,CRYPT_GEN_RANDOM(32),version_seguridad,DATEADD(hour,1,SYSUTCDATETIME()) FROM dbo.Usuario WHERE id_usuario=@u;
                    SELECT CONVERT(bigint,SCOPE_IDENTITY());
                    """).Rows[0][0];
                int idUsuario = Convert.ToInt32(Tabla($"SELECT id_usuario FROM dbo.SesionUsuario WHERE id_sesion={idSesion}").Rows[0][0]);
                sesion.SetValue(null, Activator.CreateInstance(app.GetType("Sistema_ERP.AccesoUsuario")!, [idUsuario, codigo, codigo, false, Array.Empty<byte>(), idSesion]));
                object permisos = app.GetType("Sistema_ERP.AutorizacionDatos")!.GetMethods(estaticos)
                    .Single(m => m.Name == "Obtener" && m.GetParameters().Length == 0).Invoke(null, null)!;
                bool Puede(string modulo, bool gestionar = false) => (bool)permisos.GetType().GetMethod("Puede", instancia)!.Invoke(permisos, [modulo, gestionar])!;
                Comprobar(Puede("VENTAS") && Puede("APARTADOS_DEVOLUCIONES") && Puede("PRODUCTOS"), $"{codigo} puede consultar las pantallas de su perfil");
                Comprobar(Puede("VENTAS", true) == (codigo != "ANALISTA_DATOS"), $"{codigo} aplica la restricción de escritura de ventas");
                Comprobar(Puede("SEGURIDAD", true) == (codigo == "ADMIN_PERSONALIZADO") && Puede("CONFIGURACION") == (codigo == "ADMIN_PERSONALIZADO"),
                    $"{codigo} respeta el acceso administrativo en SQL");
                if (codigo == "ANALISTA_DATOS")
                {
                    using var productos = ProductoDatos.Listar();
                    Comprobar(productos.Rows.Count > 0, "Analista consulta productos desde la capa de datos");
                    bool denegado = false;
                    try { ProductoDatos.Agregar(new Producto { CodigoBarras = "SIN-AUTORIZACION", Nombre = "Producto sin autorización", PrecioUnitario = 10, IdCategoria = categoria, IdProveedor = proveedor }); } catch (UnauthorizedAccessException) { denegado = true; }
                    Comprobar(denegado, "Analista no puede guardar productos mediante una llamada directa");
                }
                if (codigo == "CAJERO")
                {
                    bool denegado = false;
                    try { CompraDatos.Listar(); } catch (UnauthorizedAccessException) { denegado = true; }
                    Comprobar(denegado && !Puede("INVENTARIO", true), "Cajero no consulta compras ni modifica inventario");
                }
            }
        }
        finally { sesion.SetValue(null, original); }
        Console.WriteLine($"OK: {comprobaciones} comprobaciones de presets, pantallas, personalización, persistencia y autorización real.");
    }
}
