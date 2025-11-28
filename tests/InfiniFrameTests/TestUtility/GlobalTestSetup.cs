// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class GlobalTestSetup {
    [After(Assembly)]
    public static void AfterAll(AssemblyHookContext _) {
        foreach (InfiniFrameWindowTestUtility utility in InfiniFrameWindowTestUtility.Utilities) {
            utility.Cleanup();
        }
    }
}
