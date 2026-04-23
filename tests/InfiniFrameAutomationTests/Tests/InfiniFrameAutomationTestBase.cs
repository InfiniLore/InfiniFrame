// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using TUnit.Engine.Exceptions;

namespace InfiniFrameAutomationTests.Tests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class InfiniFrameAutomationTestBase {
    protected abstract IAutomationRuntimeContext RuntimeContext { get; }

    private const string RootRelativeUrl = "/";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Before(Test)]
    public async Task ResetStateBeforeEachTest() {
        RuntimeContext.ResetWindowCloseRequestCount();
        RuntimeContext.Window.SetTitle(RuntimeContext.DefaultDocumentTitle);

        IAutomationPage page = await GetRootPageAsync();
        await page.EvaluateAsync(
            // lang=javascript
            $"() => {{ document.title = '{RuntimeContext.DefaultDocumentTitle}'; }}"
        );
    }

    // ReSharper disable once MemberCanBePrivate.Global
    protected Task<IAutomationPage> GetPageAsync(string relativeUrl)
        => RuntimeContext.GetOrCreatePageAsync(relativeUrl);

    protected Task<IAutomationPage> GetRootPageAsync()
        => GetPageAsync(RootRelativeUrl);

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
        throw new TestFailedException("State change timeout exceeded", null);
    }

    // ReSharper disable once UnusedMember.Global
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
        throw new TestFailedException("State change timeout exceeded", null);
    }
}


