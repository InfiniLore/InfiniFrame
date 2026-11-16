---
id: scripts
slug: /guides/scripts
title: Scripts Reference
---

This page documents the repository scripts and what each command does.

## Linux CI Image

The Linux test image is built and published by
`.github/workflows/ci-build-linux-images.yml`. Validate its X11 or Wayland
startup locally with:

```bash
bash .github/ci/linux-image/validate.sh x11
bash .github/ci/linux-image/validate.sh wayland
```

## NuGet Setup Script

### `nuget-install.sh`

Installs NuGet CLI in a cross-platform way:

- Windows/WSL: downloads `nuget.exe` and configures Windows user PATH.
- Linux/macOS: downloads `nuget.exe` and creates a `nuget` wrapper (`mono` required).

```bash
bash ./scripts/nuget-install.sh
```
