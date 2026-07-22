// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class MonitorsWebMessageDispatcher : WindowFeatureWebMessageDispatcherBase<IMonitorsInfiniFrameWindowFeature> {
    public override string FeatureName => "monitors";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected override IMonitorsInfiniFrameWindowFeature SelectFeature(IInfiniFrameWindowFeatures features)
        => features.Monitors;

    protected override object Get(IMonitorsInfiniFrameWindowFeature feature, string command, JsonElement? args) => command switch {
        "monitors" => feature.GetMonitors().ToArray(),
        "mainMonitor" => feature.GetMainMonitor(),
        "mainMonitorScreenDpi" => feature.GetMainMonitorScreenDpi(),
        _ => throw Unsupported(command)
    };
}
