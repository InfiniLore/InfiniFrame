// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;
using InfiniFrame.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.ComponentModel;
using System.Diagnostics;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class OpenExternalTargetWebMessageHandler {
    private static readonly ILogger Logger = NullLogger.Instance;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static T RegisterOpenExternalTargetWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        builder.RegisterWebMessagePostHandler(JsHandlerNames.OpenExternal, HandleWebMessage);
        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, JsHandlerNames.RegisterOpenExternal);
        return builder;
    }

    private static void HandleWebMessage(IInfiniFrameWindow window, string? payload) {
        if (string.IsNullOrWhiteSpace(payload)) return;

        if (!Uri.TryCreate(payload, UriKind.Absolute, out Uri? uri) || !uri.IsAbsoluteUri) {
            Logger.LogWarning("Rejected external URI due to parsing failure or non-absolute URI. Payload: {Payload}", payload);
            return;
        }

        IInfiniFrameUriSecurityPolicy uriSecurityPolicy = InfiniFrameUriSecurityPolicyRegistry.GetForWindow(window);
        if (!uriSecurityPolicy.IsExternalSchemeAllowed(uri.Scheme)) {
            Logger.LogWarning("Rejected external URI due to disallowed scheme. Scheme: {Scheme}, Uri: {Uri}", uri.Scheme, uri);
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
            Logger.LogError(ex, "Failed to open external URL: {Uri}", uri);
        }
        catch (InvalidOperationException ex) {
            Logger.LogError(ex, "Failed to open external URL: {Uri}", uri);
        }
        catch (PlatformNotSupportedException ex) {
            Logger.LogError(ex, "Failed to open external URL: {Uri}", uri);
        }
    }
}