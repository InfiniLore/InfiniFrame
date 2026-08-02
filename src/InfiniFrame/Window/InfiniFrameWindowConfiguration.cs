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
    /// <inheritdoc cref="IInfiniFrameWindowConfiguration.ChildWindows"/>
    public List<IInfiniFrameWindow> ChildWindows { get; } = [];

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowConfiguration.AssignNativeParameters"/>
    public void AssignNativeParameters(InfiniFrameNativeParameters nativeParameters) {
        StartupParameters = nativeParameters;
    }
}