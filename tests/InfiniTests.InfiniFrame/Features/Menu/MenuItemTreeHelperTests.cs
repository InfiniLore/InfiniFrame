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
            new InfiniFrameMenuItem(Id: "file", Label: "File"),
            new InfiniFrameMenuItem(Id: "edit", Label: "Edit")
        );

        // Act
        var result = MenuItemTreeHelper.UpdateItem(items, "edit", item => item with { Label = "Modified" });

        // Assert
        await Assert.That(result[0].Label).IsEqualTo("File");
        await Assert.That(result[1].Label).IsEqualTo("Modified");
    }

    [Test]
    public async Task UpdateItem_MissingId_ReturnsUnchanged(CancellationToken ct = default) {
        // Arrange
        var items = ImmutableArray.Create(
            new InfiniFrameMenuItem(Id: "file", Label: "File")
        );

        // Act
        var result = MenuItemTreeHelper.UpdateItem(items, "nonexistent", item => item with { Label = "Changed" });

        // Assert
        await Assert.That(result[0].Label).IsEqualTo("File");
    }

    [Test]
    public async Task UpdateItem_UpdatesNestedChild(CancellationToken ct = default) {
        // Arrange
        var items = ImmutableArray.Create(
            new InfiniFrameMenuItem(
                Id: "menu",
                Label: "Menu",
                Type: InfiniFrameMenuItemType.Submenu,
                Children: ImmutableArray.Create(
                    new InfiniFrameMenuItem(Id: "item-a", Label: "A"),
                    new InfiniFrameMenuItem(Id: "item-b", Label: "B")
                )
            )
        );

        // Act
        var result = MenuItemTreeHelper.UpdateItem(items, "item-b", item => item with { Label = "Modified B" });

        // Assert
        await Assert.That(result[0].Children[1].Label).IsEqualTo("Modified B");
        await Assert.That(result[0].Children[0].Label).IsEqualTo("A");
    }

    [Test]
    public async Task UpdateItem_EmptyArray_ReturnsEmpty(CancellationToken ct = default) {
        // Arrange
        var items = ImmutableArray<InfiniFrameMenuItem>.Empty;

        // Act
        var result = MenuItemTreeHelper.UpdateItem(items, "any", item => item with { Label = "Changed" });

        // Assert
        await Assert.That(result).IsEmpty();
    }

    [Test]
    public async Task UpdateItem_DoesNotMutateOriginal(CancellationToken ct = default) {
        // Arrange
        var items = ImmutableArray.Create(
            new InfiniFrameMenuItem(Id: "a", Label: "Original")
        );

        // Act
        var result = MenuItemTreeHelper.UpdateItem(items, "a", item => item with { Label = "Changed" });

        // Assert
        await Assert.That(items[0].Label).IsEqualTo("Original");
        await Assert.That(result[0].Label).IsEqualTo("Changed");
    }
}
