// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using FluentValidation;
using InfiniFrame.NativeBridge.Parameters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Creates <see cref="IInfiniFrameWindowFeatures" /> instances for a given window, resolving dependencies from a
///     <see cref="IServiceProvider" />.
/// </summary>
/// <param name="provider">The service provider used to resolve feature dependencies such as loggers and validators.</param>
public class InfiniFrameWindowFeaturesFactory(IServiceProvider provider) {
    private static ILogger<T> GetLogger<T>(IServiceProvider provider) {
        try {
            return provider.GetRequiredService<ILogger<T>>();
        }
        catch (InvalidOperationException ex) {
            throw new InvalidOperationException(
                $"Failed to resolve ILogger<{typeof(T).Name}> from the service provider. " +
                "Ensure that logging services are registered (e.g., builder.Services.AddLogging()).", ex);
        }
    }

    /// <summary>
    ///     Creates a complete set of window features for the specified window using the original builder configuration.
    /// </summary>
    /// <param name="window">The window for which to create features.</param>
    /// <param name="originalBuilder">The original builder used to configure the window.</param>
    /// <returns>An <see cref="IInfiniFrameWindowFeatures" /> instance with all feature implementations.</returns>
    public IInfiniFrameWindowFeatures Create(IInfiniFrameWindow window, IInfiniFrameWindowBuilder originalBuilder)
        => new InfiniFrameWindowFeatures(
            new DebuggingInfiniFrameWindowFeature(
                window,
                GetLogger<DebuggingInfiniFrameWindowFeature>(provider)
            ),
            new LifecycleInfiniFrameWindowFeature(
                window,
                GetLogger<LifecycleInfiniFrameWindowFeature>(provider),
                provider.GetRequiredService<IValidator<InfiniFrameNativeParameters>>()
            ),
            new InvokeInfiniFrameWindowFeature(
                window,
                GetLogger<InvokeInfiniFrameWindowFeature>(provider)
            ),
            new WebMessagingInfiniFrameWindowFeature(
                window,
                GetLogger<WebMessagingInfiniFrameWindowFeature>(provider)
            ),
            new NotificationsInfiniFrameWindowFeature(
                window,
                GetLogger<NotificationsInfiniFrameWindowFeature>(provider)
            ),
            new FilePickerDialogsInfiniFrameWindowFeature(
                window,
                GetLogger<FilePickerDialogsInfiniFrameWindowFeature>(provider)
            ),
            new MonitorsInfiniFrameWindowFeature(
                window,
                GetLogger<MonitorsInfiniFrameWindowFeature>(provider)
            ),
            new PageNavigationInfiniFrameWindowFeature(
                window,
                GetLogger<PageNavigationInfiniFrameWindowFeature>(provider),
                provider.GetService<IInfiniFrameStaticAssets>()
                ?? originalBuilder.StaticAssets?.DeepCopy()
            ),
            new PositionInfiniFrameWindowFeature(
                window,
                GetLogger<PositionInfiniFrameWindowFeature>(provider)
            ),
            new SizeInfiniFrameWindowFeature(
                window,
                GetLogger<SizeInfiniFrameWindowFeature>(provider)
            ),
            new DecorationsInfiniFrameWindowFeature(
                window,
                originalBuilder,
                GetLogger<DecorationsInfiniFrameWindowFeature>(provider)
            ),
            new StateInfiniFrameWindowFeature(
                window,
                GetLogger<StateInfiniFrameWindowFeature>(provider)
            ),
            new BrowserInfiniFrameWindowFeature(
                window,
                GetLogger<BrowserInfiniFrameWindowFeature>(provider)
            ),
            new DragDropInfiniFrameWindowFeature(
                window,
                GetLogger<DragDropInfiniFrameWindowFeature>(provider)
            ),
            new TaskbarInfiniFrameWindowFeature(
                window,
                GetLogger<TaskbarInfiniFrameWindowFeature>(provider)
            ),
            new MenuInfiniFrameWindowFeature(
                window,
                GetLogger<MenuInfiniFrameWindowFeature>(provider),
                originalBuilder.Features.Menu.MenuBar
            ),
            new JavaScriptInfiniFrameWindowFeature(
                window,
                GetLogger<JavaScriptInfiniFrameWindowFeature>(provider)
            )
        );
}
