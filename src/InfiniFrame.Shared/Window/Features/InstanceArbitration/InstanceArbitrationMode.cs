// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Defines the instance arbitration mode for single-instance enforcement.
/// </summary>
public enum InstanceArbitrationMode {
    /// <summary>
    ///     Instance arbitration is disabled. Multiple instances can run simultaneously.
    /// </summary>
    Disabled = 0,

    /// <summary>
    ///     Only the primary instance is allowed. A secondary instance throws <see cref="InvalidOperationException"/>.
    /// </summary>
    PrimaryOnly = 1,

    /// <summary>
    ///     Only the primary instance is allowed, with command-line argument forwarding to the primary.
    /// </summary>
    PrimaryWithArgForwarding = 2
}
