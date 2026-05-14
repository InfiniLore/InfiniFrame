param(
    [string]$Configuration = "Debug",
    [string]$Arch = "x64"
)

$RootDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Resolve-Path "$RootDir/.."

$NativeDir = Join-Path $RootDir "native"
$BuildDir = Join-Path $RootDir "build/$Arch/$Configuration"
$ArtifactsDir = Join-Path $RootDir "artifacts/native"

$LockFile = Join-Path $ArtifactsDir ".build.lock"

$LockStream = New-Object System.IO.FileStream(
    $LockFile,
    [System.IO.FileMode]::OpenOrCreate,
    [System.IO.FileAccess]::ReadWrite,
    [System.IO.FileShare]::None
)

$OS = $PSVersionTable.OS
if ($IsWindows) {
    $Platform = "windows"
} elseif ($IsLinux) {
    $Platform = "linux"
} else {
    $Platform = "osx"
}

New-Item -ItemType Directory -Force -Path $BuildDir | Out-Null
New-Item -ItemType Directory -Force -Path "$ArtifactsDir/$Platform/$Arch/$Configuration" | Out-Null

Write-Host "========================================="
Write-Host "Building InfiniFrame.nativeLib"
Write-Host "Configuration: $Configuration"
Write-Host "Architecture : $Arch"
Write-Host "Platform     : $Platform"
Write-Host "========================================="

$CMakeArgs = @()

if ($Platform -eq "osx") {
    if ($Arch -eq "arm64") {
        $CMakeArgs += "-DCMAKE_OSX_ARCHITECTURES=arm64"
    } else {
        $CMakeArgs += "-DCMAKE_OSX_ARCHITECTURES=x86_64"
    }
}

cmake -B $BuildDir -S $NativeDir @CMakeArgs
cmake --build $BuildDir --config $Configuration --parallel

Get-ChildItem $BuildDir -Recurse -Include *.dll,*.so,*.dylib |
    Copy-Item -Destination "$ArtifactsDir/$Platform/$Arch/$Configuration" -Force

Write-Host "Native build complete."
$LockStream.Dispose()