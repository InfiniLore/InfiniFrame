// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using InfiniFrame.Hosting;
using InfiniFrame.Window.Hosting;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace InfiniTests.InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// Document isolation of hosted Blazor pages: each browser document's page stays inside that document. The tests use the
// real Blazor WebView manager and play the page side of Blazor's IPC through the scripted provider (contract tests, no
// real browser). They use only behaviour that hosted windows had before per-document managers, so the same file runs
// before and after that change; HostedDocumentManagerTests covers the managers' own counters.
// Without per-document managers these fail: RetiredPagesLateJsNavigationAndRender_*, CrossTalk_CollidingTaskIds_*,
// CrossTalk_DotNetObjectIds_*, PendingCalls_* and both DisposalJs_* cases (document isolation), and Shutdown_* and
// CloseWhileThePageAttaches_* (the last hosted page was never released at close). The other tests guard behaviour that
// both must have.
[NotInParallel]
public sealed class HostedDocumentIsolationTests {
    // ---- the defect, its control, and both forms of cross-talk ------------------------------------------------------
    [Test]
    [Timeout(60_000)]
    public async Task RetiredPagesLateJsNavigationAndRender_NeverReachTheNextDocument(CancellationToken ct) {
        LateWork run = await RunRetiredPagesLateWorkAsync(ct);
        BlazorDocumentScenario s = run.Scenario;
        Console.WriteLine($"Sent after page 2 attached: {BlazorDocumentScenario.Describe(run.Late)}");
        Console.WriteLine($"Page 1 late JS call: {BlazorDocumentScenario.Outcome(run.Js)}; NavigateTo: {run.Navigation}; StateHasChanged: {run.Render}");
        await Assert.That(run.Late.Any(static m => m.Message.Contains("page-1-late-js", StringComparison.Ordinal))).IsFalse();
        await Assert.That(run.Late.Any(static m => m.Message.Contains("page-1-late-navigation", StringComparison.Ordinal))).IsFalse();
        await Assert.That(run.Late.Any(static m => BlazorDocumentScenario.IsType(m.Message, "RenderBatch"))).IsFalse();
        await Assert.That(run.Js.Exception?.GetBaseException()).IsTypeOf<JSDisconnectedException>();
        await Assert.That(run.Navigation).IsEqualTo("returned");

        // Page 2 is unaffected and still live.
        int batches = s.RenderBatches(2);
        await run.Page2.OnDispatcher(() => run.Page2.Rerender("page-2-still-live"));
        await Waits.UntilAsync(() => s.RenderBatches(2) > batches, "page 2 rendered into document 2");
        await s.CloseAsync(ct);
    }

    internal readonly record struct LateWork(BlazorDocumentScenario Scenario, IsolationProbe Page2, SentMessage[] Late, Task Js, string Navigation, string Render);

    /// <summary>The reproduction: page 1's captured services act after document 2's page attached.</summary>
    internal static async Task<LateWork> RunRetiredPagesLateWorkAsync(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        s.Answer("probe.stamp", static _ => "null");// a real page runs probe.stamp in whichever document it reaches
        IsolationProbe page1 = await s.LoadPageAsync(1);
        await s.Host.RetireAsync(1);
        IsolationProbe page2 = await s.LoadPageAsync(2);
        await s.SettleAsync();
        int mark = s.Mark;

        Task? js = null;
        string navigation = "", render = "";
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        await page1.OnDispatcher(() => {
            js = Call(() => page1.JS.InvokeVoidAsync("probe.stamp", timeout.Token, "page-1-late-js"));
            navigation = Capture(() => page1.Navigation.NavigateTo("/page-1-late-navigation"));
            render = Capture(() => page1.Rerender("page-1-late-render"));
        });
        await Waits.UntilAsync(() => js!.IsCompleted, "page 1's late JS call ended", 5_000);
        await s.SettleAsync();
        return new LateWork(s, page2, s.SentSince(mark), js!, navigation, render);
    }

    [Test]
    [Timeout(60_000)]
    public async Task Control_CurrentPagesJsNavigationAndRender_ReachTheCurrentDocument(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        s.Answer("probe.stamp", static document => $"\"run-by-document-{document}\"");
        await s.LoadPageAsync(1);
        await s.Host.RetireAsync(1);
        IsolationProbe page2 = await s.LoadPageAsync(2);
        await s.SettleAsync();
        int mark = s.Mark;

        Task<string>? js = null;
        await page2.OnDispatcher(() => {
            js = Call(() => page2.JS.InvokeAsync<string>("probe.stamp", "page-2-control-js"));
            page2.Navigation.NavigateTo("/page-2-control-navigation");
            page2.Rerender("page-2-control-render");
        });
        await Waits.UntilAsync(() => js!.IsCompleted, "page 2's JS call answered");
        await Waits.UntilAsync(() => s.SentSince(mark).Any(static m => BlazorDocumentScenario.IsType(m.Message, "Navigate")), "page 2's navigation sent");
        await s.SettleAsync();

        SentMessage[] sent = s.SentSince(mark);
        Console.WriteLine($"Control sent after page 2 attached: {BlazorDocumentScenario.Describe(sent)}");
        await Assert.That(sent.Any(static m => m.Document == 2 && BlazorDocumentScenario.IsType(m.Message, "BeginInvokeJS")
            && m.Message.Contains("page-2-control-js", StringComparison.Ordinal))).IsTrue();
        await Assert.That(sent.Any(static m => m.Document == 2 && BlazorDocumentScenario.IsType(m.Message, "Navigate")
            && m.Message.Contains("page-2-control-navigation", StringComparison.Ordinal))).IsTrue();
        await Assert.That(sent.Any(static m => m.Document == 2 && BlazorDocumentScenario.IsType(m.Message, "RenderBatch"))).IsTrue();
        await Assert.That(sent.All(static m => m.Document == 2)).IsTrue();
        await Assert.That(js!.Result).IsEqualTo("run-by-document-2");
        await s.CloseAsync(ct);
    }

