param(
    [switch]$Required,
    [switch]$AutoInstallDoxygen
)

$ErrorActionPreference = "Stop"

$docsRoot = Split-Path -Parent $PSScriptRoot
$repoRoot = Split-Path -Parent $docsRoot
$outputFile = Join-Path $docsRoot "api/cpp/native-cpp-reference.md"
$doxygenRoot = Join-Path $docsRoot ".doxygen"
$xmlDir = Join-Path $doxygenRoot "xml"
$doxyfilePath = Join-Path $doxygenRoot "Doxyfile.generated"

function New-PlaceholderFile {
    $content = @"
# Native C++ API Reference

This page is generated from C++ headers via Doxygen XML and moxygen.

Generation did not run in this environment (missing `doxygen` and/or `npx`).

To generate locally:

1. Install Doxygen.
2. Ensure Node.js/npm is installed.
3. Run `./docs/scripts/update-cpp-api.ps1`.
"@

    $content | Set-Content -Path $outputFile -Encoding UTF8
}

function Handle-MissingTool {
    param([string]$Message)
    if ($Required) {
        throw $Message
    }

    Write-Warning $Message
    if (-not (Test-Path $outputFile)) {
        New-PlaceholderFile
    }
    exit 0
}

function Ensure-Doxygen {
    if (Get-Command doxygen -ErrorAction SilentlyContinue) {
        return $true
    }

    if (-not $AutoInstallDoxygen) {
        return $false
    }

    Write-Host "Doxygen not found. Attempting automatic installation..."

    if (Get-Command winget -ErrorAction SilentlyContinue) {
        try {
            winget install --id DimitriVanHeesch.Doxygen --exact --accept-package-agreements --accept-source-agreements --silent
        } catch {
            Write-Warning "winget installation attempt failed: $($_.Exception.Message)"
        }
    }

    if (-not (Get-Command doxygen -ErrorAction SilentlyContinue) -and (Get-Command choco -ErrorAction SilentlyContinue)) {
        try {
            choco install doxygen.install -y --no-progress
        } catch {
            Write-Warning "choco installation attempt failed: $($_.Exception.Message)"
        }
    }

    return [bool](Get-Command doxygen -ErrorAction SilentlyContinue)
}

if (-not (Ensure-Doxygen)) {
    Handle-MissingTool "Doxygen not found on PATH. Skipping C++ API generation."
}

if (-not (Get-Command npx -ErrorAction SilentlyContinue)) {
    Handle-MissingTool "npx not found on PATH. Skipping C++ API generation."
}

New-Item -ItemType Directory -Path $doxygenRoot -Force | Out-Null
New-Item -ItemType Directory -Path (Split-Path -Parent $outputFile) -Force | Out-Null

$inputDirs = @(
    (Join-Path $repoRoot "src/InfiniFrame.Native/Core"),
    (Join-Path $repoRoot "src/InfiniFrame.Native/Types"),
    (Join-Path $repoRoot "src/InfiniFrame.Native/Utils"),
    (Join-Path $repoRoot "src/InfiniFrame.Native/Platform")
)

$inputValue = ($inputDirs | ForEach-Object { """$_""" }) -join " "

$doxyfile = @"
PROJECT_NAME           = "InfiniFrame.Native"
OUTPUT_DIRECTORY       = "$($doxygenRoot.Replace('\','/'))"
GENERATE_HTML          = NO
GENERATE_XML           = YES
GENERATE_LATEX         = NO
XML_OUTPUT             = xml
RECURSIVE              = YES
EXTRACT_ALL            = NO
EXTRACT_PRIVATE        = NO
EXTRACT_STATIC         = NO
EXTRACT_LOCAL_CLASSES  = NO
HIDE_UNDOC_MEMBERS     = YES
HIDE_UNDOC_CLASSES     = YES
FILE_PATTERNS          = *.h *.hpp
INPUT                  = $inputValue
QUIET                  = YES
WARN_IF_UNDOCUMENTED   = NO
WARN_IF_DOC_ERROR      = YES
FULL_PATH_NAMES        = NO
HAVE_DOT               = NO
EXCLUDE_SYMBOLS        = std detail
"@

$doxyfile | Set-Content -Path $doxyfilePath -Encoding UTF8

doxygen $doxyfilePath | Out-Null

if (-not (Test-Path (Join-Path $xmlDir "index.xml"))) {
    if ($Required) {
        throw "Doxygen did not produce XML index at '$xmlDir/index.xml'."
    }
    Write-Warning "Doxygen XML index not found. Skipping C++ API markdown generation."
    if (-not (Test-Path $outputFile)) {
        New-PlaceholderFile
    }
    exit 0
}

npx --yes moxygen $xmlDir --output $outputFile --language cpp --anchors --quiet | Out-Null

if (-not (Test-Path $outputFile)) {
    throw "moxygen did not generate '$outputFile'."
}

Write-Host "Generated C++ API markdown at '$outputFile'."
