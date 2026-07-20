// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Testing.Platform.Builder;
using System.Reflection;
using System.Runtime.InteropServices;
using Assembly = System.Reflection.Assembly;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static partial class MacOsTestingPlatformEntryPoint {
    private const string CoreFoundation = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";
    private const string ObjCRuntime = "/usr/lib/libobjc.A.dylib";

    [LibraryImport(CoreFoundation)]
    private static partial IntPtr CFRunLoopGetMain();

    // ReSharper disable once InconsistentNaming
    [LibraryImport(CoreFoundation)]
    private static partial int CFRunLoopRunInMode(IntPtr mode, double seconds, [MarshalAs(UnmanagedType.I1)] bool returnAfterSourceHandled);

    [LibraryImport(CoreFoundation)]
    private static partial void CFRunLoopStop(IntPtr runLoop);

    [LibraryImport(ObjCRuntime, EntryPoint = "objc_autoreleasePoolPush")]
    private static partial IntPtr AutoreleasePoolPush();

    [LibraryImport(ObjCRuntime, EntryPoint = "objc_autoreleasePoolPop")]
    private static partial void AutoreleasePoolPop(IntPtr pool);

    public static async Task<int> Main(string[] args) {
        if (!OperatingSystem.IsMacOS()) return await RunTestingPlatformAsync(args);

        IntPtr mainRunLoop = CFRunLoopGetMain();
        IntPtr defaultMode = ResolveDefaultRunLoopMode();
        Task<int> testTask = Task.Run(async () => {
            try {
                return await RunTestingPlatformAsync(args);
            }
            finally {
                CFRunLoopStop(mainRunLoop);
            }
        });

        while (!testTask.IsCompleted) {
            // A bounded run avoids the completion-vs-CFRunLoopStop race where Stop arrives
            // just before an unbounded Run begins and leaves the test host asleep forever.
            // NSApplication.run normally installs and drains an autorelease pool for each
            // event-loop turn. This custom host owns the CFRunLoop instead, so it must do
            // the same or repeated WKWebView tests retain autoreleased WebKit/AppKit state.
            IntPtr pool = AutoreleasePoolPush();
            try {
                _ = CFRunLoopRunInMode(defaultMode, 0.25, false);
            }
            finally {
                AutoreleasePoolPop(pool);
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
