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

    protected async Task<IPage> GetPageAsync(string relativeUrl) {
        var url = new Uri(GlobalPlaywrightContext.PlaywrightConnectionUri, relativeUrl);
        
        IBrowser browser = await Playwright.Chromium.ConnectOverCDPAsync(url.ToString());
        IBrowserContext context = browser.Contexts[0];
        IPage page = context.Pages[0];
        
        return page;
    }
    
    protected async Task<IPage> GetRootPageAsync() {
        return await GetPageAsync("/");
    }
    
    protected static async Task<T> WaitForStateChangeAsync<T>(Func<T> stateProvider, T initialValue, TimeSpan timeout = default, TimeSpan interval = default) {
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
    
    protected static async Task<T> WaitForStateChangeAsync<T>(Func<Task<T>> stateProvider, T initialValue, TimeSpan timeout = default, TimeSpan interval = default) {
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
