// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.ComponentModel;

namespace InfiniFrame.Js.MessageHandlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class OpenExternalTargetWebMessageHandler {
    public static T RegisterOpenExternalTargetWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        RegisterWindowCreatedUtility.RegisterMessageHandler(builder, HandlerNames.OpenExternal, HandleWebMessage);
        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, HandlerNames.RegisterOpenExternal);
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
        catch (Win32Exception ex) {
            window.Logger.LogError(ex, "Failed to open external URL: {Uri}", uri);
        }
        catch (InvalidOperationException ex) {
            window.Logger.LogError(ex, "Failed to open external URL: {Uri}", uri);
        }
        catch (PlatformNotSupportedException ex) {
            window.Logger.LogError(ex, "Failed to open external URL: {Uri}", uri);
        }
    }
}
