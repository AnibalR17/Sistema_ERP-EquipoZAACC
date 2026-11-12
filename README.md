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
  Administracion/  Usuarios, permisos, directorio, tiendas y auditoria
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

## Acceso por roles y permisos

Para una base existente, **Roles → Pantallas y presets…** prepara automáticamente los permisos base que falten al abrirse con permiso para gestionar seguridad. También puede ejecutarse `Datos/Migraciones/005_permisos_contenido.sql`. Las bases nuevas incluyen el catálogo en el esquema. La preparación conserva las asignaciones y no otorga privilegios automáticamente.

Desde **Usuarios, roles y permisos**, una cuenta con el rol activo `ADMINISTRADOR` puede asignar a cada rol los permisos `MODULO.CONSULTAR` y `MODULO.GESTIONAR`. Gestionar requiere también consultar. Las cuentas sin permisos solo ven Inicio sin información de negocio. El menú, las acciones, el resumen y las consultas de datos aplican esas asignaciones; el administrador conserva acceso total. El alcance es por módulo, abarcando sus tiendas. Consultar [verificación y alcance de la autorización](Datos/Autorizacion-verificacion.md).

En **Roles → Pantallas y presets…**, selecciona cada pantalla por su nombre y marca `CONSULTAR` para abrirla o `GESTIONAR` para modificar y operar. El detalle indica qué incluye cada módulo; gestionar agrega la consulta y quitar la consulta retira la gestión. Puedes aplicar un preset y luego ajustar sus pantallas:

| Preset | Acceso inicial |
| --- | --- |
| Administrador | Consulta y gestión de las 12 pantallas / módulos. |
| Cajero | Gestiona ventas, apartados y devoluciones; consulta productos e inventario. |
| Gerente | Consulta y gestión de los 10 módulos operativos, incluida la validación de traspasos y el directorio. |
| Analista de datos | Consulta ventas, compras, inventario, productos, proveedores, traspasos, validación, apartados y devoluciones, con sus exportaciones disponibles. |

**Aplicar preset** reemplaza la selección completa sin guardar. **Aceptar** lleva los permisos al formulario; **Guardar** o **Actualizar** los conserva. Para configurar un rol existente, selecciona su fila primero. Para crear otro, pulsa **Nuevo**; al aceptar un preset se completan los campos vacíos y se propone un código disponible, incluso si ya existe un rol con ese código base. El preset Administrador propone `ADMIN_PERSONALIZADO`, que admite restricciones; el código reservado `ADMINISTRADOR` conserva siempre acceso total. Los presets no reactivan permisos ni conceden acciones desconocidas; si faltan permisos activos se informa la selección incompleta.

En **Usuarios**, elige un rol en el desplegable y pulsa **Agregar**. La lista conserva todos los roles asignados; selecciona uno y pulsa **Quitar** para retirarlo. En **Generar permisos…** puedes ajustar los mismos roles y consultar las pantallas que aportan. **Aceptar** aplica la selección al formulario; **Guardar** o **Actualizar** la conserva junto con el usuario. **Cancelar** conserva la selección anterior. Los roles inactivos ya asignados se conservan y pueden retirarse, pero no agregarse; no conceden permisos. `ADMINISTRADOR` muestra acceso total. La asignación requiere permiso para gestionar seguridad. Los permisos de varios roles se suman: para restringir una cuenta, retira los otros roles que concedan ese acceso.

En **Pantallas y presets…**, **Seleccionar módulo** incluye todos los permisos activos del módulo mostrado y **Quitar del módulo** retira solamente los de ese módulo. **Quitar todos** vacía la selección completa para empezar sin accesos.

## Busqueda global

La barra superior del MDI permite buscar en todos los módulos autorizados: nombres, códigos, contactos y números de documentos. Escribe dos caracteres o más; **Enter** busca inmediatamente, **Ctrl+K** enfoca la barra y **Escape** limpia la búsqueda. Los resultados permiten consultar detalles de solo lectura o abrir su módulo. El selector de tienda filtra los documentos y las existencias; los catálogos compartidos conservan su alcance. Ver [búsqueda global y verificación](Datos/BusquedaGlobal-verificacion.md).

## Conexion a la base de datos

Configure la variable de entorno `SISTEMA_ERP_CONNECTION_STRING` o cree `connection.local.txt` en la raiz del proyecto con la cadena de conexion de SQL Server. La variable de entorno tiene prioridad. El archivo local se copia al directorio de salida y esta excluido de Git para proteger las credenciales.

En Inicio, haga clic en **Probar conexion** para comprobar el acceso a la base de datos. Se mostrara un mensaje de exito o error sin bloquear la interfaz.

