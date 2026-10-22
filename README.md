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
  Administracion/  Inversiones, directorio y auditoria
  Dialogos/        Confirmacion de venta y notificaciones
  Base/            Clases compartidas de pantallas y dialogos
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

Aplicacion de escritorio desarrollada con C# y Windows Forms sobre .NET 10 para Windows.

## Estandar grafico

Documento extraido de la implementacion actual de `Formularios/Principal/Pagina_Central.Designer.cs` y `Formularios/Principal/Pagina_Central.cs`. Describe la unica pantalla implementada en el proyecto; sirve como base para mantener coherencia en nuevas pantallas.

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

El texto principal hereda el color de texto de WinForms; no tiene un HEX explicito. Los fondos transparentes de las etiquetas dejan ver el contenedor. No existen colores semanticos definidos para exito, advertencia, error o informacion.

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

- Los valores visuales estan repetidos en los controles; todavia no existe un tema compartido o una clase de constantes de diseno.
- Las esquinas redondeadas y el grafico se dibujan durante la ejecucion y pueden no aparecer completos en el disenador.
- La implementacion no dibuja las sombras de la imagen de referencia.
- No se han definido variantes para tablas, formularios de captura, cuadros de dialogo, validacion, botones de accion principal o modo oscuro.
- Este documento describe y propone reglas visuales; no confirma que las opciones de navegacion o el buscador tengan funcionalidad de negocio implementada.

## Desactivacion de registros

Antes de ejecutar esta version, aplique `Datos/Migraciones/001_desactivar_registros.sql` a la base de datos de SQL Server configurada. La migracion agrega `activo` (`bit`, valor predeterminado `1`) a Producto, Categoria y Proveedor, conserva los registros existentes y puede ejecutarse nuevamente.

Las acciones **Desactivar** conservan el registro y sus relaciones, asignando `activo = 0`. Los listados de productos y proveedores y los selectores de categorias y proveedores muestran unicamente registros activos. Los codigos y nombres existentes siguen reservados aunque se desactive el registro. Desactivar una categoria o proveedor no desactiva sus productos ni modifica el historial. La interfaz aun no incluye una accion para reactivar registros.
