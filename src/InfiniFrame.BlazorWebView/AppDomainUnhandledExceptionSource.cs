// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class AppDomainUnhandledExceptionSource : IInfiniFrameUnhandledExceptionSource {
    /// <inheritdoc cref="IInfiniFrameUnhandledExceptionSource.Register" />
    public IDisposable Register(UnhandledExceptionEventHandler handler) {
        ArgumentNullException.ThrowIfNull(handler);
        AppDomain.CurrentDomain.UnhandledException += handler;
        return new Subscription(handler);
    }

    private sealed class Subscription(UnhandledExceptionEventHandler handler) : IDisposable {
        private UnhandledExceptionEventHandler? _handler = handler;

        // -----------------------------------------------------------------------------------------------------------------
        // Methods
        // -----------------------------------------------------------------------------------------------------------------
        public void Dispose() {
            UnhandledExceptionEventHandler? handler = Interlocked.Exchange(ref _handler, null);
            if (handler is null) return;

            AppDomain.CurrentDomain.UnhandledException -= handler;
        }
    }
}
