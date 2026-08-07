// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using System.Collections.Immutable;

namespace InfiniTests.InfiniFrame.Window.Features.Menu;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MenuItemEnableDisableTests {

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_SetMenuItemEnabled(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Menu.SetMenuBar(new InfiniFrameMenuBar(
                Items: ImmutableArray.Create(
                    new InfiniFrameMenuItem("file", "File", InfiniFrameMenuItemType.Submenu,
                        Children: ImmutableArray.Create(
                            new InfiniFrameMenuItem("save", "Save")
                        )
                    )
                )
            ));
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Menu.SetMenuItemEnabled("save", false);

        // Assert
        var menuBar = window.Features.Menu.MenuBar;
        var fileItem = menuBar.Items[0];
        var saveItem = fileItem.Children[0];
        await Assert.That(saveItem.IsEnabled).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_SetMenuItemVisible(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Menu.SetMenuBar(new InfiniFrameMenuBar(
                Items: ImmutableArray.Create(
                    new InfiniFrameMenuItem("file", "File", InfiniFrameMenuItemType.Submenu,
                        Children: ImmutableArray.Create(
                            new InfiniFrameMenuItem("save", "Save")
                        )
                    )
                )
            ));
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Menu.SetMenuItemVisible("save", false);

        // Assert
        var menuBar = window.Features.Menu.MenuBar;
        var fileItem = menuBar.Items[0];
        var saveItem = fileItem.Children[0];
        await Assert.That(saveItem.IsVisible).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ClickMenuItem(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Menu.SetMenuBar(new InfiniFrameMenuBar(
                Items: ImmutableArray.Create(
                    new InfiniFrameMenuItem("file", "File")
                )
            ));
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act & Assert (should not throw)
        window.Features.Menu.ClickMenuItem("file");
        await Assert.That(window.Features.Menu.MenuBar.Items[0].Id).IsEqualTo("file");
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectSetMenuBar(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        var menuBar = new InfiniFrameMenuBar(
            Items: ImmutableArray.Create(
                new InfiniFrameMenuItem("help", "Help")
            )
        );

        // Act
        window.Features.Menu.SetMenuBar(menuBar);

        // Assert
        await Assert.That(window.Features.Menu.MenuBar).IsEqualTo(menuBar);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectSetMenuBar_ReplacesExisting(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Menu.SetMenuBar(new InfiniFrameMenuBar(
                Items: ImmutableArray.Create(
                    new InfiniFrameMenuItem("old", "Old")
                )
            ));
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        var newMenuBar = new InfiniFrameMenuBar(
            Items: ImmutableArray.Create(
                new InfiniFrameMenuItem("new", "New")
            )
        );

        // Act
        window.Features.Menu.SetMenuBar(newMenuBar);

        // Assert
        await Assert.That(window.Features.Menu.MenuBar.Items).Count().IsEqualTo(1);
        await Assert.That(window.Features.Menu.MenuBar.Items[0].Id).IsEqualTo("new");
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_SetMenuBar_Null_ClearsMenu(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Menu.SetMenuBar(new InfiniFrameMenuBar(
                Items: ImmutableArray.Create(
                    new InfiniFrameMenuItem("file", "File")
                )
            ));
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Menu.SetMenuBar(null!);

        // Assert
        await Assert.That(window.Features.Menu.MenuBar.Items).IsEmpty();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_SetMenuBar_EmptyItems_ClearsMenu(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Menu.SetMenuBar(new InfiniFrameMenuBar(
                Items: ImmutableArray.Create(
                    new InfiniFrameMenuItem("file", "File")
                )
            ));
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Menu.SetMenuBar(new InfiniFrameMenuBar());

        // Assert
        await Assert.That(window.Features.Menu.MenuBar.Items).IsEmpty();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_SetMenuItemEnabled_NonExistentId_NoOp(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Menu.SetMenuBar(new InfiniFrameMenuBar(
                Items: ImmutableArray.Create(
                    new InfiniFrameMenuItem("file", "File")
                )
            ));
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Menu.SetMenuItemEnabled("nonexistent", false);

        // Assert - original item unchanged
        await Assert.That(window.Features.Menu.MenuBar.Items[0].IsEnabled).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_SetMenuItemVisible_NonExistentId_NoOp(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Menu.SetMenuBar(new InfiniFrameMenuBar(
                Items: ImmutableArray.Create(
                    new InfiniFrameMenuItem("file", "File")
                )
            ));
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Menu.SetMenuItemVisible("nonexistent", false);

        // Assert - original item unchanged
        await Assert.That(window.Features.Menu.MenuBar.Items[0].IsVisible).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_SetMenuItemEnabled_DeeplyNestedItem(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Menu.SetMenuBar(new InfiniFrameMenuBar(
                Items: ImmutableArray.Create(
                    new InfiniFrameMenuItem("level1", "Level 1", InfiniFrameMenuItemType.Submenu,
                        Children: ImmutableArray.Create(
                            new InfiniFrameMenuItem("level2", "Level 2", InfiniFrameMenuItemType.Submenu,
                                Children: ImmutableArray.Create(
                                    new InfiniFrameMenuItem("level3", "Level 3")
                                )
                            )
                        )
                    )
                )
            ));
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Menu.SetMenuItemEnabled("level3", false);

        // Assert
        var item = window.Features.Menu.MenuBar.Items[0].Children[0].Children[0];
        await Assert.That(item.IsEnabled).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_SetMenuItemVisible_DeeplyNestedItem(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Menu.SetMenuBar(new InfiniFrameMenuBar(
                Items: ImmutableArray.Create(
                    new InfiniFrameMenuItem("level1", "Level 1", InfiniFrameMenuItemType.Submenu,
                        Children: ImmutableArray.Create(
                            new InfiniFrameMenuItem("level2", "Level 2", InfiniFrameMenuItemType.Submenu,
                                Children: ImmutableArray.Create(
                                    new InfiniFrameMenuItem("level3", "Level 3")
                                )
                            )
                        )
                    )
                )
            ));
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Menu.SetMenuItemVisible("level3", false);

        // Assert
        var item = window.Features.Menu.MenuBar.Items[0].Children[0].Children[0];
        await Assert.That(item.IsVisible).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_SetMenuItemEnabled_ToggleBackToTrue(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Menu.SetMenuBar(new InfiniFrameMenuBar(
                Items: ImmutableArray.Create(
                    new InfiniFrameMenuItem("test", "Test")
                )
            ));
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Menu.SetMenuItemEnabled("test", false);
        window.Features.Menu.SetMenuItemEnabled("test", true);

        // Assert
        await Assert.That(window.Features.Menu.MenuBar.Items[0].IsEnabled).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_SetMenuItemVisible_ToggleBackToTrue(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Menu.SetMenuBar(new InfiniFrameMenuBar(
                Items: ImmutableArray.Create(
                    new InfiniFrameMenuItem("test", "Test")
                )
            ));
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Menu.SetMenuItemVisible("test", false);
        window.Features.Menu.SetMenuItemVisible("test", true);

        // Assert
        await Assert.That(window.Features.Menu.MenuBar.Items[0].IsVisible).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionSetMenuBar(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        var menuBar = new InfiniFrameMenuBar(
            Items: ImmutableArray.Create(
                new InfiniFrameMenuItem("help", "Help")
            )
        );

        // Act
        IInfiniFrameWindow returnedWindow = window.SetMenuBar(menuBar);

        // Assert
        await Assert.That(window.Features.Menu.MenuBar).IsEqualTo(menuBar);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionSetMenuItemEnabled(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Menu.SetMenuBar(new InfiniFrameMenuBar(
                Items: ImmutableArray.Create(
                    new InfiniFrameMenuItem("test", "Test")
                )
            ));
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetMenuItemEnabled("test", false);

        // Assert
        await Assert.That(window.Features.Menu.MenuBar.Items[0].IsEnabled).IsFalse();
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionSetMenuItemVisible(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Menu.SetMenuBar(new InfiniFrameMenuBar(
                Items: ImmutableArray.Create(
                    new InfiniFrameMenuItem("test", "Test")
                )
            ));
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetMenuItemVisible("test", false);

        // Assert
        await Assert.That(window.Features.Menu.MenuBar.Items[0].IsVisible).IsFalse();
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionClickMenuItem(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Menu.SetMenuBar(new InfiniFrameMenuBar(
                Items: ImmutableArray.Create(
                    new InfiniFrameMenuItem("test", "Test")
                )
            ));
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IInfiniFrameWindow returnedWindow = window.ClickMenuItem("test");

        // Assert
        await Assert.That(window.Features.Menu.MenuBar.Items[0].Id).IsEqualTo("test");
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
