// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using System.Reflection;

namespace InfiniTests.InfiniFrame.Window.Features.WebMessaging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowFeatureParityTests {
    private static readonly IReadOnlyDictionary<Type, FeatureMembers> Manifest
        = new Dictionary<Type, FeatureMembers> {
            [typeof(IBrowserInfiniFrameWindowFeature)] = Included(
                "IsContextMenuEnabled",
                "IsMediaAutoplayEnabled",
                "UserAgent",
                "IsFileSystemAccessEnabled",
                "IsWebSecurityEnabled",
                "IsJavascriptClipboardAccessEnabled",
                "IsMediaStreamEnabled",
                "IsIgnoreCertificateErrorsEnabled",
                "GrantBrowserPermissions",
                "IsSmoothScrollingEnabled",
                "IsStatusBarEnabled",
                "BrowserControlInitParameters",
                "EnableContextMenu",
                "EnableMediaAutoplay",
                "EnableStatusBar",
                "SetUserAgent",
                "Win32SetWebView2Path",
                "ClearBrowserAutoFill"
            ),
            [typeof(IDebuggingInfiniFrameWindowFeature)] = Included(
                "IsDevToolsEnabled",
                "SupportsWebInspectorAttach",
                "IsWebInspectorEnabled",
                "SupportsRemoteDebuggingEndpoint",
                "RemoteDebuggingPort",
                "Capabilities",
                "EnableDevTools",
                "GetDiagnostics",
                "TryGetRemoteDebuggingEndpoint",
                "TryProbeEndpoint"
            ),
            [typeof(IDecorationsInfiniFrameWindowFeature)] = Included(
                "IsChromeless",
                "IsTransparent",
                "BackgroundColor",
                "Title",
                "IconFilePath",
                "LimitLinuxWindowTitleLength",
                "SetTransparent",
                "SetBackgroundColor",
                "SetTitle",
                "SetIconFile",
                "SetLimitLinuxWindowTitleLength"
            ),
            [typeof(IFilePickerDialogsInfiniFrameWindowFeature)] = Included(
                "ShowOpenFile",
                "ShowOpenFileAsync",
                "ShowOpenFolder",
                "ShowOpenFolderAsync",
                "ShowSaveFile",
                "ShowSaveFileAsync"
            ),
            [typeof(IInvokeInfiniFrameWindowFeature)] = Excluded(
                ("Invoke", "Managed Action delegates cannot cross web messaging."),
                ("DispatchAsync", "Managed Action delegates cannot cross web messaging.")
            ),
            [typeof(ILifecycleInfiniFrameWindowFeature)] = IncludedAndExcluded(
                ["State", "Close", "CloseAsync", "IsClosedOrClosing"],
                ("WaitForClose", "Blocking the web-message/UI thread would deadlock."),
                ("WaitForCloseAsync", "A JS wait requires a future event-backed Promise."),
                ("WaitForReadyAsync", "Readiness is already represented by the JS handshake."),
                ("WaitForClosedCallbacksAsync", "Managed callback delivery is not a browser feature."),
                ("WaitForTeardownAsync", "Backend teardown is a managed/native lifetime concern.")
            ),
            [typeof(IMonitorsInfiniFrameWindowFeature)] = Included(
                "GetMonitors",
                "GetMainMonitor",
                "GetMainMonitorScreenDpi"
            ),
            [typeof(INotificationsInfiniFrameWindowFeature)] = Included(
                "ShowNotification",
                "ShowMessage",
                "ShowMessageAsync"
            ),
            [typeof(IPageNavigationInfiniFrameWindowFeature)] = IncludedWithCounts(
                ("Load", 2),
                ("LoadAsync", 2),
                ("TryLoadUri", 1),
                ("TryLoadPath", 1),
                ("LoadRawString", 1),
                ("LoadRawStringAsync", 1),
                ("GetCurrentUrl", 1),
                ("GetCurrentUri", 1)
            ),
            [typeof(IPositionInfiniFrameWindowFeature)] = IncludedWithCounts(
                ("Location", 1),
                ("Top", 1),
                ("Left", 1),
                ("SetLocation", 2),
                ("SetLeft", 1),
                ("SetTop", 1),
                ("Offset", 3),
                ("Center", 1),
                ("CenterOnCurrentMonitor", 1),
                ("CenterOnMonitor", 1),
                ("MoveWithinCurrentMonitorArea", 3)
            ),
            [typeof(ISizeInfiniFrameWindowFeature)] = IncludedWithCounts(
                ("Size", 1),
                ("Height", 1),
                ("Width", 1),
                ("MaxSize", 1),
                ("MaxHeight", 1),
                ("MaxWidth", 1),
                ("MinSize", 1),
                ("MinHeight", 1),
                ("MinWidth", 1),
                ("IsResizable", 1),
                ("SetSize", 2),
                ("SetHeight", 1),
                ("SetMaxSize", 2),
                ("SetMaxHeight", 1),
                ("SetMaxWidth", 1),
                ("SetMinSize", 2),
                ("SetMinHeight", 1),
                ("SetMinWidth", 1),
                ("SetWidth", 1),
                ("Resize", 1),
                ("SetResizable", 1)
            ),
            [typeof(IStateInfiniFrameWindowFeature)] = Included(
                "IsFullScreen",
                "IsMaximized",
                "IsMinimized",
                "IsTopMost",
                "IsFocused",
                "ZoomFactor",
                "IsZoomEnabled",
                "CachedPreFullScreenBounds",
                "CachedPreMaximizedBounds",
                "SetMaximized",
                "ToggleMaximized",
                "SetMinimized",
                "SetFullScreen",
                "SetFocused",
                "SetZoomFactor",
                "EnableZoom",
                "SetTopMost"
            ),
            [typeof(IWebMessagingInfiniFrameWindowFeature)] = Included(
                "SendWebMessage",
                "SendWebMessageAsync",
                "SendWebMessageWithAcknowledgementAsync"
            )
        };

