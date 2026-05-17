$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$composeFile = Join-Path $scriptDir "..\compose\infiniframe-linux.yml"

docker compose -f $composeFile build --no-cache `
    linux-tests `
    linux-tests-playwright `
    linux-example-blazorwebview
