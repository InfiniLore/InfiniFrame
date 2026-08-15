// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniTests.Native;
using Microsoft.Testing.Platform.Builder;
using System.Reflection;
using System.Runtime.InteropServices;
using Assembly=System.Reflection.Assembly;

namespace InfiniAutomationTests.BlazorWebView.MudBlazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class MacOsTestingPlatformEntryPoint {
    private const string CoreFoundation = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

    public static async Task<int> Main(string[] args) {
        if (!OperatingSystem.IsMacOS()) return await RunTestingPlatformAsync(args);

        IntPtr mainRunLoop = MacOsNative.GetMainRunLoop();
        IntPtr defaultMode = ResolveDefaultRunLoopMode();
        Task<int> testTask = Task.Run(async () => {
            try {
                return await RunTestingPlatformAsync(args);
            }
            finally {
                MacOsNative.StopRunLoop(mainRunLoop);
            }
        });

        while (!testTask.IsCompleted) {
            // A bounded run avoids the completion-vs-CFRunLoopStop race where Stop arrives
            // just before an unbounded Run begins and leaves the test host asleep forever.
            // NSApplication.run normally installs and drains an autorelease pool for each
            // event-loop turn. This custom host owns the CFRunLoop instead, so it must do
            // the same or repeated WKWebView tests retain autoreleased WebKit/AppKit state.
            IntPtr pool = MacOsNative.PushAutoreleasePool();
            try {
                _ = MacOsNative.RunLoopInMode(defaultMode, 0.25, false);
            }
            finally {
                MacOsNative.PopAutoreleasePool(pool);
            }
        }

        // Drain pending main-queue blocks (deferred destruction, etc.) that were dispatched
        // after the test host stopped the run loop. Without this, dispatch_async(delete this)
        // in ScheduleDeferredDestruction never executes on net10.0, leaving the native instance
        // alive long enough for the GC finalizer to trigger a dispatch_sync deadlock that the
        // runtime converts to SIGABRT (exit code 134).
        for (int i = 0; i < 10; i++) {
            IntPtr pool = MacOsNative.PushAutoreleasePool();
            try {
                _ = MacOsNative.RunLoopInMode(defaultMode, 0.1, false);
            }
            finally {
                MacOsNative.PopAutoreleasePool(pool);
            }
        }

        return await testTask;
    }

    private static IntPtr ResolveDefaultRunLoopMode() {
        IntPtr library = NativeLibrary.Load(CoreFoundation);
        IntPtr symbol = NativeLibrary.GetExport(library, "kCFRunLoopDefaultMode");
        IntPtr mode = Marshal.ReadIntPtr(symbol);
        if (mode == IntPtr.Zero)
            throw new InvalidOperationException("CoreFoundation returned a null default run-loop mode.");

        return mode;
    }

    private static async Task<int> RunTestingPlatformAsync(string[] args) {
        ITestApplicationBuilder builder = await TestApplication.CreateBuilderAsync(args);
        AddSelfRegisteredExtensions(builder, args);

        using ITestApplication app = await builder.BuildAsync();
        return await app.RunAsync();
    }

    private static void AddSelfRegisteredExtensions(ITestApplicationBuilder builder, string[] args) {
        MethodInfo? addExtensions = Assembly.GetExecutingAssembly()
            .GetTypes()
            .FirstOrDefault(type => type.Name == "SelfRegisteredExtensions")?
            .GetMethod(
                "AddSelfRegisteredExtensions",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static
            );

        if (addExtensions is null)
            throw new InvalidOperationException("Could not find the generated MTP SelfRegisteredExtensions hook.");

        addExtensions.Invoke(null, [builder, args]);
    }
}