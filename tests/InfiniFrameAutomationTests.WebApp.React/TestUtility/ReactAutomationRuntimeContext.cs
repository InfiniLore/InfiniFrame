// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniFrameAutomationTests.WebApp.React.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class ReactAutomationRuntimeContext : IAutomationRuntimeContext {
    public static ReactAutomationRuntimeContext Instance { get; } = new();
    
    public string DefaultDocumentTitle => GlobalAutomationContext.DefaultDocumentTitle;

    public IInfiniFrameWindow Window => GlobalAutomationContext.Window;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private ReactAutomationRuntimeContext() {}
    
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


