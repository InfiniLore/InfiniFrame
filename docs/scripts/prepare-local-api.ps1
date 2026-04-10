param(
    [switch]$Required
)

$ErrorActionPreference = "Stop"

$docsRoot = Split-Path -Parent $PSScriptRoot
$repoRoot = Split-Path -Parent $docsRoot
$docfxConfig = Join-Path $docsRoot "docfx.api.json"
$docfxOutput = Join-Path $docsRoot "_site_api/api/cs"
$localCsStatic = Join-Path $docsRoot "static/api/cs"

function Ensure-CsPlaceholder {
    $placeholder = @"
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>InfiniFrame C# API Reference</title>
</head>
<body>
<main>
  <h1>InfiniFrame C# API Reference</h1>
  <p>
    This local placeholder is replaced by generated C# API pages when DocFX is available.
  </p>
</main>
</body>
</html>
"@

    New-Item -ItemType Directory -Path $localCsStatic -Force | Out-Null

    $placeholderPath = Join-Path $localCsStatic "index.html"
    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($placeholderPath, $placeholder, $utf8NoBom)
}

function Copy-CsApi {
    if (Test-Path $localCsStatic) {
        Remove-Item $localCsStatic -Recurse -Force
    }

    New-Item -ItemType Directory -Path (Split-Path -Parent $localCsStatic) -Force | Out-Null
    Copy-Item -Path $docfxOutput -Destination $localCsStatic -Recurse -Force
}

Write-Host "Preparing local C++ API reference..."
& (Join-Path $PSScriptRoot "update-cpp-api.ps1") @(
    if ($Required) { "-Required" }
)

Write-Host "Preparing local C# API reference..."
if (-not (Get-Command docfx -ErrorAction SilentlyContinue)) {
    $message = "docfx is not installed or not on PATH. Install with: dotnet tool install --global docfx --version 2.78.5"
    if ($Required) {
        throw $message
    }

    Write-Warning $message
    Ensure-CsPlaceholder
    exit 0
}

Push-Location $repoRoot
try {
    docfx metadata $docfxConfig
    docfx build $docfxConfig

    if (-not (Test-Path (Join-Path $docfxOutput "index.html"))) {
        throw "DocFX output missing expected index at '$docfxOutput/index.html'."
    }

    Copy-CsApi
} finally {
    Pop-Location
}

Write-Host "Local API references are ready."
