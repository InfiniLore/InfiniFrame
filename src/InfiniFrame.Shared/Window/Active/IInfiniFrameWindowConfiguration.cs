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
    IInfiniFrameWindow? ParentWindow { get; internal set; }
    List<IInfiniFrameWindow> ChildWindows { get; }
    
    internal void AssignNativeParameters(InfiniFrameNativeParameters nativeParameters);
}
