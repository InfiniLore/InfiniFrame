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
    /// <inheritdoc cref="IInfiniFrameWindowFeatureInvoke.Invoke"/>
    public void Invoke(Action callback) {
        ArgumentNullException.ThrowIfNull(callback);
        
        NativeInvoke.InvokeSyncWithValidation(logger, window.InstanceHandle, window.ManagedThreadId, callback);
    }
}
