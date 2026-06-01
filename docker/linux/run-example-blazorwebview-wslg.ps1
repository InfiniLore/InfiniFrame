param(
    [string]$Distro = "",
    [switch]$Build
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$drive = $repoRoot.Substring(0, 1).ToLowerInvariant()
$rest = $repoRoot.Substring(2).Replace("\", "/")
$wslRepo = "/mnt/$drive$rest"
$wslBuildArg = if ($Build) { "--build" } else { "" }
$wslCommand = "cd '$wslRepo' && bash ./docker/linux/run-example-blazorwebview-wslg.sh $wslBuildArg"

if ([string]::IsNullOrWhiteSpace($Distro)) {
    & wsl.exe -- bash -lc $wslCommand
}
else {
    & wsl.exe -d $Distro -- bash -lc $wslCommand
}

exit $LASTEXITCODE
