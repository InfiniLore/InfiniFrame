# Browser Feature

The Browser feature controls the web browser engine settings: context menu, media autoplay, web security, clipboard access, user agent, status bar, and more. It is available at build time for initial configuration and at runtime for live changes.

## Contents

- [Builder Configuration](#builder-configuration)
- [Runtime Control](#runtime-control)
- [Context Menu](#context-menu)
- [Media and Permissions](#media-and-permissions)
- [Web Security](#web-security)
- [User Agent](#user-agent)
- [Status Bar](#status-bar)
- [Certificate Error Handling](#certificate-error-handling)
- [Browser Shortcuts](#browser-shortcuts)
- [Platform-Specific Browser Parameters](#platform-specific-browser-parameters)

## Builder Configuration

```csharp
var builder = InfiniFrameWindowBuilder.Create()
    .EnableContextMenu(false)
    .EnableZoom(false)
    .EnableMediaAutoplay(true)
    .EnableFileSystemAccess(true)
    .EnableWebSecurity(false)
    .EnableJavascriptClipboardAccess(true)
    .EnableMediaStream(true)
    .EnableSmoothScrolling(true)
    .EnableIgnoreCertificateErrors()
    .EnableStatusBar(false)
    .EnableBrowserShortcuts(false)
    .SetUserAgent("MyApp/1.0")
    .SetStartPageUrl("https://myapp.local");
```

| Method | Description |
|--------|-------------|
| `EnableContextMenu(bool)` | Show/hide right-click context menu |
| `EnableMediaAutoplay(bool)` | Allow/block media autoplay |
| `EnableFileSystemAccess(bool)` | Enable/disable file system access API |
| `EnableWebSecurity(bool)` | Toggle browser-level web security |
| `EnableJavascriptClipboardAccess(bool)` | Allow JS clipboard access |
| `EnableMediaStream(bool)` | Allow camera/microphone access |
| `EnableSmoothScrolling(bool)` | Enable smooth scrolling |
| `EnableIgnoreCertificateErrors(bool)` | Ignore SSL/TLS certificate errors |
| `EnableStatusBar(bool)` | Show/hide URL hover status indicator |
| `EnableBrowserShortcuts(bool)` | Enable/disable browser keyboard shortcuts |
| `SetUserAgent(string?)` | Set a custom user agent string |
| `SetBrowserControlInitParameters(string?)` | Pass raw flags to the browser engine |
| `SetTemporaryFilesPath(string)` | Set browser cache directory |
| `SetWebView2RuntimePath(string)` | Set bundled WebView2 runtime path (Windows only) |

## Runtime Control

After `Build()`, modify browser settings through extension methods or the feature interface:

```csharp
// Extension methods
window.EnableContextMenu(false);
window.EnableStatusBar(false);
window.EnableBrowserShortcuts(true);

// Direct feature access
window.Features.Browser.EnableContextMenu(false);
window.Features.Browser.EnableStatusBar(false);

// Read current values
bool contextMenuEnabled = window.Features.Browser.IsContextMenuEnabled;
string? userAgent = window.Features.Browser.UserAgent;
bool statusBarEnabled = window.Features.Browser.IsStatusBarEnabled;
```

## Context Menu

Control whether the right-click context menu appears in the browser:

```csharp
// Builder
builder.SetContextMenuEnabled(false);

// Runtime
window.Features.Browser.EnableContextMenu(false);
bool enabled = window.Features.Browser.IsContextMenuEnabled;
```

## Media and Permissions

### Media autoplay

```csharp
builder.SetMediaAutoplayEnabled(true);
window.Features.Browser.EnableMediaAutoplay(true);
```

### Camera and microphone

```csharp
builder
    .SetMediaStreamEnabled(true)    // Allow camera/microphone
    .GrantBrowserPermissions();     // Auto-grant permissions (Windows only)
```

### File system access

```csharp
builder.SetFileSystemAccessEnabled(true);
```

### Clipboard access

Allow JavaScript to read/write the clipboard:

```csharp
builder.SetJavascriptClipboardAccessEnabled(true);
```

## Web Security

Toggle browser-level web security (same-origin policy, CORS checks):

```csharp
builder.SetWebSecurityEnabled(false); // Disable (not recommended in production)
window.Features.Browser.EnableWebSecurity(true); // Re-enable at runtime
```

:::warning
Disabling web security disables same-origin policy and CORS checks. Only do this in development or for fully trusted local content. See also [Trusted Origins](#uri-security-policy-trusted-origins) for a more targeted approach.
:::

### URI Security Policy (Trusted Origins)

InfiniFrame validates URI origins independently from browser `WebSecurity` toggles. For embedded apps (including BlazorWebView), trust external module/CDN origins explicitly:

```csharp
builder
    .AddTrustedOrigin("https://xyz")
    .AddTrustedOrigin("https://cdn.jsdelivr.net")
    .AddTrustedOrigin("https://unpkg.com");
```

To replace the trusted list entirely:

```csharp
builder.SetTrustedOrigins("https://xyz", "https://cdn.jsdelivr.net");
```

To trust all origins (high risk, not recommended in production):

```csharp
builder.SetTrustAllOrigins(true);
```

## User Agent

Set a custom user agent string to identify your application:

```csharp
builder.SetUserAgent("MyApp/1.0");
window.Features.Browser.SetUserAgent("MyApp/2.0");
string? ua = window.Features.Browser.UserAgent;
```

## Status Bar

`EnableStatusBar(bool)` controls whether the URL hover status indicator (status bar) is shown at the bottom-left of the browser window when hovering over a hyperlink.

- Default: `true` (status bar shown)
- Platform support: **Windows only** (maps to `ICoreWebView2Settings.IsStatusBarEnabled`)
- Linux/macOS: The flag is accepted and stored, but has no native effect

```csharp
builder.EnableStatusBar(false);                    // fluent extension
window.Features.Browser.EnableStatusBar(false);    // direct
bool enabled = window.Features.Browser.IsStatusBarEnabled; // default: true
```

## Certificate Error Handling

`EnableIgnoreCertificateErrors(bool)` controls whether SSL/TLS certificate errors are ignored by the browser engine.

:::warning
Enabling this feature bypasses SSL/TLS certificate validation. Only use in controlled development/test scenarios. Never enable in production applications handling sensitive data.
:::

- This is a **startup-only** configuration and cannot be changed at runtime.
- The builder default is `true`; the native layer default is `false`.
- Platform-specific behavior:
  - **Windows**: Passes `--ignore-certificate-errors` Chromium flag to WebView2
  - **Linux**: Sets `WEBKIT_TLS_ERRORS_POLICY_IGNORE` on WebKit data manager
  - **macOS**: Trusts all server certificates in `didReceiveAuthenticationChallenge:` delegate

## Browser Shortcuts

Control whether browser keyboard shortcuts (Ctrl+T, Ctrl+W, F5, etc.) are enabled:

```csharp
builder.EnableBrowserShortcuts(false);              // Builder
window.Features.Browser.EnableBrowserShortcuts(true); // Runtime
bool enabled = window.Features.Browser.IsBrowserShortcutsEnabled;
```

## Platform-Specific Browser Parameters

The `SetBrowserControlInitParameters` method passes raw flags to the underlying browser engine:

```csharp
// Windows: space-separated Chromium flags
builder.SetBrowserControlInitParameters("--disable-gpu --no-sandbox")

// Linux: JSON object matching WebKit2GTK settings
builder.SetBrowserControlInitParameters("{ \"enable_developer_extras\": true }")

// macOS: JSON object matching WKPreferences keys
builder.SetBrowserControlInitParameters("{ \"minimumFontSize\": 12 }")
```

For remote debugging, prefer `SetRemoteDebuggingPort(...)` over raw flags.

### WebView2 Runtime Path (Windows)

For a bundled fixed-version WebView2 runtime on Windows, set its extracted directory on the builder before `Build()`:

```csharp
builder.SetWebView2RuntimePath(Path.Combine(AppContext.BaseDirectory, "WebView2Runtime"));
```

The path applies only to that window. It is ignored on Linux and macOS.

### Clearing Auto-fill Data

```csharp
window.Features.Browser.ClearBrowserAutoFill();
```

## See Also

- [Debugging Feature](debugging-feature.md) DevTools, remote debugging, and diagnostics
- [Window Features Architecture](window-features-architecture.md) How the feature system works
- [Core Window Guide](core-window.md) Builder API and feature overview
