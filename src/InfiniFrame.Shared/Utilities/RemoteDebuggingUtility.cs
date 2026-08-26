// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides utility methods for remote debugging configuration and validation.
/// </summary>
internal static partial class RemoteDebuggingUtility {
    /// <summary>
    ///     The minimum valid port number.
    /// </summary>
    public const int MinPort = 1;
    /// <summary>
    ///     The maximum valid port number.
    /// </summary>
    public const int MaxPort = 65535;

    private const string LoopbackAddress = "127.0.0.1";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Normalizes a port value, throwing if it is outside the valid range.
    /// </summary>
    /// <param name="port">The port value to normalize.</param>
    /// <param name="parameterName">The name of the parameter for error reporting.</param>
    /// <returns>The normalized port value, or 0 if the input is 0.</returns>
    public static int NormalizePort(int port, string parameterName = "port") {
        return port switch {
            0 => 0,
            < MinPort or > MaxPort => throw new ArgumentOutOfRangeException(parameterName, port, $"Remote debugging port must be {MinPort}..{MaxPort}, or 0/null to disable."),
            _ => port
        };

    }

    /// <summary>
    ///     Determines whether remote debugging is supported on the current platform.
    /// </summary>
    /// <returns><c>true</c> if the platform is Windows or Linux; otherwise, <c>false</c>.</returns>
    public static bool IsSupportedPlatform() => OperatingSystem.IsWindows() || OperatingSystem.IsLinux();

    /// <summary>
    ///     Ensures the current platform supports remote debugging for the given port.
    /// </summary>
    /// <param name="normalizedPort">The normalized port value.</param>
    public static void EnsureSupportedPlatform(int normalizedPort) {
        switch (normalizedPort) {
            case 0:
                return;
            case < MinPort or > MaxPort:
                throw new ArgumentOutOfRangeException(nameof(normalizedPort), normalizedPort, $"Remote debugging port must be {MinPort}..{MaxPort}, or 0/null to disable.");
        }

        if (!IsSupportedPlatform()) {
            throw new PlatformNotSupportedException("Remote debugging is only supported on Windows and Linux in InfiniFrame.");
        }
    }

    /// <summary>
    ///     Composes browser control initialization parameters with remote debugging switches appended.
    /// </summary>
    /// <param name="rawParameters">The raw initialization parameters.</param>
    /// <param name="normalizedPort">The normalized remote debugging port.</param>
    /// <returns>The composed parameters with remote debugging switches, or the sanitized raw parameters if the port is 0.</returns>
    public static string? ComposeBrowserControlInitParameters(string? rawParameters, int normalizedPort) {
        string? sanitized = StripRemoteDebuggingSwitches(rawParameters);
        if (normalizedPort == 0 || !OperatingSystem.IsWindows()) return sanitized;

        string explicitArguments = $"--remote-debugging-address={LoopbackAddress} --remote-debugging-port={normalizedPort}";
        return string.IsNullOrWhiteSpace(sanitized)
            ? explicitArguments
            : $"{sanitized} {explicitArguments}";
    }

    /// <summary>
    ///     Creates a URI for the remote debugging endpoint.
    /// </summary>
    /// <param name="port">The remote debugging port.</param>
    /// <returns>The endpoint URI.</returns>
    public static Uri CreateEndpointUri(int port)
        => new($"http://{LoopbackAddress}:{port}", UriKind.Absolute);

    /// <summary>
    ///     Attempts to probe a remote debugging endpoint to check if it is reachable.
    /// </summary>
    /// <param name="endpoint">The endpoint URI to probe.</param>
    /// <param name="reason">When this method returns, contains the reason if the probe failed.</param>
    /// <returns><c>true</c> if the endpoint is reachable; otherwise, <c>false</c>.</returns>
    public static bool TryProbeEndpoint(Uri endpoint, out string? reason) {
        ArgumentNullException.ThrowIfNull(endpoint);
        reason = null;

        try {
            using var client = new TcpClient();
            IAsyncResult connect = client.BeginConnect(endpoint.Host, endpoint.Port, null, null);
            bool signaled = connect.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(500));
            if (!signaled) {
                client.Close();
                reason = "Timed out while probing endpoint.";
                return false;
            }

            client.EndConnect(connect);
            return client.Connected;
        }
        catch (SocketException ex) {
            reason = $"{ex.SocketErrorCode}";
            return false;
        }
        catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
            reason = ex.Message;
            return false;
        }
    }

    /// <summary>
    ///     Validates that the specified port is available on the loopback interface.
    /// </summary>
    /// <param name="normalizedPort">The normalized port to validate.</param>
    /// <param name="logger">The logger for diagnostic output.</param>
    public static void ValidatePortAvailabilityOrThrow(int normalizedPort, ILogger logger) {
        if (normalizedPort == 0) return;

        try {
            using var listener = new TcpListener(IPAddress.Loopback, normalizedPort);
            listener.Start();
            logger.LogDebug("Remote debugging startup preflight succeeded for loopback port {RemoteDebuggingPort}.", normalizedPort);
        }
        catch (SocketException ex) {
            logger.LogError(ex, "Remote debugging startup preflight failed for loopback port {RemoteDebuggingPort}.", normalizedPort);
            throw new InvalidOperationException(
                $"Remote debugging port {normalizedPort} is unavailable on loopback. Choose a different port or disable remote debugging.",
                ex);
        }
    }

    private static string? StripRemoteDebuggingSwitches(string? rawParameters) {
        if (string.IsNullOrWhiteSpace(rawParameters))
            return null;

        string sanitized = RemoteDebuggingPortSwitchRegex().Replace(rawParameters, " ");
        sanitized = RemoteDebuggingAddressSwitchRegex().Replace(sanitized, " ");
        sanitized = WhitespaceRegex().Replace(sanitized, " ").Trim();

        return string.IsNullOrWhiteSpace(sanitized)
            ? null
            : sanitized;
    }

    [GeneratedRegex(@"(?:^|\s)--remote-debugging-port=\S+")]
    private static partial Regex RemoteDebuggingPortSwitchRegex();

    [GeneratedRegex(@"(?:^|\s)--remote-debugging-address=\S+")]
    private static partial Regex RemoteDebuggingAddressSwitchRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();
}
