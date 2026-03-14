# Custom Window Chrome Guide

`InfiniLore.InfiniFrame.Blazor` provides pre-built Razor components for building custom window title bars and resize handles — typically used together with a chromeless window

## Contents

- [Installation](#installation)
- [Enable Chromeless Mode](#enable-chromeless-mode)
- [Components](#components)
- [Full Layout Example](#full-layout-example)
- [JavaScript Interop for Drag Areas](#javascript-interop-for-drag-areas)
- [Styling Tips](#styling-tips)

## Installation

```bash
dotnet add package InfiniLore.InfiniFrame.Blazor
```

This package is a companion to `InfiniLore.InfiniFrame.BlazorWebView` or `InfiniLore.InfiniFrame.WebServer`

## Enable Chromeless Mode

Remove the native OS title bar so your Blazor UI is the entire window:

```csharp
builder.WithInfiniFrameWindowBuilder(w => w
    .SetChromeless(true)
    .SetTransparent(true)  // Optional: for rounded corners or glassmorphism effects
    .SetSize(1280, 720)
    .Center()
);
```

On Windows, enabling chromeless mode automatically disables `UseOsDefaultLocation`, `UseOsDefaultSize`, and `Resizable` — set them explicitly if needed after calling `SetChromeless`

## Components

### InfiniFrameWindowDragArea

Makes any area of the page draggable — acts as the window's title bar

```razor
<InfiniFrameWindowDragArea>
    <span>My Application</span>
</InfiniFrameWindowDragArea>
```

Place this at the top of your layout to create a custom drag region
The component handles pointer capture automatically so drag operations remain stable even when the cursor moves fast

### InfiniFrameWindowButton

A button that performs a window action (minimize, maximize, or close):

```razor
<InfiniFrameWindowButton Action="WindowAction.Minimize" />
<InfiniFrameWindowButton Action="WindowAction.Maximize" />
<InfiniFrameWindowButton Action="WindowAction.Close" />
```

| `WindowAction` | Description |
|----------------|-------------|
| `Minimize` | Minimizes the window to the taskbar |
| `Maximize` | Maximizes or restores the window |
| `Close` | Closes the window and exits the application |

Each button is styled via its `.razor.css` scoped stylesheet — override the styles in your own CSS by targeting the component's generated class or wrapping it in a styled container

### InfiniFrameWindowResizeThumb

A drag handle for resizing the window from a specific edge or corner:

```razor
<InfiniFrameWindowResizeThumb Origin="ResizeOrigin.BottomRight" />
```

### InfiniFrameWindowResizeThumbContainer

Renders resize thumbs for all edges and corners in a single declaration:

```razor
<InfiniFrameWindowResizeThumbContainer />
```

Place this at the root level of your layout so it covers the entire window perimeter

## Full Layout Example

A complete custom window chrome in a Blazor layout:

```razor
@* MainLayout.razor *@
@inherits LayoutComponentBase

<div class="window-root">

    <!-- Resize handles on all edges -->
    <InfiniFrameWindowResizeThumbContainer />

    <!-- Custom title bar -->
    <div class="titlebar">
        <InfiniFrameWindowDragArea class="drag-region">
            <img src="icon.png" alt="App icon" width="16" />
            <span class="title">My Application</span>
        </InfiniFrameWindowDragArea>

        <div class="window-buttons">
            <InfiniFrameWindowButton Action="WindowAction.Minimize" />
            <InfiniFrameWindowButton Action="WindowAction.Maximize" />
            <InfiniFrameWindowButton Action="WindowAction.Close" />
        </div>
    </div>

    <!-- Page content -->
    <main class="content">
        @Body
    </main>

</div>
```

```css
/* MainLayout.razor.css */
.window-root {
    display: flex;
    flex-direction: column;
    height: 100vh;
    overflow: hidden;
}

.titlebar {
    display: flex;
    align-items: center;
    height: 32px;
    background: #1e1e2e;
    user-select: none;
}

.drag-region {
    flex: 1;
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 0 12px;
    height: 100%;
}

.window-buttons {
    display: flex;
    height: 100%;
}

.content {
    flex: 1;
    overflow: auto;
}
```

## JavaScript Interop for Drag Areas

`InfiniLore.InfiniFrame.Js` is used internally by the drag and resize components to call `setPointerCapture` on the underlying DOM element — this ensures drag operations continue even when the pointer leaves the element boundary

If you are building your own drag components, you can use `IInfiniFrameJs` directly:

```csharp
@inject IInfiniFrameJs InfiniJs

<div @ref="dragRef" @onpointerdown="OnPointerDown">...</div>

@code {
    ElementReference dragRef;

    async Task OnPointerDown(PointerEventArgs e) {
        await InfiniJs.SetPointerCaptureAsync(dragRef, e.PointerId, CancellationToken.None);
    }
}
```

See the [JavaScript Interop Guide](JsInterop.md) for full details

## Styling Tips

- The resize thumbs are transparent by default — they only respond to pointer events at the window edge
- On Windows with `SetTransparent(true)`, your CSS `background: transparent` will show through to the desktop, enabling acrylic or mica-style effects via the CSS backdrop
- Double-clicking on a `InfiniFrameWindowDragArea` does not automatically maximize — handle `@ondblclick` yourself if you want that behavior:

```razor
<InfiniFrameWindowDragArea @ondblclick="ToggleMaximize">
    ...
</InfiniFrameWindowDragArea>

@code {
    @inject IInfiniFrameJs InfiniJs
    // Call window maximize via JS interop or IInfiniFrameWindow
}
```