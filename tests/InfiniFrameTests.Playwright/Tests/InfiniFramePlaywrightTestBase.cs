// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using Microsoft.Playwright;

namespace InfiniFrameTests.Playwright.Tests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class InfiniFramePlaywrightTestBase {
    protected abstract IPlaywrightRuntimeContext RuntimeContext { get; }

    private const string RootRelativeUrl = "/";
    private const int NavigationRetryCount = 5;
    private const int NavigationRetryDelayMs = 150;
    private const int InfiniFrameReadyTimeoutMs = 20_000;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Before(Test)]
    public async Task ResetStateBeforeEachTest() {
        RuntimeContext.ResetWindowCloseRequestCount();

        IPage page = await GetRootPageAsync();
        RuntimeContext.Window.SetTitle(RuntimeContext.DefaultDocumentTitle);
        await EvaluateWhenPageReadyAsync(
            page,
            // lang=javascript
            $"() => {{ document.title = '{RuntimeContext.DefaultDocumentTitle}'; }}"
        );
    }

    protected async Task<IPage> GetPageAsync(string relativeUrl) {
        for (int attempt = 1; attempt <= NavigationRetryCount; attempt++) {
            IBrowserContext context = await GetContextAsync(relativeUrl);
            IPage? page = context.Pages.FirstOrDefault();

            if (page is null) {
                if (attempt < NavigationRetryCount) {
                    await Task.Delay(NavigationRetryDelayMs);
                    continue;
                }

                Fail.Test($"No page was found in the context for '{relativeUrl}' after {NavigationRetryCount} attempts.");
            }

            try {
                await WaitForInfiniFrameReadyAsync(page!);
                return page!;
            }
            catch (PlaywrightException exception) when (
                attempt < NavigationRetryCount &&
                IsExecutionContextDestroyedByNavigation(exception)
            ) {
                await Task.Delay(NavigationRetryDelayMs);
            }
        }

        Fail.Test($"No ready page was found for '{relativeUrl}' after {NavigationRetryCount} attempts.");
        return null!;
    }

    protected Task<IPage> GetRootPageAsync()
        => GetPageAsync(RootRelativeUrl);

    protected async Task<IBrowserContext> GetContextAsync(string relativeUrl) {
        for (int attempt = 1; attempt <= NavigationRetryCount; attempt++) {
            IBrowser browser = await GetBrowserAsync(relativeUrl);
            IBrowserContext? context = browser.Contexts.FirstOrDefault();

            if (context is null) {
                if (attempt < NavigationRetryCount) {
                    await Task.Delay(NavigationRetryDelayMs);
                    continue;
                }

                Fail.Test($"No context was found for '{relativeUrl}' after {NavigationRetryCount} attempts.");
            }

            IPage? page = context?.Pages.FirstOrDefault();
            if (page is null) {
                if (attempt < NavigationRetryCount) {
                    await Task.Delay(NavigationRetryDelayMs);
                    continue;
                }

                Fail.Test($"No page was found in the context for '{relativeUrl}' after {NavigationRetryCount} attempts.");
            }

            try {
                await WaitForInfiniFrameReadyAsync(page!);
                return context!;
            }
            catch (PlaywrightException exception) when (
                attempt < NavigationRetryCount &&
                IsExecutionContextDestroyedByNavigation(exception)
            ) {
                await Task.Delay(NavigationRetryDelayMs);
            }
        }

        Fail.Test($"No ready context was found for '{relativeUrl}' after {NavigationRetryCount} attempts.");
        return null!;
    }

    protected Task<IBrowser> GetBrowserAsync(string relativeUrl)
        => RuntimeContext.GetOrCreateBrowserAsync(relativeUrl);

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

    private static bool IsExecutionContextDestroyedByNavigation(PlaywrightException exception)
        => exception.Message.Contains("Execution context was destroyed", StringComparison.OrdinalIgnoreCase);
}
