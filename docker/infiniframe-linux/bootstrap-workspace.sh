#!/usr/bin/env bash
set -euo pipefail

SRC_DIR="${1:-/src}"
WORK_DIR="${2:-/work}"

# If src contains a scripts folder, assume it's repo root
if [ -d "${SRC_DIR}/scripts" ]; then
  REPO_ROOT="${SRC_DIR}"
else
  REPO_ROOT="$(cd "${SRC_DIR}/.." && pwd)"
fi

SCRIPTS_DIR="${REPO_ROOT}/scripts"

echo "[bootstrap] repo root: ${REPO_ROOT}"
echo "[bootstrap] src:       ${SRC_DIR}"
echo "[bootstrap] scripts:   ${SCRIPTS_DIR}"
echo "[bootstrap] work:      ${WORK_DIR}"

echo "[bootstrap] cleaning workspace volume..."
mkdir -p "${WORK_DIR}"
find "${WORK_DIR}" -mindepth 1 -delete

echo "[bootstrap] copying repository snapshot (src)..."
tar -C "${SRC_DIR}" \
  --checkpoint=2000 \
  --checkpoint-action=echo='[bootstrap] copied %u files' \
  --exclude=".git" \
  --exclude=".github" \
  --exclude=".idea" \
  --exclude=".run" \
  --exclude="artifacts" \
  --exclude=".tmp" \
  --exclude=".pytest_cache" \
  --exclude="docs/node_modules" \
  --exclude="docs/.docusaurus" \
  --exclude="docs/build" \
  --exclude="src/InfiniFrame.NativeBridge/Native/packages" \
  --exclude="src/InfiniFrame.NativeBridge/build" \
  --exclude="*/node_modules" \
  --exclude="*/bin" \
  --exclude="*/obj" \
  -cf - . | tar -C "${WORK_DIR}" -xf -

echo "[bootstrap] copying scripts directory..."
if [ -d "${SCRIPTS_DIR}" ]; then
  mkdir -p "${WORK_DIR}/scripts"
  tar -C "${SCRIPTS_DIR}" -cf - . | tar -C "${WORK_DIR}/scripts" -xf -
else
  echo "[bootstrap] warning: scripts directory not found at ${SCRIPTS_DIR}"
fi

echo "[bootstrap] verifying src exists in workspace..."
if [ ! -d "${WORK_DIR}/src" ]; then
  echo "[bootstrap] ERROR: src folder missing in workspace!"
  exit 1
fi

echo "[bootstrap] workspace ready"