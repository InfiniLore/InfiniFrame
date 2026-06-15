// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowFeatureInvoke(
    IInfiniFrameWindow window,
    ILogger<InfiniFrameWindowFeatureInvoke> logger
) : IInfiniFrameWindowFeatureInvoke {
    /// <summary>
    /// Executes a provided callback function on the native window thread with appropriate validation and synchronization.
    /// </summary>
    /// <param name="callback">
    /// The action to be executed. Must not be null.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the <paramref name="callback"/> parameter is null.
    /// </exception>
    /// <remarks>
    /// This method ensures proper validation and synchronization, enabling the safe execution of managed code
    /// in the context of the native window thread. It uses the window's instance handle and managed thread ID
    /// for accurate execution.
    /// </remarks>
    public void Invoke(Action callback) {
        ArgumentNullException.ThrowIfNull(callback);
        
        NativeInvoke.InvokeSyncWithValidation(logger, window.InstanceHandle, window.ManagedThreadId, callback);
    }
}
