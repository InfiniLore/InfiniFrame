param(
    [string]$Profile = "",
    [switch]$SkipNativeBuild
)

$ErrorActionPreference = "Stop"

$projectDir = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $projectDir "InfiniFrameExample.EmbeddedAssets.csproj"
$profilesDir = Join-Path $projectDir "Properties/PublishProfiles"
$repoRoot = Split-Path -Parent (Split-Path -Parent $projectDir)
$nativeProjPath = Join-Path $repoRoot "src/InfiniFrame.Native/InfiniFrame.Native.proj"

if (-not (Test-Path $projectPath)) {
    throw "Project file not found at: $projectPath"
}

if (-not (Test-Path $profilesDir)) {
    throw "PublishProfiles folder not found at: $profilesDir"
}

function Resolve-DefaultProfileName {
    $osPrefix = if ($IsWindows) {
        "Win"
    }
    elseif ($IsLinux) {
        "Linux"
    }
    elseif ($IsMacOS) {
        "Osx"
    }
    else {
        throw "Unsupported OS for automatic publish profile selection."
    }

    $archName = switch ([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture) {
        ([System.Runtime.InteropServices.Architecture]::X64) { "X64" }
        ([System.Runtime.InteropServices.Architecture]::Arm64) { "Arm64" }
        default { throw "Unsupported architecture for automatic publish profile selection." }
    }

    return "$($osPrefix)$($archName)SingleFile"
}

function Resolve-RidFromProfileName([string]$profileName) {
    switch ($profileName) {
        "WinX64SingleFile" { return "win-x64" }
        "WinArm64SingleFile" { return "win-arm64" }
        "LinuxX64SingleFile" { return "linux-x64" }
        "LinuxArm64SingleFile" { return "linux-arm64" }
        "OsxX64SingleFile" { return "osx-x64" }
        "OsxArm64SingleFile" { return "osx-arm64" }
        default { throw "Unsupported profile name for RID mapping: $profileName" }
    }
}

function Resolve-NativeInfoFromRid([string]$rid) {
    switch -Wildcard ($rid) {
        "win-*" {
            $platform = if ($rid -like "*arm64*") { "arm64" } else { "x64" }
            return @{
                Platform = $platform
            }
        }
        "linux-*" {
            $platform = if ($rid -like "*arm64*") { "arm64" } else { "x64" }
            return @{
                Platform = $platform
            }
        }
        "osx-*" {
            $platform = if ($rid -like "*arm64*") { "arm64" } else { "x64" }
            return @{
                Platform = $platform
            }
        }
        default {
            throw "Unsupported RID for native mapping: $rid"
        }
    }
}

$resolvedProfile = if ([string]::IsNullOrWhiteSpace($Profile)) {
    Resolve-DefaultProfileName
}
else {
    $Profile
}

$profilePath = Join-Path $profilesDir "$resolvedProfile.pubxml"
if (-not (Test-Path $profilePath)) {
    throw "Publish profile not found: $profilePath"
}

$rid = Resolve-RidFromProfileName $resolvedProfile
$nativeInfo = Resolve-NativeInfoFromRid $rid

if (-not $SkipNativeBuild) {
    if (-not (Test-Path $nativeProjPath)) {
        throw "Native project file not found at: $nativeProjPath"
    }

    Write-Host "Building native runtime..."
    Write-Host "Native project: $nativeProjPath"
    Write-Host "RID: $rid | Platform: $($nativeInfo.Platform)"

    dotnet msbuild $nativeProjPath `
        /t:Build `
        /p:Configuration=Release `
        /p:Platform=$($nativeInfo.Platform) `
        /p:SolutionDir="$repoRoot\"

    if ($LASTEXITCODE -ne 0) {
        throw "Native build failed with exit code $LASTEXITCODE"
    }
}

Write-Host "Publishing single-file app..."
Write-Host "Project: $projectPath"
Write-Host "Profile: $resolvedProfile"
Write-Host "Profile file: $profilePath"
Write-Host "SkipNativeBuild: $SkipNativeBuild"

$publishArgs = @(
    "publish",
    $projectPath,
    "-p:PublishProfile=$resolvedProfile",
    "-p:SolutionDir=$repoRoot\",
    "-p:BuildNativeOnPublish=false",
    "--nologo"
)

$publishDir = Join-Path $projectDir "bin/Release/net10.0/$rid/publish"
if (Test-Path $publishDir) {
    Write-Host "Cleaning previous publish directory: $publishDir"
    Remove-Item -Path $publishDir -Recurse -Force
}

dotnet @publishArgs

if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed with exit code $LASTEXITCODE"
}

Write-Host "Publish completed: $publishDir"
