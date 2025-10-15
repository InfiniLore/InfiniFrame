# Define download URL and destination
$nugetUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
$nugetDir = "$env:USERPROFILE\.nuget\cli"
$nugetPath = Join-Path $nugetDir "nuget.exe"

# Create directory if it doesn't exist
if (-not (Test-Path $nugetDir)) {
    New-Item -ItemType Directory -Path $nugetDir | Out-Null
}

# Download nuget.exe
Write-Host "Downloading nuget.exe..."
Invoke-WebRequest -Uri $nugetUrl -OutFile $nugetPath

# Add to user PATH if not already present
$oldPath = [Environment]::GetEnvironmentVariable("PATH", [EnvironmentVariableTarget]::User)
if (-not $oldPath.Split(";") -contains $nugetDir) {
    [Environment]::SetEnvironmentVariable("PATH", "$oldPath;$nugetDir", [EnvironmentVariableTarget]::User)
    Write-Host "Added $nugetDir to user PATH. You may need to restart your terminal."
} else {
    Write-Host "$nugetDir is already in PATH."
}
