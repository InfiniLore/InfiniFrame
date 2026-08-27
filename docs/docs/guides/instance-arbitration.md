# Instance Arbitration

This guide covers single-instance enforcement and elevation-aware arbitration for InfiniFrame applications.

Instance arbitration is a builder-only window feature. For an overview of the feature system, see [Window Features Architecture](window-features-architecture.md).

## Contents

- [Overview](#overview)
- [Usage](#usage)
- [Modes](#modes)
- [Elevation Behavior](#elevation-behavior)
- [Platform Notes](#platform-notes)

## Overview

Instance arbitration ensures that only one instance of your application runs at a time. When a second instance attempts to start, it can either be blocked entirely or forward its arguments to the primary instance.

This feature uses a named mutex for process-level synchronization and can optionally detect whether the process is running with elevated (administrator/root) privileges.

## Usage

Configure instance arbitration through the builder API before calling `Build()`:

```csharp
using InfiniFrame;

IInfiniFrameWindow window = InfiniFrameWindowBuilder.Create()
    .SetInstanceArbitrationMode(InstanceArbitrationMode.PrimaryOnly)
    .SetInstanceArbitrationMutexName("MyApp.SingleInstance")
    .SetStartPageUrl("https://example.com")
    .Build();
window.WaitForClose();
```

You can also configure it through the features property directly:

```csharp
var builder = InfiniFrameWindowBuilder.Create();
builder.Features.InstanceArbitration.SetMode(InstanceArbitrationMode.PrimaryOnly);
builder.Features.InstanceArbitration.SetMutexName("MyApp.SingleInstance");

IInfiniFrameWindow window = builder.Build();
window.WaitForClose();
```

## Modes

| Mode | Description |
|------|-------------|
| `Disabled` | No instance arbitration. Multiple instances can run simultaneously. This is the default. |
| `PrimaryOnly` | Only the primary instance is allowed. A secondary instance throws `InstanceAlreadyRunningException`. |
| `PrimaryWithArgForwarding` | Only the primary instance is allowed, with command-line argument forwarding to the primary. |

## Elevation Behavior

When instance arbitration is enabled, InfiniFrame detects whether the process is running with elevated privileges:

- **Windows**: Uses `WindowsIdentity.GetCurrent()` and checks for `WindowsBuiltInRole.Administrator`
- **Linux/macOS**: Checks if the process user ID is `0` (root)

Elevation detection is available for audit and logging purposes. The arbitration behavior itself does not change based on elevation level.

## Platform Notes

> **Windows:** Named mutexes are fully supported. Elevation detection uses the Windows security principal API.

> **Linux:** Named mutexes are supported via file-based synchronization. Elevation detection checks `getuid() == 0`.

> **macOS:** Named mutexes are supported via file-based synchronization. Elevation detection checks `getuid() == 0`.

## See Also

- [Window Features Architecture](window-features-architecture.md) — How the feature system works
- [Core Window Guide](core-window.md) — Builder API and feature overview
