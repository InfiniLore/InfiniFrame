$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$composeFile = Join-Path $scriptDir "..\compose\infiniframe-linux-arm64.yml"

$displayValue = if ($env:DISPLAY) { $env:DISPLAY } else { ":0" }

docker compose -f $composeFile run --rm `
    -e USE_HOST_DISPLAY=1 `
    -e DISPLAY=$displayValue `
    -v /tmp/.X11-unix:/tmp/.X11-unix `
    linux-arm64-tests
