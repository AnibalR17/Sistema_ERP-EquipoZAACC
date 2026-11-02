# Sistema ERP


## Organizacion del proyecto

```text
Formularios/
  Acceso/          Inicio de sesion
  Principal/       Ventana MDI y navegacion compartida
  Productos/       Catalogo, productos, carga masiva y nuevo producto
  Proveedores/     Gestion de proveedores
  Compras/         Registro de compras
  Inventario/      Inventario, traspasos y validacion
  Ventas/          Punto de venta, apartados y devoluciones
  Administracion/  Inversiones, directorio, tiendas y auditoria
  Dialogos/        Confirmacion de venta y notificaciones
  Base/            Clases compartidas de pantallas y dialogos
Clases/            Modelos, tipos de dominio y operaciones compartidas
Datos/             Conexion y acceso a SQL Server
Presentacion/      Estilos y utilidades compartidas de interfaz
Controles/         Controles visuales reutilizables
Program.cs         Punto de entrada
```

Cada formulario conserva juntos sus archivos `.cs`, `.Designer.cs` y `.resx`, cuando existen. Los archivos `.Presentacion.cs` contienen la configuracion visual de esa misma clase y aparecen anidados bajo el formulario en Visual Studio. Los estilos compartidos se encuentran en `Presentacion/`.

Los namespaces originales (`Sistema_ERP`, `Sistema_ERP.Vistas` y `LoginERP`) se mantienen para conservar las referencias existentes. Al agregar o mover archivos del disenador, actualice las rutas y las relaciones `DependentUpon` en `Sistema_ERP.csproj`.

`connection.local.txt` permanece en la raiz y se copia al directorio de salida. No debe incluirse en Git.

## Conexion a la base de datos

Configure la variable de entorno `SISTEMA_ERP_CONNECTION_STRING` o cree `connection.local.txt` en la raiz del proyecto con la cadena de conexion de SQL Server. La variable de entorno tiene prioridad. El archivo local se copia al directorio de salida y esta excluido de Git para proteger las credenciales.

En Inicio, haga clic en **Probar conexion** para comprobar el acceso a la base de datos. Se mostrara un mensaje de exito o error sin bloquear la interfaz.

Los traspasos requieren el esquema 3FN y `Base de Datos/Triggers2.sql`, basado en la base proporcionada. Ejecute los triggers en la misma base configurada para la aplicacion. La base genera el historial y reserva inventario al aprobar; las tiendas de una solicitud creada no se cambian. La preparacion y las pruebas se describen en [Datos/Traspasos-verificacion.md](Datos/Traspasos-verificacion.md).

Aplicacion de escritorio desarrollada con C# y Windows Forms sobre .NET 10 para Windows.

## Estandar grafico

El estandar parte de `Formularios/Principal/Pagina_Central.Designer.cs` y `Formularios/Principal/Pagina_Central.cs`. Las variantes de captura, tablas y dialogos utilizadas por Configuracion y Directorio se documentan mas abajo para mantener coherencia entre pantallas.

### 1. Identidad visual

Interfaz de escritorio clara y minimalista: fondos blancos, grises neutros, tipografia Segoe UI, navegacion lateral y paneles con esquinas redondeadas. La jerarquia se establece mediante tamano, negrita y espaciado.

La marca se presenta como texto **Sistema ERP**. No hay logotipo grafico ni color corporativo de acento definido en el codigo.

### 2. Paleta de colores

| Uso implementado | HEX | RGB |
| --- | --- | --- |
| Fondo de ventana, menu, barra superior y tarjetas | `#FFFFFF` | 255, 255, 255 |
| Fondo del area principal | `#FDFDFD` | 253, 253, 253 |
| Texto secundario y descripciones | `#7D7D7D` | 125, 125, 125 |
| Fondo de la franja informativa | `#F3F3F3` | 243, 243, 243 |
| Fondo de Cerrar sesion | `#F2F2F2` | 242, 242, 242 |
| Fondo de botones al pasar el puntero | `#F5F5F5` | 245, 245, 245 |
| Contorno de tarjetas y grafico | `#F7F7F7` | 247, 247, 247 |
| Separadores del menu y barra superior | `#D3D3D3` | 211, 211, 211 |
| Puntos y linea del grafico | `#121212` | 18, 18, 18 |

El texto principal hereda el color de texto de WinForms; no tiene un HEX explicito. Los fondos transparentes de las etiquetas dejan ver el contenedor. Los formularios CRUD usan las variantes de estado documentadas en la seccion 8; el resto del diseno conserva la paleta neutra.

### 3. Tipografia

