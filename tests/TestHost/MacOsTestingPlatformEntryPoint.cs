// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Testing.Platform.Builder;
using System.Reflection;
using System.Runtime.InteropServices;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class MacOsTestingPlatformEntryPoint {
    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    private static extern IntPtr CFRunLoopGetMain();

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    private static extern void CFRunLoopRun();

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    private static extern void CFRunLoopStop(IntPtr runLoop);

    public static async Task<int> Main(string[] args) {
        if (!OperatingSystem.IsMacOS())
            return await RunTestingPlatformAsync(args);

        IntPtr mainRunLoop = CFRunLoopGetMain();
        Task<int> testTask = Task.Run(async () => {
            try {
                return await RunTestingPlatformAsync(args);
            }
            finally {
                CFRunLoopStop(mainRunLoop);
            }
        });

        while (!testTask.IsCompleted) {
            CFRunLoopRun();
        }

        return await testTask;
    }

    private static async Task<int> RunTestingPlatformAsync(string[] args) {
        ITestApplicationBuilder builder = await TestApplication.CreateBuilderAsync(args);
        AddSelfRegisteredExtensions(builder, args);

        using ITestApplication app = await builder.BuildAsync();
        return await app.RunAsync();
    }

    private static void AddSelfRegisteredExtensions(ITestApplicationBuilder builder, string[] args) {
        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        Type? selfRegisteredExtensions = assembly
            .GetTypes()
            .FirstOrDefault(type => type.Name == "SelfRegisteredExtensions");

        MethodInfo? addExtensions = selfRegisteredExtensions?.GetMethod(
            "AddSelfRegisteredExtensions",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

        if (addExtensions is null)
            throw new InvalidOperationException("Could not find the generated MTP SelfRegisteredExtensions hook.");

        addExtensions.Invoke(null, [builder, args]);
    }
}
