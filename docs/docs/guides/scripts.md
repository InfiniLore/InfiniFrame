---
id: scripts
slug: /guides/scripts
title: Scripts Reference
---

This page documents the repository scripts in `scripts/` and what each command does.

## Linux Docker Test Scripts

### `docker-compose-linux.sh`

Builds the Linux testing image from `docker/compose/linux-tests.yml` with a no-cache build.

```bash
bash ./scripts/docker-compose-linux.sh
```

### `docker-run-linux-tests.sh`

Runs the Linux test suite container (`linux-tests`) with internal virtual display mode.

```bash
bash ./scripts/docker-run-linux-tests.sh
```

### `docker-run-linux-tests-display.sh`

Runs the Linux test suite container (`linux-tests`) with host `DISPLAY` passthrough.

```bash
bash ./scripts/docker-run-linux-tests-display.sh
```

### `docker-run-linux-playwright.sh`

Runs the Linux Playwright test container (`linux-playwright-tests`) for `net8.0`, `net9.0`, and `net10.0`.

```bash
bash ./scripts/docker-run-linux-playwright.sh
```

### `docker-run-linux-playwright-display.sh`

Runs the Linux Playwright test container with host `DISPLAY` passthrough.

```bash
bash ./scripts/docker-run-linux-playwright-display.sh
```

## NuGet Setup Script

### `nuget-install.sh`

Installs NuGet CLI in a cross-platform way:

- Windows/WSL: downloads `nuget.exe` and configures Windows user PATH.
- Linux/macOS: downloads `nuget.exe` and creates a `nuget` wrapper (`mono` required).

```bash
bash ./scripts/nuget-install.sh
```
