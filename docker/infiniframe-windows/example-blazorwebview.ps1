$ErrorActionPreference = "Stop"
. "C:\work\docker\infiniframe-windows\common.ps1"

$env:SOLUTION_FILTER = if ($env:SOLUTION_FILTER) { $env:SOLUTION_FILTER } else { "InfiniFrame.slnx" }
Initialize-CommonDefaults
Restore-Solution $script:SolutionFilter
Build-NativeProject
Build-Solution $script:SolutionFilter

dotnet run `
    --project examples/InfiniFrameExample.BlazorWebView/InfiniFrameExample.BlazorWebView.csproj `
    --configuration $script:Configuration `
    --no-build `
    --no-restore `
    /p:UseAppHost=false `
    /p:DisableImplicitNuGetFallbackFolder=true `
    /p:RestoreFallbackFolders= `
    /p:RestoreAdditionalProjectFallbackFolders=
