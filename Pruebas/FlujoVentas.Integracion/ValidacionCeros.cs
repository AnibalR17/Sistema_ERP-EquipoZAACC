using System.Data;
using System.Reflection;
using Sistema_ERP;
using Sistema_ERP.Clases;

internal static partial class Program
{
    private static void ProbarValidacionCeros()
    {
        int comprobaciones = 0;
        void Comprobar(bool condicion, string contexto) { Igual(true, condicion, contexto); comprobaciones++; }
        void Rechazar(Action accion, string contexto)
        {
            try { accion(); }
            catch (Exception ex)
            {
                while (ex is TargetInvocationException { InnerException: not null }) ex = ex.InnerException;
                Comprobar(ex is ArgumentException && ex.Message.Contains("solo ceros"), contexto);
                return;
            }
            throw new Exception("Se aceptaron ceros en " + contexto);
        }
        var soloCeros = app.GetType("Sistema_ERP.ValidacionDatos")!.GetMethod("SoloCeros", estaticos)!;
        foreach (string valor in new[] { "0", "000000", " 000 ", "0000-0000", "(000) 000-0000", "+00 0000", "0.00", "0,00", "０００", "٠٠٠" })
            Comprobar((bool)soloCeros.Invoke(null, [valor])!, "Detecta ceros con espacios y separadores: " + valor);
        foreach (string? valor in new string?[] { null, "", " ", "---", "000123", "A0000", "Producto 0", "(000) 000-0001", "0@ejemplo.com" })
            Comprobar(!(bool)soloCeros.Invoke(null, [valor])!, "Conserva vacíos opcionales y texto que contiene otro dato");

        foreach (string valor in new[] { "0", "0000", " 00-00 " })
        {
            Rechazar(() => new Cliente { Nombre = valor }.Validar(), "nombre del cliente");
            Rechazar(() => new Cliente { Nombre = "Cliente", Telefono = valor }.Validar(), "teléfono del cliente");
            Rechazar(() => new Cliente { Nombre = "Cliente", Correo = valor }.Validar(), "correo del cliente");
            foreach (string propiedad in new[] { "NombreEmpresa", "Rtn", "Contacto", "Telefono", "Correo", "Direccion" })
            {
                var proveedor = new Proveedor { NombreEmpresa = "Proveedor" };
                typeof(Proveedor).GetProperty(propiedad)!.SetValue(proveedor, valor);
                Rechazar(proveedor.Validar, "proveedor: " + propiedad);
            }
            foreach (TipoDirectorio tipo in Enum.GetValues<TipoDirectorio>())
            foreach (int indice in Enumerable.Range(0, 6))
            {
                string[] campos = ["Nombre", "9999-9999", "", "0001", "Dirección", "Contacto"];
                campos[indice] = valor;
                Rechazar(() => DirectorioDatos.Agregar(tipo, campos[0], campos[1], campos[2], campos[3], campos[4], campos[5], 1), "directorio: " + tipo);
                Rechazar(() => DirectorioDatos.Editar(tipo, 1, campos[0], campos[1], campos[2], campos[3], campos[4], campos[5], 1), "actualización de directorio: " + tipo);
            }
            foreach (int indice in Enumerable.Range(0, 4))
            {
                string[] campos = ["T0001", "Tienda", "Dirección", "9999-9999"];
                campos[indice] = valor;
                Rechazar(() => TiendaDatos.Agregar(campos[0], campos[1], campos[2], campos[3]), "alta de tienda");
                Rechazar(() => TiendaDatos.Editar(1, campos[0], campos[1], campos[2], campos[3]), "actualización de tienda");
            }
            foreach (bool codigo in new[] { false, true })
            {
                var producto = new Producto { IdProducto = 1, CodigoBarras = codigo ? valor : "000123", Nombre = codigo ? "Producto" : valor, PrecioUnitario = 1, IdCategoria = 1, IdProveedor = 1 };
                Rechazar(() => ProductoDatos.Agregar(producto), "alta de producto");
                Rechazar(() => ProductoDatos.Editar(producto), "actualización de producto");
                Rechazar(() => CargaMasivaDatos.ImportarProductos([new(producto.CodigoBarras, producto.Nombre, 1, 0, 1, 1)]), "persistencia de carga masiva");
            }
            Rechazar(() => CategoriaDatos.Agregar(valor), "categoría");
            Rechazar(() => CategoriaDatos.Editar(1, "Categoría", valor), "descripción de categoría");
            Rechazar(() => CompraDatos.Guardar(null, null, 1, 1, valor, DateTime.Today, "", [new(1, 1, 1)]), "número de factura");
            Rechazar(() => CompraDatos.Guardar(null, null, 1, 1, "0001", DateTime.Today, valor, [new(1, 1, 1)]), "observaciones de compra");
            Rechazar(() => Ingresar.Traspaso(1, 2, 1, valor, [new(1, 1, 1)]), "observaciones de traspaso");
            Rechazar(() => CompraDatos.Anular(1, new byte[32], valor), "motivo de anulación");
            Rechazar(() => Borrar.Producto(1, valor), "motivo de desactivación");
            Rechazar(() => DirectorioDatos.Desactivar(TipoDirectorio.Clientes, 1, valor), "motivo de directorio");
            Rechazar(() => Invocar("FlujoVentaDatos", "ReferenciaPago", false, valor), "referencia de transferencia");
            foreach (TipoSeguridad tipo in Enum.GetValues<TipoSeguridad>())
                Rechazar(() => SeguridadDatos.Guardar(tipo, null, null, valor, "Nombre", "Descripción", 1, "ClavePrueba123", []), "identificador de seguridad: " + tipo);
            Rechazar(() => SeguridadDatos.Guardar(TipoSeguridad.Roles, null, null, "ROL", valor, "", null, "", []), "nombre del rol");
            Rechazar(() => SeguridadDatos.Guardar(TipoSeguridad.Permisos, null, null, "PRODUCTOS", valor, "Descripción", null, "", []), "acción del permiso");
            Rechazar(() => SeguridadDatos.Guardar(TipoSeguridad.Roles, null, null, "ROL", "Rol", valor, null, "", []), "descripción de seguridad");
        }
        Rechazar(() => Invocar("VentaDatos", "NormalizarRtn", new string('0', 14)), "RTN de la factura");
        Rechazar(() => Invocar("ConfiguracionFacturacionDatos", "NormalizarCai", new string('0', 32)), "CAI de facturación");
        Rechazar(() => Invocar("ClaveSegura", "Crear", new string('0', 12)), "nueva contraseña");
        Rechazar(() => SeguridadDatos.Guardar(TipoSeguridad.Usuarios, null, null, "Usuario", "", "", 1, new string('0', 12), []), "contraseña en alta de usuario");
        new Cliente { Nombre = "Cliente 0", Telefono = "0001" }.Validar();
        new Proveedor { NombreEmpresa = "Proveedor 0", Rtn = "00000000000001", Telefono = "0001" }.Validar();
        new Producto { CodigoBarras = "000123", Nombre = "Producto 0", PrecioUnitario = 1, StockActual = 0, IdCategoria = 1, IdProveedor = 1 }.Validar();
        Comprobar((string)Invocar("VentaDatos", "NormalizarRtn", "00000000000001")! == "00000000000001", "El RTN conserva ceros iniciales");
        Comprobar(Invocar("VentaDatos", "NormalizarRtn", "") is null, "El RTN vacío sigue siendo opcional");
        Comprobar(Invocar("ConfiguracionFacturacionDatos", "NormalizarCai", "") is null, "El CAI vacío conserva su significado");
        Comprobar(((string)Invocar("FlujoVentaDatos", "ReferenciaPago", true, "0000")!).StartsWith("EFE-"), "El efectivo genera su propia referencia");
        ProbarCerosEnPrevisualizacion(Comprobar);
        ProbarCerosEnDialogos(Comprobar);
        Console.WriteLine($"OK: {comprobaciones} comprobaciones de ceros en altas, actualizaciones, documentos y Excel, sin SQL.");
    }