Familia: **Segoe UI**. Los tamanos declarados con `Font` estan en puntos (pt).

| Elemento | Tamano | Peso |
| --- | --- | --- |
| Titulo de pantalla: Inicio | 27 pt | Negrita |
| Valores de las tarjetas | 25 pt | Negrita |
| Letras de dias del grafico | 18 pt | Regular |
| Marca Sistema ERP | 16 pt | Negrita |
| Nombre de usuario | 14 pt | Negrita |
| Subtitulo, buscador y titulo del grafico | 12 pt | Regular |
| Texto general, navegacion y cuenta | 11 pt | Regular |
| Encabezados del menu e Inicio | 11 pt | Negrita |
| Titulos y detalles de las tarjetas | 10 pt | Regular |
| Rol del usuario | 9 pt | Regular |

Los textos se alinean principalmente a la izquierda y se centran verticalmente dentro de sus controles. Los botones de menu y notificaciones de la barra superior centran su simbolo.

### 4. Distribucion y medidas

Las medidas siguientes son valores de diseno de WinForms. La ventana utiliza `AutoScaleMode.Font`, por lo que el tamano visible puede variar con el escalado y la fuente.

| Elemento | Medida base |
| --- | --- |
| Area cliente inicial | 1500 × 900 |
| Tamano minimo de ventana (`MinimumSize`) | 1050 × 720 |
| Ancho del menu lateral | 265 |
| Alto de la barra superior | 64 |
| Relleno del contenido: izquierda, arriba, derecha, abajo | 32, 34, 32, 36 |
| Fila de titulo y subtitulo | 86 de alto |
| Franja informativa | 42 de alto |
| Separacion despues de la franja | 20 |
| Fila de indicadores | 144 de alto |
| Distribucion de indicadores | 4 columnas, 25 % cada una |
| Separacion entre tarjetas | 20 |
| Margen inferior de las tarjetas | 20 |
| Desplazamiento horizontal del texto en tarjetas | 24 |
| Radio de tarjetas y panel del grafico | 30 |
| Radio de la franja informativa | 18 |

El menu se acopla a la izquierda; la barra superior, arriba; el contenido ocupa el area disponible. El grafico toma el espacio vertical restante. El bloque de usuario se ancla abajo a la izquierda.

La fila de tarjetas conserva cuatro columnas al reducir la ventana: no hay una distribucion alternativa para pantallas pequenas. El boton ☰ permite ocultar y mostrar el menu.

### 5. Componentes y estados

- **Navegacion lateral:** botones planos de 236 × 40, sin borde, fondo blanco y relleno izquierdo de 10. Inicio se distingue con negrita, sin resaltado gris permanente. Al pasar el puntero se aplica `#F5F5F5`.
- **Grupos de navegacion:** Descubrir y Administracion, con encabezados en negrita.
- **Sesion:** nombre de usuario, rol en gris y boton Cerrar sesion de 216 × 38 con fondo `#F2F2F2`.
- **Barra superior:** boton ☰ de 36 × 30, buscador sin borde, nombre de cuenta con simbolo ⌄ y boton de notificaciones de 54 × 30. Los botones de simbolos tienen borde de 1.
- **Buscador:** ancho entre 160 y 420, ajustado al espacio disponible; texto de ayuda «🔍 Buscador global...».
- **Franja informativa:** texto secundario, simbolo ℹ y relleno horizontal de 16 a la izquierda y 8 a la derecha.
- **Indicadores:** titulo gris, valor grande en negrita y detalle gris. El ejemplo monetario usa `L. 0.00`; no hay formateador monetario centralizado.
- **Grafico semanal:** panel blanco, titulo a la izquierda, dias L, M, X, J, V, S y D; puntos de 22 de diametro y linea horizontal de grosor 8, ambos `#121212`. No hay ejes numericos, leyenda ni cuadricula. Los datos son de ejemplo.

Los simbolos son caracteres Unicode o emoji, no imagenes de una biblioteca de iconos. Su apariencia puede variar segun las fuentes disponibles.

### 6. Reglas para extender el diseno

Estas reglas se proponen a partir de los patrones existentes:

1. Usar Segoe UI y conservar la jerarquia tipografica descrita.
2. Mantener menu lateral, barra superior y margenes del contenido en las nuevas pantallas.
3. Usar blanco para superficies y gris para informacion secundaria; reservar las negritas para titulos, valores y seleccion del menu.
4. Reutilizar radios de 30 en paneles principales y 18 en franjas informativas.
5. Mantener navegacion plana, texto a la izquierda y respuesta visual al pasar el puntero.
6. Declarar controles y propiedades editables en `InitializeComponent`, con sus campos en `.Designer.cs`; dejar comportamiento y dibujo personalizado en `.cs`.
7. Documentar cualquier nuevo color de estado, control o variante antes de extenderlo a otras pantallas.

