#!/usr/bin/env bash
set -euo pipefail

source "/work/docker/infiniframe-linux/common.sh"

start_wayland_compositor() {
  local weston_log="${1:-/tmp/weston.log}"
  local weston_backend="${WESTON_BACKEND:-headless-backend.so}"
  local enable_xwayland="${WESTON_ENABLE_XWAYLAND:-1}"
  export XDG_RUNTIME_DIR="${XDG_RUNTIME_DIR:-/tmp/runtime-$(id -un)}"
  export WAYLAND_DISPLAY="${WAYLAND_DISPLAY:-wayland-0}"
  export XDG_SESSION_TYPE=wayland
  export XDG_CURRENT_DESKTOP=weston
  export DESKTOP_SESSION=weston
  export GDK_BACKEND=wayland
  export QT_QPA_PLATFORM=wayland
  export MOZ_ENABLE_WAYLAND=1

  mkdir -p "${XDG_RUNTIME_DIR}"
  chmod 700 "${XDG_RUNTIME_DIR}"

  if [[ "${weston_backend}" == "x11-backend.so" ]]; then
    : "${DISPLAY:?DISPLAY must be set when WESTON_BACKEND=x11-backend.so}"
  fi

  local weston_args=(
    "--backend=${weston_backend}"
    "--socket=${WAYLAND_DISPLAY}"
    "--idle-time=0"
  )
  if [[ "${enable_xwayland}" == "1" ]]; then
    weston_args+=("--xwayland")
  fi

  weston "${weston_args[@]}" > "${weston_log}" 2>&1 &
  WESTON_PID=$!
  MUTTER_PID="${WESTON_PID}"

  timeout 30 bash -c "until [ -S \"${XDG_RUNTIME_DIR}/${WAYLAND_DISPLAY}\" ]; do sleep 1; done" || {
    echo "Weston failed to start"
    cat "${weston_log}" || true
    exit 1
  }

  if ! kill -0 "${WESTON_PID}" >/dev/null 2>&1; then
    echo "Weston exited unexpectedly"
    cat "${weston_log}" || true
    exit 1
  fi
}

setup_display_mode() {
  local weston_log="${1:-/tmp/weston.log}"
  local weston_backend="${WESTON_BACKEND:-headless-backend.so}"
  export NO_AT_BRIDGE="${NO_AT_BRIDGE:-1}"
  export XDG_RUNTIME_DIR="${XDG_RUNTIME_DIR:-/tmp/runtime-$(id -un)}"
  mkdir -p "${XDG_RUNTIME_DIR}"
  chmod 700 "${XDG_RUNTIME_DIR}"
  start_dbus_session

  export LIBGL_ALWAYS_SOFTWARE=1
  export GALLIUM_DRIVER=llvmpipe
  export MESA_GL_VERSION_OVERRIDE=3.3
  export NO_AT_BRIDGE=1
  export XDG_SESSION_TYPE=wayland
  export GDK_BACKEND=wayland
  export QT_QPA_PLATFORM=wayland
  export MOZ_ENABLE_WAYLAND=1
  if [[ "${USE_HOST_DISPLAY}" == "1" ]]; then
    export WEBKIT_DISABLE_COMPOSITING_MODE="${WEBKIT_DISABLE_COMPOSITING_MODE:-0}"
  elif [[ "${weston_backend}" == "x11-backend.so" ]]; then
    export WEBKIT_DISABLE_COMPOSITING_MODE="${WEBKIT_DISABLE_COMPOSITING_MODE:-0}"
  else
    export WEBKIT_DISABLE_COMPOSITING_MODE="${WEBKIT_DISABLE_COMPOSITING_MODE:-1}"
  fi
  if [[ "${USE_HOST_DISPLAY}" == "1" ]]; then
    unset DISPLAY || true
  fi

  if [[ "${USE_HOST_DISPLAY}" == "1" ]]; then
    echo "Using host Wayland mode"
    : "${WAYLAND_DISPLAY:?WAYLAND_DISPLAY must be set when USE_HOST_DISPLAY=1}"
    : "${XDG_RUNTIME_DIR:?XDG_RUNTIME_DIR must be set when USE_HOST_DISPLAY=1}"
    if [[ ! -S "${XDG_RUNTIME_DIR}/${WAYLAND_DISPLAY}" ]]; then
      echo "Wayland socket is not available in container: ${XDG_RUNTIME_DIR}/${WAYLAND_DISPLAY}"
      echo "Ensure host XDG_RUNTIME_DIR is mounted and WAYLAND_DISPLAY is correct."
      exit 1
    fi
  else
    echo "Using internal virtual Wayland mode (Weston)"
    echo "Weston backend: ${weston_backend}"
    if [[ "${weston_backend}" == "x11-backend.so" ]]; then
      : "${DISPLAY:?DISPLAY must be set when WESTON_BACKEND=x11-backend.so}"
    fi
    start_wayland_compositor "${weston_log}"
    # Keep DISPLAY when Weston uses x11-backend (nested/X runner mode), because
    # some subprocesses in the WebKit stack may still rely on X access even when
    # the main GTK client backend is forced to Wayland.
    if [[ "${weston_backend}" != "x11-backend.so" ]]; then
      unset DISPLAY || true
    fi
  fi

  echo "Display env: XDG_SESSION_TYPE=${XDG_SESSION_TYPE}, GDK_BACKEND=${GDK_BACKEND}, WAYLAND_DISPLAY=${WAYLAND_DISPLAY:-}, DISPLAY=${DISPLAY:-<unset>}, WEBKIT_DISABLE_COMPOSITING_MODE=${WEBKIT_DISABLE_COMPOSITING_MODE}"
}
