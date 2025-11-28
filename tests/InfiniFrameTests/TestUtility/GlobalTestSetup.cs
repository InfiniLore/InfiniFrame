// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class GlobalTestSetup {
    [Before(Assembly)]
    public static void BeforeAll(AssemblyHookContext __) {
        // We need a parent window for all tests to be started in a correct manner
        var windowBuilder = InfiniFrameWindowBuilder.Create();

        windowBuilder.SetStartString("""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="UTF-8">
            </head>
            <body>
            </body>
            </html>
            """);
            
        IInfiniFrameWindow window = windowBuilder.Build();
        _ = Task.Run(window.WaitForClose);
        
        InfiniFrameWindowTestUtility.DefineParentWindow(window);
    }
    
    [After(Assembly)]
    public static void AfterAll(AssemblyHookContext _) {
        foreach (InfiniFrameWindowTestUtility utility in InfiniFrameWindowTestUtility.Utilities) {
            utility.Cleanup();
        }
    }
}
