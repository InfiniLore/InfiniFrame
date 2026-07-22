// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class DecorationsWebMessageDispatcher : WindowFeatureWebMessageDispatcherBase<IInfiniFrameWindowFeatureDecorations> {
    public override string FeatureName => "decorations";
    protected override IInfiniFrameWindowFeatureDecorations SelectFeature(IInfiniFrameWindowFeatures features) => features.Decorations;

    protected override object? Get(IInfiniFrameWindowFeatureDecorations feature, string command, JsonElement? args) => command switch {
        "isChromeless" => feature.IsChromeless,
        "isTransparent" => feature.IsTransparent,
        "title" or "getTitle" => feature.Title,
        "iconFilePath" => feature.IconFilePath,
        "limitLinuxWindowTitleLength" => feature.LimitLinuxWindowTitleLength,
        _ => throw Unsupported(command)
    };

    protected override void Post(IInfiniFrameWindowFeatureDecorations feature, string command, JsonElement? args) {
        switch (command) {
            case "setTransparent": feature.SetTransparent(Arg(args, "enabled", true)); return;
            case "setTitle": feature.SetTitle(Arg<string?>(args, "title", null)); return;
            case "setIconFile": feature.SetIconFile(Required<string>(args, "iconFilePath")); return;
            case "setLimitLinuxWindowTitleLength": feature.SetLimitLinuxWindowTitleLength(Arg(args, "enabled", true)); return;
            default: throw Unsupported(command);
        }
    }
}
