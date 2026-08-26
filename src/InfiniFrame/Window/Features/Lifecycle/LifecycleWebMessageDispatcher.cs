// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class LifecycleWebMessageDispatcher : WindowFeatureWebMessageDispatcherBase<ILifecycleInfiniFrameWindowFeature> {
    public override string FeatureName => "lifecycle";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected override ILifecycleInfiniFrameWindowFeature SelectFeature(IInfiniFrameWindowFeatures features)
        => features.Lifecycle;

    protected override object Get(ILifecycleInfiniFrameWindowFeature feature, string command, JsonElement? args) => command switch {
        "state" => feature.State,
        "isClosedOrClosing" => feature.IsClosedOrClosing(),
        _ => throw Unsupported(command)
    };

    protected override void Post(ILifecycleInfiniFrameWindowFeature feature, string command, JsonElement? args) {
        if (command == "close") feature.Close();
        else throw Unsupported(command);
    }
}