### 7. Limites del estandar actual

- `Presentacion/ProductoEstilo.cs` concentra colores y botones compartidos. Las propiedades editables y los valores de distribucion se conservan en cada `.Designer.cs`.
- Las esquinas redondeadas y el grafico se dibujan durante la ejecucion y pueden no aparecer completos en el disenador.
- La implementacion no dibuja las sombras de la imagen de referencia.
- Las variantes de tablas, captura, dialogos, validacion y acciones principales se describen en la seccion 8. No se ha definido modo oscuro.
- Este documento describe y propone reglas visuales; no confirma que las opciones de navegacion o el buscador tengan funcionalidad de negocio implementada.

### 8. Variantes de captura y CRUD

- **Superficie:** fondo blanco, margenes de contenido de 32, 34, 32 y 36; el menu y el encabezado compartidos los aporta la ventana MDI. Configuracion y Directorio desplazan su contenido de forma independiente.
- **Captura:** campos distribuidos en tres columnas; etiquetas de 10 pt y entradas de 11 pt. Los campos obligatorios llevan `*`. El formulario cambia sus campos segun Clientes, Proveedores o Empleados.
- **Acciones:** Guardar, Actualizar, Desactivar, Reactivar y Nuevo. Botones planos de 110 x 44; la accion principal usa fondo `#181818` y texto blanco. Solo se habilitan las acciones aplicables al registro seleccionado.
- **Listados:** tabla de solo lectura dentro de un panel blanco de radio 30 y borde `#F7F7F7`, mediante `PanelDisenoInventario.ColorBorde`. Seleccion de una fila, encabezados de 10 pt en negrita y altura de filas de 42. Un buscador filtra el listado y una casilla permite incluir inactivos.
- **Pestanas:** Clientes, Proveedores y Empleados comparten la misma distribucion. La pestana seleccionada usa la variante principal de boton; no se duplica la navegacion MDI.
- **Estados:** exito `#006400` y error `#B22222`, centralizados en `ProductoEstilo.Exito` y `ProductoEstilo.Error`. Siempre se acompanan de un mensaje; el estado no depende solo del color.
- **Dialogos:** captura modal con Guardar y Cancelar; desactivacion con motivo obligatorio de hasta 300 caracteres, confirmacion explicita y Cancelar. El dialogo de motivo conserva el estilo sin bordes y esquinas redondeadas, mediante `DialogosMaqueta.Redondear`.

## Organizacion de los cambios de formularios

`ConfiguracionAuditoria`, `DirectorioRoles`, `AgregarDirectorio` y `MotivoDesactivacion` conservan sus controles, columnas y propiedades iniciales en `InitializeComponent`, dentro de `.Designer.cs`, con su `.resx` junto al formulario. Los eventos, validaciones y llamadas a datos permanecen en `.cs`. Las variantes de presentacion por tipo de registro y los ajustes visuales dinamicos se encuentran en `.Presentacion.cs`. Las relaciones `DependentUpon` del proyecto anidan estos archivos en Visual Studio.

`Datos/Conexion.cs` conserva el namespace `Sistema_ERP.Clases` para mantener las referencias existentes. `Datos/DirectorioDatos.cs` y `Datos/TiendaDatos.cs` contienen consultas y persistencia; `Clases/TipoDirectorio.cs` define los tipos del directorio. La construccion de formularios y sus eventos de carga comprueban el modo de diseno antes de consultar la base de datos o transformar la navegacion.

## Directorio y tiendas

En **Directorio y roles**, las pestanas Clientes, Proveedores y Empleados permiten crear, consultar, actualizar, desactivar y reactivar registros. Seleccionar una fila carga sus campos; **Nuevo** limpia la seleccion para crear otro registro. Los inactivos se muestran como solo lectura y pueden reactivarse. Se validan los campos obligatorios, las longitudes y el correo; SQL Server conserva las restricciones de unicidad de documentos, RTN y codigos.

En **Configuracion y auditoria**, el CRUD de tiendas usa el mismo flujo, con codigo y nombre obligatorios, direccion y telefono opcionales. Las tiendas activas alimentan el selector de empleados. Un empleado requiere codigo y tienda; no se permiten nuevas asignaciones a una tienda inactiva, pero se puede conservar una asignacion previa al editar sus otros datos. Registrar un empleado no crea una cuenta de usuario ni asigna roles de acceso.

