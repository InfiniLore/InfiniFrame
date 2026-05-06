// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal readonly record struct InfiniFrameWindowBuilderSnapshot(
    InfiniFrameNativeParameters StartupParameters,
    IInfiniFrameEventsStore EventsStore,
    IInfiniFrameStaticAssets? StaticAssets,
    InfiniFrameUriSecurityPolicy UriSecurityPolicy
);
