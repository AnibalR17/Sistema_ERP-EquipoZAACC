using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using Sistema_ERP;

internal static class Program
{
    private static readonly Assembly app = typeof(PuntoVenta).Assembly;
    private const BindingFlags estaticos = BindingFlags.Static | BindingFlags.NonPublic;
    private static string cadena = "";
    private static int tienda, cliente, producto, usuario, efectivo, transferencia, categoria, proveedor;

    [STAThread]
    private static int Main()
    {
        ApplicationConfiguration.Initialize();
        string raiz = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
        string? anterior = Environment.GetEnvironmentVariable("SISTEMA_ERP_CONNECTION_STRING");
        string original = anterior ?? File.ReadAllText(Path.Combine(raiz, "connection.local.txt")).Trim();
        string bd = "CodexERP_Prueba_" + Guid.NewGuid().ToString("N");
        var servidor = new SqlConnectionStringBuilder(original) { InitialCatalog = "master" };
        bool creada = false;
        try
        {
            using (var cn = new SqlConnection(servidor.ConnectionString))
            { cn.Open(); using var cmd = new SqlCommand("CREATE DATABASE [" + bd + "]", cn); cmd.ExecuteNonQuery(); creada = true; }
            cadena = new SqlConnectionStringBuilder(original) { InitialCatalog = bd }.ConnectionString;
            Environment.SetEnvironmentVariable("SISTEMA_ERP_CONNECTION_STRING", cadena);
            foreach (string archivo in new[] { "Base de Datos/01_esquema_erp_3fn.sql", "Base de Datos/Triggers2.sql", "Datos/Migraciones/002_bitacora_sistema.sql", "Datos/Migraciones/003_rtn_ventas.sql", "Datos/Migraciones/004_isv_cai.sql" })
            {
                if (archivo.EndsWith("004_isv_cai.sql")) SimularEsquemaAnterior();
                string sql = File.ReadAllText(Path.Combine(raiz, archivo)).Replace("EquipoZAACC", bd);
                foreach (string lote in Regex.Split(sql, @"(?im)^\s*GO\s*$")) if (!string.IsNullOrWhiteSpace(lote)) Ejecutar(lote);
            }
            var inicial = Tabla("""
                SET NOCOUNT ON;
                INSERT dbo.Tienda(codigo,nombre) VALUES('PRUEBA','Tienda prueba'); DECLARE @t int=SCOPE_IDENTITY();
                INSERT dbo.Categoria(nombre) VALUES(N'Categoría prueba'); DECLARE @c int=SCOPE_IDENTITY();
                INSERT dbo.Proveedor(nombre_empresa,rtn) VALUES(N'Proveedor prueba','08019999999999'); DECLARE @p int=SCOPE_IDENTITY();
                INSERT dbo.Cliente(nombre) VALUES(N'Cliente prueba'); DECLARE @cliente int=SCOPE_IDENTITY();
                INSERT dbo.Empleado(codigo,nombre,id_tienda) VALUES('PRUEBA',N'Empleado prueba',@t); DECLARE @e int=SCOPE_IDENTITY();
                INSERT dbo.Usuario(id_empleado,nombre_usuario,contrasena_hash,debe_cambiar_contrasena) VALUES(@e,N'prueba',N'NO-AUTENTICABLE',0); DECLARE @u int=SCOPE_IDENTITY();
                INSERT dbo.SesionUsuario(id_usuario,token_hash,version_seguridad,expira_en)
                    SELECT @u,CRYPT_GEN_RANDOM(32),version_seguridad,DATEADD(hour,1,SYSUTCDATETIME()) FROM dbo.Usuario WHERE id_usuario=@u;
                SELECT @t AS tienda,@cliente AS cliente,@u AS usuario,@c AS categoria,@p AS proveedor,CONVERT(bigint,SCOPE_IDENTITY()) AS sesion,
                    (SELECT id_metodo_pago FROM dbo.MetodoPago WHERE codigo='EFECTIVO') AS efectivo,
                    (SELECT id_metodo_pago FROM dbo.MetodoPago WHERE codigo='TRANSFERENCIA') AS transferencia;
                """).Rows[0];
            tienda = (int)inicial["tienda"]; cliente = (int)inicial["cliente"]; usuario = (int)inicial["usuario"];
            categoria = (int)inicial["categoria"]; proveedor = (int)inicial["proveedor"]; efectivo = (int)inicial["efectivo"]; transferencia = (int)inicial["transferencia"];
            var acceso = Activator.CreateInstance(app.GetType("Sistema_ERP.AccesoUsuario")!, [usuario, "Prueba", "Prueba", false, Array.Empty<byte>(), (long)inicial["sesion"]]);
            app.GetType("Sistema_ERP.SesionActual")!.GetProperty("Usuario", estaticos)!.SetValue(null, acceso);
            ProbarImportacion(); ProbarApartados(); ProbarDevoluciones(); ProbarCasosAdicionales(); ProbarRtn(); ProbarImpuestos(); ProbarExportaciones(); ProbarPlantilla(); ProbarReferenciasPago();
            using var pantalla = new ApartadosDevoluciones(); using var carga = new CargaMasiva(); using var configuracion = new ConfiguracionAuditoria();
            pantalla.Size = new Size(1400, 900); pantalla.CreateControl(); pantalla.PerformLayout();
            carga.Size = new Size(1400, 900); carga.CreateControl(); carga.PerformLayout();
            configuracion.ShowInTaskbar = false; configuracion.StartPosition = FormStartPosition.Manual;
            configuracion.Location = new Point(-5000, -5000); configuracion.Opacity = 0; configuracion.Show();
            var tabs = configuracion.Controls.Find("pestanasConfiguracion", true).OfType<TabControl>().Single();
            Igual(3, tabs.TabPages.Count, "Pestañas de configuración"); tabs.SelectedIndex = 1;
            var tiendaFiscal = configuracion.Controls.Find("tiendaCAI", true).OfType<ComboBox>().Single(); tiendaFiscal.SelectedValue = tienda;
            var campoCai = configuracion.Controls.Find("valorCAI", true).OfType<TextBox>().Single();
            Igual("DEF456-ABC123-DEF456-ABC123-DEF456-BB", campoCai.Text, "Pantalla carga CAI de la tienda seleccionada");
            Console.WriteLine("OK: construcción de las pantallas y disposición de controles.");
            Console.WriteLine("TODAS LAS PRUEBAS COMPLETADAS. La base temporal se elimina al salir.");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
        finally
        {
            app.GetType("Sistema_ERP.SesionActual")!.GetProperty("Usuario", estaticos)!.SetValue(null, null);
            Environment.SetEnvironmentVariable("SISTEMA_ERP_CONNECTION_STRING", anterior);
            SqlConnection.ClearAllPools();
            if (creada && Regex.IsMatch(bd, "^CodexERP_Prueba_[a-f0-9]{32}$"))
            {
                using var cn = new SqlConnection(servidor.ConnectionString); cn.Open();
                using var cmd = new SqlCommand("ALTER DATABASE [" + bd + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [" + bd + "]", cn); cmd.ExecuteNonQuery();
            }
        }
    }

    private static void ProbarImportacion()
    {
        CargaMasivaDatos.ImportarProductos([new("000123456789", "Producto prueba", 10m, 20, categoria, proveedor, 2)], tienda, "prueba.xlsx", new byte[32]);
        producto = (int)Tabla("SELECT id_producto FROM dbo.Producto WHERE codigo_barras='000123456789'").Rows[0][0];
        Stock(20, 0);
        Igual(1, Convert.ToInt32(Tabla("SELECT COUNT(*) FROM dbo.Importacion WHERE estado='COMPLETADA'").Rows[0][0]), "Historial de importación");
        Fallar(() => CargaMasivaDatos.ImportarProductos([new("PRIMERO", "Debe revertirse", 5, 2, categoria, proveedor), new("000123456789", "Duplicado", 5, 2, categoria, proveedor)], tienda));
        Igual(0, Convert.ToInt32(Tabla("SELECT COUNT(*) FROM dbo.Producto WHERE codigo_barras='PRIMERO'").Rows[0][0]), "Importación atómica"); Stock(20, 0);
        Igual(proveedor, CargaMasivaDatos.MapaProveedoresPorRtn()["08019999999999"], "RTN del esquema 3FN");
        Console.WriteLine("OK: carga masiva, stock por tienda, RTN, historial y rollback de lote duplicado.");
    }

    private static void SimularEsquemaAnterior()
    {
        if (!Regex.IsMatch(new SqlConnectionStringBuilder(cadena).InitialCatalog, "^CodexERP_Prueba_[a-f0-9]{32}$")) throw new InvalidOperationException("Solo se simula el esquema anterior en la base temporal de pruebas.");
        Ejecutar("""
            ALTER TABLE dbo.DetalleVenta DROP CONSTRAINT CK_DetalleVenta_tasa_isv,DF_DetalleVenta_tasa_isv;
            ALTER TABLE dbo.DetalleVenta DROP COLUMN total_linea,isv;
            ALTER TABLE dbo.DetalleVenta DROP COLUMN tasa_isv;
            ALTER TABLE dbo.DetalleApartado DROP CONSTRAINT CK_DetalleApartado_tasa_isv,DF_DetalleApartado_tasa_isv;
            ALTER TABLE dbo.DetalleApartado DROP COLUMN total_linea,isv;
            ALTER TABLE dbo.DetalleApartado DROP COLUMN tasa_isv;
            ALTER TABLE dbo.DetalleDevolucion DROP CONSTRAINT CK_DetalleDevolucion_isv,DF_DetalleDevolucion_isv;
            ALTER TABLE dbo.DetalleDevolucion DROP COLUMN isv;
            ALTER TABLE dbo.Venta DROP COLUMN cai;
            DROP TABLE dbo.ConfiguracionFacturacion;
            """);
    }

    private static int apartadoEntregado, ventaEntregada;
    private static void ProbarApartados()
    {
        int id = CrearApartado(2); apartadoEntregado = id; Stock(20, 2);
        Fallar(() => Invocar("ApartadoDatos", "Abonar", id, VersionApartado(id), transferencia, 5m, ""));
        Invocar("ApartadoDatos", "Abonar", id, VersionApartado(id), efectivo, 5m, "");
        string referenciaAbono = Convert.ToString(Tabla("SELECT referencia FROM dbo.AbonoApartado WHERE id_apartado=" + id).Rows[0][0])!;
        Igual(true, Regex.IsMatch(referenciaAbono, "^EFE-[a-f0-9]{32}$"), "Referencia automática del abono en efectivo");
        Fallar(() => Invocar("ApartadoDatos", "Abonar", id, VersionApartado(id), efectivo, 30m, ""));
        Invocar("ApartadoDatos", "Abonar", id, VersionApartado(id), transferencia, 5m, "ABONO-2");
        Fallar(() => Invocar("ApartadoDatos", "Entregar", id, VersionApartado(id), transferencia, 0m, ""));
        var factura = Invocar("ApartadoDatos", "Entregar", id, VersionApartado(id), efectivo, 20m, "")!;
        Igual(23m, (decimal)Prop(factura, "Total"), "Factura de apartado con ISV"); Igual(7m, (decimal)Prop(factura, "Cambio"), "Cambio");
        ventaEntregada = (int)Prop(factura, "Id"); Stock(18, 0);
        Igual("ENTREGADO", Convert.ToString(Tabla("SELECT estado FROM dbo.Apartado WHERE id_apartado=" + id).Rows[0][0])!, "Entrega de apartado");
        Igual(2, Convert.ToInt32(Tabla("SELECT COUNT(*) FROM dbo.AbonoApartado WHERE id_apartado=" + id + " AND id_pago_venta IS NOT NULL").Rows[0][0]), "Enlace de abonos");
        var pagos = Tabla($"SELECT p.referencia,m.codigo FROM dbo.PagoVenta p JOIN dbo.MetodoPago m ON m.id_metodo_pago=p.id_metodo_pago WHERE p.id_venta={ventaEntregada}");
        var efectivoApartado = pagos.AsEnumerable().Where(p => (string)p["codigo"] == "EFECTIVO").Select(p => (string)p["referencia"]).ToArray();
        Igual(2, efectivoApartado.Length, "Abono y pago final en efectivo");
        Igual(true, efectivoApartado.Contains(referenciaAbono), "La entrega conserva la referencia original del abono");
        Igual(true, efectivoApartado.All(r => Regex.IsMatch(r, "^EFE-[a-f0-9]{32}$")), "Referencia automática del saldo en efectivo");
        Igual(2, efectivoApartado.Distinct().Count(), "Cada cobro de efectivo tiene su referencia");
        Igual("ABONO-2", (string)pagos.AsEnumerable().Single(p => (string)p["codigo"] == "TRANSFERENCIA")["referencia"], "Se conserva la referencia del abono por transferencia");
        Fallar(() => Invocar("ApartadoDatos", "Entregar", id, VersionApartado(id), efectivo, 20m, ""));
        int cancelar = CrearApartado(1); Invocar("ApartadoDatos", "Abonar", cancelar, VersionApartado(cancelar), efectivo, 3m, "");
        Invocar("ApartadoDatos", "Cerrar", cancelar, VersionApartado(cancelar), false, "Prueba cancelación"); Stock(18, 0);
        Igual(3m, (decimal)Invocar("ApartadoDatos", "Reembolsar", cancelar, VersionApartado(cancelar), "Devolver abono")!, "Reembolso de abono");
        Fallar(() => Invocar("ApartadoDatos", "Reembolsar", cancelar, VersionApartado(cancelar), "Duplicado"));
        int vencido = CrearApartado(1);
        Invocar("ApartadoDatos", "Abonar", vencido, VersionApartado(vencido), efectivo, 1m, "");
        Ejecutar("UPDATE dbo.Apartado SET fecha=DATEADD(day,-2,SYSUTCDATETIME()),vence_en=DATEADD(day,-1,SYSUTCDATETIME()) WHERE id_apartado=" + vencido);
        Fallar(() => Invocar("ApartadoDatos", "Abonar", vencido, VersionApartado(vencido), efectivo, 1m, ""));
        Invocar("ApartadoDatos", "Cerrar", vencido, VersionApartado(vencido), true, "Prueba vencimiento"); Stock(18, 0);
        Igual(1m, (decimal)Invocar("ApartadoDatos", "Reembolsar", vencido, VersionApartado(vencido), "Abono vencido")!, "Reembolso de apartado vencido");
        Console.WriteLine("OK: reservas, abonos, rechazo de sobrepago, entrega con pagos enlazados, cancelación, vencimiento y reembolso único.");
    }

    private static void ProbarDevoluciones()
    {
        int detalle = (int)Tabla("SELECT id_detalle_venta FROM dbo.DetalleVenta WHERE id_venta=" + ventaEntregada).Rows[0][0];
        int Crear(int cantidad, bool stock) => (int)Invocar("DevolucionDatos", "Guardar", null, null, ventaEntregada, "Prueba", LineasDevolucion(detalle, cantidad, stock))!;
        int uno = Crear(1, true); Invocar("DevolucionDatos", "Confirmar", uno, VersionDevolucion(uno)); Stock(19, 0);
        Igual(11.5m, (decimal)Invocar("DevolucionDatos", "Reembolsar", uno, VersionDevolucion(uno), "Reembolso parcial")!, "Reembolso con varios pagos e ISV");
        Fallar(() => Invocar("DevolucionDatos", "Reembolsar", uno, VersionDevolucion(uno), "Duplicado"));
        Fallar(() => Invocar("DevolucionDatos", "Anular", uno, VersionDevolucion(uno), "Con reembolso"));
        int dos = Crear(1, false); byte[] antigua = VersionDevolucion(dos);
        Invocar("DevolucionDatos", "Guardar", dos, antigua, ventaEntregada, "Motivo editado", LineasDevolucion(detalle, 1, false));
        Fallar(() => Invocar("DevolucionDatos", "Confirmar", dos, antigua));
        Invocar("DevolucionDatos", "Confirmar", dos, VersionDevolucion(dos)); Stock(19, 0);
        Invocar("DevolucionDatos", "Reembolsar", dos, VersionDevolucion(dos), "Segundo reembolso");
        Fallar(() => Crear(1, true)); Stock(19, 0);
        object factura = Invocar("VentaDatos", "Emitir", tienda, cliente, efectivo, 30m, "", "Prueba", LineasVenta(2))!;
        int venta = (int)Prop(factura, "Id"); int dv = (int)Tabla("SELECT id_detalle_venta FROM dbo.DetalleVenta WHERE id_venta=" + venta).Rows[0][0];
        int tres = (int)Invocar("DevolucionDatos", "Guardar", null, null, venta, "Prueba reverso", LineasDevolucion(dv, 1, true))!;
        Invocar("DevolucionDatos", "Confirmar", tres, VersionDevolucion(tres)); Stock(18, 0);
        Invocar("DevolucionDatos", "Anular", tres, VersionDevolucion(tres), "Anular sin reembolso"); Stock(17, 0);
        Igual(0, Convert.ToInt32(Tabla("SELECT COUNT(*) FROM dbo.Auditoria WHERE entidad IN('Apartado','Devolucion','AbonoApartado','Reembolso') AND id_usuario IS NULL").Rows[0][0]), "Actor de auditoría");
        Console.WriteLine("OK: devoluciones parciales, reintegro opcional, edición, concurrencia, límite vendido, reembolsos y reverso de inventario.");
    }

    private static void ProbarPlantilla()
    {
        string ruta = Path.Combine(Path.GetTempPath(), "ERP-plantilla-prueba-" + Guid.NewGuid().ToString("N") + ".xlsx");
        try
        {
            Invocar("CargaMasivaPlantilla", "Crear", ruta);
            using (var libro = new XLWorkbook(ruta))
            { Igual(XLDataType.Text, libro.Worksheet(1).Cell(2, 1).DataType, "Código como texto"); Igual(XLDataType.Text, libro.Worksheet(1).Cell(2, 6).DataType, "RTN como texto"); }
            Type carga = typeof(CargaMasiva); var tabla = (DataTable)carga.GetMethod("LeerArchivo", estaticos)!.Invoke(null, [ruta])!;
            Type cat = carga.GetNestedType("Catalogos", BindingFlags.NonPublic)!; object catalogos = Activator.CreateInstance(cat)!;
            cat.GetProperty("CodigosBD")!.SetValue(catalogos, CargaMasivaDatos.CodigosExistentes());
            cat.GetProperty("Categorias")!.SetValue(catalogos, CargaMasivaDatos.MapaCategorias());
            cat.GetProperty("Proveedores")!.SetValue(catalogos, CargaMasivaDatos.MapaProveedoresPorRtn());
            foreach (DataRow fila in tabla.Rows) Igual(true, (bool)carga.GetMethod("ValidarFila", estaticos)!.Invoke(null, [fila, catalogos])!, "Fila válida de plantilla: " + fila["Observacion"]);
            Igual(10, tabla.Rows.Count, "Diez ejemplos");
            var negativa = tabla.NewRow(); negativa.ItemArray = ["NUEVO", "Inválido", "-2", "0", "Categoría prueba", "08019999999999", ""];
            Igual(false, (bool)carga.GetMethod("ValidarFila", estaticos)!.Invoke(null, [negativa, catalogos])!, "Precio negativo rechazado");
            var productos = (List<ProductoImportado>)carga.GetMethod("ConstruirProductosValidos", estaticos)!.Invoke(null, [tabla, catalogos])!;
            Igual(10, CargaMasivaDatos.ImportarProductos(productos, tienda, "plantilla.xlsx", System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(ruta))), "Importación desde plantilla");
            Igual(35, Convert.ToInt32(Tabla("SELECT SUM(i.existencia) FROM dbo.InventarioTienda i JOIN dbo.Producto p ON p.id_producto=i.id_producto WHERE p.codigo_barras LIKE '000990%' AND i.id_tienda=" + tienda).Rows[0][0]), "Stock de los ejemplos");
            Console.WriteLine("OK: XLSX leído y validado por el importador real, ceros iniciales y rechazo de datos inválidos.");
        }
        finally { if (File.Exists(ruta)) File.Delete(ruta); }
    }

    private static void ProbarRtn()
    {
        object Emitir(string? rtn) => Invocar("VentaDatos", "Emitir", tienda, cliente, efectivo, 20m, "", "Prueba RTN", LineasVenta(1), rtn)!;
        object factura = Emitir("0801-9999-999999"); int id = (int)Prop(factura, "Id");
        Igual("08019999999999", Convert.ToString(Tabla("SELECT rtn_cliente FROM dbo.Venta WHERE id_venta=" + id).Rows[0][0])!, "RTN normalizado y guardado");
        string texto = (string)Invocar("VentaDatos", "Comprobante", id)!;
        Igual(true, texto.Contains("RTN: 08019999999999"), "RTN impreso");
        Ejecutar("UPDATE dbo.Cliente SET documento_fiscal='08028888888888' WHERE id_cliente=" + cliente);
        Igual(true, ((string)Invocar("VentaDatos", "Comprobante", id)!).Contains("RTN: 08019999999999"), "RTN histórico independiente del cliente");
        object sinRtn = Emitir("   "); int sinId = (int)Prop(sinRtn, "Id");
        Igual(true, Tabla("SELECT rtn_cliente FROM dbo.Venta WHERE id_venta=" + sinId).Rows[0][0] == DBNull.Value, "RTN opcional se guarda nulo");
        Igual(false, ((string)Invocar("VentaDatos", "Comprobante", sinId)!).Contains("RTN:"), "Comprobante sin RTN");
        object ceros = Emitir("0000 0000 000001");
        Igual(true, ((string)Invocar("VentaDatos", "Comprobante", (int)Prop(ceros, "Id"))!).Contains("RTN: 00000000000001"), "Ceros iniciales conservados");
        int antes = Convert.ToInt32(Tabla("SELECT COUNT(*) FROM dbo.Venta").Rows[0][0]);
        Fallar(() => Emitir("123")); Fallar(() => Emitir("0801999999999A"));
        Igual(antes, Convert.ToInt32(Tabla("SELECT COUNT(*) FROM dbo.Venta").Rows[0][0]), "RTN inválido no crea ventas");
        Igual(true, Convert.ToString(Tabla("SELECT valores_despues_json FROM dbo.Auditoria WHERE entidad='Venta' AND accion='CREAR' AND id_registro='" + id + "'").Rows[0][0])!.Contains("08019999999999"), "RTN en auditoría");

        using var pantalla = new PuntoVenta(); pantalla.CreateControl();
        var combo = (ComboBox)typeof(PuntoVenta).GetField("cliente", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(pantalla)!;
        var campo = pantalla.Controls.Find("rtnCliente", true).OfType<TextBox>().Single();
        var clientes = new DataTable(); clientes.Columns.Add("id_cliente", typeof(int)); clientes.Columns.Add("nombre"); clientes.Columns.Add("documento_fiscal");
        clientes.Rows.Add(1, "Con RTN", "0801-9999-999999"); clientes.Rows.Add(2, "Sin RTN", DBNull.Value);
        combo.ValueMember = "id_cliente"; combo.DisplayMember = "nombre"; combo.DataSource = clientes;
        combo.SelectedIndex = 0; Igual("08019999999999", campo.Text, "Precarga del RTN del cliente");
        campo.Text = "00000000000001"; combo.SelectedIndex = 1; Igual("", campo.Text, "Cambio de cliente limpia RTN anterior");
        Console.WriteLine("OK: RTN opcional, validación, almacenamiento, impresión, historial, auditoría y precarga por cliente.");
    }

    private static void ProbarCasosAdicionales()
    {
        int id = CrearApartado(1);
        Invocar("ApartadoDatos", "Abonar", id, VersionApartado(id), efectivo, 11.5m, "");
        var factura = Invocar("ApartadoDatos", "Entregar", id, VersionApartado(id), 0, 0m, "")!;
        Igual(11.5m, (decimal)Prop(factura, "Total"), "Apartado completamente abonado con ISV"); Stock(16, 0);
        Igual(1, Convert.ToInt32(Tabla("SELECT COUNT(*) FROM dbo.PagoVenta WHERE id_venta=" + (int)Prop(factura, "Id")).Rows[0][0]), "La entrega sin saldo no crea otro pago");
        CargaMasivaDatos.ImportarProductos([new("SIN-STOCK", "Sin stock", 10, 0, categoria, proveedor)]);
        int sinStock = (int)Tabla("SELECT id_producto FROM dbo.Producto WHERE codigo_barras='SIN-STOCK'").Rows[0][0];
        int antes = Convert.ToInt32(Tabla("SELECT COUNT(*) FROM dbo.Apartado").Rows[0][0]);
        Type t = app.GetType("Sistema_ERP.LineaVenta")!; var lineas = Array.CreateInstance(t, 2);
        lineas.SetValue(Activator.CreateInstance(t, [producto, 3, 10m]), 0); lineas.SetValue(Activator.CreateInstance(t, [sinStock, 1, 10m]), 1);
        Fallar(() => Invocar("ApartadoDatos", "Crear", tienda, cliente, DateTime.UtcNow.AddDays(3), lineas));
        Stock(16, 0); Igual(antes, Convert.ToInt32(Tabla("SELECT COUNT(*) FROM dbo.Apartado").Rows[0][0]), "Reserva fallida completamente revertida");
        Console.WriteLine("OK: entrega sin saldo y rollback de una reserva de varios productos con stock insuficiente.");
    }

    private static void ProbarImpuestos()
    {
        const string cai = "ABC123-DEF456-ABC123-DEF456-ABC123-AA", siguiente = "DEF456-ABC123-DEF456-ABC123-DEF456-BB";
        Invocar("ConfiguracionFacturacionDatos", "Guardar", tienda, cai, null);
        byte[] VersionCai() => (byte[])((DataTable)Invocar("ConfiguracionFacturacionDatos", "Obtener", tienda)!).Rows[0]["version_fila"];
        byte[] primeraVersion = VersionCai();
        Array Linea(int prod, int unidades, decimal precio, decimal tasa)
        {
            Type tipo = app.GetType("Sistema_ERP.LineaVenta")!; var array = Array.CreateInstance(tipo, 1);
            var linea = Activator.CreateInstance(tipo, [prod, unidades, precio])!; tipo.GetProperty("TasaIsv")!.SetValue(linea, tasa); array.SetValue(linea, 0); return array;
        }
        object Venta(decimal tasa, decimal recibido) => Invocar("VentaDatos", "Emitir", tienda, cliente, efectivo, recibido, "", "Prueba ISV", Linea(producto, 1, 10m, tasa))!;
        object factura = Venta(15, 20); int id = (int)Prop(factura, "Id");
        Igual(11.5m, (decimal)Prop(factura, "Total"), "ISV 15% adicional"); Igual(8.5m, (decimal)Prop(factura, "Cambio"), "Cambio con impuesto");
        var importes = Tabla("SELECT subtotal,isv,total FROM dbo.vw_VentaTotal WHERE id_venta=" + id).Rows[0];
        Igual(10m, (decimal)importes[0], "Base imponible"); Igual(1.5m, (decimal)importes[1], "ISV guardado"); Igual(11.5m, (decimal)importes[2], "Total cobrado");
        string texto = (string)Invocar("VentaDatos", "Comprobante", id)!;
        Igual(true, texto.Contains(cai) && texto.Contains("ISV 15%") && texto.Contains("Subtotal:"), "Desglose y CAI en recibo");
        Invocar("ConfiguracionFacturacionDatos", "Guardar", tienda, siguiente, primeraVersion);
        Fallar(() => Invocar("ConfiguracionFacturacionDatos", "Guardar", tienda, cai, primeraVersion));
        Fallar(() => Invocar("ConfiguracionFacturacionDatos", "Guardar", tienda, "CAI-INVALIDO", VersionCai()));
        Igual(true, ((string)Invocar("VentaDatos", "Comprobante", id)!).Contains(cai), "Reimpresión conserva CAI original");
        var especial = Venta(18, 20); Igual(11.8m, (decimal)Prop(especial, "Total"), "ISV 18%");
        Igual(true, ((string)Invocar("VentaDatos", "Comprobante", (int)Prop(especial, "Id"))!).Contains(siguiente), "Nuevas facturas usan CAI actualizado");
        Igual(10m, (decimal)Prop(Venta(0, 10), "Total"), "Venta exenta");
        int antes = Convert.ToInt32(Tabla("SELECT COUNT(*) FROM dbo.Venta").Rows[0][0]);
        Fallar(() => Venta(15, 11.49m)); Fallar(() => Venta(7.5m, 20));
        Igual(antes, Convert.ToInt32(Tabla("SELECT COUNT(*) FROM dbo.Venta").Rows[0][0]), "No se guarda una venta con pago insuficiente o tasa inválida");

        int otra = (int)Tabla("INSERT dbo.Tienda(codigo,nombre) VALUES('OTRA','Otra tienda'); SELECT CONVERT(int,SCOPE_IDENTITY())").Rows[0][0];
        Ejecutar($"INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,motivo) VALUES({otra},{producto},'AJUSTE',2,{usuario},N'Prueba tienda CAI')");
        var otraFactura = Invocar("VentaDatos", "Emitir", otra, cliente, efectivo, 20m, "", "Prueba tienda", Linea(producto, 1, 10m, 15))!;
        Igual(true, ((string)Invocar("VentaDatos", "Comprobante", (int)Prop(otraFactura, "Id"))!).Contains("CAI: Sin configurar"), "CAI independiente por tienda");
        Igual(usuario, Convert.ToInt32(Tabla("SELECT TOP(1) id_usuario FROM dbo.Auditoria WHERE entidad='ConfiguracionFacturacion' ORDER BY id_auditoria DESC").Rows[0][0]), "CAI auditado");

        CargaMasivaDatos.ImportarProductos([new("REDONDEO", "Redondeo", 0.01m, 4, categoria, proveedor)], tienda);
        int prod = (int)Tabla("SELECT id_producto FROM dbo.Producto WHERE codigo_barras='REDONDEO'").Rows[0][0];
        var pequena = Invocar("VentaDatos", "Emitir", tienda, cliente, efectivo, 0.05m, "", "Redondeo", Linea(prod, 4, 0.01m, 15))!;
        int venta = (int)Prop(pequena, "Id"); Igual(0.05m, (decimal)Prop(pequena, "Total"), "Redondeo fiscal por línea");
        int detalle = (int)Tabla("SELECT id_detalle_venta FROM dbo.DetalleVenta WHERE id_venta=" + venta).Rows[0][0];
        decimal reembolsado = 0;
        for (int i = 0; i < 2; i++)
        {
            int devolucion = (int)Invocar("DevolucionDatos", "Guardar", null, null, venta, "Prueba centavos", LineasDevolucion(detalle, 2, true))!;
            Invocar("DevolucionDatos", "Confirmar", devolucion, VersionDevolucion(devolucion));
            reembolsado += (decimal)Invocar("DevolucionDatos", "Reembolsar", devolucion, VersionDevolucion(devolucion), "Devolver centavos")!;
        }
        Igual(0.05m, reembolsado, "Devoluciones parciales restituyen exactamente base e ISV original");

        using var pos = new PuntoVenta(); pos.CreateControl();
        var lineas = (DataTable)typeof(PuntoVenta).GetField("detalle", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(pos)!;
        lineas.Rows.Add(producto, "Prueba", 2, 100m);
        typeof(PuntoVenta).GetMethod("Totales", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(pos, null);
        Igual(230m, (decimal)lineas.Rows[0]["Total"], "Pantalla calcula total con ISV");
        lineas.Rows[0]["TasaISV"] = 18m; typeof(PuntoVenta).GetMethod("Totales", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(pos, null);
        Igual(236m, (decimal)lineas.Rows[0]["Total"], "Pantalla permite tasa 18%");
        Console.WriteLine("OK: ISV 15/18/exento, totales, cambio, redondeos, devoluciones con impuesto y CAI por tienda, histórico y auditado.");
    }

    private static void ProbarReferenciasPago()
    {
        object Vender(int metodo, string referencia, decimal recibido = 20m)
            => Invocar("VentaDatos", "Emitir", tienda, cliente, metodo, recibido, referencia, "Referencia de pago", LineasVenta(1))!;
        var referencias = new HashSet<string>();
        foreach (string entrada in new[] { "", "REFERENCIA-ANTERIOR" })
        {
            int venta = (int)Prop(Vender(efectivo, entrada), "Id");
            var pago = Tabla("SELECT id_pago_venta,referencia FROM dbo.PagoVenta WHERE id_venta=" + venta).Rows[0];
            string referencia = (string)pago["referencia"];
            Igual(true, Regex.IsMatch(referencia, "^EFE-[a-f0-9]{32}$"), "Venta en efectivo genera su referencia");
            Igual(true, referencias.Add(referencia), "Referencias diferentes entre ventas");
            Igual(true, ((string)Invocar("VentaDatos", "Comprobante", venta)!).Contains("Referencia: " + referencia), "Recibo incluye la referencia guardada");
            int eventos = Convert.ToInt32(Tabla($"SELECT COUNT(*) FROM dbo.Auditoria WHERE entidad='PagoVenta' AND accion='CREAR' AND TRY_CONVERT(int,JSON_VALUE(id_registro,'$.id_pago_venta'))={pago["id_pago_venta"]} AND id_usuario={usuario}").Rows[0][0]);
            Igual(1, eventos, "Pago con referencia automática auditado con su usuario");
        }
        int transferenciaId = (int)Prop(Vender(transferencia, "  BANCO-001  "), "Id");
        Igual("BANCO-001", (string)Tabla("SELECT referencia FROM dbo.PagoVenta WHERE id_venta=" + transferenciaId).Rows[0][0], "Otros métodos conservan la referencia manual");
        int ventasAntes = Convert.ToInt32(Tabla("SELECT COUNT(*) FROM dbo.Venta").Rows[0][0]);
        int pagosAntes = Convert.ToInt32(Tabla("SELECT COUNT(*) FROM dbo.PagoVenta").Rows[0][0]);
        Fallar(() => Vender(transferencia, "")); Fallar(() => Vender(transferencia, "   ")); Fallar(() => Vender(efectivo, "", 0m));
        Igual(ventasAntes, Convert.ToInt32(Tabla("SELECT COUNT(*) FROM dbo.Venta").Rows[0][0]), "Cobro rechazado no crea una factura");
        Igual(pagosAntes, Convert.ToInt32(Tabla("SELECT COUNT(*) FROM dbo.PagoVenta").Rows[0][0]), "Cobro rechazado no crea un pago");

        void ProbarSelector(Form ventana, ComboBox metodo)
        {
            var campo = ventana.Controls.Find("referenciaPago", true).OfType<TextBox>().Single();
            metodo.SelectedValue = transferencia; campo.Text = "BANCO-001";
            metodo.SelectedValue = efectivo;
            Igual(true, campo.ReadOnly, "Efectivo indica referencia automática");
            Igual("", campo.Text, "Efectivo limpia una referencia manual anterior");
            Igual("Se genera al registrar el pago", campo.PlaceholderText, "Ayuda visible para referencia automática");
            metodo.SelectedValue = transferencia;
            Igual(false, campo.ReadOnly, "Otros métodos permiten introducir la referencia");
            campo.Text = "BANCO-002";
            Igual("BANCO-002", campo.Text, "Referencia manual editable al cambiar de método");
        }
        using var pantalla = new PuntoVenta(); pantalla.CreateControl();
        var selector = (ComboBox)typeof(PuntoVenta).GetField("metodo", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(pantalla)!;
        selector.ValueMember = "id_metodo_pago"; selector.DisplayMember = "nombre";
        selector.DataSource = Tabla("SELECT id_metodo_pago,nombre,codigo FROM dbo.MetodoPago WHERE activo=1");
        ProbarSelector(pantalla, selector);
        var dialogoTipo = typeof(ApartadosDevoluciones).GetNestedType("CobroDialogo", BindingFlags.NonPublic)!;
        foreach (bool abono in new[] { true, false })
        {
            using var dialogo = (Form)Activator.CreateInstance(dialogoTipo, BindingFlags.Instance | BindingFlags.NonPublic, null, ["Cobro", 10m, abono], null)!;
            dialogo.ShowInTaskbar = false; dialogo.StartPosition = FormStartPosition.Manual;
            dialogo.Location = new Point(-5000, -5000); dialogo.Opacity = 0; dialogo.Show();
            var metodo = (ComboBox)dialogoTipo.GetField("metodo", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(dialogo)!;
            ProbarSelector(dialogo, metodo);
        }
        Console.WriteLine("OK: referencias automáticas en ventas, abonos y entregas; recibo, auditoría, referencias manuales y cambio de método en pantalla.");
    }

    private static object? Invocar(string tipo, string metodo, params object?[] args)
    {
        var funcion = app.GetType("Sistema_ERP." + tipo)!.GetMethod(metodo, estaticos)!;
        var parametros = funcion.GetParameters();
        if (args.Length > parametros.Length) throw new ArgumentException("Demasiados argumentos para " + metodo);
        var completos = parametros.Select((_, i) => i < args.Length ? args[i] : Type.Missing).ToArray();
        try { return funcion.Invoke(null, completos); }
        catch (TargetInvocationException ex) when (ex.InnerException != null) { System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw(); throw; }
    }

    private static void ProbarExportaciones()
    {
        string carpeta = Path.Combine(Path.GetTempPath(), "ERP-exportaciones-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(carpeta);
        string excel = Path.Combine(carpeta, "catalogo.xlsx"), pdf = Path.Combine(carpeta, "recibo.pdf"), extenso = Path.Combine(carpeta, "recibo-extenso.pdf");
        try
        {
            using var todos = ProductoDatos.Listar(false, tienda);
            todos.DefaultView.RowFilter = "codigo_barras='000123456789'";
            using var seleccion = todos.DefaultView.ToTable();
            Invocar("ExportacionArchivos", "CatalogoExcel", seleccion, excel, "Tienda prueba", "000123456789", false);
            using (var libro = new XLWorkbook(excel))
            {
                var hoja = libro.Worksheet("Catalogo");
                Igual(2, hoja.LastRowUsed()!.RowNumber(), "Excel contiene únicamente el catálogo filtrado");
                Igual("000123456789", hoja.Cell(2, 1).GetString(), "Excel conserva ceros iniciales");
                Igual(XLDataType.Text, hoja.Cell(2, 1).DataType, "Código exportado como texto");
                Igual(10m, hoja.Cell(2, 5).GetValue<decimal>(), "Precio exportado como número");
                Igual((int)seleccion.Rows[0]["stock_actual"], hoja.Cell(2, 4).GetValue<int>(), "Stock de la tienda exportado");
                Igual("Tienda prueba", libro.Worksheet("Información").Cell(2, 2).GetString(), "Alcance del catálogo exportado");
            }
            byte[] anterior = File.ReadAllBytes(excel); bool bloqueado = false;
            using (var bloqueo = new FileStream(excel, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                try { Invocar("ExportacionArchivos", "CatalogoExcel", seleccion, excel, "Tienda prueba", "", false); }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { bloqueado = true; }
            Igual(true, bloqueado, "Archivo abierto no se sobrescribe parcialmente");
            Igual(true, anterior.SequenceEqual(File.ReadAllBytes(excel)), "El Excel previo se conserva tras un error");

            object factura = Invocar("VentaDatos", "Emitir", tienda, cliente, efectivo, 20m, "", "Exportación de José, devolución y CAI.", LineasVenta(1), "08019999999999")!;
            int id = (int)Prop(factura, "Id"); string numero = (string)Prop(factura, "Numero");
            string recibo = (string)Invocar("VentaDatos", "Comprobante", id)!;
            Task.Run(() => Invocar("ExportacionArchivos", "ReciboPdf", pdf, numero, recibo)).GetAwaiter().GetResult();
            string Normalizar(string t) => Regex.Replace(t, @"\s", "");
            using (var documento = UglyToad.PdfPig.PdfDocument.Open(pdf))
            {
                string texto = Normalizar(string.Join("\n", documento.GetPages().Select(p => p.Text)));
                foreach (string dato in new[] { numero, "08019999999999", "DEF456-ABC123-DEF456-ABC123-DEF456-BB", "ISV 15%", "Subtotal:", "Total:", "Cambio:", "Exportación de José, devolución y CAI." })
                    Igual(true, texto.Contains(Normalizar(dato)), "PDF conserva: " + dato);
            }
            string largo = recibo + "\n" + string.Join("\n", Enumerable.Range(1, 220).Select(i => "Detalle " + i + ": devolución, artículos y observaciones."))
                + "\n" + new string('X', 4000) + "\nMARCADOR_FINAL_DEL_RECIBO";
            Invocar("ExportacionArchivos", "ReciboPdf", extenso, numero, largo);
            using (var documento = UglyToad.PdfPig.PdfDocument.Open(extenso))
            {
                Igual(true, documento.NumberOfPages > 1, "PDF admite varias páginas");
                string texto = Normalizar(string.Join("\n", documento.GetPages().Select(p => p.Text)));
                Igual(true, texto.Contains("MARCADOR_FINAL_DEL_RECIBO"), "El PDF no pierde el final del recibo");
                Igual(4000, documento.GetPages().SelectMany(p => p.Letters).Count(l => l.Value == "X"), "El PDF ajusta palabras extensas sin perder caracteres");
                foreach (var pagina in documento.GetPages())
                    Igual(true, pagina.Letters.All(l => l.GlyphRectangle.Left >= 20 && l.GlyphRectangle.Right <= pagina.Width - 20), "Texto PDF dentro del ancho de página");
            }
            Invocar("AuditoriaDatos", "RegistrarExportacion", "PRODUCTOS", "Catalogo", tienda.ToString(), "Catálogo exportado en prueba");
            Invocar("AuditoriaDatos", "RegistrarExportacion", "VENTAS", "Venta", id.ToString(), "Recibo PDF exportado en prueba");
            Igual(2, Convert.ToInt32(Tabla("SELECT COUNT(*) FROM dbo.Auditoria WHERE accion='EXPORTAR' AND id_usuario=" + usuario).Rows[0][0]), "Exportaciones auditadas");
            Console.WriteLine("OK: catálogo Excel filtrado, códigos y números; PDF con RTN/CAI/ISV, acentos, varias páginas, protección de archivos y auditoría.");
        }
        finally { foreach (string archivo in Directory.EnumerateFiles(carpeta)) File.Delete(archivo); Directory.Delete(carpeta); }
    }
    private static object Prop(object o, string nombre) => o.GetType().GetProperty(nombre)!.GetValue(o)!;
    private static Array LineasVenta(int cantidad)
    { Type t = app.GetType("Sistema_ERP.LineaVenta")!; var a = Array.CreateInstance(t, 1); a.SetValue(Activator.CreateInstance(t, [producto, cantidad, 10m]), 0); return a; }
    private static Array LineasDevolucion(int detalle, int cantidad, bool stock)
    { Type t = app.GetType("Sistema_ERP.LineaDevolucion")!; var a = Array.CreateInstance(t, 1); a.SetValue(Activator.CreateInstance(t, [detalle, cantidad, stock]), 0); return a; }
    private static int CrearApartado(int cantidad) => (int)Invocar("ApartadoDatos", "Crear", tienda, cliente, DateTime.UtcNow.AddDays(7), LineasVenta(cantidad))!;
    private static byte[] VersionApartado(int id) => (byte[])Tabla("SELECT version_fila FROM dbo.Apartado WHERE id_apartado=" + id).Rows[0][0];
    private static byte[] VersionDevolucion(int id) { using var ds = (DataSet)Invocar("DevolucionDatos", "Obtener", id)!; return (byte[])ds.Tables[0].Rows[0]["version_fila"]; }
    private static void Stock(int existencia, int reservado)
    { var r = Tabla($"SELECT existencia,reservado FROM dbo.InventarioTienda WHERE id_tienda={tienda} AND id_producto={producto}").Rows[0]; Igual(existencia, (int)r[0], "Existencia"); Igual(reservado, (int)r[1], "Reservado"); }
    private static void Igual<T>(T esperado, T actual, string contexto) { if (!EqualityComparer<T>.Default.Equals(esperado, actual)) throw new Exception($"{contexto}: esperado {esperado}; actual {actual}"); }
    private static void Fallar(Action accion) { try { accion(); } catch (SqlException) { return; } catch (InvalidOperationException) { return; } catch (ArgumentException) { return; } throw new Exception("La operación debía rechazarse."); }
    private static void Ejecutar(string sql)
    {
        using var cn = new SqlConnection(cadena); cn.Open();
        if (usuario > 0) { using var actor = new SqlCommand("EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@actor", cn); actor.Parameters.AddWithValue("@actor", usuario); actor.ExecuteNonQuery(); }
        using var cmd = new SqlCommand(sql, cn) { CommandTimeout = 90 }; cmd.ExecuteNonQuery();
    }
    private static DataTable Tabla(string sql) { using var cn = new SqlConnection(cadena); cn.Open(); using var cmd = new SqlCommand(sql, cn); using var a = new SqlDataAdapter(cmd); var t = new DataTable(); a.Fill(t); return t; }
}
