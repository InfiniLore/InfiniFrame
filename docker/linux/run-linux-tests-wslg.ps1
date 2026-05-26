param(
    [string]$Distro = "",
    [switch]$Build,
    [switch]$NoNativeDiagnostics
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$drive = $repoRoot.Substring(0, 1).ToLowerInvariant()
$rest = $repoRoot.Substring(2).Replace("\", "/")
$wslRepo = "/mnt/$drive$rest"
$wslBuildArg = if ($Build) { "--build" } else { "" }
$wslDiagArg = if ($NoNativeDiagnostics) { "--no-native-diagnostics" } else { "" }
$wslCommand = "cd '$wslRepo' && bash ./docker/linux/run-linux-tests-wslg.sh $wslBuildArg $wslDiagArg"

if ([string]::IsNullOrWhiteSpace($Distro)) {
    & wsl.exe -- bash -lc $wslCommand
}
else {
    & wsl.exe -d $Distro -- bash -lc $wslCommand
}

exit $LASTEXITCODE
