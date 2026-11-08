using System.Data;
using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;

namespace Sistema_ERP;

internal sealed record LineaVenta(int Producto, int Cantidad, decimal Precio)
{
    public decimal TasaIsv { get; init; } = 15m;
}
internal sealed record FacturaEmitida(int Id, string Numero, decimal Total, decimal Cambio);

internal static class VentaDatos
{
    internal static string? NormalizarRtn(string? rtn)
    {
        string valor = new((rtn ?? "").Where(c => c != '-' && !char.IsWhiteSpace(c)).ToArray());
        if (valor.Length == 0) return null;
        if (valor.Length != 14 || valor.Any(c => c is < '0' or > '9'))
            throw new ArgumentException("El RTN es opcional. Si lo ingresas, debe contener 14 dígitos; se permiten espacios y guiones.");
        return valor;
    }

    internal static string RtnSugerido(object? documento)
    {
        try { return NormalizarRtn(Convert.ToString(documento)) ?? ""; }
        catch (ArgumentException) { return ""; }
    }

    internal static string Comprobante(int id)
    {
        using var cn = Conexion.AbrirConexion();
        SesionActual.Validar(cn);
        using var cmd = new SqlCommand("""
            SELECT v.numero,v.fecha,v.estado,t.nombre AS tienda,c.nombre AS cliente,v.rtn_cliente,v.cai,u.nombre_usuario,v.observaciones,
                p.monto,p.monto_recibido,p.referencia,m.nombre AS metodo
            FROM dbo.Venta v JOIN dbo.Tienda t ON t.id_tienda=v.id_tienda
            JOIN dbo.Cliente c ON c.id_cliente=v.id_cliente JOIN dbo.Usuario u ON u.id_usuario=v.id_usuario
            JOIN dbo.PagoVenta p ON p.id_venta=v.id_venta JOIN dbo.MetodoPago m ON m.id_metodo_pago=p.id_metodo_pago
            WHERE v.id_venta=@id;
            SELECT cantidad,nombre_producto,precio_unitario,subtotal,tasa_isv,isv,total_linea FROM dbo.DetalleVenta WHERE id_venta=@id ORDER BY id_detalle_venta;
            """, cn);
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
        using var adapter = new SqlDataAdapter(cmd);
        var datos = new DataSet(); adapter.Fill(datos);
        if (datos.Tables[0].Rows.Count == 0) throw new InvalidOperationException("No se encontró la factura y su pago.");
        var h = datos.Tables[0].Rows[0];
        var texto = new System.Text.StringBuilder($"FACTURA {h["numero"]}\nFecha UTC: {h["fecha"]:dd/MM/yyyy HH:mm}\nEstado: {h["estado"]}\nTienda: {h["tienda"]}\nCliente: {h["cliente"]}\n");
        if (h["rtn_cliente"] != DBNull.Value) texto.AppendLine("RTN: " + h["rtn_cliente"]);
        texto.AppendLine("CAI: " + (h["cai"] == DBNull.Value ? "Sin configurar" : h["cai"]));
        texto.AppendLine($"Usuario: {h["nombre_usuario"]}\n");
        decimal subtotal = 0, isv15 = 0, isv18 = 0, exento = 0, cambio = 0;
        foreach (DataRow d in datos.Tables[1].Rows)
        {
            texto.AppendLine($"{d["cantidad"]} x {d["nombre_producto"]}  L {d["precio_unitario"]:N2} · ISV {d["tasa_isv"]:0.##}% = L {d["total_linea"]:N2}");
            subtotal += (decimal)d["subtotal"];
            if ((decimal)d["tasa_isv"] == 15) isv15 += (decimal)d["isv"];
            else if ((decimal)d["tasa_isv"] == 18) isv18 += (decimal)d["isv"];
            else exento += (decimal)d["subtotal"];
        }
        texto.AppendLine($"\nSubtotal: L {subtotal:N2}\nExento: L {exento:N2}\nISV 15%: L {isv15:N2}\nISV 18%: L {isv18:N2}\nTotal: L {subtotal+isv15+isv18:N2}");
        foreach (DataRow p in datos.Tables[0].Rows)
        {
            texto.AppendLine($"Pago: {p["metodo"]} L {p["monto"]:N2} / Referencia: {p["referencia"]}");
            if (p["monto_recibido"] != DBNull.Value) cambio += (decimal)p["monto_recibido"] - (decimal)p["monto"];
        }
        texto.AppendLine($"Cambio: L {cambio:N2}\n{h["observaciones"]}");
        return texto.ToString().Replace("\r\n", "\n");
    }

