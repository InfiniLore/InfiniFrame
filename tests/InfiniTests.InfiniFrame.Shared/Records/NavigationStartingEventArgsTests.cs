// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Records;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NavigationStartingEventArgsTests {

    [Test]
    public async Task Constructor_SetsAllProperties(CancellationToken ct = default) {
        // Arrange & Act
        var args = new NavigationStartingEventArgs(
            Url: "https://example.com",
            IsUserInitiated: true,
            IsRedirect: false,
            IsMainFrame: true
        );

        // Assert
        await Assert.That(args.Url).IsEqualTo("https://example.com");
        await Assert.That(args.IsUserInitiated).IsTrue();
        await Assert.That(args.IsRedirect).IsFalse();
        await Assert.That(args.IsMainFrame).IsTrue();
    }

    [Test]
    public async Task Equality_SameValues_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var args1 = new NavigationStartingEventArgs("https://example.com", true, false, true);
        var args2 = new NavigationStartingEventArgs("https://example.com", true, false, true);

        // Act & Assert
        await Assert.That(args1).IsEqualTo(args2);
    }

    [Test]
    public async Task Equality_DifferentValues_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var args1 = new NavigationStartingEventArgs("https://example.com", true, false, true);
        var args2 = new NavigationStartingEventArgs("https://other.com", true, false, true);

        // Act & Assert
        await Assert.That(args1).IsNotEqualTo(args2);
    }

    [Test]
    public async Task WithExpression_CreatesNewInstance(CancellationToken ct = default) {
        // Arrange
        var original = new NavigationStartingEventArgs("https://example.com", true, false, true);

        // Act
        NavigationStartingEventArgs modified = original with { Url = "https://modified.com" };

        // Assert
        await Assert.That(modified.Url).IsEqualTo("https://modified.com");
        await Assert.That(modified.IsUserInitiated).IsTrue();
        await Assert.That(original.Url).IsEqualTo("https://example.com");
    }
}
