// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Dialogs;
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class NotificationsWebMessageDispatcher : WindowFeatureWebMessageDispatcherBase<INotificationsInfiniFrameWindowFeature> {
    public override string FeatureName => "notifications";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected override INotificationsInfiniFrameWindowFeature SelectFeature(IInfiniFrameWindowFeatures features)
        => features.Notifications;

    protected override object Get(INotificationsInfiniFrameWindowFeature feature, string command, JsonElement? args)
        => command == "showMessage"
            ? feature.ShowMessage(
                Required<string>(args, "title"),
                Arg<string?>(args, "text", null),
                Arg(args, "buttons", InfiniFrameDialogButtons.Ok),
                Arg(args, "icon", InfiniFrameDialogIcon.Info))
            : throw Unsupported(command);

    protected override void Post(INotificationsInfiniFrameWindowFeature feature, string command, JsonElement? args) {
        if (command == "showNotification")
            feature.ShowNotification(Required<string>(args, "title"), Required<string>(args, "body"));
        else throw Unsupported(command);
    }
}
