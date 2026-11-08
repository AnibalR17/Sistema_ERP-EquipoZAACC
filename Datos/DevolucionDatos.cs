using System.Data;
using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;
using static Sistema_ERP.FlujoVentaDatos;

namespace Sistema_ERP;

internal sealed record LineaDevolucion(int DetalleVenta, int Cantidad, bool Reintegrar);

internal static class DevolucionDatos
{
    private const string Version = """
        HASHBYTES('SHA2_256',(SELECT d.id_devolucion,d.id_venta,d.estado,d.motivo,d.anulado_en,d.motivo_anulacion,
            (SELECT x.id_detalle_devolucion,x.id_detalle_venta,x.cantidad,x.reintegra_stock,x.isv FROM dbo.DetalleDevolucion x
                WHERE x.id_devolucion=d.id_devolucion ORDER BY x.id_detalle_devolucion FOR JSON PATH) AS lineas,
            (SELECT r.id_reembolso,r.monto FROM dbo.Reembolso r WHERE r.id_devolucion=d.id_devolucion ORDER BY r.id_reembolso FOR JSON PATH) AS reembolsos
            FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES))
        """;
    private static readonly string Consulta = $"""
        SELECT d.id_devolucion AS Id,v.numero AS Factura,c.nombre AS Cliente,t.nombre AS Tienda,d.fecha AS FechaUTC,
            d.estado AS Estado,d.motivo AS Motivo,total.total AS Total,ISNULL(re.Monto,0) AS Reembolsado,
            total.total-ISNULL(re.Monto,0) AS Pendiente,d.id_venta,{Version} AS version_fila
        FROM dbo.Devolucion d JOIN dbo.Venta v ON v.id_venta=d.id_venta JOIN dbo.Cliente c ON c.id_cliente=v.id_cliente
        JOIN dbo.Tienda t ON t.id_tienda=d.id_tienda JOIN dbo.vw_DevolucionTotal total ON total.id_devolucion=d.id_devolucion
        OUTER APPLY(SELECT SUM(monto) AS Monto FROM dbo.Reembolso WHERE id_devolucion=d.id_devolucion) re
        """;

    internal static DataTable Listar(string buscar = "", string estado = "")
    {
        using var cn = Conexion.AbrirConexion(); SesionActual.Validar(cn);
        return Tabla(cn, null, Consulta + """
            WHERE (@estado='' OR d.estado=@estado) AND (@buscar='' OR CHARINDEX(@buscar,v.numero)>0
                OR CHARINDEX(@buscar,c.nombre)>0 OR CHARINDEX(@buscar,CONVERT(nvarchar(20),d.id_devolucion))>0)
            ORDER BY d.id_devolucion DESC
            """, ("@estado", estado), ("@buscar", buscar.Trim()));
    }

    internal static DataTable Ventas() => VentaDatos.Consultar("""
        SELECT v.id_venta,CONCAT(v.numero,N' · ',c.nombre,N' · ',t.nombre) AS nombre
        FROM dbo.Venta v JOIN dbo.Cliente c ON c.id_cliente=v.id_cliente JOIN dbo.Tienda t ON t.id_tienda=v.id_tienda
        WHERE v.estado='CONFIRMADA' ORDER BY v.id_venta DESC
        """);

    internal static DataTable LineasVenta(int venta, int? devolucion = null)
    {
        using var cn = Conexion.AbrirConexion(); SesionActual.Validar(cn);
        return Tabla(cn, null, """
            SELECT dv.id_detalle_venta AS Id,dv.nombre_producto AS Producto,dv.cantidad AS Vendido,dv.precio_unitario AS Precio,dv.tasa_isv AS TasaISV,
                CONVERT(int,dv.cantidad-ISNULL(dev.cantidad,0)) AS Disponible,
                ISNULL(editar.cantidad,0) AS Devolver,ISNULL(editar.reintegra_stock,CONVERT(bit,1)) AS Reintegrar
            FROM dbo.DetalleVenta dv
            OUTER APPLY(SELECT SUM(CONVERT(bigint,x.cantidad)) AS cantidad FROM dbo.DetalleDevolucion x
                JOIN dbo.Devolucion d ON d.id_devolucion=x.id_devolucion
                WHERE x.id_detalle_venta=dv.id_detalle_venta AND d.estado<>'ANULADA' AND (@devolucion=0 OR d.id_devolucion<>@devolucion)) dev
            OUTER APPLY(SELECT cantidad,reintegra_stock FROM dbo.DetalleDevolucion WHERE id_detalle_venta=dv.id_detalle_venta AND id_devolucion=@devolucion) editar
            WHERE dv.id_venta=@venta ORDER BY dv.id_detalle_venta
            """, ("@venta", venta), ("@devolucion", devolucion ?? 0));
    }

