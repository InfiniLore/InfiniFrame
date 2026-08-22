// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Immutable;
using InfiniFrame;
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.Shared.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MenuItemTreeHelperTests {

    // -----------------------------------------------------------------------------------------------------------------
    // UpdateItem
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task UpdateItem_TopLevelItem_UpdatesItem(CancellationToken ct = default) {
        // Arrange
        ImmutableArray<InfiniFrameMenuItem> items = [
            new InfiniFrameMenuItem("menu1", "File"),
            new InfiniFrameMenuItem("menu2", "Edit")
        ];

        // Act
        ImmutableArray<InfiniFrameMenuItem> result = MenuItemTreeHelper.UpdateItem(
            items, "menu2", item => item with { Label = "Edit Updated" });

        // Assert
        await Assert.That(result.Length).IsEqualTo(2);
        await Assert.That(result[1].Label).IsEqualTo("Edit Updated");
        await Assert.That(result[0].Label).IsEqualTo("File");
    }

    [Test]
    public async Task UpdateItem_NestedItem_UpdatesCorrectItem(CancellationToken ct = default) {
        // Arrange
        ImmutableArray<InfiniFrameMenuItem> items = [
            new InfiniFrameMenuItem("menu1", "File", Children: [
                new InfiniFrameMenuItem("sub1", "Open"),
                new InfiniFrameMenuItem("sub2", "Save")
            ])
        ];

        // Act
        ImmutableArray<InfiniFrameMenuItem> result = MenuItemTreeHelper.UpdateItem(
            items, "sub2", item => item with { Label = "Save As" });

        // Assert
        await Assert.That(result[0].Children[1].Label).IsEqualTo("Save As");
        await Assert.That(result[0].Children[0].Label).IsEqualTo("Open");
    }

    [Test]
    public async Task UpdateItem_NonExistentId_ReturnsUnchangedItems(CancellationToken ct = default) {
        // Arrange
        ImmutableArray<InfiniFrameMenuItem> items = [
            new InfiniFrameMenuItem("menu1", "File")
        ];

        // Act
        ImmutableArray<InfiniFrameMenuItem> result = MenuItemTreeHelper.UpdateItem(
            items, "nonexistent", item => item with { Label = "Changed" });

        // Assert
        await Assert.That(result[0].Label).IsEqualTo("File");
    }

    [Test]
    public async Task UpdateItem_EmptyArray_ReturnsEmptyArray(CancellationToken ct = default) {
        // Arrange
        ImmutableArray<InfiniFrameMenuItem> items = [];

        // Act
        ImmutableArray<InfiniFrameMenuItem> result = MenuItemTreeHelper.UpdateItem(
            items, "any", item => item with { Label = "Changed" });

        // Assert
        await Assert.That(result.IsEmpty).IsTrue();
    }

    [Test]
    public async Task UpdateItem_DeeplyNestedItem_UpdatesCorrectly(CancellationToken ct = default) {
        // Arrange
        ImmutableArray<InfiniFrameMenuItem> items = [
            new InfiniFrameMenuItem("root", "Root", Children: [
                new InfiniFrameMenuItem("level1", "Level1", Children: [
                    new InfiniFrameMenuItem("level2", "Level2")
                ])
            ])
        ];

        // Act
        ImmutableArray<InfiniFrameMenuItem> result = MenuItemTreeHelper.UpdateItem(
            items, "level2", item => item with { Label = "Updated" });

        // Assert
        await Assert.That(result[0].Children[0].Children[0].Label).IsEqualTo("Updated");
    }

    [Test]
    public async Task UpdateItem_MultipleSiblings_UpdatesOnlyMatching(CancellationToken ct = default) {
        // Arrange
        ImmutableArray<InfiniFrameMenuItem> items = [
            new InfiniFrameMenuItem("a", "A"),
            new InfiniFrameMenuItem("b", "B"),
            new InfiniFrameMenuItem("c", "C")
        ];

        // Act
        ImmutableArray<InfiniFrameMenuItem> result = MenuItemTreeHelper.UpdateItem(
            items, "b", item => item with { Label = "Updated B" });

        // Assert
        await Assert.That(result[0].Label).IsEqualTo("A");
        await Assert.That(result[1].Label).IsEqualTo("Updated B");
        await Assert.That(result[2].Label).IsEqualTo("C");
    }
}
