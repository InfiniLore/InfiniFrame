// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class PageNavigationWebMessageDispatcher : WindowFeatureWebMessageDispatcherBase<IPageNavigationInfiniFrameWindowFeature> {
    public override string FeatureName => "pageNavigation";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected override IPageNavigationInfiniFrameWindowFeature SelectFeature(IInfiniFrameWindowFeatures features)
        => features.PageNavigation;

    protected override object Get(IPageNavigationInfiniFrameWindowFeature feature, string command, JsonElement? args) => command switch {
        "tryLoadUri" => feature.TryLoadUri(new Uri(Required<string>(args, "uri"), UriKind.RelativeOrAbsolute)),
        "tryLoadPath" => feature.TryLoadPath(Required<string>(args, "path")),
        _ => throw Unsupported(command)
    };

    protected override void Post(IPageNavigationInfiniFrameWindowFeature feature, string command, JsonElement? args) {
        switch (command) {
            case "loadUri": feature.Load(new Uri(Required<string>(args, "uri"), UriKind.RelativeOrAbsolute)); return;
            case "loadPath": feature.Load(Required<string>(args, "path")); return;
            case "loadRawString": feature.LoadRawString(Required<string>(args, "content")); return;
            default: throw Unsupported(command);
        }
    }
}
