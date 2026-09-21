// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Reflection;
using System.Runtime.InteropServices;
using InfiniTests.Native;
using JetBrains.Annotations;
using Microsoft.Testing.Platform.Builder;
using Assembly=System.Reflection.Assembly;

namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[UsedImplicitly]
public class MacOsTestingPlatform {
    private const string CoreFoundation = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    [UsedImplicitly]
    public static async Task<int> RunTestingPlatformAsync(string[] args) {
        ITestApplicationBuilder builder = await TestApplication.CreateBuilderAsync(args);
        AddSelfRegisteredExtensions(builder, args);

        using ITestApplication app = await builder.BuildAsync();
        return await app.RunAsync();
    }
    
    [UsedImplicitly]
    public static Task<int> RunMacOsTestingPlatformAsync(string[] args) {
        try {
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
                // A bounded run avoids the completion-CFRunLoopStop race where Stop arrives
                // just before an unbounded Run begins and leaves the test host asleep forever.
                // NSApplication.run normally installs and drains an autorelease pool for each
                // event-loop turn. This custom host owns the CFRunLoop instead, so it must do
                // the same, or repeated WKWebView tests retain autorelease WebKit/AppKit state.
                IntPtr pool = MacOsNative.PushAutoreleasePool();
                try {
                    _ = MacOsNative.RunLoopInMode(defaultMode, 0.25, false);
                }
                finally {
                    MacOsNative.PopAutoreleasePool(pool);
                }
            }

            // .NET 10's runtime teardown can call abort() on macOS after results are
            // written. Exit directly but preserve the test platform's result so failures
            // cannot be hidden by the teardown workaround.
            int exitCode = testTask.GetAwaiter().GetResult();
            MacOsNative.PosixExit(exitCode);
            return Task.FromResult(exitCode);
        }
        catch (Exception exception) {
            return Task.FromException<int>(exception);
        }
    }

    private static IntPtr ResolveDefaultRunLoopMode() {
        IntPtr library = NativeLibrary.Load(CoreFoundation);
        IntPtr symbol = NativeLibrary.GetExport(library, "kCFRunLoopDefaultMode");
        IntPtr mode = Marshal.ReadIntPtr(symbol);
        return mode != IntPtr.Zero 
            ? mode 
            : throw new InvalidOperationException("CoreFoundation returned a null default run-loop mode.");
    }

    private static void AddSelfRegisteredExtensions(ITestApplicationBuilder builder, string[] args) {
        // This helper lives in the shared test-support assembly, while TUnit emits the
        // registration hook into the runnable test project assembly.
        MethodInfo? addExtensions = (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly())
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
