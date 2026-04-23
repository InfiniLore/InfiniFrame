// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using Microsoft.Playwright;

namespace InfiniFrameTests.Playwright.WebApp.Vue.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class VuePlaywrightRuntimeContext : IPlaywrightRuntimeContext {
    public static VuePlaywrightRuntimeContext Instance { get; } = new();
    
    public string DefaultDocumentTitle => GlobalPlaywrightContext.DefaultDocumentTitle;

    public IInfiniFrameWindow Window => GlobalPlaywrightContext.Window;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private VuePlaywrightRuntimeContext() {}
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public Task<IBrowser> GetOrCreateBrowserAsync(string relativeUrl = "/")
        => GlobalPlaywrightContext.GetOrCreateBrowserAsync(relativeUrl);

    public void ResetWindowCloseRequestCount()
        => GlobalPlaywrightContext.ResetWindowCloseRequestCount();

    public int GetWindowCloseRequestCount()
        => GlobalPlaywrightContext.GetWindowCloseRequestCount();

    public void SuppressWindowCloseRequests(bool suppress)
        => GlobalPlaywrightContext.SuppressWindowCloseRequests(suppress);
}
