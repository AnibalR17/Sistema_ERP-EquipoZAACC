# Verificacion de usuarios, roles y permisos

## Preparacion

Usar una base de pruebas con `Base de Datos/01_esquema_erp_3fn.sql` ya instalado y la conexion descrita en README. El script de creacion es para una base vacia; no ejecutarlo sobre una base existente con datos. No se requiere migracion para estos CRUD. Para invalidacion de sesiones y auditoria instalar los triggers proporcionados en la misma base. La aplicacion no instala scripts.

Crear una tienda y dos empleados activos mediante los CRUD existentes. Abrir **Usuarios, roles y permisos** desde la ventana principal.

## Casos funcionales contra SQL Server (pendientes de ejecucion)

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

## Verificacion local realizada

Un ejecutable temporal sobre .NET 10 comprobo construccion del formulario sin consultar SQL, columnas de las tres variantes, filtros con caracteres especiales y validaciones previas a persistencia: 17 comprobaciones aprobadas. Compilacion y revision de espacios se verifican por separado. Estas comprobaciones no sustituyen los casos de integracion anteriores.

## Limites

El login autentica y obliga a cambiar claves temporales; las asignaciones todavia no restringen la navegacion ni las operaciones del ERP. Los actores de creacion, modificacion, desactivacion y asignacion de los CRUD quedan nulos. No se gestionan UsuarioTienda ni recuperacion de acceso en esta pantalla. Desactivar conserva las relaciones: reactivar un rol o permiso puede restaurar privilegios de sus asignaciones vigentes.

## Login: integracion pendiente contra SQL Server

Verificacion local del login: compilacion sin errores ni advertencias, 14 comprobaciones aprobadas de hashes, rechazo de formatos invalidos, limites de clave, sal aleatoria y construccion del formulario. La creacion inicial ahora se realiza desde la aplicacion y reemplaza la herramienta PowerShell anterior.

Regresion del manejo de errores: 23 comprobaciones aprobadas con respuestas de autenticacion simuladas. Un rechazo y su reintento conservan abierto el formulario y el nombre de usuario, muestran el mensaje, limpian la clave y habilitan Ingresar; un fallo de conexion se controla del mismo modo. Una clave vacia devuelve un resultado de validacion sin consultar SQL. Las credenciales rechazadas se comunican como ResultadoAcceso, sin lanzar excepciones de validacion desde Ingresar. Falta repetir el caso con la base real del operador.

1. Base nueva con esquema instalado y Usuario vacia: ingresar con admin/admin. Verificar creacion de la cuenta temporal y asignacion al rol ADMINISTRADOR, con empleado ADMIN-INICIAL y tienda INICIAL si hacen falta. Debe exigir cambio por clave de 12 a 128 caracteres antes de abrir Inicio. Reabrir y probar dos instancias: no debe duplicar registros ni restablecer la clave. Si ya existen usuarios, incluidos inactivos, admin/admin no se crea automaticamente. Los registros iniciales inactivos no se reactivan. Probar en una base de pruebas, no en produccion.
2. Arrancar la aplicacion: solo debe mostrarse login. Credenciales invalidas, cuenta inactiva o empleado inactivo no abren Inicio; no puede elegirse un rol manualmente.
3. Clave temporal: verificar cambio obligatorio, longitud, confirmacion y rechazo de la misma clave. Cancelar cierra la aplicacion sin abrir Inicio. Una modificacion concurrente de la cuenta impide usar el cambio pendiente.
4. Cinco claves incorrectas bloquean durante 15 minutos. Una correcta durante el bloqueo tampoco entra. Al vencer, un fallo inicia una cuenta nueva de intentos; una entrada completa los reinicia. Revisar EventoAcceso.
5. Verificar creacion de SesionUsuario, nombre y roles en Inicio. Cerrar sesion revoca la fila y vuelve al login; entrar con otra cuenta debe mostrar su identidad. Cerrar Inicio termina el proceso.
6. Con triggers instalados cambiar privilegios o clave desde otra instancia: en un minuto la sesion anterior debe terminar. Verificar tambien expiracion, desactivacion de empleado y perdida de conexion. Una sesion que no logra revocarse por desconexion caduca a las ocho horas.
7. Los hashes de ejemplo del poblado no son claves utilizables. Para cuentas existentes sin hash compatible se requiere recuperacion administrada; admin/admin solo se crea cuando Usuario esta vacia y no permite acceder a cuentas existentes.
