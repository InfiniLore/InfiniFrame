// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Records;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameMenuItemTests {

    [Test]
    public async Task DefaultConstructor_SetsEmptyId(CancellationToken ct = default) {
        // Arrange & Act
        var item = new InfiniFrameMenuItem();

        // Assert
        await Assert.That(item.Id).IsEqualTo(string.Empty);
        await Assert.That(item.Label).IsNull();
        await Assert.That(item.Type).IsEqualTo(InfiniFrameMenuItemType.Normal);
        await Assert.That(item.IsEnabled).IsTrue();
        await Assert.That(item.IsVisible).IsTrue();
        await Assert.That(item.KeyboardShortcut).IsNull();
        await Assert.That(item.Children).IsEmpty();
    }

    [Test]
    public async Task ParameterizedConstructor_SetsValues(CancellationToken ct = default) {
        // Arrange & Act
        var item = new InfiniFrameMenuItem(
            "menu-file",
            "File",
            InfiniFrameMenuItemType.Submenu,
            true,
            true,
            "Ctrl+F"
        );

        // Assert
        await Assert.That(item.Id).IsEqualTo("menu-file");
        await Assert.That(item.Label).IsEqualTo("File");
        await Assert.That(item.Type).IsEqualTo(InfiniFrameMenuItemType.Submenu);
        await Assert.That(item.IsEnabled).IsTrue();
        await Assert.That(item.IsVisible).IsTrue();
        await Assert.That(item.KeyboardShortcut).IsEqualTo("Ctrl+F");
    }

    [Test]
    public async Task Children_DefaultValue_IsEmptyArray(CancellationToken ct = default) {
        // Arrange
        var item = new InfiniFrameMenuItem(
            "test",
            "Test"
        );

        // Act & Assert
        await Assert.That(item.Children).IsEmpty();
    }

    [Test]
    public async Task Children_CanBeSetToNonEmptyArray(CancellationToken ct = default) {
        // Arrange
        var child = new InfiniFrameMenuItem("child-1", "Child 1");

        // Act
        var item = new InfiniFrameMenuItem(
            "parent",
            "Parent",
            InfiniFrameMenuItemType.Submenu,
            Children: [child]
        );

        // Assert
        await Assert.That(item.Children.Length).IsEqualTo(1);
        await Assert.That(item.Children[0].Id).IsEqualTo("child-1");
    }

    [Test]
    public async Task Equality_SameValues_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var item1 = new InfiniFrameMenuItem("test", "Test");
        var item2 = new InfiniFrameMenuItem("test", "Test");

        // Act & Assert
        await Assert.That(item1).IsEqualTo(item2);
    }

    [Test]
    public async Task WithExpression_CreatesNewInstance(CancellationToken ct = default) {
        // Arrange
        var original = new InfiniFrameMenuItem("test", "Test");

        // Act
        InfiniFrameMenuItem modified = original with { Label = "Modified" };

        // Assert
        await Assert.That(modified.Label).IsEqualTo("Modified");
        await Assert.That(modified.Id).IsEqualTo("test");
    }
}
