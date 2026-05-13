---
id: scripts
slug: /guides/scripts
title: Scripts Reference
---

This page documents the repository scripts and what each command does.

## Docker Wrapper Scripts

Docker wrapper scripts now live in `docker/scripts/`.

### Linux (x64)

- Build images:

```bash
bash ./docker/scripts/docker-linux-compose.sh
```

- Run tests:

```bash
bash ./docker/scripts/docker-linux-run-tests.sh
```

- Run Playwright tests:

```bash
bash ./docker/scripts/docker-linux-run-playwrighttests.sh
```

- Run BlazorWebView example:

```bash
bash ./docker/scripts/docker-linux-run-blazorwebview.sh
```

### Linux (arm64)

- Build images:

```bash
bash ./docker/scripts/docker-linux-arm64-compose.sh
```

- Run tests:

```bash
bash ./docker/scripts/docker-linux-arm64-run-tests.sh
```

- Run Playwright tests:

```bash
bash ./docker/scripts/docker-linux-arm64-run-playwrighttests.sh
```

- Run BlazorWebView example:

```bash
bash ./docker/scripts/docker-linux-arm64-run-blazorwebview.sh
```

### Linux (Wayland)

- Build images:

```bash
bash ./docker/scripts/docker-linux-wayland-compose.sh
```

- Run tests:

```bash
bash ./docker/scripts/docker-linux-wayland-run-tests.sh
```

- Run Playwright tests:

```bash
bash ./docker/scripts/docker-linux-wayland-run-playwrighttests.sh
```

- Run BlazorWebView example:

```bash
bash ./docker/scripts/docker-linux-wayland-run-blazorwebview.sh
```

### Windows

- Build images:

```powershell
.\docker\scripts\docker-windows-compose.ps1
```

- Run tests:

```powershell
.\docker\scripts\docker-windows-run-tests.ps1
```

- Run Playwright tests:

```powershell
.\docker\scripts\docker-windows-run-playwrighttests.ps1
```

- Run BlazorWebView example:

```powershell
.\docker\scripts\docker-windows-run-blazorwebview.ps1
```

## NuGet Setup Script

### `nuget-install.sh`

Installs NuGet CLI in a cross-platform way:

- Windows/WSL: downloads `nuget.exe` and configures Windows user PATH.
- Linux/macOS: downloads `nuget.exe` and creates a `nuget` wrapper (`mono` required).

```bash
bash ./scripts/nuget-install.sh
```
