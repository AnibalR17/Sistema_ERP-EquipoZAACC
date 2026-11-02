# Traspasos entre tiendas

Solicitud: Alejandro Rovelo. Validación: Anibal Rovelo; depende de solicitudes persistidas.

La referencia es files (1).zip: 01_esquema_erp_3fn.sql y Triggers2.sql. Las definiciones de tablas, restricciones, índices y vistas coinciden con el esquema del proyecto. Se conserva su configuración inicial de base de datos.

## Preparación

En una base vacía, ejecute primero Base de Datos/01_esquema_erp_3fn.sql. Después, seleccione esa misma base y ejecute Base de Datos/Triggers2.sql. La copia de triggers conserva las reglas del ZIP y utiliza la base seleccionada, sin forzar el nombre Inventario. No se han ejecutado estos scripts sobre la base configurada ni se ha utilizado el script de reinicio.

El script requiere SQL Server 2016 SP1 o posterior, nested triggers habilitado y RECURSIVE_TRIGGERS deshabilitado. Antes de instalarlo sobre datos existentes, revise sus requisitos de conciliación de inventario. Configure la conexión como indica el proyecto y use datos de prueba con dos tiendas, productos, usuarios activos e inventario disponible en origen.

La aplicación establece id_usuario y motivo en SESSION_CONTEXT dentro de la conexión y transacción de escritura. El trigger trg_Traspaso_operar genera los eventos y reserva inventario al aprobar; el código C# no duplica esas operaciones. Sin ese trigger habilitado, la aplicación impide las escrituras de traspasos.

## Prueba funcional con SQL Server

1. Abra Traspaso entre Tiendas: debe mostrar directamente el formulario de creación con Guardar solicitud en una fila inferior fija, sin Consultar solicitudes. Seleccione un usuario, tiendas distintas y productos con cantidades positivas y precios de referencia. Guarde y compruebe el mensaje con número y estado PENDIENTE, y la limpieza de los campos. Abra Validación de Traspasos para comprobar el listado y el detalle persistidos. Compruebe la fila de guardado al redimensionar la ventana; si falla la carga de datos, el botón debe seguir visible y deshabilitado.
2. Desde Validación de Traspasos, seleccione un usuario y edite una solicitud pendiente; compruebe observaciones y líneas después de actualizar. Las tiendas deben estar deshabilitadas: el trigger impide cambiar tiendas y solicitante. El listado centraliza edición, cancelación, aprobación y rechazo de pendientes.
3. Intente guardar tiendas iguales, productos repetidos, cantidades cero, negativas o decimales, precios negativos y solicitudes vacías. Deben impedir el guardado completo.
4. Cancele una pendiente con motivo. Compruebe el estado y el evento con usuario y fecha; no debe permitir editarla ni validarla.
5. Abra Validación de traspasos. Revise el detalle y apruebe una pendiente con inventario suficiente; compruebe que reservado aumenta y existencia se conserva. Rechace otra con motivo: no debe reservar inventario. Cada creación y decisión debe generar un solo evento con el usuario elegido. El rechazo sin motivo debe impedirse.
6. Abra la misma pendiente en dos ventanas. Resuelva desde una y trate de editar, cancelar o resolver desde la otra: debe rechazar la operación por cambio de versión, sin guardar cambios parciales.
7. Solicitar no reserva inventario. Aprobar sí reserva; no debe generar movimientos de salida o entrada. Envío y recepción son etapas posteriores fuera de este alcance. Intente aprobar otra solicitud cuya cantidad exceda existencia menos reservado: el trigger debe impedirlo y conservar estado PENDIENTE, sin reserva ni evento parcial.
8. En creación, compruebe que cambiar la tienda de origen limpia las líneas y actualiza la disponibilidad. Elegir un producto propone su precio actual. En edición, abrir una solicitud debe conservar su precio histórico hasta que se cambie el producto.
9. En creación, pulse Agregar sucursal y registre código, nombre, dirección y teléfono. Compruebe su aparición en ambos selectores y en dbo.Tienda al volver a abrir la pantalla, conservando las líneas y selecciones del traspaso. Código o nombre vacíos y código duplicado deben impedir el alta. Cerrar el diálogo no debe guardar. Si falla SQL Server, el formulario debe permanecer abierto y no confirmar el registro.

## Verificación realizada

Compilación correcta. Se ejecutaron diez comprobaciones de validación: tiendas iguales, solicitud vacía, cantidad cero, producto repetido, precio negativo, observaciones largas, edición sin versión, estado inválido, rechazo sin motivo y cancelación sin motivo. Las diez impidieron la operación antes de acceder a datos. Se intentó una consulta de solo lectura a SQL Server, pero la conexión falló con código 20. Las pruebas transaccionales y visuales siguen pendientes.

## Límites de integración

El inicio de sesión existente es una maqueta. La selección explícita de usuario atribuye eventos, pero no autentica ni autoriza al operador. Debe sustituirse por la identidad de sesión cuando se implemente autenticación y permisos por tienda/rol.

El índice único existente de EventoTraspaso permite un evento por estado. El historial registra creación y decisiones; las ediciones pendientes se protegen con rowversion, pero no producen eventos repetidos PENDIENTE.
