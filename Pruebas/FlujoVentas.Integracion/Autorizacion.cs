using System.Data;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Sistema_ERP;

internal static partial class Program
{
    private static void ProbarAutorizacion()
    {
        var sesion = app.GetType("Sistema_ERP.SesionActual")!.GetProperty("Usuario", estaticos)!;
        object? administradorOriginal = sesion.GetValue(null);
        int comprobaciones = 0;
        var datos = Tabla($"""
            SET NOCOUNT ON;
            INSERT dbo.Empleado(codigo,nombre,id_tienda) VALUES('LIMITADO',N'Usuario limitado',{tienda});
            DECLARE @empleado int=SCOPE_IDENTITY();
            INSERT dbo.Usuario(id_empleado,nombre_usuario,contrasena_hash,debe_cambiar_contrasena)
                VALUES(@empleado,N'limitado',N'NO-AUTENTICABLE',0);
            DECLARE @usuario int=SCOPE_IDENTITY();
            INSERT dbo.Rol(codigo,nombre) VALUES('LECTURA',N'Lectura'); DECLARE @lectura int=SCOPE_IDENTITY();
            INSERT dbo.Rol(codigo,nombre) VALUES('ESCRITURA',N'Escritura'); DECLARE @escritura int=SCOPE_IDENTITY();
            SELECT @usuario AS Usuario,@lectura AS Lectura,@escritura AS Escritura;
            """).Rows[0];
        int limitado = (int)datos["Usuario"], lectura = (int)datos["Lectura"], escritura = (int)datos["Escritura"];
        long idSesion = 0;
        void Entrar()
        {
            idSesion = (long)Tabla($"""
                SET NOCOUNT ON;
                INSERT dbo.SesionUsuario(id_usuario,token_hash,version_seguridad,expira_en)
                    SELECT id_usuario,CRYPT_GEN_RANDOM(32),version_seguridad,DATEADD(hour,1,SYSUTCDATETIME())
                    FROM dbo.Usuario WHERE id_usuario={limitado};
                SELECT CONVERT(bigint,SCOPE_IDENTITY());
                """).Rows[0][0];
            object acceso = Activator.CreateInstance(app.GetType("Sistema_ERP.AccesoUsuario")!,
                [limitado, "Limitado", "Administrador", false, Array.Empty<byte>(), idSesion])!;
            sesion.SetValue(null, acceso);
        }
        void Denegado(Action accion)
        {
            try { accion(); }
            catch (UnauthorizedAccessException) { comprobaciones++; return; }
            throw new Exception("Se permitió acceder a contenido restringido.");
        }
        void Comprobar(bool condicion, string contexto)
        { Igual(true, condicion, contexto); comprobaciones++; }
        void Permiso(int rol, string accion)
        {
            Ejecutar($"INSERT dbo.RolPermiso(id_rol,id_permiso) SELECT {rol},id_permiso FROM dbo.Permiso WHERE modulo='PRODUCTOS' AND accion='{accion}'");
        }
        void ResumenSinContenido()
        {
            object resumen = Invocar("InicioDatos", "Obtener", null, DateTime.UtcNow.AddHours(-6).Date)!;
            var dias = ((Array)Prop(resumen, "Dias")).Cast<object>();
            Comprobar(dias.All(d => (decimal)Prop(d, "Ventas") == 0 && (decimal)Prop(d, "Compras") == 0), "Inicio no filtra montos de módulos restringidos");
            Comprobar((long)Prop(resumen, "TraspasosPendientes") == 0 && (long)Prop(resumen, "AlertasStock") == 0, "Inicio no filtra conteos restringidos");
        }
        try
        {
            Entrar();
            Denegado(() => ProductoDatos.Listar());
            Denegado(() => ProveedorDatos.Listar());
            Denegado(() => SeguridadDatos.Listar(TipoSeguridad.Usuarios, false));
            Denegado(() => SeguridadDatos.PermisosPorRol());
            Denegado(() => Invocar("SeguridadDatos", "CatalogoPermisosParaRol", (object?)null));
            Denegado(() => SeguridadDatos.Guardar(TipoSeguridad.Permisos, null, null, "NO_AUTORIZADO", "CONSULTAR", "Prueba de acceso", null, "", []));
            Denegado(() => TiendaDatos.Listar());
            Denegado(() => TiendaDatos.ListarParaSeleccion());
            Denegado(() => InventarioDatos.AlertasStock());
            Denegado(() => CompraDatos.Listar());
            Denegado(() => TraspasoDatos.Listar());
            Denegado(() => Invocar("ApartadoDatos", "Listar"));
            Denegado(() => Invocar("DevolucionDatos", "Listar"));
            Denegado(() => Invocar("VentaDatos", "Consultar", "SELECT id_venta FROM dbo.Venta"));
            Denegado(() => Invocar("AuditoriaDatos", "Listar", ""));
            Denegado(() => Invocar("ConfiguracionFacturacionDatos", "Obtener", tienda));
            Denegado(() => Invocar("AutorizacionDatos", "ExigirPantalla", "Usuarios, roles y permisos"));
            Denegado(() => Invocar("AutorizacionDatos", "ExigirPantalla", "Pantalla sin registrar"));
            ResumenSinContenido();

            using (var principal = (Pagina_Central)Activator.CreateInstance(typeof(Pagina_Central),
                BindingFlags.Instance | BindingFlags.NonPublic, null, [sesion.GetValue(null)], null)!)
            {
                principal.ShowInTaskbar = false; principal.Opacity = 0;
                principal.StartPosition = FormStartPosition.Manual; principal.Location = new Point(-5000, -5000);
                principal.Show();
                Comprobar(!principal.Controls.Find("salesButton", true).Single().Visible, "Menú oculta ventas sin permiso");
                Comprobar(!principal.Controls.Find("seguridadButton", true).Single().Visible, "Menú oculta administración sin permiso");
                Comprobar(!principal.Controls.Find("notificationButton", true).Single().Visible, "No hay indicador de inventario sin permiso");
                principal.Close();
            }

            Ejecutar($"INSERT dbo.UsuarioRol(id_usuario,id_rol) VALUES({limitado},{lectura}),({limitado},{escritura})");
            Permiso(escritura, "GESTIONAR"); Entrar();
            Denegado(() => ProductoDatos.Listar());
            Denegado(() => ProductoDatos.Agregar(new Producto { CodigoBarras = "SIN-AUTORIZACION", Nombre = "Producto sin autorización", PrecioUnitario = 10, IdCategoria = categoria, IdProveedor = proveedor }));
            Permiso(lectura, "CONSULTAR"); Entrar();
            Comprobar(ProductoDatos.Listar().Rows.Count > 0, "Consulta concedida por un rol activo");
            int creado = ProductoDatos.Agregar(new Producto { CodigoBarras = "AUTORIZADO", Nombre = "Autorizado", PrecioUnitario = 10, IdCategoria = categoria, IdProveedor = proveedor });
            Comprobar(creado > 0, "Permisos de varios roles permiten guardar");
            Denegado(() => SeguridadDatos.Listar(TipoSeguridad.Usuarios, false));
            Denegado(() => CompraDatos.Guardar(null, null, proveedor, tienda, "NO-AUTORIZADA", DateTime.Today, "", [new LineaCompra(producto, 1, 10)]));

            Ejecutar($"UPDATE dbo.UsuarioRol SET activo=0 WHERE id_usuario={limitado} AND id_rol={escritura}"); Entrar();
            Comprobar(ProductoDatos.Listar().Rows.Count > 0, "El rol de consulta conserva lectura");
            Denegado(() => ProductoDatos.Agregar(new Producto { CodigoBarras = "SIN-AUTORIZACION", Nombre = "Producto sin autorización", PrecioUnitario = 10, IdCategoria = categoria, IdProveedor = proveedor }));
            Denegado(() => ProductoDatos.Desactivar(creado, "No permitido"));
            Denegado(() => CargaMasivaDatos.ImportarProductos([new("SIN-PERMISO", "Rechazado", 10, 0, categoria, proveedor)], tienda));
            using (var catalogo = new Catalogo_Productos())
            using (var ventana = new Form { ShowInTaskbar = false, Opacity = 0 })
            {
                ventana.Controls.Add(catalogo); ventana.Show();
                Comprobar(!catalogo.Controls.Find("btnNuevoProducto", true).Single().Visible, "Consulta oculta Nuevo producto");
                Comprobar(!catalogo.Controls.Find("btnEditar", true).Single().Visible, "Consulta oculta Editar producto");
                Comprobar(catalogo.Controls.Find("btnReiniciar", true).Single().Visible, "Consulta conserva el botón Reiniciar");
                ventana.Close();
            }
            Ejecutar($"UPDATE dbo.RolPermiso SET activo=0 WHERE id_rol={lectura}"); Entrar();
            Denegado(() => ProductoDatos.Listar());
            Ejecutar($"UPDATE dbo.RolPermiso SET activo=1 WHERE id_rol={lectura}; UPDATE dbo.Rol SET desactivado_en=SYSUTCDATETIME() WHERE id_rol={lectura}"); Entrar();
            Denegado(() => ProductoDatos.Listar());
            Ejecutar($"UPDATE dbo.Rol SET desactivado_en=NULL WHERE id_rol={lectura}; UPDATE dbo.Permiso SET desactivado_en=SYSUTCDATETIME() WHERE modulo='PRODUCTOS' AND accion='CONSULTAR'"); Entrar();
            Denegado(() => ProductoDatos.Listar());
            Ejecutar("UPDATE dbo.Permiso SET desactivado_en=NULL WHERE modulo='PRODUCTOS' AND accion='CONSULTAR'"); Entrar();
            Comprobar(ProductoDatos.Listar().Rows.Count > 0, "Permiso activo permite lectura nuevamente");
            Ejecutar($"UPDATE dbo.SesionUsuario SET revocada_en=SYSUTCDATETIME() WHERE id_sesion={idSesion}");
            bool revocada = false;
            try { ProductoDatos.Listar(); } catch (InvalidOperationException) { revocada = true; }
            Comprobar(revocada, "Una sesión revocada no usa permisos previos");

            Ejecutar($"INSERT dbo.UsuarioRol(id_usuario,id_rol) SELECT {limitado},id_rol FROM dbo.Rol WHERE codigo='ADMINISTRADOR'"); Entrar();
            Comprobar(SeguridadDatos.Listar(TipoSeguridad.Usuarios, false).Rows.Count > 0, "El rol administrativo real conserva acceso");
            Ejecutar($"UPDATE dbo.UsuarioRol SET activo=0 WHERE id_usuario={limitado};"); Entrar();
            Denegado(() => SeguridadDatos.Listar(TipoSeguridad.Usuarios, false));
            sesion.SetValue(null, null);
            bool sinSesion = false;
            try { ProductoDatos.Listar(); } catch (InvalidOperationException) { sinSesion = true; }
            Comprobar(sinSesion, "No se consulta sin iniciar sesión");
            Console.WriteLine($"OK: {comprobaciones} comprobaciones de autorización, operaciones, menú, consulta y revocación.");
        }
        finally { sesion.SetValue(null, administradorOriginal); }
    }
}
