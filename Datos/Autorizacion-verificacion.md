# Acceso al contenido por roles y permisos

Al abrir **Roles → Pantallas y presets…** con permiso para gestionar seguridad, el sistema crea los permisos base que falten en la base existente: consulta y gestión para las 12 pantallas. El esquema de una base nueva ya incluye ese catálogo. También se puede preparar con `Migraciones/005_permisos_contenido.sql`. Ambas opciones son repetibles y conservan los permisos inactivos y las asignaciones, sin conceder accesos a cuentas existentes.

Un usuario con el rol activo `ADMINISTRADOR` tiene acceso total, tal como define ese rol en el esquema, y puede preparar las asignaciones desde **Usuarios, roles y permisos**. El nombre de rol mostrado en la interfaz no concede acceso: se verifica su código y su relación activa en SQL. Los demás roles solo conceden los permisos activos que tengan asignados mediante `RolPermiso` y `UsuarioRol`.

Cada módulo usa dos permisos:

- `CONSULTAR`: abrir sus pantallas, consultar sus datos y exportar contenido permitido.
- `GESTIONAR`: guardar, editar, desactivar, reactivar y ejecutar sus operaciones. Requiere también `CONSULTAR` del mismo módulo, aunque los permisos provengan de roles distintos.

Módulos: `VENTAS`, `INVENTARIO`, `COMPRAS`, `PROVEEDORES`, `TRASPASOS`, `VALIDACION_TRASPASOS`, `CARGA_MASIVA`, `PRODUCTOS`, `APARTADOS_DEVOLUCIONES`, `DIRECTORIO`, `SEGURIDAD`, `CONFIGURACION`.

`TRASPASOS.GESTIONAR` permite solicitar, editar, cancelar, enviar y recibir. Aprobar o rechazar requiere `VALIDACION_TRASPASOS.GESTIONAR` y su permiso de consulta. El directorio permite gestionar sus proveedores; los módulos de productos y compras solo consultan las referencias de proveedores que necesitan para su operación. Los selectores de tienda ofrecen identificador, nombre y estado activo a los módulos que los necesitan.

Las cuentas sin roles o sin permisos pueden iniciar sesión y ver Inicio sin información de negocio. Inicio consulta únicamente los indicadores autorizados; el gráfico y las tarjetas correspondientes se ocultan. Las notificaciones requieren `INVENTARIO.CONSULTAR`. El menú y las acciones de escritura se ocultan según los permisos. Abrir una pantalla y llamar directamente a los métodos de datos exige autorización, además de una sesión vigente. La autorización se consulta de nuevo en SQL para cada operación; no depende de la instantánea usada para presentar el menú. Cada minuto se actualiza el menú y se cierran las pantallas cuyo acceso se haya retirado. Con los triggers de seguridad instalados, los cambios de privilegios además invalidan la sesión.

El alcance de esta autorización es por módulo y por consulta/modificación. `UsuarioTienda` no se usa para filtrar filas por sucursal; los permisos de consulta de un módulo abarcan sus tiendas. Solo se debe otorgar un módulo cuando ese alcance sea apropiado.

## Selección por pantalla y presets

En **Usuarios, roles y permisos → Roles**, selecciona un rol existente o pulsa **Nuevo** y abre **Pantallas y presets…**. La lista muestra los nombres de las pantallas y explica sus acciones de consulta y gestión. `DIRECTORIO` incluye clientes, proveedores y empleados; `CONFIGURACION` incluye tiendas, facturación y auditoría. Esos contenidos comparten permiso dentro de su módulo.

La pantalla principal tiene únicamente las pestañas **Usuarios** y **Roles**. **Roles → Crear / editar permisos…** administra el catálogo compartido, incluyendo alta, edición con control de versión, desactivación con motivo, reactivación y búsqueda de inactivos. Guardar un permiso lo conserva en el catálogo. Al volver se prepara su asignación al rol sin borrar los datos ni las selecciones anteriores; guardar o actualizar el rol conserva esa asignación. No se asigna una gestión nueva sin consulta activa del mismo módulo. La opción requiere permiso para gestionar seguridad y las operaciones de datos vuelven a exigirlo.

Los presets definidos en `Clases/CatalogoPermisos.cs` son selecciones iniciales editables:

