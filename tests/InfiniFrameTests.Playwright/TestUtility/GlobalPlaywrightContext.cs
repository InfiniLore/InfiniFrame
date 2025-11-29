
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Js.MessageHandlers;
using InfiniFrameTests.Shared;
using System.Net;
using System.Net.Sockets;

namespace InfiniFrameTests.Playwright.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class GlobalPlaywrightContext {
    private static InfiniFrameServerTestUtility Utility { get; set; } = null!;
    public static IInfiniFrameWindow Window => Utility.Window;
    public static WebApplication WebApplication => Utility.WebApplication;
    
    private static readonly int ServerPort = GetAvailablePort();
    private static readonly int PlaywrightDevtoolsPort = GetAvailablePort();

    private static readonly string ServerUrl = $"http://127.0.0.1:{ServerPort}";
    private static readonly string PlaywrightConnectionString = $"http://127.0.0.1:{PlaywrightDevtoolsPort}";
    public static readonly Uri PlaywrightConnectionUri = new(PlaywrightConnectionString);

    public const string InfiniFrameWindowTitle = "InfiniFrame Playwright";
    public const string VueDocumentTitle = "InfiniFrame Playwright Vue";

    private static int GetAvailablePort() {
        using TcpListener listener = new(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    [Before(Assembly)]
    public static void BeforeAll(AssemblyHookContext _) {
        Utility = InfiniFrameServerTestUtility.Create(
            appBuilder: static serverBuilder => serverBuilder
                .WebHost.UseUrls(ServerUrl),
            windowBuilder: static windowBuilder => windowBuilder
                .SetStartUrl(ServerUrl)
                .SetTitle(InfiniFrameWindowTitle)
                .SetBrowserControlInitParameters($"--remote-debugging-port={PlaywrightDevtoolsPort}")
                .RegisterFullScreenWebMessageHandler()
                .RegisterOpenExternalTargetWebMessageHandler()
                .RegisterTitleChangedWebMessageHandler()
        );
        
        Thread.Sleep(TimeSpan.FromSeconds(5));
    }

    [After(Assembly)]
    public static void AfterAll(AssemblyHookContext _) {
        Utility.Dispose();
    }
}