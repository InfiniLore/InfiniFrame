// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.BlazorWebView;
using InfiniFrame.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniTests.InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// The page-scoped HttpClient that InfiniFrame registers wraps the application's singleton InfiniFrameHttpHandler. A page
// scope must not dispose that handler: the next page's client must keep working, and the application keeps owning the
// handler's final disposal. The probe uses an ftp:// URI: a live handler hands it to its inner handler, which refuses the
// scheme without opening a socket, while a disposed handler throws ObjectDisposedException first. No network is used.
[NotInParallel]
public sealed class HostedHttpClientOwnershipTests {
    private const string LiveHandler = "NotSupportedException";
    private const string DisposedHandler = "ObjectDisposedException";

    [Test]
    [Timeout(60_000)]
    public async Task PageScopedHttpClient_StillWorksAfterTheFirstPageScopeIsDisposed(CancellationToken ct) {
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct);
        var manager = s.Window.ServiceProvider!.GetRequiredService<IInfiniFrameWebViewManager>();
        IsolationProbe page1 = await s.LoadPageAsync(1);
        HttpClient? first = null;
        await manager.TryDispatchAsync(services => first = services.GetRequiredService<HttpClient>());
        string before = await ProbeAsync(first!);

        await s.Host.RetireAsync(1);
        await s.LoadPageAsync(2);
        await Waits.UntilAsync(() => page1.Scope.DisposeCalls > 0, "page 1's scope released", 10_000);
        HttpClient? second = null;
        await manager.TryDispatchAsync(services => second = services.GetRequiredService<HttpClient>());
        string after = await ProbeAsync(second!);

        Console.WriteLine($"HttpClient page 1 before the reload: {before}; page 2 after page 1's scope was disposed: {after}");
        await Assert.That(ReferenceEquals(first, second)).IsFalse();// one client per page scope
        await Assert.That(before).StartsWith(LiveHandler);
        await Assert.That(after).StartsWith(LiveHandler);
        await s.CloseAsync(ct);
    }

    [Test]
    [Timeout(60_000)]
    public async Task TheApplication_StillOwnsAndDisposesTheSharedHandler(CancellationToken ct) {
        InfiniFrameHostSelection.ResetForTests();
        InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .UseHostProvider(new ScriptedHostProvider())
            .WithWindow("main", static _ => {})
            .UseBlazorWebView(static blazor => blazor.RootComponents.Add<IsolationProbe>("#app"), "main")
            .Build();
        IServiceProvider root = application.RootServiceProvider;
        var handler = root.GetRequiredService<InfiniFrameHttpHandler>();

        // Two page scopes in turn, as two documents' pages have them.
        string first, second;
        await using (AsyncServiceScope scope = root.CreateAsyncScope()) first = await ProbeAsync(scope.ServiceProvider.GetRequiredService<HttpClient>());
        await using (AsyncServiceScope scope = root.CreateAsyncScope()) second = await ProbeAsync(scope.ServiceProvider.GetRequiredService<HttpClient>());
        await application.DisposeAsync();
        string afterApplication;
        using (var client = new HttpClient(handler, disposeHandler: false)) afterApplication = await ProbeAsync(client);

        Console.WriteLine($"HttpClient first scope: {first}; second scope: {second}; after the application was disposed: {afterApplication}");
        await Assert.That(first).StartsWith(LiveHandler);
        await Assert.That(second).StartsWith(LiveHandler);// the first scope's client did not dispose the shared handler
        await Assert.That(afterApplication).StartsWith(DisposedHandler);// the application's root provider disposed it
        InfiniFrameHostSelection.ResetForTests();
    }

    private static async Task<string> ProbeAsync(HttpClient client) {
        try {
            using HttpResponseMessage response = await client.GetAsync("ftp://127.0.0.1/no-such-file");
            return $"status {(int)response.StatusCode}";
        }
        catch (Exception exception) {
            return $"{exception.GetType().Name}: {exception.Message}";
        }
    }
}