| Preset | Consulta | Gestión |
| --- | --- | --- |
| Administrador | Todos los 12 módulos registrados | Todos los 12 módulos registrados |
| Cajero | Ventas, apartados/devoluciones, productos e inventario | Ventas y apartados/devoluciones |
| Gerente | Todos salvo seguridad y configuración | Todos salvo seguridad y configuración |
| Analista de datos | Ventas, compras, inventario, productos, proveedores, traspasos, validación y apartados/devoluciones | Ninguno |

Elegir un preset en la lista solo muestra su descripción; **Aplicar preset** reemplaza la selección completa de todos los módulos. Después se pueden añadir o quitar permisos individuales y usar **Quitar todos** para empezar sin accesos. Se seleccionan únicamente permisos activos `CONSULTAR` y `GESTIONAR` de los módulos registrados. Nunca se asigna gestión sin consulta activa y se informa cuántos permisos del preset no están disponibles. Los módulos y acciones personalizados del catálogo siguen pudiendo seleccionarse manualmente.

**Aceptar** prepara el formulario y completa los campos vacíos de un rol nuevo, con un código disponible que no colisione con roles activos o inactivos. **Guardar** o **Actualizar** persiste los cambios usando la autorización y transacción existentes. Aplicar, aceptar o cancelar no crea roles automáticamente. Las asignaciones a usuarios se hacen después desde **Usuarios → Generar permisos…**. Abrir el selector o pulsar **Reiniciar** completa únicamente los registros faltantes de `Permiso`, en una transacción autorizada. Conserva las descripciones, los estados y las asignaciones existentes; tampoco asigna automáticamente permisos a roles o usuarios.

El preset Administrador propone el código `ADMIN_PERSONALIZADO` y el nombre Administrador personalizado para permitir ajustes de acceso. El rol reservado `ADMINISTRADOR` siempre tiene acceso total, aunque se retiren sus casillas; la ventana muestra esta condición al editarlo. Para restringir una cuenta se deben retirar sus roles más amplios, ya que los permisos de sus roles se acumulan.

## Verificación

`dotnet run --project Pruebas/FlujoVentas.Integracion` crea una base temporal, instala los scripts, comprueba los flujos existentes y los rechazos de acceso; elimina únicamente esa base al terminar. Requiere una conexión con permiso para crear bases de prueba. No ejecutar el esquema completo sobre una base existente.

La prueba de autorización comprueba denegación sin roles; permiso de gestión sin consulta; lectura sin escritura; permisos aportados por varios roles; roles, permisos y asignaciones inactivos; acceso administrativo real frente a etiquetas de rol falsas; consultas directas a módulos restringidos; filtrado del resumen; y sesión revocada. Las pruebas de interfaz verifican que una cuenta de consulta no vea acciones para guardar y que el menú no ofrezca módulos restringidos.

La regresión del catálogo vacío, en `Pruebas/FlujoVentas.Integracion/CatalogoPermisos.cs`, vacía únicamente el catálogo de la base temporal antes de los demás flujos. Sus 10 comprobaciones verifican la preparación automática, las 12 pantallas, la aplicación inmediata de Cajero, ausencia de asignaciones automáticas, idempotencia, instalación parcial y conservación de permisos inactivos con sus descripciones y relaciones. La suite completa también aprobó sus 44 comprobaciones de presets y las 43 de autorización, incluida la denegación de la preparación del catálogo a cuentas sin permiso de gestión.

El 9 de noviembre de 2026 se completaron 44 comprobaciones nuevas en `Pruebas/FlujoVentas.Integracion/PresetsPermisos.cs`: los cuatro perfiles, descripción de pantallas, selección previa a Aplicar, personalización, reemplazo de permisos, catálogo incompleto, consulta inactiva, catálogo vacío, advertencia del rol reservado, tamaño mínimo, cancelación, códigos disponibles, persistencia y autorización efectiva de los usuarios con cada perfil. La prueba confirmó que el analista consulta productos y no puede guardarlos con una llamada directa, y que el cajero no consulta compras ni modifica inventario. La suite completa pasó también sus 23 comprobaciones de permisos de usuarios, 23 de permisos por módulo, 25 de recarga, 42 de autorización y 103 de búsqueda global, además de los flujos de ventas. Se creó y eliminó una base temporal, sin modificar la base configurada.

Verificación realizada el 8 de noviembre de 2026: compilación sin errores ni advertencias; todos los flujos de integración existentes y 40 comprobaciones adicionales de autorización aprobados contra SQL Server en una base temporal eliminada al finalizar. El catálogo se instaló desde el esquema y se volvió a ejecutar la migración para comprobar que no duplica permisos.
