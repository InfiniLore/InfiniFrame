// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class WindowRegistrationStateMachine {
    private readonly object _lock = new();
    private WindowRegistrationHandshakeState _handshakeState = WindowRegistrationHandshakeState.ReadyPending;
    private bool _registrationSendInProgress;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public bool TryBeginRegistrationSendOnReady() {
        lock (_lock) {
            if (_registrationSendInProgress) return false;
            if (_handshakeState == WindowRegistrationHandshakeState.ReadyAcknowledged) return false;

            _handshakeState = WindowRegistrationHandshakeState.RegistrationSending;
            _registrationSendInProgress = true;
            return true;
        }
    }

    public void CompleteRegistrationSend(bool success) {
        lock (_lock) {
            _registrationSendInProgress = false;
            _handshakeState = success
                ? WindowRegistrationHandshakeState.ReadyAcknowledged
                : WindowRegistrationHandshakeState.Failed;
        }
    }

    public bool IsReadyPending() {
        lock (_lock) {
            return _handshakeState == WindowRegistrationHandshakeState.ReadyPending;
        }
    }
}