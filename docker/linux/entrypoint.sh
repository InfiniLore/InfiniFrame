#!/usr/bin/env bash
set -euo pipefail

NATIVE_ARCH="${NATIVE_ARCH:-x64}"
RUN_MODE="${RUN_MODE:-tests}"
ENABLE_TEST_EXPORTS="true"
INFINIFRAME_ENABLE_NATIVE_DIAGNOSTICS="${INFINIFRAME_ENABLE_NATIVE_DIAGNOSTICS:-1}"
INFINIFRAME_NATIVE_CRASH_DIR="${INFINIFRAME_NATIVE_CRASH_DIR:-/src/artifacts/native-crash}"
INFINIFRAME_NATIVE_GDB_FALLBACK="${INFINIFRAME_NATIVE_GDB_FALLBACK:-1}"
INFINIFRAME_GDB_TIMEOUT_SEC="${INFINIFRAME_GDB_TIMEOUT_SEC:-900}"
INFINIFRAME_ENABLE_STRACE="${INFINIFRAME_ENABLE_STRACE:-0}"
INFINIFRAME_TEST_FRAMEWORKS="${INFINIFRAME_TEST_FRAMEWORKS:-net8.0 net9.0 net10.0}"
INFINIFRAME_TEST_FILTER="${INFINIFRAME_TEST_FILTER:-}"
INFINIFRAME_ENABLE_TEST_BLAME_CRASH="${INFINIFRAME_ENABLE_TEST_BLAME_CRASH:-0}"
INFINIFRAME_TEST_TARGET="${INFINIFRAME_TEST_TARGET:-InfiniFrame.GitHubActions.Testing.slnf}"
INFINIFRAME_ENABLE_TEST_DIAG="${INFINIFRAME_ENABLE_TEST_DIAG:-0}"
INFINIFRAME_TEST_TREENODE_FILTER="${INFINIFRAME_TEST_TREENODE_FILTER:-}"
INFINIFRAME_ENABLE_TESTAPP_DIAGNOSTIC="${INFINIFRAME_ENABLE_TESTAPP_DIAGNOSTIC:-1}"

setup_display() {
  if [ "${USE_WSLG:-0}" = "1" ]; then
    export DISPLAY="${DISPLAY:-:0}"
    export WAYLAND_DISPLAY="${WAYLAND_DISPLAY:-wayland-0}"
    export XDG_RUNTIME_DIR="${XDG_RUNTIME_DIR:-/mnt/wslg/runtime-dir}"
    export PULSE_SERVER="${PULSE_SERVER:-unix:/mnt/wslg/PulseServer}"
    export XDG_SESSION_TYPE=wayland
    echo "Using WSLg display: DISPLAY=$DISPLAY WAYLAND_DISPLAY=$WAYLAND_DISPLAY"
    timeout 30 bash -c 'until xdpyinfo >/dev/null 2>&1; do sleep 1; done' || {
      echo "Could not connect to WSLg display (DISPLAY=$DISPLAY)." >&2
      echo "Run this from inside WSL with WSLg enabled." >&2
      exit 1
    }
    return
  fi

  export XDG_RUNTIME_DIR="/tmp/runtime-${USER:-root}"
  export XDG_SESSION_TYPE=x11
  export DESKTOP_SESSION=openbox
  export XDG_CURRENT_DESKTOP=Openbox
  mkdir -p "$XDG_RUNTIME_DIR"
  chmod 700 "$XDG_RUNTIME_DIR"

  if [ "${USE_HOST_X11:-1}" = "1" ]; then
    export DISPLAY="${DISPLAY:-host.docker.internal:0.0}"
    echo "Using host X11 display: $DISPLAY"
    local connected=0
    for _ in $(seq 1 30); do
      if timeout 2 xdpyinfo >/dev/null 2>&1; then
        connected=1
        break
      fi
      sleep 1
    done
    if [ "$connected" != "1" ]; then
      echo "Could not connect to X11 server at DISPLAY=$DISPLAY" >&2
      echo "Start your Windows X server and allow external TCP clients (port 6000)." >&2
      exit 1
    fi
    return
  fi

  Xvfb :99 -screen 0 1920x1080x24 -ac +extension GLX +extension RANDR +extension RENDER -nolisten tcp -noreset > xvfb.log 2>&1 &
  export DISPLAY=:99
  timeout 30 bash -c 'until xdpyinfo >/dev/null 2>&1; do sleep 1; done'
  openbox > openbox.log 2>&1 &
  timeout 20 bash -c 'until pgrep -x openbox >/dev/null; do sleep 1; done'
}

