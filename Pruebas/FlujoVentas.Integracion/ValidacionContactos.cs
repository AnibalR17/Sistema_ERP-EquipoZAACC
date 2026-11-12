using System.Data;
using System.Reflection;
using Sistema_ERP;
using Sistema_ERP.Clases;

internal static partial class Program
{
    private static void ProbarValidacionContactos()
    {
        int comprobaciones = 0;
        void Comprobar(bool condicion, string detalle) { Igual(true, condicion, detalle); comprobaciones++; }
        void Rechazar(Action accion, string mensaje, string detalle)
        {
            try { accion(); }
            catch (Exception ex)
            {
                while (ex is TargetInvocationException { InnerException: not null }) ex = ex.InnerException;
                Comprobar(ex is ArgumentException && ex.Message.Contains(mensaje, StringComparison.OrdinalIgnoreCase), detalle);
                return;
            }
            throw new Exception("Se aceptó un dato inválido: " + detalle);
        }
        var validacion = app.GetType("Sistema_ERP.ValidacionDatos")!;
        var correoValido = validacion.GetMethod("CorreoValido", estaticos)!;
        var nombreValido = validacion.GetMethod("NombreValido", estaticos)!;
        foreach (string? correo in new string?[] { null, "", " ", "ana@example.com", "nombre.apellido+ventas@sub.ejemplo.com", "ana_perez@example.co.uk", "o'connor@example.org", " usuario@dominio.com ", "persona@mañana.es", "empleado@xn--maana-pta.es", "u@a.co" })
        {
            Comprobar((bool)correoValido.Invoke(null, [correo, 254])!, "Se acepta correo válido u opcional: " + correo);
            new Cliente { Nombre = "Ana", Correo = correo }.Validar();
            new Proveedor { NombreEmpresa = "Empresa", Contacto = "José", Correo = correo ?? "" }.Validar();
        }
        foreach (string correo in new[]
        {
            "ana", "@dominio.com", "ana@", "ana@@dominio.com", "ana@dominio", "ana@.com", "ana@dominio.",
            ".ana@example.com", "ana.@example.com", "ana..perez@example.com", "ana perez@example.com",
            "Ana <ana@example.com>", "ana@example.com;otra@example.com", "ana@example.com\r\nBcc:otro@example.com",
            "ana@-example.com", "ana@example-.com", "ana@exam_ple.com", "ana@example..com", "ana@example.123",
            "ana@127.0.0.1", new string('a', 65) + "@example.com", "ana@" + new string('a', 64) + ".com"
        })
        {
            Comprobar(!(bool)correoValido.Invoke(null, [correo, 254])!, "Se rechaza el formato de correo: " + correo);
            Rechazar(() => new Cliente { Nombre = "Ana", Correo = correo }.Validar(), "correo válido", "correo del cliente");
            Rechazar(() => new Proveedor { NombreEmpresa = "Empresa", Correo = correo }.Validar(), "correo válido", "correo del proveedor");
            foreach (TipoDirectorio tipo in Enum.GetValues<TipoDirectorio>())
            {
                Rechazar(() => DirectorioDatos.Agregar(tipo, "Nombre válido", "", correo, "CODIGO", "", "", 1), "correo válido", "correo en alta de directorio");
                Rechazar(() => DirectorioDatos.Editar(tipo, 1, "Nombre válido", "", correo, "CODIGO", "", "", 1), "correo válido", "correo en edición de directorio");
            }
        }
        string correoLimite = new string('a', 64) + "@" + new string('b', 63) + "." + new string('c', 63) + "." + new string('d', 57) + ".com";
        Comprobar(correoLimite.Length == 254 && (bool)correoValido.Invoke(null, [correoLimite, 254])!, "El formato admite exactamente 254 caracteres");
        Comprobar(!(bool)correoValido.Invoke(null, ["a" + correoLimite, 254])!, "Se rechazan correos demasiado largos");
        Rechazar(() => new Cliente { Nombre = "Ana", Correo = correoLimite }.Validar(), "correo válido", "Se respeta el límite de 150 del formulario de clientes");
        foreach (string nombre in new[] { "Ana", "José", "Álex", "  Ana  ", "A B C", "María-José", "O'Neil", "USB 2.0", "ABC123" })
        {
            Comprobar((bool)nombreValido.Invoke(null, [nombre, false])!, "Se cuentan letras y acentos: " + nombre);
            new Cliente { Nombre = nombre }.Validar();
            new Proveedor { NombreEmpresa = nombre, Contacto = nombre }.Validar();
            new Producto { CodigoBarras = "0001", Nombre = nombre }.Validar();
        }
        foreach (string? nombre in new string?[] { null, "", " ", "AB", "A12", "12A", "123", "A - B", "A..." })
            Comprobar(!(bool)nombreValido.Invoke(null, [nombre, false])!, "Se rechazan nombres sin tres letras: " + nombre);
        Comprobar((bool)nombreValido.Invoke(null, ["", true])!, "El contacto opcional vacío sigue permitido");
        foreach (string nombre in new[] { "AB", "A12", "A - B" })
        {
            Rechazar(() => new Cliente { Nombre = nombre }.Validar(), "3 letras", "nombre del cliente");
            Rechazar(() => new Proveedor { NombreEmpresa = nombre }.Validar(), "3 letras", "nombre de empresa");
            Rechazar(() => new Proveedor { NombreEmpresa = "Empresa", Contacto = nombre }.Validar(), "3 letras", "nombre del contacto");
            var producto = new Producto { IdProducto = 1, CodigoBarras = "0001", Nombre = nombre, PrecioUnitario = 1, IdCategoria = 1, IdProveedor = 1 };
            Rechazar(() => ProductoDatos.Agregar(producto), "3 letras", "alta de producto");
            Rechazar(() => ProductoDatos.Editar(producto), "3 letras", "edición de producto");
            Rechazar(() => CargaMasivaDatos.ImportarProductos([new("0001", nombre, 1, 0, 1, 1)]), "3 letras", "persistencia de importación");
            Rechazar(() => CategoriaDatos.Agregar(nombre), "3 letras", "alta de categoría");
            Rechazar(() => CategoriaDatos.Editar(1, nombre, ""), "3 letras", "edición de categoría");
            Rechazar(() => TiendaDatos.Agregar("CODIGO", nombre, "", ""), "3 letras", "alta de tienda");
            Rechazar(() => TiendaDatos.Editar(1, "CODIGO", nombre, "", ""), "3 letras", "edición de tienda");
            Rechazar(() => SeguridadDatos.Guardar(TipoSeguridad.Roles, null, null, "ROL", nombre, "", null, "", []), "3 letras", "nombre del rol");
            Rechazar(() => SeguridadDatos.Guardar(TipoSeguridad.Usuarios, null, null, nombre, "", "", 1, "ClavePrueba123", []), "3 letras", "nombre de usuario");
            foreach (TipoDirectorio tipo in Enum.GetValues<TipoDirectorio>())
            {
                Rechazar(() => DirectorioDatos.Agregar(tipo, nombre, "", "", "CODIGO", "", "", 1), "3 letras", "alta de nombre en directorio");
                Rechazar(() => DirectorioDatos.Editar(tipo, 1, nombre, "", "", "CODIGO", "", "", 1), "3 letras", "edición de nombre en directorio");
            }
        }
        ProbarNombresEnExcel(Comprobar);
        Console.WriteLine($"OK: {comprobaciones} comprobaciones de correos, nombres, altas, ediciones y Excel, sin SQL.");
    }

