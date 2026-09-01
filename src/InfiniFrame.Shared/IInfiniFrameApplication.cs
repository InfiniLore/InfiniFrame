// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents an InfiniFrame application singleton managing platform registration, message loop, and window collection.
///     One instance per process. Must be created before any windows and destroyed after all windows.
/// </summary>
public interface IInfiniFrameApplication : IDisposable, IAsyncDisposable {
    /// <summary>Gets the unique identifier for this application instance.</summary>
    Guid Id { get; }

    /// <summary>Gets the native application handle pointer.</summary>
    IntPtr ApplicationHandle { get; }

    /// <summary>Gets whether Shutdown() has been called.</summary>
    bool IsShutdownRequested { get; }

    /// <summary>
    ///     Initializes the application with the specified configuration.
    ///     Must be called before any windows are created.
    /// </summary>
    /// <param name="config">The application configuration.</param>
    void Initialize(ApplicationConfiguration config);

    /// <summary>
    ///     Runs the application message loop, blocking until all windows close or Shutdown() is called.
    /// </summary>
    void Run();

    /// <summary>
    ///     Runs the application message loop asynchronously.
    /// </summary>
    /// <param name="ct">A cancellation token to cancel the run operation.</param>
    /// <returns>A task that completes when the application exits.</returns>
    Task RunAsync(CancellationToken ct = default);

    /// <summary>
    ///     Signals the application message loop to exit. Safe to call from any thread.
    /// </summary>
    void Shutdown();
}
