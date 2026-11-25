# Passkeys

## Preparacion de la base de datos

El primer arranque prepara la base indicada en SISTEMA_ERP_CONNECTION_STRING (o connection.local.txt). En una base nueva o vacia aplica el esquema completo, triggers, bitacora y passkeys; en una base 3FN compatible aplica solo los scripts faltantes. Si encuentra tablas pero no dbo.Usuario, o un esquema anterior incompleto, se detiene sin ejecutar el esquema sobre esos datos. Para revisar el flujo, consulte Datos/Seguridad-verificacion.md.

La tabla `PasskeyUsuario` guarda la clave publica, el identificador de credencial y los datos que WebAuthn necesita para verificar accesos. La clave privada queda en Windows Hello, el telefono o la llave de seguridad. La migracion declara la clave primaria como `NONCLUSTERED` porque el identificador puede superar el limite de longitud de una clave clustered de SQL Server.

## Uso

Con la sesion iniciada, abrir **Mis passkeys**, confirmar la contrasena actual y elegir **Registrar passkey**. En la pantalla segura de Windows, elegir Windows Hello para guardar la passkey en este equipo, el telefono o una llave de seguridad FIDO2. Windows Hello puede confirmar con el PIN o la biometria configurada. Para completar el registro por QR, Windows 11 23H2 o posterior y Bluetooth deben estar activos en el PC y en el telefono, y el proveedor del telefono debe admitir FIDO2/WebAuthn.

Para entrar, escribir el nombre de usuario y elegir **Usar passkey**. La contrasena sigue disponible como alternativa. Desde **Mis passkeys** se puede revocar una credencial despues de confirmar la contrasena.
