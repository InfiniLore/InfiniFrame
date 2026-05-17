// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.Security;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal readonly record struct InfiniFrameWindowBuilderSnapshot(
    InfiniFrameNativeParameters StartupParameters,
    IInfiniFrameEventsStore EventsStore,
    IInfiniFrameStaticAssets? StaticAssets,
    IInfiniFrameUriSecurityPolicy UriSecurityPolicy
);
