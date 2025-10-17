// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Playwright;
using TUnit.Playwright;

namespace Tests.InfiniFrame.Playwright.Utility;
using TUnit.Engine.Exceptions;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class InfiniFrameWebviewTest : PageTest {
    public override string BrowserName => "webkit";

    /// <summary>
    /// Asynchronously retrieves a page object for the specified relative URL.
    /// Relative to the root of the InfiniFrame application.
    /// </summary>
    /// <param name="relativeUrl">The relative URL of the page to retrieve.</param>
    /// <returns>An asynchronously resolved task containing the page object for the specified URL.</returns>
    protected static async Task<IPage> GetPageAsync(string relativeUrl) {
        IBrowserContext context = await GetContextAsync(relativeUrl);
        return context.Pages[0];
    }

    /// <summary>
    /// Retrieves the root page of the web application that corresponds to the root URL ("/") using the appropriate Playwright behavior.
    /// </summary>
    /// <returns>Task containing an instance of an IPage object representing the root page.</returns>
    protected static async Task<IPage> GetRootPageAsync()
        => await GetPageAsync("/");

    /// <summary>
    /// Asynchronously retrieves a browser context object for the specified relative URL.
    /// Relative to the root of the InfiniFrame application.
    /// </summary>
    /// <param name="relativeUrl">The relative URL of the browser context to retrieve.</param>
    /// <returns>An asynchronously resolved task containing the browser context object for the specified URL.</returns>
    protected static async Task<IBrowserContext> GetContextAsync(string relativeUrl) {
        IBrowser browser = await GetBrowserAsync(relativeUrl);
        return browser.Contexts[0];
    }

    /// <summary>
    /// Asynchronously retrieves the root browser context for the web application corresponding to the root URL ("/").
    /// </summary>
    /// <returns>An asynchronously resolved task containing the root browser context instance.</returns>
    protected static async Task<IBrowserContext> GetRootContextAsync()
        => await GetContextAsync("/");

    /// <summary>
    /// Asynchronously retrieves a browser instance connected to the specified relative URL.
    /// </summary>
    /// <param name="relativeUrl">The relative URL to connect the browser to.</param>
    /// <returns>A task that resolves to an <see cref="IBrowser"/> instance representing the connected browser.</returns>
    protected static async Task<IBrowser> GetBrowserAsync(string relativeUrl) {
        var url = new Uri(GlobalPlaywrightContext.PlaywrightConnectionUri, relativeUrl);
        return await Playwright.Chromium.ConnectOverCDPAsync(url.ToString());
    }

    /// <summary>
    /// Retrieves the root browser instance associated with the root URL ("/") of the application.
    /// </summary>
    /// <returns>Task containing an instance of the IBrowser object representing the root browser.</returns>
    protected static async Task<IBrowser> GetRootBrowserAsync()
        => await GetBrowserAsync("/");

    /// <summary>
    /// Waits for a state change by repeatedly invoking the specified state provider function until the returned state
    /// differs from the initial value or the timeout period elapses. The function retries at specified intervals.
    /// Will fail the test if the timeout is exceeded.
    /// </summary>
    /// <typeparam name="T">The type of the state being monitored.</typeparam>
    /// <param name="stateProvider">A function that provides the current state. This may be a synchronous or asynchronous function.</param>
    /// <param name="initialValue">The initial value of the state to compare against.</param>
    /// <param name="timeout">The maximum amount of time to wait for the state to change. Defaults to 5 seconds if not specified.</param>
    /// <param name="interval">The interval at which to check for state changes. Defaults to 100 milliseconds if not specified.</param>
    /// <returns>The new state once it changes from the initial value or throws an exception if the timeout is exceeded.</returns>
    /// <exception cref="TUnit.Engine.Exceptions.TestFailedException">Thrown when the timeout for waiting for the state change is exceeded.</exception>
    protected static async Task<T> WaitForStateChangeAsync<T>(T initialValue, Func<T> stateProvider, TimeSpan timeout = default, TimeSpan interval = default) {
        if (timeout == TimeSpan.Zero) timeout = TimeSpan.FromSeconds(5);
        if (interval == TimeSpan.Zero) interval = TimeSpan.FromMilliseconds(100);

        DateTime expectedEnd = DateTime.UtcNow.Add(timeout);

        while (DateTime.UtcNow < expectedEnd) {
            T state = stateProvider();
            if (!Equals(state, initialValue)) return state;

            await Task.Delay(interval);
        }

        Fail.Test("State change timeout exceeded");
        throw new TestFailedException("State change timeout exceeded", null);
    }

    /// <inheritdoc cref="WaitForStateChangeAsync{T}(T, Func{T}, TimeSpan, TimeSpan)"/>
    protected static async Task<T> WaitForStateChangeAsync<T>(T initialValue, Func<Task<T>> stateProvider, TimeSpan timeout = default, TimeSpan interval = default) {
        if (timeout == TimeSpan.Zero) timeout = TimeSpan.FromSeconds(5);
        if (interval == TimeSpan.Zero) interval = TimeSpan.FromMilliseconds(100);

        DateTime expectedEnd = DateTime.UtcNow.Add(timeout);

        while (DateTime.UtcNow < expectedEnd) {
            T state = await stateProvider();
            if (!Equals(state, initialValue)) return state;

            await Task.Delay(interval);
        }

        Fail.Test("State change timeout exceeded");
        throw new TestFailedException("State change timeout exceeded", null);
    }
}
