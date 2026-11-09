using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;

namespace Sistema_ERP;

internal sealed record ResultadoBusqueda(string Tipo, int Id, string Nombre, string Detalle, string Pantalla, int? Tienda);
internal sealed record DestinoBusqueda(string Nombre, string Pantalla, string Palabras);

internal static class BusquedaGlobalDatos
{
    internal const int LimitePorTipo = 10;
    internal static readonly DestinoBusqueda[] Destinos = [
        new("Inicio", "Inicio", "resumen dashboard"),
        new("Punto de venta", "Punto de Venta", "ventas facturas facturación pagos recibos"),
        new("Inventario y movimientos", "Inventario y movimientos", "stock existencias mínimos notificaciones alertas"),
        new("Compras", "Compras", "facturas proveedores pedidos"),
        new("Proveedores", "Proveedores", "empresas RTN contactos"),
        new("Traspasos", "Traspasos", "solicitudes envíos recepciones transferencias tiendas"),
        new("Validación de traspasos", "Validacion", "aprobar rechazar solicitudes"),
        new("Carga masiva", "Carga masiva", "importar Excel plantilla productos"),
        new("Catálogo de productos", "Catalogo", "categorías precios códigos barras"),
        new("Apartados y devoluciones", "Apartados y devoluciones", "reservas abonos entregas reembolsos"),
        new("Directorio", "Directorio y roles", "clientes proveedores empleados contactos"),
        new("Usuarios, roles y permisos", "Usuarios, roles y permisos", "seguridad acceso cuentas"),
        new("Configuración y auditoría", "Configuración y auditoría", "tiendas sucursales facturación CAI bitácora configuración")
    ];

    private sealed record Fuente(string Tipo, string Pantalla, string Desde, string Id, string Nombre,
        string Resumen, string Texto, string DetalleSql, string FiltroTienda = "1=1", string IdTienda = "@tienda");

    internal static bool PuedeVer(PermisosUsuario permisos, string pantalla) =>
        AutorizacionDatos.ModuloPantalla(pantalla) is not string modulo || permisos.Puede(modulo);

