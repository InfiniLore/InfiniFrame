param(
    [string]$Configuration = "Debug",
    [string]$Arch = "x64"
)

$ErrorActionPreference = "Stop"

# -----------------------------------------------------------------------------------------------------------------
# PATH SETUP
# -----------------------------------------------------------------------------------------------------------------
$RootDir = Split-Path -Parent $MyInvocation.MyCommand.Path

$NativeDir    = Join-Path $RootDir "Native"
$BuildDir     = Join-Path $RootDir "build/$Arch/$Configuration"
$ArtifactsDir = Join-Path $RootDir "artifacts/native"

# -----------------------------------------------------------------------------------------------------------------
# ENSURE DIRECTORIES EXIST
# -----------------------------------------------------------------------------------------------------------------
New-Item -ItemType Directory -Force -Path $NativeDir | Out-Null
New-Item -ItemType Directory -Force -Path $BuildDir | Out-Null
New-Item -ItemType Directory -Force -Path $ArtifactsDir | Out-Null

$Platform = if ($IsWindows) { "windows" }
elseif ($IsLinux) { "linux" }
else { "osx" }

New-Item -ItemType Directory -Force -Path "$ArtifactsDir/$Platform/$Arch/$Configuration" | Out-Null

# -----------------------------------------------------------------------------------------------------------------
# LOCK (blocking, CI-safe, race-free)
# -----------------------------------------------------------------------------------------------------------------
$LockFile = Join-Path $ArtifactsDir ".build.lock"

$LockStream = $null

try {

    # Wait until lock becomes available (prevents crash)
    while ($true) {
        try {
            $LockStream = New-Object System.IO.FileStream(
            $LockFile,
            [System.IO.FileMode]::OpenOrCreate,
            [System.IO.FileAccess]::ReadWrite,
            [System.IO.FileShare]::None
            )
            break
        }
        catch {
            Start-Sleep -Milliseconds 200
        }
    }

    # -----------------------------------------------------------------------------------------------------------------
    # INFO
    # -----------------------------------------------------------------------------------------------------------------
    Write-Host "========================================="
    Write-Host "Building InfiniFrame.Native"
    Write-Host "Configuration: $Configuration"
    Write-Host "Architecture : $Arch"
    Write-Host "Platform     : $Platform"
    Write-Host "========================================="

    # -----------------------------------------------------------------------------------------------------------------
    # CMAKE CONFIG
    # -----------------------------------------------------------------------------------------------------------------
    $CMakeArgs = @()

    if ($Platform -eq "osx") {
        if ($Arch -eq "arm64") {
            $CMakeArgs += "-DCMAKE_OSX_ARCHITECTURES=arm64"
        } else {
            $CMakeArgs += "-DCMAKE_OSX_ARCHITECTURES=x86_64"
        }
    }

    cmake -B $BuildDir -S $NativeDir @CMakeArgs
    if ($LASTEXITCODE -ne 0) {
        throw "CMake configure failed with exit code $LASTEXITCODE."
    }

    cmake --build $BuildDir --config $Configuration --parallel
    if ($LASTEXITCODE -ne 0) {
        throw "CMake build failed with exit code $LASTEXITCODE."
    }

    # -----------------------------------------------------------------------------------------------------------------
    # COPY OUTPUTS
    # -----------------------------------------------------------------------------------------------------------------
    Get-ChildItem $BuildDir -Recurse -Include *.dll,*.so,*.dylib -ErrorAction SilentlyContinue |
        Copy-Item -Destination "$ArtifactsDir/$Platform/$Arch/$Configuration" -Force

    Write-Host "Native build complete."
}
finally {
    if ($LockStream) {
        $LockStream.Dispose()
    }
}