Inicio muestra las ventas y compras confirmadas del dia y un grafico de los ultimos siete dias, filtrados por el selector de tienda de la cabecera. Las tarjetas abren los modulos correspondientes. El resumen se actualiza al regresar, al cambiar de tienda, con **Reiniciar** y cada minuto mientras Inicio esta visible. Consulte [Datos/Inicio-verificacion.md](Datos/Inicio-verificacion.md) para criterios y pruebas.

Todas las pantallas que consultan datos incluyen **Reiniciar** para recargarlos manualmente, también en sus pestañas y ventanas de detalle. La acción está disponible con permiso de consulta y vuelve a usar la autorización del módulo. En los editores de captura actualiza los catálogos; las ventanas de detalle consultan nuevamente el registro. Ver [alcance y pruebas de la recarga manual](Datos/RecargaManual-verificacion.md).

### Como obtener el connection string

Puede obtener la cadena desde Visual Studio y guardarla en la configuracion local del proyecto:

1. Abra **Ver > Explorador de servidores** y, en **Conexiones de datos**, seleccione **Agregar conexion**.
2. Seleccione Microsoft SQL Server como origen de datos. Ingrese el nombre del servidor y su instancia, tal como los utiliza para conectarse desde SQL Server Management Studio (por ejemplo, `localhost\SQLEXPRESS`).
3. Seleccione la autenticacion correspondiente: Windows o SQL Server. Si utiliza SQL Server, ingrese el usuario y la contrasena de su entorno.
4. Seleccione la base de datos del ERP. El script `Base de Datos/01_esquema_erp_3fn.sql` crea la base `EquipoZAACC`; use ese nombre si preparo la base con dicho script.
5. Pulse **Probar conexion** y acepte. Seleccione la conexion creada y abra **Propiedades** para copiar el valor de **Cadena de conexion / Connection String**. Si la contrasena no aparece, debera completarla localmente cuando use autenticacion SQL Server.
6. Pegue la cadena completa en `connection.local.txt`, ubicado junto a `Sistema_ERP.csproj`, en una sola linea, sin comillas externas ni prefijos como `ConnectionString=`. Vuelva a compilar para copiar el archivo al directorio de salida.

Ejemplo con autenticacion de Windows (reemplace el servidor por el suyo):

```text
Server=localhost\SQLEXPRESS;Database=EquipoZAACC;Integrated Security=True;Encrypt=True;
```

Con autenticacion de SQL Server, sustituya `Integrated Security=True` por `User ID=TU_USUARIO;Password=TU_CONTRASENA`. Si el servidor local de desarrollo usa un certificado autofirmado y aparece un error de confianza del certificado, puede agregar `TrustServerCertificate=True` para ese entorno; en produccion utilice un certificado valido.

Tambien puede guardar la cadena en la variable de entorno `SISTEMA_ERP_CONNECTION_STRING`. Si la define desde Windows, reinicie Visual Studio o la terminal desde donde ejecuta la aplicacion para que reciba el nuevo valor. Esta variable tiene prioridad sobre el archivo local.

`Datos/Conexion.cs` obtiene primero la variable de entorno y, si esta vacia o no existe, lee `connection.local.txt` desde el directorio del ejecutable. No publique cadenas con credenciales en el README ni en archivos versionados; el archivo local ya esta excluido de Git.

Los traspasos requieren el esquema 3FN y `Base de Datos/Triggers2.sql`, basado en la base proporcionada. Ejecute los triggers en la misma base configurada para la aplicacion. La base genera el historial y reserva inventario al aprobar; las tiendas de una solicitud creada no se cambian. La preparacion y las pruebas se describen en [Datos/Traspasos-verificacion.md](Datos/Traspasos-verificacion.md).

Aplicacion de escritorio desarrollada con C# y Windows Forms sobre .NET 10 para Windows.

## Pruebas

`dotnet run --project Pruebas/FlujoVentas.Integracion -- --sin-bd` verifica los formatos de precios, el selector de roles, los cambios de tema, el contraste, las preferencias de apariencia y la distribución de botones, etiquetas y campos sin conexión a SQL Server.

`dotnet run --project Pruebas/FlujoVentas.Integracion -- --disposicion` comprueba 14 pantallas con dos anchos de ventana y métricas simuladas de 100 %, 125 %, 150 % y 200 %. Añade `--rapida` para comprobar solo 900 px al 100 %, o `--capturas` para guardar imágenes de pantallas representativas en `capturas-disposicion` dentro de la salida de las pruebas. Estas comprobaciones no cambian la escala de Windows ni requieren SQL Server.

