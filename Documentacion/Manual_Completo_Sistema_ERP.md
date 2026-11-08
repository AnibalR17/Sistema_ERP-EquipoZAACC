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

# 14. Inventario de fuentes y cobertura documental

Se analizaron 100 archivos C# y 9 scripts SQL del árbol de trabajo. El anexo B contiene 216 entradas de sentencias, plantillas y fragmentos SQL identificados en C#, con sus ubicaciones. El conteo corresponde a apariciones en el código, no a consultas distintas ni a tablas. Incluye las modificaciones locales presentes al preparar el documento.

La revisión es estática y no consulta la base configurada. Los objetos existentes realmente en el servidor, sus datos y la ejecución de las pruebas de integración no se certifican mediante este documento. Los bloques de SQL se copiaron o se extrajeron de los archivos locales; no contienen la cadena de conexión privada.

## 14.1 Archivos C# y responsabilidad

| Archivo | Tipos declarados | Métodos | Líneas |
|---|---|---|---|
| Clases/Borrar.cs | Borrar | 5 | 41 |
| Clases/ClaveSegura.cs | ClaveSegura | 4 | 38 |
| Clases/Cliente.cs | Cliente | 1 | 32 |
| Clases/Ingresar.cs | Ingresar | 3 | 49 |
| Clases/Modificar.cs | Modificar | 8 | 111 |
| Clases/Proveedor.cs | Proveedor | 1 | 35 |
| Clases/SesionActual.cs | SesionActual | 2 | 31 |
| Clases/TipoDirectorio.cs | TipoDirectorio | 0 | 5 |
| Controles/PanelDisenoInventario.cs | PanelDisenoInventario | 3 | 89 |
| Datos/AccesoDatos.cs | AccesoUsuario, ResultadoAcceso, AccesoDatos | 6 | 192 |
| Datos/ApartadoDatos.cs | ApartadoDatos | 9 | 185 |
| Datos/AuditoriaDatos.cs | AuditoriaDatos | 3 | 67 |
| Datos/CargaMasivaDatos.cs | ProductoImportado, CargaMasivaDatos | 5 | 136 |
| Datos/CargaMasivaPlantilla.cs | CargaMasivaPlantilla | 2 | 74 |
| Datos/CategoriaDatos.cs | CategoriaDatos | 6 | 49 |
| Datos/ClienteDatos.cs | ClienteDatos | 7 | 115 |
| Datos/CompraDatos.cs | LineaCompra, CompraDatos | 16 | 333 |
| Datos/Conexion.cs | Conexion | 3 | 45 |
| Datos/ConfiguracionFacturacionDatos.cs | ConfiguracionFacturacionDatos | 3 | 45 |
| Datos/DevolucionDatos.cs | LineaDevolucion, DevolucionDatos | 9 | 171 |
| Datos/DirectorioDatos.cs | DirectorioDatos | 9 | 133 |
| Datos/ExportacionArchivos.cs | ExportacionArchivos | 4 | 118 |
| Datos/FlujoVentaDatos.cs | FlujoVentaDatos | 10 | 91 |
| Datos/ImpuestosVenta.cs | ImpuestosVenta | 1 | 11 |
| Datos/InicioDatos.cs | DiaInicio, ResumenInicio, InicioDatos | 1 | 60 |
| Datos/InventarioDatos.cs | InventarioDatos | 6 | 133 |
| Datos/ProductoDatos.cs | Producto, ProductoDatos | 7 | 104 |
| Datos/ProveedorDatos.cs | ProveedorDatos | 8 | 65 |
| Datos/SeguridadDatos.cs | TipoSeguridad, SeguridadDatos | 8 | 197 |
| Datos/TiendaDatos.cs | TiendaDatos | 7 | 70 |
| Datos/TraspasoDatos.cs | LineaTraspaso, TraspasoDatos | 9 | 183 |
| Datos/VentaDatos.cs | LineaVenta, FacturaEmitida, VentaDatos | 6 | 181 |
| Formularios/Acceso/FrmLogin.Designer.cs | FrmLogin | 2 | 451 |
| Formularios/Acceso/FrmLogin.cs | FrmLogin | 6 | 97 |
| Formularios/Administracion/ConfiguracionAuditoria.Designer.cs | ConfiguracionAuditoria | 2 | 1200 |
| Formularios/Administracion/ConfiguracionAuditoria.Facturacion.cs | ConfiguracionAuditoria | 3 | 77 |
| Formularios/Administracion/ConfiguracionAuditoria.cs | ConfiguracionAuditoria | 14 | 197 |
| Formularios/Administracion/DirectorioRoles.Designer.cs | DirectorioRoles | 2 | 1383 |
| Formularios/Administracion/DirectorioRoles.Presentacion.cs | DirectorioRoles | 2 | 61 |
| Formularios/Administracion/DirectorioRoles.cs | DirectorioRoles | 11 | 164 |
| Formularios/Administracion/UsuariosRolesPermisos.Designer.cs | UsuariosRolesPermisos | 2 | 129 |
| Formularios/Administracion/UsuariosRolesPermisos.Presentacion.cs | UsuariosRolesPermisos | 1 | 38 |
| Formularios/Administracion/UsuariosRolesPermisos.cs | UsuariosRolesPermisos, Opcion | 13 | 197 |
| Formularios/Base/DialogoMaqueta.cs | DialogoMaqueta | 1 | 25 |
| Formularios/Base/PantallaMaqueta.cs | PantallaMaqueta | 6 | 97 |
| Formularios/Cliente/PantallaClientes.Presentacion.cs | PantallaClientes | 3 | 105 |
| Formularios/Cliente/PantallaClientes.cs | PantallaClientes | 14 | 246 |
| Formularios/Compras/PantallaCompras.Designer.cs | PantallaCompras | 2 | 260 |
| Formularios/Compras/PantallaCompras.Presentacion.cs | PantallaCompras | 2 | 27 |
| Formularios/Compras/PantallaCompras.cs | PantallaCompras | 30 | 511 |
| Formularios/Dialogos/AgregarDirectorio.Designer.cs | AgregarDirectorio | 2 | 525 |
| Formularios/Dialogos/AgregarDirectorio.Presentacion.cs | AgregarDirectorio | 1 | 26 |
| Formularios/Dialogos/AgregarDirectorio.cs | AgregarDirectorio | 2 | 48 |
| Formularios/Dialogos/ConfirmarVenta.cs | ConfirmarVenta | 0 | 17 |
| Formularios/Dialogos/MotivoDesactivacion.Designer.cs | MotivoDesactivacion | 2 | 330 |
| Formularios/Dialogos/MotivoDesactivacion.Presentacion.cs | MotivoDesactivacion | 3 | 32 |
| Formularios/Dialogos/MotivoDesactivacion.cs | MotivoDesactivacion | 1 | 30 |
| Formularios/Dialogos/Notificaciones.Designer.cs | Notificaciones | 2 | 122 |
| Formularios/Dialogos/Notificaciones.cs | Notificaciones | 1 | 69 |
| Formularios/Inventario/FlujoTraspasos.Editor.cs | FlujoTraspasos, EditorTraspaso | 0 | 225 |
| Formularios/Inventario/FlujoTraspasos.cs | FlujoTraspasos | 14 | 273 |
| Formularios/Inventario/Inventario.Designer.cs | Inventario | 2 | 542 |
| Formularios/Inventario/Inventario.Funcional.Designer.cs | Inventario | 1 | 78 |
| Formularios/Inventario/Inventario.cs | Inventario | 7 | 102 |
| Formularios/Inventario/TraspasoEstilo.cs | TraspasoEstilo | 7 | 126 |
| Formularios/Inventario/Traspaso_Tiendas.Designer.cs | Traspaso_Tiendas | 2 | 671 |
| Formularios/Inventario/Traspaso_Tiendas.cs | Traspaso_Tiendas | 1 | 20 |
| Formularios/Inventario/Validacion_Traspasos.Designer.cs | Validacion_Traspasos | 2 | 510 |
| Formularios/Inventario/Validacion_Traspasos.cs | Validacion_Traspasos | 1 | 20 |
| Formularios/Principal/Pagina_Central.Designer.cs | Pagina_Central | 2 | 893 |
| Formularios/Principal/Pagina_Central.Inicio.cs | Pagina_Central | 4 | 132 |
| Formularios/Principal/Pagina_Central.Notificaciones.cs | Pagina_Central | 6 | 89 |
| Formularios/Principal/Pagina_Central.Tiendas.cs | Pagina_Central | 2 | 58 |
| Formularios/Principal/Pagina_Central.cs | Pagina_Central | 28 | 324 |
| Formularios/Productos/CargaMasiva.Importacion.cs | CargaMasiva, Catalogos | 9 | 272 |
| Formularios/Productos/CargaMasiva.cs | CargaMasiva | 2 | 148 |
| Formularios/Productos/Catalogo_Productos.Categorias.cs | Catalogo_Productos | 8 | 237 |
| Formularios/Productos/Catalogo_Productos.Designer.cs | Catalogo_Productos | 2 | 635 |
| Formularios/Productos/Catalogo_Productos.Presentacion.cs | Catalogo_Productos | 3 | 138 |
| Formularios/Productos/Catalogo_Productos.cs | Catalogo_Productos | 14 | 256 |
| Formularios/Productos/Nuevo_Producto.Designer.cs | Nuevo_Producto | 2 | 241 |
| Formularios/Productos/Nuevo_Producto.Presentacion.cs | Nuevo_Producto | 1 | 48 |
| Formularios/Productos/Nuevo_Producto.cs | Nuevo_Producto | 5 | 133 |
| Formularios/Productos/PantallaProductos.Designer.cs | PantallaProductos | 2 | 226 |
| Formularios/Productos/PantallaProductos.Presentacion.cs | PantallaProductos | 1 | 12 |
| Formularios/Productos/PantallaProductos.cs | PantallaProductos | 9 | 128 |
| Formularios/Proveedores/PantallaProveedores.Designer.cs | PantallaProveedores | 2 | 204 |
| Formularios/Proveedores/PantallaProveedores.Presentacion.cs | PantallaProveedores | 1 | 95 |
| Formularios/Proveedores/PantallaProveedores.cs | PantallaProveedores | 17 | 316 |
| Formularios/Ventas/ApartadosDevoluciones.Editores.cs | ApartadosDevoluciones, CobroDialogo | 6 | 179 |
| Formularios/Ventas/ApartadosDevoluciones.cs | ApartadosDevoluciones | 22 | 200 |
| Formularios/Ventas/PuntoVenta.cs | PuntoVenta | 13 | 259 |
| Presentacion/DialogosExportacion.cs | DialogosExportacion | 1 | 30 |
| Presentacion/DialogosMaqueta.cs | DialogosMaqueta | 2 | 46 |
| Presentacion/EstiloFormularios.cs | EstiloFormularios | 3 | 63 |
| Presentacion/ModoDiseno.cs | ModoDiseno | 1 | 16 |
| Presentacion/ProductoEstilo.cs | ProductoEstilo | 1 | 27 |
| Presentacion/VistaContenido.cs | VistaContenido | 1 | 21 |
| Program.cs | Program | 1 | 32 |
| Pruebas/FlujoVentas.Integracion/Program.cs | Program | 25 | 506 |

Los archivos Designer y Presentacion describen controles, distribución y estilos. Los métodos de operación y las consultas se documentan por módulo y en el anexo B. Los recursos .resx acompañan a sus formularios y las plantillas .xlsx y el PDF de ejemplo son archivos de datos, no componentes que ejecuten consultas.

# 15. Diccionario completo de la base de datos

El esquema base define 39 tablas. Las declaraciones de columnas se mantienen con su tipo, nulabilidad, DEFAULT y expresiones calculadas. Las restricciones completas y los ALTER TABLE posteriores aparecen en el anexo A; por eso la tabla de columnas debe leerse junto con las relaciones y reglas, no como un reemplazo del DDL.

## 15.1 dbo.Categoria

Clasifica los productos mediante un catálogo reutilizable.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 11.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_categoria | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| nombre | nvarchar(100) NOT NULL | Dato de nombre de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| descripcion | nvarchar(300) NULL | Dato de descripcion de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| creado_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| creado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| actualizado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| actualizado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| desactivado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| desactivado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| motivo_desactivacion | nvarchar(300) NULL | Justificación de la operación; se conserva como parte de su historial. |
| activo | AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED | Estado lógico; revisar si es columna calculada a partir de desactivado_en o bit persistido en esta tabla. |
| version_fila | rowversion NOT NULL | Token rowversion para detectar modificaciones concurrentes; no es una fecha. |

Clave primaria: id_categoria


Reglas CHECK: CK_Categoria_desactivacion. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.2 dbo.Proveedor

Guarda empresas proveedoras, RTN y datos de contacto.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 29.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_proveedor | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| nombre_empresa | nvarchar(150) NOT NULL | Dato de nombre empresa de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| rtn | varchar(30) NULL | Dato de rtn de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| contacto | nvarchar(150) NULL | Dato de contacto de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| telefono | nvarchar(30) NULL | Dato de telefono de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| correo | nvarchar(254) NULL | Dato de correo de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| direccion | nvarchar(300) NULL | Dato de direccion de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| creado_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| creado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| actualizado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| actualizado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| desactivado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| desactivado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| motivo_desactivacion | nvarchar(300) NULL | Justificación de la operación; se conserva como parte de su historial. |
| activo | AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED | Estado lógico; revisar si es columna calculada a partir de desactivado_en o bit persistido en esta tabla. |
| version_fila | rowversion NOT NULL | Token rowversion para detectar modificaciones concurrentes; no es una fecha. |

Clave primaria: id_proveedor


Reglas CHECK: CK_Proveedor_desactivacion. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.3 dbo.Cliente

Guarda clientes, documento fiscal y datos de contacto.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 50.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_cliente | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| nombre | nvarchar(150) NOT NULL | Dato de nombre de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| documento_fiscal | varchar(30) NULL | Dato de documento fiscal de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| telefono | nvarchar(30) NULL | Dato de telefono de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| correo | nvarchar(254) NULL | Dato de correo de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| direccion | nvarchar(300) NULL | Dato de direccion de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| creado_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| creado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| actualizado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| actualizado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| desactivado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| desactivado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| motivo_desactivacion | nvarchar(300) NULL | Justificación de la operación; se conserva como parte de su historial. |
| activo | AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED | Estado lógico; revisar si es columna calculada a partir de desactivado_en o bit persistido en esta tabla. |
| version_fila | rowversion NOT NULL | Token rowversion para detectar modificaciones concurrentes; no es una fecha. |

Clave primaria: id_cliente


Reglas CHECK: CK_Cliente_desactivacion. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.4 dbo.Tienda

Identifica sucursales y datos generales de cada establecimiento.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 70.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_tienda | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| codigo | varchar(20) NOT NULL | Dato de codigo de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| nombre | nvarchar(100) NOT NULL | Dato de nombre de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| direccion | nvarchar(300) NULL | Dato de direccion de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| telefono | nvarchar(30) NULL | Dato de telefono de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| creado_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| creado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| actualizado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| actualizado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| desactivado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| desactivado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| motivo_desactivacion | nvarchar(300) NULL | Justificación de la operación; se conserva como parte de su historial. |
| activo | AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED | Estado lógico; revisar si es columna calculada a partir de desactivado_en o bit persistido en esta tabla. |
| version_fila | rowversion NOT NULL | Token rowversion para detectar modificaciones concurrentes; no es una fecha. |

Clave primaria: id_tienda


Reglas CHECK: CK_Tienda_desactivacion. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.5 dbo.MetodoPago

Catálogo de medios de cobro: efectivo, tarjeta y transferencia.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 90.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_metodo_pago | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| codigo | varchar(30) NOT NULL | Dato de codigo de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| nombre | nvarchar(80) NOT NULL | Dato de nombre de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| creado_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| creado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| actualizado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| actualizado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| desactivado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| desactivado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| motivo_desactivacion | nvarchar(300) NULL | Justificación de la operación; se conserva como parte de su historial. |
| activo | AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED | Estado lógico; revisar si es columna calculada a partir de desactivado_en o bit persistido en esta tabla. |
| version_fila | rowversion NOT NULL | Token rowversion para detectar modificaciones concurrentes; no es una fecha. |

Clave primaria: id_metodo_pago


Reglas CHECK: CK_MetodoPago_desactivacion. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.6 dbo.Empleado

Representa al trabajador y su tienda; una cuenta de acceso se vincula a un empleado.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 108.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_empleado | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| codigo | varchar(30) NOT NULL | Dato de codigo de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| nombre | nvarchar(150) NOT NULL | Dato de nombre de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| telefono | nvarchar(30) NULL | Dato de telefono de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| correo | nvarchar(254) NULL | Dato de correo de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| id_tienda | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| creado_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| creado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| actualizado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| actualizado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| desactivado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| desactivado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| motivo_desactivacion | nvarchar(300) NULL | Justificación de la operación; se conserva como parte de su historial. |
| activo | AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED | Estado lógico; revisar si es columna calculada a partir de desactivado_en o bit persistido en esta tabla. |
| version_fila | rowversion NOT NULL | Token rowversion para detectar modificaciones concurrentes; no es una fecha. |

Clave primaria: id_empleado

- id_tienda → Tienda(id_tienda).

Reglas CHECK: CK_Empleado_desactivacion. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.7 dbo.Usuario

Cuenta de autenticación, hash de contraseña, bloqueo y versión de seguridad.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 130.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_usuario | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_empleado | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| nombre_usuario | nvarchar(50) NOT NULL | Dato de nombre usuario de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| nombre_usuario_normalizado | AS (UPPER(LTRIM(RTRIM(nombre_usuario)))) PERSISTED | Nombre de acceso normalizado para impedir duplicados por espacios o capitalización. |
| contrasena_hash | nvarchar(512) NOT NULL | Hash PBKDF2 de la contraseña; no almacena la clave en texto plano. |
| contrasena_actualizada_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| debe_cambiar_contrasena | bit NOT NULL DEFAULT 1 | Obliga a cambiar la clave antes de crear una sesión operativa. |
| intentos_fallidos | int NOT NULL DEFAULT 0 | Contador utilizado para el bloqueo temporal del acceso. |
| bloqueado_hasta | datetime2(3) NULL | Dato de bloqueado hasta de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| ultimo_acceso_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| version_seguridad | uniqueidentifier NOT NULL DEFAULT NEWID() | Versión utilizada para invalidar sesiones después de cambios relevantes. |
| creado_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| creado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| actualizado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| actualizado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| desactivado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| desactivado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| motivo_desactivacion | nvarchar(300) NULL | Justificación de la operación; se conserva como parte de su historial. |
| activo | AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED | Estado lógico; revisar si es columna calculada a partir de desactivado_en o bit persistido en esta tabla. |
| version_fila | rowversion NOT NULL | Token rowversion para detectar modificaciones concurrentes; no es una fecha. |

Clave primaria: id_usuario

- id_empleado → Empleado(id_empleado).

Reglas CHECK: CK_Usuario_intentos; CK_Usuario_desactivacion. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.8 dbo.Rol

Agrupa responsabilidades y permisos administrativos.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 159.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_rol | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| codigo | varchar(30) NOT NULL | Dato de codigo de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| nombre | nvarchar(80) NOT NULL | Dato de nombre de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| descripcion | nvarchar(300) NULL | Dato de descripcion de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| creado_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| creado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| actualizado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| actualizado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| desactivado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| desactivado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| motivo_desactivacion | nvarchar(300) NULL | Justificación de la operación; se conserva como parte de su historial. |
| activo | AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED | Estado lógico; revisar si es columna calculada a partir de desactivado_en o bit persistido en esta tabla. |
| version_fila | rowversion NOT NULL | Token rowversion para detectar modificaciones concurrentes; no es una fecha. |

Clave primaria: id_rol


Reglas CHECK: CK_Rol_desactivacion. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.9 dbo.Permiso

Define la combinación módulo/acción; su código se calcula en SQL.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 178.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_permiso | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| modulo | varchar(50) NOT NULL | Dato de modulo de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| accion | varchar(30) NOT NULL | Dato de accion de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| codigo | AS (modulo + '.' + accion) PERSISTED | Dato de codigo de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| descripcion | nvarchar(300) NOT NULL | Dato de descripcion de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| creado_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| creado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| actualizado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| actualizado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| desactivado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| desactivado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| motivo_desactivacion | nvarchar(300) NULL | Justificación de la operación; se conserva como parte de su historial. |
| activo | AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED | Estado lógico; revisar si es columna calculada a partir de desactivado_en o bit persistido en esta tabla. |
| version_fila | rowversion NOT NULL | Token rowversion para detectar modificaciones concurrentes; no es una fecha. |

Clave primaria: id_permiso


Reglas CHECK: CK_Permiso_desactivacion. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.10 dbo.UsuarioRol

Relaciona usuarios y roles, con asignación activa o retirada.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 198.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_usuario | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_rol | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| activo | bit NOT NULL DEFAULT 1 | Estado lógico; revisar si es columna calculada a partir de desactivado_en o bit persistido en esta tabla. |
| asignado_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| asignado_por | int NULL | Dato de asignado por de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |

Clave primaria: id_usuario, id_rol

- id_usuario → Usuario(id_usuario).
- id_rol → Rol(id_rol).
- asignado_por → Usuario(id_usuario).

## 15.11 dbo.RolPermiso

Relaciona roles y permisos, conservando relaciones inactivas.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 210.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_rol | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_permiso | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| activo | bit NOT NULL DEFAULT 1 | Estado lógico; revisar si es columna calculada a partir de desactivado_en o bit persistido en esta tabla. |
| asignado_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| asignado_por | int NULL | Dato de asignado por de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |

Clave primaria: id_rol, id_permiso

- id_rol → Rol(id_rol).
- id_permiso → Permiso(id_permiso).
- asignado_por → Usuario(id_usuario).

## 15.12 dbo.UsuarioTienda

Modelo de asignación usuario/sucursal; no dispone de administración ni aplicación granular en las pantallas actuales.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 222.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_usuario | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_tienda | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| activo | bit NOT NULL DEFAULT 1 | Estado lógico; revisar si es columna calculada a partir de desactivado_en o bit persistido en esta tabla. |
| asignado_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| asignado_por | int NULL | Dato de asignado por de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |

Clave primaria: id_usuario, id_tienda

- id_usuario → Usuario(id_usuario).
- id_tienda → Tienda(id_tienda).
- asignado_por → Usuario(id_usuario).

## 15.13 dbo.SesionUsuario

Registra sesiones con expiración, revocación, token representado por un hash y versión de seguridad.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 234.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_sesion | bigint IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_usuario | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| token_hash | binary(32) NOT NULL | Hash del token de sesión o de recuperación. |
| version_seguridad | uniqueidentifier NOT NULL | Versión utilizada para invalidar sesiones después de cambios relevantes. |
| creada_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| expira_en | datetime2(3) NOT NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| ultima_actividad_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| revocada_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| equipo | nvarchar(150) NULL | Nombre del equipo desde el que se registra la operación. |
| direccion_ip | varchar(45) NULL | Dato de direccion ip de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |

Clave primaria: id_sesion

- id_usuario → Usuario(id_usuario).

Reglas CHECK: CK_SesionUsuario_expira. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.14 dbo.EventoAcceso

Historial de intentos de acceso, resultado y equipo.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 251.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_evento_acceso | bigint IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_usuario | int NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| nombre_intentado | nvarchar(50) NOT NULL | Dato de nombre intentado de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| fecha | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| resultado | varchar(30) NOT NULL | Dato de resultado de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| equipo | nvarchar(150) NULL | Nombre del equipo desde el que se registra la operación. |
| direccion_ip | varchar(45) NULL | Dato de direccion ip de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |

Clave primaria: id_evento_acceso

- id_usuario → Usuario(id_usuario).

Reglas CHECK: CK_EventoAcceso_resultado. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.15 dbo.RecuperacionAcceso

Estructura para tokens de recuperación; no se encontró un flujo de recuperación expuesto en la aplicación.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 264.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_recuperacion | bigint IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_usuario | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| token_hash | binary(32) NOT NULL | Hash del token de sesión o de recuperación. |
| creado_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| expira_en | datetime2(3) NOT NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| utilizado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| revocado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |

Clave primaria: id_recuperacion

- id_usuario → Usuario(id_usuario).

Reglas CHECK: CK_RecuperacionAcceso_expira. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.16 dbo.Producto

Maestro de artículos, código de barras, precio base, categoría y proveedor; el stock se conserva por tienda.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 278.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_producto | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| codigo_barras | nvarchar(50) NOT NULL | Código único del producto, conservado como texto para no perder ceros. |
| nombre | nvarchar(150) NOT NULL | Dato de nombre de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| precio_unitario | decimal(10,2) NOT NULL | Importe unitario base con la precisión decimal declarada. |
| id_categoria | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_proveedor | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| creado_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| creado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| actualizado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| actualizado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| desactivado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| desactivado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| motivo_desactivacion | nvarchar(300) NULL | Justificación de la operación; se conserva como parte de su historial. |
| activo | AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED | Estado lógico; revisar si es columna calculada a partir de desactivado_en o bit persistido en esta tabla. |
| version_fila | rowversion NOT NULL | Token rowversion para detectar modificaciones concurrentes; no es una fecha. |

Clave primaria: id_producto

- id_categoria → Categoria(id_categoria).
- id_proveedor → Proveedor(id_proveedor).

Reglas CHECK: CK_Producto_precio; CK_Producto_desactivacion. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.17 dbo.InventarioTienda

Saldo físico, reservas y mínimo para cada pareja tienda/producto.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 302.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_tienda | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_producto | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| existencia | int NOT NULL DEFAULT 0 | Cantidad física disponible en la tienda antes de descontar reservas. |
| reservado | int NOT NULL DEFAULT 0 | Unidades comprometidas; deben estar entre cero y la existencia. |
| stock_minimo | int NOT NULL DEFAULT 0 | Umbral no negativo utilizado para generar alertas de disponible. |
| version_fila | rowversion NOT NULL | Token rowversion para detectar modificaciones concurrentes; no es una fecha. |

Clave primaria: id_tienda, id_producto

- id_tienda → Tienda(id_tienda).
- id_producto → Producto(id_producto).

Reglas CHECK: CK_InventarioTienda_existencia; CK_InventarioTienda_reservado; CK_InventarioTienda_minimo. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.18 dbo.Compra

Cabecera de compra por proveedor y tienda, con factura y estado.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 317.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_compra | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_proveedor | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_tienda | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| numero_factura | nvarchar(50) NOT NULL | Dato de numero factura de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| fecha_factura | date NOT NULL | Fecha de negocio sin conversión implícita a la hora local. |
| registrado_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| id_usuario | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| estado | varchar(20) NOT NULL DEFAULT 'BORRADOR' | Etapa del documento o proceso; los CHECK y triggers limitan los valores y transiciones. |
| observaciones | nvarchar(500) NULL | Dato de observaciones de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| anulado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| anulado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| motivo_anulacion | nvarchar(500) NULL | Justificación de la operación; se conserva como parte de su historial. |

Clave primaria: id_compra

- id_proveedor → Proveedor(id_proveedor).
- id_tienda → Tienda(id_tienda).
- id_usuario → Usuario(id_usuario).
- anulado_por → Usuario(id_usuario).

Reglas CHECK: CK_Compra_estado; CK_Compra_anulacion. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.19 dbo.DetalleCompra

Productos, unidades y costo de cada compra; calcula su subtotal.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 340.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_detalle_compra | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_compra | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_producto | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| cantidad | int NOT NULL | Dato de cantidad de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| costo_unitario | decimal(18,2) NOT NULL | Importe unitario base con la precisión decimal declarada. |
| subtotal | AS (CONVERT(decimal(18,2), cantidad * costo_unitario)) | Importe base de cantidad por precio o costo, sin ISV. |

Clave primaria: id_detalle_compra

- id_compra → Compra(id_compra).
- id_producto → Producto(id_producto).

Reglas CHECK: CK_DetalleCompra_cantidad; CK_DetalleCompra_costo. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.20 dbo.Venta

Factura emitida, cliente, tienda, usuario, estado, RTN y CAI histórico.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 354.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_venta | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| numero | varchar(40) NOT NULL | Dato de numero de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| id_tienda | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_cliente | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| rtn_cliente | varchar(30) NULL | RTN opcional conservado con la venta; nulo o exactamente 14 dígitos. |
| cai | varchar(37) NULL | Código de autorización: configuración vigente o copia histórica en la factura. |
| id_usuario | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| fecha | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| estado | varchar(20) NOT NULL DEFAULT 'BORRADOR' | Etapa del documento o proceso; los CHECK y triggers limitan los valores y transiciones. |
| observaciones | nvarchar(500) NULL | Dato de observaciones de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| anulado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| anulado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| motivo_anulacion | nvarchar(500) NULL | Justificación de la operación; se conserva como parte de su historial. |

Clave primaria: id_venta

- id_tienda → Tienda(id_tienda).
- id_cliente → Cliente(id_cliente).
- id_usuario → Usuario(id_usuario).
- anulado_por → Usuario(id_usuario).

Reglas CHECK: CK_Venta_rtn_cliente; CK_Venta_estado; CK_Venta_anulacion. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.21 dbo.DetalleVenta

Líneas históricas de venta: unidades, nombre, precio e ISV.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 379.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_detalle_venta | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_venta | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_producto | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| cantidad | int NOT NULL | Dato de cantidad de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| precio_unitario | decimal(18,2) NOT NULL | Importe unitario base con la precisión decimal declarada. |
| nombre_producto | nvarchar(150) NOT NULL | Dato de nombre producto de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| tasa_isv | decimal(5,2) NOT NULL CONSTRAINT DF_DetalleVenta_tasa_isv DEFAULT 15 | Tasa guardada por línea: 0, 15 o 18; el valor predeterminado nuevo es 15. |
| subtotal | AS (CONVERT(decimal(18,2), cantidad * precio_unitario)) | Importe base de cantidad por precio o costo, sin ISV. |
| isv | AS (CONVERT(decimal(18,2), ROUND(cantidad * precio_unitario * tasa_isv / 100,2))) | Impuesto de la línea; calculado en venta/apartado y almacenado en devolución. |
| total_linea | AS (CONVERT(decimal(18,2), cantidad * precio_unitario + ROUND(cantidad * precio_unitario * tasa_isv / 100,2))) | Subtotal más impuesto de la línea. |

Clave primaria: id_detalle_venta

- id_venta → Venta(id_venta).
- id_producto → Producto(id_producto).

Reglas CHECK: CK_DetalleVenta_cantidad; CK_DetalleVenta_tasa_isv; CK_DetalleVenta_precio. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.22 dbo.PagoVenta

Cobros aplicados a una venta y monto recibido o referencia.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 398.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_pago_venta | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_venta | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_metodo_pago | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| monto | decimal(18,2) NOT NULL | Importe monetario; revisar restricciones de positividad y relación con el cobro. |
| monto_recibido | decimal(18,2) NULL | Importe monetario; revisar restricciones de positividad y relación con el cobro. |
| referencia | nvarchar(100) NULL | Dato de referencia de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| fecha | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |

Clave primaria: id_pago_venta

- id_venta → Venta(id_venta).
- id_metodo_pago → MetodoPago(id_metodo_pago).

Reglas CHECK: CK_PagoVenta_monto; CK_PagoVenta_recibido. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.23 dbo.Traspaso

Solicitud de movimiento entre dos tiendas, con estado y solicitante.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 413.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_traspaso | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_tienda_origen | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_tienda_destino | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| estado | varchar(20) NOT NULL DEFAULT 'PENDIENTE' | Etapa del documento o proceso; los CHECK y triggers limitan los valores y transiciones. |
| solicitado_por | int NOT NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| solicitado_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| observaciones | nvarchar(500) NULL | Dato de observaciones de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| version_fila | rowversion NOT NULL | Token rowversion para detectar modificaciones concurrentes; no es una fecha. |

Clave primaria: id_traspaso

- id_tienda_origen → Tienda(id_tienda).
- id_tienda_destino → Tienda(id_tienda).
- solicitado_por → Usuario(id_usuario).

Reglas CHECK: CK_Traspaso_tiendas; CK_Traspaso_estado. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.24 dbo.DetalleTraspaso

Productos y cantidades completas a mover entre sucursales.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 430.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_detalle_traspaso | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_traspaso | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_producto | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| cantidad | int NOT NULL | Dato de cantidad de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| precio_referencia | decimal(18,2) NOT NULL | Importe unitario base con la precisión decimal declarada. |

Clave primaria: id_detalle_traspaso

- id_traspaso → Traspaso(id_traspaso).
- id_producto → Producto(id_producto).

Reglas CHECK: CK_DetalleTraspaso_cantidad; CK_DetalleTraspaso_precio. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.25 dbo.EventoTraspaso

Historial inmutable de las etapas del traspaso y sus operadores.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 444.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_evento_traspaso | bigint IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_traspaso | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| estado_anterior | varchar(20) NULL | Dato de estado anterior de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| estado_nuevo | varchar(20) NOT NULL | Dato de estado nuevo de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| id_usuario | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| fecha | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| motivo | nvarchar(500) NULL | Justificación de la operación; se conserva como parte de su historial. |

Clave primaria: id_evento_traspaso

- id_traspaso → Traspaso(id_traspaso).
- id_usuario → Usuario(id_usuario).

Reglas CHECK: CK_EventoTraspaso_anterior; CK_EventoTraspaso_nuevo; CK_EventoTraspaso_motivo. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.26 dbo.Apartado

Reserva de un cliente en una tienda, con vencimiento y venta de entrega opcional.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 461.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_apartado | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_cliente | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_tienda | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_usuario | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| fecha | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| vence_en | datetime2(3) NOT NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| estado | varchar(20) NOT NULL DEFAULT 'ACTIVO' | Etapa del documento o proceso; los CHECK y triggers limitan los valores y transiciones. |
| id_venta | int NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| version_fila | rowversion NOT NULL | Token rowversion para detectar modificaciones concurrentes; no es una fecha. |

Clave primaria: id_apartado

- id_cliente → Cliente(id_cliente).
- id_tienda → Tienda(id_tienda).
- id_usuario → Usuario(id_usuario).
- id_venta → Venta(id_venta).

Reglas CHECK: CK_Apartado_vence; CK_Apartado_estado; CK_Apartado_entrega. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.27 dbo.DetalleApartado

Cantidades, precio e ISV reservados para el cliente.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 481.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_detalle_apartado | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_apartado | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_producto | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| cantidad | int NOT NULL | Dato de cantidad de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| precio_unitario | decimal(18,2) NOT NULL | Importe unitario base con la precisión decimal declarada. |
| tasa_isv | decimal(5,2) NOT NULL CONSTRAINT DF_DetalleApartado_tasa_isv DEFAULT 15 | Tasa guardada por línea: 0, 15 o 18; el valor predeterminado nuevo es 15. |
| subtotal | AS (CONVERT(decimal(18,2), cantidad * precio_unitario)) | Importe base de cantidad por precio o costo, sin ISV. |
| isv | AS (CONVERT(decimal(18,2), ROUND(cantidad * precio_unitario * tasa_isv / 100,2))) | Impuesto de la línea; calculado en venta/apartado y almacenado en devolución. |
| total_linea | AS (CONVERT(decimal(18,2), cantidad * precio_unitario + ROUND(cantidad * precio_unitario * tasa_isv / 100,2))) | Subtotal más impuesto de la línea. |

Clave primaria: id_detalle_apartado

- id_apartado → Apartado(id_apartado).
- id_producto → Producto(id_producto).

Reglas CHECK: CK_DetalleApartado_cantidad; CK_DetalleApartado_tasa_isv; CK_DetalleApartado_precio. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.28 dbo.AbonoApartado

Cobros parciales del apartado, enlazables una sola vez a pagos de la venta de entrega.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 499.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_abono | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_apartado | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_metodo_pago | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| monto | decimal(18,2) NOT NULL | Importe monetario; revisar restricciones de positividad y relación con el cobro. |
| fecha | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| id_usuario | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| referencia | nvarchar(100) NULL | Dato de referencia de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| id_pago_venta | int NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |

Clave primaria: id_abono

- id_apartado → Apartado(id_apartado).
- id_metodo_pago → MetodoPago(id_metodo_pago).
- id_usuario → Usuario(id_usuario).
- id_pago_venta → PagoVenta(id_pago_venta).

Reglas CHECK: CK_AbonoApartado_monto. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.29 dbo.Devolucion

Documento relacionado con una factura original y una tienda, con motivo y estado.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 516.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_devolucion | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_venta | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_tienda | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_usuario | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| fecha | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| motivo | nvarchar(500) NOT NULL | Justificación de la operación; se conserva como parte de su historial. |
| estado | varchar(20) NOT NULL DEFAULT 'BORRADOR' | Etapa del documento o proceso; los CHECK y triggers limitan los valores y transiciones. |
| anulado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| anulado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| motivo_anulacion | nvarchar(500) NULL | Justificación de la operación; se conserva como parte de su historial. |

Clave primaria: id_devolucion

- id_venta → Venta(id_venta).
- id_tienda → Tienda(id_tienda).
- id_usuario → Usuario(id_usuario).
- anulado_por → Usuario(id_usuario).

Reglas CHECK: CK_Devolucion_estado; CK_Devolucion_anulacion. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.30 dbo.DetalleDevolucion

Cantidades devueltas de líneas de venta, impuesto y decisión de reintegrar stock.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 536.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_detalle_devolucion | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_devolucion | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_detalle_venta | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| cantidad | int NOT NULL | Dato de cantidad de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| reintegra_stock | bit NOT NULL | Indica si las unidades devueltas vuelven al inventario. |
| isv | decimal(18,2) NOT NULL CONSTRAINT DF_DetalleDevolucion_isv DEFAULT 0 | Impuesto de la línea; calculado en venta/apartado y almacenado en devolución. |

Clave primaria: id_detalle_devolucion

- id_devolucion → Devolucion(id_devolucion).
- id_detalle_venta → DetalleVenta(id_detalle_venta).

Reglas CHECK: CK_DetalleDevolucion_cantidad; CK_DetalleDevolucion_isv. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.31 dbo.Reembolso

Salida de dinero ligada a un pago de venta o a un abono, sin exceder el cobro original.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 550.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_reembolso | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_pago_venta | int NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_abono | int NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_devolucion | int NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| monto | decimal(18,2) NOT NULL | Importe monetario; revisar restricciones de positividad y relación con el cobro. |
| id_metodo_pago | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_usuario | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| fecha | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| referencia | nvarchar(100) NULL | Dato de referencia de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| motivo | nvarchar(500) NOT NULL | Justificación de la operación; se conserva como parte de su historial. |

Clave primaria: id_reembolso

- id_pago_venta → PagoVenta(id_pago_venta).
- id_abono → AbonoApartado(id_abono).
- id_devolucion → Devolucion(id_devolucion).
- id_metodo_pago → MetodoPago(id_metodo_pago).
- id_usuario → Usuario(id_usuario).

Reglas CHECK: CK_Reembolso_monto; CK_Reembolso_cobro. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.32 dbo.MovimientoInventario

Libro inmutable de variaciones de unidades, documento de origen y reverso opcional.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 571.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_movimiento | bigint IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_tienda | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_producto | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| fecha | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| tipo | varchar(30) NOT NULL | Dato de tipo de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| cantidad_delta | int NOT NULL | Variación firmada del inventario: positiva para entrada, negativa para salida. |
| id_usuario | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_detalle_compra | int NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_detalle_venta | int NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_detalle_traspaso | int NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_detalle_devolucion | int NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_movimiento_revertido | bigint NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| motivo | nvarchar(500) NULL | Justificación de la operación; se conserva como parte de su historial. |

Clave primaria: id_movimiento

- id_tienda → Tienda(id_tienda).
- id_producto → Producto(id_producto).
- id_usuario → Usuario(id_usuario).
- id_detalle_compra → DetalleCompra(id_detalle_compra).
- id_detalle_venta → DetalleVenta(id_detalle_venta).
- id_detalle_traspaso → DetalleTraspaso(id_detalle_traspaso).
- id_detalle_devolucion → DetalleDevolucion(id_detalle_devolucion).
- id_movimiento_revertido → MovimientoInventario(id_movimiento).

Reglas CHECK: CK_Movimiento_tipo; CK_Movimiento_delta; CK_Movimiento_origen. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.33 dbo.Inversion

Estructura de inversiones con monto, tasa y vencimiento; no se identificó una pantalla operativa dedicada.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 607.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_inversion | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| institucion | nvarchar(150) NOT NULL | Dato de institucion de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| tipo | varchar(20) NOT NULL | Dato de tipo de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| monto | decimal(18,2) NOT NULL | Importe monetario; revisar restricciones de positividad y relación con el cobro. |
| tasa_interes | decimal(5,2) NOT NULL | Dato de tasa interes de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| fecha_inicio | date NOT NULL | Fecha de negocio sin conversión implícita a la hora local. |
| fecha_vencimiento | date NOT NULL | Fecha de negocio sin conversión implícita a la hora local. |
| estado | varchar(20) NOT NULL | Etapa del documento o proceso; los CHECK y triggers limitan los valores y transiciones. |
| creado_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| creado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| actualizado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| actualizado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| desactivado_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| desactivado_por | int NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| motivo_desactivacion | nvarchar(300) NULL | Justificación de la operación; se conserva como parte de su historial. |
| activo | AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED | Estado lógico; revisar si es columna calculada a partir de desactivado_en o bit persistido en esta tabla. |
| version_fila | rowversion NOT NULL | Token rowversion para detectar modificaciones concurrentes; no es una fecha. |

Clave primaria: id_inversion


Reglas CHECK: CK_Inversion_tipo; CK_Inversion_monto; CK_Inversion_tasa; CK_Inversion_fechas; CK_Inversion_estado; CK_Inversion_desactivacion. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.34 dbo.Importacion

Cabecera de un lote importado: archivo, SHA-256, usuario y estado.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 634.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_importacion | int IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| nombre_archivo | nvarchar(260) NOT NULL | Dato de nombre archivo de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| archivo_hash | binary(32) NOT NULL | Huella SHA-256 de 32 bytes del archivo de importación. |
| id_usuario | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| fecha | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| estado | varchar(20) NOT NULL | Etapa del documento o proceso; los CHECK y triggers limitan los valores y transiciones. |

Clave primaria: id_importacion

- id_usuario → Usuario(id_usuario).

Reglas CHECK: CK_Importacion_estado. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.35 dbo.DetalleImportacion

Trazabilidad por fila del archivo, datos JSON, estado, error y producto resultante.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 646.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_detalle_importacion | bigint IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_importacion | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| numero_fila | int NOT NULL | Número de fila original del Excel, único dentro del lote. |
| datos_originales_json | nvarchar(max) NOT NULL | Datos estructurados JSON; las restricciones comprueban su formato cuando corresponde. |
| estado | varchar(20) NOT NULL | Etapa del documento o proceso; los CHECK y triggers limitan los valores y transiciones. |
| error | nvarchar(1000) NULL | Dato de error de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| id_producto | int NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |

Clave primaria: id_detalle_importacion

- id_importacion → Importacion(id_importacion).
- id_producto → Producto(id_producto).

Reglas CHECK: CK_DetalleImportacion_fila; CK_DetalleImportacion_json; CK_DetalleImportacion_estado. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.36 dbo.ConfiguracionFacturacion

CAI vigente por tienda, actor y versión de concurrencia.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 663.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_tienda | int NOT NULL CONSTRAINT PK_ConfiguracionFacturacion PRIMARY KEY | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| cai | varchar(37) NULL | Código de autorización: configuración vigente o copia histórica en la factura. |
| actualizado_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| actualizado_por | int NOT NULL | Usuario responsable o afectado por la operación indicada, según la relación de la tabla. |
| version_fila | rowversion NOT NULL | Token rowversion para detectar modificaciones concurrentes; no es una fecha. |

Clave primaria: id_tienda (declarada en la columna).

- id_tienda → Tienda(id_tienda).
- actualizado_por → Usuario(id_usuario).

Reglas CHECK: CK_ConfiguracionFacturacion_cai. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.37 dbo.Auditoria

Bitácora de operaciones, navegaciones y exportaciones con identidad y equipo.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 674.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_auditoria | bigint IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| fecha | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |
| id_usuario | int NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| modulo | varchar(50) NOT NULL | Dato de modulo de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| accion | varchar(50) NOT NULL | Dato de accion de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| entidad | varchar(80) NOT NULL | Dato de entidad de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| id_registro | nvarchar(100) NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| descripcion | nvarchar(1000) NOT NULL | Dato de descripcion de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| valores_antes_json | nvarchar(max) NULL | Datos estructurados JSON; las restricciones comprueban su formato cuando corresponde. |
| valores_despues_json | nvarchar(max) NULL | Datos estructurados JSON; las restricciones comprueban su formato cuando corresponde. |
| equipo | nvarchar(150) NULL | Nombre del equipo desde el que se registra la operación. |

Clave primaria: id_auditoria

- id_usuario → Usuario(id_usuario).

Reglas CHECK: CK_Auditoria_antes_json; CK_Auditoria_despues_json. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.38 dbo.EventoNotificacion

Historial de eventos producidos por triggers o por procesamiento de vencimientos.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 692.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_evento_notificacion | bigint IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| clave_evento | varchar(150) NOT NULL | Dato de clave evento de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| tipo | varchar(40) NOT NULL | Dato de tipo de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| titulo | nvarchar(150) NOT NULL | Dato de titulo de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| mensaje | nvarchar(500) NOT NULL | Dato de mensaje de esta entidad; tipo, nulabilidad y valor inicial se indican en la declaración. |
| creada_en | datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME() | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |

Clave primaria: id_evento_notificacion


Reglas CHECK: CK_EventoNotificacion_tipo. Sus condiciones literales se encuentran en el DDL del anexo A.

## 15.39 dbo.Notificacion

Relación evento/destinatario y lectura; la campana actual consulta stock directamente.

Fuente: Base de Datos/01_esquema_erp_3fn.sql, línea 704.

| Campo | Declaración SQL | Finalidad |
|---|---|---|
| id_notificacion | bigint IDENTITY(1,1) NOT NULL | Identificador generado automáticamente para esta fila. |
| id_evento_notificacion | bigint NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| id_usuario | int NOT NULL | Identificador o referencia relacional; revisar PK y FK de la tabla. |
| leida_en | datetime2(3) NULL | Marca de fecha y hora; los DEFAULT SYSUTCDATETIME almacenan UTC. |

Clave primaria: id_notificacion

- id_evento_notificacion → EventoNotificacion(id_evento_notificacion).
- id_usuario → Usuario(id_usuario).

# Anexo A. Todos los scripts SQL del repositorio

Los siguientes bloques reproducen el contenido íntegro de cada archivo .sql localizado, incluidos comentarios, datos de demostración, migraciones, pruebas y redefiniciones. El orden del anexo es un catálogo; el orden recomendado de instalación se explica en el capítulo 11. GO separa lotes del cliente SQL y no es una instrucción enviada mediante SqlCommand. No ejecutar el anexo como un único script: contiene reinicio de esquema y pruebas con efectos diferentes.

## A.1 Base de Datos/00_reiniciar_esquema.sql

Utilidad que genera texto de DROP VIEW, eliminación de claves foráneas y DROP TABLE para dbo. En esta versión solo lo muestra con PRINT @sql; no ejecuta las eliminaciones ni incluye procedimientos. El texto producido sería destructivo si se ejecuta manualmente.

Extensión: 37 líneas. SHA-256 del texto normalizado a LF: a0108ea6d5aecd89c0c486ea3dc851b4b7be34e40ff715dc36e6cc8c7ea5a0d0.

```sql















DECLARE @sql nvarchar(max) = N'';


SELECT @sql += N'DROP VIEW dbo.' + QUOTENAME(name) + N';' + CHAR(10)
FROM sys.views
WHERE schema_id = SCHEMA_ID('dbo');


SELECT @sql += N'ALTER TABLE dbo.' + QUOTENAME(OBJECT_NAME(parent_object_id))
             + N' DROP CONSTRAINT ' + QUOTENAME(name) + N';' + CHAR(10)
FROM sys.foreign_keys
WHERE schema_id = SCHEMA_ID('dbo');


SELECT @sql += N'DROP TABLE dbo.' + QUOTENAME(name) + N';' + CHAR(10)
FROM sys.tables
WHERE schema_id = SCHEMA_ID('dbo');

PRINT @sql;

GO
```

## A.2 Base de Datos/01_esquema_erp_3fn.sql

Crea la base EquipoZAACC si no existe, selecciona la base y crea el esquema 3FN, índices, vistas y catálogos iniciales. Requiere una base sin las tablas de este esquema.

Extensión: 911 líneas. SHA-256 del texto normalizado a LF: 42d672ddf35a5598ab682f1c073cdbd097ffe0773fa251dd279245926d18e466.

```sql

IF DB_ID(N'EquipoZAACC') IS NULL
    EXEC(N'CREATE DATABASE [EquipoZAACC]');
GO
USE EquipoZAACC;
GO
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO
CREATE TABLE dbo.Categoria (
    id_categoria         int IDENTITY(1,1) NOT NULL,
    nombre               nvarchar(100) NOT NULL,
    descripcion          nvarchar(300) NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_Categoria PRIMARY KEY (id_categoria),
    CONSTRAINT UQ_Categoria_nombre UNIQUE (nombre),
    CONSTRAINT CK_Categoria_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.Proveedor (
    id_proveedor         int IDENTITY(1,1) NOT NULL,
    nombre_empresa       nvarchar(150) NOT NULL,
    rtn                  varchar(30) NULL,
    contacto             nvarchar(150) NULL,
    telefono             nvarchar(30) NULL,
    correo               nvarchar(254) NULL,
    direccion            nvarchar(300) NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_Proveedor PRIMARY KEY (id_proveedor),
    CONSTRAINT CK_Proveedor_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.Cliente (
    id_cliente           int IDENTITY(1,1) NOT NULL,
    nombre               nvarchar(150) NOT NULL,
    documento_fiscal     varchar(30) NULL,
    telefono             nvarchar(30) NULL,
    correo               nvarchar(254) NULL,
    direccion            nvarchar(300) NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_Cliente PRIMARY KEY (id_cliente),
    CONSTRAINT CK_Cliente_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.Tienda (
    id_tienda            int IDENTITY(1,1) NOT NULL,
    codigo               varchar(20) NOT NULL,
    nombre               nvarchar(100) NOT NULL,
    direccion            nvarchar(300) NULL,
    telefono             nvarchar(30) NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_Tienda PRIMARY KEY (id_tienda),
    CONSTRAINT UQ_Tienda_codigo UNIQUE (codigo),
    CONSTRAINT CK_Tienda_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.MetodoPago (
    id_metodo_pago       int IDENTITY(1,1) NOT NULL,
    codigo               varchar(30) NOT NULL,
    nombre               nvarchar(80) NOT NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_MetodoPago PRIMARY KEY (id_metodo_pago),
    CONSTRAINT UQ_MetodoPago_codigo UNIQUE (codigo),
    CONSTRAINT CK_MetodoPago_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.Empleado (
    id_empleado          int IDENTITY(1,1) NOT NULL,
    codigo               varchar(30) NOT NULL,
    nombre               nvarchar(150) NOT NULL,
    telefono             nvarchar(30) NULL,
    correo               nvarchar(254) NULL,
    id_tienda            int NOT NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_Empleado PRIMARY KEY (id_empleado),
    CONSTRAINT UQ_Empleado_codigo UNIQUE (codigo),
    CONSTRAINT FK_Empleado_Tienda FOREIGN KEY (id_tienda) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT CK_Empleado_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.Usuario (
    id_usuario                 int IDENTITY(1,1) NOT NULL,
    id_empleado                int NOT NULL,
    nombre_usuario             nvarchar(50) NOT NULL,
    nombre_usuario_normalizado AS (UPPER(LTRIM(RTRIM(nombre_usuario)))) PERSISTED,
    contrasena_hash            nvarchar(512) NOT NULL,
    contrasena_actualizada_en  datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    debe_cambiar_contrasena    bit NOT NULL DEFAULT 1,
    intentos_fallidos          int NOT NULL DEFAULT 0,
    bloqueado_hasta            datetime2(3) NULL,
    ultimo_acceso_en           datetime2(3) NULL,
    version_seguridad          uniqueidentifier NOT NULL DEFAULT NEWID(),
    creado_en                  datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por                 int NULL,
    actualizado_en             datetime2(3) NULL,
    actualizado_por            int NULL,
    desactivado_en             datetime2(3) NULL,
    desactivado_por            int NULL,
    motivo_desactivacion       nvarchar(300) NULL,
    activo                     AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila               rowversion NOT NULL,
    CONSTRAINT PK_Usuario PRIMARY KEY (id_usuario),
    CONSTRAINT UQ_Usuario_empleado UNIQUE (id_empleado),
    CONSTRAINT UQ_Usuario_normalizado UNIQUE (nombre_usuario_normalizado),
    CONSTRAINT FK_Usuario_Empleado FOREIGN KEY (id_empleado) REFERENCES dbo.Empleado (id_empleado),
    CONSTRAINT CK_Usuario_intentos CHECK (intentos_fallidos >= 0),
    CONSTRAINT CK_Usuario_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.Rol (
    id_rol               int IDENTITY(1,1) NOT NULL,
    codigo               varchar(30) NOT NULL,
    nombre               nvarchar(80) NOT NULL,
    descripcion          nvarchar(300) NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_Rol PRIMARY KEY (id_rol),
    CONSTRAINT UQ_Rol_codigo UNIQUE (codigo),
    CONSTRAINT CK_Rol_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.Permiso (
    id_permiso           int IDENTITY(1,1) NOT NULL,
    modulo               varchar(50) NOT NULL,
    accion               varchar(30) NOT NULL,
    codigo               AS (modulo + '.' + accion) PERSISTED,
    descripcion          nvarchar(300) NOT NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_Permiso PRIMARY KEY (id_permiso),
    CONSTRAINT UQ_Permiso_modulo_accion UNIQUE (modulo, accion),
    CONSTRAINT CK_Permiso_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.UsuarioRol (
    id_usuario   int NOT NULL,
    id_rol       int NOT NULL,
    activo       bit NOT NULL DEFAULT 1,
    asignado_en  datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    asignado_por int NULL,
    CONSTRAINT PK_UsuarioRol PRIMARY KEY (id_usuario, id_rol),
    CONSTRAINT FK_UsuarioRol_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT FK_UsuarioRol_Rol FOREIGN KEY (id_rol) REFERENCES dbo.Rol (id_rol),
    CONSTRAINT FK_UsuarioRol_asignado_por FOREIGN KEY (asignado_por) REFERENCES dbo.Usuario (id_usuario)
);
GO
CREATE TABLE dbo.RolPermiso (
    id_rol       int NOT NULL,
    id_permiso   int NOT NULL,
    activo       bit NOT NULL DEFAULT 1,
    asignado_en  datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    asignado_por int NULL,
    CONSTRAINT PK_RolPermiso PRIMARY KEY (id_rol, id_permiso),
    CONSTRAINT FK_RolPermiso_Rol FOREIGN KEY (id_rol) REFERENCES dbo.Rol (id_rol),
    CONSTRAINT FK_RolPermiso_Permiso FOREIGN KEY (id_permiso) REFERENCES dbo.Permiso (id_permiso),
    CONSTRAINT FK_RolPermiso_asignado_por FOREIGN KEY (asignado_por) REFERENCES dbo.Usuario (id_usuario)
);
GO
CREATE TABLE dbo.UsuarioTienda (
    id_usuario   int NOT NULL,
    id_tienda    int NOT NULL,
    activo       bit NOT NULL DEFAULT 1,
    asignado_en  datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    asignado_por int NULL,
    CONSTRAINT PK_UsuarioTienda PRIMARY KEY (id_usuario, id_tienda),
    CONSTRAINT FK_UsuarioTienda_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT FK_UsuarioTienda_Tienda FOREIGN KEY (id_tienda) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT FK_UsuarioTienda_asignado_por FOREIGN KEY (asignado_por) REFERENCES dbo.Usuario (id_usuario)
);
GO
CREATE TABLE dbo.SesionUsuario (
    id_sesion           bigint IDENTITY(1,1) NOT NULL,
    id_usuario          int NOT NULL,
    token_hash          binary(32) NOT NULL,
    version_seguridad   uniqueidentifier NOT NULL,
    creada_en           datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    expira_en           datetime2(3) NOT NULL,
    ultima_actividad_en datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    revocada_en         datetime2(3) NULL,
    equipo              nvarchar(150) NULL,
    direccion_ip        varchar(45) NULL,
    CONSTRAINT PK_SesionUsuario PRIMARY KEY (id_sesion),
    CONSTRAINT UQ_SesionUsuario_token UNIQUE (token_hash),
    CONSTRAINT FK_SesionUsuario_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_SesionUsuario_expira CHECK (expira_en > creada_en)
);
GO
CREATE TABLE dbo.EventoAcceso (
    id_evento_acceso bigint IDENTITY(1,1) NOT NULL,
    id_usuario       int NULL,
    nombre_intentado nvarchar(50) NOT NULL,
    fecha            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    resultado        varchar(30) NOT NULL,
    equipo           nvarchar(150) NULL,
    direccion_ip     varchar(45) NULL,
    CONSTRAINT PK_EventoAcceso PRIMARY KEY (id_evento_acceso),
    CONSTRAINT FK_EventoAcceso_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_EventoAcceso_resultado CHECK (resultado IN ('EXITOSO','INVALIDO','BLOQUEADO','INACTIVO'))
);
GO
CREATE TABLE dbo.RecuperacionAcceso (
    id_recuperacion bigint IDENTITY(1,1) NOT NULL,
    id_usuario      int NOT NULL,
    token_hash      binary(32) NOT NULL,
    creado_en       datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    expira_en       datetime2(3) NOT NULL,
    utilizado_en    datetime2(3) NULL,
    revocado_en     datetime2(3) NULL,
    CONSTRAINT PK_RecuperacionAcceso PRIMARY KEY (id_recuperacion),
    CONSTRAINT UQ_RecuperacionAcceso_token UNIQUE (token_hash),
    CONSTRAINT FK_RecuperacionAcceso_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_RecuperacionAcceso_expira CHECK (expira_en > creado_en)
);
GO
CREATE TABLE dbo.Producto (
    id_producto          int IDENTITY(1,1) NOT NULL,
    codigo_barras        nvarchar(50) NOT NULL,
    nombre               nvarchar(150) NOT NULL,
    precio_unitario      decimal(10,2) NOT NULL,
    id_categoria         int NOT NULL,
    id_proveedor         int NOT NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_Producto PRIMARY KEY (id_producto),
    CONSTRAINT UQ_Producto_codigo_barras UNIQUE (codigo_barras),
    CONSTRAINT FK_Producto_Categoria FOREIGN KEY (id_categoria) REFERENCES dbo.Categoria (id_categoria),
    CONSTRAINT FK_Producto_Proveedor FOREIGN KEY (id_proveedor) REFERENCES dbo.Proveedor (id_proveedor),
    CONSTRAINT CK_Producto_precio CHECK (precio_unitario > 0),
    CONSTRAINT CK_Producto_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.InventarioTienda (
    id_tienda    int NOT NULL,
    id_producto  int NOT NULL,
    existencia   int NOT NULL DEFAULT 0,
    reservado    int NOT NULL DEFAULT 0,
    stock_minimo int NOT NULL DEFAULT 0,
    version_fila rowversion NOT NULL,
    CONSTRAINT PK_InventarioTienda PRIMARY KEY (id_tienda, id_producto),
    CONSTRAINT FK_InventarioTienda_Tienda FOREIGN KEY (id_tienda) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT FK_InventarioTienda_Producto FOREIGN KEY (id_producto) REFERENCES dbo.Producto (id_producto),
    CONSTRAINT CK_InventarioTienda_existencia CHECK (existencia >= 0),
    CONSTRAINT CK_InventarioTienda_reservado CHECK (reservado >= 0 AND reservado <= existencia),
    CONSTRAINT CK_InventarioTienda_minimo CHECK (stock_minimo >= 0)
);
GO
CREATE TABLE dbo.Compra (
    id_compra          int IDENTITY(1,1) NOT NULL,
    id_proveedor       int NOT NULL,
    id_tienda          int NOT NULL,
    numero_factura     nvarchar(50) NOT NULL,
    fecha_factura      date NOT NULL,
    registrado_en      datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    id_usuario         int NOT NULL,
    estado             varchar(20) NOT NULL DEFAULT 'BORRADOR',
    observaciones      nvarchar(500) NULL,
    anulado_en         datetime2(3) NULL,
    anulado_por        int NULL,
    motivo_anulacion   nvarchar(500) NULL,
    CONSTRAINT PK_Compra PRIMARY KEY (id_compra),
    CONSTRAINT UQ_Compra_proveedor_factura UNIQUE (id_proveedor, numero_factura),
    CONSTRAINT FK_Compra_Proveedor FOREIGN KEY (id_proveedor) REFERENCES dbo.Proveedor (id_proveedor),
    CONSTRAINT FK_Compra_Tienda FOREIGN KEY (id_tienda) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT FK_Compra_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT FK_Compra_anulado_por FOREIGN KEY (anulado_por) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_Compra_estado CHECK (estado IN ('BORRADOR','CONFIRMADA','ANULADA')),
    CONSTRAINT CK_Compra_anulacion CHECK (estado <> 'ANULADA' OR (anulado_en IS NOT NULL AND anulado_por IS NOT NULL AND motivo_anulacion IS NOT NULL))
);
GO
CREATE TABLE dbo.DetalleCompra (
    id_detalle_compra int IDENTITY(1,1) NOT NULL,
    id_compra         int NOT NULL,
    id_producto       int NOT NULL,
    cantidad          int NOT NULL,
    costo_unitario    decimal(18,2) NOT NULL,
    subtotal          AS (CONVERT(decimal(18,2), cantidad * costo_unitario)),
    CONSTRAINT PK_DetalleCompra PRIMARY KEY (id_detalle_compra),
    CONSTRAINT FK_DetalleCompra_Compra FOREIGN KEY (id_compra) REFERENCES dbo.Compra (id_compra),
    CONSTRAINT FK_DetalleCompra_Producto FOREIGN KEY (id_producto) REFERENCES dbo.Producto (id_producto),
    CONSTRAINT CK_DetalleCompra_cantidad CHECK (cantidad > 0),
    CONSTRAINT CK_DetalleCompra_costo CHECK (costo_unitario > 0)
);
GO
CREATE TABLE dbo.Venta (
    id_venta         int IDENTITY(1,1) NOT NULL,
    numero           varchar(40) NOT NULL,
    id_tienda        int NOT NULL,
    id_cliente       int NOT NULL,
    rtn_cliente      varchar(30) NULL,
    cai              varchar(37) NULL,
    id_usuario       int NOT NULL,
    fecha            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    estado           varchar(20) NOT NULL DEFAULT 'BORRADOR',
    observaciones    nvarchar(500) NULL,
    anulado_en       datetime2(3) NULL,
    anulado_por      int NULL,
    motivo_anulacion nvarchar(500) NULL,
    CONSTRAINT PK_Venta PRIMARY KEY (id_venta),
    CONSTRAINT UQ_Venta_numero UNIQUE (numero),
    CONSTRAINT CK_Venta_rtn_cliente CHECK (rtn_cliente IS NULL OR (DATALENGTH(rtn_cliente)=14 AND rtn_cliente COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9]%')),
    CONSTRAINT FK_Venta_Tienda FOREIGN KEY (id_tienda) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT FK_Venta_Cliente FOREIGN KEY (id_cliente) REFERENCES dbo.Cliente (id_cliente),
    CONSTRAINT FK_Venta_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT FK_Venta_anulado_por FOREIGN KEY (anulado_por) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_Venta_estado CHECK (estado IN ('BORRADOR','CONFIRMADA','ANULADA')),
    CONSTRAINT CK_Venta_anulacion CHECK (estado <> 'ANULADA' OR (anulado_en IS NOT NULL AND anulado_por IS NOT NULL AND motivo_anulacion IS NOT NULL))
);
GO
CREATE TABLE dbo.DetalleVenta (
    id_detalle_venta int IDENTITY(1,1) NOT NULL,
    id_venta         int NOT NULL,
    id_producto      int NOT NULL,
    cantidad         int NOT NULL,
    precio_unitario  decimal(18,2) NOT NULL,
    nombre_producto  nvarchar(150) NOT NULL,
    tasa_isv         decimal(5,2) NOT NULL CONSTRAINT DF_DetalleVenta_tasa_isv DEFAULT 15,
    subtotal         AS (CONVERT(decimal(18,2), cantidad * precio_unitario)),
    isv              AS (CONVERT(decimal(18,2), ROUND(cantidad * precio_unitario * tasa_isv / 100,2))),
    total_linea      AS (CONVERT(decimal(18,2), cantidad * precio_unitario + ROUND(cantidad * precio_unitario * tasa_isv / 100,2))),
    CONSTRAINT PK_DetalleVenta PRIMARY KEY (id_detalle_venta),
    CONSTRAINT FK_DetalleVenta_Venta FOREIGN KEY (id_venta) REFERENCES dbo.Venta (id_venta),
    CONSTRAINT FK_DetalleVenta_Producto FOREIGN KEY (id_producto) REFERENCES dbo.Producto (id_producto),
    CONSTRAINT CK_DetalleVenta_cantidad CHECK (cantidad > 0),
    CONSTRAINT CK_DetalleVenta_tasa_isv CHECK (tasa_isv IN (0,15,18)),
    CONSTRAINT CK_DetalleVenta_precio CHECK (precio_unitario > 0)
);
GO
CREATE TABLE dbo.PagoVenta (
    id_pago_venta  int IDENTITY(1,1) NOT NULL,
    id_venta       int NOT NULL,
    id_metodo_pago int NOT NULL,
    monto          decimal(18,2) NOT NULL,
    monto_recibido decimal(18,2) NULL,
    referencia     nvarchar(100) NULL,
    fecha          datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_PagoVenta PRIMARY KEY (id_pago_venta),
    CONSTRAINT FK_PagoVenta_Venta FOREIGN KEY (id_venta) REFERENCES dbo.Venta (id_venta),
    CONSTRAINT FK_PagoVenta_MetodoPago FOREIGN KEY (id_metodo_pago) REFERENCES dbo.MetodoPago (id_metodo_pago),
    CONSTRAINT CK_PagoVenta_monto CHECK (monto > 0),
    CONSTRAINT CK_PagoVenta_recibido CHECK (monto_recibido IS NULL OR monto_recibido >= monto)
);
GO
CREATE TABLE dbo.Traspaso (
    id_traspaso       int IDENTITY(1,1) NOT NULL,
    id_tienda_origen  int NOT NULL,
    id_tienda_destino int NOT NULL,
    estado            varchar(20) NOT NULL DEFAULT 'PENDIENTE',
    solicitado_por    int NOT NULL,
    solicitado_en     datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    observaciones     nvarchar(500) NULL,
    version_fila      rowversion NOT NULL,
    CONSTRAINT PK_Traspaso PRIMARY KEY (id_traspaso),
    CONSTRAINT FK_Traspaso_Origen FOREIGN KEY (id_tienda_origen) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT FK_Traspaso_Destino FOREIGN KEY (id_tienda_destino) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT FK_Traspaso_solicitado_por FOREIGN KEY (solicitado_por) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_Traspaso_tiendas CHECK (id_tienda_origen <> id_tienda_destino),
    CONSTRAINT CK_Traspaso_estado CHECK (estado IN ('PENDIENTE','APROBADO','EN_TRANSITO','RECIBIDO','RECHAZADO','CANCELADO'))
);
GO
CREATE TABLE dbo.DetalleTraspaso (
    id_detalle_traspaso int IDENTITY(1,1) NOT NULL,
    id_traspaso         int NOT NULL,
    id_producto         int NOT NULL,
    cantidad            int NOT NULL,
    precio_referencia   decimal(18,2) NOT NULL,
    CONSTRAINT PK_DetalleTraspaso PRIMARY KEY (id_detalle_traspaso),
    CONSTRAINT UQ_DetalleTraspaso_producto UNIQUE (id_traspaso, id_producto),
    CONSTRAINT FK_DetalleTraspaso_Traspaso FOREIGN KEY (id_traspaso) REFERENCES dbo.Traspaso (id_traspaso),
    CONSTRAINT FK_DetalleTraspaso_Producto FOREIGN KEY (id_producto) REFERENCES dbo.Producto (id_producto),
    CONSTRAINT CK_DetalleTraspaso_cantidad CHECK (cantidad > 0),
    CONSTRAINT CK_DetalleTraspaso_precio CHECK (precio_referencia >= 0)
);
GO
CREATE TABLE dbo.EventoTraspaso (
    id_evento_traspaso bigint IDENTITY(1,1) NOT NULL,
    id_traspaso        int NOT NULL,
    estado_anterior    varchar(20) NULL,
    estado_nuevo       varchar(20) NOT NULL,
    id_usuario         int NOT NULL,
    fecha              datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    motivo             nvarchar(500) NULL,
    CONSTRAINT PK_EventoTraspaso PRIMARY KEY (id_evento_traspaso),
    CONSTRAINT FK_EventoTraspaso_Traspaso FOREIGN KEY (id_traspaso) REFERENCES dbo.Traspaso (id_traspaso),
    CONSTRAINT FK_EventoTraspaso_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_EventoTraspaso_anterior CHECK (estado_anterior IS NULL OR estado_anterior IN ('PENDIENTE','APROBADO','EN_TRANSITO','RECIBIDO','RECHAZADO','CANCELADO')),
    CONSTRAINT CK_EventoTraspaso_nuevo CHECK (estado_nuevo IN ('PENDIENTE','APROBADO','EN_TRANSITO','RECIBIDO','RECHAZADO','CANCELADO')),
    CONSTRAINT CK_EventoTraspaso_motivo CHECK (estado_nuevo NOT IN ('RECHAZADO','CANCELADO') OR motivo IS NOT NULL)
);
GO

CREATE TABLE dbo.Apartado (
    id_apartado  int IDENTITY(1,1) NOT NULL,
    id_cliente   int NOT NULL,
    id_tienda    int NOT NULL,
    id_usuario   int NOT NULL,
    fecha        datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    vence_en     datetime2(3) NOT NULL,
    estado       varchar(20) NOT NULL DEFAULT 'ACTIVO',
    id_venta     int NULL,
    version_fila rowversion NOT NULL,
    CONSTRAINT PK_Apartado PRIMARY KEY (id_apartado),
    CONSTRAINT FK_Apartado_Cliente FOREIGN KEY (id_cliente) REFERENCES dbo.Cliente (id_cliente),
    CONSTRAINT FK_Apartado_Tienda FOREIGN KEY (id_tienda) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT FK_Apartado_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT FK_Apartado_Venta FOREIGN KEY (id_venta) REFERENCES dbo.Venta (id_venta),
    CONSTRAINT CK_Apartado_vence CHECK (vence_en > fecha),
    CONSTRAINT CK_Apartado_estado CHECK (estado IN ('ACTIVO','ENTREGADO','CANCELADO','VENCIDO')),
    CONSTRAINT CK_Apartado_entrega CHECK (estado <> 'ENTREGADO' OR id_venta IS NOT NULL)
);
GO
CREATE TABLE dbo.DetalleApartado (
    id_detalle_apartado int IDENTITY(1,1) NOT NULL,
    id_apartado         int NOT NULL,
    id_producto         int NOT NULL,
    cantidad            int NOT NULL,
    precio_unitario     decimal(18,2) NOT NULL,
    tasa_isv            decimal(5,2) NOT NULL CONSTRAINT DF_DetalleApartado_tasa_isv DEFAULT 15,
    subtotal            AS (CONVERT(decimal(18,2), cantidad * precio_unitario)),
    isv                 AS (CONVERT(decimal(18,2), ROUND(cantidad * precio_unitario * tasa_isv / 100,2))),
    total_linea         AS (CONVERT(decimal(18,2), cantidad * precio_unitario + ROUND(cantidad * precio_unitario * tasa_isv / 100,2))),
    CONSTRAINT PK_DetalleApartado PRIMARY KEY (id_detalle_apartado),
    CONSTRAINT FK_DetalleApartado_Apartado FOREIGN KEY (id_apartado) REFERENCES dbo.Apartado (id_apartado),
    CONSTRAINT FK_DetalleApartado_Producto FOREIGN KEY (id_producto) REFERENCES dbo.Producto (id_producto),
    CONSTRAINT CK_DetalleApartado_cantidad CHECK (cantidad > 0),
    CONSTRAINT CK_DetalleApartado_tasa_isv CHECK (tasa_isv IN (0,15,18)),
    CONSTRAINT CK_DetalleApartado_precio CHECK (precio_unitario > 0)
);
GO
CREATE TABLE dbo.AbonoApartado (
    id_abono       int IDENTITY(1,1) NOT NULL,
    id_apartado    int NOT NULL,
    id_metodo_pago int NOT NULL,
    monto          decimal(18,2) NOT NULL,
    fecha          datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    id_usuario     int NOT NULL,
    referencia     nvarchar(100) NULL,
    id_pago_venta  int NULL,
    CONSTRAINT PK_AbonoApartado PRIMARY KEY (id_abono),
    CONSTRAINT FK_AbonoApartado_Apartado FOREIGN KEY (id_apartado) REFERENCES dbo.Apartado (id_apartado),
    CONSTRAINT FK_AbonoApartado_MetodoPago FOREIGN KEY (id_metodo_pago) REFERENCES dbo.MetodoPago (id_metodo_pago),
    CONSTRAINT FK_AbonoApartado_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT FK_AbonoApartado_PagoVenta FOREIGN KEY (id_pago_venta) REFERENCES dbo.PagoVenta (id_pago_venta),
    CONSTRAINT CK_AbonoApartado_monto CHECK (monto > 0)
);
GO
CREATE TABLE dbo.Devolucion (
    id_devolucion    int IDENTITY(1,1) NOT NULL,
    id_venta         int NOT NULL,
    id_tienda        int NOT NULL,
    id_usuario       int NOT NULL,
    fecha            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    motivo           nvarchar(500) NOT NULL,
    estado           varchar(20) NOT NULL DEFAULT 'BORRADOR',
    anulado_en       datetime2(3) NULL,
    anulado_por      int NULL,
    motivo_anulacion nvarchar(500) NULL,
    CONSTRAINT PK_Devolucion PRIMARY KEY (id_devolucion),
    CONSTRAINT FK_Devolucion_Venta FOREIGN KEY (id_venta) REFERENCES dbo.Venta (id_venta),
    CONSTRAINT FK_Devolucion_Tienda FOREIGN KEY (id_tienda) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT FK_Devolucion_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT FK_Devolucion_anulado_por FOREIGN KEY (anulado_por) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_Devolucion_estado CHECK (estado IN ('BORRADOR','CONFIRMADA','ANULADA')),
    CONSTRAINT CK_Devolucion_anulacion CHECK (estado <> 'ANULADA' OR (anulado_en IS NOT NULL AND anulado_por IS NOT NULL AND motivo_anulacion IS NOT NULL))
);
GO
CREATE TABLE dbo.DetalleDevolucion (
    id_detalle_devolucion int IDENTITY(1,1) NOT NULL,
    id_devolucion         int NOT NULL,
    id_detalle_venta      int NOT NULL,
    cantidad              int NOT NULL,
    reintegra_stock       bit NOT NULL,
    isv                   decimal(18,2) NOT NULL CONSTRAINT DF_DetalleDevolucion_isv DEFAULT 0,
    CONSTRAINT PK_DetalleDevolucion PRIMARY KEY (id_detalle_devolucion),
    CONSTRAINT FK_DetalleDevolucion_Devolucion FOREIGN KEY (id_devolucion) REFERENCES dbo.Devolucion (id_devolucion),
    CONSTRAINT FK_DetalleDevolucion_DetalleVenta FOREIGN KEY (id_detalle_venta) REFERENCES dbo.DetalleVenta (id_detalle_venta),
    CONSTRAINT CK_DetalleDevolucion_cantidad CHECK (cantidad > 0),
    CONSTRAINT CK_DetalleDevolucion_isv CHECK (isv>=0)
);
GO
CREATE TABLE dbo.Reembolso (
    id_reembolso   int IDENTITY(1,1) NOT NULL,
    id_pago_venta  int NULL,
    id_abono       int NULL,
    id_devolucion  int NULL,
    monto          decimal(18,2) NOT NULL,
    id_metodo_pago int NOT NULL,
    id_usuario     int NOT NULL,
    fecha          datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    referencia     nvarchar(100) NULL,
    motivo         nvarchar(500) NOT NULL,
    CONSTRAINT PK_Reembolso PRIMARY KEY (id_reembolso),
    CONSTRAINT FK_Reembolso_PagoVenta FOREIGN KEY (id_pago_venta) REFERENCES dbo.PagoVenta (id_pago_venta),
    CONSTRAINT FK_Reembolso_AbonoApartado FOREIGN KEY (id_abono) REFERENCES dbo.AbonoApartado (id_abono),
    CONSTRAINT FK_Reembolso_Devolucion FOREIGN KEY (id_devolucion) REFERENCES dbo.Devolucion (id_devolucion),
    CONSTRAINT FK_Reembolso_MetodoPago FOREIGN KEY (id_metodo_pago) REFERENCES dbo.MetodoPago (id_metodo_pago),
    CONSTRAINT FK_Reembolso_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_Reembolso_monto CHECK (monto > 0),
    CONSTRAINT CK_Reembolso_cobro CHECK ((id_pago_venta IS NOT NULL AND id_abono IS NULL) OR (id_pago_venta IS NULL AND id_abono IS NOT NULL))
);
GO
CREATE TABLE dbo.MovimientoInventario (
    id_movimiento           bigint IDENTITY(1,1) NOT NULL,
    id_tienda               int NOT NULL,
    id_producto             int NOT NULL,
    fecha                   datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    tipo                    varchar(30) NOT NULL,
    cantidad_delta          int NOT NULL,
    id_usuario              int NOT NULL,
    id_detalle_compra       int NULL,
    id_detalle_venta        int NULL,
    id_detalle_traspaso     int NULL,
    id_detalle_devolucion   int NULL,
    id_movimiento_revertido bigint NULL,
    motivo                  nvarchar(500) NULL,
    CONSTRAINT PK_MovimientoInventario PRIMARY KEY (id_movimiento),
    CONSTRAINT FK_Movimiento_Tienda FOREIGN KEY (id_tienda) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT FK_Movimiento_Producto FOREIGN KEY (id_producto) REFERENCES dbo.Producto (id_producto),
    CONSTRAINT FK_Movimiento_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT FK_Movimiento_DetalleCompra FOREIGN KEY (id_detalle_compra) REFERENCES dbo.DetalleCompra (id_detalle_compra),
    CONSTRAINT FK_Movimiento_DetalleVenta FOREIGN KEY (id_detalle_venta) REFERENCES dbo.DetalleVenta (id_detalle_venta),
    CONSTRAINT FK_Movimiento_DetalleTraspaso FOREIGN KEY (id_detalle_traspaso) REFERENCES dbo.DetalleTraspaso (id_detalle_traspaso),
    CONSTRAINT FK_Movimiento_DetalleDevolucion FOREIGN KEY (id_detalle_devolucion) REFERENCES dbo.DetalleDevolucion (id_detalle_devolucion),
    CONSTRAINT FK_Movimiento_Revertido FOREIGN KEY (id_movimiento_revertido) REFERENCES dbo.MovimientoInventario (id_movimiento),
    CONSTRAINT CK_Movimiento_tipo CHECK (tipo IN ('COMPRA','VENTA','TRASPASO_SALIDA','TRASPASO_ENTRADA','DEVOLUCION','AJUSTE','REVERSO')),
    CONSTRAINT CK_Movimiento_delta CHECK (cantidad_delta <> 0),
    CONSTRAINT CK_Movimiento_origen CHECK (
           (tipo = 'COMPRA' AND id_detalle_compra IS NOT NULL AND id_detalle_venta IS NULL AND id_detalle_traspaso IS NULL AND id_detalle_devolucion IS NULL AND id_movimiento_revertido IS NULL)
        OR (tipo = 'VENTA' AND id_detalle_venta IS NOT NULL AND id_detalle_compra IS NULL AND id_detalle_traspaso IS NULL AND id_detalle_devolucion IS NULL AND id_movimiento_revertido IS NULL)
        OR (tipo IN ('TRASPASO_SALIDA','TRASPASO_ENTRADA') AND id_detalle_traspaso IS NOT NULL AND id_detalle_compra IS NULL AND id_detalle_venta IS NULL AND id_detalle_devolucion IS NULL AND id_movimiento_revertido IS NULL)
        OR (tipo = 'DEVOLUCION' AND id_detalle_devolucion IS NOT NULL AND id_detalle_compra IS NULL AND id_detalle_venta IS NULL AND id_detalle_traspaso IS NULL AND id_movimiento_revertido IS NULL)
        OR (tipo = 'AJUSTE' AND id_detalle_compra IS NULL AND id_detalle_venta IS NULL AND id_detalle_traspaso IS NULL AND id_detalle_devolucion IS NULL AND id_movimiento_revertido IS NULL AND motivo IS NOT NULL)
        OR (tipo = 'REVERSO' AND id_movimiento_revertido IS NOT NULL AND id_detalle_compra IS NULL AND id_detalle_venta IS NULL AND id_detalle_traspaso IS NULL AND id_detalle_devolucion IS NULL AND motivo IS NOT NULL)
    )
);
GO

CREATE TABLE dbo.Inversion (
    id_inversion         int IDENTITY(1,1) NOT NULL,
    institucion          nvarchar(150) NOT NULL,
    tipo                 varchar(20) NOT NULL,
    monto                decimal(18,2) NOT NULL,
    tasa_interes         decimal(5,2) NOT NULL,
    fecha_inicio         date NOT NULL,
    fecha_vencimiento    date NOT NULL,
    estado               varchar(20) NOT NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_Inversion PRIMARY KEY (id_inversion),
    CONSTRAINT CK_Inversion_tipo CHECK (tipo IN ('PLAZO_FIJO','AHORRO','OTRO')),
    CONSTRAINT CK_Inversion_monto CHECK (monto > 0),
    CONSTRAINT CK_Inversion_tasa CHECK (tasa_interes BETWEEN 0 AND 100),
    CONSTRAINT CK_Inversion_fechas CHECK (fecha_vencimiento >= fecha_inicio),
    CONSTRAINT CK_Inversion_estado CHECK (estado IN ('VIGENTE','VENCIDA','LIQUIDADA','CANCELADA')),
    CONSTRAINT CK_Inversion_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.Importacion (
    id_importacion int IDENTITY(1,1) NOT NULL,
    nombre_archivo nvarchar(260) NOT NULL,
    archivo_hash   binary(32) NOT NULL,
    id_usuario     int NOT NULL,
    fecha          datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    estado         varchar(20) NOT NULL,
    CONSTRAINT PK_Importacion PRIMARY KEY (id_importacion),
    CONSTRAINT FK_Importacion_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_Importacion_estado CHECK (estado IN ('VALIDANDO','CON_ERRORES','LISTA','COMPLETADA','CANCELADA'))
);
GO
CREATE TABLE dbo.DetalleImportacion (
    id_detalle_importacion bigint IDENTITY(1,1) NOT NULL,
    id_importacion         int NOT NULL,
    numero_fila            int NOT NULL,
    datos_originales_json  nvarchar(max) NOT NULL,
    estado                 varchar(20) NOT NULL,
    error                  nvarchar(1000) NULL,
    id_producto            int NULL,
    CONSTRAINT PK_DetalleImportacion PRIMARY KEY (id_detalle_importacion),
    CONSTRAINT UQ_DetalleImportacion_fila UNIQUE (id_importacion, numero_fila),
    CONSTRAINT FK_DetalleImportacion_Importacion FOREIGN KEY (id_importacion) REFERENCES dbo.Importacion (id_importacion),
    CONSTRAINT FK_DetalleImportacion_Producto FOREIGN KEY (id_producto) REFERENCES dbo.Producto (id_producto),
    CONSTRAINT CK_DetalleImportacion_fila CHECK (numero_fila > 0),
    CONSTRAINT CK_DetalleImportacion_json CHECK (ISJSON(datos_originales_json) = 1),
    CONSTRAINT CK_DetalleImportacion_estado CHECK (estado IN ('VALIDA','ERROR','DUPLICADA','IMPORTADA','OMITIDA'))
);
GO
CREATE TABLE dbo.ConfiguracionFacturacion (
    id_tienda int NOT NULL CONSTRAINT PK_ConfiguracionFacturacion PRIMARY KEY,
    cai varchar(37) NULL,
    actualizado_en datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    actualizado_por int NOT NULL,
    version_fila rowversion NOT NULL,
    CONSTRAINT FK_ConfiguracionFacturacion_Tienda FOREIGN KEY(id_tienda) REFERENCES dbo.Tienda(id_tienda),
    CONSTRAINT FK_ConfiguracionFacturacion_Usuario FOREIGN KEY(actualizado_por) REFERENCES dbo.Usuario(id_usuario),
    CONSTRAINT CK_ConfiguracionFacturacion_cai CHECK(cai IS NULL OR DATALENGTH(cai)=37)
);
GO
CREATE TABLE dbo.Auditoria (
    id_auditoria          bigint IDENTITY(1,1) NOT NULL,
    fecha                 datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    id_usuario            int NULL,
    modulo                varchar(50) NOT NULL,
    accion                varchar(50) NOT NULL,
    entidad               varchar(80) NOT NULL,
    id_registro           nvarchar(100) NOT NULL,
    descripcion           nvarchar(1000) NOT NULL,
    valores_antes_json    nvarchar(max) NULL,
    valores_despues_json  nvarchar(max) NULL,
    equipo                nvarchar(150) NULL,
    CONSTRAINT PK_Auditoria PRIMARY KEY (id_auditoria),
    CONSTRAINT FK_Auditoria_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_Auditoria_antes_json CHECK (valores_antes_json IS NULL OR ISJSON(valores_antes_json) = 1),
    CONSTRAINT CK_Auditoria_despues_json CHECK (valores_despues_json IS NULL OR ISJSON(valores_despues_json) = 1)
);
GO
CREATE TABLE dbo.EventoNotificacion (
    id_evento_notificacion bigint IDENTITY(1,1) NOT NULL,
    clave_evento           varchar(150) NOT NULL,
    tipo                   varchar(40) NOT NULL,
    titulo                 nvarchar(150) NOT NULL,
    mensaje                nvarchar(500) NOT NULL,
    creada_en              datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_EventoNotificacion PRIMARY KEY (id_evento_notificacion),
    CONSTRAINT UQ_EventoNotificacion_clave UNIQUE (clave_evento),
    CONSTRAINT CK_EventoNotificacion_tipo CHECK (tipo IN ('STOCK_BAJO','TRASPASO_PENDIENTE','INVERSION_VENCIMIENTO'))
);
GO
CREATE TABLE dbo.Notificacion (
    id_notificacion        bigint IDENTITY(1,1) NOT NULL,
    id_evento_notificacion bigint NOT NULL,
    id_usuario             int NOT NULL,
    leida_en               datetime2(3) NULL,
    CONSTRAINT PK_Notificacion PRIMARY KEY (id_notificacion),
    CONSTRAINT UQ_Notificacion_evento_usuario UNIQUE (id_evento_notificacion, id_usuario),
    CONSTRAINT FK_Notificacion_Evento FOREIGN KEY (id_evento_notificacion) REFERENCES dbo.EventoNotificacion (id_evento_notificacion),
    CONSTRAINT FK_Notificacion_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario)
);
GO

ALTER TABLE dbo.Categoria  ADD CONSTRAINT FK_Categoria_creado_por       FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Categoria_actualizado_por  FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Categoria_desactivado_por  FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.Proveedor  ADD CONSTRAINT FK_Proveedor_creado_por       FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Proveedor_actualizado_por  FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Proveedor_desactivado_por  FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.Cliente    ADD CONSTRAINT FK_Cliente_creado_por         FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Cliente_actualizado_por    FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Cliente_desactivado_por    FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.Tienda     ADD CONSTRAINT FK_Tienda_creado_por          FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Tienda_actualizado_por     FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Tienda_desactivado_por     FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.MetodoPago ADD CONSTRAINT FK_MetodoPago_creado_por      FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_MetodoPago_actualizado_por FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_MetodoPago_desactivado_por FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.Empleado   ADD CONSTRAINT FK_Empleado_creado_por        FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Empleado_actualizado_por   FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Empleado_desactivado_por   FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.Usuario    ADD CONSTRAINT FK_Usuario_creado_por         FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Usuario_actualizado_por    FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Usuario_desactivado_por    FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.Rol        ADD CONSTRAINT FK_Rol_creado_por             FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Rol_actualizado_por        FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Rol_desactivado_por        FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.Permiso    ADD CONSTRAINT FK_Permiso_creado_por         FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Permiso_actualizado_por    FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Permiso_desactivado_por    FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.Producto   ADD CONSTRAINT FK_Producto_creado_por        FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Producto_actualizado_por   FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Producto_desactivado_por   FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.Inversion  ADD CONSTRAINT FK_Inversion_creado_por       FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Inversion_actualizado_por  FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Inversion_desactivado_por  FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
GO
CREATE UNIQUE INDEX UX_Proveedor_rtn                    ON dbo.Proveedor (rtn)                          WHERE rtn IS NOT NULL;
CREATE UNIQUE INDEX UX_Cliente_documento_fiscal         ON dbo.Cliente (documento_fiscal)               WHERE documento_fiscal IS NOT NULL;
CREATE UNIQUE INDEX UX_Apartado_venta                   ON dbo.Apartado (id_venta)                      WHERE id_venta IS NOT NULL;
CREATE UNIQUE INDEX UX_AbonoApartado_pago_venta         ON dbo.AbonoApartado (id_pago_venta)            WHERE id_pago_venta IS NOT NULL;
CREATE UNIQUE INDEX UX_EventoTraspaso_estado            ON dbo.EventoTraspaso (id_traspaso, estado_nuevo);
CREATE UNIQUE INDEX UX_Movimiento_detalle_compra        ON dbo.MovimientoInventario (id_detalle_compra)                 WHERE id_detalle_compra IS NOT NULL;
CREATE UNIQUE INDEX UX_Movimiento_detalle_venta         ON dbo.MovimientoInventario (id_detalle_venta)                  WHERE id_detalle_venta IS NOT NULL;
CREATE UNIQUE INDEX UX_Movimiento_detalle_traspaso_tipo ON dbo.MovimientoInventario (id_detalle_traspaso, tipo)         WHERE id_detalle_traspaso IS NOT NULL;
CREATE UNIQUE INDEX UX_Movimiento_detalle_devolucion    ON dbo.MovimientoInventario (id_detalle_devolucion)             WHERE id_detalle_devolucion IS NOT NULL;
CREATE UNIQUE INDEX UX_Movimiento_revertido             ON dbo.MovimientoInventario (id_movimiento_revertido)           WHERE id_movimiento_revertido IS NOT NULL;
GO
CREATE INDEX IX_Empleado_tienda               ON dbo.Empleado (id_tienda);
CREATE INDEX IX_Producto_categoria            ON dbo.Producto (id_categoria);
CREATE INDEX IX_Producto_proveedor            ON dbo.Producto (id_proveedor);
CREATE INDEX IX_InventarioTienda_producto     ON dbo.InventarioTienda (id_producto);
CREATE INDEX IX_UsuarioRol_rol                ON dbo.UsuarioRol (id_rol);
CREATE INDEX IX_RolPermiso_permiso            ON dbo.RolPermiso (id_permiso);
CREATE INDEX IX_UsuarioTienda_tienda          ON dbo.UsuarioTienda (id_tienda);
CREATE INDEX IX_SesionUsuario_usuario         ON dbo.SesionUsuario (id_usuario);
CREATE INDEX IX_EventoAcceso_usuario          ON dbo.EventoAcceso (id_usuario);
CREATE INDEX IX_EventoAcceso_nombre_fecha     ON dbo.EventoAcceso (nombre_intentado, fecha);
CREATE INDEX IX_RecuperacionAcceso_usuario    ON dbo.RecuperacionAcceso (id_usuario);
CREATE INDEX IX_Compra_tienda                 ON dbo.Compra (id_tienda);
CREATE INDEX IX_Compra_usuario                ON dbo.Compra (id_usuario);
CREATE INDEX IX_DetalleCompra_compra          ON dbo.DetalleCompra (id_compra);
CREATE INDEX IX_DetalleCompra_producto        ON dbo.DetalleCompra (id_producto);
CREATE INDEX IX_Venta_tienda_fecha            ON dbo.Venta (id_tienda, fecha);
CREATE INDEX IX_Venta_cliente                 ON dbo.Venta (id_cliente);
CREATE INDEX IX_Venta_usuario                 ON dbo.Venta (id_usuario);
CREATE INDEX IX_DetalleVenta_venta            ON dbo.DetalleVenta (id_venta);
CREATE INDEX IX_DetalleVenta_producto         ON dbo.DetalleVenta (id_producto);
CREATE INDEX IX_PagoVenta_venta               ON dbo.PagoVenta (id_venta);
CREATE INDEX IX_PagoVenta_metodo_pago         ON dbo.PagoVenta (id_metodo_pago);
CREATE INDEX IX_Traspaso_estado_destino       ON dbo.Traspaso (estado, id_tienda_destino);
CREATE INDEX IX_Traspaso_origen               ON dbo.Traspaso (id_tienda_origen);
CREATE INDEX IX_Traspaso_solicitado_por       ON dbo.Traspaso (solicitado_por);
CREATE INDEX IX_DetalleTraspaso_producto      ON dbo.DetalleTraspaso (id_producto);
CREATE INDEX IX_EventoTraspaso_usuario        ON dbo.EventoTraspaso (id_usuario);
CREATE INDEX IX_Apartado_cliente              ON dbo.Apartado (id_cliente);
CREATE INDEX IX_Apartado_tienda               ON dbo.Apartado (id_tienda);
CREATE INDEX IX_Apartado_usuario              ON dbo.Apartado (id_usuario);
CREATE INDEX IX_DetalleApartado_apartado      ON dbo.DetalleApartado (id_apartado);
CREATE INDEX IX_DetalleApartado_producto      ON dbo.DetalleApartado (id_producto);
CREATE INDEX IX_AbonoApartado_apartado        ON dbo.AbonoApartado (id_apartado);
CREATE INDEX IX_AbonoApartado_metodo_pago     ON dbo.AbonoApartado (id_metodo_pago);
CREATE INDEX IX_AbonoApartado_usuario         ON dbo.AbonoApartado (id_usuario);
CREATE INDEX IX_Devolucion_venta              ON dbo.Devolucion (id_venta);
CREATE INDEX IX_Devolucion_tienda             ON dbo.Devolucion (id_tienda);
CREATE INDEX IX_Devolucion_usuario            ON dbo.Devolucion (id_usuario);
CREATE INDEX IX_DetalleDevolucion_devolucion  ON dbo.DetalleDevolucion (id_devolucion);
CREATE INDEX IX_DetalleDevolucion_detalle_venta ON dbo.DetalleDevolucion (id_detalle_venta);
CREATE INDEX IX_Reembolso_pago_venta          ON dbo.Reembolso (id_pago_venta);
CREATE INDEX IX_Reembolso_abono               ON dbo.Reembolso (id_abono);
CREATE INDEX IX_Reembolso_devolucion          ON dbo.Reembolso (id_devolucion);
CREATE INDEX IX_Reembolso_metodo_pago         ON dbo.Reembolso (id_metodo_pago);
CREATE INDEX IX_Reembolso_usuario             ON dbo.Reembolso (id_usuario);
CREATE INDEX IX_Movimiento_tienda_producto_fecha ON dbo.MovimientoInventario (id_tienda, id_producto, fecha);
CREATE INDEX IX_Movimiento_producto           ON dbo.MovimientoInventario (id_producto);
CREATE INDEX IX_Movimiento_usuario            ON dbo.MovimientoInventario (id_usuario);
CREATE INDEX IX_Importacion_usuario           ON dbo.Importacion (id_usuario);
CREATE INDEX IX_DetalleImportacion_producto   ON dbo.DetalleImportacion (id_producto);
CREATE INDEX IX_Auditoria_fecha_usuario       ON dbo.Auditoria (fecha, id_usuario);
CREATE INDEX IX_Auditoria_usuario             ON dbo.Auditoria (id_usuario);
CREATE INDEX IX_Notificacion_usuario          ON dbo.Notificacion (id_usuario);
GO

CREATE VIEW dbo.vw_ProductoStock AS
SELECT p.id_producto, p.codigo_barras, p.nombre, p.precio_unitario,
       p.id_categoria, p.id_proveedor,
       ISNULL(s.stock_actual, 0) AS stock_actual,
       p.creado_en, p.creado_por, p.actualizado_en, p.actualizado_por,
       p.desactivado_en, p.desactivado_por, p.motivo_desactivacion,
       p.activo, p.version_fila
FROM dbo.Producto p
OUTER APPLY (SELECT SUM(i.existencia) AS stock_actual
             FROM dbo.InventarioTienda i
             WHERE i.id_producto = p.id_producto) s;
GO
CREATE VIEW dbo.vw_CompraTotal AS
SELECT c.id_compra,
       CONVERT(decimal(18,2), ISNULL(SUM(d.cantidad * d.costo_unitario), 0)) AS total
FROM dbo.Compra c
LEFT JOIN dbo.DetalleCompra d ON d.id_compra = c.id_compra
GROUP BY c.id_compra;
GO
CREATE VIEW dbo.vw_VentaTotal AS
SELECT v.id_venta,
       CONVERT(decimal(18,2), ISNULL(SUM(d.subtotal),0)) AS subtotal,
       CONVERT(decimal(18,2), ISNULL(SUM(d.isv),0)) AS isv,
       CONVERT(decimal(18,2), ISNULL(SUM(d.total_linea),0)) AS total
FROM dbo.Venta v
LEFT JOIN dbo.DetalleVenta d ON d.id_venta = v.id_venta
GROUP BY v.id_venta;
GO
CREATE VIEW dbo.vw_ApartadoTotal AS
SELECT a.id_apartado,
       CONVERT(decimal(18,2), ISNULL(SUM(d.subtotal),0)) AS subtotal,
       CONVERT(decimal(18,2), ISNULL(SUM(d.isv),0)) AS isv,
       CONVERT(decimal(18,2), ISNULL(SUM(d.total_linea),0)) AS total
FROM dbo.Apartado a
LEFT JOIN dbo.DetalleApartado d ON d.id_apartado = a.id_apartado
GROUP BY a.id_apartado;
GO
CREATE VIEW dbo.vw_DevolucionDetalle AS
SELECT dd.id_detalle_devolucion, dd.id_devolucion, dd.id_detalle_venta,
       dd.cantidad, dv.precio_unitario,
       CONVERT(decimal(18,2), dd.cantidad * dv.precio_unitario) AS subtotal,
       dd.isv,CONVERT(decimal(18,2),dd.cantidad*dv.precio_unitario+dd.isv) AS total_linea,
       dd.reintegra_stock
FROM dbo.DetalleDevolucion dd
JOIN dbo.DetalleVenta dv ON dv.id_detalle_venta = dd.id_detalle_venta;
GO
CREATE VIEW dbo.vw_DevolucionTotal AS
SELECT d.id_devolucion,
       CONVERT(decimal(18,2), ISNULL(SUM(x.subtotal),0)) AS subtotal,
       CONVERT(decimal(18,2), ISNULL(SUM(x.isv),0)) AS isv,
       CONVERT(decimal(18,2), ISNULL(SUM(x.total_linea),0)) AS total
FROM dbo.Devolucion d
LEFT JOIN dbo.vw_DevolucionDetalle x ON x.id_devolucion = d.id_devolucion
GROUP BY d.id_devolucion;
GO
CREATE VIEW dbo.vw_TraspasoPasos AS
SELECT t.id_traspaso,
       MAX(CASE WHEN e.estado_nuevo = 'APROBADO'    THEN e.id_usuario END) AS aprobado_por,
       MAX(CASE WHEN e.estado_nuevo = 'APROBADO'    THEN e.fecha END)      AS aprobado_en,
       MAX(CASE WHEN e.estado_nuevo = 'EN_TRANSITO' THEN e.id_usuario END) AS enviado_por,
       MAX(CASE WHEN e.estado_nuevo = 'EN_TRANSITO' THEN e.fecha END)      AS enviado_en,
       MAX(CASE WHEN e.estado_nuevo = 'RECIBIDO'    THEN e.id_usuario END) AS recibido_por,
       MAX(CASE WHEN e.estado_nuevo = 'RECIBIDO'    THEN e.fecha END)      AS recibido_en
FROM dbo.Traspaso t
LEFT JOIN dbo.EventoTraspaso e ON e.id_traspaso = t.id_traspaso
GROUP BY t.id_traspaso;
GO




INSERT INTO dbo.Rol (codigo, nombre, descripcion)
SELECT v.codigo, v.nombre, v.descripcion
FROM (VALUES
    ('ADMINISTRADOR', N'Administrador', N'Acceso total al sistema'),
    ('GERENTE',       N'Gerente',       N'Gestion de catalogo, compras, ventas y traspasos de sus tiendas'),
    ('CAJERO',        N'Cajero',        N'Ventas, apartados y consulta de disponibilidad en sus tiendas')
) AS v (codigo, nombre, descripcion)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Rol r WHERE r.codigo = v.codigo);
INSERT INTO dbo.MetodoPago (codigo, nombre)
SELECT v.codigo, v.nombre
FROM (VALUES
    ('EFECTIVO',      N'Efectivo'),
    ('TARJETA',       N'Tarjeta'),
    ('TRANSFERENCIA', N'Transferencia')
) AS v (codigo, nombre)
WHERE NOT EXISTS (SELECT 1 FROM dbo.MetodoPago m WHERE m.codigo = v.codigo);
IF NOT EXISTS (SELECT 1 FROM dbo.Cliente WHERE nombre = N'Cliente general')
    INSERT INTO dbo.Cliente (nombre) VALUES (N'Cliente general');
GO

SELECT (SELECT COUNT(*) FROM sys.tables  WHERE schema_id = SCHEMA_ID('dbo')) AS tablas,
       (SELECT COUNT(*) FROM sys.views   WHERE schema_id = SCHEMA_ID('dbo')) AS vistas,
       (SELECT COUNT(*) FROM sys.foreign_keys WHERE schema_id = SCHEMA_ID('dbo')) AS llaves_foraneas;
GO
```

## A.3 Base de Datos/Triggers2.sql

Instala reglas de estados, inventario, reservas, seguridad, auditoría y avisos. Contiene objetos generados dinámicamente y redefiniciones: la última definición ejecutada de un objeto es la vigente.

Extensión: 1280 líneas. SHA-256 del texto normalizado a LF: 9ff4b044d94e7b50fa9e365f7e920b6855af8e9ef06a265f40595b0c051159ca.

```sql












GO

IF OBJECT_ID(N'dbo.InventarioTienda', N'U') IS NULL
    THROW 50991, 'Ejecute primero el esquema 01_esquema_erp_3fn.sql.', 1;
IF (SELECT value_in_use FROM sys.configurations WHERE name = 'nested triggers') <> 1
    THROW 50992, 'Esta version requiere nested triggers habilitado.', 1;
IF DATABASEPROPERTYEX(DB_NAME(), 'IsRecursiveTriggersEnabled') = 1
    THROW 50993, 'Esta version requiere RECURSIVE_TRIGGERS OFF.', 1;
GO




























SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO









DECLARE @maestras TABLE (tabla sysname, pk sysname);
INSERT INTO @maestras (tabla, pk) VALUES
    ('Categoria',  'id_categoria'),  ('Proveedor', 'id_proveedor'), ('Cliente', 'id_cliente'),
    ('Tienda',     'id_tienda'),     ('MetodoPago','id_metodo_pago'), ('Empleado', 'id_empleado'),
    ('Usuario',    'id_usuario'),    ('Rol',       'id_rol'),       ('Permiso', 'id_permiso'),
    ('Producto',   'id_producto'),   ('Inversion', 'id_inversion');

DECLARE @tabla sysname, @pk sysname, @sql nvarchar(max);
DECLARE c CURSOR LOCAL FAST_FORWARD FOR SELECT tabla, pk FROM @maestras;
OPEN c;
FETCH NEXT FROM c INTO @tabla, @pk;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql = N'
CREATE OR ALTER TRIGGER dbo.trg_' + @tabla + N'_actualizado
ON dbo.' + QUOTENAME(@tabla) + N'
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF TRIGGER_NESTLEVEL(@@PROCID) > 1 OR UPDATE(actualizado_en) RETURN;

    UPDATE t
       SET actualizado_en = SYSUTCDATETIME()
      FROM dbo.' + QUOTENAME(@tabla) + N' AS t
      JOIN inserted AS i ON i.' + QUOTENAME(@pk) + N' = t.' + QUOTENAME(@pk) + N';
END;';
    EXEC sys.sp_executesql @sql;
    FETCH NEXT FROM c INTO @tabla, @pk;
END
CLOSE c;
DEALLOCATE c;
GO









IF TYPE_ID('dbo.tt_IdUsuario') IS NULL
    CREATE TYPE dbo.tt_IdUsuario AS TABLE (id_usuario int NOT NULL PRIMARY KEY);
GO

CREATE OR ALTER PROCEDURE dbo.usp_InvalidarSesiones
    @ids dbo.tt_IdUsuario READONLY
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE u
       SET version_seguridad = NEWID()
      FROM dbo.Usuario AS u
      JOIN @ids AS x ON x.id_usuario = u.id_usuario;

    UPDATE s
       SET revocada_en = SYSUTCDATETIME()
      FROM dbo.SesionUsuario AS s
      JOIN @ids AS x ON x.id_usuario = s.id_usuario
     WHERE s.revocada_en IS NULL;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Usuario_seguridad
ON dbo.Usuario
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF TRIGGER_NESTLEVEL(@@PROCID) > 1 RETURN;
    IF NOT (UPDATE(contrasena_hash) OR UPDATE(desactivado_en)) RETURN;


    UPDATE u
       SET contrasena_actualizada_en = SYSUTCDATETIME()
      FROM dbo.Usuario AS u
      JOIN inserted AS i ON i.id_usuario = u.id_usuario
      JOIN deleted  AS d ON d.id_usuario = u.id_usuario
     WHERE i.contrasena_hash <> d.contrasena_hash;


    DECLARE @ids dbo.tt_IdUsuario;
    INSERT INTO @ids (id_usuario)
    SELECT DISTINCT i.id_usuario
      FROM inserted AS i
      JOIN deleted  AS d ON d.id_usuario = i.id_usuario
     WHERE i.contrasena_hash <> d.contrasena_hash
        OR (d.desactivado_en IS NULL AND i.desactivado_en IS NOT NULL);

    IF EXISTS (SELECT 1 FROM @ids) EXEC dbo.usp_InvalidarSesiones @ids;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_UsuarioRol_seguridad
ON dbo.UsuarioRol
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ids dbo.tt_IdUsuario;
    INSERT INTO @ids (id_usuario)
    SELECT id_usuario FROM inserted UNION SELECT id_usuario FROM deleted;
    IF EXISTS (SELECT 1 FROM @ids) EXEC dbo.usp_InvalidarSesiones @ids;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_UsuarioTienda_seguridad
ON dbo.UsuarioTienda
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ids dbo.tt_IdUsuario;
    INSERT INTO @ids (id_usuario)
    SELECT id_usuario FROM inserted UNION SELECT id_usuario FROM deleted;
    IF EXISTS (SELECT 1 FROM @ids) EXEC dbo.usp_InvalidarSesiones @ids;
END;
GO


CREATE OR ALTER TRIGGER dbo.trg_RolPermiso_seguridad
ON dbo.RolPermiso
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ids dbo.tt_IdUsuario;
    INSERT INTO @ids (id_usuario)
    SELECT DISTINCT ur.id_usuario
      FROM dbo.UsuarioRol AS ur
     WHERE ur.id_rol IN (SELECT id_rol FROM inserted UNION SELECT id_rol FROM deleted);
    IF EXISTS (SELECT 1 FROM @ids) EXEC dbo.usp_InvalidarSesiones @ids;
END;
GO






CREATE OR ALTER TRIGGER dbo.trg_MovimientoInventario_inmutable
ON dbo.MovimientoInventario
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM deleted) RETURN;
    ROLLBACK TRANSACTION;
    THROW 50101, 'MovimientoInventario es inmutable: para corregir un asiento registra un movimiento REVERSO.', 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Auditoria_inmutable
ON dbo.Auditoria
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM deleted) RETURN;
    ROLLBACK TRANSACTION;
    THROW 50102, 'Auditoria es inmutable: no se puede modificar ni eliminar.', 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_EventoTraspaso_inmutable
ON dbo.EventoTraspaso
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM deleted) RETURN;
    ROLLBACK TRANSACTION;
    THROW 50103, 'EventoTraspaso es un historial: no se puede modificar ni eliminar.', 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_EventoAcceso_inmutable
ON dbo.EventoAcceso
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM deleted) RETURN;
    ROLLBACK TRANSACTION;
    THROW 50104, 'EventoAcceso es un registro de accesos: no se puede modificar ni eliminar.', 1;
END;
GO










CREATE OR ALTER TRIGGER dbo.trg_Traspaso_estado
ON dbo.Traspaso
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(estado) RETURN;

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_traspaso = i.id_traspaso
         WHERE i.estado <> d.estado
           AND NOT (   (d.estado = 'PENDIENTE'   AND i.estado IN ('APROBADO','RECHAZADO','CANCELADO'))
                    OR (d.estado = 'APROBADO'    AND i.estado IN ('EN_TRANSITO','CANCELADO'))
                    OR (d.estado = 'EN_TRANSITO' AND i.estado = 'RECIBIDO'))
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50201, 'Traspaso: transicion de estado no permitida.', 1;
    END
END;
GO


CREATE OR ALTER TRIGGER dbo.trg_Apartado_estado
ON dbo.Apartado
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(estado) RETURN;

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_apartado = i.id_apartado
         WHERE i.estado <> d.estado
           AND NOT (d.estado = 'ACTIVO' AND i.estado IN ('ENTREGADO','CANCELADO','VENCIDO'))
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50202, 'Apartado: transicion de estado no permitida.', 1;
    END
END;
GO



CREATE OR ALTER TRIGGER dbo.trg_Compra_estado
ON dbo.Compra
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(estado) RETURN;

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_compra = i.id_compra
         WHERE i.estado <> d.estado
           AND NOT (   (d.estado = 'BORRADOR'   AND i.estado IN ('CONFIRMADA','ANULADA'))
                    OR (d.estado = 'CONFIRMADA' AND i.estado = 'ANULADA'))
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50203, 'Compra: transicion de estado no permitida.', 1;
    END

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_compra = i.id_compra
         WHERE i.estado = 'CONFIRMADA' AND d.estado <> 'CONFIRMADA'
           AND NOT EXISTS (SELECT 1 FROM dbo.DetalleCompra AS x WHERE x.id_compra = i.id_compra)
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50204, 'Compra: no se puede confirmar una compra sin detalles.', 1;
    END
END;
GO



CREATE OR ALTER TRIGGER dbo.trg_Venta_estado
ON dbo.Venta
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(estado) RETURN;

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_venta = i.id_venta
         WHERE i.estado <> d.estado
           AND NOT (   (d.estado = 'BORRADOR'   AND i.estado IN ('CONFIRMADA','ANULADA'))
                    OR (d.estado = 'CONFIRMADA' AND i.estado = 'ANULADA'))
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50205, 'Venta: transicion de estado no permitida.', 1;
    END

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_venta = i.id_venta
         WHERE i.estado = 'CONFIRMADA' AND d.estado <> 'CONFIRMADA'
           AND NOT EXISTS (SELECT 1 FROM dbo.DetalleVenta AS x WHERE x.id_venta = i.id_venta)
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50206, 'Venta: no se puede confirmar una venta sin detalles.', 1;
    END

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_venta = i.id_venta
          JOIN dbo.vw_VentaTotal AS t ON t.id_venta = i.id_venta
         WHERE i.estado = 'CONFIRMADA' AND d.estado <> 'CONFIRMADA'
           AND (SELECT ISNULL(SUM(p.monto), 0) FROM dbo.PagoVenta AS p WHERE p.id_venta = i.id_venta) <> t.total
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50207, 'Venta: la suma de pagos debe coincidir con el total antes de confirmar.', 1;
    END
END;
GO



CREATE OR ALTER TRIGGER dbo.trg_Devolucion_estado
ON dbo.Devolucion
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(estado) RETURN;

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_devolucion = i.id_devolucion
         WHERE i.estado <> d.estado
           AND NOT (   (d.estado = 'BORRADOR'   AND i.estado IN ('CONFIRMADA','ANULADA'))
                    OR (d.estado = 'CONFIRMADA' AND i.estado = 'ANULADA'))
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50208, 'Devolucion: transicion de estado no permitida.', 1;
    END

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_devolucion = i.id_devolucion
         WHERE i.estado = 'CONFIRMADA' AND d.estado <> 'CONFIRMADA'
           AND NOT EXISTS (SELECT 1 FROM dbo.DetalleDevolucion AS x WHERE x.id_devolucion = i.id_devolucion)
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50209, 'Devolucion: no se puede confirmar una devolucion sin detalles.', 1;
    END

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_devolucion = i.id_devolucion
          JOIN dbo.Venta AS v ON v.id_venta = i.id_venta
         WHERE i.estado = 'CONFIRMADA' AND d.estado <> 'CONFIRMADA'
           AND v.estado <> 'CONFIRMADA'
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50210, 'Devolucion: la venta original debe estar CONFIRMADA.', 1;
    END
END;
GO










CREATE OR ALTER TRIGGER dbo.trg_DetalleCompra_bloqueo
ON dbo.DetalleCompra
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
          FROM (SELECT id_compra FROM inserted UNION SELECT id_compra FROM deleted) AS x
          JOIN dbo.Compra AS h ON h.id_compra = x.id_compra
         WHERE h.estado <> 'BORRADOR'
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50301, 'DetalleCompra: la compra ya no esta en BORRADOR; sus detalles no se pueden modificar.', 1;
    END
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_DetalleVenta_bloqueo
ON dbo.DetalleVenta
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
          FROM (SELECT id_venta FROM inserted UNION SELECT id_venta FROM deleted) AS x
          JOIN dbo.Venta AS h ON h.id_venta = x.id_venta
         WHERE h.estado <> 'BORRADOR'
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50302, 'DetalleVenta: la venta ya no esta en BORRADOR; sus detalles no se pueden modificar.', 1;
    END
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_DetalleDevolucion_bloqueo
ON dbo.DetalleDevolucion
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
          FROM (SELECT id_devolucion FROM inserted UNION SELECT id_devolucion FROM deleted) AS x
          JOIN dbo.Devolucion AS h ON h.id_devolucion = x.id_devolucion
         WHERE h.estado <> 'BORRADOR'
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50303, 'DetalleDevolucion: la devolucion ya no esta en BORRADOR; sus detalles no se pueden modificar.', 1;
    END
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_DetalleApartado_bloqueo
ON dbo.DetalleApartado
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
          FROM (SELECT id_apartado FROM inserted UNION SELECT id_apartado FROM deleted) AS x
          JOIN dbo.Apartado AS h ON h.id_apartado = x.id_apartado
         WHERE h.estado <> 'ACTIVO'
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50304, 'DetalleApartado: el apartado ya no esta ACTIVO; sus detalles no se pueden modificar.', 1;
    END
END;
GO









CREATE OR ALTER TRIGGER dbo.trg_MovimientoInventario_validar
ON dbo.MovimientoInventario
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;


    IF EXISTS (
        SELECT 1 FROM inserted AS i
         WHERE (i.tipo IN ('COMPRA','TRASPASO_ENTRADA','DEVOLUCION') AND i.cantidad_delta < 0)
            OR (i.tipo IN ('VENTA','TRASPASO_SALIDA')               AND i.cantidad_delta > 0)
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50401, 'MovimientoInventario: el signo de cantidad_delta no corresponde al tipo de movimiento.', 1;
    END


    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          LEFT JOIN dbo.DetalleCompra     AS dc  ON dc.id_detalle_compra     = i.id_detalle_compra
          LEFT JOIN dbo.Compra            AS c   ON c.id_compra              = dc.id_compra
          LEFT JOIN dbo.DetalleVenta      AS dv  ON dv.id_detalle_venta      = i.id_detalle_venta
          LEFT JOIN dbo.Venta             AS v   ON v.id_venta               = dv.id_venta
          LEFT JOIN dbo.DetalleTraspaso   AS dt  ON dt.id_detalle_traspaso   = i.id_detalle_traspaso
          LEFT JOIN dbo.Traspaso          AS t   ON t.id_traspaso            = dt.id_traspaso
          LEFT JOIN dbo.DetalleDevolucion AS dd  ON dd.id_detalle_devolucion = i.id_detalle_devolucion
          LEFT JOIN dbo.DetalleVenta      AS dvd ON dvd.id_detalle_venta     = dd.id_detalle_venta
          LEFT JOIN dbo.Devolucion        AS dev ON dev.id_devolucion        = dd.id_devolucion
         WHERE (i.tipo = 'COMPRA'           AND (dc.id_producto  <> i.id_producto OR c.id_tienda          <> i.id_tienda OR dc.cantidad  <> ABS(i.cantidad_delta)))
            OR (i.tipo = 'VENTA'            AND (dv.id_producto  <> i.id_producto OR v.id_tienda          <> i.id_tienda OR dv.cantidad  <> ABS(i.cantidad_delta)))
            OR (i.tipo = 'TRASPASO_SALIDA'  AND (dt.id_producto  <> i.id_producto OR t.id_tienda_origen   <> i.id_tienda OR dt.cantidad  <> ABS(i.cantidad_delta)))
            OR (i.tipo = 'TRASPASO_ENTRADA' AND (dt.id_producto  <> i.id_producto OR t.id_tienda_destino  <> i.id_tienda OR dt.cantidad  <> ABS(i.cantidad_delta)))
            OR (i.tipo = 'DEVOLUCION'       AND (dvd.id_producto <> i.id_producto OR dev.id_tienda        <> i.id_tienda OR dd.cantidad  <> ABS(i.cantidad_delta) OR dd.reintegra_stock = 0))
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50402, 'MovimientoInventario: tienda, producto o cantidad no coinciden con el documento de origen.', 1;
    END


    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN dbo.MovimientoInventario AS o ON o.id_movimiento = i.id_movimiento_revertido
         WHERE i.tipo = 'REVERSO'
           AND (o.id_tienda <> i.id_tienda OR o.id_producto <> i.id_producto OR o.cantidad_delta <> -i.cantidad_delta)
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50403, 'MovimientoInventario: el REVERSO debe tener la misma tienda y producto y la cantidad opuesta al movimiento original.', 1;
    END
END;
GO






CREATE OR ALTER TRIGGER dbo.trg_DetalleDevolucion_validar
ON dbo.DetalleDevolucion
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;


    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN dbo.Devolucion  AS d ON d.id_devolucion    = i.id_devolucion
          JOIN dbo.DetalleVenta AS v ON v.id_detalle_venta = i.id_detalle_venta
         WHERE v.id_venta <> d.id_venta
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50501, 'DetalleDevolucion: la linea de venta no pertenece a la venta de la devolucion.', 1;
    END


    IF EXISTS (
        SELECT 1
          FROM (SELECT DISTINCT id_detalle_venta FROM inserted) AS x
          JOIN dbo.DetalleVenta AS v ON v.id_detalle_venta = x.id_detalle_venta
         WHERE (SELECT SUM(dd.cantidad)
                  FROM dbo.DetalleDevolucion AS dd
                  JOIN dbo.Devolucion AS d ON d.id_devolucion = dd.id_devolucion
                 WHERE dd.id_detalle_venta = x.id_detalle_venta
                   AND d.estado <> 'ANULADA') > v.cantidad
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50502, 'DetalleDevolucion: lo devuelto acumulado supera la cantidad vendida.', 1;
    END
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Reembolso_validar
ON dbo.Reembolso
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;


    IF EXISTS (SELECT 1 FROM inserted AS i WHERE i.id_devolucion IS NOT NULL AND i.id_pago_venta IS NULL)
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50601, 'Reembolso: un reembolso con devolucion debe indicar el pago de la venta (id_pago_venta).', 1;
    END

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN dbo.Devolucion AS d ON d.id_devolucion = i.id_devolucion
          JOIN dbo.PagoVenta  AS p ON p.id_pago_venta = i.id_pago_venta
         WHERE p.id_venta <> d.id_venta
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50602, 'Reembolso: el pago no corresponde a la venta de la devolucion.', 1;
    END


    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN dbo.AbonoApartado AS a ON a.id_abono = i.id_abono
         WHERE a.id_pago_venta IS NOT NULL
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50603, 'Reembolso: el abono ya se aplico a una venta; reembolsalo por su PagoVenta.', 1;
    END


    IF EXISTS (
        SELECT 1
          FROM (SELECT DISTINCT id_pago_venta FROM inserted WHERE id_pago_venta IS NOT NULL) AS x
          JOIN dbo.PagoVenta AS p ON p.id_pago_venta = x.id_pago_venta
         WHERE (SELECT SUM(r.monto) FROM dbo.Reembolso AS r WHERE r.id_pago_venta = x.id_pago_venta) > p.monto
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50604, 'Reembolso: el total reembolsado supera el monto del pago original.', 1;
    END

    IF EXISTS (
        SELECT 1
          FROM (SELECT DISTINCT id_abono FROM inserted WHERE id_abono IS NOT NULL) AS x
          JOIN dbo.AbonoApartado AS a ON a.id_abono = x.id_abono
         WHERE (SELECT SUM(r.monto) FROM dbo.Reembolso AS r WHERE r.id_abono = x.id_abono) > a.monto
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50605, 'Reembolso: el total reembolsado supera el monto del abono original.', 1;
    END
END;
GO






SELECT OBJECT_NAME(parent_id) AS tabla, name AS trigger_nombre, is_disabled
FROM sys.triggers
WHERE parent_class = 1 AND is_ms_shipped = 0
ORDER BY OBJECT_NAME(parent_id), name;

SELECT COUNT(*) AS total_triggers
FROM sys.triggers
WHERE parent_class = 1 AND is_ms_shipped = 0;
GO






DECLARE @docs TABLE(tabla sysname, pk sysname, inicial varchar(20));
INSERT @docs VALUES ('Compra','id_compra','BORRADOR'),('Venta','id_venta','BORRADOR'),
 ('Devolucion','id_devolucion','BORRADOR'),('Traspaso','id_traspaso','PENDIENTE'),
 ('Apartado','id_apartado','ACTIVO');
DECLARE @t sysname, @pk sysname, @inicial varchar(20), @sql nvarchar(max);
DECLARE documentos CURSOR LOCAL FAST_FORWARD FOR SELECT tabla,pk,inicial FROM @docs;
OPEN documentos; FETCH NEXT FROM documentos INTO @t,@pk,@inicial;
WHILE @@FETCH_STATUS=0
BEGIN
 SET @sql=N'CREATE OR ALTER TRIGGER dbo.trg_'+@t+N'_alta_baja ON dbo.'+QUOTENAME(@t)+N'
 AFTER INSERT, DELETE AS
 BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM deleted) THROW 50701, ''Use la anulacion/cancelacion; no elimine documentos.'',1;
 IF EXISTS(SELECT 1 FROM inserted WHERE estado <> '''+@inicial+N''')
 THROW 50702, ''El documento debe crearse en su estado inicial.'',1;
 END;';
 EXEC sys.sp_executesql @sql;
 FETCH NEXT FROM documentos INTO @t,@pk,@inicial;
END
CLOSE documentos; DEALLOCATE documentos;
GO

CREATE OR ALTER TRIGGER dbo.trg_Compra_cabecera ON dbo.Compra AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_compra=i.id_compra
 WHERE i.id_tienda<>d.id_tienda OR i.id_proveedor<>d.id_proveedor OR i.id_usuario<>d.id_usuario)
 THROW 50703,'No cambie tienda, proveedor ni autor de una compra existente.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.trg_Venta_cabecera ON dbo.Venta AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta
 WHERE i.id_tienda<>d.id_tienda OR i.id_cliente<>d.id_cliente OR i.id_usuario<>d.id_usuario)
 THROW 50704,'No cambie tienda, cliente ni autor de una venta existente.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.trg_Devolucion_cabecera ON dbo.Devolucion AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_devolucion=i.id_devolucion
 WHERE i.id_venta<>d.id_venta OR i.id_tienda<>d.id_tienda OR i.id_usuario<>d.id_usuario)
 THROW 50705,'No cambie la venta, tienda ni autor de una devolucion existente.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.trg_Traspaso_cabecera ON dbo.Traspaso AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_traspaso=i.id_traspaso
 WHERE i.id_tienda_origen<>d.id_tienda_origen OR i.id_tienda_destino<>d.id_tienda_destino
 OR i.solicitado_por<>d.solicitado_por)
 THROW 50706,'No cambie las tiendas ni el solicitante del traspaso.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.trg_Apartado_cabecera ON dbo.Apartado AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_apartado=i.id_apartado
 WHERE i.id_tienda<>d.id_tienda OR i.id_cliente<>d.id_cliente OR i.id_usuario<>d.id_usuario
 OR (d.estado<>'ACTIVO' AND (ISNULL(i.id_venta,0)<>ISNULL(d.id_venta,0) OR i.vence_en<>d.vence_en)))
 THROW 50707,'No cambie la identidad del apartado ni un enlace de venta cerrado.',1;
 IF EXISTS(SELECT id_venta FROM dbo.Apartado WITH(UPDLOCK,HOLDLOCK)
 WHERE id_venta IN(SELECT id_venta FROM inserted WHERE id_venta IS NOT NULL)
 GROUP BY id_venta HAVING COUNT(*)>1)
 THROW 50708,'Una venta no puede entregar mas de un apartado.',1;
END;
GO



DECLARE @detalles TABLE(tabla sysname,padre sysname,fk sysname,editable varchar(20));
INSERT @detalles VALUES ('DetalleCompra','Compra','id_compra','BORRADOR'),
 ('DetalleVenta','Venta','id_venta','BORRADOR'),
 ('DetalleDevolucion','Devolucion','id_devolucion','BORRADOR'),
 ('DetalleTraspaso','Traspaso','id_traspaso','PENDIENTE'),
 ('DetalleApartado','Apartado','id_apartado','ACTIVO'),
 ('PagoVenta','Venta','id_venta','BORRADOR');
DECLARE @t sysname,@p sysname,@fk sysname,@e varchar(20),@sql nvarchar(max);
DECLARE detalles CURSOR LOCAL FAST_FORWARD FOR SELECT tabla,padre,fk,editable FROM @detalles;
OPEN detalles; FETCH NEXT FROM detalles INTO @t,@p,@fk,@e;
WHILE @@FETCH_STATUS=0
BEGIN
 SET @sql=N'CREATE OR ALTER TRIGGER dbo.trg_'+@t+N'_bloqueo ON dbo.'+QUOTENAME(@t)+N'
 AFTER INSERT,UPDATE,DELETE AS
 BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM (SELECT '+QUOTENAME(@fk)+N' FROM inserted UNION SELECT '+QUOTENAME(@fk)+N' FROM deleted)x
 JOIN dbo.'+QUOTENAME(@p)+N' h WITH(UPDLOCK,HOLDLOCK) ON h.'+QUOTENAME(@fk)+N'=x.'+QUOTENAME(@fk)+N'
 WHERE h.estado<>'''+@e+N''') THROW 50709,''El documento ya no permite modificar detalles o pagos.'',1;
 END;';
 EXEC sys.sp_executesql @sql;
 FETCH NEXT FROM detalles INTO @t,@p,@fk,@e;
END
CLOSE detalles; DEALLOCATE detalles;
GO




CREATE OR ALTER TRIGGER dbo.trg_MovimientoInventario_aplicar
ON dbo.MovimientoInventario AFTER INSERT AS
BEGIN
 SET NOCOUNT ON;
 DECLARE @d TABLE(id_tienda int,id_producto int,delta bigint,PRIMARY KEY(id_tienda,id_producto));
 INSERT @d SELECT id_tienda,id_producto,SUM(CONVERT(bigint,cantidad_delta))
 FROM inserted GROUP BY id_tienda,id_producto;
 INSERT dbo.InventarioTienda(id_tienda,id_producto)
 SELECT d.id_tienda,d.id_producto FROM @d d
 WHERE NOT EXISTS(SELECT 1 FROM dbo.InventarioTienda t WITH(UPDLOCK,HOLDLOCK)
 WHERE t.id_tienda=d.id_tienda AND t.id_producto=d.id_producto);
 IF EXISTS(SELECT 1 FROM @d d JOIN dbo.InventarioTienda t WITH(UPDLOCK,HOLDLOCK)
 ON t.id_tienda=d.id_tienda AND t.id_producto=d.id_producto
 WHERE CONVERT(bigint,t.existencia)+d.delta<t.reservado
 OR CONVERT(bigint,t.existencia)+d.delta>2147483647)
 THROW 50710,'Inventario insuficiente, comprometido en reservas o fuera de rango.',1;
 UPDATE t SET existencia=CONVERT(int,CONVERT(bigint,t.existencia)+d.delta)
 FROM dbo.InventarioTienda t JOIN @d d
 ON t.id_tienda=d.id_tienda AND t.id_producto=d.id_producto;
END;
GO



CREATE OR ALTER TRIGGER dbo.trg_MovimientoInventario_documento
ON dbo.MovimientoInventario AFTER INSERT AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i
 LEFT JOIN dbo.DetalleCompra dc ON dc.id_detalle_compra=i.id_detalle_compra
 LEFT JOIN dbo.Compra c ON c.id_compra=dc.id_compra
 LEFT JOIN dbo.DetalleVenta dv ON dv.id_detalle_venta=i.id_detalle_venta
 LEFT JOIN dbo.Venta v ON v.id_venta=dv.id_venta
 LEFT JOIN dbo.DetalleTraspaso dt ON dt.id_detalle_traspaso=i.id_detalle_traspaso
 LEFT JOIN dbo.Traspaso t ON t.id_traspaso=dt.id_traspaso
 LEFT JOIN dbo.DetalleDevolucion dd ON dd.id_detalle_devolucion=i.id_detalle_devolucion
 LEFT JOIN dbo.Devolucion dev ON dev.id_devolucion=dd.id_devolucion
 WHERE (i.tipo='COMPRA' AND c.estado<>'CONFIRMADA')
 OR(i.tipo='VENTA' AND v.estado<>'CONFIRMADA')
 OR(i.tipo='TRASPASO_SALIDA' AND t.estado<>'EN_TRANSITO')
 OR(i.tipo='TRASPASO_ENTRADA' AND t.estado<>'RECIBIDO')
 OR(i.tipo='DEVOLUCION' AND dev.estado<>'CONFIRMADA'))
 THROW 50711,'El documento de origen no esta en el estado requerido para contabilizar.',1;
END;
GO



CREATE OR ALTER TRIGGER dbo.trg_DetalleApartado_reservas
ON dbo.DetalleApartado AFTER INSERT,UPDATE,DELETE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM dbo.vw_ApartadoTotal t
 JOIN(SELECT id_apartado FROM inserted UNION SELECT id_apartado FROM deleted)x ON x.id_apartado=t.id_apartado
 WHERE(SELECT ISNULL(SUM(monto),0) FROM dbo.AbonoApartado WITH(UPDLOCK,HOLDLOCK)
 WHERE id_apartado=t.id_apartado)>t.total)
 THROW 50734,'No reduzca el total del apartado por debajo de sus abonos.',1;
 DECLARE @d TABLE(id_tienda int,id_producto int,delta bigint,PRIMARY KEY(id_tienda,id_producto));
 INSERT @d
 SELECT a.id_tienda,x.id_producto,SUM(x.delta) FROM
 (SELECT id_apartado,id_producto,CONVERT(bigint,cantidad) delta FROM inserted
 UNION ALL SELECT id_apartado,id_producto,-CONVERT(bigint,cantidad) FROM deleted)x
 JOIN dbo.Apartado a WITH(UPDLOCK,HOLDLOCK) ON a.id_apartado=x.id_apartado
 GROUP BY a.id_tienda,x.id_producto;
 IF EXISTS(SELECT 1 FROM @d d LEFT JOIN dbo.InventarioTienda t WITH(UPDLOCK,HOLDLOCK)
 ON t.id_tienda=d.id_tienda AND t.id_producto=d.id_producto
 WHERE (t.id_producto IS NULL AND d.delta<>0)
 OR CONVERT(bigint,t.reservado)+d.delta NOT BETWEEN 0 AND t.existencia)
 THROW 50712,'No hay inventario disponible para reservar el apartado.',1;
 UPDATE t SET reservado=CONVERT(int,CONVERT(bigint,t.reservado)+d.delta)
 FROM dbo.InventarioTienda t JOIN @d d
 ON t.id_tienda=d.id_tienda AND t.id_producto=d.id_producto;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Apartado_cierre ON dbo.Apartado AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_apartado=i.id_apartado
 WHERE d.estado='ACTIVO' AND i.estado='ENTREGADO')
 AND TRIGGER_NESTLEVEL(OBJECT_ID(N'dbo.trg_Venta_contabilizar'),'AFTER','DML')=0
 THROW 50733,'Enlace el apartado a la venta borrador y confirme la venta para entregarlo.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_apartado=i.id_apartado
 LEFT JOIN dbo.Venta v ON v.id_venta=i.id_venta
 WHERE d.estado='ACTIVO' AND i.estado='ENTREGADO'
 AND (v.id_venta IS NULL OR v.estado<>'CONFIRMADA' OR v.id_cliente<>i.id_cliente OR v.id_tienda<>i.id_tienda))
 THROW 50713,'Para entregar el apartado enlace una venta confirmada del mismo cliente y tienda.',1;
 ;WITH cantidades AS
 (SELECT i.id_tienda,da.id_producto,SUM(CONVERT(bigint,da.cantidad)) cantidad
 FROM inserted i JOIN deleted d ON d.id_apartado=i.id_apartado
 JOIN dbo.DetalleApartado da ON da.id_apartado=i.id_apartado
 WHERE d.estado='ACTIVO' AND i.estado<>'ACTIVO' GROUP BY i.id_tienda,da.id_producto)
 UPDATE t SET reservado=CONVERT(int,CONVERT(bigint,t.reservado)-c.cantidad)
 FROM dbo.InventarioTienda t JOIN cantidades c
 ON c.id_tienda=t.id_tienda AND c.id_producto=t.id_producto;
END;
GO




CREATE OR ALTER TRIGGER dbo.trg_Compra_contabilizar ON dbo.Compra AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF NOT UPDATE(estado) RETURN;
 DECLARE @actor int=TRY_CONVERT(int,SESSION_CONTEXT(N'id_usuario'));
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_compra=i.id_compra WHERE i.estado<>d.estado)
 AND NOT EXISTS(SELECT 1 FROM dbo.Usuario WHERE id_usuario=@actor AND activo=1)
 THROW 50714,'Establezca SESSION_CONTEXT id_usuario con un usuario activo.',1;
 INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,id_detalle_compra)
 SELECT i.id_tienda,dc.id_producto,'COMPRA',dc.cantidad,@actor,dc.id_detalle_compra
 FROM inserted i JOIN deleted d ON d.id_compra=i.id_compra
 JOIN dbo.DetalleCompra dc ON dc.id_compra=i.id_compra
 WHERE d.estado='BORRADOR' AND i.estado='CONFIRMADA';
 INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,id_movimiento_revertido,motivo)
 SELECT m.id_tienda,m.id_producto,'REVERSO',-m.cantidad_delta,@actor,m.id_movimiento,i.motivo_anulacion
 FROM inserted i JOIN deleted d ON d.id_compra=i.id_compra
 JOIN dbo.DetalleCompra dc ON dc.id_compra=i.id_compra
 JOIN dbo.MovimientoInventario m ON m.id_detalle_compra=dc.id_detalle_compra AND m.tipo='COMPRA'
 WHERE d.estado='CONFIRMADA' AND i.estado='ANULADA';
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Venta_contabilizar ON dbo.Venta AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF NOT UPDATE(estado) RETURN;
 DECLARE @actor int=TRY_CONVERT(int,SESSION_CONTEXT(N'id_usuario'));
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta WHERE i.estado<>d.estado)
 AND NOT EXISTS(SELECT 1 FROM dbo.Usuario WHERE id_usuario=@actor AND activo=1)
 THROW 50714,'Establezca SESSION_CONTEXT id_usuario con un usuario activo.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta
 JOIN dbo.Devolucion dev WITH(UPDLOCK,HOLDLOCK) ON dev.id_venta=i.id_venta
 WHERE d.estado='CONFIRMADA' AND i.estado='ANULADA' AND dev.estado<>'ANULADA')
 THROW 50715,'Anule las devoluciones antes de anular la venta.',1;

 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta
 JOIN dbo.Apartado a WITH(UPDLOCK,HOLDLOCK) ON a.id_venta=i.id_venta
 WHERE d.estado='BORRADOR' AND i.estado='CONFIRMADA' AND
 (a.estado<>'ACTIVO' OR a.id_tienda<>i.id_tienda OR a.id_cliente<>i.id_cliente
 OR EXISTS(SELECT id_producto,precio_unitario,tasa_isv,SUM(CONVERT(bigint,cantidad)) FROM dbo.DetalleApartado
 WHERE id_apartado=a.id_apartado GROUP BY id_producto,precio_unitario,tasa_isv
 EXCEPT SELECT id_producto,precio_unitario,tasa_isv,SUM(CONVERT(bigint,cantidad)) FROM dbo.DetalleVenta
 WHERE id_venta=i.id_venta GROUP BY id_producto,precio_unitario,tasa_isv)
 OR EXISTS(SELECT id_producto,precio_unitario,tasa_isv,SUM(CONVERT(bigint,cantidad)) FROM dbo.DetalleVenta
 WHERE id_venta=i.id_venta GROUP BY id_producto,precio_unitario,tasa_isv
 EXCEPT SELECT id_producto,precio_unitario,tasa_isv,SUM(CONVERT(bigint,cantidad)) FROM dbo.DetalleApartado
 WHERE id_apartado=a.id_apartado GROUP BY id_producto,precio_unitario,tasa_isv)))
 THROW 50716,'Los datos de venta no coinciden con el apartado enlazado.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta
 JOIN dbo.Apartado a ON a.id_venta=i.id_venta JOIN dbo.AbonoApartado ab ON ab.id_apartado=a.id_apartado
 LEFT JOIN dbo.PagoVenta p ON p.id_pago_venta=ab.id_pago_venta
 WHERE d.estado='BORRADOR' AND i.estado='CONFIRMADA'
 AND(p.id_pago_venta IS NULL OR p.id_venta<>i.id_venta OR p.monto<>ab.monto OR p.id_metodo_pago<>ab.id_metodo_pago))
 THROW 50717,'Aplique cada abono a un pago de la venta antes de confirmar.',1;

 UPDATE a SET estado='ENTREGADO' FROM dbo.Apartado a
 JOIN inserted i ON i.id_venta=a.id_venta JOIN deleted d ON d.id_venta=i.id_venta
 WHERE d.estado='BORRADOR' AND i.estado='CONFIRMADA' AND a.estado='ACTIVO';
 INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,id_detalle_venta)
 SELECT i.id_tienda,dv.id_producto,'VENTA',-dv.cantidad,@actor,dv.id_detalle_venta
 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta JOIN dbo.DetalleVenta dv ON dv.id_venta=i.id_venta
 WHERE d.estado='BORRADOR' AND i.estado='CONFIRMADA';
 INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,id_movimiento_revertido,motivo)
 SELECT m.id_tienda,m.id_producto,'REVERSO',-m.cantidad_delta,@actor,m.id_movimiento,i.motivo_anulacion
 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta
 JOIN dbo.DetalleVenta dv ON dv.id_venta=i.id_venta
 JOIN dbo.MovimientoInventario m ON m.id_detalle_venta=dv.id_detalle_venta AND m.tipo='VENTA'
 WHERE d.estado='CONFIRMADA' AND i.estado='ANULADA';
END;
GO



CREATE OR ALTER TRIGGER dbo.trg_Traspaso_operar ON dbo.Traspaso AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 DECLARE @actor int=TRY_CONVERT(int,SESSION_CONTEXT(N'id_usuario'));
 IF EXISTS(SELECT 1 FROM inserted i LEFT JOIN deleted d ON d.id_traspaso=i.id_traspaso
 WHERE d.id_traspaso IS NULL OR i.estado<>d.estado)
 AND NOT EXISTS(SELECT 1 FROM dbo.Usuario WHERE id_usuario=@actor AND activo=1)
 THROW 50714,'Establezca SESSION_CONTEXT id_usuario con un usuario activo.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_traspaso=i.id_traspaso
 WHERE d.estado='PENDIENTE' AND i.estado='APROBADO'
 AND NOT EXISTS(SELECT 1 FROM dbo.DetalleTraspaso x WHERE x.id_traspaso=i.id_traspaso))
 THROW 50718,'No puede aprobar un traspaso sin detalles.',1;
 DECLARE @r TABLE(id_tienda int,id_producto int,delta bigint,PRIMARY KEY(id_tienda,id_producto));
 INSERT @r SELECT i.id_tienda_origen,dt.id_producto,
 SUM(CONVERT(bigint,dt.cantidad)*CASE WHEN d.estado='PENDIENTE' THEN 1 ELSE -1 END)
 FROM inserted i JOIN deleted d ON d.id_traspaso=i.id_traspaso
 JOIN dbo.DetalleTraspaso dt ON dt.id_traspaso=i.id_traspaso
 WHERE(d.estado='PENDIENTE' AND i.estado='APROBADO')
 OR(d.estado='APROBADO' AND i.estado IN('EN_TRANSITO','CANCELADO'))
 GROUP BY i.id_tienda_origen,dt.id_producto;
 IF EXISTS(SELECT 1 FROM @r r LEFT JOIN dbo.InventarioTienda t WITH(UPDLOCK,HOLDLOCK)
 ON t.id_tienda=r.id_tienda AND t.id_producto=r.id_producto
 WHERE t.id_producto IS NULL OR CONVERT(bigint,t.reservado)+r.delta NOT BETWEEN 0 AND t.existencia)
 THROW 50719,'Inventario insuficiente para reservar o liberar el traspaso.',1;
 UPDATE t SET reservado=CONVERT(int,CONVERT(bigint,t.reservado)+r.delta)
 FROM dbo.InventarioTienda t JOIN @r r ON r.id_tienda=t.id_tienda AND r.id_producto=t.id_producto;
 INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,id_detalle_traspaso)
 SELECT i.id_tienda_origen,dt.id_producto,'TRASPASO_SALIDA',-dt.cantidad,@actor,dt.id_detalle_traspaso
 FROM inserted i JOIN deleted d ON d.id_traspaso=i.id_traspaso
 JOIN dbo.DetalleTraspaso dt ON dt.id_traspaso=i.id_traspaso
 WHERE d.estado='APROBADO' AND i.estado='EN_TRANSITO';
 INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,id_detalle_traspaso)
 SELECT i.id_tienda_destino,dt.id_producto,'TRASPASO_ENTRADA',dt.cantidad,@actor,dt.id_detalle_traspaso
 FROM inserted i JOIN deleted d ON d.id_traspaso=i.id_traspaso
 JOIN dbo.DetalleTraspaso dt ON dt.id_traspaso=i.id_traspaso
 WHERE d.estado='EN_TRANSITO' AND i.estado='RECIBIDO';
 INSERT dbo.EventoTraspaso(id_traspaso,estado_anterior,estado_nuevo,id_usuario,motivo)
 SELECT i.id_traspaso,d.estado,i.estado,@actor,CONVERT(nvarchar(500),SESSION_CONTEXT(N'motivo'))
 FROM inserted i LEFT JOIN deleted d ON d.id_traspaso=i.id_traspaso
 WHERE d.id_traspaso IS NULL OR i.estado<>d.estado;
END;
GO


CREATE OR ALTER TRIGGER dbo.trg_DetalleDevolucion_validar
ON dbo.DetalleDevolucion AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 DECLARE @bloqueados bigint;
 SELECT @bloqueados=COUNT_BIG(*) FROM dbo.DetalleVenta v WITH(UPDLOCK,HOLDLOCK)
 JOIN(SELECT id_detalle_venta FROM inserted UNION SELECT id_detalle_venta FROM deleted)x
 ON x.id_detalle_venta=v.id_detalle_venta;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.Devolucion d ON d.id_devolucion=i.id_devolucion
 JOIN dbo.DetalleVenta v ON v.id_detalle_venta=i.id_detalle_venta WHERE v.id_venta<>d.id_venta)
 THROW 50501,'El detalle no pertenece a la venta original.',1;
 IF EXISTS(SELECT 1 FROM dbo.DetalleVenta v JOIN(SELECT DISTINCT id_detalle_venta FROM inserted)x
 ON x.id_detalle_venta=v.id_detalle_venta
 WHERE(SELECT SUM(CONVERT(bigint,dd.cantidad)) FROM dbo.DetalleDevolucion dd WITH(UPDLOCK,HOLDLOCK)
 JOIN dbo.Devolucion d WITH(UPDLOCK,HOLDLOCK) ON d.id_devolucion=dd.id_devolucion
 WHERE dd.id_detalle_venta=v.id_detalle_venta AND d.estado<>'ANULADA')>v.cantidad)
 THROW 50502,'La cantidad acumulada devuelta supera lo vendido.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.trg_Devolucion_contabilizar ON dbo.Devolucion AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF NOT UPDATE(estado) RETURN;
 DECLARE @actor int=TRY_CONVERT(int,SESSION_CONTEXT(N'id_usuario'));
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_devolucion=i.id_devolucion WHERE i.estado<>d.estado)
 AND NOT EXISTS(SELECT 1 FROM dbo.Usuario WHERE id_usuario=@actor AND activo=1)
 THROW 50714,'Establezca SESSION_CONTEXT id_usuario con un usuario activo.',1;
 INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,id_detalle_devolucion)
 SELECT i.id_tienda,dv.id_producto,'DEVOLUCION',dd.cantidad,@actor,dd.id_detalle_devolucion
 FROM inserted i JOIN deleted d ON d.id_devolucion=i.id_devolucion
 JOIN dbo.DetalleDevolucion dd ON dd.id_devolucion=i.id_devolucion
 JOIN dbo.DetalleVenta dv ON dv.id_detalle_venta=dd.id_detalle_venta
 WHERE d.estado='BORRADOR' AND i.estado='CONFIRMADA' AND dd.reintegra_stock=1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_devolucion=i.id_devolucion
 JOIN dbo.Reembolso r ON r.id_devolucion=i.id_devolucion
 WHERE d.estado='CONFIRMADA' AND i.estado='ANULADA')
 THROW 50720,'Una devolucion reembolsada necesita conciliacion financiera antes de anularse.',1;
 INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,id_movimiento_revertido,motivo)
 SELECT m.id_tienda,m.id_producto,'REVERSO',-m.cantidad_delta,@actor,m.id_movimiento,i.motivo_anulacion
 FROM inserted i JOIN deleted d ON d.id_devolucion=i.id_devolucion
 JOIN dbo.DetalleDevolucion dd ON dd.id_devolucion=i.id_devolucion
 JOIN dbo.MovimientoInventario m ON m.id_detalle_devolucion=dd.id_detalle_devolucion AND m.tipo='DEVOLUCION'
 WHERE d.estado='CONFIRMADA' AND i.estado='ANULADA';
END;
GO



CREATE OR ALTER TRIGGER dbo.trg_Reembolso_concurrencia ON dbo.Reembolso AFTER INSERT,UPDATE,DELETE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM deleted) THROW 50721,'Los reembolsos son historicos; no se modifican ni eliminan.',1;
 DECLARE @n bigint;
 SELECT @n=COUNT_BIG(*) FROM dbo.PagoVenta p WITH(UPDLOCK,HOLDLOCK)
 JOIN(SELECT DISTINCT id_pago_venta FROM inserted)x ON x.id_pago_venta=p.id_pago_venta;
 SELECT @n=COUNT_BIG(*) FROM dbo.AbonoApartado a WITH(UPDLOCK,HOLDLOCK)
 JOIN(SELECT DISTINCT id_abono FROM inserted)x ON x.id_abono=a.id_abono;
 SELECT @n=COUNT_BIG(*) FROM dbo.Devolucion d WITH(UPDLOCK,HOLDLOCK)
 JOIN(SELECT DISTINCT id_devolucion FROM inserted)x ON x.id_devolucion=d.id_devolucion;
 IF EXISTS(SELECT 1 FROM dbo.PagoVenta p JOIN(SELECT DISTINCT id_pago_venta FROM inserted)x
 ON x.id_pago_venta=p.id_pago_venta WHERE(SELECT SUM(r.monto) FROM dbo.Reembolso r WITH(UPDLOCK,HOLDLOCK)
 WHERE r.id_pago_venta=p.id_pago_venta)>p.monto)
 OR EXISTS(SELECT 1 FROM dbo.AbonoApartado a JOIN(SELECT DISTINCT id_abono FROM inserted)x
 ON x.id_abono=a.id_abono WHERE(SELECT SUM(r.monto) FROM dbo.Reembolso r WITH(UPDLOCK,HOLDLOCK)
 WHERE r.id_abono=a.id_abono)>a.monto)
 THROW 50722,'El reembolso acumulado supera el cobro original.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.Devolucion d ON d.id_devolucion=i.id_devolucion
 WHERE d.estado<>'CONFIRMADA') THROW 50723,'Solo se reembolsa una devolucion confirmada.',1;
 IF EXISTS(SELECT 1 FROM dbo.vw_DevolucionTotal t JOIN(SELECT DISTINCT id_devolucion FROM inserted)x
 ON x.id_devolucion=t.id_devolucion
 WHERE(SELECT SUM(r.monto) FROM dbo.Reembolso r WITH(UPDLOCK,HOLDLOCK)
 WHERE r.id_devolucion=t.id_devolucion)>t.total)
 THROW 50724,'El reembolso acumulado supera el importe de la devolucion.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.PagoVenta p ON p.id_pago_venta=i.id_pago_venta
 JOIN dbo.Venta v ON v.id_venta=p.id_venta WHERE i.id_devolucion IS NULL AND v.estado<>'ANULADA')
 THROW 50725,'Un reembolso sin devolucion requiere una venta anulada.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.AbonoApartado a ON a.id_abono=i.id_abono
 JOIN dbo.Apartado h ON h.id_apartado=a.id_apartado WHERE h.estado NOT IN('CANCELADO','VENCIDO'))
 THROW 50726,'Solo se reembolsa un abono de apartado cancelado o vencido.',1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_AbonoApartado_proteger ON dbo.AbonoApartado AFTER INSERT,UPDATE,DELETE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM deleted d LEFT JOIN inserted i ON i.id_abono=d.id_abono
 WHERE i.id_abono IS NULL OR i.id_apartado<>d.id_apartado OR i.monto<>d.monto
 OR i.id_metodo_pago<>d.id_metodo_pago OR i.id_usuario<>d.id_usuario
 OR i.fecha<>d.fecha OR ISNULL(i.referencia,N'')<>ISNULL(d.referencia,N'')
 OR(d.id_pago_venta IS NOT NULL AND ISNULL(i.id_pago_venta,0)<>d.id_pago_venta))
 THROW 50727,'No altere el cobro original de un abono; solo puede enlazarlo una vez a su pago.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.Apartado a WITH(UPDLOCK,HOLDLOCK) ON a.id_apartado=i.id_apartado
 WHERE a.estado<>'ACTIVO') THROW 50728,'El apartado no permite nuevos abonos o enlaces.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.Apartado a ON a.id_apartado=i.id_apartado
 LEFT JOIN dbo.PagoVenta p ON p.id_pago_venta=i.id_pago_venta
 WHERE i.id_pago_venta IS NOT NULL AND
 (p.id_venta<>a.id_venta OR a.id_venta IS NULL OR p.monto<>i.monto OR p.id_metodo_pago<>i.id_metodo_pago))
 THROW 50729,'El pago enlazado no corresponde al apartado, monto o metodo de pago.',1;
 IF EXISTS(SELECT id_pago_venta FROM dbo.AbonoApartado WITH(UPDLOCK,HOLDLOCK)
 WHERE id_pago_venta IN(SELECT id_pago_venta FROM inserted WHERE id_pago_venta IS NOT NULL)
 GROUP BY id_pago_venta HAVING COUNT(*)>1)
 THROW 50730,'Cada abono debe enlazarse a su propio pago.',1;
 IF EXISTS(SELECT 1 FROM dbo.vw_ApartadoTotal t JOIN(SELECT DISTINCT id_apartado FROM inserted)x
 ON x.id_apartado=t.id_apartado WHERE(SELECT SUM(monto) FROM dbo.AbonoApartado WITH(UPDLOCK,HOLDLOCK)
 WHERE id_apartado=t.id_apartado)>t.total)
 THROW 50731,'Los abonos superan el total del apartado.',1;
END;
GO



CREATE OR ALTER TRIGGER dbo.trg_Categoria_auditoria ON dbo.Categoria AFTER INSERT,UPDATE,DELETE AS
BEGIN
 SET NOCOUNT ON;

 IF TRIGGER_NESTLEVEL()>1 RETURN;
 INSERT dbo.Auditoria(id_usuario,modulo,accion,entidad,id_registro,descripcion,valores_antes_json,valores_despues_json,equipo)
 SELECT TRY_CONVERT(int,SESSION_CONTEXT(N'id_usuario')),'CATALOGO',
 CASE WHEN d.id_categoria IS NULL THEN 'CREAR' WHEN i.id_categoria IS NULL THEN 'ELIMINAR'
 WHEN d.activo=1 AND i.activo=0 THEN 'DESACTIVAR' WHEN d.activo=0 AND i.activo=1 THEN 'REACTIVAR' ELSE 'MODIFICAR' END,
 'Categoria',CONVERT(nvarchar(100),COALESCE(i.id_categoria,d.id_categoria)),N'Cambio de categoria',
 CASE WHEN d.id_categoria IS NOT NULL THEN(SELECT d.nombre,d.descripcion,d.activo FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES) END,
 CASE WHEN i.id_categoria IS NOT NULL THEN(SELECT i.nombre,i.descripcion,i.activo FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES) END,
 HOST_NAME()
 FROM inserted i FULL JOIN deleted d ON d.id_categoria=i.id_categoria;
END;
GO




CREATE OR ALTER PROCEDURE dbo.usp_ProcesarVencimientos @id_usuario int AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF @@TRANCOUNT<>0 THROW 50732,'Ejecute esta tarea fuera de una transaccion existente.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.Usuario WHERE id_usuario=@id_usuario AND activo=1)
 THROW 50714,'Se requiere un usuario de servicio activo.',1;
 DECLARE @antes sql_variant=SESSION_CONTEXT(N'id_usuario');
 BEGIN TRY
  EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@id_usuario;
  BEGIN TRANSACTION;
  UPDATE dbo.Apartado SET estado='VENCIDO' WHERE estado='ACTIVO' AND vence_en<=SYSUTCDATETIME();
  INSERT dbo.EventoNotificacion(clave_evento,tipo,titulo,mensaje)
  SELECT CONCAT('INVERSION:',id_inversion,':',CONVERT(char(8),fecha_vencimiento,112)),
  'INVERSION_VENCIMIENTO',N'Inversion vencida',CONCAT(N'La inversion ',id_inversion,N' ha alcanzado su vencimiento.')
  FROM dbo.Inversion i WHERE i.activo=1 AND i.fecha_vencimiento<=CONVERT(date,SYSUTCDATETIME())
  AND NOT EXISTS(SELECT 1 FROM dbo.EventoNotificacion e WITH(UPDLOCK,HOLDLOCK)
  WHERE e.clave_evento=CONCAT('INVERSION:',i.id_inversion,':',CONVERT(char(8),i.fecha_vencimiento,112)));
  COMMIT;
  EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@antes;
 END TRY
 BEGIN CATCH
  IF XACT_STATE()<>0 ROLLBACK;
  EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@antes;
  THROW;
 END CATCH
END;
GO



CREATE OR ALTER TRIGGER dbo.trg_InventarioTienda_aviso ON dbo.InventarioTienda AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 INSERT dbo.EventoNotificacion(clave_evento,tipo,titulo,mensaje)
 SELECT CONCAT('STOCK:',i.id_tienda,':',i.id_producto,':',CONVERT(char(8),SYSUTCDATETIME(),112)),
 'STOCK_BAJO',N'Stock disponible bajo',CONCAT(N'Producto ',i.id_producto,N', tienda ',i.id_tienda,
 N': disponible ',i.existencia-i.reservado,N', minimo ',i.stock_minimo)
 FROM inserted i WHERE i.existencia-i.reservado<i.stock_minimo
 AND NOT EXISTS(SELECT 1 FROM dbo.EventoNotificacion e WITH(UPDLOCK,HOLDLOCK)
 WHERE e.clave_evento=CONCAT('STOCK:',i.id_tienda,':',i.id_producto,':',CONVERT(char(8),SYSUTCDATETIME(),112)));
END;
GO
CREATE OR ALTER TRIGGER dbo.trg_Traspaso_aviso ON dbo.Traspaso AFTER INSERT AS
BEGIN
 SET NOCOUNT ON;
 INSERT dbo.EventoNotificacion(clave_evento,tipo,titulo,mensaje)
 SELECT CONCAT('TRASPASO:',i.id_traspaso),'TRASPASO_PENDIENTE',N'Traspaso pendiente',
 CONCAT(N'Traspaso ',i.id_traspaso,N' de tienda ',i.id_tienda_origen,N' a ',i.id_tienda_destino)
 FROM inserted i WHERE NOT EXISTS(SELECT 1 FROM dbo.EventoNotificacion e WITH(UPDLOCK,HOLDLOCK)
 WHERE e.clave_evento=CONCAT('TRASPASO:',i.id_traspaso));
END;
GO



DECLARE @docs TABLE(tabla sysname,pk sysname);
INSERT @docs VALUES('Compra','id_compra'),('Venta','id_venta'),('Devolucion','id_devolucion'),
 ('Traspaso','id_traspaso'),('Apartado','id_apartado');
DECLARE @t sysname,@pk sysname,@sql nvarchar(max);
DECLARE auditar CURSOR LOCAL FAST_FORWARD FOR SELECT tabla,pk FROM @docs;
OPEN auditar; FETCH NEXT FROM auditar INTO @t,@pk;
WHILE @@FETCH_STATUS=0
BEGIN
 SET @sql=N'CREATE OR ALTER TRIGGER dbo.trg_'+@t+N'_auditoria ON dbo.'+QUOTENAME(@t)+N'
 AFTER INSERT,UPDATE AS BEGIN
 SET NOCOUNT ON;
 INSERT dbo.Auditoria(id_usuario,modulo,accion,entidad,id_registro,descripcion,valores_antes_json,valores_despues_json,equipo)
 SELECT TRY_CONVERT(int,SESSION_CONTEXT(N''id_usuario'')),''DOCUMENTOS'',
 CASE WHEN d.'+QUOTENAME(@pk)+N' IS NULL THEN ''CREAR'' ELSE ''MODIFICAR'' END,
 '''+@t+N''',CONVERT(nvarchar(100),i.'+QUOTENAME(@pk)+N'),N''Cambio de documento'',
 CASE WHEN d.'+QUOTENAME(@pk)+N' IS NOT NULL THEN(SELECT d.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES) END,
 (SELECT i.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES),HOST_NAME()
 FROM inserted i LEFT JOIN deleted d ON d.'+QUOTENAME(@pk)+N'=i.'+QUOTENAME(@pk)+N'; END;';
 EXEC sys.sp_executesql @sql;
 FETCH NEXT FROM auditar INTO @t,@pk;
END
CLOSE auditar; DEALLOCATE auditar;
GO


CREATE OR ALTER TRIGGER dbo.trg_Rol_seguridad ON dbo.Rol AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 DECLARE @ids dbo.tt_IdUsuario;
 INSERT @ids SELECT DISTINCT ur.id_usuario FROM dbo.UsuarioRol ur
 JOIN inserted i ON i.id_rol=ur.id_rol JOIN deleted d ON d.id_rol=i.id_rol
 WHERE i.activo<>d.activo;
 IF EXISTS(SELECT 1 FROM @ids) EXEC dbo.usp_InvalidarSesiones @ids;
END;
GO
CREATE OR ALTER TRIGGER dbo.trg_Permiso_seguridad ON dbo.Permiso AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 DECLARE @ids dbo.tt_IdUsuario;
 INSERT @ids SELECT DISTINCT ur.id_usuario FROM dbo.UsuarioRol ur
 JOIN dbo.RolPermiso rp ON rp.id_rol=ur.id_rol
 JOIN inserted i ON i.id_permiso=rp.id_permiso JOIN deleted d ON d.id_permiso=i.id_permiso
 WHERE i.activo<>d.activo OR i.modulo<>d.modulo OR i.accion<>d.accion;
 IF EXISTS(SELECT 1 FROM @ids) EXEC dbo.usp_InvalidarSesiones @ids;
END;
GO


SELECT OBJECT_NAME(parent_id) tabla,name trigger_nombre,is_disabled
FROM sys.triggers WHERE parent_class=1 AND is_ms_shipped=0
ORDER BY OBJECT_NAME(parent_id),name;
GO

select * from dbo.Categoria
```

## A.4 Base de Datos/poblar_bdd.sql

Carga datos de demostración para los módulos. Sus hashes de contraseña ficticios no son credenciales utilizables del login. Debe evaluarse en una base de prueba y no confundirse con una migración.

Extensión: 174 líneas. SHA-256 del texto normalizado a LF: 5c1f594e78e2c44130b5a538bc5a96f68de89eda76237394358c189df934c09c.

```sql




USE EquipoZAACC;
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF EXISTS (SELECT 1 FROM sys.triggers WHERE parent_class = 1 AND is_disabled = 0)
    THROW 51000, 'Ejecute esta carga antes de instalar Triggers2.sql, en una base de pruebas.', 1;
IF EXISTS (SELECT 1 FROM dbo.Tienda WHERE codigo IN ('DEMO-T1','DEMO-T2'))
    THROW 51001, 'Los datos DEMO ya existen. Esta carga se ejecuta una sola vez.', 1;

BEGIN TRY
    BEGIN TRANSACTION;


    DECLARE @tienda1 int, @tienda2 int;
    INSERT dbo.Tienda (codigo, nombre) VALUES ('DEMO-T1', N'Tienda demo central');
    SET @tienda1 = SCOPE_IDENTITY();
    INSERT dbo.Tienda (codigo, nombre) VALUES ('DEMO-T2', N'Tienda demo sucursal');
    SET @tienda2 = SCOPE_IDENTITY();


    DECLARE @i int = 1, @s varchar(1), @tienda int, @destino int;
    DECLARE @categoria int, @proveedor int, @cliente int, @metodo int,
            @empleado int, @usuario int, @rol int, @permiso int, @producto int,
            @compra int, @venta int, @detalleVenta int, @traspaso int,
            @apartado int, @abono int, @devolucion int, @importacion int;
    DECLARE @evento bigint, @ahora datetime2(3) = SYSUTCDATETIME();

    WHILE @i <= 2
    BEGIN
        SET @s = CONVERT(varchar(1), @i);
        SET @tienda = CASE WHEN @i = 1 THEN @tienda1 ELSE @tienda2 END;
        SET @destino = CASE WHEN @i = 1 THEN @tienda2 ELSE @tienda1 END;


        INSERT dbo.Categoria (nombre, descripcion)
        VALUES (N'Categoria demo ' + @s, N'Categoria de ejemplo');
        SET @categoria = SCOPE_IDENTITY();

        INSERT dbo.Proveedor (nombre_empresa, contacto)
        VALUES (N'Proveedor demo ' + @s, N'Contacto demo ' + @s);
        SET @proveedor = SCOPE_IDENTITY();

        INSERT dbo.Cliente (nombre, telefono)
        VALUES (N'Cliente demo ' + @s, N'5555000' + @s);
        SET @cliente = SCOPE_IDENTITY();

        INSERT dbo.MetodoPago (codigo, nombre)
        VALUES ('DEMO-P' + @s, N'Metodo demo ' + @s);
        SET @metodo = SCOPE_IDENTITY();

        INSERT dbo.Empleado (codigo, nombre, id_tienda)
        VALUES ('DEMO-E' + @s, N'Empleado demo ' + @s, @tienda);
        SET @empleado = SCOPE_IDENTITY();

        INSERT dbo.Usuario (id_empleado, nombre_usuario, contrasena_hash)
        VALUES (@empleado, N'demo_usuario_' + @s, N'NO_ES_UN_HASH_VALIDO_DEMO_' + @s);
        SET @usuario = SCOPE_IDENTITY();

        INSERT dbo.Rol (codigo, nombre)
        VALUES ('DEMO-R' + @s, N'Rol demo ' + @s);
        SET @rol = SCOPE_IDENTITY();

        INSERT dbo.Permiso (modulo, accion, descripcion)
        VALUES ('DEMO', 'CONSULTAR' + @s, N'Permiso de ejemplo');
        SET @permiso = SCOPE_IDENTITY();

        INSERT dbo.UsuarioRol (id_usuario, id_rol, asignado_por)
        VALUES (@usuario, @rol, @usuario);
        INSERT dbo.RolPermiso (id_rol, id_permiso, asignado_por)
        VALUES (@rol, @permiso, @usuario);
        INSERT dbo.UsuarioTienda (id_usuario, id_tienda, asignado_por)
        VALUES (@usuario, @tienda, @usuario);

        INSERT dbo.SesionUsuario (id_usuario, token_hash, version_seguridad, creada_en, expira_en)
        SELECT @usuario, HASHBYTES('SHA2_256', 'DEMO-SESION-' + @s),
               version_seguridad, @ahora, DATEADD(hour, 8, @ahora)
        FROM dbo.Usuario WHERE id_usuario = @usuario;

        INSERT dbo.EventoAcceso (id_usuario, nombre_intentado, resultado, equipo)
        VALUES (@usuario, N'demo_usuario_' + @s, 'EXITOSO', N'Equipo demo');

        INSERT dbo.RecuperacionAcceso (id_usuario, token_hash, creado_en, expira_en)
        VALUES (@usuario, HASHBYTES('SHA2_256', 'DEMO-RECUPERACION-' + @s),
                @ahora, DATEADD(hour, 1, @ahora));


        INSERT dbo.Producto (codigo_barras, nombre, precio_unitario, id_categoria, id_proveedor)
        VALUES (N'DEMO-PROD-' + @s, N'Producto demo ' + @s, 100, @categoria, @proveedor);
        SET @producto = SCOPE_IDENTITY();

        INSERT dbo.InventarioTienda (id_tienda, id_producto, existencia, reservado, stock_minimo)
        VALUES (@tienda, @producto, 20, 0, 5);
        INSERT dbo.MovimientoInventario (id_tienda, id_producto, tipo, cantidad_delta, id_usuario, motivo)
        VALUES (@tienda, @producto, 'AJUSTE', 20, @usuario, N'Existencia inicial demo');


        INSERT dbo.Compra (id_proveedor, id_tienda, numero_factura, fecha_factura, id_usuario)
        VALUES (@proveedor, @tienda, N'DEMO-FAC-' + @s, CONVERT(date, @ahora), @usuario);
        SET @compra = SCOPE_IDENTITY();
        INSERT dbo.DetalleCompra (id_compra, id_producto, cantidad, costo_unitario)
        VALUES (@compra, @producto, 10, 60);

        INSERT dbo.Venta (numero, id_tienda, id_cliente, id_usuario)
        VALUES ('DEMO-V-' + @s, @tienda, @cliente, @usuario);
        SET @venta = SCOPE_IDENTITY();
        INSERT dbo.DetalleVenta (id_venta, id_producto, cantidad, precio_unitario, nombre_producto)
        VALUES (@venta, @producto, 1, 100, N'Producto demo ' + @s);
        SET @detalleVenta = SCOPE_IDENTITY();
        INSERT dbo.PagoVenta (id_venta, id_metodo_pago, monto, monto_recibido)
        VALUES (@venta, @metodo, 115, 115);

        INSERT dbo.Traspaso (id_tienda_origen, id_tienda_destino, solicitado_por)
        VALUES (@tienda, @destino, @usuario);
        SET @traspaso = SCOPE_IDENTITY();
        INSERT dbo.DetalleTraspaso (id_traspaso, id_producto, cantidad, precio_referencia)
        VALUES (@traspaso, @producto, 1, 100);
        INSERT dbo.EventoTraspaso (id_traspaso, estado_anterior, estado_nuevo, id_usuario)
        VALUES (@traspaso, NULL, 'PENDIENTE', @usuario);


        INSERT dbo.Apartado (id_cliente, id_tienda, id_usuario, fecha, vence_en, estado)
        VALUES (@cliente, @tienda, @usuario, @ahora, DATEADD(day, 15, @ahora), 'CANCELADO');
        SET @apartado = SCOPE_IDENTITY();
        INSERT dbo.DetalleApartado (id_apartado, id_producto, cantidad, precio_unitario)
        VALUES (@apartado, @producto, 1, 100);
        INSERT dbo.AbonoApartado (id_apartado, id_metodo_pago, monto, id_usuario)
        VALUES (@apartado, @metodo, 25, @usuario);
        SET @abono = SCOPE_IDENTITY();
        INSERT dbo.Reembolso (id_abono, monto, id_metodo_pago, id_usuario, motivo)
        VALUES (@abono, 25, @metodo, @usuario, N'Cancelacion del apartado demo');

        INSERT dbo.Devolucion (id_venta, id_tienda, id_usuario, motivo)
        VALUES (@venta, @tienda, @usuario, N'Devolucion demo pendiente de confirmar la venta');
        SET @devolucion = SCOPE_IDENTITY();
        INSERT dbo.DetalleDevolucion (id_devolucion, id_detalle_venta, cantidad, reintegra_stock)
        VALUES (@devolucion, @detalleVenta, 1, 0);


        INSERT dbo.Inversion (institucion, tipo, monto, tasa_interes, fecha_inicio, fecha_vencimiento, estado)
        VALUES (N'Banco demo ' + @s, 'PLAZO_FIJO', 1000, 5,
                CONVERT(date, @ahora), CONVERT(date, DATEADD(year, 1, @ahora)), 'VIGENTE');

        INSERT dbo.Importacion (nombre_archivo, archivo_hash, id_usuario, estado)
        VALUES (N'demo_' + @s + N'.csv', HASHBYTES('SHA2_256', 'DEMO-ARCHIVO-' + @s), @usuario, 'LISTA');
        SET @importacion = SCOPE_IDENTITY();
        INSERT dbo.DetalleImportacion (id_importacion, numero_fila, datos_originales_json, estado, id_producto)
        VALUES (@importacion, 1, N'{"origen":"demo"}', 'VALIDA', @producto);

        INSERT dbo.Auditoria (id_usuario, modulo, accion, entidad, id_registro, descripcion)
        VALUES (@usuario, 'DEMO', 'CARGA', 'Producto', CONVERT(nvarchar(100), @producto), N'Carga de ejemplo');

        INSERT dbo.EventoNotificacion (clave_evento, tipo, titulo, mensaje)
        VALUES ('DEMO-TRASPASO-' + @s, 'TRASPASO_PENDIENTE', N'Traspaso demo pendiente', N'Revisar traspaso demo ' + @s);
        SET @evento = SCOPE_IDENTITY();
        INSERT dbo.Notificacion (id_evento_notificacion, id_usuario)
        VALUES (@evento, @usuario);

        SET @i += 1;
    END;

    COMMIT TRANSACTION;
    PRINT 'Carga completada: se agregaron 76 filas (2 por cada una de las 38 tablas).';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
```

## A.5 Datos/Migraciones/001_desactivar_registros.sql

Compatibilidad con un esquema anterior: añade activo como bit si falta en Producto, Categoria y Proveedor. No transforma por sí sola una base antigua al modelo 3FN actual.

Extensión: 11 líneas. SHA-256 del texto normalizado a LF: 97813cb7032d1746fd53aa03c241d7ff5dc308be804d1a4ee34210573e74173c.

```sql

SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF COL_LENGTH('dbo.Producto', 'activo') IS NULL
    ALTER TABLE dbo.Producto ADD activo bit NOT NULL CONSTRAINT DF_Producto_activo DEFAULT (1) WITH VALUES;
IF COL_LENGTH('dbo.Categoria', 'activo') IS NULL
    ALTER TABLE dbo.Categoria ADD activo bit NOT NULL CONSTRAINT DF_Categoria_activo DEFAULT (1) WITH VALUES;
IF COL_LENGTH('dbo.Proveedor', 'activo') IS NULL
    ALTER TABLE dbo.Proveedor ADD activo bit NOT NULL CONSTRAINT DF_Proveedor_activo DEFAULT (1) WITH VALUES;
COMMIT TRANSACTION;
```

## A.6 Datos/Migraciones/002_bitacora_sistema.sql

Complementa los triggers existentes con bitácoras por tabla generadas mediante metadatos, sin copiar contraseñas ni tokens.

Extensión: 57 líneas. SHA-256 del texto normalizado a LF: a2925af9fa2d8ac066ee2db7d55e0194f2ff4068c3aefb1c2058dd4620aa40a1.

```sql
-- Complementa Triggers2.sql. No guarda contraseñas, tokens ni datos sensibles.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
DECLARE @tabla sysname,@obj int,@sql nvarchar(max),@clave nvarchar(max),@actor nvarchar(300),@descripcion nvarchar(max),@modulo varchar(50);
DECLARE tablas CURSOR LOCAL FAST_FORWARD FOR
 SELECT t.name,t.object_id FROM sys.tables t JOIN sys.schemas s ON s.schema_id=t.schema_id
 WHERE s.name='dbo' AND t.name<>'Auditoria'
 AND NOT EXISTS(SELECT 1 FROM sys.triggers tr WHERE tr.parent_id=t.object_id AND tr.name IN
 ('trg_Categoria_auditoria','trg_Compra_auditoria','trg_Venta_auditoria','trg_Devolucion_auditoria','trg_Traspaso_auditoria','trg_Apartado_auditoria'));
OPEN tablas; FETCH NEXT FROM tablas INTO @tabla,@obj;
WHILE @@FETCH_STATUS=0
BEGIN
 SELECT @clave=STRING_AGG(CONVERT(nvarchar(max),'r.'+QUOTENAME(c.name)+' AS '+QUOTENAME(c.name)),',') WITHIN GROUP(ORDER BY ic.key_ordinal)
 FROM sys.indexes ix JOIN sys.index_columns ic ON ic.object_id=ix.object_id AND ic.index_id=ix.index_id
 JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id
 WHERE ix.object_id=@obj AND ix.is_primary_key=1;
 IF @clave IS NOT NULL
 BEGIN
 SET @modulo=CASE
 WHEN @tabla IN ('Producto','Categoria') THEN 'PRODUCTOS'
 WHEN @tabla='Proveedor' THEN 'PROVEEDORES'
 WHEN @tabla IN ('Cliente','Empleado') THEN 'DIRECTORIO'
 WHEN @tabla IN ('Usuario','Rol','Permiso','UsuarioRol','RolPermiso','UsuarioTienda','SesionUsuario','EventoAcceso','RecuperacionAcceso') THEN 'SEGURIDAD'
 WHEN @tabla IN ('InventarioTienda','MovimientoInventario') THEN 'INVENTARIO'
 WHEN @tabla IN ('Compra','DetalleCompra') THEN 'COMPRAS'
 WHEN @tabla IN ('Venta','DetalleVenta','PagoVenta') THEN 'VENTAS'
 WHEN @tabla IN ('Traspaso','DetalleTraspaso','EventoTraspaso') THEN 'TRASPASOS'
 WHEN @tabla IN ('Apartado','DetalleApartado','AbonoApartado') THEN 'APARTADOS'
 WHEN @tabla IN ('Devolucion','DetalleDevolucion','Reembolso') THEN 'DEVOLUCIONES'
 WHEN @tabla='Inversion' THEN 'INVERSIONES'
 WHEN @tabla IN ('Importacion','DetalleImportacion') THEN 'IMPORTACIONES'
 WHEN @tabla IN ('Notificacion','EventoNotificacion') THEN 'NOTIFICACIONES'
 WHEN @tabla IN ('Tienda','MetodoPago','ConfiguracionFacturacion') THEN 'CONFIGURACION'
 ELSE 'SISTEMA' END;
 -- En seguridad el usuario afectado puede ser distinto del administrador responsable.
 SET @actor=CASE WHEN @tabla IN ('SesionUsuario','EventoAcceso')
 THEN N'COALESCE(TRY_CONVERT(int,SESSION_CONTEXT(N''id_usuario'')),r.id_usuario)' ELSE N'TRY_CONVERT(int,SESSION_CONTEXT(N''id_usuario''))' END;
 SET @descripcion=CASE @tabla
 WHEN 'EventoAcceso' THEN N'CONCAT(N''Acceso '',r.resultado,N'': '',r.nombre_intentado)'
 WHEN 'SesionUsuario' THEN N'CASE WHEN r.revocada_en IS NULL THEN N''Sesión iniciada / actualizada'' ELSE N''Sesión cerrada o revocada'' END'
 ELSE N'CONCAT(@accion,N'' en '+@tabla+N''',CASE WHEN SESSION_CONTEXT(N''motivo'') IS NOT NULL THEN CONCAT(N'': '',CONVERT(nvarchar(500),SESSION_CONTEXT(N''motivo''))) ELSE N'''' END)' END;
 SET @sql=N'CREATE OR ALTER TRIGGER dbo.'+QUOTENAME('trg_Bitacora_'+@tabla)+N' ON dbo.'+QUOTENAME(@tabla)+N'
 AFTER INSERT,UPDATE,DELETE AS BEGIN
 SET NOCOUNT ON;
 DECLARE @accion varchar(50)=CASE WHEN EXISTS(SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM deleted) THEN ''MODIFICAR'' WHEN EXISTS(SELECT 1 FROM inserted) THEN ''CREAR'' ELSE ''ELIMINAR'' END;
 INSERT dbo.Auditoria(id_usuario,modulo,accion,entidad,id_registro,descripcion,equipo)
 SELECT '+@actor+N','''+@modulo+N''',@accion,N'''+@tabla+N''',LEFT((SELECT '+@clave+N' FOR JSON PATH,WITHOUT_ARRAY_WRAPPER),100),
 '+@descripcion+N',HOST_NAME()
 FROM (SELECT * FROM inserted UNION ALL SELECT * FROM deleted WHERE NOT EXISTS(SELECT 1 FROM inserted)) r;
 END;';
 EXEC sys.sp_executesql @sql;
 END;
 FETCH NEXT FROM tablas INTO @tabla,@obj;
END;
CLOSE tablas; DEALLOCATE tablas;
COMMIT;
```

## A.7 Datos/Migraciones/003_rtn_ventas.sql

Añade el RTN opcional a la venta y valida nulo o 14 dígitos; el esquema nuevo ya incluye el campo.

Extensión: 12 líneas. SHA-256 del texto normalizado a LF: 993812792e4c1037c96a7fff854afc62fb0cddde7b0acfc5f0cec40a571420b9.

```sql
-- RTN opcional del receptor, conservado por factura.
SET XACT_ABORT ON;
IF OBJECT_ID(N'dbo.Venta',N'U') IS NULL
    THROW 50001,'Ejecute primero el esquema de la base de datos.',1;
BEGIN TRANSACTION;
IF COL_LENGTH('dbo.Venta','rtn_cliente') IS NULL
    EXEC(N'ALTER TABLE dbo.Venta ADD rtn_cliente varchar(30) NULL;');
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.Venta') AND name=N'CK_Venta_rtn_cliente')
    EXEC(N'ALTER TABLE dbo.Venta WITH CHECK ADD CONSTRAINT CK_Venta_rtn_cliente
        CHECK(rtn_cliente IS NULL OR (DATALENGTH(rtn_cliente)=14 AND rtn_cliente COLLATE Latin1_General_100_BIN2 NOT LIKE ''%[^0-9]%''));');
COMMIT;
```

## A.8 Datos/Migraciones/004_isv_cai.sql

Actualiza impuestos, totales y CAI por tienda, conserva impuesto cero en líneas antiguas y redefine vistas y el trigger de contabilización de venta.

Extensión: 143 líneas. SHA-256 del texto normalizado a LF: 4f8298e5c9c035f0a9a396356e8dff87df1588c5757d9f9a3f5977ee506a4e07.

```sql
-- ISV adicional a precios base y CAI por tienda. Las líneas anteriores conservan impuesto cero.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF COL_LENGTH('dbo.Venta','cai') IS NULL EXEC(N'ALTER TABLE dbo.Venta ADD cai varchar(37) NULL;');
DECLARE @tabla sysname,@sql nvarchar(max);
DECLARE tasas CURSOR LOCAL FAST_FORWARD FOR SELECT tabla FROM (VALUES(N'DetalleVenta'),(N'DetalleApartado')) x(tabla);
OPEN tasas; FETCH NEXT FROM tasas INTO @tabla;
WHILE @@FETCH_STATUS=0
BEGIN
 IF COL_LENGTH('dbo.'+@tabla,'tasa_isv') IS NULL
 BEGIN
  SET @sql=N'ALTER TABLE dbo.'+QUOTENAME(@tabla)+N' ADD tasa_isv decimal(5,2) NOT NULL CONSTRAINT DF_'+@tabla+N'_tasa_isv DEFAULT 0 WITH VALUES;
   ALTER TABLE dbo.'+QUOTENAME(@tabla)+N' DROP CONSTRAINT DF_'+@tabla+N'_tasa_isv;
   ALTER TABLE dbo.'+QUOTENAME(@tabla)+N' ADD CONSTRAINT DF_'+@tabla+N'_tasa_isv DEFAULT 15 FOR tasa_isv;';
  EXEC sys.sp_executesql @sql;
 END;
 IF COL_LENGTH('dbo.'+@tabla,'isv') IS NULL
 BEGIN
  SET @sql=N'ALTER TABLE dbo.'+QUOTENAME(@tabla)+N' ADD isv AS (CONVERT(decimal(18,2),ROUND(cantidad*precio_unitario*tasa_isv/100,2))),
   total_linea AS (CONVERT(decimal(18,2),cantidad*precio_unitario+ROUND(cantidad*precio_unitario*tasa_isv/100,2)));';
  EXEC sys.sp_executesql @sql;
 END;
 IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.'+@tabla) AND name=N'CK_'+@tabla+N'_tasa_isv')
 BEGIN
  SET @sql=N'ALTER TABLE dbo.'+QUOTENAME(@tabla)+N' WITH CHECK ADD CONSTRAINT CK_'+@tabla+N'_tasa_isv CHECK(tasa_isv IN(0,15,18));';
  EXEC sys.sp_executesql @sql;
 END;
 FETCH NEXT FROM tasas INTO @tabla;
END;
CLOSE tasas; DEALLOCATE tasas;
IF COL_LENGTH('dbo.DetalleDevolucion','isv') IS NULL
 EXEC(N'ALTER TABLE dbo.DetalleDevolucion ADD isv decimal(18,2) NOT NULL CONSTRAINT DF_DetalleDevolucion_isv DEFAULT 0 WITH VALUES;');
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.DetalleDevolucion') AND name=N'CK_DetalleDevolucion_isv')
 EXEC(N'ALTER TABLE dbo.DetalleDevolucion WITH CHECK ADD CONSTRAINT CK_DetalleDevolucion_isv CHECK(isv>=0);');
IF OBJECT_ID(N'dbo.ConfiguracionFacturacion',N'U') IS NULL
 CREATE TABLE dbo.ConfiguracionFacturacion(
  id_tienda int NOT NULL CONSTRAINT PK_ConfiguracionFacturacion PRIMARY KEY,
  cai varchar(37) NULL,actualizado_en datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),actualizado_por int NOT NULL,version_fila rowversion NOT NULL,
  CONSTRAINT FK_ConfiguracionFacturacion_Tienda FOREIGN KEY(id_tienda) REFERENCES dbo.Tienda(id_tienda),
  CONSTRAINT FK_ConfiguracionFacturacion_Usuario FOREIGN KEY(actualizado_por) REFERENCES dbo.Usuario(id_usuario),
  CONSTRAINT CK_ConfiguracionFacturacion_cai CHECK(cai IS NULL OR DATALENGTH(cai)=37));
COMMIT;
GO
CREATE OR ALTER VIEW dbo.vw_VentaTotal AS
SELECT v.id_venta,
       CONVERT(decimal(18,2), ISNULL(SUM(d.subtotal),0)) AS subtotal,
       CONVERT(decimal(18,2), ISNULL(SUM(d.isv),0)) AS isv,
       CONVERT(decimal(18,2), ISNULL(SUM(d.total_linea),0)) AS total
FROM dbo.Venta v
LEFT JOIN dbo.DetalleVenta d ON d.id_venta = v.id_venta
GROUP BY v.id_venta;
GO
CREATE OR ALTER VIEW dbo.vw_ApartadoTotal AS
SELECT a.id_apartado,
       CONVERT(decimal(18,2), ISNULL(SUM(d.subtotal),0)) AS subtotal,
       CONVERT(decimal(18,2), ISNULL(SUM(d.isv),0)) AS isv,
       CONVERT(decimal(18,2), ISNULL(SUM(d.total_linea),0)) AS total
FROM dbo.Apartado a
LEFT JOIN dbo.DetalleApartado d ON d.id_apartado = a.id_apartado
GROUP BY a.id_apartado;
GO
CREATE OR ALTER VIEW dbo.vw_DevolucionDetalle AS
SELECT dd.id_detalle_devolucion, dd.id_devolucion, dd.id_detalle_venta,
       dd.cantidad, dv.precio_unitario,
       CONVERT(decimal(18,2), dd.cantidad * dv.precio_unitario) AS subtotal,
       dd.isv,CONVERT(decimal(18,2),dd.cantidad*dv.precio_unitario+dd.isv) AS total_linea,
       dd.reintegra_stock
FROM dbo.DetalleDevolucion dd
JOIN dbo.DetalleVenta dv ON dv.id_detalle_venta = dd.id_detalle_venta;
GO
CREATE OR ALTER VIEW dbo.vw_DevolucionTotal AS
SELECT d.id_devolucion,
       CONVERT(decimal(18,2), ISNULL(SUM(x.subtotal),0)) AS subtotal,
       CONVERT(decimal(18,2), ISNULL(SUM(x.isv),0)) AS isv,
       CONVERT(decimal(18,2), ISNULL(SUM(x.total_linea),0)) AS total
FROM dbo.Devolucion d
LEFT JOIN dbo.vw_DevolucionDetalle x ON x.id_devolucion = d.id_devolucion
GROUP BY d.id_devolucion;
GO
CREATE OR ALTER TRIGGER dbo.trg_Venta_contabilizar ON dbo.Venta AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF NOT UPDATE(estado) RETURN;
 DECLARE @actor int=TRY_CONVERT(int,SESSION_CONTEXT(N'id_usuario'));
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta WHERE i.estado<>d.estado)
 AND NOT EXISTS(SELECT 1 FROM dbo.Usuario WHERE id_usuario=@actor AND activo=1)
 THROW 50714,'Establezca SESSION_CONTEXT id_usuario con un usuario activo.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta
 JOIN dbo.Devolucion dev WITH(UPDLOCK,HOLDLOCK) ON dev.id_venta=i.id_venta
 WHERE d.estado='CONFIRMADA' AND i.estado='ANULADA' AND dev.estado<>'ANULADA')
 THROW 50715,'Anule las devoluciones antes de anular la venta.',1;

 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta
 JOIN dbo.Apartado a WITH(UPDLOCK,HOLDLOCK) ON a.id_venta=i.id_venta
 WHERE d.estado='BORRADOR' AND i.estado='CONFIRMADA' AND
 (a.estado<>'ACTIVO' OR a.id_tienda<>i.id_tienda OR a.id_cliente<>i.id_cliente
 OR EXISTS(SELECT id_producto,precio_unitario,tasa_isv,SUM(CONVERT(bigint,cantidad)) FROM dbo.DetalleApartado
 WHERE id_apartado=a.id_apartado GROUP BY id_producto,precio_unitario,tasa_isv
 EXCEPT SELECT id_producto,precio_unitario,tasa_isv,SUM(CONVERT(bigint,cantidad)) FROM dbo.DetalleVenta
 WHERE id_venta=i.id_venta GROUP BY id_producto,precio_unitario,tasa_isv)
 OR EXISTS(SELECT id_producto,precio_unitario,tasa_isv,SUM(CONVERT(bigint,cantidad)) FROM dbo.DetalleVenta
 WHERE id_venta=i.id_venta GROUP BY id_producto,precio_unitario,tasa_isv
 EXCEPT SELECT id_producto,precio_unitario,tasa_isv,SUM(CONVERT(bigint,cantidad)) FROM dbo.DetalleApartado
 WHERE id_apartado=a.id_apartado GROUP BY id_producto,precio_unitario,tasa_isv)))
 THROW 50716,'Los datos de venta no coinciden con el apartado enlazado.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta
 JOIN dbo.Apartado a ON a.id_venta=i.id_venta JOIN dbo.AbonoApartado ab ON ab.id_apartado=a.id_apartado
 LEFT JOIN dbo.PagoVenta p ON p.id_pago_venta=ab.id_pago_venta
 WHERE d.estado='BORRADOR' AND i.estado='CONFIRMADA'
 AND(p.id_pago_venta IS NULL OR p.id_venta<>i.id_venta OR p.monto<>ab.monto OR p.id_metodo_pago<>ab.id_metodo_pago))
 THROW 50717,'Aplique cada abono a un pago de la venta antes de confirmar.',1;

 UPDATE a SET estado='ENTREGADO' FROM dbo.Apartado a
 JOIN inserted i ON i.id_venta=a.id_venta JOIN deleted d ON d.id_venta=i.id_venta
 WHERE d.estado='BORRADOR' AND i.estado='CONFIRMADA' AND a.estado='ACTIVO';
 INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,id_detalle_venta)
 SELECT i.id_tienda,dv.id_producto,'VENTA',-dv.cantidad,@actor,dv.id_detalle_venta
 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta JOIN dbo.DetalleVenta dv ON dv.id_venta=i.id_venta
 WHERE d.estado='BORRADOR' AND i.estado='CONFIRMADA';
 INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,id_movimiento_revertido,motivo)
 SELECT m.id_tienda,m.id_producto,'REVERSO',-m.cantidad_delta,@actor,m.id_movimiento,i.motivo_anulacion
 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta
 JOIN dbo.DetalleVenta dv ON dv.id_venta=i.id_venta
 JOIN dbo.MovimientoInventario m ON m.id_detalle_venta=dv.id_detalle_venta AND m.tipo='VENTA'
 WHERE d.estado='CONFIRMADA' AND i.estado='ANULADA';
END;
GO



CREATE OR ALTER TRIGGER dbo.trg_Bitacora_ConfiguracionFacturacion ON dbo.ConfiguracionFacturacion AFTER INSERT,UPDATE,DELETE AS
BEGIN
 SET NOCOUNT ON;
 INSERT dbo.Auditoria(id_usuario,modulo,accion,entidad,id_registro,descripcion,valores_antes_json,valores_despues_json,equipo)
 SELECT TRY_CONVERT(int,SESSION_CONTEXT(N'id_usuario')),'CONFIGURACION',
 CASE WHEN d.id_tienda IS NULL THEN 'CREAR' WHEN i.id_tienda IS NULL THEN 'ELIMINAR' ELSE 'MODIFICAR' END,
 'ConfiguracionFacturacion',CONVERT(nvarchar(100),COALESCE(i.id_tienda,d.id_tienda)),N'Configuración del CAI por tienda',
 CASE WHEN d.id_tienda IS NOT NULL THEN (SELECT d.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER) END,
 CASE WHEN i.id_tienda IS NOT NULL THEN (SELECT i.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER) END,HOST_NAME()
 FROM inserted i FULL JOIN deleted d ON d.id_tienda=i.id_tienda;
END;
GO
```

## A.9 Datos/Pruebas/Bitacora-sistema.sql

Prueba transaccional de auditoría en base de pruebas: prepara registros y comprueba eventos y exclusión de datos sensibles; revisar sus prerrequisitos y rollback antes de ejecutarla.

Extensión: 112 líneas. SHA-256 del texto normalizado a LF: acdcbf3cf2c9a360eb2e76a54feed8b36b7be8751f750ffee86235a459c522ec.

```sql
-- Ejecutar después de Triggers2.sql y Migraciones/002_bitacora_sistema.sql.
-- Todas las operaciones usan registros temporales y se revierten al terminar.
SET NOCOUNT ON;
SET XACT_ABORT ON;
DECLARE @contexto sql_variant=SESSION_CONTEXT(N'id_usuario'),@motivo sql_variant=SESSION_CONTEXT(N'motivo');
BEGIN TRY
    IF EXISTS(SELECT 1 FROM sys.tables t WHERE t.schema_id=SCHEMA_ID('dbo') AND t.name<>'Auditoria'
        AND NOT EXISTS(SELECT 1 FROM sys.triggers tr WHERE tr.parent_id=t.object_id AND tr.is_disabled=0
            AND (tr.name LIKE 'trg_Bitacora_%' OR tr.name LIKE '%_auditoria')))
        THROW 51000,'Hay tablas sin captura de auditoría habilitada.',1;
    BEGIN TRANSACTION;
    DECLARE @actor int=(SELECT TOP(1) id_usuario FROM dbo.Usuario WHERE activo=1 ORDER BY id_usuario);
    IF @actor IS NULL THROW 51000,'La prueba necesita un usuario activo existente.',1;
    EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@actor;
    EXEC sys.sp_set_session_context @key=N'motivo',@value=N'Prueba integral de bitácora (rollback)';
    DECLARE @inicio bigint=ISNULL((SELECT MAX(id_auditoria) FROM dbo.Auditoria),0),
        @tag varchar(20)=CONCAT('AUD-',LEFT(CONVERT(varchar(36),NEWID()),12)),
        @tienda int,@destino int,@categoria int,@proveedor int,@cliente int,@empleado int,@usuario int,
        @rol int,@permiso int,@producto int,@metodo int,@compra int,@venta int,@detalleVenta int,
        @pago int,@traspaso int,@apartado int,@devolucion int,@importacion int,@evento bigint;
    INSERT dbo.Tienda(codigo,nombre) VALUES(@tag,N'Prueba auditoría origen'); SET @tienda=SCOPE_IDENTITY();
    INSERT dbo.ConfiguracionFacturacion(id_tienda,cai,actualizado_por) VALUES(@tienda,'ABC123-DEF456-ABC123-DEF456-ABC123-AA',@actor);
    INSERT dbo.Tienda(codigo,nombre) VALUES(CONCAT(@tag,'-D'),N'Prueba auditoría destino'); SET @destino=SCOPE_IDENTITY();
    INSERT dbo.Categoria(nombre) VALUES(@tag); SET @categoria=SCOPE_IDENTITY();
    INSERT dbo.Proveedor(nombre_empresa) VALUES(@tag); SET @proveedor=SCOPE_IDENTITY();
    INSERT dbo.Cliente(nombre) VALUES(@tag); SET @cliente=SCOPE_IDENTITY();
    INSERT dbo.Empleado(codigo,nombre,id_tienda) VALUES(@tag,@tag,@tienda); SET @empleado=SCOPE_IDENTITY();
    INSERT dbo.Usuario(id_empleado,nombre_usuario,contrasena_hash) VALUES(@empleado,@tag,N'SENTINELA-NO-AUTENTICABLE-PRUEBA'); SET @usuario=SCOPE_IDENTITY();
    INSERT dbo.Rol(codigo,nombre) VALUES(@tag,@tag); SET @rol=SCOPE_IDENTITY();
    INSERT dbo.Permiso(modulo,accion,descripcion) VALUES(@tag,'PRUEBA',N'Prueba'); SET @permiso=SCOPE_IDENTITY();
    INSERT dbo.UsuarioRol(id_usuario,id_rol,asignado_por) VALUES(@usuario,@rol,@actor);
    INSERT dbo.RolPermiso(id_rol,id_permiso,asignado_por) VALUES(@rol,@permiso,@actor);
    INSERT dbo.UsuarioTienda(id_usuario,id_tienda,asignado_por) VALUES(@usuario,@tienda,@actor);
    INSERT dbo.SesionUsuario(id_usuario,token_hash,version_seguridad,expira_en)
        SELECT @usuario,CRYPT_GEN_RANDOM(32),version_seguridad,DATEADD(hour,1,SYSUTCDATETIME()) FROM dbo.Usuario WHERE id_usuario=@usuario;
    INSERT dbo.EventoAcceso(id_usuario,nombre_intentado,resultado)
        SELECT @usuario,@tag,resultado FROM (VALUES('EXITOSO'),('INVALIDO'),('BLOQUEADO'),('INACTIVO')) r(resultado);
    INSERT dbo.RecuperacionAcceso(id_usuario,token_hash,expira_en) VALUES(@usuario,CRYPT_GEN_RANDOM(32),DATEADD(hour,1,SYSUTCDATETIME()));
    INSERT dbo.MetodoPago(codigo,nombre) VALUES(@tag,N'Pago prueba'); SET @metodo=SCOPE_IDENTITY();
    INSERT dbo.Producto(codigo_barras,nombre,precio_unitario,id_categoria,id_proveedor)
        VALUES(@tag,@tag,10,@categoria,@proveedor); SET @producto=SCOPE_IDENTITY();
    INSERT dbo.Compra(id_proveedor,id_tienda,numero_factura,fecha_factura,id_usuario)
        VALUES(@proveedor,@tienda,@tag,CONVERT(date,SYSUTCDATETIME()),@actor); SET @compra=SCOPE_IDENTITY();
    INSERT dbo.DetalleCompra(id_compra,id_producto,cantidad,costo_unitario) VALUES(@compra,@producto,20,5);
    UPDATE dbo.Compra SET estado='CONFIRMADA' WHERE id_compra=@compra;
    UPDATE dbo.InventarioTienda SET stock_minimo=3 WHERE id_tienda=@tienda AND id_producto=@producto;
    INSERT dbo.Venta(numero,id_tienda,id_cliente,id_usuario) VALUES(@tag,@tienda,@cliente,@actor); SET @venta=SCOPE_IDENTITY();
    INSERT dbo.DetalleVenta(id_venta,id_producto,cantidad,precio_unitario,nombre_producto,tasa_isv)
        VALUES(@venta,@producto,2,10,@tag,0); SET @detalleVenta=SCOPE_IDENTITY();
    INSERT dbo.PagoVenta(id_venta,id_metodo_pago,monto,monto_recibido) VALUES(@venta,@metodo,20,20); SET @pago=SCOPE_IDENTITY();
    UPDATE dbo.Venta SET estado='CONFIRMADA' WHERE id_venta=@venta;
    INSERT dbo.Traspaso(id_tienda_origen,id_tienda_destino,solicitado_por) VALUES(@tienda,@destino,@actor); SET @traspaso=SCOPE_IDENTITY();
    INSERT dbo.DetalleTraspaso(id_traspaso,id_producto,cantidad,precio_referencia) VALUES(@traspaso,@producto,1,10);
    UPDATE dbo.Traspaso SET estado='APROBADO' WHERE id_traspaso=@traspaso;
    UPDATE dbo.Traspaso SET estado='EN_TRANSITO' WHERE id_traspaso=@traspaso;
    UPDATE dbo.Traspaso SET estado='RECIBIDO' WHERE id_traspaso=@traspaso;
    INSERT dbo.Apartado(id_cliente,id_tienda,id_usuario,vence_en) VALUES(@cliente,@tienda,@actor,DATEADD(day,2,SYSUTCDATETIME())); SET @apartado=SCOPE_IDENTITY();
    INSERT dbo.DetalleApartado(id_apartado,id_producto,cantidad,precio_unitario,tasa_isv) VALUES(@apartado,@producto,1,10,0);
    INSERT dbo.AbonoApartado(id_apartado,id_metodo_pago,monto,id_usuario) VALUES(@apartado,@metodo,5,@actor);
    INSERT dbo.Devolucion(id_venta,id_tienda,id_usuario,motivo) VALUES(@venta,@tienda,@actor,N'Prueba'); SET @devolucion=SCOPE_IDENTITY();
    INSERT dbo.DetalleDevolucion(id_devolucion,id_detalle_venta,cantidad,reintegra_stock) VALUES(@devolucion,@detalleVenta,1,1);
    UPDATE dbo.Devolucion SET estado='CONFIRMADA' WHERE id_devolucion=@devolucion;
    INSERT dbo.Reembolso(id_pago_venta,id_devolucion,monto,id_metodo_pago,id_usuario,motivo) VALUES(@pago,@devolucion,10,@metodo,@actor,N'Prueba');
    INSERT dbo.Inversion(institucion,tipo,monto,tasa_interes,fecha_inicio,fecha_vencimiento,estado)
        VALUES(@tag,'AHORRO',100,1,CONVERT(date,SYSUTCDATETIME()),CONVERT(date,DATEADD(day,10,SYSUTCDATETIME())),'VIGENTE');
    INSERT dbo.Importacion(nombre_archivo,archivo_hash,id_usuario,estado) VALUES(@tag,CRYPT_GEN_RANDOM(32),@actor,'VALIDANDO'); SET @importacion=SCOPE_IDENTITY();
    INSERT dbo.DetalleImportacion(id_importacion,numero_fila,datos_originales_json,estado) VALUES(@importacion,1,N'{}','VALIDA');
    INSERT dbo.EventoNotificacion(clave_evento,tipo,titulo,mensaje) VALUES(@tag,'STOCK_BAJO',N'Prueba',N'Prueba'); SET @evento=SCOPE_IDENTITY();
    INSERT dbo.Notificacion(id_evento_notificacion,id_usuario) VALUES(@evento,@usuario);
    UPDATE dbo.Notificacion SET leida_en=SYSUTCDATETIME() WHERE id_evento_notificacion=@evento AND id_usuario=@usuario;

    -- El administrador responsable debe conservarse aunque el registro afecte a otro usuario.
    UPDATE dbo.Usuario SET desactivado_en=SYSUTCDATETIME(),motivo_desactivacion=N'Prueba' WHERE id_usuario=@usuario;
    IF NOT EXISTS(SELECT 1 FROM dbo.Auditoria WHERE id_auditoria>@inicio AND entidad='Usuario' AND accion='MODIFICAR'
        AND id_registro=CONCAT('{"id_usuario":',@usuario,'}') AND id_usuario=@actor)
        THROW 51000,'La modificación de usuario perdió el administrador responsable.',1;
    IF NOT EXISTS(SELECT 1 FROM dbo.Auditoria WHERE id_auditoria>@inicio AND entidad='SesionUsuario'
        AND descripcion=N'Sesión cerrada o revocada' AND id_usuario=@actor)
        THROW 51000,'La revocación de sesión no se auditó.',1;
    -- Alta, modificación y eliminación de una entidad sin referencias.
    INSERT dbo.Categoria(nombre) VALUES(CONCAT(@tag,'-BORRAR'));
    DELETE dbo.Categoria WHERE nombre=CONCAT(@tag,'-BORRAR');
    UPDATE dbo.Proveedor SET telefono=N'0000' WHERE id_proveedor=@proveedor;
    INSERT dbo.Cliente(nombre) VALUES(CONCAT(@tag,'-BORRAR'));
    DELETE dbo.Cliente WHERE nombre=CONCAT(@tag,'-BORRAR');
    IF NOT EXISTS(SELECT 1 FROM dbo.Auditoria WHERE id_auditoria>@inicio AND entidad='Cliente' AND accion='ELIMINAR')
        OR NOT EXISTS(SELECT 1 FROM dbo.Auditoria WHERE id_auditoria>@inicio AND entidad='Proveedor' AND accion='MODIFICAR')
        THROW 51000,'Falta auditoría de modificaciones o eliminaciones fuera de ventas.',1;

    -- Cada tabla operativa debe haber generado un evento real, incluidos efectos de triggers anidados.
    IF EXISTS(SELECT 1 FROM sys.tables t WHERE t.schema_id=SCHEMA_ID('dbo') AND t.name<>'Auditoria'
        AND NOT EXISTS(SELECT 1 FROM dbo.Auditoria a WHERE a.id_auditoria>@inicio AND a.entidad=t.name AND a.id_usuario=@actor))
    BEGIN
        SELECT t.name AS EntidadSinEvento FROM sys.tables t WHERE t.schema_id=SCHEMA_ID('dbo') AND t.name<>'Auditoria'
            AND NOT EXISTS(SELECT 1 FROM dbo.Auditoria a WHERE a.id_auditoria>@inicio AND a.entidad=t.name AND a.id_usuario=@actor);
        THROW 51000,'Hay entidades sin un evento de prueba auditado.',1;
    END;
    IF EXISTS(SELECT 1 FROM dbo.Auditoria WHERE id_auditoria>@inicio
        AND (CHARINDEX('SENTINELA-NO-AUTENTICABLE-PRUEBA',ISNULL(valores_antes_json,N'')+ISNULL(valores_despues_json,N''))>0))
        THROW 51000,'La auditoría expuso credenciales de prueba.',1;
    SELECT entidad AS Entidad,COUNT(*) AS Eventos FROM dbo.Auditoria WHERE id_auditoria>@inicio AND id_usuario=@actor GROUP BY entidad ORDER BY entidad;
    ROLLBACK TRANSACTION;
    EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@contexto;
    EXEC sys.sp_set_session_context @key=N'motivo',@value=@motivo;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
    EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@contexto;
    EXEC sys.sp_set_session_context @key=N'motivo',@value=@motivo;
    THROW;
END CATCH;
```

# Anexo B. Todas las consultas y fragmentos SQL en C#

Cada entrada identifica archivo, línea y método o campo, explica su uso y muestra el SQL. Se incluyen SQL de producción y SQL de pruebas. Se conservan apariciones repetidas para poder localizar cada llamada. Las cadenas concatenadas y las plantillas compartidas se expanden cuando son determinables estáticamente. Las expresiones entre llaves, como {tabla}, son partes variables de C#: deben resolverse con las variantes indicadas y no se pueden pegar directamente en SQL Server. Los parámetros @nombre se enlazan desde C#; los DECLARE internos pueden usar el mismo formato.

Los fragmentos auxiliares (por ejemplo HASHBYTES, WHERE o una lista de SET) forman parte de consultas mayores. Se etiquetan como fragmentos, no como instrucciones independientes. Los bloques dinámicos incluyen la expresión C# original y los selectores auxiliares disponibles, para no presentar una variante supuesta como si fuera SQL literal.

## B.0 Índice de consultas por archivo

| Archivo | Entradas | Identificadores |
|---|---|---|
| Clases/Borrar.cs | 1 | Q001–Q001 |
| Clases/Ingresar.cs | 2 | Q002–Q003 |
| Clases/Modificar.cs | 4 | Q004–Q007 |
| Clases/SesionActual.cs | 1 | Q008–Q008 |
| Datos/AccesoDatos.cs | 11 | Q009–Q019 |
| Datos/ApartadoDatos.cs | 24 | Q020–Q043 |
| Datos/AuditoriaDatos.cs | 3 | Q044–Q046 |
| Datos/CargaMasivaDatos.cs | 9 | Q047–Q055 |
| Datos/CategoriaDatos.cs | 2 | Q056–Q057 |
| Datos/ClienteDatos.cs | 5 | Q058–Q062 |
| Datos/CompraDatos.cs | 20 | Q063–Q082 |
| Datos/Conexion.cs | 1 | Q083–Q083 |
| Datos/ConfiguracionFacturacionDatos.cs | 3 | Q084–Q086 |
| Datos/DevolucionDatos.cs | 18 | Q087–Q104 |
| Datos/DirectorioDatos.cs | 8 | Q105–Q112 |
| Datos/FlujoVentaDatos.cs | 3 | Q113–Q115 |
| Datos/InicioDatos.cs | 1 | Q116–Q116 |
| Datos/InventarioDatos.cs | 6 | Q117–Q122 |
| Datos/ProductoDatos.cs | 4 | Q123–Q126 |
| Datos/ProveedorDatos.cs | 3 | Q127–Q129 |
| Datos/SeguridadDatos.cs | 10 | Q130–Q139 |
| Datos/TiendaDatos.cs | 4 | Q140–Q143 |
| Datos/TraspasoDatos.cs | 12 | Q144–Q155 |
| Datos/VentaDatos.cs | 7 | Q156–Q162 |
| Formularios/Administracion/ConfiguracionAuditoria.Facturacion.cs | 1 | Q163–Q163 |
| Formularios/Productos/CargaMasiva.cs | 1 | Q164–Q164 |
| Formularios/Ventas/ApartadosDevoluciones.Editores.cs | 3 | Q165–Q167 |
| Formularios/Ventas/PuntoVenta.cs | 2 | Q168–Q169 |
| Pruebas/FlujoVentas.Integracion/Program.cs | 47 | Q170–Q216 |
## B. Clases/Borrar.cs

### Q001 · Borrar.Desactivar · línea 27

Fuente: Clases/Borrar.cs:27. Sentencia o fragmento utilizado en Borrar.Desactivar. Operaciones presentes: UPDATE, SELECT. Su contexto y sus variantes se identifican en la referencia al código.

Tipo: plantilla SQL dinámica. Marcadores SQL detectados: @id, @motivo.

```sql
UPDATE dbo.{tabla}
                            SET desactivado_en = SYSUTCDATETIME(), motivo_desactivacion = @motivo,
                                actualizado_en = SYSUTCDATETIME()
                            WHERE {columnaId} = @id AND activo = 1;
                            SELECT CONVERT(int, @@ROWCOUNT);
```

Construcción original en C#:

```csharp
$@"UPDATE dbo.{tabla}
                            SET desactivado_en = SYSUTCDATETIME(), motivo_desactivacion = @motivo,
                                actualizado_en = SYSUTCDATETIME()
                            WHERE {columnaId} = @id AND activo = 1;
                            SELECT CONVERT(int, @@ROWCOUNT);"
```

## B. Clases/Ingresar.cs

### Q002 · Ingresar.Proveedor · línea 14

Fuente: Clases/Ingresar.cs:14. Sentencia o fragmento utilizado en Ingresar.Proveedor. Operaciones presentes: INSERT, SELECT. Objetos referidos: Proveedor.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @contacto, @correo, @direccion, @insertados, @nombre, @rtn, @telefono.

```sql
DECLARE @insertados TABLE (id_proveedor int);
                                 INSERT INTO dbo.Proveedor
                                     (nombre_empresa, rtn, contacto, telefono, correo, direccion)
                                 OUTPUT INSERTED.id_proveedor INTO @insertados
                                 VALUES (@nombre, @rtn, @contacto, @telefono, @correo, @direccion);
                                 SELECT id_proveedor FROM @insertados;
```

### Q003 · Ingresar.Categoria · línea 34

Fuente: Clases/Ingresar.cs:34. Sentencia o fragmento utilizado en Ingresar.Categoria. Operaciones presentes: INSERT, SELECT. Objetos referidos: Categoria.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @categoriasInsertadas, @descripcion, @nombre.

```sql
DECLARE @categoriasInsertadas TABLE (id_categoria int);
                                 INSERT INTO dbo.Categoria (nombre, descripcion)
                                 OUTPUT INSERTED.id_categoria INTO @categoriasInsertadas
                                 VALUES (@nombre, @descripcion);
                                 SELECT id_categoria FROM @categoriasInsertadas;
```

## B. Clases/Modificar.cs

### Q004 · Modificar.ResolverTraspaso · línea 29

Fuente: Clases/Modificar.cs:29. Sentencia o fragmento utilizado en Modificar.ResolverTraspaso. Operaciones presentes: UPDATE, THROW. Objetos referidos: Traspaso.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @estado, @id, @version.

```sql
UPDATE dbo.Traspaso SET estado=@estado
                WHERE id_traspaso=@id AND version_fila=@version AND
                ((estado='PENDIENTE' AND @estado IN ('APROBADO','RECHAZADO','CANCELADO'))
                 OR (estado='APROBADO' AND @estado IN ('EN_TRANSITO','CANCELADO'))
                 OR (estado='EN_TRANSITO' AND @estado='RECIBIDO'));
                IF @@ROWCOUNT=0 THROW 50003, 'La solicitud cambió o no permite esta transición. Actualice la lista.', 1;
```

### Q005 · Modificar.Reactivar · línea 56

Fuente: Clases/Modificar.cs:56. Sentencia o fragmento utilizado en Modificar.Reactivar. Operaciones presentes: UPDATE, SELECT. Su contexto y sus variantes se identifican en la referencia al código.

Tipo: plantilla SQL dinámica. Marcadores SQL detectados: @id.

```sql
UPDATE dbo.{tabla}
                            SET desactivado_en = NULL, desactivado_por = NULL,
                                motivo_desactivacion = NULL, actualizado_en = SYSUTCDATETIME()
                            WHERE {columnaId} = @id AND activo = 0;
                            SELECT CONVERT(int, @@ROWCOUNT);
```

Construcción original en C#:

```csharp
$@"UPDATE dbo.{tabla}
                            SET desactivado_en = NULL, desactivado_por = NULL,
                                motivo_desactivacion = NULL, actualizado_en = SYSUTCDATETIME()
                            WHERE {columnaId} = @id AND activo = 0;
                            SELECT CONVERT(int, @@ROWCOUNT);"
```

### Q006 · Modificar.Proveedor · línea 73

Fuente: Clases/Modificar.cs:73. Sentencia o fragmento utilizado en Modificar.Proveedor. Operaciones presentes: UPDATE, SELECT. Objetos referidos: Proveedor.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @contacto, @correo, @direccion, @id, @nombre, @rtn, @telefono.

```sql
UPDATE dbo.Proveedor
                                 SET nombre_empresa = @nombre, rtn = @rtn, contacto = @contacto,
                                     telefono = @telefono, correo = @correo, direccion = @direccion,
                                     actualizado_en = SYSUTCDATETIME()
                                 WHERE id_proveedor = @id AND activo = 1;
                                 SELECT CONVERT(int, @@ROWCOUNT);
```

### Q007 · Modificar.Categoria · línea 96

Fuente: Clases/Modificar.cs:96. Sentencia o fragmento utilizado en Modificar.Categoria. Operaciones presentes: UPDATE, SELECT. Objetos referidos: Categoria.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @descripcion, @id, @nombre.

```sql
UPDATE dbo.Categoria SET nombre = @nombre, descripcion = @descripcion,
                                 actualizado_en = SYSUTCDATETIME()
                                 WHERE id_categoria = @id AND activo = 1;
                                 SELECT CONVERT(int, @@ROWCOUNT);
```

## B. Clases/SesionActual.cs

### Q008 · SesionActual.Validar · línea 15

Fuente: Clases/SesionActual.cs:15. Comprueba que se cumplen las condiciones de sesión o de negocio.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @sesion, @usuario.

```sql
SELECT COUNT(*) FROM dbo.SesionUsuario s WITH(HOLDLOCK)
JOIN dbo.Usuario u WITH(HOLDLOCK) ON u.id_usuario=s.id_usuario
JOIN dbo.Empleado e WITH(HOLDLOCK) ON e.id_empleado=u.id_empleado
WHERE s.id_sesion=@sesion AND s.id_usuario=@usuario AND s.revocada_en IS NULL
AND s.expira_en>SYSUTCDATETIME() AND s.version_seguridad=u.version_seguridad
AND u.activo=1 AND e.activo=1 AND u.debe_cambiar_contrasena=0
```

## B. Datos/AccesoDatos.cs

### Q009 · AccesoDatos.Ingresar · línea 26

Fuente: Datos/AccesoDatos.cs:26. Autentica la cuenta, registra el resultado del acceso y prepara su sesión.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @usuario.

```sql
SELECT u.id_usuario,e.nombre,u.contrasena_hash,u.activo,e.activo AS empleado_activo,
u.bloqueado_hasta,u.debe_cambiar_contrasena,u.version_fila,u.intentos_fallidos
FROM dbo.Usuario u WITH (UPDLOCK,HOLDLOCK) JOIN dbo.Empleado e ON e.id_empleado=u.id_empleado
WHERE u.nombre_usuario_normalizado=UPPER(LTRIM(RTRIM(@usuario)))
```

### Q010 · AccesoDatos.Ingresar · línea 49

Fuente: Datos/AccesoDatos.cs:49. Autentica la cuenta, registra el resultado del acceso y prepara su sesión.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT SYSUTCDATETIME()
```

### Q011 · AccesoDatos.Ingresar · línea 55

Fuente: Datos/AccesoDatos.cs:55. Autentica la cuenta, registra el resultado del acceso y prepara su sesión.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
UPDATE dbo.Usuario SET intentos_fallidos=CASE WHEN bloqueado_hasta IS NOT NULL AND bloqueado_hasta<=SYSUTCDATETIME() THEN 1 ELSE intentos_fallidos+1 END,
bloqueado_hasta=CASE WHEN bloqueado_hasta IS NOT NULL AND bloqueado_hasta<=SYSUTCDATETIME() THEN NULL
    WHEN intentos_fallidos>=4 THEN DATEADD(minute,15,SYSUTCDATETIME()) ELSE NULL END
WHERE id_usuario=@id
```

### Q012 · AccesoDatos.Ingresar · línea 64

Fuente: Datos/AccesoDatos.cs:64. Autentica la cuenta, registra el resultado del acceso y prepara su sesión.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @equipo, @id, @resultado, @usuario.

```sql
INSERT dbo.EventoAcceso(id_usuario,nombre_intentado,resultado,equipo) VALUES(@id,@usuario,@resultado,@equipo)
```

### Q013 · AccesoDatos.Ingresar · línea 79

Fuente: Datos/AccesoDatos.cs:79. Autentica la cuenta, registra el resultado del acceso y prepara su sesión.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT r.nombre FROM dbo.UsuarioRol ur JOIN dbo.Rol r ON r.id_rol=ur.id_rol WHERE ur.id_usuario=@id AND ur.activo=1 AND r.activo=1 ORDER BY r.nombre
```

### Q014 · AccesoDatos.Ingresar · línea 89

Fuente: Datos/AccesoDatos.cs:89. Autentica la cuenta, registra el resultado del acceso y prepara su sesión.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @equipo, @id, @token.

```sql
UPDATE dbo.Usuario SET intentos_fallidos=0,bloqueado_hasta=NULL,ultimo_acceso_en=SYSUTCDATETIME() WHERE id_usuario=@id;
INSERT dbo.SesionUsuario(id_usuario,token_hash,version_seguridad,expira_en,equipo)
SELECT id_usuario,@token,version_seguridad,DATEADD(hour,8,SYSUTCDATETIME()),@equipo FROM dbo.Usuario WHERE id_usuario=@id;
SELECT CONVERT(bigint,SCOPE_IDENTITY());
```

### Q015 · AccesoDatos.CrearAdministradorTemporal · línea 106

Fuente: Datos/AccesoDatos.cs:106. Prepara la cuenta inicial únicamente cuando no existen usuarios.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT COUNT(*) FROM dbo.Usuario WITH (UPDLOCK,HOLDLOCK)
```

### Q016 · AccesoDatos.CrearAdministradorTemporal · línea 109

Fuente: Datos/AccesoDatos.cs:109. Prepara la cuenta inicial únicamente cuando no existen usuarios.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @empleado, @hash, @rol, @tienda, @usuario.

```sql
DECLARE @tienda int, @empleado int, @rol int, @usuario int;
SELECT @rol=id_rol FROM dbo.Rol WITH (UPDLOCK,HOLDLOCK) WHERE codigo='ADMINISTRADOR' AND activo=1;
IF @rol IS NULL
BEGIN
    IF EXISTS(SELECT 1 FROM dbo.Rol WHERE codigo='ADMINISTRADOR')
        THROW 50001, 'El rol ADMINISTRADOR esta inactivo. Reactivelo mediante administracion de la base.', 1;
    INSERT dbo.Rol(codigo,nombre,descripcion) VALUES('ADMINISTRADOR',N'Administrador',N'Administracion del sistema');
    SET @rol=CONVERT(int,SCOPE_IDENTITY());
END;
SELECT @empleado=id_empleado FROM dbo.Empleado WITH (UPDLOCK,HOLDLOCK) WHERE codigo='ADMIN-INICIAL' AND activo=1;
IF @empleado IS NULL
BEGIN
    IF EXISTS(SELECT 1 FROM dbo.Empleado WHERE codigo='ADMIN-INICIAL')
        THROW 50002, 'El empleado ADMIN-INICIAL esta inactivo. Revise la configuracion inicial.', 1;
    SELECT TOP(1) @tienda=id_tienda FROM dbo.Tienda WITH (UPDLOCK,HOLDLOCK) WHERE activo=1 ORDER BY id_tienda;
    IF @tienda IS NULL
    BEGIN
        IF EXISTS(SELECT 1 FROM dbo.Tienda WHERE codigo='INICIAL')
            THROW 50003, 'La tienda INICIAL esta inactiva. Revise la configuracion inicial.', 1;
        INSERT dbo.Tienda(codigo,nombre) VALUES('INICIAL',N'Tienda inicial');
        SET @tienda=CONVERT(int,SCOPE_IDENTITY());
    END;
    INSERT dbo.Empleado(codigo,nombre,id_tienda) VALUES('ADMIN-INICIAL',N'Administrador temporal',@tienda);
    SET @empleado=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT dbo.Usuario(id_empleado,nombre_usuario,contrasena_hash,debe_cambiar_contrasena)
    VALUES(@empleado,N'admin',@hash,1);
SET @usuario=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.UsuarioRol(id_usuario,id_rol) VALUES(@usuario,@rol);
```

### Q017 · AccesoDatos.CambiarClave · línea 150

Fuente: Datos/AccesoDatos.cs:150. Sustituye la contraseña temporal con comprobación de la versión de la cuenta.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @hash, @id, @key, @value, @version.

```sql
EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@id;
UPDATE u SET contrasena_hash=@hash,debe_cambiar_contrasena=0,intentos_fallidos=0,bloqueado_hasta=NULL,
contrasena_actualizada_en=SYSUTCDATETIME(),actualizado_en=SYSUTCDATETIME()
FROM dbo.Usuario u JOIN dbo.Empleado e ON e.id_empleado=u.id_empleado
WHERE u.id_usuario=@id AND u.version_fila=@version AND u.activo=1 AND e.activo=1 AND u.debe_cambiar_contrasena=1;
SELECT CONVERT(int,@@ROWCOUNT);
```

### Q018 · AccesoDatos.Vigente · línea 167

Fuente: Datos/AccesoDatos.cs:167. Verifica vigencia, estado o vencimiento antes de permitir la operación.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id, @sesion.

```sql
SELECT COUNT(*) FROM dbo.SesionUsuario s JOIN dbo.Usuario u ON u.id_usuario=s.id_usuario
JOIN dbo.Empleado e ON e.id_empleado=u.id_empleado
WHERE s.id_sesion=@sesion AND s.id_usuario=@id AND s.revocada_en IS NULL AND s.expira_en>SYSUTCDATETIME()
AND s.version_seguridad=u.version_seguridad AND u.activo=1 AND e.activo=1 AND u.debe_cambiar_contrasena=0
```

### Q019 · AccesoDatos.Cerrar · línea 181

Fuente: Datos/AccesoDatos.cs:181. Cierra el documento o la sesión según la entidad y el estado indicados.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id, @key, @sesion, @value.

```sql
EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@id;
UPDATE dbo.SesionUsuario SET revocada_en=SYSUTCDATETIME()
WHERE id_sesion=@sesion AND id_usuario=@id AND revocada_en IS NULL;
```

## B. Datos/ApartadoDatos.cs

### Q020 · ApartadoDatos.Consulta · línea 10

Fuente: Datos/ApartadoDatos.cs:10. Plantilla base reutilizada para cabecera, filtros y recuperación del módulo.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT a.id_apartado AS Id,c.nombre AS Cliente,t.nombre AS Tienda,a.fecha AS FechaUTC,a.vence_en AS VenceUTC,a.estado AS Estado,
    total.total AS Total,ISNULL(ab.Monto,0) AS Abonado,total.total-ISNULL(ab.Monto,0) AS Saldo,
    ISNULL(ab.Monto,0)-ISNULL(re.Monto,0) AS Reembolsable,a.id_venta AS Venta,a.version_fila,a.id_tienda,a.id_cliente
FROM dbo.Apartado a JOIN dbo.Cliente c ON c.id_cliente=a.id_cliente JOIN dbo.Tienda t ON t.id_tienda=a.id_tienda
JOIN dbo.vw_ApartadoTotal total ON total.id_apartado=a.id_apartado
OUTER APPLY(SELECT SUM(monto) AS Monto FROM dbo.AbonoApartado WHERE id_apartado=a.id_apartado) ab
OUTER APPLY(SELECT SUM(r.monto) AS Monto FROM dbo.Reembolso r JOIN dbo.AbonoApartado b ON b.id_abono=r.id_abono WHERE b.id_apartado=a.id_apartado) re
```

### Q021 · ApartadoDatos.Listar · línea 23

Fuente: Datos/ApartadoDatos.cs:23. Obtiene el listado y aplica los filtros del módulo.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @buscar, @estado.

```sql
SELECT a.id_apartado AS Id,c.nombre AS Cliente,t.nombre AS Tienda,a.fecha AS FechaUTC,a.vence_en AS VenceUTC,a.estado AS Estado,
    total.total AS Total,ISNULL(ab.Monto,0) AS Abonado,total.total-ISNULL(ab.Monto,0) AS Saldo,
    ISNULL(ab.Monto,0)-ISNULL(re.Monto,0) AS Reembolsable,a.id_venta AS Venta,a.version_fila,a.id_tienda,a.id_cliente
FROM dbo.Apartado a JOIN dbo.Cliente c ON c.id_cliente=a.id_cliente JOIN dbo.Tienda t ON t.id_tienda=a.id_tienda
JOIN dbo.vw_ApartadoTotal total ON total.id_apartado=a.id_apartado
OUTER APPLY(SELECT SUM(monto) AS Monto FROM dbo.AbonoApartado WHERE id_apartado=a.id_apartado) ab
OUTER APPLY(SELECT SUM(r.monto) AS Monto FROM dbo.Reembolso r JOIN dbo.AbonoApartado b ON b.id_abono=r.id_abono WHERE b.id_apartado=a.id_apartado) re
WHERE (@estado='' OR a.estado=@estado) AND (@buscar='' OR CHARINDEX(@buscar,c.nombre)>0
    OR CHARINDEX(@buscar,t.nombre)>0 OR CHARINDEX(@buscar,CONVERT(nvarchar(20),a.id_apartado))>0)
ORDER BY a.id_apartado DESC
```

### Q022 · ApartadoDatos.Obtener · línea 34

Fuente: Datos/ApartadoDatos.cs:34. Recupera la cabecera y los detalles del registro solicitado.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT a.id_apartado AS Id,c.nombre AS Cliente,t.nombre AS Tienda,a.fecha AS FechaUTC,a.vence_en AS VenceUTC,a.estado AS Estado,
    total.total AS Total,ISNULL(ab.Monto,0) AS Abonado,total.total-ISNULL(ab.Monto,0) AS Saldo,
    ISNULL(ab.Monto,0)-ISNULL(re.Monto,0) AS Reembolsable,a.id_venta AS Venta,a.version_fila,a.id_tienda,a.id_cliente
FROM dbo.Apartado a JOIN dbo.Cliente c ON c.id_cliente=a.id_cliente JOIN dbo.Tienda t ON t.id_tienda=a.id_tienda
JOIN dbo.vw_ApartadoTotal total ON total.id_apartado=a.id_apartado
OUTER APPLY(SELECT SUM(monto) AS Monto FROM dbo.AbonoApartado WHERE id_apartado=a.id_apartado) ab
OUTER APPLY(SELECT SUM(r.monto) AS Monto FROM dbo.Reembolso r JOIN dbo.AbonoApartado b ON b.id_abono=r.id_abono WHERE b.id_apartado=a.id_apartado) re WHERE a.id_apartado=@id
```

### Q023 · ApartadoDatos.Obtener · línea 35

Fuente: Datos/ApartadoDatos.cs:35. Recupera la cabecera y los detalles del registro solicitado.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT p.nombre AS Producto,d.cantidad AS Cantidad,d.precio_unitario AS Precio,d.subtotal AS Subtotal,d.tasa_isv AS TasaISV,d.isv AS ISV,d.total_linea AS Importe
FROM dbo.DetalleApartado d JOIN dbo.Producto p ON p.id_producto=d.id_producto WHERE d.id_apartado=@id
```

### Q024 · ApartadoDatos.Obtener · línea 39

Fuente: Datos/ApartadoDatos.cs:39. Recupera la cabecera y los detalles del registro solicitado.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT a.id_abono AS Id,a.fecha AS FechaUTC,m.nombre AS Metodo,a.monto AS Monto,a.referencia AS Referencia,a.id_pago_venta AS PagoVenta,
    a.monto-ISNULL((SELECT SUM(r.monto) FROM dbo.Reembolso r WHERE r.id_abono=a.id_abono),0) AS Reembolsable
FROM dbo.AbonoApartado a JOIN dbo.MetodoPago m ON m.id_metodo_pago=a.id_metodo_pago WHERE a.id_apartado=@id ORDER BY a.id_abono
```

### Q025 · ApartadoDatos.Obtener · línea 44

Fuente: Datos/ApartadoDatos.cs:44. Recupera la cabecera y los detalles del registro solicitado.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT r.id_reembolso AS Id,r.fecha AS FechaUTC,r.id_abono AS Abono,r.monto AS Monto,r.motivo AS Motivo
FROM dbo.Reembolso r JOIN dbo.AbonoApartado a ON a.id_abono=r.id_abono WHERE a.id_apartado=@id
```

### Q026 · ApartadoDatos.Crear · línea 58

Fuente: Datos/ApartadoDatos.cs:58. Crea un nuevo registro con sus validaciones y referencias.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @cliente, @tienda, @vence.

```sql
IF @vence<=SYSUTCDATETIME() THROW 50001,'El vencimiento debe ser posterior a la fecha actual.',1;
IF NOT EXISTS(SELECT 1 FROM dbo.Tienda WITH(HOLDLOCK) WHERE id_tienda=@tienda AND activo=1)
    OR NOT EXISTS(SELECT 1 FROM dbo.Cliente WITH(HOLDLOCK) WHERE id_cliente=@cliente AND activo=1)
    THROW 50001,'La tienda y el cliente deben estar activos.',1;
```

### Q027 · ApartadoDatos.Crear · línea 64

Fuente: Datos/ApartadoDatos.cs:64. Crea un nuevo registro con sus validaciones y referencias.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @cliente, @tienda, @usuario, @vence.

```sql
INSERT dbo.Apartado(id_tienda,id_cliente,id_usuario,vence_en) VALUES(@tienda,@cliente,@usuario,@vence);
SELECT CONVERT(int,SCOPE_IDENTITY());
```

### Q028 · ApartadoDatos.Crear · línea 72

Fuente: Datos/ApartadoDatos.cs:72. Crea un nuevo registro con sus validaciones y referencias.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @cantidad, @id, @precio, @producto, @tasa, @tienda.

```sql
INSERT dbo.DetalleApartado(id_apartado,id_producto,cantidad,precio_unitario,tasa_isv)
SELECT @id,p.id_producto,@cantidad,p.precio_unitario,@tasa FROM dbo.Producto p WITH(HOLDLOCK)
JOIN dbo.InventarioTienda i WITH(UPDLOCK,HOLDLOCK) ON i.id_producto=p.id_producto AND i.id_tienda=@tienda
WHERE p.id_producto=@producto AND p.activo=1 AND p.precio_unitario=@precio AND i.existencia-i.reservado>=@cantidad;
IF @@ROWCOUNT<>1 THROW 50001,'El precio o stock cambió. Recarga los productos antes de reservar.',1;
```

### Q029 · ApartadoDatos.Bloquear · línea 85

Fuente: Datos/ApartadoDatos.cs:85. Bloquea y compara la versión del registro para evitar operar sobre información obsoleta.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT * FROM dbo.Apartado WITH(UPDLOCK,HOLDLOCK) WHERE id_apartado=@id
```

### Q030 · ApartadoDatos.Vigente · línea 94

Fuente: Datos/ApartadoDatos.cs:94. Verifica vigencia, estado o vencimiento antes de permitir la operación.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
IF EXISTS(SELECT 1 FROM dbo.Apartado WHERE id_apartado=@id AND vence_en<=SYSUTCDATETIME()) THROW 50001,'El apartado venció. Márcalo como vencido para liberar la reserva.',1;
```

### Q031 · ApartadoDatos.Abonar · línea 102

Fuente: Datos/ApartadoDatos.cs:102. Registra un cobro del apartado, con método y referencia.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id, @metodo, @monto, @referencia, @usuario.

```sql
INSERT dbo.AbonoApartado(id_apartado,id_metodo_pago,monto,id_usuario,referencia) VALUES(@id,@metodo,@monto,@usuario,@referencia);
```

### Q032 · ApartadoDatos.Cerrar · línea 113

Fuente: Datos/ApartadoDatos.cs:113. Cierra el documento o la sesión según la entidad y el estado indicados.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @estado, @id.

```sql
IF @estado='VENCIDO' AND EXISTS(SELECT 1 FROM dbo.Apartado WHERE id_apartado=@id AND vence_en>SYSUTCDATETIME())
    THROW 50001,'El apartado todavía no ha vencido.',1;
UPDATE dbo.Apartado SET estado=@estado WHERE id_apartado=@id;
```

### Q033 · ApartadoDatos.Entregar · línea 125

Fuente: Datos/ApartadoDatos.cs:125. Convierte el apartado en venta, enlaza sus abonos y confirma la entrega.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
IF NOT EXISTS(SELECT 1 FROM dbo.DetalleApartado WHERE id_apartado=@id)
    THROW 50001,'El apartado no contiene productos.',1;
IF EXISTS(SELECT 1 FROM dbo.Apartado a JOIN dbo.Tienda t WITH(HOLDLOCK) ON t.id_tienda=a.id_tienda
    JOIN dbo.Cliente c WITH(HOLDLOCK) ON c.id_cliente=a.id_cliente WHERE a.id_apartado=@id AND (t.activo=0 OR c.activo=0))
    OR EXISTS(SELECT 1 FROM dbo.DetalleApartado d JOIN dbo.Producto p WITH(HOLDLOCK) ON p.id_producto=d.id_producto WHERE d.id_apartado=@id AND p.activo=0)
    THROW 50001,'La tienda, el cliente y los productos deben estar activos para facturar.',1;
```

### Q034 · ApartadoDatos.Entregar · línea 133

Fuente: Datos/ApartadoDatos.cs:133. Convierte el apartado en venta, enlaza sus abonos y confirma la entrega.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT total, total-ISNULL((SELECT SUM(monto) FROM dbo.AbonoApartado WHERE id_apartado=@id),0) AS saldo FROM dbo.vw_ApartadoTotal WHERE id_apartado=@id
```

### Q035 · ApartadoDatos.Entregar · línea 139

Fuente: Datos/ApartadoDatos.cs:139. Convierte el apartado en venta, enlaza sus abonos y confirma la entrega.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @cliente, @numero, @texto, @tienda, @usuario.

```sql
INSERT dbo.Venta(numero,id_tienda,id_cliente,cai,id_usuario,observaciones)
VALUES(@numero,@tienda,@cliente,(SELECT cai FROM dbo.ConfiguracionFacturacion WITH(HOLDLOCK) WHERE id_tienda=@tienda),@usuario,@texto);
SELECT CONVERT(int,SCOPE_IDENTITY());
```

### Q036 · ApartadoDatos.Entregar · línea 144

Fuente: Datos/ApartadoDatos.cs:144. Convierte el apartado en venta, enlaza sus abonos y confirma la entrega.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id, @venta.

```sql
INSERT dbo.DetalleVenta(id_venta,id_producto,cantidad,precio_unitario,nombre_producto,tasa_isv)
SELECT @venta,d.id_producto,d.cantidad,d.precio_unitario,p.nombre,d.tasa_isv FROM dbo.DetalleApartado d JOIN dbo.Producto p ON p.id_producto=d.id_producto WHERE d.id_apartado=@id;
UPDATE dbo.Apartado SET id_venta=@venta WHERE id_apartado=@id;
```

### Q037 · ApartadoDatos.Entregar · línea 149

Fuente: Datos/ApartadoDatos.cs:149. Convierte el apartado en venta, enlaza sus abonos y confirma la entrega.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT * FROM dbo.AbonoApartado WITH(UPDLOCK,HOLDLOCK) WHERE id_apartado=@id ORDER BY id_abono
```

### Q038 · ApartadoDatos.Entregar · línea 152

Fuente: Datos/ApartadoDatos.cs:152. Convierte el apartado en venta, enlaza sus abonos y confirma la entrega.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @metodo, @monto, @referencia, @venta.

```sql
INSERT dbo.PagoVenta(id_venta,id_metodo_pago,monto,monto_recibido,referencia) VALUES(@venta,@metodo,@monto,@monto,@referencia);
SELECT CONVERT(int,SCOPE_IDENTITY());
```

### Q039 · ApartadoDatos.Entregar · línea 156

Fuente: Datos/ApartadoDatos.cs:156. Convierte el apartado en venta, enlaza sus abonos y confirma la entrega.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @abono, @pago.

```sql
UPDATE dbo.AbonoApartado SET id_pago_venta=@pago WHERE id_abono=@abono
```

### Q040 · ApartadoDatos.Entregar · línea 158

Fuente: Datos/ApartadoDatos.cs:158. Convierte el apartado en venta, enlaza sus abonos y confirma la entrega.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @metodo, @monto, @recibido, @referencia, @venta.

```sql
INSERT dbo.PagoVenta(id_venta,id_metodo_pago,monto,monto_recibido,referencia) VALUES(@venta,@metodo,@monto,@recibido,@referencia)
```

### Q041 · ApartadoDatos.Entregar · línea 160

Fuente: Datos/ApartadoDatos.cs:160. Convierte el apartado en venta, enlaza sus abonos y confirma la entrega.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @venta.

```sql
UPDATE dbo.Venta SET estado='CONFIRMADA' WHERE id_venta=@venta
```

### Q042 · ApartadoDatos.Reembolsar · línea 170

Fuente: Datos/ApartadoDatos.cs:170. Calcula el saldo reembolsable y registra la devolución del dinero contra el cobro original.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT b.id_abono,b.id_metodo_pago,b.monto-ISNULL((SELECT SUM(r.monto) FROM dbo.Reembolso r WHERE r.id_abono=b.id_abono),0) AS pendiente
FROM dbo.AbonoApartado b WITH(UPDLOCK,HOLDLOCK) WHERE b.id_apartado=@id AND b.id_pago_venta IS NULL ORDER BY b.id_abono
```

### Q043 · ApartadoDatos.Reembolsar · línea 178

Fuente: Datos/ApartadoDatos.cs:178. Calcula el saldo reembolsable y registra la devolución del dinero contra el cobro original.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @abono, @metodo, @monto, @motivo, @usuario.

```sql
INSERT dbo.Reembolso(id_abono,monto,id_metodo_pago,id_usuario,motivo) VALUES(@abono,@monto,@metodo,@usuario,@motivo)
```

## B. Datos/AuditoriaDatos.cs

### Q044 · AuditoriaDatos.RegistrarExportacion · línea 12

Fuente: Datos/AuditoriaDatos.cs:12. Registra la exportación realizada por el usuario.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @descripcion, @entidad, @equipo, @id, @modulo, @usuario.

```sql
INSERT dbo.Auditoria(id_usuario,modulo,accion,entidad,id_registro,descripcion,equipo)
VALUES(@usuario,@modulo,'EXPORTAR',@entidad,@id,@descripcion,@equipo)
```

### Q045 · AuditoriaDatos.RegistrarNavegacion · línea 29

Fuente: Datos/AuditoriaDatos.cs:29. Registra la apertura de una pantalla en la bitácora.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @descripcion, @equipo, @pantalla, @usuario.

```sql
INSERT dbo.Auditoria(id_usuario,modulo,accion,entidad,id_registro,descripcion,equipo)
VALUES(@usuario,'NAVEGACION','ABRIR','Pantalla',@pantalla,@descripcion,@equipo)
```

### Q046 · AuditoriaDatos.Listar · línea 44

Fuente: Datos/AuditoriaDatos.cs:44. Obtiene el listado y aplica los filtros del módulo.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @antes, @buscar.

```sql
SELECT TOP(200) a.id_auditoria AS Id,a.fecha AS [Fecha UTC],
COALESCE(u.nombre_usuario,N'Sistema / acceso sin autenticar') AS Usuario,
m.modulo AS Modulo,a.accion AS Accion,a.entidad AS Entidad,a.id_registro AS Registro,
a.descripcion AS Descripcion,a.equipo AS Equipo
FROM dbo.Auditoria a LEFT JOIN dbo.Usuario u ON u.id_usuario=a.id_usuario
CROSS APPLY (VALUES(CASE WHEN a.modulo IN ('DOCUMENTOS','CATALOGO') THEN
    CASE a.entidad WHEN 'Compra' THEN 'COMPRAS' WHEN 'Venta' THEN 'VENTAS'
    WHEN 'Traspaso' THEN 'TRASPASOS' WHEN 'Apartado' THEN 'APARTADOS'
    WHEN 'Devolucion' THEN 'DEVOLUCIONES' WHEN 'Categoria' THEN 'PRODUCTOS' ELSE a.modulo END
    ELSE a.modulo END)) m(modulo)
WHERE a.id_auditoria<@antes AND (@buscar=N'' OR CHARINDEX(@buscar,a.descripcion)>0
    OR CHARINDEX(@buscar,m.modulo)>0 OR CHARINDEX(@buscar,a.entidad)>0
    OR CHARINDEX(@buscar,a.id_registro)>0 OR CHARINDEX(@buscar,a.equipo)>0
    OR CHARINDEX(@buscar,a.accion)>0 OR CHARINDEX(@buscar,u.nombre_usuario)>0)
ORDER BY a.id_auditoria DESC
```

## B. Datos/CargaMasivaDatos.cs

### Q047 · CargaMasivaDatos.CodigosExistentes · línea 25

Fuente: Datos/CargaMasivaDatos.cs:25. Sentencia o fragmento utilizado en CargaMasivaDatos.CodigosExistentes. Operaciones presentes: SELECT. Su contexto y sus variantes se identifican en la referencia al código.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT codigo_barras FROM Producto
```

### Q048 · CargaMasivaDatos.MapaCategorias · línea 36

Fuente: Datos/CargaMasivaDatos.cs:36. Sentencia o fragmento utilizado en CargaMasivaDatos.MapaCategorias. Operaciones presentes: SELECT. Objetos referidos: Categoria.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT id_categoria, nombre FROM dbo.Categoria WHERE activo=1
```

### Q049 · CargaMasivaDatos.MapaProveedoresPorRtn · línea 47

Fuente: Datos/CargaMasivaDatos.cs:47. Sentencia o fragmento utilizado en CargaMasivaDatos.MapaProveedoresPorRtn. Operaciones presentes: SELECT. Objetos referidos: Proveedor.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT id_proveedor, rtn FROM dbo.Proveedor WHERE activo=1 AND rtn IS NOT NULL
```

### Q050 · CargaMasivaDatos.ImportarProductos · línea 69

Fuente: Datos/CargaMasivaDatos.cs:69. Integra el lote de Excel: productos, stock inicial e historial de importación, dentro de una transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @codigo, @idCategoria, @idProveedor, @nombre, @precio, @usuario.

```sql
IF NOT EXISTS(SELECT 1 FROM dbo.Categoria WITH(HOLDLOCK) WHERE id_categoria=@idCategoria AND activo=1)
    OR NOT EXISTS(SELECT 1 FROM dbo.Proveedor WITH(HOLDLOCK) WHERE id_proveedor=@idProveedor AND activo=1)
    THROW 50001,'La categoría o el proveedor ya no están activos. Vuelve a cargar el archivo.',1;
INSERT dbo.Producto(codigo_barras,nombre,precio_unitario,id_categoria,id_proveedor,creado_por)
VALUES(@codigo,@nombre,@precio,@idCategoria,@idProveedor,@usuario);
SELECT CONVERT(int,SCOPE_IDENTITY());
```

### Q051 · CargaMasivaDatos.ImportarProductos · línea 83

Fuente: Datos/CargaMasivaDatos.cs:83. Integra el lote de Excel: productos, stock inicial e historial de importación, dentro de una transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @conStock, @key, @tienda, @usuario, @value.

```sql
EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@usuario;
EXEC sys.sp_set_session_context @key=N'motivo',@value=N'Importación de productos desde Excel';
IF @tienda>0 AND NOT EXISTS(SELECT 1 FROM dbo.Tienda WITH(HOLDLOCK) WHERE id_tienda=@tienda AND activo=1)
    THROW 50001,'Selecciona una tienda activa para el stock inicial.',1;
IF @conStock=1 AND EXISTS(SELECT 1 FROM (VALUES('trg_MovimientoInventario_aplicar'),('trg_MovimientoInventario_validar'),
    ('trg_MovimientoInventario_documento'),('trg_MovimientoInventario_inmutable')) r(nombre)
    WHERE NOT EXISTS(SELECT 1 FROM sys.triggers WHERE object_id=OBJECT_ID('dbo.'+r.nombre) AND is_disabled=0))
    THROW 50001,'Instala Triggers2.sql antes de importar stock inicial.',1;
```

### Q052 · CargaMasivaDatos.ImportarProductos · línea 93

Fuente: Datos/CargaMasivaDatos.cs:93. Integra el lote de Excel: productos, stock inicial e historial de importación, dentro de una transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @archivo, @hash, @usuario.

```sql
INSERT dbo.Importacion(nombre_archivo,archivo_hash,id_usuario,estado) VALUES(@archivo,@hash,@usuario,'VALIDANDO');
SELECT CONVERT(int,SCOPE_IDENTITY());
```

### Q053 · CargaMasivaDatos.ImportarProductos · línea 116

Fuente: Datos/CargaMasivaDatos.cs:116. Integra el lote de Excel: productos, stock inicial e historial de importación, dentro de una transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @cantidad, @producto, @tienda, @usuario.

```sql
INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,motivo)
VALUES(@tienda,@producto,'AJUSTE',@cantidad,@usuario,N'Stock inicial de carga masiva');
```

### Q054 · CargaMasivaDatos.ImportarProductos · línea 120

Fuente: Datos/CargaMasivaDatos.cs:120. Integra el lote de Excel: productos, stock inicial e historial de importación, dentro de una transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @fila, @id, @json, @producto.

```sql
INSERT dbo.DetalleImportacion(id_importacion,numero_fila,datos_originales_json,estado,id_producto)
VALUES(@id,@fila,@json,'IMPORTADA',@producto);
```

### Q055 · CargaMasivaDatos.ImportarProductos · línea 125

Fuente: Datos/CargaMasivaDatos.cs:125. Integra el lote de Excel: productos, stock inicial e historial de importación, dentro de una transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
UPDATE dbo.Importacion SET estado='COMPLETADA' WHERE id_importacion=@id
```

## B. Datos/CategoriaDatos.cs

### Q056 · CategoriaDatos.Listar · línea 12

Fuente: Datos/CategoriaDatos.cs:12. Obtiene el listado y aplica los filtros del módulo.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @incluirInactivas.

```sql
SELECT id_categoria, nombre, descripcion, activo FROM dbo.Categoria
                                 WHERE activo = 1 OR @incluirInactivas = 1 ORDER BY nombre
```

### Q057 · CategoriaDatos.ExisteNombre · línea 25

Fuente: Datos/CategoriaDatos.cs:25. Sentencia o fragmento utilizado en CategoriaDatos.ExisteNombre. Operaciones presentes: SELECT. Objetos referidos: Categoria.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @excluirId, @nombre.

```sql
SELECT COUNT(1) FROM dbo.Categoria
                                 WHERE nombre = @nombre
                                   AND (@excluirId IS NULL OR id_categoria <> @excluirId)
```

## B. Datos/ClienteDatos.cs

### Q058 · ClienteDatos.Listar · línea 16

Fuente: Datos/ClienteDatos.cs:16. Obtiene el listado y aplica los filtros del módulo.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @incluir.

```sql
SELECT id_cliente, nombre, telefono, correo,
                                        CAST(CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END AS bit) AS activo,
                                        desactivado_en, motivo_desactivacion, actualizado_en
                                 FROM Cliente
                                 WHERE (@incluir = 1 OR desactivado_en IS NULL)
                                 ORDER BY nombre
```

### Q059 · ClienteDatos.Agregar · línea 36

Fuente: Datos/ClienteDatos.cs:36. Sentencia o fragmento utilizado en ClienteDatos.Agregar. Operaciones presentes: INSERT. Su contexto y sus variantes se identifican en la referencia al código.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @correo, @nombre, @telefono.

```sql
INSERT INTO Cliente (nombre, telefono, correo)
                                 OUTPUT INSERTED.id_cliente
                                 VALUES (@nombre, @telefono, @correo)
```

### Q060 · ClienteDatos.Editar · línea 55

Fuente: Datos/ClienteDatos.cs:55. Sentencia o fragmento utilizado en ClienteDatos.Editar. Operaciones presentes: UPDATE. Su contexto y sus variantes se identifican en la referencia al código.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @correo, @id, @nombre, @telefono.

```sql
UPDATE Cliente
                                 SET nombre = @nombre, telefono = @telefono, correo = @correo,
                                     actualizado_en = SYSDATETIME()
                                 WHERE id_cliente = @id
```

### Q061 · ClienteDatos.Desactivar · línea 79

Fuente: Datos/ClienteDatos.cs:79. Sentencia o fragmento utilizado en ClienteDatos.Desactivar. Operaciones presentes: UPDATE. Su contexto y sus variantes se identifican en la referencia al código.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id, @motivo, @por.

```sql
UPDATE Cliente
                                 SET desactivado_en = SYSDATETIME(), desactivado_por = @por,
                                     motivo_desactivacion = @motivo, actualizado_en = SYSDATETIME()
                                 WHERE id_cliente = @id AND desactivado_en IS NULL
```

### Q062 · ClienteDatos.Reactivar · línea 94

Fuente: Datos/ClienteDatos.cs:94. Sentencia o fragmento utilizado en ClienteDatos.Reactivar. Operaciones presentes: UPDATE. Su contexto y sus variantes se identifican en la referencia al código.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
UPDATE Cliente
                                 SET desactivado_en = NULL, desactivado_por = NULL,
                                     motivo_desactivacion = NULL, actualizado_en = SYSDATETIME()
                                 WHERE id_cliente = @id AND desactivado_en IS NOT NULL
```

## B. Datos/CompraDatos.cs

### Q063 · CompraDatos.VersionSql · línea 13

Fuente: Datos/CompraDatos.cs:13. Fragmento de huella SHA-256 sobre cabecera y detalles para detectar modificaciones.

Tipo: fragmento SQL auxiliar. Sin marcadores @ detectados.

```sql
HASHBYTES('SHA2_256',(
    SELECT c.id_compra,c.id_proveedor,c.id_tienda,c.numero_factura,c.fecha_factura,
           c.registrado_en,c.id_usuario,c.estado,c.observaciones,
           c.anulado_en,c.anulado_por,c.motivo_anulacion,
           (SELECT dc.id_detalle_compra,dc.id_producto,dc.cantidad,dc.costo_unitario
            FROM dbo.DetalleCompra dc WHERE dc.id_compra=c.id_compra
            ORDER BY dc.id_detalle_compra FOR JSON PATH) AS detalle
    FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES))
```

### Q064 · CompraDatos.CabeceraSql · línea 24

Fuente: Datos/CompraDatos.cs:24. Plantilla de cabecera con catálogos, total y huella de concurrencia.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT c.id_compra,p.nombre_empresa AS Proveedor,t.nombre AS Tienda,
       c.numero_factura,c.fecha_factura,c.registrado_en,u.nombre_usuario AS Usuario,
       c.estado,c.observaciones,c.motivo_anulacion,c.id_proveedor,c.id_tienda,
       total.total AS Total,HASHBYTES('SHA2_256',(
    SELECT c.id_compra,c.id_proveedor,c.id_tienda,c.numero_factura,c.fecha_factura,
           c.registrado_en,c.id_usuario,c.estado,c.observaciones,
           c.anulado_en,c.anulado_por,c.motivo_anulacion,
           (SELECT dc.id_detalle_compra,dc.id_producto,dc.cantidad,dc.costo_unitario
            FROM dbo.DetalleCompra dc WHERE dc.id_compra=c.id_compra
            ORDER BY dc.id_detalle_compra FOR JSON PATH) AS detalle
    FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES)) AS version_fila
FROM dbo.Compra c
JOIN dbo.Proveedor p ON p.id_proveedor=c.id_proveedor
JOIN dbo.Tienda t ON t.id_tienda=c.id_tienda
JOIN dbo.Usuario u ON u.id_usuario=c.id_usuario
JOIN dbo.vw_CompraTotal total ON total.id_compra=c.id_compra
```

### Q065 · CompraDatos.DetalleSql · línea 36

Fuente: Datos/CompraDatos.cs:36. Plantilla del detalle de compra, incluidos producto, cantidad y costo.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT d.id_detalle_compra,d.id_producto,p.codigo_barras AS Codigo,
       p.nombre AS Producto,d.cantidad,d.costo_unitario,d.subtotal,p.activo AS Activo
FROM dbo.DetalleCompra d JOIN dbo.Producto p ON p.id_producto=d.id_producto
WHERE d.id_compra=@id ORDER BY d.id_detalle_compra
```

### Q066 · CompraDatos.Proveedores · línea 43

Fuente: Datos/CompraDatos.cs:43. Sentencia o fragmento utilizado en CompraDatos.Proveedores. Operaciones presentes: SELECT. Objetos referidos: Proveedor.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT id_proveedor,nombre_empresa AS nombre FROM dbo.Proveedor
WHERE activo=1 ORDER BY nombre_empresa
```

### Q067 · CompraDatos.Tiendas · línea 48

Fuente: Datos/CompraDatos.cs:48. Sentencia o fragmento utilizado en CompraDatos.Tiendas. Operaciones presentes: SELECT. Objetos referidos: Tienda.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT id_tienda,nombre FROM dbo.Tienda WHERE activo=1 ORDER BY nombre
```

### Q068 · CompraDatos.Productos · línea 52

Fuente: Datos/CompraDatos.cs:52. Consulta los productos disponibles para el módulo y, cuando corresponde, sus saldos por tienda.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT id_producto,codigo_barras,nombre,precio_unitario FROM dbo.Producto
WHERE activo=1 ORDER BY nombre
```

### Q069 · CompraDatos.Listar · línea 66

Fuente: Datos/CompraDatos.cs:66. Obtiene el listado y aplica los filtros del módulo.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @buscar, @desde, @estado, @hasta.

```sql
SELECT c.id_compra,p.nombre_empresa AS Proveedor,t.nombre AS Tienda,
       c.numero_factura,c.fecha_factura,c.registrado_en,u.nombre_usuario AS Usuario,
       c.estado,c.observaciones,c.motivo_anulacion,c.id_proveedor,c.id_tienda,
       total.total AS Total,HASHBYTES('SHA2_256',(
    SELECT c.id_compra,c.id_proveedor,c.id_tienda,c.numero_factura,c.fecha_factura,
           c.registrado_en,c.id_usuario,c.estado,c.observaciones,
           c.anulado_en,c.anulado_por,c.motivo_anulacion,
           (SELECT dc.id_detalle_compra,dc.id_producto,dc.cantidad,dc.costo_unitario
            FROM dbo.DetalleCompra dc WHERE dc.id_compra=c.id_compra
            ORDER BY dc.id_detalle_compra FOR JSON PATH) AS detalle
    FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES)) AS version_fila
FROM dbo.Compra c
JOIN dbo.Proveedor p ON p.id_proveedor=c.id_proveedor
JOIN dbo.Tienda t ON t.id_tienda=c.id_tienda
JOIN dbo.Usuario u ON u.id_usuario=c.id_usuario
JOIN dbo.vw_CompraTotal total ON total.id_compra=c.id_compra
WHERE (@estado='' OR c.estado=@estado)
AND (@desde IS NULL OR c.fecha_factura>=@desde)
AND (@hasta IS NULL OR c.fecha_factura<=@hasta)
AND (@buscar=N'' OR CHARINDEX(@buscar,c.numero_factura)>0
    OR CHARINDEX(@buscar,p.nombre_empresa)>0 OR CHARINDEX(@buscar,t.nombre)>0
    OR CHARINDEX(@buscar,ISNULL(c.observaciones,N''))>0
    OR CHARINDEX(@buscar,CONVERT(nvarchar(20),c.id_compra))>0)
ORDER BY c.id_compra DESC
```

### Q070 · CompraDatos.Obtener · línea 90

Fuente: Datos/CompraDatos.cs:90. Recupera la cabecera y los detalles del registro solicitado.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT c.id_compra,p.nombre_empresa AS Proveedor,t.nombre AS Tienda,
       c.numero_factura,c.fecha_factura,c.registrado_en,u.nombre_usuario AS Usuario,
       c.estado,c.observaciones,c.motivo_anulacion,c.id_proveedor,c.id_tienda,
       total.total AS Total,HASHBYTES('SHA2_256',(
    SELECT c.id_compra,c.id_proveedor,c.id_tienda,c.numero_factura,c.fecha_factura,
           c.registrado_en,c.id_usuario,c.estado,c.observaciones,
           c.anulado_en,c.anulado_por,c.motivo_anulacion,
           (SELECT dc.id_detalle_compra,dc.id_producto,dc.cantidad,dc.costo_unitario
            FROM dbo.DetalleCompra dc WHERE dc.id_compra=c.id_compra
            ORDER BY dc.id_detalle_compra FOR JSON PATH) AS detalle
    FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES)) AS version_fila
FROM dbo.Compra c
JOIN dbo.Proveedor p ON p.id_proveedor=c.id_proveedor
JOIN dbo.Tienda t ON t.id_tienda=c.id_tienda
JOIN dbo.Usuario u ON u.id_usuario=c.id_usuario
JOIN dbo.vw_CompraTotal total ON total.id_compra=c.id_compra WHERE c.id_compra=@id
```

### Q071 · CompraDatos.Guardar · línea 130

Fuente: Datos/CompraDatos.cs:130. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @factura, @fecha, @id, @proveedor, @texto, @tienda.

```sql
UPDATE dbo.Compra SET numero_factura=@factura,fecha_factura=@fecha,observaciones=@texto
WHERE id_compra=@id AND estado='BORRADOR' AND id_proveedor=@proveedor AND id_tienda=@tienda;
IF @@ROWCOUNT=0 THROW 50001, 'No se permite cambiar proveedor ni tienda de una compra existente.', 1;
SELECT @id;
```

### Q072 · CompraDatos.Guardar · línea 135

Fuente: Datos/CompraDatos.cs:135. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @factura, @fecha, @proveedor, @texto, @tienda, @usuario.

```sql
INSERT dbo.Compra(id_proveedor,id_tienda,numero_factura,fecha_factura,id_usuario,observaciones)
VALUES(@proveedor,@tienda,@factura,@fecha,@usuario,@texto);
SELECT CAST(SCOPE_IDENTITY() AS int);
```

### Q073 · CompraDatos.Guardar · línea 148

Fuente: Datos/CompraDatos.cs:148. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
DELETE dbo.DetalleCompra WHERE id_compra=@id
```

### Q074 · CompraDatos.Guardar · línea 155

Fuente: Datos/CompraDatos.cs:155. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @cantidad, @costo, @id, @producto.

```sql
INSERT dbo.DetalleCompra(id_compra,id_producto,cantidad,costo_unitario)
VALUES(@id,@producto,@cantidad,@costo)
```

### Q075 · CompraDatos.Confirmar · línea 184

Fuente: Datos/CompraDatos.cs:184. Valida el documento y lo pasa a confirmado; sus triggers aplican el efecto correspondiente.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
IF NOT EXISTS(SELECT 1 FROM dbo.DetalleCompra WHERE id_compra=@id)
    THROW 50001, 'Agrega al menos un producto antes de confirmar.', 1;
IF EXISTS(SELECT 1 FROM dbo.Compra c JOIN dbo.Proveedor p WITH(HOLDLOCK) ON p.id_proveedor=c.id_proveedor
    JOIN dbo.Tienda t WITH(HOLDLOCK) ON t.id_tienda=c.id_tienda
    WHERE c.id_compra=@id AND (p.activo=0 OR t.activo=0))
    THROW 50001, 'El proveedor y la tienda deben estar activos para confirmar.', 1;
IF EXISTS(SELECT 1 FROM dbo.DetalleCompra d JOIN dbo.Producto p WITH(HOLDLOCK) ON p.id_producto=d.id_producto
    WHERE d.id_compra=@id AND p.activo=0)
    THROW 50001, 'La compra contiene productos inactivos. Actualiza su detalle antes de confirmar.', 1;
```

### Q076 · CompraDatos.Confirmar · línea 199

Fuente: Datos/CompraDatos.cs:199. Valida el documento y lo pasa a confirmado; sus triggers aplican el efecto correspondiente.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
UPDATE dbo.Compra SET estado='CONFIRMADA' WHERE id_compra=@id
```

### Q077 · CompraDatos.Anular · línea 215

Fuente: Datos/CompraDatos.cs:215. Anula el documento con actor, fecha y motivo; los triggers verifican si procede el reverso.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id, @motivo, @usuario.

```sql
UPDATE dbo.Compra SET estado='ANULADA',anulado_en=SYSUTCDATETIME(),
    anulado_por=@usuario,motivo_anulacion=@motivo WHERE id_compra=@id
```

### Q078 · CompraDatos.ValidarEntidades · línea 250

Fuente: Datos/CompraDatos.cs:250. Sentencia o fragmento utilizado en CompraDatos.ValidarEntidades. Operaciones presentes: SELECT, THROW. Objetos referidos: Proveedor, Tienda.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @proveedor, @tienda.

```sql
IF NOT EXISTS(SELECT 1 FROM dbo.Proveedor WITH(HOLDLOCK) WHERE id_proveedor=@proveedor AND activo=1)
    OR NOT EXISTS(SELECT 1 FROM dbo.Tienda WITH(HOLDLOCK) WHERE id_tienda=@tienda AND activo=1)
    THROW 50001, 'El proveedor y la tienda deben estar activos.', 1;
```

### Q079 · CompraDatos.ValidarEntidades · línea 262

Fuente: Datos/CompraDatos.cs:262. Sentencia o fragmento utilizado en CompraDatos.ValidarEntidades. Operaciones presentes: SELECT, THROW. Objetos referidos: Producto.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @producto.

```sql
IF NOT EXISTS(SELECT 1 FROM dbo.Producto WITH(HOLDLOCK) WHERE id_producto=@producto AND activo=1)
    THROW 50001, 'Un producto ya no está activo. Recarga los catálogos y revisa el detalle.', 1;
```

### Q080 · CompraDatos.ComprobarVersion · línea 274

Fuente: Datos/CompraDatos.cs:274. Lee la versión bajo bloqueo y detecta cambios concurrentes.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT id_compra FROM dbo.Compra WITH(UPDLOCK,HOLDLOCK) WHERE id_compra=@id;
SELECT id_detalle_compra FROM dbo.DetalleCompra WITH(UPDLOCK,HOLDLOCK) WHERE id_compra=@id;
```

### Q081 · CompraDatos.ComprobarVersion · línea 282

Fuente: Datos/CompraDatos.cs:282. Lee la versión bajo bloqueo y detecta cambios concurrentes.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT c.estado,HASHBYTES('SHA2_256',(
    SELECT c.id_compra,c.id_proveedor,c.id_tienda,c.numero_factura,c.fecha_factura,
           c.registrado_en,c.id_usuario,c.estado,c.observaciones,
           c.anulado_en,c.anulado_por,c.motivo_anulacion,
           (SELECT dc.id_detalle_compra,dc.id_producto,dc.cantidad,dc.costo_unitario
            FROM dbo.DetalleCompra dc WHERE dc.id_compra=c.id_compra
            ORDER BY dc.id_detalle_compra FOR JSON PATH) AS detalle
    FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES)) AS version_fila FROM dbo.Compra c WHERE c.id_compra=@id
```

### Q082 · CompraDatos.Preparar · línea 293

Fuente: Datos/CompraDatos.cs:293. Valida dependencias y sesión y establece el contexto del operador para los triggers.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @key, @motivo, @usuario, @value.

```sql
IF (SELECT value_in_use FROM sys.configurations WHERE name='nested triggers')<>1
    OR DATABASEPROPERTYEX(DB_NAME(),'IsRecursiveTriggersEnabled')=1
    THROW 50004, 'Compras requiere nested triggers habilitado y RECURSIVE_TRIGGERS OFF, como indica Triggers2.sql.', 1;
IF EXISTS(SELECT requerido.nombre FROM (VALUES
    (N'trg_Compra_alta_baja'),(N'trg_Compra_cabecera'),(N'trg_Compra_estado'),
    (N'trg_DetalleCompra_bloqueo'),(N'trg_Compra_contabilizar'),(N'trg_Compra_auditoria'),
    (N'trg_MovimientoInventario_validar'),(N'trg_MovimientoInventario_documento'),
    (N'trg_MovimientoInventario_aplicar'),(N'trg_MovimientoInventario_inmutable')) requerido(nombre)
    WHERE NOT EXISTS(SELECT 1 FROM sys.triggers t
        WHERE t.object_id=OBJECT_ID(N'dbo.'+requerido.nombre) AND t.is_disabled=0))
    THROW 50004, 'Instale todos los triggers de compras e inventario de Triggers2.sql antes de operar.', 1;
EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@usuario;
EXEC sys.sp_set_session_context @key=N'motivo',@value=@motivo;
```

## B. Datos/Conexion.cs

### Q083 · Conexion.AbrirConexion · línea 32

Fuente: Datos/Conexion.cs:32. Sentencia o fragmento utilizado en Conexion.AbrirConexion. Operaciones presentes: EXEC. Su contexto y sus variantes se identifican en la referencia al código.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @key, @usuario, @value.

```sql
EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@usuario; EXEC sys.sp_set_session_context @key=N'motivo',@value=NULL;
```

## B. Datos/ConfiguracionFacturacionDatos.cs

### Q084 · ConfiguracionFacturacionDatos.Obtener · línea 23

Fuente: Datos/ConfiguracionFacturacionDatos.cs:23. Recupera la cabecera y los detalles del registro solicitado.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @tienda.

```sql
SELECT cai,version_fila FROM dbo.ConfiguracionFacturacion WHERE id_tienda=@tienda
```

### Q085 · ConfiguracionFacturacionDatos.Guardar · línea 31

Fuente: Datos/ConfiguracionFacturacionDatos.cs:31. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @tienda.

```sql
SELECT version_fila FROM dbo.ConfiguracionFacturacion WITH(UPDLOCK,HOLDLOCK) WHERE id_tienda=@tienda
```

### Q086 · ConfiguracionFacturacionDatos.Guardar · línea 34

Fuente: Datos/ConfiguracionFacturacionDatos.cs:34. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @cai, @key, @tienda, @usuario, @value.

```sql
EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@usuario;
IF NOT EXISTS(SELECT 1 FROM dbo.Tienda WITH(HOLDLOCK) WHERE id_tienda=@tienda AND activo=1)
    THROW 50001,'Selecciona una tienda activa.',1;
IF EXISTS(SELECT 1 FROM dbo.ConfiguracionFacturacion WHERE id_tienda=@tienda)
    UPDATE dbo.ConfiguracionFacturacion SET cai=@cai,actualizado_en=SYSUTCDATETIME(),actualizado_por=@usuario WHERE id_tienda=@tienda;
ELSE INSERT dbo.ConfiguracionFacturacion(id_tienda,cai,actualizado_por) VALUES(@tienda,@cai,@usuario);
```

## B. Datos/DevolucionDatos.cs

### Q087 · DevolucionDatos.Version · línea 12

Fuente: Datos/DevolucionDatos.cs:12. Fragmento de huella SHA-256 calculada sobre el documento y sus relaciones.

Tipo: fragmento SQL auxiliar. Sin marcadores @ detectados.

```sql
HASHBYTES('SHA2_256',(SELECT d.id_devolucion,d.id_venta,d.estado,d.motivo,d.anulado_en,d.motivo_anulacion,
    (SELECT x.id_detalle_devolucion,x.id_detalle_venta,x.cantidad,x.reintegra_stock,x.isv FROM dbo.DetalleDevolucion x
        WHERE x.id_devolucion=d.id_devolucion ORDER BY x.id_detalle_devolucion FOR JSON PATH) AS lineas,
    (SELECT r.id_reembolso,r.monto FROM dbo.Reembolso r WHERE r.id_devolucion=d.id_devolucion ORDER BY r.id_reembolso FOR JSON PATH) AS reembolsos
    FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES))
```

### Q088 · DevolucionDatos.Consulta · línea 19

Fuente: Datos/DevolucionDatos.cs:19. Plantilla base reutilizada para cabecera, filtros y recuperación del módulo.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT d.id_devolucion AS Id,v.numero AS Factura,c.nombre AS Cliente,t.nombre AS Tienda,d.fecha AS FechaUTC,
    d.estado AS Estado,d.motivo AS Motivo,total.total AS Total,ISNULL(re.Monto,0) AS Reembolsado,
    total.total-ISNULL(re.Monto,0) AS Pendiente,d.id_venta,HASHBYTES('SHA2_256',(SELECT d.id_devolucion,d.id_venta,d.estado,d.motivo,d.anulado_en,d.motivo_anulacion,
    (SELECT x.id_detalle_devolucion,x.id_detalle_venta,x.cantidad,x.reintegra_stock,x.isv FROM dbo.DetalleDevolucion x
        WHERE x.id_devolucion=d.id_devolucion ORDER BY x.id_detalle_devolucion FOR JSON PATH) AS lineas,
    (SELECT r.id_reembolso,r.monto FROM dbo.Reembolso r WHERE r.id_devolucion=d.id_devolucion ORDER BY r.id_reembolso FOR JSON PATH) AS reembolsos
    FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES)) AS version_fila
FROM dbo.Devolucion d JOIN dbo.Venta v ON v.id_venta=d.id_venta JOIN dbo.Cliente c ON c.id_cliente=v.id_cliente
JOIN dbo.Tienda t ON t.id_tienda=d.id_tienda JOIN dbo.vw_DevolucionTotal total ON total.id_devolucion=d.id_devolucion
OUTER APPLY(SELECT SUM(monto) AS Monto FROM dbo.Reembolso WHERE id_devolucion=d.id_devolucion) re
```

### Q089 · DevolucionDatos.Listar · línea 31

Fuente: Datos/DevolucionDatos.cs:31. Obtiene el listado y aplica los filtros del módulo.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @buscar, @estado.

```sql
SELECT d.id_devolucion AS Id,v.numero AS Factura,c.nombre AS Cliente,t.nombre AS Tienda,d.fecha AS FechaUTC,
    d.estado AS Estado,d.motivo AS Motivo,total.total AS Total,ISNULL(re.Monto,0) AS Reembolsado,
    total.total-ISNULL(re.Monto,0) AS Pendiente,d.id_venta,HASHBYTES('SHA2_256',(SELECT d.id_devolucion,d.id_venta,d.estado,d.motivo,d.anulado_en,d.motivo_anulacion,
    (SELECT x.id_detalle_devolucion,x.id_detalle_venta,x.cantidad,x.reintegra_stock,x.isv FROM dbo.DetalleDevolucion x
        WHERE x.id_devolucion=d.id_devolucion ORDER BY x.id_detalle_devolucion FOR JSON PATH) AS lineas,
    (SELECT r.id_reembolso,r.monto FROM dbo.Reembolso r WHERE r.id_devolucion=d.id_devolucion ORDER BY r.id_reembolso FOR JSON PATH) AS reembolsos
    FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES)) AS version_fila
FROM dbo.Devolucion d JOIN dbo.Venta v ON v.id_venta=d.id_venta JOIN dbo.Cliente c ON c.id_cliente=v.id_cliente
JOIN dbo.Tienda t ON t.id_tienda=d.id_tienda JOIN dbo.vw_DevolucionTotal total ON total.id_devolucion=d.id_devolucion
OUTER APPLY(SELECT SUM(monto) AS Monto FROM dbo.Reembolso WHERE id_devolucion=d.id_devolucion) re
WHERE (@estado='' OR d.estado=@estado) AND (@buscar='' OR CHARINDEX(@buscar,v.numero)>0
    OR CHARINDEX(@buscar,c.nombre)>0 OR CHARINDEX(@buscar,CONVERT(nvarchar(20),d.id_devolucion))>0)
ORDER BY d.id_devolucion DESC
```

### Q090 · DevolucionDatos.Ventas · línea 38

Fuente: Datos/DevolucionDatos.cs:38. Lista las facturas confirmadas que pueden utilizarse como origen de una devolución.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT v.id_venta,CONCAT(v.numero,N' · ',c.nombre,N' · ',t.nombre) AS nombre
FROM dbo.Venta v JOIN dbo.Cliente c ON c.id_cliente=v.id_cliente JOIN dbo.Tienda t ON t.id_tienda=v.id_tienda
WHERE v.estado='CONFIRMADA' ORDER BY v.id_venta DESC
```

### Q091 · DevolucionDatos.LineasVenta · línea 47

Fuente: Datos/DevolucionDatos.cs:47. Calcula cantidades todavía devolvibles y recupera el detalle que se está editando.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @devolucion, @venta.

```sql
SELECT dv.id_detalle_venta AS Id,dv.nombre_producto AS Producto,dv.cantidad AS Vendido,dv.precio_unitario AS Precio,dv.tasa_isv AS TasaISV,
    CONVERT(int,dv.cantidad-ISNULL(dev.cantidad,0)) AS Disponible,
    ISNULL(editar.cantidad,0) AS Devolver,ISNULL(editar.reintegra_stock,CONVERT(bit,1)) AS Reintegrar
FROM dbo.DetalleVenta dv
OUTER APPLY(SELECT SUM(CONVERT(bigint,x.cantidad)) AS cantidad FROM dbo.DetalleDevolucion x
    JOIN dbo.Devolucion d ON d.id_devolucion=x.id_devolucion
    WHERE x.id_detalle_venta=dv.id_detalle_venta AND d.estado<>'ANULADA' AND (@devolucion=0 OR d.id_devolucion<>@devolucion)) dev
OUTER APPLY(SELECT cantidad,reintegra_stock FROM dbo.DetalleDevolucion WHERE id_detalle_venta=dv.id_detalle_venta AND id_devolucion=@devolucion) editar
WHERE dv.id_venta=@venta ORDER BY dv.id_detalle_venta
```

### Q092 · DevolucionDatos.Obtener · línea 64

Fuente: Datos/DevolucionDatos.cs:64. Recupera la cabecera y los detalles del registro solicitado.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT d.id_devolucion AS Id,v.numero AS Factura,c.nombre AS Cliente,t.nombre AS Tienda,d.fecha AS FechaUTC,
    d.estado AS Estado,d.motivo AS Motivo,total.total AS Total,ISNULL(re.Monto,0) AS Reembolsado,
    total.total-ISNULL(re.Monto,0) AS Pendiente,d.id_venta,HASHBYTES('SHA2_256',(SELECT d.id_devolucion,d.id_venta,d.estado,d.motivo,d.anulado_en,d.motivo_anulacion,
    (SELECT x.id_detalle_devolucion,x.id_detalle_venta,x.cantidad,x.reintegra_stock,x.isv FROM dbo.DetalleDevolucion x
        WHERE x.id_devolucion=d.id_devolucion ORDER BY x.id_detalle_devolucion FOR JSON PATH) AS lineas,
    (SELECT r.id_reembolso,r.monto FROM dbo.Reembolso r WHERE r.id_devolucion=d.id_devolucion ORDER BY r.id_reembolso FOR JSON PATH) AS reembolsos
    FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES)) AS version_fila
FROM dbo.Devolucion d JOIN dbo.Venta v ON v.id_venta=d.id_venta JOIN dbo.Cliente c ON c.id_cliente=v.id_cliente
JOIN dbo.Tienda t ON t.id_tienda=d.id_tienda JOIN dbo.vw_DevolucionTotal total ON total.id_devolucion=d.id_devolucion
OUTER APPLY(SELECT SUM(monto) AS Monto FROM dbo.Reembolso WHERE id_devolucion=d.id_devolucion) re WHERE d.id_devolucion=@id
```

### Q093 · DevolucionDatos.Obtener · línea 65

Fuente: Datos/DevolucionDatos.cs:65. Recupera la cabecera y los detalles del registro solicitado.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT dv.nombre_producto AS Producto,x.cantidad AS Cantidad,dv.precio_unitario AS Precio,
    x.cantidad*dv.precio_unitario AS Subtotal,x.isv AS ISV,x.cantidad*dv.precio_unitario+x.isv AS Importe,x.reintegra_stock AS Reintegrar
FROM dbo.DetalleDevolucion x JOIN dbo.DetalleVenta dv ON dv.id_detalle_venta=x.id_detalle_venta WHERE x.id_devolucion=@id
```

### Q094 · DevolucionDatos.Obtener · línea 70

Fuente: Datos/DevolucionDatos.cs:70. Recupera la cabecera y los detalles del registro solicitado.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT r.id_reembolso AS Id,r.fecha AS FechaUTC,m.nombre AS Metodo,r.monto AS Monto,r.id_pago_venta AS Pago,r.motivo AS Motivo
FROM dbo.Reembolso r JOIN dbo.MetodoPago m ON m.id_metodo_pago=r.id_metodo_pago WHERE r.id_devolucion=@id ORDER BY r.id_reembolso
```

### Q095 · DevolucionDatos.Bloquear · línea 80

Fuente: Datos/DevolucionDatos.cs:80. Bloquea y compara la versión del registro para evitar operar sobre información obsoleta.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT v.id_venta FROM dbo.Venta v WITH(UPDLOCK,HOLDLOCK) JOIN dbo.Devolucion d ON d.id_venta=v.id_venta WHERE d.id_devolucion=@id;
SELECT id_devolucion FROM dbo.Devolucion WITH(UPDLOCK,HOLDLOCK) WHERE id_devolucion=@id;
```

### Q096 · DevolucionDatos.Bloquear · línea 84

Fuente: Datos/DevolucionDatos.cs:84. Bloquea y compara la versión del registro para evitar operar sobre información obsoleta.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT d.id_devolucion AS Id,v.numero AS Factura,c.nombre AS Cliente,t.nombre AS Tienda,d.fecha AS FechaUTC,
    d.estado AS Estado,d.motivo AS Motivo,total.total AS Total,ISNULL(re.Monto,0) AS Reembolsado,
    total.total-ISNULL(re.Monto,0) AS Pendiente,d.id_venta,HASHBYTES('SHA2_256',(SELECT d.id_devolucion,d.id_venta,d.estado,d.motivo,d.anulado_en,d.motivo_anulacion,
    (SELECT x.id_detalle_devolucion,x.id_detalle_venta,x.cantidad,x.reintegra_stock,x.isv FROM dbo.DetalleDevolucion x
        WHERE x.id_devolucion=d.id_devolucion ORDER BY x.id_detalle_devolucion FOR JSON PATH) AS lineas,
    (SELECT r.id_reembolso,r.monto FROM dbo.Reembolso r WHERE r.id_devolucion=d.id_devolucion ORDER BY r.id_reembolso FOR JSON PATH) AS reembolsos
    FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES)) AS version_fila
FROM dbo.Devolucion d JOIN dbo.Venta v ON v.id_venta=d.id_venta JOIN dbo.Cliente c ON c.id_cliente=v.id_cliente
JOIN dbo.Tienda t ON t.id_tienda=d.id_tienda JOIN dbo.vw_DevolucionTotal total ON total.id_devolucion=d.id_devolucion
OUTER APPLY(SELECT SUM(monto) AS Monto FROM dbo.Reembolso WHERE id_devolucion=d.id_devolucion) re WHERE d.id_devolucion=@id
```

### Q097 · DevolucionDatos.Guardar · línea 97

Fuente: Datos/DevolucionDatos.cs:97. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @venta.

```sql
SELECT id_tienda FROM dbo.Venta WITH(UPDLOCK,HOLDLOCK) WHERE id_venta=@venta AND estado='CONFIRMADA'
```

### Q098 · DevolucionDatos.Guardar · línea 104

Fuente: Datos/DevolucionDatos.cs:104. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id, @motivo.

```sql
DELETE dbo.DetalleDevolucion WHERE id_devolucion=@id; UPDATE dbo.Devolucion SET motivo=@motivo WHERE id_devolucion=@id
```

### Q099 · DevolucionDatos.Guardar · línea 106

Fuente: Datos/DevolucionDatos.cs:106. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @motivo, @tienda, @usuario, @venta.

```sql
INSERT dbo.Devolucion(id_venta,id_tienda,id_usuario,motivo) VALUES(@venta,@tienda,@usuario,@motivo);
SELECT CONVERT(int,SCOPE_IDENTITY());
```

### Q100 · DevolucionDatos.Guardar · línea 111

Fuente: Datos/DevolucionDatos.cs:111. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @cantidad, @detalle, @id, @stock, @venta.

```sql
IF NOT EXISTS(SELECT 1 FROM dbo.DetalleVenta WITH(UPDLOCK,HOLDLOCK) WHERE id_detalle_venta=@detalle AND id_venta=@venta)
    THROW 50001,'Una línea no pertenece a la factura seleccionada.',1;
INSERT dbo.DetalleDevolucion(id_devolucion,id_detalle_venta,cantidad,reintegra_stock,isv)
SELECT @id,@detalle,@cantidad,@stock,
    CASE WHEN calculo.acumulado>ISNULL(prev.isv,0) THEN calculo.acumulado-ISNULL(prev.isv,0) ELSE 0 END
FROM dbo.DetalleVenta dv
OUTER APPLY(SELECT SUM(CONVERT(bigint,x.cantidad)) AS cantidad,SUM(x.isv) AS isv
    FROM dbo.DetalleDevolucion x JOIN dbo.Devolucion d ON d.id_devolucion=x.id_devolucion
    WHERE x.id_detalle_venta=dv.id_detalle_venta AND d.estado<>'ANULADA') prev
CROSS APPLY(VALUES(CONVERT(decimal(18,2),ROUND(dv.isv*(CONVERT(decimal(18,4),ISNULL(prev.cantidad,0))+@cantidad)/dv.cantidad,2)))) calculo(acumulado)
WHERE dv.id_detalle_venta=@detalle;
```

### Q101 · DevolucionDatos.Confirmar · línea 132

Fuente: Datos/DevolucionDatos.cs:132. Valida el documento y lo pasa a confirmado; sus triggers aplican el efecto correspondiente.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
UPDATE dbo.Devolucion SET estado='CONFIRMADA' WHERE id_devolucion=@id
```

### Q102 · DevolucionDatos.Anular · línea 141

Fuente: Datos/DevolucionDatos.cs:141. Anula el documento con actor, fecha y motivo; los triggers verifican si procede el reverso.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id, @motivo, @usuario.

```sql
UPDATE dbo.Devolucion SET estado='ANULADA',anulado_en=SYSUTCDATETIME(),anulado_por=@usuario,motivo_anulacion=@motivo WHERE id_devolucion=@id
```

### Q103 · DevolucionDatos.Reembolsar · línea 154

Fuente: Datos/DevolucionDatos.cs:154. Calcula el saldo reembolsable y registra la devolución del dinero contra el cobro original.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @venta.

```sql
SELECT p.id_pago_venta,p.id_metodo_pago,p.monto-ISNULL((SELECT SUM(monto) FROM dbo.Reembolso r WHERE r.id_pago_venta=p.id_pago_venta),0) AS Disponible
FROM dbo.PagoVenta p WITH(UPDLOCK,HOLDLOCK) WHERE p.id_venta=@venta ORDER BY p.id_pago_venta
```

### Q104 · DevolucionDatos.Reembolsar · línea 161

Fuente: Datos/DevolucionDatos.cs:161. Calcula el saldo reembolsable y registra la devolución del dinero contra el cobro original.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id, @metodo, @monto, @motivo, @pago, @usuario.

```sql
INSERT dbo.Reembolso(id_pago_venta,id_devolucion,monto,id_metodo_pago,id_usuario,motivo)
VALUES(@pago,@id,@monto,@metodo,@usuario,@motivo)
```

## B. Datos/DirectorioDatos.cs

### Q105 · DirectorioDatos.Listar · línea 28

Fuente: Datos/DirectorioDatos.cs:28. Obtiene el listado y aplica los filtros del módulo.

Tipo: plantilla SQL dinámica. Marcadores SQL detectados: @incluir.

```sql
SELECT p.{e.Id} AS Id, p.{e.Nombre} AS Nombre, p.telefono AS Telefono, p.correo AS Correo,
                p.{e.Identificador} AS Identificador, {extras}, p.activo, p.motivo_desactivacion
                FROM dbo.{e.Tabla} p {join} WHERE p.activo=1 OR @incluir=1 ORDER BY p.{e.Nombre}
```

Construcción original en C#:

```csharp
$@"SELECT p.{e.Id} AS Id, p.{e.Nombre} AS Nombre, p.telefono AS Telefono, p.correo AS Correo,
                p.{e.Identificador} AS Identificador, {extras}, p.activo, p.motivo_desactivacion
                FROM dbo.{e.Tabla} p {join} WHERE p.activo=1 OR @incluir=1 ORDER BY p.{e.Nombre}"
```

Selector auxiliar:

```csharp
extras = tipo switch
            {
                TipoDirectorio.Clientes => "p.direccion AS Direccion, CAST(NULL AS nvarchar(150)) AS Contacto, CAST(NULL AS int) AS IdTienda, CAST(NULL AS nvarchar(100)) AS Tienda",
                TipoDirectorio.Proveedores => "p.direccion AS Direccion, p.contacto AS Contacto, CAST(NULL AS int) AS IdTienda, CAST(NULL AS nvarchar(100)) AS Tienda",
                _ => "CAST(NULL AS nvarchar(300)) AS Direccion, CAST(NULL AS nvarchar(150)) AS Contacto, p.id_tienda AS IdTienda, t.nombre AS Tienda"
            }
```

Selector auxiliar:

```csharp
join = tipo == TipoDirectorio.Empleados ? "JOIN dbo.Tienda t ON t.id_tienda=p.id_tienda" : ""
```

### Q106 · DirectorioDatos.Tiendas · línea 40

Fuente: Datos/DirectorioDatos.cs:40. Sentencia o fragmento utilizado en DirectorioDatos.Tiendas. Operaciones presentes: SELECT. Objetos referidos: Tienda.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT id_tienda, nombre FROM dbo.Tienda WHERE activo=1 ORDER BY nombre
```

### Q107 · DirectorioDatos.Guardar · línea 90

Fuente: Datos/DirectorioDatos.cs:90. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: plantilla SQL dinámica. Marcadores SQL detectados: @correo, @id, @identificador, @nombre, @telefono.

```sql
UPDATE dbo.{e.Tabla} SET {e.Nombre}=@nombre, telefono=@telefono, correo=@correo,
                    {e.Identificador}=@identificador, {extraColumna}={extraParametro}, actualizado_en=SYSUTCDATETIME()
                    WHERE {e.Id}=@id AND activo=1; SELECT CONVERT(int, @@ROWCOUNT);
```

Construcción original en C#:

```csharp
$@"UPDATE dbo.{e.Tabla} SET {e.Nombre}=@nombre, telefono=@telefono, correo=@correo,
                    {e.Identificador}=@identificador, {extraColumna}={extraParametro}, actualizado_en=SYSUTCDATETIME()
                    WHERE {e.Id}=@id AND activo=1; SELECT CONVERT(int, @@ROWCOUNT);"
```

Selector auxiliar:

```csharp
extraColumna = tipo == TipoDirectorio.Empleados ? "id_tienda" : "direccion"
```

Selector auxiliar:

```csharp
extraParametro = tipo == TipoDirectorio.Empleados ? "@tienda" : "@direccion"
```

### Q108 · DirectorioDatos.Guardar · línea 93

Fuente: Datos/DirectorioDatos.cs:93. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: plantilla SQL dinámica. Marcadores SQL detectados: @correo, @identificador, @nombre, @telefono.

```sql
INSERT INTO dbo.{e.Tabla}({e.Nombre},telefono,correo,{e.Identificador},{extraColumna})
                    VALUES(@nombre,@telefono,@correo,@identificador,{extraParametro}); SELECT CONVERT(int, @@ROWCOUNT);
```

Construcción original en C#:

```csharp
$@"INSERT INTO dbo.{e.Tabla}({e.Nombre},telefono,correo,{e.Identificador},{extraColumna})
                    VALUES(@nombre,@telefono,@correo,@identificador,{extraParametro}); SELECT CONVERT(int, @@ROWCOUNT);"
```

Selector auxiliar:

```csharp
extraColumna = tipo == TipoDirectorio.Empleados ? "id_tienda" : "direccion"
```

Selector auxiliar:

```csharp
extraParametro = tipo == TipoDirectorio.Empleados ? "@tienda" : "@direccion"
```

### Q109 · DirectorioDatos.Guardar · línea 96

Fuente: Datos/DirectorioDatos.cs:96. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: plantilla SQL dinámica. Marcadores SQL detectados: @id, @tienda.

```sql
IF NOT EXISTS(SELECT 1 FROM dbo.Tienda WHERE id_tienda=@tienda AND activo=1)
                    AND NOT EXISTS(SELECT 1 FROM dbo.Empleado WHERE id_empleado=@id AND id_tienda=@tienda)
                    THROW 50001, 'Selecciona una tienda activa para el empleado.', 1; {sql}
```

Construcción original en C#:

```csharp
@"IF NOT EXISTS(SELECT 1 FROM dbo.Tienda WHERE id_tienda=@tienda AND activo=1)
                    AND NOT EXISTS(SELECT 1 FROM dbo.Empleado WHERE id_empleado=@id AND id_tienda=@tienda)
                    THROW 50001, 'Selecciona una tienda activa para el empleado.', 1; " + sql
```

Selector auxiliar:

```csharp
sql = id.HasValue
                ? $@"UPDATE dbo.{e.Tabla} SET {e.Nombre}=@nombre, telefono=@telefono, correo=@correo,
                    {e.Identificador}=@identificador, {extraColumna}={extraParametro}, actualizado_en=SYSUTCDATETIME()
                    WHERE {e.Id}=@id AND activo=1; SELECT CONVERT(int, @@ROWCOUNT);"
                : $@"INSERT INTO dbo.{e.Tabla}({e.Nombre},telefono,correo,{e.Identificador},{extraColumna})
                    VALUES(@nombre,@telefono,@correo,@identificador,{extraParametro}); SELECT CONVERT(int, @@ROWCOUNT);"
```

### Q110 · DirectorioDatos.CambiarEstado · línea 121

Fuente: Datos/DirectorioDatos.cs:121. Activa o desactiva lógicamente el registro, conservando sus relaciones.

Tipo: fragmento SQL auxiliar. Sin marcadores @ detectados.

```sql
desactivado_en=NULL, desactivado_por=NULL, motivo_desactivacion=NULL
```

### Q111 · DirectorioDatos.CambiarEstado · línea 122

Fuente: Datos/DirectorioDatos.cs:122. Activa o desactiva lógicamente el registro, conservando sus relaciones.

Tipo: fragmento SQL auxiliar. Marcadores SQL detectados: @motivo.

```sql
desactivado_en=SYSUTCDATETIME(), motivo_desactivacion=@motivo
```

### Q112 · DirectorioDatos.CambiarEstado · línea 124

Fuente: Datos/DirectorioDatos.cs:124. Activa o desactiva lógicamente el registro, conservando sus relaciones.

Tipo: plantilla SQL dinámica. Marcadores SQL detectados: @anterior, @id.

```sql
UPDATE dbo.{e.Tabla} SET {valores}, actualizado_en=SYSUTCDATETIME()
                WHERE {e.Id}=@id AND activo=@anterior; SELECT CONVERT(int, @@ROWCOUNT);
```

Construcción original en C#:

```csharp
$@"UPDATE dbo.{e.Tabla} SET {valores}, actualizado_en=SYSUTCDATETIME()
                WHERE {e.Id}=@id AND activo=@anterior; SELECT CONVERT(int, @@ROWCOUNT);"
```

Selector auxiliar:

```csharp
valores = activar
                ? "desactivado_en=NULL, desactivado_por=NULL, motivo_desactivacion=NULL"
                : "desactivado_en=SYSUTCDATETIME(), motivo_desactivacion=@motivo"
```

## B. Datos/FlujoVentaDatos.cs

### Q113 · FlujoVentaDatos.Preparar · línea 37

Fuente: Datos/FlujoVentaDatos.cs:37. Valida dependencias y sesión y establece el contexto del operador para los triggers.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @key, @motivo, @usuario, @value.

```sql
IF (SELECT value_in_use FROM sys.configurations WHERE name='nested triggers')<>1
    OR DATABASEPROPERTYEX(DB_NAME(),'IsRecursiveTriggersEnabled')=1
    THROW 50001,'Se requieren nested triggers habilitado y RECURSIVE_TRIGGERS OFF.',1;
IF EXISTS(SELECT 1 FROM (VALUES('trg_DetalleApartado_reservas'),('trg_DetalleApartado_bloqueo'),
    ('trg_Apartado_cierre'),('trg_Apartado_estado'),('trg_Apartado_cabecera'),('trg_Apartado_alta_baja'),
    ('trg_AbonoApartado_proteger'),('trg_Apartado_auditoria'),('trg_Venta_contabilizar'),('trg_Venta_estado'),
    ('trg_Venta_alta_baja'),('trg_Venta_cabecera'),('trg_Venta_auditoria'),('trg_DetalleVenta_bloqueo'),('trg_PagoVenta_bloqueo'),
    ('trg_DetalleDevolucion_validar'),('trg_DetalleDevolucion_bloqueo'),('trg_Devolucion_estado'),
    ('trg_Devolucion_alta_baja'),('trg_Devolucion_cabecera'),('trg_Devolucion_contabilizar'),('trg_Devolucion_auditoria'),
    ('trg_Reembolso_validar'),('trg_Reembolso_concurrencia'),('trg_Bitacora_AbonoApartado'),('trg_Bitacora_Reembolso'),
    ('trg_MovimientoInventario_validar'),('trg_MovimientoInventario_aplicar'),('trg_MovimientoInventario_documento'),
    ('trg_MovimientoInventario_inmutable')) r(nombre)
    WHERE NOT EXISTS(SELECT 1 FROM sys.triggers WHERE object_id=OBJECT_ID('dbo.'+r.nombre) AND is_disabled=0))
    THROW 50001,'Instala Triggers2.sql y Migraciones/002_bitacora_sistema.sql antes de operar apartados y devoluciones.',1;
EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@usuario;
EXEC sys.sp_set_session_context @key=N'motivo',@value=@motivo;
```

### Q114 · FlujoVentaDatos.ValidarPago · línea 74

Fuente: Datos/FlujoVentaDatos.cs:74. Verifica un método de pago activo e identifica el efectivo.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT codigo FROM dbo.MetodoPago WITH(HOLDLOCK) WHERE id_metodo_pago=@id AND activo=1
```

### Q115 · FlujoVentaDatos.Evento · línea 85

Fuente: Datos/FlujoVentaDatos.cs:85. Añade una anotación funcional a la bitácora dentro de la transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @accion, @entidad, @equipo, @id, @modulo, @texto, @usuario.

```sql
INSERT dbo.Auditoria(id_usuario,modulo,accion,entidad,id_registro,descripcion,equipo)
VALUES(@usuario,@modulo,@accion,@entidad,@id,@texto,@equipo)
```

## B. Datos/InicioDatos.cs

### Q116 · InicioDatos.Obtener · línea 19

Fuente: Datos/InicioDatos.cs:19. Recupera la cabecera y los detalles del registro solicitado.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @desde, @finUtc, @hasta, @inicioUtc, @tienda.

```sql
WITH dias AS (
    SELECT DATEADD(day,n,@desde) AS Fecha FROM (VALUES(0),(1),(2),(3),(4),(5),(6)) d(n)
), ventas AS (
    SELECT CONVERT(date,DATEADD(hour,-6,v.fecha)) AS Fecha,SUM(t.total) AS Total,COUNT_BIG(*) AS Cantidad
    FROM dbo.Venta v JOIN dbo.vw_VentaTotal t ON t.id_venta=v.id_venta
    WHERE v.estado='CONFIRMADA' AND (@tienda IS NULL OR v.id_tienda=@tienda)
        AND v.fecha>=@inicioUtc AND v.fecha<@finUtc
    GROUP BY CONVERT(date,DATEADD(hour,-6,v.fecha))
), compras AS (
    SELECT c.fecha_factura AS Fecha,SUM(t.total) AS Total,COUNT_BIG(*) AS Cantidad
    FROM dbo.Compra c JOIN dbo.vw_CompraTotal t ON t.id_compra=c.id_compra
    WHERE c.estado='CONFIRMADA' AND (@tienda IS NULL OR c.id_tienda=@tienda)
        AND c.fecha_factura>=@desde AND c.fecha_factura<@hasta
    GROUP BY c.fecha_factura
)
SELECT d.Fecha,ISNULL(v.Total,0) AS Ventas,ISNULL(v.Cantidad,0) AS FacturasVenta,
    ISNULL(c.Total,0) AS Compras,ISNULL(c.Cantidad,0) AS FacturasCompra
FROM dias d LEFT JOIN ventas v ON v.Fecha=d.Fecha LEFT JOIN compras c ON c.Fecha=d.Fecha
ORDER BY d.Fecha;

SELECT (SELECT COUNT_BIG(*) FROM dbo.Traspaso
    WHERE estado='PENDIENTE' AND (@tienda IS NULL OR id_tienda_origen=@tienda OR id_tienda_destino=@tienda)) AS TraspasosPendientes,
    (SELECT COUNT_BIG(*) FROM dbo.InventarioTienda i
    JOIN dbo.Tienda t ON t.id_tienda=i.id_tienda JOIN dbo.Producto p ON p.id_producto=i.id_producto
    WHERE t.activo=1 AND p.activo=1 AND i.existencia-i.reservado<=i.stock_minimo
        AND (@tienda IS NULL OR i.id_tienda=@tienda)) AS AlertasStock;
```

## B. Datos/InventarioDatos.cs

### Q117 · InventarioDatos.AlertasStock · línea 17

Fuente: Datos/InventarioDatos.cs:17. Consulta pares producto/tienda activos cuyo disponible es igual o inferior al mínimo.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT i.id_tienda, i.id_producto, t.nombre AS Tienda, p.codigo_barras AS Codigo,
       p.nombre AS Producto, i.existencia AS Existencia, i.reservado AS Reservado,
       i.existencia-i.reservado AS Disponible, i.stock_minimo AS Minimo
FROM dbo.InventarioTienda i
JOIN dbo.Tienda t ON t.id_tienda=i.id_tienda
JOIN dbo.Producto p ON p.id_producto=i.id_producto
WHERE t.activo=1 AND p.activo=1 AND i.existencia-i.reservado<=i.stock_minimo
ORDER BY t.nombre, p.nombre, i.id_producto
```

### Q118 · InventarioDatos.Existencias · línea 38

Fuente: Datos/InventarioDatos.cs:38. Obtiene saldo físico, reservado, disponible y mínimo por producto en la tienda.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @buscar, @tienda.

```sql
SELECT p.id_producto, p.codigo_barras AS Codigo, p.nombre AS Producto,
       ISNULL(i.existencia,0) AS Existencia, ISNULL(i.reservado,0) AS Reservado,
       ISNULL(i.existencia-i.reservado,0) AS Disponible, ISNULL(i.stock_minimo,0) AS Minimo,
       p.activo AS Activo, i.version_fila
FROM dbo.Producto p LEFT JOIN dbo.InventarioTienda i ON i.id_producto=p.id_producto AND i.id_tienda=@tienda
WHERE (p.activo=1 OR i.id_producto IS NOT NULL)
AND (@buscar=N'' OR CHARINDEX(@buscar,p.nombre)>0 OR CHARINDEX(@buscar,p.codigo_barras)>0)
ORDER BY p.nombre
```

### Q119 · InventarioDatos.Movimientos · línea 60

Fuente: Datos/InventarioDatos.cs:60. Consulta el libro de inventario por tienda, texto y fechas UTC.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @buscar, @desde, @hasta, @tienda.

```sql
SELECT m.id_movimiento AS Id, m.fecha AS FechaUTC, p.nombre AS Producto, m.tipo AS Tipo,
       m.cantidad_delta AS Cantidad, u.nombre_usuario AS Usuario, m.motivo AS Motivo,
       dt.id_traspaso AS Traspaso, m.id_movimiento_revertido AS ReversoDe,
       CONVERT(bit,CASE WHEN r.id_movimiento IS NULL THEN 0 ELSE 1 END) AS Revertido
FROM dbo.MovimientoInventario m JOIN dbo.Producto p ON p.id_producto=m.id_producto
JOIN dbo.Usuario u ON u.id_usuario=m.id_usuario
LEFT JOIN dbo.DetalleTraspaso dt ON dt.id_detalle_traspaso=m.id_detalle_traspaso
LEFT JOIN dbo.MovimientoInventario r ON r.id_movimiento_revertido=m.id_movimiento
WHERE m.id_tienda=@tienda AND m.fecha>=@desde AND m.fecha<@hasta
AND (@buscar=N'' OR CHARINDEX(@buscar,p.nombre)>0 OR CHARINDEX(@buscar,p.codigo_barras)>0
     OR CHARINDEX(@buscar,m.tipo)>0 OR CHARINDEX(@buscar,ISNULL(m.motivo,N''))>0)
ORDER BY m.id_movimiento DESC
```

### Q120 · InventarioDatos.GuardarMinimo · línea 90

Fuente: Datos/InventarioDatos.cs:90. Actualiza el umbral de alerta sin registrar entradas o salidas de unidades.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @minimo, @producto, @tienda.

```sql
IF NOT EXISTS(SELECT 1 FROM dbo.Tienda WITH(HOLDLOCK) WHERE id_tienda=@tienda AND activo=1)
    OR NOT EXISTS(SELECT 1 FROM dbo.Producto WITH(HOLDLOCK) WHERE id_producto=@producto AND activo=1)
    THROW 50001, 'La tienda y el producto deben estar activos.', 1;
IF EXISTS(SELECT 1 FROM dbo.InventarioTienda WHERE id_tienda=@tienda AND id_producto=@producto)
    UPDATE dbo.InventarioTienda SET stock_minimo=@minimo WHERE id_tienda=@tienda AND id_producto=@producto;
ELSE INSERT dbo.InventarioTienda(id_tienda,id_producto,stock_minimo) VALUES(@tienda,@producto,@minimo);
```

### Q121 · InventarioDatos.ComprobarVersion · línea 107

Fuente: Datos/InventarioDatos.cs:107. Lee la versión bajo bloqueo y detecta cambios concurrentes.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @producto, @tienda.

```sql
SELECT version_fila FROM dbo.InventarioTienda WITH(UPDLOCK,HOLDLOCK) WHERE id_tienda=@tienda AND id_producto=@producto
```

### Q122 · InventarioDatos.Preparar · línea 118

Fuente: Datos/InventarioDatos.cs:118. Valida dependencias y sesión y establece el contexto del operador para los triggers.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @key, @motivo, @usuario, @value.

```sql
IF EXISTS(SELECT requerido.nombre FROM (VALUES (N'trg_MovimientoInventario_aplicar'),
    (N'trg_MovimientoInventario_validar'),(N'trg_MovimientoInventario_inmutable')) requerido(nombre)
    WHERE NOT EXISTS(SELECT 1 FROM sys.triggers t WHERE t.object_id=OBJECT_ID(N'dbo.'+requerido.nombre) AND t.is_disabled=0))
    THROW 50004, 'Instale Triggers2.sql antes de guardar el mínimo.', 1;
EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@usuario;
EXEC sys.sp_set_session_context @key=N'motivo',@value=@motivo;
```

## B. Datos/ProductoDatos.cs

### Q123 · ProductoDatos.Listar · línea 24

Fuente: Datos/ProductoDatos.cs:24. Obtiene el listado y aplica los filtros del módulo.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @idTienda, @incluirInactivos.

```sql
SELECT p.id_producto, p.codigo_barras, p.nombre,
                                        c.nombre AS categoria,
                                        CASE WHEN @idTienda IS NULL THEN p.stock_actual
                                             ELSE ISNULL(i.existencia, 0) END AS stock_actual,
                                        p.precio_unitario,
                                        p.id_categoria, p.id_proveedor, p.activo, p.motivo_desactivacion
                                 FROM dbo.vw_ProductoStock p
                                 INNER JOIN dbo.Categoria c ON c.id_categoria = p.id_categoria
                                 LEFT JOIN dbo.InventarioTienda i ON i.id_producto = p.id_producto AND i.id_tienda = @idTienda
                                 WHERE p.activo = 1 OR @incluirInactivos = 1
                                 ORDER BY p.nombre
```

### Q124 · ProductoDatos.ExisteCodigo · línea 47

Fuente: Datos/ProductoDatos.cs:47. Sentencia o fragmento utilizado en ProductoDatos.ExisteCodigo. Operaciones presentes: SELECT. Objetos referidos: Producto.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @codigo, @excluirId.

```sql
SELECT COUNT(1) FROM dbo.Producto
                                 WHERE codigo_barras = @codigo
                                   AND (@excluirId IS NULL OR id_producto <> @excluirId)
```

### Q125 · ProductoDatos.Agregar · línea 60

Fuente: Datos/ProductoDatos.cs:60. Sentencia o fragmento utilizado en ProductoDatos.Agregar. Operaciones presentes: INSERT, SELECT. Objetos referidos: Producto.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @codigo, @idCategoria, @idProveedor, @insertados, @nombre, @precio.

```sql
DECLARE @insertados TABLE (id_producto int);
                                 INSERT INTO dbo.Producto
                                    (codigo_barras, nombre, precio_unitario, id_categoria, id_proveedor)
                                 OUTPUT INSERTED.id_producto INTO @insertados
                                 VALUES (@codigo, @nombre, @precio, @idCategoria, @idProveedor);
                                 SELECT id_producto FROM @insertados;
```

### Q126 · ProductoDatos.Editar · línea 75

Fuente: Datos/ProductoDatos.cs:75. Sentencia o fragmento utilizado en ProductoDatos.Editar. Operaciones presentes: UPDATE, SELECT. Objetos referidos: Producto.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @codigo, @id, @idCategoria, @idProveedor, @nombre, @precio.

```sql
UPDATE dbo.Producto
                                 SET codigo_barras = @codigo, nombre = @nombre, precio_unitario = @precio,
                                     id_categoria = @idCategoria, id_proveedor = @idProveedor
                                 WHERE id_producto = @id AND activo = 1;
                                 SELECT CONVERT(int, @@ROWCOUNT);
```

## B. Datos/ProveedorDatos.cs

### Q127 · ProveedorDatos.ListarParaCombo · línea 11

Fuente: Datos/ProveedorDatos.cs:11. Sentencia o fragmento utilizado en ProveedorDatos.ListarParaCombo. Operaciones presentes: SELECT. Objetos referidos: Proveedor.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT id_proveedor, nombre_empresa FROM dbo.Proveedor WHERE activo = 1 ORDER BY nombre_empresa
```

### Q128 · ProveedorDatos.Listar · línea 21

Fuente: Datos/ProveedorDatos.cs:21. Obtiene el listado y aplica los filtros del módulo.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @incluirInactivos.

```sql
SELECT id_proveedor, nombre_empresa, rtn, contacto, telefono, correo, direccion,
                                        activo, motivo_desactivacion
                                 FROM dbo.Proveedor WHERE activo = 1 OR @incluirInactivos = 1 ORDER BY nombre_empresa
```

### Q129 · ProveedorDatos.ExisteRtn · línea 35

Fuente: Datos/ProveedorDatos.cs:35. Sentencia o fragmento utilizado en ProveedorDatos.ExisteRtn. Operaciones presentes: SELECT. Objetos referidos: Proveedor.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id, @rtn.

```sql
SELECT COUNT(*) FROM dbo.Proveedor
                                 WHERE rtn = @rtn AND (@id IS NULL OR id_proveedor <> @id)
```

## B. Datos/SeguridadDatos.cs

### Q130 · SeguridadDatos.Listar · línea 29

Fuente: Datos/SeguridadDatos.cs:29. Obtiene el listado y aplica los filtros del módulo.

Tipo: plantilla SQL dinámica. Marcadores SQL detectados: @inactivos.

```sql
SELECT {id} AS Id, {campos}, activo, motivo_desactivacion, version_fila FROM dbo.{tabla} WHERE activo=1 OR @inactivos=1 ORDER BY Primero
```

Construcción original en C#:

```csharp
$"SELECT {id} AS Id, {campos}, activo, motivo_desactivacion, version_fila FROM dbo.{tabla} WHERE activo=1 OR @inactivos=1 ORDER BY Primero"
```

Selector auxiliar:

```csharp
campos = tipo switch
            {
                TipoSeguridad.Usuarios => "nombre_usuario AS Primero, CONVERT(nvarchar(20),id_empleado) AS Segundo, id_empleado, CAST('' AS nvarchar(300)) AS Descripcion",
                TipoSeguridad.Roles => "codigo AS Primero, nombre AS Segundo, descripcion AS Descripcion",
                _ => "modulo AS Primero, accion AS Segundo, descripcion AS Descripcion"
            }
```

### Q131 · SeguridadDatos.Empleados · línea 39

Fuente: Datos/SeguridadDatos.cs:39. Sentencia o fragmento utilizado en SeguridadDatos.Empleados. Operaciones presentes: SELECT. Objetos referidos: Empleado.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT id_empleado AS Id, nombre AS Nombre, activo FROM dbo.Empleado ORDER BY nombre
```

### Q132 · SeguridadDatos.Asignaciones · línea 55

Fuente: Datos/SeguridadDatos.cs:55. Consulta o guarda las relaciones entre usuarios, roles y permisos.

Tipo: plantilla SQL dinámica. Marcadores SQL detectados: @id.

```sql
SELECT e.{clave} AS Id, {nombre} + CASE WHEN e.activo=0 THEN N' (inactivo)' ELSE N'' END AS Nombre,
       e.activo, CONVERT(bit,CASE WHEN r.activo=1 THEN 1 ELSE 0 END) AS Asignado
FROM dbo.{entidad} e LEFT JOIN dbo.{relacion} r ON r.{clave}=e.{clave} AND r.{propietario}=@id
WHERE e.activo=1 OR r.activo=1 ORDER BY Nombre
```

Construcción original en C#:

```csharp
$"""
                SELECT e.{clave} AS Id, {nombre} + CASE WHEN e.activo=0 THEN N' (inactivo)' ELSE N'' END AS Nombre,
                       e.activo, CONVERT(bit,CASE WHEN r.activo=1 THEN 1 ELSE 0 END) AS Asignado
                FROM dbo.{entidad} e LEFT JOIN dbo.{relacion} r ON r.{clave}=e.{clave} AND r.{propietario}=@id
                WHERE e.activo=1 OR r.activo=1 ORDER BY Nombre
                """
```

Selector auxiliar:

```csharp
clave = usuario ? "id_rol" : "id_permiso"
```

Selector auxiliar:

```csharp
nombre = usuario ? "e.nombre" : "e.codigo"
```

Selector auxiliar:

```csharp
entidad = usuario ? "Rol" : "Permiso"
```

Selector auxiliar:

```csharp
relacion = usuario ? "UsuarioRol" : "RolPermiso"
```

Selector auxiliar:

```csharp
propietario = usuario ? "id_usuario" : "id_rol"
```

### Q133 · SeguridadDatos.Guardar · línea 93

Fuente: Datos/SeguridadDatos.cs:93. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @empleado, @id.

```sql
SELECT COUNT(*) FROM dbo.Empleado e WITH (UPDLOCK,HOLDLOCK) WHERE e.id_empleado=@empleado
AND (e.activo=1 OR EXISTS(SELECT 1 FROM dbo.Usuario u WHERE u.id_usuario=@id AND u.id_empleado=e.id_empleado))
```

### Q134 · SeguridadDatos.Guardar · línea 110

Fuente: Datos/SeguridadDatos.cs:110. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: fragmento SQL auxiliar. Marcadores SQL detectados: @empleado, @hash, @primero.

```sql
nombre_usuario=@primero,id_empleado=@empleado,
contrasena_hash=COALESCE(@hash,contrasena_hash),
debe_cambiar_contrasena=CASE WHEN @hash IS NULL THEN debe_cambiar_contrasena ELSE 1 END,
contrasena_actualizada_en=CASE WHEN @hash IS NULL THEN contrasena_actualizada_en ELSE SYSUTCDATETIME() END
```

### Q135 · SeguridadDatos.Guardar · línea 120

Fuente: Datos/SeguridadDatos.cs:120. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: plantilla SQL dinámica. Marcadores SQL detectados: @id, @version.

```sql
UPDATE dbo.{tabla} SET {cambios},actualizado_en=SYSUTCDATETIME() WHERE {clave}=@id AND activo=1 AND version_fila=@version; SELECT CONVERT(int,@@ROWCOUNT);
```

Construcción original en C#:

```csharp
$"UPDATE dbo.{tabla} SET {cambios},actualizado_en=SYSUTCDATETIME() WHERE {clave}=@id AND activo=1 AND version_fila=@version; SELECT CONVERT(int,@@ROWCOUNT);"
```

Selector auxiliar:

```csharp
cambios = tipo switch
            {
                TipoSeguridad.Usuarios => """
                    nombre_usuario=@primero,id_empleado=@empleado,
                    contrasena_hash=COALESCE(@hash,contrasena_hash),
                    debe_cambiar_contrasena=CASE WHEN @hash IS NULL THEN debe_cambiar_contrasena ELSE 1 END,
                    contrasena_actualizada_en=CASE WHEN @hash IS NULL THEN contrasena_actualizada_en ELSE SYSUTCDATETIME() END
                    """,
                TipoSeguridad.Roles => "codigo=@primero,nombre=@segundo,descripcion=@descripcion",
                _ => "modulo=@primero,accion=@segundo,descripcion=@descripcion"
            }
```

### Q136 · SeguridadDatos.Guardar · línea 121

Fuente: Datos/SeguridadDatos.cs:121. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
INSERT dbo.{tabla}({columnas}) VALUES({valores}); SELECT CONVERT(int,SCOPE_IDENTITY());
```

Construcción original en C#:

```csharp
$"INSERT dbo.{tabla}({columnas}) VALUES({valores}); SELECT CONVERT(int,SCOPE_IDENTITY());"
```

Selector auxiliar:

```csharp
columnas = tipo switch
            {
                TipoSeguridad.Usuarios => "nombre_usuario,id_empleado,contrasena_hash",
                TipoSeguridad.Roles => "codigo,nombre,descripcion",
                _ => "modulo,accion,descripcion"
            }
```

Selector auxiliar:

```csharp
valores = tipo == TipoSeguridad.Usuarios ? "@primero,@empleado,@hash" : "@primero,@segundo,@descripcion"
```

### Q137 · SeguridadDatos.GuardarAsignaciones · línea 145

Fuente: Datos/SeguridadDatos.cs:145. Sentencia o fragmento utilizado en SeguridadDatos.GuardarAsignaciones. Operaciones presentes: SELECT. Su contexto y sus variantes se identifican en la referencia al código.

Tipo: plantilla SQL dinámica. Marcadores SQL detectados: @id.

```sql
SELECT {destino},activo FROM dbo.{tabla} WITH (UPDLOCK,HOLDLOCK) WHERE {propietario}=@id
```

Construcción original en C#:

```csharp
$"SELECT {destino},activo FROM dbo.{tabla} WITH (UPDLOCK,HOLDLOCK) WHERE {propietario}=@id"
```

Selector auxiliar:

```csharp
destino = usuario ? "id_rol" : "id_permiso"
```

Selector auxiliar:

```csharp
tabla = usuario ? "UsuarioRol" : "RolPermiso"
```

Selector auxiliar:

```csharp
propietario = usuario ? "id_usuario" : "id_rol"
```

### Q138 · SeguridadDatos.GuardarAsignaciones · línea 155

Fuente: Datos/SeguridadDatos.cs:155. Sentencia o fragmento utilizado en SeguridadDatos.GuardarAsignaciones. Operaciones presentes: SELECT, THROW, UPDATE, INSERT. Su contexto y sus variantes se identifican en la referencia al código.

Tipo: plantilla SQL dinámica. Marcadores SQL detectados: @activo, @destino, @id.

```sql
IF @activo=1 AND NOT EXISTS(SELECT 1 FROM dbo.{entidad} WITH (UPDLOCK,HOLDLOCK) WHERE {destino}=@destino AND activo=1)
    THROW 50001, 'No se pueden agregar asignaciones a registros inactivos.', 1;
IF EXISTS(SELECT 1 FROM dbo.{tabla} WHERE {propietario}=@id AND {destino}=@destino)
    UPDATE dbo.{tabla} SET activo=@activo,asignado_en=SYSUTCDATETIME(),asignado_por=NULL WHERE {propietario}=@id AND {destino}=@destino;
ELSE
    INSERT dbo.{tabla}({propietario},{destino},activo) VALUES(@id,@destino,@activo);
```

Construcción original en C#:

```csharp
$"""
                    IF @activo=1 AND NOT EXISTS(SELECT 1 FROM dbo.{entidad} WITH (UPDLOCK,HOLDLOCK) WHERE {destino}=@destino AND activo=1)
                        THROW 50001, 'No se pueden agregar asignaciones a registros inactivos.', 1;
                    IF EXISTS(SELECT 1 FROM dbo.{tabla} WHERE {propietario}=@id AND {destino}=@destino)
                        UPDATE dbo.{tabla} SET activo=@activo,asignado_en=SYSUTCDATETIME(),asignado_por=NULL WHERE {propietario}=@id AND {destino}=@destino;
                    ELSE
                        INSERT dbo.{tabla}({propietario},{destino},activo) VALUES(@id,@destino,@activo);
                    """
```

Selector auxiliar:

```csharp
entidad = usuario ? "Rol" : "Permiso"
```

Selector auxiliar:

```csharp
destino = usuario ? "id_rol" : "id_permiso"
```

Selector auxiliar:

```csharp
tabla = usuario ? "UsuarioRol" : "RolPermiso"
```

Selector auxiliar:

```csharp
propietario = usuario ? "id_usuario" : "id_rol"
```

### Q139 · SeguridadDatos.CambiarEstado · línea 175

Fuente: Datos/SeguridadDatos.cs:175. Activa o desactiva lógicamente el registro, conservando sus relaciones.

Tipo: plantilla SQL dinámica. Marcadores SQL detectados: @activar, @id, @motivo, @version.

```sql
UPDATE dbo.{tabla} SET desactivado_en=CASE WHEN @activar=1 THEN NULL ELSE SYSUTCDATETIME() END,
desactivado_por=NULL,motivo_desactivacion=CASE WHEN @activar=1 THEN NULL ELSE @motivo END,
actualizado_en=SYSUTCDATETIME() WHERE {clave}=@id AND version_fila=@version AND activo<>@activar;
SELECT CONVERT(int,@@ROWCOUNT);
```

Construcción original en C#:

```csharp
$"""
                UPDATE dbo.{tabla} SET desactivado_en=CASE WHEN @activar=1 THEN NULL ELSE SYSUTCDATETIME() END,
                desactivado_por=NULL,motivo_desactivacion=CASE WHEN @activar=1 THEN NULL ELSE @motivo END,
                actualizado_en=SYSUTCDATETIME() WHERE {clave}=@id AND version_fila=@version AND activo<>@activar;
                SELECT CONVERT(int,@@ROWCOUNT);
                """
```

## B. Datos/TiendaDatos.cs

### Q140 · TiendaDatos.Listar · línea 12

Fuente: Datos/TiendaDatos.cs:12. Obtiene el listado y aplica los filtros del módulo.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @incluirInactivas.

```sql
SELECT id_tienda, codigo AS Codigo, nombre AS Nombre, direccion AS Direccion, telefono AS Telefono, activo, motivo_desactivacion FROM dbo.Tienda WHERE activo=1 OR @incluirInactivas=1 ORDER BY nombre
```

### Q141 · TiendaDatos.Reactivar · línea 23

Fuente: Datos/TiendaDatos.cs:23. Sentencia o fragmento utilizado en TiendaDatos.Reactivar. Operaciones presentes: UPDATE, SELECT. Objetos referidos: Tienda.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
UPDATE dbo.Tienda SET desactivado_en=NULL, desactivado_por=NULL,
                motivo_desactivacion=NULL, actualizado_en=SYSUTCDATETIME() WHERE id_tienda=@id AND activo=0;
                SELECT CONVERT(int, @@ROWCOUNT);
```

### Q142 · TiendaDatos.Agregar · línea 34

Fuente: Datos/TiendaDatos.cs:34. Sentencia o fragmento utilizado en TiendaDatos.Agregar. Operaciones presentes: INSERT. Objetos referidos: Tienda.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @codigo, @direccion, @nombre, @telefono.

```sql
INSERT INTO dbo.Tienda(codigo,nombre,direccion,telefono) VALUES(@codigo,@nombre,@direccion,@telefono)
```

### Q143 · TiendaDatos.Editar · línea 44

Fuente: Datos/TiendaDatos.cs:44. Sentencia o fragmento utilizado en TiendaDatos.Editar. Operaciones presentes: UPDATE, SELECT. Objetos referidos: Tienda.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @codigo, @direccion, @id, @nombre, @telefono.

```sql
UPDATE dbo.Tienda SET codigo=@codigo, nombre=@nombre,
                direccion=@direccion, telefono=@telefono, actualizado_en=SYSUTCDATETIME()
                WHERE id_tienda=@id AND activo=1; SELECT CONVERT(int, @@ROWCOUNT);
```

## B. Datos/TraspasoDatos.cs

### Q144 · TraspasoDatos.Usuarios · línea 25

Fuente: Datos/TraspasoDatos.cs:25. Sentencia o fragmento utilizado en TraspasoDatos.Usuarios. Operaciones presentes: SELECT. Objetos referidos: Usuario.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT id_usuario, nombre_usuario FROM dbo.Usuario
                                 WHERE activo = 1 ORDER BY nombre_usuario
```

### Q145 · TraspasoDatos.Listar · línea 32

Fuente: Datos/TraspasoDatos.cs:32. Obtiene el listado y aplica los filtros del módulo.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT t.id_traspaso, o.nombre AS Origen, d.nombre AS Destino,
                                        t.estado, u.nombre_usuario AS Solicitante, t.solicitado_en,
                                        t.observaciones, t.id_tienda_origen, t.id_tienda_destino, t.version_fila
                                 FROM dbo.Traspaso t
                                 JOIN dbo.Tienda o ON o.id_tienda = t.id_tienda_origen
                                 JOIN dbo.Tienda d ON d.id_tienda = t.id_tienda_destino
                                 JOIN dbo.Usuario u ON u.id_usuario = t.solicitado_por
                                 ORDER BY t.id_traspaso DESC
```

### Q146 · TraspasoDatos.Detalle · línea 45

Fuente: Datos/TraspasoDatos.cs:45. Consulta las líneas del documento seleccionado.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT d.id_producto, p.nombre AS Producto, d.cantidad, d.precio_referencia,
                                        ISNULL(i.existencia, 0) AS existencia,
                                        ISNULL(i.reservado, 0) AS reservado,
                                        ISNULL(i.existencia - i.reservado, 0) AS disponible
                                 FROM dbo.DetalleTraspaso d
                                 JOIN dbo.Producto p ON p.id_producto = d.id_producto
                                 JOIN dbo.Traspaso t ON t.id_traspaso = d.id_traspaso
                                 LEFT JOIN dbo.InventarioTienda i ON i.id_producto = d.id_producto
                                     AND i.id_tienda = t.id_tienda_origen
                                 WHERE d.id_traspaso = @id
```

### Q147 · TraspasoDatos.Productos · línea 60

Fuente: Datos/TraspasoDatos.cs:60. Consulta los productos disponibles para el módulo y, cuando corresponde, sus saldos por tienda.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id, @incluirInactivos.

```sql
SELECT p.id_producto, p.nombre, p.precio_unitario,
                                        ISNULL(i.existencia, 0) AS existencia,
                                        ISNULL(i.reservado, 0) AS reservado,
                                        ISNULL(i.existencia - i.reservado, 0) AS disponible
                                 FROM dbo.Producto p
                                 LEFT JOIN dbo.InventarioTienda i ON i.id_producto = p.id_producto
                                     AND i.id_tienda = @id
                                 WHERE p.activo = 1 OR @incluirInactivos = 1
                                 ORDER BY p.nombre
```

### Q148 · TraspasoDatos.Historial · línea 81

Fuente: Datos/TraspasoDatos.cs:81. Recupera los eventos históricos del documento.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT e.fecha, e.estado_anterior, e.estado_nuevo,
                                        u.nombre_usuario AS Usuario, e.motivo
                                 FROM dbo.EventoTraspaso e
                                 JOIN dbo.Usuario u ON u.id_usuario = e.id_usuario
                                 WHERE e.id_traspaso = @id ORDER BY e.id_evento_traspaso
```

### Q149 · TraspasoDatos.Guardar · línea 103

Fuente: Datos/TraspasoDatos.cs:103. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @destino, @origen.

```sql
SELECT COUNT(*) FROM dbo.Tienda WITH (HOLDLOCK) WHERE activo=1 AND id_tienda IN (@origen,@destino)
```

### Q150 · TraspasoDatos.Guardar · línea 113

Fuente: Datos/TraspasoDatos.cs:113. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @destino, @id, @origen, @texto, @version.

```sql
UPDATE dbo.Traspaso SET observaciones = @texto
                WHERE id_traspaso=@id AND estado='PENDIENTE' AND version_fila=@version
                    AND id_tienda_origen=@origen AND id_tienda_destino=@destino;
                IF @@ROWCOUNT=0 THROW 50001, 'La solicitud cambió o ya no está pendiente. Actualice la lista.', 1;
                SELECT @id;
```

### Q151 · TraspasoDatos.Guardar · línea 118

Fuente: Datos/TraspasoDatos.cs:118. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @destino, @origen, @texto, @usuario.

```sql
INSERT dbo.Traspaso(id_tienda_origen,id_tienda_destino,solicitado_por,observaciones)
                VALUES(@origen,@destino,@usuario,@texto); SELECT CAST(SCOPE_IDENTITY() AS int);
```

### Q152 · TraspasoDatos.Guardar · línea 129

Fuente: Datos/TraspasoDatos.cs:129. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
DELETE dbo.DetalleTraspaso WHERE id_traspaso=@id
```

### Q153 · TraspasoDatos.Guardar · línea 136

Fuente: Datos/TraspasoDatos.cs:136. Guarda la cabecera o el detalle y verifica las condiciones necesarias para conservar la transacción.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @cantidad, @id, @precio, @producto.

```sql
IF NOT EXISTS(SELECT 1 FROM dbo.Producto WITH(HOLDLOCK) WHERE id_producto=@producto AND activo=1)
                THROW 50002, 'El producto ya no está activo.', 1;
                INSERT dbo.DetalleTraspaso(id_traspaso,id_producto,cantidad,precio_referencia) VALUES(@id,@producto,@cantidad,@precio)
```

### Q154 · TraspasoDatos.ValidarUsuario · línea 154

Fuente: Datos/TraspasoDatos.cs:154. Sentencia o fragmento utilizado en TraspasoDatos.ValidarUsuario. Operaciones presentes: SELECT. Objetos referidos: Usuario.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @usuario.

```sql
SELECT COUNT(*) FROM dbo.Usuario WITH(HOLDLOCK) WHERE id_usuario=@usuario AND activo=1
```

### Q155 · TraspasoDatos.PrepararOperacion · línea 165

Fuente: Datos/TraspasoDatos.cs:165. Verifica sesión y triggers requeridos y transmite usuario y motivo al contexto SQL.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @key, @motivo, @usuario, @value.

```sql
IF NOT EXISTS (SELECT 1 FROM sys.triggers
                                     WHERE object_id = OBJECT_ID(N'dbo.trg_Traspaso_operar') AND is_disabled = 0)
                                     THROW 50004, 'Instale Triggers2.sql antes de operar traspasos.', 1;
                                 IF EXISTS(SELECT requerido.nombre FROM (VALUES
                                     (N'trg_Traspaso_estado'),(N'trg_MovimientoInventario_aplicar'),
                                     (N'trg_MovimientoInventario_validar'),(N'trg_MovimientoInventario_documento')) requerido(nombre)
                                     WHERE NOT EXISTS(SELECT 1 FROM sys.triggers t WHERE t.object_id=OBJECT_ID(N'dbo.'+requerido.nombre) AND t.is_disabled=0))
                                     THROW 50004, 'Instale todos los triggers de inventario y traspasos de Triggers2.sql.', 1;
                                 EXEC sys.sp_set_session_context @key=N'id_usuario', @value=@usuario;
                                 EXEC sys.sp_set_session_context @key=N'motivo', @value=@motivo;
```

## B. Datos/VentaDatos.cs

### Q156 · VentaDatos.Comprobante · línea 34

Fuente: Datos/VentaDatos.cs:34. Obtiene la información histórica necesaria para generar o reimprimir el recibo.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @id.

```sql
SELECT v.numero,v.fecha,v.estado,t.nombre AS tienda,c.nombre AS cliente,v.rtn_cliente,v.cai,u.nombre_usuario,v.observaciones,
    p.monto,p.monto_recibido,p.referencia,m.nombre AS metodo
FROM dbo.Venta v JOIN dbo.Tienda t ON t.id_tienda=v.id_tienda
JOIN dbo.Cliente c ON c.id_cliente=v.id_cliente JOIN dbo.Usuario u ON u.id_usuario=v.id_usuario
JOIN dbo.PagoVenta p ON p.id_venta=v.id_venta JOIN dbo.MetodoPago m ON m.id_metodo_pago=p.id_metodo_pago
WHERE v.id_venta=@id;
SELECT cantidad,nombre_producto,precio_unitario,subtotal,tasa_isv,isv,total_linea FROM dbo.DetalleVenta WHERE id_venta=@id ORDER BY id_detalle_venta;
```

### Q157 · VentaDatos.Productos · línea 81

Fuente: Datos/VentaDatos.cs:81. Consulta los productos disponibles para el módulo y, cuando corresponde, sus saldos por tienda.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @tienda.

```sql
SELECT p.id_producto,p.codigo_barras AS Codigo,p.nombre AS Producto,p.precio_unitario AS Precio,
ISNULL(i.existencia,0) AS Existencia,ISNULL(i.reservado,0) AS Reservado,
ISNULL(i.existencia-i.reservado,0) AS Disponible
FROM dbo.Producto p LEFT JOIN dbo.InventarioTienda i ON i.id_producto=p.id_producto AND i.id_tienda=@tienda
WHERE p.activo=1 ORDER BY p.nombre
```

### Q158 · VentaDatos.Emitir · línea 102

Fuente: Datos/VentaDatos.cs:102. Forma parte de la facturación atómica: valida catálogos, inserta venta, detalle o pago y confirma el documento.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @cliente, @key, @metodo, @tienda, @usuario, @value.

```sql
IF (SELECT value_in_use FROM sys.configurations WHERE name='nested triggers')<>1
    OR DATABASEPROPERTYEX(DB_NAME(),'IsRecursiveTriggersEnabled')=1
    THROW 50001,'Ventas requiere nested triggers habilitado y RECURSIVE_TRIGGERS OFF.',1;
IF EXISTS(SELECT 1 FROM (VALUES ('trg_Venta_estado'),('trg_Venta_cabecera'),('trg_DetalleVenta_bloqueo'),
    ('trg_Venta_contabilizar'),('trg_Venta_auditoria'),('trg_MovimientoInventario_validar'),
    ('trg_MovimientoInventario_documento'),('trg_MovimientoInventario_aplicar'),('trg_MovimientoInventario_inmutable')) r(nombre)
    WHERE NOT EXISTS(SELECT 1 FROM sys.triggers WHERE object_id=OBJECT_ID('dbo.'+r.nombre) AND is_disabled=0))
    THROW 50001,'Instale los triggers de ventas e inventario de Triggers2.sql.',1;
EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@usuario;
EXEC sys.sp_set_session_context @key=N'motivo',@value=N'Facturación de venta';
IF NOT EXISTS(SELECT 1 FROM dbo.Tienda WITH(HOLDLOCK) WHERE id_tienda=@tienda AND activo=1)
    OR NOT EXISTS(SELECT 1 FROM dbo.Cliente WITH(HOLDLOCK) WHERE id_cliente=@cliente AND activo=1)
    OR NOT EXISTS(SELECT 1 FROM dbo.MetodoPago WITH(HOLDLOCK) WHERE id_metodo_pago=@metodo AND activo=1)
    THROW 50001,'La tienda, cliente y método de pago deben estar activos.',1;
SELECT codigo FROM dbo.MetodoPago WHERE id_metodo_pago=@metodo;
```

### Q159 · VentaDatos.Emitir · línea 127

Fuente: Datos/VentaDatos.cs:127. Forma parte de la facturación atómica: valida catálogos, inserta venta, detalle o pago y confirma el documento.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @cliente, @numero, @rtn, @texto, @tienda, @usuario.

```sql
INSERT dbo.Venta(numero,id_tienda,id_cliente,rtn_cliente,cai,id_usuario,observaciones)
VALUES(@numero,@tienda,@cliente,@rtn,(SELECT cai FROM dbo.ConfiguracionFacturacion WITH(HOLDLOCK) WHERE id_tienda=@tienda),@usuario,@texto);
SELECT CONVERT(int,SCOPE_IDENTITY());
```

### Q160 · VentaDatos.Emitir · línea 142

Fuente: Datos/VentaDatos.cs:142. Forma parte de la facturación atómica: valida catálogos, inserta venta, detalle o pago y confirma el documento.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @cantidad, @producto, @tienda.

```sql
SELECT p.precio_unitario FROM dbo.Producto p WITH(HOLDLOCK)
JOIN dbo.InventarioTienda i WITH(UPDLOCK,HOLDLOCK) ON i.id_producto=p.id_producto
WHERE p.id_producto=@producto AND p.activo=1 AND i.id_tienda=@tienda AND i.existencia-i.reservado>=@cantidad
```

### Q161 · VentaDatos.Emitir · línea 155

Fuente: Datos/VentaDatos.cs:155. Forma parte de la facturación atómica: valida catálogos, inserta venta, detalle o pago y confirma el documento.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @cantidad, @producto, @tasa, @venta.

```sql
INSERT dbo.DetalleVenta(id_venta,id_producto,cantidad,precio_unitario,nombre_producto,tasa_isv)
SELECT @venta,id_producto,@cantidad,precio_unitario,nombre,@tasa FROM dbo.Producto WHERE id_producto=@producto;
```

### Q162 · VentaDatos.Emitir · línea 167

Fuente: Datos/VentaDatos.cs:167. Forma parte de la facturación atómica: valida catálogos, inserta venta, detalle o pago y confirma el documento.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @metodo, @recibido, @referencia, @total, @venta.

```sql
INSERT dbo.PagoVenta(id_venta,id_metodo_pago,monto,monto_recibido,referencia) VALUES(@venta,@metodo,@total,@recibido,@referencia);
UPDATE dbo.Venta SET estado='CONFIRMADA' WHERE id_venta=@venta;
```

## B. Formularios/Administracion/ConfiguracionAuditoria.Facturacion.cs

### Q163 · ConfiguracionAuditoria.CargarTiendasCai · línea 56

Fuente: Formularios/Administracion/ConfiguracionAuditoria.Facturacion.cs:56. Sentencia o fragmento utilizado en ConfiguracionAuditoria.CargarTiendasCai. Operaciones presentes: SELECT. Objetos referidos: Tienda.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT id_tienda,nombre FROM dbo.Tienda WHERE activo=1 ORDER BY nombre
```

## B. Formularios/Productos/CargaMasiva.cs

### Q164 · CargaMasiva.OnLoad · línea 142

Fuente: Formularios/Productos/CargaMasiva.cs:142. Sentencia o fragmento utilizado en CargaMasiva.OnLoad. Operaciones presentes: SELECT. Objetos referidos: Tienda.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT id_tienda,nombre FROM dbo.Tienda WHERE activo=1 ORDER BY nombre
```

## B. Formularios/Ventas/ApartadosDevoluciones.Editores.cs

### Q165 · ApartadosDevoluciones.NuevoApartado · línea 17

Fuente: Formularios/Ventas/ApartadosDevoluciones.Editores.cs:17. Sentencia o fragmento utilizado en ApartadosDevoluciones.NuevoApartado. Operaciones presentes: SELECT. Objetos referidos: Tienda.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT id_tienda,nombre FROM dbo.Tienda WHERE activo=1 ORDER BY nombre
```

### Q166 · ApartadosDevoluciones.NuevoApartado · línea 18

Fuente: Formularios/Ventas/ApartadosDevoluciones.Editores.cs:18. Sentencia o fragmento utilizado en ApartadosDevoluciones.NuevoApartado. Operaciones presentes: SELECT. Objetos referidos: Cliente.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT id_cliente,nombre FROM dbo.Cliente WHERE activo=1 ORDER BY nombre
```

### Q167 · CobroDialogo.CobroDialogo · línea 149

Fuente: Formularios/Ventas/ApartadosDevoluciones.Editores.cs:149. Sentencia o fragmento utilizado en CobroDialogo.CobroDialogo. Operaciones presentes: SELECT. Objetos referidos: MetodoPago.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT id_metodo_pago,nombre,codigo FROM dbo.MetodoPago WHERE activo=1 ORDER BY nombre
```

## B. Formularios/Ventas/PuntoVenta.cs

### Q168 · PuntoVenta.Enlazar · línea 98

Fuente: Formularios/Ventas/PuntoVenta.cs:98. Sentencia o fragmento utilizado en PuntoVenta.Enlazar. Operaciones presentes: SELECT. Su contexto y sus variantes se identifican en la referencia al código.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT {id},nombre{extra} FROM dbo.{tabla} WHERE activo=1 ORDER BY nombre
```

Construcción original en C#:

```csharp
$"SELECT {id},nombre{extra} FROM dbo.{tabla} WHERE activo=1 ORDER BY nombre"
```

### Q169 · PuntoVenta.Historial · línea 223

Fuente: Formularios/Ventas/PuntoVenta.cs:223. Recupera los eventos históricos del documento.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT TOP(500) v.id_venta AS Id,v.numero AS Numero,v.fecha AS [Fecha UTC],c.nombre AS Cliente,v.rtn_cliente AS RTN,
    t.nombre AS Tienda,v.estado AS Estado,total.total AS Total
FROM dbo.Venta v JOIN dbo.Cliente c ON c.id_cliente=v.id_cliente
JOIN dbo.Tienda t ON t.id_tienda=v.id_tienda JOIN dbo.vw_VentaTotal total ON total.id_venta=v.id_venta
ORDER BY v.id_venta DESC
```

## B. Pruebas/FlujoVentas.Integracion/Program.cs

### Q170 · Program.Main · línea 28

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:28. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.Main. Operaciones presentes: CREATE. Su contexto y sus variantes se identifican en la referencia al código.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
CREATE DATABASE [CodexERP_Prueba_{Guid.NewGuid().ToString("N")}]
```

Construcción original en C#:

```csharp
"CREATE DATABASE [" + bd + "]"
```

### Q171 · Program.Main · línea 37

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:37. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.Main. Operaciones presentes: INSERT, SELECT. Objetos referidos: Categoria, Cliente, Empleado, MetodoPago, Proveedor, SesionUsuario, Tienda, Usuario.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @c, @cliente, @e, @p, @t, @u.

```sql
SET NOCOUNT ON;
INSERT dbo.Tienda(codigo,nombre) VALUES('PRUEBA','Tienda prueba'); DECLARE @t int=SCOPE_IDENTITY();
INSERT dbo.Categoria(nombre) VALUES(N'Categoría prueba'); DECLARE @c int=SCOPE_IDENTITY();
INSERT dbo.Proveedor(nombre_empresa,rtn) VALUES(N'Proveedor prueba','08019999999999'); DECLARE @p int=SCOPE_IDENTITY();
INSERT dbo.Cliente(nombre) VALUES(N'Cliente prueba'); DECLARE @cliente int=SCOPE_IDENTITY();
INSERT dbo.Empleado(codigo,nombre,id_tienda) VALUES('PRUEBA',N'Empleado prueba',@t); DECLARE @e int=SCOPE_IDENTITY();
INSERT dbo.Usuario(id_empleado,nombre_usuario,contrasena_hash,debe_cambiar_contrasena) VALUES(@e,N'prueba',N'NO-AUTENTICABLE',0); DECLARE @u int=SCOPE_IDENTITY();
INSERT dbo.SesionUsuario(id_usuario,token_hash,version_seguridad,expira_en)
    SELECT @u,CRYPT_GEN_RANDOM(32),version_seguridad,DATEADD(hour,1,SYSUTCDATETIME()) FROM dbo.Usuario WHERE id_usuario=@u;
SELECT @t AS tienda,@cliente AS cliente,@u AS usuario,@c AS categoria,@p AS proveedor,CONVERT(bigint,SCOPE_IDENTITY()) AS sesion,
    (SELECT id_metodo_pago FROM dbo.MetodoPago WHERE codigo='EFECTIVO') AS efectivo,
    (SELECT id_metodo_pago FROM dbo.MetodoPago WHERE codigo='TRANSFERENCIA') AS transferencia;
```

### Q172 · Program.Main · línea 79

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:79. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.Main. Operaciones presentes: ALTER, DROP. Su contexto y sus variantes se identifican en la referencia al código.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
ALTER DATABASE [CodexERP_Prueba_{Guid.NewGuid().ToString("N")}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [CodexERP_Prueba_{Guid.NewGuid().ToString("N")}]
```

Construcción original en C#:

```csharp
"ALTER DATABASE [" + bd + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [" + bd + "]"
```

### Q173 · Program.ProbarImportacion · línea 87

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:87. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarImportacion. Operaciones presentes: SELECT. Objetos referidos: Producto.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT id_producto FROM dbo.Producto WHERE codigo_barras='000123456789'
```

### Q174 · Program.ProbarImportacion · línea 89

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:89. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarImportacion. Operaciones presentes: SELECT. Objetos referidos: Importacion.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT COUNT(*) FROM dbo.Importacion WHERE estado='COMPLETADA'
```

### Q175 · Program.ProbarImportacion · línea 91

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:91. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarImportacion. Operaciones presentes: SELECT. Objetos referidos: Producto.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT COUNT(*) FROM dbo.Producto WHERE codigo_barras='PRIMERO'
```

### Q176 · Program.SimularEsquemaAnterior · línea 99

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:99. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.SimularEsquemaAnterior. Operaciones presentes: ALTER, DROP. Objetos referidos: ConfiguracionFacturacion, DetalleApartado, DetalleDevolucion, DetalleVenta, Venta.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
ALTER TABLE dbo.DetalleVenta DROP CONSTRAINT CK_DetalleVenta_tasa_isv,DF_DetalleVenta_tasa_isv;
ALTER TABLE dbo.DetalleVenta DROP COLUMN total_linea,isv;
ALTER TABLE dbo.DetalleVenta DROP COLUMN tasa_isv;
ALTER TABLE dbo.DetalleApartado DROP CONSTRAINT CK_DetalleApartado_tasa_isv,DF_DetalleApartado_tasa_isv;
ALTER TABLE dbo.DetalleApartado DROP COLUMN total_linea,isv;
ALTER TABLE dbo.DetalleApartado DROP COLUMN tasa_isv;
ALTER TABLE dbo.DetalleDevolucion DROP CONSTRAINT CK_DetalleDevolucion_isv,DF_DetalleDevolucion_isv;
ALTER TABLE dbo.DetalleDevolucion DROP COLUMN isv;
ALTER TABLE dbo.Venta DROP COLUMN cai;
DROP TABLE dbo.ConfiguracionFacturacion;
```

### Q177 · Program.ProbarApartados · línea 119

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:119. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarApartados. Operaciones presentes: SELECT. Objetos referidos: AbonoApartado.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT referencia FROM dbo.AbonoApartado WHERE id_apartado={id}
```

Construcción original en C#:

```csharp
"SELECT referencia FROM dbo.AbonoApartado WHERE id_apartado=" + id
```

Selector auxiliar:

```csharp
id = CrearApartado(2)
```

### Q178 · Program.ProbarApartados · línea 127

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:127. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarApartados. Operaciones presentes: SELECT. Objetos referidos: Apartado.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT estado FROM dbo.Apartado WHERE id_apartado={id}
```

Construcción original en C#:

```csharp
"SELECT estado FROM dbo.Apartado WHERE id_apartado=" + id
```

Selector auxiliar:

```csharp
id = CrearApartado(2)
```

### Q179 · Program.ProbarApartados · línea 128

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:128. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarApartados. Operaciones presentes: SELECT. Objetos referidos: AbonoApartado.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT COUNT(*) FROM dbo.AbonoApartado WHERE id_apartado={id} AND id_pago_venta IS NOT NULL
```

Construcción original en C#:

```csharp
"SELECT COUNT(*) FROM dbo.AbonoApartado WHERE id_apartado=" + id + " AND id_pago_venta IS NOT NULL"
```

Selector auxiliar:

```csharp
id = CrearApartado(2)
```

### Q180 · Program.ProbarApartados · línea 129

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:129. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarApartados. Operaciones presentes: SELECT. Objetos referidos: MetodoPago, PagoVenta.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT p.referencia,m.codigo FROM dbo.PagoVenta p JOIN dbo.MetodoPago m ON m.id_metodo_pago=p.id_metodo_pago WHERE p.id_venta={ventaEntregada}
```

Construcción original en C#:

```csharp
$"SELECT p.referencia,m.codigo FROM dbo.PagoVenta p JOIN dbo.MetodoPago m ON m.id_metodo_pago=p.id_metodo_pago WHERE p.id_venta={ventaEntregada}"
```

### Q181 · Program.ProbarApartados · línea 143

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:143. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarApartados. Operaciones presentes: UPDATE. Objetos referidos: Apartado.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
UPDATE dbo.Apartado SET fecha=DATEADD(day,-2,SYSUTCDATETIME()),vence_en=DATEADD(day,-1,SYSUTCDATETIME()) WHERE id_apartado={vencido}
```

Construcción original en C#:

```csharp
"UPDATE dbo.Apartado SET fecha=DATEADD(day,-2,SYSUTCDATETIME()),vence_en=DATEADD(day,-1,SYSUTCDATETIME()) WHERE id_apartado=" + vencido
```

Selector auxiliar:

```csharp
vencido = CrearApartado(1)
```

### Q182 · Program.ProbarDevoluciones · línea 152

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:152. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarDevoluciones. Operaciones presentes: SELECT. Objetos referidos: DetalleVenta.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT id_detalle_venta FROM dbo.DetalleVenta WHERE id_venta={ventaEntregada}
```

Construcción original en C#:

```csharp
"SELECT id_detalle_venta FROM dbo.DetalleVenta WHERE id_venta=" + ventaEntregada
```

### Q183 · Program.ProbarDevoluciones · línea 165

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:165. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarDevoluciones. Operaciones presentes: SELECT. Objetos referidos: DetalleVenta.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT id_detalle_venta FROM dbo.DetalleVenta WHERE id_venta={venta}
```

Construcción original en C#:

```csharp
"SELECT id_detalle_venta FROM dbo.DetalleVenta WHERE id_venta=" + venta
```

Selector auxiliar:

```csharp
venta = (int)Prop(factura, "Id")
```

### Q184 · Program.ProbarDevoluciones · línea 169

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:169. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarDevoluciones. Operaciones presentes: SELECT. Objetos referidos: Auditoria.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT COUNT(*) FROM dbo.Auditoria WHERE entidad IN('Apartado','Devolucion','AbonoApartado','Reembolso') AND id_usuario IS NULL
```

### Q185 · Program.ProbarPlantilla · línea 192

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:192. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarPlantilla. Operaciones presentes: SELECT. Objetos referidos: InventarioTienda, Producto.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT SUM(i.existencia) FROM dbo.InventarioTienda i JOIN dbo.Producto p ON p.id_producto=i.id_producto WHERE p.codigo_barras LIKE '000990%' AND i.id_tienda={tienda}
```

Construcción original en C#:

```csharp
"SELECT SUM(i.existencia) FROM dbo.InventarioTienda i JOIN dbo.Producto p ON p.id_producto=i.id_producto WHERE p.codigo_barras LIKE '000990%' AND i.id_tienda=" + tienda
```

### Q186 · Program.ProbarRtn · línea 202

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:202. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarRtn. Operaciones presentes: SELECT. Objetos referidos: Venta.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT rtn_cliente FROM dbo.Venta WHERE id_venta={id}
```

Construcción original en C#:

```csharp
"SELECT rtn_cliente FROM dbo.Venta WHERE id_venta=" + id
```

Selector auxiliar:

```csharp
id = (int)Prop(factura, "Id")
```

### Q187 · Program.ProbarRtn · línea 205

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:205. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarRtn. Operaciones presentes: UPDATE. Objetos referidos: Cliente.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
UPDATE dbo.Cliente SET documento_fiscal='08028888888888' WHERE id_cliente={cliente}
```

Construcción original en C#:

```csharp
"UPDATE dbo.Cliente SET documento_fiscal='08028888888888' WHERE id_cliente=" + cliente
```

### Q188 · Program.ProbarRtn · línea 208

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:208. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarRtn. Operaciones presentes: SELECT. Objetos referidos: Venta.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT rtn_cliente FROM dbo.Venta WHERE id_venta={sinId}
```

Construcción original en C#:

```csharp
"SELECT rtn_cliente FROM dbo.Venta WHERE id_venta=" + sinId
```

Selector auxiliar:

```csharp
sinId = (int)Prop(sinRtn, "Id")
```

### Q189 · Program.ProbarRtn · línea 212

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:212. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarRtn. Operaciones presentes: SELECT. Objetos referidos: Venta.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT COUNT(*) FROM dbo.Venta
```

### Q190 · Program.ProbarRtn · línea 214

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:214. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarRtn. Operaciones presentes: SELECT. Objetos referidos: Venta.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT COUNT(*) FROM dbo.Venta
```

### Q191 · Program.ProbarRtn · línea 215

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:215. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarRtn. Operaciones presentes: SELECT. Objetos referidos: Auditoria.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT valores_despues_json FROM dbo.Auditoria WHERE entidad='Venta' AND accion='CREAR' AND id_registro='{id}'
```

Construcción original en C#:

```csharp
"SELECT valores_despues_json FROM dbo.Auditoria WHERE entidad='Venta' AND accion='CREAR' AND id_registro='" + id + "'"
```

Selector auxiliar:

```csharp
id = (int)Prop(factura, "Id")
```

### Q192 · Program.ProbarCasosAdicionales · línea 234

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:234. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarCasosAdicionales. Operaciones presentes: SELECT. Objetos referidos: PagoVenta.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT COUNT(*) FROM dbo.PagoVenta WHERE id_venta={(int)Prop(factura, "Id")}
```

Construcción original en C#:

```csharp
"SELECT COUNT(*) FROM dbo.PagoVenta WHERE id_venta=" + (int)Prop(factura, "Id")
```

### Q193 · Program.ProbarCasosAdicionales · línea 236

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:236. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarCasosAdicionales. Operaciones presentes: SELECT. Objetos referidos: Producto.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT id_producto FROM dbo.Producto WHERE codigo_barras='SIN-STOCK'
```

### Q194 · Program.ProbarCasosAdicionales · línea 237

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:237. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarCasosAdicionales. Operaciones presentes: SELECT. Objetos referidos: Apartado.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT COUNT(*) FROM dbo.Apartado
```

### Q195 · Program.ProbarCasosAdicionales · línea 241

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:241. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarCasosAdicionales. Operaciones presentes: SELECT. Objetos referidos: Apartado.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT COUNT(*) FROM dbo.Apartado
```

### Q196 · Program.ProbarImpuestos · línea 259

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:259. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarImpuestos. Operaciones presentes: SELECT. Objetos referidos: vw_VentaTotal.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT subtotal,isv,total FROM dbo.vw_VentaTotal WHERE id_venta={id}
```

Construcción original en C#:

```csharp
"SELECT subtotal,isv,total FROM dbo.vw_VentaTotal WHERE id_venta=" + id
```

Selector auxiliar:

```csharp
id = (int)Prop(factura, "Id")
```

### Q197 · Program.ProbarImpuestos · línea 270

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:270. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarImpuestos. Operaciones presentes: SELECT. Objetos referidos: Venta.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT COUNT(*) FROM dbo.Venta
```

### Q198 · Program.ProbarImpuestos · línea 272

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:272. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarImpuestos. Operaciones presentes: SELECT. Objetos referidos: Venta.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT COUNT(*) FROM dbo.Venta
```

### Q199 · Program.ProbarImpuestos · línea 274

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:274. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarImpuestos. Operaciones presentes: INSERT, SELECT. Objetos referidos: Tienda.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
INSERT dbo.Tienda(codigo,nombre) VALUES('OTRA','Otra tienda'); SELECT CONVERT(int,SCOPE_IDENTITY())
```

### Q200 · Program.ProbarImpuestos · línea 275

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:275. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarImpuestos. Operaciones presentes: INSERT. Objetos referidos: MovimientoInventario.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,motivo) VALUES({otra},{producto},'AJUSTE',2,{usuario},N'Prueba tienda CAI')
```

Construcción original en C#:

```csharp
$"INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,motivo) VALUES({otra},{producto},'AJUSTE',2,{usuario},N'Prueba tienda CAI')"
```

Selector auxiliar:

```csharp
otra = (int)Tabla("INSERT dbo.Tienda(codigo,nombre) VALUES('OTRA','Otra tienda'); SELECT CONVERT(int,SCOPE_IDENTITY())").Rows[0][0]
```

### Q201 · Program.ProbarImpuestos · línea 278

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:278. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarImpuestos. Operaciones presentes: SELECT. Objetos referidos: Auditoria.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT TOP(1) id_usuario FROM dbo.Auditoria WHERE entidad='ConfiguracionFacturacion' ORDER BY id_auditoria DESC
```

### Q202 · Program.ProbarImpuestos · línea 281

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:281. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarImpuestos. Operaciones presentes: SELECT. Objetos referidos: Producto.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT id_producto FROM dbo.Producto WHERE codigo_barras='REDONDEO'
```

### Q203 · Program.ProbarImpuestos · línea 284

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:284. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarImpuestos. Operaciones presentes: SELECT. Objetos referidos: DetalleVenta.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT id_detalle_venta FROM dbo.DetalleVenta WHERE id_venta={venta}
```

Construcción original en C#:

```csharp
"SELECT id_detalle_venta FROM dbo.DetalleVenta WHERE id_venta=" + venta
```

Selector auxiliar:

```csharp
venta = (int)Prop(pequena, "Id")
```

### Q204 · Program.ProbarReferenciasPago · línea 312

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:312. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarReferenciasPago. Operaciones presentes: SELECT. Objetos referidos: PagoVenta.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT id_pago_venta,referencia FROM dbo.PagoVenta WHERE id_venta={venta}
```

Construcción original en C#:

```csharp
"SELECT id_pago_venta,referencia FROM dbo.PagoVenta WHERE id_venta=" + venta
```

Selector auxiliar:

```csharp
venta = (int)Prop(Vender(efectivo, entrada), "Id")
```

### Q205 · Program.ProbarReferenciasPago · línea 317

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:317. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarReferenciasPago. Operaciones presentes: SELECT. Objetos referidos: Auditoria.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT COUNT(*) FROM dbo.Auditoria WHERE entidad='PagoVenta' AND accion='CREAR' AND TRY_CONVERT(int,JSON_VALUE(id_registro,'$.id_pago_venta'))={pago["id_pago_venta"]} AND id_usuario={usuario}
```

Construcción original en C#:

```csharp
$"SELECT COUNT(*) FROM dbo.Auditoria WHERE entidad='PagoVenta' AND accion='CREAR' AND TRY_CONVERT(int,JSON_VALUE(id_registro,'$.id_pago_venta'))={pago["id_pago_venta"]} AND id_usuario={usuario}"
```

### Q206 · Program.ProbarReferenciasPago · línea 321

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:321. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarReferenciasPago. Operaciones presentes: SELECT. Objetos referidos: PagoVenta.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT referencia FROM dbo.PagoVenta WHERE id_venta={transferenciaId}
```

Construcción original en C#:

```csharp
"SELECT referencia FROM dbo.PagoVenta WHERE id_venta=" + transferenciaId
```

Selector auxiliar:

```csharp
transferenciaId = (int)Prop(Vender(transferencia, "  BANCO-001  "), "Id")
```

### Q207 · Program.ProbarReferenciasPago · línea 322

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:322. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarReferenciasPago. Operaciones presentes: SELECT. Objetos referidos: Venta.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT COUNT(*) FROM dbo.Venta
```

### Q208 · Program.ProbarReferenciasPago · línea 323

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:323. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarReferenciasPago. Operaciones presentes: SELECT. Objetos referidos: PagoVenta.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT COUNT(*) FROM dbo.PagoVenta
```

### Q209 · Program.ProbarReferenciasPago · línea 325

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:325. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarReferenciasPago. Operaciones presentes: SELECT. Objetos referidos: Venta.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT COUNT(*) FROM dbo.Venta
```

### Q210 · Program.ProbarReferenciasPago · línea 326

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:326. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarReferenciasPago. Operaciones presentes: SELECT. Objetos referidos: PagoVenta.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT COUNT(*) FROM dbo.PagoVenta
```

### Q211 · Program.ProbarReferenciasPago · línea 344

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:344. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarReferenciasPago. Operaciones presentes: SELECT. Objetos referidos: MetodoPago.

Tipo: SQL literal o composición estática. Sin marcadores @ detectados.

```sql
SELECT id_metodo_pago,nombre,codigo FROM dbo.MetodoPago WHERE activo=1
```

### Q212 · Program.ProbarListados · línea 367

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:367. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarListados. Operaciones presentes: SELECT. Su contexto y sus variantes se identifican en la referencia al código.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT COUNT(*) FROM dbo.{tabla}
```

Construcción original en C#:

```csharp
"SELECT COUNT(*) FROM dbo." + tabla
```

### Q213 · Program.ProbarExportaciones · línea 481

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:481. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.ProbarExportaciones. Operaciones presentes: SELECT. Objetos referidos: Auditoria.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT COUNT(*) FROM dbo.Auditoria WHERE accion='EXPORTAR' AND id_usuario={usuario}
```

Construcción original en C#:

```csharp
"SELECT COUNT(*) FROM dbo.Auditoria WHERE accion='EXPORTAR' AND id_usuario=" + usuario
```

### Q214 · Program.VersionApartado · línea 492

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:492. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.VersionApartado. Operaciones presentes: SELECT. Objetos referidos: Apartado.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT version_fila FROM dbo.Apartado WHERE id_apartado={id}
```

Construcción original en C#:

```csharp
"SELECT version_fila FROM dbo.Apartado WHERE id_apartado=" + id
```

### Q215 · Program.Stock · línea 495

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:495. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.Stock. Operaciones presentes: SELECT. Objetos referidos: InventarioTienda.

Tipo: plantilla SQL dinámica. Sin marcadores @ detectados.

```sql
SELECT existencia,reservado FROM dbo.InventarioTienda WHERE id_tienda={tienda} AND id_producto={producto}
```

Construcción original en C#:

```csharp
$"SELECT existencia,reservado FROM dbo.InventarioTienda WHERE id_tienda={tienda} AND id_producto={producto}"
```

### Q216 · Program.Ejecutar · línea 501

Fuente: Pruebas/FlujoVentas.Integracion/Program.cs:501. Uso de prueba: prepara, comprueba o limpia la base temporal; no pertenece a la operación normal. Sentencia o fragmento utilizado en Program.Ejecutar. Operaciones presentes: EXEC. Su contexto y sus variantes se identifican en la referencia al código.

Tipo: SQL literal o composición estática. Marcadores SQL detectados: @actor, @key, @value.

```sql
EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@actor
```

## B. Complemento: construcción completa de las plantillas dinámicas

Los métodos siguientes se conservan como C# de referencia para mostrar todos los selectores de tabla, columna y condición utilizados por las plantillas dinámicas de producción. Complementan las entradas anteriores: no son nuevas instrucciones SQL ni se ejecutan desde este documento.

### Construcción: Clases/Borrar.cs · Desactivar

```csharp
private static bool Desactivar(string tabla, string columnaId, int id, string motivo)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            ArgumentException.ThrowIfNullOrWhiteSpace(motivo);
            if (motivo.Trim().Length > 300)
                throw new ArgumentException("El motivo admite hasta 300 caracteres.", nameof(motivo));

            string sql = $@"UPDATE dbo.{tabla}
                            SET desactivado_en = SYSUTCDATETIME(), motivo_desactivacion = @motivo,
                                actualizado_en = SYSUTCDATETIME()
                            WHERE {columnaId} = @id AND activo = 1;
                            SELECT CONVERT(int, @@ROWCOUNT);";

            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
            comando.Parameters.Add("@motivo", SqlDbType.NVarChar, 300).Value = motivo.Trim();
            return (int)comando.ExecuteScalar() > 0;
        }
```

### Construcción: Clases/Modificar.cs · Reactivar

```csharp
private static bool Reactivar(string tabla, string columnaId, int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            string sql = $@"UPDATE dbo.{tabla}
                            SET desactivado_en = NULL, desactivado_por = NULL,
                                motivo_desactivacion = NULL, actualizado_en = SYSUTCDATETIME()
                            WHERE {columnaId} = @id AND activo = 0;
                            SELECT CONVERT(int, @@ROWCOUNT);";

            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
            return (int)comando.ExecuteScalar() > 0;
        }
```

### Construcción: Datos/DirectorioDatos.cs · Entidad

```csharp
private static (string Tabla, string Id, string Nombre, string Identificador) Entidad(TipoDirectorio tipo) => tipo switch
        {
            TipoDirectorio.Clientes => ("Cliente", "id_cliente", "nombre", "documento_fiscal"),
            TipoDirectorio.Proveedores => ("Proveedor", "id_proveedor", "nombre_empresa", "rtn"),
            TipoDirectorio.Empleados => ("Empleado", "id_empleado", "nombre", "codigo"),
            _ => throw new ArgumentOutOfRangeException(nameof(tipo))
        };
```

### Construcción: Datos/DirectorioDatos.cs · Listar

```csharp
public static DataTable Listar(TipoDirectorio tipo, bool incluirInactivos = false)
        {
            var e = Entidad(tipo);
            string extras = tipo switch
            {
                TipoDirectorio.Clientes => "p.direccion AS Direccion, CAST(NULL AS nvarchar(150)) AS Contacto, CAST(NULL AS int) AS IdTienda, CAST(NULL AS nvarchar(100)) AS Tienda",
                TipoDirectorio.Proveedores => "p.direccion AS Direccion, p.contacto AS Contacto, CAST(NULL AS int) AS IdTienda, CAST(NULL AS nvarchar(100)) AS Tienda",
                _ => "CAST(NULL AS nvarchar(300)) AS Direccion, CAST(NULL AS nvarchar(150)) AS Contacto, p.id_tienda AS IdTienda, t.nombre AS Tienda"
            };
            string join = tipo == TipoDirectorio.Empleados ? "JOIN dbo.Tienda t ON t.id_tienda=p.id_tienda" : "";
            string sql = $@"SELECT p.{e.Id} AS Id, p.{e.Nombre} AS Nombre, p.telefono AS Telefono, p.correo AS Correo,
                p.{e.Identificador} AS Identificador, {extras}, p.activo, p.motivo_desactivacion
                FROM dbo.{e.Tabla} p {join} WHERE p.activo=1 OR @incluir=1 ORDER BY p.{e.Nombre}";
            using var conexion = Conexion.AbrirConexion();
            using var adaptador = new SqlDataAdapter(sql, conexion);
            adaptador.SelectCommand.Parameters.Add("@incluir", SqlDbType.Bit).Value = incluirInactivos;
            var tabla = new DataTable(); adaptador.Fill(tabla); return tabla;
        }
```

### Construcción: Datos/DirectorioDatos.cs · Guardar

```csharp
private static bool Guardar(TipoDirectorio tipo, int? id, string nombre, string telefono, string correo,
            string identificador, string direccion, string contacto, int? tienda)
        {
            var e = Entidad(tipo);
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Introduce el nombre.");
            if (nombre.Trim().Length > 150 || telefono.Trim().Length > 30 || correo.Trim().Length > 254 ||
                identificador.Trim().Length > 30 || direccion.Trim().Length > 300 || contacto.Trim().Length > 150)
                throw new ArgumentException("Uno de los campos supera la longitud permitida.");
            if (!string.IsNullOrWhiteSpace(correo) &&
                (!MailAddress.TryCreate(correo.Trim(), out var email) || email.Address != correo.Trim()))
                throw new ArgumentException("Introduce un correo electronico valido.");
            if (tipo == TipoDirectorio.Proveedores)
            {
                var proveedor = new Proveedor
                {
                    IdProveedor = id ?? 0,
                    NombreEmpresa = nombre,
                    Telefono = telefono,
                    Correo = correo,
                    Rtn = identificador,
                    Direccion = direccion,
                    Contacto = contacto
                };
                proveedor.Validar();
                if (id.HasValue) return ProveedorDatos.Editar(proveedor);
                ProveedorDatos.Agregar(proveedor); return true;
            }
            if (tipo == TipoDirectorio.Empleados && (string.IsNullOrWhiteSpace(identificador) || tienda is null || tienda <= 0))
                throw new ArgumentException("Introduce el codigo y selecciona la tienda del empleado.");
            if (tipo == TipoDirectorio.Clientes && identificador.Any(c => c > 127))
                throw new ArgumentException("El documento fiscal debe contener caracteres ASCII.");

            string extraColumna = tipo == TipoDirectorio.Empleados ? "id_tienda" : "direccion";
            string extraParametro = tipo == TipoDirectorio.Empleados ? "@tienda" : "@direccion";
            string sql = id.HasValue
                ? $@"UPDATE dbo.{e.Tabla} SET {e.Nombre}=@nombre, telefono=@telefono, correo=@correo,
                    {e.Identificador}=@identificador, {extraColumna}={extraParametro}, actualizado_en=SYSUTCDATETIME()
                    WHERE {e.Id}=@id AND activo=1; SELECT CONVERT(int, @@ROWCOUNT);"
                : $@"INSERT INTO dbo.{e.Tabla}({e.Nombre},telefono,correo,{e.Identificador},{extraColumna})
                    VALUES(@nombre,@telefono,@correo,@identificador,{extraParametro}); SELECT CONVERT(int, @@ROWCOUNT);";
            if (tipo == TipoDirectorio.Empleados)
                sql = @"IF NOT EXISTS(SELECT 1 FROM dbo.Tienda WHERE id_tienda=@tienda AND activo=1)
                    AND NOT EXISTS(SELECT 1 FROM dbo.Empleado WHERE id_empleado=@id AND id_tienda=@tienda)
                    THROW 50001, 'Selecciona una tienda activa para el empleado.', 1; " + sql;
            using var conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            foreach (var (campo, largo, valor) in new[] { ("@nombre",150,nombre), ("@telefono",30,telefono),
                ("@correo",254,correo), ("@identificador",30,identificador), ("@direccion",300,direccion) })
                comando.Parameters.Add(campo, campo == "@identificador" && tipo == TipoDirectorio.Clientes ? SqlDbType.VarChar : SqlDbType.NVarChar, largo).Value =
                    string.IsNullOrWhiteSpace(valor) ? DBNull.Value : valor.Trim();
            comando.Parameters.Add("@tienda", SqlDbType.Int).Value = (object?)tienda ?? DBNull.Value;
            comando.Parameters.Add("@id", SqlDbType.Int).Value = (object?)id ?? DBNull.Value;
            return (int)comando.ExecuteScalar() > 0;
        }
```

### Construcción: Datos/DirectorioDatos.cs · CambiarEstado

```csharp
private static bool CambiarEstado(TipoDirectorio tipo, int id, bool activar, string motivo)
        {
            var e = Entidad(tipo);
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (!activar && (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length > 300))
                throw new ArgumentException("Introduce un motivo de entre 1 y 300 caracteres.");
            string valores = activar
                ? "desactivado_en=NULL, desactivado_por=NULL, motivo_desactivacion=NULL"
                : "desactivado_en=SYSUTCDATETIME(), motivo_desactivacion=@motivo";
            using var conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand($@"UPDATE dbo.{e.Tabla} SET {valores}, actualizado_en=SYSUTCDATETIME()
                WHERE {e.Id}=@id AND activo=@anterior; SELECT CONVERT(int, @@ROWCOUNT);", conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
            comando.Parameters.Add("@anterior", SqlDbType.Bit).Value = !activar;
            comando.Parameters.Add("@motivo", SqlDbType.NVarChar, 300).Value = motivo.Trim();
            return (int)comando.ExecuteScalar() > 0;
        }
```

### Construcción: Datos/SeguridadDatos.cs · Entidad

```csharp
private static (string Tabla, string Id) Entidad(TipoSeguridad tipo) => tipo switch
        {
            TipoSeguridad.Usuarios => ("Usuario", "id_usuario"),
            TipoSeguridad.Roles => ("Rol", "id_rol"),
            TipoSeguridad.Permisos => ("Permiso", "id_permiso"),
            _ => throw new ArgumentOutOfRangeException(nameof(tipo))
        };
```

### Construcción: Datos/SeguridadDatos.cs · Listar

```csharp
public static DataTable Listar(TipoSeguridad tipo, bool inactivos)
        {
            var (tabla, id) = Entidad(tipo);
            string campos = tipo switch
            {
                TipoSeguridad.Usuarios => "nombre_usuario AS Primero, CONVERT(nvarchar(20),id_empleado) AS Segundo, id_empleado, CAST('' AS nvarchar(300)) AS Descripcion",
                TipoSeguridad.Roles => "codigo AS Primero, nombre AS Segundo, descripcion AS Descripcion",
                _ => "modulo AS Primero, accion AS Segundo, descripcion AS Descripcion"
            };
            using var conexion = Conexion.AbrirConexion();
            using var adaptador = new SqlDataAdapter($"SELECT {id} AS Id, {campos}, activo, motivo_desactivacion, version_fila FROM dbo.{tabla} WHERE activo=1 OR @inactivos=1 ORDER BY Primero", conexion);
            adaptador.SelectCommand.Parameters.Add("@inactivos", SqlDbType.Bit).Value = inactivos;
            var resultado = new DataTable();
            adaptador.Fill(resultado);
            return resultado;
        }
```

### Construcción: Datos/SeguridadDatos.cs · Asignaciones

```csharp
public static DataTable Asignaciones(TipoSeguridad tipo, int? id)
        {
            if (tipo == TipoSeguridad.Permisos) return new DataTable();
            bool usuario = tipo == TipoSeguridad.Usuarios;
            string entidad = usuario ? "Rol" : "Permiso";
            string clave = usuario ? "id_rol" : "id_permiso";
            string relacion = usuario ? "UsuarioRol" : "RolPermiso";
            string propietario = usuario ? "id_usuario" : "id_rol";
            string nombre = usuario ? "e.nombre" : "e.codigo";
            using var conexion = Conexion.AbrirConexion();
            using var adaptador = new SqlDataAdapter($"""
                SELECT e.{clave} AS Id, {nombre} + CASE WHEN e.activo=0 THEN N' (inactivo)' ELSE N'' END AS Nombre,
                       e.activo, CONVERT(bit,CASE WHEN r.activo=1 THEN 1 ELSE 0 END) AS Asignado
                FROM dbo.{entidad} e LEFT JOIN dbo.{relacion} r ON r.{clave}=e.{clave} AND r.{propietario}=@id
                WHERE e.activo=1 OR r.activo=1 ORDER BY Nombre
                """, conexion);
            adaptador.SelectCommand.Parameters.Add("@id", SqlDbType.Int).Value = (object?)id ?? DBNull.Value;
            var tabla = new DataTable();
            adaptador.Fill(tabla);
            return tabla;
        }
```

### Construcción: Datos/SeguridadDatos.cs · Guardar

```csharp
public static void Guardar(TipoSeguridad tipo, int? id, byte[]? version, string primero,
            string segundo, string descripcion, int? empleado, string contrasena, int[] asignaciones)
        {
            var (tabla, clave) = Entidad(tipo);
            primero = primero.Trim(); segundo = segundo.Trim(); descripcion = descripcion.Trim();
            ValidarTexto(primero, tipo == TipoSeguridad.Roles ? 30 : 50, "El primer campo", true, tipo != TipoSeguridad.Usuarios);
            if (tipo != TipoSeguridad.Usuarios)
            {
                ValidarTexto(segundo, tipo == TipoSeguridad.Roles ? 80 : 30, "El segundo campo", true, tipo == TipoSeguridad.Permisos);
                ValidarTexto(descripcion, 300, "La descripción", tipo == TipoSeguridad.Permisos);
            }
            string? hash = null;
            if (tipo == TipoSeguridad.Usuarios)
            {
                if (empleado is null or <= 0) throw new ArgumentException("Selecciona un empleado.");
                if (!id.HasValue || contrasena.Length > 0)
                {
                    if (contrasena.Length < 12 || contrasena.Length > 128)
                        throw new ArgumentException("La contraseña debe tener entre 12 y 128 caracteres.");
                    hash = ClaveSegura.Crear(contrasena);
                }
            }
            using var conexion = Conexion.AbrirConexion();
            using var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
            if (tipo == TipoSeguridad.Usuarios)
            {
                using var comprobar = new SqlCommand("""
                    SELECT COUNT(*) FROM dbo.Empleado e WITH (UPDLOCK,HOLDLOCK) WHERE e.id_empleado=@empleado
                    AND (e.activo=1 OR EXISTS(SELECT 1 FROM dbo.Usuario u WHERE u.id_usuario=@id AND u.id_empleado=e.id_empleado))
                    """, conexion, transaccion);
                comprobar.Parameters.Add("@empleado", SqlDbType.Int).Value = empleado!.Value;
                comprobar.Parameters.Add("@id", SqlDbType.Int).Value = (object?)id ?? DBNull.Value;
                if (Convert.ToInt32(comprobar.ExecuteScalar()) != 1) throw new ArgumentException("Selecciona un empleado activo para una nueva asignación.");
            }
            string columnas = tipo switch
            {
                TipoSeguridad.Usuarios => "nombre_usuario,id_empleado,contrasena_hash",
                TipoSeguridad.Roles => "codigo,nombre,descripcion",
                _ => "modulo,accion,descripcion"
            };
            string valores = tipo == TipoSeguridad.Usuarios ? "@primero,@empleado,@hash" : "@primero,@segundo,@descripcion";
            string cambios = tipo switch
            {
                TipoSeguridad.Usuarios => """
                    nombre_usuario=@primero,id_empleado=@empleado,
                    contrasena_hash=COALESCE(@hash,contrasena_hash),
                    debe_cambiar_contrasena=CASE WHEN @hash IS NULL THEN debe_cambiar_contrasena ELSE 1 END,
                    contrasena_actualizada_en=CASE WHEN @hash IS NULL THEN contrasena_actualizada_en ELSE SYSUTCDATETIME() END
                    """,
                TipoSeguridad.Roles => "codigo=@primero,nombre=@segundo,descripcion=@descripcion",
                _ => "modulo=@primero,accion=@segundo,descripcion=@descripcion"
            };
            using var comando = new SqlCommand(id.HasValue
                ? $"UPDATE dbo.{tabla} SET {cambios},actualizado_en=SYSUTCDATETIME() WHERE {clave}=@id AND activo=1 AND version_fila=@version; SELECT CONVERT(int,@@ROWCOUNT);"
                : $"INSERT dbo.{tabla}({columnas}) VALUES({valores}); SELECT CONVERT(int,SCOPE_IDENTITY());", conexion, transaccion);
            comando.Parameters.Add("@primero", tipo == TipoSeguridad.Usuarios ? SqlDbType.NVarChar : SqlDbType.VarChar, tipo == TipoSeguridad.Roles ? 30 : 50).Value = primero;
            comando.Parameters.Add("@segundo", tipo == TipoSeguridad.Permisos ? SqlDbType.VarChar : SqlDbType.NVarChar, tipo == TipoSeguridad.Roles ? 80 : 30).Value = segundo;
            comando.Parameters.Add("@descripcion", SqlDbType.NVarChar, 300).Value = descripcion;
            comando.Parameters.Add("@empleado", SqlDbType.Int).Value = (object?)empleado ?? DBNull.Value;
            comando.Parameters.Add("@hash", SqlDbType.NVarChar, 512).Value = (object?)hash ?? DBNull.Value;
            comando.Parameters.Add("@id", SqlDbType.Int).Value = (object?)id ?? DBNull.Value;
            comando.Parameters.Add("@version", SqlDbType.Timestamp, 8).Value = (object?)version ?? DBNull.Value;
            int resultado = Convert.ToInt32(comando.ExecuteScalar());
            if (id.HasValue && resultado != 1) throw new InvalidOperationException("El registro cambió o fue desactivado. Recarga la pestaña antes de continuar.");
            int registro = id ?? resultado;
            if (tipo != TipoSeguridad.Permisos) GuardarAsignaciones(conexion, transaccion, tipo, registro, asignaciones);
            transaccion.Commit();
        }
```

### Construcción: Datos/SeguridadDatos.cs · GuardarAsignaciones

```csharp
private static void GuardarAsignaciones(SqlConnection conexion, SqlTransaction transaccion, TipoSeguridad tipo, int id, int[] seleccion)
        {
            bool usuario = tipo == TipoSeguridad.Usuarios;
            string tabla = usuario ? "UsuarioRol" : "RolPermiso";
            string propietario = usuario ? "id_usuario" : "id_rol";
            string destino = usuario ? "id_rol" : "id_permiso";
            string entidad = usuario ? "Rol" : "Permiso";
            var seleccionados = seleccion.ToHashSet();
            var existentes = new Dictionary<int, bool>();
            using (var leer = new SqlCommand($"SELECT {destino},activo FROM dbo.{tabla} WITH (UPDLOCK,HOLDLOCK) WHERE {propietario}=@id", conexion, transaccion))
            {
                leer.Parameters.Add("@id", SqlDbType.Int).Value = id;
                using var lector = leer.ExecuteReader();
                while (lector.Read()) existentes.Add(lector.GetInt32(0), lector.GetBoolean(1));
            }
            foreach (int relacionado in existentes.Keys.Union(seleccionados))
            {
                bool activar = seleccionados.Contains(relacionado);
                if (existentes.TryGetValue(relacionado, out bool previo) && previo == activar) continue;
                using var comando = new SqlCommand($"""
                    IF @activo=1 AND NOT EXISTS(SELECT 1 FROM dbo.{entidad} WITH (UPDLOCK,HOLDLOCK) WHERE {destino}=@destino AND activo=1)
                        THROW 50001, 'No se pueden agregar asignaciones a registros inactivos.', 1;
                    IF EXISTS(SELECT 1 FROM dbo.{tabla} WHERE {propietario}=@id AND {destino}=@destino)
                        UPDATE dbo.{tabla} SET activo=@activo,asignado_en=SYSUTCDATETIME(),asignado_por=NULL WHERE {propietario}=@id AND {destino}=@destino;
                    ELSE
                        INSERT dbo.{tabla}({propietario},{destino},activo) VALUES(@id,@destino,@activo);
                    """, conexion, transaccion);
                comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
                comando.Parameters.Add("@destino", SqlDbType.Int).Value = relacionado;
                comando.Parameters.Add("@activo", SqlDbType.Bit).Value = activar;
                comando.ExecuteNonQuery();
            }
        }
```

### Construcción: Datos/SeguridadDatos.cs · CambiarEstado

```csharp
public static void CambiarEstado(TipoSeguridad tipo, int id, byte[] version, bool activar, string motivo)
        {
            if (!activar) ValidarTexto(motivo.Trim(), 300, "El motivo", true);
            var (tabla, clave) = Entidad(tipo);
            using var conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand($"""
                UPDATE dbo.{tabla} SET desactivado_en=CASE WHEN @activar=1 THEN NULL ELSE SYSUTCDATETIME() END,
                desactivado_por=NULL,motivo_desactivacion=CASE WHEN @activar=1 THEN NULL ELSE @motivo END,
                actualizado_en=SYSUTCDATETIME() WHERE {clave}=@id AND version_fila=@version AND activo<>@activar;
                SELECT CONVERT(int,@@ROWCOUNT);
                """, conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
            comando.Parameters.Add("@version", SqlDbType.Timestamp, 8).Value = version;
            comando.Parameters.Add("@activar", SqlDbType.Bit).Value = activar;
            comando.Parameters.Add("@motivo", SqlDbType.NVarChar, 300).Value = motivo.Trim();
            if (Convert.ToInt32(comando.ExecuteScalar()) != 1)
                throw new InvalidOperationException("El registro cambió. Recarga la pestaña antes de continuar.");
        }
```

### Construcción: Formularios/Ventas/PuntoVenta.cs · Cargar

```csharp
private void Cargar()
    {
        try
        {
            void Enlazar(ComboBox combo, string tabla, string id, string extra = "")
            { combo.DisplayMember = "nombre"; combo.ValueMember = id; combo.DataSource = VentaDatos.Consultar($"SELECT {id},nombre{extra} FROM dbo.{tabla} WHERE activo=1 ORDER BY nombre"); }
            Enlazar(cliente, "Cliente", "id_cliente", ",documento_fiscal"); Enlazar(metodo, "MetodoPago", "id_metodo_pago", ",codigo"); Enlazar(tienda, "Tienda", "id_tienda"); CargarProductos();
        }
        catch (Exception ex) { estado.Text = "No se pudieron cargar los catálogos."; MessageBox.Show(this, ex.Message, "Facturación", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
```

# Anexo C. Guías de verificación existentes

Se reproducen las guías locales como evidencia de qué se diseñó para comprobar y qué pruebas constan registradas. Las afirmaciones de verificaciones previas pertenecen a esos archivos y a su momento de edición; no indican que se hayan vuelto a ejecutar al generar este manual.

## C. Datos/ApartadosDevoluciones-verificacion.md

### Apartados y devoluciones

La sección tiene dos pestañas conectadas a SQL Server, con búsqueda, filtro por estado, consulta de detalles y actualización al regresar al módulo. Usa el esquema 3FN, `Base de Datos/Triggers2.sql` y `Datos/Migraciones/002_bitacora_sistema.sql`, ya utilizados por facturación. No requiere reiniciar la base ni eliminar datos.

#### Apartados

1. Crear un apartado seleccionando tienda, cliente, vencimiento (hora de Honduras) y productos. Antes de guardar se pueden agregar y quitar líneas. El guardado reserva las unidades disponibles; valida precios y stock nuevamente en la transacción.
2. Registrar abonos hasta cubrir el saldo. En efectivo, la referencia se genera automáticamente al guardar el abono o cobrar el saldo de una entrega. Para métodos distintos de efectivo se solicita una referencia manual. No se aceptan abonos sobre el total ni sobre apartados vencidos o cerrados.
3. Entregar y facturar cobra el saldo pendiente, transforma cada abono en un pago de la factura y genera el pago final si corresponde. También admite apartados totalmente abonados. La confirmación de la venta libera la reserva y descuenta la existencia una sola vez. La factura aparece en el historial de facturación, donde puede reimprimirse.
4. Cancelar o marcar vencido libera la reserva. Vencer exige que ya haya llegado la fecha. Se solicita motivo y se registra en auditoría. Los abonos no se eliminan: Reembolsar abonos registra el importe pendiente por cada método original. Un segundo intento no duplica los reembolsos.

Ver detalle muestra resumen, productos, abonos y reembolsos. Las fechas de los listados y detalles se identifican como UTC. Las cantidades y precios quedan fijados al guardar el apartado; si se necesita una reserva diferente, se cancela la anterior y se crea otra, gestionando sus abonos por separado.

#### Devoluciones

1. Elegir una factura confirmada y especificar un motivo.
2. En Devolver escribir la cantidad por línea (cero omite la línea). Disponible descuenta otras devoluciones no anuladas; no se puede superar lo vendido. Reintegrar indica si las unidades vuelven físicamente al inventario.
3. Guardar el borrador. Puede editarse antes de confirmarlo, conservando su factura original.
4. Confirmar aplica el inventario de las líneas marcadas. Reembolsar registra el saldo completo pendiente contra los pagos originales, distribuyéndolo cuando hubo varios métodos. Los acumulados nunca exceden el importe devuelto ni los cobros originales.
5. Anular revierte el inventario de una devolución sin reembolsos. La base rechaza anulaciones que dejen stock insuficiente o que afecten una devolución ya reembolsada.

Los reembolsos son registros contables del sistema; no hay integración con bancos o pasarelas. Las mutaciones validan la sesión y usan transacciones serializables y bloqueos. Se comprueba la versión del apartado o la huella del documento de devolución antes de operar. La devolución incluye sus detalles y reembolsos en la huella para rechazar una ventana desactualizada.

#### Prueba de integración

```powershell
dotnet run --project Pruebas/FlujoVentas.Integracion/FlujoVentas.Integracion.csproj
```

El ejecutable obtiene la conexión de la configuración habitual. Requiere permiso para crear una base temporal en ese servidor. Crea una base con un nombre aleatorio `CodexERP_Prueba_<guid>`, instala el esquema y triggers, ejecuta los métodos reales contra datos propios y elimina únicamente esa base al terminar. No modifica documentos de la base configurada. No debe confundirse con los scripts de reinicio del proyecto.

Verificado:

- Importación de productos, stock inicial por tienda, RTN, historial y reversión completa de un lote con duplicados.
- Reserva, varios abonos, sobrepago rechazado, entrega y factura con abonos enlazados y cambio correcto.
- Cancelación, vencimiento, rechazo de abonos vencidos y reembolso de los abonos una sola vez.
- Entrega con saldo cero y reversión de una reserva con varios productos cuando uno carece de stock.
- Devoluciones parciales, reintegro opcional, edición del borrador y rechazo de versiones desactualizadas.
- Listados completos de apartados y devoluciones, búsqueda por cliente, filtros por cada estado y búsquedas sin coincidencias. Se añadió un salto de línea entre la consulta base y `WHERE` para corregir `Incorrect syntax near '@estado'`, provocado por la concatenación `reWHERE`.
- Límite acumulado de unidades devueltas, reembolsos distribuidos entre pagos, rechazo de doble reembolso y anulación con reverso de stock.
- Atribución de auditoría, lectura y validación de los diez ejemplos XLSX por el importador real y carga de sus 35 unidades de stock en la base temporal.
- Construcción y disposición de las pantallas. Compilación sin errores ni advertencias.
- Apertura real en un contenedor MDI, cambio de pestañas, recarga conservando selección y búsquedas vacías. Las columnas de presentación se crean antes de enlazar los datos: `version_fila` permanece en el origen para comprobar concurrencia y no se genera como columna de imagen. Esto evita la referencia nula al ajustar anchos y el intento de convertir la versión binaria en una imagen, también en el resumen de los detalles.


## C. Datos/Bitacora-verificacion.md

### Bitácora de toda la aplicación

La captura se realiza en SQL Server para las 37 tablas operativas, además del registro de navegación de todas las pantallas desde `Pagina_Central.MostrarPantalla` y la apertura de Notificaciones. Cada conexión de la aplicación establece el usuario actual y limpia el motivo anterior. El cambio inicial de contraseña y el cierre de sesión establecen explícitamente el usuario del acceso, incluso cuando la sesión global todavía no existe o ya se ha limpiado.

#### Cobertura

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

#### Prueba integral ejecutada

Ejecutar `Pruebas/Bitacora-sistema.sql` en la base configurada después del esquema, Triggers2.sql y la migración 002. Requiere un usuario activo y permisos para las operaciones de prueba. Desde PowerShell, leer el SQL con `Get-Content -Raw -Encoding UTF8`.

La prueba crea sus propios catálogos y documentos dentro de una transacción, confirma una compra y una venta, recorre un traspaso hasta recibido, registra un apartado con abono, una devolución con reembolso, inversiones, importaciones y notificaciones. Comprueba modificaciones y eliminaciones fuera de ventas, los cuatro resultados de acceso, revocación de sesiones y atribución al administrador. Exige un evento real con el actor correcto para cada tabla operativa y comprueba que no se copie la credencial de prueba en los JSON. También comprueba que todos los triggers de captura estén habilitados.

Resultado local: **37 de 37 entidades con eventos verificados**. La transacción se revirtió completa; no quedan registros de negocio de prueba. SQL Server puede consumir identidades aunque la transacción se revierta. Compilación .NET: cero errores y cero advertencias.

Después de incorporar el CAI se amplió y ejecutó de nuevo la prueba: **38 de 38 entidades con eventos verificados**, incluida ConfiguracionFacturacion. Se revirtió completa.


## C. Datos/Compras-verificacion.md

### Verificacion de compras y detalles

#### Preparacion

Usar una base de pruebas con `Base de Datos/01_esquema_erp_3fn.sql` instalado y la conexion descrita en README. El esquema de creacion es para una base vacia; no ejecutarlo sobre una base existente con datos. Instalar `Base de Datos/Triggers2.sql` en la misma base, con nested triggers habilitado y RECURSIVE_TRIGGERS deshabilitado. La aplicacion no instala ni modifica el esquema.

Iniciar sesion con usuario y empleado activos. Preparar proveedor, tienda y dos productos activos mediante los CRUD existentes. El costo de compra es independiente del precio de venta del catalogo: `DetalleCompra.costo_unitario` requiere decimal(18,2) positivo; el esquema no incluye impuestos ni descuentos de compra.

#### Casos funcionales contra SQL Server pendientes

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

#### Verificacion local

Un ejecutable temporal sobre .NET 10 para Windows realizo **67 comprobaciones aprobadas**, sin conectarse a SQL Server ni consultar credenciales. Se verificaron construccion del formulario, pestanas, columnas y fechas iniciales; captura, actualizacion y retiro de lineas; subtotales y total; costos independientes del precio de venta; rechazo de duplicados, cantidades invalidas y desbordamientos; longitudes y campos obligatorios; estados de botones para compras nuevas, borradores, confirmadas y anuladas; bloqueo de resolucion con cambios pendientes; referencias historicas inactivas y limpieza mediante Nuevo; y lectura de costos en culturas es-HN y es-ES.

Tambien se renderizo la captura a 1180 x 900 y 1000 x 700 para revisar distribucion y desplazamiento. Se comprobo que el identificador interno del producto permanece oculto despues de crear los controles y enlazar sus datos. La construccion y la compilacion del ejecutable temporal finalizaron correctamente.

Estas comprobaciones no ejecutaron transacciones, triggers ni cambios de inventario. No se automatizaron respuestas a dialogos modales. Los casos anteriores requieren una base de pruebas operativa y la comprobacion interactiva del flujo MDI; permanecen pendientes.

#### Limites

La autorizacion granular por permiso y tienda sigue pendiente en el sistema. Las compras usan la identidad de la sesion autenticada; no hay selector manual de operador. No se implementan recepciones parciales, impuestos ni pagos a proveedores porque no forman parte del flujo de Compra del esquema actual.


## C. Datos/Exportaciones-verificacion.md

### Exportar catálogo y recibos

#### Catálogo en Excel

En **Catálogo → Productos → Exportar a Excel**, elegir dónde guardar el `.xlsx`. Se exportan las filas del catálogo cargado, respetando la búsqueda, la tienda de la cabecera y Mostrar también productos inactivos. La exportación usa una copia de esa vista, de modo que cambiar el filtro mientras se escribe el archivo no cambia sus datos.

La hoja Catalogo contiene Código, Nombre, Categoría, Stock, Precio base (L), Estado y Motivo de desactivación. Los códigos se guardan como texto para conservar ceros iniciales; el stock y precio son números. Incluye filtros de Excel y encabezados inmovilizados. La hoja Información indica el alcance, búsqueda, inclusión de inactivos, cantidad de productos y fecha de generación en Honduras. El precio exportado es el precio base antes del ISV, igual que en el catálogo.

El botón se habilita cuando hay filas para exportar. Si falla la carga del catálogo se retiran los datos anteriores para no exportar otra tienda con un encabezado incorrecto. La plantilla de carga masiva conserva su opción de descarga y vista de ejemplo.

#### Recibo de venta en PDF

- Desde la pantalla de ventas: **Exportar recibo PDF** guarda la última factura emitida o abierta.
- Desde el recibo abierto: **Guardar PDF**, junto a Imprimir.
- Desde **Facturas registradas**: seleccionar la factura y pulsar **Exportar PDF**, incluyendo facturas de sesiones anteriores.
- Al entregar un apartado: el comprobante de la factura también incluye **Guardar PDF**.

El PDF se genera directamente, sin depender de una impresora virtual. Su contenido se consulta en la base de datos e incluye número, fecha, estado, tienda, cliente, RTN opcional, CAI guardado con la venta, productos, subtotal, exento, ISV del 15 % y 18 %, total, pagos, referencias, cambio y observaciones. Los recibos extensos se distribuyen en varias páginas A4, con ajuste de líneas y numeración de página. Se conservan acentos y ceros iniciales.

Se usa [PDFsharp-GDI 6.2.4](https://www.nuget.org/packages/PDFsharp-GDI/6.2.4), compatible con la aplicación Windows, con licencia MIT. La resolución de fuentes usa las fuentes de Windows según la [documentación del motor GDI](https://docs.pdfsharp.net/PDFsharp/Topics/Fonts/Font-Resolving.html).

#### Guardado y bitácora

Se elige el destino mediante Guardar como. El archivo se genera primero en un temporal de la misma carpeta y se mueve al destino al terminar. Si el destino está bloqueado, se informa el error conservando el archivo anterior. No se abre el documento exportado automáticamente.

Después de guardar, la aplicación registra EXPORTAR en la bitácora con el usuario, módulo y factura o alcance del catálogo. No almacena la ruta personal de guardado. Si el archivo se guardó pero el registro en bitácora falla, el mensaje distingue ambas situaciones.

#### Verificación

Se amplió la prueba `Pruebas/FlujoVentas.Integracion` usando una base temporal aislada. Se verificó:

- El Excel contiene únicamente la vista filtrada y mantiene código textual, precio y stock numéricos y metadatos de tienda.
- Un archivo Excel bloqueado no se reemplaza parcialmente.
- Una venta real se exporta a PDF y se lee de nuevo con PdfPig, comprobando RTN, CAI, ISV, identificador, subtotal, total, cambio y texto acentuado.
- Un recibo extenso se reparte en varias páginas, conserva el final y los caracteres de palabras muy largas y mantiene el texto dentro del ancho de página.
- Se registran ambas exportaciones con el usuario responsable.
- Continúan pasando los flujos de facturación, apartados, devoluciones, RTN, ISV, CAI y carga masiva.

La base temporal y archivos de prueba se eliminan al terminar. Compilación sin errores ni advertencias. No se guardaron ventas de prueba en la base de uso.


## C. Datos/Inicio-verificacion.md

### Inicio conectado a ventas y compras

#### Datos y actualización

`InicioDatos.Obtener` consulta las ventas y compras confirmadas mediante `vw_VentaTotal` y `vw_CompraTotal`. Devuelve siete fechas consecutivas, completando con cero las fechas sin movimientos. Las tarjetas muestran el importe y número de facturas del día; el gráfico compara los importes de los últimos siete días. Los borradores y documentos anulados no se incluyen.

Las ventas se agrupan en días de Honduras (UTC-6), convirtiendo la fecha UTC guardada en Venta. Las compras usan `fecha_factura`, igual que el módulo de compras. Los importes representan los totales de los documentos confirmados; no descuentan reembolsos de devoluciones.

Con la incorporación del ISV, el total de ventas incluye el impuesto guardado en las líneas originales. Las facturas anteriores conservan sus importes.

El selector de cabecera filtra todas las consultas por tienda; Todas las tiendas consolida los documentos de todas las sucursales. También se consultan traspasos pendientes y alertas de stock disponible de productos y tiendas activos. Al hacer clic en una tarjeta se abre ventas, compras, validación de traspasos o notificaciones mediante la navegación existente y su registro en bitácora.

La actualización se realiza al entrar o volver a Inicio, cambiar de tienda, pulsar Actualizar resumen y cada minuto mientras Inicio está activo. Las consultas se ejecutan fuera del hilo de interfaz. Si cambia el filtro durante una consulta, se descarta el resultado anterior y se solicita el filtro actual. Los errores muestran datos sin cargar y permiten reintentar; no se presentan ceros como si la consulta hubiera tenido éxito.

#### Verificación realizada

Se ejecutó un programa temporal .NET 10 que usa el método de datos real y una sesión de prueba dentro de una transacción ambiente. Creó catálogos, dos tiendas, compras y ventas propias. Se verificó:

- Tienda A: ventas del día por L 20 y compras por L 100. Tienda B: ventas por L 30 y compras por L 150.
- Inclusión del inicio de la ventana de siete días, días vacíos y exclusión de fechas anteriores y futuras.
- Venta exactamente a las 06:00 UTC como venta del día en Honduras y venta un segundo antes como venta del día anterior.
- Exclusión de compras y ventas en borrador o anuladas.
- Actualización del resumen global después de anular una venta por L 20 y confirmar una compra por L 20: ventas bajaron L 20 y compras subieron L 20, con el cambio correspondiente en el número de documentos.
- Conteo de un traspaso pendiente y una alerta de stock de la tienda de prueba.
- Construcción de las tarjetas y el botón Actualizar resumen, y renderizado del gráfico con el resultado real.

Todas las operaciones de prueba se revirtieron al cerrar la transacción; no quedan documentos de prueba guardados. SQL Server puede consumir identidades aunque una transacción se revierta. Compilación: cero errores y cero advertencias.


## C. Datos/ISV-CAI-verificacion.md

### ISV y CAI en ventas

#### Cálculo y recibos

Los precios guardados son precios base. Según la opción elegida por el usuario, el sistema **suma** el impuesto al precio actual. La tasa inicial de cada producto agregado al carrito es 15 %. El selector junto a Cantidad permite elegir ISV 18 % o Exento (0 %) para ese producto cuando corresponda. Para corregir la tasa de una línea, quitarla y agregarla con la tasa apropiada.

El SAR publica la tasa general del 15 % y la del 18 % en su [ayuda del ISV](https://www.sar.gob.hn/wp-content/uploads/2026/03/DECLARACION-REGIMEN-SIMPLIFICADO-DEL-IMPUESTO-SOBRE-VENTA-CODIGO-202.pdf). La clasificación fiscal debe corresponder al producto vendido. No se determina automáticamente a partir del nombre del producto.

Por línea: `subtotal = cantidad × precio base`; `ISV = redondear(subtotal × tasa / 100, 2)`; `total = subtotal + ISV`. Se redondea a dos decimales alejándose de cero, de forma coincidente entre C# y SQL Server. La pantalla presenta subtotal, ISV del 15 %, ISV del 18 %, total y cambio. Los pagos y el cambio usan el total con impuesto. Ejemplo: precio base L 100, ISV 15 % L 15, total L 115.

El recibo y las reimpresiones se generan desde los detalles y tasas guardados con la factura. Muestran también el importe exento, RTN opcional y CAI. Inicio e historial de ventas usan el total de la vista de ventas, incluido el impuesto.

Los apartados nuevos reservan productos con ISV 15 % y calculan el saldo con impuesto. Al entregarlos, la factura conserva sus tasas, precios y abonos. Las devoluciones calculan el impuesto a partir de la línea original, distribuyendo los centavos entre devoluciones parciales para restituir exactamente el ISV cobrado al devolver todas las unidades. Cancelar o confirmar una devolución mantiene las reglas de stock y reembolso.

#### Configuración del CAI

Abrir **Configuración y auditoría → Facturación / CAI**, seleccionar la tienda e ingresar el CAI autorizado. Guardar registra el cambio en la bitácora. El [SAR identifica el CAI como la Clave de Autorización de Impresión](https://www.sar.gob.hn/wp-content/uploads/2021/02/Solicitud-de-Activacion-de-Impresion-por-Imprenta-SAR-925.pdf).

Se admite el formato `XXXXXX-XXXXXX-XXXXXX-XXXXXX-XXXXXX-XX`, o los 32 caracteres sin guiones; se normaliza a mayúsculas y 37 caracteres. El formato no valida la autorización ante el SAR. La clave se guarda por tienda y se copia a cada nueva venta, incluidas las entregas de apartados. Cambiar el CAI de configuración no cambia las facturas anteriores. Si la tienda no tiene CAI, el recibo indica “Sin configurar”; no se genera una clave ficticia.

Esta incorporación guarda la clave y el desglose de ISV. El sistema utiliza su número interno de venta; la autorización de rangos, correlativos fiscales y fecha límite de emisión requiere un flujo fiscal adicional.

Configuración queda organizada en pestañas de Tiendas, Facturación / CAI y Auditoría, con desplazamiento dentro de sus tablas y formularios.

#### Base de datos y verificación

Ejecutar `Datos/Migraciones/004_isv_cai.sql` después de las migraciones anteriores en bases existentes. Agrega CAI por factura, la configuración por tienda, tasas e impuestos y actualiza las vistas de totales y la comprobación de entrega de apartados. Es idempotente. El esquema inicial y Triggers2.sql también incluyen estos cambios.

Las líneas existentes reciben tasa e impuesto cero, conservando los importes anteriores. Los nuevos detalles tienen tasa 15 % por defecto y aceptan solo 0, 15 y 18. Se verificó en la base local que el total histórico anterior se conserva y que una segunda ejecución de la migración termina correctamente.

`dotnet run --project Pruebas/FlujoVentas.Integracion/FlujoVentas.Integracion.csproj` crea y elimina una base temporal. Se prueba la actualización desde la estructura anterior y los métodos reales de emisión, configuración y reembolso. Pasaron los casos de 15/18/exento, pago insuficiente, cambio, tasas inválidas, CAI por tienda, actualización concurrente del CAI, reimpresión histórica y redondeo de devoluciones parciales. También pasó el flujo previo de apartados, RTN e importación y la construcción de las pantallas. Compilación sin errores ni advertencias.

La prueba de auditoría de `Datos/Pruebas/Bitacora-sistema.sql` cubre ahora 38 entidades operativas, incluida ConfiguracionFacturacion, y se revierte al finalizar.


## C. Datos/Notificaciones-verificacion.md

### Verificacion de notificaciones de inventario

#### Preparacion y criterio

Usar una base de pruebas con el esquema 3FN y la conexion descrita en README. Iniciar sesion con usuario y empleado activos. Preparar tiendas, productos e inventario mediante los flujos documentales existentes; no registrar ajustes manuales ni instalar scripts desde la aplicacion.

Una alerta corresponde a un par producto/tienda activo cuyo **disponible (existencia menos reservado) es igual o inferior al stock minimo**. Se consultan las filas actuales de InventarioTienda; un producto sin fila en esa tienda no genera una alerta. Una fila con disponible 0 y minimo 0 si cumple el criterio. Se muestran existencia, reservado, disponible y minimo para distinguir las cantidades.

La campana consulta al iniciar la ventana autenticada y cada 60 segundos. Abrirla o pulsar Actualizar vuelve a consultar. El contador usa 9+ cuando hay mas de nueve pares; el texto de ayuda conserva el numero completo. Un signo ? indica consulta pendiente o fallida y se distingue de un resultado correcto de cero alertas.

#### Verificacion local realizada

Un ejecutable temporal sobre .NET 10 para Windows realizo **47 comprobaciones aprobadas**, sin SQL ni lectura de credenciales. Se verificaron constructor sin consultas; condicion visible; siete columnas explicitas de solo lectura; cargas con resultados, vacias y fallidas; reintento; limpieza de datos obsoletos ante errores; exclusion de cargas simultaneas; cierre durante una consulta pendiente; contador, texto accesible y ayuda de la campana; espera compartida de consultas; temporizador y rechazo de consulta sin sesion antes de preparar la conexion.

La comprobacion visual detecto que el contador 9+ se cortaba a 54 x 30. Se aumento el ancho a 64 y se confirmo su representacion completa en bitmap. Se verifico la ausencia de solapes del encabezado a 1050 x 720. La revision visual del dialogo a 940 x 560 y 720 x 440 comprobo acciones visibles y desplazamiento de la tabla. El ejecutable temporal compilo sin errores ni advertencias.

Estas comprobaciones usan cargadores y tablas simulados. No ejecutaron el predicado SQL, transacciones de lectura, triggers, cambios de inventario ni consultas automaticas contra una base real. Los casos siguientes siguen pendientes de integracion.

#### Casos contra SQL Server pendientes

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

#### Limites

Es una consulta de alertas vigentes, no una bandeja de mensajes leidos/no leidos ni un despachador de EventoNotificacion. La autorizacion granular por tienda y permiso sigue pendiente en el sistema; la consulta usa el mismo alcance general de inventario para la sesion autenticada. Los eventos historicos de los triggers se conservan y no se asignan destinatarios automaticamente desde este flujo.


## C. Datos/Seguridad-verificacion.md

### Verificacion de usuarios, roles y permisos

#### Preparacion

Usar una base de pruebas con `Base de Datos/01_esquema_erp_3fn.sql` ya instalado y la conexion descrita en README. El script de creacion es para una base vacia; no ejecutarlo sobre una base existente con datos. No se requiere migracion para estos CRUD. Para invalidacion de sesiones y auditoria instalar los triggers proporcionados en la misma base. La aplicacion no instala scripts.

Crear una tienda y dos empleados activos mediante los CRUD existentes. Abrir **Usuarios, roles y permisos** desde la ventana principal.

#### Casos funcionales contra SQL Server (pendientes de ejecucion)

1. Crear dos permisos con modulo, accion y descripcion; comprobar que el codigo calculado corresponde. Probar campos vacios, limites de longitud, caracteres no ASCII en modulo/accion y combinaciones duplicadas.
2. Crear un rol, asignarle ambos permisos, actualizar nombre y retirar uno. Comprobar `RolPermiso.activo` y que la relacion retirada se conserva. Probar codigo duplicado y nombre vacio.
3. Crear una cuenta ligada a un empleado, con contrasena confirmada de 12 a 128 caracteres y el rol seleccionado. Comprobar `UsuarioRol`, `debe_cambiar_contrasena=1` y hash con prefijo PBKDF2-SHA256. Nunca registrar contrasenas ni hashes en capturas o bitacoras de pruebas.
4. Intentar reutilizar usuario con espacios o diferente capitalizacion y reutilizar empleado, incluidos registros inactivos: la base debe rechazar duplicados sin cambios parciales.
5. Actualizar usuario y roles dejando ambas claves vacias: se conserva el hash. Cambiar la clave confirmada: cambia el hash y la fecha, con sal nueva. Una confirmacion distinta debe impedir guardar.
6. Desactivar cada entidad: cancelar no cambia nada, motivo vacio no permite confirmar y confirmar conserva relaciones y registra motivo. Incluir inactivos, comprobar solo lectura y reactivar; fecha, actor y motivo deben quedar nulos.
7. Desactivar un empleado, rol o permiso previamente asignado. Conservar su asignacion al editar otros campos, retirarla y comprobar que no puede asignarse nuevamente mientras siga inactivo. Comprobar que un nuevo usuario requiere empleado activo.
8. Buscar por texto, descripcion, apostrofo, corchetes, asterisco y porcentaje: estos caracteres se interpretan literalmente. Nuevo limpia campos y seleccion; cambiar de pestana elimina claves escritas y datos de la pestana anterior.
9. Abrir dos ventanas o instancias sobre el mismo registro. Actualizar o desactivar en una; guardar o cambiar estado desde la otra debe informar conflicto y no sobrescribir. Recargar permite continuar.
10. Provocar un fallo de asignacion (por ejemplo, desactivar un permiso desde otra instancia antes de guardar el rol). Verificar que tambien se revierte la actualizacion del registro principal.
11. Con triggers instalados comprobar invalidacion de sesiones al cambiar contrasena, desactivar una cuenta o modificar privilegios. Los CRUD no insertan eventos manuales ni duplican la tarea de los triggers.
12. Sin conexion, abrir la pantalla y cambiar pestanas: debe mostrar error y deshabilitar escrituras. Restablecer conexion y pulsar Recargar. Revisar visualmente escalado, desplazamiento independiente y acceso a acciones en ventana pequena.

#### Verificacion local realizada

Un ejecutable temporal sobre .NET 10 comprobo construccion del formulario sin consultar SQL, columnas de las tres variantes, filtros con caracteres especiales y validaciones previas a persistencia: 17 comprobaciones aprobadas. Compilacion y revision de espacios se verifican por separado. Estas comprobaciones no sustituyen los casos de integracion anteriores.

#### Limites

El login autentica y obliga a cambiar claves temporales; las asignaciones todavia no restringen la navegacion ni las operaciones del ERP. Los actores de creacion, modificacion, desactivacion y asignacion de los CRUD quedan nulos. No se gestionan UsuarioTienda ni recuperacion de acceso en esta pantalla. Desactivar conserva las relaciones: reactivar un rol o permiso puede restaurar privilegios de sus asignaciones vigentes.

#### Login: integracion pendiente contra SQL Server

Verificacion local del login: compilacion sin errores ni advertencias, 14 comprobaciones aprobadas de hashes, rechazo de formatos invalidos, limites de clave, sal aleatoria y construccion del formulario. La creacion inicial ahora se realiza desde la aplicacion y reemplaza la herramienta PowerShell anterior.

Regresion del manejo de errores: 23 comprobaciones aprobadas con respuestas de autenticacion simuladas. Un rechazo y su reintento conservan abierto el formulario y el nombre de usuario, muestran el mensaje, limpian la clave y habilitan Ingresar; un fallo de conexion se controla del mismo modo. Una clave vacia devuelve un resultado de validacion sin consultar SQL. Las credenciales rechazadas se comunican como ResultadoAcceso, sin lanzar excepciones de validacion desde Ingresar. Falta repetir el caso con la base real del operador.

1. Base nueva con esquema instalado y Usuario vacia: ingresar con admin/admin. Verificar creacion de la cuenta temporal y asignacion al rol ADMINISTRADOR, con empleado ADMIN-INICIAL y tienda INICIAL si hacen falta. Debe exigir cambio por clave de 12 a 128 caracteres antes de abrir Inicio. Reabrir y probar dos instancias: no debe duplicar registros ni restablecer la clave. Si ya existen usuarios, incluidos inactivos, admin/admin no se crea automaticamente. Los registros iniciales inactivos no se reactivan. Probar en una base de pruebas, no en produccion.
2. Arrancar la aplicacion: solo debe mostrarse login. Credenciales invalidas, cuenta inactiva o empleado inactivo no abren Inicio; no puede elegirse un rol manualmente.
3. Clave temporal: verificar cambio obligatorio, longitud, confirmacion y rechazo de la misma clave. Cancelar cierra la aplicacion sin abrir Inicio. Una modificacion concurrente de la cuenta impide usar el cambio pendiente.
4. Cinco claves incorrectas bloquean durante 15 minutos. Una correcta durante el bloqueo tampoco entra. Al vencer, un fallo inicia una cuenta nueva de intentos; una entrada completa los reinicia. Revisar EventoAcceso.
5. Verificar creacion de SesionUsuario, nombre y roles en Inicio. Cerrar sesion revoca la fila y vuelve al login; entrar con otra cuenta debe mostrar su identidad. Cerrar Inicio termina el proceso.
6. Con triggers instalados cambiar privilegios o clave desde otra instancia: en un minuto la sesion anterior debe terminar. Verificar tambien expiracion, desactivacion de empleado y perdida de conexion. Una sesion que no logra revocarse por desconexion caduca a las ocho horas.
7. Los hashes de ejemplo del poblado no son claves utilizables. Para cuentas existentes sin hash compatible se requiere recuperacion administrada; admin/admin solo se crea cuando Usuario esta vacia y no permite acceder a cuentas existentes.


## C. Datos/Traspasos-verificacion.md

### Inventario, movimientos y traspasos

#### Preparacion

Usar una base de pruebas con el esquema 3FN instalado. Base de Datos/01_esquema_erp_3fn.sql es para una base vacia; no ejecutarlo sobre una base con datos. Instalar Base de Datos/Triggers2.sql en la misma base configurada para la aplicacion. La aplicacion no instala ni modifica el esquema.

Se requieren los triggers de movimientos (validar, aplicar e inmutable), los de estados y operacion de traspasos y sus dependencias. Configurar nested triggers habilitado y RECURSIVE_TRIGGERS deshabilitado, como indica el script. No deshabilitar triggers para eludir errores de disponibilidad.

Iniciar sesion con una cuenta activa ligada a un empleado activo. Preparar dos tiendas activas, un proveedor activo y productos activos. Para cargar existencias de prueba, registrar y confirmar una compra desde Compras en la tienda de origen, siguiendo [Compras-verificacion.md](Compras-verificacion.md). Se pueden consultar productos sin saldo y registrar su primera entrada mediante la compra sin crear InventarioTienda manualmente.

#### Comportamiento

- Inventario y movimientos abre su pantalla; Compras conserva una opcion independiente.
- Existencias muestra saldo fisico, reservado, disponible y minimo por tienda. Incluye productos activos sin saldo y productos inactivos con una fila de inventario. Las fechas no afectan al saldo actual.
- Movimientos muestra el libro por tienda, texto y fechas UTC, con 30 dias iniciales. Consultar o Enter ejecuta los filtros. El texto se interpreta literalmente mediante parametros y CHARINDEX.
- Inventario no permite ingresar, retirar ni revertir unidades manualmente. Las compras y ventas registran sus movimientos desde los documentos correspondientes; los traspasos mantienen su flujo documental entre tiendas.
- Guardar minimo cambia solo el umbral, con comprobacion de version. No registra entradas o salidas de unidades.
- Los movimientos historicos, incluidos los ajustes anteriores, permanecen consultables. No se editan, eliminan ni revierten desde Inventario.
- Solicitar crea PENDIENTE sin reservar. Aprobar reserva en origen sin cambiar existencia. Enviar pasa APROBADO a EN_TRANSITO, libera reserva y genera TRASPASO_SALIDA. Recibir pasa EN_TRANSITO a RECIBIDO y genera TRASPASO_ENTRADA en destino.
- Rechazar una pendiente requiere motivo. Cancelar una pendiente no toca existencias; cancelar una aprobada libera reservas. No se cancela un envio ni se recibe dos veces.
- Los triggers registran historial, movimientos y notificaciones dentro de la misma transaccion. No se insertan eventos ni movimientos de traspaso desde C#.
- El operador es el usuario autenticado. Cada escritura verifica la sesion en SQL Server dentro de su transaccion; una sesion vencida, revocada, con otra version o con usuario/empleado inactivo no puede escribir. Ya no hay selector manual de usuarios.
- Las solicitudes y los saldos usan rowversion para impedir sobrescrituras de cambios concurrentes. Ante conflicto, actualizar el listado antes de reintentar.

#### Pruebas funcionales con SQL Server pendientes

1. Abrir Inventario y movimientos y Compras: comprobar que llevan a pantallas distintas. Seleccionar tienda, buscar producto con apostrofos, corchetes, porcentaje y asterisco; verificar busqueda literal, tablas y conteos. Cambiar fechas solo modifica Movimientos.
2. Producto sin saldo: registrar una compra de 20 unidades y guardarla como borrador. El saldo no cambia hasta confirmar. Al confirmarla debe crearse InventarioTienda con existencia 20, reservado 0 y disponible 20, y un solo movimiento COMPRA. Consultar nuevamente Inventario para comprobarlo.
3. Cambiar minimo y comprobar persistencia sin generar movimientos de unidades. Modificar saldo o minimo desde otra instancia antes de guardar: debe informar conflicto. Esto incluye la creacion concurrente de un saldo previamente inexistente.
4. Comprobar que Inventario no ofrece Registrar ajuste ni Revertir ajuste en ninguna pestana, y que ambas tablas son de solo lectura. Si existen ajustes de versiones anteriores, siguen visibles en Movimientos y no tienen accion de reversion. Guardar minimo no modifica existencia, reservado ni disponible.
5. Crear solicitud con productos y tiendas distintas: comprobar PENDIENTE, solicitante autenticado y evento unico. Nuevo guardado debe limpiar campos y mostrar el numero. Intentar productos repetidos, cantidad cero, precio negativo, campos vacios y tiendas iguales.
6. En Validacion revisar detalle e historial y editar una pendiente. Las tiendas y el solicitante no se cambian. Verificar precios historicos al abrir edicion y precio actual al cambiar de producto.
7. Aprobar con disponibilidad: aumenta reservado y conserva existencia. Aprobar sin disponible falla sin cambios parciales. Rechazar exige motivo y no reserva.
8. Enviar un aprobado: existencia y reservado en origen disminuyen por la cantidad, destino no cambia, estado EN_TRANSITO y un movimiento de salida por detalle. Recibir: aumenta existencia del destino, estado RECIBIDO, un movimiento de entrada por detalle. Actualizar filtro de estado para localizar cada etapa.
9. Cancelar pendiente y cancelar aprobado con motivo: verificar que solo el segundo libera reserva. Acciones de enviar, recibir, editar, aprobar y cancelar deben estar habilitadas unicamente para los estados aplicables. Reintentar envio o recepcion desde una ventana antigua debe fallar por version o estado, sin movimientos repetidos.
10. Abrir la misma solicitud en dos instancias. Resolver en una y editar o resolver desde la otra: debe informar conflicto. Comprobar que la transaccion revierte cabecera, detalles, inventario e historial ante cualquier error de trigger.
11. Desactivar tienda o producto despues de seleccionarlo para cambiar el minimo: guardar debe fallar. Los saldos historicos siguen consultables. Consultar de nuevo debe deshabilitar Guardar minimo para la entidad inactiva y conservar la consulta de movimientos anteriores.
12. Revocar/expirar la sesion o desactivar al usuario/empleado antes de confirmar una operacion: debe rechazarse. No debe poder atribuirse un movimiento a otra cuenta ni operar sin sesion.
13. Crear una sucursal desde la solicitud y verificar persistencia y actualizacion de selectores sin perder lineas o selecciones. Codigo duplicado o campos obligatorios vacios impiden guardarla.
14. Deshabilitar un trigger requerido en la base de pruebas: las escrituras deben mostrar que falta Triggers2.sql. Perder conexion no debe cerrar el programa ni confirmar una operacion fallida. Revisar tambien el desplazamiento y las acciones al reducir la ventana.

#### Verificacion realizada

El 2026-11-03 se verifico en SQL Server local el flujo de compra y traspaso dentro de una transaccion revertida al finalizar: guardar el borrador no cambia existencias; confirmar ingresa 20 unidades; aprobar un traspaso reserva 6; enviar deja 14 unidades en origen y libera la reserva; recibir ingresa 6 en destino. Repetir el estado confirmado o recibido no duplica movimientos: se comprobaron exactamente un movimiento COMPRA, una salida y una entrada. No quedaron registros de prueba (los contadores IDENTITY pueden avanzar).

Inventario recarga tiendas, existencias y movimientos al activar su ventana, incluyendo el regreso desde Compras o Validacion, conservando la tienda seleccionada y los filtros. La compilacion de este cambio termino sin errores ni advertencias. La comprobacion visual del regreso entre ventanas sigue pendiente.

Tras retirar los ajustes manuales, la compilacion termino sin errores ni advertencias y git diff --check no encontro errores de espacios. La revision de referencias C# confirmo la eliminacion de las acciones, el dialogo y los metodos de datos de ajuste y reversion manual. No se ejecutaron cambios contra SQL Server.

En la version anterior, antes de retirar los ajustes manuales de Inventario, se verificaron compilacion sin errores ni advertencias y git diff --check sin errores de espacios. Un ejecutable temporal realizo 41 comprobaciones de construccion de controles, estados de botones para las seis etapas, validaciones de ajustes/minimos/reversiones/fechas/traspasos y exigencia de sesion antes de enviar: todas aprobadas. Esta evidencia es historica y no valida las acciones de ajuste o reversion en la version actual, donde fueron eliminadas.

Se intento una conexion de solo lectura a la base configurada para verificar los seis triggers principales. Fallo con codigo SQL 20. No se escribieron datos ni se instalaron scripts; siguen pendientes las pruebas transaccionales y visuales con una conexion operativa.

#### Limites

La autorizacion granular por permiso y tienda todavia no se aplica; las operaciones se atribuyen a la identidad autenticada y requieren una sesion vigente. No se implementa recepcion parcial ni diferencias de cantidades: cada envio y recepcion procesa todas las lineas del documento.

El indice unico de EventoTraspaso permite un evento por solicitud y estado. Las ediciones pendientes se protegen con rowversion, pero no generan eventos PENDIENTE adicionales. Las fechas se muestran y filtran en UTC para coincidir con el almacenamiento de SQL Server.


## C. Datos/Ventas-verificacion.md

### Ventas y bitácora

#### Instalación

Usar la conexión local descrita en README. En una instalación nueva, ejecutar el esquema 3FN, `Base de Datos/Triggers2.sql` y después `Datos/Migraciones/002_bitacora_sistema.sql`, en esa misma base. La migración es transaccional y se puede volver a ejecutar. No ejecutar los scripts de reinicio para actualizar una base existente.

Para una base existente, ejecutar también `Datos/Migraciones/003_rtn_ventas.sql`: agrega `Venta.rtn_cliente` opcional y su validación, sin cambiar las facturas anteriores. Ya se aplicó en la base local y se comprobó que puede volver a ejecutarse. El esquema inicial también incluye el campo para instalaciones nuevas.

Para ISV y CAI, aplicar después `Datos/Migraciones/004_isv_cai.sql`. Consulte [ISV-CAI-verificacion.md](ISV-CAI-verificacion.md). La base local ya está actualizada.

La migración 002 ya se aplicó a la base local durante esta implementación. Complementa los seis triggers de auditoría existentes con 31 triggers sobre las demás tablas. Las 37 tablas operativas del esquema actual quedan cubiertas; Auditoria conserva su protección contra modificaciones y eliminaciones. Al agregar tablas nuevas, volver a ejecutar la migración y verificar la cobertura.

#### Uso

En Ventas, seleccionar tienda, cliente y método de pago activos. Buscar por nombre o código, seleccionar un producto y agregar la cantidad al carrito (también con doble clic). Quitar o vaciar permite corregir el detalle. Actualizar catálogos vacía el carrito para evitar conservar precios antiguos.

El botón **Refrescar stock**, junto al buscador, vuelve a consultar las existencias de la tienda seleccionada y limpia el filtro para mostrar todos sus productos activos. Conserva la tienda, cliente, pago y carrito. La tabla muestra Existencia, Reservado y Disponible (existencia menos reservado), y el mensaje indica la tienda y hora de actualización. Al regresar a ventas desde otro módulo también se actualiza el stock, conservando la búsqueda. Si la consulta falla, se retiran los datos antiguos y se muestra el error.

Para efectivo, ingresar un monto recibido suficiente; se calcula el cambio. Para otros métodos, ingresar la referencia. Emitir factura pide confirmación y guarda cabecera, detalles y pago en una transacción serializable. Los precios se verifican contra la base antes de cobrar; los triggers descuentan el inventario disponible, respetan reservas y registran auditoría. Cualquier error antes del commit revierte toda la operación. Al precio base se suma el ISV seleccionado por línea (15 % por defecto; también 18 % y exento). El cobro, el cambio y el recibo usan el total con impuesto.

El campo **RTN del cliente (opcional)** permite facturar sin RTN. Si se escribe, debe contener 14 dígitos; se eliminan guiones y espacios y se conservan los ceros iniciales. Al cambiar de cliente se precarga su `documento_fiscal` si tiene ese formato; se puede editar o borrar para esa factura. El RTN se guarda con la venta, aparece en el comprobante y la reimpresión y permite buscar la factura en el historial. No modifica el registro del cliente ni depende de cambios posteriores en ese registro. Después de emitir la factura se limpia el campo.

Los cobros nuevos en efectivo generan su referencia automáticamente al guardarse, con formato `EFE-` seguido de un identificador aleatorio de 32 caracteres. Se aplica a ventas, abonos y cobro del saldo al entregar apartados. El campo de referencia es de solo lectura para efectivo y muestra «Se genera al registrar el pago». La referencia queda en el pago y en el recibo; al facturar un apartado se conserva la referencia original de cada abono. Los demás métodos requieren una referencia manual. No requiere migraciones ni modifica pagos anteriores.

Se puede ver e imprimir el comprobante emitido. Facturas registradas permite buscar entre las últimas 500 facturas y recuperar sus detalles y pagos desde SQL Server para reimprimir. La búsqueda de auditoría opera en servidor y las páginas muestran hasta 200 eventos. Actualizar vuelve a los más recientes; Anteriores/Siguientes permiten recorrer las páginas. Ambas tablas de configuración usan desplazamiento local; el formulario conserva desplazamiento para pantallas pequeñas.

La bitácora registra altas, modificaciones, eliminaciones, intentos de acceso (incluido su resultado), sesiones y navegación entre módulos. El contexto de usuario se establece al abrir cada conexión y se limpia el motivo para evitar atribuciones incorrectas por reutilización de conexiones. La captura nueva evita copiar contraseñas y tokens. Los cambios que no se registraban antes de instalar la migración no se pueden reconstruir retrospectivamente.

#### Verificación realizada

- `dotnet build --no-restore`: cero errores y cero advertencias.
- Creación y disposición de ambos formularios mediante un ejecutable temporal .NET 10/WinForms: correctas; tablas de consulta de solo lectura.
- Migración aplicada y vuelta a ejecutar correctamente en la base local; cero tablas operativas sin trigger de auditoría.
- Prueba SQL transaccional: creación y confirmación de una venta con detalle y pago; descuento de una unidad; verificación de auditoría de venta y pago con usuario responsable; registro de un acceso inválido con su resultado. Se revirtió la transacción completa para no conservar ventas de prueba.
- Cobertura integral posterior: [Bitacora-verificacion.md](Bitacora-verificacion.md) documenta la prueba con eventos reales en las 37 entidades del sistema, también fuera de ventas.
- Refresco de stock: se ejecutó la consulta real de facturación antes y después de una entrada de inventario y de una reserva por apartado. Reconoció la unidad nueva y descontó la reservada. Toda la prueba se revirtió. Se comprobó también la disposición del botón junto al buscador en un formulario de 1400 × 900.
- RTN: probado con y sin valor, guiones, espacios y ceros iniciales. Se verificó su persistencia, impresión, auditoría y conservación tras editar el documento fiscal del cliente. Los RTN inválidos no crean ventas; cambiar de cliente limpia el valor anterior y precarga el nuevo si corresponde. La prueba de integración usa una base temporal y la elimina al terminar.
- Referencias de efectivo: integración verificada en ventas, abonos y saldo de entrega; referencias distintas por cobro, conservación de la referencia del abono al facturar, aparición en el recibo y registro auditado del pago. También se comprobaron el cambio de método en las tres pantallas de cobro, las referencias manuales de transferencias y la reversión de cobros inválidos. La suite completa pasó en una base temporal eliminada al terminar; compilación con cero errores y advertencias.

Pendiente de comprobación manual: recorrido visual interactivo a distintas resoluciones y salida en una impresora física. No se envió ninguna impresión durante las pruebas.
