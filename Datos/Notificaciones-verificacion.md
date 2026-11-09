# Verificacion de notificaciones de inventario

## Preparacion y criterio

Usar una base de pruebas con el esquema 3FN y la conexion descrita en README. Iniciar sesion con usuario y empleado activos. Preparar tiendas, productos e inventario mediante los flujos documentales existentes; no registrar ajustes manuales ni instalar scripts desde la aplicacion.

Una alerta corresponde a un par producto/tienda activo cuyo **disponible (existencia menos reservado) es igual o inferior al stock minimo**. Se consultan las filas actuales de InventarioTienda; un producto sin fila en esa tienda no genera una alerta. Una fila con disponible 0 y minimo 0 si cumple el criterio. Se muestran existencia, reservado, disponible y minimo para distinguir las cantidades.

La campana consulta al iniciar la ventana autenticada y cada 60 segundos. Abrirla o pulsar Actualizar vuelve a consultar. El contador usa 9+ cuando hay mas de nueve pares; el texto de ayuda conserva el numero completo. Un signo ? indica consulta pendiente o fallida y se distingue de un resultado correcto de cero alertas.

## Verificacion local realizada

Un ejecutable temporal sobre .NET 10 para Windows realizo **47 comprobaciones aprobadas**, sin SQL ni lectura de credenciales. Se verificaron constructor sin consultas; condicion visible; siete columnas explicitas de solo lectura; cargas con resultados, vacias y fallidas; reintento; limpieza de datos obsoletos ante errores; exclusion de cargas simultaneas; cierre durante una consulta pendiente; contador, texto accesible y ayuda de la campana; espera compartida de consultas; temporizador y rechazo de consulta sin sesion antes de preparar la conexion.

La comprobacion visual detecto que el contador 9+ se cortaba a 54 x 30. Se aumento el ancho a 64 y se confirmo su representacion completa en bitmap. Se verifico la ausencia de solapes del encabezado a 1050 x 720. La revision visual del dialogo a 940 x 560 y 720 x 440 comprobo acciones visibles y desplazamiento de la tabla. El ejecutable temporal compilo sin errores ni advertencias.

Estas comprobaciones usan cargadores y tablas simulados. No ejecutaron el predicado SQL, transacciones de lectura, triggers, cambios de inventario ni consultas automaticas contra una base real. Los casos siguientes siguen pendientes de integracion.

## Casos contra SQL Server pendientes

1. Producto de una tienda activa con minimo 10: disponible 11 no alerta; 10 y 9 si alertan. Comprobar igualdad y ambas caras del limite despues de Consultar/Actualizar.
2. Reservar unidades mediante un traspaso aprobado: existencia 12, reservado 4 y minimo 10 debe alertar por disponible 8. Cancelar la reserva y actualizar debe retirar el aviso si el disponible vuelve a 12.
3. Guardar minimo para un producto sin saldo y comprobar la nueva fila de inventario. Disponible 0 con minimo 0 debe alertar; un producto sin fila no se agrega artificialmente a todas las tiendas.
4. Desactivar el producto o la tienda y actualizar: el par sale de las alertas aunque conserve historial. Reactivar y volver a consultar debe aplicar nuevamente el criterio.
5. El mismo producto bajo minimo en dos tiendas cuenta como dos alertas, con tienda y cantidades correctas. Probar mas de nueve pares y comprobar 9+ junto al numero completo en la ayuda del boton.
6. Confirmar una compra para reponer unidades y actualizar: la alerta desaparece cuando el disponible supera el minimo. Un borrador no cambia la alerta. Comprobar tambien movimientos y reservas de otros documentos mediante sus flujos disponibles.
7. Reponer un producto que tenga eventos de notificacion antiguos: no debe mantenerse una alerta resuelta por leer mensajes historicos. Leer o actualizar las alertas no modifica EventoNotificacion, Notificacion ni inventario.
8. Perder conexion durante la consulta: la campana muestra ? y el dialogo muestra el error, sin presentar cero alertas como resultado correcto ni conservar filas obsoletas. Restablecer conexion y Actualizar debe recuperar el contador.
9. Revocar o expirar la sesion, cambiar su version o desactivar al usuario/empleado antes de consultar: rechazar la consulta. Verificar el retorno al acceso mediante el control de sesion existente.
10. Mantener abierta la ventana principal para comprobar refresco cada 60 segundos, abrir el dialogo durante una consulta y cerrarlo antes de recibir respuesta: no duplicar consultas ni actualizar controles destruidos. Revisar teclado, escalado y desplazamiento con datos reales.

## Limites

Es una consulta de alertas vigentes, no una bandeja de mensajes leidos/no leidos ni un despachador de EventoNotificacion. La consulta exige INVENTARIO.CONSULTAR y una sesion vigente; conserva el alcance general de tiendas del modulo. Los eventos historicos de los triggers se conservan y no se asignan destinatarios automaticamente desde este flujo.