    internal static DataSet Obtener(int id)
    {
        using var cn = Conexion.AbrirConexion(); SesionActual.Validar(cn);
        var ds = new DataSet();
        ds.Tables.Add(Tabla(cn, null, Consulta + " WHERE d.id_devolucion=@id", ("@id", id)));
        ds.Tables.Add(Tabla(cn, null, """
            SELECT dv.nombre_producto AS Producto,x.cantidad AS Cantidad,dv.precio_unitario AS Precio,
                x.cantidad*dv.precio_unitario AS Subtotal,x.isv AS ISV,x.cantidad*dv.precio_unitario+x.isv AS Importe,x.reintegra_stock AS Reintegrar
            FROM dbo.DetalleDevolucion x JOIN dbo.DetalleVenta dv ON dv.id_detalle_venta=x.id_detalle_venta WHERE x.id_devolucion=@id
            """, ("@id", id)));
        ds.Tables.Add(Tabla(cn, null, """
            SELECT r.id_reembolso AS Id,r.fecha AS FechaUTC,m.nombre AS Metodo,r.monto AS Monto,r.id_pago_venta AS Pago,r.motivo AS Motivo
            FROM dbo.Reembolso r JOIN dbo.MetodoPago m ON m.id_metodo_pago=r.id_metodo_pago WHERE r.id_devolucion=@id ORDER BY r.id_reembolso
            """, ("@id", id)));
        if (ds.Tables[0].Rows.Count != 1) throw new InvalidOperationException("La devolución ya no existe.");
        return ds;
    }

    private static DataRow Bloquear(SqlConnection cn, SqlTransaction tx, int id, byte[] version)
    {
        Ejecutar(cn, tx, """
            SELECT v.id_venta FROM dbo.Venta v WITH(UPDLOCK,HOLDLOCK) JOIN dbo.Devolucion d ON d.id_venta=v.id_venta WHERE d.id_devolucion=@id;
            SELECT id_devolucion FROM dbo.Devolucion WITH(UPDLOCK,HOLDLOCK) WHERE id_devolucion=@id;
            """, ("@id", id));
        var tabla = Tabla(cn, tx, Consulta + " WHERE d.id_devolucion=@id", ("@id", id));
        if (tabla.Rows.Count != 1 || !((byte[])tabla.Rows[0]["version_fila"]).SequenceEqual(version))
            throw new InvalidOperationException("La devolución cambió. Actualiza la lista antes de continuar.");
        return tabla.Rows[0];
    }

    internal static int Guardar(int? id, byte[]? version, int venta, string motivo, IReadOnlyCollection<LineaDevolucion> lineas)
    {
        motivo = Motivo(motivo);
        if (lineas.Count == 0 || lineas.Any(l => l.DetalleVenta <= 0 || l.Cantidad <= 0) || lineas.Select(l => l.DetalleVenta).Distinct().Count() != lineas.Count)
            throw new ArgumentException("Selecciona al menos un producto y una cantidad positiva a devolver, sin repetir líneas.");
        using var cn = Conexion.AbrirConexion(); using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
        int usuario = Preparar(cn, tx, motivo);
        var original = Tabla(cn, tx, "SELECT id_tienda FROM dbo.Venta WITH(UPDLOCK,HOLDLOCK) WHERE id_venta=@venta AND estado='CONFIRMADA'", ("@venta", venta));
        if (original.Rows.Count != 1) throw new InvalidOperationException("Selecciona una venta confirmada.");
        if (id.HasValue)
        {
            var anterior = Bloquear(cn, tx, id.Value, version ?? []);
            if (Convert.ToString(anterior["Estado"]) != "BORRADOR" || (int)anterior["id_venta"] != venta)
                throw new InvalidOperationException("Solo puedes editar el borrador y no puedes cambiar su factura.");
            Ejecutar(cn, tx, "DELETE dbo.DetalleDevolucion WHERE id_devolucion=@id; UPDATE dbo.Devolucion SET motivo=@motivo WHERE id_devolucion=@id", ("@id", id.Value), ("@motivo", motivo));
        }
        int numero = id ?? Id(cn, tx, """
            INSERT dbo.Devolucion(id_venta,id_tienda,id_usuario,motivo) VALUES(@venta,@tienda,@usuario,@motivo);
            SELECT CONVERT(int,SCOPE_IDENTITY());
            """, ("@venta", venta), ("@tienda", original.Rows[0][0]), ("@usuario", usuario), ("@motivo", motivo));
        foreach (var l in lineas.OrderBy(l => l.DetalleVenta))
            Ejecutar(cn, tx, """
                IF NOT EXISTS(SELECT 1 FROM dbo.DetalleVenta WITH(UPDLOCK,HOLDLOCK) WHERE id_detalle_venta=@detalle AND id_venta=@venta)
                    THROW 50001,'Una línea no pertenece a la factura seleccionada.',1;
                INSERT dbo.DetalleDevolucion(id_devolucion,id_detalle_venta,cantidad,reintegra_stock,isv)
                SELECT @id,@detalle,@cantidad,@stock,
                    CASE WHEN calculo.acumulado>ISNULL(prev.isv,0) THEN calculo.acumulado-ISNULL(prev.isv,0) ELSE 0 END
                FROM dbo.DetalleVenta dv
                OUTER APPLY(SELECT SUM(CONVERT(bigint,x.cantidad)) AS cantidad,SUM(x.isv) AS isv
                    FROM dbo.DetalleDevolucion x JOIN dbo.Devolucion d ON d.id_devolucion=x.id_devolucion
                    WHERE x.id_detalle_venta=dv.id_detalle_venta AND d.estado<>'ANULADA') prev
                CROSS APPLY(VALUES(CONVERT(decimal(18,2),ROUND(dv.isv*(CONVERT(decimal(18,4),ISNULL(prev.cantidad,0))+@cantidad)/dv.cantidad,2)))) calculo(acumulado)
                WHERE dv.id_detalle_venta=@detalle;
                """, ("@id", numero), ("@venta", venta), ("@detalle", l.DetalleVenta), ("@cantidad", l.Cantidad), ("@stock", l.Reintegrar));
        tx.Commit(); return numero;
    }

