# File Dialogs Feature

The File Dialogs feature provides access to native OS file and folder picker dialogs. These are available as both synchronous and asynchronous operations.

For message boxes and notifications, see the [Notifications guide](notifications.md).

## Contents

- [Open File Dialog](#open-file-dialog)
- [Folder Pickers](#folder-pickers)
- [Save File Dialog](#save-file-dialog)

## Open File Dialog

Open one or more files:

```csharp
// Synchronous
string?[] files = window.ShowOpenFile(
    title: "Open File",
    defaultPath: null,
    multiSelect: true,
    filters: [("Images", ["png", "jpg", "gif"]), ("All Files", ["*"])]
);

// Async
string?[] files = await window.ShowOpenFileAsync(
    title: "Open File",
    defaultPath: null,
    multiSelect: true,
    filters: [("Images", ["png", "jpg", "gif"]), ("All Files", ["*"])]
);
```

### Via feature interface

```csharp
string?[] files = window.Features.FilePickerDialogs.ShowOpenFile(
    title: "Open File",
    defaultPath: null,
    multiSelect: true,
    filters: [("Images", ["png", "jpg", "gif"])]
);
```

### File filters

File filters are tuples of `(string label, string[] extensions)`:

```csharp
var filters = new[] {
    ("Images", new[] { "png", "jpg", "gif" }),
    ("Documents", new[] { "pdf", "docx", "txt" }),
    ("All Files", new[] { "*" })
};
```

## Folder Pickers

Open one or more folders:

```csharp
// Synchronous
string?[] folders = window.ShowOpenFolder("Select Folder", multiSelect: false);

// Async
string?[] folders = await window.ShowOpenFolderAsync("Select Folder", multiSelect: false);

// Via feature interface
string?[] folders = window.Features.FilePickerDialogs.ShowOpenFolder(
    title: "Select Folder",
    defaultPath: null,
    multiSelect: false
);
```

## Save File Dialog

Show a save file dialog:

```csharp
// Synchronous
string? path = window.ShowSaveFile(
    title: "Save As",
    defaultPath: null,
    filters: [("Text Files", ["txt"])],
    defaultFileName: "document.txt"
);

// Async
string? path = await window.ShowSaveFileAsync(
    title: "Save As",
    defaultPath: null,
    filters: [("Text Files", ["txt"])],
    defaultFileName: "document.txt"
);

// Via feature interface
string? path = window.Features.FilePickerDialogs.ShowSaveFile(
    title: "Save As",
    defaultPath: null,
    filters: [("Text Files", ["txt"])],
    defaultFileName: "document.txt"
);
```

All file picker methods return `null` if the user cancels the dialog.

## See Also

- [Notifications](notifications.md) — Native notifications and message boxes
- [Window Features Architecture](window-features-architecture.md) — How the feature system works
- [Core Window Guide](core-window.md) — Builder API and feature overview
