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
$repoRoot = Split-Path -Parent (Split-Path -Parent $projectDir)
$toolProject = Join-Path $repoRoot "src/InfiniFrame.Tools.Pack/InfiniFrame.Tools.Pack.csproj"

if (-not (Test-Path $projectPath)) {
    throw "Project file not found at: $projectPath"
}

if (-not (Test-Path $toolProject)) {
    throw "InfiniFrame.Tools.Pack project not found at: $toolProject"
}

$toolArgs = @(
    "run",
    "--project", $toolProject,
    "--",
    "publish",
    $projectPath,
    "--rid", $Rid,
    "--configuration", $Configuration,
    "--framework", $Framework,
    "--self-contained", $SelfContained.ToString().ToLowerInvariant()
)

if (-not [string]::IsNullOrWhiteSpace($Output)) {
    $toolArgs += @("--output", $Output)
}

if ($NoRestore) {
    $toolArgs += "--no-restore"
}

if ($Verbose) {
    $toolArgs += "--verbose"
}

Write-Host "Publishing via InfiniFrame.Tools.Pack..."
Write-Host "Tool: $toolProject"
Write-Host "Project: $projectPath"
Write-Host "RID: $Rid | Configuration: $Configuration | Framework: $Framework | SelfContained: $SelfContained"
if (-not [string]::IsNullOrWhiteSpace($Output)) {
    Write-Host "Output: $Output"
}

dotnet @toolArgs

if ($LASTEXITCODE -ne 0) {
    throw "InfiniFrame.Tools.Pack publish failed with exit code $LASTEXITCODE"
}