    [Test]
    [Timeout(60_000)]
    public async Task CrossTalk_CollidingTaskIds_CannotCompleteAnotherPagesCall(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        s.Answer("probe.stamp", static _ => "\"stamp-answer\"");// a real page answers probe.stamp wherever it runs
        IsolationProbe page1 = await s.LoadPageAsync(1);
        await s.Host.RetireAsync(1);
        IsolationProbe page2 = await s.LoadPageAsync(2);
        await s.SettleAsync();

        Task<string>? own = null, stamp = null;
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        await page2.OnDispatcher(() => own = Call(() => page2.JS.InvokeAsync<string>("probe.page2own")));// unanswered for now
        await page1.OnDispatcher(() => stamp = Call(() => page1.JS.InvokeAsync<string>("probe.stamp", timeout.Token, "page-1-late")));
        await Waits.UntilAsync(() => stamp!.IsCompleted, "page 1's late call ended", 5_000);
        await s.SettleAsync();

        long ownTask = BlazorDocumentScenario.ParseJsCall(s.JsCalls("probe.page2own").Single().Message).TaskId;
        SentMessage[] stamps = s.JsCalls("probe.stamp");
        string stampSent = stamps.Length == 0
            ? "never sent"
            : $"sent as task {BlazorDocumentScenario.ParseJsCall(stamps[0].Message).TaskId} to document {stamps[0].Document}";
        Console.WriteLine($"CrossTalk page 2's own call is task {ownTask}; page 1's late call was {stampSent} and {BlazorDocumentScenario.Outcome(stamp)}; " +
            $"page 2's own call before its answer: {BlazorDocumentScenario.Outcome(own)}");
        await Assert.That(own!.IsCompleted).IsFalse();// only its own document's answer may complete it

        await s.ReplyAsync(2, ownTask, "\"page-2-own-answer\"");
        await Waits.UntilAsync(() => own.IsCompleted, "page 2's own call answered by document 2");
        await Assert.That(own.Result).IsEqualTo("page-2-own-answer");
        await s.CloseAsync(ct);
    }

    [Test]
    [Timeout(60_000)]
    public async Task CrossTalk_DotNetObjectIds_RetiredPagesCallbackCannotReachTheNewPagesObject(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        s.Answer("probe.register", static _ => "null");
        // For probe.callback the page calls the passed .NET object back, in the document the call reached.
        s.Handle("probe.callback", (document, taskId, argsJson) => {
            Task callback = s.InvokeDotNetAsync(document, $"callback-{taskId}", DotNetObjectId(argsJson), nameof(IsolationProbe.Clicked),
                $"[\"called-back-in-document-{document}\"]");
            GC.KeepAlive(callback);// queued on the provider loop after this call returns
            return "null";
        });
        IsolationProbe page1 = await s.LoadPageAsync(1);
        await s.Host.RetireAsync(1);
        IsolationProbe page2 = await s.LoadPageAsync(2);
        await s.SettleAsync();

        DotNetObjectReference<IsolationProbe>? reference1 = null, reference2 = null;
        Task? register = null, late = null;
        await page2.OnDispatcher(() => {
            reference2 = DotNetObjectReference.Create(page2);
            register = Call(() => page2.JS.InvokeVoidAsync("probe.register", reference2));
        });
        await Waits.UntilAsync(() => register!.IsCompleted, "page 2 registered its .NET object");
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        await page1.OnDispatcher(() => {
            reference1 = DotNetObjectReference.Create(page1);
            late = Call(() => page1.JS.InvokeVoidAsync("probe.callback", timeout.Token, reference1));
        });
        await Waits.UntilAsync(() => late!.IsCompleted, "page 1's late call ended", 5_000);
        await s.SettleAsync();

        long page2Object = DotNetObjectId(BlazorDocumentScenario.ParseJsCall(s.JsCalls("probe.register").Single().Message).ArgsJson);
        SentMessage[] callbacks = s.JsCalls("probe.callback");
        string callbackSent = callbacks.Length == 0
            ? "never sent"
            : $"sent to document {callbacks[0].Document} with .NET object {DotNetObjectId(BlazorDocumentScenario.ParseJsCall(callbacks[0].Message).ArgsJson)}";
        Console.WriteLine($"CrossTalk page 2's .NET object is {page2Object}; page 1's late call with its own object was {callbackSent}; " +
            $"page 2's component text is \"{page2.Text}\"");
        await Assert.That(callbacks.Length).IsEqualTo(0);
        await Assert.That(page2.Text).IsNotEqualTo("called-back-in-document-2");
        reference1?.Dispose();
        reference2?.Dispose();
        await s.CloseAsync(ct);
    }

    [Test]
    [Timeout(60_000)]
    public async Task CrossTalk_InboundDotNetObjectIdOwnedOnlyByTheRetiredPage_ReachesNoObject(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        s.Answer("probe.register", static _ => "null");
        IsolationProbe page1 = await s.LoadPageAsync(1);
        // Page 1 registers two .NET objects, so its runtime owns an id that page 2's runtime never issues.
        DotNetObjectReference<IsolationProbe>? first = null, second = null, own = null;
        Task? registerFirst = null, registerSecond = null, registerOwn = null;
        await page1.OnDispatcher(() => {
            first = DotNetObjectReference.Create(page1);
            second = DotNetObjectReference.Create(page1);
            registerFirst = Call(() => page1.JS.InvokeVoidAsync("probe.register", first));
            registerSecond = Call(() => page1.JS.InvokeVoidAsync("probe.register", second));
        });
        await Waits.UntilAsync(() => registerFirst!.IsCompleted && registerSecond!.IsCompleted, "page 1 registered its objects");
        long page1Only = DotNetObjectId(BlazorDocumentScenario.ParseJsCall(s.JsCalls("probe.register")[1].Message).ArgsJson);

        await s.Host.RetireAsync(1);
        IsolationProbe page2 = await s.LoadPageAsync(2);
        await page2.OnDispatcher(() => {
            own = DotNetObjectReference.Create(page2);
            registerOwn = Call(() => page2.JS.InvokeVoidAsync("probe.register", own));
        });
        await Waits.UntilAsync(() => registerOwn!.IsCompleted, "page 2 registered its object");
        long page2Object = DotNetObjectId(BlazorDocumentScenario.ParseJsCall(s.JsCalls("probe.register")[2].Message).ArgsJson);

        // Document 2 calls the id that only page 1's runtime owns, then its own object.
        await s.InvokeDotNetAsync(2, "foreign-call", page1Only, nameof(IsolationProbe.Clicked), "[\"called-from-document-2\"]");
        await Waits.UntilAsync(() => s.Host.Sent.Any(static m => m.Document == 2 && BlazorDocumentScenario.IsType(m.Message, "EndInvokeDotNet")
            && m.Message.Contains("\"foreign-call\"", StringComparison.Ordinal)), "document 2 received an answer for the foreign id");
        SentMessage foreign = s.Host.Sent.First(static m => BlazorDocumentScenario.IsType(m.Message, "EndInvokeDotNet")
            && m.Message.Contains("\"foreign-call\"", StringComparison.Ordinal));
        await s.InvokeDotNetAsync(2, "own-call", page2Object, nameof(IsolationProbe.Clicked), "[\"own-call-from-document-2\"]");
        await Waits.UntilAsync(() => page2.Text == "own-call-from-document-2", "page 2's own object handled document 2's call");
        Console.WriteLine($"CrossTalk inbound: page 1 owns object {page1Only}, page 2 owns {page2Object}; answer to the foreign call: {foreign.Message[..Math.Min(120, foreign.Message.Length)]}");
        await Assert.That(page1.Text).IsNotEqualTo("called-from-document-2");
        await Assert.That(EndInvokeDotNetSucceeded(foreign.Message)).IsFalse();
        await Assert.That(foreign.Document).IsEqualTo(2UL);
        first?.Dispose();
        second?.Dispose();
        own?.Dispose();
        await s.CloseAsync(ct);
    }

