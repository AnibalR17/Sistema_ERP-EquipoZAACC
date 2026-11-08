using System.Data;
using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;
using static Sistema_ERP.FlujoVentaDatos;

namespace Sistema_ERP;

internal static class ApartadoDatos
{
    private const string Consulta = """
        SELECT a.id_apartado AS Id,c.nombre AS Cliente,t.nombre AS Tienda,a.fecha AS FechaUTC,a.vence_en AS VenceUTC,a.estado AS Estado,
            total.total AS Total,ISNULL(ab.Monto,0) AS Abonado,total.total-ISNULL(ab.Monto,0) AS Saldo,
            ISNULL(ab.Monto,0)-ISNULL(re.Monto,0) AS Reembolsable,a.id_venta AS Venta,a.version_fila,a.id_tienda,a.id_cliente
        FROM dbo.Apartado a JOIN dbo.Cliente c ON c.id_cliente=a.id_cliente JOIN dbo.Tienda t ON t.id_tienda=a.id_tienda
        JOIN dbo.vw_ApartadoTotal total ON total.id_apartado=a.id_apartado
        OUTER APPLY(SELECT SUM(monto) AS Monto FROM dbo.AbonoApartado WHERE id_apartado=a.id_apartado) ab
        OUTER APPLY(SELECT SUM(r.monto) AS Monto FROM dbo.Reembolso r JOIN dbo.AbonoApartado b ON b.id_abono=r.id_abono WHERE b.id_apartado=a.id_apartado) re
        """;

    internal static DataTable Listar(string buscar = "", string estado = "")
    {
        using var cn = Conexion.AbrirConexion(); SesionActual.Validar(cn);
        return Tabla(cn, null, Consulta + "\n" + """
            WHERE (@estado='' OR a.estado=@estado) AND (@buscar='' OR CHARINDEX(@buscar,c.nombre)>0
                OR CHARINDEX(@buscar,t.nombre)>0 OR CHARINDEX(@buscar,CONVERT(nvarchar(20),a.id_apartado))>0)
            ORDER BY a.id_apartado DESC
            """, ("@estado", estado), ("@buscar", buscar.Trim()));
    }

    internal static DataSet Obtener(int id)
    {
        using var cn = Conexion.AbrirConexion(); SesionActual.Validar(cn);
        var ds = new DataSet();
        ds.Tables.Add(Tabla(cn, null, Consulta + " WHERE a.id_apartado=@id", ("@id", id)));
        ds.Tables.Add(Tabla(cn, null, """
            SELECT p.nombre AS Producto,d.cantidad AS Cantidad,d.precio_unitario AS Precio,d.subtotal AS Subtotal,d.tasa_isv AS TasaISV,d.isv AS ISV,d.total_linea AS Importe
            FROM dbo.DetalleApartado d JOIN dbo.Producto p ON p.id_producto=d.id_producto WHERE d.id_apartado=@id
            """, ("@id", id)));
        ds.Tables.Add(Tabla(cn, null, """
            SELECT a.id_abono AS Id,a.fecha AS FechaUTC,m.nombre AS Metodo,a.monto AS Monto,a.referencia AS Referencia,a.id_pago_venta AS PagoVenta,
                a.monto-ISNULL((SELECT SUM(r.monto) FROM dbo.Reembolso r WHERE r.id_abono=a.id_abono),0) AS Reembolsable
            FROM dbo.AbonoApartado a JOIN dbo.MetodoPago m ON m.id_metodo_pago=a.id_metodo_pago WHERE a.id_apartado=@id ORDER BY a.id_abono
            """, ("@id", id)));
        ds.Tables.Add(Tabla(cn, null, """
            SELECT r.id_reembolso AS Id,r.fecha AS FechaUTC,r.id_abono AS Abono,r.monto AS Monto,r.motivo AS Motivo
            FROM dbo.Reembolso r JOIN dbo.AbonoApartado a ON a.id_abono=r.id_abono WHERE a.id_apartado=@id
            """, ("@id", id)));
        if (ds.Tables[0].Rows.Count != 1) throw new InvalidOperationException("El apartado ya no existe.");
        return ds;
    }

