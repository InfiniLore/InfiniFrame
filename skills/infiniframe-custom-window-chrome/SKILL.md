---
name: infiniframe-custom-window-chrome
description: Building custom window title bars, resize handles, and chromeless UIs using InfiniLore.InfiniFrame.Blazor components.
---
# InfiniFrame Custom Window Chrome

> Skill for building custom window title bars, resize handles, and chromeless UIs using `InfiniLore.InfiniFrame.Blazor`.

## When to Use This Skill

- Creating chromeless windows (no native title bar)
- Building custom title bars with Blazor components
- Implementing custom resize handles
- Designing modern desktop app UIs
- Implementing drag-to-move window functionality

## Package

```bash
dotnet add package InfiniLore.InfiniFrame.Blazor
```

This is a companion package — typically used with `InfiniLore.InfiniFrame.BlazorWebView` or `InfiniLore.InfiniFrame.WebServer`.

## Enable Chromeless Mode

Remove native OS title bar so Blazor UI is entire window:

```csharp
builder.WithInfiniFrameWindowBuilder(w => w
    .SetChromeless(true)
    .SetTransparent(true)  // Optional: for rounded corners/glassmorphism
    .SetSize(1280, 720)
    .Center()
);
```

**Windows note**: `SetChromeless(true)` automatically disables `UseOsDefaultLocation`, `UseOsDefaultSize`, and `Resizable`. Set them explicitly after calling `SetChromeless` if needed.

## Components

### InfiniFrameWindowDragArea

Makes any area draggable — acts as window's title bar.

```razor
<InfiniFrameWindowDragArea>
    <span>My Application</span>
</InfiniFrameWindowDragArea>
```

**Features**:
- Handles pointer capture automatically
- Drag operations stable even when cursor moves fast
- Place at top of layout to create custom drag region

### InfiniFrameWindowButton

Button that performs window action (minimize, maximize, or close):

```razor
<InfiniFrameWindowButton Action="WindowAction.Minimize" />
<InfiniFrameWindowButton Action="WindowAction.Maximize" />
<InfiniFrameWindowButton Action="WindowAction.Close" />
```

| `WindowAction` | Description |
|----------------|-------------|
| `Minimize` | Minimizes window to taskbar |
| `Maximize` | Maximizes or restores window |
| `Close` | Closes window and exits application |

Each button styled via `.razor.css` scoped stylesheet — override styles by targeting component's generated class or wrapping in styled container.

### InfiniFrameWindowResizeThumb

Drag handle for resizing from specific edge or corner:

```razor
<InfiniFrameWindowResizeThumb Origin="ResizeOrigin.BottomRight" />
```

Available `ResizeOrigin` values:
- `TopLeft`, `Top`, `TopRight`
- `Left`, `Right`
- `BottomLeft`, `Bottom`, `BottomRight`

### InfiniFrameWindowResizeThumbContainer

Renders resize thumbs for ALL edges and corners in single declaration:

```razor
<InfiniFrameWindowResizeThumbContainer />
```

**Place at root level** of layout so it covers entire window perimeter.

## Full Layout Example

Complete custom window chrome in Blazor layout:

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

`InfiniLore.InfiniFrame.Js` used internally by drag and resize components to call `setPointerCapture` on underlying DOM element.

If building custom drag components, use `IInfiniFrameJs` directly:

```razor
@inject IInfiniFrameJs InfiniJs

<div @ref="dragRef" @onpointerdown="OnPointerDown">Drag me</div>

@code {
    ElementReference dragRef;

    async Task OnPointerDown(PointerEventArgs e) {
        await InfiniJs.SetPointerCaptureAsync(dragRef, e.PointerId, CancellationToken.None);
    }
}
```

## Styling Tips

### Transparent Resize Thumbs

Resize thumbs are transparent by default — they only respond to pointer events at window edge.

### Transparency Effects

On Windows with `SetTransparent(true)`, CSS `background: transparent` shows through to desktop — enables acrylic or mica-style effects via CSS backdrop.

```csharp
// In Program.cs
builder.WithInfiniFrameWindowBuilder(w => w
    .SetChromeless(true)
    .SetTransparent(true)
);
```

```css
/* In Blazor component */
.window-root {
    background: rgba(30, 30, 46, 0.8);
    backdrop-filter: blur(10px);
}
```

### Double-Click to Maximize

Double-clicking on `InfiniFrameWindowDragArea` does NOT automatically maximize — handle `@ondblclick` yourself:

```razor
@inject IInfiniFrameWindow Window

<InfiniFrameWindowDragArea @ondblclick="ToggleMaximize">
    <span>My App</span>
</InfiniFrameWindowDragArea>

@code {
    void ToggleMaximize() => Window.Invoke(() => {
        Window.ToggleMaximized();
    });
}
```

## Common Patterns

### Minimal Title Bar

```razor
<div class="titlebar">
    <InfiniFrameWindowDragArea class="drag-area">
        <span>My App</span>
    </InfiniFrameWindowDragArea>
    <InfiniFrameWindowButton Action="WindowAction.Close" />
</div>
```

### Custom Styled Buttons

```razor
<div class="custom-buttons">
    <InfiniFrameWindowButton Action="WindowAction.Minimize" class="btn-minimize" />
    <InfiniFrameWindowButton Action="WindowAction.Maximize" class="btn-maximize" />
    <InfiniFrameWindowButton Action="WindowAction.Close" class="btn-close" />
</div>

<style>
.custom-buttons { display: flex; }
.btn-close:hover { background: #e81123; color: white; }
.btn-minimize:hover, .btn-maximize:hover { background: rgba(255,255,255,0.1); }
</style>
```

### Drag Area with App Icon

```razor
<InfiniFrameWindowDragArea class="title-drag">
    <img src="app-icon.svg" class="app-icon" />
    <span class="app-title">@AppName</span>
</InfiniFrameWindowDragArea>

<style>
.title-drag {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 0 16px;
    height: 100%;
}
.app-icon { width: 16px; height: 16px; }
.app-title { font-weight: 600; }
</style>
```

### Window with Only Resize Thumbs (No Title Bar)

```razor
<div class="chromeless-root">
    <InfiniFrameWindowResizeThumbContainer />
    <main class="content">
        @Body
    </main>
</div>

<style>
.chromeless-root {
    height: 100vh;
    position: relative;
}
.content {
    height: 100%;
    overflow: auto;
}
</style>
```

## Anti-Patterns

❌ **Forget to enable chromeless mode**:
```razor
<!-- WRONG — native title bar will still show -->
<InfiniFrameWindowDragArea>...</InfiniFrameWindowDragArea>
```

✅ **Enable chromeless in builder**:
```csharp
builder.WithInfiniFrameWindowBuilder(w => w.SetChromeless(true));
```

❌ **Place resize thumbs inside content area**:
```razor
<!-- WRONG — thumbs won't work at window edges -->
<main>
    <InfiniFrameWindowResizeThumbContainer />
</main>
```

✅ **Place thumbs at root level**:
```razor
<div class="root">
    <InfiniFrameWindowResizeThumbContainer />
    <main>@Body</main>
</div>
```

❌ **Expect double-click to maximize automatically**:
```razor
<!-- WRONG -- does nothing on double-click -->
<InfiniFrameWindowDragArea>Title</InfiniFrameWindowDragArea>
```

✅ **Handle double-click yourself**:
```razor
<InfiniFrameWindowDragArea @ondblclick="ToggleMaximize">Title</InfiniFrameWindowDragArea>
```