    // ---- the new page works fully, and application-lifetime producers drive it ---------------------------------------
    [Test]
    [Timeout(60_000)]
    public async Task NewPage_UiEventRenderJsReplyAndNavigation_Succeed(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        s.Answer("probe.register", static _ => "null");
        s.Answer("probe.value", static document => $"\"value-from-document-{document}\"");
        await s.LoadPageAsync(1);
        await s.Host.RetireAsync(1);
        IsolationProbe page2 = await s.LoadPageAsync(2);
        await s.SettleAsync();
        int mark = s.Mark;

        DotNetObjectReference<IsolationProbe>? reference = null;
        Task? register = null;
        Task<string>? value = null;
        await page2.OnDispatcher(() => {
            reference = DotNetObjectReference.Create(page2);
            register = Call(() => page2.JS.InvokeVoidAsync("probe.register", reference));
            value = Call(() => page2.JS.InvokeAsync<string>("probe.value"));
        });
        await Waits.UntilAsync(() => register!.IsCompleted && value!.IsCompleted, "page 2's JS calls answered");
        await Assert.That(value!.Result).IsEqualTo("value-from-document-2");

        // A UI event from the page into page 2's component.
        long objectId = DotNetObjectId(BlazorDocumentScenario.ParseJsCall(s.JsCalls("probe.register").Single().Message).ArgsJson);
        int batches = s.RenderBatches(2);
        await s.InvokeDotNetAsync(2, "event-1", objectId, nameof(IsolationProbe.Clicked), "[\"clicked-in-document-2\"]");
        await Waits.UntilAsync(() => s.Host.Sent.Any(static m => m.Document == 2 && BlazorDocumentScenario.IsType(m.Message, "EndInvokeDotNet")
            && m.Message.Contains("page-2:clicked-in-document-2", StringComparison.Ordinal)), "the handler's result sent to document 2");
        await Waits.UntilAsync(() => s.RenderBatches(2) > batches, "the event re-rendered page 2 in document 2");
        await Assert.That(page2.Text).IsEqualTo("clicked-in-document-2");

        await page2.OnDispatcher(() => page2.Navigation.NavigateTo("/next-route"));
        await Waits.UntilAsync(() => s.Host.Sent.Any(static m => m.Document == 2 && BlazorDocumentScenario.IsType(m.Message, "Navigate")
            && m.Message.Contains("next-route", StringComparison.Ordinal)), "page 2's navigation sent to document 2");
        await Assert.That(s.SentSince(mark).All(static m => m.Document == 2)).IsTrue();
        reference?.Dispose();
        await s.CloseAsync(ct);
    }

    [Test]
    [Timeout(60_000)]
    public async Task ApplicationProducer_DrivesTheCurrentPage_AfterFullReload(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        IsolationProbe page1 = await s.LoadPageAsync(1);
        int batches1 = s.RenderBatches(1);
        s.Pages.Tick();
        await Waits.UntilAsync(() => s.RenderBatches(1) > batches1, "the producer re-rendered page 1 in document 1");

        await s.Host.RetireAsync(1);
        IsolationProbe page2 = await s.LoadPageAsync(2);
        await Waits.UntilAsync(() => page1.DisposeCalls > 0 && page1.Scope.DisposeCalls > 0, "page 1's component and scope released", 10_000);
        await s.SettleAsync();
        int document1 = s.Host.Sent.Count(static m => m.Document == 1);
        int batches2 = s.RenderBatches(2);
        s.Pages.Tick();
        await Waits.UntilAsync(() => s.RenderBatches(2) > batches2, "the producer re-rendered page 2 in document 2");
        await s.SettleAsync();

        await Assert.That(s.Host.Sent.Count(static m => m.Document == 1)).IsEqualTo(document1);
        await Assert.That(page2.Ticks).IsEqualTo(1);
        await Assert.That(s.Pages.Subscribers).IsEqualTo(1);// no duplicate component subscriptions
        await Assert.That(page1.DisposeCalls).IsEqualTo(1);
        await Assert.That(page1.Scope.DisposeCalls).IsEqualTo(1);
        await Assert.That(page2.Scope.DisposeCalls).IsEqualTo(0);
        await Assert.That(ReferenceEquals(page1.Registry, page2.Registry)).IsTrue();// application services are shared
        await Assert.That(ReferenceEquals(page1.Scope, page2.Scope)).IsFalse();// each page has its own scope
        await s.CloseAsync(ct);
    }

