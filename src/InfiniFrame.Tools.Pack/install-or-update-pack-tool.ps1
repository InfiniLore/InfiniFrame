Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptDir "../..")

$projectPath = Join-Path $scriptDir "InfiniFrame.Tools.Pack.csproj"
$packageId = "InfiniLore.InfiniFrame.Tools.Pack"
$toolCommand = "infiniframe-pack"
$packageOutputDir = Join-Path $repoRoot "artifacts/dotnet-tools"

Write-Host "[InfiniFrame.Tools.Pack] Packing tool package..."
dotnet pack $projectPath -c Release -o $packageOutputDir

$latestPackage = Get-ChildItem -Path $packageOutputDir -Filter "$packageId.*.nupkg" -File |
    Where-Object { $_.Name -notlike "*.symbols.nupkg" } |
    Sort-Object LastWriteTimeUtc -Descending |
    Select-Object -First 1

if ($null -eq $latestPackage)
{
    Write-Error "[InfiniFrame.Tools.Pack] ERROR: No package was produced in $packageOutputDir."
}

$packageVersion = $latestPackage.BaseName.Substring($packageId.Length + 1)

Write-Host "[InfiniFrame.Tools.Pack] Installing/updating global dotnet tool..."
try
{
    dotnet tool update --global $packageId --version $packageVersion --add-source $packageOutputDir --ignore-failed-sources
    Write-Host "[InfiniFrame.Tools.Pack] Updated $packageId ($packageVersion)."
}
catch
{
    dotnet tool install --global $packageId --version $packageVersion --add-source $packageOutputDir --ignore-failed-sources
    Write-Host "[InfiniFrame.Tools.Pack] Installed $packageId ($packageVersion)."
}

$globalToolsDir = Join-Path $env:USERPROFILE ".dotnet\tools"
$currentPathEntries = $env:PATH -split ';'
if ($currentPathEntries -notcontains $globalToolsDir)
{
    $env:PATH = "$env:PATH;$globalToolsDir"
    Write-Host "[InfiniFrame.Tools.Pack] Added $globalToolsDir to current session PATH."
}

$userPath = [Environment]::GetEnvironmentVariable("Path", "User")
$userPathEntries = @()
if (-not [string]::IsNullOrWhiteSpace($userPath))
{
    $userPathEntries = $userPath -split ';'
}

if ($userPathEntries -notcontains $globalToolsDir)
{
    $newUserPath = if ( [string]::IsNullOrWhiteSpace($userPath))
    {
        $globalToolsDir
    }
    else
    {
        "$userPath;$globalToolsDir"
    }

    [Environment]::SetEnvironmentVariable("Path", $newUserPath, "User")
    Write-Host "[InfiniFrame.Tools.Pack] Added $globalToolsDir to user PATH."
    Write-Host "[InfiniFrame.Tools.Pack] Restart your terminal/IDE so new processes pick up the PATH change."
}

Write-Host "[InfiniFrame.Tools.Pack] Done. Command: $toolCommand"
