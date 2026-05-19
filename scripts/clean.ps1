$Root = Join-Path $PSScriptRoot ".."

Get-Process dotnet,MSBuild,vstest,playwright,node -ErrorAction SilentlyContinue |
    Stop-Process -Force -ErrorAction SilentlyContinue

Get-ChildItem -Path $Root -Directory -Recurse |
    Where-Object { $_.Name -in @('bin', 'obj') } |
    ForEach-Object {
        Write-Host "Deleting $($_.FullName)"
        Remove-Item $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
    }

# Additional cleanup paths
$ExtraPaths = @(
    "../src/InfiniFrame.NativeBridge/build",
    "../src/InfiniFrame.NativeBridge/artifacts",
    "../src/InfiniFrame.Js/node_modules",
    "../src/InfiniFrame.NativeBridge/Native/cmake-build-debug-linux",
    "../src/InfiniFrame.NativeBridge/Native/cmake-build-debug-windows",
    "../src/InfiniFrame.NativeBridge/Native/cmake-build-release-linux",
    "../src/InfiniFrame.NativeBridge/Native/cmake-build-release-windows",
    "../src/InfiniFrame.NativeBridge/Native/packages"
)

foreach ($RelativePath in $ExtraPaths) {
    $FullPath = Join-Path $Root $RelativePath

    if (Test-Path $FullPath) {
        Write-Host "Deleting $FullPath"
        Remove-Item $FullPath -Recurse -Force -ErrorAction SilentlyContinue
    }
}

Write-Host "Done cleaning bin/obj folders and extra build artifacts."