# Passkeys

## Preparacion de la base de datos

El primer arranque prepara la base indicada en SISTEMA_ERP_CONNECTION_STRING (o connection.local.txt). En una base nueva o vacia aplica el esquema completo, triggers, bitacora y passkeys; en una base 3FN compatible aplica solo los scripts faltantes. Si encuentra tablas pero no dbo.Usuario, o un esquema anterior incompleto, se detiene sin ejecutar el esquema sobre esos datos. Para revisar el flujo, consulte Datos/Seguridad-verificacion.md.

La tabla `PasskeyUsuario` guarda la clave publica, el identificador de credencial y los datos que WebAuthn necesita para verificar accesos. La clave privada queda en el telefono o autenticador. La migracion declara la clave primaria como `NONCLUSTERED` porque el identificador puede superar el limite de longitud de una clave clustered de SQL Server.

## Uso

Con la sesion iniciada, abrir **Mis passkeys**, confirmar la contrasena actual y elegir **Registrar passkey**. En la pantalla segura de Windows, elegir el telefono u otro dispositivo. Para completar el registro por QR, Windows 11 23H2 o posterior y Bluetooth deben estar activos en el PC y en el telefono, y el proveedor del telefono debe admitir FIDO2/WebAuthn. Tambien se admiten llaves de seguridad FIDO2.

Para entrar, escribir el nombre de usuario y elegir **Usar passkey del telefono**. La contrasena sigue disponible como alternativa. Desde **Mis passkeys** se puede revocar una credencial despues de confirmar la contrasena.