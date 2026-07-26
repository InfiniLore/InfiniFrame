# Run a safe preview first:
# .\scripts\js-updatedependencies.ps1 -WhatIf

# Update within the version ranges already declared in each package.json:
# .\scripts\js-updatedependencies.ps1

# Also apply npm’s non-breaking audit fixes:
# .\scripts\js-updatedependencies.ps1 -AuditFix

# To update dependency ranges in package.json to latest releases too—including majors, which can require code changes:
#.\scripts\js-updatedependencies.ps1 -IncludeMajor -AuditFix

[CmdletBinding(SupportsShouldProcess, ConfirmImpact = 'High')]
param(
    # Also rewrite package.json version ranges to the latest releases. This can introduce breaking changes.
    [switch] $IncludeMajor,

    # Run npm audit fix after updating each project. Findings without an upstream fix remain reported by npm.
    [switch] $AuditFix
)

$ErrorActionPreference = 'Stop'
$RepositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))

if (-not (Get-Command npm -ErrorAction SilentlyContinue)) {
    throw 'npm was not found on PATH. Install Node.js (including npm) and try again.'
}

function Invoke-Npm {
    param(
        [Parameter(Mandatory)] [string] $ProjectDirectory,
        [Parameter(Mandatory)] [string[]] $Arguments
    )

    & npm --prefix $ProjectDirectory @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "npm $($Arguments -join ' ') failed for $ProjectDirectory (exit code $LASTEXITCODE)."
    }
}

# Git respects .gitignore, so nested node_modules package.json files are never treated as projects.
$PackageFiles = @(
    & git -C $RepositoryRoot ls-files --cached --others --exclude-standard -- '**/package.json'
    if ($LASTEXITCODE -ne 0) {
        throw "git ls-files failed (exit code $LASTEXITCODE)."
    }
) | ForEach-Object { Join-Path $RepositoryRoot $_ } | Sort-Object -Unique

if ($PackageFiles.Count -eq 0) {
    throw "No package.json files were found under $RepositoryRoot."
}

foreach ($PackageFile in $PackageFiles) {
    $ProjectDirectory = Split-Path -Parent $PackageFile
    # GetRelativePath is unavailable in Windows PowerShell's .NET Framework runtime.
    $RelativePath = $PackageFile.Substring($RepositoryRoot.Length).TrimStart([char[]]@('\', '/'))
    Write-Host "`n==> $RelativePath" -ForegroundColor Cyan

    if ($IncludeMajor) {
        if ($PSCmdlet.ShouldProcess($RelativePath, 'upgrade package.json version ranges with npm-check-updates')) {
            Push-Location $ProjectDirectory
            try {
                & npx --yes npm-check-updates@latest --upgrade
                if ($LASTEXITCODE -ne 0) {
                    throw "npm-check-updates failed for $RelativePath (exit code $LASTEXITCODE)."
                }
            }
            finally {
                Pop-Location
            }
        }
    }

    if ($PSCmdlet.ShouldProcess($RelativePath, 'update dependencies within declared version ranges')) {
        Invoke-Npm -ProjectDirectory $ProjectDirectory -Arguments @('update', '--include=dev')
    }

    if ($AuditFix -and $PSCmdlet.ShouldProcess($RelativePath, 'apply npm audit fixes')) {
        Invoke-Npm -ProjectDirectory $ProjectDirectory -Arguments @('audit', 'fix', '--include=dev')
    }
}

Write-Host "`nUpdated $($PackageFiles.Count) JavaScript project(s)." -ForegroundColor Green
