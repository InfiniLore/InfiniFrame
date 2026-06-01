#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COMPOSE_FILE="${SCRIPT_DIR}/docker-compose.yml"

NATIVE_ARCH="${NATIVE_ARCH:-x64}"
FORCE_BUILD=0
ENABLE_NATIVE_DIAGNOSTICS=1
if [[ "${1:-}" == "x64" || "${1:-}" == "arm64" ]]; then
  NATIVE_ARCH="$1"
fi
if [[ "${2:-}" == "--build" || "${1:-}" == "--build" ]]; then
  FORCE_BUILD=1
fi
if [[ "${1:-}" == "--no-native-diagnostics" || "${2:-}" == "--no-native-diagnostics" || "${3:-}" == "--no-native-diagnostics" ]]; then
  ENABLE_NATIVE_DIAGNOSTICS=0
fi

export NATIVE_ARCH
export USE_WSLG=1
export USE_HOST_X11=0
export DISPLAY="${DISPLAY:-:0}"
export WAYLAND_DISPLAY="${WAYLAND_DISPLAY:-wayland-0}"
export XDG_RUNTIME_DIR="${XDG_RUNTIME_DIR:-/mnt/wslg/runtime-dir}"
export PULSE_SERVER="${PULSE_SERVER:-unix:/mnt/wslg/PulseServer}"

echo "Running tests with WSLg: DISPLAY=${DISPLAY}, WAYLAND_DISPLAY=${WAYLAND_DISPLAY}"
REPO_ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"
mkdir -p "${REPO_ROOT}/artifacts/native-crash"

if [[ "${FORCE_BUILD}" == "1" ]] || ! docker image inspect infiniframe-linux:local >/dev/null 2>&1; then
  docker compose -f "${COMPOSE_FILE}" build linux-tests-wslg
else
  echo "Using cached image infiniframe-linux:local (pass --build to rebuild)"
fi
docker compose -f "${COMPOSE_FILE}" run --rm \
  -e INFINIFRAME_ENABLE_NATIVE_DIAGNOSTICS="${ENABLE_NATIVE_DIAGNOSTICS}" \
  -e INFINIFRAME_NATIVE_GDB_FALLBACK="${INFINIFRAME_NATIVE_GDB_FALLBACK:-1}" \
  -e INFINIFRAME_ENABLE_STRACE="${INFINIFRAME_ENABLE_STRACE:-0}" \
  -e INFINIFRAME_TEST_FRAMEWORKS="${INFINIFRAME_TEST_FRAMEWORKS:-net8.0 net9.0 net10.0}" \
  -e INFINIFRAME_TEST_TARGET="${INFINIFRAME_TEST_TARGET:-InfiniFrame.GitHubActions.Testing.slnf}" \
  -e INFINIFRAME_TEST_FILTER="${INFINIFRAME_TEST_FILTER:-}" \
  -e INFINIFRAME_TEST_TREENODE_FILTER="${INFINIFRAME_TEST_TREENODE_FILTER:-}" \
  -e INFINIFRAME_ENABLE_TEST_BLAME_CRASH="${INFINIFRAME_ENABLE_TEST_BLAME_CRASH:-0}" \
  -e INFINIFRAME_ENABLE_TESTAPP_DIAGNOSTIC="${INFINIFRAME_ENABLE_TESTAPP_DIAGNOSTIC:-1}" \
  -e INFINIFRAME_GDB_TIMEOUT_SEC="${INFINIFRAME_GDB_TIMEOUT_SEC:-900}" \
  -e INFINIFRAME_LINUX_NATIVE_SIGABRT_TRACE="${INFINIFRAME_LINUX_NATIVE_SIGABRT_TRACE:-0}" \
  -e INFINIFRAME_NATIVE_CRASH_DIR=/src/artifacts/native-crash \
  -v "${REPO_ROOT}/artifacts:/src/artifacts" \
  linux-tests-wslg
