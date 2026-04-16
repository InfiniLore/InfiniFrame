// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace InfiniFrame.Js.Interop.MessageHandlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class OpenExternalTargetWebMessageHandler {
    private static readonly HashSet<string> AllowedSchemes = new(StringComparer.OrdinalIgnoreCase) {
        Uri.UriSchemeHttps,
        Uri.UriSchemeHttp,
        Uri.UriSchemeMailto
    };

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static T RegisterOpenExternalTargetWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        RegisterWindowCreatedUtility.RegisterMessageHandler(builder, HandlerNames.OpenExternal, HandleWebMessage);
        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, HandlerNames.RegisterOpenExternal);
        return builder;
    }

    private static void HandleWebMessage(IInfiniFrameWindow window, string? payload) {
        if (string.IsNullOrWhiteSpace(payload)) return;

        if (!Uri.TryCreate(payload, UriKind.Absolute, out Uri? uri) || !uri.IsAbsoluteUri) {
            window.Logger.LogWarning("Rejected external URI due to parsing failure or non-absolute URI. Payload: {Payload}", payload);
            return;
        }

        if (!AllowedSchemes.Contains(uri.Scheme)) {
            window.Logger.LogWarning("Rejected external URI due to disallowed scheme. Scheme: {Scheme}, Uri: {Uri}", uri.Scheme, uri);
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
