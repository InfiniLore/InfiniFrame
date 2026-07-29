using InfiniAutomationTests.WebApp.TestUtility;

namespace InfiniAutomationTests.WebApp;

public sealed class PlaywrightContext : ServerPlaywrightContextBase {
    private PlaywrightContext() : base("InfiniFrame Playwright Angular") {}
    public static PlaywrightContext Instance { get; } = new();

    [Before(Assembly)]
    public static void BeforeAll(AssemblyHookContext _) => Instance.BeforeAll();

    [After(Assembly)]
    public static async ValueTask AfterAllAsync(AssemblyHookContext _) => await Instance.AfterAllAsync();
}
