$Root = Join-Path $PSScriptRoot ".."

Get-Process dotnet,MSBuild,vstest,playwright,node -ErrorAction SilentlyContinue |
    Stop-Process -Force -ErrorAction SilentlyContinue

Get-ChildItem -Path $Root -Directory -Recurse |
    Where-Object { $_.Name -in @('bin', 'obj') } |
    ForEach-Object {
        Write-Host "Deleting $($_.FullName)"
        Remove-Item $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
    }

Write-Host "Done cleaning bin/obj folders."