# Custom Window Chrome Guide

`InfiniLore.InfiniFrame.Blazor` provides pre-built Razor components for building custom window title bars and resize handles, typically used together with a chromeless window. For non-Blazor apps (React, Vue, plain HTML), a JavaScript API is also available — see [JavaScript Window Chrome API](#javascript-window-chrome-api).

## Contents

- [Installation](#installation)
- [Enable Chromeless Mode](#enable-chromeless-mode)
- [Components](#components)
- [Full Layout Example](#full-layout-example)
- [JavaScript Interop for Drag Areas](#javascript-interop-for-drag-areas)
- [JavaScript Window Chrome API](#javascript-window-chrome-api)
- [Styling Tips](#styling-tips)

## Installation

```bash
dotnet add package InfiniLore.InfiniFrame.Blazor
```

This package is a companion to `InfiniLore.InfiniFrame.BlazorWebView` or `InfiniLore.InfiniFrame.WebServer`.

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

On Windows, enabling chromeless mode automatically disables `UseOsDefaultLocation`, `UseOsDefaultSize`, and `Resizable`. Set them explicitly if needed after calling `SetChromeless`.

## Components

### InfiniFrameWindowDragArea

Makes any area of the page draggable, acting as the window's title bar.

```razor
<InfiniFrameWindowDragArea>
    <span>My Application</span>
</InfiniFrameWindowDragArea>
```

Place this at the top of your layout to create a custom drag region.
The component handles pointer capture automatically so drag operations remain stable even when the cursor moves fast.

### InfiniFrameWindowButton

A button that performs a window action (minimize, maximize, or close):

```razor
<InfiniFrameWindowButton Action="WindowAction.Minimize" />
<InfiniFrameWindowButton Action="WindowAction.Maximize" />
<InfiniFrameWindowButton Action="WindowAction.Close" />
```

| `WindowAction` | Description                                 |
|----------------|---------------------------------------------|
| `Minimize`     | Minimizes the window to the taskbar         |
| `Maximize`     | Maximizes or restores the window            |
| `Close`        | Closes the window and exits the application |

Each button is styled via its `.razor.css` scoped stylesheet. Override the styles in your own CSS by targeting the component's generated class or wrapping it in a styled container.

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

Place this at the root level of your layout so it covers the entire window perimeter.

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

`InfiniLore.InfiniFrame.Js` is used internally by the drag and resize components to call `setPointerCapture` on the underlying DOM element. This ensures drag operations continue even when the pointer leaves the element boundary.

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

See the [JavaScript Interop Guide](javascript-interop.md) for full details.

## Styling Tips

- The resize thumbs are transparent by default; they only respond to pointer events at the window edge.
- On Windows with `SetTransparent(true)`, your CSS `background: transparent` will show through to the desktop, enabling acrylic or mica-style effects via the CSS backdrop.
- Double-clicking on a `InfiniFrameWindowDragArea` automatically toggles maximize/restore. The component handles this internally.

## JavaScript Window Chrome API

For non-Blazor apps (React, Vue, plain HTML), InfiniFrame provides a JavaScript API at `window.infiniframe.windowChrome` that enables custom window chrome with native window behavior.

### API: `window.infiniframe.windowChrome.register()`

```typescript
window.infiniframe.windowChrome.register({
    dragRegion: '.app-titlebar',           // CSS selector for drag area
    controls: {
        minimize: '[data-role=minimize]',    // CSS selector for minimize button
        maximize: '[data-role=maximize]',    // CSS selector for maximize button
        close: '[data-role=close]'           // CSS selector for close button
    },
    resize: {
        top: '.resize-top',                  // Optional: CSS selectors for resize zones
        right: '.resize-right',
        bottom: '.resize-bottom',
        left: '.resize-left',
        topLeft: '.resize-top-left',
        topRight: '.resize-top-right',
        bottomLeft: '.resize-bottom-left',
        bottomRight: '.resize-bottom-right'
    }
});
```

**Drag regions:**
- Pointer-down starts window drag (with pointer capture for stable tracking)
- Double-click toggles maximize/restore
- Dragging from a maximized state restores the window and centers it under the cursor

**Window controls:**
- Minimize, maximize/restore, and close use the native window management APIs
- Click events are handled automatically

**Resize zones:**
- Pointer-drag on edges/corners resizes the window
- Works with all 8 directions (4 edges + 4 corners)
- Resize thumbs are hidden when maximized (matching Blazor component behavior)

### Data Attribute Convention

Instead of the JS API, you can use data attributes directly on HTML elements:

```html
<!-- Drag region -->
<div data-infiniframe-drag-region>
    <span>My Application</span>
</div>

<!-- Window control buttons -->
<button data-infiniframe-window-action="minimize">Minimize</button>
<button data-infiniframe-window-action="maximize">Maximize</button>
<button data-infiniframe-window-action="close">Close</button>

<!-- Resize zones -->
<div data-infiniframe-resize="top"></div>
<div data-infiniframe-resize="right"></div>
<div data-infiniframe-resize="bottom"></div>
<div data-infiniframe-resize="left"></div>
<div data-infiniframe-resize="top-left"></div>
<div data-infiniframe-resize="top-right"></div>
<div data-infiniframe-resize="bottom-left"></div>
<div data-infiniframe-resize="bottom-right"></div>
```

The API and data attributes can be used together — both are scanned on initialization.

### Plain HTML Example

```html
<!DOCTYPE html>
<html>
<head>
    <style>
        .app-titlebar {
            display: flex;
            align-items: center;
            height: 32px;
            background: #1e1e2e;
            user-select: none;
        }
        .app-titlebar span { flex: 1; padding: 0 12px; color: #fff; }
        .window-buttons { display: flex; height: 100%; }
        .window-buttons button {
            width: 46px; height: 100%; border: none;
            background: transparent; color: #fff; cursor: pointer;
            font-size: 14px;
        }
        .window-buttons button:hover { background: rgba(255,255,255,0.1); }
        .resize-top { position: absolute; top: 0; left: 0; right: 0; height: 5px; cursor: n-resize; }
        .resize-right { position: absolute; top: 0; right: 0; bottom: 0; width: 5px; cursor: e-resize; }
        .resize-bottom { position: absolute; bottom: 0; left: 0; right: 0; height: 5px; cursor: s-resize; }
        .resize-left { position: absolute; top: 0; left: 0; bottom: 0; width: 5px; cursor: w-resize; }
    </style>
</head>
<body>
    <div class="app-titlebar">
        <span>My Application</span>
        <div class="window-buttons">
            <button data-role="minimize">&#x1F5D5;</button>
            <button data-role="maximize">&#x1F5D6;</button>
            <button data-role="close">&#x2715;</button>
        </div>
    </div>
    <main>Content here</main>
    <div class="resize-top"></div>
    <div class="resize-right"></div>
    <div class="resize-bottom"></div>
    <div class="resize-left"></div>

    <script>
        window.infiniframe.windowChrome.register({
            dragRegion: '.app-titlebar',
            controls: {
                minimize: '[data-role=minimize]',
                maximize: '[data-role=maximize]',
                close: '[data-role=close]'
            },
            resize: {
                top: '.resize-top',
                right: '.resize-right',
                bottom: '.resize-bottom',
                left: '.resize-left'
            }
        });
    </script>
</body>
</html>
```

### React Example

```tsx
import { useEffect, useRef } from 'react';

function TitleBar() {
    const titleRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        // Register after mount
        window.infiniframe?.windowChrome?.register({
            dragRegion: '.app-titlebar',
            controls: {
                minimize: '[data-role=minimize]',
                maximize: '[data-role=maximize]',
                close: '[data-role=close]'
            },
            resize: {
                top: '.resize-top',
                right: '.resize-right',
                bottom: '.resize-bottom',
                left: '.resize-left'
            }
        });

        return () => {
            window.infiniframe?.windowChrome?.unregister();
        };
    }, []);

    return (
        <>
            <div className="app-titlebar" ref={titleRef}>
                <span>My React App</span>
                <div className="window-buttons">
                    <button data-role="minimize">&#x1F5D5;</button>
                    <button data-role="maximize">&#x1F5D6;</button>
                    <button data-role="close">&#x2715;</button>
                </div>
            </div>
            <div className="resize-top" />
            <div className="resize-right" />
            <div className="resize-bottom" />
            <div className="resize-left" />
        </>
    );
}
```

### Vue Example

```vue
<template>
    <div class="app-titlebar">
        <span>My Vue App</span>
        <div class="window-buttons">
            <button data-role="minimize">&#x1F5D5;</button>
            <button data-role="maximize">&#x1F5D6;</button>
            <button data-role="close">&#x2715;</button>
        </div>
    </div>
    <div class="resize-top" />
    <div class="resize-right" />
    <div class="resize-bottom" />
    <div class="resize-left" />
</template>

<script setup>
import { onMounted, onUnmounted } from 'vue';

onMounted(() => {
    window.infiniframe?.windowChrome?.register({
        dragRegion: '.app-titlebar',
        controls: {
            minimize: '[data-role=minimize]',
            maximize: '[data-role=maximize]',
            close: '[data-role=close]'
        },
        resize: {
            top: '.resize-top',
            right: '.resize-right',
            bottom: '.resize-bottom',
            left: '.resize-left'
        }
    });
});

onUnmounted(() => {
    window.infiniframe?.windowChrome?.unregister();
});
</script>
```

### Unregistering

Call `unregister()` to remove all event listeners and clean up:

```typescript
window.infiniframe.windowChrome.unregister();
```

### Platform Notes

- **Windows**: Window controls use Segoe MDL2 Assets icons. The close button turns red on hover.
- **macOS**: Traffic light-style buttons are supported via CSS (see Blazor component styles for reference).
- **Linux**: Uses Unicode symbols. Title length may be limited by the window manager.
- **All platforms**: The JS API uses the same native window management APIs as the Blazor components, ensuring consistent behavior across platforms.