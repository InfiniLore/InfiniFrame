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
        IBrowserContext context = await GetContextAsync(relativeUrl);
        IPage page = context.Pages[0];
        await WaitForInfiniFrameReadyAsync(page);
        return page;
    }

    protected Task<IPage> GetRootPageAsync()
        => GetPageAsync(RootRelativeUrl);

    protected async Task<IBrowserContext> GetContextAsync(string relativeUrl) {
        IBrowser browser = await GetBrowserAsync(relativeUrl);
        return browser.Contexts[0];
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
                await page.WaitForTimeoutAsync(150);
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
                await page.WaitForTimeoutAsync(150);
            }
        }
        Fail.Test($"Could not execute script: {script} within timeout");
        return default!;
    }

    protected static Task WaitForInfiniFrameReadyAsync(IPage page)
        => EvaluateWhenPageReadyAsync(
            page,
            // lang=javascript
            """
            async () => {
                if (!window.infiniframe?.messaging?.ready) {
                    throw new Error("InfiniFrame messaging ready promise is not initialized.");
                }

                await window.infiniframe.messaging.ready;
            }
            """
        );

    private static bool IsExecutionContextDestroyedByNavigation(PlaywrightException exception)
        => exception.Message.Contains("Execution context was destroyed", StringComparison.OrdinalIgnoreCase);
}
