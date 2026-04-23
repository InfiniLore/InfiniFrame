// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using OpenQA.Selenium;
using System.Text.Json;

namespace InfiniFrameAutomationTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class SeleniumAutomationPage(IWebDriver driver) : IAutomationPage {
    public Task ClickAsync(string selector) {
        IWebElement element = driver.FindElement(By.CssSelector(selector));
        element.Click();
        return Task.CompletedTask;
    }

    public Task<string> TitleAsync()
        => Task.FromResult(driver.Title);

    public Task EvaluateAsync(string script) {
        ExecuteWrapped(script, returnValue: false);
        return Task.CompletedTask;
    }

    public Task<T> EvaluateAsync<T>(string script) {
        object? value = ExecuteWrapped(script, returnValue: true);
        if (value is null) {
            return Task.FromResult(default(T)!);
        }

        string json = JsonSerializer.Serialize(value);
        var typed = JsonSerializer.Deserialize<T>(json);
        return Task.FromResult(typed!);
    }

    private object? ExecuteWrapped(string script, bool returnValue) {
        if (driver is not IJavaScriptExecutor jsExecutor)
            throw new InvalidOperationException("Selenium driver does not support JavaScript execution.");

        string wrapped = returnValue
            ? $"return (() => {{ const __fn = {script}; return __fn(); }})();"
            : $"(() => {{ const __fn = {script}; __fn(); }})();";

        return jsExecutor.ExecuteScript(wrapped);
    }
}


