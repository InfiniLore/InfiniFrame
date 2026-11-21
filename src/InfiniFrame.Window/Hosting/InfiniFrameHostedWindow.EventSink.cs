// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using InfiniFrame.Hosting;

namespace InfiniFrame.Window.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed partial class InfiniFrameHostedWindow {
    /// <summary>
    ///     The object handed to the provider. It is deliberately not the facade itself, so a provider holding it cannot
    ///     reach the window surface or its internal lifecycle members.
    /// </summary>
    private sealed class HostEventSink(InfiniFrameHostedWindow window) : IInfiniFrameWindowHostEvents {
        public void Created() => window.OnHostCreated();
        public void DocumentAttached(InfiniFrameDocumentId document, Uri url) => window.OnHostDocumentAttached(document, url);
        public void DocumentRetired(InfiniFrameDocumentId document) => window.OnHostDocumentRetired(document);
        public void MessageReceived(InfiniFrameDocumentId document, string message) => window.OnHostMessageReceived(document, message);

        public ValueTask<InfiniFrameHostResourceResponse> GetResourceAsync(InfiniFrameHostResourceRequest request, CancellationToken cancellationToken) =>
            window.OnHostGetResourceAsync(request, cancellationToken);

        public void BoundsChanged(Rectangle bounds) => window.OnHostBoundsChanged(bounds);
        public void StateChanged(InfiniFrameHostWindowState state) => window.OnHostStateChanged(state);
        public ValueTask<bool> CloseRequestedAsync(CancellationToken cancellationToken) => window.OnHostCloseRequestedAsync(cancellationToken);

        public void Faulted(Exception exception) =>
            window.FaultWindow(exception ?? new InvalidOperationException("The host provider reported a window fault without an exception."));

        public void Closed() => window.OnHostClosed();
    }
}
