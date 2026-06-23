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
    private static ILogger<T> GetLogger<T>(IServiceProvider provider) => provider.GetRequiredService<ILogger<T>>();
        
    /// <summary>
    ///     Creates a complete set of window features for the specified window using the original builder configuration.
    /// </summary>
    /// <param name="window">The window for which to create features.</param>
    /// <param name="originalBuilder">The original builder used to configure the window.</param>
    /// <returns>An <see cref="IInfiniFrameWindowFeatures" /> instance with all feature implementations.</returns>
    public IInfiniFrameWindowFeatures Create(IInfiniFrameWindow window, IInfiniFrameWindowBuilder originalBuilder) 
        => new InfiniFrameWindowFeatures(
            Debugging: new InfiniFrameWindowFeatureDebugging(
                window,
                GetLogger<InfiniFrameWindowFeatureDebugging>(provider)
            ),
            Lifecycle: new InfiniFrameWindowFeatureLifecycle(
                window,
                GetLogger<InfiniFrameWindowFeatureLifecycle>(provider),
                provider.GetRequiredService<IValidator<InfiniFrameNativeParameters>>()
            ),
            Invoke: new InfiniFrameWindowFeatureInvoke(
                window,
                GetLogger<InfiniFrameWindowFeatureInvoke>(provider)
            ),
            WebMessaging: new InfiniFrameWindowFeatureWebMessaging(
                window,
                GetLogger<InfiniFrameWindowFeatureWebMessaging>(provider)
            ),
            Notifications: new InfiniFrameWindowFeatureNotifications(
                window,
                GetLogger<InfiniFrameWindowFeatureNotifications>(provider)
            ),
            FilePickerDialogs: new InfiniFrameWindowFeatureFilePickerDialogs(
                window,
                GetLogger<InfiniFrameWindowFeatureFilePickerDialogs>(provider)
            ),
            Monitors: new InfiniFrameWindowFeatureMonitors(
                window,
                GetLogger<InfiniFrameWindowFeatureMonitors>(provider)
            ),
            PageNavigation: new InfiniFrameWindowFeaturePageNavigation(
                window,
                GetLogger<InfiniFrameWindowFeaturePageNavigation>(provider),
                provider.GetService<IInfiniFrameStaticAssets>()
                ?? originalBuilder.StaticAssets?.DeepCopy()
            ),
            Position: new InfiniFrameWindowFeaturePosition(
                window,
                GetLogger<InfiniFrameWindowFeaturePosition>(provider)
            ),
            Size: new InfiniFrameWindowFeatureSize(
                window,
                GetLogger<InfiniFrameWindowFeatureSize>(provider)
            ), 
            Decorations: new InfiniFrameWindowFeatureDecorations(
                window,
                originalBuilder,
                GetLogger<InfiniFrameWindowFeatureDecorations>(provider)
            ),
            State: new InfiniFrameWindowFeatureState(
                window,
                GetLogger<InfiniFrameWindowFeatureState>(provider)
            ),
            Browser: new InfiniFrameWindowFeatureBrowser(
                window,
                GetLogger<InfiniFrameWindowFeatureBrowser>(provider)
            )
        );
}
