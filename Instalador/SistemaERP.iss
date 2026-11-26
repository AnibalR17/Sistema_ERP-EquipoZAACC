; ------------------------------------------------------------------
; Instalador de Sistema ERP (Equipo ZAACC) - script de Inno Setup (6 o 7)
; No hace falta abrirlo a mano: lo compila construir_instalador.ps1
; La version llega con /DAppVersion=x.y.z; si no llega, usa 1.0.0
; ------------------------------------------------------------------
#define AppName "Sistema ERP"
#define AppExe "Sistema_ERP.exe"
#ifndef AppVersion
  #define AppVersion "1.0.0"
#endif

[Setup]
; AppId identifica al programa para actualizarlo o desinstalarlo. NO cambiarlo.
AppId={{1EFFA387-09E5-4DE6-84AB-4D6561CC09D9}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher=Equipo ZAACC
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
OutputDir=salida
OutputBaseFilename=SistemaERP_Setup_{#AppVersion}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayIcon={app}\{#AppExe}

; El idioma espanol solo se activa si el archivo existe en tu Inno Setup
#if FileExists(CompilerPath + "Languages\Spanish.isl")
[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
#endif

[Tasks]
Name: "desktopicon"; Description: "Crear un acceso directo en el Escritorio"; GroupDescription: "Accesos directos:"; Flags: unchecked

[Files]
; Todo lo que dejo "dotnet publish" en la carpeta publicado
Source: "publicado\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExe}"
Name: "{group}\Desinstalar {#AppName}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExe}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExe}"; Description: "Abrir {#AppName} al terminar"; Flags: nowait postinstall skipifsilent
