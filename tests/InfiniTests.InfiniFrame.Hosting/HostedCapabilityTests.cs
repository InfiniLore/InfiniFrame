// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using System.Reflection;
using InfiniFrame;
using InfiniFrame.Hosting;
using InfiniFrame.NativeBridge.Parameters.Window;
using InfiniFrame.Window.Hosting;

namespace InfiniTests.InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// Capabilities: supported configuration works, explicit unsupported requests fail early and clearly,
// and nothing is silently ignored.
[NotInParallel]
public sealed class HostedCapabilityTests {
    [Test]
    public async Task EveryNativeWindowParameter_IsClassifiedDeliberately(CancellationToken ct) {
        string[] fields = [.. typeof(InfiniFrameNativeWindowParameters)
            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(static field => field.Name)];
        IReadOnlySet<string>[] groups = [
            InfiniFrameHostedWindowRequestFactory.MappedFields,
            InfiniFrameHostedWindowRequestFactory.RejectedUnlessDefaultFields,
            InfiniFrameHostedWindowRequestFactory.CoreOwnedFields
        ];
        string[] unclassified = [.. fields.Where(field => !groups.Any(group => group.Contains(field)))];
        string[] duplicated = [.. fields.Where(field => groups.Count(group => group.Contains(field)) > 1)];
        string[] stale = [.. groups.SelectMany(static group => group).Where(name => !fields.Contains(name))];
        await Assert.That(string.Join(",", unclassified)).IsEqualTo(string.Empty);
        await Assert.That(string.Join(",", duplicated)).IsEqualTo(string.Empty);
        await Assert.That(string.Join(",", stale)).IsEqualTo(string.Empty);
    }

    [Test]
    [Timeout(30_000)]
    public async Task UnsupportedSettings_AreRejectedBeforeTheProviderIsCalled(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        NotSupportedException exception = await Expect.ThrowsAsync<NotSupportedException>(() => Task.Run(() => harness.Build(builder => builder
            .SetChromeless(true)
            .SetTopMost(true)
            .SetMinSize(100, 100)
            .RegisterNavigationStartingHandler((_, _) => NavigationStartingResult.Allow)), ct));

        await Assert.That(exception.Message).Contains("Chromeless");
        await Assert.That(exception.Message).Contains("Topmost");
        await Assert.That(exception.Message).Contains("MinWidth");
        await Assert.That(exception.Message).Contains("NavigationStarting");
        await Assert.That(exception.Message).Contains("No default-engine fallback");
        await Assert.That(harness.Host.Windows.Count).IsEqualTo(0);
    }

    [Test]
    [Timeout(30_000)]
    public async Task RawHtmlStartPage_AndUntrustedStartUrl_AreRejected(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        NotSupportedException raw = await Expect.ThrowsAsync<NotSupportedException>(() => Task.Run(() =>
            harness.Build(builder => builder.SetStartPageContent("<p>inline</p>")), ct));
        await Assert.That(raw.Message).Contains("StartString");

        NotSupportedException untrusted = await Expect.ThrowsAsync<NotSupportedException>(() => Task.Run(() =>
            harness.Build(builder => builder.SetStartPageUrl("https://example.invalid/")), ct));
        await Assert.That(untrusted.Message).Contains("not a trusted origin");
    }

    [Test]
    [Timeout(30_000)]
    public async Task HandlersNeedingUndeclaredNotifications_AreRejected(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync(new ScriptedHostProvider(InfiniFrameHostCapabilities.None));
        NotSupportedException exception = await Expect.ThrowsAsync<NotSupportedException>(() => Task.Run(() => harness.Build(builder => builder
            .RegisterMaximizedHandler(_ => {})
            .RegisterSizeChangedHandler((_, _) => {})
            .RegisterFocusInHandler(_ => {})
            .RegisterWindowClosingHandler((_, _) => WindowClosingResult.Close)
            .SetMaximized(true)), ct));

        await Assert.That(exception.Message).Contains(nameof(InfiniFrameHostCapabilities.StateEvents));
        await Assert.That(exception.Message).Contains(nameof(InfiniFrameHostCapabilities.BoundsEvents));
        await Assert.That(exception.Message).Contains(nameof(InfiniFrameHostCapabilities.FocusEvents));
        await Assert.That(exception.Message).Contains(nameof(InfiniFrameHostCapabilities.CloseCancellation));
        await Assert.That(exception.Message).Contains(nameof(InfiniFrameHostCapabilities.WindowState));
    }

    [Test]
    [Timeout(30_000)]
    public async Task SupportedConfiguration_IsMappedIntoThePublicRequest(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow _, ScriptedWindowHostCore host) = await harness.CreateAsync(builder => builder
            .SetTitle("Hosted title")
            .SetSize(1024, 700)
            .SetLocation(30, 40)
            .SetMaximized(true)
            .SetUserAgent("contract-test")
            .EnableDevTools(false)
            .RegisterCustomSchemeHandler("App", (_, _) => (null, null)));

