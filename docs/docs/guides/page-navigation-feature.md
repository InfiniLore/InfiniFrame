# Page Navigation Feature

The Page Navigation feature controls loading URLs, HTML strings, and intercepting navigation requests. It is available at build time for the initial page and at runtime for programmatic navigation.

## Contents

- [Builder Configuration](#builder-configuration)
- [Runtime Navigation](#runtime-navigation)
- [Current URL](#current-url)
- [Navigation Interception](#navigation-interception)
- [Custom URL Schemes](#custom-url-schemes)
- [Navigation Result](#navigation-result)

## Builder Configuration

Set the initial page content before `Build()`:

```csharp
var builder = InfiniFrameWindowBuilder.Create()
    .SetStartPageUrl("https://example.com")             // Load a URL as the initial page
    .SetStartPageUrl(new Uri("https://example.com"))    // Load a URI as the initial page
    .SetStartPageContent("<html><body>Hello</body></html>")  // Render HTML directly
```

| Method | Description |
|--------|-------------|
| `SetStartPageUrl(string?)` | Load a URL string as the initial page |
| `SetUrl(Uri?)` | Load a URI as the initial page |
| `SetStartPageContent(string?)` | Render raw HTML as the initial page |

`SetStartPageUrl` and `SetStartPageContent` are mutually exclusive; the last one set wins.

## Runtime Navigation

After `Build()`, navigate to new pages through the feature interface or extension methods:

```csharp
// Load a URL
window.Features.PageNavigation.Load("https://example.com");
await window.Features.PageNavigation.LoadAsync("https://example.com");

// Load a Uri
window.Features.PageNavigation.Load(new Uri("https://example.com"));

// Load raw HTML
window.Features.PageNavigation.LoadRawString("<h1>Hello</h1>");
await window.Features.PageNavigation.LoadRawStringAsync("<h1>Hello</h1>");

// Extension methods
window.Load("https://example.com");
window.LoadRawString("<h1>Hello</h1>");
```

### Safe navigation

`TryLoadUri` and `TryLoadPath` return `true` if the navigation was initiated successfully, `false` otherwise:

```csharp
bool loaded = window.Features.PageNavigation.TryLoadUri(new Uri("https://example.com"));
if (!loaded) {
    Console.WriteLine("Navigation failed to start");
}
```

## Current URL

Read the current page URL:

```csharp
string? url  = window.Features.PageNavigation.GetCurrentUrl();
Uri? uri     = window.Features.PageNavigation.GetCurrentUri();

// Extension methods
string? url2 = window.GetCurrentUrl();
Uri? uri2    = window.GetCurrentUri();
```

`GetCurrentUrl` returns the active top-level URL after any redirects. It is `null` when the window has loaded raw HTML via `LoadRawString` because there is no associated URL.

## Navigation Interception

Inspect and cancel navigation requests before they are committed by the browser engine:

```csharp
window.RegisterNavigationStartingHandler((window, args) => {
    Console.WriteLine($"Navigation to {args.Url} (userInitiated={args.IsUserInitiated})");

    // Block navigations to external origins
    if (!args.Url.StartsWith("app://"))
        return NavigationStartingResult.Cancel;

    return NavigationStartingResult.Allow;
});
```

### NavigationStartingEventArgs

| Property | Type | Description |
|----------|------|-------------|
| `Url` | `string` | The target URL |
| `IsUserInitiated` | `bool` | `true` for link clicks and form submissions |
| `IsRedirect` | `bool` | `true` for server redirects |
| `IsMainFrame` | `bool` | `true` for main frame navigations |

### Platform notes

- **Windows (WebView2):** Uses `ICoreWebView2NavigationStartingEventArgs`. `IsMainFrame` is always `true` because WebView2's `NavigationStartingEventArgs` does not expose this flag. `IsRedirect` maps to the `IsRedirected` property.
- **macOS (WKWebView):** Uses `WKNavigationDelegate.decidePolicyForNavigationAction:`. `IsUserInitiated` is `true` for `WKNavigationTypeLinkActivated` and `WKNavigationTypeFormSubmitted`.
- **Linux (WebKitGTK):** Uses the `decide-policy` signal. `IsMainFrame` is always `true` because WebKitGTK's `decide-policy` with `WEBKIT_POLICY_DECISION_TYPE_NAVIGATION_ACTION` only fires for main frame navigations.

## Custom URL Schemes

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

## Navigation Result

`NavigationResult` provides information about the outcome of a navigation operation:

```csharp
public record NavigationResult(
    ulong OperationId,
    NavigationStatus Status,
    Uri? Uri,
    int NativeErrorCode,
    string? FailureReason
);
```

### NavigationStatus

| Value | Description |
|-------|-------------|
| `Succeeded` | Navigation completed successfully |
| `Failed` | Navigation failed (check `FailureReason`) |
| `Superseded` | A new navigation replaced this one before it completed |
| `WindowClosed` | The window was closed during navigation |

## See Also

- [JavaScript Interop](javascript-interop.md) — Two-way C#/JS messaging
- [Window Features Architecture](window-features-architecture.md) — How the feature system works
- [Core Window Guide](core-window.md) — Builder API and feature overview
