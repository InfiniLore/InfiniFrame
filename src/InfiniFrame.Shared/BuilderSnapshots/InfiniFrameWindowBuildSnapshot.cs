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
    InfiniFrameWindowEventsSnapshot Events,
    InfiniFrameWindowMessageHandlersSnapshot MessageHandlers,
    InfiniFrameWindowCustomSchemeHandlersSnapshot CustomSchemes,
    StaticAssetSettings? StaticAssets
);
