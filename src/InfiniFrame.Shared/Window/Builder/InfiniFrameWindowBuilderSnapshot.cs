// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Security;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a snapshot of the window builder state at a point in time.
/// </summary>
/// <param name="EventsStore">The events store containing registered event handlers.</param>
/// <param name="StaticAssets">The static assets configuration, if any.</param>
/// <param name="UriSecurityPolicy">The URI security policy.</param>
internal readonly record struct InfiniFrameWindowBuilderSnapshot(
    IInfiniFrameEventsStore EventsStore,
    IInfiniFrameStaticAssets? StaticAssets,
    IInfiniFrameUriSecurityPolicy UriSecurityPolicy
);
