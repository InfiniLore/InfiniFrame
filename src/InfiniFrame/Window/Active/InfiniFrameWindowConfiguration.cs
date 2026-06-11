// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowConfiguration : IInfiniFrameWindowConfiguration {
    public InfiniFrameNativeParameters StartupParameters { get; init; }
    public IInfiniFrameWindow? ParentWindow { get; init; }
    public List<IInfiniFrameWindow> ChildWindows { get; init; } = [];
}
