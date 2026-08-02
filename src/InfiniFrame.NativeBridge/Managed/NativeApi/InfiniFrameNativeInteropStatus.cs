// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents the result status of a native InfiniFrame interop call.
/// </summary>
internal enum InfiniFrameNativeInteropStatus {
    /// <summary>
    ///     The operation completed successfully.
    /// </summary>
    Success = 0,
    /// <summary>
    ///     An invalid argument was provided to the native function.
    /// </summary>
    InvalidArgument = 22,
    /// <summary>
    ///     An output parameter was set to an unexpected null value by the native side.
    /// </summary>
    OutParameterSetToInvalidNull = 2001,
    /// <summary>
    ///     The operation failed with a general error.
    /// </summary>
    OperationFailed = 14
}