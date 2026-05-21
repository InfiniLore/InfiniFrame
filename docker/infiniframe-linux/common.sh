#!/usr/bin/env bash
set -euo pipefail

init_common_defaults() {
  CONFIGURATION="${CONFIGURATION:-Release}"
  NATIVE_PLATFORM="${NATIVE_PLATFORM:-x64}"
  USE_HOST_DISPLAY="${USE_HOST_DISPLAY:-0}"
  CMAKE_BUILD_DIR="${CMAKE_BUILD_DIR:-/tmp/infiniframe-cmake/${NATIVE_PLATFORM}/${CONFIGURATION}}"
  NUGET_CONFIG_FILE="${NUGET_CONFIG_FILE:-/work/docker/infiniframe-linux/NuGet.Config}"
  NUGET_PACKAGES_DIR="${NUGET_PACKAGES:-/root/.nuget/packages}"

  COMMON_DOTNET_PROPS=(
    '/p:DisableImplicitNuGetFallbackFolder=true'
    '/p:RestoreFallbackFolders='
    '/p:RestoreAdditionalProjectFallbackFolders='
  )
}

sanitize_restore_artifacts() {
  echo "Sanitizing stale NuGet restore metadata (without removing obj/bin directories)..."
  find /work/src /work/tests /work/examples -type f \
    \( -name "*.nuget.g.props" -o -name "*.nuget.g.targets" -o -name "project.assets.json" -o -name "project.nuget.cache" -o -name "*.csproj.nuget.dgspec.json" \) \
    -delete
}

setup_cleanup_trap() {
  cleanup() {
    if [[ -n "${DBUS_SESSION_BUS_PID:-}" ]]; then
      kill "${DBUS_SESSION_BUS_PID}" >/dev/null 2>&1 || true
    fi
    if [[ -n "${MUTTER_PID:-}" ]]; then
      kill "${MUTTER_PID}" >/dev/null 2>&1 || true
    fi
    if [[ -n "${XVFB_PID:-}" ]]; then
      kill "${XVFB_PID}" >/dev/null 2>&1 || true
    fi
  }
  trap cleanup EXIT
}

start_dbus_session() {
  if [[ -n "${DBUS_SESSION_BUS_ADDRESS:-}" ]]; then
    return
  fi

  echo "Starting D-Bus session..."
  eval "$(dbus-launch --sh-syntax)"
}

start_virtual_display() {
  local xvfb_log="${1:-/tmp/xvfb.log}"
  local openbox_log="${2:-/tmp/openbox.log}"

  echo "Launching Xvfb..."
  Xvfb :99 \
    -screen 0 1920x1080x24 \
    -ac \
    +extension GLX \
    +extension RANDR \
    +extension RENDER \
    -nolisten tcp \
    -noreset > "${xvfb_log}" 2>&1 &

  XVFB_PID=$!

  export DISPLAY=:99
  export XDG_RUNTIME_DIR="/tmp/runtime-$(id -un)"
  export XDG_SESSION_TYPE=x11
  export XDG_SESSION_CLASS=user
  export XDG_CURRENT_DESKTOP=Openbox
  export DESKTOP_SESSION=openbox

  mkdir -p "${XDG_RUNTIME_DIR}"
  chmod 700 "${XDG_RUNTIME_DIR}"

  echo "Waiting for X server..."
  timeout 30 bash -c 'until xdpyinfo >/dev/null 2>&1; do sleep 1; done' || {
    echo "X server failed to start"
    exit 1
  }

  echo "Starting Openbox..."
  openbox > "${openbox_log}" 2>&1 &

  OPENBOX_PID=$!

  timeout 20 bash -c 'until pgrep -x openbox >/dev/null; do sleep 1; done' || {
    echo "Openbox failed to start"
    cat "${openbox_log}" || true
    exit 1
  }

  echo "Openbox is running"
}

setup_display_mode() {
  local xvfb_log="${1:-/tmp/xvfb.log}"
  local mutter_log="${2:-/tmp/mutter.log}"
  export NO_AT_BRIDGE="${NO_AT_BRIDGE:-1}"
  start_dbus_session

  export LIBGL_ALWAYS_SOFTWARE=1
  export GALLIUM_DRIVER=llvmpipe
  export MESA_GL_VERSION_OVERRIDE=3.3
  export NO_AT_BRIDGE=1

  if [[ "${USE_HOST_DISPLAY}" == "1" ]]; then
    echo "Using host DISPLAY mode"
    : "${DISPLAY:?DISPLAY must be set when USE_HOST_DISPLAY=1}"
    export WEBKIT_DISABLE_COMPOSITING_MODE="${WEBKIT_DISABLE_COMPOSITING_MODE:-0}"
  else
    echo "Using internal virtual display mode (Xvfb + Mutter)"
    export WEBKIT_DISABLE_COMPOSITING_MODE="${WEBKIT_DISABLE_COMPOSITING_MODE:-1}"
    start_virtual_display "${xvfb_log}" "${mutter_log}"
  fi

  echo "Display env: XDG_SESSION_TYPE=${XDG_SESSION_TYPE:-<unset>}, DISPLAY=${DISPLAY:-<unset>}, WEBKIT_DISABLE_COMPOSITING_MODE=${WEBKIT_DISABLE_COMPOSITING_MODE}"
}

restore_solution_filter() {
  local solution_filter="$1"
  sanitize_restore_artifacts
  echo "Restoring solution filter ${solution_filter}..."
  dotnet restore "${solution_filter}" \
    --force \
    --force-evaluate \
    --configfile "${NUGET_CONFIG_FILE}" \
    --packages "${NUGET_PACKAGES_DIR}" \
    /p:NoWarn=NU1503 \
    "${COMMON_DOTNET_PROPS[@]}"
}

build_native_project() {
  echo "Building native project..."
  mkdir -p "${CMAKE_BUILD_DIR}"
  dotnet build src/InfiniFrame.NativeBridge/InfiniFrame.NativeBridge.csproj \
    --configuration "${CONFIGURATION}" \
    --no-restore \
    /p:SolutionDir="/work/" \
    /p:Platform="${NATIVE_PLATFORM}" \
    /p:CMakeBuildDir="${CMAKE_BUILD_DIR}" \
    "${COMMON_DOTNET_PROPS[@]}"
}

build_solution_filter() {
  local solution_filter="$1"
  local label="${2:-projects}"
  echo "Building ${label} ..."
  dotnet build "${solution_filter}" \
    --configuration "${CONFIGURATION}" \
    --no-restore \
    /p:UseAppHost=false \
    /p:BuildInParallel=false \
    "${COMMON_DOTNET_PROPS[@]}"
}
