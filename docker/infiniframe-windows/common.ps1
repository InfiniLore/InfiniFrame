$ErrorActionPreference = "Stop"

function Initialize-CommonDefaults {
    $script:Configuration = if ($env:CONFIGURATION) { $env:CONFIGURATION } else { "Release" }
    $script:NativePlatform = if ($env:NATIVE_PLATFORM) { $env:NATIVE_PLATFORM } else { "x64" }
    $script:SolutionFilter = if ($env:SOLUTION_FILTER) { $env:SOLUTION_FILTER } else { "InfiniFrame.GitHubActions.Testing.slnf" }
    $script:NuGetPackages = if ($env:NUGET_PACKAGES) { $env:NUGET_PACKAGES } else { "C:\.nuget\packages" }
    $script:NuGetConfigFile = if ($env:NUGET_CONFIG_FILE) { $env:NUGET_CONFIG_FILE } else { "C:\work\docker\infiniframe-windows\NuGet.Config" }
    $script:SkipNativeBuild = if ($env:SKIP_NATIVE_BUILD) { [int]$env:SKIP_NATIVE_BUILD } else { 1 }
}

function Restore-Solution {
    param([string]$FilterPath)
    dotnet restore $FilterPath `
        --force `
        --force-evaluate `
        --configfile $script:NuGetConfigFile `
        --packages $script:NuGetPackages `
        /p:DisableImplicitNuGetFallbackFolder=true `
        /p:RestoreFallbackFolders= `
        /p:RestoreAdditionalProjectFallbackFolders=
}

function Build-NativeProject {
    if ($script:SkipNativeBuild -eq 1) {
        Write-Host "SKIP_NATIVE_BUILD=1, skipping native build in Windows container."
        return
    }

    dotnet build src/InfiniFrame.Native/InfiniFrame.Native.proj `
        --configuration $script:Configuration `
        --no-restore `
        /p:SolutionDir="C:\work\" `
        /p:Platform=$script:NativePlatform
}

function Build-Solution {
    param([string]$FilterPath)
    dotnet build $FilterPath `
        --configuration $script:Configuration `
        --no-restore `
        /p:UseAppHost=false `
        /p:BuildInParallel=false `
        /p:DisableImplicitNuGetFallbackFolder=true `
        /p:RestoreFallbackFolders= `
        /p:RestoreAdditionalProjectFallbackFolders=
}
