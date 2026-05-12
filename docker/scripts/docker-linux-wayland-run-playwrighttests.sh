#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COMPOSE_FILE="${SCRIPT_DIR}/../compose/infiniframe-linux-wayland.yml"

WAYLAND_DISPLAY_VALUE="${WAYLAND_DISPLAY:-wayland-0}"
XDG_RUNTIME_DIR_VALUE="${XDG_RUNTIME_DIR:-/run/user/$(id -u)}"
XDG_RUNTIME_DIR_VALUE="${XDG_RUNTIME_DIR_VALUE%/}"
PLAYWRIGHT_VISIBLE_DEBUG_VALUE="${PLAYWRIGHT_VISIBLE_DEBUG:-0}"
PLAYWRIGHT_VISIBLE_DEBUG_SECONDS_VALUE="${PLAYWRIGHT_VISIBLE_DEBUG_SECONDS:-8}"
USE_HOST_DISPLAY_VALUE="${USE_HOST_DISPLAY:-0}"
WAYLAND_SOCKET_PATH="${XDG_RUNTIME_DIR_VALUE}/${WAYLAND_DISPLAY_VALUE}"
RUN_ARGS=(run --rm)
SERVICE_NAME="linux-wayland-tests-playwright"

if [[ "${USE_HOST_DISPLAY_VALUE}" == "1" ]]; then
  echo "Using host Wayland mode."
  if [[ ! -S "${WAYLAND_SOCKET_PATH}" ]]; then
    echo "Host Wayland socket not found: ${WAYLAND_SOCKET_PATH}"
    echo "Set USE_HOST_DISPLAY=0 to use internal Weston mode."
    exit 1
  fi
  RUN_ARGS+=(
    -e USE_HOST_DISPLAY=1
    -e WAYLAND_DISPLAY="${WAYLAND_DISPLAY_VALUE}"
    -e XDG_RUNTIME_DIR="${XDG_RUNTIME_DIR_VALUE}"
    -e GDK_BACKEND=wayland
    -e QT_QPA_PLATFORM=wayland
    -e XDG_SESSION_TYPE=wayland
    -v "${XDG_RUNTIME_DIR_VALUE}:${XDG_RUNTIME_DIR_VALUE}"
  )
else
  echo "Using internal Weston Wayland mode."
  RUN_ARGS+=(
    -e USE_HOST_DISPLAY=0
  )
fi

RUN_ARGS+=(
  -e PLAYWRIGHT_VISIBLE_DEBUG="${PLAYWRIGHT_VISIBLE_DEBUG_VALUE}"
  -e PLAYWRIGHT_VISIBLE_DEBUG_SECONDS="${PLAYWRIGHT_VISIBLE_DEBUG_SECONDS_VALUE}"
)

docker compose -f "${COMPOSE_FILE}" "${RUN_ARGS[@]}" "${SERVICE_NAME}"
