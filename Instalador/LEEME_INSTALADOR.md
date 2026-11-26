# Instalador de Sistema ERP (candidato)

Esta carpeta contiene la **receta para construir el instalador** de Sistema ERP.
Estado: **instalador candidato (K20). No esta validado**; la validacion es K21 (entorno limpio) y K28 (version final).

## Que hay aqui

| Archivo | Para que sirve |
|---|---|
| `construir_instalador.ps1` | Script que publica el programa y crea el instalador, todo en un paso |
| `SistemaERP.iss` | Instrucciones para Inno Setup (que copiar, donde instalar, accesos directos) |
| `LEEME_INSTALADOR.md` | Este archivo |

Las carpetas `publicado/` y `salida/` se generan solas y **no se suben a Git** (estan en `.gitignore`).

## Requisitos para construirlo (en tu computadora)

- Windows 10/11 de 64 bits
- .NET SDK 10 (`dotnet --version` debe mostrar 10.x)
- Git
- Inno Setup 6 o 7 (gratis): https://jrsoftware.org/isdl.php

## Como construirlo

1. Abre PowerShell en la carpeta del proyecto (donde esta `Sistema_ERP.csproj`).
2. Ejecuta:

```
powershell -ExecutionPolicy Bypass -File .\Instalador\construir_instalador.ps1
```

Para otra version: agrega `-Version 1.0.1`.

3. Al terminar, el instalador queda en `Instalador\salida\SistemaERP_Setup_<version>.exe`
   junto a `VERSION.txt` con la version, el commit de Git, la fecha y la huella SHA256.

El script verifica antes de crear el instalador que **no haya contrasenas** escritas en los archivos publicados.
Si las encuentra, se detiene.

## Version identificada

El instalador lleva la version en el nombre y en `VERSION.txt` junto al commit de Git del que salio.
Si hay cambios sin guardar en Git, el script avisa: en ese caso el instalador no corresponde exactamente al commit.

## Que incluye el paquete

- El programa publicado para Windows x64 con .NET incluido (`--self-contained`), por lo que
  **no hace falta instalar .NET** en la computadora destino.
- No incluye la base de datos.
- No incluye contrasenas ni cadenas de conexion con secretos.

## Prerrequisitos para instalarlo

- Windows 10/11 de 64 bits.
- Acceso a un SQL Server (servidor, instancia y base de datos).

## Base de datos (por completar)

La aplicacion se conecta a SQL Server. Cada instalacion debe configurar su propia conexion;
los pasos para crear/preparar la base y configurar la conexion estan en el `README.md` del proyecto.

> PENDIENTE: confirmar con K11 (Alejandro) hasta donde llega el alcance del instalador respecto a la base de datos
> (solo aplicacion, o tambien scripts de creacion). Completar esta seccion segun esa decision.

## Puntos a revisar en la validacion (K21 / K28)

- Que el programa abra en una computadora limpia (sin Visual Studio ni .NET instalado).
- Que, si el programa escribe archivos (registros, configuracion, exportaciones) dentro de su propia carpeta,
  esto funcione estando instalado en `Archivos de programa`.
- Que el desinstalador quite el programa y los accesos directos.
