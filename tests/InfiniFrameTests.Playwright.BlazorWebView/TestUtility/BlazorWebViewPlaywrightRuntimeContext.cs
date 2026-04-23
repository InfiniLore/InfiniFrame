// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using Microsoft.Playwright;

namespace InfiniFrameTests.Playwright.BlazorWebView.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class BlazorWebViewPlaywrightRuntimeContext : IPlaywrightRuntimeContext {
    public static BlazorWebViewPlaywrightRuntimeContext Instance { get; } = new();
    
    public string DefaultDocumentTitle => GlobalPlaywrightContext.DefaultDocumentTitle;

    public IInfiniFrameWindow Window => GlobalPlaywrightContext.Window;
    
    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private BlazorWebViewPlaywrightRuntimeContext() {}
    
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
