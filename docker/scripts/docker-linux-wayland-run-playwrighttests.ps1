$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$composeFile = Join-Path $scriptDir "..\compose\infiniframe-linux-wayland.yml"

$waylandDisplayValue = if ($env:WAYLAND_DISPLAY) { $env:WAYLAND_DISPLAY } else { "wayland-0" }
if ($env:XDG_RUNTIME_DIR) {
    $xdgRuntimeDirValue = $env:XDG_RUNTIME_DIR.TrimEnd('/')
}
else {
    $uid = "1000"
    try {
        $uid = (id -u).Trim()
    }
    catch {
        $uid = "1000"
    }
    $xdgRuntimeDirValue = "/run/user/$uid"
}
$playwrightVisibleDebugValue = if ($env:PLAYWRIGHT_VISIBLE_DEBUG) { $env:PLAYWRIGHT_VISIBLE_DEBUG } else { "0" }
$playwrightVisibleDebugSecondsValue = if ($env:PLAYWRIGHT_VISIBLE_DEBUG_SECONDS) { $env:PLAYWRIGHT_VISIBLE_DEBUG_SECONDS } else { "8" }
$useHostDisplayValue = if ($env:USE_HOST_DISPLAY) { $env:USE_HOST_DISPLAY } else { "0" }
$waylandSocketPath = "$xdgRuntimeDirValue/$waylandDisplayValue"
$serviceName = "linux-wayland-tests-playwright"
$runArgs = @("run", "--rm")

if ($useHostDisplayValue -eq "1") {
    Write-Host "Using host Wayland mode."
    if (-not (Test-Path $waylandSocketPath)) {
        Write-Host "Host Wayland socket not found: $waylandSocketPath"
        Write-Host "Set USE_HOST_DISPLAY=0 to use internal Weston mode."
        exit 1
    }
    $runArgs += @(
        "-e", "USE_HOST_DISPLAY=1",
        "-e", "WAYLAND_DISPLAY=$waylandDisplayValue",
        "-e", "XDG_RUNTIME_DIR=$xdgRuntimeDirValue",
        "-e", "GDK_BACKEND=wayland",
        "-e", "QT_QPA_PLATFORM=wayland",
        "-e", "XDG_SESSION_TYPE=wayland",
        "-v", "${xdgRuntimeDirValue}:$xdgRuntimeDirValue"
    )
}
else {
    Write-Host "Using internal Weston Wayland mode."
    $runArgs += @("-e", "USE_HOST_DISPLAY=0")
}

$runArgs += @(
    "-e", "PLAYWRIGHT_VISIBLE_DEBUG=$playwrightVisibleDebugValue",
    "-e", "PLAYWRIGHT_VISIBLE_DEBUG_SECONDS=$playwrightVisibleDebugSecondsValue"
)

docker compose -f $composeFile @runArgs $serviceName
