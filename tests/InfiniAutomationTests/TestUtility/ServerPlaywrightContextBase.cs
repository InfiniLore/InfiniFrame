// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniTests;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Playwright;

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
        => new(PlaywrightConnectionUtility.CreateCdpConnectionUrl(_playwrightDevtoolsPort), relativeUrl);

    public override async Task RestoreDefaultStateAsync() {
        Window.Features.State.SetFullScreen(false);
        Window.Features.Decorations.SetTitle(DefaultDocumentTitle);

        IBrowser browser = await GetOrCreateBrowserAsync();
        IPage? page = browser.Contexts.SelectMany(context => context.Pages).FirstOrDefault();
        if (page is null) return;

        await page.EvaluateAsync(
            """
            async title => {
                if (document.fullscreenElement && document.exitFullscreen) await document.exitFullscreen();
                document.title = title;
                window.dispatchEvent(new Event('infiniframe:test-reset'));
                await new Promise(resolve => requestAnimationFrame(() => requestAnimationFrame(resolve)));
            }
            """,
            DefaultDocumentTitle
        );
    }

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
                    .SetIconFile("favicon.ico")
                    .SetStartPageUrl(ServerUrl)
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
