// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class MonitorsWebMessageDispatcher : WindowFeatureWebMessageDispatcherBase<IInfiniFrameWindowFeatureMonitors> {
    public override string FeatureName => "monitors";
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected override IInfiniFrameWindowFeatureMonitors SelectFeature(IInfiniFrameWindowFeatures features) 
        => features.Monitors;

    protected override object Get(IInfiniFrameWindowFeatureMonitors feature, string command, JsonElement? args) => command switch {
        "monitors" => feature.GetMonitors().ToArray(),
        "mainMonitor" => feature.GetMainMonitor(),
        "mainMonitorScreenDpi" => feature.GetMainMonitorScreenDpi(),
        _ => throw Unsupported(command)
    };
}
