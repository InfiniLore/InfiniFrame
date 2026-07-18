param(
    [string]$Configuration = "Debug",
    [string]$Arch = "x64",
    [string]$EnableTestExports = "",
    [ValidateSet("None", "AddressUndefined", "Thread")]
    [string]$Sanitizer = "None",
    [switch]$Clean
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

function Test-VisualStudioGeneratorAvailable {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Generator
    )

    $vswherePath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
    if (-not (Test-Path -LiteralPath $vswherePath)) {
        return $false
    }

    $versionRange = switch -Regex ($Generator) {
        "Visual Studio 18 2026" { "[18.0,19.0)" }
        "Visual Studio 17 2022" { "[17.0,18.0)" }
        default { $null }
    }

    if ([string]::IsNullOrWhiteSpace($versionRange)) {
        return $false
    }

    $installationPath = & $vswherePath -latest -products * -requires Microsoft.Component.MSBuild -version $versionRange -property installationPath
    return -not [string]::IsNullOrWhiteSpace($installationPath)
}

# -----------------------------------------------------------------------------------------------------------------
# LOCK (blocking, CI-safe, race-free)
# -----------------------------------------------------------------------------------------------------------------
$LockFile = Join-Path $ArtifactsDir ".build.lock"
$LockTimeoutSeconds = 600
$LockDeadline = (Get-Date).AddSeconds($LockTimeoutSeconds)

$LockStream = $null

try {

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
    Write-Host "Sanitizer    : $Sanitizer"
    Write-Host "========================================="

    Write-Host ""
    Write-Host "CMake Version:"
    cmake --version
    Write-Host ""

    # -----------------------------------------------------------------------------------------------------------------
    # BUILD DIRECTORY
    # -----------------------------------------------------------------------------------------------------------------
    # Preserve CMake's build graph between invocations. MSBuild calls this script only
    # when an input changes; deleting the build directory here discarded all dependency
    # scanning, object files, PCHs, and compiler-cache hits on every incremental build.
    if ($Clean -and (Test-Path $BuildDir)) {
        Write-Host "Removing native build directory because -Clean was requested."
        Remove-Item $BuildDir -Recurse -Force
    }

    New-Item -ItemType Directory -Force -Path $BuildDir | Out-Null

    # -----------------------------------------------------------------------------------------------------------------
    # CMAKE CONFIG
    # -----------------------------------------------------------------------------------------------------------------
    $CMakeArgs = @()

    if ($Platform -eq "osx") {
        switch ($Arch) {
            "arm64" { $CMakeArgs += "-DCMAKE_OSX_ARCHITECTURES=arm64" }
            "x64"   { $CMakeArgs += "-DCMAKE_OSX_ARCHITECTURES=x86_64" }
            default { throw "Unsupported macOS architecture '$Arch'. Expected 'x64' or 'arm64'." }
        }
    }

    if ($Platform -eq "linux") {
        if ($Arch -notin @("x64", "arm64")) {
            throw "Unsupported Linux architecture '$Arch'. Expected 'x64' or 'arm64'."
        }
    }

    if ($Platform -eq "windows") {
        $CMakeArchitecture = switch ($Arch) {
            "x64" { "x64" }
            "arm64" { "ARM64" }
            default { throw "Unsupported Windows architecture '$Arch'. Expected 'x64' or 'arm64'." }
        }

        $generatorCandidates = @("Visual Studio 18 2026", "Visual Studio 17 2022")
        $selectedGenerator = $null

        foreach ($candidate in $generatorCandidates) {
            if (Test-VisualStudioGeneratorAvailable -Generator $candidate) {
                $selectedGenerator = $candidate
                break
            }
        }

        if ([string]::IsNullOrWhiteSpace($selectedGenerator)) {
            $selectedGenerator = "Visual Studio 17 2022"
            Write-Warning "Could not detect an installed Visual Studio instance via vswhere. Falling back to generator '$selectedGenerator'."
        }

        $CMakeArgs += "-G"
        $CMakeArgs += $selectedGenerator
        $CMakeArgs += "-A"
        $CMakeArgs += $CMakeArchitecture
    }
    elseif (Get-Command ninja -ErrorAction SilentlyContinue) {
        # Ninja has lower build-system overhead and better parallel scheduling than
        # makefiles. Do not require it: the platform default remains a portable fallback.
        $CMakeArgs += "-G"
        $CMakeArgs += "Ninja"
    }

    $CMakeArgs += "-DINFINIFRAME_BUILD_TEST_EXPORTS=$EnableTestExportsCMakeValue"
    $CMakeArgs += "-DINFINIFRAME_SANITIZER=$Sanitizer"

    Write-Host ""
    Write-Host "CMake Configure Arguments:"
    Write-Host ($CMakeArgs -join " ")
    Write-Host ""

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
    Get-ChildItem $BuildDir -Recurse -Include *.dll, *.so, *.dylib -ErrorAction SilentlyContinue |
        Copy-Item -Destination "$ArtifactsDir/$Platform/$Arch/$Configuration" -Force

    Write-Host ""
    Write-Host "Native build complete."
}
finally {
    if ($LockStream) {
        $LockStream.Dispose()
    }
}
