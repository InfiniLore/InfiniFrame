// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Indicates that the implementing type provides access to an <see cref="IInfiniFrameEventsStore" />.
/// </summary>
public interface IHasInfiniFrameEventsStore {
    /// <summary>
    ///     Gets the event store containing event handler collections for window lifecycle and interaction events.
    /// </summary>
    IInfiniFrameEventsStore EventsStore { get; }
}
