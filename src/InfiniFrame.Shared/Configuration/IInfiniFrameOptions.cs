// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameOptions {
    InfiniFrameNativeParameters StartupParameters { get; }
    
    bool LimitLinuxWindowTitleLength { get; set; }
    IInfiniFrameWindow? ParentWindow { get; }
    List<IInfiniFrameWindow> ChildWindows { get; }
}
