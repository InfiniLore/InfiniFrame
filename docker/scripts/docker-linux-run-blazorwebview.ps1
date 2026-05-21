$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$composeFile = Join-Path $scriptDir "..\compose\infiniframe-linux.yml"

$extraArgs = @()

if ($env:USE_HOST_DISPLAY -eq "1") {
    $displayValue = if ($env:DISPLAY) { $env:DISPLAY } else { ":0" }
    $extraArgs += "-e", "USE_HOST_DISPLAY=1"
    $extraArgs += "-e", "DISPLAY=$displayValue"
    $extraArgs += "-v", "/tmp/.X11-unix:/tmp/.X11-unix"
}

docker compose -f $composeFile run --rm @extraArgs linux-example-blazorwebview