`dotnet run --project Pruebas/FlujoVentas.Integracion -- --cierre-sesion` verifica, sin SQL Server, el cierre por el botón y por la X con Compras en segundo plano. Las consultas pendientes permiten salir; una escritura en curso informa que debe terminar antes del cierre. Si el cierre se cancela, se conserva la sesión actual.

`dotnet run --project Pruebas/FlujoVentas.Integracion -- --validacion-ceros` verifica el rechazo de campos de texto formados únicamente por ceros, tanto en altas y actualizaciones como en la previsualización de Excel y los diálogos de motivo y cambio de contraseña, sin requerir SQL Server.

`dotnet run --project Pruebas/FlujoVentas.Integracion -- --validacion-contactos` comprueba el formato de los correos y el mínimo de tres letras en nombres, tanto al crear y actualizar registros como al previsualizar Excel, sin SQL Server.

`dotnet run --project Pruebas/FlujoVentas.Integracion` ejecuta también la integración completa. Requiere la conexión local configurada y permisos para crear una base temporal de pruebas, que se elimina al terminar.

## Validación de datos

Los nombres, códigos, datos de contacto, identificadores, números de factura, referencias de pago, motivos, observaciones, RTN, CAI y nuevas contraseñas rechazan valores compuestos solo por ceros. La comprobación también reconoce espacios y separadores habituales, como `0000-0000` o `(000) 000-0000`. Los mensajes indican el campo que debe corregirse y la carga masiva señala el error en cada fila. La validación se aplica antes de persistir los datos mediante `Clases/ValidacionDatos.cs`.

Se conservan los ceros iniciales de datos válidos, como `000123`. Los campos opcionales pueden permanecer vacíos y los valores numéricos conservan sus reglas de negocio: por ejemplo, el stock inicial, el mínimo de inventario y la tasa de un producto exento pueden ser cero. La consulta y el acceso a cuentas existentes siguen disponibles para corregir registros anteriores.

Los nombres de clientes, empleados, proveedores, contactos, productos, categorías, tiendas, usuarios y roles deben contener al menos tres letras. Se cuentan letras con acentos; los espacios, dígitos y signos no cuentan para el mínimo. Por ejemplo, `Ana`, `José` y `USB 2.0` son válidos; `Al` y `A12` requieren corrección. El contacto del proveedor continúa siendo opcional.

Los correos opcionales pueden quedar vacíos. Si se ingresan, deben ser una sola dirección con formato `usuario@dominio.com`, sin espacios internos, con usuario y dominio completos. Se admiten alias con `+`, subdominios y dominios internacionales. Se rechazan direcciones incompletas, puntos consecutivos, nombres de dominio mal formados y valores que excedan la longitud admitida por el campo. Los mismos criterios se aplican a clientes, proveedores y empleados.

## Estandar grafico

El estandar parte de `Formularios/Principal/Pagina_Central.Designer.cs` y `Formularios/Principal/Pagina_Central.cs`. Las variantes de captura, tablas y dialogos utilizadas por Configuracion y Directorio se documentan mas abajo para mantener coherencia entre pantallas.

### 1. Identidad visual

Interfaz de escritorio clara: fondos blancos y gris azulado, acciones azules, texto oscuro, tipografia Segoe UI, navegacion lateral y paneles con esquinas redondeadas. La jerarquia se establece mediante color, tamano, negrita y espaciado.

La marca se presenta como texto **Sistema ERP** con acento azul. Las paletas clara y oscura se definen en `Presentacion/ProductoEstilo.cs`; `TemaAplicacion` las aplica al acceso, las pantallas MDI, sus controles dinamicos y los dialogos. `FormularioTematico` aplica el tema antes de mostrar cada ventana y `PestanasTematicas` adapta las cabeceras de las pestañas.

En **Configuración y auditoría → Apariencia**, selecciona **Claro (azul y gris)** u **Oscuro (estilo VS Code)** y pulsa **Aplicar tema**. Cambia las ventanas abiertas sin reiniciar ni perder la captura. La preferencia se guarda por usuario de Windows en `%LOCALAPPDATA%\Sistema_ERP\apariencia.json` y se recupera antes de mostrar el acceso. Si no existe o está dañada, se usa el tema claro. Si no se puede guardar, se conserva el tema anterior y se muestra un mensaje.

### 2. Paleta de colores

