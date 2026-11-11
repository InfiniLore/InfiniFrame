// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// Default (non-hosted) windows: the IInfiniFrameWebViewManager members keep the framework's own semantics. The
// document-isolation correction re-implements them explicitly for hosted windows; these tests pass before and after it.
public class InfiniFrameWebViewManagerInterfaceTests {
    [Test]
    public async Task TryDispatch_WithANullWorkItem_FaultsTheReturnedTask_WithoutThrowingSynchronously() {
        await using ServiceProvider provider = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using InfiniFrameWebViewManager manager = Create(provider);
        IInfiniFrameWebViewManager facade = manager;
        Task<bool>? dispatched = null;
        Exception? synchronous = null;
        try {
            dispatched = facade.TryDispatchAsync(null!);
        }
        catch (Exception exception) {
            synchronous = exception;
        }
        await Assert.That(synchronous).IsNull();
        await Assert.That(dispatched is not null).IsTrue();
        await Assert.ThrowsAsync<ArgumentNullException>(() => dispatched!);
    }

    [Test]
    public async Task TryDispatch_WithoutAnAttachedPage_ReturnsFalse() {
        await using ServiceProvider provider = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using InfiniFrameWebViewManager manager = Create(provider);
        IInfiniFrameWebViewManager facade = manager;
        bool ran = false;
        bool dispatched = await facade.TryDispatchAsync(_ => ran = true);
        await Assert.That(dispatched).IsFalse();
        await Assert.That(ran).IsFalse();
    }

    [Test]
    public async Task AddRootComponent_TwiceForOneSelector_ThrowsAsTheFrameworkDoes() {
        await using ServiceProvider provider = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using InfiniFrameWebViewManager manager = Create(provider);
        IInfiniFrameWebViewManager facade = manager;
        await facade.AddRootComponentAsync(typeof(Probe), "#app", ParameterView.Empty);
        await Assert.ThrowsAsync<InvalidOperationException>(() => facade.AddRootComponentAsync(typeof(Probe), "#app", ParameterView.Empty));
    }

    [Test]
    public async Task RemoveRootComponent_ForAnUnknownSelector_ThrowsAsTheFrameworkDoes() {
        await using ServiceProvider provider = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using InfiniFrameWebViewManager manager = Create(provider);
        IInfiniFrameWebViewManager facade = manager;
        await Assert.ThrowsAsync<InvalidOperationException>(() => facade.RemoveRootComponentAsync("#missing"));
    }

    [Test]
    public async Task AddRemoveAndAddAgain_ThroughTheInterface_Succeeds() {
        await using ServiceProvider provider = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using InfiniFrameWebViewManager manager = Create(provider);
        IInfiniFrameWebViewManager facade = manager;
        await facade.AddRootComponentAsync(typeof(Probe), "#app", ParameterView.Empty);
        await facade.RemoveRootComponentAsync("#app");
        await facade.AddRootComponentAsync(typeof(Probe), "#app", ParameterView.Empty);
        await Assert.ThrowsAsync<InvalidOperationException>(() => facade.RemoveRootComponentAsync("#other"));
    }

    private static InfiniFrameWebViewManager Create(IServiceProvider provider) => new(
        provider,
        MockFactory.CreateDispatcherMock().Object,
        new NullFileProvider(),
        new JSComponentConfigurationStore(),
        Options.Create(new InfiniFrameBlazorAppConfiguration()),
        NullLogger<InfiniFrameWebViewManager>.Instance);

    public sealed class Probe : ComponentBase {}
}
