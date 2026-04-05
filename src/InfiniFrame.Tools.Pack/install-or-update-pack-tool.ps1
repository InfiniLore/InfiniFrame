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

if ($null -eq $latestPackage) {
    Write-Error "[InfiniFrame.Tools.Pack] ERROR: No package was produced in $packageOutputDir."
}

$packageVersion = $latestPackage.BaseName.Substring($packageId.Length + 1)

Write-Host "[InfiniFrame.Tools.Pack] Installing/updating global dotnet tool..."
try {
    dotnet tool update --global $packageId --version $packageVersion --add-source $packageOutputDir --ignore-failed-sources
    Write-Host "[InfiniFrame.Tools.Pack] Updated $packageId ($packageVersion)."
}
catch {
    dotnet tool install --global $packageId --version $packageVersion --add-source $packageOutputDir --ignore-failed-sources
    Write-Host "[InfiniFrame.Tools.Pack] Installed $packageId ($packageVersion)."
}

Write-Host "[InfiniFrame.Tools.Pack] Done. Command: $toolCommand"
