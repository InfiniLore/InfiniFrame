// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using System.Collections.Concurrent;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Process-wide Windows WebView2 window manager defaults.
/// </summary>
public static class WebView2WindowManager {
    private static readonly ConcurrentDictionary<WebView2EnvironmentKey, WebView2EnvironmentGroup> Groups = new();
    private static readonly ConcurrentDictionary<Guid, WebView2EnvironmentGroup> WindowGroups = new();
    internal static readonly ConcurrentDictionary<int, WebView2RemoteDebuggingGroupReservation> SharedRemoteDebuggingReservations = new();
    
    #if NET9_0_OR_GREATER
    private static readonly Lock SharedGroupsLock = new();
    #else
    private static readonly object SharedGroupsLock = new();
    #endif
    
    internal static readonly SemaphoreSlim IsolatedInitializationGate = new(1, 1);
    internal static readonly Mutex IsolatedInitializationProcessGate = new(
        initiallyOwned: false,
        name: @"Local\InfiniFrame.WebView2.IsolatedInitialization"
    );
    private static readonly string DefaultSharedProfileRoot = Path.Combine(
        Path.GetTempPath(),
        "infiniframe",
        "webview2-shared",
        Environment.ProcessId.ToString()
    );

    private static int _defaultMode = (int)WebView2WindowMode.IsolatedPerWindow;
    private static string? _sharedEnvironmentProfileRoot;

    /// <summary>
    ///     Gets or sets the default WebView2 mode used by new Windows builders that do not set a per-builder mode.
    /// </summary>
    public static WebView2WindowMode DefaultMode {
        get => (WebView2WindowMode)Volatile.Read(ref _defaultMode);
        set => Volatile.Write(ref _defaultMode, (int)value);
    }

    /// <summary>
    ///     Gets or sets the process-wide shared profile root used by managed shared environments.
    /// </summary>
    public static string SharedEnvironmentProfileRoot {
        get => Volatile.Read(ref _sharedEnvironmentProfileRoot) ?? DefaultSharedProfileRoot;
        set {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            Volatile.Write(ref _sharedEnvironmentProfileRoot, value);
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------

    internal static IInfiniFrameWindow Build(InfiniFrameWindowBuilder builder, IServiceProvider provider)
        => builder.BuildCore(provider, CreatePlan(builder));

    internal static void ReleaseWindow(IInfiniFrameWindow window) {
        if (!OperatingSystem.IsWindows()) return;
        if (!WindowGroups.TryRemove(window.Id, out WebView2EnvironmentGroup? group)) return;

        group.Release(window.Id);
        if (group.ReferenceCount != 0) return;
        lock (SharedGroupsLock) {
            if (group.ReferenceCount != 0) return;
            Groups.TryRemove(new KeyValuePair<WebView2EnvironmentKey, WebView2EnvironmentGroup>(group.Key, group));
        }
        group.Dispose();
    }

    internal static bool IsManagedShared(InfiniFrameNativeParameters parameters)
        => parameters.WebView2WindowMode == (int)WebView2WindowMode.ManagedShared;

    internal static WebView2EnvironmentGroup RegisterWindowWithGroup(WebView2EnvironmentKey key, Guid windowId) {
        lock (SharedGroupsLock) {
            foreach (WebView2EnvironmentKey activeKey in Groups.Keys) {
                if (activeKey.Equals(key)) continue;
                if (!string.Equals(activeKey.ProfileRoot, key.ProfileRoot, StringComparison.OrdinalIgnoreCase)) continue;

                throw new InvalidOperationException(
                    "ManagedShared WebView2 mode cannot create windows with incompatible active environment settings. " +
                    $"Existing key: '{activeKey.Diagnostics}'. Requested key: '{key.Diagnostics}'.");
            }

            WebView2EnvironmentGroup group = Groups.GetOrAdd(
                key,
                static createdKey => new WebView2EnvironmentGroup(createdKey)
            );
            group.AddReference(windowId);
            if (WindowGroups.TryAdd(windowId, group)) return group;

            group.Release(windowId);
            throw new InvalidOperationException($"Window {windowId} is already registered with the WebView2 manager.");
        }
    }

    private static WebView2WindowBuildPlan CreatePlan(InfiniFrameWindowBuilder builder) {
        var browser = (InfiniFrameWindowBuilderFeatureBrowser)builder.Features.Browser;
        WebView2WindowMode mode = browser.WebView2ModeExplicitlyAssigned
            ? browser.WebView2Mode!.Value
            : DefaultMode;

        string profileRoot = browser.WebView2SharedEnvironmentProfileRootExplicitlyAssigned
            ? browser.WebView2SharedEnvironmentProfileRoot!
            : SharedEnvironmentProfileRoot;

        return new WebView2WindowBuildPlan(mode, profileRoot);
    }
}
