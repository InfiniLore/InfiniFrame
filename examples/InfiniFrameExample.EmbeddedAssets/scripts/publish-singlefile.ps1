param(
    [string]$Rid = "auto",
    [string]$Configuration = "Release",
    [string]$Framework = "net10.0",
    [bool]$SelfContained = $true,
    [string]$Output = "",
    [switch]$NoRestore,
    [switch]$Verbose
)

$ErrorActionPreference = "Stop"

$projectDir = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $projectDir "InfiniFrameExample.EmbeddedAssets.csproj"

if (-not (Test-Path $projectPath)) {
    throw "Project file not found at: $projectPath"
}

$msbuildArgs = @(
    "msbuild",
    $projectPath,
    "-t:InfiniFramePackPublish",
    "-p:Configuration=$Configuration",
    "-p:TargetFramework=$Framework",
    "-p:InfiniFramePackRid=$Rid",
    "-p:InfiniFramePackSelfContained=$($SelfContained.ToString().ToLowerInvariant())"
)

if (-not [string]::IsNullOrWhiteSpace($Output)) {
    $msbuildArgs += "-p:InfiniFramePackOutput=$Output"
}

if ($NoRestore) {
    $msbuildArgs += "-p:InfiniFramePackNoRestore=true"
}

if ($Verbose) {
    $msbuildArgs += "-p:InfiniFramePackVerbose=true"
}

Write-Host "Publishing via MSBuild target InfiniFramePackPublish..."
Write-Host "Project: $projectPath"
Write-Host "RID: $Rid | Configuration: $Configuration | Framework: $Framework | SelfContained: $SelfContained"
if (-not [string]::IsNullOrWhiteSpace($Output)) {
    Write-Host "Output: $Output"
}

dotnet @msbuildArgs

if ($LASTEXITCODE -ne 0) {
    throw "InfiniFramePackPublish failed with exit code $LASTEXITCODE"
}
