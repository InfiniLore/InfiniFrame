// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using Microsoft.Playwright;

namespace InfiniAutomationTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IPlaywrightRuntimeContext {
    string DefaultDocumentTitle { get; }
    IInfiniFrameWindow Window { get; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    Task<IBrowser> GetOrCreateBrowserAsync(string relativeUrl = "/");

    Task RestoreDefaultStateAsync();

    void ResetWindowCloseRequestCount();

    int GetWindowCloseRequestCount();

    void SuppressWindowCloseRequests(bool suppress);
}