| Uso implementado | HEX | RGB |
| --- | --- | --- |
| Menu, barra superior, tarjetas y botones secundarios | `#FFFFFF` | 255, 255, 255 |
| Fondo del area principal | `#F4F7FB` | 244, 247, 251 |
| Texto principal | `#1F2937` | 31, 41, 55 |
| Texto secundario y descripciones | `#475569` | 71, 85, 105 |
| Franja informativa y encabezados de tablas | `#E8EEF5` | 232, 238, 245 |
| Acciones principales y texto de botones secundarios | `#1D4ED8` | 29, 78, 216 |
| Accion principal al pasar el puntero | `#1E40AF` | 30, 64, 175 |
| Seleccion del menu y filas de tablas | `#DBEAFE` | 219, 234, 254 |
| Texto de la seleccion | `#1E3A8A` | 30, 58, 138 |
| Bordes y separadores | `#B8C5D6` | 184, 197, 214 |
| Fondo de botones deshabilitados | `#E2E8F0` | 226, 232, 240 |
| Texto de botones deshabilitados | `#475569` | 71, 85, 105 |

Las acciones principales usan texto blanco. Los botones deshabilitados conservan `Enabled = false` y dibujan su texto con contraste en ambos temas para evitar el atenuado de WinForms. Al habilitarse recuperan el estilo del tema actual. `ColoresBoton` permite cambiar colores sin modificar las medidas del disenador.

