// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Thrown when another instance of the application is already running and instance arbitration
///     is configured to prevent multiple instances.
/// </summary>
public sealed class InstanceAlreadyRunningException()
    : InvalidOperationException("Another instance of the application is already running.");
