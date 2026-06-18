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
    IInfiniFrameWindowFeatureDebugging Debugging,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.Lifecycle"/>
    IInfiniFrameWindowFeatureLifecycle Lifecycle,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.Invoke"/>
    IInfiniFrameWindowFeatureInvoke Invoke,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.WebMessaging"/>
    IInfiniFrameWindowFeatureWebMessaging WebMessaging,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.Notifications"/>
    IInfiniFrameWindowFeatureNotifications Notifications,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.FilePickerDialogs"/>
    IInfiniFrameWindowFeatureFilePickerDialogs FilePickerDialogs,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.Monitors"/>
    IInfiniFrameWindowFeatureMonitors Monitors,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.PageNavigation"/>
    IInfiniFrameWindowFeaturePageNavigation PageNavigation,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.Position"/>
    IInfiniFrameWindowFeaturePosition Position,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.Size"/>
    IInfiniFrameWindowFeatureSize Size,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.Decorations"/>
    IInfiniFrameWindowFeatureDecorations Decorations,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.State"/>
    IInfiniFrameWindowFeatureState State,
    /// <inheritdoc cref="IInfiniFrameWindowFeatures.Browser"/>
    IInfiniFrameWindowFeatureBrowser Browser
) : IInfiniFrameWindowFeatures;
