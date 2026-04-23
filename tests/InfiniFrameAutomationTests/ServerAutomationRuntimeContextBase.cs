// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Js.Interop.MessageHandlers;
using InfiniFrameTests.Shared;
using Microsoft.AspNetCore.Hosting;

namespace InfiniFrameAutomationTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class ServerAutomationRuntimeContextBase : IAutomationRuntimeContext {
    private readonly AutomationSessionManager _automation = new(AutomationPortUtility.GetAvailablePort());
    private readonly int _serverPort = AutomationPortUtility.GetAvailablePort();

    private InfiniFrameServerTestUtility? _utility;

    protected string ServerUrl => $"http://127.0.0.1:{_serverPort}";

    public abstract string DefaultDocumentTitle { get; }

    public IInfiniFrameWindow Window => _utility!.Window;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected void Start() {
        AutomationSessionManager.EnableLinuxWebKitAutomationIfNeeded();

        using var startupCancellation = new CancellationTokenSource(TimeSpan.FromSeconds(90));

        _utility = InfiniFrameServerTestUtility.Create(
            appBuilder: serverBuilder => {
                serverBuilder.WebHost.UseUrls(ServerUrl);
            },
            windowBuilder: windowBuilder => {
                windowBuilder
                    .SetStartUrl(ServerUrl)
                    .SetTitle(DefaultDocumentTitle)
                    .RegisterWindowManagementWebMessageHandler()
                    .RegisterFullScreenWebMessageHandler()
                    .RegisterOpenExternalTargetWebMessageHandler()
                    .RegisterTitleChangedWebMessageHandler()
                    .RegisterWindowClosingHandler((_, _) => _automation.OnWindowClosingRequested());

                if (OperatingSystem.IsWindows()) {
                    windowBuilder.SetBrowserControlInitParameters(_automation.GetWindowsRemoteDebuggingArgs());
                }
            },
            cancellationToken: startupCancellation.Token
        );
    }

    protected void Stop() {
        AutomationSessionManager.DelayIfVisibleDebugEnabled();
        _automation.Dispose();
        _utility?.Dispose();
        _utility = null;
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
