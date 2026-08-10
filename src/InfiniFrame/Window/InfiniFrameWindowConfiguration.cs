// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Stores configuration and runtime state for an <see cref="IInfiniFrameWindow" />, including native startup parameters,
///     parent/child window relationships, and assigned native parameters.
/// </summary>
public class InfiniFrameWindowConfiguration : IInfiniFrameWindowConfiguration {
    /// <inheritdoc cref="IInfiniFrameWindowConfiguration.StartupParameters"/>
    public InfiniFrameNativeParameters StartupParameters { get; private set; }
    /// <inheritdoc cref="IInfiniFrameWindowConfiguration.ParentWindow"/>
    public IInfiniFrameWindow? ParentWindow { get; set; }
    /// <summary>
    ///     Gets the mutable list of child windows.
    /// </summary>
    internal List<IInfiniFrameWindow> ChildWindowsInternal { get; } = [];
    /// <inheritdoc cref="IInfiniFrameWindowConfiguration.ChildWindows"/>
    IReadOnlyList<IInfiniFrameWindow> IInfiniFrameWindowConfiguration.ChildWindows => ChildWindowsInternal;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowConfiguration.AssignNativeParameters"/>
    public void AssignNativeParameters(InfiniFrameNativeParameters nativeParameters) {
        StartupParameters = nativeParameters;
    }
}