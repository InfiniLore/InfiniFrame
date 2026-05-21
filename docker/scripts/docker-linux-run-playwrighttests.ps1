$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$composeFile = Join-Path $scriptDir "..\compose\infiniframe-linux.yml"

$displayValue = if ($env:DISPLAY) { $env:DISPLAY } else { ":0" }
$playwrightVisibleDebugValue = if ($env:PLAYWRIGHT_VISIBLE_DEBUG) { $env:PLAYWRIGHT_VISIBLE_DEBUG } else { "0" }
$playwrightVisibleDebugSecondsValue = if ($env:PLAYWRIGHT_VISIBLE_DEBUG_SECONDS) { $env:PLAYWRIGHT_VISIBLE_DEBUG_SECONDS } else { "8" }

docker compose -f $composeFile run --rm `
    -e USE_HOST_DISPLAY=1 `
    -e DISPLAY=$displayValue `
    -e PLAYWRIGHT_VISIBLE_DEBUG=$playwrightVisibleDebugValue `
    -e PLAYWRIGHT_VISIBLE_DEBUG_SECONDS=$playwrightVisibleDebugSecondsValue `
    -v /tmp/.X11-unix:/tmp/.X11-unix `
    linux-tests-playwright
