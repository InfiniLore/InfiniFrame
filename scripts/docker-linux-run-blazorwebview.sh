#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"
COMPOSE_FILE="${REPO_ROOT}/docker/compose/infiniframe-linux.yml"

DISPLAY_VALUE="${DISPLAY:-:0}"

docker compose -f "${COMPOSE_FILE}" run --rm \
  -e USE_HOST_DISPLAY=1 \
  -e DISPLAY="${DISPLAY_VALUE}" \
  -v /tmp/.X11-unix:/tmp/.X11-unix \
  linux-example-blazorwebview
