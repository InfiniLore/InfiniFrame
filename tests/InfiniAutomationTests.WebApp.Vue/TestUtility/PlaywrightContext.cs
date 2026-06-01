// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniAutomationTests.TestUtility;

namespace InfiniAutomationTests.WebApp.Vue.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class PlaywrightContext : ServerPlaywrightContextBase {
    public static PlaywrightContext Instance { get; } = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private PlaywrightContext() : base("InfiniFrame Playwright Vue") {}

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Before(Assembly)]
    public static void BeforeAll(AssemblyHookContext _)
        => Instance.BeforeAll();

    [After(Assembly)]
    public static async ValueTask AfterAllAsync(AssemblyHookContext _)
        => await Instance.AfterAllAsync();
}
