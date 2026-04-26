#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd -- "${SCRIPT_DIR}/../.." && pwd)"

PROJECT_PATH="${SCRIPT_DIR}/InfiniFrame.Tools.Pack.csproj"
PACKAGE_ID="InfiniLore.InfiniFrame.Tools.Pack"
TOOL_COMMAND="infiniframe-pack"
PACKAGE_OUTPUT_DIR="${REPO_ROOT}/artifacts/dotnet-tools"

log() {
  echo "[InfiniFrame.Tools.Pack] $*"
}

# Ensure dotnet exists early (fail fast with a useful message)
if ! command -v dotnet >/dev/null 2>&1; then
  log "ERROR: dotnet CLI not found in PATH."
  log "Make sure .NET SDK is installed and PATH is configured."
  exit 1
fi

log "Packing tool package..."
dotnet pack "${PROJECT_PATH}" -c Release -o "${PACKAGE_OUTPUT_DIR}"

# Find latest package safely
shopt -s nullglob
packages=("${PACKAGE_OUTPUT_DIR}/${PACKAGE_ID}".*.nupkg)
shopt -u nullglob

# Filter out symbol packages
filtered=()
for pkg in "${packages[@]}"; do
  [[ "$pkg" == *.symbols.nupkg ]] && continue
  filtered+=("$pkg")
done

if (( ${#filtered[@]} == 0 )); then
  log "ERROR: No package was produced in ${PACKAGE_OUTPUT_DIR}."
  exit 1
fi

# Sort by modification time (newest first)
IFS=$'\n' sorted=($(ls -t "${filtered[@]}"))
unset IFS

LATEST_PACKAGE="${sorted[0]}"

PACKAGE_VERSION="${LATEST_PACKAGE##*/}"
PACKAGE_VERSION="${PACKAGE_VERSION#${PACKAGE_ID}.}"
PACKAGE_VERSION="${PACKAGE_VERSION%.nupkg}"

log "Resolved version: ${PACKAGE_VERSION}"

log "Installing/updating global dotnet tool..."

if dotnet tool update \
    --global "${PACKAGE_ID}" \
    --version "${PACKAGE_VERSION}" \
    --add-source "${PACKAGE_OUTPUT_DIR}" \
    --ignore-failed-sources; then
  log "Updated ${PACKAGE_ID} (${PACKAGE_VERSION})."
else
  dotnet tool install \
    --global "${PACKAGE_ID}" \
    --version "${PACKAGE_VERSION}" \
    --add-source "${PACKAGE_OUTPUT_DIR}" \
    --ignore-failed-sources
  log "Installed ${PACKAGE_ID} (${PACKAGE_VERSION})."
fi

log "Done. Command available: ${TOOL_COMMAND}"