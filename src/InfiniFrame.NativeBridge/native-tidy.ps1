param(
    [string]$BuildDirectoryName = "build-clang-tidy",
    [switch]$ApplyFixes,
    [switch]$FixErrors
)

$ErrorActionPreference = "Stop"

function Invoke-ExternalCommand {
    param(
        [Parameter(Mandatory = $true)][string]$FilePath,
        [string[]]$Arguments = @()
    )

    & $FilePath @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "Command failed with exit code ${LASTEXITCODE}: $FilePath $($Arguments -join ' ')"
    }
}

function Get-VsInstallationPath {
    $vsWhere = Join-Path ${env:ProgramFiles(x86)} "Microsoft Visual Studio\Installer\vswhere.exe"
    if (-not (Test-Path $vsWhere)) {
        return $null
    }

    $path = & $vsWhere -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($path)) {
        return $null
    }

    return $path.Trim()
}

function Import-VsDevEnvironment {
    param([Parameter(Mandatory = $true)][string]$VsDevCmdPath)

    $envDump = & cmd.exe /s /c "`"$VsDevCmdPath`" -arch=x64 -host_arch=x64 >nul && set"
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to initialize Visual Studio developer environment using '$VsDevCmdPath'."
    }

    foreach ($line in $envDump) {
        $separatorIndex = $line.IndexOf("=")
        if ($separatorIndex -lt 1) {
            continue
        }

        $name = $line.Substring(0, $separatorIndex)
        $value = $line.Substring($separatorIndex + 1)
        Set-Item -Path "env:$name" -Value $value
    }
}

$NativeRoot = Join-Path $PSScriptRoot "Native"
$BuildDirectory = Join-Path $NativeRoot $BuildDirectoryName
$DependenciesRoot = Join-Path $NativeRoot "Dependencies"

Push-Location $NativeRoot

try {
    if ($FixErrors -and -not $ApplyFixes) {
        $ApplyFixes = $true
    }

    $clangTidy = Get-Command clang-tidy -ErrorAction SilentlyContinue
    if (-not $clangTidy) {
        throw "clang-tidy was not found on PATH."
    }

    $vsInstallPath = Get-VsInstallationPath
    if (-not $vsInstallPath) {
        throw "Could not locate a Visual Studio installation with C++ tools."
    }

    $vsDevCmd = Join-Path $vsInstallPath "Common7\Tools\VsDevCmd.bat"
    if (-not (Test-Path $vsDevCmd)) {
        throw "VsDevCmd.bat not found at '$vsDevCmd'."
    }

    Write-Host "Initializing Visual Studio developer environment..."
    Import-VsDevEnvironment -VsDevCmdPath $vsDevCmd

    $ninja = Get-Command ninja -ErrorAction SilentlyContinue
    if (-not $ninja) {
        throw "ninja was not found on PATH after VsDevCmd initialization."
    }

    Write-Host "Generating compile_commands.json in '$BuildDirectoryName'..."
    Invoke-ExternalCommand -FilePath "cmake" -Arguments @(
        "-S", ".",
        "-B", $BuildDirectory,
        "--fresh",
        "-G", "Ninja",
        "-DCMAKE_MAKE_PROGRAM=$($ninja.Source)",
        "-DCMAKE_BUILD_TYPE=Debug",
        "-DCMAKE_DISABLE_PRECOMPILE_HEADERS=ON",
        "-DCMAKE_CXX_SCAN_FOR_MODULES=OFF",
        "-DCMAKE_EXPORT_COMPILE_COMMANDS=ON"
    )

    $compileCommandsPath = Join-Path $BuildDirectory "compile_commands.json"
    if (-not (Test-Path $compileCommandsPath)) {
        throw "compile_commands.json was not generated at '$compileCommandsPath'."
    }

    Write-Host "Running clang-tidy..."

    $sourceFiles = Get-ChildItem . -Recurse -File -Include *.cpp,*.cc,*.cxx,*.c,*.mm |
        Where-Object {
            $_.FullName -notmatch '\\(build($|[-_][^\\]+)?|out|bin|obj|vcpkg|Dependencies|packages)\\'
        }

    foreach ($sourceFile in $sourceFiles) {
        Write-Host "Running clang-tidy on $($sourceFile.FullName)"

        $tidyArgs = @(
            $sourceFile.FullName,
            "-p", $BuildDirectory,
            "--header-filter=^$",
            "--extra-arg=/external:I$DependenciesRoot",
            "--extra-arg=/external:W0",
            "--extra-arg=-Wno-c++11-narrowing"
        )

        if ($ApplyFixes) {
            $tidyArgs += "--fix"
        }

        if ($ApplyFixes -and $FixErrors) {
            $tidyArgs += "--fix-errors"
        }

        Invoke-ExternalCommand -FilePath $clangTidy.Source -Arguments $tidyArgs
    }

    Write-Host "clang-tidy complete."
}
finally {
    Pop-Location
}
