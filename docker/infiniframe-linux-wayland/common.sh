#!/usr/bin/env bash
set -euo pipefail

source "/work/docker/infiniframe-linux/common.sh"

start_wayland_compositor() {
  local weston_log="${1:-/tmp/weston.log}"
  export XDG_RUNTIME_DIR="${XDG_RUNTIME_DIR:-/tmp/runtime-$(id -un)}"
  export WAYLAND_DISPLAY="${WAYLAND_DISPLAY:-wayland-0}"
  export XDG_SESSION_TYPE=wayland
  export XDG_CURRENT_DESKTOP=weston
  export DESKTOP_SESSION=weston
  export GDK_BACKEND=wayland
  export QT_QPA_PLATFORM=wayland
  export MOZ_ENABLE_WAYLAND=1
  export WEBKIT_DISABLE_COMPOSITING_MODE=1

  mkdir -p "${XDG_RUNTIME_DIR}"
  chmod 700 "${XDG_RUNTIME_DIR}"

  weston \
    --backend=headless-backend.so \
    --socket="${WAYLAND_DISPLAY}" \
    --idle-time=0 \
    --xwayland \
    > "${weston_log}" 2>&1 &
  WESTON_PID=$!
  MUTTER_PID="${WESTON_PID}"

  timeout 30 bash -c "until [ -S \"${XDG_RUNTIME_DIR}/${WAYLAND_DISPLAY}\" ]; do sleep 1; done" || {
    echo "Weston failed to start"
    cat "${weston_log}" || true
    exit 1
  }
}

setup_display_mode() {
  local weston_log="${1:-/tmp/weston.log}"
  export NO_AT_BRIDGE="${NO_AT_BRIDGE:-1}"
  start_dbus_session

  export LIBGL_ALWAYS_SOFTWARE=1
  export GALLIUM_DRIVER=llvmpipe
  export MESA_GL_VERSION_OVERRIDE=3.3
  export NO_AT_BRIDGE=1

  if [[ "${USE_HOST_DISPLAY}" == "1" ]]; then
    echo "Using host Wayland mode"
    : "${WAYLAND_DISPLAY:?WAYLAND_DISPLAY must be set when USE_HOST_DISPLAY=1}"
    : "${XDG_RUNTIME_DIR:?XDG_RUNTIME_DIR must be set when USE_HOST_DISPLAY=1}"
  else
    echo "Using internal virtual Wayland mode (Weston headless)"
    start_wayland_compositor "${weston_log}"
  fi
}
