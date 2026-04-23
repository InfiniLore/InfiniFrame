// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrameAutomationTests.WebApp.Vue.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class VueAutomationRuntimeContext : ServerAutomationRuntimeContextBase {
    public static VueAutomationRuntimeContext Instance { get; } = new();

    public override string DefaultDocumentTitle => "InfiniFrame Playwright Vue";

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private VueAutomationRuntimeContext() {}

    [Before(Assembly)]
    public static void BeforeAll(AssemblyHookContext _)
        => Instance.Start();

    [After(Assembly)]
    public static void AfterAll(AssemblyHookContext _)
        => Instance.Stop();
}


