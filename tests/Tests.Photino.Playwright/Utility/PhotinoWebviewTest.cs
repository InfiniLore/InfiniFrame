// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Playwright;
using TUnit.Playwright;

namespace Tests.Photino.Playwright.Utility;
using TUnit.Engine.Exceptions;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class PhotinoWebviewTest : PageTest {
    public override string BrowserName => "webkit";

    /// <summary>
    /// Asynchronously retrieves a page object for the specified relative URL.
    /// Relative to the root of the Photino application.
    /// </summary>
    /// <param name="relativeUrl">The relative URL of the page to retrieve.</param>
    /// <returns>An asynchronously resolved task containing the page object for the specified URL.</returns>
    protected static async Task<IPage> GetPageAsync(string relativeUrl) {
        var url = new Uri(GlobalPlaywrightContext.PlaywrightConnectionUri, relativeUrl);
        
        IBrowser browser = await Playwright.Chromium.ConnectOverCDPAsync(url.ToString());
        IBrowserContext context = browser.Contexts[0];
        IPage page = context.Pages[0];
        
        return page;
    }

    /// <summary>
    /// Retrieves the root page of the web application that corresponds to the root URL ("/") using the appropriate Playwright behavior.
    /// </summary>
    /// <returns>Task containing an instance of an IPage object representing the root page.</returns>
    protected static async Task<IPage> GetRootPageAsync() {
        return await GetPageAsync("/");
    }

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
