# Core Window Guide

This guide covers the `InfiniFrameWindowBuilder` — the foundation for creating all InfiniFrame windows. It walks through the builder pattern, single-file packaging, and provides a complete reference to every window feature.

For a conceptual overview of how the feature system works, see [Window Features Architecture](window-features-architecture.md).

## Contents

- [Building a Window](#building-a-window)
- [Single-File Native Packaging](#single-file-native-packaging)
- [Builder Quick Reference](#builder-quick-reference)
- [Feature Guides](#feature-guides)

## Building a Window

All windows are created through `InfiniFrameWindowBuilder` using a fluent API.

```csharp
using InfiniFrame;

var window = InfiniFrameWindowBuilder.Create()
    .SetTitle("My App")
    .SetSize(1280, 720)
    .Center()
    .SetStartPageUrl("https://myapp.local")
    .Build();

window.WaitForClose();
```

`Build()` creates and displays the native window immediately on the calling thread.
The returned `IInfiniFrameWindow` gives you full control over the window at runtime.

## Single-File Native Packaging

When your app is published as a single-file executable with embedded InfiniFrame native binaries, call `InfiniFrameSingleFileBootstrap.Initialize()` before creating any windows.

```csharp
using InfiniFrame;

public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        InfiniFrameSingleFileBootstrap.Initialize();

        var window = InfiniFrameWindowBuilder.Create()
            .SetTitle("My App")
            .SetSize(1280, 720)
            .Center()
            .SetStartPageUrl("app://index.html")
            .Build();

        window.WaitForClose();
    }
}
```

`Initialize()` is idempotent and safe to call once at startup.
Use it for packaged deployments created by `InfiniLore.InfiniFrame.SingleFile` (or any equivalent flow that embeds native files as resources), not for standard development runs where native binaries are already present beside your app.

## Builder Quick Reference

Every builder method maps to a feature. The table below shows the most common methods and their corresponding feature guides.

### Window appearance

| Method | Feature | Guide |
|--------|---------|-------|
| `SetTitle(string)` | Decorations | [Decorations](decorations-feature.md) |
| `SetIconFile(string)` | Decorations | [Decorations](decorations-feature.md) |
| `SetBackgroundColor(string?)` | Decorations | [Decorations](decorations-feature.md) |
| `SetChromeless(bool)` | Decorations | [Decorations](decorations-feature.md) |
| `SetTransparent(bool)` | Decorations | [Decorations](decorations-feature.md) |

### Size and position

| Method | Feature | Guide |
|--------|---------|-------|
| `SetSize(int, int)` | Size | [Size](size-feature.md) |
| `SetMinSize(int, int)` | Size | [Size](size-feature.md) |
| `SetMaxSize(int, int)` | Size | [Size](size-feature.md) |
| `SetLocation(int, int)` | Position | [Position](position-feature.md) |
| `Center()` | Position | [Position](position-feature.md) |

### Window state

| Method | Feature | Guide |
|--------|---------|-------|
| `SetMaximized(bool)` | State | [State](state-feature.md) |
| `SetMinimized(bool)` | State | [State](state-feature.md) |
| `SetFullScreen(bool)` | State | [State](state-feature.md) |
| `SetTopMost(bool)` | State | [State](state-feature.md) |
| `SetResizable(bool)` | Size | [Size](size-feature.md) |

### Content

| Method | Feature | Guide |
|--------|---------|-------|
| `SetStartPageUrl(string?)` | Page Navigation | [Page Navigation](page-navigation-feature.md) |
| `SetStartPageContent(string?)` | Page Navigation | [Page Navigation](page-navigation-feature.md) |
| `SetUrl(Uri?)` | Page Navigation | [Page Navigation](page-navigation-feature.md) |
| `RegisterCustomSchemeHandler(...)` | Page Navigation | [Page Navigation](page-navigation-feature.md) |

### Browser settings

| Method | Feature | Guide |
|--------|---------|-------|
| `SetDevToolsEnabled(bool)` | Debugging | [Debugging](debugging-feature.md) |
| `SetRemoteDebuggingPort(int?)` | Debugging | [Debugging](debugging-feature.md) |
| `SetContextMenuEnabled(bool)` | Browser | [Browser](browser-feature.md) |
| `SetUserAgent(string?)` | Browser | [Browser](browser-feature.md) |
| `EnableStatusBar(bool)` | Browser | [Browser](browser-feature.md) |
| `EnableIgnoreCertificateErrors(bool)` | Browser | [Browser](browser-feature.md) |
| `EnableWebSecurity(bool)` | Browser | [Browser](browser-feature.md) |
| `SetBrowserControlInitParameters(string)` | Browser | [Browser](browser-feature.md) |

### Other features

| Method | Feature | Guide |
|--------|---------|-------|
| `SetMenuBar(InfiniFrameMenuBar)` | Menu | [Native Menu](native-menu.md) |
| `EnableNotifications(bool)` | Notifications | [Notifications](notifications.md) |
| `SetInstanceArbitrationMode(...)` | Instance Arbitration | [Instance Arbitration](instance-arbitration.md) |
| `AddTrustedOrigin(string)` | Browser | [Browser](browser-feature.md) |

### Events

| Property | Guide |
|----------|-------|
| `builder.Events.WindowCreated` | [Lifecycle](lifecycle-feature.md) |
| `builder.Events.WindowClosingRequested` | [Lifecycle](lifecycle-feature.md) |
| `builder.Events.WindowSizeChanged` | [Lifecycle](lifecycle-feature.md) |
| `builder.Events.WebMessageReceived` | [JavaScript Interop](javascript-interop.md) |
| `builder.MessageHandlers` | [JavaScript Interop](javascript-interop.md) |

### Runtime access

After `Build()`, all features are accessible through `window.Features.<Name>` or as extension methods on `IInfiniFrameWindow`:

```csharp
// Extension methods (fluent chaining)
window.SetSize(800, 600);
window.SetTitle("New Title");
window.Close();

// Direct feature access
window.Features.Size.SetSize(800, 600);
window.Features.Decorations.SetTitle("New Title");
window.Features.Lifecycle.Close();
```

## Feature Guides

Each window feature has a dedicated guide covering both builder configuration and runtime control:

### Core features

| Feature | Description | Guide |
|---------|-------------|-------|
| [Size](size-feature.md) | Window dimensions, min/max constraints, resizability | Builder + Runtime |
| [Position](position-feature.md) | Window placement, centering, monitor-aware positioning | Builder + Runtime |
| [State](state-feature.md) | Maximized, minimized, fullscreen, topmost, zoom | Builder + Runtime |
| [Decorations](decorations-feature.md) | Title, icon, chromeless mode, transparency, background color | Builder + Runtime |
| [Browser](browser-feature.md) | Context menu, media, web security, clipboard, user agent, status bar | Builder + Runtime |
| [Debugging](debugging-feature.md) | DevTools, remote debugging, web inspector, diagnostics | Builder + Runtime |
| [Page Navigation](page-navigation-feature.md) | Load URLs/HTML, navigation interception, custom URL schemes | Builder + Runtime |
| [Lifecycle](lifecycle-feature.md) | Window close, ready wait, teardown, events, cross-thread invocation | Runtime |

### Dialogs and system integration

| Feature | Description | Guide |
|---------|-------------|-------|
| [File Dialogs](file-dialogs-feature.md) | Open/save file and folder dialogs | Runtime |
| [Notifications](notifications.md) | Native desktop notifications, rich notifications, async callbacks | Builder + Runtime |
| [Native Menu](native-menu.md) | Menu bar configuration, runtime menu manipulation | Builder + Runtime |
| [Taskbar](#taskbar-progress-and-flash) | Progress indicators and flash notifications | Builder + Runtime |
| [Monitors](monitors-feature.md) | Display enumeration, work area, DPI | Runtime |

### Input and messaging

| Feature | Description | Guide |
|---------|-------------|-------|
| [Drag and Drop](drag-drop-feature.md) | File drop handling, extension filtering | Runtime |
| [JavaScript Execution](javascript-execution-feature.md) | Execute arbitrary JS in the browser control | Runtime |
| [JavaScript Interop](javascript-interop.md) | Two-way C#/JS messaging, InfiniFrame.Js | Runtime |
| [Invoke](invoke-feature.md) | Cross-thread dispatch to the window's native thread | Runtime |

### Configuration

| Feature | Description | Guide |
|---------|-------------|-------|
| [Instance Arbitration](instance-arbitration.md) | Single-instance enforcement, elevation detection | Builder |
| [Custom Window Chrome](custom-window-chrome.md) | Blazor components and JS API for chromeless windows | Builder + Runtime |

## Taskbar Progress and Flash

InfiniFrame provides cross-platform taskbar integration for progress indicators and flash notifications. Access the taskbar feature through `window.Features.Taskbar`.

### Platform Support

| Feature | Windows | macOS | Linux |
|---------|---------|-------|-------|
| Progress states | Yes | Yes | Desktop-dependent |
| Flash notifications | Yes | Yes | Desktop-dependent |
| Capability detection | Yes | Yes | Yes |

**Platform details:**
- **Windows:** Full support via `ITaskbarList3` COM and `FlashWindowEx`
- **macOS:** Progress via dock tile badge label, flash via `NSApp.requestUserAttention`
- **Linux:** D-Bus StatusNotifierItem + UnityLauncherEntry (GNOME may report `IsSupported=false`)

### Progress Indicator

```csharp
// Show download progress (current, total)
window.Features.Taskbar.SetProgress(TaskbarProgressState.Normal, 75, 100);

// Show indeterminate progress
window.Features.Taskbar.SetProgress(TaskbarProgressState.Indeterminate, 0, 0);

// Show error state
window.Features.Taskbar.SetProgress(TaskbarProgressState.Error, 0, 0);

// Clear progress
window.Features.Taskbar.ClearProgress();
```

### Flash Notifications

```csharp
// Flash all windows continuously (count 0 = until user interacts)
window.Features.Taskbar.SetFlash(TaskbarFlashMode.All, 0);

// Flash timer-based
window.Features.Taskbar.SetFlash(TaskbarFlashMode.Timer, 3);

// Stop flashing
window.Features.Taskbar.StopFlash();
```

### Capability Detection

```csharp
InfiniFrameTaskbarCapabilities caps = window.Features.Taskbar.Capabilities;

if (caps.SupportsProgress) {
    window.Features.Taskbar.SetProgress(TaskbarProgressState.Normal, 50, 100);
} else {
    // Fallback to in-app progress indicator
    Console.WriteLine("Taskbar progress not supported on this platform");
}
```

## Examples

- `InfiniFrameExample.WebApp.React` (`examples/InfiniFrameExample.WebApp.React`) - custom URL scheme handler and web messaging with DI-resolved services
- `InfiniFrameExample.BlazorWebView` (`examples/InfiniFrameExample.BlazorWebView`) - window builder configuration with size, position, and icon
- `InfiniFrameExample.SingleFileExe` (`examples/InfiniFrameExample.SingleFileExe`) - embedded static assets and single-file native bootstrap
