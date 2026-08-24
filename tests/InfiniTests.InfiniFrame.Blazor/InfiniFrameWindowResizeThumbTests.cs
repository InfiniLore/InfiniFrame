// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using AngleSharp.Dom;
using Bunit;
using InfiniFrame;
using InfiniFrame.Blazor;

namespace InfiniTests.InfiniFrame.Blazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowResizeThumbTests : BunitContext {

    [Test]
    [Arguments(ResizeOrigin.Top, "top")]
    [Arguments(ResizeOrigin.Right, "right")]
    [Arguments(ResizeOrigin.Bottom, "bottom")]
    [Arguments(ResizeOrigin.Left, "left")]
    [Arguments(ResizeOrigin.TopLeft, "top-left")]
    [Arguments(ResizeOrigin.TopRight, "top-right")]
    [Arguments(ResizeOrigin.BottomRight, "bottom-right")]
    [Arguments(ResizeOrigin.BottomLeft, "bottom-left")]
    public async Task HasCorrectDataAttribute(ResizeOrigin origin, string expectedAttribute, CancellationToken ct = default) {
        // Arrange
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = Render<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(parameterSelector: p => p.ResizeThumb, origin)
        );

        // Act
        IElement div = cut.Find("div");

        // Assert
        await Assert.That(div.GetAttribute("data-infiniframe-resize")).IsEqualTo(expectedAttribute);
    }

    [Test]
    public async Task Thumb_HasPositionAbsoluteStyle(CancellationToken ct = default) {
        // Arrange
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = Render<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(parameterSelector: p => p.ResizeThumb, ResizeOrigin.Top)
        );

        // Act
        IElement div = cut.Find("div");
        string style = div.GetAttribute("style") ?? "";

        // Assert
        await Assert.That(style).Contains("position: absolute");
    }

    [Test]
    public async Task Thumb_HasCorrectZIndex(CancellationToken ct = default) {
        // Arrange
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = Render<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(parameterSelector: p => p.ResizeThumb, ResizeOrigin.Right)
                .Add(parameterSelector: p => p.ZIndex, 500)
        );

        // Act
        IElement div = cut.Find("div");
        string style = div.GetAttribute("style") ?? "";

        // Assert
        await Assert.That(style).Contains("z-index: 500");
    }

    [Test]
    [Arguments(ResizeOrigin.TopLeft, "nw-resize")]
    [Arguments(ResizeOrigin.Top, "n-resize")]
    [Arguments(ResizeOrigin.TopRight, "ne-resize")]
    [Arguments(ResizeOrigin.Right, "e-resize")]
    [Arguments(ResizeOrigin.BottomRight, "se-resize")]
    [Arguments(ResizeOrigin.Bottom, "s-resize")]
    [Arguments(ResizeOrigin.BottomLeft, "sw-resize")]
    [Arguments(ResizeOrigin.Left, "w-resize")]
    public async Task Thumb_HasCorrectCursorForOrigin(ResizeOrigin origin, string expectedCursor, CancellationToken ct = default) {
        // Arrange
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = Render<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(parameterSelector: p => p.ResizeThumb, origin)
        );

        // Act
        IElement div = cut.Find("div");
        string style = div.GetAttribute("style") ?? "";

        // Assert
        await Assert.That(style).Contains($"cursor: {expectedCursor}");
    }

    [Test]
    public async Task Thumb_HasDefaultResizeArea(CancellationToken ct = default) {
        // Arrange
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = Render<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(parameterSelector: p => p.ResizeThumb, ResizeOrigin.Top)
        );

        // Act
        IElement div = cut.Find("div");
        string style = div.GetAttribute("style") ?? "";

        // Assert
        await Assert.That(style).Contains("height: 10px");
    }

    [Test]
    [Arguments(10, "height: 10px")]
    [Arguments(20, "height: 20px")]
    [Arguments(5, "height: 5px")]
    public async Task Thumb_UsesCustomResizeArea(int resizeArea, string expectedStyle, CancellationToken ct = default) {
        // Arrange
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = Render<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(parameterSelector: p => p.ResizeThumb, ResizeOrigin.Top)
                .Add(parameterSelector: p => p.ResizeArea, resizeArea)
        );

        // Act
        IElement div = cut.Find("div");
        string style = div.GetAttribute("style") ?? "";

        // Assert
        await Assert.That(style).Contains(expectedStyle);
    }

    [Test]
    public async Task Thumb_HasDefaultZIndex(CancellationToken ct = default) {
        // Arrange
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = Render<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(parameterSelector: p => p.ResizeThumb, ResizeOrigin.Left)
        );

        // Act
        IElement div = cut.Find("div");
        string style = div.GetAttribute("style") ?? "";

        // Assert
        await Assert.That(style).Contains("z-index: 1000");
    }
}
