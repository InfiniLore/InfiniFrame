// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class WebMessagingWebMessageDispatcher : WindowFeatureWebMessageDispatcherBase<IWebMessagingInfiniFrameWindowFeature> {
    public override string FeatureName => "webMessaging";
    protected override IWebMessagingInfiniFrameWindowFeature SelectFeature(IInfiniFrameWindowFeatures features) => features.WebMessaging;

    protected override void Post(IWebMessagingInfiniFrameWindowFeature feature, string command, JsonElement? args) {
        if (command == "sendWebMessage") feature.SendWebMessage(Required<string>(args, "message"));
        else throw Unsupported(command);
    }
}
