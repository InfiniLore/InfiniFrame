$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

& (Join-Path $scriptDir "docker-windows-run-tests.ps1")
& (Join-Path $scriptDir "docker-windows-run-playwrighttests.ps1")
& (Join-Path $scriptDir "docker-windows-run-trim-aot.ps1")
