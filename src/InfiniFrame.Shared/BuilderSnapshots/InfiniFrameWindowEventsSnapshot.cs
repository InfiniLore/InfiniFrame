// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame.BuilderSnapshots;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal readonly record struct InfiniFrameWindowEventsSnapshot(
    Action<IInfiniFrameWindow, Point>[] WindowLocationChanged,
    Action<IInfiniFrameWindow, Size>[] WindowSizeChanged,
    Action<IInfiniFrameWindow>[] WindowFocusIn,
    Action<IInfiniFrameWindow>[] WindowMaximized,
    Action<IInfiniFrameWindow>[] WindowRestored,
    Action<IInfiniFrameWindow>[] WindowFocusOut,
    Action<IInfiniFrameWindow>[] WindowMinimized,
    Action<IInfiniFrameWindow, string>[] WebMessageReceived,
    Action<IInfiniFrameWindow>[] WindowClosingRequested,
    Func<IInfiniFrameWindow, EventArgs?, bool>[] WindowClosing,
    Action<IInfiniFrameWindow>[] WindowCreating,
    Action<IInfiniFrameWindow>[] WindowCreated
);
