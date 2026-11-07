// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using InfiniFrame.BlazorWebView.Hosted;
using InfiniFrame.Window.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniTests.InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// The hosted window's per-document Blazor managers, observed through their bounded counters. The scenarios are shared
// with HostedDocumentIsolationTests.
[NotInParallel]
public sealed class HostedDocumentManagerTests {
    [Test]
    [Timeout(60_000)]
    public async Task EachDocument_GetsOneManager_ARepeatedAttachCreatesNone_AndEachIsDisposedOnce(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        InfiniFrameWebViewManager manager = ManagerOf(s);
        await s.LoadPageAsync(1);
        await s.Host.PostAsync(1, BlazorDocumentScenario.AttachPage);// the same document's page attaches again
        await Waits.UntilAsync(() => s.Pages.Count == 2, "the repeated attach created its page");
        HostedDocumentDiagnostics d = manager.GetHostedDocumentDiagnostics();
        Console.WriteLine($"MANAGERS after a repeated attach: {d}");
        await Assert.That(d.ManagersCreated).IsEqualTo(1);
        await Assert.That(d.RepeatedAttaches).IsEqualTo(1);
        await Assert.That(d.AttachMessages).IsEqualTo(2);
        await Assert.That(d.LiveManagers).IsEqualTo(1);
        await Assert.That(d.LatestLiveDocument).IsEqualTo(1UL);

        await s.Host.RetireAsync(1);
        await s.LoadPageAsync(2);
        await Waits.UntilAsync(() => manager.GetHostedDocumentDiagnostics().ManagersDisposed == 1, "document 1's manager disposed", 10_000);
        d = manager.GetHostedDocumentDiagnostics();
        Console.WriteLine($"MANAGERS after the reload: {d}");
        await Assert.That(d.ManagersCreated).IsEqualTo(2);
        await Assert.That(d.ManagersRetired).IsEqualTo(1);
        await Assert.That(d.LiveManagers).IsEqualTo(1);
        await Assert.That(d.LatestLiveDocument).IsEqualTo(2UL);

        await s.CloseAsync(ct);
        await Waits.UntilAsync(() => manager.GetHostedDocumentDiagnostics().ManagersDisposed == 2, "every manager disposed after close", 10_000);
        await Task.Delay(300, ct);
        d = manager.GetHostedDocumentDiagnostics();
        Console.WriteLine($"MANAGERS after close: {d}");
        await Assert.That(d.ManagersCreated).IsEqualTo(2);
        await Assert.That(d.ManagersRetired).IsEqualTo(2);
        await Assert.That(d.ManagersDisposed).IsEqualTo(2);
        await Assert.That(d.LiveManagers).IsEqualTo(0);
        await Assert.That(d.DisposeFailures).IsEqualTo(0);
        await Assert.That(d.DisposeTimeouts).IsEqualTo(0);
        await Assert.That(d.ShutdownTimeouts).IsEqualTo(0);
        await Assert.That(d.UnboundOutbound).IsEqualTo(0);
    }

    [Test]
    [Timeout(90_000)]
    public async Task RapidReloads_KeepOneLiveManager_AndDisposeEveryRetiredOneExactlyOnce(CancellationToken ct) {
        const ulong last = HostedDocumentIsolationTests.RapidDocuments;
        BlazorDocumentScenario s = await HostedDocumentIsolationTests.RunRapidReloadsAsync(waitForEachPage: true, ct);
        InfiniFrameWebViewManager manager = ManagerOf(s);
        await Waits.UntilAsync(() => manager.GetHostedDocumentDiagnostics().ManagersDisposed == (long)last - 1, "every retired manager disposed", 15_000);
        HostedDocumentDiagnostics settled = manager.GetHostedDocumentDiagnostics();
        Console.WriteLine($"MANAGERS after {last} documents whose pages all attached: {settled}");
        await Assert.That(settled.ManagersCreated).IsEqualTo((long)last);// one manager per document, none extra
        await Assert.That(settled.ManagersRetired).IsEqualTo((long)last - 1);
        await Assert.That(settled.ManagersDisposed).IsEqualTo((long)last - 1);
        await Assert.That(settled.LiveManagers).IsEqualTo(1);
        await Assert.That(settled.LatestLiveDocument).IsEqualTo(last);
        await Assert.That(settled.DisposeFailures).IsEqualTo(0);

        await s.CloseAsync(ct);
        await Waits.UntilAsync(() => manager.GetHostedDocumentDiagnostics() is { LiveManagers: 0 } d && d.ManagersDisposed == d.ManagersCreated,
            "every manager disposed after close", 10_000);
        HostedDocumentDiagnostics closed = manager.GetHostedDocumentDiagnostics();
        Console.WriteLine($"MANAGERS after close: {closed}");
        await Assert.That(closed.ManagersRetired).IsEqualTo((long)last);
        await Assert.That(closed.ManagersDisposed).IsEqualTo((long)last);
        await Assert.That(closed.DisposeTimeouts).IsEqualTo(0);
        await Assert.That(closed.ShutdownTimeouts).IsEqualTo(0);
    }

