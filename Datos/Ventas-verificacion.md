# Ventas y bitácora

## Instalación

Usar la conexión local descrita en README. En una instalación nueva, ejecutar el esquema 3FN, `Base de Datos/Triggers2.sql` y después `Datos/Migraciones/002_bitacora_sistema.sql`, en esa misma base. La migración es transaccional y se puede volver a ejecutar. No ejecutar los scripts de reinicio para actualizar una base existente.

Para una base existente, ejecutar también `Datos/Migraciones/003_rtn_ventas.sql`: agrega `Venta.rtn_cliente` opcional y su validación, sin cambiar las facturas anteriores. Ya se aplicó en la base local y se comprobó que puede volver a ejecutarse. El esquema inicial también incluye el campo para instalaciones nuevas.

Para ISV y CAI, aplicar después `Datos/Migraciones/004_isv_cai.sql`. Consulte [ISV-CAI-verificacion.md](ISV-CAI-verificacion.md). La base local ya está actualizada.

La migración 002 ya se aplicó a la base local durante esta implementación. Complementa los seis triggers de auditoría existentes con 31 triggers sobre las demás tablas. Las 37 tablas operativas del esquema actual quedan cubiertas; Auditoria conserva su protección contra modificaciones y eliminaciones. Al agregar tablas nuevas, volver a ejecutar la migración y verificar la cobertura.

## Uso

En Ventas, seleccionar tienda, cliente y método de pago activos. Buscar por nombre o código, seleccionar un producto y agregar la cantidad al carrito (también con doble clic). Quitar o vaciar permite corregir el detalle. Reiniciar actualiza los catálogos y conserva el carrito y sus precios capturados mientras la tienda seleccionada siga disponible; si cambia la tienda se limpia el carrito. Los productos disponibles muestran el catálogo actual para las nuevas líneas.

El botón **Refrescar stock**, junto al buscador, vuelve a consultar las existencias de la tienda seleccionada y limpia el filtro para mostrar todos sus productos activos. Conserva la tienda, cliente, pago y carrito. La tabla muestra Existencia, Reservado y Disponible (existencia menos reservado), y el mensaje indica la tienda y hora de actualización. Al regresar a ventas desde otro módulo también se actualiza el stock, conservando la búsqueda. Si la consulta falla, se retiran los datos antiguos y se muestra el error.

Para efectivo, ingresar un monto recibido suficiente; se calcula el cambio. Para otros métodos, ingresar la referencia. Emitir factura pide confirmación y guarda cabecera, detalles y pago en una transacción serializable. Los precios se verifican contra la base antes de cobrar; los triggers descuentan el inventario disponible, respetan reservas y registran auditoría. Cualquier error antes del commit revierte toda la operación. Al precio base se suma el ISV seleccionado por línea (15 % por defecto; también 18 % y exento). El cobro, el cambio y el recibo usan el total con impuesto.

El campo **RTN del cliente (opcional)** permite facturar sin RTN. Si se escribe, debe contener 14 dígitos; se eliminan guiones y espacios y se conservan los ceros iniciales. Al cambiar de cliente se precarga su `documento_fiscal` si tiene ese formato; se puede editar o borrar para esa factura. El RTN se guarda con la venta, aparece en el comprobante y la reimpresión y permite buscar la factura en el historial. No modifica el registro del cliente ni depende de cambios posteriores en ese registro. Después de emitir la factura se limpia el campo.

Los cobros nuevos en efectivo generan su referencia automáticamente al guardarse, con formato `EFE-` seguido de un identificador aleatorio de 32 caracteres. Se aplica a ventas, abonos y cobro del saldo al entregar apartados. El campo de referencia es de solo lectura para efectivo y muestra «Se genera al registrar el pago». La referencia queda en el pago y en el recibo; al facturar un apartado se conserva la referencia original de cada abono. Los demás métodos requieren una referencia manual. No requiere migraciones ni modifica pagos anteriores.

Se puede ver e imprimir el comprobante emitido. Facturas registradas permite buscar entre las últimas 500 facturas y recuperar sus detalles y pagos desde SQL Server para reimprimir. La búsqueda de auditoría opera en servidor y las páginas muestran hasta 200 eventos. Actualizar vuelve a los más recientes; Anteriores/Siguientes permiten recorrer las páginas. Ambas tablas de configuración usan desplazamiento local; el formulario conserva desplazamiento para pantallas pequeñas.

La bitácora registra altas, modificaciones, eliminaciones, intentos de acceso (incluido su resultado), sesiones y navegación entre módulos. El contexto de usuario se establece al abrir cada conexión y se limpia el motivo para evitar atribuciones incorrectas por reutilización de conexiones. La captura nueva evita copiar contraseñas y tokens. Los cambios que no se registraban antes de instalar la migración no se pueden reconstruir retrospectivamente.

## Verificación realizada

- `dotnet build --no-restore`: cero errores y cero advertencias.
- Creación y disposición de ambos formularios mediante un ejecutable temporal .NET 10/WinForms: correctas; tablas de consulta de solo lectura.
- Migración aplicada y vuelta a ejecutar correctamente en la base local; cero tablas operativas sin trigger de auditoría.
- Prueba SQL transaccional: creación y confirmación de una venta con detalle y pago; descuento de una unidad; verificación de auditoría de venta y pago con usuario responsable; registro de un acceso inválido con su resultado. Se revirtió la transacción completa para no conservar ventas de prueba.
- Cobertura integral posterior: [Bitacora-verificacion.md](Bitacora-verificacion.md) documenta la prueba con eventos reales en las 37 entidades del sistema, también fuera de ventas.
- Refresco de stock: se ejecutó la consulta real de facturación antes y después de una entrada de inventario y de una reserva por apartado. Reconoció la unidad nueva y descontó la reservada. Toda la prueba se revirtió. Se comprobó también la disposición del botón junto al buscador en un formulario de 1400 × 900.
- RTN: probado con y sin valor, guiones, espacios y ceros iniciales. Se verificó su persistencia, impresión, auditoría y conservación tras editar el documento fiscal del cliente. Los RTN inválidos no crean ventas; cambiar de cliente limpia el valor anterior y precarga el nuevo si corresponde. La prueba de integración usa una base temporal y la elimina al terminar.
- Referencias de efectivo: integración verificada en ventas, abonos y saldo de entrega; referencias distintas por cobro, conservación de la referencia del abono al facturar, aparición en el recibo y registro auditado del pago. También se comprobaron el cambio de método en las tres pantallas de cobro, las referencias manuales de transferencias y la reversión de cobros inválidos. La suite completa pasó en una base temporal eliminada al terminar; compilación con cero errores y advertencias.

Pendiente de comprobación manual: recorrido visual interactivo a distintas resoluciones y salida en una impresora física. No se envió ninguna impresión durante las pruebas.