    internal static IReadOnlyList<ResultadoBusqueda> Buscar(string texto, int? tienda = null, CancellationToken cancelacion = default)
    {
        texto = texto.Trim();
        if (texto.Length > 150) throw new ArgumentException("La búsqueda admite hasta 150 caracteres.");
        if (tienda is <= 0) throw new ArgumentException("Selecciona una tienda válida.");
        cancelacion.ThrowIfCancellationRequested();
        using var conexion = Conexion.AbrirConexion();
        var permisos = AutorizacionDatos.Obtener(conexion);
        if (texto.Length < 2) return [];
        var resultados = Destinos.Where(d => PuedeVer(permisos, d.Pantalla) &&
            CultureInfo.InvariantCulture.CompareInfo.IndexOf(d.Nombre + " " + d.Palabras, texto,
                CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0)
            .Select(d => new ResultadoBusqueda("Módulo", 0, d.Nombre, "Abrir módulo", d.Pantalla, tienda)).ToList();
        var fuentes = Fuentes(permisos);
        if (fuentes.Count > 0)
        {
            string sql = string.Join("\n", fuentes.Select(f => $"""
                SELECT TOP(@limite) {f.Id}, {f.Nombre}, {f.Resumen}, {f.IdTienda}
                FROM {f.Desde}
                WHERE ({f.FiltroTienda}) AND CHARINDEX(@buscar COLLATE Latin1_General_100_CI_AI,
                    CONVERT(nvarchar(max),{f.Texto}) COLLATE Latin1_General_100_CI_AI)>0
                ORDER BY {f.Nombre},{f.Id};
                """));
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@buscar", SqlDbType.NVarChar, 150).Value = texto;
            comando.Parameters.Add("@limite", SqlDbType.Int).Value = LimitePorTipo;
            comando.Parameters.Add("@tienda", SqlDbType.Int).Value = (object?)tienda ?? DBNull.Value;
            using var registro = cancelacion.Register(comando.Cancel);
            try
            {
                using var lector = comando.ExecuteReader();
                foreach (var fuente in fuentes)
                {
                    while (lector.Read())
                    {
                        cancelacion.ThrowIfCancellationRequested();
                        resultados.Add(new(fuente.Tipo, lector.GetInt32(0), lector.GetString(1), lector.GetString(2),
                            fuente.Pantalla, lector.IsDBNull(3) ? null : lector.GetInt32(3)));
                    }
                    lector.NextResult();
                }
            }
            catch (SqlException) when (cancelacion.IsCancellationRequested) { throw new OperationCanceledException(cancelacion); }
        }
        cancelacion.ThrowIfCancellationRequested();
        var actuales = AutorizacionDatos.Obtener(conexion);
        return resultados.Where(r => PuedeVer(actuales, r.Pantalla)).ToArray();
    }

    internal static DataSet ObtenerDetalle(ResultadoBusqueda resultado)
    {
        if (resultado.Id <= 0) throw new ArgumentException("Selecciona un registro para ver su detalle.");
        using var conexion = Conexion.AbrirConexion();
        var fuente = Fuentes(AutorizacionDatos.Obtener(conexion)).SingleOrDefault(f => f.Tipo == resultado.Tipo)
            ?? throw new UnauthorizedAccessException("No tienes permiso para consultar este resultado.");
        DataSet datos;
        if (fuente.Tipo == "Compra") datos = CompraDatos.Obtener(resultado.Id);
        else if (fuente.Tipo == "Apartado") datos = ApartadoDatos.Obtener(resultado.Id);
        else if (fuente.Tipo == "Devolución") datos = DevolucionDatos.Obtener(resultado.Id);
        else
        {
            using var comando = new SqlCommand(fuente.DetalleSql, conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = resultado.Id;
            comando.Parameters.Add("@tienda", SqlDbType.Int).Value = (object?)resultado.Tienda ?? DBNull.Value;
            using var adaptador = new SqlDataAdapter(comando);
            datos = new DataSet(); adaptador.Fill(datos);
        }
        if (fuente.Tipo == "Traspaso")
        {
            datos.Tables.Add(TraspasoDatos.Detalle(resultado.Id));
            datos.Tables.Add(TraspasoDatos.Historial(resultado.Id));
        }
        if (datos.Tables.Count == 0 || datos.Tables[0].Rows.Count == 0)
        { datos.Dispose(); throw new InvalidOperationException("El registro ya no está disponible. Vuelve a buscar."); }
        if (!PuedeVer(AutorizacionDatos.Obtener(conexion), fuente.Pantalla))
        { datos.Dispose(); throw new UnauthorizedAccessException("El permiso para consultar este contenido fue retirado."); }
        return datos;
    }

    private static List<Fuente> Fuentes(PermisosUsuario permisos)
    {
        var fuentes = new List<Fuente>();
        void Agregar(Fuente fuente) { if (PuedeVer(permisos, fuente.Pantalla)) fuentes.Add(fuente); }
        Agregar(new("Producto", "Catalogo", "dbo.Producto p JOIN dbo.Categoria c ON c.id_categoria=p.id_categoria", "p.id_producto", "p.nombre",
            "CONCAT(p.codigo_barras,N' · ',c.nombre,CASE WHEN p.activo=0 THEN N' · Inactivo' ELSE N'' END)", "CONCAT(p.codigo_barras,N' ',p.nombre,N' ',c.nombre)",
            """
            SELECT p.codigo_barras AS Código,p.nombre AS Producto,c.nombre AS Categoría,pr.nombre_empresa AS Proveedor,
                p.precio_unitario AS Precio,p.activo AS Activo,p.motivo_desactivacion AS [Motivo de desactivación],
                CASE WHEN @tienda IS NULL THEN p.stock_actual ELSE ISNULL(i.existencia,0) END AS Existencia
            FROM dbo.vw_ProductoStock p JOIN dbo.Categoria c ON c.id_categoria=p.id_categoria
            JOIN dbo.Proveedor pr ON pr.id_proveedor=p.id_proveedor
            LEFT JOIN dbo.InventarioTienda i ON i.id_producto=p.id_producto AND i.id_tienda=@tienda WHERE p.id_producto=@id
            """));
        Agregar(new("Categoría", "Catalogo", "dbo.Categoria c", "c.id_categoria", "c.nombre",
            "CONCAT(c.descripcion,CASE WHEN c.activo=0 THEN N' · Inactiva' ELSE N'' END)", "CONCAT(c.nombre,N' ',c.descripcion)",
            "SELECT nombre AS Categoría,descripcion AS Descripción,activo AS Activa FROM dbo.Categoria WHERE id_categoria=@id"));
        string proveedores = permisos.Puede(Modulos.Proveedores) ? "Proveedores" : "Directorio y roles";
        Agregar(new("Proveedor", proveedores, "dbo.Proveedor p", "p.id_proveedor", "p.nombre_empresa",
            "CONCAT(p.rtn,N' · ',p.contacto,CASE WHEN p.activo=0 THEN N' · Inactivo' ELSE N'' END)",
            "CONCAT(p.nombre_empresa,N' ',p.rtn,N' ',p.contacto,N' ',p.telefono,N' ',p.correo)",
            "SELECT nombre_empresa AS Proveedor,rtn AS RTN,contacto AS Contacto,telefono AS Teléfono,correo AS Correo,direccion AS Dirección,activo AS Activo FROM dbo.Proveedor WHERE id_proveedor=@id"));
        Agregar(new("Cliente", "Directorio y roles", "dbo.Cliente c", "c.id_cliente", "c.nombre",
            "CONCAT(c.documento_fiscal,N' · ',c.telefono,CASE WHEN c.activo=0 THEN N' · Inactivo' ELSE N'' END)",
            "CONCAT(c.nombre,N' ',c.documento_fiscal,N' ',c.telefono,N' ',c.correo)",
            "SELECT nombre AS Cliente,documento_fiscal AS Documento,telefono AS Teléfono,correo AS Correo,direccion AS Dirección,activo AS Activo FROM dbo.Cliente WHERE id_cliente=@id"));
        Agregar(new("Empleado", "Directorio y roles", "dbo.Empleado e JOIN dbo.Tienda t ON t.id_tienda=e.id_tienda", "e.id_empleado", "e.nombre",
            "CONCAT(e.codigo,N' · ',t.nombre,CASE WHEN e.activo=0 THEN N' · Inactivo' ELSE N'' END)",
            "CONCAT(e.nombre,N' ',e.codigo,N' ',e.telefono,N' ',e.correo,N' ',t.nombre)",
            "SELECT e.codigo AS Código,e.nombre AS Empleado,e.telefono AS Teléfono,e.correo AS Correo,t.nombre AS Tienda,e.activo AS Activo FROM dbo.Empleado e JOIN dbo.Tienda t ON t.id_tienda=e.id_tienda WHERE e.id_empleado=@id",
            "@tienda IS NULL OR e.id_tienda=@tienda", "e.id_tienda"));
        Agregar(new("Tienda", "Configuración y auditoría", "dbo.Tienda t", "t.id_tienda", "t.nombre",
            "CONCAT(t.codigo,N' · ',t.direccion,CASE WHEN t.activo=0 THEN N' · Inactiva' ELSE N'' END)",
            "CONCAT(t.codigo,N' ',t.nombre,N' ',t.direccion,N' ',t.telefono)",
            "SELECT codigo AS Código,nombre AS Tienda,direccion AS Dirección,telefono AS Teléfono,activo AS Activa FROM dbo.Tienda WHERE id_tienda=@id",
            "@tienda IS NULL OR t.id_tienda=@tienda", "t.id_tienda"));
        Agregar(new("Compra", "Compras", "dbo.Compra c JOIN dbo.Proveedor p ON p.id_proveedor=c.id_proveedor JOIN dbo.Tienda t ON t.id_tienda=c.id_tienda",
            "c.id_compra", "c.numero_factura", "CONCAT(p.nombre_empresa,N' · ',t.nombre,N' · ',c.estado)",
            "CONCAT(c.id_compra,N' ',c.numero_factura,N' ',p.nombre_empresa,N' ',t.nombre,N' ',c.observaciones)", "",
            "@tienda IS NULL OR c.id_tienda=@tienda", "c.id_tienda"));
        Agregar(new("Venta", "Punto de Venta", "dbo.Venta v JOIN dbo.Cliente c ON c.id_cliente=v.id_cliente JOIN dbo.Tienda t ON t.id_tienda=v.id_tienda",
            "v.id_venta", "v.numero", "CONCAT(c.nombre,N' · ',t.nombre,N' · ',v.estado)",
            "CONCAT(v.id_venta,N' ',v.numero,N' ',c.nombre,N' ',v.rtn_cliente,N' ',t.nombre,N' ',v.observaciones)",
            """
            SELECT v.numero AS Factura,v.fecha AS [Fecha UTC],c.nombre AS Cliente,v.rtn_cliente AS RTN,t.nombre AS Tienda,
                v.estado AS Estado,v.cai AS CAI,total.total AS Total,v.observaciones AS Observaciones
            FROM dbo.Venta v JOIN dbo.Cliente c ON c.id_cliente=v.id_cliente JOIN dbo.Tienda t ON t.id_tienda=v.id_tienda
            JOIN dbo.vw_VentaTotal total ON total.id_venta=v.id_venta WHERE v.id_venta=@id;
            SELECT nombre_producto AS Producto,cantidad AS Cantidad,precio_unitario AS Precio,subtotal AS Subtotal,tasa_isv AS [ISV %],isv AS ISV,total_linea AS Total
                FROM dbo.DetalleVenta WHERE id_venta=@id ORDER BY id_detalle_venta;
            SELECT m.nombre AS Método,p.monto AS Monto,p.monto_recibido AS Recibido,p.referencia AS Referencia
                FROM dbo.PagoVenta p JOIN dbo.MetodoPago m ON m.id_metodo_pago=p.id_metodo_pago WHERE p.id_venta=@id;
            """, "@tienda IS NULL OR v.id_tienda=@tienda", "v.id_tienda"));
        Agregar(new("Apartado", "Apartados y devoluciones", "dbo.Apartado a JOIN dbo.Cliente c ON c.id_cliente=a.id_cliente JOIN dbo.Tienda t ON t.id_tienda=a.id_tienda",
            "a.id_apartado", "CONCAT(N'Apartado #',a.id_apartado)", "CONCAT(c.nombre,N' · ',t.nombre,N' · ',a.estado)",
            "CONCAT(N'Apartado #',a.id_apartado,N' ',c.nombre,N' ',t.nombre)", "", "@tienda IS NULL OR a.id_tienda=@tienda", "a.id_tienda"));
        Agregar(new("Devolución", "Apartados y devoluciones", "dbo.Devolucion d JOIN dbo.Venta v ON v.id_venta=d.id_venta JOIN dbo.Cliente c ON c.id_cliente=v.id_cliente JOIN dbo.Tienda t ON t.id_tienda=d.id_tienda",
            "d.id_devolucion", "CONCAT(N'Devolución #',d.id_devolucion)", "CONCAT(v.numero,N' · ',c.nombre,N' · ',t.nombre,N' · ',d.estado)",
            "CONCAT(N'Devolución #',d.id_devolucion,N' ',v.numero,N' ',c.nombre,N' ',t.nombre,N' ',d.motivo)", "", "@tienda IS NULL OR d.id_tienda=@tienda", "d.id_tienda"));
        string traspasos = permisos.Puede(Modulos.Validacion) ? "Validacion" : "Traspasos";
        Agregar(new("Traspaso", traspasos, "dbo.Traspaso t JOIN dbo.Tienda o ON o.id_tienda=t.id_tienda_origen JOIN dbo.Tienda d ON d.id_tienda=t.id_tienda_destino",
            "t.id_traspaso", "CONCAT(N'Traspaso #',t.id_traspaso)", "CONCAT(o.nombre,N' → ',d.nombre,N' · ',t.estado)",
            "CONCAT(N'Traspaso #',t.id_traspaso,N' ',o.nombre,N' ',d.nombre,N' ',t.observaciones)",
            "SELECT t.id_traspaso AS Número,o.nombre AS Origen,d.nombre AS Destino,t.estado AS Estado,t.solicitado_en AS [Fecha UTC],t.observaciones AS Observaciones FROM dbo.Traspaso t JOIN dbo.Tienda o ON o.id_tienda=t.id_tienda_origen JOIN dbo.Tienda d ON d.id_tienda=t.id_tienda_destino WHERE t.id_traspaso=@id",
            "@tienda IS NULL OR t.id_tienda_origen=@tienda OR t.id_tienda_destino=@tienda"));
        Agregar(new("Existencia", "Inventario y movimientos", "dbo.InventarioTienda i JOIN dbo.Producto p ON p.id_producto=i.id_producto JOIN dbo.Tienda t ON t.id_tienda=i.id_tienda",
            "i.id_producto", "p.nombre", "CONCAT(p.codigo_barras,N' · ',t.nombre,N' · Disponible: ',i.existencia-i.reservado)",
            "CONCAT(p.codigo_barras,N' ',p.nombre,N' ',t.nombre)",
            "SELECT p.codigo_barras AS Código,p.nombre AS Producto,t.nombre AS Tienda,i.existencia AS Existencia,i.reservado AS Reservado,i.existencia-i.reservado AS Disponible,i.stock_minimo AS Mínimo FROM dbo.InventarioTienda i JOIN dbo.Producto p ON p.id_producto=i.id_producto JOIN dbo.Tienda t ON t.id_tienda=i.id_tienda WHERE i.id_producto=@id AND i.id_tienda=@tienda",
            "@tienda IS NULL OR i.id_tienda=@tienda", "i.id_tienda"));
        Agregar(new("Usuario", "Usuarios, roles y permisos", "dbo.Usuario u JOIN dbo.Empleado e ON e.id_empleado=u.id_empleado", "u.id_usuario", "u.nombre_usuario",
            "CONCAT(e.nombre,CASE WHEN u.activo=0 THEN N' · Inactivo' ELSE N'' END)", "CONCAT(u.nombre_usuario,N' ',e.nombre)",
            "SELECT u.nombre_usuario AS Usuario,e.nombre AS Empleado,u.activo AS Activo,u.debe_cambiar_contrasena AS [Cambio de contraseña pendiente] FROM dbo.Usuario u JOIN dbo.Empleado e ON e.id_empleado=u.id_empleado WHERE u.id_usuario=@id"));
        Agregar(new("Rol", "Usuarios, roles y permisos", "dbo.Rol r", "r.id_rol", "r.nombre", "CONCAT(r.codigo,N' · ',r.descripcion)",
            "CONCAT(r.codigo,N' ',r.nombre,N' ',r.descripcion)", "SELECT codigo AS Código,nombre AS Rol,descripcion AS Descripción,activo AS Activo FROM dbo.Rol WHERE id_rol=@id"));
        Agregar(new("Permiso", "Usuarios, roles y permisos", "dbo.Permiso p", "p.id_permiso", "p.codigo", "CONCAT(p.descripcion,CASE WHEN p.activo=0 THEN N' · Inactivo' ELSE N'' END)",
            "CONCAT(p.codigo,N' ',p.descripcion)", "SELECT codigo AS Código,modulo AS Módulo,accion AS Acción,descripcion AS Descripción,activo AS Activo FROM dbo.Permiso WHERE id_permiso=@id"));
        return fuentes;
    }
}
