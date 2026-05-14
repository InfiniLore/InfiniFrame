$RootDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Resolve-Path "$RootDir/.."

Write-Host "Cleaning native build directories..."

Remove-Item -Recurse -Force "$RootDir/build" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "$RootDir/artifacts/native" -ErrorAction SilentlyContinue

Write-Host "Native clean complete."