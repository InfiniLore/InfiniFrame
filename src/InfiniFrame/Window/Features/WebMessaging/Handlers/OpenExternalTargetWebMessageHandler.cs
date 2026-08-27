// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using InfiniFrame.Interop;
using InfiniFrame.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides extension methods to register a JavaScript handler that opens external URLs in the default system
///     browser, with URI scheme validation.
/// </summary>
public static class OpenExternalTargetWebMessageHandler {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Registers a JavaScript handler that opens external URLs in the default system browser.
    /// </summary>
    /// <typeparam name="T">The builder type.</typeparam>
    /// <param name="builder">The window builder.</param>
    /// <returns>The builder for chaining.</returns>
    public static T RegisterOpenExternalTargetWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        builder.RegisterWebMessagePostHandler(JsHandlerNames.OpenExternal, HandleWebMessage);
        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, JsHandlerNames.RegisterOpenExternal);
        return builder;
    }

    private static void HandleWebMessage(IInfiniFrameWindow window, string? payload) {
        if (string.IsNullOrWhiteSpace(payload)) return;

        ILogger logger = window.ServiceProvider?.GetService(typeof(ILogger)) as ILogger ?? NullLogger.Instance;
        IExternalProcessLauncher launcher = window.ServiceProvider?.GetService(typeof(IExternalProcessLauncher)) as IExternalProcessLauncher ?? new ExternalProcessLauncher();

        if (!Uri.TryCreate(payload, UriKind.Absolute, out Uri? uri) || !uri.IsAbsoluteUri) {
            logger.LogWarning("Rejected external URI due to parsing failure or non-absolute URI. Payload: {Payload}", payload);
            return;
        }

        IInfiniFrameUriSecurityPolicy uriSecurityPolicy = InfiniFrameUriSecurityPolicyRegistry.GetForWindow(window);
        if (!uriSecurityPolicy.IsExternalSchemeAllowed(uri.Scheme)) {
            logger.LogWarning("Rejected external URI due to disallowed scheme. Scheme: {Scheme}, Uri: {Uri}", uri.Scheme, uri);
            return;
        }

        // Block loopback and private IP addresses for http/https to prevent SSRF attacks.
        if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) {
            if (IsLoopbackOrPrivateIp(uri.Host)) {
                logger.LogWarning("Rejected external URI due to loopback/private IP. Uri: {Uri}", uri);
                return;
            }
        }

        try {
            var psi = new ProcessStartInfo {
                FileName = uri.AbsoluteUri,
                UseShellExecute = true,
                CreateNoWindow = true
            };
            launcher.Start(psi);
        }
        catch (Win32Exception ex) {
            logger.LogError(ex, "Failed to open external URL: {Uri}", uri);
        }
        catch (InvalidOperationException ex) {
            logger.LogError(ex, "Failed to open external URL: {Uri}", uri);
        }
        catch (PlatformNotSupportedException ex) {
            logger.LogError(ex, "Failed to open external URL: {Uri}", uri);
        }
    }

    private static bool IsLoopbackOrPrivateIp(string host) {
        if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase)) return true;
        if (IPAddress.TryParse(host, out IPAddress? ip)) {
            return IPAddress.IsLoopback(ip) || IsPrivateIp(ip);
        }
        return false;
    }

    private static bool IsPrivateIp(IPAddress ip) {
        byte[] bytes = ip.GetAddressBytes();
        return bytes[0] == 10 // 10.0.0.0/8
            || (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) // 172.16.0.0/12
            || (bytes[0] == 192 && bytes[1] == 168); // 192.168.0.0/16
    }
}
