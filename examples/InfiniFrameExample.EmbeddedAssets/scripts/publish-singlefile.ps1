param(
    [string]$Runtime = "win-x64",
    [string]$Configuration = "Release",
    [bool]$SelfContained = $true,
    [string]$Framework = "net10.0",
    [switch]$SkipNativeBuild
)

$ErrorActionPreference = "Stop"

$projectDir = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $projectDir "InfiniFrameExample.EmbeddedAssets.csproj"
$repoRoot = Split-Path -Parent (Split-Path -Parent $projectDir)
$nativeProjPath = Join-Path $repoRoot "src/InfiniFrame.Native/InfiniFrame.Native.proj"

if (-not (Test-Path $projectPath)) {
    throw "Project file not found at: $projectPath"
}

if (-not (Test-Path $nativeProjPath)) {
    throw "Native project file not found at: $nativeProjPath"
}

switch -Wildcard ($Runtime) {
    "*arm64*" { $platform = "arm64"; break }
    default { $platform = "x64"; break }
}

if (-not $SkipNativeBuild) {
    Write-Host "Building native runtime..."
    Write-Host "Native project: $nativeProjPath"
    Write-Host "Platform: $platform | Configuration: $Configuration"

    dotnet msbuild $nativeProjPath `
        /t:Build `
        /p:Configuration=$Configuration `
        /p:Platform=$platform `
        /p:SolutionDir="$repoRoot\"

    if ($LASTEXITCODE -ne 0) {
        throw "Native build failed with exit code $LASTEXITCODE"
    }
}

Write-Host "Publishing single-file app..."
Write-Host "Project: $projectPath"
Write-Host "Runtime: $Runtime | Configuration: $Configuration | Framework: $Framework | SelfContained: $SelfContained"

dotnet publish $projectPath `
    -c $Configuration `
    -r $Runtime `
    -f $Framework `
    -p:PublishSingleFile=true `
    -p:SelfContained=$SelfContained `
    -p:SolutionDir="$repoRoot\" `
    -p:IncludeNativeLibrariesForSelfExtract=true

if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed with exit code $LASTEXITCODE"
}

$publishDir = Join-Path $projectDir "bin\$Configuration\$Framework\$Runtime\publish"
Write-Host "Publish completed: $publishDir"
