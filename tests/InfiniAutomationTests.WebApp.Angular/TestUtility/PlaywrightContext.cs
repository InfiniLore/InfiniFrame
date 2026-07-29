// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniAutomationTests.WebApp.TestUtility;

// ReSharper disable once CheckNamespace
namespace InfiniAutomationTests.WebApp;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class PlaywrightContext : ServerPlaywrightContextBase {
    
    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private PlaywrightContext() : base("InfiniFrame Playwright Angular") {}
    public static PlaywrightContext Instance { get; } = new();

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
