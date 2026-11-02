# Inventario, movimientos y traspasos

## Preparacion

Usar una base de pruebas con el esquema 3FN instalado. Base de Datos/01_esquema_erp_3fn.sql es para una base vacia; no ejecutarlo sobre una base con datos. Instalar Base de Datos/Triggers2.sql en la misma base configurada para la aplicacion. La aplicacion no instala ni modifica el esquema.

Se requieren los triggers de movimientos (validar, aplicar e inmutable), los de estados y operacion de traspasos y sus dependencias. Configurar nested triggers habilitado y RECURSIVE_TRIGGERS deshabilitado, como indica el script. No deshabilitar triggers para eludir errores de disponibilidad.

Iniciar sesion con una cuenta activa ligada a un empleado activo. Preparar dos tiendas activas y productos activos. Para cargar existencias de prueba puede registrarse un ajuste positivo con motivo desde Inventario. Se pueden consultar productos sin saldo y registrar su primer movimiento sin crear InventarioTienda manualmente.

## Comportamiento

- Inventario y movimientos abre su pantalla; Compras conserva una opcion independiente.
- Existencias muestra saldo fisico, reservado, disponible y minimo por tienda. Incluye productos activos sin saldo y productos inactivos con una fila de inventario. Las fechas no afectan al saldo actual.
- Movimientos muestra el libro por tienda, texto y fechas UTC, con 30 dias iniciales. Consultar o Enter ejecuta los filtros. El texto se interpreta literalmente mediante parametros y CHARINDEX.
- Ajustar solicita unidades positivas o negativas y motivo obligatorio de hasta 500 caracteres. El trigger aplica el movimiento al saldo; el codigo no actualiza existencia directamente.
- Guardar minimo cambia solo el umbral, con comprobacion de version. No registra entradas o salidas de unidades.
- Revertir ajuste inserta un REVERSO exclusivamente para un AJUSTE manual no revertido. Nunca borra ni modifica el asiento original ni revierte documentos de compra, venta o traspaso.
- Solicitar crea PENDIENTE sin reservar. Aprobar reserva en origen sin cambiar existencia. Enviar pasa APROBADO a EN_TRANSITO, libera reserva y genera TRASPASO_SALIDA. Recibir pasa EN_TRANSITO a RECIBIDO y genera TRASPASO_ENTRADA en destino.
- Rechazar una pendiente requiere motivo. Cancelar una pendiente no toca existencias; cancelar una aprobada libera reservas. No se cancela un envio ni se recibe dos veces.
- Los triggers registran historial, movimientos y notificaciones dentro de la misma transaccion. No se insertan eventos ni movimientos de traspaso desde C#.
- El operador es el usuario autenticado. Cada escritura verifica la sesion en SQL Server dentro de su transaccion; una sesion vencida, revocada, con otra version o con usuario/empleado inactivo no puede escribir. Ya no hay selector manual de usuarios.
- Las solicitudes y los saldos usan rowversion para impedir sobrescrituras de cambios concurrentes. Ante conflicto, actualizar el listado antes de reintentar.

## Pruebas funcionales con SQL Server pendientes

