// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.CompilerServices;

namespace InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class WindowReadyRegistrationState {
    #if NET9_0_OR_GREATER
    public readonly Lock Lock = new();
    #else
    public readonly object Lock = new();
    #endif

    public bool ReadyHandlerRegistered { get; set; }
    public bool WindowCreatedHandlerRegistered { get; set; }
    public HashSet<string> RegistrationMessageIds { get; } = new(StringComparer.Ordinal);
    public ConditionalWeakTable<IInfiniFrameWindow, WindowRegistrationState> Windows { get; } = new();
}
