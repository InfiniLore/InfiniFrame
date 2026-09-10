// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Menu;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MenuBarSerializationTests {

    [Test]
    public async Task MenuBar_RoundTrip_JsonSerialization(CancellationToken ct) {
        // Arrange
        var menuBar = new InfiniFrameMenuBar(
            Items: [
                new InfiniFrameMenuItem("file", "File", InfiniFrameMenuItemType.Submenu,
                    Children: [
                        new InfiniFrameMenuItem("open", "Open", KeyboardShortcut: "Ctrl+O"),
                        new InfiniFrameMenuItem("save", "Save", KeyboardShortcut: "Ctrl+S"),
                        new InfiniFrameMenuItem("sep", Type: InfiniFrameMenuItemType.Separator),
                        new InfiniFrameMenuItem("exit", "Exit", KeyboardShortcut: "Alt+F4")
                    ]
                ),
                new InfiniFrameMenuItem("edit", "Edit"),
                new InfiniFrameMenuItem("help", "Help", IsEnabled: false, IsVisible: false)
            ]
        );

        // Act
        string json = JsonSerializer.Serialize(menuBar);
        var deserialized = JsonSerializer.Deserialize<InfiniFrameMenuBar>(json);

        // Assert
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized!.Items.Length).IsEqualTo(3);
        await Assert.That(deserialized.Items[0].Id).IsEqualTo("file");
        await Assert.That(deserialized.Items[0].Children.Length).IsEqualTo(4);
        await Assert.That(deserialized.Items[0].Children[2].Type).IsEqualTo(InfiniFrameMenuItemType.Separator);
        await Assert.That(deserialized.Items[2].IsEnabled).IsFalse();
        await Assert.That(deserialized.Items[2].IsVisible).IsFalse();
    }

    [Test]
    public async Task MenuBar_Null_DeserializesToEmpty(CancellationToken ct) {
        // Arrange
        string json = "null";

        // Act
        var deserialized = JsonSerializer.Deserialize<InfiniFrameMenuBar>(json);

        // Assert
        await Assert.That(deserialized).IsNull();
    }

    [Test]
    public async Task MenuBar_EmptyItems_DeserializesCorrectly(CancellationToken ct) {
        // Arrange
        string json = """{"Items":[]}""";

        // Act
        var deserialized = JsonSerializer.Deserialize<InfiniFrameMenuBar>(json);

        // Assert
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized!.Items.IsEmpty).IsTrue();
    }
}