    internal static void Confirmar(int id, byte[] version)
    {
        using var cn = Conexion.AbrirConexion(); using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
        Preparar(cn, tx, "Confirmar devolución"); var d = Bloquear(cn, tx, id, version);
        if (Convert.ToString(d["Estado"]) != "BORRADOR") throw new InvalidOperationException("Solo se confirma una devolución en borrador.");
        Ejecutar(cn, tx, "UPDATE dbo.Devolucion SET estado='CONFIRMADA' WHERE id_devolucion=@id", ("@id", id)); tx.Commit();
    }

    internal static void Anular(int id, byte[] version, string motivo)
    {
        motivo = Motivo(motivo);
        using var cn = Conexion.AbrirConexion(); using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
        int usuario = Preparar(cn, tx, motivo); var d = Bloquear(cn, tx, id, version);
        if (Convert.ToString(d["Estado"]) == "ANULADA") throw new InvalidOperationException("La devolución ya está anulada.");
        Ejecutar(cn, tx, """
            UPDATE dbo.Devolucion SET estado='ANULADA',anulado_en=SYSUTCDATETIME(),anulado_por=@usuario,motivo_anulacion=@motivo WHERE id_devolucion=@id
            """, ("@id", id), ("@usuario", usuario), ("@motivo", motivo)); tx.Commit();
    }

    internal static decimal Reembolsar(int id, byte[] version, string motivo)
    {
        motivo = Motivo(motivo);
        using var cn = Conexion.AbrirConexion(); using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
        int usuario = Preparar(cn, tx, motivo); var d = Bloquear(cn, tx, id, version);
        if (Convert.ToString(d["Estado"]) != "CONFIRMADA") throw new InvalidOperationException("Primero confirma la devolución.");
        decimal pendiente = (decimal)d["Pendiente"], total = pendiente;
        if (pendiente <= 0) throw new InvalidOperationException("La devolución ya está reembolsada.");
        var pagos = Tabla(cn, tx, """
            SELECT p.id_pago_venta,p.id_metodo_pago,p.monto-ISNULL((SELECT SUM(monto) FROM dbo.Reembolso r WHERE r.id_pago_venta=p.id_pago_venta),0) AS Disponible
            FROM dbo.PagoVenta p WITH(UPDLOCK,HOLDLOCK) WHERE p.id_venta=@venta ORDER BY p.id_pago_venta
            """, ("@venta", d["id_venta"]));
        foreach (DataRow p in pagos.Rows)
        {
            decimal monto = Math.Min(pendiente, (decimal)p["Disponible"]); if (monto <= 0) continue;
            Ejecutar(cn, tx, """
                INSERT dbo.Reembolso(id_pago_venta,id_devolucion,monto,id_metodo_pago,id_usuario,motivo)
                VALUES(@pago,@id,@monto,@metodo,@usuario,@motivo)
                """, ("@pago", p["id_pago_venta"]), ("@id", id), ("@monto", monto), ("@metodo", p["id_metodo_pago"]), ("@usuario", usuario), ("@motivo", motivo));
            pendiente -= monto; if (pendiente == 0) break;
        }
        if (pendiente > 0) throw new InvalidOperationException("Los pagos de la venta no tienen suficiente saldo reembolsable.");
        tx.Commit(); return total;
    }
}