El tema oscuro se inspira en [Dark Modern de VS Code](https://github.com/microsoft/vscode/blob/main/extensions/theme-defaults/themes/dark_modern.json):

| Uso | HEX |
| --- | --- |
| Fondo principal | `#1F1F1F` |
| Navegación y cabeceras | `#181818` |
| Tarjetas y superficies | `#252526` |
| Campos | `#313131` |
| Texto principal / secundario | `#CCCCCC` / `#9D9D9D` |
| Acciones principales / hover | `#0078D4` / `#026EC1` |
| Selección | `#264F78` |
| Bordes | `#3C3C3C` |
| Botones deshabilitados: fondo / texto | `#2D2D2D` / `#A0A0A0` |

Los controles nativos usan además el modo de color de Windows Forms cuando Windows lo admite. Las ventanas del sistema (por ejemplo, impresión y selección de archivos) conservan el estilo que ofrece Windows.

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

Los botones de acción comparten una altura mínima de 44 y un ancho mínimo de 120 unidades a 96 DPI; el ancho aumenta según el texto. `DisposicionControles` adapta las filas a las fuentes, los márgenes y la altura nativa de los desplegables. Las barras de acciones pueden ocupar varias filas al reducir la ventana. La aplicación utiliza `PerMonitorV2` y vuelve a ajustar la distribución cuando cambia el DPI del monitor.

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

- **Navegacion lateral:** botones planos de 236 × 40, sin borde, fondo blanco y relleno izquierdo de 10. La pantalla actual se distingue con negrita, fondo `#DBEAFE` y texto `#1E3A8A`. Al pasar el puntero se aplica el mismo azul suave.
- **Grupos de navegacion:** Descubrir y Administracion, con encabezados en negrita.
- **Sesion:** nombre de usuario, rol en gris oscuro y boton Cerrar sesion de 216 × 38 con fondo blanco, texto azul y borde visible.
- **Barra superior:** boton ☰ de 36 × 30, buscador sin borde, nombre de cuenta con simbolo ⌄ y boton de notificaciones de 64 × 30 para mostrar campana y contador. Los botones de simbolos tienen borde de 1.
- **Buscador:** ancho ajustado al espacio disponible; texto de ayuda «Buscar en todo el ERP… (Ctrl+K)».
- **Franja informativa:** texto secundario, simbolo ℹ y relleno horizontal de 16 a la izquierda y 8 a la derecha.
- **Indicadores:** titulo gris, valor grande en negrita y detalle gris. El ejemplo monetario usa `L. 0.00`; no hay formateador monetario centralizado.
- **Grafico de Inicio:** ventas y compras confirmadas de los ultimos siete dias, con fechas, importes en lempiras, cuadricula y leyenda. Las ventas usan linea oscura continua y las compras una linea azul discontinua. Los valores se consultan en SQL Server para la tienda seleccionada o todas las tiendas.

Los simbolos son caracteres Unicode o emoji, no imagenes de una biblioteca de iconos. Su apariencia puede variar segun las fuentes disponibles.

### 6. Reglas para extender el diseno

Estas reglas se proponen a partir de los patrones existentes:

1. Usar Segoe UI y conservar la jerarquia tipografica descrita.
2. Mantener menu lateral, barra superior y margenes del contenido en las nuevas pantallas.
3. Usar blanco para superficies, gris oscuro para informacion secundaria y azul para acciones; reservar las negritas para titulos, valores y seleccion del menu.
4. Reutilizar radios de 30 en paneles principales y 18 en franjas informativas.
5. Mantener navegacion plana, texto a la izquierda y respuesta visual al pasar el puntero.
6. Declarar controles y propiedades editables en `InitializeComponent`, con sus campos en `.Designer.cs`; dejar comportamiento y dibujo personalizado en `.cs`.
7. Documentar cualquier nuevo color de estado, control o variante antes de extenderlo a otras pantallas.

### 7. Limites del estandar actual

- `Presentacion/ProductoEstilo.cs` concentra colores y botones compartidos. Las propiedades editables y los valores de distribucion se conservan en cada `.Designer.cs`.
- Las esquinas redondeadas y el grafico se dibujan durante la ejecucion y pueden no aparecer completos en el disenador.
- La implementacion no dibuja las sombras de la imagen de referencia.
- Las variantes de tablas, captura, dialogos, validacion y acciones principales se describen en la seccion 8. El tema oscuro se elige en Configuración → Apariencia.
- Las reglas visuales de esta sección acompañan los flujos implementados; la búsqueda global se describe en `Datos/BusquedaGlobal-verificacion.md`.

### 8. Variantes de captura y CRUD

- **Superficie:** fondo blanco, margenes de contenido de 32, 34, 32 y 36; el menu y el encabezado compartidos los aporta la ventana MDI. Configuracion y Directorio desplazan su contenido de forma independiente.
- **Captura:** campos distribuidos en tres columnas; etiquetas de 10 pt y entradas de 11 pt. Los campos obligatorios llevan `*`. El formulario cambia sus campos segun Clientes, Proveedores o Empleados.
- **Acciones:** Guardar, Actualizar, Desactivar, Reactivar y Nuevo. Botones planos de 110 x 44; la accion principal usa fondo `#1D4ED8` y texto blanco. Las acciones no disponibles usan fondo `#E2E8F0`, texto `#475569` y borde visible. Solo se habilitan las acciones aplicables al registro seleccionado.
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

## Compras y detalles

**Compras** abre una pantalla con las pestanas Captura e Historial dentro de la navegacion MDI. Selecciona proveedor y tienda activos, indica numero y fecha de factura y agrega productos activos con cantidad entera positiva y costo unitario positivo de hasta dos decimales. El costo de compra es independiente del precio de venta. Seleccionar una linea permite **Actualizar linea** o **Quitar linea**; los subtotales y el total se calculan automaticamente en lempiras. No se repiten productos ni se aceptan importes fuera de decimal(18,2). El esquema actual no contempla impuestos, descuentos ni pagos de compras.

**Guardar borrador** guarda cabecera y detalles en una sola transaccion sin mover inventario. Un borrador permite editar factura, fecha, observaciones y lineas; proveedor, tienda y autor se conservan segun los triggers. La factura admite 50 caracteres, las observaciones 500 y no puede repetirse el numero para un mismo proveedor, incluso si la compra anterior fue anulada. **Nuevo** limpia la captura y **Reiniciar** actualiza los catalogos e inicia otra captura; ambas acciones piden confirmacion si hay cambios pendientes.

**Confirmar compra** requiere un borrador guardado sin cambios pendientes, con proveedor, tienda y productos activos. Tras confirmar, los triggers registran las entradas COMPRA en inventario y el detalle queda de solo lectura. **Anular compra** requiere motivo de hasta 500 caracteres y confirmacion: un borrador no mueve inventario; una confirmada genera REVERSO de sus entradas. Si el stock disponible no alcanza para revertir sin afectar reservas, toda la anulacion se rechaza. No se eliminan documentos ni se editan compras confirmadas o anuladas.

En **Historial**, Consultar o Enter busca por numero, proveedor, tienda, observaciones o identificador, con filtro de estado y fechas de factura inclusivas (ultimos 30 dias por defecto). **Abrir detalle** o doble clic carga el documento completo, incluidos productos y referencias que luego fueron desactivados. Los costos guardados se conservan. Para resolver un conflicto, vuelve a consultar y abrir la compra; la aplicacion compara una huella SHA256 de cabecera y lineas bajo bloqueo transaccional porque Compra no tiene rowversion en el esquema proporcionado.

Las escrituras comprueban la sesion autenticada dentro de la transaccion y establecen SESSION_CONTEXT para movimientos y auditoria. Se requieren el esquema 3FN y los triggers de compras e inventario de `Base de Datos/Triggers2.sql`, habilitados con nested triggers activo y RECURSIVE_TRIGGERS desactivado; la aplicacion comprueba estas dependencias sin instalar scripts ni duplicar sus efectos. Los controles y columnas se declaran en `.Designer.cs`, los estilos compartidos se aplican desde `.Presentacion.cs` y la persistencia reside en `Datos/CompraDatos.cs`. La autorizacion granular por permiso y tienda sigue pendiente. Las comprobaciones locales y los casos SQL pendientes estan en [Datos/Compras-verificacion.md](Datos/Compras-verificacion.md).

## Inventario, movimientos y traspasos

**Inventario y movimientos** abre ahora su propia pantalla; **Compras** conserva una opcion independiente. Selecciona una tienda y pulsa Consultar o Enter en el buscador para actualizar. Existencias muestra productos, cantidades fisicas, reservas, disponibilidad y stock minimo, incluyendo productos activos sin saldo y productos inactivos que conservan inventario. Movimientos muestra el libro de esa tienda, filtrado por texto y fechas UTC (ultimos 30 dias por defecto). El intervalo de fechas solo afecta al historial; las existencias siempre son actuales.

Inventario permite consultar existencias y movimientos; no ofrece acciones para ingresar, retirar ni revertir unidades manualmente. Las entradas y salidas se registran desde Compras y Ventas, y los traspasos conservan su flujo documental entre tiendas. Seleccionar un producto activo de una tienda activa habilita **Guardar minimo**, que cambia exclusivamente el umbral de aviso de stock y no genera movimientos de unidades. El guardado verifica la version del saldo; ante cambios simultaneos hay que consultar otra vez.

La pestana Movimientos conserva el historial, incluidos los ajustes manuales anteriores. Los movimientos no se editan ni eliminan. Las correcciones de compras, ventas y traspasos se realizan desde sus respectivos documentos.

**Traspaso entre tiendas** crea solicitudes PENDIENTE con tiendas distintas, productos sin repetir, cantidades enteras positivas y precios de referencia. **Validacion de traspasos** consulta por estado, muestra detalle e historial, edita pendientes, aprueba, rechaza, cancela, envia y confirma recepcion. Las acciones se habilitan segun el estado: aprobar reserva en origen; enviar un APROBADO libera la reserva y registra la salida, pasando a EN_TRANSITO; recibir registra la entrada en destino y pasa a RECIBIDO. Cancelar un PENDIENTE no mueve inventario y cancelar un APROBADO libera su reserva. No se cancela un traspaso en transito ni se vuelve a recibir uno finalizado.

Estas pantallas usan el usuario autenticado sin selector de operador. Cada escritura verifica en la misma transaccion la sesion, su vigencia, version de seguridad y estados de usuario y empleado; cambiar el identificador del operador no permite atribuir traspasos a otra cuenta. Se establece SESSION_CONTEXT para los triggers. Las operaciones requieren los triggers de Triggers2.sql habilitados: la aplicacion no duplica reservas, movimientos ni eventos y no instala scripts automaticamente. La autorizacion granular por rol/permiso y tienda sigue pendiente. Las comprobaciones locales y la integracion pendiente se detallan en [Datos/Traspasos-verificacion.md](Datos/Traspasos-verificacion.md).

## Notificaciones de inventario

La campana del encabezado consulta el inventario real y muestra la cantidad de alertas por producto y tienda; a partir de diez muestra `9+` y el total completo aparece al pasar el puntero. Se actualiza al iniciar la ventana principal, cada minuto y al abrir o actualizar Notificaciones. Una alerta corresponde a **stock disponible menor o igual al minimo**: disponible es existencia menos reservas. Incluye productos y tiendas activos con un saldo registrado en InventarioTienda; un saldo disponible de cero con minimo cero tambien genera alerta. El minimo se configura desde Inventario y no cambia las unidades.

El dialogo muestra tienda, codigo, producto, existencia, reservas, disponible y minimo en una tabla de solo lectura. Al reponer existencias, liberar reservas o cambiar el minimo, la alerta desaparece en la siguiente consulta si el disponible supera el umbral. Si falla la consulta, la campana muestra `?`, el dialogo informa el error y **Reiniciar** permite reintentar; una consulta fallida no se presenta como ausencia de alertas.

Son alertas actuales, sin estados de leido ni bandeja historica. La consulta valida la sesion y no crea eventos, notificaciones almacenadas ni movimientos; funciona sobre el esquema existente sin depender de destinatarios aun no asignados a EventoNotificacion. Las tiendas consultables siguen la misma visibilidad general de Inventario; la autorizacion granular por permiso y tienda permanece pendiente. Los controles y columnas del dialogo estan en `Notificaciones.Designer.cs`. La comprobacion local y los casos SQL pendientes se documentan en [Datos/Notificaciones-verificacion.md](Datos/Notificaciones-verificacion.md).

## Usuarios, roles y permisos

La opcion **Usuarios, roles y permisos** del menu abre dos pestanas: **Usuarios** y **Roles**, con captura en tres columnas, listado, buscador, inclusion de inactivos y las acciones Guardar, Actualizar, Desactivar, Reactivar y Nuevo. **Reiniciar** vuelve a consultar los datos. Sus controles estan en `UsuariosRolesPermisos.Designer.cs`, sus variantes en `.Presentacion.cs` y su comportamiento en `.cs`.

- **Usuarios:** nombre de hasta 50 caracteres y empleado obligatorios. Cada empleado solo puede tener una cuenta, incluso si esta inactiva. Una nueva asignacion requiere un empleado activo; se puede conservar el empleado anterior si fue desactivado. La contrasena inicial requiere entre 12 y 128 caracteres y confirmacion; al actualizar, dejar ambos campos vacios conserva la clave. Se almacena un hash PBKDF2-SHA256 con 600000 iteraciones, sal aleatoria de 16 bytes y resultado de 32 bytes, en formato `PBKDF2-SHA256$iteraciones$salBase64$hashBase64`. No se consulta ni muestra el hash. Una clave nueva establece `debe_cambiar_contrasena=1`.
- **Roles:** codigo ASCII de hasta 30 caracteres, nombre obligatorio de hasta 80 y descripcion opcional de hasta 300. Las casillas permiten asignar o retirar permisos. **Pantallas y presets…** ofrece las selecciones por pantalla y los cuatro perfiles.

Desde **Roles → Crear / editar permisos…** se pueden crear permisos con módulo, acción y descripción, buscar, actualizar, desactivar y reactivar registros del catálogo compartido. Módulo admite hasta 50 caracteres ASCII, acción hasta 30 y descripción hasta 300. El código `modulo.accion` es único. **Guardar permiso** conserva el permiso en el catálogo; **Volver a Roles** prepara la selección de los permisos nuevos sin borrar los datos del rol. **Guardar** o **Actualizar** en Roles conserva esa asignación. Una gestión nueva incluye la consulta activa de su módulo; si esta no existe, el permiso de gestión se conserva en el catálogo sin seleccionarse. Los permisos personalizados solo controlan operaciones que los validen en código; los accesos actuales usan `CONSULTAR` y `GESTIONAR`.

Las casillas de Usuarios asignan roles. Las asignaciones existentes a entidades inactivas aparecen identificadas y se pueden conservar o retirar; no se permiten nuevas asignaciones a entidades inactivas. Retirar una asignacion cambia su `activo` almacenado, sin borrar la relacion. Guardar los datos y sus asignaciones es una sola transaccion; las actualizaciones y cambios de estado comprueban `version_fila` para detectar cambios simultaneos. Un conflicto requiere **Reiniciar** antes de reintentar.

Desactivar requiere motivo y confirmacion mediante MotivoDesactivacion. Los inactivos son de solo lectura y conservan sus relaciones. Reactivar limpia fecha, usuario y motivo de desactivacion; no se escribe en el `activo` calculado de Usuario, Rol ni Permiso. Se utilizan las tablas del esquema 3FN existente sin cambios automaticos de esquema; abrir **Pantallas y presets…** completa los registros faltantes del catalogo de permisos. SQL Server conserva las restricciones de unicidad y claves foraneas.

La aplicacion inicia en **Iniciar sesion** y abre la ventana principal solo despues de validar usuario y contrasena contra SQL Server. Los roles se obtienen de las asignaciones activas; no se eligen en el login. Se rechazan cuentas y empleados inactivos. Cinco fallos bloquean la cuenta durante 15 minutos; se registra cada intento en EventoAcceso. Una clave temporal exige cambiarla y confirmarla antes de entrar. Los hashes demo o de formatos distintos de PBKDF2-SHA256 no se aceptan.

Cada entrada completa crea una SesionUsuario de ocho horas y muestra el nombre del empleado y sus roles. Cada minuto se verifica vigencia, revocacion, estado del empleado y version de seguridad; si falla la verificacion se vuelve al acceso. **Cerrar sesion** revoca la sesion y permite ingresar otra cuenta. Cerrar la ventana principal termina la aplicacion. `Base de Datos/Triggers2.sql`, cuando esta instalado, invalida sesiones al cambiar claves, estados o asignaciones. La autorizacion por permiso de cada operacion y la atribucion de actores en los otros CRUD siguen pendientes; autenticar no restringe aun las pantallas por rol.

En una base con el esquema instalado y **sin usuarios**, el primer intento de ingreso crea automaticamente la cuenta temporal **usuario: `admin` / contrasena: `admin`**, con el rol ADMINISTRADOR. No se necesita un script de PowerShell. La clave se almacena con hash y **debe cambiarse antes de entrar** por una nueva de 12 a 128 caracteres, confirmada y diferente de `admin`.

El arranque crea el empleado `ADMIN-INICIAL` si no existe, asociado a la primera tienda activa; si no hay tiendas activas crea `INICIAL`. Tambien crea el rol ADMINISTRADOR si falta. No reactiva registros iniciales desactivados. Todo se realiza en una sola transaccion con bloqueo para evitar cuentas duplicadas al iniciar varias instancias. Si ya existe cualquier usuario, incluso inactivo, **no crea ni restablece** `admin` ni cambia sus claves. Las cuentas posteriores se crean con el CRUD. La aplicacion no instala el esquema automaticamente. La preparacion y las pruebas pendientes se describen en [Datos/Seguridad-verificacion.md](Datos/Seguridad-verificacion.md).

## Desactivacion de registros y esquema requerido

La version actual espera el esquema normalizado con `desactivado_en`, `desactivado_por`, `motivo_desactivacion`, `actualizado_en` y `activo` calculado a partir de `desactivado_en` en Producto, Categoria, Proveedor, Cliente, Empleado y Tienda. Categoria requiere `descripcion`; Proveedor requiere tambien `rtn`, `correo` y `direccion`. La consulta de productos utiliza `dbo.vw_ProductoStock` y las existencias de `InventarioTienda`.

El archivo `Datos/Migraciones/001_desactivar_registros.sql` corresponde al esquema anterior: agrega un `activo` almacenado a Producto, Categoria y Proveedor. No es una migracion completa al esquema normalizado ni debe usarse como unico paso para preparar esta version. Una base existente con el esquema anterior requiere una migracion que preserve sus datos; el script de creacion `01_esquema_erp_3fn.sql` esta destinado a una base vacia. La aplicacion no modifica el esquema automaticamente.

**Desactivar** establece fecha y motivo de desactivacion; **Reactivar** limpia fecha, usuario y motivo. No se escribe directamente en la columna calculada `activo`. Se conservan los registros, sus relaciones y los identificadores reservados. Desactivar una entidad no desactiva en cascada sus registros relacionados ni modifica el historial. Los listados muestran activos por defecto; los CRUD de tiendas y directorio permiten incluir inactivos y reactivarlos.

La facturacion y la bitacora requieren tambien [Datos/Migraciones/002_bitacora_sistema.sql](Datos/Migraciones/002_bitacora_sistema.sql), despues de Triggers2.sql. Consulte [Datos/Ventas-verificacion.md](Datos/Ventas-verificacion.md) para uso y verificaciones.

Ventas incluye **RTN del cliente (opcional)**, guardado por factura para imprimirlo y conservarlo al reimprimir. En una base existente, ejecute [Datos/Migraciones/003_rtn_ventas.sql](Datos/Migraciones/003_rtn_ventas.sql). El esquema inicial ya incluye el campo.

El ISV se suma al precio base: 15 % por defecto, con opciones de 18 % y exento por producto en ventas. Pantalla, recibo, historial, apartados y devoluciones usan los importes con impuesto. El CAI se guarda por tienda en **Configuración y auditoría → Facturación / CAI** y se conserva en cada nueva factura. Para bases existentes, aplicar [Datos/Migraciones/004_isv_cai.sql](Datos/Migraciones/004_isv_cai.sql). [Detalle y verificaciones de ISV y CAI](Datos/ISV-CAI-verificacion.md).

**Catalogo → Exportar a Excel** guarda los productos de la vista actual, con sus filtros y tienda. Los recibos de venta incluyen **Guardar PDF**, disponible tambien desde la ultima venta, el historial y las entregas de apartados. Las exportaciones se registran en bitacora. [Uso y pruebas de exportaciones](Datos/Exportaciones-verificacion.md).

## Apartados, devoluciones y carga masiva

Apartados y devoluciones incluye reservas, abonos, entrega con factura, cancelaciones, vencimientos y reembolsos, ademas del flujo de borrador, confirmacion y anulacion de devoluciones. Consulte [Datos/ApartadosDevoluciones-verificacion.md](Datos/ApartadosDevoluciones-verificacion.md) para uso y pruebas de integracion en una base temporal.

La carga masiva acepta un `.xlsx` con las columnas `CodigoBarras`, `Nombre`, `Precio`, `StockInicial`, `NombreCategoria` y `RTNProveedor`. Usa [la plantilla de Excel](Datos/Plantillas/Plantilla_carga_masiva.xlsx) o el boton **Descargar plantilla** para generar ejemplos con los catalogos actuales. [Formato y requisitos](Datos/Plantillas/LEEME.md). El stock inicial se asigna a la tienda seleccionada mediante movimientos de inventario.
