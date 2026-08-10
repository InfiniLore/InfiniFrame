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
            Debugging: new DebuggingInfiniFrameWindowFeature(
                window,
                GetLogger<DebuggingInfiniFrameWindowFeature>(provider)
            ),
            Lifecycle: new LifecycleInfiniFrameWindowFeature(
                window,
                GetLogger<LifecycleInfiniFrameWindowFeature>(provider),
                provider.GetRequiredService<IValidator<InfiniFrameNativeParameters>>()
            ),
            Invoke: new InvokeInfiniFrameWindowFeature(
                window,
                GetLogger<InvokeInfiniFrameWindowFeature>(provider)
            ),
            WebMessaging: new WebMessagingInfiniFrameWindowFeature(
                window,
                GetLogger<WebMessagingInfiniFrameWindowFeature>(provider)
            ),
            Notifications: new NotificationsInfiniFrameWindowFeature(
                window,
                GetLogger<NotificationsInfiniFrameWindowFeature>(provider)
            ),
            FilePickerDialogs: new FilePickerDialogsInfiniFrameWindowFeature(
                window,
                GetLogger<FilePickerDialogsInfiniFrameWindowFeature>(provider)
            ),
            Monitors: new MonitorsInfiniFrameWindowFeature(
                window,
                GetLogger<MonitorsInfiniFrameWindowFeature>(provider)
            ),
            PageNavigation: new PageNavigationInfiniFrameWindowFeature(
                window,
                GetLogger<PageNavigationInfiniFrameWindowFeature>(provider),
                provider.GetService<IInfiniFrameStaticAssets>()
                ?? originalBuilder.StaticAssets?.DeepCopy()
            ),
            Position: new PositionInfiniFrameWindowFeature(
                window,
                GetLogger<PositionInfiniFrameWindowFeature>(provider)
            ),
            Size: new SizeInfiniFrameWindowFeature(
                window,
                GetLogger<SizeInfiniFrameWindowFeature>(provider)
            ),
            Decorations: new DecorationsInfiniFrameWindowFeature(
                window,
                originalBuilder,
                GetLogger<DecorationsInfiniFrameWindowFeature>(provider)
            ),
            State: new StateInfiniFrameWindowFeature(
                window,
                GetLogger<StateInfiniFrameWindowFeature>(provider)
            ),
            Browser: new BrowserInfiniFrameWindowFeature(
                window,
                GetLogger<BrowserInfiniFrameWindowFeature>(provider)
            ),
            DragDrop: new DragDropInfiniFrameWindowFeature(
                window,
                GetLogger<DragDropInfiniFrameWindowFeature>(provider)
            ),
            Taskbar: new TaskbarInfiniFrameWindowFeature(
                window,
                GetLogger<TaskbarInfiniFrameWindowFeature>(provider)
            ),
            Menu: new MenuInfiniFrameWindowFeature(
                window,
                GetLogger<MenuInfiniFrameWindowFeature>(provider),
                originalBuilder.Features.Menu?.MenuBar
            ),
            JavaScript: new JavaScriptInfiniFrameWindowFeature(
                window,
                GetLogger<JavaScriptInfiniFrameWindowFeature>(provider)
            )
        );
}