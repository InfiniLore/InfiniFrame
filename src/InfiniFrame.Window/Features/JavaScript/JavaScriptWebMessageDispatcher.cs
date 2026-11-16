// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using InfiniFrame.Window.Features.WebMessaging.Handlers;

namespace InfiniFrame.Window.Features.JavaScript;
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
        "eval" => ExecuteEvalAsync(feature, Required<string>(args, "script")).GetAwaiter().GetResult(),
        _ => throw Unsupported(command)
    };

    /// <summary>
    ///     Awaits the evaluation on a <see cref="Task" /> instead of blocking on the <see cref="ValueTask" /> directly.
    ///     Reading the result of an incomplete <see cref="ValueTask" /> is undefined behaviour, so the value task is
    ///     awaited inside an asynchronous helper first.
    /// </summary>
    private static async Task<string?> ExecuteEvalAsync(IJavaScriptInfiniFrameWindowFeature feature, string script)
        => await feature.ExecuteJavaScriptAsync<string>(script).ConfigureAwait(false);
}