prepare_native() {
  # Prevent host-path CMake cache reuse when images are built from Windows worktrees.
  rm -rf /src/src/InfiniFrame.NativeBridge/build
  pwsh ./src/InfiniFrame.NativeBridge/native-build.ps1 Release "${NATIVE_ARCH}" true
}

ensure_native_debug_tools() {
  if [ "${INFINIFRAME_ENABLE_NATIVE_DIAGNOSTICS}" != "1" ]; then
    return
  fi

  if command -v gdb >/dev/null 2>&1; then
    return
  fi

  echo "[stage] installing native debug tools (gdb, libc6-dbg)"
  apt-get update
  apt-get install -y --no-install-recommends gdb libc6-dbg strace
}

configure_core_dumping() {
  if [ "${INFINIFRAME_ENABLE_NATIVE_DIAGNOSTICS}" != "1" ]; then
    return
  fi

  mkdir -p "${INFINIFRAME_NATIVE_CRASH_DIR}"
  ulimit -c unlimited || true

  if sysctl -w kernel.core_uses_pid=1 >/dev/null 2>&1; then
    if sysctl -w "kernel.core_pattern=${INFINIFRAME_NATIVE_CRASH_DIR}/core.%e.%p.%t" >/dev/null 2>&1; then
      echo "[stage] core dumps configured: ${INFINIFRAME_NATIVE_CRASH_DIR}/core.%e.%p.%t"
      return
    fi
  fi

  echo "[warn] could not set kernel.core_pattern inside container; using host/container default"
}

collect_native_crash_diagnostics() {
  if [ "${INFINIFRAME_ENABLE_NATIVE_DIAGNOSTICS}" != "1" ]; then
    return
  fi

  mkdir -p "${INFINIFRAME_NATIVE_CRASH_DIR}"

  shopt -s nullglob
  local found_core=0
  local core_candidates=(
    "${INFINIFRAME_NATIVE_CRASH_DIR}"/core.*
    /src/core*
    /tmp/core*
    ./core*
  )

  for core_file in "${core_candidates[@]}"; do
    [ -f "${core_file}" ] || continue
    case "${core_file}" in
      *.gdb.txt|*.gdb.txt.*)
        continue
        ;;
    esac
    found_core=1

    local core_name
    core_name="$(basename "${core_file}")"
    local staged_core="${INFINIFRAME_NATIVE_CRASH_DIR}/${core_name}"
    if [ "$(realpath "${core_file}")" != "$(realpath "${staged_core}" 2>/dev/null || echo "${staged_core}")" ]; then
      cp -f "${core_file}" "${staged_core}" || true
    fi

    local dump_file="${INFINIFRAME_NATIVE_CRASH_DIR}/${core_name}.gdb.txt"
    local core_meta
    if command -v file >/dev/null 2>&1; then
      core_meta="$(file "${staged_core}" || true)"
    else
      core_meta="(file utility unavailable) ${staged_core}"
    fi
    local exe_path
    exe_path="$(echo "${core_meta}" | sed -n "s/.*execfn: '\\([^']*\\)'.*/\\1/p")"
    if [ -z "${exe_path}" ] || [ ! -x "${exe_path}" ]; then
      exe_path="/usr/bin/dotnet"
    fi

    {
      echo "===== CORE FILE ====="
      echo "${staged_core}"
      echo
      echo "${core_meta}"
      echo
      echo "===== EXECUTABLE ====="
      echo "${exe_path}"
      echo
    } > "${dump_file}"

    gdb -q --batch \
      -ex "set pagination off" \
      -ex "set confirm off" \
      -ex "echo ===== INFO FILES =====\n" \
      -ex "info files" \
      -ex "echo \n===== THREAD BACKTRACES =====\n" \
      -ex "thread apply all bt full" \
      -ex "echo \n===== REGISTERS =====\n" \
      -ex "info registers" \
      -ex "echo \n===== SHARED LIBS =====\n" \
      -ex "info sharedlibrary" \
      "${exe_path}" "${staged_core}" >> "${dump_file}" 2>&1 || true
  done

  if [ "${found_core}" = "0" ]; then
    echo "[warn] no core files found; crash diagnostics may be limited by host core_pattern policy"
  fi
}

