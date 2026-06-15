// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowConfiguration : IInfiniFrameWindowConfiguration {
    public InfiniFrameNativeParameters StartupParameters { get; private set; }
    public IInfiniFrameWindow? ParentWindow { get; set; }
    public List<IInfiniFrameWindow> ChildWindows { get; } = [];
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void AssignNativeParameters(InfiniFrameNativeParameters nativeParameters) {
        StartupParameters = nativeParameters;
    }
}
