#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COMPOSE_FILE="${SCRIPT_DIR}/../compose/infiniframe-linux-wayland.yml"

WAYLAND_DISPLAY_VALUE="${WAYLAND_DISPLAY:-wayland-0}"
XDG_RUNTIME_DIR_VALUE="${XDG_RUNTIME_DIR:-/run/user/$(id -u)}"
DISPLAY_VALUE="${DISPLAY:-}"
GDK_BACKEND_VALUE="${GDK_BACKEND:-wayland}"
QT_QPA_PLATFORM_VALUE="${QT_QPA_PLATFORM:-wayland}"
XDG_SESSION_TYPE_VALUE="${XDG_SESSION_TYPE:-wayland}"

docker compose -f "${COMPOSE_FILE}" run --rm \
  -e USE_HOST_DISPLAY=1 \
  -e WAYLAND_DISPLAY="${WAYLAND_DISPLAY_VALUE}" \
  -e XDG_RUNTIME_DIR="${XDG_RUNTIME_DIR_VALUE}" \
  -e DISPLAY="${DISPLAY_VALUE}" \
  -e GDK_BACKEND="${GDK_BACKEND_VALUE}" \
  -e QT_QPA_PLATFORM="${QT_QPA_PLATFORM_VALUE}" \
  -e XDG_SESSION_TYPE="${XDG_SESSION_TYPE_VALUE}" \
  -v "${XDG_RUNTIME_DIR_VALUE}:${XDG_RUNTIME_DIR_VALUE}" \
  linux-wayland-example-blazorwebview
