# Single-File Executable Packaging Guide

`InfiniLore.InfiniFrame.SingleFile` packages an InfiniFrame application into a single-file executable while embedding:

- `wwwroot` content
- Native InfiniFrame runtime binaries for the selected runtime identifier (RID)

This guide covers how to use the MSBuild target, configure options, and avoid common packaging issues.

## Contents

- [Overview](#overview)
- [How It Works](#how-it-works)
- [Install](#install)
- [Command Syntax](#command-syntax)
- [Usage Examples](#usage-examples)
- [Common Patterns](#common-patterns)
- [App Bootstrap Requirement](#app-bootstrap-requirement)
- [MSBuild Target Reference](#msbuild-target-reference)
- [Edge Cases and Pitfalls](#edge-cases-and-pitfalls)

## Overview

Use `InfiniLore.InfiniFrame.SingleFile` when you want a single distributable output for an InfiniFrame app.

Compared to a regular `dotnet publish`, the target additionally:

- Embeds `wwwroot` content as managed resources
- Embeds native InfiniFrame runtime files (`InfiniFrame.Native.dll`, `WebView2Loader.dll`, etc.) as managed resources
- Removes unpacked sidecar files from the final publish directory
- Performs a two-pass publish to ensure all content is available before embedding

Because native files are embedded as resources, your app must initialize the runtime resolver at startup with `InfiniFrameSingleFileBootstrap.Initialize()`.

## How It Works

The `InfiniFrameSingleFile` MSBuild target runs a two-pass publish pipeline:

1. **Pass 1**: Publish without single-file to generate all `wwwroot` content, static web assets, and framework files.
2. **Pass 2**: Publish with `PublishSingleFile=true`, embedding all generated content and native runtime files as embedded resources.
3. **Cleanup**: Remove unpacked `wwwroot`, sidecar files (`*.staticwebassets.endpoints.json`, `web.config`), and native runtime files from the publish directory.

## Install

### NuGet package

```bash
dotnet add package InfiniLore.InfiniFrame.SingleFile
```

The package ships MSBuild `.targets` that are automatically imported when the package is referenced.

### Build from source (for repo development)

From the repository root:

```bash
dotnet build src/InfiniFrame.SingleFile/InfiniFrame.SingleFile.csproj -c Release
```

No separate tool installation is required -- the targets are consumed directly via MSBuild.

## Command Syntax

```bash
dotnet publish <project.csproj> -t:InfiniFrameSingleFile -r <RID> -c <Configuration>
```

Or set the target to auto-run after a standard publish:

```bash
dotnet publish <project.csproj> -r <RID> -c Release -p:InfiniFrameSingleFileAuto=true
```

### Target properties

| Property | Default | Description |
|----------|---------|-------------|
| `-r <RID>` | (required) | Target runtime identifier (e.g. `win-x64`, `linux-arm64`, `osx-x64`) |
| `-c <Configuration>` | `Release` | Build configuration |
| `InfiniFrameSingleFileSelfContained` | `true` | Self-contained publish mode |
| `InfiniFrameSingleFileAuto` | `false` | Auto-run after `dotnet publish` |

## Usage Examples

### Basic publish

```bash
dotnet publish src/MyApp/MyApp.csproj -t:InfiniFrameSingleFile -r win-x64 -c Release
```

### Publish for Linux

```bash
dotnet publish src/MyApp/MyApp.csproj -t:InfiniFrameSingleFile -r linux-x64 -c Release
```

### Auto-run after publish

```bash
dotnet publish src/MyApp/MyApp.csproj -r win-x64 -c Release -p:InfiniFrameSingleFileAuto=true
```

### Non-self-contained publish

```bash
dotnet publish src/MyApp/MyApp.csproj -t:InfiniFrameSingleFile -r win-x64 -c Release -p:InfiniFrameSingleFileSelfContained=false
```

## Common Patterns

### Packaging multiple RIDs

Run the publish command once per RID:

```bash
dotnet publish src/MyApp/MyApp.csproj -t:InfiniFrameSingleFile -r win-x64 -c Release
dotnet publish src/MyApp/MyApp.csproj -t:InfiniFrameSingleFile -r linux-x64 -c Release
dotnet publish src/MyApp/MyApp.csproj -t:InfiniFrameSingleFile -r osx-arm64 -c Release
```

### CI-friendly deterministic output paths

Pass an explicit `-o` directory so build artifacts land in a stable path:

```bash
dotnet publish src/MyApp/MyApp.csproj -t:InfiniFrameSingleFile -r win-x64 -c Release -o artifacts/publish/MyApp
```

### MSBuild auto-run integration

To automatically run single-file packaging as part of your build, add to your `.csproj`:

```xml
<PropertyGroup>
    <InfiniFrameSingleFileAuto>true</InfiniFrameSingleFileAuto>
</PropertyGroup>
```

## App Bootstrap Requirement

After publishing with `InfiniLore.InfiniFrame.SingleFile`, initialize the single-file bootstrap before creating a window:

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

- The publish target embeds `InfiniFrame.Native` and platform loader files (`WebView2Loader.dll` on Windows) as resources.
- `InfiniFrameSingleFileBootstrap.Initialize()` extracts them to a temporary RID-specific folder and registers a native resolver so P/Invoke can load them.

Alternatively, use the higher-level `InfiniFrameSingleFile.Initialize()` helper which also configures embedded static web assets for Blazor apps:

```csharp
using InfiniFrame.SingleFile;

public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        InfiniFrameSingleFile.Initialize();

        var window = InfiniFrameWindowBuilder.Create()
            .SetTitle("My App")
            .SetSize(1280, 720)
            .Center()
            .Build();

        window.WaitForClose();
    }
}
```

## MSBuild Target Reference

The `InfiniFrame.SingleFile.targets` file defines the following MSBuild targets:

| Target | Description |
|--------|-------------|
| `InfiniFrameSingleFile` | Two-pass publish for truly single-file output |
| `InfiniFramePackEmbedStaticWebAssets` | Embeds static web assets and `wwwroot` content as resources |
| `InfiniFramePackEmbedNativeArtifacts` | Embeds native runtime files (`InfiniFrame.Native.dll`, `WebView2Loader.dll`, etc.) |
| `InfiniFramePackCleanupPublishArtifacts` | Removes sidecar files and native files from the publish directory |
| `InfiniFramePackGenerateConfig` | Generates a module initializer to set `InfiniFramePackMode.IsActive` at compile time |
| `InfiniFrameSingleFileAuto` | Auto-runs `InfiniFrameSingleFile` after `Publish` when enabled |

### Pack mode detection

The targets set `InfiniFramePackMode.IsActive = true` via a generated module initializer when packaging is active. The `InfiniFrameSingleFile` library checks this flag at runtime to skip bootstrap when not in pack mode.

## Edge Cases and Pitfalls

- `-r <RID>` is required. The target fails with an error if no `RuntimeIdentifier` is specified.
- `--rid auto` is not supported. You must specify an explicit RID (`win-x64`, `linux-arm64`, `osx-x64`, etc.).
- The two-pass publish performs a full non-single-file publish first. Ensure your project builds successfully in non-single-file mode.
- If your project defines `TargetFrameworks` (plural), the target uses the first framework entry. Pass `-f <TFM>` to select a specific framework.
- The target requires `pwsh` (PowerShell Core) to be available on the system PATH for generating the pack mode initializer.
- If final output does not contain the expected single-file executable, the build may succeed but the app may fail at runtime. Verify the publish output contains the expected executable.
