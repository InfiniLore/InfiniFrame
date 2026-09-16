# Docker Setup

Docker infrastructure for building and testing InfiniFrame on Linux.

## Directory Structure

```
docker/
  linux/
    Dockerfile          # Linux container image with all build/test dependencies
    docker-compose.yml  # Service definitions for tests and examples
    entrypoint.sh       # Container entrypoint (display setup, build, test dispatch)
    linux/              # Linux Dockerfiles, Compose files, and WSLg wrappers
```

## Quick Start

### Run Tests in Docker

```bash
cd docker/linux
docker compose run linux-tests
```

### Run Tests with WSLg (Windows)

```bash
cd docker/linux
docker compose run linux-tests-wslg
```

### Run BlazorWebView Example

```bash
cd docker/linux
docker compose run example-blazorwebview-wslg
```

## Services

### `linux-tests`

Runs the full test suite inside the container. Connects to the host X11 display (for GUI tests) or uses Xvfb.

**Environment variables:**
- `USE_HOST_X11` Set to `1` to use the host's X11 display
- `NATIVE_ARCH` Target architecture (default: `x64`)

### `linux-tests-wslg`

Same test suite but configured for WSLg (Windows Subsystem for Linux with GUI support). Mounts `/mnt/wslg` and `/tmp/.X11-unix`, uses Wayland display.

### `example-blazorwebview-wslg`

Launches the BlazorWebView example app inside Docker with WSLg display support for visual verification.

## Image Contents

The Docker image is based on `mcr.microsoft.com/dotnet/sdk:10.0` and includes:

- .NET SDKs 8, 9, and 10
- GTK3 and WebKit2GTK 4.1 (Linux native dependencies)
- X11/Xvfb and Openbox window manager
- Node.js 24
- CMake
- PowerShell
- Build essentials (gcc, g++, make)
- Python 3

## Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `RUN_MODE` | `run_tests` | `run_tests` or `run_example_blazorwebview` |
| `NATIVE_ARCH` | `x64` | Target architecture for native builds |
| `INFINIFRAME_TEST_FRAMEWORKS` | `net8.0;net9.0;net10.0` | Semicolon-separated target frameworks |
| `INFINIFRAME_TEST_FILTER` | (none) | Test filter expression |
| `INFINIFRAME_ENABLE_NATIVE_DIAGNOSTICS` | `false` | Enable native diagnostic output |
| `INFINIFRAME_ENABLE_STRACE` | `false` | Enable strace for system call tracing |
| `INFINIFRAME_TEST_TARGET` | (solution filter) | Solution filter for test projects |

## Prerequisites

- Docker Desktop or Docker Engine
- For WSLg: Windows 11 or WSL2 with GUI support
- For host X11: X11 display server on Linux

The WSL2 path requires Windows 11 with WSLg enabled. Run `bash ./docker/linux/run-linux-tests-wslg.sh` from the repository root in WSL. The wrapper exports `DISPLAY`, `WAYLAND_DISPLAY`, `XDG_RUNTIME_DIR`, and `PULSE_SERVER`, mounts the WSLg runtime, and uses the Linux native build inside the container. If the display check fails, confirm `/mnt/wslg` exists and that `echo "$DISPLAY"` is not empty.
