// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrameTests.Playwright.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class BlazorPlaywrightContextBase<TRootComponent>(string documentTitle) : PlaywrightContextBase(documentTitle)
    where TRootComponent : IComponent {
    public override IInfiniFrameWindow Window => _window!;
    
    [UsedImplicitly] private InfiniFrameBlazorApp? _app; // kept for future reference
    private IInfiniFrameWindow? _window;
    private Thread? _appThread;
    private readonly int _playwrightDevtoolsPort = PlaywrightConnectionUtility.GetAvailablePort();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected async Task BeforeAllAsync() {
        using var startupCancellation = new CancellationTokenSource(TimeSpan.FromSeconds(90));
        var ready = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);

        _appThread = CreateAppThread(ready);
        _appThread.Start();

        await ready.Task.WaitAsync(startupCancellation.Token);
    }

    protected void AfterAll() {
        BeforeAssemblyTeardown();
        CloseWindowSafely();

        JoinAppThreadSafely();

        _app = null;
        _window = null;
        _appThread = null;
    }

    protected override Uri CreatePlaywrightConnectionUri(string relativeUrl)
        => new(new Uri($"http://127.0.0.1:{_playwrightDevtoolsPort}"), relativeUrl);

    protected virtual void ConfigureServices(IServiceCollection services) {}

    protected virtual void ConfigureRootComponents(IInfiniFrameRootComponentList rootComponents) {}

    protected virtual void ConfigureWindowBuilder(IInfiniFrameWindowBuilder windowBuilder, int playwrightDevtoolsPort) {
        windowBuilder
            .SetTitle(DefaultDocumentTitle)
            .SetBrowserControlInitParameters($"--remote-debugging-port={playwrightDevtoolsPort}")
                .RegisterWindowManagementWebMessageHandler()
                .RegisterFullScreenWebMessageHandler()
                .RegisterOpenExternalTargetWebMessageHandler()
                .RegisterTitleChangedWebMessageHandler()
                .RegisterWindowClosingHandler((_, _) => {
                    bool suppressClose = OnWindowClosingRequested();
                    return suppressClose ? WindowClosingResult.Cancel : WindowClosingResult.Close;
                });
    }

    protected virtual void RunApp(InfiniFrameBlazorApp app)
        => app.Run();

    private Thread CreateAppThread(TaskCompletionSource<object?> ready) {
        var thread = new Thread(() => RunAppOnThread(ready)) {
            IsBackground = true,
            Name = $"InfiniFrame Playwright {typeof(TRootComponent).Name} App Thread"
        };

        if (OperatingSystem.IsWindows())
            thread.SetApartmentState(ApartmentState.STA);

        return thread;
    }

    private void RunAppOnThread(TaskCompletionSource<object?> ready) {
        try {
            var builder = InfiniFrameBlazorAppBuilder.CreateDefault();

            ConfigureServices(builder.Services);
            ConfigureRootComponents(builder.RootComponents);
            builder.RootComponents.Add<TRootComponent>("app");
            builder.WithInfiniFrameWindowBuilder(windowBuilder => ConfigureWindowBuilder(windowBuilder, _playwrightDevtoolsPort));

            InfiniFrameBlazorApp app = builder.Build();
            var window = app.ServiceProvider.GetRequiredService<IInfiniFrameWindow>();

            _app = app;
            _window = window;
            ready.SetResult(null);

            RunApp(app);
        }
        catch (InvalidOperationException ex) {
            ready.TrySetException(ex);
        }
        catch (TimeoutException ex) {
            ready.TrySetException(ex);
        }
        catch (Microsoft.Playwright.PlaywrightException ex) {
            ready.TrySetException(ex);
        }
    }

    private void CloseWindowSafely() {
        try {
            _window?.Close();
        }
        catch (ApplicationException) {
            // ignored
        }
        catch (ObjectDisposedException) {
            // ignored
        }
    }

    private void JoinAppThreadSafely() {
        Thread? appThread = _appThread;
        if (appThread is null)
            return;

        if (!appThread.Join(TimeSpan.FromSeconds(10))) {
            Console.WriteLine(
                $"[PlaywrightTeardown] Background app thread '{appThread.Name}' did not stop within timeout.");
        }
    }
}
