#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"

PROJECT_PATH="${SCRIPT_DIR}/InfiniFrame.Tools.Pack.csproj"
PACKAGE_ID="InfiniLore.InfiniFrame.Tools.Pack"
TOOL_COMMAND="infiniframe-pack"
PACKAGE_OUTPUT_DIR="${REPO_ROOT}/artifacts/dotnet-tools"

echo "[InfiniFrame.Tools.Pack] Packing tool package..."
dotnet pack "${PROJECT_PATH}" -c Release -o "${PACKAGE_OUTPUT_DIR}"

LATEST_PACKAGE="$(ls -1t "${PACKAGE_OUTPUT_DIR}/${PACKAGE_ID}".*.nupkg | grep -v '\.symbols\.nupkg$' | head -n 1)"
if [[ -z "${LATEST_PACKAGE}" ]]; then
  echo "[InfiniFrame.Tools.Pack] ERROR: No package was produced in ${PACKAGE_OUTPUT_DIR}."
  exit 1
fi

PACKAGE_VERSION="$(basename "${LATEST_PACKAGE}")"
PACKAGE_VERSION="${PACKAGE_VERSION#${PACKAGE_ID}.}"
PACKAGE_VERSION="${PACKAGE_VERSION%.nupkg}"

echo "[InfiniFrame.Tools.Pack] Installing/updating global dotnet tool..."
if dotnet tool update --global "${PACKAGE_ID}" --version "${PACKAGE_VERSION}" --add-source "${PACKAGE_OUTPUT_DIR}" --ignore-failed-sources; then
  echo "[InfiniFrame.Tools.Pack] Updated ${PACKAGE_ID} (${PACKAGE_VERSION})."
else
  dotnet tool install --global "${PACKAGE_ID}" --version "${PACKAGE_VERSION}" --add-source "${PACKAGE_OUTPUT_DIR}" --ignore-failed-sources
  echo "[InfiniFrame.Tools.Pack] Installed ${PACKAGE_ID} (${PACKAGE_VERSION})."
fi

echo "[InfiniFrame.Tools.Pack] Done. Command: ${TOOL_COMMAND}"
