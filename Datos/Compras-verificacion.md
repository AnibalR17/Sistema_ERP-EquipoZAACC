# Verificacion de compras y detalles

## Preparacion

Usar una base de pruebas con `Base de Datos/01_esquema_erp_3fn.sql` instalado y la conexion descrita en README. El esquema de creacion es para una base vacia; no ejecutarlo sobre una base existente con datos. Instalar `Base de Datos/Triggers2.sql` en la misma base, con nested triggers habilitado y RECURSIVE_TRIGGERS deshabilitado. La aplicacion no instala ni modifica el esquema.

Iniciar sesion con usuario y empleado activos. Preparar proveedor, tienda y dos productos activos mediante los CRUD existentes. El costo de compra es independiente del precio de venta del catalogo: `DetalleCompra.costo_unitario` requiere decimal(18,2) positivo; el esquema no incluye impuestos ni descuentos de compra.

## Casos funcionales contra SQL Server pendientes

1. Abrir Compras desde el menu principal. Comprobar proveedores, tiendas y productos activos, campos obligatorios, navegacion MDI, pestanas y acceso a acciones al reducir la ventana. No debe duplicarse el menu compartido.
2. Agregar dos productos con cantidades enteras positivas y costos de dos decimales. Seleccionar una linea, actualizar cantidad y costo y quitarla; comprobar subtotal y total. Probar producto repetido, seleccion vacia, cero, negativos, decimales en cantidad y valores fuera del limite decimal(18,2), tanto por linea como en el total.
3. Guardar una factura como BORRADOR. Verificar cabecera y detalles en una sola transaccion, autor autenticado y ausencia de movimientos de inventario. El costo no debe modificar el precio de venta del producto.
4. Buscar la compra en Historial por texto, estado y fechas de factura. Probar texto con apostrofo, corchetes, porcentaje y asterisco: se interpreta literalmente. Abrir el documento y comprobar cantidades, costos historicos y total, sin sustituirlos por precios actuales del catalogo.
5. Editar un borrador: proveedor y tienda permanecen fijos; factura, fecha, observaciones y lineas se pueden actualizar. Comprobar longitudes de factura (50) y observaciones (500). Dos facturas del mismo proveedor con el mismo numero deben rechazarse sin dejar cabeceras o detalles parciales.
6. Confirmar un borrador guardado con lineas. Debe pasar a CONFIRMADA, generar un movimiento COMPRA por detalle y aumentar existencias de la tienda una sola vez. Una compra confirmada queda de solo lectura. Repetir confirmacion desde otra instancia no debe duplicar movimientos.
7. Anular un borrador con motivo y confirmacion: pasa a ANULADA sin cambiar existencias. Anular una confirmada: genera REVERSO por cada movimiento original, conserva documento y detalles y reduce existencias sin dejar saldo menor que las reservas. Cancelar el dialogo no modifica datos; motivo vacio no permite continuar.
8. Consumir o reservar parte del inventario comprado y luego intentar anular la compra. Si falta stock disponible, comprobar rollback completo: estado CONFIRMADA, saldo y movimientos originales intactos, sin reversos parciales.
9. Abrir el mismo borrador en dos instancias. Cambiar cabecera o detalles desde una; guardar, confirmar o anular desde la otra debe informar conflicto y exigir recargar. El control de concurrencia incluye cabecera y lineas, ya que Compra no tiene rowversion en el esquema proporcionado.
10. Desactivar proveedor, tienda o producto despues de abrir un borrador. Guardar o confirmar debe rechazar la nueva operacion segun la validacion de activos. Los documentos historicos deben seguir visibles con sus referencias y costos originales.
11. Revocar o expirar la sesion, cambiar su version de seguridad o desactivar al usuario/empleado antes de escribir. Debe rechazarse la operacion sin atribuirla a otra cuenta. Comprobar SESSION_CONTEXT para auditoria y movimientos.
12. Deshabilitar un trigger requerido solo en la base de pruebas. Guardar, confirmar o anular debe informar la dependencia y no dejar cambios. Perder conexion durante carga o guardado debe mostrar el error sin cerrar la aplicacion ni borrar la captura.
13. Con cambios sin guardar, intentar abrir otro documento, Nuevo o cerrar Compras. Verificar confirmacion y conservacion de la captura cuando se cancela. Recargar catalogos no debe sustituir un borrador modificado silenciosamente.

## Verificacion local

Un ejecutable temporal sobre .NET 10 para Windows realizo **67 comprobaciones aprobadas**, sin conectarse a SQL Server ni consultar credenciales. Se verificaron construccion del formulario, pestanas, columnas y fechas iniciales; captura, actualizacion y retiro de lineas; subtotales y total; costos independientes del precio de venta; rechazo de duplicados, cantidades invalidas y desbordamientos; longitudes y campos obligatorios; estados de botones para compras nuevas, borradores, confirmadas y anuladas; bloqueo de resolucion con cambios pendientes; referencias historicas inactivas y limpieza mediante Nuevo; y lectura de costos en culturas es-HN y es-ES.

Tambien se renderizo la captura a 1180 x 900 y 1000 x 700 para revisar distribucion y desplazamiento. Se comprobo que el identificador interno del producto permanece oculto despues de crear los controles y enlazar sus datos. La construccion y la compilacion del ejecutable temporal finalizaron correctamente.

Estas comprobaciones no ejecutaron transacciones, triggers ni cambios de inventario. No se automatizaron respuestas a dialogos modales. Los casos anteriores requieren una base de pruebas operativa y la comprobacion interactiva del flujo MDI; permanecen pendientes.

## Limites

La autorizacion granular por permiso y tienda sigue pendiente en el sistema. Las compras usan la identidad de la sesion autenticada; no hay selector manual de operador. No se implementan recepciones parciales, impuestos ni pagos a proveedores porque no forman parte del flujo de Compra del esquema actual.
