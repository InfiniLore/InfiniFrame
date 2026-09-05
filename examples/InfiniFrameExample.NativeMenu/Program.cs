// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using System.Drawing;
using System.Text.Json;

namespace InfiniFrameExample.NativeMenu;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        InfiniFrameMenuBar menuBar = new(
            Items: [
                new InfiniFrameMenuItem(
                    Id: "file",
                    Label: "File",
                    Type: InfiniFrameMenuItemType.Submenu,
                    Children: [
                        new InfiniFrameMenuItem(Id: "file-new", Label: "New", KeyboardShortcut: "Ctrl+N"),
                        new InfiniFrameMenuItem(Id: "file-open", Label: "Open...", KeyboardShortcut: "Ctrl+O"),
                        new InfiniFrameMenuItem(Id: "file-separator", Type: InfiniFrameMenuItemType.Separator),
                        new InfiniFrameMenuItem(Id: "file-save", Label: "Save", KeyboardShortcut: "Ctrl+S", IsEnabled: false),
                        new InfiniFrameMenuItem(Id: "file-save-as", Label: "Save As...", KeyboardShortcut: "Ctrl+Shift+S"),
                        new InfiniFrameMenuItem(Id: "file-separator2", Type: InfiniFrameMenuItemType.Separator),
                        new InfiniFrameMenuItem(Id: "file-exit", Label: "Exit", KeyboardShortcut: "Alt+F4")
                    ]
                ),
                new InfiniFrameMenuItem(
                    Id: "edit",
                    Label: "Edit",
                    Type: InfiniFrameMenuItemType.Submenu,
                    Children: [
                        new InfiniFrameMenuItem(Id: "edit-undo", Label: "Undo", KeyboardShortcut: "Ctrl+Z"),
                        new InfiniFrameMenuItem(Id: "edit-redo", Label: "Redo", KeyboardShortcut: "Ctrl+Y"),
                        new InfiniFrameMenuItem(Id: "edit-separator", Type: InfiniFrameMenuItemType.Separator),
                        new InfiniFrameMenuItem(Id: "edit-cut", Label: "Cut", KeyboardShortcut: "Ctrl+X"),
                        new InfiniFrameMenuItem(Id: "edit-copy", Label: "Copy", KeyboardShortcut: "Ctrl+C"),
                        new InfiniFrameMenuItem(Id: "edit-paste", Label: "Paste", KeyboardShortcut: "Ctrl+V")
                    ]
                ),
                new InfiniFrameMenuItem(
                    Id: "view",
                    Label: "View",
                    Type: InfiniFrameMenuItemType.Submenu,
                    Children: [
                        new InfiniFrameMenuItem(
                            Id: "view-zoom",
                            Label: "Zoom",
                            Type: InfiniFrameMenuItemType.Submenu,
                            Children: [
                                new InfiniFrameMenuItem(Id: "view-zoom-in", Label: "Zoom In", KeyboardShortcut: "Ctrl+="),
                                new InfiniFrameMenuItem(Id: "view-zoom-out", Label: "Zoom Out", KeyboardShortcut: "Ctrl+-"),
                                new InfiniFrameMenuItem(Id: "view-zoom-reset", Label: "Reset Zoom", KeyboardShortcut: "Ctrl+0")
                            ]
                        ),
                        new InfiniFrameMenuItem(Id: "view-separator", Type: InfiniFrameMenuItemType.Separator),
                        new InfiniFrameMenuItem(Id: "view-fullscreen", Label: "Fullscreen", KeyboardShortcut: "F11")
                    ]
                ),
                new InfiniFrameMenuItem(
                    Id: "help",
                    Label: "Help",
                    Type: InfiniFrameMenuItemType.Submenu,
                    Children: [
                        new InfiniFrameMenuItem(Id: "help-about", Label: "About InfiniFrame")
                    ]
                )
            ]
        );

        IInfiniFrameWindow window = new InfiniFrameWindowBuilder()
            .SetTitle("InfiniFrame Native Menu Example")
            .SetSize(new Size(960, 640))
            .CenteredOnMainMonitor()
            .SetMenuBar(menuBar)
            .UseEmbeddedWwwrootAssets(
                scheme: "app",
                includePhysicalFallback: true,
                physicalWwwrootPath: Path.Join(AppContext.BaseDirectory, "wwwroot"),
                setStartUrl: true
            )
            .RegisterWebMessageReceivedHandler((win, message) => {
                string? action = ExtractAction(message);
                if (action == null) return;

                switch (action) {
                    case "enable-save":
                        win.Features.Menu.SetMenuItemEnabled("file-save", true);
                        win.SendWebMessage("status:Save enabled");
                        break;

                    case "disable-save":
                        win.Features.Menu.SetMenuItemEnabled("file-save", false);
                        win.SendWebMessage("status:Save disabled");
                        break;

                    case "toggle-undo":
                        bool undoVisible = win.Features.Menu.MenuBar.Items
                            .First(i => i.Id == "edit").Children
                            .First(i => i.Id == "edit-undo").IsVisible;
                        win.Features.Menu.SetMenuItemVisible("edit-undo", !undoVisible);
                        win.SendWebMessage($"status:Undo {(undoVisible ? "hidden" : "shown")}");
                        break;

                    default:
                        win.SendWebMessage($"status:Action: {action}");
                        break;
                }
            })
            .Build();

        window.WaitForClose();
    }

    private static string? ExtractAction(string rawMessage) {
        try {
            using JsonDocument doc = JsonDocument.Parse(rawMessage);
            JsonElement root = doc.RootElement;
            if (root.TryGetProperty("data", out JsonElement data) && data.TryGetProperty("action", out JsonElement action)) {
                return action.GetString();
            }
        } catch {
            // Not an envelope from sendMessageToHost, ignore.
        }
        return null;
    }
}
