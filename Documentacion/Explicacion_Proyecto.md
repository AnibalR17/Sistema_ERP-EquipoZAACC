# 1. Propósito y alcance del sistema

Sistema ERP — Equipo ZAACC es una aplicación de escritorio para administrar la operación comercial de varias tiendas. Integra productos, categorías, proveedores, clientes, empleados, compras, ventas, inventario, traspasos, apartados y devoluciones. Centraliza la información en SQL Server y conserva el historial de operaciones mediante documentos, movimientos de inventario y una bitácora.

El recorrido comercial principal es registrar catálogos, comprar productos para una tienda, confirmar su entrada al inventario y venderlos. Cuando una sucursal necesita mercancía de otra, se utiliza un traspaso. Cuando un cliente reserva productos, se utiliza un apartado con abonos y posterior entrega. Cuando devuelve una compra, se registra una devolución y, si corresponde, un reembolso.

Este manual describe el código disponible en el espacio de trabajo al 8 de noviembre de 2026, incluidos cambios locales todavía no confirmados en Git. Explica el comportamiento implementado, el modelo de datos y las apariciones SQL identificadas en C# y en los archivos .sql. No incluye contraseñas privadas ni la cadena de conexión del equipo.

Aunque el nombre es ERP, el alcance observado corresponde principalmente a gestión comercial e inventario. No se identificaron módulos completos de contabilidad general, nómina, cuentas por cobrar, cuentas por pagar, comercio electrónico o conciliación bancaria. Inversion y RecuperacionAcceso existen como estructuras de datos, pero no cuentan con un flujo de pantalla completo identificado en esta revisión.

## 1.1 Qué resuelve cada módulo

| Módulo | Qué permite hacer | Resultado principal |
|---|---|---|
| Acceso | Autenticar usuarios, cambiar claves temporales y cerrar sesión | Sesión con vencimiento y registro del intento |
| Inicio | Consultar ventas, compras, traspasos pendientes y alertas | Resumen del día y gráfico de siete días |
| Catálogo | Mantener productos y categorías, consultar stock y exportar | Maestro de artículos y Excel de la vista actual |
| Proveedores y directorio | Mantener empresas, clientes y empleados | Entidades relacionadas con las operaciones |
| Compras | Crear borradores, confirmar y anular | Entrada de unidades al confirmar y reverso si procede |
| Punto de venta | Seleccionar tienda, cliente, artículos, impuesto y pago | Factura, cobro y salida de inventario |
| Inventario | Consultar existencias y movimientos, cambiar mínimos | Saldo por tienda y trazabilidad de unidades |
| Traspasos | Solicitar, aprobar, enviar y recibir mercancía | Reserva y movimiento entre dos sucursales |
| Apartados | Reservar, abonar, entregar, cancelar o vencer | Compromiso de stock, cobros y factura de entrega |
| Devoluciones | Registrar artículos devueltos, confirmar y reembolsar | Reintegro opcional de stock y devolución de dinero |
| Carga masiva | Validar e importar productos desde Excel | Lote de productos y stock inicial por tienda |
| Usuarios, roles y permisos | Mantener cuentas y asignaciones | Estructura administrativa de seguridad |
| Configuración y auditoría | Mantener tiendas y CAI, consultar bitácora | Parámetros por sucursal y seguimiento |

## 1.2 Autenticación y permisos

El sistema comprueba la identidad y la vigencia de la sesión en los flujos que invocan SesionActual.Validar. También mantiene usuarios, roles y permisos y puede invalidar sesiones al cambiar asignaciones. Sin embargo, la autorización granular por módulo, acción y tienda todavía no restringe de forma general la navegación y las operaciones. La existencia de RolPermiso o UsuarioTienda no significa que cada botón ya aplique esos permisos.

# 2. Arquitectura, tecnologías y organización

## 2.1 Tecnologías utilizadas

| Elemento | Implementación observada | Función |
|---|---|---|
| Lenguaje | C# | Interfaz, validaciones y coordinación de operaciones |
| Plataforma | .NET 10, net10.0-windows | Ejecución en Windows |
| Interfaz | Windows Forms | Formularios, tablas, diálogos y ventana principal MDI |
| Persistencia | SQL Server y T-SQL | Datos, restricciones, vistas, procedimientos y triggers |
| Acceso SQL | Microsoft.Data.SqlClient 7.1.1 | Conexiones, parámetros, lectores y adaptadores |
| Excel | ClosedXML 0.105.1 | Lectura .xlsx, plantillas y exportación |
| PDF | PDFsharp-GDI 6.2.4 | Recibos PDF con varias páginas |
| Claves | PBKDF2 con SHA-256 | Derivación y verificación de hashes |
| Solución | Sistema_ERP.slnx y Sistema_ERP.csproj | Organización y compilación del ejecutable WinExe |

El acceso a datos es directo mediante ADO.NET. No se observa Entity Framework, una API HTTP intermedia ni un servidor web propio. La aplicación Windows se comunica con SQL Server mediante una cadena de conexión configurable.

## 2.2 Cómo circula una operación

```text
Operador
   ↓
Formulario WinForms: captura, selección y mensajes
   ↓
Clases y Datos: validación, sesión y transacción
   ↓
SqlCommand con parámetros y SESSION_CONTEXT
   ↓
SQL Server: tablas + restricciones + vistas + triggers
   ↓
Resultado, comprobante o lista actualizada
```

Por ejemplo, C# confirma una compra cambiando Compra.estado. El trigger de contabilización crea movimientos COMPRA y el trigger de movimientos actualiza InventarioTienda. Si alguna regla falla, la transacción completa se revierte. Una parte importante de las reglas de negocio reside en SQL Server.

## 2.3 Responsabilidad de las carpetas

| Ruta | Contenido y responsabilidad |
|---|---|
| Program.cs | Login, apertura de la ventana principal y retorno al acceso |
| Formularios/Acceso | FrmLogin y diseñador |
| Formularios/Principal | Pagina_Central, resumen, tienda y campana |
| Formularios/Productos | Catálogo, categorías, productos y carga masiva |
| Formularios/Proveedores y Formularios/Cliente | Pantallas especializadas de proveedores y clientes |
| Formularios/Compras | Registro, detalle y estados de compra |
| Formularios/Inventario | Existencias, movimientos, solicitudes y validación de traspasos |
| Formularios/Ventas | PuntoVenta, ApartadosDevoluciones y editores |
| Formularios/Administracion | Directorio, seguridad, tiendas, CAI y bitácora |
| Formularios/Dialogos | Confirmaciones, alertas y motivos de desactivación |
| Formularios/Base | PantallaMaqueta y DialogoMaqueta, bases reutilizables |
| Datos | Conexión, operaciones por entidad y utilidades de exportación |
| Clases | Modelos, sesión, hashes, Ingresar, Modificar y Borrar |
| Presentacion | Estilos, diseño y diálogos compartidos |
| Controles | PanelDisenoInventario para superficies y tarjetas |
| Base de Datos | Esquema inicial, triggers, demostración y utilidad de reinicio |
| Datos/Migraciones y Datos/Pruebas | Evolución del esquema y comprobación de bitácora |
| Datos/Plantillas | Plantilla Excel, ejemplo de papelería, instrucciones y PDF de ejemplo |
| Pruebas/FlujoVentas.Integracion | Ejecutable de pruebas con una base temporal |

