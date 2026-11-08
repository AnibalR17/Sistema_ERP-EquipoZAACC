# Apartados y devoluciones

La sección tiene dos pestañas conectadas a SQL Server, con búsqueda, filtro por estado, consulta de detalles y actualización al regresar al módulo. Usa el esquema 3FN, `Base de Datos/Triggers2.sql` y `Datos/Migraciones/002_bitacora_sistema.sql`, ya utilizados por facturación. No requiere reiniciar la base ni eliminar datos.

## Apartados

1. Crear un apartado seleccionando tienda, cliente, vencimiento (hora de Honduras) y productos. Antes de guardar se pueden agregar y quitar líneas. El guardado reserva las unidades disponibles; valida precios y stock nuevamente en la transacción.
2. Registrar abonos hasta cubrir el saldo. En efectivo, la referencia se genera automáticamente al guardar el abono o cobrar el saldo de una entrega. Para métodos distintos de efectivo se solicita una referencia manual. No se aceptan abonos sobre el total ni sobre apartados vencidos o cerrados.
3. Entregar y facturar cobra el saldo pendiente, transforma cada abono en un pago de la factura y genera el pago final si corresponde. También admite apartados totalmente abonados. La confirmación de la venta libera la reserva y descuenta la existencia una sola vez. La factura aparece en el historial de facturación, donde puede reimprimirse.
4. Cancelar o marcar vencido libera la reserva. Vencer exige que ya haya llegado la fecha. Se solicita motivo y se registra en auditoría. Los abonos no se eliminan: Reembolsar abonos registra el importe pendiente por cada método original. Un segundo intento no duplica los reembolsos.

Ver detalle muestra resumen, productos, abonos y reembolsos. Las fechas de los listados y detalles se identifican como UTC. Las cantidades y precios quedan fijados al guardar el apartado; si se necesita una reserva diferente, se cancela la anterior y se crea otra, gestionando sus abonos por separado.

## Devoluciones

1. Elegir una factura confirmada y especificar un motivo.
2. En Devolver escribir la cantidad por línea (cero omite la línea). Disponible descuenta otras devoluciones no anuladas; no se puede superar lo vendido. Reintegrar indica si las unidades vuelven físicamente al inventario.
3. Guardar el borrador. Puede editarse antes de confirmarlo, conservando su factura original.
4. Confirmar aplica el inventario de las líneas marcadas. Reembolsar registra el saldo completo pendiente contra los pagos originales, distribuyéndolo cuando hubo varios métodos. Los acumulados nunca exceden el importe devuelto ni los cobros originales.
5. Anular revierte el inventario de una devolución sin reembolsos. La base rechaza anulaciones que dejen stock insuficiente o que afecten una devolución ya reembolsada.

Los reembolsos son registros contables del sistema; no hay integración con bancos o pasarelas. Las mutaciones validan la sesión y usan transacciones serializables y bloqueos. Se comprueba la versión del apartado o la huella del documento de devolución antes de operar. La devolución incluye sus detalles y reembolsos en la huella para rechazar una ventana desactualizada.

## Prueba de integración

```powershell
dotnet run --project Pruebas/FlujoVentas.Integracion/FlujoVentas.Integracion.csproj
```

El ejecutable obtiene la conexión de la configuración habitual. Requiere permiso para crear una base temporal en ese servidor. Crea una base con un nombre aleatorio `CodexERP_Prueba_<guid>`, instala el esquema y triggers, ejecuta los métodos reales contra datos propios y elimina únicamente esa base al terminar. No modifica documentos de la base configurada. No debe confundirse con los scripts de reinicio del proyecto.

Verificado:

- Importación de productos, stock inicial por tienda, RTN, historial y reversión completa de un lote con duplicados.
- Reserva, varios abonos, sobrepago rechazado, entrega y factura con abonos enlazados y cambio correcto.
- Cancelación, vencimiento, rechazo de abonos vencidos y reembolso de los abonos una sola vez.
- Entrega con saldo cero y reversión de una reserva con varios productos cuando uno carece de stock.
- Devoluciones parciales, reintegro opcional, edición del borrador y rechazo de versiones desactualizadas.
- Límite acumulado de unidades devueltas, reembolsos distribuidos entre pagos, rechazo de doble reembolso y anulación con reverso de stock.
- Atribución de auditoría, lectura y validación de los diez ejemplos XLSX por el importador real y carga de sus 35 unidades de stock en la base temporal.
- Construcción y disposición de las pantallas. Compilación sin errores ni advertencias.