        InfiniFrameHostWindowRequest request = host.Request;
        await Assert.That(request.Title).IsEqualTo("Hosted title");
        await Assert.That(request.InitialSize).IsEqualTo(new Size(1024, 700));
        await Assert.That(request.InitialLocation).IsEqualTo(new Point(30, 40));
        await Assert.That(request.InitialState).IsEqualTo(InfiniFrameHostWindowState.Maximized);
        await Assert.That(request.BrowserPolicy.UserAgent).IsEqualTo("contract-test");
        await Assert.That(request.BrowserPolicy.DevTools).IsFalse();
        await Assert.That(request.CustomSchemes.Contains("app")).IsTrue();
        await Assert.That(request.TrustedOrigins.Any(static origin => origin.Host == "localhost")).IsTrue();
        await Assert.That(request.BridgeContractVersion).IsEqualTo(InfiniFrameHostContract.BridgeContractVersion);
        await Assert.That(request.BridgeScript).Contains("window.infiniframe");
    }

    [Test]
    [Timeout(30_000)]
    public async Task DeclaredCapability_IsServedByTheProvider_OnItsOwnThread(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore core) = await harness.CreateAsync();
        var host = (ScriptedWindowHost)core;
        await window.WaitForReadyAsync(ct);

        window.SetSize(640, 480);
        window.SetLocation(5, 6);
        window.SetTitle("renamed");
        window.SetMaximized();
        await Assert.That(window.Size).IsEqualTo(new Size(640, 480));
        await Assert.That(window.Location).IsEqualTo(new Point(5, 6));
        await Assert.That(window.Title).IsEqualTo("renamed");
        await Assert.That(window.IsMaximized).IsTrue();
        await Assert.That(host.LastOperationThreadId).IsEqualTo(harness.Host.LoopThreadId);
    }

    [Test]
    [Timeout(30_000)]
    public async Task UndeclaredCapability_ThrowsNotSupported_AndTheWindowStaysHealthy(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync(new ScriptedHostProvider(InfiniFrameHostCapabilities.None));
        (InfiniFrameHostedWindow window, _) = await harness.CreateAsync();
        await window.WaitForReadyAsync(ct);

        await Assert.That(() => window.SetSize(640, 480)).Throws<NotSupportedException>();
        await Assert.That(() => window.SetTitle("x")).Throws<NotSupportedException>();
        await Assert.That(() => window.SetMinimized()).Throws<NotSupportedException>();
        await Assert.That(() => ((IInfiniFrameWindow)window).Features.Menu).Throws<NotSupportedException>();
        await Assert.That(() => ((IInfiniFrameWindow)window).Features.FilePickerDialogs).Throws<NotSupportedException>();
        await Assert.That(() => window.WindowHandle).Throws<NotSupportedException>();
        await Assert.That(() => window.LoadRawString("<p/>")).Throws<NotSupportedException>();
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Ready);
        await Assert.That(window.IsResizable).IsTrue();
    }

    [Test]
    [Timeout(30_000)]
    public async Task DeclaredButUnimplementedCapability_FailsCreationAndRollsBack(CancellationToken ct) {
        var provider = new ScriptedHostProvider { ImplementCapabilityInterfaces = false };
        await using HostedHarness harness = await HostedHarness.StartAsync(provider);
        InfiniFrameHostedWindow window = harness.Build();

        InvalidOperationException exception = await Expect.ThrowsAsync<InvalidOperationException>(() => harness.Runtime.CreateWindowsAsync([window], ct));
        await Assert.That(exception.Message).Contains(nameof(IInfiniFrameWindowHostBounds));
        ScriptedWindowHostCore host = harness.Host.Windows[0];
        await Assert.That(host.RequestCloseCalls).IsEqualTo(1);
        await Assert.That(host.DisposeCalls).IsEqualTo(1);
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Disposed);
    }

    [Test]
    [Timeout(30_000)]
    public async Task StateAndBoundsNotifications_RaiseTheExistingWindowEvents(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        var events = new System.Collections.Concurrent.ConcurrentQueue<string>();
        (InfiniFrameHostedWindow window, _) = await harness.CreateAsync(builder => builder
            .RegisterMaximizedHandler(_ => events.Enqueue("maximized"))
            .RegisterRestoredHandler(_ => events.Enqueue("restored"))
            .RegisterSizeChangedHandler((_, size) => events.Enqueue($"size:{size.Width}x{size.Height}"))
            .RegisterFocusInHandler(_ => events.Enqueue("focus-in")));
        await window.WaitForReadyAsync(ct);

        window.SetMaximized();
        window.SetMaximized(false);
        window.SetSize(300, 200);
        window.SetFocused();
        await harness.DrainDispatcherAsync();

        await Assert.That(string.Join(",", events)).IsEqualTo("maximized,restored,size:300x200,focus-in");
    }

    [Test]
    public async Task UnknownContractMajorOrNewerMinor_IsRejected(CancellationToken ct) {
        await Assert.That(() => new InfiniFrameHostedRuntime(new ScriptedHostProvider { ContractVersion = new Version(2, 0) }, Guid.NewGuid()))
            .Throws<NotSupportedException>();
        await Assert.That(() => new InfiniFrameHostedRuntime(new ScriptedHostProvider { ContractVersion = new Version(1, 1) }, Guid.NewGuid()))
            .Throws<NotSupportedException>();
    }
}
