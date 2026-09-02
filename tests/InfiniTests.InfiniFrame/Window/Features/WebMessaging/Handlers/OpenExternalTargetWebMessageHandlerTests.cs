// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics;
using InfiniFrame;
using InfiniFrame.Interop;
using InfiniFrame.NativeBridge.Handles;
using InfiniFrame.NativeBridge.Parameters;
using InfiniTests.Substitutes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniTests.InfiniFrame.Window.Features.WebMessaging.Handlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class OpenExternalTargetWebMessageHandlerTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Null / Empty Payload Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    [Arguments("\t")]
    public async Task HandleWebMessage_NullOrEmptyPayload_IsIgnored(string? payload) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterOpenExternalTargetWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.OpenExternal, payload));

        // Assert: no exception and no crash; handler returns silently
        await Assert.That(window.GetSentMessagesSnapshot().Count).IsEqualTo(0);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Malformed URL Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments("not-a-url")]
    [Arguments("://missing-scheme")]
    [Arguments("http://")]
    public async Task HandleWebMessage_MalformedUrl_IsRejected(string payload) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterOpenExternalTargetWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.OpenExternal, payload));

        // Assert: handler returns silently without opening browser
        await Assert.That(window.GetSentMessagesSnapshot().Count).IsEqualTo(0);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Non-Absolute URI Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments("relative/path")]
    [Arguments("/absolute/without/scheme")]
    public async Task HandleWebMessage_NonAbsoluteUri_IsRejected(string payload) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterOpenExternalTargetWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.OpenExternal, payload));

        // Assert: handler returns silently without opening browser
        await Assert.That(window.GetSentMessagesSnapshot().Count).IsEqualTo(0);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Loopback IP Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments("http://127.0.0.1/")]
    [Arguments("http://localhost/")]
    [Arguments("http://[::1]/")]
    [Arguments("https://127.0.0.1/")]
    [Arguments("https://localhost/")]
    [Arguments("https://[::1]/")]
    public async Task HandleWebMessage_LoopbackIp_IsBlocked(string payload) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterOpenExternalTargetWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.OpenExternal, payload));

        // Assert: handler returns silently without opening browser
        await Assert.That(window.GetSentMessagesSnapshot().Count).IsEqualTo(0);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Private IP Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments("http://10.0.0.1/")]
    [Arguments("http://10.255.255.255/")]
    [Arguments("http://172.16.0.1/")]
    [Arguments("http://172.31.255.255/")]
    [Arguments("http://192.168.1.1/")]
    [Arguments("http://192.168.0.1/")]
    [Arguments("https://10.0.0.1/")]
    [Arguments("https://172.16.0.1/")]
    [Arguments("https://192.168.1.1/")]
    public async Task HandleWebMessage_PrivateIp_IsBlocked(string payload) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterOpenExternalTargetWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.OpenExternal, payload));

        // Assert: handler returns silently without opening browser
        await Assert.That(window.GetSentMessagesSnapshot().Count).IsEqualTo(0);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Public URL Tests (with mock launcher - no real browser opens)
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task HandleWebMessage_PublicUrl_Http_OpensBrowser(CancellationToken ct = default) {
        // Arrange
        RecordingExternalProcessLauncher launcher = new();
        IServiceProvider serviceProvider = CreateServiceProvider(launcher);
        var builder = new InfiniFrameWindowBuilder();
        builder.RegisterOpenExternalTargetWebMessageHandler();
        var eventsStore = (InfiniFrameEventsStore)builder.EventsStore;
        var events = new InfiniFrameEvents(eventsStore, NullLogger<InfiniFrameEvents>.Instance);
        var nativeParameters = default(InfiniFrameNativeParameters);
        events.AssignToNativeParameters(ref nativeParameters);

        var windowStub = new WindowWithServiceProviderStub(serviceProvider);
        var windowEvents = new InfiniFrameEvents(new InfiniFrameEventsStore(), NullLogger<InfiniFrameEvents>.Instance);
        windowEvents.PopulateFromBuilderEventStore(eventsStore);
        windowEvents.AssignToWindow(windowStub);

        // Act
        windowEvents.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.OpenExternal, "http://example.com"));

        // Assert: launcher was called with the correct URL
        await Assert.That(launcher.LastStartInfo).IsNotNull();
        await Assert.That(launcher.LastStartInfo!.FileName).StartsWith("http://example.com");
        await Assert.That(launcher.CallCount).IsEqualTo(1);
    }

    [Test]
    public async Task HandleWebMessage_PublicUrl_Https_OpensBrowser(CancellationToken ct = default) {
        // Arrange
        RecordingExternalProcessLauncher launcher = new();
        IServiceProvider serviceProvider = CreateServiceProvider(launcher);
        var builder = new InfiniFrameWindowBuilder();
        builder.RegisterOpenExternalTargetWebMessageHandler();
        var eventsStore = (InfiniFrameEventsStore)builder.EventsStore;

        var windowStub = new WindowWithServiceProviderStub(serviceProvider);
        var windowEvents = new InfiniFrameEvents(new InfiniFrameEventsStore(), NullLogger<InfiniFrameEvents>.Instance);
        windowEvents.PopulateFromBuilderEventStore(eventsStore);
        windowEvents.AssignToWindow(windowStub);

        // Act
        windowEvents.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.OpenExternal, "https://example.com"));

        // Assert
        await Assert.That(launcher.LastStartInfo).IsNotNull();
        await Assert.That(launcher.LastStartInfo!.FileName).StartsWith("https://example.com");
        await Assert.That(launcher.CallCount).IsEqualTo(1);
    }

    [Test]
    public async Task HandleWebMessage_MailtoUri_OpensMailClient(CancellationToken ct = default) {
        // Arrange
        RecordingExternalProcessLauncher launcher = new();
        IServiceProvider serviceProvider = CreateServiceProvider(launcher);
        var builder = new InfiniFrameWindowBuilder();
        builder.RegisterOpenExternalTargetWebMessageHandler();
        var eventsStore = (InfiniFrameEventsStore)builder.EventsStore;

        var windowStub = new WindowWithServiceProviderStub(serviceProvider);
        var windowEvents = new InfiniFrameEvents(new InfiniFrameEventsStore(), NullLogger<InfiniFrameEvents>.Instance);
        windowEvents.PopulateFromBuilderEventStore(eventsStore);
        windowEvents.AssignToWindow(windowStub);

        // Act
        windowEvents.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.OpenExternal, "mailto:user@example.com"));

        // Assert
        await Assert.That(launcher.LastStartInfo).IsNotNull();
        await Assert.That(launcher.LastStartInfo!.FileName).IsEqualTo("mailto:user@example.com");
        await Assert.That(launcher.CallCount).IsEqualTo(1);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Disallowed Scheme Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments("javascript:alert(1)")]
    [Arguments("file:///etc/passwd")]
    [Arguments("ftp://example.com")]
    public async Task HandleWebMessage_DisallowedScheme_IsRejected(string payload) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterOpenExternalTargetWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.OpenExternal, payload));

        // Assert: handler returns silently
        await Assert.That(window.GetSentMessagesSnapshot().Count).IsEqualTo(0);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Port Boundary Tests (with mock launcher - no real browser opens)
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task HandleWebMessage_PrivateIp_Boundary172_15x_IsAllowed(CancellationToken ct = default) {
        // Arrange: 172.15.x.x is NOT in the 172.16.0.0/12 private range
        RecordingExternalProcessLauncher launcher = new();
        IServiceProvider serviceProvider = CreateServiceProvider(launcher);
        var builder = new InfiniFrameWindowBuilder();
        builder.RegisterOpenExternalTargetWebMessageHandler();
        var eventsStore = (InfiniFrameEventsStore)builder.EventsStore;

        var windowStub = new WindowWithServiceProviderStub(serviceProvider);
        var windowEvents = new InfiniFrameEvents(new InfiniFrameEventsStore(), NullLogger<InfiniFrameEvents>.Instance);
        windowEvents.PopulateFromBuilderEventStore(eventsStore);
        windowEvents.AssignToWindow(windowStub);

        // Act
        windowEvents.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.OpenExternal, "http://172.15.0.1"));

        // Assert: 172.15.x.x should NOT be blocked
        await Assert.That(launcher.CallCount).IsEqualTo(1);
    }

    [Test]
    public async Task HandleWebMessage_PrivateIp_Boundary172_32x_IsAllowed(CancellationToken ct = default) {
        // Arrange: 172.32.x.x is NOT in the 172.16.0.0/12 private range
        RecordingExternalProcessLauncher launcher = new();
        IServiceProvider serviceProvider = CreateServiceProvider(launcher);
        var builder = new InfiniFrameWindowBuilder();
        builder.RegisterOpenExternalTargetWebMessageHandler();
        var eventsStore = (InfiniFrameEventsStore)builder.EventsStore;

        var windowStub = new WindowWithServiceProviderStub(serviceProvider);
        var windowEvents = new InfiniFrameEvents(new InfiniFrameEventsStore(), NullLogger<InfiniFrameEvents>.Instance);
        windowEvents.PopulateFromBuilderEventStore(eventsStore);
        windowEvents.AssignToWindow(windowStub);

        // Act
        windowEvents.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.OpenExternal, "http://172.32.0.1"));

        // Assert: 172.32.x.x should NOT be blocked
        await Assert.That(launcher.CallCount).IsEqualTo(1);
    }

    [Test]
    public async Task HandleWebMessage_NonPrivate_9x_Ip_IsAllowed(CancellationToken ct = default) {
        // Arrange: 9.x.x.x is not in any private range
        RecordingExternalProcessLauncher launcher = new();
        IServiceProvider serviceProvider = CreateServiceProvider(launcher);
        var builder = new InfiniFrameWindowBuilder();
        builder.RegisterOpenExternalTargetWebMessageHandler();
        var eventsStore = (InfiniFrameEventsStore)builder.EventsStore;

        var windowStub = new WindowWithServiceProviderStub(serviceProvider);
        var windowEvents = new InfiniFrameEvents(new InfiniFrameEventsStore(), NullLogger<InfiniFrameEvents>.Instance);
        windowEvents.PopulateFromBuilderEventStore(eventsStore);
        windowEvents.AssignToWindow(windowStub);

        // Act
        windowEvents.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.OpenExternal, "http://9.9.9.9"));

        // Assert: 9.x.x.x should NOT be blocked
        await Assert.That(launcher.CallCount).IsEqualTo(1);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Helper Methods
    // -----------------------------------------------------------------------------------------------------------------
    private static IServiceProvider CreateServiceProvider(IExternalProcessLauncher launcher) {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        serviceCollection.AddSingleton(launcher);
        return serviceCollection.BuildServiceProvider();
    }

    private static (InfiniFrameWindowBuilder Builder, InfiniFrameEvents Events, RecordingInfiniFrameWindowSubstitute Window) CreateWindowHarness() {
        var builder = new InfiniFrameWindowBuilder();
        var eventsStore = (InfiniFrameEventsStore)builder.EventsStore;

        RecordingInfiniFrameWindowSubstitute window = new RecordingInfiniFrameWindowSubstitute()
            .BindToBuilder(builder);

        var events = new InfiniFrameEvents(eventsStore, NullLogger<InfiniFrameEvents>.Instance);
        var nativeParameters = default(InfiniFrameNativeParameters);
        events.AssignToNativeParameters(ref nativeParameters);
        events.AssignToWindow(window.Window);

        return (builder, events, window);
    }

    /// <summary>
    ///     Test double that records calls instead of opening a real browser.
    /// </summary>
    private sealed class RecordingExternalProcessLauncher : IExternalProcessLauncher {
        public int CallCount { get; private set; }
        public ProcessStartInfo? LastStartInfo { get; private set; }

        public Process? Start(ProcessStartInfo startInfo) {
            CallCount++;
            LastStartInfo = startInfo;
            return null;
        }
    }

    /// <summary>
    ///     Minimal window stub that provides only the ServiceProvider needed by the handler.
    /// </summary>
    private sealed class WindowWithServiceProviderStub(IServiceProvider serviceProvider) : IInfiniFrameWindow {
        IServiceProvider IInfiniFrameWindow.ServiceProvider => serviceProvider;
        IInfiniFrameEvents IInfiniFrameWindow.Events => throw new NotSupportedException();
        IInfiniFrameEventsStore IHasInfiniFrameEventsStore.EventsStore => throw new NotSupportedException();
        IDebuggingInfiniFrameWindowFeature IInfiniFrameWindow.Debugging => throw new NotSupportedException();
        IInfiniFrameWindowConfiguration IInfiniFrameWindow.Configuration => throw new NotSupportedException();
        IInfiniFrameWindowFeatures IInfiniFrameWindow.Features => throw new NotSupportedException();
        IntPtr IInfiniFrameWindow.MainProgramHandle => IntPtr.Zero;
        InfiniFrameWindowLifecycleState IInfiniFrameWindow.LifecycleState => InfiniFrameWindowLifecycleState.Running;
        IntPtr IInfiniFrameWindow.WindowHandle => IntPtr.Zero;
        int IInfiniFrameWindow.ManagedThreadId => Environment.CurrentManagedThreadId;
        Guid IInfiniFrameWindow.Id => Guid.NewGuid();
        NativeHandleLease INativeWindowHandleOwner.AcquireNativeHandle(NativeHandleAccess access) => throw new NotSupportedException();
        void IInfiniFrameWindow.BeginInitialization() => throw new NotSupportedException();
        void IInfiniFrameWindow.AssignNativeHandle(IntPtr handle) => throw new NotSupportedException();
        void IInfiniFrameWindow.MarkReady() => throw new NotSupportedException();
        bool IInfiniFrameWindow.RequestClose() => throw new NotSupportedException();
        void IInfiniFrameWindow.CancelCloseRequest() => throw new NotSupportedException();
        void IInfiniFrameWindow.MarkNativeClosed() => throw new NotSupportedException();
        void IInfiniFrameWindow.MarkTeardownPending() => throw new NotSupportedException();
        void IInfiniFrameWindow.MarkTeardownComplete() => throw new NotSupportedException();
        void IInfiniFrameWindow.MarkNativeHandleReleased() => throw new NotSupportedException();
        void IInfiniFrameWindow.MarkDisposed() => throw new NotSupportedException();
        void IInfiniFrameWindow.ReleaseNativeHandle() => throw new NotSupportedException();
        void IInfiniFrameWindow.SetManagedThreadId(int managedThreadId) => throw new NotSupportedException();
    }
}
