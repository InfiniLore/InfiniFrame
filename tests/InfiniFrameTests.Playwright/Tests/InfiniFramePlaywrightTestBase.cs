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

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Before(Test)]
    public async Task ResetStateBeforeEachTest() {
        RuntimeContext.ResetWindowCloseRequestCount();

        IPage page = await GetRootPageAsync();

        RuntimeContext.Window.SetTitle(RuntimeContext.DefaultDocumentTitle);
        await page.EvaluateAsync(
            // lang=javascript
            $"() => {{ document.title = '{RuntimeContext.DefaultDocumentTitle}'; }}"
        );
    }

    protected async Task<IPage> GetPageAsync(string relativeUrl) {
        IBrowserContext context = await GetContextAsync(relativeUrl);
        return context.Pages[0];
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
}
