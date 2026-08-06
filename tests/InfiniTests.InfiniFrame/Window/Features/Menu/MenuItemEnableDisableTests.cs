// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

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
                Items: System.Collections.Immutable.ImmutableArray.Create(
                    new InfiniFrameMenuItem("file", "File", InfiniFrameMenuItemType.Submenu,
                        Children: System.Collections.Immutable.ImmutableArray.Create(
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
                Items: System.Collections.Immutable.ImmutableArray.Create(
                    new InfiniFrameMenuItem("file", "File", InfiniFrameMenuItemType.Submenu,
                        Children: System.Collections.Immutable.ImmutableArray.Create(
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
                Items: System.Collections.Immutable.ImmutableArray.Create(
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
    public async Task AtWindowStage_ExtensionSetMenuBar(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        var menuBar = new InfiniFrameMenuBar(
            Items: System.Collections.Immutable.ImmutableArray.Create(
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
                Items: System.Collections.Immutable.ImmutableArray.Create(
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
}
