$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$composeFile = Join-Path $scriptDir "..\compose\infiniframe-windows.yml"

docker compose -f $composeFile build --no-cache `
    windows-tests `
    windows-tests-playwright `
    windows-example-blazorwebview
