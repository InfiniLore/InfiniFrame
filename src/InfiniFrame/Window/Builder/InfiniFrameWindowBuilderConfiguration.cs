// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Stores builder-level configuration for an <see cref="IInfiniFrameWindow" />, including parent window and child
///     window
///     information that is applied to the native parameters before window creation.
/// </summary>
public class InfiniFrameWindowBuilderConfiguration : IInfiniFrameWindowBuilderConfiguration {
    /// <inheritdoc cref="IInfiniFrameWindowBuilderConfiguration.ChildWindows" />
    public List<IInfiniFrameWindow> ChildWindows { get; } = [];
    /// <inheritdoc cref="IInfiniFrameWindowBuilderConfiguration.ParentWindow" />
    public IInfiniFrameWindow? ParentWindow { get; set; }

    IReadOnlyList<IInfiniFrameWindow> IInfiniFrameWindowBuilderConfiguration.ChildWindows => ChildWindows;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowBuilderConfiguration.ApplyToNativeParameters" />
    public void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters) {
        // NativeParent is populated under a parent-handle lease immediately before native construction.
    }
}
