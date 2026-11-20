# Linux teardown / lifecycle stress

teardown work (GTK4/WebKitGTK 6.0 migration) tooling: exercises window teardown scenarios through
the public native ABI and checks the lifecycle invariants that the WebKitGTK teardown
workarounds exist for.

## Scenarios

| Scenario | What it exercises |
|---|---|
| `close-during-load` | `Close()` while custom-scheme resources are loading |
| `close-during-js` | `Close()` while the web process is running a long JS task |
| `close-during-scheme` | `Close()` while a custom-scheme request handler is in flight |
| `close-with-dialog` | `Close()` with an async message dialog open (completion must fire exactly once) |
| `repeated-start-stop` | N sequential create/close cycles; reports `close_latency_ms` distribution |
| `multiple-windows` | concurrent windows closed in mixed (middle-out) order |
| `callback-triggered-close` | `Close()` re-entered from a web-message callback (owner thread) |
| `webprocess-kill-close` | `Close()` while the web process is dying (page OOM crash) |
| `host-teardown` | abrupt `Shutdown()` with a live, never-disposed window |

## Invariants

- the closed notification fires **exactly once** per window;
- teardown completes within bounded waits (**no deadlock**);
- no native crash (**no signal death** — the runner flags signal exits explicitly, e.g.
  134 = SIGABRT, the code behind the CI exit-code workarounds).

## Running

```bash
# post-migration library
scripts/linux-teardown-stress/run-teardown-stress.sh \
    after-gtk4 build/x64/Release/x64/InfiniFrame.Native.so <Native>/src

# pre-migration library (for close-latency comparison)
scripts/linux-teardown-stress/run-teardown-stress.sh \
    before-gtk3 <before>/build/x64/Release/x64/InfiniFrame.Native.so <before>/Native/src
```

The runner starts a private Xvfb + dbus session with the CI-deterministic software-rendering
environment and runs each scenario in its own process under a 180 s watchdog. Per-scenario logs
are archived in `/tmp/infiniframe-bench/out-<variant>-teardown/`.
