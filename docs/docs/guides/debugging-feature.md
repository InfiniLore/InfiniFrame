# Debugging Feature

The Debugging feature controls developer tools access, remote debugging endpoints, and runtime diagnostics. It is available at build time for initial configuration and at runtime for diagnostics and probing.

## Contents

- [Builder Configuration](#builder-configuration)
- [Runtime Diagnostics](#runtime-diagnostics)
- [DevTools](#devtools)
- [Remote Debugging](#remote-debugging)
- [Web Inspector (macOS)](#web-inspector-macos)
- [Debug Tooling Matrix](#debug-tooling-matrix)
- [Precedence with Raw Browser Arguments](#precedence-with-raw-browser-arguments)
- [Security and Networking](#security-and-networking)

## Builder Configuration

```csharp
var window = InfiniFrameWindowBuilder.Create()
    .SetTitle("Debuggable App")
    .SetStartPageUrl("https://example.com")
    .SetDevToolsEnabled(true)          // local inspector
    .SetWebInspectorEnabled(true)      // macOS 13.3+ Safari Web Inspector
    .SetRemoteDebuggingPort(9222)      // remote endpoint (Windows and Linux)
    .Build();
```

| Method | Description |
|--------|-------------|
| `SetDevToolsEnabled(bool)` | Enable/disable local dev tools access |
| `SetRemoteDebuggingPort(int)` | Configure a loopback TCP debug endpoint |
| `SetWebInspectorEnabled(bool)` | Enable Safari Web Inspector attachability (macOS 13.3+) |

## Runtime Diagnostics

After `Build()`, access debug information through `window.Debugging`:

```csharp
// Capabilities
InfiniFrameDebugCapabilities caps = window.Debugging.Capabilities;

// Diagnostics snapshot
InfiniFrameDebugDiagnostics diag = window.Debugging.GetDiagnostics();

// Event stream (best effort)
window.Debugging.Event += (_, e) => {
    Console.WriteLine($"[{e.TimestampUtc:O}] {e.Kind} {e.Level} {e.Message}");
};
```

### Endpoint probing

```csharp
if (window.Debugging.TryProbeEndpoint(out Uri? endpoint, out string? reason)) {
    Console.WriteLine($"Endpoint ready: {endpoint}");
} else {
    Console.WriteLine($"Endpoint unavailable: {reason}");
}
```

### Getting the remote debugging endpoint

```csharp
if (window.Debugging.TryGetRemoteDebuggingEndpoint(out Uri? endpoint))
    Console.WriteLine(endpoint);
```

## DevTools

`SetDevToolsEnabled(bool)` controls local in-window inspector/devtools access (F12 or right-click Inspect).

- Default: `false`
- Runtime: `window.Features.Debugging.EnableDevTools(bool)` or `window.Debug.EnableDevTools(bool)`

## Remote Debugging

`SetRemoteDebuggingPort(int? port)` configures a loopback TCP debug endpoint at startup.

### Contract

- Port range: `1..65535`
- `0` or `null`: disable remote debugging
- Invalid ports throw `ArgumentOutOfRangeException`
- `SetRemoteDebuggingPort(int port)` is startup-only; configure with `builder.SetRemoteDebuggingPort(...)` before `Build()`
- `window.Debugging.RemoteDebuggingPort` remains stable after startup; after close, `TryGetRemoteDebuggingEndpoint(out _)` returns `false` with `null` endpoint

### Platform behavior

| Platform | `SetDevToolsEnabled` | `SetRemoteDebuggingPort` |
|----------|----------------------|--------------------------|
| Windows (WebView2) | Supported | Supported |
| Linux (WebKitGTK) | Supported | Supported |
| macOS (WKWebView) | Supported | Not supported (throws when enabled) |

- Use `window.Debugging.SupportsRemoteDebugging` to query support before enabling
- On unsupported platforms, `TryGetRemoteDebuggingEndpoint(out _)` throws `PlatformNotSupportedException`

### Linux specifics (WebKitGTK)

- Remote debugging is configured before WebKit context/webview creation for deterministic startup
- Uses WebKitGTK inspector server environment variables (`WEBKIT_INSPECTOR_SERVER` and `WEBKIT_INSPECTOR_HTTP_SERVER`)
- Inspector endpoint is process-scoped (all windows in the same process share the same configuration)
- WebKit requires developer extras for remote inspector; InfiniFrame keeps that capability active while remote debugging is enabled

## Web Inspector (macOS)

`SetWebInspectorEnabled(bool)` enables Safari Web Inspector attachability on macOS 13.3+.

- This is **startup-only**; calling `window.Debugging.EnableWebInspectorEnabled(...)` after `Build()` throws `InvalidOperationException`
- Platform support: macOS only. Throws `PlatformNotSupportedException` on Windows and Linux.

| Platform | `SetWebInspectorEnabled` |
|----------|--------------------------|
| Windows (WebView2) | Not supported (throws when enabled) |
| Linux (WebKitGTK) | Not supported (throws when enabled) |
| macOS (WKWebView) | Supported on macOS 13.3+ |

## Debug Tooling Matrix

| Capability | Windows (WebView2) | Linux (WebKitGTK) | macOS (WKWebView) |
|------------|-------------------|-------------------|-------------------|
| Local DevTools toggle | Yes | Yes | Yes |
| Remote debugging endpoint | Yes | Yes | No |
| Web Inspector attach mode | No | No | Yes (macOS 13.3+) |
| Script error forwarding | Yes (navigation failure mapped) | Yes | Yes |
| Navigation diagnostics | Yes | Yes | Yes |

### Guarantees vs best effort

- Capability fields are deterministic and safe to branch on
- Endpoint probing is bounded and loopback-only by design
- Debug events are best effort and platform-dependent; InfiniFrame does not emulate missing native signals
- Linux inspector endpoint is process-scoped (WebKitGTK behavior), not window-scoped
- macOS inspector mode is Safari attachability, not a TCP remote debugging endpoint

## Precedence with Raw Browser Arguments

`SetRemoteDebuggingPort(...)` is authoritative.
If `SetBrowserControlInitParameters(...)` contains `--remote-debugging-port=...` or `--remote-debugging-address=...`, those switches are stripped and replaced by the explicit API value.

## Security and Networking

- InfiniFrame binds remote debugging to loopback (`127.0.0.1`) when enabled
- It does not intentionally expose externally reachable debug endpoints
- Startup validates port availability and throws actionable `InvalidOperationException` when the port is unavailable
- Windows WebView2 and Linux inspector endpoints are exposed as `http://127.0.0.1:<port>/`

## See Also

- [Browser Feature](browser-feature.md) Browser engine settings and web security
- [Window Features Architecture](window-features-architecture.md) How the feature system works
- [Core Window Guide](core-window.md) Builder API and feature overview
