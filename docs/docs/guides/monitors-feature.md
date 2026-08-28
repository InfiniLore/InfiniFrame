# Monitors Feature

The Monitors feature provides information about connected displays: work area, DPI, and monitor enumeration. This is useful for multi-monitor positioning and DPI-aware layouts.

## Contents

- [Querying Monitors](#querying-monitors)
- [Monitor Properties](#monitor-properties)
- [DPI Information](#dpi-information)
- [Using with Positioning](#using-with-positioning)

## Querying Monitors

```csharp
// All connected monitors
foreach (InfiniMonitor monitor in window.Monitors) {
    Console.WriteLine($"Monitor: {monitor.MonitorArea}, Work area: {monitor.WorkArea}, Scale: {monitor.Scale}");
}

// The monitor the window is currently on
InfiniMonitor main = window.MainMonitor;
```

### Via feature interface

```csharp
IEnumerable<InfiniMonitor> monitors = window.Features.Monitors.GetMonitors();
InfiniMonitor main = window.Features.Monitors.GetMainMonitor();
int dpi = window.Features.Monitors.GetMainMonitorScreenDpi();
```

## Monitor Properties

Each `InfiniMonitor` provides:

| Property | Type | Description |
|----------|------|-------------|
| `MonitorArea` | `Rectangle` | Full monitor bounds (including taskbar) |
| `WorkArea` | `Rectangle` | Usable area (excluding taskbar) |
| `Scale` | `double` | DPI scale factor (1.0 = 100%, 1.5 = 150%, etc.) |

## DPI Information

Get the current screen DPI:

```csharp
int dpi = window.ScreenDpi;
double scale = window.MainMonitor.Scale;
```

## Using with Positioning

Use monitor information with the [Position feature](position-feature.md) for multi-monitor setups:

```csharp
// Center on a specific monitor
window.Features.Position.CenterOnMonitor(1);

// Move within the current monitor's work area
window.Features.Position.MoveWithinCurrentMonitorArea(100, 100);

// Get the monitor the window is currently on
InfiniMonitor current = window.MainMonitor;
Console.WriteLine($"Window is on monitor: {current.MonitorArea}");
```

## See Also

- [Position Feature](position-feature.md) Window placement and centering
- [Window Features Architecture](window-features-architecture.md) How the feature system works
- [Core Window Guide](core-window.md) Builder API and feature overview
