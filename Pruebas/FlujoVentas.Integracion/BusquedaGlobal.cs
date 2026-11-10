using System.Collections;
using System.Data;
using System.Reflection;
using Sistema_ERP;

internal static partial class Program
{
    private static void ProbarBusquedaGlobal()
    {
        var sesion = app.GetType("Sistema_ERP.SesionActual")!.GetProperty("Usuario", estaticos)!;
        object? original = sesion.GetValue(null);
        int comprobaciones = 0;
        const BindingFlags instancia = BindingFlags.Instance | BindingFlags.NonPublic;
        void Comprobar(bool condicion, string contexto) { Igual(true, condicion, contexto); comprobaciones++; }
        object[] Buscar(string texto, int? sucursal = null) => ((IEnumerable)Invocar("BusquedaGlobalDatos", "Buscar", texto, sucursal, CancellationToken.None)!).Cast<object>().ToArray();
        string Tipo(object resultado) => (string)Prop(resultado, "Tipo");
        int Id(object resultado) => (int)Prop(resultado, "Id");
        void Denegado(Action accion)
        {
            try { accion(); } catch (UnauthorizedAccessException) { comprobaciones++; return; }
            throw new Exception("La búsqueda mostró un detalle restringido.");
        }
        var inicial = Tabla($"""
            SET NOCOUNT ON;
            INSERT dbo.Categoria(nombre) VALUES(N'Global categoría'); DECLARE @categoria int=SCOPE_IDENTITY();
            INSERT dbo.Proveedor(nombre_empresa,rtn) VALUES(N'Global proveedor','08018888888888'); DECLARE @proveedor int=SCOPE_IDENTITY();
            INSERT dbo.Cliente(nombre,documento_fiscal) VALUES(N'Global cliente','08017777777777'); DECLARE @cliente int=SCOPE_IDENTITY();
            INSERT dbo.Tienda(codigo,nombre) VALUES('GLOBAL',N'Global otra tienda'); DECLARE @tienda int=SCOPE_IDENTITY();
            INSERT dbo.Empleado(codigo,nombre,id_tienda) VALUES('GLOBAL',N'Global empleado',{tienda}); DECLARE @empleado int=SCOPE_IDENTITY();
            INSERT dbo.Usuario(id_empleado,nombre_usuario,contrasena_hash,debe_cambiar_contrasena)
                VALUES(@empleado,N'global',N'NO-AUTENTICABLE',0); DECLARE @usuario int=SCOPE_IDENTITY();
            INSERT dbo.Rol(codigo,nombre,descripcion) VALUES('GLOBAL_LECTOR',N'Global lector',N'Global rol de prueba'); DECLARE @rol int=SCOPE_IDENTITY();
            INSERT dbo.Permiso(modulo,accion,descripcion) VALUES('GLOBAL_PRUEBA','CONSULTAR',N'Global permiso de prueba');
            SELECT @categoria AS Categoria,@proveedor AS Proveedor,@cliente AS Cliente,@tienda AS Tienda,@usuario AS Usuario,@rol AS Rol;
            """).Rows[0];
        int categoriaGlobal = (int)inicial["Categoria"], proveedorGlobal = (int)inicial["Proveedor"], clienteGlobal = (int)inicial["Cliente"],
            otraTienda = (int)inicial["Tienda"], usuarioGlobal = (int)inicial["Usuario"], rolGlobal = (int)inicial["Rol"];
        void Entrar()
        {
            long sesionGlobal = (long)Tabla($"""
                SET NOCOUNT ON;
                INSERT dbo.SesionUsuario(id_usuario,token_hash,version_seguridad,expira_en)
                    SELECT id_usuario,CRYPT_GEN_RANDOM(32),version_seguridad,DATEADD(hour,1,SYSUTCDATETIME()) FROM dbo.Usuario WHERE id_usuario={usuarioGlobal};
                SELECT CONVERT(bigint,SCOPE_IDENTITY());
                """).Rows[0][0];
            sesion.SetValue(null, Activator.CreateInstance(app.GetType("Sistema_ERP.AccesoUsuario")!,
                [usuarioGlobal, "Global", "ADMINISTRADOR", false, Array.Empty<byte>(), sesionGlobal]));
        }
        Array Lineas(int productoId)
        {
            Type tipo = app.GetType("Sistema_ERP.LineaVenta")!;
            Array lineas = Array.CreateInstance(tipo, 1); lineas.SetValue(Activator.CreateInstance(tipo, [productoId, 1, 10m]), 0); return lineas;
        }
        try
        {
            const string nombreEspecial = "Global Café [50%]_especial ' OR 1=1--";
            CargaMasivaDatos.ImportarProductos([new("GLOBAL-001", nombreEspecial, 10m, 20, categoriaGlobal, proveedorGlobal)], tienda);
            CargaMasivaDatos.ImportarProductos([new("GLOBAL-OTRA", "Global producto otra tienda", 10m, 5, categoriaGlobal, proveedorGlobal)], otraTienda);
            CargaMasivaDatos.ImportarProductos(Enumerable.Range(1, 12).Select(i => new ProductoImportado($"GLOBAL-LOTE-{i}", $"Global lote {i:00}", 10, 0, categoriaGlobal, proveedorGlobal)).ToArray(), tienda);
            int productoGlobal = (int)Tabla("SELECT id_producto FROM dbo.Producto WHERE codigo_barras='GLOBAL-001'").Rows[0][0];
            int productoOtra = (int)Tabla("SELECT id_producto FROM dbo.Producto WHERE codigo_barras='GLOBAL-OTRA'").Rows[0][0];
            object venta = Invocar("VentaDatos", "Emitir", tienda, clienteGlobal, efectivo, 20m, "", "Global factura", Lineas(productoGlobal))!;
            object ventaOtra = Invocar("VentaDatos", "Emitir", otraTienda, clienteGlobal, efectivo, 20m, "", "Global otra factura", Lineas(productoOtra))!;
            int ventaGlobal = (int)Prop(venta, "Id"), idVentaOtra = (int)Prop(ventaOtra, "Id");
            Invocar("ApartadoDatos", "Crear", tienda, clienteGlobal, DateTime.UtcNow.AddDays(7), Lineas(productoGlobal));
            int detalleVenta = (int)Tabla($"SELECT id_detalle_venta FROM dbo.DetalleVenta WHERE id_venta={ventaGlobal}").Rows[0][0];
            Invocar("DevolucionDatos", "Guardar", null, null, ventaGlobal, "Global devolución", LineasDevolucion(detalleVenta, 1, true));
            CompraDatos.Guardar(null, null, proveedorGlobal, tienda, "GLOBAL-COMPRA", DateTime.Today, "Global compra", [new(productoGlobal, 1, 5)]);
            int traspaso = Sistema_ERP.Clases.Ingresar.Traspaso(tienda, otraTienda, usuario, "Global solicitud", [new(productoGlobal, 1, 10)]);

            object[] todos = Buscar("Global");
            foreach (string tipo in new[] { "Producto", "Categoría", "Proveedor", "Cliente", "Empleado", "Tienda", "Compra", "Venta", "Apartado", "Devolución", "Traspaso", "Existencia", "Usuario", "Rol", "Permiso" })
            {
                object resultado = todos.FirstOrDefault(r => Tipo(r) == tipo) ?? throw new Exception("Falta el tipo de resultado: " + tipo);
                Comprobar(true, "La búsqueda incluye " + tipo);
                using var datos = (DataSet)Invocar("BusquedaGlobalDatos", "ObtenerDetalle", resultado)!;
                Comprobar(datos.Tables[0].Rows.Count == 1, "El detalle abre exactamente un registro de " + tipo);
                Comprobar(datos.Tables.Cast<DataTable>().All(t => !t.Columns.Contains("contrasena_hash") && !t.Columns.Contains("token_hash")), "El detalle no devuelve credenciales de " + tipo);
                using var dialogo = (Form)Activator.CreateInstance(app.GetType("Sistema_ERP.DetalleBusquedaGlobal")!, instancia, null, [resultado, datos], null)!;
                Comprobar(Descendientes(dialogo).OfType<DataGridView>().All(t => t.ReadOnly && !t.AutoGenerateColumns && !t.Columns.OfType<DataGridViewImageColumn>().Any()), "Detalle de solo lectura sin columnas binarias: " + tipo);
                dialogo.ShowInTaskbar = false; dialogo.Opacity = 0;
                dialogo.StartPosition = FormStartPosition.Manual; dialogo.Location = new Point(-5000, -5000); dialogo.Show();
                object? fuenteAnterior = Descendientes(dialogo).OfType<DataGridView>().First().DataSource;
                var reiniciarDetalle = dialogo.Controls.Find("btnReiniciar", true).OfType<Button>().Single();
                reiniciarDetalle.PerformClick(); EsperarRecarga(reiniciarDetalle);
                Comprobar(!ReferenceEquals(fuenteAnterior, Descendientes(dialogo).OfType<DataGridView>().First().DataSource), "Reiniciar consulta otra vez el detalle de " + tipo);
            }
            Comprobar(todos.Count(r => Tipo(r) == "Producto") == 10, "El límite se aplica por tipo");
            Comprobar(Buscar("CAFE").Any(r => Tipo(r) == "Producto" && Id(r) == productoGlobal), "La búsqueda ignora mayúsculas y acentos");
            foreach (string literal in new[] { "[50%]_", "' OR 1=1--", "%]", "  GLOBAL-001  " })
            {
                var resultados = Buscar(literal);
                Comprobar(resultados.Length > 0 && resultados.All(r => Tipo(r) is "Producto" or "Existencia" && Id(r) == productoGlobal), "Texto literal y parámetros: " + literal);
            }
            Comprobar(Buscar("SIN-COINCIDENCIAS-999999").Length == 0, "Sin coincidencias no quedan resultados previos");
            Comprobar(Buscar("G").Length == 0, "Se requieren dos caracteres");
            bool larga = false; try { Buscar(new string('x', 151)); } catch (ArgumentException) { larga = true; }
            Comprobar(larga, "Se valida la longitud máxima");
            bool cancelada = false;
            try { Invocar("BusquedaGlobalDatos", "Buscar", "Global", null, new CancellationToken(true)); } catch (OperationCanceledException) { cancelada = true; }
            Comprobar(cancelada, "Las consultas canceladas no se ejecutan");
            var locales = Buscar("Global", tienda);
            Comprobar(locales.Any(r => Tipo(r) == "Venta" && Id(r) == ventaGlobal) && locales.All(r => Tipo(r) != "Venta" || Id(r) != idVentaOtra), "Se filtran documentos por la tienda seleccionada");
            Comprobar(locales.Any(r => Tipo(r) == "Proveedor"), "Los catálogos compartidos siguen disponibles");
            Comprobar(Buscar("Global", otraTienda).Any(r => Tipo(r) == "Traspaso" && Id(r) == traspaso), "Los traspasos coinciden con origen o destino");
            Comprobar(Buscar("bitacora").Any(r => Tipo(r) == "Módulo" && (string)Prop(r, "Pantalla") == "Configuración y auditoría"), "También se encuentran módulos por sinónimos");

            using (var principal = (Pagina_Central)Activator.CreateInstance(typeof(Pagina_Central), instancia, null, [original], null)!)
            {
                principal.ShowInTaskbar = false; principal.Opacity = 0; principal.StartPosition = FormStartPosition.Manual; principal.Location = new Point(-5000, -5000);
                principal.Show();
                var campo = principal.Controls.Find("searchBox", true).OfType<TextBox>().Single();
                var buscar = typeof(Pagina_Central).GetMethod("BuscarGlobal", instancia)!;
                campo.Text = "GLOBAL-001"; Bombear((Task)buscar.Invoke(principal, null)!);
                var tabla = principal.Controls.Find("resultadosBusquedaGlobal", true).OfType<DataGridView>().Single();
                string estadoBusqueda = principal.Controls.Find("estadoBusquedaGlobal", true).Single().Text;
                Comprobar(tabla.Rows.Count > 0 && tabla.Rows.Cast<DataGridViewRow>().All(r => (string)r.Cells["Nombre"].Value! == nombreEspecial),
                    $"La barra superior busca contenido fuera del módulo actual ({tabla.Rows.Count} filas; {estadoBusqueda})");
                Comprobar(principal.MdiChildren.Any(p => p.Name == "pantallaBusquedaGlobal" && p.Visible),
                    "Los resultados se muestran dentro del MDI; activa: " + principal.ActiveMdiChild?.Text);
                Ejecutar($"UPDATE dbo.Producto SET nombre=nombre+N' recargado' WHERE id_producto={productoGlobal}");
                var reiniciarBusqueda = principal.MdiChildren.Single(p => p.Name == "pantallaBusquedaGlobal").Controls.Find("btnReiniciar", true).OfType<Button>().Single();
                reiniciarBusqueda.PerformClick(); EsperarRecarga(reiniciarBusqueda);
                Comprobar(campo.Text == "GLOBAL-001" && tabla.Rows.Count > 0 && tabla.Rows.Cast<DataGridViewRow>().All(r => ((string)r.Cells["Nombre"].Value!).EndsWith(" recargado")),
                    "Reiniciar repite la búsqueda y muestra los cambios externos sin borrar la consulta");
                campo.Text = "Global"; Task anterior = (Task)buscar.Invoke(principal, null)!;
                campo.Text = "SIN-COINCIDENCIAS-999999"; Task ultima = (Task)buscar.Invoke(principal, null)!;
                Bombear(Task.WhenAll(anterior, ultima));
                Comprobar(tabla.Rows.Count == 0, "Una respuesta anterior no sobrescribe la última consulta");
                typeof(Pagina_Central).GetMethod("AbrirModuloBusqueda", instancia)!.Invoke(principal, [todos.First(r => Tipo(r) == "Producto")]);
                Comprobar(principal.ActiveMdiChild?.Text == "Catalogo", "Se puede abrir el módulo del resultado");
                Comprobar(principal.Controls.Find("searchBox", true).Single().Visible, "La barra permanece disponible al navegar por MDI");
                var tecla = typeof(Pagina_Central).GetMethod("ProcessCmdKey", instancia)!;
                Comprobar((bool)tecla.Invoke(principal, [new Message(), Keys.Control | Keys.K])!, "Ctrl+K activa el buscador");
                Comprobar((bool)tecla.Invoke(principal, [new Message(), Keys.Escape])! && campo.TextLength == 0 && tabla.Rows.Count == 0, "Escape limpia la búsqueda y los resultados");
                principal.Close();
            }

            Entrar();
            Comprobar(Buscar("Global").Length == 0 && Buscar("inventario").Length == 0, "Una cuenta sin permisos no obtiene datos ni módulos restringidos");
            Ejecutar($"INSERT dbo.UsuarioRol(id_usuario,id_rol) VALUES({usuarioGlobal},{rolGlobal}); INSERT dbo.RolPermiso(id_rol,id_permiso) SELECT {rolGlobal},id_permiso FROM dbo.Permiso WHERE modulo='PRODUCTOS' AND accion='CONSULTAR';");
            Entrar();
            var restringidos = Buscar("Global");
            Comprobar(restringidos.Length > 0 && restringidos.All(r => Tipo(r) is "Producto" or "Categoría"), "La búsqueda global respeta los permisos actuales");
            Denegado(() => Invocar("BusquedaGlobalDatos", "ObtenerDetalle", todos.First(r => Tipo(r) == "Usuario")));
            Denegado(() => Invocar("BusquedaGlobalDatos", "ObtenerDetalle", todos.First(r => Tipo(r) == "Venta")));
            Ejecutar($"UPDATE dbo.UsuarioRol SET activo=0 WHERE id_usuario={usuarioGlobal}"); Entrar();
            Denegado(() => Invocar("BusquedaGlobalDatos", "ObtenerDetalle", restringidos.First(r => Tipo(r) == "Producto")));
            Comprobar(Buscar("Global").Length == 0, "Los resultados anteriores no conceden permisos retirados");
            Console.WriteLine($"OK: {comprobaciones} comprobaciones de búsqueda global, detalle, permisos, tienda, texto literal, cancelación, navegación MDI y atajos.");
        }
        finally { sesion.SetValue(null, original); }

        static IEnumerable<Control> Descendientes(Control control) => new[] { control }.Concat(control.Controls.Cast<Control>().SelectMany(Descendientes));
        static void EsperarRecarga(Button boton)
        {
            DateTime limite = DateTime.UtcNow.AddSeconds(30);
            while (!boton.Enabled && DateTime.UtcNow < limite) { Application.DoEvents(); Thread.Sleep(10); }
            if (!boton.Enabled) throw new TimeoutException("No terminó la recarga de búsqueda.");
        }
        static void Bombear(Task tarea)
        {
            DateTime limite = DateTime.UtcNow.AddSeconds(30);
            while (!tarea.IsCompleted && DateTime.UtcNow < limite) { Application.DoEvents(); Thread.Sleep(10); }
            if (!tarea.IsCompleted) throw new TimeoutException("La búsqueda no terminó dentro del tiempo de prueba.");
            tarea.GetAwaiter().GetResult();
        }
    }
}
