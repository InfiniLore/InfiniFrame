// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.NativeBridge.Handles;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Identifies which lifecycle operation is acquiring a native instance.
/// </summary>
public enum NativeHandleAccess {
    /// <summary>
    ///     Standard feature-level access. Requires the window to be in the Creating or Ready state.
    /// </summary>
    Feature,
    /// <summary>
    ///     Close-level access. Also allowed when a close is in progress (CloseRequested state).
    /// </summary>
    Close,
    /// <summary>
    ///     Wait-for-exit access. Same as Close, used by teardown paths that must outlive the message loop.
    /// </summary>
    WaitForExit
}
