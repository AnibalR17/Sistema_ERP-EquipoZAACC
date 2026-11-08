# ISV y CAI en ventas

## Cálculo y recibos

Los precios guardados son precios base. Según la opción elegida por el usuario, el sistema **suma** el impuesto al precio actual. La tasa inicial de cada producto agregado al carrito es 15 %. El selector junto a Cantidad permite elegir ISV 18 % o Exento (0 %) para ese producto cuando corresponda. Para corregir la tasa de una línea, quitarla y agregarla con la tasa apropiada.

El SAR publica la tasa general del 15 % y la del 18 % en su [ayuda del ISV](https://www.sar.gob.hn/wp-content/uploads/2026/03/DECLARACION-REGIMEN-SIMPLIFICADO-DEL-IMPUESTO-SOBRE-VENTA-CODIGO-202.pdf). La clasificación fiscal debe corresponder al producto vendido. No se determina automáticamente a partir del nombre del producto.

Por línea: `subtotal = cantidad × precio base`; `ISV = redondear(subtotal × tasa / 100, 2)`; `total = subtotal + ISV`. Se redondea a dos decimales alejándose de cero, de forma coincidente entre C# y SQL Server. La pantalla presenta subtotal, ISV del 15 %, ISV del 18 %, total y cambio. Los pagos y el cambio usan el total con impuesto. Ejemplo: precio base L 100, ISV 15 % L 15, total L 115.

El recibo y las reimpresiones se generan desde los detalles y tasas guardados con la factura. Muestran también el importe exento, RTN opcional y CAI. Inicio e historial de ventas usan el total de la vista de ventas, incluido el impuesto.

Los apartados nuevos reservan productos con ISV 15 % y calculan el saldo con impuesto. Al entregarlos, la factura conserva sus tasas, precios y abonos. Las devoluciones calculan el impuesto a partir de la línea original, distribuyendo los centavos entre devoluciones parciales para restituir exactamente el ISV cobrado al devolver todas las unidades. Cancelar o confirmar una devolución mantiene las reglas de stock y reembolso.

## Configuración del CAI

Abrir **Configuración y auditoría → Facturación / CAI**, seleccionar la tienda e ingresar el CAI autorizado. Guardar registra el cambio en la bitácora. El [SAR identifica el CAI como la Clave de Autorización de Impresión](https://www.sar.gob.hn/wp-content/uploads/2021/02/Solicitud-de-Activacion-de-Impresion-por-Imprenta-SAR-925.pdf).

Se admite el formato `XXXXXX-XXXXXX-XXXXXX-XXXXXX-XXXXXX-XX`, o los 32 caracteres sin guiones; se normaliza a mayúsculas y 37 caracteres. El formato no valida la autorización ante el SAR. La clave se guarda por tienda y se copia a cada nueva venta, incluidas las entregas de apartados. Cambiar el CAI de configuración no cambia las facturas anteriores. Si la tienda no tiene CAI, el recibo indica “Sin configurar”; no se genera una clave ficticia.

Esta incorporación guarda la clave y el desglose de ISV. El sistema utiliza su número interno de venta; la autorización de rangos, correlativos fiscales y fecha límite de emisión requiere un flujo fiscal adicional.

Configuración queda organizada en pestañas de Tiendas, Facturación / CAI y Auditoría, con desplazamiento dentro de sus tablas y formularios.

## Base de datos y verificación

Ejecutar `Datos/Migraciones/004_isv_cai.sql` después de las migraciones anteriores en bases existentes. Agrega CAI por factura, la configuración por tienda, tasas e impuestos y actualiza las vistas de totales y la comprobación de entrega de apartados. Es idempotente. El esquema inicial y Triggers2.sql también incluyen estos cambios.

Las líneas existentes reciben tasa e impuesto cero, conservando los importes anteriores. Los nuevos detalles tienen tasa 15 % por defecto y aceptan solo 0, 15 y 18. Se verificó en la base local que el total histórico anterior se conserva y que una segunda ejecución de la migración termina correctamente.

`dotnet run --project Pruebas/FlujoVentas.Integracion/FlujoVentas.Integracion.csproj` crea y elimina una base temporal. Se prueba la actualización desde la estructura anterior y los métodos reales de emisión, configuración y reembolso. Pasaron los casos de 15/18/exento, pago insuficiente, cambio, tasas inválidas, CAI por tienda, actualización concurrente del CAI, reimpresión histórica y redondeo de devoluciones parciales. También pasó el flujo previo de apartados, RTN e importación y la construcción de las pantallas. Compilación sin errores ni advertencias.

La prueba de auditoría de `Datos/Pruebas/Bitacora-sistema.sql` cubre ahora 38 entidades operativas, incluida ConfiguracionFacturacion, y se revierte al finalizar.
