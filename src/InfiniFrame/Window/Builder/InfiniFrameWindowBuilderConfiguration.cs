// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilderConfiguration : IInfiniFrameWindowBuilderConfiguration {
    public IInfiniFrameWindow? ParentWindow { get; set; }
    public List<IInfiniFrameWindow> ChildWindows { get; init; } = [];

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters) {
        if (ParentWindow is not null) parameters.NativeParent = ParentWindow.InstanceHandle;
    }
}
