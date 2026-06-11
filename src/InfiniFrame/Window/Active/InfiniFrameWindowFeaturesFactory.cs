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
public class InfiniFrameWindowFeaturesFactory(IServiceProvider provider) {
    private static ILogger<T> GetLogger<T>(IServiceProvider provider) => provider.GetRequiredService<ILogger<T>>();
        
    public IInfiniFrameWindowFeatures Create(IInfiniFrameWindow window, IInfiniFrameWindowBuilder originalBuilder) 
        => new InfiniFrameWindowFeatures(
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