    internal static DataTable Consultar(string sql, int? tienda = null)
    {
        using var cn = Conexion.AbrirConexion();
        SesionActual.Validar(cn);
        using var cmd = new SqlCommand(sql, cn);
        if (tienda.HasValue) cmd.Parameters.Add("@tienda", SqlDbType.Int).Value = tienda.Value;
        using var adapter = new SqlDataAdapter(cmd);
        var tabla = new DataTable(); adapter.Fill(tabla); return tabla;
    }

    internal static DataTable Productos(int tienda) => Consultar("""
        SELECT p.id_producto,p.codigo_barras AS Codigo,p.nombre AS Producto,p.precio_unitario AS Precio,
        ISNULL(i.existencia,0) AS Existencia,ISNULL(i.reservado,0) AS Reservado,
        ISNULL(i.existencia-i.reservado,0) AS Disponible
        FROM dbo.Producto p LEFT JOIN dbo.InventarioTienda i ON i.id_producto=p.id_producto AND i.id_tienda=@tienda
        WHERE p.activo=1 ORDER BY p.nombre
        """, tienda);

    internal static FacturaEmitida Emitir(int tienda, int cliente, int metodo, decimal recibido,
        string referencia, string observaciones, IReadOnlyCollection<LineaVenta> lineas, string? rtn = null)
    {
        rtn = NormalizarRtn(rtn);
        if (tienda <= 0 || cliente <= 0 || metodo <= 0 || lineas.Count == 0
            || lineas.Any(l => l.Producto <= 0 || l.Cantidad <= 0 || l.Precio <= 0)
            || lineas.Select(l => l.Producto).Distinct().Count() != lineas.Count)
            throw new ArgumentException("Selecciona tienda, cliente, método de pago y productos con cantidades positivas.");
        if (referencia.Length > 100 || observaciones.Length > 500 || recibido < 0)
            throw new ArgumentException("Revisa el monto, la referencia y las observaciones.");
        using var cn = Conexion.AbrirConexion();
        using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
        int usuario = SesionActual.Validar(cn, tx);
        using var preparar = new SqlCommand("""
            IF (SELECT value_in_use FROM sys.configurations WHERE name='nested triggers')<>1
                OR DATABASEPROPERTYEX(DB_NAME(),'IsRecursiveTriggersEnabled')=1
                THROW 50001,'Ventas requiere nested triggers habilitado y RECURSIVE_TRIGGERS OFF.',1;
            IF EXISTS(SELECT 1 FROM (VALUES ('trg_Venta_estado'),('trg_Venta_cabecera'),('trg_DetalleVenta_bloqueo'),
                ('trg_Venta_contabilizar'),('trg_Venta_auditoria'),('trg_MovimientoInventario_validar'),
                ('trg_MovimientoInventario_documento'),('trg_MovimientoInventario_aplicar'),('trg_MovimientoInventario_inmutable')) r(nombre)
                WHERE NOT EXISTS(SELECT 1 FROM sys.triggers WHERE object_id=OBJECT_ID('dbo.'+r.nombre) AND is_disabled=0))
                THROW 50001,'Instale los triggers de ventas e inventario de Triggers2.sql.',1;
            EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@usuario;
            EXEC sys.sp_set_session_context @key=N'motivo',@value=N'Facturación de venta';
            IF NOT EXISTS(SELECT 1 FROM dbo.Tienda WITH(HOLDLOCK) WHERE id_tienda=@tienda AND activo=1)
                OR NOT EXISTS(SELECT 1 FROM dbo.Cliente WITH(HOLDLOCK) WHERE id_cliente=@cliente AND activo=1)
                OR NOT EXISTS(SELECT 1 FROM dbo.MetodoPago WITH(HOLDLOCK) WHERE id_metodo_pago=@metodo AND activo=1)
                THROW 50001,'La tienda, cliente y método de pago deben estar activos.',1;
            SELECT codigo FROM dbo.MetodoPago WHERE id_metodo_pago=@metodo;
            """, cn, tx);
        preparar.Parameters.AddWithValue("@usuario", usuario);
        preparar.Parameters.AddWithValue("@tienda", tienda);
        preparar.Parameters.AddWithValue("@cliente", cliente);
        preparar.Parameters.AddWithValue("@metodo", metodo);
        bool efectivo = string.Equals(Convert.ToString(preparar.ExecuteScalar()), "EFECTIVO", StringComparison.OrdinalIgnoreCase);
        if (!efectivo && string.IsNullOrWhiteSpace(referencia)) throw new ArgumentException("Introduce la referencia del pago.");
        string numero = "V-" + Guid.NewGuid().ToString("N");
        using var cabecera = new SqlCommand("""
            INSERT dbo.Venta(numero,id_tienda,id_cliente,rtn_cliente,cai,id_usuario,observaciones)
            VALUES(@numero,@tienda,@cliente,@rtn,(SELECT cai FROM dbo.ConfiguracionFacturacion WITH(HOLDLOCK) WHERE id_tienda=@tienda),@usuario,@texto);
            SELECT CONVERT(int,SCOPE_IDENTITY());
            """, cn, tx);
        cabecera.Parameters.AddWithValue("@numero", numero);
        cabecera.Parameters.AddWithValue("@tienda", tienda);
        cabecera.Parameters.AddWithValue("@cliente", cliente);
        cabecera.Parameters.Add("@rtn", SqlDbType.VarChar, 30).Value = (object?)rtn ?? DBNull.Value;
        cabecera.Parameters.AddWithValue("@usuario", usuario);
        cabecera.Parameters.AddWithValue("@texto", observaciones.Trim());
        int id = Convert.ToInt32(cabecera.ExecuteScalar());
        decimal total = 0;
        foreach (var linea in lineas.OrderBy(l => l.Producto))
        {
            using var producto = new SqlCommand("""
                SELECT p.precio_unitario FROM dbo.Producto p WITH(HOLDLOCK)
                JOIN dbo.InventarioTienda i WITH(UPDLOCK,HOLDLOCK) ON i.id_producto=p.id_producto
                WHERE p.id_producto=@producto AND p.activo=1 AND i.id_tienda=@tienda AND i.existencia-i.reservado>=@cantidad
                """, cn, tx);
            producto.Parameters.AddWithValue("@producto", linea.Producto);
            producto.Parameters.AddWithValue("@tienda", tienda);
            producto.Parameters.AddWithValue("@cantidad", linea.Cantidad);
            object? precio = producto.ExecuteScalar();
            if (precio is null || precio == DBNull.Value) throw new InvalidOperationException("Un producto no está activo o no tiene existencias suficientes. Recarga el catálogo.");
            if (Convert.ToDecimal(precio) != linea.Precio) throw new InvalidOperationException("El precio de un producto cambió. Actualiza los catálogos y vuelve a agregar los productos.");
            decimal subtotalLinea = Convert.ToDecimal(precio) * linea.Cantidad;
            total = checked(total + subtotalLinea + ImpuestosVenta.Isv(subtotalLinea, linea.TasaIsv));
            using var detalle = new SqlCommand("""
                INSERT dbo.DetalleVenta(id_venta,id_producto,cantidad,precio_unitario,nombre_producto,tasa_isv)
                SELECT @venta,id_producto,@cantidad,precio_unitario,nombre,@tasa FROM dbo.Producto WHERE id_producto=@producto;
                """, cn, tx);
            detalle.Parameters.AddWithValue("@venta", id);
            detalle.Parameters.AddWithValue("@producto", linea.Producto);
            detalle.Parameters.AddWithValue("@cantidad", linea.Cantidad);
            detalle.Parameters.AddWithValue("@tasa", linea.TasaIsv);
            detalle.ExecuteNonQuery();
        }
        if (total <= 0 || total > 9999999999999999.99m) throw new ArgumentException("El total de la venta está fuera del rango permitido.");
        if (efectivo && recibido < total) throw new ArgumentException("El monto recibido es insuficiente.");
        using var pago = new SqlCommand("""
            INSERT dbo.PagoVenta(id_venta,id_metodo_pago,monto,monto_recibido,referencia) VALUES(@venta,@metodo,@total,@recibido,@referencia);
            UPDATE dbo.Venta SET estado='CONFIRMADA' WHERE id_venta=@venta;
            """, cn, tx);
        pago.Parameters.AddWithValue("@venta", id);
        pago.Parameters.AddWithValue("@metodo", metodo);
        foreach (var par in new[] { ("@total", total), ("@recibido", efectivo ? recibido : total) })
        { var p = pago.Parameters.Add(par.Item1, SqlDbType.Decimal); p.Precision = 18; p.Scale = 2; p.Value = par.Item2; }
        pago.Parameters.Add("@referencia", SqlDbType.NVarChar, 100).Value = referencia.Trim();
        pago.ExecuteNonQuery();
        tx.Commit();
        return new(id, numero, total, efectivo ? recibido-total : 0);
    }
}
