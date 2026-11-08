# Bitácora de toda la aplicación

La captura se realiza en SQL Server para las 37 tablas operativas, además del registro de navegación de todas las pantallas desde `Pagina_Central.MostrarPantalla` y la apertura de Notificaciones. Cada conexión de la aplicación establece el usuario actual y limpia el motivo anterior. El cambio inicial de contraseña y el cierre de sesión establecen explícitamente el usuario del acceso, incluso cuando la sesión global todavía no existe o ya se ha limpiado.

## Cobertura

| Área | Entidades auditadas |
| --- | --- |
| Productos | Categoria, Producto |
| Proveedores y directorio | Proveedor, Cliente, Empleado |
| Configuración | Tienda, MetodoPago |
| Configuración de facturación | ConfiguracionFacturacion (CAI por tienda) |
| Seguridad | Usuario, Rol, Permiso, UsuarioRol, RolPermiso, UsuarioTienda, SesionUsuario, EventoAcceso, RecuperacionAcceso |
| Compras | Compra, DetalleCompra |
| Ventas | Venta, DetalleVenta, PagoVenta |
| Inventario | InventarioTienda, MovimientoInventario |
| Traspasos | Traspaso, DetalleTraspaso, EventoTraspaso |
| Apartados | Apartado, DetalleApartado, AbonoApartado |
| Devoluciones | Devolucion, DetalleDevolucion, Reembolso |
| Inversiones (legado) | Inversion |
| Importaciones | Importacion, DetalleImportacion |
| Notificaciones | EventoNotificacion, Notificacion |

La migración `Migraciones/002_bitacora_sistema.sql` clasifica las nuevas entradas por área y conserva los seis triggers de auditoría existentes. La consulta de auditoría clasifica también las cabeceras históricas guardadas como DOCUMENTOS y CATALOGO. No modifica eventos anteriores, ya que Auditoria es inmutable. El administrador que modifica otra cuenta permanece como responsable; el usuario afectado se identifica mediante la clave del registro.

Configuración y auditoría recarga los eventos al volver desde otro módulo y después de modificar una tienda. Mantiene la búsqueda y el desplazamiento local. Apartados, devoluciones e importación registran sus operaciones de guardado, pagos, movimientos e historial. La sección de control de inversiones fue retirada de la interfaz; la tabla histórica Inversion sigue incluida en la cobertura de auditoría. No se reconstruyen cambios anteriores a la instalación de los triggers.

## Prueba integral ejecutada

Ejecutar `Pruebas/Bitacora-sistema.sql` en la base configurada después del esquema, Triggers2.sql y la migración 002. Requiere un usuario activo y permisos para las operaciones de prueba. Desde PowerShell, leer el SQL con `Get-Content -Raw -Encoding UTF8`.

La prueba crea sus propios catálogos y documentos dentro de una transacción, confirma una compra y una venta, recorre un traspaso hasta recibido, registra un apartado con abono, una devolución con reembolso, inversiones, importaciones y notificaciones. Comprueba modificaciones y eliminaciones fuera de ventas, los cuatro resultados de acceso, revocación de sesiones y atribución al administrador. Exige un evento real con el actor correcto para cada tabla operativa y comprueba que no se copie la credencial de prueba en los JSON. También comprueba que todos los triggers de captura estén habilitados.

Resultado local: **37 de 37 entidades con eventos verificados**. La transacción se revirtió completa; no quedan registros de negocio de prueba. SQL Server puede consumir identidades aunque la transacción se revierta. Compilación .NET: cero errores y cero advertencias.

Después de incorporar el CAI se amplió y ejecutó de nuevo la prueba: **38 de 38 entidades con eventos verificados**, incluida ConfiguracionFacturacion. Se revirtió completa.