    // ---- pending calls across retirement (a separate acceptance question: outcomes are reported as they are) --------
    [Test]
    [Timeout(60_000)]
    public async Task PendingCalls_AcrossRetirement_EndOnlyByRefusalOrTheirCallersToken(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        // Only a page in document 2 answers these: an answer would mean page 1's work ran in the wrong document.
        s.Answer("probe.slow", static document => document == 2 ? "\"run-by-document-2\"" : null);
        s.Answer("probe.late", static document => document == 2 ? "\"run-by-document-2\"" : null);
        IsolationProbe page1 = await s.LoadPageAsync(1);

        using var beforeToken = new CancellationTokenSource();
        Task<string>? beforeNoToken = null, beforeWithToken = null;
        await page1.OnDispatcher(() => {
            beforeNoToken = Call(() => page1.JS.InvokeAsync<string>("probe.slow", ["pending-before-retirement"]));
            beforeWithToken = Call(() => page1.JS.InvokeAsync<string>("probe.slow", beforeToken.Token, ["pending-before-retirement-with-token"]));
        });
        await Waits.UntilAsync(() => s.JsCalls("probe.slow").Length == 2, "both calls sent to document 1");

        await s.Host.RetireAsync(1);
        await s.LoadPageAsync(2);
        await s.SettleAsync();

        using var afterToken = new CancellationTokenSource();
        Task<string>? onNoToken = null, onWithToken = null;
        await page1.OnDispatcher(() => {
            onNoToken = Call(() => page1.JS.InvokeAsync<string>("probe.late", ["after-retirement-on-dispatcher"]));
            onWithToken = Call(() => page1.JS.InvokeAsync<string>("probe.late", afterToken.Token, ["after-retirement-on-dispatcher-with-token"]));
        });
        Task<string> offNoToken = Task.Run(() => Call(() => page1.JS.InvokeAsync<string>("probe.late", ["after-retirement-off-dispatcher"])), ct);
        Task<string> offWithToken = Task.Run(() => Call(() => page1.JS.InvokeAsync<string>("probe.late", afterToken.Token,
            ["after-retirement-off-dispatcher-with-token"])), ct);
        await Task.Delay(1_000, ct);
        await s.SettleAsync();
        // Then the callers cancel.
        await beforeToken.CancelAsync();
        await afterToken.CancelAsync();
        await Task.Delay(300, ct);

        (string Row, Task<string>? Task)[] rows = [
            ("pending before retirement, no token", beforeNoToken),
            ("pending before retirement, caller cancels", beforeWithToken),
            ("after retirement on Blazor's dispatcher, no token", onNoToken),
            ("after retirement on Blazor's dispatcher, caller cancels", onWithToken),
            ("after retirement off the dispatcher, no token", offNoToken),
            ("after retirement off the dispatcher, caller cancels", offWithToken)
        ];
        foreach ((string row, Task<string>? task) in rows) Console.WriteLine($"PENDING-CALL {row}: {BlazorDocumentScenario.Outcome(task)}");

        await Assert.That(s.Host.Sent.Any(static m => m.Document == 2 && (m.Message.Contains("\"probe.slow\"", StringComparison.Ordinal)
            || m.Message.Contains("\"probe.late\"", StringComparison.Ordinal)))).IsFalse();
        await Assert.That(rows.Any(static r => r.Task!.IsCompletedSuccessfully)).IsFalse();// no result invented or produced elsewhere
        await Assert.That(beforeNoToken!.IsCompleted).IsFalse();// residual: nothing fails a call already sent
        await Assert.That(beforeWithToken!.IsCanceled).IsTrue();
        await Assert.That(onNoToken!.Exception?.GetBaseException()).IsTypeOf<JSDisconnectedException>();
        await Assert.That(onWithToken!.Exception?.GetBaseException()).IsTypeOf<JSDisconnectedException>();
        await Assert.That(offNoToken.IsCompleted).IsFalse();// residual: the framework keeps the refusal from this caller
        await Assert.That(offWithToken.IsCanceled).IsTrue();
        await s.CloseAsync(ct);
    }

    // ---- queued and delayed work keeps its original document ----------------------------------------------------------
    [Test]
    [Timeout(60_000)]
    public async Task QueuedOutbound_ForARetiredDocument_IsCancelledNeverRedirected(CancellationToken ct) {
        QueuedWork run = await RunQueuedOutboundAsync(ct);
        BlazorDocumentScenario s = run.Scenario;
        Console.WriteLine($"QueuedOutbound sent after the retirement: {BlazorDocumentScenario.Describe(run.After)}; queued call: {BlazorDocumentScenario.Outcome(run.QueuedJs)}");
        await Assert.That(run.After.Any(static m => m.Message.Contains("queued-for-document-1", StringComparison.Ordinal))).IsFalse();
        await Assert.That(run.After.All(static m => m.Document == 2)).IsTrue();
        await Assert.That(run.Page2.Text).IsEqualTo("initial");
        await Assert.That(run.QueuedJs.IsCompletedSuccessfully).IsFalse();
        await s.CloseAsync(ct);
    }

    internal readonly record struct QueuedWork(BlazorDocumentScenario Scenario, IsolationProbe Page2, SentMessage[] After, Task QueuedJs);

