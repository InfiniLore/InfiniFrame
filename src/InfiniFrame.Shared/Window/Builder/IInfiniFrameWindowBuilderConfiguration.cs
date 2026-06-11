// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilderConfiguration {
    IInfiniFrameWindow? ParentWindow { get; }
    List<IInfiniFrameWindow> ChildWindows { get; }
    
    void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters);
}