    internal static int Crear(int tienda, int cliente, DateTime venceUtc, IReadOnlyCollection<LineaVenta> lineas)
    {
        if (lineas.Count == 0 || lineas.Any(l => l.Producto <= 0 || l.Cantidad <= 0) || lineas.Select(l => l.Producto).Distinct().Count() != lineas.Count)
            throw new ArgumentException("Agrega productos sin repetir y con cantidades positivas.");
        using var cn = Conexion.AbrirConexion(); using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
        int usuario = Preparar(cn, tx, "Crear apartado");
        Ejecutar(cn, tx, """
            IF @vence<=SYSUTCDATETIME() THROW 50001,'El vencimiento debe ser posterior a la fecha actual.',1;
            IF NOT EXISTS(SELECT 1 FROM dbo.Tienda WITH(HOLDLOCK) WHERE id_tienda=@tienda AND activo=1)
                OR NOT EXISTS(SELECT 1 FROM dbo.Cliente WITH(HOLDLOCK) WHERE id_cliente=@cliente AND activo=1)
                THROW 50001,'La tienda y el cliente deben estar activos.',1;
            """, ("@vence", venceUtc), ("@tienda", tienda), ("@cliente", cliente));
        int id = Id(cn, tx, """
            INSERT dbo.Apartado(id_tienda,id_cliente,id_usuario,vence_en) VALUES(@tienda,@cliente,@usuario,@vence);
            SELECT CONVERT(int,SCOPE_IDENTITY());
            """, ("@tienda", tienda), ("@cliente", cliente), ("@usuario", usuario), ("@vence", venceUtc));
        decimal total = 0;
        foreach (var l in lineas.OrderBy(l => l.Producto))
        {
            Importe(l.Precio); total += l.Cantidad * l.Precio + ImpuestosVenta.Isv(l.Cantidad*l.Precio,l.TasaIsv); Importe(total);
            Ejecutar(cn, tx, """
                INSERT dbo.DetalleApartado(id_apartado,id_producto,cantidad,precio_unitario,tasa_isv)
                SELECT @id,p.id_producto,@cantidad,p.precio_unitario,@tasa FROM dbo.Producto p WITH(HOLDLOCK)
                JOIN dbo.InventarioTienda i WITH(UPDLOCK,HOLDLOCK) ON i.id_producto=p.id_producto AND i.id_tienda=@tienda
                WHERE p.id_producto=@producto AND p.activo=1 AND p.precio_unitario=@precio AND i.existencia-i.reservado>=@cantidad;
                IF @@ROWCOUNT<>1 THROW 50001,'El precio o stock cambió. Recarga los productos antes de reservar.',1;
                """, ("@id", id), ("@tienda", tienda), ("@producto", l.Producto), ("@cantidad", l.Cantidad), ("@precio", l.Precio), ("@tasa", l.TasaIsv));
        }
        tx.Commit(); return id;
    }

    private static DataRow Bloquear(SqlConnection cn, SqlTransaction tx, int id, byte[] version, bool activo)
    {
        var datos = Tabla(cn, tx, "SELECT * FROM dbo.Apartado WITH(UPDLOCK,HOLDLOCK) WHERE id_apartado=@id", ("@id", id));
        if (datos.Rows.Count != 1 || !((byte[])datos.Rows[0]["version_fila"]).SequenceEqual(version))
            throw new InvalidOperationException("El apartado cambió. Actualiza la lista antes de continuar.");
        var a = datos.Rows[0];
        if (activo && Convert.ToString(a["estado"]) != "ACTIVO") throw new InvalidOperationException("El apartado ya no está activo.");
        return a;
    }

    private static void Vigente(SqlConnection cn, SqlTransaction tx, int id) => Ejecutar(cn, tx,
        "IF EXISTS(SELECT 1 FROM dbo.Apartado WHERE id_apartado=@id AND vence_en<=SYSUTCDATETIME()) THROW 50001,'El apartado venció. Márcalo como vencido para liberar la reserva.',1;", ("@id", id));