run_command_with_live_gdb_capture() {
  local framework="$1"
  shift

  if [ "${INFINIFRAME_ENABLE_STRACE}" = "1" ]; then
    mkdir -p "${INFINIFRAME_NATIVE_CRASH_DIR}"
    local strace_prefix="${INFINIFRAME_NATIVE_CRASH_DIR}/strace-${framework}"
    echo "[stage] running tests under strace for ${framework} -> ${strace_prefix}.*"
    strace -ff -tt -s 256 -o "${strace_prefix}" -e trace=process,signal "$@"
    return $?
  fi

  if [ "${INFINIFRAME_ENABLE_NATIVE_DIAGNOSTICS}" != "1" ] || [ "${INFINIFRAME_NATIVE_GDB_FALLBACK}" != "1" ]; then
    "$@"
    return $?
  fi

  mkdir -p "${INFINIFRAME_NATIVE_CRASH_DIR}"
  local gdb_log="${INFINIFRAME_NATIVE_CRASH_DIR}/gdb-live-dotnet-test-${framework}.txt"

  echo "[stage] running tests under gdb for ${framework} (timeout=${INFINIFRAME_GDB_TIMEOUT_SEC}s) -> ${gdb_log}"
  local gdb_exit=0
  set +e
  timeout --signal=SIGINT "${INFINIFRAME_GDB_TIMEOUT_SEC}" \
    gdb --return-child-result -q --batch \
      -ex "set pagination off" \
      -ex "set confirm off" \
      -ex "handle all nostop noprint pass" \
      -ex "handle SIGSEGV stop print nopass" \
      -ex "handle SIGFPE stop print nopass" \
      -ex "handle SIGILL stop print nopass" \
      -ex "handle SIGBUS stop print nopass" \
      -ex "run" \
      -ex "echo \n===== INFERIORS =====\n" \
      -ex "info inferiors" \
      -ex "echo \n===== THREAD BACKTRACES =====\n" \
      -ex "thread apply all bt full" \
      -ex "echo \n===== REGISTERS =====\n" \
      -ex "info registers" \
      -ex "echo \n===== SHARED LIBS =====\n" \
      -ex "info sharedlibrary" \
      --args "$@" 2>&1 | tee "${gdb_log}"
  gdb_exit=${PIPESTATUS[0]}
  set -e

  if [ "${gdb_exit}" = "124" ]; then
    echo "[warn] gdb timed out for ${framework}; falling back to direct test run"
    "$@"
    return $?
  fi

  return "${gdb_exit}"
}

