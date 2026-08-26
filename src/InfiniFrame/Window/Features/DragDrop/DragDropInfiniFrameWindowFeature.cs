// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics;
using InfiniFrame.NativeBridge;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides drag and drop functionality for the window.
/// </summary>
public class DragDropInfiniFrameWindowFeature(
    IInfiniFrameWindow window,
    ILogger<DragDropInfiniFrameWindowFeature> logger
) : IDragDropInfiniFrameWindowFeature {
    private List<string> _allowedExtensions = new();

    /// <inheritdoc cref="IDragDropInfiniFrameWindowFeature.IsEnabled" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsEnabled { get; private set; }

    /// <inheritdoc cref="IDragDropInfiniFrameWindowFeature.AllowedExtensions" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IReadOnlyList<string> AllowedExtensions => _allowedExtensions.AsReadOnly();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IDragDropInfiniFrameWindowFeature.SetEnabled" />
    public void SetEnabled(bool enabled) {
        logger.LogDebug(".SetDragDropEnabled({Enabled})", enabled);

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetDragDropEnabled,
            enabled
        );
        IsEnabled = enabled;
    }

    /// <inheritdoc cref="IDragDropInfiniFrameWindowFeature.SetAllowedExtensions" />
    public void SetAllowedExtensions(IReadOnlyList<string> extensions) {
        _allowedExtensions = new List<string>(extensions);
    }
}