    internal static void Abonar(int id, byte[] version, int metodo, decimal monto, string referencia)
    {
        Importe(monto);
        using var cn = Conexion.AbrirConexion(); using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
        int usuario = Preparar(cn, tx, "Abono a apartado"); Bloquear(cn, tx, id, version, true); Vigente(cn, tx, id);
        referencia = ReferenciaPago(ValidarPago(cn, tx, metodo, referencia), referencia);
        Ejecutar(cn, tx, """
            INSERT dbo.AbonoApartado(id_apartado,id_metodo_pago,monto,id_usuario,referencia) VALUES(@id,@metodo,@monto,@usuario,@referencia);
            """, ("@id", id), ("@metodo", metodo), ("@monto", monto), ("@usuario", usuario), ("@referencia", referencia.Trim()));
        tx.Commit();
    }

    internal static void Cerrar(int id, byte[] version, bool vencido, string motivo)
    {
        motivo = Motivo(motivo);
        using var cn = Conexion.AbrirConexion(); using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
        int usuario = Preparar(cn, tx, motivo); Bloquear(cn, tx, id, version, true);
        Ejecutar(cn, tx, """
            IF @estado='VENCIDO' AND EXISTS(SELECT 1 FROM dbo.Apartado WHERE id_apartado=@id AND vence_en>SYSUTCDATETIME())
                THROW 50001,'El apartado todavía no ha vencido.',1;
            UPDATE dbo.Apartado SET estado=@estado WHERE id_apartado=@id;
            """, ("@id", id), ("@estado", vencido ? "VENCIDO" : "CANCELADO"));
        Evento(cn, tx, usuario, "APARTADOS", vencido ? "VENCER" : "CANCELAR", id, motivo); tx.Commit();
    }

    internal static FacturaEmitida Entregar(int id, byte[] version, int metodo, decimal recibido, string referencia)
    {
        using var cn = Conexion.AbrirConexion(); using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
        int usuario = Preparar(cn, tx, "Entregar y facturar apartado"); var a = Bloquear(cn, tx, id, version, true); Vigente(cn, tx, id);
        Ejecutar(cn, tx, """
            IF NOT EXISTS(SELECT 1 FROM dbo.DetalleApartado WHERE id_apartado=@id)
                THROW 50001,'El apartado no contiene productos.',1;
            IF EXISTS(SELECT 1 FROM dbo.Apartado a JOIN dbo.Tienda t WITH(HOLDLOCK) ON t.id_tienda=a.id_tienda
                JOIN dbo.Cliente c WITH(HOLDLOCK) ON c.id_cliente=a.id_cliente WHERE a.id_apartado=@id AND (t.activo=0 OR c.activo=0))
                OR EXISTS(SELECT 1 FROM dbo.DetalleApartado d JOIN dbo.Producto p WITH(HOLDLOCK) ON p.id_producto=d.id_producto WHERE d.id_apartado=@id AND p.activo=0)
                THROW 50001,'La tienda, el cliente y los productos deben estar activos para facturar.',1;
            """, ("@id", id));
        var totales = Tabla(cn, tx, "SELECT total, total-ISNULL((SELECT SUM(monto) FROM dbo.AbonoApartado WHERE id_apartado=@id),0) AS saldo FROM dbo.vw_ApartadoTotal WHERE id_apartado=@id", ("@id", id)).Rows[0];
        decimal total = (decimal)totales["total"], saldo = (decimal)totales["saldo"]; Importe(total);
        bool efectivo = saldo > 0 && ValidarPago(cn, tx, metodo, referencia);
        if (efectivo && (recibido < saldo || decimal.Round(recibido, 2) != recibido)) throw new ArgumentException("El monto recibido no cubre el saldo.");
        if (saldo > 0) referencia = ReferenciaPago(efectivo, referencia);
        string numero = "V-" + Guid.NewGuid().ToString("N");
        int venta = Id(cn, tx, """
            INSERT dbo.Venta(numero,id_tienda,id_cliente,cai,id_usuario,observaciones)
            VALUES(@numero,@tienda,@cliente,(SELECT cai FROM dbo.ConfiguracionFacturacion WITH(HOLDLOCK) WHERE id_tienda=@tienda),@usuario,@texto);
            SELECT CONVERT(int,SCOPE_IDENTITY());
            """, ("@numero", numero), ("@tienda", a["id_tienda"]), ("@cliente", a["id_cliente"]), ("@usuario", usuario), ("@texto", "Entrega del apartado " + id));
        Ejecutar(cn, tx, """
            INSERT dbo.DetalleVenta(id_venta,id_producto,cantidad,precio_unitario,nombre_producto,tasa_isv)
            SELECT @venta,d.id_producto,d.cantidad,d.precio_unitario,p.nombre,d.tasa_isv FROM dbo.DetalleApartado d JOIN dbo.Producto p ON p.id_producto=d.id_producto WHERE d.id_apartado=@id;
            UPDATE dbo.Apartado SET id_venta=@venta WHERE id_apartado=@id;
            """, ("@id", id), ("@venta", venta));
        var abonos = Tabla(cn, tx, "SELECT * FROM dbo.AbonoApartado WITH(UPDLOCK,HOLDLOCK) WHERE id_apartado=@id ORDER BY id_abono", ("@id", id));
        foreach (DataRow b in abonos.Rows)
        {
            int pago = Id(cn, tx, """
                INSERT dbo.PagoVenta(id_venta,id_metodo_pago,monto,monto_recibido,referencia) VALUES(@venta,@metodo,@monto,@monto,@referencia);
                SELECT CONVERT(int,SCOPE_IDENTITY());
                """, ("@venta", venta), ("@metodo", b["id_metodo_pago"]), ("@monto", b["monto"]), ("@referencia", b["referencia"]));
            Ejecutar(cn, tx, "UPDATE dbo.AbonoApartado SET id_pago_venta=@pago WHERE id_abono=@abono", ("@pago", pago), ("@abono", b["id_abono"]));
        }
        if (saldo > 0) Ejecutar(cn, tx, "INSERT dbo.PagoVenta(id_venta,id_metodo_pago,monto,monto_recibido,referencia) VALUES(@venta,@metodo,@monto,@recibido,@referencia)",
            ("@venta", venta), ("@metodo", metodo), ("@monto", saldo), ("@recibido", efectivo ? recibido : saldo), ("@referencia", referencia.Trim()));
        Ejecutar(cn, tx, "UPDATE dbo.Venta SET estado='CONFIRMADA' WHERE id_venta=@venta", ("@venta", venta));
        tx.Commit(); return new(venta, numero, total, efectivo ? recibido - saldo : 0);
    }

