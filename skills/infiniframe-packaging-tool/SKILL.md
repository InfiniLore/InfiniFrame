---
name: infiniframe-packaging-tool
description: Packaging InfiniFrame applications into single-file executables using InfiniLore.InfiniFrame.Tools.Pack. CLI usage, bootstrap, and fallback policy.
---
# InfiniFrame Packaging Tool

> Skill for packaging InfiniFrame applications into single-file executables using `InfiniLore.InfiniFrame.Tools.Pack`.

## When to Use This Skill

- Publishing single-file distributable apps
- Embedding native InfiniFrame binaries
- Embedding wwwroot content
- CI/CD pipeline for app distribution
- Multi-platform packaging (Windows, Linux, macOS)

## Package

```bash
# Install from NuGet
dotnet tool install --global InfiniLore.InfiniFrame.Tools.Pack

# Or build from source (repo root)
.\src\InfiniFrame.Tools.Pack\install-or-update-pack-tool.ps1
```

## What It Does

Packages InfiniFrame app into single-file executable by embedding:
- `wwwroot` content
- Native InfiniFrame runtime binaries for selected RID

### How It Works

1. Parse CLI options and resolve defaults
2. Resolve native runtime artifacts from preflight `dotnet publish`
3. Run `dotnet publish` in single-file mode with custom MSBuild targets
4. Remove unpacked runtime files from publish folder

**Native files embedded as resources** — app MUST initialize with `InfiniFrameSingleFileBootstrap.Initialize()`.

## Command Syntax

```bash
dotnet tool run infiniframe-pack publish <project.csproj> [options]
```

### Options

| Option | Default | Description |
|--------|---------|-------------|
| `--rid <RID\|auto>` | `auto` | Target runtime identifier |
| `--configuration <Config>` | `Release` | Build configuration |
| `--framework <TFM>` | TargetFramework or first TargetFrameworks | Target framework |
| `--self-contained <true\|false>` | `true` | Self-contained publish mode |
| `--output <path>` | `bin/<Config>/<TFM>/<RID>/publish` | Output directory |
| `--no-restore` | — | Skip restore during publish |
| `--verbose` | — | Normal verbosity for publish |
| `--force-clean-output` | — | Allow recursive deletion of non-default outputs |
| `--native-artifacts-fallback <path>` | — | Explicit fallback native artifact directory |
| `--allow-stale-native-fallback` | — | Permit fallback artifacts when preflight fails |

### Environment Overrides

```bash
INFINIFRAME_PACK_NATIVE_ARTIFACT_FALLBACK=<path>
INFINIFRAME_PACK_ALLOW_STALE_NATIVE_FALLBACK=true|false
```

## Usage Examples

### Basic Publish with Defaults

```bash
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj
```

### Publish for Specific Runtime

```bash
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj --rid win-x64
```

### Multi-Targeted App, Choose Framework

```bash
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj --framework net10.0
```

### Custom Output and Faster Inner-Loop

```bash
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj \
  --configuration Debug \
  --no-restore \
  --output artifacts/publish/MyApp-win-x64 \
  --verbose
```

### CI-Friendly Deterministic Output

```bash
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj \
  --output artifacts/publish/MyApp
```

### Multi-RID Packaging

Run once per RID with separate outputs:

```bash
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj --rid win-x64 --output artifacts/publish/MyApp-win-x64
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj --rid linux-x64 --output artifacts/publish/MyApp-linux-x64
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj --rid osx-arm64 --output artifacts/publish/MyApp-osx-arm64
```

### Prefer Explicit --framework for Multi-Targeting

```bash
# If project uses TargetFrameworks, pass --framework explicitly
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj --framework net9.0
```

## App Bootstrap Requirement

**After packaging**, initialize single-file bootstrap before creating window:

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

### Why This Is Required

- `infiniframe-pack publish` embeds `InfiniFrame.Native` and platform loader files (`WebView2Loader.dll` on Windows) as resources
- `InfiniFrameSingleFileBootstrap.Initialize()` extracts them to temporary RID-specific folder
- Registers native resolver so P/Invoke can load them

**`Initialize()` is idempotent and safe to call once at startup.**

## MSBuild Integration

If project runs packaging from MSBuild target (e.g., with `$(InfiniFramePackCommand)`), tool command must be available on machine.

### Post-Build Packaging

```xml
<PropertyGroup>
  <InfiniFramePackCommand>dotnet tool run infiniframe-pack</InfiniFramePackCommand>
  <InfiniFramePackAfterBuild>true</InfiniFramePackAfterBuild>
</PropertyGroup>
```

See https://github.com/InfiniLore/InfiniFrame/tree/master/examples/InfiniFrameExample.SingleFileExe for complete example.

## Native Artifact Fallback Policy

**Fail-fast by default, repo-agnostic by default.**

### Normal Flow

