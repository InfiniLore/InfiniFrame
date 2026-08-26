// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Menu;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MenuItemTests {

    [Test]
    public async Task MenuItem_Creation(CancellationToken ct) {
        // Arrange & Act
        var item = new InfiniFrameMenuItem(
            "test-id",
            "Test Label",
            InfiniFrameMenuItemType.Normal,
            false,
            false,
            "Ctrl+T"
        );

        // Assert
        await Assert.That(item.Id).IsEqualTo("test-id");
        await Assert.That(item.Label).IsEqualTo("Test Label");
        await Assert.That(item.Type).IsEqualTo(InfiniFrameMenuItemType.Normal);
        await Assert.That(item.IsEnabled).IsFalse();
        await Assert.That(item.IsVisible).IsFalse();
        await Assert.That(item.KeyboardShortcut).IsEqualTo("Ctrl+T");
        await Assert.That(item.Children.IsEmpty).IsTrue();
    }

    [Test]
    public async Task MenuItem_DefaultValues(CancellationToken ct) {
        // Arrange & Act
        var item = new InfiniFrameMenuItem("id");

        // Assert
        await Assert.That(item.IsEnabled).IsTrue();
        await Assert.That(item.IsVisible).IsTrue();
        await Assert.That(item.Type).IsEqualTo(InfiniFrameMenuItemType.Normal);
        await Assert.That(item.Label).IsNull();
        await Assert.That(item.KeyboardShortcut).IsNull();
        await Assert.That(item.Children.IsEmpty).IsTrue();
    }

    [Test]
    public async Task MenuItem_Separator(CancellationToken ct) {
        // Arrange & Act
        var item = new InfiniFrameMenuItem("sep", Type: InfiniFrameMenuItemType.Separator);

        // Assert
        await Assert.That(item.Type).IsEqualTo(InfiniFrameMenuItemType.Separator);
        await Assert.That(item.Label).IsNull();
    }

    [Test]
    public async Task MenuItem_SubmenuWithChildren(CancellationToken ct) {
        // Arrange
        var child1 = new InfiniFrameMenuItem("child1", "Child 1");
        var child2 = new InfiniFrameMenuItem("child2", "Child 2");

        // Act
        var submenu = new InfiniFrameMenuItem(
            "parent",
            "Parent",
            InfiniFrameMenuItemType.Submenu,
            Children: [child1, child2]
        );

        // Assert
        await Assert.That(submenu.Type).IsEqualTo(InfiniFrameMenuItemType.Submenu);
        await Assert.That(submenu.Children.Length).IsEqualTo(2);
        await Assert.That(submenu.Children[0].Id).IsEqualTo("child1");
        await Assert.That(submenu.Children[1].Id).IsEqualTo("child2");
    }

    [Test]
    public async Task MenuBar_EmptyDefault(CancellationToken ct) {
        // Arrange & Act
        var menuBar = new InfiniFrameMenuBar();

        // Assert
        await Assert.That(menuBar.Items.IsEmpty).IsTrue();
    }

    [Test]
    public async Task MenuBar_WithItems(CancellationToken ct) {
        // Arrange & Act
        var menuBar = new InfiniFrameMenuBar(
            Items: [
                new InfiniFrameMenuItem("file", "File"),
                new InfiniFrameMenuItem("edit", "Edit")
            ]
        );

        // Assert
        await Assert.That(menuBar.Items.Length).IsEqualTo(2);
        await Assert.That(menuBar.Items[0].Id).IsEqualTo("file");
        await Assert.That(menuBar.Items[1].Id).IsEqualTo("edit");
    }
}
