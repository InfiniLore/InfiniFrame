// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using AngleSharp.Dom;
using Bunit;
using InfiniFrame.Blazor;
using TestContext = Bunit.TestContext;

namespace InfiniTests.InfiniFrame.Blazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowDragAreaTests : TestContext {
    [Test]
    public async Task RendersDragRegionAttribute(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowDragArea> cut = RenderComponent<InfiniFrameWindowDragArea>();

        IElement div = cut.Find("div");
        await Assert.That(div.HasAttribute("data-infiniframe-drag-region")).IsTrue();
    }

    [Test]
    public async Task RendersChildContent(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowDragArea> cut = RenderComponent<InfiniFrameWindowDragArea>(parameters =>
            parameters.AddChildContent("<span class='title'>My App</span>")
        );

        IElement span = cut.Find("span.title");
        await Assert.That(span.TextContent).IsEqualTo("My App");
    }

    [Test]
    public async Task PassesExtraAttributes(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowDragArea> cut = RenderComponent<InfiniFrameWindowDragArea>(parameters =>
            parameters.AddUnmatched("class", "my-drag-area")
                      .AddUnmatched("id", "titlebar")
        );

        IElement div = cut.Find("div");
        await Assert.That(div.GetAttribute("class")).IsEqualTo("my-drag-area");
        await Assert.That(div.GetAttribute("id")).IsEqualTo("titlebar");
    }

    [Test]
    public async Task CombinesExtraAttributesWithDragRegion(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowDragArea> cut = RenderComponent<InfiniFrameWindowDragArea>(parameters =>
            parameters.AddUnmatched("class", "custom")
                      .AddChildContent("Content")
        );

        IElement div = cut.Find("div");
        await Assert.That(div.HasAttribute("data-infiniframe-drag-region")).IsTrue();
        await Assert.That(div.GetAttribute("class")).IsEqualTo("custom");
        await Assert.That(div.InnerHtml.Trim()).IsEqualTo("Content");
    }

    [Test]
    public async Task RendersEmptyWhenNoChildContent(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowDragArea> cut = RenderComponent<InfiniFrameWindowDragArea>();

        IElement div = cut.Find("div");
        await Assert.That(div.ChildNodes.Length).IsEqualTo(0);
    }
}
