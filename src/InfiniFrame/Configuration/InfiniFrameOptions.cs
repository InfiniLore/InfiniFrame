// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;

namespace InfiniFrame;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameOptions : IInfiniFrameOptions {
    public required InfiniFrameNativeParameters StartupParameters { get; init; }
    
    public required bool LimitLinuxWindowTitleLength { get; set; }
}
