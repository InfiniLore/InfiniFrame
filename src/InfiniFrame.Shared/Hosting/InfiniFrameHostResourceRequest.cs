// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>One browser request for a registered custom scheme.</summary>
public sealed class InfiniFrameHostResourceRequest {
    /// <summary>The HTTP method. InfiniFrame serves <c>GET</c> and <c>HEAD</c> and answers others with 405.</summary>
    public required string Method { get; init; }

    /// <summary>The absolute request URL as reported by the browser.</summary>
    public required Uri Url { get; init; }
}
