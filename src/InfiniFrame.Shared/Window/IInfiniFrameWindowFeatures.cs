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
    IDebuggingInfiniFrameWindowFeature Debugging { get; }

    /// <summary>
    ///     Gets the lifecycle feature, which manages window creation, closing, and cleanup.
    /// </summary>
    ILifecycleInfiniFrameWindowFeature Lifecycle { get; }

    /// <summary>
    ///     Gets the invoke feature for dispatching calls to the window's native thread.
    /// </summary>
    IInvokeInfiniFrameWindowFeature Invoke { get; }

    /// <summary>
    ///     Gets the web messaging feature for sending and receiving messages to and from the web view.
    /// </summary>
    IWebMessagingInfiniFrameWindowFeature WebMessaging { get; }

    /// <summary>
    ///     Gets the notifications feature for showing native desktop notifications.
    /// </summary>
    INotificationsInfiniFrameWindowFeature Notifications { get; }

    /// <summary>
    ///     Gets the file picker dialogs feature.
    /// </summary>
    IFilePickerDialogsInfiniFrameWindowFeature FilePickerDialogs { get; }

    /// <summary>
    ///     Gets the monitors feature for querying connected display information.
    /// </summary>
    IMonitorsInfiniFrameWindowFeature Monitors { get; }

    /// <summary>
    ///     Gets the page navigation feature.
    /// </summary>
    IPageNavigationInfiniFrameWindowFeature PageNavigation { get; }

    /// <summary>
    ///     Gets the position feature for getting or setting the window position.
    /// </summary>
    IPositionInfiniFrameWindowFeature Position { get; }

    /// <summary>
    ///     Gets the size feature for getting or setting the window size.
    /// </summary>
    ISizeInfiniFrameWindowFeature Size { get; }

    /// <summary>
    ///     Gets the decorations feature for controlling window chrome and borders.
    /// </summary>
    IDecorationsInfiniFrameWindowFeature Decorations { get; }

    /// <summary>
    ///     Gets the state feature for controlling window state such as minimized or maximized.
    /// </summary>
    IStateInfiniFrameWindowFeature State { get; }

    /// <summary>
    ///     Gets the browser feature for configuring web view behavior.
    /// </summary>
    IBrowserInfiniFrameWindowFeature Browser { get; }

    /// <summary>
    ///     Gets the drag and drop feature for handling file drop operations.
    /// </summary>
    IDragDropInfiniFrameWindowFeature DragDrop { get; }

    /// <summary>
    ///     Gets the taskbar feature for controlling taskbar progress indicators and icon flashing.
    /// </summary>
    ITaskbarInfiniFrameWindowFeature Taskbar { get; }
}