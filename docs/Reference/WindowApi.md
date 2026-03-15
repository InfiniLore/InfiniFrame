# Window API Reference

Full reference for `IInfiniFrameWindow` — the interface returned by `IInfiniFrameWindowBuilder.Build()`

Namespace: `InfiniFrame`
Package: `InfiniLore.InfiniFrame`

## Contents

- [Properties](#properties)
  - [Identity](#identity)
  - [Size and Position](#size-and-position)
  - [State](#state)
  - [Monitors](#monitors)
  - [Services](#services)
- [Methods](#methods)
  - [Lifecycle](#lifecycle)
  - [Web Messaging](#web-messaging)
  - [Dialogs](#dialogs)
  - [Notifications](#notifications)
  - [Custom Schemes](#custom-schemes)
- [IInfiniFrameWindowBuilder](#iinfiframewindowbuilder)
- [IInfiniFrameWindowMessageHandlers](#iinfiframewindowmessagehandlers)

## Properties

### Identity

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | Unique identifier for this window instance |
| `ManagedThreadId` | `int` | The managed thread ID of the window's UI thread |
| `WindowHandle` | `IntPtr` | Native OS window handle (HWND on Windows) |
| `InstanceHandle` | `IntPtr` | Native application instance handle |
| `NativeType` | `IntPtr` | Pointer to the underlying C++ window object |

### Size and Position

| Property | Type | Access | Description |
|----------|------|--------|-------------|
| `Size` | `Size` | get | Current window size in pixels |
| `Location` | `Point` | get | Current window position in screen coordinates |
| `MinSize` | `Size` | get, set | Minimum allowed window size |
| `MaxSize` | `Size` | get, set | Maximum allowed window size |

### State

| Property | Type | Description |
|----------|------|-------------|
| `Focused` | `bool` | Whether the window currently has keyboard focus |
| `ScreenDpi` | `uint` | Current screen DPI (e.g. 96 for 100%, 192 for 200%) |

### Monitors

| Property | Type | Description |
|----------|------|-------------|
| `Monitors` | `ImmutableArray<InfiniMonitor>` | All currently connected monitors |
| `MainMonitor` | `InfiniMonitor` | The monitor that contains the largest portion of the window |

### Services

| Property | Type | Description |
|----------|------|-------------|
| `ServiceProvider` | `IServiceProvider?` | The DI container, if the window was built with one |
| `Logger` | `ILogger<IInfiniFrameWindow>` | The window's logger instance |
| `MessageHandlers` | `IInfiniFrameWindowMessageHandlers` | Named web message routing |
| `Parent` | `IInfiniFrameWindow?` | Parent window, or `null` if this is a top-level window |

## Methods

### Lifecycle

#### `void WaitForClose()`
Blocks the calling thread until the native window is destroyed
Call this on the application's main thread to keep the process alive while the window is open

#### `Task WaitForCloseAsync()`
Asynchronous equivalent of `WaitForClose()`

#### `void Close()`
Requests the window to close
Fires `WindowClosingRequested` first; if not cancelled, fires `WindowClosing`, then destroys the window

#### `void Invoke(Action workItem)`
Marshals a delegate to the window's UI thread and executes it synchronously
Required for any window API calls made from a background thread

```csharp
Task.Run(() => {
    window.Invoke(() => window.Close());
});
```

### Web Messaging

#### `void SendWebMessage(string message)`
Sends a string message to JavaScript running in the browser control
The message is delivered via `window.external.receiveMessage` in the browser

#### `Task SendWebMessageAsync(string message)`
Async equivalent of `SendWebMessage`

### Dialogs

#### `InfiniFrameDialogResult ShowMessage(string title, string? text, InfiniFrameDialogButtons buttons, InfiniFrameDialogIcon icon)`
Shows a native OS message dialog and returns the button the user pressed

```csharp
var result = window.ShowMessage(
    "Confirm exit",
    "All unsaved changes will be lost",
    InfiniFrameDialogButtons.YesNo,
    InfiniFrameDialogIcon.Warning
);
```

#### `string?[] ShowOpenFile(string title, string? defaultPath, bool multiSelect, (string Name, string[] Extensions)[]? filters)`
Opens a native file picker and returns the selected file path(s)
Returns an empty array if the user cancels

```csharp
var files = window.ShowOpenFile(
    "Open document",
    null,
    multiSelect: false,
    filters: [("Documents", ["pdf", "docx"]), ("All Files", ["*"])]
);
```

#### `Task<string?[]> ShowOpenFileAsync(...)`
Async equivalent of `ShowOpenFile`

#### `string?[] ShowOpenFolder(string title, string? defaultPath, bool multiSelect)`
Opens a native folder picker

#### `Task<string?[]> ShowOpenFolderAsync(...)`
Async equivalent of `ShowOpenFolder`

#### `string? ShowSaveFile(string title, string? defaultPath, (string Name, string[] Extensions)[]? filters)`
Opens a native save file dialog and returns the chosen path, or `null` if cancelled

#### `Task<string?> ShowSaveFileAsync(...)`
Async equivalent of `ShowSaveFile`

### Notifications

#### `void SendNotification(string title, string body)`
Sends a native OS notification
Windows only — requires `SetNotificationsEnabled()` and `SetNotificationRegistrationId(...)` during configuration

### Custom Schemes

#### `IInfiniFrameWindow RegisterCustomSchemeHandler(string scheme, NetCustomSchemeDelegate handler)`
Registers a handler for a custom URL scheme after the window has been created
Returns `this` for chaining

```csharp
window.RegisterCustomSchemeHandler("data", (sender, scheme, url, out string? contentType) => {
    contentType = "application/json";
    return new MemoryStream(Encoding.UTF8.GetBytes("{\"ok\":true}"));
});
```

Up to 16 custom scheme handlers can be registered in total (including those set before `Build()`)

## IInfiniFrameWindowBuilder

`InfiniFrameWindowBuilder.Create()` returns an `IInfiniFrameWindowBuilder`

| Member | Type | Description |
|--------|------|-------------|
| `Configuration` | `IInfiniFrameWindowConfiguration` | All window configuration properties |
| `Events` | `IInfiniFrameWindowEvents` | Window lifecycle and input events |
| `MessageHandlers` | `IInfiniFrameWindowMessageHandlers` | Named web message handlers |
| `CustomSchemeHandlers` | `Dictionary<string, NetCustomSchemeDelegate?>` | Pre-registered custom schemes |
| `UseDefaultLogger` | `bool` | Whether to use the built-in console logger (default: `true`) |

#### `IInfiniFrameWindow Build(IServiceProvider? provider = null)`
Builds and opens the native window
Pass a `ServiceProvider` to enable DI integration and `appsettings.json` configuration binding

## IInfiniFrameWindowMessageHandlers

Manages routing of named web messages

```csharp
window.MessageHandlers.RegisterMessageHandler("my-action", (window, payload) => {
    // payload is everything after the first ';' in the raw message string
});
```

Messages are dispatched by splitting on `;` — the part before the first `;` is matched against registered handler keys, and everything after is passed as the optional payload string

```js
// JavaScript — format: "handlerId" or "handlerId;payload"
window.external.sendMessage("my-action");
window.external.sendMessage("my-action;some data");
```
