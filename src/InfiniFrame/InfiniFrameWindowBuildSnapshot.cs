// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal readonly record struct InfiniFrameWindowBuildSnapshot(
    InfiniFrameNativeParameters StartupParameters,
    InfiniFrameWindowEvents Events,
    InfiniFrameWindowMessageHandlers MessageHandlers,
    Dictionary<string, NetCustomSchemeDelegate?> CustomSchemes,
    StaticAssetSettings? StaticAssets
);
