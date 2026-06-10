// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;
using InfiniFrame.Security;
using System.ComponentModel;
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class OpenExternalTargetWebMessageHandler {
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
            // window.Logger.LogWarning("Rejected external URI due to parsing failure or non-absolute URI. Payload: {Payload}", payload);
            return;
        }

        IInfiniFrameUriSecurityPolicy uriSecurityPolicy = InfiniFrameUriSecurityPolicyRegistry.GetForWindow(window);
        if (!uriSecurityPolicy.IsExternalSchemeAllowed(uri.Scheme)) {
            // window.Logger.LogWarning("Rejected external URI due to disallowed scheme. Scheme: {Scheme}, Uri: {Uri}", uri.Scheme, uri);
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
        catch (Win32Exception) {
            // window.Logger.LogError(ex, "Failed to open external URL: {Uri}", uri);
        }
        catch (InvalidOperationException) {
            // window.Logger.LogError(ex, "Failed to open external URL: {Uri}", uri);
        }
        catch (PlatformNotSupportedException) {
            // window.Logger.LogError(ex, "Failed to open external URL: {Uri}", uri);
        }
    }
}
