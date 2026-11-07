// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Security;
using InfiniFrame.Window.Builder;
using InfiniFrame.Window.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniTests.InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Drives the core hosted runtime and window facade directly (no application, so no process-wide selection latch).
/// </summary>
internal sealed class HostedHarness : IAsyncDisposable {
    internal const string Origin = "app://localhost/";
    private readonly List<InfiniFrameHostedWindow> _windows = [];

    private HostedHarness(ScriptedHostProvider provider, InfiniFrameHostedRuntime runtime, ServiceProvider services) {
        Provider = provider;
        Runtime = runtime;
        Services = services;
    }

    internal ScriptedHostProvider Provider { get; }
    internal InfiniFrameHostedRuntime Runtime { get; }
    internal ServiceProvider Services { get; }
    internal ScriptedHost Host => Provider.Host;

    internal static async Task<HostedHarness> StartAsync(ScriptedHostProvider? provider = null, TimeSpan? closeTimeout = null) {
        provider ??= new ScriptedHostProvider();
        ServiceProvider services = new ServiceCollection().AddLogging().BuildServiceProvider();
        var runtime = new InfiniFrameHostedRuntime(provider, Guid.NewGuid()) { CloseTimeout = closeTimeout ?? TimeSpan.FromSeconds(30) };
        await runtime.StartAsync(services, CancellationToken.None);
        return new HostedHarness(provider, runtime, services);
    }

    internal static InfiniFrameWindowBuilder NewBuilder(Action<IInfiniFrameWindowBuilder>? configure = null) {
        var builder = new InfiniFrameWindowBuilder();
        ConfigureDefaults(builder);
        configure?.Invoke(builder);
        return builder;
    }

    internal static void ConfigureDefaults(IInfiniFrameWindowBuilder builder) {
        builder.SetStartPageUrl(Origin);
        InfiniFrameUriSecurityPolicyRegistry.ConfigureForBuilder(builder, policy => policy.AddTrustedOrigin(new Uri(Origin)));
    }

    internal InfiniFrameHostedWindow Build(Action<IInfiniFrameWindowBuilder>? configure = null) {
        InfiniFrameHostedWindow window = InfiniFrameHostedWindow.Create(Runtime, NewBuilder(configure), Services);
        _windows.Add(window);
        return window;
    }

    internal async Task<(InfiniFrameHostedWindow Window, ScriptedWindowHostCore Host)> CreateAsync(Action<IInfiniFrameWindowBuilder>? configure = null) {
        InfiniFrameHostedWindow window = Build(configure);
        await Runtime.CreateWindowsAsync([window], CancellationToken.None);
        return (window, Host.Windows[^1]);
    }

    /// <summary>Creates a window and attaches its first document.</summary>
    internal async Task<(InfiniFrameHostedWindow Window, ScriptedWindowHostCore Host)> CreateAttachedAsync(
        ulong document = 1, Action<IInfiniFrameWindowBuilder>? configure = null) {
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await CreateAsync(configure);
        await window.WaitForReadyAsync();
        await host.AttachAsync(document);
        return (window, host);
    }

    /// <summary>Waits until the dispatcher has run everything queued before this call.</summary>
    internal Task DrainDispatcherAsync() => Runtime.Dispatcher.InvokeAsync(static () => true);

    public async ValueTask DisposeAsync() {
        foreach (InfiniFrameHostedWindow window in _windows) await window.DisposeAsync();
        await Runtime.DisposeAsync();
        await Services.DisposeAsync();
    }
}

internal static class Expect {
    /// <summary>Asserts that <paramref name="action" /> throws <typeparamref name="TException" /> and returns it.</summary>
    internal static async Task<TException> ThrowsAsync<TException>(Func<Task> action) where TException : Exception =>
        await Assert.ThrowsAsync<TException>(action) ?? throw new InvalidOperationException($"Expected {typeof(TException).Name}.");
}

internal static class Waits {
    internal static async Task UntilAsync(Func<bool> condition, string what, int timeoutMilliseconds = 5000) {
        DateTime deadline = DateTime.UtcNow.AddMilliseconds(timeoutMilliseconds);
        while (!condition()) {
            if (DateTime.UtcNow > deadline) throw new TimeoutException($"Timed out waiting for: {what}");
            await Task.Delay(5);
        }
    }

    /// <summary>Completes if <paramref name="task" /> completes within the bound, otherwise throws (deadlock detector).</summary>
    internal static async Task WithinAsync(Task task, string what, int timeoutMilliseconds = 5000) {
        Task finished = await Task.WhenAny(task, Task.Delay(timeoutMilliseconds));
        if (finished != task) throw new TimeoutException($"Did not complete (possible deadlock): {what}");
        await task;
    }
}
