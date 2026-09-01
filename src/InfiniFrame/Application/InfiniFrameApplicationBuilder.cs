// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Builder for creating and configuring an <see cref="IInfiniFrameApplication"/>.
/// </summary>
public class InfiniFrameApplicationBuilder {
    /// <summary>
    ///     Gets the application configuration.
    /// </summary>
    public ApplicationConfiguration Configuration { get; } = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------

    /// <summary>
    ///     Creates a new <see cref="InfiniFrameApplicationBuilder"/>.
    /// </summary>
    /// <returns>A new builder instance.</returns>
    public static InfiniFrameApplicationBuilder Create() => new();

    /// <summary>
    ///     Builds and initializes the application using the configured settings.
    /// </summary>
    /// <param name="provider">Optional service provider. If null, a new provider with InfiniFrame services is created.</param>
    /// <returns>The initialized application.</returns>
    public IInfiniFrameApplication Build(IServiceProvider? provider = null) {
        IServiceProvider actualProvider = provider ?? new ServiceCollection()
            .AddLogging()
            .AddInfiniFrame()
            .BuildServiceProvider();

        var app = actualProvider.GetRequiredService<IInfiniFrameApplication>();
        ((InfiniFrameApplication)app).Initialize(Configuration);
        return app;
    }
}