    private static void ProbarNombresEnExcel(Action<bool, string> comprobar)
    {
        var tipo = typeof(CargaMasiva).GetNestedType("Catalogos", BindingFlags.NonPublic)!;
        object catalogos = Activator.CreateInstance(tipo)!;
        tipo.GetProperty("Categorias")!.SetValue(catalogos, new Dictionary<string, int> { ["Papelería"] = 1, ["AB"] = 2 });
        tipo.GetProperty("Proveedores")!.SetValue(catalogos, new Dictionary<string, int> { ["08019999999999"] = 1 });
        using var tabla = new DataTable();
        foreach (string columna in new[] { "CodigoBarras", "Nombre", "Precio", "StockInicial", "NombreCategoria", "RTNProveedor", "Observacion" }) tabla.Columns.Add(columna);
        var fila = tabla.Rows.Add("000123", "USB", "1.00", "0", "Papelería", "08019999999999", "");
        var validar = typeof(CargaMasiva).GetMethod("ValidarFila", estaticos)!;
        comprobar((bool)validar.Invoke(null, [fila, catalogos])!, "Excel admite nombres de exactamente tres letras");
        foreach (string columna in new[] { "Nombre", "NombreCategoria" })
        {
            ((HashSet<string>)tipo.GetProperty("CodigosArchivo")!.GetValue(catalogos)!).Clear();
            string anterior = (string)fila[columna];
            fila[columna] = "AB";
            comprobar(!(bool)validar.Invoke(null, [fila, catalogos])! && ((string)fila["Observacion"]).Contains("3 letras"), "Excel indica el nombre que debe corregirse: " + columna);
            fila[columna] = anterior;
        }
    }
}
