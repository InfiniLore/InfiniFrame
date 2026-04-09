param(
    [switch]$BuildCsApiOnly,
    [string]$ConfigPath = "docs/docfx.api.json",
    [string]$ApiSitePath = "docs/_site_api"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$docsRoot = Join-Path $repoRoot "docs"

if (-not [System.IO.Path]::IsPathRooted($ConfigPath)) {
    $ConfigPath = Join-Path $repoRoot $ConfigPath
}

if (-not [System.IO.Path]::IsPathRooted($ApiSitePath)) {
    $ApiSitePath = Join-Path $repoRoot $ApiSitePath
}

if (-not $BuildCsApiOnly) {
    npm --prefix "$docsRoot" run dev
    exit $LASTEXITCODE
}

if (-not (Get-Command docfx -ErrorAction SilentlyContinue)) {
    Write-Error "docfx is not installed or not on PATH. Install with: dotnet tool install --global docfx --version 2.78.5"
}

docfx metadata $ConfigPath
docfx build $ConfigPath
docfx serve $ApiSitePath
