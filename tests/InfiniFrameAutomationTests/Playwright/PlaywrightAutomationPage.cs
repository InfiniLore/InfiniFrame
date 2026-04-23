// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Playwright;

namespace InfiniFrameAutomationTests.Playwright;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class PlaywrightAutomationPage(IPage page) : IAutomationPage {
    public Task ClickAsync(string selector)
        => page.ClickAsync(selector);

    public Task<string> TitleAsync()
        => page.TitleAsync();

    public Task EvaluateAsync(string script)
        => page.EvaluateAsync(script);

    public Task<T> EvaluateAsync<T>(string script)
        => page.EvaluateAsync<T>(script);
}