# Decorations Feature

The Decorations feature controls the visual appearance of the window: title, icon, chromeless mode, transparency, and background color. It is available at build time for initial configuration and at runtime for live changes.

## Contents

- [Builder Configuration](#builder-configuration)
- [Runtime Control](#runtime-control)
- [Title and Icon](#title-and-icon)
- [Chromeless Mode](#chromeless-mode)
- [Transparency](#transparency)
- [Background Color](#background-color)

## Builder Configuration

```csharp
var builder = InfiniFrameWindowBuilder.Create()
    .SetTitle("My Application")
    .SetIconFile("assets/icon.ico")  // Windows and Linux only
    .SetWindowsAppUserModelId("MyCompany.MyApplication")
    .SetChromeless(true)
    .SetTransparent(true)
    .SetBackgroundColor("#FF5733")
    .SetStartPageUrl("https://myapp.local");
```

| Method | Description |
|--------|-------------|
| `SetTitle(string)` | Set the window title |
| `SetIconFile(string)` | Set the window icon (`.ico` on Windows, `.png` on Linux) |
| `SetWindowsAppUserModelId(string)` | Set the Windows taskbar identity |
| `SetChromeless(bool)` | Remove the native title bar and borders |
| `SetTransparent(bool)` | Enable window transparency |
| `SetBackgroundColor(string?)` | Set the native background color |

## Runtime Control

After `Build()`, change decorations through extension methods or the feature interface:

```csharp
// Extension methods
window.SetTitle("New Title");
window.SetBackgroundColor("#00FF00");

// Direct feature access
window.Features.Decorations.SetTitle("New Title");
window.Features.Decorations.SetBackgroundColor("#00FF00");

// Read current values
string? title = window.Features.Decorations.Title;
string? color = window.Features.Decorations.BackgroundColor;
bool chromeless = window.Features.Decorations.IsChromeless;
bool transparent = window.Features.Decorations.IsTransparent;
```

## Title and Icon

### Builder-time

```csharp
builder
    .SetTitle("My Application")
    .SetIconFile("assets/icon.ico")  // Windows and Linux only; .ico on Windows, .png on Linux
    .SetWindowsAppUserModelId("MyCompany.MyApplication") // Windows taskbar identity
```

`SetWindowsAppUserModelId` assigns an explicit process identity before the first window is shown. Use one stable,
whitespace-free ID of at most 128 characters for every window in the process. For installed Windows applications,
configure shortcuts with the same AppUserModelID so pinned taskbar items group with the running application.

### Runtime

```csharp
window.SetTitle("Updated Title");
string? currentTitle = window.Features.Decorations.Title;

window.Features.Decorations.SetIconFile("assets/new-icon.ico");
```

### Platform notes

- **Windows:** `.ico` format recommended. The icon appears in the taskbar and title bar.
- **Linux:** `.png` format. Title length may be limited by the window manager (use `SetLimitLinuxWindowTitleLength` to manage this).
- **macOS:** Icon is not displayed in the title bar by default.

## Chromeless Mode

Remove the native OS title bar so your web UI is the entire window:

```csharp
builder
    .SetChromeless(true)
    .SetTransparent(true)  // Optional: for rounded corners or glassmorphism effects
```

On Windows, enabling chromeless mode automatically disables `UseOsDefaultLocation`, `UseOsDefaultSize`, and `Resizable` since they are incompatible. Set them explicitly if needed after calling `SetChromeless`.

See the [Custom Window Chrome guide](custom-window-chrome.md) for Blazor components and JavaScript APIs for building your own title bar.

## Transparency

Enable window transparency to let the web content show through:

```csharp
builder
    .SetTransparent(true)
    .SetBackgroundColor("transparent")
```

Transparency is useful for:
- Acrylic or mica-style effects via CSS `backdrop-filter`
- Rounded corners on chromeless windows
- Fully transparent window regions

:::note
Transparency is a startup-only setting. It cannot be changed at runtime.
:::

## Background Color

Set the native window background color using hex color strings:

```csharp
// Builder
builder
    .SetBackgroundColor("#FF5733")    // Set to a specific color
    .SetBackgroundColor("#AARRGGBB")  // With alpha channel
    .SetBackgroundColor("transparent") // Reset to platform default
    .SetBackgroundColor(null)          // Same as "transparent"
```

At runtime, the background color can be changed dynamically:

```csharp
window.SetBackgroundColor("#00FF00");
window.Features.Decorations.SetBackgroundColor(null); // Reset
string? currentColor = window.Features.Decorations.BackgroundColor;
```

| Platform | Builder-time | Runtime | Notes |
|----------|-------------|---------|-------|
| Windows (WebView2) | Sets `DefaultBackgroundColor` at init; also applies if called before window creation | Sets `DefaultBackgroundColor` and reloads the webview | Color format: `#RRGGBB` or `#AARRGGBB`. Alpha=0 means transparent. |
| Linux (WebKitGTK) | Sets WebKitGTK background color at init | Sets WebKitGTK background color via `webkit_web_view_set_background_color` | Color format: `#RRGGBB`. GTK handles alpha via RGBA visual. |
| macOS (WKWebView) | Sets WKWebView `backgroundColor` at init | Sets WKWebView `backgroundColor` | Color format: `#RRGGBB`. NSColor parsing from hex string. |

- Pass `null` or `"transparent"` to reset to the platform default (no background color override).
- Invalid hex strings throw `ArgumentException` at runtime.

## See Also

- [Custom Window Chrome](custom-window-chrome.md) — Blazor components and JS API for chromeless windows
- [Window Features Architecture](window-features-architecture.md) — How the feature system works
- [Core Window Guide](core-window.md) — Builder API and feature overview
