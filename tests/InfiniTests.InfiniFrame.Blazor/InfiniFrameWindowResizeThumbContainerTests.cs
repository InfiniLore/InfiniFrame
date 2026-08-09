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
public class InfiniFrameWindowResizeThumbContainerTests : BunitContext {
    [Test]
    public async Task RendersAllEightResizeThumbs(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowResizeThumbContainer> cut = Render<InfiniFrameWindowResizeThumbContainer>();

        IReadOnlyList<IElement> thumbs = cut.FindAll("div[data-infiniframe-resize]");
        await Assert.That(thumbs.Count).IsEqualTo(8);
    }

    [Test]
    public async Task RendersAllEightDirections(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowResizeThumbContainer> cut = Render<InfiniFrameWindowResizeThumbContainer>();

        string[] expectedDirections = ["top", "right", "bottom", "left", "top-left", "top-right", "bottom-right", "bottom-left"];
        IReadOnlyList<IElement> thumbs = cut.FindAll("div[data-infiniframe-resize]");

        foreach (string direction in expectedDirections) {
            IElement? matching = thumbs.FirstOrDefault(t => t.GetAttribute("data-infiniframe-resize") == direction);
            await Assert.That(matching).IsNotNull();
        }
    }

    [Test]
    public async Task PassesCustomZIndex(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowResizeThumbContainer> cut = Render<InfiniFrameWindowResizeThumbContainer>(parameters =>
            parameters.Add(p => p.ZIndex, 500)
        );

        IReadOnlyList<IElement> thumbs = cut.FindAll("div[data-infiniframe-resize]");
        foreach (IElement thumb in thumbs) {
            string style = thumb.GetAttribute("style") ?? "";
            await Assert.That(style).Contains("z-index: 500");
        }
    }

    [Test]
    public async Task PassesCustomResizeArea(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowResizeThumbContainer> cut = Render<InfiniFrameWindowResizeThumbContainer>(parameters =>
            parameters.Add(p => p.ResizeArea, 15)
        );

        IElement topThumb = cut.Find("div[data-infiniframe-resize='top']");
        string style = topThumb.GetAttribute("style") ?? "";
        await Assert.That(style).Contains("height: 15px");
    }
}