Los .Designer.cs crean y configuran controles. Los .Presentacion.cs completan la apariencia mediante clases partial. Los .resx contienen recursos. Los namespaces Sistema_ERP, Sistema_ERP.Vistas y LoginERP conviven por la organización histórica; no son programas independientes.

El nombre PantallaMaqueta no significa por sí mismo que una pantalla carezca de funciones: PuntoVenta y ApartadosDevoluciones heredan de ella y ejecutan operaciones reales.

## 2.4 Clases compartidas

- Conexion abre conexiones, configura el contexto del usuario y limpia el motivo anterior.
- SesionActual conserva la identidad y valida su sesión contra SQL Server.
- ClaveSegura crea y verifica hashes de contraseñas.
- Ingresar concentra algunas altas y delega el registro de traspasos.
- Modificar actualiza catálogos, reactiva registros y resuelve traspasos.
- Borrar realiza desactivaciones lógicas; conserva las filas.
- FlujoVentaDatos comparte comandos parametrizados, validación de importes, pagos y dependencias de apartados y devoluciones.
- ImpuestosVenta calcula ISV y valida sus tasas.
- ExportacionArchivos y DialogosExportacion separan la generación de archivos de la elección de su destino.
- ModoDiseno evita consultas de negocio al usar el diseñador de formularios.

# 3. Arranque, navegación e Inicio

## 3.1 Desde la ejecución hasta la ventana principal

Program.Main inicializa Windows Forms y abre FrmLogin. Si el usuario cancela o no obtiene un AccesoUsuario válido, termina. Cuando la autenticación está completa, guarda la identidad en SesionActual.Usuario y abre Pagina_Central. Al cerrar la ventana principal, limpia la identidad e intenta revocar la sesión en la base.

Cerrar sesión vuelve al login dentro del ciclo principal; cerrar normalmente la aplicación termina el proceso. Si falla la revocación por conexión, se informa y la sesión conserva su vencimiento registrado.

La ventana principal guarda formularios por clave en un diccionario y reutiliza la pantalla abierta al cambiar de módulo. Asocia formularios y botones para mostrar la navegación seleccionada. La apertura de módulos se registra mediante AuditoriaDatos.RegistrarNavegacion.

El encabezado muestra usuario, roles, selector de tienda y campana. La tienda elegida se utiliza expresamente en Inicio y Catálogo. Otros módulos mantienen sus propios selectores o alcances: no hay un filtro universal aplicado a todas las consultas. El buscador global visual no tiene una búsqueda integral implementada identificada.

## 3.2 Qué muestra Inicio

InicioDatos.Obtener genera siete días, incluidos los que no tienen operaciones. Agrupa ventas CONFIRMADAS y compras CONFIRMADAS mediante sus vistas de total y cuenta documentos. También obtiene traspasos PENDIENTES y pares producto/tienda con disponible igual o inferior al mínimo.

Las ventas se guardan en UTC y se convierten a fecha de Honduras restando seis horas. Las compras se agrupan por fecha_factura, una fecha de negocio. Los límites UTC se construyen a partir del día local para evitar mezclar ventas de dos días.

Inicio se actualiza al entrar, regresar, cambiar de tienda, pulsar Actualizar resumen y cada 60 segundos mientras está activo. La consulta ocurre fuera del hilo de interfaz y se evita mostrar resultados de un filtro anterior. Un fallo no se presenta como importes cero: se indica que no se cargaron los datos.

Probar conexión verifica el acceso configurado sin instalar scripts. Las tarjetas abren módulos relacionados y el gráfico compara ventas y compras de los últimos siete días.

# 4. Contraseñas, sesiones y administración de seguridad

## 4.1 Autenticación y bloqueo

AccesoDatos.Ingresar busca la cuenta por nombre_usuario_normalizado y verifica el hash. Exige usuario y empleado activos y cuenta no bloqueada. Registra cada intento en EventoAcceso con resultado EXITOSO, INVALIDO, BLOQUEADO o INACTIVO y el equipo.

Cinco fallos provocan bloqueo de 15 minutos. Una clave correcta durante el bloqueo tampoco abre sesión. Al vencer el plazo se vuelve a evaluar el acceso; una entrada completa reinicia el contador. La consulta y la creación inicial usan una transacción Serializable. Para usuarios inexistentes se verifica un hash simulado, evitando omitir todo el trabajo criptográfico.

## 4.2 Almacenamiento de contraseñas

ClaveSegura usa PBKDF2-SHA256 con 600 000 iteraciones, sal aleatoria de 16 bytes y resultado de 32 bytes. Guarda algoritmo, iteraciones, sal y hash en Base64 y verifica mediante FixedTimeEquals. No almacena claves en texto plano.

Las nuevas claves deben tener entre 12 y 128 caracteres. El cambio temporal exige una contraseña diferente y compara version_fila para rechazar una cuenta modificada desde otra ventana.

## 4.3 Administrador inicial

Si Usuario está completamente vacía, la aplicación prepara admin con contraseña temporal admin y rol ADMINISTRADOR. Reutiliza o crea el empleado ADMIN-INICIAL y una tienda activa; crea INICIAL si hace falta. Exige cambiar la contraseña antes de abrir el sistema.

La inicialización no ocurre si ya hay usuarios, aunque estén inactivos, ni reactiva automáticamente registros iniciales incompatibles. Los usuarios de poblar_bdd.sql tienen hashes ficticios: no sirven para entrar. Cargar esa demostración elimina la condición de Usuario vacía y requiere preparar credenciales administradas de prueba.

## 4.4 Vigencia y revocación

SesionUsuario conserva la sesión durante ocho horas, hash de token y copia de version_seguridad. SesionActual.Validar exige una sesión existente, no revocada, no vencida, con versión coincidente, usuario y empleado activos y sin cambio de contraseña pendiente.

Pagina_Central revisa la vigencia cada minuto. Los triggers pueden revocar sesiones por cambios de cuenta o asignaciones mediante usp_InvalidarSesiones. Si la revisión falla por conexión, solicita iniciar sesión de nuevo.

Las operaciones transaccionales validan dentro de su conexión y transacción. Algunos CRUD antiguos no llaman esta validación de forma uniforme: la autenticación del programa y SESSION_CONTEXT no equivalen por sí solos a una comprobación completa en cada método.

## 4.5 Usuarios, roles y permisos

UsuariosRolesPermisos mantiene cuentas ligadas a empleados; roles con código, nombre y descripción; y permisos por módulo/acción. UsuarioRol y RolPermiso retienen asignaciones retiradas con activo=0.

Guardar un usuario sin nueva clave mantiene su hash. Proporcionar una nueva genera otro hash y obliga al cambio. Registro y asignaciones se guardan juntos en una transacción. La edición y los cambios de estado usan rowversion.

Desactivar un rol o permiso conserva sus asignaciones. Reactivarlo puede devolver efecto a esas relaciones cuando se aplique su autorización. UsuarioTienda y RecuperacionAcceso están en el modelo, pero esta pantalla no los administra. La autorización granular por permiso y sucursal sigue pendiente de aplicación general.

