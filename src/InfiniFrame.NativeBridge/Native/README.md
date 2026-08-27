# InfiniFrame Native C++ Codebase

This directory contains the native C++ implementation of InfiniFrame's window management layer. It provides the cross-platform browser engine integration (WebView2 on Windows, WebKitGTK on Linux, WKWebView on macOS) and exposes a C ABI that the .NET managed layer calls via P/Invoke.

## Architecture

```
Native/
  CMakeLists.txt              # Top-level CMake build configuration
  BUILDING.md                 # Build performance tips

  src/
    Api/
      Exports/                # extern "C" functions (the public C ABI)
        Exports.Window.Actions.cpp    # Center, Restore, Focus, Notifications
        Exports.Window.Getters.cpp    # Query window state (size, position, flags)
        Exports.Window.Setters.cpp    # Modify window state
        Exports.Window.Navigation.cpp # URL/HTML navigation, web messaging
        Exports.Window.Taskbar.cpp    # Taskbar progress and flash
        Exports.Dialog.cpp            # File picker and message dialogs
        Exports.Events.cpp            # Event callback registration
        Exports.Dispatch.cpp          # Cross-thread dispatch
        Exports.Lifecycle.cpp         # Window creation, destruction, shutdown
        Exports.Memory.cpp            # String memory management
        Exports.CustomSchemes.cpp     # Custom URL scheme registration
        Exports.Monitors.cpp          # Display enumeration
        Exports.Menu.cpp              # Native menu bar
        Exports.Platform.Windows.cpp  # Windows-specific exports
        Exports.Platform.MacOs.cpp    # macOS-specific exports
        Exports.Platform.Linux.cpp    # Linux-specific exports
      Testing/                # Test-only exports
      Utilities/              # Export infrastructure (validation, error state, string helpers)

    Runtime/
      Shared/                 # Cross-platform runtime code
        Window/               # Window state, events, configuration
        Operations/           # Async operation infrastructure
        Platform/             # Platform detection

    Embedded/                 # Embedded JS assets

    Platforms/
      Windows/                # WebView2 implementation
      Linux/                  # WebKitGTK implementation
      MacOs/                  # WKWebView implementation

  include/
    InfiniFrameWindow.h       # Main public header — InfiniFrameWindow class
    Types/                    # Shared ABI types (enums, structs)
```

## Public C API

The public API is defined in `src/Api/Exports/` and consists of `extern "C"` functions with the prefix `InfiniFrameNative_`. These functions are called by the .NET managed layer via P/Invoke.

### String Ownership

- **Input strings**: The caller owns strings passed to the native layer. The native layer copies them if needed.
- **Output strings**: The native layer allocates strings returned to the caller. The caller must free them using `InfiniFrameNative_FreeString`.
- **String arrays**: Use `InfiniFrameNative_FreeStringArray` to free arrays returned by functions like `GetAllMonitors`.

### Error Handling

All exported functions return `InteropStatus` (defined in `Types/InteropStatus.h`):
- `Success` — Operation completed successfully
- `ErrorNullParameter` — A required parameter was null
- `ErrorInvalidState` — The window is not in a valid state for this operation
- `ErrorPlatformUnsupported` — The operation is not supported on this platform

Last error messages can be retrieved with `InfiniFrameNative_GetLastErrorMessage`.

## Building

See [BUILDING.md](BUILDING.md) for build performance tips.

### Prerequisites

- CMake 4.0+
- C++23 compiler (MSVC 17+ on Windows, GCC 13+ on Linux, Clang 17+ on macOS)
- Platform SDKs: WebView2 (Windows), WebKit2GTK 4.1 (Linux), macOS SDK (macOS)

### Build

```bash
cmake -S . -B build -DCMAKE_BUILD_TYPE=Release
cmake --build build --config Release
```

## Platform Details

### Windows (WebView2)

- Uses Chromium-based WebView2 control
- Requires WebView2 Runtime (fixed-version or system)
- COM-based threading model (STA)

### Linux (WebKitGTK)

- Uses WebKit2GTK 4.1 for web rendering
- GTK3 for window management
- X11/Wayland display server support

### macOS (WKWebView)

- Uses WKWebView for web rendering
- AppKit for window management
- NSMenu for native menus

## Testing

Test-only exports are enabled with `INFINIFRAME_BUILD_TEST_EXPORTS=ON` (defaults to ON in Debug builds). These provide access to internal state for verification.