    private static void ProbarCerosEnDialogos(Action<bool, string> comprobar)
    {
        const BindingFlags instancia = BindingFlags.Instance | BindingFlags.NonPublic;
        using (var motivo = new MotivoDesactivacion())
        {
            var campo = (TextBox)typeof(MotivoDesactivacion).GetField("txtMotivo", instancia)!.GetValue(motivo)!;
            campo.Text = "0000";
            typeof(MotivoDesactivacion).GetMethod("Confirmar", instancia)!.Invoke(motivo, null);
            var mensaje = (Label)typeof(MotivoDesactivacion).GetField("error", instancia)!.GetValue(motivo)!;
            comprobar(motivo.DialogResult != DialogResult.OK && mensaje.Text.Contains("solo ceros"), "El diálogo de motivo explica el error sin aceptar los ceros");
            campo.Text = "Producto 0001 duplicado";
            typeof(MotivoDesactivacion).GetMethod("Confirmar", instancia)!.Invoke(motivo, null);
            comprobar(motivo.DialogResult == DialogResult.OK, "El motivo con texto y ceros sigue siendo válido");
        }
        using var login = new LoginERP.FrmLogin();
        object acceso = Activator.CreateInstance(app.GetType("Sistema_ERP.AccesoUsuario")!, [1, "Prueba", "Prueba", true, new byte[8], null])!;
        typeof(LoginERP.FrmLogin).GetField("pendiente", instancia)!.SetValue(login, acceso);
        foreach (string nombre in new[] { "txtClave", "txtConfirmacion" })
            ((TextBox)typeof(LoginERP.FrmLogin).GetField(nombre, instancia)!.GetValue(login)!).Text = new string('0', 12);
        typeof(LoginERP.FrmLogin).GetMethod("btnIngresar_Click", instancia)!.Invoke(login, [null, EventArgs.Empty]);
        var estado = (Label)typeof(LoginERP.FrmLogin).GetField("lblEstado", instancia)!.GetValue(login)!;
        var ingresar = (Button)typeof(LoginERP.FrmLogin).GetField("btnIngresar", instancia)!.GetValue(login)!;
        comprobar(estado.Text.Contains("solo ceros") && ingresar.Enabled && !login.IsDisposed,
            "El cambio de contraseña rechaza los ceros y permite corregirlos sin cerrar el acceso");
    }