# 5. Productos, proveedores, directorio y tiendas

## 5.1 Productos y categorías

Producto tiene código de barras único, nombre, precio base, categoría y proveedor. El código es texto para conservar ceros iniciales. El precio es positivo y tiene dos decimales. El stock pertenece a InventarioTienda y sus movimientos, no a un campo editable de la ficha.

Catalogo_Productos permite buscar, agregar, editar, desactivar y reactivar. Consulta vw_ProductoStock para el total físico o el saldo de una tienda. Para vender se usa disponible, restando reservas. Incluye mantenimiento de categorías y exportación de la vista filtrada.

Categoria tiene nombre único y descripción opcional. La aplicación revisa duplicados y SQL los protege. Un código o nombre de un registro inactivo sigue reservado por su restricción única.

Nuevo_Producto exige código, nombre, precio positivo, categoría y proveedor. ProductoDatos y las clases compartidas persisten los cambios. PantallaProductos es otra pantalla existente de listado y desactivación; la navegación habitual utiliza el catálogo.

## 5.2 Proveedores y clientes

Proveedor contiene empresa, RTN, contacto, teléfono, correo y dirección. Valida longitudes, correo cuando se proporciona y caracteres ASCII en su RTN. No exige el mismo formato de 14 dígitos aplicado al RTN opcional de factura.

Hay una pantalla especializada y otra variante en DirectorioRoles. La importación resuelve proveedores por RTN sin guiones ni espacios. Si dos proveedores activos comparten ese RTN normalizado, exige corregirlo antes de importar.

Cliente guarda nombre, contacto, documento fiscal y dirección en el esquema. PantallaClientes y ClienteDatos mantienen una variante especializada; el directorio ofrece otra captura. Venta.rtn_cliente guarda una copia por factura para conservarla aunque cambie el documento del cliente.

## 5.3 Directorio y empleados

DirectorioRoles agrupa Clientes, Proveedores y Empleados. DirectorioDatos selecciona tablas y campos desde TipoDirectorio; no utiliza nombres de tabla escritos por el operador. Comparte búsqueda, listado, guardado, actualización y cambios de estado.

Empleado tiene código, nombre, teléfono, correo y tienda. Las nuevas asignaciones exigen tienda activa. Usuario se vincula a un empleado; desactivarlo impide que su sesión siga siendo válida, sin borrar la cuenta.

## 5.4 Tiendas y desactivación lógica

ConfiguracionAuditoria mantiene código, nombre, dirección y teléfono de tiendas. Cada sucursal tiene inventario independiente y aparece en empleados, compras, ventas, apartados y traspasos.

En los maestros del esquema 3FN, activo se calcula desde desactivado_en. Desactivar registra fecha y motivo; reactivar limpia esos campos y mantiene relaciones. El motivo de catálogo admite hasta 300 caracteres; los motivos documentales suelen admitir hasta 500.

No todos los métodos rellenan creado_por, actualizado_por o desactivado_por. La auditoría puede obtener el actor desde SESSION_CONTEXT sin que cada columna de actor del maestro quede poblada. Son mecanismos relacionados, pero diferentes.

# 6. Compras y entrada de mercancía

## 6.1 Datos y estados

Compra registra proveedor, tienda, factura del proveedor, fecha de factura, usuario y observaciones. DetalleCompra contiene producto, cantidad y costo. El subtotal es cantidad por costo y vw_CompraTotal suma las líneas. El costo de compra y el precio de venta son conceptos distintos; no se encontró actualización automática del precio del catálogo al confirmar.

| Estado o acción | Qué permite | Efecto en inventario |
|---|---|---|
| Crear BORRADOR | Capturar cabecera y líneas | Ninguna entrada de unidades |
| Editar BORRADOR | Cambiar factura, fecha, observaciones y detalle | Ninguno; proveedor y tienda no cambian |
| Confirmar | Validar catálogos activos y líneas | Movimiento COMPRA por detalle y aumento del saldo |
| Anular borrador | Registrar actor, fecha y motivo | No revierte unidades porque no se contabilizó |
| Anular confirmada | Validar que procede y conservar historial | Movimiento REVERSO contra la entrada original |

Exige número de factura, productos sin repetir, cantidades enteras y costos positivos con hasta dos decimales. Proveedor y número de factura forman una combinación única, incluso si la compra anterior se anuló.

## 6.2 Guardar y detectar conflictos

CompraDatos.Guardar valida, abre una transacción Serializable y comprueba sesión y triggers. Al editar bloquea cabecera y detalle y compara una huella SHA-256 calculada sobre ambos. Así detecta cambios de líneas aunque Compra no tenga rowversion propio.

Actualiza la cabecera si conserva proveedor y tienda y sigue BORRADOR. Sustituye las líneas mediante DELETE e INSERT dentro de la transacción. Los triggers impiden esa sustitución cuando el documento deja de ser editable; no se borran líneas confirmadas como parte de una edición ordinaria.

## 6.3 Confirmación y anulación

Confirmar verifica líneas, proveedor, tienda y productos activos. C# cambia Compra.estado a CONFIRMADA; trg_Compra_contabilizar inserta movimientos referidos a DetalleCompra; trg_MovimientoInventario_aplicar crea o actualiza el saldo de la tienda.

Anular una compra confirmada genera reversos sin borrar entradas originales. No puede dejar existencia negativa ni inferior a las reservas. Si las unidades ya se vendieron o están comprometidas, SQL puede rechazarla. Un error revierte la operación completa.

Ejemplo: confirmar una compra de 20 unidades deja existencia 20. Aprobar un traspaso por 6 deja reservado 6 y disponible 14. Anular esa compra debe respetar los compromisos y los consumos posteriores; no consiste en eliminar la factura.

# 7. Ventas, pagos, impuesto, RTN, CAI y recibos

## 7.1 Captura en PuntoVenta

El operador elige tienda, cliente y método de pago, consulta productos y agrega cantidades al carrito. Ve existencia, reservado y disponible. Puede quitar líneas, vaciar el carrito, recargar catálogos, actualizar stock y seleccionar impuesto por producto agregado.

En efectivo, el recibido debe cubrir el total y se calcula cambio. Tarjeta y transferencia exigen referencia. En efectivo se genera una referencia EFE- seguida de GUID. La venta directa captura un método de pago; la base admite varios PagoVenta y la entrega de apartados utiliza esa capacidad para preservar abonos.

## 7.2 Transacción de emisión

VentaDatos.Emitir valida datos y abre una transacción Serializable. Comprueba sesión, catálogos, precio y stock otra vez. Si el precio cambió después de seleccionar el producto, pide actualizar. Procesa las líneas en orden de producto y utiliza bloqueos para coordinar ventas concurrentes.

Genera un número interno V- seguido de GUID, crea Venta en BORRADOR y copia el CAI de la tienda. Inserta DetalleVenta con nombre y precio históricos, registra PagoVenta y cambia el estado a CONFIRMADA. Los triggers comprueban suma de pagos y crean los movimientos VENTA. Solo entonces se confirma la transacción.

