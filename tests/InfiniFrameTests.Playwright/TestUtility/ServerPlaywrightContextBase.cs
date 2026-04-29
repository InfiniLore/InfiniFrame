// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Js.Interop.MessageHandlers;
using InfiniFrameTests.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace InfiniFrameTests.Playwright.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class ServerPlaywrightContextBase(string documentTitle) : PlaywrightContextBase(documentTitle) {
    public override IInfiniFrameWindow Window => _utility!.Window;
    [UsedImplicitly] public WebApplication WebApplication => _utility!.WebApplication; // kept for future reference
    
    private InfiniFrameServerTestUtility? _utility;
    private int _serverPort;
    private int _playwrightDevtoolsPort;
    private string? _webViewUserDataPath;
    
    private string ServerUrl => $"http://127.0.0.1:{_serverPort}";
    private string PlaywrightConnectionString => $"http://127.0.0.1:{_playwrightDevtoolsPort}";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected void BeforeAll()
        => StartUtilityWithFreshPorts();

    protected void AfterAll() {
        BeforeAssemblyTeardown();

        _utility?.Dispose();
        _utility = null;

        PlaywrightConnectionUtility.DeleteDirectorySafely(_webViewUserDataPath);
        _webViewUserDataPath = null;
    }

    protected override Uri CreatePlaywrightConnectionUri(string relativeUrl)
        => new(new Uri(PlaywrightConnectionString), relativeUrl);

    private void StartUtilityWithFreshPorts() {
        _serverPort = PlaywrightConnectionUtility.GetAvailablePort();
        _playwrightDevtoolsPort = PlaywrightConnectionUtility.GetAvailablePort();
        _webViewUserDataPath = PlaywrightConnectionUtility.CreateUniqueWebViewUserDataPath(GetType().FullName ?? GetType().Name);
        
        using var startupCancellation = new CancellationTokenSource(TimeSpan.FromSeconds(90));

        _utility = InfiniFrameServerTestUtility.Create(
            appBuilder: serverBuilder => serverBuilder
                .WebHost.UseUrls(ServerUrl),
            windowBuilder: windowBuilder => windowBuilder
                .SetStartUrl(ServerUrl)
                .SetTitle(DefaultDocumentTitle)
                .SetTemporaryFilesPath(_webViewUserDataPath)
                .SetBrowserControlInitParameters($"--remote-debugging-port={_playwrightDevtoolsPort}")
                .RegisterWindowManagementWebMessageHandler()
                .RegisterFullScreenWebMessageHandler()
                .RegisterOpenExternalTargetWebMessageHandler()
                .RegisterTitleChangedWebMessageHandler()
                .RegisterWindowClosingHandler((_, _) => OnWindowClosingRequested()),
            cancellationToken: startupCancellation.Token
        );
        Console.WriteLine("[PlaywrightSetup] Assembly setup completed.");
    }
}
