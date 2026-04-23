// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniFrameAutomationTests.WebApp.Vue.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class VueAutomationRuntimeContext : IAutomationRuntimeContext {
    public static VueAutomationRuntimeContext Instance { get; } = new();
    
    public string DefaultDocumentTitle => GlobalAutomationContext.DefaultDocumentTitle;

    public IInfiniFrameWindow Window => GlobalAutomationContext.Window;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private VueAutomationRuntimeContext() {}
    
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


