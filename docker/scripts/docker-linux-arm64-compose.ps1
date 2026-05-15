$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$composeFile = Join-Path $scriptDir "..\compose\infiniframe-linux-arm64.yml"

docker compose -f $composeFile build --no-cache `
    linux-arm64-tests `
    linux-arm64-tests-playwright `
    linux-arm64-example-blazorwebview
