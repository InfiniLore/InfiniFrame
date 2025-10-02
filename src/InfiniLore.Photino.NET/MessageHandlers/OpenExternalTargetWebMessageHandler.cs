// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.Photino.NET.MessageHandlers;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class OpenExternalTargetWebMessageHandler {
    private const string OpenExternal = "open:external";
    private const string RegisterOpenExternal = "register:open:external";

    public static T RegisterOpenExternalTargetWebMessageHandler<T>(this T builder) where T : class,IPhotinoWindowBuilder {
        builder.MessageHandlers.RegisterMessageHandler(OpenExternal, HandleWebMessage);
        RegisterWindowCreatedUtilities.RegisterWindowCreatedWebMessage(builder, RegisterOpenExternal);
        return builder;
    }

    private static void HandleWebMessage(IPhotinoWindow window, string? payload) {
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
