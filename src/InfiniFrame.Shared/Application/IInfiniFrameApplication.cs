// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Owns the windows registered for an InfiniFrame application.
/// </summary>
public interface IInfiniFrameApplication : IDisposable, IAsyncDisposable {
    /// <summary>Registers an unnamed window to be built when the application runs.</summary>
    void RegisterWindow(Action<IInfiniFrameWindowBuilder> configure);

    /// <summary>Registers a named window to be built when the application runs.</summary>
    void RegisterWindow(string id, Action<IInfiniFrameWindowBuilder> configure);

    /// <summary>Gets a built window by its application-local identifier.</summary>
    IInfiniFrameWindow GetWindow(string id);

    /// <summary>Tries to get a built window by its application-local identifier.</summary>
    IInfiniFrameWindow? TryGetWindow(string id);

    /// <summary>Gets the windows built by this application.</summary>
    IReadOnlyList<IInfiniFrameWindow> Windows { get; }

    /// <summary>Runs the application until its windows have closed.</summary>
    void Run();

    /// <summary>Runs the application asynchronously until its windows have closed.</summary>
    Task RunAsync(CancellationToken ct = default);

    /// <summary>Requests all owned windows to close.</summary>
    void Shutdown();
}
