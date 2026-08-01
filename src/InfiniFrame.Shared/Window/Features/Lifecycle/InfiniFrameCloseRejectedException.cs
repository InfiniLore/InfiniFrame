// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>Thrown when a native close attempt is vetoed by a window-closing handler.</summary>
public sealed class InfiniFrameCloseRejectedException() 
    : InvalidOperationException("The window close request was rejected by a window-closing handler.");