Si falla una línea, pago o trigger, se revierte todo. Si la transacción terminó pero después falla mostrar el recibo, la factura sigue guardada y puede consultarse en el historial. Imprimir no es lo que descuenta inventario.

## 7.3 Cálculo del ISV implementado

El programa admite tasas 0 %, 15 % y 18 %. Se describen como reglas del código: no clasifica automáticamente artículos ni comprueba el tratamiento fiscal con una autoridad externa.

```text
subtotal = cantidad × precio base
ISV = redondear(subtotal × tasa / 100, 2)
total de línea = subtotal + ISV
total de venta = suma de totales de línea
cambio en efectivo = recibido − total
```

ImpuestosVenta.Isv redondea a dos decimales con MidpointRounding.AwayFromZero. SQL calcula las columnas mediante ROUND y decimal(18,2). La tasa inicial es 15. Para corregirla en el carrito se quita la línea y se agrega con la opción deseada.

Ejemplo: dos unidades a L 100 con tasa 15 % generan subtotal L 200, impuesto L 30 y total L 230. Con recibido L 250, el cambio es L 20. Una línea exenta conserva impuesto cero y aparece en el importe exento del recibo.

## 7.4 RTN opcional

NormalizarRtn retira espacios y guiones. Vacío significa NULL; otro valor debe tener 14 dígitos. Un CHECK protege esa condición. Venta.rtn_cliente conserva el dato para reimpresiones, aunque cambie el documento del cliente.

La pantalla puede sugerirlo desde el documento fiscal del cliente si es válido. La entrega de apartados observada no captura ni copia rtn_cliente en su INSERT de venta; sí copia CAI. Por tanto, la captura de RTN no es idéntica en venta directa y entrega de apartado.

## 7.5 CAI por tienda

ConfiguracionFacturacionDatos obtiene y guarda CAI por tienda. Admite 32 caracteres alfanuméricos sin guiones o XXXXXX-XXXXXX-XXXXXX-XXXXXX-XXXXXX-XX. Quita espacios, convierte a mayúsculas y normaliza a 37 caracteres. Usa rowversion y registra el cambio en bitácora.

La factura guarda una copia del CAI vigente; cambiar la configuración no cambia facturas anteriores. Sin configuración, el recibo indica Sin configurar. El programa no genera un CAI autorizado ni comprueba autorización externa.

El número V-GUID es interno. No se identificó un flujo completo de rangos autorizados, correlativos fiscales y fecha límite de emisión. Guardar CAI y desglosar impuesto no acredita por sí solo todo el proceso fiscal.

## 7.6 Historial, impresión y PDF

El historial consulta las últimas 500 ventas con número, fecha UTC, cliente, RTN, tienda, estado y total. Su búsqueda filtra ese conjunto por número, cliente o RTN. Desde allí se ve el recibo y se guarda PDF.

VentaDatos.Comprobante reconstruye el recibo con Venta, DetalleVenta, PagoVenta y catálogos. Muestra cantidades, precios, tasas, subtotal, exento, ISV 15, ISV 18, total, pagos, referencias, cambio, usuario, RTN y CAI. Conserva el nombre del producto del detalle; los nombres de cliente, tienda y usuario se consultan del catálogo actual.

La impresión usa Windows Forms. ExportacionArchivos.ReciboPdf produce PDF A4, ajusta líneas largas y agrega páginas. Está disponible desde última venta, historial y entregas de apartado.

El esquema y triggers contemplan BORRADOR, CONFIRMADA y ANULADA y reversos de venta. No se identificó una acción general de anulación expuesta en PuntoVenta comparable al botón de compras. El soporte de base no implica que todo ese flujo esté disponible en pantalla.

# 8. Inventario y traspasos

## 8.1 Existencia, reserva y disponibilidad

```text
existencia = unidades físicas registradas en la tienda
reservado = unidades comprometidas por apartados o traspasos
disponible = existencia − reservado
```

InventarioTienda tiene clave compuesta tienda/producto. Impide existencia negativa, reserva negativa o superior a la existencia y mínimos negativos. El mismo producto tiene saldos independientes en cada sucursal.

Existencias incluye productos activos sin saldo, mostrados con cero, y productos inactivos con fila de inventario. Movimientos filtra tienda, texto y fechas UTC. Cambiar fechas no modifica el saldo actual de Existencias.

Guardar mínimo modifica solo stock_minimo con verificación de versión. La pantalla no permite ingresar, retirar o revertir unidades mediante ajustes manuales. Pueden existir movimientos AJUSTE por carga masiva o historial, sin que haya un botón actual de ajuste.

## 8.2 Libro de movimientos

| Tipo | Signo habitual | Origen |
|---|---|---|
| COMPRA | Positivo | Compra confirmada |
| VENTA | Negativo | Venta confirmada |
| TRASPASO_SALIDA | Negativo | Envío desde origen |
| TRASPASO_ENTRADA | Positivo | Recepción en destino |
| DEVOLUCION | Positivo | Devolución que reintegra stock |
| AJUSTE | Según el caso | Stock inicial de importación u operaciones históricas |
| REVERSO | Opuesto al original | Compensación referida a id_movimiento_revertido |

MovimientoInventario es inmutable. Una corrección documental genera reverso cuando procede; no actualiza ni borra el asiento original. Los índices únicos impiden contabilizar dos veces una línea o revertir dos veces el mismo movimiento.

## 8.3 Etapas del traspaso

| Transición | Acción | Cambio de unidades |
|---|---|---|
| Alta → PENDIENTE | Solicitar entre tiendas distintas | No reserva ni descuenta |
| PENDIENTE → APROBADO | Aprobar detalle | Reserva en origen; existencia no cambia |
| PENDIENTE → RECHAZADO | Rechazar con motivo | No mueve unidades |
| PENDIENTE → CANCELADO | Cancelar con motivo | No mueve unidades |
| APROBADO → CANCELADO | Cancelar antes del envío | Libera reserva |
| APROBADO → EN_TRANSITO | Enviar | Libera reserva y descuenta origen |
| EN_TRANSITO → RECIBIDO | Confirmar recepción | Aumenta existencia en destino |

El documento guarda origen, destino, solicitante, observaciones y líneas con cantidad y precio de referencia. Este precio es un dato del traspaso, no una venta. Las tiendas de una solicitud creada no cambian.

TraspasoDatos.Guardar mantiene la solicitud PENDIENTE y su detalle. Modificar.ResolverTraspaso exige estado y rowversion correctos. El operador debe coincidir con la identidad autenticada; no se selecciona otro usuario para atribuirse operaciones.

trg_Traspaso_operar aplica reservas, salidas y entradas. EventoTraspaso conserva operador y fecha de cada etapa. El índice único permite un evento por solicitud/estado; editar una pendiente no crea otro evento PENDIENTE. Validación permite ver líneas e historial.

Ejemplo: origen 20/0 en existencia/reservado. Aprobar 6 deja 20/6 y disponible 14. Enviar deja 14/0. Recibir agrega 6 en destino. Repetir envío o recepción no duplica movimientos. No existe recepción parcial; cada etapa procesa todas las líneas.

# 9. Apartados, abonos, devoluciones y reembolsos

## 9.1 Reserva y estados del apartado

