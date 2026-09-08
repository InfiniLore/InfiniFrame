// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Application;
using InfiniFrame.Security;
using InfiniFrame.Window.Features.WebMessaging.Handlers;

namespace InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniFrameApplicationWebServerExtensions {
    public static InfiniFrameApplicationBuilder UseWebServer(
        this InfiniFrameApplicationBuilder builder,
        Action<InfiniFrameWebServerConfiguration> configure
    ) => builder.UseWebServer("web", configure);

    public static InfiniFrameApplicationBuilder UseWebServer(
        this InfiniFrameApplicationBuilder builder,
        string windowId,
        Action<InfiniFrameWebServerConfiguration> configure
    ) {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(windowId);
        ArgumentNullException.ThrowIfNull(configure);

        var configuration = new InfiniFrameWebServerConfiguration(
            WebApplication.CreateBuilder(builder.Args),
            builder.Services
        );
        configure(configuration);

        builder.AddIntegration(application => {
            WebApplication webApplication = configuration.BuildApplication();
            string? startUrl = configuration.ConfiguredStartUrl;
            
            application.RegisterWindowConvention(windowBuilder => {
                if (startUrl is not null) windowBuilder.SetStartPageUrl(startUrl);
                
                windowBuilder.RegisterGetWebMessageHandler();
                
                if (startUrl is not null && Uri.TryCreate(startUrl, UriKind.Absolute, out Uri? baseUri))
                    InfiniFrameUriSecurityPolicyRegistry.ConfigureForBuilder(
                        windowBuilder,
                        configure: policyBuilder => policyBuilder.AddTrustedOrigin(baseUri)
                    );
            });

            application.RegisterStartupAction(() => webApplication.StartAsync());

            application.RegisterShutdownAction(async () => {
                await webApplication.StopAsync(CancellationToken.None).ConfigureAwait(false);
                await webApplication.DisposeAsync().ConfigureAwait(false);
            });
        });
        return builder;
    }
}
