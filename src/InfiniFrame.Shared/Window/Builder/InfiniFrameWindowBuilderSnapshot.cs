// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Security;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal readonly record struct InfiniFrameWindowBuilderSnapshot(
    IInfiniFrameEventsStore EventsStore,
    IInfiniFrameStaticAssets? StaticAssets,
    IInfiniFrameUriSecurityPolicy UriSecurityPolicy
);