Apartado reserva artículos para un cliente, tienda y vencimiento. Crear exige líneas sin repetir, cantidades positivas, catálogos activos, fecha futura, precio vigente y stock suficiente. Guarda precio e ISV por línea; los triggers reservan unidades.

El listado muestra cliente, tienda, fecha y vencimiento UTC, estado, total, abonado, saldo, reembolsable y venta de entrega. Filtra estado y busca cliente, tienda o ID. El detalle incluye productos, abonos y reembolsos.

| Estado o acción | Efecto comercial | Reserva |
|---|---|---|
| ACTIVO | Mantiene compromiso y permite abonos | Unidades reservadas |
| Abonar | Registra importe, método y referencia | No duplica reserva |
| Entregar → ENTREGADO | Crea y confirma venta enlazada | Libera reserva y contabiliza venta |
| Cancelar → CANCELADO | Termina compromiso con motivo | Libera reserva |
| Vencer → VENCIDO | Cierra cuando alcanzó vencimiento | Libera reserva |
| Reembolsar cerrado | Devuelve abonos no aplicados a venta | No vuelve a mover unidades |

Los abonos son positivos, con hasta dos decimales y sin superar el total. No se editan o eliminan cobros originales. Cada abono puede enlazarse una sola vez a un PagoVenta de igual importe y método.

## 9.2 Entrega y dinero

Entregar verifica apartado activo y no vencido, con catálogos activos. Calcula total con impuesto menos abonos. Crea Venta con precios y tasas reservados, la enlaza al apartado y transforma cada abono en su propio pago. Cobra el saldo pendiente si lo hay; sin saldo no exige otro cobro.

El trigger comprueba factura y apartado: cliente, tienda, productos, cantidades, precios y tasas. También exige todos los abonos enlazados. Confirmar la venta marca ENTREGADO, libera reserva y contabiliza salida. Todo se realiza en una transacción.

Cancelar o vencer no devuelve dinero automáticamente. Reembolsar devuelve los abonos pendientes, restando reembolsos previos. El vencimiento de la fecha impide abonar o entregar, pero no modifica por sí solo el estado de la fila: se marca vencida desde el flujo o mediante usp_ProcesarVencimientos. No se encontró una tarea programada instalada por la aplicación.

## 9.3 Devolución de una factura

Devolucion parte de Venta CONFIRMADA. Cada línea referencia DetalleVenta para conservar el precio y la factura de origen. El operador selecciona cantidad, motivo y si se reintegran unidades al inventario.

LineasVenta calcula lo devolvible considerando otras devoluciones no anuladas, incluidos borradores. Guardar crea o edita BORRADOR sin permitir cambiar la factura existente. Rechaza líneas repetidas, cantidades no positivas y acumulados superiores a lo vendido.

Confirmar pasa a CONFIRMADA y genera DEVOLUCION solo para líneas que reintegran stock. Un artículo dañado puede devolver dinero sin volver a inventario vendible si esa opción no está activada.

El ISV parcial deriva de la línea original y lo ya devuelto. Distribuye centavos de forma acumulada para que devolver todas las unidades restituya exactamente el impuesto original.

## 9.4 Reembolso, anulación y concurrencia

Reembolsar exige devolución confirmada. Recorre pagos originales, resta lo reembolsado y registra el pendiente con el método de cada cobro. Rechaza saldo insuficiente. Reembolso debe referenciar un PagoVenta o un AbonoApartado; el CHECK impide ambos a la vez o ninguno.

Anular una confirmada necesita un reverso de inventario válido. Los triggers rechazan anular una devolución reembolsada sin conciliación financiera adicional. Una venta con devoluciones no anuladas tampoco puede anularse.

Apartado usa rowversion. Devolucion calcula SHA-256 sobre cabecera, líneas y reembolsos para detectar cambios. Los métodos bloquean y comparan antes de operar. Los reembolsos históricos no se editan ni eliminan para deshacer dinero.

# 10. Excel, archivos, alertas y auditoría

## 10.1 Carga masiva

CargaMasiva.Importacion lee solo la primera hoja de un .xlsx. La plantilla recomienda encabezados en fila 1; el lector usa la primera fila utilizada. Resuelve columnas por nombre y rechaza repetidas o faltantes.

| Columna obligatoria | Contenido | Validación |
|---|---|---|
| CodigoBarras | Código como texto | Único, obligatorio, hasta 50 caracteres |
| Nombre | Nombre de artículo | Obligatorio, hasta 150 caracteres |
| Precio | Precio base | Positivo, hasta 99 999 999,99 y dos decimales |
| StockInicial | Unidades en tienda elegida | Entero no negativo; vacío equivale a cero |
| NombreCategoria | Nombre del catálogo | Categoría activa existente; no es ID |
| RTNProveedor | RTN como texto | Proveedor activo, normalizando guiones y espacios; no es ID |

La vista previa agrega errores por fila: datos vacíos, formato, duplicados, categoría inexistente y proveedor no resuelto. El operador puede confirmar importar solo las válidas. No se actualizan artículos existentes.

ImportarProductos valida otra vez y abre una transacción Serializable. Inserta productos, crea AJUSTE si hay stock inicial positivo y tienda y guarda historial. Con nombre de archivo y SHA-256 proporcionados por la pantalla, crea Importacion y DetalleImportacion con JSON, filas IMPORTADAS y cabecera COMPLETADA.

La omisión de inválidas se decide antes del guardado. Durante el guardado de las aceptadas, el lote es atómico: cualquier error lo revierte entero. El hash se conserva como trazabilidad; no significa por sí mismo que esté prohibido volver a importar el archivo. Los códigos únicos impiden duplicar productos.

## 10.2 Plantillas y exportaciones

CargaMasivaPlantilla incorpora un Excel como recurso y genera otro con hojas Productos, Catalogos e Instrucciones. Consulta categorías y RTN actuales y busca códigos de ejemplo libres. Los ejemplos son ficticios y se guardan realmente si se importan.

Datos/Plantillas contiene la plantilla general y el ejemplo de papelería. Precios y cantidades son de ejemplo; categoría y RTN deben corresponder a la base elegida. No debe suponerse que los catálogos de otra instalación coinciden.

Exportar a Excel desde Catálogo conserva búsqueda, tienda e inclusión de inactivos. Incluye código, nombre, categoría, stock físico, precio base, estado y motivo. Una segunda hoja describe filtros y fecha de Honduras. No exporta toda la base automáticamente.

Excel y PDF se escriben en un temporal junto al destino y se mueven cuando terminan. Si falla generar, se intenta conservar el archivo previo. La bitácora se registra aparte: si el archivo existe pero falla SQL, la interfaz informa ambas circunstancias.

## 10.3 Campana y eventos históricos

La campana consulta AlertasStock al iniciar, cada minuto y al actualizar el diálogo. Cuenta pares producto/tienda activos con existencia − reservado menor o igual a stock_minimo. Sin fila de inventario no crea una alerta artificial; disponible 0 y mínimo 0 sí cumple.

Muestra 9+ por encima de nueve; la ayuda conserva el número completo. ? indica carga pendiente o fallida, distinto de cero alertas. El diálogo muestra tienda, código, producto, existencia, reservado, disponible y mínimo.

