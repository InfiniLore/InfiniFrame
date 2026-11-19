// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Menu;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Regression tests for GTK4 menu resource lifecycle (audit finding 5): menu
///     replacement, enabled/visibility overrides, nested submenus, and repeated
///     window creation/destruction with menus attached. GObject-level leak validation
///     for the same scenario runs under ASan/LSan in scripts/linux-teardown-stress
///     ("menu-window-lifecycle"); this test validates the managed-to-native contract.
/// </summary>
public class MenuLifecycleRegressionTests {
    private static InfiniFrameMenuBar BuildMenuBarA() => new(
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

    private static InfiniFrameMenuBar BuildMenuBarB() => new(
        Items: [
            new InfiniFrameMenuItem("recent", "Recent", InfiniFrameMenuItemType.Submenu,
                Children: [
                    new InfiniFrameMenuItem("one", "One", InfiniFrameMenuItemType.Submenu,
                        Children: [new InfiniFrameMenuItem("deep", "Deep")])
                ]
            )
        ]
    );

    [Test]
    [NotInParallelInfiniTests]
    [DefaultTimeout(120_000)]
    public async Task MenuReplacementAndOverrides_AcrossRepeatedWindowCycles_RemainFunctional(CancellationToken ct = default) {
        // Arrange: repeated create/replace/toggle/click/destroy cycles exercise the
        // full menu resource ownership path (models, action groups, activation data).
        const int cycles = 5;

        for (int i = 0; i < cycles; i++) {
            ct.ThrowIfCancellationRequested();

            // Act
            using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
                builder.Features.Menu.SetMenuBar(BuildMenuBarA());
            }, ct);
            IInfiniFrameWindow window = windowUtility.Window;
            InfiniFrameMenuBar initial = window.Features.Menu.MenuBar;
            await Assert.That(initial.Items.Length).IsEqualTo(2);
            await Assert.That(initial.Items[0].Id).IsEqualTo("file");
            await Assert.That(initial.Items[0].Children.Length).IsEqualTo(2);

            // Replace with a nested menu definition.
            window.Features.Menu.SetMenuBar(BuildMenuBarB());
            InfiniFrameMenuBar replaced = window.Features.Menu.MenuBar;
            await Assert.That(replaced.Items.Length).IsEqualTo(1);
            await Assert.That(replaced.Items[0].Id).IsEqualTo("recent");
            await Assert.That(replaced.Items[0].Children[0].Id).IsEqualTo("one");
            await Assert.That(replaced.Items[0].Children[0].Children[0].Id).IsEqualTo("deep");

            // Runtime overrides on nested items, then flip back.
            window.Features.Menu.SetMenuItemEnabled("deep", false);
            window.Features.Menu.SetMenuItemVisible("one", false);
            window.Features.Menu.SetMenuItemVisible("one", true);
            window.Features.Menu.ClickMenuItem("deep");
            InfiniFrameMenuBar afterOverrides = window.Features.Menu.MenuBar;
            await Assert.That(afterOverrides.Items[0].Children[0].Children[0].IsEnabled).IsFalse();
            await Assert.That(afterOverrides.Items[0].Children[0].IsVisible).IsTrue();

            // Replace back to the first definition (full state replacement).
            window.Features.Menu.SetMenuBar(BuildMenuBarA());
            InfiniFrameMenuBar restored = window.Features.Menu.MenuBar;
            await Assert.That(restored.Items.Length).IsEqualTo(2);
            await Assert.That(restored.Items[1].Id).IsEqualTo("edit");
            window.Features.Menu.SetMenuItemEnabled("save", false);
            window.Features.Menu.ClickMenuItem("open");
            await Assert.That(window.Features.Menu.MenuBar.Items[0].Children[1].IsEnabled).IsFalse();

            // Assert: the window stays healthy and closes cleanly (no leak-induced
            // hang or crash in native menu teardown).
            await Assert.That(window.IsClosedOrClosing()).IsFalse();
            window.Close();
            window.WaitForClose();
        }
    }
}
