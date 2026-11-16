using System.Data;
using System.Reflection;
using System.Security.Cryptography;
using Sistema_ERP;
using Sistema_ERP.Clases;

internal static partial class Program
{
    private static void ProbarValidacionesAcciones()
    {
        int comprobaciones = 0;
        void Comprobar(bool condicion, string detalle) { Igual(true, condicion, detalle); comprobaciones++; }
        void Rechazar(string tipo, string metodo, params object?[] argumentos)
        {
            Fallar(() => Invocar(tipo, metodo, argumentos)); comprobaciones++;
        }
        var seleccionProducto = ((int, int))Invocar("GuardarProductoValidacion", "Captura", "000123", "Producto válido", 10m, 3, 5)!;
        Comprobar(seleccionProducto == (3, 5), "La acción de guardar producto devuelve las referencias existentes");
        try { Invocar("GuardarProductoValidacion", "Captura", "000123", "Producto válido", 10m, null, 5); throw new Exception("Faltó validar categoría."); }
        catch (ArgumentException ex)
        {
            Comprobar((string?)ex.GetType().GetProperty("Campo", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(ex) == "IdCategoria",
                "La validación identifica el campo sin depender de controles visuales");
        }
        Rechazar("GuardarProductoValidacion", "CodigoDisponible", true);
        Rechazar("GuardarProveedorValidacion", "RtnDisponible", true);
        Rechazar("GuardarCategoriaValidacion", "NombreDisponible", true);
        Rechazar("GuardarCompraValidacion", "Captura", true, 1, 1);
        Rechazar("GuardarCompraValidacion", "Version", 0, new byte[32]);
        Rechazar("GuardarCompraValidacion", "Validar", 1, 1, "FAC-1", "", new LineaCompra[] { new(1, 1, 1m), new(1, 2, 2m) });
        Rechazar("GuardarCompraValidacion", "Validar", 1, 1, "FAC-1", "", new LineaCompra[] { new(1, 2, 9999999999999999.99m) });
        Rechazar("AnularCompraValidacion", "Motivo", "0000");
        Rechazar("RegistrarAbonoApartadoValidacion", "Validar", 1, 0m, 10m, true, "");
        Rechazar("RegistrarAbonoApartadoValidacion", "Validar", 1, 5m, 10m, false, "");
        Rechazar("EntregarApartadoValidacion", "Captura", 1, 10m, 10m, true, "", 9m);
        Invocar("EntregarApartadoValidacion", "Captura", 0, 0m, 0m, false, "", 0m);
        Comprobar(true, "Una entrega completamente abonada no exige un cobro adicional");
        Rechazar("GuardarTraspasoValidacion", "Validar", 1, 1, "", new LineaTraspaso[] { new(1, 1, 1m) });
        Rechazar("ResolverTraspasoValidacion", "Transicion", "RECIBIDO", "EN_TRANSITO");
        Rechazar("GuardarUsuarioValidacion", "Validar", null, "Usuario", null, "ClavePrueba123");
        Comprobar(!(bool)Invocar("GuardarUsuarioValidacion", "Validar", 1, "Usuario", 1, "")!, "Editar usuario conserva su contraseña cuando se deja vacía");
        Rechazar("GuardarRolValidacion", "Validar", "ROL", "AB", "");
        Rechazar("GuardarPermisoValidacion", "Validar", "VENTAS", "CONSULTAR", "");
        Rechazar("CambiarClaveValidacion", "Confirmacion", "ClavePrueba123", "OtraClave123");
        Rechazar("ConsultarHistorialValidacion", "Fechas", new DateTime(2026, 2, 2), new DateTime(2026, 2, 1));
        Rechazar("GuardarMinimoInventarioValidacion", "Validar", 1, 1, -1);
        Rechazar("AgregarProductoVentaValidacion", "Cantidad", 1, int.MaxValue, int.MaxValue);
        Rechazar("GuardarDevolucionValidacion", "EdicionTerminada", false);
        Rechazar("GuardarMinimoInventarioValidacion", "VersionActual", new byte[] { 1 }, new byte[] { 2 });
        Invocar("GuardarMinimoInventarioValidacion", "VersionActual", null, null);
        Comprobar(true, "Un inventario que aún no existe conserva la versión vacía para su creación");
        Rechazar("ConsultarCompraValidacion", "VersionActual", true, new byte[] { 1 }, new byte[] { 2 });
        Rechazar("ConfirmarCompraValidacion", "Estado", "CONFIRMADA");
        Rechazar("ConfirmarDevolucionValidacion", "Estado", "ANULADA");
        Rechazar("ReembolsarDevolucionValidacion", "Pendiente", 0m);
        Rechazar("ReembolsarApartadoValidacion", "Estado", "ACTIVO");
        Rechazar("GuardarRegistroSeguridadValidacion", "Resultado", 1, 0);
        Comprobar((int)Invocar("SeleccionarOpcionValidacion", "Validar", "Ana", new[] { "Ana", "Ana" }, 1)! == 1,
            "Una selección con nombres repetidos mantiene su identidad");
        Rechazar("SeleccionarOpcionValidacion", "Validar", "No existe", new[] { "Ana" }, 0);

        using var productos = new DataTable(); productos.Columns.Add("id_producto", typeof(int)); productos.Columns.Add("nombre"); productos.Columns.Add("codigo_barras");
        productos.Rows.Add(1, "Producto", "000123");
        using var carrito = new DataTable(); carrito.Columns.Add("id_producto", typeof(int)); carrito.Columns.Add("Subtotal", typeof(decimal)); carrito.Rows.Add(1, 10m);
        Rechazar("AgregarLineaCompraValidacion", "Validar", productos.DefaultView[0], "1", "10", carrito, null, false);
        var linea = ((DataRowView Producto, int Cantidad, decimal Costo))Invocar("AgregarLineaCompraValidacion", "Validar", productos.DefaultView[0], "2", "12,50", carrito, carrito.Rows[0], true)!;
        Comprobar(linea.Cantidad == 2 && linea.Costo == 12.50m, "Actualizar una línea admite su propio producto y conserva el importe interpretado");

        Type permiso = app.GetType("Sistema_ERP.PermisoAsignable")!;
        object Opcion(int id, string accion, bool activo = true) => Activator.CreateInstance(permiso, [id, "VENTAS", accion, "Descripción", activo])!;
        object consulta = Opcion(1, "CONSULTAR"), gestion = Opcion(2, "GESTIONAR"), inactivo = Opcion(3, "BORRAR", false);
        var opciones = Array.CreateInstance(permiso, 3); opciones.SetValue(consulta, 0); opciones.SetValue(gestion, 1); opciones.SetValue(inactivo, 2);
        var elegidos = new HashSet<int>(); var iniciales = new HashSet<int>();
        Rechazar("AsignarPermisoRolValidacion", "Cambiar", inactivo, true, opciones, elegidos, iniciales);
        Comprobar(elegidos.Count == 0, "Rechazar un permiso inactivo conserva la selección");
        Invocar("AsignarPermisoRolValidacion", "Cambiar", gestion, true, opciones, elegidos, iniciales);
        Comprobar(elegidos.SetEquals([1, 2]), "Gestionar incluye la consulta activa del mismo módulo");
        Invocar("AsignarPermisoRolValidacion", "Cambiar", consulta, false, opciones, elegidos, iniciales);
        Comprobar(elegidos.Count == 0, "Retirar consulta también retira gestionar");

        string ruta = Path.Combine(Path.GetTempPath(), "ERP-validacion-accion-" + Guid.NewGuid().ToString("N") + ".xlsx");
        try
        {
            File.WriteAllText(ruta, "contenido inicial"); byte[] hash = SHA256.HashData(File.ReadAllBytes(ruta));
            var lote = new ProductoImportado[] { new("000123", "Producto", 1m, 1, 1, 1) };
            Invocar("ImportarProductosValidacion", "AntesDeImportar", lote, 1, ruta, hash);
            File.WriteAllText(ruta, "contenido cambiado");
            Rechazar("ImportarProductosValidacion", "AntesDeImportar", lote, 1, ruta, hash);
            Rechazar("ImportarProductosValidacion", "AntesDeImportar", lote, null, ruta, hash);
        }
        finally { File.Delete(ruta); }
        var tipos = app.GetTypes().Where(t => t.Name.EndsWith("Validacion", StringComparison.Ordinal)).ToArray();
        Comprobar(tipos.Length > 0 && tipos.All(t => !typeof(Control).IsAssignableFrom(t)), "Las clases de validación se pueden usar sin instanciar formularios");
        Comprobar(tipos.SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .All(c => !typeof(Control).IsAssignableFrom(c.FieldType)), "Las validaciones no almacenan controles visuales");
        Console.WriteLine($"OK: {comprobaciones} comprobaciones directas de acciones, referencias, pagos, permisos e importación, sin formularios ni SQL.");
    }
}
