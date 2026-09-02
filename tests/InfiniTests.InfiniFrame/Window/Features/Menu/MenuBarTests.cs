// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Menu;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MenuBarTests {

    [Test]
    public async Task AtBuilderStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();
        var menuBar = new InfiniFrameMenuBar(
            Items: [
                new InfiniFrameMenuItem("file", "File", InfiniFrameMenuItemType.Submenu,
                    Children: [
                        new InfiniFrameMenuItem("open", "Open"),
                        new InfiniFrameMenuItem("save", "Save")
                    ]
                ),
                new InfiniFrameMenuItem("edit", "Edit")
            ]
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
        var builder = new InfiniFrameWindowBuilder();
        var menuBar = new InfiniFrameMenuBar(
            Items: [
                new InfiniFrameMenuItem("help", "Help")
            ]
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
        var builder = new InfiniFrameWindowBuilder();

        // Act
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Menu.MenuBar.Items.IsEmpty).IsTrue();
        await Assert.That(initParameters.MenuBarJson).IsNull();
    }

    [Test]
    public async Task AtBuilderStage_NullMenuBar(CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        builder.Features.Menu.SetMenuBar(null!);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Menu.MenuBar.Items.IsEmpty).IsTrue();
        await Assert.That(initParameters.MenuBarJson).IsNull();
    }

    [Test]
    public async Task AtBuilderStage_MenuBarJson_SerializesCorrectly(CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();
        var menuBar = new InfiniFrameMenuBar(
            Items: [
                new InfiniFrameMenuItem("file", "File", InfiniFrameMenuItemType.Submenu,
                    Children: [
                        new InfiniFrameMenuItem("open", "Open", KeyboardShortcut: "Ctrl+O")
                    ]
                )
            ]
        );

        // Act
        builder.Features.Menu.SetMenuBar(menuBar);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(initParameters.MenuBarJson).IsNotNull();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var deserialized = JsonSerializer.Deserialize<InfiniFrameMenuBar>(initParameters.MenuBarJson!, options);
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized!.Items.Length).IsEqualTo(1);
        await Assert.That(deserialized.Items[0].Id).IsEqualTo("file");
        await Assert.That(deserialized.Items[0].Children.Length).IsEqualTo(1);
        await Assert.That(deserialized.Items[0].Children[0].KeyboardShortcut).IsEqualTo("Ctrl+O");
    }

    [Test]
    public async Task AtBuilderStage_SetMenuBar_EmptyItems_JsonIsNull(CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();
        builder.Features.Menu.SetMenuBar(new InfiniFrameMenuBar(
            Items: [
                new InfiniFrameMenuItem("file", "File")
            ]
        ));

        // Act
        builder.Features.Menu.SetMenuBar(new InfiniFrameMenuBar());
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Menu.MenuBar.Items.IsEmpty).IsTrue();
        await Assert.That(initParameters.MenuBarJson).IsNull();
    }

    [Test]
    public async Task AtBuilderStage_SetMenuBar_ReplacesExisting(CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();
        builder.Features.Menu.SetMenuBar(new InfiniFrameMenuBar(
            Items: [
                new InfiniFrameMenuItem("old", "Old")
            ]
        ));

        // Act
        builder.Features.Menu.SetMenuBar(new InfiniFrameMenuBar(
            Items: [
                new InfiniFrameMenuItem("new", "New")
            ]
        ));
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Menu.MenuBar.Items.Length).IsEqualTo(1);
        await Assert.That(builder.Features.Menu.MenuBar.Items[0].Id).IsEqualTo("new");
        await Assert.That(initParameters.MenuBarJson).IsNotNull();
    }

    [Test]
    public async Task AtBuilderStage_ExtensionReturnsBuilder_ForChaining(CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        IInfiniFrameWindowBuilder result = builder
            .SetMenuBar(new InfiniFrameMenuBar(
                Items: [
                    new InfiniFrameMenuItem("file", "File")
                ]
            ));

        // Assert
        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.Features.Menu.MenuBar.Items.Length).IsEqualTo(1);
    }
}
