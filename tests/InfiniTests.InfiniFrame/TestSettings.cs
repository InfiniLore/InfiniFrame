// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniTests;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[assembly: DefaultInfiniTestsTimeout]
[assembly: Retry(5)]

namespace InfiniTests.InfiniFrame;
public static class TestSettings {
    [Before(Assembly)]
    public static void BeforeAssembly(AssemblyHookContext context) {
        MacOsWindowExecutor.CaptureMainThread(context);
    }
}