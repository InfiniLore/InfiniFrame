// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrameAutomationTests.WebApp.React.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class ReactAutomationRuntimeContext : ServerAutomationRuntimeContextBase {
    public static ReactAutomationRuntimeContext Instance { get; } = new();

    public override string DefaultDocumentTitle => "InfiniFrame Playwright React";

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private ReactAutomationRuntimeContext() {}

    [Before(Assembly)]
    public static void BeforeAll(AssemblyHookContext _)
        => Instance.Start();

    [After(Assembly)]
    public static void AfterAll(AssemblyHookContext _)
        => Instance.Stop();
}


