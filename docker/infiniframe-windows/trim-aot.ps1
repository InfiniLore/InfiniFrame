$ErrorActionPreference = "Stop"
. "C:\work\docker\infiniframe-windows\common.ps1"

Initialize-CommonDefaults

$dotnetProps = @(
    "/p:DisableImplicitNuGetFallbackFolder=true",
    "/p:RestoreFallbackFolders=",
    "/p:RestoreAdditionalProjectFallbackFolders=",
    "/p:GeneratePackageOnBuild=false",
    "/p:SkipTypeScriptBuild=true"
)

dotnet restore InfiniFrame.GitHubActions.Release.slnf `
    --force `
    --force-evaluate `
    --configfile $script:NuGetConfigFile `
    --packages $script:NuGetPackages `
    /p:NoWarn=NU1503 `
    $dotnetProps

dotnet build src/InfiniFrame.Shared/InfiniFrame.Shared.csproj `
    --configuration $script:Configuration `
    --framework net10.0 `
    -p:EnableTrimAnalyzer=true `
    -p:EnableAotAnalyzer=true `
    $dotnetProps

dotnet build src/InfiniFrame.BlazorWebView/InfiniFrame.BlazorWebView.csproj `
    --configuration $script:Configuration `
    --framework net10.0 `
    -p:EnableTrimAnalyzer=true `
    -p:EnableAotAnalyzer=true `
    $dotnetProps

dotnet build src/InfiniFrame.WebServer/InfiniFrame.WebServer.csproj `
    --configuration $script:Configuration `
    --framework net10.0 `
    -p:EnableTrimAnalyzer=true `
    -p:EnableAotAnalyzer=true `
    $dotnetProps

dotnet publish examples/InfiniFrameExample.TrimAotSmoke/InfiniFrameExample.TrimAotSmoke.csproj `
    --configuration $script:Configuration `
    --framework net10.0 `
    --runtime win-x64 `
    $dotnetProps

$publishDir = "examples/InfiniFrameExample.TrimAotSmoke/bin/${script:Configuration}/net10.0/win-x64/publish"
$output = Join-Path $publishDir "InfiniFrameExample.TrimAotSmoke.exe"

if (Test-Path $output) {
    Write-Host "NativeAOT smoke output validated: $output"
    exit 0
}

$availableExeNames = Get-ChildItem -Path $publishDir -Filter *.exe -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Name
Write-Host "Expected NativeAOT output not found: $output"
Write-Host "Available *.exe in publish dir: $($availableExeNames -join ', ')"
exit 1
