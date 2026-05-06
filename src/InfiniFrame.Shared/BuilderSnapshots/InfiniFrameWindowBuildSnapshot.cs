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
    IInfiniFrameWindowEventsStore EventsStore,
    StaticAssetSettings? StaticAssets,
    InfiniFrameUriSecurityPolicy UriSecurityPolicy
);
