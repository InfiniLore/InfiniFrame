// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class JavaScriptWebMessageDispatcher : WindowFeatureWebMessageDispatcherBase<IJavaScriptInfiniFrameWindowFeature> {
    public override string FeatureName => "javaScript";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected override IJavaScriptInfiniFrameWindowFeature SelectFeature(IInfiniFrameWindowFeatures features)
        => features.JavaScript;

    protected override void Post(IJavaScriptInfiniFrameWindowFeature feature, string command, JsonElement? args) {
        switch (command) {
            case "eval": {
                string script = Required<string>(args, "script");
                string? requestId = Arg<string?>(args, "requestId", null);
                feature.SendEvalToBrowser(script, requestId);
                return;
            }
            default: throw Unsupported(command);
        }
    }

    protected override object? Get(IJavaScriptInfiniFrameWindowFeature feature, string command, JsonElement? args) => command switch {
        "eval" => feature.ExecuteJavaScriptAsync<string>(Required<string>(args, "script")).GetAwaiter().GetResult(),
        _ => throw Unsupported(command)
    };
}
