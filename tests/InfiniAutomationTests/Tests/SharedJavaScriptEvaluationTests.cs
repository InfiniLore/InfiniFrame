// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniTests;
using Microsoft.Playwright;

namespace InfiniAutomationTests.Tests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class SharedJavaScriptEvaluationTests : InfiniFramePlaywrightTestBase {

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task ExecuteJavaScriptAsync_FromJs_ShouldEvaluateExpression(CancellationToken ct = default) {
        // Arrange
        IPage page = await GetRootPageAsync();

        // Act - Use JS to call the eval feature via the window.infiniframe.window.features.javaScript.evalAsync
        // Note: This tests the JS→C#→JS round-trip through the feature
        string? result = await EvaluateWhenPageReadyAsync<string?>(
            page,
            // lang=javascript
            "async () => { try { return await window.infiniframe.window.features.javaScript.evalAsync('1 + 1'); } catch(e) { return 'error: ' + e.message; } }"
        );

        // Assert - The result should be the JSON-encoded result of evaluating "1 + 1"
        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsNotEqualTo("error: undefined");
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task ExecuteJavaScriptAsync_DocumentTitle_ShouldReturnCurrentTitle(CancellationToken ct = default) {
        // Arrange
        IPage page = await GetRootPageAsync();

        // Act
        string? result = await EvaluateWhenPageReadyAsync<string?>(
            page,
            // lang=javascript
            "async () => { try { return await window.infiniframe.window.features.javaScript.evalAsync('document.title'); } catch(e) { return 'error: ' + e.message; } }"
        );

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsNotEqualTo("error: undefined");
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task ExecuteJavaScriptAsync_NullResult_ShouldReturnNull(CancellationToken ct = default) {
        // Arrange
        IPage page = await GetRootPageAsync();

        // Act
        string? result = await EvaluateWhenPageReadyAsync<string?>(
            page,
            // lang=javascript
            "async () => { try { return await window.infiniframe.window.features.javaScript.evalAsync('undefined'); } catch(e) { return 'error: ' + e.message; } }"
        );

        // Assert - undefined should map to null
        await Assert.That(result).IsNull();
    }
}
