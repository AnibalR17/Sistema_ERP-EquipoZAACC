# Recarga manual de datos

El botón **Reiniciar** vuelve a consultar los datos sin cerrar la pantalla. Se aplica a las pantallas con consultas y sus ventanas de detalle; los diálogos que solo piden confirmación, credenciales o un motivo no requieren recarga.

| Pantalla | Datos que recarga |
| --- | --- |
| Inicio | Tiendas de la cabecera, tarjetas y gráfico según los permisos y la tienda seleccionada. |
| Catálogo y pantalla de productos | Listado de productos; la pestaña Categorías tiene su propio botón. |
| Nuevo/editar producto | Categorías y proveedores, conservando los campos capturados y las selecciones que sigan disponibles. |
| Clientes, proveedores y directorio | Listado de la pestaña actual; el directorio de empleados también consulta las tiendas. |
| Agregar empleado | Tiendas activas disponibles para la captura. |
| Usuarios, roles y permisos | Registros y asignaciones de la pestaña actual. |
| Ventanas de permisos | Roles, módulos y acciones actuales, conservando las selecciones que continúen en el catálogo. Al aceptar se consulta nuevamente el catálogo del formulario para incorporar opciones nuevas. |
| Configuración | Tiendas; Facturación/CAI y Auditoría tienen botones propios. Auditoría vuelve a la primera página del filtro actual. |
| Compras | Catálogos e historial. Conserva la confirmación de descarte antes de sustituir una captura modificada. |
| Inventario | Tiendas, existencias y movimientos del filtro actual. |
| Facturación | Tiendas, clientes, métodos de pago y stock. Conserva el carrito y la captura si sigue disponible la misma tienda. |
| Historial y recibos de ventas | Facturas registradas o comprobante mostrado. El historial conserva su búsqueda. |
| Apartados y devoluciones | Listados de ambas pestañas con sus filtros y detalles abiertos. Los editores actualizan sus catálogos; el borrador de devolución también vuelve a consultar sus líneas y versión. El cobro actualiza los métodos de pago. |
| Traspasos y validación | Catálogos del editor, listado de validación y productos/historial del detalle abierto. |
| Carga masiva | Tiendas; si hay un archivo seleccionado, vuelve a leerlo y validarlo con los catálogos actuales. |
| Notificaciones | Alertas de stock actuales. |
| Búsqueda global | Repite la consulta de la barra superior; el detalle vuelve a consultar el registro autorizado. |

Los botones nuevos comparten presentación, nombre accesible y manejo de errores mediante `Presentacion/RecargaManual.cs`. Durante su consulta se deshabilitan para evitar solicitudes repetidas y vuelven a habilitarse al terminar. Los botones existentes conservan las protecciones de carga de su pantalla. Recargar no guarda registros y continúa aplicando la autorización de la capa de datos.

## Verificación

El 9 de noviembre de 2026, `dotnet run --project Pruebas/FlujoVentas.Integracion` completó 25 comprobaciones nuevas de recarga manual con SQL Server en una base temporal eliminada al finalizar. Se verificaron altas externas en catálogos, búsquedas conservadas, pestañas, carga asíncrona de compras/inventario, conservación del carrito y captura, opciones nuevas en los selectores de permisos y recarga de traspasos/validación. La suite también completó los flujos anteriores, 23 comprobaciones de permisos de usuarios, 23 de permisos por módulo, 42 de autorización y 103 de búsqueda global. Estas últimas incluyen la recarga real de los 15 tipos de detalle y la repetición de la búsqueda después de modificar un producto desde otra conexión. Una cuenta de consulta conserva visible Reiniciar sin habilitar acciones de escritura.

Compilación sobre .NET 10 sin errores ni advertencias. Al abrir la pantalla alternativa de Clientes se corrigieron dos errores previos de construcción: uso de un buscador nulo y peso cero en una columna oculta.
