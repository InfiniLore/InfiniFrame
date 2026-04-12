// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Js.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class WindowRegistrationState {
    public bool ReadyReceived { get; set; }
    public bool RegistrationSendInProgress { get; set; }
    public bool RegistrationSent { get; set; }
    public CancellationTokenSource? HandshakeTimeoutCancellationSource { get; set; }
}
