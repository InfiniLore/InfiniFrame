// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowConfiguration {
    InfiniFrameNativeParameters StartupParameters { get; }
    IInfiniFrameWindow? ParentWindow { get; }
    List<IInfiniFrameWindow> ChildWindows { get; }
}
