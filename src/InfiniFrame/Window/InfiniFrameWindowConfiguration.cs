// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Stores configuration and runtime state for an <see cref="IInfiniFrameWindow" />, including native startup
///     parameters,
///     parent/child window relationships, and assigned native parameters.
/// </summary>
public class InfiniFrameWindowConfiguration : IInfiniFrameWindowConfiguration {
    /// <summary>
    ///     Gets the mutable list of child windows.
    ///     All access to this list must be synchronized via <see cref="ChildWindowsLock" />.
    /// </summary>
    internal List<IInfiniFrameWindow> ChildWindowsInternal { get; } = [];
    /// <summary>
    ///     Dedicated lock object for synchronizing access to <see cref="ChildWindowsInternal" />.
    /// </summary>
    internal object ChildWindowsLock { get; } = new();
    /// <inheritdoc cref="IInfiniFrameWindowConfiguration.StartupParameters" />
    public InfiniFrameNativeParameters StartupParameters { get; private set; }
    /// <inheritdoc cref="IInfiniFrameWindowConfiguration.ParentWindow" />
    public IInfiniFrameWindow? ParentWindow { get; set; }
    /// <inheritdoc cref="IInfiniFrameWindowConfiguration.ChildWindows" />
    IReadOnlyList<IInfiniFrameWindow> IInfiniFrameWindowConfiguration.ChildWindows => ChildWindowsInternal;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowConfiguration.AssignNativeParameters" />
    public void AssignNativeParameters(InfiniFrameNativeParameters nativeParameters) {
        StartupParameters = nativeParameters;
    }
}
