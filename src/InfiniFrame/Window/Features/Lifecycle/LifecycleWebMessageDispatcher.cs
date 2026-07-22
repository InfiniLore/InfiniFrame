// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class LifecycleWebMessageDispatcher : WindowFeatureWebMessageDispatcherBase<IInfiniFrameWindowFeatureLifecycle> {
    public override string FeatureName => "lifecycle";
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected override IInfiniFrameWindowFeatureLifecycle SelectFeature(IInfiniFrameWindowFeatures features) 
        => features.Lifecycle;

    protected override object Get(IInfiniFrameWindowFeatureLifecycle feature, string command, JsonElement? args) => command switch {
        "state" => feature.State,
        "isClosedOrClosing" => feature.IsClosedOrClosing(),
        _ => throw Unsupported(command)
    };

    protected override void Post(IInfiniFrameWindowFeatureLifecycle feature, string command, JsonElement? args) {
        if (command == "close") feature.Close();
        else throw Unsupported(command);
    }
}
