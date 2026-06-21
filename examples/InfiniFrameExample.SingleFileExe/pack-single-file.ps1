param(
    [string]$Configuration = "Debug",
    [string]$Framework = "net10.0",
    [string]$Rid = "auto",
    [bool]$SelfContained = $true
)

$ErrorActionPreference = "Stop"

$projectDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectPath = Join-Path $projectDir "InfiniFrameExample.SingleFileExe.csproj"
$repoPackProject = Join-Path $projectDir "..\..\src\InfiniFrame.Tools.Pack\InfiniFrame.Tools.Pack.csproj"
$localToolExe = Join-Path $HOME ".dotnet\tools\infiniframe-pack.exe"

if (Test-Path $repoPackProject) {
    $packCommand = @("dotnet", "run", "--project", $repoPackProject, "--")
}
elseif (Test-Path $localToolExe) {
    $packCommand = @($localToolExe)
}
else {
    $packCommand = @("infiniframe-pack")
}

$args = @(
    "publish",
    $projectPath,
    "--rid", $Rid,
    "--configuration", $Configuration,
    "--framework", $Framework,
    "--self-contained", $SelfContained.ToString().ToLowerInvariant()
)

$packPrefix = if ($packCommand.Length -gt 1) { $packCommand[1..($packCommand.Length - 1)] } else { @() }
& $packCommand[0] ($packPrefix + $args)
