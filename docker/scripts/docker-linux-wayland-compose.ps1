$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$composeFile = Join-Path $scriptDir "..\compose\infiniframe-linux-wayland.yml"

docker compose -f $composeFile build --no-cache `
    linux-wayland-tests `
    linux-wayland-tests-playwright `
    linux-wayland-example-blazorwebview
