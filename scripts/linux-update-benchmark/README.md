# Linux high-frequency update benchmark

Repeatable benchmark for the Linux native bridge's high-frequency update pipeline
(native → web message dispatch → web → native acknowledgement), used by the
GTK4/WebKitGTK 6.0 migration (migration) to compare rendering/messaging performance
before and after the port.

## What it measures

- **Round-trip latency** (`roundtrip_latency_ms`): native `SendWebMessage` → web handler →
  web `sendMessageToHost` ack → native callback. Reported as mean/min/p50/p95/p99/max over
  1000 sequential round trips (after a 100-trip warmup). Used as the frame/update-latency
  proxy for the migration comparison.
- **Update throughput** (`burst_throughput`): 1000 updates queued back-to-back, measured
  until every update is acknowledged (updates/second).
- **CPU** (`cpu_seconds`): `/proc/self/stat` utime+stime delta across both workloads.
- **Memory** (`memory_rss_kib`, `memory_hwm_kib`): `/proc/self/status` RSS and high-water
  mark around the workloads.

The harness drives the **public native ABI** (`InfiniFrameNative_ctor`, `InfiniFrameNative_Invoke`,
`InfiniFrameNative_SendWebMessage`, …) — the same exported surface the managed layer marshals
to — and runs unchanged against any `InfiniFrame.Native.so` built from the same ABI.

## Prerequisites

- Linux with GTK/WebKit dev dependencies installed (the CI image works).
- The native library built with test exports, e.g.:

```bash
cmake -B build/x64/Release -S src/InfiniFrame.NativeBridge/Native \
      -G Ninja -DINFINIFRAME_BUILD_TEST_EXPORTS=ON -DINFINIFRAME_SANITIZER=None
cmake --build build/x64/Release --config Release --parallel
```

Use the **identical** cmake invocation for every variant under comparison
(`native-build.ps1` parity: no `CMAKE_BUILD_TYPE`), so build configuration is a
controlled variable.

## Running

```bash
# pre-migration (e.g. a clean checkout of the last pre-migration commit)
ABI_INCLUDE=.../Native/src scripts/linux-update-benchmark/run-benchmark.sh \
    before-gtk3 .../build/x64/Release/x64/InfiniFrame.Native.so 5 "1a9b23c1 GTK3/WebKit2GTK-4.1"

# post-migration
ABI_INCLUDE=.../Native/src scripts/linux-update-benchmark/run-benchmark.sh \
    after-gtk4 .../build/x64/Release/x64/InfiniFrame.Native.so 5 "GTK4/WebKitGTK-6.0"
```

Each run starts a private Xvfb + dbus session and forces the CI-deterministic
software-rendering environment, so results are comparable across variants and machines.
`run-*.log` files are kept per variant for raw result archives.

## Caveats

- "Frame latency" is proxied by the update round-trip latency distribution under load;
  true compositor frame timing is not observable portably from inside the bridge.
- Results are only comparable when the same environment block is used (the runner enforces
  this) and both libraries were built with the same compiler/flags.
