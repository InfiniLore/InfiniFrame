// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniTests;
using TUnit.Core.Executors;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[assembly: DefaultInfiniTestsTimeout]
[assembly: Retry(5)]
[assembly: TestExecutor<MacOsWindowExecutor>]

namespace InfiniTests.InfiniFrame;
public static class TestSettings {
    [Before(Assembly)]
    public static void BeforeAssembly(AssemblyHookContext context) {
        MacOsWindowExecutor.CaptureMainThread(context);
    }
}