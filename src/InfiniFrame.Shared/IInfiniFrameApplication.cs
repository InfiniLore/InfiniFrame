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
    ///     Registers a window to be built when Run() or RunAsync() is called.
    ///     The window is not created immediately — it is lazily built on the first Run/RunAsync call.
    /// </summary>
    /// <param name="id">A unique string identifier for the window.</param>
    /// <param name="configure">A callback to configure the window builder.</param>
    void RegisterWindow(string id, Action<IInfiniFrameWindowBuilder> configure);

    /// <summary>
    ///     Registers a window with an auto-generated GUID identifier.
    /// </summary>
    /// <param name="configure">A callback to configure the window builder.</param>
    void RegisterWindow(Action<IInfiniFrameWindowBuilder> configure);

    /// <summary>
    ///     Gets a previously registered window by its identifier.
    ///     Throws if Run() has not been called yet, or if the id is not found.
    /// </summary>
    /// <param name="id">The window identifier.</param>
    /// <returns>The window instance.</returns>
    IInfiniFrameWindow GetWindow(string id);

    /// <summary>
    ///     Tries to get a previously registered window by its identifier.
    ///     Returns null if Run() has not been called, or if the id is not found.
    /// </summary>
    /// <param name="id">The window identifier.</param>
    /// <returns>The window instance, or null.</returns>
    IInfiniFrameWindow? TryGetWindow(string id);

    /// <summary>
    ///     Gets all built windows. Empty until Run() is called.
    /// </summary>
    IReadOnlyList<IInfiniFrameWindow> Windows { get; }
}
