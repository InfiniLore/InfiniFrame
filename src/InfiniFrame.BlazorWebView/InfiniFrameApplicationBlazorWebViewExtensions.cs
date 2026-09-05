using InfiniFrame;

namespace InfiniFrame.BlazorWebView;

/// <summary>Application-first integration for in-process Blazor WebView applications.</summary>
public static class InfiniFrameApplicationBlazorWebViewExtensions {
    /// <summary>
    ///     Adds an in-process Blazor WebView and registers its native window with the application owner.
    /// </summary>
    public static InfiniFrameApplication WithBlazorWebView(
        this InfiniFrameApplication application,
        Action<InfiniFrameBlazorAppBuilder> configure
    ) {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentNullException.ThrowIfNull(configure);

        InfiniFrameBlazorAppBuilder builder = InfiniFrameBlazorAppBuilder.CreateDefault();
        configure(builder);
        InfiniFrameBlazorApp blazorApp = builder.Build();
        blazorApp.InitializeForApplication();

        application.RegisterWindowBuilder(
            "blazor",
            (InfiniFrameWindowBuilder)builder.WindowBuilder,
            blazorApp.ServiceProvider
        );
        application.RegisterShutdownAction(() => blazorApp.DisposeAsync().AsTask());
        return application;
    }
}
