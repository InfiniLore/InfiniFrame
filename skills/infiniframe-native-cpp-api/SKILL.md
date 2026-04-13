---
name: infiniframe-native-cpp-api
description: Extending InfiniFrame at the native C++ layer. Pimpl idiom, C ABI exports, P/Invoke, string ownership, and platform implementations.
---
# InfiniFrame Native C++ API

> Skill for extending InfiniFrame at the native C++ layer or calling native DLLs directly.

## When to Use This Skill

- Extending InfiniFrame native functionality
- Building custom platform-specific features
- Debugging native interop issues
- Understanding C ABI exports
- Working with P/Invoke bindings

## Architecture Overview

### Pimpl Idiom

InfiniFrame uses [Pimpl idiom](https://en.cppreference.com/w/cpp/language/pimpl.html) throughout:
- `struct Impl` held by `std::unique_ptr<Impl>`
- Defined per-platform in `.cpp`/`.mm` files
- Headers are entirely platform-agnostic

### File Structure

```
src/InfiniFrame.Native/
├── Core/
│   ├── InfiniFrame.h           # Top-level native interop include
│   ├── InfiniFrameWindow.h     # Main native window class and callbacks
│   ├── InfiniFrameDialog.h     # Dialog surface for file/folder/message dialogs
│   ├── InfiniFrameInitParams.h # Build-time and startup window parameters
│   └── InfiniFrameWindowImpl.h # Shared implementation state for platform windows
├── Types/
│   ├── Basic.h                 # Primitive and interop-safe aliases
│   ├── Dialog.h                # Dialog enums and result contracts
│   └── Callbacks.h             # Callback signatures used by interop exports
├── Utils/
│   └── Event.h                 # Event helper abstraction
├── Platform/
│   ├── Windows/                # Win32 + WebView2 implementation
│   │   └── Window.cpp
│   ├── Mac/                    # Cocoa/WKWebView implementation
│   │   ├── Window.mm
│   │   ├── AppDelegate.mm
│   │   ├── UiDelegate.mm
│   │   ├── WindowDelegate.mm
│   │   ├── NavigationDelegate.mm
│   │   ├── UrlSchemeHandler.mm
│   │   └── NSWindowBorderless.mm
│   └── Linux/                  # GTK/WebKitGTK implementation
│       └── Window.cpp
└── Exports.cpp                 # Exported C ABI used by managed layer
```

## Core Headers

### InfiniFrame.h

Top-level native interop include. Use this header for:
- Basic type definitions
- Interop constants
- Common utility functions

### InfiniFrameWindow.h

Main native window class and callbacks. Contains:
- `InfiniFrameWindow` class declaration
- Window callback types
- Public API for window operations

### InfiniFrameDialog.h

Dialog surface for native OS dialogs:
- File picker dialogs
- Folder picker dialogs
- Message box dialogs
- Dialog enums and result types

### InfiniFrameInitParams.h

Build-time and startup window parameters:
- `InfiniFrameInitParams` struct
- Configuration flags
- Platform-specific init options

### InfiniFrameWindowImpl.h

Shared implementation state:
- Platform-agnostic window state
- Shared between all platform implementations

## Shared Native Types

### Basic.h

Primitive and interop-safe aliases:
- String types
- Numeric types
- Boolean types
- Handle types

### Dialog.h

Dialog enums and result contracts:
- Button configurations
- Icon types
- Result values

### Callbacks.h

Callback signatures used by interop exports:
- Function pointer types
- Delegate signatures
- Event handler types

### Event.h

Event helper abstraction:
- Multi-subscriber event handling
- Ordered event execution
- Event registration management

## Platform Implementations

### Windows (Platform/Windows/*)

- Win32 API for window management
- WebView2 for browser control
- COM-based WebView2 integration
- Native menu and dialog support

### macOS (Platform/Mac/*)

- Cocoa for window management
- WKWebView for browser control
- Objective-C++ (`.mm`) files
- Separate delegate files for:
  - `AppDelegate` — application lifecycle
  - `UiDelegate` — UI delegate for WebView
  - `WindowDelegate` — window events
  - `NavigationDelegate` — navigation events
  - `UrlSchemeHandler` — custom scheme handling
  - `NSWindowBorderless` — chromeless window support

### Linux (Platform/Linux/*)

- GTK3 for window management
- WebKitGTK for browser control
- GTK signal-based event handling

## Exported Bridge (Exports.cpp)

C ABI exports used by managed layer:
- All functions prefixed with `InfiniFrame_`
- C linkage (`extern "C"`)
- Platform-independent signatures

### String Ownership

**CRITICAL**: InfiniFrame exports explicit free functions for strings returned from native layer:

```csharp
InfiniFrame_FreeString(ptr);
InfiniFrame_FreeStringArray(ptr, count);
```

These are called automatically by managed wrapper — but if calling native exports directly, you are responsible for invoking them.

**Failure to free returned strings = memory leaks in long-running applications.**

## Build System

### CMake 4.0

```bash
# Build
cmake -B build -S .
cmake --build build --config Release
```

### C++ Standard

- **C++23** required
- Uses `std::format` (not external formatting library)
- Uses `simdjson` via CMake FetchContent (not bundled nlohmann/json)
- Uses `simdutf` for UTF conversions (not custom ToWide/ToUTF8String)

### Sanitizers (Debug)

- AddressSanitizer (ASan)
- UndefinedBehaviorSanitizer (UBSan)
- LeakSanitizer enabled

## P/Invoke Generation

InfiniFrame uses source-generated `[LibraryImport]` (requires .NET 7+):

```csharp
// Modern approach (InfiniFrame)
[LibraryImport("InfiniFrame.Native", ...)]
static partial void InfiniFrame_SetTitle(IntPtr instance, ...);
```

Not manual `[DllImport]` (old Photino approach):
```csharp
// Old approach (Photino) — NOT used in InfiniFrame
[DllImport("Photino.Native", ...)]
static extern void Photino_SetTitle(IntPtr instance, string title);
```

## String Conversions

All UTF-8 ↔ UTF-16 conversions use `simdutf`:
- High-performance
- Validated for correctness
- No silent corruption of non-ASCII paths (fixed from Photino #163)

## Native DLL Direct Calls

If calling native exports directly (bypassing managed wrapper):

### Function Naming

All exports prefixed with `InfiniFrame_`:
- `InfiniFrame_SetTitle`
- `InfiniFrame_GetTitle`
- `InfiniFrame_ShowMessage`
- `InfiniFrame_ShowSaveFile` (note: gained `defaultFileName` parameter vs Photino)

### String Return Ownership

```csharp
// If native function returns char*, you MUST free it
IntPtr titlePtr = InfiniFrame_GetTitle(windowHandle);
string title = Marshal.PtrToStringAnsi(titlePtr);
InfiniFrame_FreeString(titlePtr);  // MANDATORY
```

### Array Return Ownership

```csharp
// If native function returns string array
IntPtr arrayPtr = InfiniFrame_GetMonitors(windowHandle, out int count);
// ... use array ...
InfiniFrame_FreeStringArray(arrayPtr, count);  // MANDATORY
```

## Extending InfiniFrame Natively

### Adding Platform-Specific Code

1. Create files under `Platform/<PlatformName>/`
2. Implement platform-specific logic in `.cpp` (or `.mm` for macOS)
3. Use Pimpl idiom — keep headers platform-agnostic
4. Add exports to `Exports.cpp` if managed layer needs access

### Adding New Exports

1. Define callback signatures in `Types/Callbacks.h`
2. Implement function in platform-specific `.cpp`
3. Export via `Exports.cpp` with `InfiniFrame_` prefix
4. Add managed wrapper in C# with `[LibraryImport]`
5. Add explicit free function if returning strings

### Example: Adding New Window Method

**Exports.cpp**:
```cpp
extern "C" INFINIFRAME_API void InfiniFrame_MyNewMethod(InfWindow* instance, int param) {
    instance->Impl->MyNewMethod(param);
}
```

**C# wrapper**:
```csharp
[LibraryImport("InfiniFrame.Native")]
static partial void InfiniFrame_MyNewMethod(IntPtr instance, int param);
```

## Known Issues Fixed from Photino

| Photino Issue | How InfiniFrame Fixed It |
|---------------|--------------------------|
| Custom scheme handlers broken on Windows (#173/174) | Rewritten registration path, tested end-to-end |
| Memory leak in SendWebMessage (#165) | Explicit InfiniFrame_FreeString ownership model |
| No programmatic window focus (#158) | InfiniFrame_SetFocused/GetFocused exported |
| UTF encoding bug corrupts non-ASCII paths (#163) | simdutf used for all conversions |
| Stack overflow in WaitForExit on Linux (#141) | Per-window independent message loops |
| SetTopmost uses wrong Win32 style (#175) | Fixed HWND_TOPMOST/HWND_NOTOPMOST usage |

## Code Style

- **K&R brace style** (enforced by https://github.com/InfiniLore/InfiniFrame/blob/master/.clang-format)
- **C++23** features (`std::format`, etc.)
- **Pimpl idiom** throughout
- **Platform-agnostic headers**, platform-specific implementations

Run `clang-format` with repo configuration when editing native sources:

```bash
clang-format -i src/InfiniFrame.Native/**/*.cpp
clang-format -i src/InfiniFrame.Native/**/*.h
```

## Anti-Patterns

❌ **Include platform headers in core headers**:
```cpp
// WRONG in InfiniFrameWindow.h — breaks cross-platform build
#include <windows.h>
#include <gtk/gtk.h>
```

✅ **Keep headers platform-agnostic**:
```cpp
// Correct — implementation details in .cpp files
struct Impl;
std::unique_ptr<Impl> _impl;
```

❌ **Forget to free native strings**:
```csharp
// WRONG — memory leak
IntPtr str = InfiniFrame_GetTitle(handle);
string title = Marshal.PtrToStringAnsi(str);
// InfiniFrame_FreeString(str); // MISSING
```

✅ **Always free returned strings**:
```csharp
IntPtr str = InfiniFrame_GetTitle(handle);
string title = Marshal.PtrToStringAnsi(str);
InfiniFrame_FreeString(str);  // MANDATORY
```

❌ **Use bundled JSON headers**:
```cpp
// WRONG — Photino approach
#include "json.hpp"
```

✅ **Use simdjson via FetchContent**:
```cpp
// Correct — InfiniFrame approach
#include <simdjson.h>
```
