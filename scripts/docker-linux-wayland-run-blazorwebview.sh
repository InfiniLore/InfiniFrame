#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"
COMPOSE_FILE="${REPO_ROOT}/docker/compose/infiniframe-linux-wayland.yml"

WAYLAND_DISPLAY_VALUE="${WAYLAND_DISPLAY:-wayland-0}"
XDG_RUNTIME_DIR_VALUE="${XDG_RUNTIME_DIR:-/run/user/$(id -u)}"

docker compose -f "${COMPOSE_FILE}" run --rm \
  -e USE_HOST_DISPLAY=1 \
  -e WAYLAND_DISPLAY="${WAYLAND_DISPLAY_VALUE}" \
  -e XDG_RUNTIME_DIR="${XDG_RUNTIME_DIR_VALUE}" \
  -v "${XDG_RUNTIME_DIR_VALUE}:${XDG_RUNTIME_DIR_VALUE}" \
  linux-wayland-example-blazorwebview
