#!/usr/bin/env bash
# ---------------------------------------------------------------------------------------------------------------------
# InfiniFrame Linux teardown/lifecycle stress runner (Phase 07).
#
# Usage: run-teardown-stress.sh <variant-name> <path-to-InfiniFrame.Native.so> [abi-include-root]
#
# Runs every lifecycle scenario in its own process with a watchdog. A scenario passes when:
#   - the process exits 0 (internal invariants: exactly-once close, no deadlock),
#   - the watchdog does not fire (no hang),
#   - the process is not killed by a signal (no native crash).
# Crash exit codes (e.g. 134=SIGABRT, 139=SIGSEGV) are reported explicitly — these are the
# codes the CI exit-code workarounds are about.
# ---------------------------------------------------------------------------------------------------------------------
set -uo pipefail

VARIANT="${1:?variant name required}"
SOLIB="${2:?path to InfiniFrame.Native.so required}"
ABI_INCLUDE="${3:-${ABI_INCLUDE:-}}"

if [ -z "$ABI_INCLUDE" ]; then
    echo "ERROR: ABI include root required (arg 3 or ABI_INCLUDE env)" >&2
    exit 2
fi

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
WORK="${BENCH_WORK_DIR:-/tmp/infiniframe-bench}"
mkdir -p "$WORK/out-$VARIANT-teardown"
NATIVE_LIB_DIR="$(cd "$(dirname "$SOLIB")" && pwd)"

echo "=== building teardown stress harness: variant=$VARIANT ==="
g++ -std=c++23 -O2 -Wall -Wextra \
    -I "$ABI_INCLUDE" \
    "$SCRIPT_DIR/teardown-stress.cpp" \
    "$NATIVE_LIB_DIR/InfiniFrame.Native.so" \
    -Wl,-rpath,"$NATIVE_LIB_DIR" \
    -o "$WORK/teardown-$VARIANT"

echo "=== stress environment (CI-deterministic) ==="
eval "$(dbus-launch --sh-syntax)"
Xvfb :90 -screen 0 1280x900x24 -ac -nolisten tcp >/tmp/teardown-xvfb.log 2>&1 &
export DISPLAY=:90
timeout 30 bash -c 'until xdpyinfo >/dev/null 2>&1; do sleep 1; done' || { echo "no X server"; exit 1; }
unset WAYLAND_DISPLAY XDG_SESSION_TYPE XDG_RUNTIME_DIR
export GDK_BACKEND=x11
export LIBGL_ALWAYS_SOFTWARE=1
export GALLIUM_DRIVER=llvmpipe
export MESA_LOADER_DRIVER_OVERRIDE=llvmpipe
export MESA_GL_VERSION_OVERRIDE=3.3
export WEBKIT_DISABLE_COMPOSITING_MODE=1
export WEBKIT_DISABLE_DMABUF_RENDERER=1
export INFINIFRAME_LINUX_FORCE_SOFTWARE_RENDERING=1

SCENARIOS=(
    close-during-load
    close-during-js
    close-during-scheme
    close-with-dialog
    repeated-start-stop
    multiple-windows
    callback-triggered-close
    webprocess-kill-close
    host-teardown
)

failures=0
for scenario in "${SCENARIOS[@]}"; do
    echo "--- scenario: $scenario ---"
    log="$WORK/out-$VARIANT-teardown/$scenario.log"
    timeout 180 "$WORK/teardown-$VARIANT" "$scenario" 25 > "$log" 2>&1
    code=$?
    if [ $code -eq 0 ]; then
        status="PASS"
    elif [ $code -eq 124 ]; then
        status="FAIL(watchdog-timeout)"
        failures=$((failures + 1))
    elif [ $code -gt 128 ]; then
        status="FAIL(signal $((code - 128)))"
        failures=$((failures + 1))
    else
        status="FAIL(exit $code)"
        failures=$((failures + 1))
    fi
    echo "scenario $scenario: $status (exit $code)"
    grep -E "SCENARIO-RESULT|close_latency_ms|ERROR" "$log" || true
done

echo
echo "=== TEARDOWN STRESS SUMMARY: $VARIANT: failures=$failures ==="
exit $failures
