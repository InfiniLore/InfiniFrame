# InfiniLore.InfiniFrame.Blazor

Pre-built Razor components for custom window chrome — drag areas, window control buttons, and resize handles — designed for use with chromeless InfiniFrame windows

## What it does

- Provides a `InfiniFrameWindowDragArea` component that makes any region of the page draggable as a window title bar
- Provides `InfiniFrameWindowButton` for minimize, maximize, and close
- Provides `InfiniFrameWindowResizeThumb` and `InfiniFrameWindowResizeThumbContainer` for window edge/corner resizing

## Installation

```bash
dotnet add package InfiniLore.InfiniFrame.Blazor
```

Typically used alongside `InfiniLore.InfiniFrame.BlazorWebView` or `InfiniLore.InfiniFrame.WebServer`

## Basic Usage

Enable chromeless mode first:

```csharp
builder.WithInfiniFrameWindowBuilder(w => w
    .SetChromeless(true)
    .SetSize(1280, 720)
    .Center()
);
```

Then use the components in your Blazor layout:

```xml
<InfiniFrameWindowResizeThumbContainer />

<div class="titlebar">
    <InfiniFrameWindowDragArea>
        <span>My Application</span>
    </InfiniFrameWindowDragArea>

    <InfiniFrameWindowButton Action="WindowAction.Minimize" />
    <InfiniFrameWindowButton Action="WindowAction.Maximize" />
    <InfiniFrameWindowButton Action="WindowAction.Close" />
</div>
```

## Components

| Component | Description |
|-----------|-------------|
| `InfiniFrameWindowDragArea` | Draggable region — acts as window title bar |
| `InfiniFrameWindowButton` | Window action button (minimize / maximize / close) |
| `InfiniFrameWindowResizeThumb` | Single edge/corner resize handle |
| `InfiniFrameWindowResizeThumbContainer` | All resize handles in one declaration |

## Enums

| Enum | Values |
|------|--------|
| `WindowAction` | `Minimize`, `Maximize`, `Close` |
| `ResizeOrigin` | `Top`, `Bottom`, `Left`, `Right`, `TopLeft`, `TopRight`, `BottomLeft`, `BottomRight` |

## Links

- [Full Documentation](../../docs/Guides/CustomChrome.md)
- [NuGet](https://www.nuget.org/packages/InfiniLore.InfiniFrame.Blazor)
