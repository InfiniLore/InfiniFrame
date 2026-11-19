#!/usr/bin/env bash
# ---------------------------------------------------------------------------------------------------------------------
# InfiniFrame Linux high-frequency update benchmark runner (repeatable).
#
# Usage: run-benchmark.sh <variant-name> <path-to-InfiniFrame.Native.so> [runs] [label]
#   variant-name            output tag, e.g. "before-gtk3" / "after-gtk4"
#   path-to-InfiniFrame.Native.so   the native library to benchmark (built with test exports)
#   runs                    repetitions (default 5); median across runs is the headline figure
#   label                   optional free-form label printed in the summary
#
# The run environment mirrors the Linux CI test environment (Xvfb + dbus + deterministic
# software rendering) so before/after comparisons are like-for-like. Build configuration is
# identical across variants by construction: both libraries are produced by the same
# `cmake -G Ninja -DINFINIFRAME_BUILD_TEST_EXPORTS=ON -DINFINIFRAME_SANITIZER=None`
# invocation (no CMAKE_BUILD_TYPE), matching native-build.ps1.
# ---------------------------------------------------------------------------------------------------------------------
set -euo pipefail

VARIANT="${1:?variant name required}"
SOLIB="${2:?path to InfiniFrame.Native.so required}"
RUNS="${3:-5}"
LABEL="${4:-}"

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
WORK="${BENCH_WORK_DIR:-/tmp/infiniframe-bench}"
mkdir -p "$WORK/out-$VARIANT"

if [ ! -f "$SOLIB" ]; then
    echo "ERROR: native library not found: $SOLIB" >&2
    exit 1
fi

echo "=== building benchmark harness: variant=$VARIANT ==="
# ABI headers come from the native tree the library was built from (ABI is identical across
# variants; the harness compiling unchanged against both is itself an ABI parity check).
ABI_INCLUDE="${ABI_INCLUDE:?ABI_INCLUDE must point at the Native/src include root}"
NATIVE_LIB_DIR="$(cd "$(dirname "$SOLIB")" && pwd)"

g++ -std=c++23 -O2 -Wall -Wextra \
    -I "$ABI_INCLUDE" \
    "$SCRIPT_DIR/benchmark.cpp" \
    "$NATIVE_LIB_DIR/InfiniFrame.Native.so" \
    -Wl,-rpath,"$NATIVE_LIB_DIR" \
    -o "$WORK/bench-$VARIANT"
echo "harness: $WORK/bench-$VARIANT"

echo "=== benchmark environment (CI-deterministic) ==="
eval "$(dbus-launch --sh-syntax)"
Xvfb :92 -screen 0 1280x900x24 -ac -nolisten tcp >/tmp/bench-xvfb.log 2>&1 &
export DISPLAY=:92
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

echo "=== running $RUNS iteration(s) ==="
for run in $(seq 1 "$RUNS"); do
    echo "--- run $run/$RUNS ---"
    timeout 600 "$WORK/bench-$VARIANT" | tee "$WORK/out-$VARIANT/run-$run.log"
done

echo
echo "=== SUMMARY: $VARIANT ${LABEL:+($LABEL)} ==="
grep -h "roundtrip_latency_ms\|burst_throughput\|cpu_seconds\|memory_rss_kib" "$WORK/out-$VARIANT"/run-*.log
echo "per-run logs: $WORK/out-$VARIANT/"
