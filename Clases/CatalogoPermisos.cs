namespace Sistema_ERP;

internal static class CatalogoPermisos
{
    internal sealed record Pantalla(string Modulo, string Nombre, string Consulta, string Gestion);

    internal static IReadOnlyList<Pantalla> Pantallas { get; } = Array.AsReadOnly<Pantalla>([
        new(Modulos.ventas, "Punto de venta", "Productos, clientes y comprobantes de venta.", "Registrar y confirmar ventas y cobros."),
        new(Modulos.inventario, "Inventario, movimientos y alertas", "Existencias, movimientos y notificaciones de stock.", "Registrar ajustes, entradas y salidas de inventario."),
        new(Modulos.compras, "Compras", "Compras y sus detalles.", "Registrar y confirmar compras."),
        new(Modulos.proveedores, "Proveedores", "Directorio de proveedores.", "Crear, editar, desactivar y reactivar proveedores."),
        new(Modulos.traspasos, "Traspasos entre tiendas", "Solicitudes, envíos y recepciones.", "Solicitar, editar, cancelar, enviar y recibir traspasos. La aprobación se elige en Validación."),
        new(Modulos.validacion, "Validación de traspasos", "Traspasos pendientes y su estado.", "Aprobar y rechazar traspasos."),
        new(Modulos.cargaMasiva, "Carga masiva de productos", "Plantillas, catálogos e historial de importaciones.", "Importar archivos de productos."),
        new(Modulos.productos, "Catálogo de productos y categorías", "Consultar y exportar el catálogo y sus categorías.", "Crear, editar, desactivar y reactivar productos y categorías."),
        new(Modulos.apartados, "Apartados y devoluciones", "Apartados, abonos y devoluciones.", "Crear, abonar, entregar y cancelar apartados; registrar devoluciones y reembolsos."),
        new(Modulos.directorio, "Directorio: clientes, proveedores y empleados", "Clientes, proveedores y empleados del directorio.", "Crear, editar, desactivar y reactivar los registros del directorio."),
        new(Modulos.seguridad, "Usuarios, roles y permisos", "Cuentas, roles y permisos.", "Crear y modificar cuentas, roles, permisos y sus asignaciones."),
        new(Modulos.configuracion, "Configuración, facturación y auditoría", "Tiendas, configuración fiscal y bitácora del sistema.", "Gestionar tiendas y configuración de facturación.")
    ]);

    internal static Pantalla? Buscar(string modulo) => Pantallas.FirstOrDefault(p =>
        string.Equals(p.Modulo, modulo, StringComparison.OrdinalIgnoreCase));

    internal static string NombrePantalla(string modulo) => Buscar(modulo)?.Nombre ?? modulo;
}

internal sealed class PlantillaPermisos(string codigo, string nombre, string descripcion,
    IEnumerable<string> consultas, IEnumerable<string> gestiones)
{
    private readonly HashSet<string> lectura = new(consultas, StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> escritura = new(gestiones, StringComparer.OrdinalIgnoreCase);

    internal string Codigo { get; } = codigo;
    internal string Nombre { get; } = nombre;
    internal string NombreRol => Codigo == "ADMIN_PERSONALIZADO" ? "Administrador personalizado" : Nombre;
    internal string Descripcion { get; } = descripcion;

    internal bool Incluye(string modulo, string accion) =>
        string.Equals(accion, "CONSULTAR", StringComparison.OrdinalIgnoreCase) ? lectura.Contains(modulo) :
        string.Equals(accion, "GESTIONAR", StringComparison.OrdinalIgnoreCase) && escritura.Contains(modulo);

    internal IEnumerable<(string Modulo, string Accion)> Permisos =>
        lectura.Select(m => (m, "CONSULTAR")).Concat(escritura.Select(m => (m, "GESTIONAR")));

    public override string ToString() => Nombre;

    internal static IReadOnlyList<PlantillaPermisos> Todas { get; } = Crear();

    private static IReadOnlyList<PlantillaPermisos> Crear()
    {
        string[] todos = CatalogoPermisos.Pantallas.Select(p => p.Modulo).ToArray();
        string[] operativos = todos.Where(m => m != Modulos.seguridad && m != Modulos.configuracion).ToArray();
        return Array.AsReadOnly<PlantillaPermisos>([
            new("ADMIN_PERSONALIZADO", "Administrador",
                "Consulta y gestión de todas las pantallas. Puedes ajustar esta selección para crear un administrador personalizado.", todos, todos),
            new("CAJERO", "Cajero",
                "Gestiona ventas, apartados y devoluciones. Consulta productos e inventario. Puedes ajustar cada pantalla.",
                [Modulos.ventas, Modulos.apartados, Modulos.productos, Modulos.inventario], [Modulos.ventas, Modulos.apartados]),
            new("GERENTE", "Gerente",
                "Consulta y gestión de los módulos operativos, incluida la validación de traspasos y el directorio. Sin seguridad ni configuración.",
                operativos, operativos),
            new("ANALISTA_DATOS", "Analista de datos",
                "Solo consulta: ventas, compras, inventario, productos, proveedores, traspasos, validación, apartados y devoluciones. Incluye las exportaciones disponibles.",
                [Modulos.ventas, Modulos.compras, Modulos.inventario, Modulos.productos, Modulos.proveedores,
                    Modulos.traspasos, Modulos.validacion, Modulos.apartados], [])
        ]);
    }
}
