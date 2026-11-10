# Inicio conectado a ventas y compras

## Datos y actualización

`InicioDatos.Obtener` consulta las ventas y compras confirmadas mediante `vw_VentaTotal` y `vw_CompraTotal`. Devuelve siete fechas consecutivas, completando con cero las fechas sin movimientos. Las tarjetas muestran el importe y número de facturas del día; el gráfico compara los importes de los últimos siete días. Los borradores y documentos anulados no se incluyen.

Las ventas se agrupan en días de Honduras (UTC-6), convirtiendo la fecha UTC guardada en Venta. Las compras usan `fecha_factura`, igual que el módulo de compras. Los importes representan los totales de los documentos confirmados; no descuentan reembolsos de devoluciones.

Con la incorporación del ISV, el total de ventas incluye el impuesto guardado en las líneas originales. Las facturas anteriores conservan sus importes.

El selector de cabecera filtra todas las consultas por tienda; Todas las tiendas consolida los documentos de todas las sucursales. También se consultan traspasos pendientes y alertas de stock disponible de productos y tiendas activos. Al hacer clic en una tarjeta se abre ventas, compras, validación de traspasos o notificaciones mediante la navegación existente y su registro en bitácora.

La actualización se realiza al entrar o volver a Inicio, cambiar de tienda, pulsar Reiniciar y cada minuto mientras Inicio está activo. Reiniciar también consulta la lista de tiendas de la cabecera. Las consultas se ejecutan fuera del hilo de interfaz. Si cambia el filtro durante una consulta, se descarta el resultado anterior y se solicita el filtro actual. Los errores muestran datos sin cargar y permiten reintentar; no se presentan ceros como si la consulta hubiera tenido éxito.

## Verificación realizada

Se ejecutó un programa temporal .NET 10 que usa el método de datos real y una sesión de prueba dentro de una transacción ambiente. Creó catálogos, dos tiendas, compras y ventas propias. Se verificó:

- Tienda A: ventas del día por L 20 y compras por L 100. Tienda B: ventas por L 30 y compras por L 150.
- Inclusión del inicio de la ventana de siete días, días vacíos y exclusión de fechas anteriores y futuras.
- Venta exactamente a las 06:00 UTC como venta del día en Honduras y venta un segundo antes como venta del día anterior.
- Exclusión de compras y ventas en borrador o anuladas.
- Actualización del resumen global después de anular una venta por L 20 y confirmar una compra por L 20: ventas bajaron L 20 y compras subieron L 20, con el cambio correspondiente en el número de documentos.
- Conteo de un traspaso pendiente y una alerta de stock de la tienda de prueba.
- Construcción de las tarjetas y el botón Reiniciar, y renderizado del gráfico con el resultado real.

Todas las operaciones de prueba se revirtieron al cerrar la transacción; no quedan documentos de prueba guardados. SQL Server puede consumir identidades aunque una transacción se revierta. Compilación: cero errores y cero advertencias.
