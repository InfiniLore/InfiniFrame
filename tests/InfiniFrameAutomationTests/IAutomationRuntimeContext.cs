// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniFrameAutomationTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IAutomationRuntimeContext {
    string DefaultDocumentTitle { get; }

    IInfiniFrameWindow Window { get; }

    Task<IAutomationPage> GetOrCreatePageAsync(string relativeUrl = "/");

    void ResetWindowCloseRequestCount();

    int GetWindowCloseRequestCount();

    void SuppressWindowCloseRequests(bool suppress);
}


