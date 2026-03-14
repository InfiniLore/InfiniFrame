# Builder API Reference

Complete reference for all fluent configuration methods on `IInfiniFrameWindowBuilder`

All methods return the builder instance for chaining and must be called before `Build()`

Namespace: `InfiniFrame`
Package: `InfiniLore.InfiniFrame` (extensions defined in `InfiniLore.InfiniFrame.Shared`)

## Contents

- [Window Appearance](#window-appearance)
- [Size](#size)
- [Position](#position)
- [Window State](#window-state)
- [Content](#content)
- [Browser Features](#browser-features)
- [Notifications (Windows only)](#notifications-windows-only)
- [Platform-Specific](#platform-specific)
- [Custom Schemes](#custom-schemes)
- [Event Registration Shortcuts](#event-registration-shortcuts)

## Window Appearance

### `SetTitle(string? title)`
Sets the window title shown in the native title bar and taskbar

```csharp
builder.SetTitle("My Application")
```

### `SetIconFile(string? iconFilePath)`
Sets the window icon — Windows and Linux only
Accepts `.ico` on Windows, `.png` on Linux

```csharp
builder.SetIconFile("assets/icon.ico")
```

Invalid paths are silently ignored

## Size

### `SetSize(int width, int height)`
### `SetSize(Size size)`
Sets the initial window size in pixels
Disables `UseOsDefaultSize` and `Center`

### `SetWidth(int value)`
### `SetHeight(int value)`
Sets width or height independently

### `SetMinSize(int width, int height)`
### `SetMinSize(Size minSize)`
Sets the minimum window size — the user cannot resize below this

### `SetMinWidth(int value)`
### `SetMinHeight(int value)`
Sets minimum width or height independently

### `SetMaxSize(int width, int height)`
### `SetMaxSize(Size size)`
Sets the maximum window size — the user cannot resize above this

### `SetMaxWidth(int value)`
### `SetMaxHeight(int value)`
Sets maximum width or height independently

### `SetUseOsDefaultSize(bool useOsDefaultSize)`
When `true`, the OS chooses the initial window size (overrides `SetSize`)

## Position

### `SetLocation(int left, int top)`
### `SetLocation(Point location)`
Sets the window's initial screen position in pixels from the top-left corner of the primary monitor
Disables `UseOsDefaultLocation` and `Center`

### `SetLeft(int left)`
### `SetTop(int top)`
Sets left or top position independently

### `Center(bool enable = true)`
Centers the window on the primary monitor at startup

### `SetUseOsDefaultLocation(bool useOsDefaultLocation)`
When `true`, the OS positions the window (overrides `SetLocation`)

## Window State

### `SetMaximized(bool maximized)`
Opens the window in a maximized state

### `SetMinimized(bool minimized)`
Opens the window in a minimized state

### `SetFullScreen(bool fullscreen)`
Opens the window in fullscreen mode

### `SetResizable(bool resizable)`
Controls whether the user can resize the window by dragging its borders

### `SetTopMost(bool topmost)`
Keeps the window above all other windows that are not also top-most

### `SetChromeless(bool chromeless)`
Removes the native title bar and window borders
On Windows, also disables `UseOsDefaultLocation`, `UseOsDefaultSize`, and `Resizable` automatically

### `SetTransparent(bool transparent)`
Enables window-level transparency — the background of the WebView can be transparent
Typically combined with `SetChromeless` for custom-shaped windows

## Content

### `SetStartUrl(string? url)`
### `SetStartUrl(Uri? url)`
Sets the URL to navigate to when the window opens

```csharp
builder.SetStartUrl("https://example.com")
builder.SetStartUrl(new Uri("app://localhost/index.html"))
```

### `SetStartString(string? startString)`
Renders an HTML string directly in the browser control instead of navigating to a URL

```csharp
builder.SetStartString("<html><body><h1>Hello</h1></body></html>")
```

## Browser Features

### `SetDevToolsEnabled(bool enabled)`
Enables the browser's developer tools (F12 / right-click Inspect)

### `SetContextMenuEnabled(bool enabled)`
Enables or disables the right-click context menu

### `SetZoomEnabled(bool zoomEnabled)`
Enables or disables pinch-to-zoom and Ctrl+scroll zoom

### `SetZoom(int zoom)`
Sets the initial zoom level (100 = default, 150 = 150%)

### `SetUserAgent(string userAgent)`
Overrides the browser's user agent string

### `SetMediaAutoplayEnabled(bool enable)`
Allows media elements to autoplay without user interaction

### `SetFileSystemAccessEnabled(bool enable)`
Enables the [File System Access API](https://developer.mozilla.org/en-US/docs/Web/API/File_System_API) in JavaScript

### `SetWebSecurityEnabled(bool enable)`
Enables or disables CORS and other web security policies
Disabling this is useful for loading local files during development — use with caution in production

### `SetJavascriptClipboardAccessEnabled(bool enable)`
Allows JavaScript to read from and write to the clipboard without a permission prompt

### `SetMediaStreamEnabled(bool enable)`
Enables camera and microphone access via `getUserMedia`

### `SetSmoothScrollingEnabled(bool enable = true)`
Enables CSS smooth scrolling behavior in the browser control

### `SetIgnoreCertificateErrorsEnabled(bool enable = true)`
Ignores TLS certificate errors — useful for self-signed certificates in development

## Notifications (Windows only)

### `SetNotificationsEnabled(bool enable = true)`
Enables web push notifications
Windows only — throws `ApplicationException` on other platforms

### `SetNotificationRegistrationId(string? id)`
Sets the notification registration ID used by the Windows notification system
Windows only — throws `ApplicationException` on other platforms

### `GrantBrowserPermissions(bool enable = true)`
Automatically grants all browser permission requests (camera, microphone, location, etc.) without showing a prompt
Windows only

## Platform-Specific

### `SetTemporaryFilesPath(string? path)`
Sets the directory used by the browser engine for its cache and temporary data
Windows only

### `SetBrowserControlInitParameters(string? parameters)`
Passes raw configuration parameters to the underlying browser engine at initialization

| Platform | Format | Documentation |
|----------|--------|---------------|
| Windows | Space-separated Chromium flags | [Chromium switches](https://peter.sh/experiments/chromium-command-line-switches/) |
| Linux | JSON object for WebKit2GTK settings | [WebKitSettings](https://webkitgtk.org/reference/webkit2gtk/2.5.1/WebKitSettings.html) |
| macOS | JSON object for WKPreferences | [WKPreferences](https://developer.apple.com/documentation/webkit/wkpreferences) |

```csharp
// Windows
builder.SetBrowserControlInitParameters("--disable-web-security --allow-file-access-from-files")

// Linux
builder.SetBrowserControlInitParameters("{ \"enable_developer_extras\": true }")

// macOS
builder.SetBrowserControlInitParameters("{ \"minimumFontSize\": 12 }")
```

## Custom Schemes

### `RegisterCustomSchemeHandler(string scheme, NetCustomSchemeDelegate handler)`
Intercepts requests for a custom URL scheme and serves content from C#

```csharp
builder.RegisterCustomSchemeHandler("app", (sender, scheme, url, out string? contentType) => {
    contentType = "text/html";
    return new MemoryStream(Encoding.UTF8.GetBytes("<h1>Hello</h1>"));
});
```

- Scheme names are lowercased automatically
- Up to 16 handlers can be registered before `Build()` — additional ones can be added at runtime via `window.RegisterCustomSchemeHandler(...)`
- Throws `ArgumentException` if the scheme is empty or the handler is null
- Throws `ApplicationException` if more than 16 unique schemes are registered pre-build

## Event Registration Shortcuts

The builder also exposes shorthand methods from `IInfiniFrameWindowBuilder` for registering events:

```csharp
builder
    .RegisterWindowClosingHandler((window, cancel) => false)
    .RegisterWindowClosingRequestedHandler(window => false)
```

For the full event API, see the [Events Reference](Events.md)
