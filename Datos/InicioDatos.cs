using System.Data;
using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;

namespace Sistema_ERP;

internal sealed record DiaInicio(DateTime Fecha, decimal Ventas, long FacturasVenta, decimal Compras, long FacturasCompra);
internal sealed record ResumenInicio(DiaInicio[] Dias, long TraspasosPendientes, long AlertasStock);

internal static class InicioDatos
{
    internal static DateTime AhoraHonduras => DateTime.UtcNow.AddHours(-6);

    internal static ResumenInicio Obtener(int? tienda, DateTime hoy)
    {
        using var conexion = Conexion.AbrirConexion();
        SesionActual.Validar(conexion);
        using var comando = new SqlCommand("""
            WITH dias AS (
                SELECT DATEADD(day,n,@desde) AS Fecha FROM (VALUES(0),(1),(2),(3),(4),(5),(6)) d(n)
            ), ventas AS (
                SELECT CONVERT(date,DATEADD(hour,-6,v.fecha)) AS Fecha,SUM(t.total) AS Total,COUNT_BIG(*) AS Cantidad
                FROM dbo.Venta v JOIN dbo.vw_VentaTotal t ON t.id_venta=v.id_venta
                WHERE v.estado='CONFIRMADA' AND (@tienda IS NULL OR v.id_tienda=@tienda)
                    AND v.fecha>=@inicioUtc AND v.fecha<@finUtc
                GROUP BY CONVERT(date,DATEADD(hour,-6,v.fecha))
            ), compras AS (
                SELECT c.fecha_factura AS Fecha,SUM(t.total) AS Total,COUNT_BIG(*) AS Cantidad
                FROM dbo.Compra c JOIN dbo.vw_CompraTotal t ON t.id_compra=c.id_compra
                WHERE c.estado='CONFIRMADA' AND (@tienda IS NULL OR c.id_tienda=@tienda)
                    AND c.fecha_factura>=@desde AND c.fecha_factura<@hasta
                GROUP BY c.fecha_factura
            )
            SELECT d.Fecha,ISNULL(v.Total,0) AS Ventas,ISNULL(v.Cantidad,0) AS FacturasVenta,
                ISNULL(c.Total,0) AS Compras,ISNULL(c.Cantidad,0) AS FacturasCompra
            FROM dias d LEFT JOIN ventas v ON v.Fecha=d.Fecha LEFT JOIN compras c ON c.Fecha=d.Fecha
            ORDER BY d.Fecha;

            SELECT (SELECT COUNT_BIG(*) FROM dbo.Traspaso
                WHERE estado='PENDIENTE' AND (@tienda IS NULL OR id_tienda_origen=@tienda OR id_tienda_destino=@tienda)) AS TraspasosPendientes,
                (SELECT COUNT_BIG(*) FROM dbo.InventarioTienda i
                JOIN dbo.Tienda t ON t.id_tienda=i.id_tienda JOIN dbo.Producto p ON p.id_producto=i.id_producto
                WHERE t.activo=1 AND p.activo=1 AND i.existencia-i.reservado<=i.stock_minimo
                    AND (@tienda IS NULL OR i.id_tienda=@tienda)) AS AlertasStock;
            """, conexion);
        DateTime desde = hoy.Date.AddDays(-6), hasta = hoy.Date.AddDays(1);
        comando.Parameters.Add("@tienda", SqlDbType.Int).Value = (object?)tienda ?? DBNull.Value;
        comando.Parameters.Add("@desde", SqlDbType.Date).Value = desde;
        comando.Parameters.Add("@hasta", SqlDbType.Date).Value = hasta;
        comando.Parameters.Add("@inicioUtc", SqlDbType.DateTime2).Value = desde.AddHours(6);
        comando.Parameters.Add("@finUtc", SqlDbType.DateTime2).Value = hasta.AddHours(6);
        using var lector = comando.ExecuteReader();
        var dias = new List<DiaInicio>();
        while (lector.Read()) dias.Add(new(lector.GetDateTime(0), lector.GetDecimal(1), lector.GetInt64(2), lector.GetDecimal(3), lector.GetInt64(4)));
        if (!lector.NextResult() || !lector.Read()) throw new InvalidOperationException("No se pudo obtener el resumen de Inicio.");
        return new(dias.ToArray(), lector.GetInt64(0), lector.GetInt64(1));
    }
}
