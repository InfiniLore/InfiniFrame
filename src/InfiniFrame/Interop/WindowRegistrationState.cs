// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Holds the registration state for a single window, including the underlying handshake state machine.
/// </summary>
public sealed class WindowRegistrationState {
    internal WindowRegistrationStateMachine StateMachine { get; } = new();
}
