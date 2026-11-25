# Passkeys

## Preparacion de la base de datos

En una base existente compatible, ejecutar `Datos/Migraciones/002_bitacora_sistema.sql` si todavia no esta instalado y despues `Datos/Migraciones/006_passkeys.sql`. En una base nueva, ejecutar `Base de Datos/01_esquema_erp_3fn.sql`, instalar los triggers y la bitacora, y luego aplicar `006_passkeys.sql` para instalar la auditoria de passkeys. No ejecutar el esquema completo sobre una base que ya tenga datos.

La tabla `PasskeyUsuario` guarda la clave publica, el identificador de credencial y los datos que WebAuthn necesita para verificar accesos. La clave privada queda en el telefono o autenticador. La migracion declara la clave primaria como `NONCLUSTERED` porque el identificador puede superar el limite de longitud de una clave clustered de SQL Server.

## Uso

Con la sesion iniciada, abrir **Mis passkeys**, confirmar la contrasena actual y elegir **Registrar passkey**. En la pantalla segura de Windows, elegir el telefono u otro dispositivo. Para completar el registro por QR, Windows 11 23H2 o posterior y Bluetooth deben estar activos en el PC y en el telefono, y el proveedor del telefono debe admitir FIDO2/WebAuthn. Tambien se admiten llaves de seguridad FIDO2.

Para entrar, escribir el nombre de usuario y elegir **Usar passkey del telefono**. La contrasena sigue disponible como alternativa. Desde **Mis passkeys** se puede revocar una credencial despues de confirmar la contrasena.