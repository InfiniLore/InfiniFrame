// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>A provider could not complete a requested native operation (for example, the window system refused it).</summary>
public sealed class InfiniFrameHostOperationException : InvalidOperationException {
    /// <summary>Creates the exception.</summary>
    public InfiniFrameHostOperationException(string message, Exception? innerException = null)
        : base(message, innerException) {}
}
