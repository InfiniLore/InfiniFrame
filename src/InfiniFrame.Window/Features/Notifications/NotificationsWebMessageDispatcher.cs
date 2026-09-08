// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using InfiniFrame.NativeBridge.Dialogs;

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
        if (command == "showNotification") {
            string? iconPath = Arg<string?>(args, "iconPath", null);
            string? tag = Arg<string?>(args, "tag", null);
            string? urgencyStr = Arg<string?>(args, "urgency", null);

            if (iconPath is not null || tag is not null || urgencyStr is not null) {
                InfiniFrameNotificationUrgency urgency = urgencyStr is not null
                    && Enum.TryParse(urgencyStr, true, out InfiniFrameNotificationUrgency parsed)
                        ? parsed
                        : InfiniFrameNotificationUrgency.Normal;

                feature.ShowNotification(new InfiniFrameNotificationOptions {
                    Title = Required<string>(args, "title"),
                    Body = Required<string>(args, "body"),
                    IconPath = iconPath,
                    Urgency = urgency,
                    Tag = tag
                });
            }
            else {
                feature.ShowNotification(Required<string>(args, "title"), Required<string>(args, "body"));
            }
        }
        else throw Unsupported(command);
    }
}
