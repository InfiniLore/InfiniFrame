param(
    [string]$Configuration = "Debug",
    [string]$Arch = "x64",
    [string]$EnableTestExports = "",
    [string]$WebView2Version = "",
    [string]$WindowsImplementationLibraryVersion = "",
    [string]$NuGetPackageRoot = ""
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

if ([string]::IsNullOrWhiteSpace($EnableTestExports)) {
    $EnableTestExports = if ($Configuration -ieq "Debug") { "true" } else { "false" }
}

$EnableTestExportsCMakeValue = if ($EnableTestExports -ieq "true") { "ON" } else { "OFF" }

# -----------------------------------------------------------------------------------------------------------------
# LOCK (blocking, CI-safe, race-free)
# -----------------------------------------------------------------------------------------------------------------
$LockFile = Join-Path $ArtifactsDir ".build.lock"
$LockTimeoutSeconds = 600
$LockDeadline = (Get-Date).AddSeconds($LockTimeoutSeconds)

$LockStream = $null

try {

    # Wait until lock becomes available (prevents crash)
    while ($true) {
        if ((Get-Date) -ge $LockDeadline) {
            throw "Timed out after $LockTimeoutSeconds seconds waiting for native build lock at '$LockFile'."
        }

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
    Write-Host "Test Exports : $EnableTestExports"
    Write-Host "========================================="

    # -----------------------------------------------------------------------------------------------------------------
    # CMAKE CONFIG
    # -----------------------------------------------------------------------------------------------------------------
    $CMakeArgs = @()

    if ($Platform -eq "osx") {
        if ($Arch -eq "arm64") {
            $CMakeArgs += "-DCMAKE_OSX_ARCHITECTURES=arm64"
        }
        elseif ($Arch -eq "x64") {
            $CMakeArgs += "-DCMAKE_OSX_ARCHITECTURES=x86_64"
        }
        else {
            throw "Unsupported macOS architecture '$Arch'. Expected 'x64' or 'arm64'."
        }
    }

    if ($Platform -eq "linux") {
        if ($Arch -ne "x64" -and $Arch -ne "arm64") {
            throw "Unsupported Linux architecture '$Arch'. Expected 'x64' or 'arm64'."
        }
    }

    if ($Platform -eq "windows") {
        if ($Arch -eq "arm64") {
            $CMakeArgs += "-A"
            $CMakeArgs += "ARM64"
        }
        elseif ($Arch -eq "x64") {
            $CMakeArgs += "-A"
            $CMakeArgs += "x64"
        }
        else {
            throw "Unsupported Windows architecture '$Arch'. Expected 'x64' or 'arm64'."
        }
    }

    $CMakeArgs += "-DINFINIFRAME_BUILD_TEST_EXPORTS=$EnableTestExportsCMakeValue"
    if (-not [string]::IsNullOrWhiteSpace($WebView2Version)) {
        $CMakeArgs += "-DINFINIFRAME_WEBVIEW2_VERSION:STRING=$WebView2Version"
    }

    if (-not [string]::IsNullOrWhiteSpace($WindowsImplementationLibraryVersion)) {
        $CMakeArgs += "-DINFINIFRAME_WINDOWS_IMPLEMENTATION_LIBRARY_VERSION:STRING=$WindowsImplementationLibraryVersion"
    }

    if (-not [string]::IsNullOrWhiteSpace($NuGetPackageRoot)) {
        $CMakeArgs += "-DINFINIFRAME_NUGET_PACKAGES_ROOT:STRING=$NuGetPackageRoot"
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
