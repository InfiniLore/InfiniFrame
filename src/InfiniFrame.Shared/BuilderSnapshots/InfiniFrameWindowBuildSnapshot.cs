// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;

namespace InfiniFrame.BuilderSnapshots;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal readonly record struct InfiniFrameWindowBuildSnapshot(
    InfiniFrameNativeParameters StartupParameters,
    IInfiniFrameEventsStore EventsStore,
    StaticAssetSettings? StaticAssets,
    InfiniFrameUriSecurityPolicy UriSecurityPolicy
);
