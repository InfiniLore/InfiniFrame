#!/usr/bin/env bash
set -euo pipefail

init_common_defaults() {
  CONFIGURATION="${CONFIGURATION:-Release}"
  NATIVE_PLATFORM="${NATIVE_PLATFORM:-x64}"
  USE_HOST_DISPLAY="${USE_HOST_DISPLAY:-0}"
  CMAKE_BUILD_DIR="${CMAKE_BUILD_DIR:-/tmp/infiniframe-cmake/${NATIVE_PLATFORM}/${CONFIGURATION}}"
  NUGET_CONFIG_FILE="${NUGET_CONFIG_FILE:-/work/docker/infiniframe-linux/NuGet.Config}"
  NUGET_PACKAGES_DIR="${NUGET_PACKAGES:-/root/.nuget/packages}"
  DOTNET_MAX_CPU_COUNT="${DOTNET_MAX_CPU_COUNT:-1}"

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
  local mutter_log="${2:-/tmp/mutter.log}"

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
  export XDG_SESSION_DESKTOP=ubuntu
  export XDG_CURRENT_DESKTOP=ubuntu:GNOME
  export DESKTOP_SESSION=ubuntu

  mkdir -p "${XDG_RUNTIME_DIR}"
  chmod 700 "${XDG_RUNTIME_DIR}"

  echo "Waiting for X server..."
  timeout 30 bash -c 'until xdpyinfo >/dev/null 2>&1; do sleep 1; done' || {
    echo "X server failed to start"
    exit 1
  }

  echo "Starting Mutter..."
  mutter --x11 --replace --sm-disable > "${mutter_log}" 2>&1 &
  MUTTER_PID=$!

  timeout 20 bash -c 'until pgrep -x mutter >/dev/null; do sleep 1; done' || {
    echo "Mutter failed to start"
    exit 1
  }
}

setup_display_mode() {
  local xvfb_log="${1:-/tmp/xvfb.log}"
  local mutter_log="${2:-/tmp/mutter.log}"
  export NO_AT_BRIDGE="${NO_AT_BRIDGE:-1}"
  start_dbus_session

  if [[ "${USE_HOST_DISPLAY}" == "1" ]]; then
    echo "Using host DISPLAY mode"
    : "${DISPLAY:?DISPLAY must be set when USE_HOST_DISPLAY=1}"
  else
    echo "Using internal virtual display mode (Xvfb + Mutter)"
    start_virtual_display "${xvfb_log}" "${mutter_log}"
  fi
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
  dotnet build src/InfiniFrame.Native/InfiniFrame.Native.proj \
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
  echo "Building ${label} (max CPU count: ${DOTNET_MAX_CPU_COUNT})..."
  dotnet build "${solution_filter}" \
    -m:"${DOTNET_MAX_CPU_COUNT}" \
    --configuration "${CONFIGURATION}" \
    --no-restore \
    /p:UseAppHost=false \
    /p:BuildInParallel=false \
    "${COMMON_DOTNET_PROPS[@]}"
}