    /// <summary>Page 1's output waits in the core's outbound queue behind a slow provider send while document 1 retires.</summary>
    internal static async Task<QueuedWork> RunQueuedOutboundAsync(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        IsolationProbe page1 = await s.LoadPageAsync(1);
        await s.SettleAsync();
        ScriptedWindowHostCore host = s.Host;
        var inFlight = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var never = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        int holding = 1;
        host.SendOverride = async (document, message, token) => {
            if (document.Value == 1 && Volatile.Read(ref holding) == 1) {
                inFlight.TrySetResult();
                await never.Task.WaitAsync(token);// the provider is slow to accept this document-1 send
            }
            await host.Host.OnLoopAsync(() => {
                if (document.Value != host.ProviderDocument) throw new InfiniFrameHostDocumentRetiredException(document);
                var sent = new SentMessage(document.Value, message, Environment.CurrentManagedThreadId);
                host.Sent.Enqueue(sent);
                host.OnSent?.Invoke(sent);
            });
        };

        Task? queuedJs = null;
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        await page1.OnDispatcher(() => {
            page1.Rerender("queued-render-1");
            queuedJs = Call(() => page1.JS.InvokeVoidAsync("probe.queued", timeout.Token, "queued-for-document-1"));
            page1.Rerender("queued-render-2");
        });
        await inFlight.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);
        int mark = s.Mark;
        await host.RetireAsync(1);
        Volatile.Write(ref holding, 0);
        IsolationProbe page2 = await s.LoadPageAsync(2);
        await s.SettleAsync();
        await Waits.UntilAsync(() => queuedJs!.IsCompleted, "the queued call ended", 5_000);
        return new QueuedWork(s, page2, s.SentSince(mark), queuedJs!);
    }

    [Test]
    [Timeout(60_000)]
    public async Task DelayedInbound_QueuedBeforeRetirement_NeverReachesTheNextPage(CancellationToken ct) {
        DelayedReply run = await RunDelayedInboundAsync(ct);
        BlazorDocumentScenario s = run.Scenario;
        Task<string> page1Call = run.Page1Call;
        IsolationProbe page2 = run.Page2;
        Console.WriteLine($"DelayedInbound page 1's call after document 1's queued answer: {BlazorDocumentScenario.Outcome(page1Call)}");
        // Delivered only to its own document's page, or refused; never to page 2.
        await Assert.That(page1Call.IsCompletedSuccessfully && page1Call.Result != "answer-from-document-1").IsFalse();
        long page1Task = run.Page1Task;

        Task<string>? page2Call = null;
        await page2.OnDispatcher(() => page2Call = Call(() => page2.JS.InvokeAsync<string>("probe.answer-later")));
        await Waits.UntilAsync(() => s.JsCalls("probe.answer-later").Length == 2, "page 2's call sent");
        SentMessage page2Sent = s.JsCalls("probe.answer-later")[1];
        long page2Task = BlazorDocumentScenario.ParseJsCall(page2Sent.Message).TaskId;
        Console.WriteLine($"DelayedInbound page 1's call was task {page1Task}; page 2's call is task {page2Task} in document {page2Sent.Document}");
        await s.SettleAsync();
        await Assert.That(page2Call!.IsCompleted).IsFalse();
        await s.ReplyAsync(2, page2Task, "\"answer-from-document-2\"");
        await Waits.UntilAsync(() => page2Call.IsCompleted, "page 2's call answered by document 2");
        await Assert.That(page2Call.Result).IsEqualTo("answer-from-document-2");
        await s.CloseAsync(ct);
    }

    internal readonly record struct DelayedReply(BlazorDocumentScenario Scenario, IsolationProbe Page2, Task<string> Page1Call, long Page1Task);

    /// <summary>Document 1's answer to page 1's call waits in Blazor's own queue while document 1 retires and page 2 attaches.</summary>
    internal static async Task<DelayedReply> RunDelayedInboundAsync(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        IsolationProbe page1 = await s.LoadPageAsync(1);
        await s.SettleAsync();
        Task<string>? page1Call = null;
        await page1.OnDispatcher(() => page1Call = Call(() => page1.JS.InvokeAsync<string>("probe.answer-later")));
        await Waits.UntilAsync(() => s.JsCalls("probe.answer-later").Length == 1, "page 1's call sent to document 1");
        long page1Task = BlazorDocumentScenario.ParseJsCall(s.JsCalls("probe.answer-later")[0].Message).TaskId;

        // Hold Blazor's own work queue (an item posted from a held task scheduler); the hosted dispatcher stays free.
        var context = s.Window.ServiceProvider!.GetRequiredService<InfiniFrameSynchronizationContext>();
        var held = new HeldTaskScheduler();
        await Task.Factory.StartNew(() => {
            held.Hold();
            context.Post(static _ => {}, null);
        }, ct, TaskCreationOptions.DenyChildAttach, held);
        var hosted = (InfiniFrameHostedWindow)s.Window;

        // Document 1's answer is accepted (document 1 is current) and waits in Blazor's queue; document 1 then retires
        // and document 2's page attaches behind it.
        await s.ReplyAsync(1, page1Task, "\"answer-from-document-1\"");
        await hosted.DispatchAsync(static () => {}, cancellationToken: ct);// the hosted dispatcher has handed it to Blazor
        await s.Host.RetireAsync(1);
        await s.Host.AttachAsync(2);
        await s.Host.PostAsync(2, BlazorDocumentScenario.AttachPage);
        await hosted.DispatchAsync(static () => {}, cancellationToken: ct);
        held.Release();
        await Waits.UntilAsync(() => s.Pages.Count == 2 && s.RenderBatches(2) > 0, "page 2 rendered into document 2", 10_000);
        await s.SettleAsync();
        return new DelayedReply(s, s.Pages[2], page1Call!, page1Task);
    }

    internal const string RetiredRenderMarker = "page-1-render-after-retirement";

    [Test]
    [Timeout(60_000)]
    public async Task RetiredPagesRender_BetweenRetirementAndDisposal_NeverReachesAnyDocument(CancellationToken ct) {
        RetiredRender run = await RunRetiredPagesRenderAsync(ct);
        SentMessage[] carrying = [.. run.After.Where(static m => BlazorDocumentScenario.RenderBatchContains(m, RetiredRenderMarker))];
        Console.WriteLine($"RetiredRender page 1's render ran while its renderer was live: {run.Render}; render batches carrying it: {carrying.Length}; " +
            $"sent after the retirement: {BlazorDocumentScenario.Describe(run.After)}");
        await Assert.That(run.Render).IsEqualTo("returned");// page 1's renderer was still live, so it did render
        await Assert.That(carrying.Length).IsEqualTo(0);
        await Assert.That(run.After.Where(static m => BlazorDocumentScenario.IsType(m.Message, "RenderBatch")).All(static m => m.Document == 2)).IsTrue();
        await run.Scenario.CloseAsync(ct);
    }

    internal readonly record struct RetiredRender(BlazorDocumentScenario Scenario, SentMessage[] After, string Render);

    /// <summary>
    ///     Page 1 renders after its document retired but before its manager is disposed: Blazor's queue is held while the
    ///     render is queued, document 1 retires and document 2's page attaches behind it.
    /// </summary>
    internal static async Task<RetiredRender> RunRetiredPagesRenderAsync(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        IsolationProbe page1 = await s.LoadPageAsync(1);
        await s.SettleAsync();
        int mark = s.Mark;
        var context = s.Window.ServiceProvider!.GetRequiredService<InfiniFrameSynchronizationContext>();
        var held = new HeldTaskScheduler();
        await Task.Factory.StartNew(() => {
            held.Hold();
            context.Post(static _ => {}, null);
        }, ct, TaskCreationOptions.DenyChildAttach, held);
        var hosted = (InfiniFrameHostedWindow)s.Window;

        string render = "not run";
        Task rendered = page1.OnDispatcher(() => render = Capture(() => page1.Rerender(RetiredRenderMarker)));// queued, page 1 still live
        await s.Host.RetireAsync(1);
        await s.Host.AttachAsync(2);
        await s.Host.PostAsync(2, BlazorDocumentScenario.AttachPage);
        await hosted.DispatchAsync(static () => {}, cancellationToken: ct);
        held.Release();
        await Waits.WithinAsync(rendered, "page 1's queued render ran", 10_000);
        await Waits.UntilAsync(() => s.Pages.Count == 2 && s.RenderBatches(2) > 0, "page 2 rendered into document 2", 10_000);
        await s.SettleAsync();
        return new RetiredRender(s, s.SentSince(mark), render);
    }

    // ---- lifecycle: reloads, duplicate and failed attach, close during attach, shutdown and fresh start --------------
    internal const ulong RapidDocuments = 12;

    [Test]
    [Timeout(90_000)]
    public async Task RapidReloads_ReleaseEveryRetiredPage_AndAddRootsOncePerPage(CancellationToken ct) {
        BlazorDocumentScenario s = await RunRapidReloadsAsync(waitForEachPage: true, ct);
        IsolationProbe current = s.Pages.Latest;
        IsolationProbe[] pages = s.Pages.All;
        await Waits.UntilAsync(() => pages.Where(p => !ReferenceEquals(p, current)).All(static p => p.DisposeCalls > 0 && p.Scope.DisposeCalls > 0),
            "every retired page released", 15_000);

        Console.WriteLine($"RapidReloads {pages.Length} pages were created for {RapidDocuments} documents; subscribers {s.Pages.Subscribers}");
        await Assert.That(pages.Length).IsEqualTo((int)RapidDocuments);// every document's page attached
        await Assert.That(pages.Select(static p => p.Scope).Distinct().Count()).IsEqualTo(pages.Length);// one root per page scope
        await Assert.That(pages.All(static p => p.DisposeCalls <= 1 && p.Scope.DisposeCalls <= 1)).IsTrue();
        await Assert.That(current.DisposeCalls).IsEqualTo(0);
        await Assert.That(s.Pages.Subscribers).IsEqualTo(1);
        await Assert.That(s.Window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Ready);
        await s.CloseAsync(ct);
    }

    [Test]
    [Timeout(90_000)]
    public async Task DocumentsReplacedBeforeTheirPageAttaches_LeaveOnlyTheLastPageLive(CancellationToken ct) {
        BlazorDocumentScenario s = await RunRapidReloadsAsync(waitForEachPage: false, ct);
        IsolationProbe current = s.Pages.Latest;
        IsolationProbe[] pages = s.Pages.All;
        await Waits.UntilAsync(() => pages.Where(p => !ReferenceEquals(p, current)).All(static p => p.Scope.DisposeCalls > 0),
            "every replaced page released", 15_000);
        Console.WriteLine($"DocumentsReplacedBeforeTheirPage {pages.Length} pages were created for {RapidDocuments} documents");
        await Assert.That(current.Scope.DisposeCalls).IsEqualTo(0);
        await Assert.That(pages.All(static p => p.Scope.DisposeCalls <= 1)).IsTrue();
        await Assert.That(s.Pages.Subscribers).IsEqualTo(1);
        await s.CloseAsync(ct);
    }

    /// <summary>
    ///     Twelve full reloads. With <paramref name="waitForEachPage" /> every document's page attaches (its root component
    ///     exists) before the next document replaces it; without, documents replace each other without waiting at all.
    /// </summary>
    internal static async Task<BlazorDocumentScenario> RunRapidReloadsAsync(bool waitForEachPage, CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        await s.LoadPageAsync(1);
        for (ulong document = 2; document <= RapidDocuments; document++) {
            int pages = s.Pages.Count;
            await s.Host.AttachAsync(document);
            await s.Host.PostAsync(document, BlazorDocumentScenario.AttachPage);
            if (waitForEachPage) await Waits.UntilAsync(() => s.Pages.Count > pages, $"document {document}'s page attached", 10_000);
        }
        await Waits.UntilAsync(() => s.RenderBatches(RapidDocuments) > 0, "the last document's page rendered", 15_000);
        await s.SettleAsync();
        return s;
    }

    [Test]
    [Timeout(60_000)]
    public async Task Shutdown_ReleasesEveryPageExactlyOnce_IncludingTheLast(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        IsolationProbe page1 = await s.LoadPageAsync(1);
        await s.Host.RetireAsync(1);
        IsolationProbe page2 = await s.LoadPageAsync(2);
        await Waits.UntilAsync(() => page1.Scope.DisposeCalls > 0, "page 1 released by the reload", 10_000);
        await s.CloseAsync(ct);
        await Waits.UntilAsync(() => page2.Scope.DisposeCalls > 0, "shutdown released the last page", 10_000);
        await Task.Delay(300, ct);
        await Assert.That(s.Pages.All.All(static p => p.DisposeCalls == 1 && p.Scope.DisposeCalls == 1)).IsTrue();
        await Assert.That(s.Pages.Subscribers).IsEqualTo(0);
    }

    [Test]
    [Timeout(60_000)]
    public async Task DuplicateAttach_StaysWithinItsDocument(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        IsolationProbe first = await s.LoadPageAsync(1);
        int batches = s.RenderBatches(1);
        await s.Host.PostAsync(1, BlazorDocumentScenario.AttachPage);// the same document's page attaches again
        await Waits.UntilAsync(() => s.Pages.Count == 2 && s.RenderBatches(1) > batches, "the repeated attach rendered into document 1");
        IsolationProbe second = s.Pages.Latest;
        await Waits.UntilAsync(() => first.Scope.DisposeCalls > 0, "the replaced page context released");

        await s.SettleAsync();
        int beforeReload = s.Mark;
        await s.Host.RetireAsync(1);
        IsolationProbe next = await s.LoadPageAsync(2);
        await Waits.UntilAsync(() => second.Scope.DisposeCalls > 0, "document 1's page released after the reload", 10_000);
        await s.SettleAsync();
        await Assert.That(s.Host.Sent.Take(beforeReload).All(static m => m.Document == 1)).IsTrue();
        await Assert.That(s.SentSince(beforeReload).All(static m => m.Document == 2)).IsTrue();
        await Assert.That(first.Scope.DisposeCalls).IsEqualTo(1);
        await Assert.That(second.Scope.DisposeCalls).IsEqualTo(1);
        await Assert.That(next.Scope.DisposeCalls).IsEqualTo(0);
        await Assert.That(s.Pages.Subscribers).IsEqualTo(1);
        await s.CloseAsync(ct);
    }

    [Test]
    [Timeout(60_000)]
    public async Task FailedAttach_DoesNotAffectTheNextDocument(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        s.Pages.FailPage = 1;
        await s.Host.AttachAsync(1);
        await s.Host.PostAsync(1, BlazorDocumentScenario.AttachPage);
        await Waits.UntilAsync(() => s.Pages.Count == 1 && s.CountOf(1, "NotifyUnhandledException") > 0, "page 1's failure reported to document 1");
        IsolationProbe failed = s.Pages[1];

        await s.Host.RetireAsync(1);
        IsolationProbe page2 = await s.LoadPageAsync(2);
        await Waits.UntilAsync(() => failed.Scope.DisposeCalls > 0, "the failed page released", 10_000);
        await s.SettleAsync();
        Console.WriteLine($"FailedAttach document 1 received {s.CountOf(1, "NotifyUnhandledException")} failure notice(s); document 2 received {s.CountOf(2, "NotifyUnhandledException")}");
        await Assert.That(s.CountOf(2, "NotifyUnhandledException")).IsEqualTo(0);
        await Assert.That(page2.Number).IsEqualTo(2);
        await Assert.That(page2.Text).IsEqualTo("initial");
        await Assert.That(s.Window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Ready);
        await s.CloseAsync(ct);
    }

    [Test]
    [Timeout(60_000)]
    public async Task CloseDuringAttach_ReleasesEveryPageExactlyOnce(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        await s.Host.AttachAsync(1);
        await s.Host.PostAsync(1, BlazorDocumentScenario.AttachPage);
        await s.CloseAsync(ct);// without waiting for the page
        await Waits.UntilAsync(() => s.Pages.All.All(static p => p.Scope.DisposeCalls > 0), "every page released after close", 10_000);
        await Task.Delay(300, ct);
        Console.WriteLine($"CloseDuringAttach {s.Pages.Count} page(s) were created before the close");
        await Assert.That(s.Pages.All.All(static p => p.DisposeCalls == 1 && p.Scope.DisposeCalls == 1)).IsTrue();
    }

    [Test]
    [Timeout(60_000)]
    public async Task CloseWhileThePageAttaches_ReleasesItExactlyOnce(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        await s.Host.AttachAsync(1);
        await s.Host.PostAsync(1, BlazorDocumentScenario.AttachPage);
        await Waits.UntilAsync(() => s.Pages.Count == 1, "the page's root component created");
        await s.CloseAsync(ct);// the page may still be rendering or waiting for its first acknowledgement
        IsolationProbe page = s.Pages[1];
        await Waits.UntilAsync(() => page.Scope.DisposeCalls > 0, "the page released after close", 10_000);
        await Task.Delay(300, ct);
        await Assert.That(page.DisposeCalls).IsEqualTo(1);
        await Assert.That(page.Scope.DisposeCalls).IsEqualTo(1);
    }

    [Test]
    [Timeout(60_000)]
    [Arguments(true)]
    [Arguments(false)]
    public async Task DisposalJs_OfARetiredPage_NeverRunsInTheNextDocument_AndItsScopeIsReleased(bool catchesDisconnect, CancellationToken ct) {
        DisposalJs run = await RunDisposalJsAsync(catchesDisconnect, ct);
        Console.WriteLine($"DisposalJs (component catches JSDisconnectedException: {catchesDisconnect}) dispose-time calls sent: " +
            $"{BlazorDocumentScenario.Describe(run.DisposeCalls)}; page 1 scope disposals {run.Page1.Scope.DisposeCalls}; " +
            $"refusals caught by the component {run.Scenario.Pages.DisposeJsCaught}");
        await Assert.That(run.DisposeCalls.Any(static m => m.Document == 2)).IsFalse();// never executed by the next document
        await Assert.That(run.Page1.Scope.DisposeCalls).IsEqualTo(1);// the retired page's scope is released exactly once
        await Assert.That(run.Page2.Scope.DisposeCalls).IsEqualTo(0);
        await run.Scenario.CloseAsync(ct);
    }

    internal readonly record struct DisposalJs(BlazorDocumentScenario Scenario, IsolationProbe Page1, IsolationProbe Page2, SentMessage[] DisposeCalls);

    /// <summary>
    ///     Each page has a second root whose DisposeAsync calls JS (as a JS-module component does), catching
    ///     JSDisconnectedException or not; the page answers that call in whichever document it reaches. Then a reload.
    /// </summary>
    internal static async Task<DisposalJs> RunDisposalJsAsync(bool catchesDisconnect, CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct, static blazor => blazor.RootComponents.Add<DisposingJsProbe>("#dispose-probe"));
        s.Pages.DisposeJsCatches = catchesDisconnect;
        s.Answer("probe.dispose-js", static _ => "null");
        IsolationProbe page1 = await s.LoadPageAsync(1);
        await s.SettleAsync();
        await s.Host.RetireAsync(1);
        IsolationProbe page2 = await s.LoadPageAsync(2);
        await Waits.UntilAsync(() => page1.Scope.DisposeCalls > 0, "page 1's scope released", 15_000);
        await Task.Delay(300, ct);
        await s.SettleAsync();
        return new DisposalJs(s, page1, page2, s.JsCalls("probe.dispose-js"));
    }

    [Test]
    [Timeout(60_000)]
    public async Task ShutdownAndFreshStart_InOneProcess_StartsClean(CancellationToken ct) {
        BlazorDocumentScenario first = await BlazorDocumentScenario.StartAsync(ct);
        IsolationProbe firstPage = await first.LoadPageAsync(1);
        await first.CloseAsync(ct);

        BlazorDocumentScenario second = await BlazorDocumentScenario.StartAsync(ct);
        IsolationProbe secondPage = await second.LoadPageAsync(1);
        int batches = second.RenderBatches(1);
        second.Pages.Tick();
        await Waits.UntilAsync(() => second.RenderBatches(1) > batches, "the second application's producer drives its page");
        await Assert.That(ReferenceEquals(first.Pages, second.Pages)).IsFalse();
        await Assert.That(second.Pages.Count).IsEqualTo(1);
        await Assert.That(firstPage.Ticks).IsEqualTo(0);
        await Assert.That(secondPage.Ticks).IsEqualTo(1);
        await second.CloseAsync(ct);
    }

    // ---- resources, in-page routing, roots and dispatch -----------------------------------------------------------
    [Test]
    [Timeout(60_000)]
    public async Task Resources_StayAvailable_DuringBootstrapAndTurnover(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct, static blazor => blazor.AddSingleFileProvider(new InMemoryFiles()));
        var phases = new List<(string Phase, string Statuses)> { ("before any document", await StatusesAsync(s, ct)) };
        await s.Host.AttachAsync(1);
        phases.Add(("document 1 attached, no page yet", await StatusesAsync(s, ct)));
        await s.Host.PostAsync(1, BlazorDocumentScenario.AttachPage);
        await Waits.UntilAsync(() => s.RenderBatches(1) > 0, "page 1 rendered");
        phases.Add(("page 1 live", await StatusesAsync(s, ct)));
        await s.Host.RetireAsync(1);
        phases.Add(("document 1 retired, none current", await StatusesAsync(s, ct)));
        await s.Host.AttachAsync(2);
        phases.Add(("document 2 attached, no page yet", await StatusesAsync(s, ct)));
        await s.Host.PostAsync(2, BlazorDocumentScenario.AttachPage);
        await Waits.UntilAsync(() => s.RenderBatches(2) > 0, "page 2 rendered");
        phases.Add(("page 2 live", await StatusesAsync(s, ct)));

        foreach ((string phase, string statuses) in phases) Console.WriteLine($"RESOURCES {phase}: {statuses}");
        foreach ((_, string statuses) in phases) await Assert.That(statuses).IsEqualTo("index 200, app.css 200, missing 404, untrusted 403");
        await s.CloseAsync(ct);
    }

    [Test]
    [Timeout(60_000)]
    public async Task InPageRouting_IsNotADocumentReplacement(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        IsolationProbe page1 = await s.LoadPageAsync(1);
        await s.SettleAsync();
        await s.Host.PostAsync(1, "__bwv:[\"OnLocationChanged\",\"app://localhost/route-b\",null,false]");
        await Waits.UntilAsync(() => page1.Navigation.Uri == "app://localhost/route-b", "page 1 followed the in-page route");
        int batches = s.RenderBatches(1);
        await page1.OnDispatcher(() => page1.Rerender("after-route"));
        await Waits.UntilAsync(() => s.RenderBatches(1) > batches, "page 1 still renders into document 1");
        await Assert.That(s.Pages.Count).IsEqualTo(1);
        await Assert.That(page1.Scope.DisposeCalls).IsEqualTo(0);
        await Assert.That(s.Host.Sent.All(static m => m.Document == 1)).IsTrue();
        await s.CloseAsync(ct);
    }

    [Test]
    [Timeout(60_000)]
    public async Task RootComponents_RegisteredAtRuntime_FollowTheCurrentAndLaterPages(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        IsolationProbe page1 = await s.LoadPageAsync(1);
        var manager = s.Window.ServiceProvider!.GetRequiredService<IInfiniFrameWebViewManager>();
        await manager.Dispatcher.InvokeAsync(() => manager.AddRootComponentAsync(typeof(SecondRoot), "#second", ParameterView.Empty));
        await Waits.UntilAsync(() => s.Pages.SecondRoots(page1.Scope) == 1, "the new root rendered in page 1");

        await s.Host.RetireAsync(1);
        IsolationProbe page2 = await s.LoadPageAsync(2);
        await Waits.UntilAsync(() => s.Pages.SecondRoots(page2.Scope) == 1, "page 2 received both roots");
        await manager.Dispatcher.InvokeAsync(() => manager.RemoveRootComponentAsync("#second"));
        await s.Host.RetireAsync(2);
        IsolationProbe page3 = await s.LoadPageAsync(3);
        await s.SettleAsync();
        await Assert.That(s.Pages.SecondRoots(page1.Scope)).IsEqualTo(1);
        await Assert.That(s.Pages.SecondRoots(page2.Scope)).IsEqualTo(1);
        await Assert.That(s.Pages.SecondRoots(page3.Scope)).IsEqualTo(0);
        await Assert.That(s.Pages.Count).IsEqualTo(3);// one IsolationProbe root per page
        await s.CloseAsync(ct);
    }

    [Test]
    [Timeout(60_000)]
    public async Task TryDispatch_ReachesTheCurrentDocumentsPage(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        var manager = s.Window.ServiceProvider!.GetRequiredService<IInfiniFrameWebViewManager>();
        await Assert.That(await manager.TryDispatchAsync(static _ => {})).IsFalse();// no page yet
        IsolationProbe page1 = await s.LoadPageAsync(1);
        await s.Host.RetireAsync(1);
        IsolationProbe page2 = await s.LoadPageAsync(2);
        await s.SettleAsync();
        PageScopeProbe? reached = null;
        bool dispatched = await manager.TryDispatchAsync(services => reached = services.GetRequiredService<PageScopeProbe>());
        await Assert.That(dispatched).IsTrue();
        await Assert.That(ReferenceEquals(reached, page2.Scope)).IsTrue();
        await Assert.That(ReferenceEquals(reached, page1.Scope)).IsFalse();
        await s.CloseAsync(ct);
    }

    [Test]
    [Timeout(60_000)]
    public async Task CurrentDocumentSendFailure_StillFaultsTheWindow(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        IsolationProbe page1 = await s.LoadPageAsync(1);
        await s.SettleAsync();
        s.Host.SendOverride = static (_, _, _) => Task.FromException(new IOException("provider pipe broken"));
        await page1.OnDispatcher(() => page1.Rerender("cannot-be-delivered"));
        var hosted = (InfiniFrameHostedWindow)s.Window;
        await Waits.UntilAsync(() => hosted.Fault is not null, "the window faulted", 10_000);
        await Assert.That(hosted.Fault).IsTypeOf<InfiniFrameHostOperationException>();
        await Assert.That(hosted.Fault!.InnerException).IsTypeOf<IOException>();
        try {
            await Waits.WithinAsync(s.Run, "Run() after the fault", 15_000);
        }
        catch (Exception exception) when (exception is not TimeoutException) {
            Console.WriteLine($"SendFailure Run() ended with {exception.GetType().Name}");
        }
        InfiniFrameHostSelection.ResetForTests();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>Starts a JS call and returns its task (a synchronous refusal becomes a faulted task).</summary>
    internal static Task Call(Func<ValueTask> call) {
        try {
            return call().AsTask();
        }
        catch (Exception exception) {
            return Task.FromException(exception);
        }
    }

    internal static Task<T> Call<T>(Func<ValueTask<T>> call) {
        try {
            return call().AsTask();
        }
        catch (Exception exception) {
            return Task.FromException<T>(exception);
        }
    }

    internal static string Capture(Action action) {
        try {
            action();
            return "returned";
        }
        catch (Exception exception) {
            return $"threw {exception.GetType().Name}";
        }
    }

    internal static long DotNetObjectId(string argsJson) {
        using JsonDocument json = JsonDocument.Parse(argsJson);
        return json.RootElement[0].GetProperty("__dotNetObject").GetInt64();
    }

    /// <summary>Whether an <c>EndInvokeDotNet</c> message reports success (its third element).</summary>
    internal static bool EndInvokeDotNetSucceeded(string message) {
        using JsonDocument json = JsonDocument.Parse(message.AsMemory(6));
        return json.RootElement[2].GetBoolean();
    }

    private static async Task<string> StatusesAsync(BlazorDocumentScenario s, CancellationToken ct) {
        var parts = new List<string>();
        foreach ((string name, string url) in new[] {
                     ("index", "app://localhost/"), ("app.css", "app://localhost/app.css"),
                     ("missing", "app://localhost/missing.css"), ("untrusted", "app://untrusted.example/x.js")
                 }) {
            await using InfiniFrameHostResourceResponse response =
                await s.Host.Events.GetResourceAsync(new InfiniFrameHostResourceRequest { Method = "GET", Url = new Uri(url) }, ct);
            parts.Add($"{name} {response.StatusCode}");
        }
        return string.Join(", ", parts);
    }

    public sealed class SecondRoot : ComponentBase {
        [Inject] public PageRegistry Registry { get; set; } = default!;
        [Inject] public PageScopeProbe Scope { get; set; } = default!;
        protected override void OnInitialized() => Registry.RegisterSecondRoot(Scope);
        protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.AddMarkupContent(0, "<p>second root</p>");
    }
}
