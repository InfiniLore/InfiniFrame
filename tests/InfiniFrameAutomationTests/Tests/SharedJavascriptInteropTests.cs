// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;
using JetBrains.Annotations;

namespace InfiniFrameAutomationTests.Tests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class SharedJavascriptInteropTests : InfiniFrameAutomationTestBase {
    protected abstract string FullscreenToggleButtonSelector { get; }

    protected abstract string TitleToggleButtonSelector { get; }

    private const string ToggledTitle = "New Title";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    [UsedImplicitly]
    public async Task FullscreenHtmlButton_ShouldToggleInfiniFrameFullscreen() {
        bool originalFullscreenState = RuntimeContext.Window.FullScreen;
        IAutomationPage page = await GetRootPageAsync();

        await page.ClickAsync(FullscreenToggleButtonSelector);
        bool newFullscreenState = await WaitForStateChangeAsync(
            originalFullscreenState,
            stateProvider: () => RuntimeContext.Window.FullScreen
        );

        await page.ClickAsync(FullscreenToggleButtonSelector);
        bool finalFullscreenState = await WaitForStateChangeAsync(
            newFullscreenState,
            stateProvider: () => RuntimeContext.Window.FullScreen
        );

        await Assert.That(originalFullscreenState).IsFalse();
        await Assert.That(newFullscreenState).IsTrue();
        await Assert.That(finalFullscreenState).IsFalse();
    }

    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    [UsedImplicitly]
    public async Task TitleHtmlButton_ShouldToggleInfiniFrameTitle() {
        IAutomationPage page = await GetRootPageAsync();
        string originalTitleState = RuntimeContext.Window.Title;

        try {
            await page.ClickAsync(TitleToggleButtonSelector);
            string newTitleState = await WaitForStateChangeAsync(
                originalTitleState,
                stateProvider: () => RuntimeContext.Window.Title
            );

            await page.ClickAsync(TitleToggleButtonSelector);
            string finalTitleState = await WaitForStateChangeAsync(
                newTitleState,
                stateProvider: () => RuntimeContext.Window.Title
            );

            await Assert.That(originalTitleState).IsEqualTo(RuntimeContext.DefaultDocumentTitle);
            await Assert.That(newTitleState).IsEqualTo(ToggledTitle);
            await Assert.That(finalTitleState).IsEqualTo(RuntimeContext.DefaultDocumentTitle);
        }
        finally {
            RuntimeContext.Window.SetTitle(RuntimeContext.DefaultDocumentTitle);
            await page.EvaluateAsync(
                // lang=javascript
                $"() => {{ document.title = '{RuntimeContext.DefaultDocumentTitle}'; }}"
            );
        }
    }
}


