// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class PositionWebMessageDispatcher : WindowFeatureWebMessageDispatcherBase<IInfiniFrameWindowFeaturePosition> {
    public override string FeatureName => "position";
    protected override IInfiniFrameWindowFeaturePosition SelectFeature(IInfiniFrameWindowFeatures features) => features.Position;

    protected override object Get(IInfiniFrameWindowFeaturePosition feature, string command, JsonElement? args) => command switch {
        "location" => feature.Location,
        "top" => feature.Top,
        "left" => feature.Left,
        _ => throw Unsupported(command)
    };

    protected override void Post(IInfiniFrameWindowFeaturePosition feature, string command, JsonElement? args) {
        switch (command) {
            case "setLocation": feature.SetLocation(Required<int>(args, "left"), Required<int>(args, "top")); return;
            case "setLeft": feature.SetLeft(Required<int>(args, "left")); return;
            case "setTop": feature.SetTop(Required<int>(args, "top")); return;
            case "offset": feature.Offset(Required<double>(args, "left"), Required<double>(args, "top")); return;
            case "center": feature.Center(); return;
            case "centerOnCurrentMonitor": feature.CenterOnCurrentMonitor(); return;
            case "centerOnMonitor": feature.CenterOnMonitor(Required<int>(args, "monitorIndex")); return;
            case "moveWithinCurrentMonitorArea":
                feature.MoveWithinCurrentMonitorArea(Required<double>(args, "left"), Required<double>(args, "top"));
                return;
            default: throw Unsupported(command);
        }
    }
}
