# Drag and Drop Feature

The Drag and Drop feature handles file drop events from the operating system. It lets you receive files dragged onto the window and filter by file extension.

## Contents

- [Enabling Drag and Drop](#enabling-drag-and-drop)
- [Handling File Drops](#handling-file-drops)
- [Extension Filtering](#extension-filtering)
- [Runtime Control](#runtime-control)

## Enabling Drag and Drop

Drag and drop is a runtime-only feature. Enable it after `Build()`:

```csharp
var window = InfiniFrameWindowBuilder.Create()
    .SetTitle("My App")
    .SetStartPageUrl("https://myapp.local")
    .Build();

// Enable with default settings (all file types)
window.EnableDragDrop();
```

## Handling File Drops

Register a handler to receive file drop events:

```csharp
window.OnFileDropped((window, args) => {
    foreach (string filePath in args.FilePaths) {
        Console.WriteLine($"File dropped: {filePath}");
    }
});
```

The `FileDroppedEventArgs` contains the list of file paths that were dropped onto the window.

## Extension Filtering

Restrict which file types can be dropped:

```csharp
// Enable with extension filter
window.EnableDragDrop(".png", ".jpg", ".gif");

// Or via feature interface
window.Features.DragDrop.SetEnabled(true);
window.Features.DragDrop.SetAllowedExtensions([".png", ".jpg", ".gif"]);
```

When extensions are set, only files matching those extensions trigger the drop event.

## Runtime Control

```csharp
// Enable/disable
window.Features.DragDrop.SetEnabled(true);
window.Features.DragDrop.SetEnabled(false);

// Quick helpers
window.EnableDragDrop();
window.DisableDragDrop();

// Read current state
bool enabled = window.Features.DragDrop.IsEnabled;
IReadOnlyList<string> extensions = window.Features.DragDrop.AllowedExtensions;
```

:::note
Drag and drop is a runtime-only feature. There is no builder configuration for it you must enable it after `Build()`.
:::

## See Also

- [Window Features Architecture](window-features-architecture.md) How the feature system works
- [Core Window Guide](core-window.md) Builder API and feature overview
