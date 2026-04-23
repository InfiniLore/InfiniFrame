// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using InfiniFrame.Js.Interop.MessageHandlers;
using InfiniFrameAutomationTests.BlazorWebView.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace InfiniFrameAutomationTests.BlazorWebView.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class BlazorWebViewAutomationRuntimeContext : IAutomationRuntimeContext {
    public static BlazorWebViewAutomationRuntimeContext Instance { get; } = new();

    private readonly AutomationSessionManager _automation = new(AutomationPortUtility.GetAvailablePort());
    // ReSharper disable once NotAccessedField.Local
    private InfiniFrameBlazorApp? _blazorApp;
    private IInfiniFrameWindow? _window;
    private Thread? _appThread;

    public string DefaultDocumentTitle => "InfiniFrame Playwright BlazorWebView";

    public IInfiniFrameWindow Window => _window!;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private BlazorWebViewAutomationRuntimeContext() {}

    [Before(Assembly)]
    public static void BeforeAll(AssemblyHookContext _)
        => Instance.Start();

    [After(Assembly)]
    public static void AfterAll(AssemblyHookContext _)
        => Instance.Stop();

    private void Start() {
        AutomationSessionManager.EnableLinuxWebKitAutomationIfNeeded();

        using var startupCancellation = new CancellationTokenSource(TimeSpan.FromSeconds(90));
        var ready = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);

        Thread thread = new(() => {
            try {
                var builder = InfiniFrameBlazorAppBuilder.CreateDefault();

                builder.Services.AddMudServices();
                builder.RootComponents.Add<App>("app");

                builder.WithInfiniFrameWindowBuilder(windowBuilder => {
                    windowBuilder
                        .SetTitle(DefaultDocumentTitle)
                        .RegisterWindowManagementWebMessageHandler()
                        .RegisterFullScreenWebMessageHandler()
                        .RegisterOpenExternalTargetWebMessageHandler()
                        .RegisterTitleChangedWebMessageHandler()
                        .RegisterWindowClosingHandler((_, _) => _automation.OnWindowClosingRequested());

                    if (OperatingSystem.IsWindows()) {
                        windowBuilder.SetBrowserControlInitParameters(_automation.GetWindowsRemoteDebuggingArgs());
                    }
                });

                InfiniFrameBlazorApp app = builder.Build();
                IInfiniFrameWindow window = app.ServiceProvider.GetRequiredService<IInfiniFrameWindow>();

                _blazorApp = app;
                _window = window;
                ready.SetResult(null);

                app.Run();
            }
            catch (Exception ex) {
                ready.TrySetException(ex);
            }
        }) {
            IsBackground = true,
            Name = "InfiniFrame Automation BlazorWebView App Thread"
        };

        if (OperatingSystem.IsWindows()) {
            thread.SetApartmentState(ApartmentState.STA);
        }

        _appThread = thread;
        thread.Start();

        ready.Task.WaitAsync(startupCancellation.Token).GetAwaiter().GetResult();
    }

    private void Stop() {
        AutomationSessionManager.DelayIfVisibleDebugEnabled();
        _automation.Dispose();

        try {
            _window?.Close();
        }
        catch (ApplicationException) {
            // ignored
        }
        catch (ObjectDisposedException) {
            // ignored
        }

        if (_appThread is not null && !_appThread.Join(TimeSpan.FromSeconds(5))) {
            _appThread.Interrupt();
        }

        _blazorApp = null;
        _window = null;
        _appThread = null;
    }

    public Task<IAutomationPage> GetOrCreatePageAsync(string relativeUrl = "/")
        => _automation.GetOrCreatePageAsync(relativeUrl);

    public void ResetWindowCloseRequestCount()
        => _automation.ResetWindowCloseRequestCount();

    public int GetWindowCloseRequestCount()
        => _automation.GetWindowCloseRequestCount();

    public void SuppressWindowCloseRequests(bool suppress)
        => _automation.SuppressWindowCloseRequests(suppress);
}


