# Validaciones por acción

Cada acción reúne sus reglas en una clase `*Validacion`, compartida por sus pantallas y por el acceso a datos. Guardar un registro abarca su alta y edición cuando las reglas son comunes; las condiciones específicas se expresan en métodos como `Captura`, `Edicion`, `Estado` o `VersionActual` de esa misma clase.

| Acción | Clases principales |
| --- | --- |
| Guardar productos, clientes, proveedores, categorías o tiendas | `GuardarProductoValidacion`, `GuardarClienteValidacion`, `GuardarProveedorValidacion`, `GuardarCategoriaValidacion`, `GuardarTiendaValidacion` |
| Guardar directorio y cambiar su estado | `GuardarDirectorioValidacion`, `CambiarEstadoDirectorioValidacion`, `DesactivarRegistroValidacion`, `ReactivarRegistroValidacion` |
| Guardar, confirmar y anular compras | `AgregarLineaCompraValidacion`, `GuardarCompraValidacion`, `ConfirmarCompraValidacion`, `AnularCompraValidacion`, `ConsultarCompraValidacion` |
| Facturar y registrar pagos | `AgregarProductoVentaValidacion`, `EmitirVentaValidacion`, `RegistrarPagoValidacion`, `RegistrarImporteValidacion`, `AsignarRtnVentaValidacion`, `CalcularIsvValidacion` |
| Crear, abonar, entregar y reembolsar apartados | `CrearApartadoValidacion`, `AgregarProductoApartadoValidacion`, `RegistrarAbonoApartadoValidacion`, `EntregarApartadoValidacion`, `ReembolsarApartadoValidacion`, `ConsultarApartadoValidacion` |
| Guardar, confirmar, anular y reembolsar devoluciones | `GuardarDevolucionValidacion`, `ConfirmarDevolucionValidacion`, `AnularDevolucionValidacion`, `ReembolsarDevolucionValidacion`, `ConsultarDevolucionValidacion` |
| Guardar y resolver traspasos | `GuardarTraspasoValidacion`, `ResolverTraspasoValidacion`, `ConsultarTraspasoValidacion` |
| Consultar inventario o cambiar stock mínimo | `ConsultarInventarioValidacion`, `GuardarMinimoInventarioValidacion` |
| Importar productos | `ImportarProductosValidacion`, con `CatalogosImportacion` como datos de referencia |
| Ingresar y cambiar contraseñas | `IniciarSesionValidacion`, `CrearClaveValidacion`, `CambiarClaveValidacion` |
| Guardar usuarios, roles o permisos y cambiar su estado | `GuardarUsuarioValidacion`, `GuardarRolValidacion`, `GuardarPermisoValidacion`, `GuardarRegistroSeguridadValidacion`, `CambiarEstadoSeguridadValidacion` |
| Asignar permisos y aplicar presets | `AsignarPermisoRolValidacion`, `AplicarPresetPermisosValidacion`, con `PermisoAsignable` como dato de entrada |
| Consultar historiales, buscar y elegir opciones | `ConsultarHistorialValidacion`, `BuscarContenidoValidacion`, `SeleccionarOpcionValidacion` |
| Guardar CAI, registrar motivos, cambiar apariencia y exportar | `GuardarCaiValidacion`, `RegistrarMotivoValidacion`, `CambiarTemaValidacion`, `ExportarCatalogoValidacion`, `ExportarReciboValidacion` |

## Responsabilidades

Los formularios capturan valores, terminan la edición de sus controles, llaman a la acción correspondiente y presentan el resultado o error. Mantienen los eventos, la selección de filas, los estados de carga, las confirmaciones y la habilitación de botones. `ErrorCampoValidacion` permite indicar el campo que debe recibir el foco sin introducir controles visuales en la validación.

Las clases de acción reciben valores, modelos o tablas; no reciben formularios ni controles, no muestran mensajes y no abren conexiones SQL. Las comprobaciones comunes de nombres, correos, ceros, identificadores y texto permanecen en `Clases/ValidacionDatos.cs`; la interpretación monetaria permanece en `ImporteMonetario`. La acción de importación también comprueba el archivo, sus columnas, filas y huella antes de persistirlo.

El acceso a datos vuelve a invocar las reglas que necesita para proteger llamadas desde otras pantallas o servicios. Lee versiones, estados, existencias y referencias dentro de la transacción y pasa esos valores a la validación de la acción **antes** de escribir. Los bloqueos, las transacciones, los triggers y las restricciones SQL continúan en la base. La autorización y la vigencia de sesión siguen centralizadas en `AutorizacionDatos`, `SesionActual` y `AccesoDatos`; los fallos técnicos, la traducción de errores de restricciones y la liberación de recursos pertenecen al acceso a datos.

Al agregar una regla, ubicarla en la acción existente o crear una nueva clase para una acción nueva; reutilizar las comprobaciones comunes y llamar a la acción desde los consumidores. Evitar reimplementar la regla en eventos de formularios, modelos o repositorios. Los métodos `Validar` de los modelos y los adaptadores conservados para compatibilidad delegan en estas clases.

## Verificación

```powershell
dotnet run --project Pruebas/FlujoVentas.Integracion/FlujoVentas.Integracion.csproj -- --validaciones-acciones
dotnet run --project Pruebas/FlujoVentas.Integracion/FlujoVentas.Integracion.csproj -- --sin-bd
dotnet run --project Pruebas/FlujoVentas.Integracion/FlujoVentas.Integracion.csproj
```

La primera opción ejecuta las acciones directamente sin abrir formularios ni conectarse a SQL. La segunda comprueba además las validaciones existentes y los flujos visuales sin base de datos. La suite completa verifica persistencia, estados, versiones, permisos, importaciones y transacciones en una base temporal que elimina al terminar.
