// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Immutable;
using InfiniFrame;
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.Features.Menu;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MenuItemTreeHelperTests {

    [Test]
    public async Task UpdateItem_UpdatesMatchingItem(CancellationToken ct = default) {
        // Arrange
        var items = ImmutableArray.Create(
            new InfiniFrameMenuItem("file", "File"),
            new InfiniFrameMenuItem("edit", "Edit")
        );

        // Act
        ImmutableArray<InfiniFrameMenuItem> result = MenuItemTreeHelper.UpdateItem(items, "edit", updater: item => item with { Label = "Modified" });

        // Assert
        await Assert.That(result[0].Label).IsEqualTo("File");
        await Assert.That(result[1].Label).IsEqualTo("Modified");
    }

    [Test]
    public async Task UpdateItem_MissingId_ReturnsUnchanged(CancellationToken ct = default) {
        // Arrange
        var items = ImmutableArray.Create(
            new InfiniFrameMenuItem("file", "File")
        );

        // Act
        ImmutableArray<InfiniFrameMenuItem> result = MenuItemTreeHelper.UpdateItem(items, "nonexistent", updater: item => item with { Label = "Changed" });

        // Assert
        await Assert.That(result[0].Label).IsEqualTo("File");
    }

    [Test]
    public async Task UpdateItem_UpdatesNestedChild(CancellationToken ct = default) {
        // Arrange
        var items = ImmutableArray.Create(
            new InfiniFrameMenuItem(
                "menu",
                "Menu",
                InfiniFrameMenuItemType.Submenu,
                Children: [
                    new InfiniFrameMenuItem("item-a", "A"),
                    new InfiniFrameMenuItem("item-b", "B")
                ]
            )
        );

        // Act
        ImmutableArray<InfiniFrameMenuItem> result = MenuItemTreeHelper.UpdateItem(items, "item-b", updater: item => item with { Label = "Modified B" });

        // Assert
        await Assert.That(result[0].Children[1].Label).IsEqualTo("Modified B");
        await Assert.That(result[0].Children[0].Label).IsEqualTo("A");
    }

    [Test]
    public async Task UpdateItem_EmptyArray_ReturnsEmpty(CancellationToken ct = default) {
        // Arrange
        var items = ImmutableArray<InfiniFrameMenuItem>.Empty;

        // Act
        ImmutableArray<InfiniFrameMenuItem> result = MenuItemTreeHelper.UpdateItem(items, "any", updater: item => item with { Label = "Changed" });

        // Assert
        await Assert.That(result).IsEmpty();
    }

    [Test]
    public async Task UpdateItem_DoesNotMutateOriginal(CancellationToken ct = default) {
        // Arrange
        var items = ImmutableArray.Create(
            new InfiniFrameMenuItem("a", "Original")
        );

        // Act
        ImmutableArray<InfiniFrameMenuItem> result = MenuItemTreeHelper.UpdateItem(items, "a", updater: item => item with { Label = "Changed" });

        // Assert
        await Assert.That(items[0].Label).IsEqualTo("Original");
        await Assert.That(result[0].Label).IsEqualTo("Changed");
    }
}
