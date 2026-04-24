// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using Microsoft.Playwright;

namespace InfiniFrameTests.Playwright;
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

    void ResetWindowCloseRequestCount();

    int GetWindowCloseRequestCount();

    void SuppressWindowCloseRequests(bool suppress);
}
