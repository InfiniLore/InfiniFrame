// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static partial class RemoteDebuggingUtility {
    public const int MinPort = 1;
    public const int MaxPort = 65535;

    private const string LoopbackAddress = "127.0.0.1";
    public static int? NormalizePort(int? port, string parameterName = "port") {
        if (port is null or 0) return null;
        if (port < MinPort || port > MaxPort)
            throw new ArgumentOutOfRangeException(parameterName, port, $"Remote debugging port must be {MinPort}..{MaxPort}, or 0/null to disable.");

        return port;
    }

    public static bool IsSupportedPlatform() => OperatingSystem.IsWindows();

    public static void EnsureSupportedPlatform(int? normalizedPort) {
        if (!normalizedPort.HasValue) return;
        if (IsSupportedPlatform()) return;

        throw new PlatformNotSupportedException("Remote debugging is only supported on Windows in InfiniFrame.");
    }

    public static string? ComposeBrowserControlInitParameters(string? rawParameters, int? normalizedPort) {
        string? sanitized = StripRemoteDebuggingSwitches(rawParameters);
        if (!normalizedPort.HasValue)
            return sanitized;

        string explicitArguments = $"--remote-debugging-address={LoopbackAddress} --remote-debugging-port={normalizedPort.Value}";
        return string.IsNullOrWhiteSpace(sanitized)
            ? explicitArguments
            : $"{sanitized} {explicitArguments}";
    }

    public static Uri CreateEndpointUri(int port) => new($"https://{LoopbackAddress}:{port}", UriKind.Absolute);

    public static void ValidatePortAvailabilityOrThrow(int? normalizedPort, ILogger logger) {
        if (!normalizedPort.HasValue)
            return;

        int port = normalizedPort.Value;

        try {
            using var listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            logger.LogDebug("Remote debugging startup preflight succeeded for loopback port {RemoteDebuggingPort}.", port);
        }
        catch (SocketException ex) {
            logger.LogError(ex, "Remote debugging startup preflight failed for loopback port {RemoteDebuggingPort}.", port);
            throw new InvalidOperationException(
                $"Remote debugging port {port} is unavailable on loopback. Choose a different port or disable remote debugging.",
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

    [GeneratedRegex(@"(?:^|\s)--remote-debugging-port=\S+", RegexOptions.Compiled)]
    private static partial Regex RemoteDebuggingPortSwitchRegex();

    [GeneratedRegex(@"(?:^|\s)--remote-debugging-address=\S+", RegexOptions.Compiled)]
    private static partial Regex RemoteDebuggingAddressSwitchRegex();

    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    private static partial Regex WhitespaceRegex();
}
