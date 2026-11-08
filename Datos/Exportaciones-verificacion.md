# Exportar catálogo y recibos

## Catálogo en Excel

En **Catálogo → Productos → Exportar a Excel**, elegir dónde guardar el `.xlsx`. Se exportan las filas del catálogo cargado, respetando la búsqueda, la tienda de la cabecera y Mostrar también productos inactivos. La exportación usa una copia de esa vista, de modo que cambiar el filtro mientras se escribe el archivo no cambia sus datos.

La hoja Catalogo contiene Código, Nombre, Categoría, Stock, Precio base (L), Estado y Motivo de desactivación. Los códigos se guardan como texto para conservar ceros iniciales; el stock y precio son números. Incluye filtros de Excel y encabezados inmovilizados. La hoja Información indica el alcance, búsqueda, inclusión de inactivos, cantidad de productos y fecha de generación en Honduras. El precio exportado es el precio base antes del ISV, igual que en el catálogo.

El botón se habilita cuando hay filas para exportar. Si falla la carga del catálogo se retiran los datos anteriores para no exportar otra tienda con un encabezado incorrecto. La plantilla de carga masiva conserva su opción de descarga y vista de ejemplo.

## Recibo de venta en PDF

- Desde la pantalla de ventas: **Exportar recibo PDF** guarda la última factura emitida o abierta.
- Desde el recibo abierto: **Guardar PDF**, junto a Imprimir.
- Desde **Facturas registradas**: seleccionar la factura y pulsar **Exportar PDF**, incluyendo facturas de sesiones anteriores.
- Al entregar un apartado: el comprobante de la factura también incluye **Guardar PDF**.

El PDF se genera directamente, sin depender de una impresora virtual. Su contenido se consulta en la base de datos e incluye número, fecha, estado, tienda, cliente, RTN opcional, CAI guardado con la venta, productos, subtotal, exento, ISV del 15 % y 18 %, total, pagos, referencias, cambio y observaciones. Los recibos extensos se distribuyen en varias páginas A4, con ajuste de líneas y numeración de página. Se conservan acentos y ceros iniciales.

Se usa [PDFsharp-GDI 6.2.4](https://www.nuget.org/packages/PDFsharp-GDI/6.2.4), compatible con la aplicación Windows, con licencia MIT. La resolución de fuentes usa las fuentes de Windows según la [documentación del motor GDI](https://docs.pdfsharp.net/PDFsharp/Topics/Fonts/Font-Resolving.html).

## Guardado y bitácora

Se elige el destino mediante Guardar como. El archivo se genera primero en un temporal de la misma carpeta y se mueve al destino al terminar. Si el destino está bloqueado, se informa el error conservando el archivo anterior. No se abre el documento exportado automáticamente.

Después de guardar, la aplicación registra EXPORTAR en la bitácora con el usuario, módulo y factura o alcance del catálogo. No almacena la ruta personal de guardado. Si el archivo se guardó pero el registro en bitácora falla, el mensaje distingue ambas situaciones.

## Verificación

Se amplió la prueba `Pruebas/FlujoVentas.Integracion` usando una base temporal aislada. Se verificó:

- El Excel contiene únicamente la vista filtrada y mantiene código textual, precio y stock numéricos y metadatos de tienda.
- Un archivo Excel bloqueado no se reemplaza parcialmente.
- Una venta real se exporta a PDF y se lee de nuevo con PdfPig, comprobando RTN, CAI, ISV, identificador, subtotal, total, cambio y texto acentuado.
- Un recibo extenso se reparte en varias páginas, conserva el final y los caracteres de palabras muy largas y mantiene el texto dentro del ancho de página.
- Se registran ambas exportaciones con el usuario responsable.
- Continúan pasando los flujos de facturación, apartados, devoluciones, RTN, ISV, CAI y carga masiva.

La base temporal y archivos de prueba se eliminan al terminar. Compilación sin errores ni advertencias. No se guardaron ventas de prueba en la base de uso.
