$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$composeFile = Join-Path $scriptDir "..\compose\infiniframe-windows.yml"
$env:DOCKER_BUILDKIT = "0"
$env:COMPOSE_DOCKER_CLI_BUILD = "0"

docker compose -f $composeFile build --no-cache `
    windows-tests `
    windows-tests-playwright `
    windows-example-blazorwebview `
    windows-trim-aot
