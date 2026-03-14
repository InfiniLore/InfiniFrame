# Types Reference

All shared types, enums, value types, and delegates used across InfiniFrame

Namespace: `InfiniFrame`
Package: `InfiniLore.InfiniFrame.Shared`

## Contents

- [Value Types](#value-types)
- [Enums](#enums)
  - [InfiniFrameDialogButtons](#infiniframedialogbuttons)
  - [InfiniFrameDialogIcon](#infiniframedialogicon)
  - [InfiniFrameDialogResult](#infiniframedialogresult)
  - [ResizeOrigin](#resizeorigin)
  - [WindowAction](#windowaction)
- [Delegates](#delegates)
- [Interfaces (Shared)](#interfaces-shared)
- [Configuration Binding](#configuration-binding)



## Value Types

### `InfiniMonitor`

Represents a single physical display

```csharp
public readonly record struct InfiniMonitor(
    Rectangle MonitorArea,
    Rectangle WorkArea,
    double Scale
);
```

| Property | Type | Description |
|----------|------|-------------|
| `MonitorArea` | `Rectangle` | Full bounds of the monitor in screen coordinates, including any taskbar |
| `WorkArea` | `Rectangle` | Usable area of the monitor, excluding taskbars and docked toolbars |
| `Scale` | `double` | DPI scaling factor (1.0 = 100%, 1.5 = 150%, 2.0 = 200%) |

```csharp
foreach (InfiniMonitor monitor in window.Monitors) {
    Console.WriteLine($"Resolution: {monitor.MonitorArea.Width}×{monitor.MonitorArea.Height}");
    Console.WriteLine($"Scale: {monitor.Scale * 100}%");
    Console.WriteLine($"Work area: {monitor.WorkArea}");
}
```



## Enums

### `InfiniFrameDialogButtons`

Controls which buttons appear in a native message dialog (used with `ShowMessage`)

| Value | Buttons shown |
|-------|--------------|
| `Ok` | OK |
| `OkCancel` | OK, Cancel |
| `YesNo` | Yes, No |
| `YesNoCancel` | Yes, No, Cancel |
| `RetryCancel` | Retry, Cancel |
| `AbortRetryIgnore` | Abort, Retry, Ignore |

```csharp
window.ShowMessage("Title", "Message", InfiniFrameDialogButtons.YesNo, InfiniFrameDialogIcon.Question);
```

### `InfiniFrameDialogIcon`

The icon shown in a native message dialog

| Value | Icon |
|-------|------|
| `Info` | Information (i) |
| `Warning` | Warning (!) |
| `Error` | Error (x) |
| `Question` | Question (?) |

### `InfiniFrameDialogResult`

The button the user pressed in a message dialog

| Value | Numeric | Meaning |
|-------|---------|---------|
| `Cancel` | `-1` | Dialog was cancelled (e.g. closed with Escape) |
| `Ok` | `0` | OK button |
| `Yes` | `1` | Yes button |
| `No` | `2` | No button |
| `Abort` | `3` | Abort button |
| `Retry` | `4` | Retry button |
| `Ignore` | `5` | Ignore button |

```csharp
var result = window.ShowMessage(...);
if (result == InfiniFrameDialogResult.Yes) { ... }
```

### `ResizeOrigin`

Identifies which edge or corner a resize operation originates from
Used internally by `InfiniFrameWindowResizeThumb`

| Value | Description |
|-------|-------------|
| `Top` | Top edge |
| `Bottom` | Bottom edge |
| `Left` | Left edge |
| `Right` | Right edge |
| `TopLeft` | Top-left corner |
| `TopRight` | Top-right corner |
| `BottomLeft` | Bottom-left corner |
| `BottomRight` | Bottom-right corner |

### `WindowAction`

Used by `InfiniFrameWindowButton` to specify the action it performs

| Value | Description |
|-------|-------------|
| `Minimize` | Minimize the window to the taskbar |
| `Maximize` | Maximize or restore the window |
| `Close` | Close the window |

Namespace: `InfiniFrame.Blazor`
Package: `InfiniLore.InfiniFrame.Blazor`



## Delegates

### `NetCustomSchemeDelegate`

Handler signature for custom URL scheme callbacks

```csharp
public delegate Stream? NetCustomSchemeDelegate(
    object sender,
    string scheme,
    string url,
    out string? contentType
);
```

| Parameter | Description |
|-----------|-------------|
| `sender` | The window that received the request |
| `scheme` | The scheme name (e.g. `"app"`) |
| `url` | The full URL being requested (e.g. `"app://localhost/data.json"`) |
| `contentType` | Output — set this to the MIME type of the returned content (e.g. `"text/html"`) |
| Return | A `Stream` containing the response body, or `null` to respond with an error |

```csharp
Stream? HandleAppScheme(object sender, string scheme, string url, out string? contentType) {
    contentType = "application/json";
    var json = JsonSerializer.SerializeToUtf8Bytes(new { status = "ok" });
    return new MemoryStream(json);
}
```



## Interfaces (Shared)

### `IHasInfiniFrameProperties`

Base interface for objects that expose standard window properties — implemented by `IInfiniFrameWindow`

### `IHasInfiniFrameEvents`

Base interface for objects that expose the standard window event set — implemented by `IInfiniFrameWindow`



## Configuration Binding

`InfiniFrameWindowConfiguration` implements `IInfiniFrameWindowConfiguration` and is the concrete configuration object populated by the builder
It can also be bound from `appsettings.json` when a `ServiceProvider` is passed to `Build()`:

```json
{
  "InfiniFrame": {
    "Title": "My App",
    "Width": 1280,
    "Height": 720,
    "Centered": true,
    "DevToolsEnabled": false,
    "Resizable": true
  }
}
```

All `IInfiniFrameWindowConfiguration` properties map directly to builder extension method names (camelCase → PascalCase), e.g. `SetTitle("...")` writes to `Configuration.Title`
