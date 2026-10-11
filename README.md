# Sistema ERP

## Conexión a la base de datos

Configure la variable de entorno `SISTEMA_ERP_CONNECTION_STRING` o cree `connection.local.txt` en la raíz del proyecto con la cadena de conexión de SQL Server. La variable de entorno tiene prioridad. El archivo local se copia al directorio de salida y está excluido de Git para proteger las credenciales.

En Inicio, haga clic en **Probar conexión** para comprobar el acceso a la base de datos. Se mostrará un mensaje de éxito o error sin bloquear la interfaz.

Aplicación de escritorio desarrollada con C# y Windows Forms sobre .NET 10 para Windows.

## Estándar gráfico

Documento extraído de la implementación actual de `Pagina_Central.Designer.cs` y `Pagina_Central.cs`. Describe la única pantalla implementada en el proyecto; sirve como base para mantener coherencia en nuevas pantallas.

### 1. Identidad visual

Interfaz de escritorio clara y minimalista: fondos blancos, grises neutros, tipografía Segoe UI, navegación lateral y paneles con esquinas redondeadas. La jerarquía se establece mediante tamaño, negrita y espaciado.

La marca se presenta como texto **Sistema ERP**. No hay logotipo gráfico ni color corporativo de acento definido en el código.

### 2. Paleta de colores

| Uso implementado | HEX | RGB |
| --- | --- | --- |
| Fondo de ventana, menú, barra superior y tarjetas | `#FFFFFF` | 255, 255, 255 |
| Fondo del área principal | `#FDFDFD` | 253, 253, 253 |
| Texto secundario y descripciones | `#7D7D7D` | 125, 125, 125 |
| Fondo de la franja informativa | `#F3F3F3` | 243, 243, 243 |
| Fondo de Cerrar sesión | `#F2F2F2` | 242, 242, 242 |
| Fondo de botones al pasar el puntero | `#F5F5F5` | 245, 245, 245 |
| Contorno de tarjetas y gráfico | `#F7F7F7` | 247, 247, 247 |
| Separadores del menú y barra superior | `#D3D3D3` | 211, 211, 211 |
| Puntos y línea del gráfico | `#121212` | 18, 18, 18 |

El texto principal hereda el color de texto de WinForms; no tiene un HEX explícito. Los fondos transparentes de las etiquetas dejan ver el contenedor. No existen colores semánticos definidos para éxito, advertencia, error o información.

### 3. Tipografía

Familia: **Segoe UI**. Los tamaños declarados con `Font` están en puntos (pt).

| Elemento | Tamaño | Peso |
| --- | --- | --- |
| Título de pantalla: Inicio | 27 pt | Negrita |
| Valores de las tarjetas | 25 pt | Negrita |
| Letras de días del gráfico | 18 pt | Regular |
| Marca Sistema ERP | 16 pt | Negrita |
| Nombre de usuario | 14 pt | Negrita |
| Subtítulo, buscador y título del gráfico | 12 pt | Regular |
| Texto general, navegación y cuenta | 11 pt | Regular |
| Encabezados del menú e Inicio | 11 pt | Negrita |
| Títulos y detalles de las tarjetas | 10 pt | Regular |
| Rol del usuario | 9 pt | Regular |

Los textos se alinean principalmente a la izquierda y se centran verticalmente dentro de sus controles. Los botones de menú y notificaciones de la barra superior centran su símbolo.

### 4. Distribución y medidas

Las medidas siguientes son valores de diseño de WinForms. La ventana utiliza `AutoScaleMode.Font`, por lo que el tamaño visible puede variar con el escalado y la fuente.