    [Test]
    [Timeout(90_000)]
    public async Task DocumentsReplacedBeforeTheirPageAttaches_KeepTheLiveSetBounded(CancellationToken ct) {
        const ulong last = HostedDocumentIsolationTests.RapidDocuments;
        BlazorDocumentScenario s = await HostedDocumentIsolationTests.RunRapidReloadsAsync(waitForEachPage: false, ct);
        InfiniFrameWebViewManager manager = ManagerOf(s);
        await Waits.UntilAsync(() => manager.GetHostedDocumentDiagnostics() is { LiveManagers: 1 } d && d.ManagersDisposed == d.ManagersRetired,
            "every retired manager disposed", 15_000);
        HostedDocumentDiagnostics settled = manager.GetHostedDocumentDiagnostics();
        Console.WriteLine($"MANAGERS after {last} documents replaced without waiting: {settled}");
        await Assert.That(settled.ManagersCreated <= (long)last).IsTrue();// a document replaced before its page attaches gets none
        await Assert.That(settled.ManagersRetired).IsEqualTo(settled.ManagersCreated - 1);
        await Assert.That(settled.LatestLiveDocument).IsEqualTo(last);
        await s.CloseAsync(ct);
    }

    [Test]
    [Timeout(60_000)]
    public async Task RetiredPagesRender_BetweenRetirementAndDisposal_IsRefusedAndCounted(CancellationToken ct) {
        HostedDocumentIsolationTests.RetiredRender run = await HostedDocumentIsolationTests.RunRetiredPagesRenderAsync(ct);
        HostedDocumentDiagnostics d = ManagerOf(run.Scenario).GetHostedDocumentDiagnostics();
        Console.WriteLine($"MANAGERS after page 1 rendered between retirement and disposal: {d}");
        await Assert.That(d.RefusedRenderBatches >= 1).IsTrue();
        await run.Scenario.CloseAsync(ct);
    }

    [Test]
    [Timeout(60_000)]
    [Arguments(true)]
    [Arguments(false)]
    public async Task DisposalJs_IsRefused_AndAFailedPageDisposalStillReleasesItsScope(bool catchesDisconnect, CancellationToken ct) {
        HostedDocumentIsolationTests.DisposalJs run = await HostedDocumentIsolationTests.RunDisposalJsAsync(catchesDisconnect, ct);
        HostedDocumentDiagnostics d = ManagerOf(run.Scenario).GetHostedDocumentDiagnostics();
        Console.WriteLine($"MANAGERS after the reload (component catches: {catchesDisconnect}): {d}");
        await Assert.That(d.RefusedJsCalls >= 1).IsTrue();
        if (catchesDisconnect) {
            // The component handles the refusal, as on Blazor Server: the framework's disposal completes normally.
            await Assert.That(run.Scenario.Pages.DisposeJsCaught).IsEqualTo(1);
            await Assert.That(d.DisposeFailures).IsEqualTo(0);
            await Assert.That(d.PageScopeFallbacks).IsEqualTo(0);
        }
        else {
            // The renderer rethrows the component's failure, so the framework's page disposal stops before the scope;
            // the coordinator then releases the scope itself.
            await Assert.That(d.DisposeFailures).IsEqualTo(1);
            await Assert.That(d.PageScopeFallbacks).IsEqualTo(1);
        }
        await Assert.That(run.Page1.Scope.DisposeCalls).IsEqualTo(1);
        await run.Scenario.CloseAsync(ct);
    }

