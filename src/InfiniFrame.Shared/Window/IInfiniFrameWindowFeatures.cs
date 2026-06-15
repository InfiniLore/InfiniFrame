// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides access to all features available on an InfiniFrame window.
/// </summary>
public interface IInfiniFrameWindowFeatures {
    /// <summary>
    ///     Gets the debugging feature.
    /// </summary>
    IInfiniFrameWindowFeatureDebugging Debugging { get; }

    /// <summary>
    ///     Gets the lifecycle feature, which manages window creation, closing, and cleanup.
    /// </summary>
    IInfiniFrameWindowFeatureLifecycle Lifecycle { get; }

    /// <summary>
    ///     Gets the invoke feature for dispatching calls to the window's native thread.
    /// </summary>
    IInfiniFrameWindowFeatureInvoke Invoke { get; }

    /// <summary>
    ///     Gets the web messaging feature for sending and receiving messages to and from the web view.
    /// </summary>
    IInfiniFrameWindowFeatureWebMessaging WebMessaging { get; }

    /// <summary>
    ///     Gets the notifications feature for showing native desktop notifications.
    /// </summary>
    IInfiniFrameWindowFeatureNotifications Notifications { get; }

    /// <summary>
    ///     Gets the file picker dialogs feature.
    /// </summary>
    IInfiniFrameWindowFeatureFilePickerDialogs FilePickerDialogs { get; }

    /// <summary>
    ///     Gets the monitors feature for querying connected display information.
    /// </summary>
    IInfiniFrameWindowFeatureMonitors Monitors { get; }

    /// <summary>
    ///     Gets the page navigation feature.
    /// </summary>
    IInfiniFrameWindowFeaturePageNavigation PageNavigation { get; }

    /// <summary>
    ///     Gets the position feature for getting or setting the window position.
    /// </summary>
    IInfiniFrameWindowFeaturePosition Position { get; }

    /// <summary>
    ///     Gets the size feature for getting or setting the window size.
    /// </summary>
    IInfiniFrameWindowFeatureSize Size { get; }

    /// <summary>
    ///     Gets the decorations feature for controlling window chrome and borders.
    /// </summary>
    IInfiniFrameWindowFeatureDecorations Decorations { get; }

    /// <summary>
    ///     Gets the state feature for controlling window state such as minimized or maximized.
    /// </summary>
    IInfiniFrameWindowFeatureState State { get; }

    /// <summary>
    ///     Gets the browser feature for configuring web view behavior.
    /// </summary>
    IInfiniFrameWindowFeatureBrowser Browser { get; }
}
