// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace Tests.Photino.Playwright;
using InfiniLore.Photino.NET;
using InfiniLore.Photino.NET.Server;
using Tests.Shared.Photino;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class GlobalPlaywright {
    private static WindowServerTestUtility Utility { get; set; } = null!;
    
    public const int ServerPort = 9000; // Cannot be the same as the debug port
    public const string PlaywrightDevtoolsPort = "9222";
    public const string PlaywrightConnectionString = "http://127.0.0.1:" + PlaywrightDevtoolsPort;
    
    [Before(Assembly)]
    public static void BeforeAll(AssemblyHookContext context) {
        Utility = WindowServerTestUtility.Create(
            serverBuilder => serverBuilder
                .UsePort(ServerPort),
            
            windowBuilder => windowBuilder
                .SetBrowserControlInitParameters($"--remote-debugging-port={PlaywrightDevtoolsPort}")
        );
    }
    
    [After(Assembly)]
    public static void AfterAll(AssemblyHookContext context) {
        Utility.Dispose();
    }
    
}
