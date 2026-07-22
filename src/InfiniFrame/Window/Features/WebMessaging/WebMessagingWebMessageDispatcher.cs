// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class WebMessagingWebMessageDispatcher : WindowFeatureWebMessageDispatcherBase<IInfiniFrameWindowFeatureWebMessaging> {
    public override string FeatureName => "webMessaging";
    protected override IInfiniFrameWindowFeatureWebMessaging SelectFeature(IInfiniFrameWindowFeatures features) => features.WebMessaging;

    protected override void Post(IInfiniFrameWindowFeatureWebMessaging feature, string command, JsonElement? args) {
        if (command == "sendWebMessage") feature.SendWebMessage(Required<string>(args, "message"));
        else throw Unsupported(command);
    }
}