EventoNotificacion y Notificacion son un modelo histórico adicional. Los triggers producen eventos de stock y traspasos y el procedimiento de vencimientos puede crear avisos de inversión. La campana no implementa bandeja de leídos/no leídos ni asignación de destinatarios.

El criterio histórico difiere: trg_InventarioTienda_aviso registra disponible estrictamente menor al mínimo, con clave por tienda/producto/día; la consulta visual incluye igualdad. La campana no depende de esos eventos antiguos.

## 10.4 Auditoría

Auditoria guarda fecha UTC, usuario, módulo, acción, entidad, ID, descripción y equipo. Algunos triggers especializados guardan además JSON anterior y posterior. Los documentos y Categoria tienen triggers propios; 002 añade genéricos al resto, excluye Auditoria y evita duplicar los especializados.

El actor viene de SESSION_CONTEXT(N'id_usuario') y la justificación de motivo. Conexion y los preparadores lo configuran. Para EventoAcceso y SesionUsuario se prevé usar el usuario relacionado como respaldo cuando aún no hay actor.

Los genéricos guardan acción, clave y descripción sin copiar filas completas de seguridad. Evitan contrasena_hash y token_hash. Los documentos sí conservan instantáneas propias. Auditoria es inmutable para impedir editar o eliminar evidencia mediante operaciones normales.

La pantalla ofrece búsqueda y páginas de hasta 200 eventos. Usa el último ID como límite de la siguiente página y presenta los módulos históricos DOCUMENTOS y CATALOGO bajo sus módulos funcionales cuando corresponde.

# 11. Preparación de la base y ejecución

## 11.1 Configurar la conexión

Conexion prioriza SISTEMA_ERP_CONNECTION_STRING. Si está vacía o ausente, lee connection.local.txt desde la carpeta del ejecutable. El proyecto copia el archivo de la raíz a la salida cuando existe y Git lo excluye. Sin configuración lanza un error explicativo.

Ejemplo de desarrollo con autenticación de Windows; sustituir servidor e instancia por los reales:

```text
Server=localhost\SQLEXPRESS;Database=EquipoZAACC;Integrated Security=True;Encrypt=True;
```

Guardar la cadena en una sola línea, sin comillas externas ni prefijo ConnectionString=. Después de cambiar variables desde Windows, reiniciar Visual Studio o la terminal. Este ejemplo no revela credenciales de la instalación revisada.

## 11.2 Orden de instalación en una base nueva

1. Preparar SQL Server y un nombre de base para el entorno.
2. Abrir Base de Datos/01_esquema_erp_3fn.sql en una herramienta que procese GO. El script crea EquipoZAACC si falta y hace USE explícito; revisar ambos nombres si se necesita otra base.
3. Crear el esquema sobre una base sin sus tablas previas. No es un actualizador idempotente completo de una base poblada.
4. Si se desean datos de demostración, evaluar poblar_bdd.sql antes de triggers. El propio script rechaza triggers activos. Sus hashes no son contraseñas utilizables y sus usuarios impiden la inicialización automática admin.
5. Ejecutar Base de Datos/Triggers2.sql en esa misma base. Requiere nested triggers habilitado en la instancia y RECURSIVE_TRIGGERS OFF en la base; comprueba esas condiciones.
6. Ejecutar Datos/Migraciones/002_bitacora_sistema.sql después de Triggers2.sql. El esquema actual ya incluye RTN, ISV y CAI; las migraciones 003 y 004 agregan o actualizan objetos faltantes cuando se requiera.
7. Configurar la conexión del programa. Con Usuario vacía, completar acceso inicial y cambio de contraseña.
8. Crear o revisar tiendas, categorías, proveedores, clientes y empleados. Cargar existencias mediante compras confirmadas o importación con stock inicial.

La aplicación no instala automáticamente el esquema. Varias escrituras comprueban triggers requeridos y habilitados antes de operar. Todos los scripts sin USE explícito se ejecutan sobre la base seleccionada en la herramienta SQL.

## 11.3 Migraciones

| Archivo | Cambio | Alcance |
|---|---|---|
| 001_desactivar_registros.sql | Añade activo bit si falta en Producto, Categoria y Proveedor | Compatibilidad limitada con otro esquema; no transforma una base antigua al 3FN completo |
| 002_bitacora_sistema.sql | Crea triggers genéricos de auditoría por metadatos | Complementa Triggers2 y cubre entidades adicionales |
| 003_rtn_ventas.sql | Añade rtn_cliente y CHECK si faltan | El esquema nuevo ya los incluye |
| 004_isv_cai.sql | Añade tasas, ISV de devolución, CAI, configuración y redefine vistas/triggers | Conserva importes anteriores y actualiza facturación |

004 asigna tasa cero a las líneas antiguas para no cambiar importes históricos y deja 15 como DEFAULT de las nuevas. Mantener el orden de scripts: volver a ejecutar Triggers2 puede sustituir definiciones instaladas después.

001 debe interpretarse según la base: activo del esquema 3FN se calcula desde desactivado_en. Añadir un bit a otro esquema no instala esa lógica ni sus columnas relacionadas. En una base 3FN que ya tiene activo, sus condiciones no vuelven a añadirlo.

## 11.4 Reinicio y datos de ejemplo

00_reiniciar_esquema.sql construye DROP VIEW, ALTER TABLE DROP CONSTRAINT y DROP TABLE para dbo. La versión revisada termina en PRINT @sql: muestra las instrucciones, sin ejecutarlas. No elimina procedimientos ni crea de nuevo las tablas. Copiar y ejecutar el texto mostrado sí borraría datos; no forma parte del arranque.

poblar_bdd.sql inserta demostración en una transacción, exige ausencia de triggers activos y rechaza repetir códigos DEMO. No establece datos reales de una empresa ni credenciales válidas.

## 11.5 Compilación

Con el SDK de .NET apropiado en Windows y desde la raíz:

```text
dotnet restore Sistema_ERP.csproj
dotnet build Sistema_ERP.csproj
dotnet run --project Sistema_ERP.csproj
```

La restauración necesita dependencias locales o acceso a NuGet. Pruebas/**/*.cs se excluye del ejecutable principal y se compila mediante otro proyecto. La interfaz requiere Windows.

# 12. Diseño relacional y programación SQL

## 12.1 Normalización y relaciones

El esquema define 39 tablas y siete vistas. Separa maestros, cabeceras, detalles, pagos, relaciones y registros históricos. Un proveedor se guarda una vez; muchas compras lo referencian. Cada detalle pertenece a un documento y cada saldo a una pareja tienda/producto.

Se evita repetir teléfonos o nombres de categorías en todos los documentos. Algunos datos se copian deliberadamente para conservar historia: precio, tasa y nombre del producto vendido, RTN y CAI de factura. Cambiar después el catálogo no debe reemplazar esas copias.

