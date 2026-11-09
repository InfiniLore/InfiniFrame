// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;
using InfiniFrame;
using InfiniFrame.Hosting;
using InfiniFrame.Js.Interop;
using InfiniFrame.Window.Hosting;

namespace InfiniTests.InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// Documents and messages, including generation races at the facade boundary.
[NotInParallel]
public sealed class HostedDocumentTests {
    [Test]
    [Timeout(30_000)]
    public async Task Messages_OnlyFromTheCurrentDocument_WithBrowserDerivedOrigin(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        var received = new ConcurrentQueue<(string Message, string? Origin)>();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1, builder =>
            builder.RegisterWebMessageReceivedHandler((_, message, origin) => received.Enqueue((message, origin))));

        await host.PostAsync(1, "__bwv:from-1");
        await host.PostAsync(2, "__bwv:from-unattached-2");
        // PostAsync only queues delivery; finish document 1's positive case before replacing it.
        await harness.DrainDispatcherAsync();
        await host.AttachAsync(2, "app://localhost/second.html");
        await host.PostAsync(1, "__bwv:from-retired-1");
        await host.PostAsync(2, "__bwv:from-2");
        await harness.DrainDispatcherAsync();

        (string Message, string? Origin)[] messages = [.. received];
        await Assert.That(messages.Length).IsEqualTo(2);
        await Assert.That(messages[0].Message).IsEqualTo("__bwv:from-1");
        await Assert.That(messages[0].Origin).IsEqualTo("app://localhost/");
        await Assert.That(messages[1].Message).IsEqualTo("__bwv:from-2");
        await Assert.That(messages[1].Origin).IsEqualTo("app://localhost/second.html");
        await Assert.That(window.CurrentDocument).IsEqualTo(2UL);
    }

    [Test]
    [Timeout(30_000)]
    public async Task NonIncreasingDocumentId_FaultsTheWindow(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(5);

        await host.AttachAsync(5);
        await window.WaitForTeardownAsync(ct);
        await Assert.That(window.Fault).IsTypeOf<InvalidOperationException>();
        await Assert.That(window.Fault!.Message).Contains("strictly increasing");
    }

    [Test]
    [Timeout(30_000)]
    public async Task QueuedInboundCallback_ForARetiredDocument_IsCancelledNotRedirected(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        var received = new ConcurrentQueue<string>();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1, builder =>
            builder.RegisterWebMessageReceivedHandler((_, message) => received.Enqueue(message)));

        using var release = new ManualResetEventSlim();
        Task<InfiniFrameDispatchResult> blocker = window.DispatchAsync(() => release.Wait(ct), TimeSpan.FromSeconds(20), ct).AsTask();
        await host.PostAsync(1, "__bwv:queued-for-1");
        await host.AttachAsync(2);
        release.Set();
        await blocker;
        await host.PostAsync(2, "__bwv:for-2");
        await harness.DrainDispatcherAsync();

        await Assert.That(string.Join(",", received)).IsEqualTo("__bwv:for-2");
    }

    [Test]
    [Timeout(30_000)]
    public async Task Reply_IsBoundToTheProducingDocument_AndNeverRedirectedAfterReplacement(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        var lateReplyOutcome = new TaskCompletionSource<Exception?>(TaskCreationOptions.RunContinuationsAsynchronously);
        var attachedSecond = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1, builder =>
            builder.RegisterWebMessageReceivedHandler((sender, message) => {
                if (message == "__bwv:ping") sender.SendWebMessage("pong");
                if (message != "__bwv:slow") return;
                // Work started by document 1's callback that completes only after document 2 replaced it.
                _ = Task.Run(async () => {
                    await attachedSecond.Task;
                    try {
                        await sender.SendWebMessageAsync("late-reply-for-1");
                        lateReplyOutcome.TrySetResult(null);
                    }
                    catch (Exception exception) {
                        lateReplyOutcome.TrySetResult(exception);
                    }
                });
            }));

        await host.PostAsync(1, "__bwv:ping");
        await Waits.UntilAsync(() => !host.Sent.IsEmpty, "pong");
        await host.PostAsync(1, "__bwv:slow");
        await harness.DrainDispatcherAsync();
        await host.AttachAsync(2);
        attachedSecond.SetResult();
        Exception? outcome = await lateReplyOutcome.Task.WaitAsync(TimeSpan.FromSeconds(10), ct);

        SentMessage[] sent = [.. host.Sent];
        await Assert.That(sent.Length).IsEqualTo(1);
        await Assert.That(sent[0].Document).IsEqualTo(1UL);
        await Assert.That(sent[0].Message).IsEqualTo("pong");
        await Assert.That(sent[0].ThreadId).IsEqualTo(harness.Host.LoopThreadId);
        await Assert.That(outcome).IsTypeOf<InfiniFrameHostDocumentRetiredException>();
        await Assert.That(((InfiniFrameHostDocumentRetiredException)outcome!).Document.Value).IsEqualTo(1UL);
    }

    [Test]
    [Timeout(30_000)]
    public async Task SendBeforeAnyDocument_IsBoundToTheFirstDocument(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAsync();
        await window.WaitForReadyAsync(ct);

        Task send = window.SendWebMessageAsync("early", ct).AsTask();
        await Task.Delay(50, ct);
        await Assert.That(send.IsCompleted).IsFalse();
        await host.AttachAsync(7);
        await Waits.WithinAsync(send, "early send");

        SentMessage[] sent = [.. host.Sent];
        await Assert.That(sent.Length).IsEqualTo(1);
        await Assert.That(sent[0].Document).IsEqualTo(7UL);
    }

    [Test]
    [Timeout(30_000)]
    public async Task SendAfterRetirementWithoutReplacement_ReportsRetirement(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(3);

        await host.RetireAsync(3);
        await Assert.ThrowsAsync<InfiniFrameHostDocumentRetiredException>(async () => await window.SendWebMessageAsync("after", ct));
        await host.AttachAsync(4);
        await window.SendWebMessageAsync("to-4", ct);
        SentMessage[] sent = [.. host.Sent];
        await Assert.That(sent.Length).IsEqualTo(1);
        await Assert.That(sent[0].Document).IsEqualTo(4UL);
    }

    [Test]
    [Timeout(30_000)]
    public async Task JavaScriptEvaluation_KeepsTheDefaultRawJsonResultContract(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1);
        // The scripted page answers every evaluation request the way InfiniFrame.js does: post the raw JSON result.
        host.OnSent = sent => {
            InteropEnvelopeParseResult request = InteropEnvelopeProtocol.ParseIncomingMessage(sent.Message);
            if (request.MessageId != JsHandlerNames.JavaScriptEvalRequest || request.Payload is null) return;
            using var payload = System.Text.Json.JsonDocument.Parse(request.Payload);
            string requestId = payload.RootElement.GetProperty("requestId").GetString()!;
            string response = InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.JavaScriptEvalResult,
                $$"""{"requestId":"{{requestId}}","result":"abc"}""");
            host.Host.Post(() => host.Events.MessageReceived(new InfiniFrameDocumentId(sent.Document), response));
        };

        string? raw = await window.ExecuteJavaScriptAsync("'abc'", ct);
        string? typed = await window.ExecuteJavaScriptAsync<string>("'abc'", ct);
        await Assert.That(raw).IsEqualTo("\"abc\"");
        await Assert.That(typed).IsEqualTo("abc");
    }

    [Test]
    [Timeout(30_000)]
    public async Task JavaScriptEvaluation_IsCancelledWhenItsDocumentRetires(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1);

        Task<string?> evaluation = window.ExecuteJavaScriptAsync("never answered", ct).AsTask();
        await Waits.UntilAsync(() => !host.Sent.IsEmpty, "evaluation request delivered");
        await host.AttachAsync(2);
        InfiniFrameHostDocumentRetiredException exception = await Expect.ThrowsAsync<InfiniFrameHostDocumentRetiredException>(() => evaluation);
        await Assert.That(exception.Document.Value).IsEqualTo(1UL);
    }

    [Test]
    [Timeout(30_000)]
    public async Task Navigation_SucceedsOnAttach_IsSupersededByALaterDocument_AndRejectsUntrustedUris(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1);

        NavigationResult first = await window.LoadAsync(new Uri("app://localhost/page2"), ct);
        await Assert.That(first.Status).IsEqualTo(NavigationStatus.Succeeded);
        await Assert.That(window.CurrentDocument).IsEqualTo(2UL);

        host.AttachOnNavigate = false;
        Task<NavigationResult> superseded = window.LoadAsync(new Uri("app://localhost/page3"), ct);
        await Waits.UntilAsync(() => host.Navigations.Count == 2, "second navigation delivered");
        await host.AttachAsync(9);
        await Assert.That((await superseded).Status).IsEqualTo(NavigationStatus.Superseded);

        NavigationResult untrusted = await window.LoadAsync(new Uri("https://example.invalid/"), ct);
        await Assert.That(untrusted.Status).IsEqualTo(NavigationStatus.Failed);
        await Assert.That(host.Navigations.Count).IsEqualTo(2);
        await Assert.That(window.GetCurrentUrl()).IsEqualTo("app://localhost/");
    }
}
