// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Js.MessageHandlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class WindowRegistrationState {
    public bool ReadyReceived { get; set; }
    public bool RegistrationSent { get; set; }
    public CancellationTokenSource? HandshakeTimeoutCancellationSource { get; set; }
}