$RootDir = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "Cleaning native build directories..."

Remove-Item -Recurse -Force "$RootDir/build" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "$RootDir/artifacts" -ErrorAction SilentlyContinue

Write-Host "Native clean complete."