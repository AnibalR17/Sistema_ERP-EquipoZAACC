# Formato de carga masiva de productos

Para probar con nombres comunes, usa [Carga_masiva_productos_papeleria.xlsx](Carga_masiva_productos_papeleria.xlsx). Incluye veinte artículos (cuadernos, bolígrafos, papel bond, grapadoras y otros), precios ficticios y 746 unidades de stock inicial. Sus códigos y la categoría se verificaron contra la base local al crearlo. Antes de importarlo, registra el RTN de un proveedor activo y reemplaza `COMPLETAR_RTN_PROVEEDOR` en toda la columna correspondiente; el proveedor disponible no tenía RTN registrado. Las hojas Catalogos e Instrucciones contienen las referencias y los pasos para usarlo.

Usa [Plantilla_carga_masiva.xlsx](Plantilla_carga_masiva.xlsx), o **Descargar plantilla** en la pantalla para obtener ejemplos y catálogos actualizados de tu base. La primera hoja debe contener estos seis encabezados en la fila 1; el orden es libre y el lector ignora diferencias entre mayúsculas y minúsculas:

Dentro de **Carga masiva**, la pestaña **Ejemplo de plantilla** muestra los diez productos del Excel incluido. Permite consultar y copiar filas con sus encabezados. El ejemplo está incluido en la aplicación y puede consultarse sin conexión a la base. Al elegir un archivo, se abre **Vista previa del archivo** para revisar sus datos y realizar la importación. Cambiar entre pestañas conserva la vista previa del archivo seleccionado.

| Columna | Valor esperado |
| --- | --- |
| CodigoBarras | Texto obligatorio y único, hasta 50 caracteres. Guardar como texto para conservar los ceros iniciales. |
| Nombre | Texto obligatorio, hasta 150 caracteres. |
| Precio | Número mayor que cero, con hasta dos decimales y máximo 99999999.99. No incluir moneda ni separadores de miles. Preferir una celda numérica de Excel; como texto se acepta punto o coma decimal. |
| StockInicial | Entero de cero a 2147483647. La celda vacía equivale a cero, pero el encabezado es obligatorio. |
| NombreCategoria | Nombre de una categoría activa existente. No es el ID. Se ignoran mayúsculas/minúsculas y espacios al principio/final. |
| RTNProveedor | RTN de un proveedor activo existente, como texto. No es el nombre ni el ID del proveedor. Se eliminan guiones y espacios para comparar. |

Ejemplo de estructura (sustituye categoría y RTN por valores de tu base):

```text
CodigoBarras | Nombre             | Precio | StockInicial | NombreCategoria | RTNProveedor
000990000001 | Producto de prueba | 12.50  | 5            | TU_CATEGORIA    | TU_RTN
```

Se lee únicamente la primera hoja. Productos es la hoja de datos; Catalogos e Instrucciones son de consulta. Evita celdas combinadas, filas de títulos anteriores a los encabezados y encabezados repetidos. Los códigos existentes en la base o repetidos en el archivo se marcan con error; no se actualizan productos existentes.

Selecciona una tienda en la pantalla cuando haya StockInicial mayor que cero. El stock se registra en esa tienda mediante movimientos de inventario auditados; no se escribe un stock global en Producto. La importación guarda productos, movimientos e historial en una transacción. Si ocurre un error al guardar el lote, se revierte completo. La previsualización muestra las filas inválidas en rojo; Importar datos válidos permite omitirlas tras confirmar.

La plantilla incluye diez productos ficticios y 35 unidades de stock inicial. Importarlos sí crea datos en la base elegida. Si se vuelve a generar después de importarlos, se buscan otros códigos que todavía no existan.

Al generar el archivo incluido se encontró una categoría activa y un proveedor activo sin RTN en la base local. Por eso RTNProveedor contiene `COMPLETAR_RTN_PROVEEDOR`: completa el RTN real del proveedor desde Proveedores y reemplaza ese texto, o vuelve a descargar la plantilla. No se inventó un RTN ni se modificaron datos del proveedor para producir el ejemplo.
