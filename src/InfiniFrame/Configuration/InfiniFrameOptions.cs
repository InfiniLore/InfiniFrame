// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;

namespace InfiniFrame;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameOptions(IInfiniFrameOptionsBuilder configuration, ref InfiniFrameNativeParameters parameters) : IInfiniFrameOptions {
    public InfiniFrameNativeParameters StartupParameters { get; } = parameters;

    public bool LimitLinuxWindowTitleLength { get; set; } = configuration.LimitLinuxWindowTitleLength;
    public IInfiniFrameWindow? ParentWindow { get; } = configuration.ParentWindow;
    public List<IInfiniFrameWindow> ChildWindows { get; } = configuration.ChildWindows.ToList();
}
