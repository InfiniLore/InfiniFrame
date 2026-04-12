// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Js.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class WindowRegistrationStateMachine {
    private WindowRegistrationHandshakeState HandshakeState { get; set; } = WindowRegistrationHandshakeState.ReadyPending;
    private bool RegistrationSendInProgress { get; set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public bool TryBeginRegistrationSendOnReady() {
        if (RegistrationSendInProgress) return false;
        if (HandshakeState == WindowRegistrationHandshakeState.Sent) return false;

        HandshakeState = WindowRegistrationHandshakeState.ReadyReceived;
        RegistrationSendInProgress = true;
        return true;
    }

    public void CompleteRegistrationSend(bool success) {
        RegistrationSendInProgress = false;
        HandshakeState = success
            ? WindowRegistrationHandshakeState.Sent
            : WindowRegistrationHandshakeState.Failed;
    }

    public bool ShouldLogReadyHandshakeTimeout() {
        return HandshakeState == WindowRegistrationHandshakeState.ReadyPending;
    }
}
