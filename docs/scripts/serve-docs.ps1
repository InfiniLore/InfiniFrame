param(
    [string]$ConfigPath = "docs/docfx.json",
    [string]$SitePath = "docs/_site"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent

if (-not [System.IO.Path]::IsPathRooted($ConfigPath)) {
    $ConfigPath = Join-Path $repoRoot $ConfigPath
}

if (-not [System.IO.Path]::IsPathRooted($SitePath)) {
    $SitePath = Join-Path $repoRoot $SitePath
}

if (-not (Get-Command docfx -ErrorAction SilentlyContinue)) {
    Write-Error "docfx is not installed or not on PATH. Install with: dotnet tool install --global docfx --version 2.78.5"
}

docfx metadata $ConfigPath
& "$PSScriptRoot/update-cpp-api.ps1" -AutoInstallDoxygen
docfx build $ConfigPath
docfx serve $SitePath
