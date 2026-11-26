<#
  Construye el instalador candidato de Sistema ERP.

  Uso (desde la carpeta del proyecto, donde esta Sistema_ERP.csproj):
    powershell -ExecutionPolicy Bypass -File .\Instalador\construir_instalador.ps1
    powershell -ExecutionPolicy Bypass -File .\Instalador\construir_instalador.ps1 -Version 1.0.1

  Requisitos: Windows x64, .NET SDK 10, Git e Inno Setup (6 o 7).
#>
param([string]$Version = "1.0.0")

$ErrorActionPreference = "Stop"
$raiz      = Split-Path -Parent $PSScriptRoot
$proyecto  = Join-Path $raiz "Sistema_ERP.csproj"
$publicado = Join-Path $PSScriptRoot "publicado"
$salida    = Join-Path $PSScriptRoot "salida"
$iss       = Join-Path $PSScriptRoot "SistemaERP.iss"

Write-Host "== 1/5 Verificando requisitos =="
if (-not (Test-Path $proyecto)) { throw "No se encontro $proyecto" }
$carpetas = @("${env:ProgramFiles}", "${env:ProgramFiles(x86)}", "${env:LOCALAPPDATA}\Programs") |
    Where-Object { $_ -and (Test-Path $_) }
$iscc = $carpetas |
    ForEach-Object { Get-ChildItem $_ -Directory -Filter "Inno Setup*" -ErrorAction SilentlyContinue } |
    Sort-Object Name -Descending |
    ForEach-Object { Join-Path $_.FullName "ISCC.exe" } |
    Where-Object { Test-Path $_ } |
    Select-Object -First 1
if (-not $iscc) { throw "No se encontro Inno Setup (version 6 o 7). Instalalo desde https://jrsoftware.org/isdl.php" }
Write-Host "Inno Setup: $iscc"

Write-Host "== 2/5 Revisando el estado de Git =="
$commit = (git -C $raiz rev-parse --short HEAD).Trim()
$cambios = git -C $raiz status --porcelain -- . ":(exclude)Instalador"
if ($cambios) {
    Write-Warning "Hay cambios sin guardar en Git. El instalador NO corresponde exactamente al commit $commit."
}
Write-Host "Commit: $commit"

Write-Host "== 3/5 Publicando el programa (dotnet publish) =="
if (Test-Path $publicado) { Remove-Item $publicado -Recurse -Force }
dotnet publish $proyecto -c Release -r win-x64 --self-contained true -o $publicado -p:Version=$Version
if ($LASTEXITCODE -ne 0) { throw "dotnet publish fallo" }

Write-Host "== 4/5 Buscando contrasenas en lo publicado =="
$textos = Get-ChildItem $publicado -Recurse -File -Include *.json,*.config,*.xml,*.txt,*.ini
$hallazgos = @($textos | Select-String -Pattern 'password\s*=|pwd\s*=')
if ($hallazgos.Count -gt 0) {
    foreach ($h in $hallazgos) { Write-Host ("Posible contrasena en: " + $h.Path + " (linea " + $h.LineNumber + ")") }
    throw "Se encontraron posibles contrasenas dentro del paquete. Quitalas antes de crear el instalador."
}
Write-Host "No se encontraron contrasenas."

Write-Host "== 5/5 Creando el instalador (Inno Setup) =="
& $iscc "/DAppVersion=$Version" $iss
if ($LASTEXITCODE -ne 0) { throw "Inno Setup fallo" }

$exe = Join-Path $salida "SistemaERP_Setup_$Version.exe"
$hash = (Get-FileHash $exe -Algorithm SHA256).Hash
@(
    "Sistema ERP - instalador candidato",
    "Version: $Version",
    "Commit: $commit",
    "Fecha de construccion: $(Get-Date -Format 'yyyy-MM-dd HH:mm')",
    "SHA256: $hash"
) | Set-Content (Join-Path $salida "VERSION.txt") -Encoding UTF8

Write-Host ""
Write-Host "LISTO. Instalador creado en:"
Write-Host $exe
