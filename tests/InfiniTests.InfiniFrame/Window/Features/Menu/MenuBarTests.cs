// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;
using System.Collections.Immutable;

namespace InfiniTests.InfiniFrame.Window.Features.Menu;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MenuBarTests {

    [Test]
    public async Task AtBuilderStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        var menuBar = new InfiniFrameMenuBar(
            Items: ImmutableArray.Create(
                new InfiniFrameMenuItem("file", "File", InfiniFrameMenuItemType.Submenu,
                    Children: ImmutableArray.Create(
                        new InfiniFrameMenuItem("open", "Open"),
                        new InfiniFrameMenuItem("save", "Save")
                    )
                ),
                new InfiniFrameMenuItem("edit", "Edit")
            )
        );

        // Act
        builder.Features.Menu.SetMenuBar(menuBar);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Menu.MenuBar).IsEqualTo(menuBar);
        await Assert.That(initParameters.MenuBarJson).IsNotNull();
    }

    [Test]
    public async Task AtBuilderStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        var menuBar = new InfiniFrameMenuBar(
            Items: ImmutableArray.Create(
                new InfiniFrameMenuItem("help", "Help")
            )
        );

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetMenuBar(menuBar);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Menu.MenuBar).IsEqualTo(menuBar);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.MenuBarJson).IsNotNull();
    }

    [Test]
    public async Task AtBuilderStage_DefaultIsEmpty(CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Menu.MenuBar.Items).IsEmpty();
        await Assert.That(initParameters.MenuBarJson).IsNull();
    }

    [Test]
    public async Task AtBuilderStage_NullMenuBar(CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Menu.SetMenuBar(null!);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Menu.MenuBar.Items).IsEmpty();
        await Assert.That(initParameters.MenuBarJson).IsNull();
    }
}
