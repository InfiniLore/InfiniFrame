// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using AngleSharp.Dom;
using Bunit;
using InfiniFrame;
using InfiniFrame.Blazor;
using TestContext = Bunit.TestContext;

namespace InfiniTests.InfiniFrame.Blazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowResizeThumbTests : TestContext {
    [Test]
    public async Task TopThumb_HasCorrectDataAttribute(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = RenderComponent<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(p => p.ResizeThumb, ResizeOrigin.Top)
        );

        IElement div = cut.Find("div");
        await Assert.That(div.GetAttribute("data-infiniframe-resize")).IsEqualTo("top");
    }

    [Test]
    public async Task RightThumb_HasCorrectDataAttribute(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = RenderComponent<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(p => p.ResizeThumb, ResizeOrigin.Right)
        );

        IElement div = cut.Find("div");
        await Assert.That(div.GetAttribute("data-infiniframe-resize")).IsEqualTo("right");
    }

    [Test]
    public async Task BottomThumb_HasCorrectDataAttribute(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = RenderComponent<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(p => p.ResizeThumb, ResizeOrigin.Bottom)
        );

        IElement div = cut.Find("div");
        await Assert.That(div.GetAttribute("data-infiniframe-resize")).IsEqualTo("bottom");
    }

    [Test]
    public async Task LeftThumb_HasCorrectDataAttribute(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = RenderComponent<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(p => p.ResizeThumb, ResizeOrigin.Left)
        );

        IElement div = cut.Find("div");
        await Assert.That(div.GetAttribute("data-infiniframe-resize")).IsEqualTo("left");
    }

    [Test]
    public async Task TopLeftThumb_HasCorrectDataAttribute(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = RenderComponent<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(p => p.ResizeThumb, ResizeOrigin.TopLeft)
        );

        IElement div = cut.Find("div");
        await Assert.That(div.GetAttribute("data-infiniframe-resize")).IsEqualTo("top-left");
    }

    [Test]
    public async Task BottomRightThumb_HasCorrectDataAttribute(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = RenderComponent<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(p => p.ResizeThumb, ResizeOrigin.BottomRight)
        );

        IElement div = cut.Find("div");
        await Assert.That(div.GetAttribute("data-infiniframe-resize")).IsEqualTo("bottom-right");
    }

    [Test]
    public async Task Thumb_HasPositionAbsoluteStyle(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = RenderComponent<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(p => p.ResizeThumb, ResizeOrigin.Top)
        );

        IElement div = cut.Find("div");
        string style = div.GetAttribute("style") ?? "";
        await Assert.That(style).Contains("position: absolute");
    }

    [Test]
    public async Task Thumb_HasCorrectZIndex(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = RenderComponent<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(p => p.ResizeThumb, ResizeOrigin.Right)
                      .Add(p => p.ZIndex, 500)
        );

        IElement div = cut.Find("div");
        string style = div.GetAttribute("style") ?? "";
        await Assert.That(style).Contains("z-index: 500");
    }

    [Test]
    public async Task Thumb_HasCorrectCursor(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = RenderComponent<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(p => p.ResizeThumb, ResizeOrigin.BottomRight)
        );

        IElement div = cut.Find("div");
        string style = div.GetAttribute("style") ?? "";
        await Assert.That(style).Contains("cursor: se-resize");
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
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = RenderComponent<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(p => p.ResizeThumb, origin)
        );

        IElement div = cut.Find("div");
        string style = div.GetAttribute("style") ?? "";
        await Assert.That(style).Contains($"cursor: {expectedCursor}");
    }

    [Test]
    public async Task Thumb_HasDefaultResizeArea(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = RenderComponent<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(p => p.ResizeThumb, ResizeOrigin.Top)
        );

        IElement div = cut.Find("div");
        string style = div.GetAttribute("style") ?? "";
        await Assert.That(style).Contains("height: 10px");
    }

    [Test]
    public async Task Thumb_UsesCustomResizeArea(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = RenderComponent<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(p => p.ResizeThumb, ResizeOrigin.Top)
                      .Add(p => p.ResizeArea, 20)
        );

        IElement div = cut.Find("div");
        string style = div.GetAttribute("style") ?? "";
        await Assert.That(style).Contains("height: 20px");
    }

    [Test]
    public async Task Thumb_HasDefaultZIndex(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowResizeThumb> cut = RenderComponent<InfiniFrameWindowResizeThumb>(parameters =>
            parameters.Add(p => p.ResizeThumb, ResizeOrigin.Left)
        );

        IElement div = cut.Find("div");
        string style = div.GetAttribute("style") ?? "";
        await Assert.That(style).Contains("z-index: 1000");
    }
}