    [Test]
    [Timeout(30_000)]
    public async Task GetDocumentLifetime_IsLiveOnlyForTheCurrentDocument(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAsync();
        await window.WaitForReadyAsync(ct);
        await Assert.That(window.GetDocumentLifetime(0).IsCancellationRequested).IsTrue();// no document id
        await Assert.That(window.GetDocumentLifetime(1).IsCancellationRequested).IsTrue();// nothing attached yet

        await host.AttachAsync(1);
        CancellationToken first = window.GetDocumentLifetime(1);
        await Assert.That(first.IsCancellationRequested).IsFalse();
        await Assert.That(first.CanBeCanceled).IsTrue();
        await Assert.That(window.GetDocumentLifetime(2).IsCancellationRequested).IsTrue();// not the current document

        await host.AttachAsync(2);// replaces document 1
        await Assert.That(window.GetDocumentLifetime(1).IsCancellationRequested).IsTrue();// immediately
        await Waits.UntilAsync(() => first.IsCancellationRequested, "document 1's lifetime token cancelled");
        CancellationToken second = window.GetDocumentLifetime(2);
        await Assert.That(second.IsCancellationRequested).IsFalse();

        await host.RetireAsync(2);// retired without a replacement
        await Assert.That(window.GetDocumentLifetime(2).IsCancellationRequested).IsTrue();
        await Waits.UntilAsync(() => second.IsCancellationRequested, "document 2's lifetime token cancelled");

        await host.AttachAsync(3);
        CancellationToken third = window.GetDocumentLifetime(3);
        await Assert.That(third.IsCancellationRequested).IsFalse();
        await window.DisposeAsync();// the window ends
        await Assert.That(window.GetDocumentLifetime(3).IsCancellationRequested).IsTrue();
        await Waits.UntilAsync(() => third.IsCancellationRequested, "the window's end cancelled document 3's lifetime");
    }

    [Test]
    [Timeout(60_000)]
    public async Task RetiredPagesOutput_IsRefusedAndCounted(CancellationToken ct) {
        HostedDocumentIsolationTests.LateWork run = await HostedDocumentIsolationTests.RunRetiredPagesLateWorkAsync(ct);
        HostedDocumentDiagnostics d = ManagerOf(run.Scenario).GetHostedDocumentDiagnostics();
        Console.WriteLine($"MANAGERS after page 1's late work: {d}");
        await Assert.That(d.RefusedJsCalls >= 1).IsTrue();
        await Assert.That(d.RefusedNavigations >= 1).IsTrue();
        await Assert.That(d.RefusedOutbound >= d.RefusedJsCalls + d.RefusedNavigations).IsTrue();
        await Assert.That(d.UnboundOutbound).IsEqualTo(0);
        await run.Scenario.CloseAsync(ct);
    }

    [Test]
    [Timeout(60_000)]
    public async Task QueuedOutboundForARetiredDocument_IsCancelledAndCounted(CancellationToken ct) {
        HostedDocumentIsolationTests.QueuedWork run = await HostedDocumentIsolationTests.RunQueuedOutboundAsync(ct);
        HostedDocumentDiagnostics d = ManagerOf(run.Scenario).GetHostedDocumentDiagnostics();
        Console.WriteLine($"MANAGERS after the queued output: {d}");
        await Assert.That(d.SendsCancelledForRetiredDocument >= 1).IsTrue();
        await run.Scenario.CloseAsync(ct);
    }

    [Test]
    [Timeout(60_000)]
    public async Task InboundQueuedPastItsDocumentsRetirement_IsRefusedAndCounted(CancellationToken ct) {
        HostedDocumentIsolationTests.DelayedReply run = await HostedDocumentIsolationTests.RunDelayedInboundAsync(ct);
        HostedDocumentDiagnostics d = ManagerOf(run.Scenario).GetHostedDocumentDiagnostics();
        Console.WriteLine($"MANAGERS after the delayed answer: {d}; page 1's call: {BlazorDocumentScenario.Outcome(run.Page1Call)}");
        await Assert.That(d.RefusedInbound >= 1).IsTrue();
        await Assert.That(run.Page1Call.IsCompleted).IsFalse();// refused, so nothing completed it
        await run.Scenario.CloseAsync(ct);
    }

    private static InfiniFrameWebViewManager ManagerOf(BlazorDocumentScenario s) =>
        (InfiniFrameWebViewManager)s.Window.ServiceProvider!.GetRequiredService<IInfiniFrameWebViewManager>();
}
