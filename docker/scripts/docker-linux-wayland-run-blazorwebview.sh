#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COMPOSE_FILE="${SCRIPT_DIR}/../compose/infiniframe-linux-wayland.yml"

WAYLAND_DISPLAY_VALUE="${WAYLAND_DISPLAY:-wayland-0}"
XDG_RUNTIME_DIR_VALUE="${XDG_RUNTIME_DIR:-/run/user/$(id -u)}"
XDG_RUNTIME_DIR_VALUE="${XDG_RUNTIME_DIR_VALUE%/}"
USE_HOST_DISPLAY_VALUE="${USE_HOST_DISPLAY:-0}"
WAYLAND_SOCKET_PATH="${XDG_RUNTIME_DIR_VALUE}/${WAYLAND_DISPLAY_VALUE}"
DISPLAY_VALUE="${DISPLAY:-:0}"
USE_XRUNNER_VALUE="${USE_XRUNNER:-1}"
RUN_ARGS=(run --rm)
SERVICE_NAME="linux-wayland-example-blazorwebview"

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
  if [[ "${USE_XRUNNER_VALUE}" == "1" ]]; then
    echo "Rendering Weston to host X runner via DISPLAY=${DISPLAY_VALUE}."
    RUN_ARGS+=(
      -e WESTON_BACKEND=x11-backend.so
      -e DISPLAY="${DISPLAY_VALUE}"
      -v /tmp/.X11-unix:/tmp/.X11-unix
    )
  fi
fi

docker compose -f "${COMPOSE_FILE}" "${RUN_ARGS[@]}" "${SERVICE_NAME}"
