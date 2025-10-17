// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using static InfiniLore.InfiniFrame.Js.Utilities.RegisterWindowCreatedUtility;

namespace InfiniLore.InfiniFrame.Js.MessageHandlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class OpenExternalTargetWebMessageHandler {
    public static T RegisterOpenExternalTargetWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        RegisterMessageHandler(builder,HandlerNames.OpenExternal, HandleWebMessage);
        RegisterWindowCreatedWebMessage(builder, HandlerNames.RegisterOpenExternal);
        return builder;
    }

    private static void HandleWebMessage(IInfiniFrameWindow window, string? payload) {
        if (string.IsNullOrWhiteSpace(payload)) return;

        if (!Uri.TryCreate(payload, UriKind.Absolute, out Uri? uri)) {
            window.Logger.LogWarning("Invalid URL: {uri}", payload);
            return;
        }

        try {
            var psi = new ProcessStartInfo {
                FileName = uri.AbsoluteUri,
                UseShellExecute = true,
                CreateNoWindow = true
            };
            Process.Start(psi);
        }
        catch (Exception ex) {
            window.Logger.LogError("Failed to open external: {ex}", ex);
        }
    }
}
