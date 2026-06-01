#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COMPOSE_FILE="${SCRIPT_DIR}/docker-compose.yml"

NATIVE_ARCH="${NATIVE_ARCH:-x64}"
FORCE_BUILD=0
if [[ "${1:-}" == "x64" || "${1:-}" == "arm64" ]]; then
  NATIVE_ARCH="$1"
fi
if [[ "${2:-}" == "--build" || "${1:-}" == "--build" ]]; then
  FORCE_BUILD=1
fi

export NATIVE_ARCH
export USE_WSLG=1
export USE_HOST_X11=0
export DISPLAY="${DISPLAY:-:0}"
export WAYLAND_DISPLAY="${WAYLAND_DISPLAY:-wayland-0}"
export XDG_RUNTIME_DIR="${XDG_RUNTIME_DIR:-/mnt/wslg/runtime-dir}"
export PULSE_SERVER="${PULSE_SERVER:-unix:/mnt/wslg/PulseServer}"

echo "Running with WSLg: DISPLAY=${DISPLAY}, WAYLAND_DISPLAY=${WAYLAND_DISPLAY}"

if [[ "${FORCE_BUILD}" == "1" ]] || ! docker image inspect infiniframe-linux:local >/dev/null 2>&1; then
  docker compose -f "${COMPOSE_FILE}" build example-blazorwebview-wslg
else
  echo "Using cached image infiniframe-linux:local (pass --build to rebuild)"
fi
docker compose -f "${COMPOSE_FILE}" run --rm example-blazorwebview-wslg