1. Runs preflight publish
2. Validates native artifacts from that output
3. If validation fails → packaging fails

### Fallback Flow

When you need fallback artifacts:

1. Provide explicit path with `--native-artifacts-fallback <path>`
2. Explicitly allow stale fallback use with `--allow-stale-native-fallback`

### Risk Model

- Fallback artifacts treated as potentially stale
- Fallback usage requires explicit operator opt-in
- Without explicit stale opt-in, tool exits with error even when fallback exists

## Edge Cases and Pitfalls

### Exit Codes

| Exit Code | Meaning |
|-----------|---------|
| `0` | Success |
| `2` | Native dependency missing (prefail validation failed) |
| Non-zero | Other errors |

### RID Auto-Detection

`--rid auto` only supports:
- Current OS
- `x64` or `arm64` architectures

Other architectures → platform-not-supported error.

### Output Folder Cleaning

Existing output folders deleted before publish. By default, only project-local `bin/...` outputs allowed to be cleaned. Use `--force-clean-output` for custom output folders.

### Multi-Targeting

If project defines `TargetFrameworks` and omit `--framework`, first framework entry is used.

### Self-Contained Parsing

`--self-contained` must be `true` or `false` (case-insensitive boolean parsing).

### Main Executable Validation

If final output does not contain expected main single-file executable, tool exits with non-zero code.

## Installation Methods

### Global Install (NuGet)

```bash
dotnet tool install --global InfiniLore.InfiniFrame.Tools.Pack
infiniframe-pack --help
```

### Update or Uninstall

```bash
dotnet tool update --global InfiniLore.InfiniFrame.Tools.Pack
dotnet tool uninstall --global InfiniLore.InfiniFrame.Tools.Pack
```

### Local Install from Source

```bash
dotnet pack src/InfiniFrame.Tools.Pack/InfiniFrame.Tools.Pack.csproj -c Release
dotnet tool install --local --add-source ./src/InfiniFrame.Tools.Pack/bin/Release InfiniLore.InfiniFrame.Tools.Pack
```

Run with:
```bash
dotnet tool run infiniframe-pack --help
```

### Repo Development Install

```powershell
# PowerShell
.\src\InfiniFrame.Tools.Pack\install-or-update-pack-tool.ps1
```

```bash
# Bash
bash ./src/InfiniFrame.Tools.Pack/install-or-update-pack-tool.sh
```

### Manual Global Install from Source

```bash
dotnet pack src/InfiniFrame.Tools.Pack/InfiniFrame.Tools.Pack.csproj -c Release
dotnet tool install --global --add-source ./src/InfiniFrame.Tools.Pack/bin/Release InfiniLore.InfiniFrame.Tools.Pack
```

## Common Patterns

### CI/CD Pipeline

```yaml
# GitHub Actions example
- name: Package Windows
  run: dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj --rid win-x64 --output artifacts/win-x64

- name: Package Linux
  run: dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj --rid linux-x64 --output artifacts/linux-x64
```

### Debug Build for Testing

```bash
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj \
  --configuration Debug \
  --output artifacts/debug
```

### Clean Output Before Publish

```bash
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj \
  --force-clean-output \
  --output custom/output/path
```

## Anti-Patterns

❌ **Forget bootstrap initialization**:
```csharp
// WRONG — native binaries won't be found
var window = InfiniFrameWindowBuilder.Create().Build();
```

✅ **Always initialize bootstrap for packaged apps**:
```csharp
InfiniFrameSingleFileBootstrap.Initialize();
var window = InfiniFrameWindowBuilder.Create().Build();
```

❌ **Use --rid auto for unsupported architectures**:
```bash
# WRONG — linux-arm not supported by auto-detection
dotnet tool run infiniframe-pack publish MyApp.csproj --rid linux-arm
```

✅ **Specify RID explicitly**:
```bash
dotnet tool run infiniframe-pack publish MyApp.csproj --rid linux-arm --native-artifacts-fallback path/to/artifacts
```

❌ **Allow stale fallback without explicit path**:
```bash
# WRONG — will fail without explicit fallback path
dotnet tool run infiniframe-pack publish MyApp.csproj --allow-stale-native-fallback
```

✅ **Provide both path and stale permission**:
```bash
dotnet tool run infiniframe-pack publish MyApp.csproj \
  --native-artifacts-fallback artifacts/native \
  --allow-stale-native-fallback
```

❌ **Run packaging without native binaries in preflight**:
```bash
# WRONG — preflight will fail if natives not present
dotnet tool run infiniframe-pack publish MyApp.csproj
```

✅ **Ensure native artifacts present before packaging**:
```bash
# First ensure natives are built/present
dotnet publish MyApp.csproj -r win-x64
# Then package
dotnet tool run infiniframe-pack publish MyApp.csproj --rid win-x64
```
