// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.CompilerServices;

namespace InfiniFrame.Js.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class ReadyRegistrationState {
    public bool ReadyHandlerRegistered { get; set; }
    public bool WindowCreatedHandlerRegistered { get; set; }
    public HashSet<string> RegistrationMessageIds { get; } = new(StringComparer.Ordinal);
    public ConditionalWeakTable<IInfiniFrameWindow, WindowRegistrationState> Windows { get; } = new();
}