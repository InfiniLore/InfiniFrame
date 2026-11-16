// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Application;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniFrameApplicationBlazorWebViewExtensions {
    extension(InfiniFrameApplicationBuilder builder) {
        public InfiniFrameApplicationBuilder WithBlazorWebView(
            Action<InfiniFrameBlazorWebViewConfiguration> configure
        ) => builder.UseBlazorWebView(configure);

        public InfiniFrameApplicationBuilder WithBlazorWebView(
            string windowId,
            Action<InfiniFrameBlazorWebViewConfiguration> configure
        ) => builder.UseBlazorWebView(windowId, configure);

        public InfiniFrameApplicationBuilder WithBlazorWebView(
            IEnumerable<string> windowIds,
            Action<InfiniFrameBlazorWebViewConfiguration> configure
        ) => builder.UseBlazorWebView(windowIds, configure);

        public InfiniFrameApplicationBuilder UseBlazorWebView(
            Action<InfiniFrameBlazorWebViewConfiguration> configure
        ) => builder.UseBlazorWebView(configure, []);

        public InfiniFrameApplicationBuilder UseBlazorWebView(
            string windowId,
            Action<InfiniFrameBlazorWebViewConfiguration> configure
        ) => builder.UseBlazorWebView(configure, windowId);

        public InfiniFrameApplicationBuilder UseBlazorWebView(
            IEnumerable<string> windowIds,
            Action<InfiniFrameBlazorWebViewConfiguration> configure
        ) {
            ArgumentNullException.ThrowIfNull(windowIds);
            return builder.UseBlazorWebView(configure, windowIds.ToArray());
        }

        public InfiniFrameApplicationBuilder UseBlazorWebView(
            Action<InfiniFrameBlazorWebViewConfiguration> configure,
            params string[] windowIds
        ) {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(configure);
            ArgumentNullException.ThrowIfNull(windowIds);
            if (windowIds.Length > 1)
                throw new NotSupportedException("BlazorWebView can currently target only one window.");
            if (builder.Services.Any(static sd => sd.ServiceType == typeof(BlazorWebViewMarker)))
                throw new InvalidOperationException(
                    "UseBlazorWebView has already been registered on this application builder. " +
                    "Multiple Blazor WebView integrations are not supported. " +
                    "Register a single Blazor WebView and target it at the desired window.");

            var configuration = new InfiniFrameBlazorWebViewConfiguration(builder.Services);
            configure(configuration);
            builder.Services.AddSingleton<IInfiniFrameWindow>(provider =>
                ResolveTargetWindow(provider.GetRequiredService<IInfiniFrameApplication>(), windowIds));
            builder.AddIntegration(application => {
                application.ValidateWindowIntegrationTargets(windowIds, "BlazorWebView");
                configuration.Apply(application, windowIds);
            });
            return builder;
        }
    }

    private static IInfiniFrameWindow ResolveTargetWindow(IInfiniFrameApplication application, IReadOnlyList<string> windowIds)
        => windowIds.Count > 0
            ? application.GetWindow(windowIds[0])
            : application.Windows.Count == 1
                ? application.Windows[0]
                : throw new InvalidOperationException("BlazorWebView could not resolve its target window.");
}