| Elemento | Medida base |
| --- | --- |
| Área cliente inicial | 1500 × 900 |
| Tamaño mínimo de ventana (`MinimumSize`) | 1050 × 720 |
| Ancho del menú lateral | 265 |
| Alto de la barra superior | 64 |
| Relleno del contenido: izquierda, arriba, derecha, abajo | 32, 34, 32, 36 |
| Fila de título y subtítulo | 86 de alto |
| Franja informativa | 42 de alto |
| Separación después de la franja | 20 |
| Fila de indicadores | 144 de alto |
| Distribución de indicadores | 4 columnas, 25 % cada una |
| Separación entre tarjetas | 20 |
| Margen inferior de las tarjetas | 20 |
| Desplazamiento horizontal del texto en tarjetas | 24 |
| Radio de tarjetas y panel del gráfico | 30 |
| Radio de la franja informativa | 18 |

El menú se acopla a la izquierda; la barra superior, arriba; el contenido ocupa el área disponible. El gráfico toma el espacio vertical restante. El bloque de usuario se ancla abajo a la izquierda.

La fila de tarjetas conserva cuatro columnas al reducir la ventana: no hay una distribución alternativa para pantallas pequeñas. El botón ☰ permite ocultar y mostrar el menú.

### 5. Componentes y estados

- **Navegación lateral:** botones planos de 236 × 40, sin borde, fondo blanco y relleno izquierdo de 10. Inicio se distingue con negrita, sin resaltado gris permanente. Al pasar el puntero se aplica `#F5F5F5`.
- **Grupos de navegación:** Descubrir y Administración, con encabezados en negrita.
- **Sesión:** nombre de usuario, rol en gris y botón Cerrar sesión de 216 × 38 con fondo `#F2F2F2`.
- **Barra superior:** botón ☰ de 36 × 30, buscador sin borde, nombre de cuenta con símbolo ⌄ y botón de notificaciones de 54 × 30. Los botones de símbolos tienen borde de 1.
- **Buscador:** ancho entre 160 y 420, ajustado al espacio disponible; texto de ayuda «🔍 Buscador global...».
- **Franja informativa:** texto secundario, símbolo ℹ y relleno horizontal de 16 a la izquierda y 8 a la derecha.
- **Indicadores:** título gris, valor grande en negrita y detalle gris. El ejemplo monetario usa `L. 0.00`; no hay formateador monetario centralizado.
- **Gráfico semanal:** panel blanco, título a la izquierda, días L, M, X, J, V, S y D; puntos de 22 de diámetro y línea horizontal de grosor 8, ambos `#121212`. No hay ejes numéricos, leyenda ni cuadrícula. Los datos son de ejemplo.

Los símbolos son caracteres Unicode o emoji, no imágenes de una biblioteca de iconos. Su apariencia puede variar según las fuentes disponibles.

### 6. Reglas para extender el diseño

Estas reglas se proponen a partir de los patrones existentes:

1. Usar Segoe UI y conservar la jerarquía tipográfica descrita.
2. Mantener menú lateral, barra superior y márgenes del contenido en las nuevas pantallas.
3. Usar blanco para superficies y gris para información secundaria; reservar las negritas para títulos, valores y selección del menú.
4. Reutilizar radios de 30 en paneles principales y 18 en franjas informativas.
5. Mantener navegación plana, texto a la izquierda y respuesta visual al pasar el puntero.
6. Declarar controles y propiedades editables en `InitializeComponent`, con sus campos en `.Designer.cs`; dejar comportamiento y dibujo personalizado en `.cs`.
7. Documentar cualquier nuevo color de estado, control o variante antes de extenderlo a otras pantallas.

### 7. Límites del estándar actual

- Los valores visuales están repetidos en los controles; todavía no existe un tema compartido o una clase de constantes de diseño.
- Las esquinas redondeadas y el gráfico se dibujan durante la ejecución y pueden no aparecer completos en el diseñador.
- La implementación no dibuja las sombras de la imagen de referencia.
- No se han definido variantes para tablas, formularios de captura, cuadros de diálogo, validación, botones de acción principal o modo oscuro.
- Este documento describe y propone reglas visuales; no confirma que las opciones de navegación o el buscador tengan funcionalidad de negocio implementada.
