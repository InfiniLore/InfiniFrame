$RootDir = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "Cleaning native build directories..."

Remove-Item -Recurse -Force "$RootDir/build" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "$RootDir/artifacts" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "$RootDir/Native/build" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "$RootDir/Native/build-clang-tidy" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "$RootDir/Native/packages" -ErrorAction SilentlyContinue

Write-Host "Native clean complete."
