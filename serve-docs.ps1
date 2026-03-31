param(
    [string]$ConfigPath = "docs/docfx.json",
    [string]$SitePath = "docs/_site"
)

$ErrorActionPreference = "Stop"

if (-not (Get-Command docfx -ErrorAction SilentlyContinue)) {
    Write-Error "docfx is not installed or not on PATH. Install with: dotnet tool install --global docfx --version 2.78.5"
}

docfx metadata $ConfigPath
docfx build $ConfigPath
docfx serve $SitePath
