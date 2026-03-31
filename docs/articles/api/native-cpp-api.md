# Native C++ API

InfiniFrame native API documentation is hosted inside DocFX as conceptual reference and is sourced from the native headers and Doxygen-style comments under `src/InfiniFrame.Native`.

## Core headers

- `Core/InfiniFrame.h`: top-level native interop include.
- `Core/InfiniFrameWindow.h`: main native window class and callbacks.
- `Core/InfiniFrameDialog.h`: dialog surface for file/folder/message dialogs.
- `Core/InfiniFrameInitParams.h`: build-time and startup window parameters.
- `Core/InfiniFrameWindowImpl.h`: shared implementation state for platform windows.

## Shared native types

- `Types/Basic.h`: primitive and interop-safe aliases.
- `Types/Dialog.h`: dialog enums and result contracts.
- `Types/Callbacks.h`: callback signatures used by interop exports.
- `Utils/Event.h`: event helper abstraction used across native layers.

## Platform implementations

- `Platform/Windows/*`: Win32 + WebView2 host implementation.
- `Platform/Mac/*`: Cocoa/WKWebView host implementation.
- `Platform/Linux/*`: GTK/WebKitGTK host implementation.

## Exported bridge

- `Exports.cpp`: exported C ABI used by the managed layer.

For detailed behavior, see source comments in the files above. This page is maintained in DocFX to keep C++ documentation and C# documentation in one navigation model.

For generated member-level C++ reference, see [Native C++ API Reference (Generated)](native-cpp-reference.md).
