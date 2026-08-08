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
public class InfiniFrameWindowButtonTests : TestContext {
    [Test]
    public async Task MinimizeButton_HasCorrectDataAttribute(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowButton> cut = RenderComponent<InfiniFrameWindowButton>(parameters =>
            parameters.Add(p => p.WindowAction, WindowAction.Minimize)
        );

        IElement div = cut.Find("div");
        await Assert.That(div.GetAttribute("data-infiniframe-window-action")).IsEqualTo("minimize");
    }

    [Test]
    public async Task MaximizeButton_HasCorrectDataAttribute(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowButton> cut = RenderComponent<InfiniFrameWindowButton>(parameters =>
            parameters.Add(p => p.WindowAction, WindowAction.Maximize)
        );

        IElement div = cut.Find("div");
        await Assert.That(div.GetAttribute("data-infiniframe-window-action")).IsEqualTo("maximize");
    }

    [Test]
    public async Task CloseButton_HasCorrectDataAttribute(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowButton> cut = RenderComponent<InfiniFrameWindowButton>(parameters =>
            parameters.Add(p => p.WindowAction, WindowAction.Close)
        );

        IElement div = cut.Find("div");
        await Assert.That(div.GetAttribute("data-infiniframe-window-action")).IsEqualTo("close");
    }

    [Test]
    public async Task RendersWindowButtonClass(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowButton> cut = RenderComponent<InfiniFrameWindowButton>(parameters =>
            parameters.Add(p => p.WindowAction, WindowAction.Minimize)
        );

        IElement div = cut.Find("div");
        await Assert.That(div.ClassList.Contains("window-button")).IsTrue();
    }

    [Test]
    public async Task RendersActionSpecificClass(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowButton> cut = RenderComponent<InfiniFrameWindowButton>(parameters =>
            parameters.Add(p => p.WindowAction, WindowAction.Close)
        );

        IElement div = cut.Find("div");
        await Assert.That(div.ClassList.Contains("window-button-close")).IsTrue();
    }

    [Test]
    public async Task RendersPlatformSpecificClass(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowButton> cut = RenderComponent<InfiniFrameWindowButton>(parameters =>
            parameters.Add(p => p.WindowAction, WindowAction.Maximize)
        );

        IElement div = cut.Find("div");
        string expectedPlatform = OperatingSystem.IsWindows() ? "windows"
            : OperatingSystem.IsMacOS() ? "macos"
            : OperatingSystem.IsLinux() ? "linux" : "unknown";
        await Assert.That(div.ClassList.Contains($"window-button-{expectedPlatform}")).IsTrue();
    }

    [Test]
    public async Task PassesClassParameter(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowButton> cut = RenderComponent<InfiniFrameWindowButton>(parameters =>
            parameters.Add(p => p.WindowAction, WindowAction.Minimize)
                      .Add(p => p.Class, "my-button")
        );

        IElement div = cut.Find("div");
        await Assert.That(div.ClassList.Contains("my-button")).IsTrue();
    }

    [Test]
    public async Task RendersIconSpan(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowButton> cut = RenderComponent<InfiniFrameWindowButton>(parameters =>
            parameters.Add(p => p.WindowAction, WindowAction.Close)
        );

        IElement span = cut.Find("span.window-icon");
        await Assert.That(span).IsNotNull();
    }

    [Test]
    public async Task RendersStyleTag(CancellationToken ct = default) {
        IRenderedComponent<InfiniFrameWindowButton> cut = RenderComponent<InfiniFrameWindowButton>(parameters =>
            parameters.Add(p => p.WindowAction, WindowAction.Minimize)
        );

        IElement style = cut.Find("style");
        await Assert.That(style).IsNotNull();
    }
}
