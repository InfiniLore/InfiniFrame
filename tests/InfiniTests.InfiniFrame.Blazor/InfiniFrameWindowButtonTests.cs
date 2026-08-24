// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using AngleSharp.Dom;
using Bunit;
using InfiniFrame.Blazor;

namespace InfiniTests.InfiniFrame.Blazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowButtonTests : BunitContext {

    [Test]
    [Arguments(WindowAction.Minimize, "minimize")]
    [Arguments(WindowAction.Maximize, "maximize")]
    [Arguments(WindowAction.Close, "close")]
    public async Task HasCorrectDataAttribute(WindowAction action, string expectedAttribute, CancellationToken ct = default) {
        // Arrange
        IRenderedComponent<InfiniFrameWindowButton> cut = Render<InfiniFrameWindowButton>(parameters =>
            parameters.Add(p => p.WindowAction, action)
        );

        // Act
        IElement div = cut.Find("div");

        // Assert
        await Assert.That(div.GetAttribute("data-infiniframe-window-action")).IsEqualTo(expectedAttribute);
    }

    [Test]
    public async Task RendersWindowButtonClass(CancellationToken ct = default) {
        // Arrange
        IRenderedComponent<InfiniFrameWindowButton> cut = Render<InfiniFrameWindowButton>(parameters =>
            parameters.Add(p => p.WindowAction, WindowAction.Minimize)
        );

        // Act
        IElement div = cut.Find("div");

        // Assert
        await Assert.That(div.ClassList.Contains("window-button")).IsTrue();
    }

    [Test]
    public async Task RendersActionSpecificClass(CancellationToken ct = default) {
        // Arrange
        IRenderedComponent<InfiniFrameWindowButton> cut = Render<InfiniFrameWindowButton>(parameters =>
            parameters.Add(p => p.WindowAction, WindowAction.Close)
        );

        // Act
        IElement div = cut.Find("div");

        // Assert
        await Assert.That(div.ClassList.Contains("window-button-close")).IsTrue();
    }

    [Test]
    public async Task RendersPlatformSpecificClass(CancellationToken ct = default) {
        // Arrange
        IRenderedComponent<InfiniFrameWindowButton> cut = Render<InfiniFrameWindowButton>(parameters =>
            parameters.Add(p => p.WindowAction, WindowAction.Maximize)
        );

        // Act
        IElement div = cut.Find("div");
        string expectedPlatform = OperatingSystem.IsWindows() ? "windows"
            : OperatingSystem.IsMacOS() ? "macos"
            : OperatingSystem.IsLinux() ? "linux" : "unknown";

        // Assert
        await Assert.That(div.ClassList.Contains($"window-button-{expectedPlatform}")).IsTrue();
    }

    [Test]
    public async Task PassesClassParameter(CancellationToken ct = default) {
        // Arrange
        IRenderedComponent<InfiniFrameWindowButton> cut = Render<InfiniFrameWindowButton>(parameters =>
            parameters.Add(p => p.WindowAction, WindowAction.Minimize)
                      .Add(p => p.Class, "my-button")
        );

        // Act
        IElement div = cut.Find("div");

        // Assert
        await Assert.That(div.ClassList.Contains("my-button")).IsTrue();
    }

    [Test]
    public async Task RendersIconSpan(CancellationToken ct = default) {
        // Arrange
        IRenderedComponent<InfiniFrameWindowButton> cut = Render<InfiniFrameWindowButton>(parameters =>
            parameters.Add(p => p.WindowAction, WindowAction.Close)
        );

        // Act
        IElement span = cut.Find("span.window-icon");

        // Assert
        await Assert.That(span).IsNotNull();
    }

    [Test]
    public async Task RendersStyleTag(CancellationToken ct = default) {
        // Arrange
        IRenderedComponent<InfiniFrameWindowButton> cut = Render<InfiniFrameWindowButton>(parameters =>
            parameters.Add(p => p.WindowAction, WindowAction.Minimize)
        );

        // Act
        IElement style = cut.Find("style");

        // Assert
        await Assert.That(style).IsNotNull();
    }
}
