# Window Features Architecture

This guide explains the feature system that underpins every InfiniFrame window. Understanding this architecture helps you navigate the API surface, know which methods are available at build time versus runtime, and find the right feature for any window capability.

## Contents

- [Overview](#overview)
- [Builder Features vs Runtime Features](#builder-features-vs-runtime-features)
- [Feature Index](#feature-index)
- [How Features Are Wired](#how-features-are-wired)
- [Extension Methods](#extension-methods)
- [Custom URL Scheme Handler](#custom-url-scheme-handler)
- [DI Container Integration](#di-container-integration)

## Overview

Every window capability in InfiniFrame is encapsulated into a **feature**. Features are self-contained modules that handle a specific aspect of the window, such as size, state, browser settings, or debugging.

This design gives you:

- **Discoverability** — browse `window.Features.<Name>` to find available capabilities
- **Consistency** — every feature follows the same builder/runtime pattern
- **Fluent API** — extension methods let you chain configuration on both builder and window
- **Testability** — features are interfaces, so they can be mocked in unit tests

## Builder Features vs Runtime Features

Most features have two halves:

### Builder Feature (`I<Name>InfiniFrameWindowBuilderFeature`)

Configures the feature **before** the window is created. Only available through `InfiniFrameWindowBuilder` and must be called before `Build()`.

```csharp
var builder = InfiniFrameWindowBuilder.Create();
builder.Features.Size.SetSize(1280, 720);  // Direct feature access
builder.SetSize(1280, 720);                // Extension method equivalent
```

Builder features apply their settings to `InfiniFrameNativeParameters`, which the native layer reads during window creation. After `Build()`, builder-only settings cannot be changed.

### Runtime Feature (`I<Name>InfiniFrameWindowFeature`)

Provides getters and setters for a live window. Available through `IInfiniFrameWindow.Features.<Name>` after `Build()`.

```csharp
var window = builder.Build();

// Direct feature access
window.Features.Size.SetSize(800, 600);
int width = window.Features.Size.Width;

// Extension method equivalent
window.SetSize(800, 600);
```

### Features with both halves

These features have a builder configuration phase **and** a runtime mutation phase:

| Feature | Builder | Runtime | Notes |
|---------|---------|---------|-------|
| [Browser](browser-feature.md) | Settings like user agent, web security, clipboard | Live getters/setters | Some settings are startup-only |
| [Debugging](debugging-feature.md) | DevTools, remote port, web inspector | Diagnostics, endpoint probe | Remote port is startup-only |
| [Decorations](decorations-feature.md) | Title, icon, chromeless, transparency | Live title/color changes | Chromeless is startup-only |
| [Menu](native-menu.md) | Initial menu bar | Runtime item manipulation | Full runtime control |
| [Notifications](notifications.md) | Enable, registration ID | Show notifications | Builder config required |
| [Page Navigation](page-navigation-feature.md) | Start URL/HTML | Runtime navigation | Full runtime control |
| [Position](position-feature.md) | Initial location, centering | Runtime move/center | Full runtime control |
| [Size](size-feature.md) | Initial size, min/max, resizable | Runtime resize | Full runtime control |
| [State](state-feature.md) | Initial state (maximized, fullscreen, etc.) | Runtime state changes | Full runtime control |
| [Taskbar](#taskbar-progress-and-flash) | (empty — pattern consistency) | Progress indicators, flash | Builder exists for pattern consistency |

### Builder-only features

These features are configured at build time only and have no runtime interface:

| Feature | What it configures |
|---------|-------------------|
| [Instance Arbitration](instance-arbitration.md) | Single-instance enforcement, mutex name |

### Runtime-only features

These features have no builder configuration and are only available after `Build()`:

| Feature | What it does |
|---------|-------------|
| [Drag Drop](drag-drop-feature.md) | File drop handling and extension filtering |
| [File Picker Dialogs](file-dialogs-feature.md) | Open/save file and folder dialogs |
| [Invoke](invoke-feature.md) | Cross-thread dispatch to the window's native thread |
| [JavaScript Execution](javascript-execution-feature.md) | Execute arbitrary JS in the browser control |
| [Lifecycle](lifecycle-feature.md) | Window close, ready wait, teardown |
| [Monitors](monitors-feature.md) | Query connected display information |
| [Web Messaging](javascript-interop.md) | Two-way C#/JS messaging |

## Feature Index

Every feature is accessed through `IInfiniFrameWindow.Features`:

```csharp
window.Features.Size         // ISizeInfiniFrameWindowFeature
window.Features.Position     // IPositionInfiniFrameWindowFeature
window.Features.State        // IStateInfiniFrameWindowFeature
window.Features.Decorations  // IDecorationsInfiniFrameWindowFeature
window.Features.Browser      // IBrowserInfiniFrameWindowFeature
window.Features.Debugging    // IDebuggingInfiniFrameWindowFeature
window.Features.PageNavigation // IPageNavigationInfiniFrameWindowFeature
window.Features.Menu         // IMenuInfiniFrameWindowFeature
window.Features.Notifications // INotificationsInfiniFrameWindowFeature
window.Features.Taskbar      // ITaskbarInfiniFrameWindowFeature
window.Features.DragDrop     // IDragDropInfiniFrameWindowFeature
window.Features.JavaScript   // IJavaScriptInfiniFrameWindowFeature
window.Features.Invoke       // IInvokeInfiniFrameWindowFeature
window.Features.Lifecycle    // ILifecycleInfiniFrameWindowFeature
window.Features.Monitors     // IMonitorsInfiniFrameWindowFeature
window.Features.FilePickerDialogs // IFilePickerDialogsInfiniFrameWindowFeature
window.Features.WebMessaging // IWebMessagingInfiniFrameWindowFeature
```

On the builder, features are accessed through `IInfiniFrameWindowBuilder.Features` (only features with builder halves are available):

```csharp
builder.Features.Size         // ISizeInfiniFrameWindowBuilderFeature
builder.Features.Position     // IPositionInfiniFrameWindowBuilderFeature
builder.Features.State        // IStateInfiniFrameWindowBuilderFeature
builder.Features.Decorations  // IDecorationsInfiniFrameWindowBuilderFeature
builder.Features.Browser      // IBrowserInfiniFrameWindowBuilderFeature
builder.Features.Debugging    // IDebuggingInfiniFrameWindowBuilderFeature
builder.Features.PageNavigation // IPageNavigationInfiniFrameWindowBuilderFeature
builder.Features.Menu         // IMenuInfiniFrameWindowBuilderFeature
builder.Features.Notifications // INotificationsInfiniFrameWindowBuilderFeature
builder.Features.InstanceArbitration // IInstanceArbitrationInfiniFrameWindowBuilderFeature
```

## How Features Are Wired

1. **Builder phase**: Each builder feature implements `IInfiniFrameWindowBuilderFeature` with an `ApplyToNativeParameters(ref InfiniFrameNativeParameters)` method. The builder collects all settings into a native parameters struct.

2. **Build**: `InfiniFrameWindowBuilder.Build()` creates the window, then `InfiniFrameWindowFeaturesFactory` creates all runtime feature instances from the DI container, passing the window and the original builder.

3. **Runtime**: Each runtime feature is a live object that wraps the native window handle. Extension methods on `IInfiniFrameWindow` delegate to the appropriate feature.

## Extension Methods

Every feature provides fluent extension methods on both `IInfiniFrameWindowBuilder` and `IInfiniFrameWindow`. These let you chain configuration without touching `Features` directly:

```csharp
// Builder — extension methods on IInfiniFrameWindowBuilder
var window = InfiniFrameWindowBuilder.Create()
    .SetTitle("My App")              // Decorations feature
    .SetSize(1280, 720)              // Size feature
    .Center()                        // Position feature
    .SetMaximized(true)              // State feature
    .SetDevToolsEnabled(true)        // Debugging feature
    .SetStartPageUrl("https://app.com")  // PageNavigation feature
    .Build();

// Runtime — extension methods on IInfiniFrameWindow
window.SetSize(800, 600);
window.SetTitle("New Title");
window.SetMaximized(false);
```

Extension methods are syntactic sugar over direct feature access. Both approaches are equivalent:

```csharp
// These are identical:
window.SetSize(800, 600);
window.Features.Size.SetSize(800, 600);
```

## Custom URL Scheme Handler

You can intercept requests for custom URL schemes (e.g. `app://`) and serve content from C# code. This is useful for loading local assets or implementing a virtual file system.

```csharp
builder.RegisterCustomSchemeHandler("app", (sender, scheme, url, out string? contentType) => {
    contentType = "text/html";
    var html = "<html><body>Hello from custom scheme</body></html>";
    return new MemoryStream(Encoding.UTF8.GetBytes(html));
});
```

- Up to 16 custom schemes can be registered before `Build()` is called.
- Additional handlers can be added after `Build()` via `window.RegisterCustomSchemeHandler(...)`.
- Scheme names are lowercased automatically.

### CORS and same-origin policy

Custom scheme responses automatically include CORS headers when the request originates from the same origin (same scheme, host, and port). This allows `fetch()` and `XMLHttpRequest` to work without disabling web security.

**Same-origin behavior** (e.g., `app://localhost` page fetching `app://localhost/data.json`):
- `Access-Control-Allow-Origin: app://localhost`
- `Access-Control-Allow-Credentials: true`
- `Vary: Origin`

**Cross-origin behavior** (e.g., `https://example.com` page fetching `app://localhost/data.json`):
- No CORS headers are added
- The browser engine may block the request entirely depending on web security settings

**Platform notes:**
- **Windows (WebView2):** CORS headers are built via `BuildCustomSchemeResponseHeaders` and set on the `ICoreWebView2WebResourceResponse`. The `app` scheme is registered with `TreatAsSecure(TRUE)` and `HasAuthorityComponent(TRUE)`.
- **Linux (WebKitGTK):** The `app` scheme is registered as CORS-enabled via `webkit_security_manager_register_uri_scheme_as_cors_enabled()`. WebKitGTK handles CORS header injection natively.
- **macOS (WKWebView):** CORS headers are built in the `UrlSchemeHandler` delegate using the same `IsSameOrigin` logic as Windows.

## DI Container Integration

When building with a `ServiceProvider`, the builder reads configuration from the `InfiniFrame` section automatically:

```csharp
// appsettings.json
{
  "InfiniFrame": {
    "Title": "My App",
    "Width": 1280,
    "Height": 720
  }
}
```

Pass the provider to `Build`:

```csharp
var window = builder.Build(serviceProvider);
```

`IInfiniFrameWindow` will then be resolvable from the container if registered.
