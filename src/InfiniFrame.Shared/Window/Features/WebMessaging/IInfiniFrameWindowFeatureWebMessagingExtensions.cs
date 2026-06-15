// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowFeatureWebMessagingExtensions {
    public static void SendWebMessage(this IInfiniFrameWindow window, string message) {
        window.Features.WebMessaging.SendWebMessage(message);
    }
    public static ValueTask SendWebMessageAsync(this IInfiniFrameWindow window, string message, CancellationToken ct = default) {
        return window.Features.WebMessaging.SendWebMessageAsync(message, ct);
    }
}
