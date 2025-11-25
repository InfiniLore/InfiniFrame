// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Js.MessageHandlers;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.Playwright.Utility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class GlobalPlaywrightContext {
    private static InfiniFrameServerTestUtility Utility { get; set; } = null!;
    public static IInfiniFrameWindow Window => Utility.Window;
    public static WebApplication WebApplication => Utility.WebApplication;
    
    #if NET8_0
    private const string ServerPort = "9000";// Cannot be the same as the debug port
    private const string PlaywrightDevtoolsPort = "9222";
    #elif NET9_0
    private const string ServerPort = "9010";// Cannot be the same as the debug port
    private const string PlaywrightDevtoolsPort = "9232";
    #elif NET10_0
    private const string ServerPort = "9020";// Cannot be the same as the debug port
    private const string PlaywrightDevtoolsPort = "9242";
    #endif

    private const string ServerUrl = "http://127.0.0.1:" + ServerPort;
    private const string PlaywrightConnectionString = "http://127.0.0.1:" + PlaywrightDevtoolsPort;
    public static readonly Uri PlaywrightConnectionUri = new(PlaywrightConnectionString);

    public const string InfiniFrameWindowTitle = "InfiniFrame Playwright";
    public const string VueDocumentTitle = "InfiniFrame Playwright Vue";

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
    }

    [After(Assembly)]
    public static void AfterAll(AssemblyHookContext _) {
        Utility.Dispose();
    }

}