1. Abrir Inventario y movimientos y Compras: comprobar que llevan a pantallas distintas. Seleccionar tienda, buscar producto con apostrofos, corchetes, porcentaje y asterisco; verificar busqueda literal, tablas y conteos. Cambiar fechas solo modifica Movimientos.
2. Producto sin saldo: registrar AJUSTE de +20 con motivo. Debe crearse InventarioTienda con existencia 20, reservado 0 y disponible 20, y un solo movimiento. Registrar -3: saldo 17. Cancelar el dialogo no guarda. Cero, valores fuera de rango y motivo vacio deben impedir guardar.
3. Cambiar minimo y comprobar persistencia sin generar movimientos de unidades. Modificar saldo o minimo desde otra instancia antes de guardar: debe informar conflicto. Esto incluye la creacion concurrente de un saldo previamente inexistente.
4. Revertir un ajuste: verificar movimiento opuesto, referencia al original y marca Revertido. Un segundo intento debe fallar. Intentar revertir una salida cuya compensacion deje saldo menor que reservado: debe revertirse toda la operacion, sin asiento parcial.
5. Crear solicitud con productos y tiendas distintas: comprobar PENDIENTE, solicitante autenticado y evento unico. Nuevo guardado debe limpiar campos y mostrar el numero. Intentar productos repetidos, cantidad cero, precio negativo, campos vacios y tiendas iguales.
6. En Validacion revisar detalle e historial y editar una pendiente. Las tiendas y el solicitante no se cambian. Verificar precios historicos al abrir edicion y precio actual al cambiar de producto.
7. Aprobar con disponibilidad: aumenta reservado y conserva existencia. Aprobar sin disponible falla sin cambios parciales. Rechazar exige motivo y no reserva.
8. Enviar un aprobado: existencia y reservado en origen disminuyen por la cantidad, destino no cambia, estado EN_TRANSITO y un movimiento de salida por detalle. Recibir: aumenta existencia del destino, estado RECIBIDO, un movimiento de entrada por detalle. Actualizar filtro de estado para localizar cada etapa.
9. Cancelar pendiente y cancelar aprobado con motivo: verificar que solo el segundo libera reserva. Acciones de enviar, recibir, editar, aprobar y cancelar deben estar habilitadas unicamente para los estados aplicables. Reintentar envio o recepcion desde una ventana antigua debe fallar por version o estado, sin movimientos repetidos.
10. Abrir la misma solicitud en dos instancias. Resolver en una y editar o resolver desde la otra: debe informar conflicto. Comprobar que la transaccion revierte cabecera, detalles, inventario e historial ante cualquier error de trigger.
11. Desactivar tienda o producto despues de abrir el formulario de ajuste: guardar debe fallar. Los saldos historicos siguen consultables. La reversion de ajustes historicos conserva sus referencias, aunque el producto haya sido desactivado.
12. Revocar/expirar la sesion o desactivar al usuario/empleado antes de confirmar una operacion: debe rechazarse. No debe poder atribuirse un movimiento a otra cuenta ni operar sin sesion.
13. Crear una sucursal desde la solicitud y verificar persistencia y actualizacion de selectores sin perder lineas o selecciones. Codigo duplicado o campos obligatorios vacios impiden guardarla.
14. Deshabilitar un trigger requerido en la base de pruebas: las escrituras deben mostrar que falta Triggers2.sql. Perder conexion no debe cerrar el programa ni confirmar una operacion fallida. Revisar tambien el desplazamiento y las acciones al reducir la ventana.

## Verificacion realizada

Compilacion sin errores ni advertencias y git diff --check sin errores de espacios. Un ejecutable temporal realizo 41 comprobaciones de construccion de controles, estados de botones para las seis etapas, validaciones de ajustes/minimos/reversiones/fechas/traspasos y exigencia de sesion antes de enviar: todas aprobadas.

Se intento una conexion de solo lectura a la base configurada para verificar los seis triggers principales. Fallo con codigo SQL 20. No se escribieron datos ni se instalaron scripts; siguen pendientes las pruebas transaccionales y visuales con una conexion operativa.

## Limites

La autorizacion granular por permiso y tienda todavia no se aplica; las operaciones se atribuyen a la identidad autenticada y requieren una sesion vigente. No se implementa recepcion parcial ni diferencias de cantidades: cada envio y recepcion procesa todas las lineas del documento.

El indice unico de EventoTraspaso permite un evento por solicitud y estado. Las ediciones pendientes se protegen con rowversion, pero no generan eventos PENDIENTE adicionales. Las fechas se muestran y filtran en UTC para coincidir con el almacenamiento de SQL Server.