    internal static decimal Reembolsar(int id, byte[] version, string motivo)
    {
        motivo = Motivo(motivo);
        using var cn = Conexion.AbrirConexion(); using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
        int usuario = Preparar(cn, tx, motivo); var a = Bloquear(cn, tx, id, version, false);
        if (Convert.ToString(a["estado"]) is not ("CANCELADO" or "VENCIDO")) throw new InvalidOperationException("Primero cancela el apartado o márcalo como vencido.");
        var abonos = Tabla(cn, tx, """
            SELECT b.id_abono,b.id_metodo_pago,b.monto-ISNULL((SELECT SUM(r.monto) FROM dbo.Reembolso r WHERE r.id_abono=b.id_abono),0) AS pendiente
            FROM dbo.AbonoApartado b WITH(UPDLOCK,HOLDLOCK) WHERE b.id_apartado=@id AND b.id_pago_venta IS NULL ORDER BY b.id_abono
            """, ("@id", id));
        decimal total = 0;
        foreach (DataRow b in abonos.Rows)
        {
            decimal monto = (decimal)b["pendiente"]; if (monto <= 0) continue;
            Ejecutar(cn, tx, "INSERT dbo.Reembolso(id_abono,monto,id_metodo_pago,id_usuario,motivo) VALUES(@abono,@monto,@metodo,@usuario,@motivo)",
                ("@abono", b["id_abono"]), ("@monto", monto), ("@metodo", b["id_metodo_pago"]), ("@usuario", usuario), ("@motivo", motivo)); total += monto;
        }
        if (total == 0) throw new InvalidOperationException("No hay abonos pendientes de reembolso.");
        tx.Commit(); return total;
    }
}