| Relación | Significado |
|---|---|
| Categoria → Producto | Una categoría clasifica muchos artículos |
| Proveedor → Producto / Compra | Un proveedor suministra artículos y tiene compras |
| Tienda + Producto → InventarioTienda | Un saldo por combinación |
| Tienda → Empleado → Usuario | Empleados por tienda y cuenta vinculada al empleado |
| Usuario ↔ Rol ↔ Permiso | Muchos a muchos mediante UsuarioRol y RolPermiso |
| Usuario ↔ Tienda | Asignación prevista mediante UsuarioTienda |
| Compra → DetalleCompra | Cabecera, productos y entrada derivada por línea |
| Venta → DetalleVenta / PagoVenta | Factura con artículos y uno o varios cobros |
| Traspaso → DetalleTraspaso / EventoTraspaso | Líneas trasladadas e historial de etapas |
| Apartado → DetalleApartado / AbonoApartado | Productos reservados y cobros parciales |
| Apartado → Venta | Una entrega enlazada con venta única |
| Devolucion → Venta / DetalleDevolucion | Factura original y líneas devueltas |
| PagoVenta o AbonoApartado → Reembolso | Dinero devuelto contra cobro concreto |
| Documento → MovimientoInventario | Variaciones con origen trazable |
| Importacion → DetalleImportacion | Archivo, filas y productos resultantes |
| EventoNotificacion → Notificacion → Usuario | Evento, destinatario y lectura |

El capítulo 15 detalla columnas y relaciones. Los ALTER TABLE del anexo A agregan además referencias de creado_por, actualizado_por y desactivado_por de maestros hacia Usuario.

## 12.2 Restricciones, tipos e índices

- PRIMARY KEY identifica filas. Algunas usan IDENTITY; otras son compuestas, como InventarioTienda y UsuarioRol.
- FOREIGN KEY exige referencias existentes y relaciona cabeceras, detalles, pagos y actores.
- UNIQUE protege códigos, nombres o combinaciones. Los índices filtrados permiten nulos cuando corresponde.
- CHECK restringe estados, cantidades, importes, reservas y coherencia de anulaciones o reembolsos.
- DEFAULT asigna valores iniciales como SYSUTCDATETIME y estado documental.
- Las columnas calculadas generan activo, códigos o importes a partir de otros campos.
- rowversion es un token binario de concurrencia; no es una fecha.
- decimal conserva precisión monetaria y de tasas. Los importes no se calculan con float.
- JSON guarda trazabilidad y participa en las huellas; ISJSON verifica su formato donde se declara.

Los índices de claves foráneas ayudan a localizar detalles y relacionados. Otros atienden búsqueda por tienda/fecha, estado/destino de traspaso y bitácora por fecha/usuario. Los índices únicos filtrados de movimientos evitan doble contabilización por origen y doble reverso; los de apartados y abonos impiden enlazar varias veces una venta o pago.

Muchas fechas/hora se guardan en UTC. Inicio y exportación informativa presentan Honduras; movimientos y auditoría muestran UTC explícito. fecha_factura se trata como fecha de negocio.

## 12.3 Vistas

| Vista | Resultado | Uso |
|---|---|---|
| vw_ProductoStock | Producto y suma de existencia de tiendas, o cero | Catálogo |
| vw_CompraTotal | Suma de cantidad × costo | Compras e Inicio |
| vw_VentaTotal | Subtotal, ISV y total | Historial e Inicio |
| vw_ApartadoTotal | Subtotal, ISV y total reservado | Abonos, saldo y entrega |
| vw_DevolucionDetalle | Precio original, subtotal, ISV, total y reintegro | Cálculo de devoluciones |
| vw_DevolucionTotal | Suma de devolución | Listado y reembolso |
| vw_TraspasoPasos | Operador y fecha de aprobación, envío y recepción | Proyección del historial |

Una vista de total no significa por sí misma solo confirmados. Quien la consume aplica el estado: Inicio filtra CONFIRMADA. Las vistas consultan y calculan; no registran movimientos.

## 12.4 Triggers por responsabilidad

| Trigger o familia | Responsabilidad |
|---|---|
| trg_<maestro>_actualizado | Mantiene actualizado_en de los once maestros indicados en el script |
| trg_Usuario_seguridad | Reacciona a cambio de hash o desactivación y revoca sesiones |
| trg_UsuarioRol_seguridad | Invalida por cambios de roles del usuario |
| trg_UsuarioTienda_seguridad | Invalida por cambios de tienda asignada |
| trg_RolPermiso_seguridad | Invalida usuarios de roles modificados |
| trg_Rol_seguridad / trg_Permiso_seguridad | Reacciona a cambios relevantes de rol o permiso |
| trg_MovimientoInventario_inmutable | Impide editar o borrar movimientos |
| trg_Auditoria_inmutable | Protege bitácora |
| trg_EventoTraspaso_inmutable | Protege historia de estados |
| trg_EventoAcceso_inmutable | Protege intentos de acceso |
| trg_Compra_estado / trg_Venta_estado / trg_Devolucion_estado | Limita transiciones y valida confirmación |
| trg_Traspaso_estado / trg_Apartado_estado | Restringe secuencia de etapas |
| trg_<documento>_alta_baja | Exige estado inicial y prohíbe eliminar documentos |
| trg_<documento>_cabecera | Protege identidad y campos que deben conservarse |
| trg_DetalleCompra_bloqueo / trg_DetalleVenta_bloqueo / trg_DetalleDevolucion_bloqueo | Impide cambiar líneas cuando dejan de ser editables |
| trg_DetalleApartado_bloqueo | Protege composición de reserva |
| trg_DetalleTraspaso_bloqueo / trg_PagoVenta_bloqueo | Protege líneas de traspaso y pagos, generado dinámicamente |
| trg_MovimientoInventario_validar | Comprueba coherencia de movimiento y referencias |
| trg_MovimientoInventario_documento | Comprueba estado y origen documental |
| trg_MovimientoInventario_aplicar | Modifica saldo y respeta reservas |
| trg_DetalleApartado_reservas | Reserva unidades al formar el apartado |
| trg_Apartado_cierre | Comprueba entrega enlazada y libera reservas |
| trg_Compra_contabilizar | Genera entradas y reversos de compra |
| trg_Venta_contabilizar | Genera salidas, reversos y entrega de apartado |
| trg_Traspaso_operar | Mantiene reservas, salidas, entradas e historial |
| trg_DetalleDevolucion_validar | Comprueba factura y límite devuelto |
| trg_Devolucion_contabilizar | Reintegra unidades y revierte si procede |
| trg_Reembolso_validar / trg_Reembolso_concurrencia | Comprueba origen, saldo, estado e inmutabilidad del reembolso |
| trg_AbonoApartado_proteger | Conserva cobro original y valida límites y enlace al pago |
| trg_Categoria_auditoria / trg_<documento>_auditoria | Guarda instantáneas especializadas |
| trg_InventarioTienda_aviso / trg_Traspaso_aviso | Produce eventos históricos |
| trg_Bitacora_<tabla> | 002: amplía auditoría por metadatos |
| trg_Bitacora_ConfiguracionFacturacion | 004: especializa historial del CAI |

Triggers2 define trg_DetalleDevolucion_validar dos veces. CREATE OR ALTER sustituye la definición anterior; no instala dos triggers distintos. Las familias dinámicas usan cursores y metadatos: contar solo los CREATE TRIGGER visibles no cuenta todos los objetos instalados.

Las cadenas documento → movimiento → saldo requieren nested triggers habilitado y RECURSIVE_TRIGGERS OFF. Desactivar triggers puede omitir contabilización y protecciones que el código espera.

