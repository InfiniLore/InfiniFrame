# InfiniFrame.Tools.Pack Guide

`InfiniFrame.Tools.Pack` is a .NET tool that packages an InfiniFrame application into a single-file executable while embedding:

- `wwwroot` content
- Native InfiniFrame runtime binaries for the selected runtime identifier (RID)

This guide covers how to install the tool, run it, and avoid common packaging issues.

## Contents

- [Overview](#overview)
- [How It Works](#how-it-works)
- [Install and Setup](#install-and-setup)
- [Install from NuGet](#install-from-nuget)
- [Command Syntax](#command-syntax)
- [Usage Examples](#usage-examples)
- [Common Patterns](#common-patterns)
- [App Bootstrap Requirement](#app-bootstrap-requirement)
- [Edge Cases and Pitfalls](#edge-cases-and-pitfalls)

## Overview

Use `InfiniFrame.Tools.Pack` when you want a single distributable output for an InfiniFrame app.

Compared to a regular `dotnet publish`, the tool additionally:

- Resolves native InfiniFrame runtime files from publish output
- Verifies required native artifacts exist before publish starts
- Injects custom MSBuild targets so `wwwroot` and native runtime files are embedded as resources
- Cleans unpacked runtime artifacts from the final publish directory

Because native files are embedded as resources, your app must initialize the runtime resolver at startup with `InfiniFrameSingleFileBootstrap.Initialize()`.

## How It Works

At a high level, `infiniframe-pack publish` runs this pipeline:

1. Parse CLI options and resolve defaults (`RID`, framework, output path).
2. Resolve native runtime artifacts from a preflight `dotnet publish` output (repo-agnostic, works for NuGet consumers).
3. Run `dotnet publish` in single-file mode with custom MSBuild targets.
4. Remove unpacked `wwwroot` and native runtime files from the publish folder.

## Install and Setup

### Prerequisites

- .NET 10 SDK (or a compatible SDK for your repo setup)
- A publishable app `.csproj`
- An app publish output that includes InfiniFrame native runtime files for the selected RID

### Build and install from source (local feed)

From the repository root:

```bash
dotnet pack src/InfiniFrame.Tools.Pack/InfiniFrame.Tools.Pack.csproj -c Release
dotnet tool install --local --add-source ./src/InfiniFrame.Tools.Pack/bin/Release InfiniFrame.Tools.Pack
```

Run with:

```bash
dotnet tool run infiniframe-pack --help
```

## Install from NuGet

If the package is published to NuGet, you can install it directly without building from source.

### Global install

```bash
dotnet tool install --global InfiniFrame.Tools.Pack
```

Run with:

```bash
infiniframe-pack --help
```

### Update or uninstall

```bash
dotnet tool update --global InfiniFrame.Tools.Pack
dotnet tool uninstall --global InfiniFrame.Tools.Pack
```

## Command Syntax

```bash
dotnet tool run infiniframe-pack publish <project.csproj> [options]
```

Options:

- `--rid <RID|auto>`: Target runtime identifier. Default is `auto`.
- `--configuration <Config>`: Build configuration. Default is `Release`.
- `--framework <TFM>`: Target framework. Default is `TargetFramework`, or first `TargetFrameworks` entry.
- `--self-contained <true|false>`: Self-contained publish mode. Default is `true`.
- `--output <path>`: Publish output directory. Default is `bin/<Config>/<TFM>/<RID>/publish`.
- `--no-restore`: Skip restore during publish.
- `--verbose`: Use normal verbosity for preflight and final publish.
- `--force-clean-output`: Allow recursive deletion of non-default output folders before publish.

## Usage Examples

### Basic publish with defaults

```bash
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj
```

### Publish for a specific runtime

```bash
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj --rid win-x64
```

### Multi-targeted app, choose framework explicitly

```bash
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj --framework net10.0
```

### Custom output and faster inner-loop publish

```bash
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj \
  --configuration Debug \
  --no-restore \
  --output artifacts/publish/MyApp-win-x64 \
  --verbose
```

## Common Patterns

### CI-friendly deterministic output paths

Pass an explicit `--output` directory so build artifacts land in a stable path:

```bash
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj --output artifacts/publish/MyApp
```

### Packaging multiple RIDs

Run the tool once per RID and separate outputs:

```bash
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj --rid win-x64 --output artifacts/publish/MyApp-win-x64
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj --rid linux-x64 --output artifacts/publish/MyApp-linux-x64
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj --rid osx-arm64 --output artifacts/publish/MyApp-osx-arm64
```

### Prefer explicit `--framework` for multi-targeting projects

If your project uses `TargetFrameworks`, pass `--framework` to avoid accidental changes when framework order is edited.

## App Bootstrap Requirement

After packaging with `InfiniFrame.Tools.Pack`, initialize the single-file bootstrap before creating a window:

```csharp
using InfiniFrame;

public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        InfiniFrameSingleFileBootstrap.Initialize();

        var window = InfiniFrameWindowBuilder.Create()
            .SetTitle("My App")
            .SetSize(1280, 720)
            .Center()
            .Build();

        window.WaitForClose();
    }
}
```

Why this is required:

- `infiniframe-pack publish` embeds `InfiniFrame.Native` and platform loader files (`WebView2Loader.dll` on Windows) as resources.
- `InfiniFrameSingleFileBootstrap.Initialize()` extracts them to a temporary RID-specific folder and registers a native resolver so P/Invoke can load them.

## Edge Cases and Pitfalls

- If preflight publish output does not contain required native files for the selected RID, the tool exits with a dedicated dependency-missing failure.
  The process exit code is `2`.
- `--rid auto` only supports current OS with `x64` or `arm64`.
  Other architectures throw a platform-not-supported error.
- Existing output folders are deleted before publish.
  By default, only project-local `bin/...` outputs are allowed to be cleaned.
  Use `--force-clean-output` to allow cleaning custom output folders.
- If your project defines `TargetFrameworks` and you omit `--framework`, the first framework entry is used.
- The tool performs a preflight `dotnet publish` before final single-file publish.
  If native artifacts are missing in preflight output, packaging stops early.
- `--self-contained` must be `true` or `false` (case-insensitive boolean parsing).
- If final output does not contain the expected main single-file executable, the tool exits with a non-zero code.
