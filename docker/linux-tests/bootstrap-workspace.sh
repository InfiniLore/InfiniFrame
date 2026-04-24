#!/usr/bin/env bash
set -euo pipefail

SRC_DIR="${1:-/src}"
WORK_DIR="${2:-/work}"

echo "[bootstrap] source: ${SRC_DIR}"
echo "[bootstrap] work:   ${WORK_DIR}"
echo "[bootstrap] cleaning workspace volume..."
find "${WORK_DIR}" -mindepth 1 -delete

echo "[bootstrap] copying repository snapshot..."
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
  --exclude="src/InfiniFrame.Native/packages" \
  --exclude="src/InfiniFrame.Native/build" \
  --exclude="src/InfiniFrame.Native/cmake-build-debug" \
  --exclude="src/InfiniFrame.Native/cmake-build-debug-linux" \
  --exclude="src/InfiniFrame.Native/cmake-build-debug-windows" \
  --exclude="*/node_modules" \
  --exclude="*/bin" \
  --exclude="*/obj" \
  -cf - . | tar -C "${WORK_DIR}" -xf -

echo "[bootstrap] workspace ready"