## 12.5 Procedimientos y tipo de tabla

tt_IdUsuario es un tipo de tabla con id_usuario como clave. usp_InvalidarSesiones recibe esa lista, cambia version_seguridad y revoca sesiones abiertas. Lo usan los triggers de seguridad.

usp_ProcesarVencimientos recibe usuario activo y exige no estar dentro de una transacción previa. Establece contexto, abre una transacción, marca apartados vencidos y crea eventos de inversiones cuya fecha se alcanzó. Restaura contexto al terminar o fallar. No actualiza por sí solo Inversion.estado a VENCIDA ni instala un planificador.

## 12.6 Cómo leer las consultas

| Patrón | Uso en el proyecto |
|---|---|
| SELECT y JOIN | Recuperar datos y nombres relacionados |
| LEFT JOIN y OUTER APPLY | Conservar filas aunque falten saldo, abonos o reembolsos |
| GROUP BY y SUM | Calcular importes sin mantener totales manuales |
| CTE WITH | Generar siete días y agregados de Inicio |
| INSERT y SCOPE_IDENTITY | Crear y recuperar ID en el ámbito |
| OUTPUT INSERTED ... INTO | Capturar ID de producto de forma compatible con triggers |
| UPDATE ... WHERE estado/version | Aplicar cambio solo sobre la versión esperada |
| DELETE de detalle de borrador | Sustituir líneas editables dentro de transacción |
| CHARINDEX con parámetros | Buscar texto literal |
| UPDLOCK y HOLDLOCK | Coordinar escrituras y rangos concurrentes |
| HASHBYTES y FOR JSON PATH | Construir versión de compras y devoluciones |
| THROW | Rechazar operación inválida con explicación |
| SESSION_CONTEXT | Transmitir actor y motivo a los triggers |
| sys.tables, sys.columns y sys.triggers | Descubrir claves, instalar y comprobar objetos |
| sp_executesql y QUOTENAME | Ejecutar DDL dinámico con nombres del esquema |

Los valores se envían como @parámetros. Identificadores como tabla y columna no pueden parametrizarse del mismo modo: algunas consultas los interpolan desde enumeraciones o mapeos internos. El anexo B preserva plantillas y selectores; no se deben pegar llaves de C# en SQL Server como si fueran SQL ejecutable.

## 12.7 Variantes de SQL dinámico

| Constructor | Variantes permitidas por el código |
|---|---|
| Borrar.Desactivar | Producto/id_producto, Proveedor/id_proveedor, Categoria/id_categoria, Tienda/id_tienda |
| Modificar.Reactivar | Producto/id_producto, Proveedor/id_proveedor, Categoria/id_categoria |
| DirectorioDatos, tipo Clientes | Cliente, id_cliente, nombre, documento_fiscal, direccion |
| DirectorioDatos, tipo Proveedores | Proveedor, id_proveedor, nombre_empresa, rtn, direccion y contacto |
| DirectorioDatos, tipo Empleados | Empleado, id_empleado, nombre, codigo e id_tienda con JOIN a Tienda |
| SeguridadDatos, Usuarios | Usuario/id_usuario, nombre_usuario, id_empleado y hash; asigna UsuarioRol |
| SeguridadDatos, Roles | Rol/id_rol, codigo, nombre y descripcion; asigna RolPermiso |
| SeguridadDatos, Permisos | Permiso/id_permiso, modulo, accion y descripcion |
| PuntoVenta.Enlazar | Catálogos internos de Tienda, Cliente y MetodoPago con sus IDs y campos auxiliares |

# 13. Verificación, límites y mantenimiento

## 13.1 Pruebas incluidas

Pruebas/FlujoVentas.Integracion obtiene una conexión, crea CodexERP_Prueba_<GUID>, instala scripts y ejecuta métodos reales con referencias y reflexión. Al finalizar intenta eliminar esa base temporal y restaura la variable de conexión del proceso.

Comprueba importación y rollback, reservas, abonos y sobrepago, entrega, cancelación, vencimiento, reembolso, devoluciones parciales, reintegro opcional, concurrencia, RTN, ISV, CAI, Excel, PDF, plantillas, referencias de pago, filtros y construcción de pantallas. Simula un esquema anterior para verificar migración de impuestos.

```text
dotnet run --project Pruebas/FlujoVentas.Integracion/FlujoVentas.Integracion.csproj
```

Necesita permisos para crear y eliminar su base temporal y una instancia compatible. Es una prueba con efectos reales en esa base, no una revisión estática. El comando se documenta; no se ejecutó al preparar este manual.

Datos/Pruebas/Bitacora-sistema.sql comprueba cobertura y exclusión de información sensible mediante operaciones de prueba. Las guías *-verificacion.md contienen pasos, evidencia previa y casos pendientes. El anexo C reproduce su texto para conservar el alcance registrado.

## 13.2 Límites observados

- La autorización granular por permiso y tienda todavía no se aplica de forma general.
- Los CRUD antiguos no comprueban sesión y versión de manera uniforme.
- El buscador global no tiene un flujo integral identificado.
- La campana consulta stock vigente; no gestiona destinatarios ni mensajes leídos.
- No existe recepción parcial de traspasos ni conciliación de diferencias.
- Inventario no ofrece ajustes manuales; sus unidades vienen de documentos e importación inicial.
- El número interno y CAI no completan rangos y correlativos fiscales.
- Entregar apartados no captura RTN como la venta directa.
- No se encontró planificador de vencimientos, pantalla operativa de inversiones o recuperación de acceso completa.
- La anulación de venta existe en el modelo y triggers, pero no como acción general identificada en PuntoVenta.
- El recibo copia algunos datos históricos; no congela todos los nombres del directorio.
- El documento no certifica el esquema del servidor ni la ejecución actual de pruebas.

## 13.3 Diagnóstico práctico

| Situación | Qué revisar |
|---|---|
| No conecta | Variable, archivo local copiado, servidor, base, permisos y Probar conexión |
| Falta Triggers2 | Base correcta, triggers habilitados, nested triggers y RECURSIVE_TRIGGERS |
| El registro cambió | Recargar y obtener otra versión antes de reintentar |
| Código duplicado no visible | Incluir inactivos; la clave única sigue reservada |
| No anula compra | Stock consumido, reservado y documentos posteriores |
| Apartado vencido sigue ACTIVO | Marcar vencido o ejecutar procesamiento; el reloj no actualiza filas |
| No anula devolución | Reembolsos existentes y validez del reverso de inventario |
| Campana con ? | Consulta pendiente o fallida; abrir diálogo y actualizar |
| Excel rechazado | Hoja, encabezados, formatos, duplicados, categoría, RTN y tienda para stock |
| admin/admin no inicializa | Usuario ya poblada o registros iniciales incompatibles |
| PDF guardado con error de bitácora | Comprobar por separado archivo y registro SQL |

Para ampliar un módulo, revisar también sus vistas y triggers. Cambiar existencia directamente desde C# o insertar un movimiento ya generado por SQL puede duplicar efectos. Mantener clara la responsabilidad de cada capa y actualizar documentación y verificación junto con estados o esquema.
