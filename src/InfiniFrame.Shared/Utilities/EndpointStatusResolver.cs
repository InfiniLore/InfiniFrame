// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;

namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Pure logic for determining the remote debugging endpoint status.
///     Extracted from <see cref="DebuggingInfiniFrameWindowFeature"/> for testability.
/// </summary>
public static class EndpointStatusResolver {

    /// <summary>
    ///     Determines the endpoint status from a set of conditions.
    /// </summary>
    public static InfiniFrameDebugEndpointStatus Resolve(
        bool isPlatformSupported,
        int? remoteDebuggingPort,
        bool isWindowClosed,
        bool hasEndpoint,
        bool probeSucceeded,
        string? probeReason
    ) {
        if (!isPlatformSupported)
            return InfiniFrameDebugEndpointStatus.NotSupported;

        if (!remoteDebuggingPort.HasValue)
            return InfiniFrameDebugEndpointStatus.Disabled;

        if (isWindowClosed || !hasEndpoint)
            return InfiniFrameDebugEndpointStatus.Unavailable;

        if (probeSucceeded)
            return InfiniFrameDebugEndpointStatus.Reachable;

        if (string.IsNullOrWhiteSpace(probeReason))
            return InfiniFrameDebugEndpointStatus.Configured;

        return InfiniFrameDebugEndpointStatus.Unreachable;
    }
}