## Inventario, movimientos y traspasos

**Inventario y movimientos** abre ahora su propia pantalla; **Compras** conserva una opcion independiente. Selecciona una tienda y pulsa Consultar o Enter en el buscador para actualizar. Existencias muestra productos, cantidades fisicas, reservas, disponibilidad y stock minimo, incluyendo productos activos sin saldo y productos inactivos que conservan inventario. Movimientos muestra el libro de esa tienda, filtrado por texto y fechas UTC (ultimos 30 dias por defecto). El intervalo de fechas solo afecta al historial; las existencias siempre son actuales.

Seleccionar un producto activo de una tienda activa habilita **Registrar ajuste** y **Guardar minimo**. Un ajuste positivo ingresa unidades y uno negativo las retira; requiere motivo y confirmacion. Se registra un movimiento AJUSTE y el trigger actualiza la existencia sin tocar las reservas. No se permite retirar stock reservado ni dejar saldos negativos. Cambiar el minimo no genera movimientos de unidades. Ambos guardados verifican la version del saldo; ante cambios simultaneos hay que consultar otra vez.

En la pestana Movimientos, **Revertir ajuste** permite corregir exclusivamente un AJUSTE manual no revertido, con motivo. Inserta un REVERSO, conserva el original y no permite revertirlo dos veces. Las compras, ventas y traspasos se corrigen desde sus documentos, no desde este boton. Los movimientos no se editan ni eliminan.

**Traspaso entre tiendas** crea solicitudes PENDIENTE con tiendas distintas, productos sin repetir, cantidades enteras positivas y precios de referencia. **Validacion de traspasos** consulta por estado, muestra detalle e historial, edita pendientes, aprueba, rechaza, cancela, envia y confirma recepcion. Las acciones se habilitan segun el estado: aprobar reserva en origen; enviar un APROBADO libera la reserva y registra la salida, pasando a EN_TRANSITO; recibir registra la entrada en destino y pasa a RECIBIDO. Cancelar un PENDIENTE no mueve inventario y cancelar un APROBADO libera su reserva. No se cancela un traspaso en transito ni se vuelve a recibir uno finalizado.

Estas pantallas usan el usuario autenticado sin selector de operador. Cada escritura verifica en la misma transaccion la sesion, su vigencia, version de seguridad y estados de usuario y empleado; cambiar el identificador del operador no permite atribuir traspasos a otra cuenta. Se establece SESSION_CONTEXT para los triggers. Las operaciones requieren los triggers de Triggers2.sql habilitados: la aplicacion no duplica reservas, movimientos ni eventos y no instala scripts automaticamente. La autorizacion granular por rol/permiso y tienda sigue pendiente. Las comprobaciones locales y la integracion pendiente se detallan en [Datos/Traspasos-verificacion.md](Datos/Traspasos-verificacion.md).

## Usuarios, roles y permisos

La opcion **Usuarios, roles y permisos** del menu abre tres pestanas con captura en tres columnas, listado, buscador, inclusion de inactivos y las acciones Guardar, Actualizar, Desactivar, Reactivar y Nuevo. **Recargar** vuelve a consultar los datos. La pantalla reutiliza la navegacion MDI, ProductoEstilo, la tabla de filas de 42 y el panel de radio 30. Sus controles estan en `UsuariosRolesPermisos.Designer.cs`, sus variantes en `.Presentacion.cs` y su comportamiento en `.cs`, con `.resx` y relaciones `DependentUpon`.

- **Usuarios:** nombre de hasta 50 caracteres y empleado obligatorios. Cada empleado solo puede tener una cuenta, incluso si esta inactiva. Una nueva asignacion requiere un empleado activo; se puede conservar el empleado anterior si fue desactivado. La contrasena inicial requiere entre 12 y 128 caracteres y confirmacion; al actualizar, dejar ambos campos vacios conserva la clave. Se almacena un hash PBKDF2-SHA256 con 600000 iteraciones, sal aleatoria de 16 bytes y resultado de 32 bytes, en formato `PBKDF2-SHA256$iteraciones$salBase64$hashBase64`. No se consulta ni muestra el hash. Una clave nueva establece `debe_cambiar_contrasena=1`.
- **Roles:** codigo ASCII de hasta 30 caracteres, nombre obligatorio de hasta 80 y descripcion opcional de hasta 300. Las casillas permiten asignar o retirar permisos.
- **Permisos:** modulo ASCII de hasta 50 caracteres, accion ASCII de hasta 30 y descripcion obligatoria de hasta 300. El esquema calcula el codigo `modulo.accion` y exige una combinacion unica.

