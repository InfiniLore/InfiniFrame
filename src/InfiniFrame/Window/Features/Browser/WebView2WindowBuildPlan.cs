// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class WebView2WindowBuildPlan(WebView2WindowMode mode, string sharedProfileRoot) {
    private WebView2EnvironmentGroupStartupLease? _managedStartupLease;
    private WebView2WindowMode Mode { get; } = mode;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public bool ShouldRegisterAutoProfile(InfiniFrameWindowBuilder builder)
        => Mode == WebView2WindowMode.IsolatedPerWindow &&
           builder.Features.Browser is InfiniFrameWindowBuilderFeatureBrowser {
               TemporaryFilesPathExplicitlyAssigned: false
           };

    public void Apply(
        IInfiniFrameWindow window,
        InfiniFrameWindowBuilder builder,
        ref InfiniFrameNativeParameters parameters,
        ILogger logger
    ) {
        if (!OperatingSystem.IsWindows()) return;

        parameters.WebView2WindowMode = (int)Mode;
        if (Mode == WebView2WindowMode.IsolatedPerWindow) {
            return;
        }

        if (builder.Features.Browser is InfiniFrameWindowBuilderFeatureBrowser {
                TemporaryFilesPathExplicitlyAssigned: true
            }) {
            throw new InvalidOperationException(
                "ManagedShared WebView2 mode uses a shared environment profile root. " +
                "Do not set a per-window TemporaryFilesPath; use UseWebView2SharedEnvironmentProfileRoot instead.");
        }

        WebView2EnvironmentKey key = WebView2EnvironmentKey.Create(parameters, sharedProfileRoot);
        parameters.TemporaryFilesPath = key.ProfilePath;
        WebView2EnvironmentGroup group = WebView2WindowManager.RegisterWindowWithGroup(key, window.Id);

        try {
            group.ReserveRemoteDebugging(parameters.RemoteDebuggingPort, window.Id, logger);
            group.InitializeOrThrow(window.Id);
            _managedStartupLease = group.AcquireStartupLease(window.Id);
        }
        catch {
            WebView2WindowManager.ReleaseWindow(window);
            throw;
        }
    }

    public void Release() {
        _managedStartupLease?.Dispose();
        _managedStartupLease = null;
    }
}
