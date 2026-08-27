# Position Feature

The Position feature controls where the window appears on screen. It supports absolute positioning, centering on monitors, and runtime relocation.

## Contents

- [Builder Configuration](#builder-configuration)
- [Runtime Control](#runtime-control)
- [Centering](#centering)
- [Monitor-Aware Positioning](#monitor-aware-positioning)
- [OS Default Location](#os-default-location)

## Builder Configuration

Set the initial window position before `Build()`:

```csharp
var builder = InfiniFrameWindowBuilder.Create()
    .SetLocation(100, 100)      // Left, Top in screen coordinates
    .Center()                   // Center on the primary monitor
    .SetUseOsDefaultLocation(true)
    .SetStartPageUrl("https://myapp.local");
```

| Method | Description |
|--------|-------------|
| `SetLocation(int left, int top)` | Set absolute position in screen coordinates |
| `Center()` | Center on the primary monitor |
| `SetUseOsDefaultLocation(bool)` | Let the OS choose the initial position |

You can also set individual coordinates:

```csharp
builder
    .SetLeft(100)
    .SetTop(50);
```

## Runtime Control

After `Build()`, move the window through extension methods or the feature interface:

```csharp
// Extension methods (fluent)
window.SetLocation(200, 150);

// Direct feature access
window.Features.Position.SetLocation(200, 150);
int left = window.Features.Position.Left;
int top = window.Features.Position.Top;
Point location = window.Features.Position.Location;
```

### Relative movement

Offset the window from its current position:

```csharp
window.Features.Position.Offset(50, 50);          // Relative offset
window.Features.Position.Offset(new Point(50, 50)); // Same via Point
```

### Centering at runtime

```csharp
window.Features.Position.Center();              // Center on primary monitor
window.Features.Position.CenterOnCurrentMonitor(); // Center on whichever monitor the window is on
window.Features.Position.CenterOnMonitor(1);    // Center on a specific monitor by index
```

### Constrained movement

Move the window while keeping it within the current monitor's work area:

```csharp
window.Features.Position.MoveWithinCurrentMonitorArea(100, 100);
```

### Read current values

```csharp
int left = window.Features.Position.Left;
int top = window.Features.Position.Top;
Point location = window.Features.Position.Location;
```

## Centering

Calling `Center()` on the builder centers the window on the primary monitor. Calling `SetLocation` or `SetLeft`/`SetTop` automatically disables centering behavior.

At runtime, `CenterOnCurrentMonitor()` centers on whichever monitor the window is currently on, which is useful after moving between monitors.

## Monitor-Aware Positioning

Use `CenterOnMonitor(int monitorIndex)` and `MoveWithinCurrentMonitorArea` for multi-monitor setups. The monitor index corresponds to `window.Monitors` (see [Monitors feature](monitors-feature.md)).

## OS Default Location

When `SetUseOsDefaultLocation(true)` is set, the OS picks the initial position. This is overridden if `SetLocation`, `SetLeft`, `SetTop`, or `Center` is also called.

```csharp
builder
    .SetUseOsDefaultLocation(true)  // OS decides
    .Center();                      // Overrides OS default
```

:::note
On Windows, enabling chromeless mode (`SetChromeless(true)`) automatically disables `UseOsDefaultLocation`. Set it explicitly if needed after calling `SetChromeless`.
:::

## See Also

- [Monitors Feature](monitors-feature.md) Display enumeration for multi-monitor positioning
- [Window Features Architecture](window-features-architecture.md) How the feature system works
- [Core Window Guide](core-window.md) Builder API and feature overview
