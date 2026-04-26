// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.BuilderSnapshots;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal readonly record struct InfiniFrameWindowMessageHandlersSnapshot(
    KeyValuePair<string, Action<IInfiniFrameWindow, string?>>[] PostDataHandlers,
    KeyValuePair<string, Func<IInfiniFrameWindow, string?, string?>>[] GetDataHandlers
);
