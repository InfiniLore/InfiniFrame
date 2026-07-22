// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable InvalidXmlDocComment

/// <summary>
///     Aggregates all feature instances available for an <see cref="IInfiniFrameWindow" />.
/// </summary>
public sealed record InfiniFrameWindowFeatures(
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.Debugging"/>
    IDebuggingInfiniFrameWindowFeature Debugging,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.Lifecycle"/>
    ILifecycleInfiniFrameWindowFeature Lifecycle,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.Invoke"/>
    IInvokeInfiniFrameWindowFeature Invoke,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.WebMessaging"/>
    IWebMessagingInfiniFrameWindowFeature WebMessaging,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.Notifications"/>
    INotificationsInfiniFrameWindowFeature Notifications,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.FilePickerDialogs"/>
    IFilePickerDialogsInfiniFrameWindowFeature FilePickerDialogs,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.Monitors"/>
    IMonitorsInfiniFrameWindowFeature Monitors,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.PageNavigation"/>
    IPageNavigationInfiniFrameWindowFeature PageNavigation,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.Position"/>
    IPositionInfiniFrameWindowFeature Position,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.Size"/>
    ISizeInfiniFrameWindowFeature Size,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.Decorations"/>
    IDecorationsInfiniFrameWindowFeature Decorations,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.State"/>
    IStateInfiniFrameWindowFeature State,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.Browser"/>
    IBrowserInfiniFrameWindowFeature Browser
) : IInfiniFrameWindowFeatures;
