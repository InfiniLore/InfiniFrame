// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Photino.NET;
using InfiniLore.Photino.NET.Server;
using Tests.Shared.Photino;

namespace Tests.Photino.Playwright.Utility;
using InfiniLore.Photino.Js.MessageHandlers;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class GlobalPlaywrightContext {
    private static WindowServerTestUtility Utility { get; set; } = null!;
    public static IPhotinoWindow Window => Utility.Window;
    public static PhotinoServer Server => Utility.Server;

    private const int ServerPort = 9000; // Cannot be the same as the debug port
    private const string PlaywrightDevtoolsPort = "9222";
    private const string PlaywrightConnectionString = "http://127.0.0.1:" + PlaywrightDevtoolsPort;
    public static readonly Uri PlaywrightConnectionUri = new Uri(PlaywrightConnectionString);
    
    [Before(Assembly)]
    public static void BeforeAll(AssemblyHookContext _) {
        Utility = WindowServerTestUtility.Create(
            static serverBuilder => serverBuilder
                .UsePort(ServerPort),
            
            static windowBuilder => windowBuilder
                .SetBrowserControlInitParameters($"--remote-debugging-port={PlaywrightDevtoolsPort}")
            
                .RegisterFullScreenWebMessageHandler()
        );
    }
    
    [After(Assembly)]
    public static void AfterAll(AssemblyHookContext _) {
        Utility.Dispose();
    }
    
}