Las casillas de Usuarios asignan roles. Las asignaciones existentes a entidades inactivas aparecen identificadas y se pueden conservar o retirar; no se permiten nuevas asignaciones a entidades inactivas. Retirar una asignacion cambia su `activo` almacenado, sin borrar la relacion. Guardar los datos y sus asignaciones es una sola transaccion; las actualizaciones y cambios de estado comprueban `version_fila` para detectar cambios simultaneos. Un conflicto requiere **Recargar** antes de reintentar.

Desactivar requiere motivo y confirmacion mediante MotivoDesactivacion. Los inactivos son de solo lectura y conservan sus relaciones. Reactivar limpia fecha, usuario y motivo de desactivacion; no se escribe en el `activo` calculado de Usuario, Rol ni Permiso. Se utilizan las tablas del esquema 3FN existente, sin migraciones ni modificaciones automaticas. SQL Server conserva las restricciones de unicidad y claves foraneas.

La aplicacion inicia en **Iniciar sesion** y abre la ventana principal solo despues de validar usuario y contrasena contra SQL Server. Los roles se obtienen de las asignaciones activas; no se eligen en el login. Se rechazan cuentas y empleados inactivos. Cinco fallos bloquean la cuenta durante 15 minutos; se registra cada intento en EventoAcceso. Una clave temporal exige cambiarla y confirmarla antes de entrar. Los hashes demo o de formatos distintos de PBKDF2-SHA256 no se aceptan.

Cada entrada completa crea una SesionUsuario de ocho horas y muestra el nombre del empleado y sus roles. Cada minuto se verifica vigencia, revocacion, estado del empleado y version de seguridad; si falla la verificacion se vuelve al acceso. **Cerrar sesion** revoca la sesion y permite ingresar otra cuenta. Cerrar la ventana principal termina la aplicacion. `Base de Datos/Triggers2.sql`, cuando esta instalado, invalida sesiones al cambiar claves, estados o asignaciones. La autorizacion por permiso de cada operacion y la atribucion de actores en los otros CRUD siguen pendientes; autenticar no restringe aun las pantallas por rol.

En una base con el esquema instalado y **sin usuarios**, el primer intento de ingreso crea automaticamente la cuenta temporal **usuario: `admin` / contrasena: `admin`**, con el rol ADMINISTRADOR. No se necesita un script de PowerShell. La clave se almacena con hash y **debe cambiarse antes de entrar** por una nueva de 12 a 128 caracteres, confirmada y diferente de `admin`.

El arranque crea el empleado `ADMIN-INICIAL` si no existe, asociado a la primera tienda activa; si no hay tiendas activas crea `INICIAL`. Tambien crea el rol ADMINISTRADOR si falta. No reactiva registros iniciales desactivados. Todo se realiza en una sola transaccion con bloqueo para evitar cuentas duplicadas al iniciar varias instancias. Si ya existe cualquier usuario, incluso inactivo, **no crea ni restablece** `admin` ni cambia sus claves. Las cuentas posteriores se crean con el CRUD. La aplicacion no instala el esquema automaticamente. La preparacion y las pruebas pendientes se describen en [Datos/Seguridad-verificacion.md](Datos/Seguridad-verificacion.md).

## Desactivacion de registros y esquema requerido

La version actual espera el esquema normalizado con `desactivado_en`, `desactivado_por`, `motivo_desactivacion`, `actualizado_en` y `activo` calculado a partir de `desactivado_en` en Producto, Categoria, Proveedor, Cliente, Empleado y Tienda. Categoria requiere `descripcion`; Proveedor requiere tambien `rtn`, `correo` y `direccion`. La consulta de productos utiliza `dbo.vw_ProductoStock` y las existencias de `InventarioTienda`.

El archivo `Datos/Migraciones/001_desactivar_registros.sql` corresponde al esquema anterior: agrega un `activo` almacenado a Producto, Categoria y Proveedor. No es una migracion completa al esquema normalizado ni debe usarse como unico paso para preparar esta version. Una base existente con el esquema anterior requiere una migracion que preserve sus datos; el script de creacion `01_esquema_erp_3fn.sql` esta destinado a una base vacia. La aplicacion no modifica el esquema automaticamente.

**Desactivar** establece fecha y motivo de desactivacion; **Reactivar** limpia fecha, usuario y motivo. No se escribe directamente en la columna calculada `activo`. Se conservan los registros, sus relaciones y los identificadores reservados. Desactivar una entidad no desactiva en cascada sus registros relacionados ni modifica el historial. Los listados muestran activos por defecto; los CRUD de tiendas y directorio permiten incluir inactivos y reactivarlos.
