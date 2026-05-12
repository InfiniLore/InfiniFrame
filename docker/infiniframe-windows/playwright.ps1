$ErrorActionPreference = "Stop"
. "C:\work\docker\infiniframe-windows\common.ps1"

Initialize-CommonDefaults
$frameworks = if ($env:FRAMEWORKS) { $env:FRAMEWORKS -split " " } else { @("net8.0", "net9.0", "net10.0") }

Restore-Solution $script:SolutionFilter
Build-NativeProject
Build-Solution $script:SolutionFilter

foreach ($framework in $frameworks) {
    dotnet test --solution $script:SolutionFilter `
        --configuration $script:Configuration `
        --no-build `
        --no-restore `
        --framework $framework `
        /p:UseAppHost=false `
        /p:DisableImplicitNuGetFallbackFolder=true `
        /p:RestoreFallbackFolders= `
        /p:RestoreAdditionalProjectFallbackFolders=
}
