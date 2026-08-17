// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Reflection;

namespace InfiniFrame.SingleFile;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniFrameSingleFile {
    public static void Initialize() {
        if (IsPackDeployment()) {
            InfiniFrameSingleFileBootstrap.Initialize();
        }
    }
    
    public static void AttachWithStaticWwwroot(IInfiniFrameWindowBuilder builder) {
        if (IsPackDeployment()) {
            builder.UseEmbeddedWwwrootAssets(
                scheme: "app",
                includePhysicalFallback: true,
                physicalWwwrootPath: Path.Join(AppContext.BaseDirectory, "wwwroot"),
                setStartUrl: true
            );
        }
    }
    
    public static void AttachWithBlazor(IInfiniFrameWindowBuilder builder) {
        // In pack mode, the file provider is handled by PackModeFileProvider
        // (detected in InfiniFrameBlazorAppBuilder.ConfigureFileProvider).
        // We must NOT register a scheme handler here — it would overwrite the
        // Blazor WebViewManager handler that was already registered by Build().
    }

    private static bool IsPackDeployment() {
        Assembly entryAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        string[] resourceNames = entryAssembly.GetManifestResourceNames();
        return resourceNames.Any(r => r.StartsWith("publish.", StringComparison.Ordinal) || r.Contains(".native.") || r.Contains(".wwwroot."));
    }
}
