#!/usr/bin/env bash
set -euo pipefail

NUGET_URL="https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"

OS_NAME="$(uname -s || true)"
IS_WINDOWS=0
case "${OS_NAME}" in
  MINGW*|MSYS*|CYGWIN*)
    IS_WINDOWS=1
    ;;
esac
IS_WSL=0
if [[ -n "${WSL_DISTRO_NAME:-}" ]] || [[ -n "${WSL_INTEROP:-}" ]]; then
  IS_WSL=1
fi

install_windows_like() {
  local temp_ps1
  temp_ps1="$(mktemp "${TMPDIR:-/tmp}/nuget-install-XXXXXX.ps1")"
  cat > "${temp_ps1}" <<'PS1'
$ErrorActionPreference = "Stop"
$nugetUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
$nugetDir = Join-Path $env:USERPROFILE ".nuget\cli"
$nugetPath = Join-Path $nugetDir "nuget.exe"

if (-not (Test-Path $nugetDir)) {
    New-Item -ItemType Directory -Path $nugetDir | Out-Null
}

Write-Host "Downloading nuget.exe..."
Invoke-WebRequest -Uri $nugetUrl -OutFile $nugetPath

$oldPath = [Environment]::GetEnvironmentVariable("PATH", [EnvironmentVariableTarget]::User)
$segments = @()
if (-not [string]::IsNullOrWhiteSpace($oldPath)) {
    $segments = $oldPath.Split(";")
}
if (-not ($segments -contains $nugetDir)) {
    $newPath = if ([string]::IsNullOrWhiteSpace($oldPath)) { $nugetDir } else { "$oldPath;$nugetDir" }
    [Environment]::SetEnvironmentVariable("PATH", $newPath, [EnvironmentVariableTarget]::User)
    Write-Host "Added $nugetDir to user PATH. You may need to restart your terminal."
} else {
    Write-Host "$nugetDir is already in PATH."
}
PS1
  powershell.exe -NoProfile -ExecutionPolicy Bypass -File "${temp_ps1}"
  rm -f "${temp_ps1}"
}

install_unix_like() {
  local nuget_home nuget_exe tools_dir wrapper
  nuget_home="${HOME}/.nuget/cli"
  nuget_exe="${nuget_home}/nuget.exe"
  tools_dir="${HOME}/.local/bin"
  wrapper="${tools_dir}/nuget"

  mkdir -p "${nuget_home}" "${tools_dir}"
  export PATH="${tools_dir}:${PATH}"

  if [[ ! -f "${nuget_exe}" ]]; then
    echo "Downloading nuget.exe..."
    if command -v curl >/dev/null 2>&1; then
      curl -fL --retry 3 --retry-all-errors -o "${nuget_exe}" "${NUGET_URL}"
    elif command -v wget >/dev/null 2>&1; then
      wget -O "${nuget_exe}" "${NUGET_URL}"
    else
      echo "curl or wget is required to download nuget.exe."
      exit 1
    fi
  fi

  if ! command -v mono >/dev/null 2>&1; then
    echo "mono runtime is required on Linux/macOS to run nuget.exe."
    exit 1
  fi

  cat > "${wrapper}" <<'EOF'
#!/usr/bin/env bash
exec mono "${HOME}/.nuget/cli/nuget.exe" "$@"
EOF
  chmod +x "${wrapper}"
  echo "nuget installed successfully: ${wrapper}"
}

if [[ "${IS_WINDOWS}" -eq 1 ]]; then
  install_windows_like
  echo "nuget install complete for Windows."
elif [[ "${IS_WSL}" -eq 1 ]] && command -v powershell.exe >/dev/null 2>&1; then
  install_windows_like
  echo "nuget install complete for Windows (via WSL)."
else
  install_unix_like
fi
