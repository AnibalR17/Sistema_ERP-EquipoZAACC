# Acceso al contenido por roles y permisos

Aplicar `Migraciones/005_permisos_contenido.sql` sobre la base existente; el esquema de una base nueva ya incluye el catálogo. El script es repetible y conserva permisos inactivos y asignaciones. No concede permisos a cuentas existentes.

Un usuario con el rol activo `ADMINISTRADOR` tiene acceso total, tal como define ese rol en el esquema, y puede preparar las asignaciones desde **Usuarios, roles y permisos**. El nombre de rol mostrado en la interfaz no concede acceso: se verifica su código y su relación activa en SQL. Los demás roles solo conceden los permisos activos que tengan asignados mediante `RolPermiso` y `UsuarioRol`.

Cada módulo usa dos permisos:

- `CONSULTAR`: abrir sus pantallas, consultar sus datos y exportar contenido permitido.
- `GESTIONAR`: guardar, editar, desactivar, reactivar y ejecutar sus operaciones. Requiere también `CONSULTAR` del mismo módulo, aunque los permisos provengan de roles distintos.

Módulos: `VENTAS`, `INVENTARIO`, `COMPRAS`, `PROVEEDORES`, `TRASPASOS`, `VALIDACION_TRASPASOS`, `CARGA_MASIVA`, `PRODUCTOS`, `APARTADOS_DEVOLUCIONES`, `DIRECTORIO`, `SEGURIDAD`, `CONFIGURACION`.

`TRASPASOS.GESTIONAR` permite solicitar, editar, cancelar, enviar y recibir. Aprobar o rechazar requiere `VALIDACION_TRASPASOS.GESTIONAR` y su permiso de consulta. El directorio permite gestionar sus proveedores; los módulos de productos y compras solo consultan las referencias de proveedores que necesitan para su operación. Los selectores de tienda ofrecen identificador, nombre y estado activo a los módulos que los necesitan.

Las cuentas sin roles o sin permisos pueden iniciar sesión y ver Inicio sin información de negocio. Inicio consulta únicamente los indicadores autorizados; el gráfico y las tarjetas correspondientes se ocultan. Las notificaciones requieren `INVENTARIO.CONSULTAR`. El menú y las acciones de escritura se ocultan según los permisos. Abrir una pantalla y llamar directamente a los métodos de datos exige autorización, además de una sesión vigente. La autorización se consulta de nuevo en SQL para cada operación; no depende de la instantánea usada para presentar el menú. Cada minuto se actualiza el menú y se cierran las pantallas cuyo acceso se haya retirado. Con los triggers de seguridad instalados, los cambios de privilegios además invalidan la sesión.

El alcance de esta autorización es por módulo y por consulta/modificación. `UsuarioTienda` no se usa para filtrar filas por sucursal; los permisos de consulta de un módulo abarcan sus tiendas. Solo se debe otorgar un módulo cuando ese alcance sea apropiado.

## Verificación

`dotnet run --project Pruebas/FlujoVentas.Integracion` crea una base temporal, instala los scripts, comprueba los flujos existentes y los rechazos de acceso; elimina únicamente esa base al terminar. Requiere una conexión con permiso para crear bases de prueba. No ejecutar el esquema completo sobre una base existente.

La prueba de autorización comprueba denegación sin roles; permiso de gestión sin consulta; lectura sin escritura; permisos aportados por varios roles; roles, permisos y asignaciones inactivos; acceso administrativo real frente a etiquetas de rol falsas; consultas directas a módulos restringidos; filtrado del resumen; y sesión revocada. Las pruebas de interfaz verifican que una cuenta de consulta no vea acciones para guardar y que el menú no ofrezca módulos restringidos.

Verificación realizada el 8 de noviembre de 2026: compilación sin errores ni advertencias; todos los flujos de integración existentes y 40 comprobaciones adicionales de autorización aprobados contra SQL Server en una base temporal eliminada al finalizar. El catálogo se instaló desde el esquema y se volvió a ejecutar la migración para comprobar que no duplica permisos.
