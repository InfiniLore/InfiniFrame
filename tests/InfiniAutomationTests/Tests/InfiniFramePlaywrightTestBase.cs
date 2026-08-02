// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Playwright;

namespace InfiniAutomationTests.Tests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class InfiniFramePlaywrightTestBase {

    private const string RootRelativeUrl = "/";
    private const int NavigationRetryCount = 5;
    private const int NavigationRetryDelayMs = 150;
    private const int InfiniFrameReadyTimeoutMs = 20_000;
    private const int BrowserContextReadyTimeoutMs = 20_000;
    private const int BrowserContextReadyPollDelayMs = 100;
    protected abstract IPlaywrightRuntimeContext RuntimeContext { get; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Before(Test)]
    public async Task ResetStateBeforeEachTest() {
        RuntimeContext.ResetWindowCloseRequestCount();

        IPage page = await GetRootPageAsync();
        await EvaluateWhenPageReadyAsync(
            page,
            // lang=javascript
            $"() => {{ document.title = '{RuntimeContext.DefaultDocumentTitle}'; }}"
        );
    }

    [After(Test)]
    public Task ResetStateAfterEachTest()
        => RuntimeContext.RestoreDefaultStateAsync();

    protected async Task<IPage> GetPageAsync(string relativeUrl) {
        IBrowserContext context = await GetContextAsync(relativeUrl);
        IPage page = await WaitForPageAsync(context);
        await WaitForInfiniFrameReadyAsync(page);
        return page;
    }

    protected Task<IPage> GetRootPageAsync()
        => GetPageAsync(RootRelativeUrl);

    protected async Task<IBrowserContext> GetContextAsync(string relativeUrl) {
        IBrowser browser = await GetBrowserAsync(relativeUrl);
        return await WaitForContextAsync(browser);
    }

    protected Task<IBrowserContext> GetRootContextAsync()
        => GetContextAsync(RootRelativeUrl);

    protected Task<IBrowser> GetBrowserAsync(string relativeUrl)
        => RuntimeContext.GetOrCreateBrowserAsync(relativeUrl);

    protected Task<IBrowser> GetRootBrowserAsync()
        => GetBrowserAsync(RootRelativeUrl);

    protected static async Task<T> WaitForStateChangeAsync<T>(
        T initialValue,
        Func<T> stateProvider,
        TimeSpan timeout = default,
        TimeSpan interval = default
    ) {
        if (timeout == TimeSpan.Zero) timeout = TimeSpan.FromSeconds(20);
        if (interval == TimeSpan.Zero) interval = TimeSpan.FromMilliseconds(100);

        DateTime expectedEnd = DateTime.UtcNow.Add(timeout);

        while (DateTime.UtcNow < expectedEnd) {
            T state = stateProvider();
            if (!Equals(state, initialValue)) return state;

            await Task.Delay(interval);
        }

        Fail.Test("State change timeout exceeded");
        return default!;
    }

    protected static async Task<T> WaitForStateChangeAsync<T>(
        T initialValue,
        Func<Task<T>> stateProvider,
        TimeSpan timeout = default,
        TimeSpan interval = default
    ) {
        if (timeout == TimeSpan.Zero) timeout = TimeSpan.FromSeconds(20);
        if (interval == TimeSpan.Zero) interval = TimeSpan.FromMilliseconds(100);

        DateTime expectedEnd = DateTime.UtcNow.Add(timeout);

        while (DateTime.UtcNow < expectedEnd) {
            T state = await stateProvider();
            if (!Equals(state, initialValue)) return state;

            await Task.Delay(interval);
        }

        Fail.Test("State change timeout exceeded");
        return default!;
    }

    protected static async Task EvaluateWhenPageReadyAsync(IPage page, string script) {
        for (int attempt = 1; attempt <= NavigationRetryCount; attempt++) {
            try {
                await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                await page.EvaluateAsync(script);
                return;
            }
            catch (PlaywrightException exception) when (
                attempt < NavigationRetryCount &&
                IsExecutionContextDestroyedByNavigation(exception)
            ) {
                await page.WaitForTimeoutAsync(NavigationRetryDelayMs);
            }
        }

        Fail.Test($"Could not execute script: {script} within timeout");
    }

    protected static async Task<T> EvaluateWhenPageReadyAsync<T>(IPage page, string script) {
        for (int attempt = 1; attempt <= NavigationRetryCount; attempt++) {
            try {
                await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                var result = await page.EvaluateAsync<T>(script);
                return result;
            }
            catch (PlaywrightException exception) when (
                attempt < NavigationRetryCount &&
                IsExecutionContextDestroyedByNavigation(exception)
            ) {
                await page.WaitForTimeoutAsync(NavigationRetryDelayMs);
            }
        }

        Fail.Test($"Could not execute script: {script} within timeout");
        return default!;
    }

    protected static async Task WaitForInfiniFrameReadyAsync(IPage page) {
        for (int attempt = 1; attempt <= NavigationRetryCount; attempt++) {
            try {
                await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                await page.WaitForFunctionAsync(
                    // lang=javascript
                    """
                    () => window.infiniframe?.messaging?.isReady === true
                    """,
                    new PageWaitForFunctionOptions {
                        Timeout = InfiniFrameReadyTimeoutMs
                    }
                );
                return;
            }
            catch (PlaywrightException exception) when (
                attempt < NavigationRetryCount &&
                IsExecutionContextDestroyedByNavigation(exception)
            ) {
                await page.WaitForTimeoutAsync(NavigationRetryDelayMs);
            }
        }

        Fail.Test("InfiniFrame JavaScript interop readiness was not acknowledged.");
    }

    private static async Task<IBrowserContext> WaitForContextAsync(IBrowser browser) {
        DateTime timeoutAt = DateTime.UtcNow.AddMilliseconds(BrowserContextReadyTimeoutMs);

        while (DateTime.UtcNow < timeoutAt) {
            IBrowserContext? context = browser.Contexts.FirstOrDefault();
            if (context is not null) {
                return context;
            }

            await Task.Delay(BrowserContextReadyPollDelayMs);
        }

        Fail.Test("Timed out waiting for browser context.");
        return null!;
    }

    private static async Task<IPage> WaitForPageAsync(IBrowserContext context) {
        DateTime timeoutAt = DateTime.UtcNow.AddMilliseconds(BrowserContextReadyTimeoutMs);

        while (DateTime.UtcNow < timeoutAt) {
            IPage? page = context.Pages.FirstOrDefault();
            if (page is not null) {
                return page;
            }

            await Task.Delay(BrowserContextReadyPollDelayMs);
        }

        Fail.Test("Timed out waiting for browser page.");
        return null!;
    }

    private static bool IsExecutionContextDestroyedByNavigation(PlaywrightException exception)
        => exception.Message.Contains("Execution context was destroyed", StringComparison.OrdinalIgnoreCase);
}