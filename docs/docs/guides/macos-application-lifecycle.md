# macOS Application Lifecycle

InfiniFrame owns AppKit and WebKit initialization through `InfiniFrameApplication`.
The application must be registered before its first window is constructed. Every
window receives the native application handle and is registered with the same
application owner.

## Prerequisites

- macOS 12.0 or newer
- Xcode with the matching macOS 12.0 or newer SDK
- CMake 4.0 or newer and an Objective-C++ capable AppleClang toolchain
- An Xcode or Ninja CMake generator
- AppKit and WebKit available in the target macOS SDK and runtime

The native target enables Objective-C++ and defaults `CMAKE_OSX_DEPLOYMENT_TARGET`
to `12.0`. Use `-DCMAKE_OSX_ARCHITECTURES=x86_64` or `arm64` when building a
single architecture.

## Ownership and Threading

`InfiniFrameApplication.Register` initializes the process `NSApplication`, its
delegate, menu, activation policy, and launch state. Window construction and all
AppKit/WKWebView operations are dispatched to the AppKit main thread.

`Run` owns the application event-loop boundary. `RunAsync` may be started from a
thread-pool continuation; native execution schedules AppKit work on the process
main thread and waits for the tracked logical windows to close. Closing one
window removes only that window from the live set. The final live window stops
the application loop, while the native window object remains registered until
deferred destruction has completed.

Shutdown is a drain request. Callback routes are detached before WebKit hosts are
returned to the bounded pool, and one additional main-queue turn protects against
deletion while an AppKit or reverse P/Invoke callback is unwinding. Application
disposal drains the pooled hosts on the AppKit thread before releasing the native
application handle.

## Process-Scoped Runtime

`NSApplication` and WebKit's process infrastructure are process-scoped on macOS.
InfiniFrame therefore does not attempt to terminate and reconstruct AppKit in the
same process. Sequential `InfiniFrameApplication` instances are supported after
the previous instance has drained its windows and pooled hosts; the shared
`NSApplication` delegate remains installed for the process lifetime.

## CI Verification

Windows and WSL can perform managed builds, ABI/header checks, and static native
checks, but cannot execute AppKit or WKWebView. Run genuine macOS verification in
the repository macOS workflow or on a self-hosted Mac:

```sh
dotnet restore InfiniFrame.GitHubActions.Testing.slnf -p:NativeArch=arm64
dotnet build InfiniFrame.GitHubActions.Testing.slnf --configuration Release --no-restore -p:NativeArch=arm64
dotnet test --solution InfiniFrame.GitHubActions.Testing.slnf --configuration Release --no-build --no-restore --framework net10.0 -p:NativeArch=arm64
```

The macOS workflow also builds x64 and arm64 native artifacts and runs the full
macOS test matrix. No Windows-built native binary is valid for macOS runtime
verification.