    private static void ProbarCerosEnPrevisualizacion(Action<bool, string> comprobar)
    {
        var tipo = typeof(CargaMasiva).GetNestedType("Catalogos", BindingFlags.NonPublic)!;
        object catalogos = Activator.CreateInstance(tipo)!;
        tipo.GetProperty("Categorias")!.SetValue(catalogos, new Dictionary<string, int> { ["Papelería"] = 1, ["0000"] = 2 });
        tipo.GetProperty("Proveedores")!.SetValue(catalogos, new Dictionary<string, int> { ["00000000000001"] = 1, ["0000"] = 2 });
        using var tabla = new DataTable();
        foreach (string columna in new[] { "CodigoBarras", "Nombre", "Precio", "StockInicial", "NombreCategoria", "RTNProveedor", "Observacion" }) tabla.Columns.Add(columna);
        var fila = tabla.Rows.Add("000123", "Producto 0", "1.00", "000", "Papelería", "00000000000001", "");
        var validar = typeof(CargaMasiva).GetMethod("ValidarFila", estaticos)!;
        comprobar((bool)validar.Invoke(null, [fila, catalogos])!, "Excel conserva códigos con ceros iniciales y stock inicial cero");
        foreach (string columna in new[] { "CodigoBarras", "Nombre", "NombreCategoria", "RTNProveedor" })
        {
            ((HashSet<string>)tipo.GetProperty("CodigosArchivo")!.GetValue(catalogos)!).Clear();
            string anterior = (string)fila[columna];
            fila[columna] = "0000";
            comprobar(!(bool)validar.Invoke(null, [fila, catalogos])!, "Excel rechaza ceros en " + columna);
            comprobar(((string)fila["Observacion"]).Contains("solo ceros"), "Excel explica el error de " + columna);
            fila[columna] = anterior;
        }
    }
}
