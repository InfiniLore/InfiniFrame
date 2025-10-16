// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace InfiniLore.InfiniFrame.Js.MessageHandlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class OpenExternalTargetWebMessageHandler {
    private const string OpenExternal = HandlerNames.InfiniWindowPrefix + "open:external";
    private const string RegisterOpenExternal = HandlerNames.InfiniWindowPrefix + "register:open:external";

    public static T RegisterOpenExternalTargetWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        builder.MessageHandlers.RegisterMessageHandler(OpenExternal, HandleWebMessage);
        RegisterWindowCreatedUtilities.RegisterWindowCreatedWebMessage(builder, RegisterOpenExternal);
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
