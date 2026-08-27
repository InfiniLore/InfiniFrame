# Native Menu Support

This guide covers native menu bar support in InfiniFrame: configuring menu bars at build time, managing menu items at runtime, and platform-specific behavior.

The native menu is implemented as a window feature. For an overview of the feature system, see [Window Features Architecture](window-features-architecture.md).

## Contents

- [Quick Start](#quick-start)
- [Builder Configuration](#builder-configuration)
- [Menu Model Reference](#menu-model-reference)
- [Runtime Operations](#runtime-operations)
- [Platform Support](#platform-support)
- [Migration from Photino](#migration-from-photino)

## Quick Start

```csharp
using InfiniFrame;
using System.Collections.Immutable;

var window = InfiniFrameWindowBuilder.Create()
    .SetTitle("My App")
    .SetMenuBar(new InfiniFrameMenuBar(
        Items: ImmutableArray.Create(
            new InfiniFrameMenuItem("file", "File", InfiniFrameMenuItemType.Submenu,
                Children: ImmutableArray.Create(
                    new InfiniFrameMenuItem("open", "Open", KeyboardShortcut: "Ctrl+O"),
                    new InfiniFrameMenuItem("save", "Save", KeyboardShortcut: "Ctrl+S"),
                    new InfiniFrameMenuItem("sep", Type: InfiniFrameMenuItemType.Separator),
                    new InfiniFrameMenuItem("exit", "Exit", KeyboardShortcut: "Alt+F4")
                )
            ),
            new InfiniFrameMenuItem("edit", "Edit"),
            new InfiniFrameMenuItem("help", "Help")
        )
    ))
    .SetStartPageUrl("https://myapp.local")
    .Build();

window.WaitForClose();
```

## Builder Configuration

Set the menu bar during window construction using `SetMenuBar`:

```csharp
var builder = InfiniFrameWindowBuilder.Create()
    .SetMenuBar(new InfiniFrameMenuBar(
        Items: ImmutableArray.Create(
            new InfiniFrameMenuItem("file", "File", InfiniFrameMenuItemType.Submenu,
                Children: ImmutableArray.Create(
                    new InfiniFrameMenuItem("new", "New", KeyboardShortcut: "Ctrl+N"),
                    new InfiniFrameMenuItem("open", "Open", KeyboardShortcut: "Ctrl+O"),
                    new InfiniFrameMenuItem("save", "Save", KeyboardShortcut: "Ctrl+S")
                )
            )
        )
    ));

IInfiniFrameWindow window = builder.Build();
```

The menu bar is serialized to JSON and passed to the native layer at window creation.

## Menu Model Reference

### InfiniFrameMenuBar

| Property | Type | Description |
|----------|------|-------------|
| `Items` | `ImmutableArray<InfiniFrameMenuItem>` | Top-level menu items |

### InfiniFrameMenuItem

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Id` | `string` | (required) | Unique identifier for programmatic access |
| `Label` | `string?` | `null` | Display text (required for Normal/Submenu) |
| `Type` | `InfiniFrameMenuItemType` | `Normal` | Item type |
| `IsEnabled` | `bool` | `true` | Whether the item can be interacted with |
| `IsVisible` | `bool` | `true` | Whether the item is visible |
| `KeyboardShortcut` | `string?` | `null` | Keyboard shortcut (e.g., "Ctrl+S") |
| `Children` | `ImmutableArray<InfiniFrameMenuItem>` | `Empty` | Child items for submenus |

### InfiniFrameMenuItemType

| Value | Description |
|-------|-------------|
| `Normal` | Standard clickable menu item |
| `Separator` | Visual separator between items |
| `Submenu` | Item containing child items |

## Runtime Operations

Once the window is created, you can modify menu items at runtime:

```csharp
// Disable a menu item
window.Features.Menu.SetMenuItemEnabled("save", false);

// Hide a menu item
window.Features.Menu.SetMenuItemVisible("save", false);

// Send a click command
window.Features.Menu.ClickMenuItem("open");

// Replace the entire menu bar
window.Features.Menu.SetMenuBar(newInMenuBar);
```

### Extension Methods

Fluent extension methods are available for method chaining:

```csharp
window
    .SetMenuBar(newMenuBar)
    .SetMenuItemEnabled("save", false)
    .SetMenuItemVisible("sep", false)
    .ClickMenuItem("open");
```

### Updating Nested Items

Enable/disable and visibility changes work recursively through submenu children:

```csharp
// This finds "save" inside the "File" submenu
window.Features.Menu.SetMenuItemEnabled("save", false);
```

## Platform Support

| Feature | Windows | Linux | macOS |
|---------|---------|-------|-------|
| Menu bar at build time | ✅ | ✅ | ✅ |
| Runtime menu modification | ✅ | ✅ | ✅ |
| Keyboard shortcuts | ✅ | ✅ | ✅ |
| Submenus | ✅ | ✅ | ✅ |
| Separators | ✅ | ✅ | ✅ |
| Enable/disable items | ✅ | ✅ | ✅ |
| Show/hide items | ✅ | ✅ | ✅ |
| Native click commands | ✅ | ✅ | ✅ |

:::note
Native menu rendering is handled by the platform adapters (Win32, GTK, macOS NSMenu). The managed API is available on all platforms; native rendering support is being implemented incrementally.
:::

## Migration from Photino

The Photino ecosystem has an open issue for native menu support ([Photino.Native#44](https://github.com/tryphotino/photino.Native/issues/44)). InfiniFrame implements this as a first-class feature with the managed API.

If you were using a custom solution for menus in Photino, you can now use the built-in `InfiniFrameMenuBar` API:

```csharp
// Before (Photino - manual approach)
// Custom JavaScript or native interop required

// After (InfiniFrame)
var menuBar = new InfiniFrameMenuBar(
    Items: ImmutableArray.Create(
        new InfiniFrameMenuItem("file", "File", InfiniFrameMenuItemType.Submenu,
            Children: ImmutableArray.Create(
                new InfiniFrameMenuItem("exit", "Exit")
            )
        )
    )
);
window.SetMenuBar(menuBar);
```

## See Also

- [Window Features Architecture](window-features-architecture.md) How the feature system works
- [Core Window Guide](core-window.md) Builder API and feature overview
