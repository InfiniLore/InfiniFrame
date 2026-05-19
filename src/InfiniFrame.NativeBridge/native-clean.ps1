$RootDir = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "Cleaning native build directories..."

Remove-Item -Recurse -Force "$RootDir/build" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "$RootDir/artifacts" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "$RootDir/Native/build" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "$RootDir/Native/build-clang-tidy" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "$RootDir/Native/cmake-build-debug-windows" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "$RootDir/Native/cmake-build-debug-linux" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "$RootDir/Native/cmake-build-release-windows" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "$RootDir/Native/cmake-build-release-linux" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "$RootDir/Native/packages" -ErrorAction SilentlyContinue

Write-Host "Native clean complete."