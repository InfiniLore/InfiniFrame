// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame;
using InfiniLore.InfiniFrame.Js.MessageHandlers;
using InfiniLore.InfiniFrame.Server;
using Tests.Shared;

namespace Tests.InfiniFrame.Playwright.Utility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class GlobalPlaywrightContext {
    private static InfiniFrameServerTestUtility Utility { get; set; } = null!;
    public static IInfiniFrameWindow Window => Utility.Window;
    public static InfiniFrameServer Server => Utility.Server;

    private const int ServerPort = 9000;// Cannot be the same as the debug port
    private const string PlaywrightDevtoolsPort = "9222";
    private const string PlaywrightConnectionString = "http://127.0.0.1:" + PlaywrightDevtoolsPort;
    public static readonly Uri PlaywrightConnectionUri = new(PlaywrightConnectionString);

    public const string InfiniFrameWindowTitle = "InfiniFrame Playwright";
    public const string VueDocumentTitle = "InfiniFrame Playwright Vue";

    [Before(Assembly)]
    public static void BeforeAll(AssemblyHookContext _) {
        Utility = InfiniFrameServerTestUtility.Create(
            serverBuilder: static serverBuilder => serverBuilder
                .UsePort(ServerPort),
            windowBuilder: static windowBuilder => windowBuilder
                .SetTitle(InfiniFrameWindowTitle)
                .SetBrowserControlInitParameters($"--remote-debugging-port={PlaywrightDevtoolsPort}")
                .RegisterFullScreenWebMessageHandler()
                .RegisterTitleChangedWebMessageHandler()
        );
    }

    [After(Assembly)]
    public static void AfterAll(AssemblyHookContext _) {
        Utility.Dispose();
    }

}
