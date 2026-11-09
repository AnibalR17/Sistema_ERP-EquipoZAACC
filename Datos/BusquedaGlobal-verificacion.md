# Búsqueda global del MDI

La barra superior busca desde cualquier pantalla. Escribe al menos dos caracteres: la consulta comienza después de 400 ms sin escribir; **Enter** busca inmediatamente, **Ctrl+K** enfoca la barra y **Escape**, mientras está enfocada, limpia el texto y los resultados. Se admiten hasta 150 caracteres.

Los resultados aparecen en una pantalla del mismo MDI, conservando los módulos abiertos y sus cambios. Incluyen módulos y coincidencias en productos, categorías, proveedores, clientes, empleados, tiendas, compras, ventas, apartados, devoluciones, traspasos, existencias, usuarios, roles y permisos. Se busca en nombres, códigos, documentos, contactos y referencias según cada entidad. Las coincidencias ignoran mayúsculas y acentos. Se muestran hasta diez registros por tipo; afinar el texto permite encontrar otros.

**Ver detalle**, doble clic o Enter sobre un registro abre una consulta de solo lectura. Los documentos incluyen productos, pagos, abonos o historial según corresponda. **Abrir módulo** lleva a su pantalla habitual; los registros de tipo Módulo se abren directamente. Los detalles no muestran hashes, tokens ni versiones binarias.

El selector superior filtra documentos y existencias por tienda, empleados por su tienda y traspasos por origen o destino. Productos, categorías, clientes, proveedores y seguridad son catálogos compartidos. Esto conserva el alcance de permisos por módulo: no agrega restricciones por UsuarioTienda.

La búsqueda consulta los permisos vigentes en SQL antes de seleccionar las fuentes y antes de devolver resultados. Cada detalle y apertura de módulo se autoriza de nuevo. Las cuentas sin permisos no reciben contenido restringido. Al cambiar permisos se invalidan las coincidencias anteriores y se cierran los detalles cuyo permiso se haya retirado. Las consultas usan parámetros y CHARINDEX para tratar como texto literal apóstrofos, corchetes, porcentajes y guiones bajos. No se requieren nuevas tablas ni migraciones.

Cambiar el texto cancela la consulta anterior; un número de versión impide que una respuesta atrasada sobrescriba los resultados actuales. Cambiar la tienda invalida los resultados y repite la búsqueda si están visibles. Vaciar la barra retira las coincidencias; los errores aparecen en la pantalla de búsqueda y Enter permite reintentar.

## Verificación

Ejecutar `dotnet run --project Pruebas/FlujoVentas.Integracion`. La prueba utiliza únicamente una base temporal y la elimina al terminar. Comprueba todas las entidades, sus detalles, límites, acentos, texto literal, cancelación, filtro por tienda, cuentas sin permisos, permisos retirados, resultados atrasados, apertura de módulos, navegación MDI y atajos de teclado.

Verificación realizada el 8 de noviembre de 2026: compilación sin errores ni advertencias; 87 comprobaciones de búsqueda global, las 40 comprobaciones de autorización y todos los flujos de integración existentes aprobados contra SQL Server. La base temporal se eliminó al finalizar.
