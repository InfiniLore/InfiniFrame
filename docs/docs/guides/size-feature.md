# Size Feature

The Size feature controls window dimensions, minimum/maximum constraints, and resizability. It is available both at build time (to set initial values) and at runtime (to resize the live window).

## Contents

- [Builder Configuration](#builder-configuration)
- [Runtime Control](#runtime-control)
- [Resizability](#resizability)
- [OS Default Size](#os-default-size)

## Builder Configuration

All size methods are chainable and must be called before `Build()`:

```csharp
var builder = InfiniFrameWindowBuilder.Create()
    .SetSize(1280, 720)         // Width x Height
    .SetMinSize(800, 600)
    .SetMaxSize(1920, 1080)
    .SetUseOsDefaultSize(true)  // Let the OS choose the initial size
    .SetStartPageUrl("https://myapp.local");
```

| Method | Description |
|--------|-------------|
| `SetSize(int width, int height)` | Set the initial window size |
| `SetMinSize(int width, int height)` | Set the minimum allowed size |
| `SetMaxSize(int width, int height)` | Set the maximum allowed size |
| `SetUseOsDefaultSize(bool)` | Let the OS choose the initial size |

You can also set individual dimensions:

```csharp
builder
    .SetWidth(1280)
    .SetHeight(720);
```

## Runtime Control

After `Build()`, resize the window through the feature interface or extension methods:

```csharp
// Extension methods (fluent)
window.SetSize(800, 600);

// Direct feature access
window.Features.Size.SetSize(800, 600);
int width = window.Features.Size.Width;
int height = window.Features.Size.Height;
```

### Individual dimension setters

```csharp
window.SetWidth(1024);
window.SetHeight(768);
```

### Min/Max constraints at runtime

```csharp
window.SetMinSize(400, 300);
window.SetMaxSize(1920, 1080);
```

### Relative resize

Resize relative to the current dimensions:

```csharp
// Grow by 100px in each direction
window.Features.Size.Resize(100, 100, ResizeOrigin.BottomRight);
```

### Read current values

```csharp
int width = window.Features.Size.Width;
int height = window.Features.Size.Height;
Size size = window.Features.Size.Size;         // (Width, Height) tuple
Size maxSize = window.Features.Size.MaxSize;
Size minSize = window.Features.Size.MinSize;
```

## Resizability

Control whether the user can resize the window by dragging its edges:

```csharp
// Builder
builder.SetResizable(false);

// Runtime
window.Features.Size.SetResizable(false);
bool resizable = window.Features.Size.IsResizable;
```

:::note
On Windows, enabling chromeless mode (`SetChromeless(true)`) automatically disables resizability. Set it explicitly if needed after calling `SetChromeless`.
:::

## OS Default Size

When `SetUseOsDefaultSize(true)` is set, the OS picks the initial window size based on screen resolution and DPI. This is overridden if `SetSize`, `SetWidth`, or `SetHeight` is also called.

```csharp
builder
    .SetUseOsDefaultSize(true)  // OS decides
    .SetSize(1280, 720);        // Overrides OS default
```

At build time, calling `SetSize` or `SetWidth`/`SetHeight` automatically disables `UseOsDefaultSize`.

## See Also

- [Window Features Architecture](window-features-architecture.md) — How the feature system works
- [Core Window Guide](core-window.md) — Builder API and feature overview
