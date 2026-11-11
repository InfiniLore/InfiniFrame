// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Js.Interop;

/// <summary>The generated page bridge (wwwroot/InfiniFrame.js) embedded for external host providers.</summary>
internal static class InfiniFrameHostBridgeScript {
    internal const string ResourceName = "InfiniFrame.Js.HostBridge.js";

    private static readonly Lazy<string> Script = new(Load, LazyThreadSafetyMode.ExecutionAndPublication);

    internal static string Value => Script.Value;

    private static string Load() {
        using Stream stream = typeof(InfiniFrameHostBridgeScript).Assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException(
                $"The generated InfiniFrame bridge script resource '{ResourceName}' is missing from {typeof(InfiniFrameHostBridgeScript).Assembly.GetName().Name}.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
