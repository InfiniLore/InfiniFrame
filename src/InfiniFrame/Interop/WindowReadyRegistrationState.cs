// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.CompilerServices;

namespace InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Tracks the registration state for windows created by a specific builder, including ready-handler registration and
///     per-window state machines.
/// </summary>
public sealed class WindowReadyRegistrationState {
    #if NET9_0_OR_GREATER
    /// <summary>Synchronization lock for thread-safe access to registration state.</summary>
    public readonly Lock Lock = new();
    #else
    /// <summary>Synchronization lock for thread-safe access to registration state.</summary>
    public readonly object Lock = new();
    #endif

    /// <summary>Gets or sets whether the ready handler has been registered for the associated builder.</summary>
    public bool ReadyHandlerRegistered { get; set; }
    /// <summary>Gets or sets whether the window-created handler has been registered for the associated builder.</summary>
    public bool WindowCreatedHandlerRegistered { get; set; }
    /// <summary>Gets the set of registration message IDs to send when a window signals readiness.</summary>
    public HashSet<string> RegistrationMessageIds { get; } = new(StringComparer.Ordinal);
    /// <summary>Gets the per-window registration states tracked for windows created by the associated builder.</summary>
    public ConditionalWeakTable<IInfiniFrameWindow, WindowRegistrationState> Windows { get; } = new();
}
