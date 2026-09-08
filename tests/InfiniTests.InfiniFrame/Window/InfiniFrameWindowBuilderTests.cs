using FluentValidation;
using InfiniFrame.Window;
using InfiniFrame.Window.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniTests.InfiniFrame.Window;

public sealed class InfiniFrameWindowBuilderTests {
    [Test]
    public async Task Build_InvalidParametersAreRejectedBeforeWindowResolution(CancellationToken ct = default) {
        var services = new ServiceCollection();
        services.AddLogging().AddInfiniFrame();
        services.AddTransient<InfiniFrameWindow>(_ =>
            throw new InvalidOperationException("The window should not be resolved before validation."));
        await using ServiceProvider provider = services.BuildServiceProvider();
        var builder = new InfiniFrameWindowBuilder();

        // ReSharper disable once AccessToDisposedClosure
        await Assert.That(() => builder.Build(provider)).Throws<ValidationException>();
    }
}
