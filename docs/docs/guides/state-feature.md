# State Feature

The State feature controls window state: maximized, minimized, fullscreen, topmost, focus, and zoom level. It is available at build time for initial state and at runtime for live state changes.

## Contents

- [Builder Configuration](#builder-configuration)
- [Runtime Control](#runtime-control)
- [Window State Operations](#window-state-operations)
- [Zoom](#zoom)

## Builder Configuration

Set the initial window state before `Build()`:

```csharp
var builder = InfiniFrameWindowBuilder.Create()
    .SetMaximized(true)
    .SetMinimized(true)
    .SetFullScreen(true)
    .SetTopMost(true)           // Always on top
    .SetZoom(150)               // Zoom level (100 = default)
    .SetStartPageUrl("https://myapp.local");
```

| Method | Description |
|--------|-------------|
| `SetMaximized(bool)` | Start maximized |
| `SetMinimized(bool)` | Start minimized |
| `SetFullScreen(bool)` | Start in fullscreen mode |
| `SetTopMost(bool)` | Always on top of other windows |
| `SetZoom(int)` | Set initial zoom level (100 = default) |

## Runtime Control

After `Build()`, change window state through extension methods or the feature interface:

```csharp
// Extension methods (fluent)
window.SetMaximized(true);
window.SetMinimized(false);
window.SetFullScreen(false);

// Direct feature access
window.Features.State.SetMaximized(true);
```

### Read current state

```csharp
bool maximized = window.Features.State.IsMaximized;
bool minimized = window.Features.State.IsMinimized;
bool fullscreen = window.Features.State.IsFullScreen;
bool topMost = window.Features.State.IsTopMost;
bool focused = window.Features.State.IsFocused;
```

## Window State Operations

### Maximize / Restore

```csharp
window.SetMaximized(true);               // Maximize
window.SetMaximized(false);              // Restore
window.Features.State.ToggleMaximized(); // Toggle between maximized and restored
```

### Minimize

```csharp
window.SetMinimized(true);  // Minimize to taskbar
```

### Fullscreen

```csharp
window.SetFullScreen(true);   // Enter fullscreen
window.SetFullScreen(false);  // Exit fullscreen
```

When entering fullscreen, the window caches its pre-fullscreen bounds and restores them when exiting.

### Topmost

Keep the window above all other windows:

```csharp
window.Features.State.SetTopMost(true);
```

### Focus

Bring the window to the foreground and give it keyboard focus:

```csharp
window.Features.State.SetFocused();
```

## Zoom

Control the browser zoom level. The zoom factor is a percentage where 100 is the default.

### Builder configuration

```csharp
builder
    .SetZoom(150)               // 150% zoom at startup
    .SetZoomEnabled(false);     // Disable user zoom controls
```

### Runtime control

```csharp
window.Features.State.SetZoomFactor(200);     // Set to 200%
float zoom = window.Features.State.ZoomFactor; // Read current zoom

window.Features.State.EnableZoom(false);      // Disable/enable zoom at runtime
```

## See Also

- [Window Features Architecture](window-features-architecture.md) — How the feature system works
- [Core Window Guide](core-window.md) — Builder API and feature overview
