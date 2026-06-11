// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniTests;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace InfiniAutomationTests.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class ServerPlaywrightContextBase(string documentTitle) : PlaywrightContextBase(documentTitle) {
    private int _playwrightDevtoolsPort;
    private int _serverPort;

    private InfiniFrameTestServer? _utility;
    public override IInfiniFrameWindow Window => _utility!.Window;
    [UsedImplicitly]
    public WebApplication WebApplication => _utility!.WebApplication;// kept for future reference

    private string ServerUrl => $"http://127.0.0.1:{_serverPort}";
    private string PlaywrightConnectionString => $"http://127.0.0.1:{_playwrightDevtoolsPort}";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected void BeforeAll()
        => StartUtilityWithFreshPorts();

    protected async ValueTask AfterAllAsync() {
        BeforeAssemblyTeardown();

        if (_utility is not null) {
            await _utility.DisposeAsync();
        }

        _utility = null;
    }

    protected override Uri CreatePlaywrightConnectionUri(string relativeUrl)
        => new(new Uri(PlaywrightConnectionString), relativeUrl);

    private void StartUtilityWithFreshPorts() {
        _serverPort = PlaywrightConnectionUtility.GetAvailablePort();
        _playwrightDevtoolsPort = PlaywrightConnectionUtility.GetAvailablePort();

        using var startupCancellation = new CancellationTokenSource(TimeSpan.FromSeconds(90));

        _utility = InfiniFrameTestServer.Create(
            appBuilder: serverBuilder => serverBuilder
                .WebHost.UseUrls(ServerUrl),
            windowBuilder: windowBuilder => {
                if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux()) windowBuilder.Debugging.SetRemoteDebuggingPort(_playwrightDevtoolsPort);
                windowBuilder
                    .SetUrl(ServerUrl)
                    .SetTitle(DefaultDocumentTitle)
                    .RegisterWindowManagementWebMessageHandler()
                    .RegisterFullScreenWebMessageHandler()
                    .RegisterOpenExternalTargetWebMessageHandler()
                    .RegisterTitleChangedWebMessageHandler()
                    .RegisterWindowClosingHandler((_, _) => {
                        bool suppressClose = OnWindowClosingRequested();
                        return suppressClose ? WindowClosingResult.Cancel : WindowClosingResult.Close;
                    });
            },
            startupCancellation.Token
        );
        Console.WriteLine("[PlaywrightSetup] Assembly setup completed.");
    }
}
