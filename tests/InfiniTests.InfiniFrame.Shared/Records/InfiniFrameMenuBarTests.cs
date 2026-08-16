// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Immutable;
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Records;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameMenuBarTests {

    [Test]
    public async Task DefaultConstructor_CreatesEmptyMenuBar(CancellationToken ct = default) {
        // Arrange & Act
        var menuBar = new InfiniFrameMenuBar();

        // Assert
        await Assert.That(menuBar.Items).IsEmpty();
    }

    [Test]
    public async Task Constructor_WithItems_SetsItems(CancellationToken ct = default) {
        // Arrange
        var item = new InfiniFrameMenuItem(Id: "menu-1", Label: "Menu 1");

        // Act
        var menuBar = new InfiniFrameMenuBar(ImmutableArray.Create(item));

        // Assert
        await Assert.That(menuBar.Items.Length).IsEqualTo(1);
        await Assert.That(menuBar.Items[0].Id).IsEqualTo("menu-1");
    }

    [Test]
    public async Task DefaultImmutableArray_IsHandledCorrectly(CancellationToken ct = default) {
        // Arrange, passing default(ImmutableArray<...>) should result in empty
        var menuBar = new InfiniFrameMenuBar(default);

        // Act & Assert
        await Assert.That(menuBar.Items).IsEmpty();
    }

    [Test]
    public async Task Equality_SameValues_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var bar1 = new InfiniFrameMenuBar();
        var bar2 = new InfiniFrameMenuBar();

        // Act & Assert
        await Assert.That(bar1).IsEqualTo(bar2);
    }
}
