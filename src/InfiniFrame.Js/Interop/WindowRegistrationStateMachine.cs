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
        if (HandshakeState == WindowRegistrationHandshakeState.ReadyAcknowledged) return false;

        HandshakeState = WindowRegistrationHandshakeState.RegistrationSending;
        RegistrationSendInProgress = true;
        return true;
    }

    public void CompleteRegistrationSend(bool success) {
        RegistrationSendInProgress = false;
        HandshakeState = success
            ? WindowRegistrationHandshakeState.ReadyAcknowledged
            : WindowRegistrationHandshakeState.Failed;
    }

    public bool IsReadyPending() 
        => HandshakeState == WindowRegistrationHandshakeState.ReadyPending;
}
