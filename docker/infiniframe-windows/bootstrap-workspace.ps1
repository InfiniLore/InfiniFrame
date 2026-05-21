param(
    [string]$Source = "C:\src",
    [string]$Destination = "C:\work"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path -LiteralPath $Destination)) {
    New-Item -ItemType Directory -Path $Destination | Out-Null
}

Get-ChildItem -LiteralPath $Destination -Force | Remove-Item -Recurse -Force

$excludes = @(
    ".git",
    ".github",
    ".idea",
    ".run",
    "artifacts",
    ".tmp",
    ".pytest_cache",
    "docs\node_modules",
    "docs\.docusaurus",
    "docs\build",
    "src\InfiniFrame.NativeBridge\Native\packages",
    "src\InfiniFrame.NativeBridge\build"
)

$excludeArgs = @()
foreach ($entry in $excludes) {
    $excludeArgs += "/XD"
    $excludeArgs += "$Source\$entry"
}

robocopy $Source $Destination /E /NFL /NDL /NJH /NJS /NP @excludeArgs | Out-Null
if ($LASTEXITCODE -ge 8) {
    throw "robocopy failed with exit code $LASTEXITCODE"
}
