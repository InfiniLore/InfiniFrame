// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.BlazorWebView;
using InfiniFrame.Utilities;
using InfiniFrame.Window.Features.WebMessaging.Handlers;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

namespace InfiniAutomationTests.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class BlazorPlaywrightContextBase<TRootComponent>(string documentTitle) : PlaywrightContextBase(documentTitle)
    where TRootComponent : IComponent {
    private readonly int _playwrightDevtoolsPort = PlaywrightConnectionUtility.GetAvailablePort();

    [UsedImplicitly]
    private Thread? _appThread;
    private IInfiniFrameWindow? _window;
    public override IInfiniFrameWindow Window => _window!;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected async Task BeforeAllAsync() {
        TimeSpan startupTimeout = TimeSpan.FromSeconds(90);
        using var startupCancellation = new CancellationTokenSource(startupTimeout);
        var ready = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);

        _appThread = CreateAppThread(ready);
        _appThread.Start();

        try {
            await ready.Task.WaitAsync(startupCancellation.Token);
        }
        catch (OperationCanceledException) {
            throw new TimeoutException(
                $"The Blazor application did not create its window within {startupTimeout.TotalSeconds:0} seconds.");
        }

        Uri cdpEndpoint = PlaywrightConnectionUtility.CreateCdpConnectionUrl(_playwrightDevtoolsPort);
        using var probeCancellation = CancellationTokenSource.CreateLinkedTokenSource(startupCancellation.Token);
        while (true) {
            if (RemoteDebuggingUtility.TryProbeEndpoint(cdpEndpoint, out _)) {
                return;
            }
            try {
                await Task.Delay(500, probeCancellation.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) {
                throw new TimeoutException(
                    $"The Blazor CDP endpoint '{cdpEndpoint}' was not reachable within " +
                    $"{startupTimeout.TotalSeconds:0} seconds.");
            }
        }
    }

    protected void AfterAll() {
        BeforeAssemblyTeardown();
        CloseWindowSafely();

        JoinAppThreadSafely();

        _window = null;
        _appThread = null;
    }

    protected override Uri CreatePlaywrightConnectionUri(string relativeUrl)
        => new(PlaywrightConnectionUtility.CreateCdpConnectionUrl(_playwrightDevtoolsPort), relativeUrl);

    protected virtual void ConfigureServices(IServiceCollection services) {}

    protected virtual void ConfigureRootComponents(IInfiniFrameRootComponentList rootComponents) {}

    protected virtual void ConfigureWindowBuilder(IInfiniFrameWindowBuilder windowBuilder, int playwrightDevtoolsPort) {
        if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux()) windowBuilder.Debugging.SetRemoteDebuggingPort(playwrightDevtoolsPort);
        windowBuilder
            .SetIconFile("wwwroot/favicon.ico")
            .SetTitle(DefaultDocumentTitle)
            .RegisterWindowManagementWebMessageHandler()
            .RegisterFullScreenWebMessageHandler()
            .RegisterOpenExternalTargetWebMessageHandler()
            .RegisterTitleChangedWebMessageHandler()
            .RegisterWindowClosingHandler((_, _) => {
                bool suppressClose = OnWindowClosingRequested();
                return suppressClose ? WindowClosingResult.Cancel : WindowClosingResult.Close;
            });
    }

    protected virtual void RunApp(InfiniFrameApplication app)
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
            InfiniFrameApplicationBuilder builder = InfiniFrameApplication.CreateBuilder();

            ConfigureServices(builder.Services);
            builder.WithWindow(windowBuilder => ConfigureWindowBuilder(windowBuilder, _playwrightDevtoolsPort));
            builder.UseBlazorWebView(configuration => {
                ConfigureRootComponents(configuration.RootComponents);
                configuration.RootComponents.Add<TRootComponent>("app");
            });

            InfiniFrameApplication application = builder.Build();
            application.WindowCreated += window => {
                _window = window;
                ready.TrySetResult(null);
            };
            RunApp(application);
        }
        catch (InvalidOperationException ex) {
            ready.TrySetException(ex);
        }
        catch (TimeoutException ex) {
            ready.TrySetException(ex);
        }
        catch (PlaywrightException ex) {
            ready.TrySetException(ex);
        }
        catch (Exception ex) {
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
