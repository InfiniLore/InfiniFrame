[CmdletBinding()]
param(
    [string]$Destination = (Join-Path $PSScriptRoot "..\..\artifacts\webview2-fixed-runtime")
)

$ErrorActionPreference = "Stop"

# This is an immutable Fixed Version Runtime CAB published by Microsoft. Keep this version in sync with the
# WebView2RuntimePath integration test and the CI cache key.
$Version = "150.0.4078.99"
$ArchiveUrl = "https://msedge.sf.dl.delivery.mp.microsoft.com/filestreamingservice/files/1c394b0d-2689-4d8b-af57-2f2018abccf6/Microsoft.WebView2.FixedVersionRuntime.150.0.4078.99.x64.cab"
$ExpectedLength = 297899501L

$destinationRoot = [IO.Path]::GetFullPath($Destination)
$runtimeRoot = Join-Path $destinationRoot $Version
$runtimeExecutable = Get-ChildItem -LiteralPath $runtimeRoot -Filter "msedgewebview2.exe" -Recurse -ErrorAction SilentlyContinue |
    Select-Object -First 1

if ($null -eq $runtimeExecutable) {
    New-Item -ItemType Directory -Force -Path $destinationRoot | Out-Null
    $archivePath = Join-Path $destinationRoot "Microsoft.WebView2.FixedVersionRuntime.$Version.x64.cab"

    if (-not (Test-Path -LiteralPath $archivePath) -or (Get-Item -LiteralPath $archivePath).Length -ne $ExpectedLength) {
        Remove-Item -LiteralPath $archivePath -Force -ErrorAction SilentlyContinue
        Invoke-WebRequest -Uri $ArchiveUrl -OutFile $archivePath
    }

    if ((Get-Item -LiteralPath $archivePath).Length -ne $ExpectedLength) {
        throw "The downloaded WebView2 fixed runtime archive has an unexpected length."
    }

    New-Item -ItemType Directory -Force -Path $runtimeRoot | Out-Null
    & expand.exe $archivePath -F:* $runtimeRoot | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Could not extract the WebView2 fixed runtime archive."
    }

    $runtimeExecutable = Get-ChildItem -LiteralPath $runtimeRoot -Filter "msedgewebview2.exe" -Recurse |
        Select-Object -First 1
}

if ($null -eq $runtimeExecutable) {
    throw "The extracted WebView2 fixed runtime does not contain msedgewebview2.exe."
}

# WebView2 expects the directory containing msedgewebview2.exe, not the archive extraction root.
Write-Output $runtimeExecutable.Directory.FullName
