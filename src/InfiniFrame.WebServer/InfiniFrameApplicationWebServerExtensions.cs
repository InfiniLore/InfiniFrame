// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Application;
using InfiniFrame.Security;
using InfiniFrame.Window.Features.WebMessaging.Handlers;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniFrameApplicationWebServerExtensions {
    public static InfiniFrameApplicationBuilder UseWebServer(
        this InfiniFrameApplicationBuilder builder,
        Action<InfiniFrameWebServerConfiguration> configure
    ) => builder.UseWebServer(configure, []);

    public static InfiniFrameApplicationBuilder UseWebServer(
        this InfiniFrameApplicationBuilder builder,
        string windowId,
        Action<InfiniFrameWebServerConfiguration> configure
    ) => builder.UseWebServer(configure, windowId);

    public static InfiniFrameApplicationBuilder UseWebServer(
        this InfiniFrameApplicationBuilder builder,
        IEnumerable<string> windowIds,
        Action<InfiniFrameWebServerConfiguration> configure
    ) {
        ArgumentNullException.ThrowIfNull(windowIds);
        return builder.UseWebServer(configure, windowIds.ToArray());
    }

    public static InfiniFrameApplicationBuilder UseWebServer(
        this InfiniFrameApplicationBuilder builder,
        Action<InfiniFrameWebServerConfiguration> configure,
        params string[] windowIds
    ) {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        var configuration = new InfiniFrameWebServerConfiguration(
            WebApplication.CreateBuilder(builder.Args),
            builder.Services
        );
        configure(configuration);

        builder.AddIntegration(application => {
            application.ValidateWindowIntegrationTargets(windowIds, "WebServer");
            WebApplication webApplication = configuration.BuildApplication();
            application.RegisterStartupAction(async () => {
                await webApplication.StartAsync().ConfigureAwait(false);
                Uri address = ResolveStartedAddress(webApplication);
                application.ApplyWindowIntegration(windowIds, "WebServer", configure: windowBuilder => {
                    windowBuilder.SetStartPageUrl(address.ToString());
                    windowBuilder.RegisterGetWebMessageHandler();
                    InfiniFrameUriSecurityPolicyRegistry.ConfigureForBuilder(
                        windowBuilder,
                        configure: policyBuilder => policyBuilder.AddTrustedOrigin(address)
                    );
                });
            });

            application.RegisterShutdownAction(async () => {
                await webApplication.StopAsync(CancellationToken.None).ConfigureAwait(false);
                await webApplication.DisposeAsync().ConfigureAwait(false);
            });
        });
        return builder;
    }

    private static Uri ResolveStartedAddress(WebApplication application) {
        IEnumerable<string> addresses = application.Services
                .GetService<IServer>()?.Features
                .Get<IServerAddressesFeature>()?.Addresses
            ?? application.Urls;
        string? address = addresses.FirstOrDefault(candidate =>
            Uri.TryCreate(candidate, UriKind.Absolute, out Uri? uri) && uri.Port != 0);
        if (address is null || !Uri.TryCreate(address, UriKind.Absolute, out Uri? result))
            throw new InvalidOperationException(
                "The ASP.NET Core web server started but its bound address could not be resolved. " +
                "Refusing to navigate a window to an unresolved port.");

        return result;
    }
}
