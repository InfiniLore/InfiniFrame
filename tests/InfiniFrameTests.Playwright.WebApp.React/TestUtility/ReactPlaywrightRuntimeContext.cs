// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using Microsoft.Playwright;

namespace InfiniFrameTests.Playwright.WebApp.React.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class ReactPlaywrightRuntimeContext : IPlaywrightRuntimeContext {
    public static ReactPlaywrightRuntimeContext Instance { get; } = new();
    
    public string DefaultDocumentTitle => GlobalPlaywrightContext.DefaultDocumentTitle;

    public IInfiniFrameWindow Window => GlobalPlaywrightContext.Window;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private ReactPlaywrightRuntimeContext() {}
    
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
