// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class SizeWebMessageDispatcher : WindowFeatureWebMessageDispatcherBase<ISizeInfiniFrameWindowFeature> {
    public override string FeatureName => "size";
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected override ISizeInfiniFrameWindowFeature SelectFeature(IInfiniFrameWindowFeatures features)
        => features.Size;

    protected override object Get(ISizeInfiniFrameWindowFeature feature, string command, JsonElement? args) => command switch {
        "size" => feature.Size,
        "height" => feature.Height,
        "width" => feature.Width,
        "maxSize" => feature.MaxSize,
        "maxHeight" => feature.MaxHeight,
        "maxWidth" => feature.MaxWidth,
        "minSize" => feature.MinSize,
        "minHeight" => feature.MinHeight,
        "minWidth" => feature.MinWidth,
        "isResizable" => feature.IsResizable,
        _ => throw Unsupported(command)
    };

    protected override void Post(ISizeInfiniFrameWindowFeature feature, string command, JsonElement? args) {
        switch (command) {
            case "setSize":
                feature.SetSize(Required<int>(args, "width"), Required<int>(args, "height"));
                return;
            case "setHeight":
                feature.SetHeight(Required<int>(args, "height"));
                return;
            case "setMaxSize":
                feature.SetMaxSize(Required<int>(args, "width"), Required<int>(args, "height"));
                return;
            case "setMaxHeight":
                feature.SetMaxHeight(Required<int>(args, "height"));
                return;
            case "setMaxWidth":
                feature.SetMaxWidth(Required<int>(args, "width"));
                return;
            case "setMinSize":
                feature.SetMinSize(Required<int>(args, "width"), Required<int>(args, "height"));
                return;
            case "setMinHeight":
                feature.SetMinHeight(Required<int>(args, "height"));
                return;
            case "setMinWidth":
                feature.SetMinWidth(Required<int>(args, "width"));
                return;
            case "setWidth":
                feature.SetWidth(Required<int>(args, "width"));
                return;
            case "resize":
                feature.Resize(
                    Required<int>(args, "widthOffset"),
                    Required<int>(args, "heightOffset"),
                    Required<ResizeOrigin>(args, "origin"));
                return;
            case "setResizable":
                feature.SetResizable(Arg(args, "resizable", true));
                return;
            default: throw Unsupported(command);
        }
    }
}