run_tests() {
  dotnet restore InfiniFrame.GitHubActions.Testing.slnf /p:NoWarn=NU1503 /p:NativeArch="${NATIVE_ARCH}"
  prepare_native

  dotnet build InfiniFrame.GitHubActions.Testing.slnf \
    --configuration Release \
    --no-restore \
    -m:1 \
    -p:SolutionDir=/src/ \
    -p:NativeArch="${NATIVE_ARCH}" \
    -p:InfiniFrameSkipNativeBuild=true \
    -p:InfiniFrameEnableTestExports="${ENABLE_TEST_EXPORTS}" \
    -p:UseAppHost=false

  local exit_code=0
  local framework
  for framework in ${INFINIFRAME_TEST_FRAMEWORKS}; do
    local test_cmd=(
      dotnet test
      "${INFINIFRAME_TEST_TARGET}"
      --configuration Release
      --no-build
      --no-restore
      --framework "${framework}"
      -p:NativeArch="${NATIVE_ARCH}"
      -p:InfiniFrameSkipNativeBuild=true
      -p:InfiniFrameEnableTestExports="${ENABLE_TEST_EXPORTS}"
      -p:UseAppHost=false
    )
    if [ -n "${INFINIFRAME_TEST_FILTER}" ]; then
      test_cmd+=(--filter "${INFINIFRAME_TEST_FILTER}")
    fi
    if [ "${INFINIFRAME_ENABLE_TEST_BLAME_CRASH}" = "1" ]; then
      test_cmd+=(--blame-crash --blame-crash-dump-type full)
    fi
    if [ -n "${INFINIFRAME_TEST_TREENODE_FILTER}" ] || [ "${INFINIFRAME_ENABLE_TESTAPP_DIAGNOSTIC}" = "1" ]; then
      test_cmd+=(--)
      if [ -n "${INFINIFRAME_TEST_TREENODE_FILTER}" ]; then
        test_cmd+=(--treenode-filter "${INFINIFRAME_TEST_TREENODE_FILTER}")
      fi
      if [ "${INFINIFRAME_ENABLE_TESTAPP_DIAGNOSTIC}" = "1" ]; then
        mkdir -p "${INFINIFRAME_NATIVE_CRASH_DIR}"
        test_cmd+=(
          --diagnostic
          --diagnostic-output-directory "${INFINIFRAME_NATIVE_CRASH_DIR}"
          --diagnostic-file-prefix "tunit-${framework}"
        )
      fi
    fi

    local test_exit=0
    run_command_with_live_gdb_capture "${framework}" "${test_cmd[@]}" || test_exit=$?
    if [ "${test_exit}" != "0" ]; then
      exit_code="${test_exit}"
    fi
  done

  collect_native_crash_diagnostics
  if [ "${INFINIFRAME_ENABLE_NATIVE_DIAGNOSTICS}" = "1" ]; then
    echo "[stage] native crash artifacts: ${INFINIFRAME_NATIVE_CRASH_DIR}"
  fi
  exit "${exit_code}"
}

run_example_blazorwebview() {
  dotnet restore examples/InfiniFrameExample.BlazorWebView/InfiniFrameExample.BlazorWebView.csproj /p:NativeArch="${NATIVE_ARCH}"
  prepare_native
  dotnet run \
    --project examples/InfiniFrameExample.BlazorWebView/InfiniFrameExample.BlazorWebView.csproj \
    -c Release \
    -p:NativeArch="${NATIVE_ARCH}" \
    -p:InfiniFrameSkipNativeBuild=true \
    -p:InfiniFrameEnableTestExports="${ENABLE_TEST_EXPORTS}"
}

glib-compile-schemas /usr/share/glib-2.0/schemas/
setup_display
eval "$(dbus-launch --sh-syntax)"
echo "[stage] display ready"
export LIBGL_ALWAYS_SOFTWARE=1
export GALLIUM_DRIVER=llvmpipe
export MESA_GL_VERSION_OVERRIDE=3.3
export WEBKIT_DISABLE_COMPOSITING_MODE=1
ulimit -c unlimited
ensure_native_debug_tools
configure_core_dumping

case "${RUN_MODE}" in
  tests)
    echo "[stage] running linux test workflow"
    run_tests
    ;;
  example-blazorwebview)
    echo "[stage] launching example-blazorwebview"
    run_example_blazorwebview
    ;;
  *)
    echo "Unknown RUN_MODE: ${RUN_MODE}" >&2
    exit 2
    ;;
esac
