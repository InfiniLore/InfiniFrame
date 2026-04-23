// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniFrameAutomationTests.BlazorWebView.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class BlazorWebViewAutomationRuntimeContext : IAutomationRuntimeContext {
    public static BlazorWebViewAutomationRuntimeContext Instance { get; } = new();
    
    public string DefaultDocumentTitle => GlobalAutomationContext.DefaultDocumentTitle;

    public IInfiniFrameWindow Window => GlobalAutomationContext.Window;
    
    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private BlazorWebViewAutomationRuntimeContext() {}
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public Task<IAutomationPage> GetOrCreatePageAsync(string relativeUrl = "/")
        => GlobalAutomationContext.GetOrCreatePageAsync(relativeUrl);

    public void ResetWindowCloseRequestCount()
        => GlobalAutomationContext.ResetWindowCloseRequestCount();

    public int GetWindowCloseRequestCount()
        => GlobalAutomationContext.GetWindowCloseRequestCount();

    public void SuppressWindowCloseRequests(bool suppress)
        => GlobalAutomationContext.SuppressWindowCloseRequests(suppress);
}