    [Test]
    public async Task EveryPublicFeatureMember_IsRepresentedOrHasAnExplicitExclusion() {
        // Arrange, Act & Assert
        foreach ((Type featureType, FeatureMembers expected) in Manifest) {
            Dictionary<string, int> actual = featureType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Cast<MemberInfo>()
                .Concat(featureType.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(method => !method.IsSpecialName))
                .GroupBy(member => member.Name)
                .ToDictionary(
                keySelector: group => group.Key,
                elementSelector: group => group.Count(), StringComparer.Ordinal);
            Dictionary<string, int> audited = expected.Included
                .Concat(expected.Excluded.Keys.Select(name => new KeyValuePair<string, int>(name, 1)))
                .ToDictionary(
                keySelector: pair => pair.Key,
                elementSelector: pair => pair.Value, StringComparer.Ordinal);

            await Assert.That(actual).IsEquivalentTo(audited);
            await Assert.That(expected.Excluded.Values.All(reason => !string.IsNullOrWhiteSpace(reason))).IsTrue();
        }
    }

    private static FeatureMembers Included(params string[] names)
        => new(names.ToDictionary(
            keySelector: name => name,
            elementSelector: _ => 1, StringComparer.Ordinal), new Dictionary<string, string>()
        );

    private static FeatureMembers IncludedWithCounts(params (string Name, int Count)[] members)
        => new(members.ToDictionary(
            keySelector: member => member.Name,
            elementSelector: member => member.Count, StringComparer.Ordinal), new Dictionary<string, string>()
        );

    private static FeatureMembers Excluded(params (string Name, string Reason)[] members)
        => new(new Dictionary<string, int>(), members.ToDictionary(
            keySelector: member => member.Name,
            elementSelector: member => member.Reason, StringComparer.Ordinal)
        );

    private static FeatureMembers IncludedAndExcluded(string[] included, params (string Name, string Reason)[] excluded)
        => new(
            included.ToDictionary(
            keySelector: name => name,
            elementSelector: _ => 1, StringComparer.Ordinal),
            excluded.ToDictionary(
            keySelector: member => member.Name,
            elementSelector: member => member.Reason, StringComparer.Ordinal)
        );
}