$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$composeFile = Join-Path $scriptDir "..\docker\compose\infiniframe-windows.yml"

docker compose -f $composeFile run --rm windows-example-blazorwebview
