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

    /// <summary>Gets the number of windows currently tracked by this application.</summary>
    int WindowCount { get; }

    /// <summary>
    ///     Raised when a window is tracked by this application.
    ///     The handler receives the window that was created.
    /// </summary>
    event Action<IInfiniFrameWindow>? WindowCreated;

    /// <summary>
    ///     Raised when a window is untracked by this application.
    ///     The handler receives the window that was destroyed.
    /// </summary>
    event Action<IInfiniFrameWindow>? WindowDestroyed;

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

    /// <summary>
    ///     Closes all tracked windows gracefully.
    ///     Each window receives a close request; windows with closing callbacks may reject the close.
    ///     After all windows close, the application message loop exits automatically.
    /// </summary>
    void CloseAll();

    /// <summary>
    ///     Tracks a window as owned by this application and raises the WindowCreated event.
    ///     Called by the lifecycle feature after native window creation.
    /// </summary>
    /// <param name="window">The window to track.</param>
    void TrackWindow(IInfiniFrameWindow window);

    /// <summary>
    ///     Untracks a window and raises the WindowDestroyed event.
    ///     Called by the lifecycle feature during teardown.
    /// </summary>
    /// <param name="window">The window to untrack.</param>
    void UntrackWindow(IInfiniFrameWindow window);
}
