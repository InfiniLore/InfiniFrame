// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Describes the status of a remote debugging endpoint.
/// </summary>
public enum InfiniFrameDebugEndpointStatus {
    /// <summary>
    ///     Remote debugging is not supported on this platform.
    /// </summary>
    NotSupported,
    /// <summary>
    ///     Remote debugging is disabled.
    /// </summary>
    Disabled,
    /// <summary>
    ///     Remote debugging is unavailable.
    /// </summary>
    Unavailable,
    /// <summary>
    ///     Remote debugging has been configured.
    /// </summary>
    Configured,
    /// <summary>
    ///     The remote debugging endpoint is reachable.
    /// </summary>
    Reachable,
    /// <summary>
    ///     The remote debugging endpoint is unreachable.
    /// </summary>
    Unreachable,
    /// <summary>
    ///     Probing the remote debugging endpoint failed.
    /// </summary>
    ProbeFailed
}
