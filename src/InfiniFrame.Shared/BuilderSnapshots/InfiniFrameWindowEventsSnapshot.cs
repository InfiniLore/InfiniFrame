// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame.BuilderSnapshots;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal readonly struct InfiniFrameWindowEventsSnapshot {
    public required Action<IInfiniFrameWindow, string>[] WebMessageReceived {get; init;}
    public required Action<IInfiniFrameWindow>[] WindowClosed {get; init;}
    public required Func<IInfiniFrameWindow, EventArgs?, bool>[] WindowClosing {get; init;}
    public required Action<IInfiniFrameWindow>[] WindowClosingRequested {get; init;}
    public required Action<IInfiniFrameWindow>[] WindowCreated {get; init;}
    public required Action<IInfiniFrameWindow>[] WindowCreating {get; init;}
    public required Action<IInfiniFrameWindow>[] WindowFocusIn {get; init;}
    public required Action<IInfiniFrameWindow>[] WindowFocusOut {get; init;}
    public required Action<IInfiniFrameWindow, Point>[] WindowLocationChanged {get; init;}
    public required Action<IInfiniFrameWindow>[] WindowMaximized {get; init;}
    public required Action<IInfiniFrameWindow>[] WindowMinimized {get; init;}
    public required Action<IInfiniFrameWindow>[] WindowRestored {get; init;}
    public required Action<IInfiniFrameWindow, Size>[] WindowSizeChanged {get; init;}
}
