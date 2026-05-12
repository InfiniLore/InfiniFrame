$ErrorActionPreference = "Stop"
. "C:\work\docker\infiniframe-windows\common.ps1"

Initialize-CommonDefaults
Restore-Solution $script:SolutionFilter
Build-NativeProject
Build-Solution $script:SolutionFilter

dotnet test --solution $script:SolutionFilter `
    --configuration $script:Configuration `
    --no-build `
    --no-restore `
    /p:UseAppHost=false `
    /p:DisableImplicitNuGetFallbackFolder=true `
    /p:RestoreFallbackFolders= `
    /p:RestoreAdditionalProjectFallbackFolders=
