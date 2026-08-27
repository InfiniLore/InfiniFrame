// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView.FileProviders;

namespace InfiniTests.InfiniFrame.Shared.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class StaticWebAssetPatternTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Default values
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task DefaultValues_ShouldBeCorrect(CancellationToken ct = default) {
        // Arrange & Act
        var pattern = new StaticWebAssetPattern();

        // Assert
        await Assert.That(pattern.ContentRootIndex).IsEqualTo(0);
        await Assert.That(pattern.Pattern).IsEqualTo(string.Empty);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Property setters
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ContentRootIndex_CanBeSet(CancellationToken ct = default) {
        // Arrange & Act
        var pattern = new StaticWebAssetPattern { ContentRootIndex = 2 };

        // Assert
        await Assert.That(pattern.ContentRootIndex).IsEqualTo(2);
    }

    [Test]
    public async Task Pattern_CanBeSet(CancellationToken ct = default) {
        // Arrange & Act
        var pattern = new StaticWebAssetPattern { Pattern = "*.css" };

        // Assert
        await Assert.That(pattern.Pattern).IsEqualTo("*.css");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Wildcard patterns
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments("*.css")]
    [Arguments("*.js")]
    [Arguments("*.html")]
    [Arguments("*.png")]
    public async Task WildcardPatterns_StandardExtensions_ShouldStoreCorrectly(string patternValue, CancellationToken ct = default) {
        // Arrange & Act
        var pattern = new StaticWebAssetPattern { Pattern = patternValue };

        // Assert
        await Assert.That(pattern.Pattern).IsEqualTo(patternValue);
    }

    [Test]
    [Arguments("**/*.css")]
    [Arguments("**/*.js")]
    [Arguments("**/*.png")]
    public async Task WildcardPatterns_NestedWildcards_ShouldStoreCorrectly(string patternValue, CancellationToken ct = default) {
        // Arrange & Act
        var pattern = new StaticWebAssetPattern { Pattern = patternValue };

        // Assert
        await Assert.That(pattern.Pattern).IsEqualTo(patternValue);
    }

    [Test]
    public async Task WildcardPatterns_FrameworkPattern_ShouldStoreCorrectly(CancellationToken ct = default) {
        // Arrange & Act
        var pattern = new StaticWebAssetPattern {
            ContentRootIndex = 0,
            Pattern = "_framework/**"
        };

        // Assert
        await Assert.That(pattern.Pattern).IsEqualTo("_framework/**");
        await Assert.That(pattern.ContentRootIndex).IsEqualTo(0);
    }

    [Test]
    public async Task WildcardPatterns_ContentPattern_ShouldStoreCorrectly(CancellationToken ct = default) {
        // Arrange & Act
        var pattern = new StaticWebAssetPattern {
            ContentRootIndex = 1,
            Pattern = "_content/**"
        };

        // Assert
        await Assert.That(pattern.Pattern).IsEqualTo("_content/**");
        await Assert.That(pattern.ContentRootIndex).IsEqualTo(1);
    }

    [Test]
    [Arguments("*")]
    [Arguments("*.*")]
    [Arguments(".*")]
    public async Task WildcardPatterns_GlobPatterns_ShouldStoreCorrectly(string patternValue, CancellationToken ct = default) {
        // Arrange & Act
        var pattern = new StaticWebAssetPattern { Pattern = patternValue };

        // Assert
        await Assert.That(pattern.Pattern).IsEqualTo(patternValue);
    }

    [Test]
    public async Task Pattern_EmptyString_ShouldBeAllowed(CancellationToken ct = default) {
        // Arrange & Act
        var pattern = new StaticWebAssetPattern { Pattern = "" };

        // Assert
        await Assert.That(pattern.Pattern).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task Pattern_SpecialCharacters_ShouldStoreCorrectly(CancellationToken ct = default) {
        // Arrange & Act
        var pattern = new StaticWebAssetPattern { Pattern = "file-[0-9]+.txt" };

        // Assert
        await Assert.That(pattern.Pattern).IsEqualTo("file-[0-9]+.txt");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Patterns in node context
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task MultiplePatterns_CanBeAddedToNode(CancellationToken ct = default) {
        // Arrange & Act
        var node = new StaticWebAssetNode {
            Patterns = [
                new StaticWebAssetPattern { ContentRootIndex = 0, Pattern = "*.css" },
                new StaticWebAssetPattern { ContentRootIndex = 0, Pattern = "*.js" },
                new StaticWebAssetPattern { ContentRootIndex = 1, Pattern = "_content/**" }
            ]
        };

        // Assert
        await Assert.That(node.Patterns!.Count).IsEqualTo(3);
        await Assert.That(node.Patterns[0].Pattern).IsEqualTo("*.css");
        await Assert.That(node.Patterns[1].Pattern).IsEqualTo("*.js");
        await Assert.That(node.Patterns[2].Pattern).IsEqualTo("_content/**");
    }
}
