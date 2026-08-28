# Scripts Reference

Build, CI/CD, and developer tooling scripts for the InfiniFrame repository.

## Build and Development

| Script | Description |
|--------|-------------|
| `clean.ps1` | Cleans all `bin/` and `obj/` directories plus native build artifacts. Use `-KillProcesses` to stop dotnet/MSBuild/node processes first. |
| `js-buildfrontend.mjs` | Node.js build orchestrator for JS frontends. Handles file locking, stale build detection, `npm ci`/`npm install`, and stamp files. |
| `js-updatedependencies.ps1` | Updates all `package.json` projects runs `npm update`, optionally `npm-check-updates` for major bumps, and `npm audit fix`. Supports `-WhatIf` for dry runs. |
| `clion-linux-environment.sh` | Provisions a full Linux dev environment for CLion: .NET SDKs 8/9/10, Node.js 24, CMake, GCC 13, Clang/libc++, GDB, and GTK/WebKit dependencies. |
| `nuget-install.sh` | Cross-platform NuGet CLI installer. Uses PowerShell on Windows/WSL, mono on Linux/macOS. |

## Versioning and Packaging

| Script | Description |
|--------|-------------|
| `bump_version.py` | Bumps the project version (major/minor/patch/preview/custom) across `src/Directory.Build.props`, `CMakeLists.txt`, and all `package.json` files. |
| `validate_package_id_prefix.py` | CI gate that ensures all `<PackageId>` values start with the required prefix (default: `InfiniLore`). |

## CI/CD

| Script | Description |
|--------|-------------|
| `coverage_report.py` | Aggregates TypeScript, C#, and Python coverage data, generates badge JSON, posts PR comments with coverage tables. |
| `sync_github_checks.py` | Posts commit statuses and completes GitHub check-runs via the GitHub API. Handles SSL fallback for CI environments. |
| `update_native_vendor_deps.py` | Updates vendored native (C/C++) dependencies from GitHub releases. Reads `native-vendor-deps.json` manifest. Supports `--check-only` for CI. |

## Debugging

| Script | Description |
|--------|-------------|
| `lsan-dotnet.supp` | AddressSanitizer/LeakSanitizer suppression file for .NET runtime leaks. Suppresses known leaks from `libcoreclr.so`, `libcrypto.so`, etc. |

## Usage

Most scripts can be run directly from the repository root:

```bash
# Python scripts
python scripts/bump_version.py --help

# PowerShell scripts
./scripts/clean.ps1 -KillProcesses

# Node.js scripts (called by MSBuild, not directly)
# See InfiniFrame.Js.csproj for the frontend build invocation
```
