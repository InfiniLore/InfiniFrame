// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Application-level owner for lazily built InfiniFrame windows.
/// </summary>
public sealed class InfiniFrameApplication(ILogger<InfiniFrameApplication> logger)
    : IInfiniFrameApplication {
    private readonly object _gate = new();
    private readonly List<(string? Id, Action<IInfiniFrameWindowBuilder> Configure)> _registrations = [];
    private readonly Dictionary<string, IInfiniFrameWindow> _windows = [];
    private int _disposed;
    private bool _built;

    /// <summary>Creates an application without requiring a dependency-injection container.</summary>
    public static InfiniFrameApplication Initialize()
        => new(NullLogger<InfiniFrameApplication>.Instance);

    /// <summary>Creates an application using the supplied logger.</summary>
    public static InfiniFrameApplication Initialize(ILogger<InfiniFrameApplication> logger) {
        ArgumentNullException.ThrowIfNull(logger);
        return new InfiniFrameApplication(logger);
    }

    /// <inheritdoc />
    public void RegisterWindow(Action<IInfiniFrameWindowBuilder> configure) {
        ArgumentNullException.ThrowIfNull(configure);
        RegisterWindowCore(null, configure);
    }

    /// <inheritdoc />
    public void RegisterWindow(string id, Action<IInfiniFrameWindowBuilder> configure) {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(configure);
        RegisterWindowCore(id, configure);
    }

    /// <summary>Registers an unnamed window and returns this application for fluent configuration.</summary>
    public InfiniFrameApplication WithWindow(Action<IInfiniFrameWindowBuilder> configure) {
        RegisterWindow(configure);
        return this;
    }

    /// <summary>Registers a named window and returns this application for fluent configuration.</summary>
    public InfiniFrameApplication WithWindow(string id, Action<IInfiniFrameWindowBuilder> configure) {
        RegisterWindow(id, configure);
        return this;
    }

    /// <inheritdoc />
    public IInfiniFrameWindow GetWindow(string id) {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        lock (_gate) {
            EnsureBuilt();
            return _windows.TryGetValue(id, out IInfiniFrameWindow? window)
                ? window
                : throw new KeyNotFoundException($"Window with id '{id}' was not found.");
        }
    }

    /// <inheritdoc />
    public IInfiniFrameWindow? TryGetWindow(string id) {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        lock (_gate) return _built && _windows.TryGetValue(id, out IInfiniFrameWindow? window) ? window : null;
    }

    /// <inheritdoc />
    public IReadOnlyList<IInfiniFrameWindow> Windows {
        get {
            lock (_gate) return _windows.Values.ToArray();
        }
    }

    /// <inheritdoc />
    public void Run() {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        BuildAllWindows();

        IInfiniFrameWindow[] windows = Windows.ToArray();
        if (windows.Length == 0) return;

        // The native layer currently exposes a per-window message loop. The application
        // native loop will replace this bridge when multi-window ownership is added.
        foreach (IInfiniFrameWindow window in windows) window.WaitForClose();
    }

    /// <inheritdoc />
    public async Task RunAsync(CancellationToken ct = default) {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        BuildAllWindows();
        using CancellationTokenRegistration registration = ct.Register(Shutdown);
        await Task.Run(Run, CancellationToken.None).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Shutdown() {
        IInfiniFrameWindow[] windows = Windows.ToArray();
        foreach (IInfiniFrameWindow window in windows) {
            try {
                window.Close();
            }
            catch (Exception ex) when (ex is ObjectDisposedException or InvalidOperationException) {
                logger.LogDebug(ex, "Window was already unavailable during application shutdown.");
            }
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        IInfiniFrameWindow[] windows;
        lock (_gate) {
            windows = _windows.Values.ToArray();
            _windows.Clear();
            _registrations.Clear();
        }

        foreach (IInfiniFrameWindow window in windows) {
            try { (window as IDisposable)?.Dispose(); }
            catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException) {
                logger.LogWarning(ex, "Failed to dispose an application window.");
            }
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        IInfiniFrameWindow[] windows;
        lock (_gate) {
            windows = _windows.Values.ToArray();
            _windows.Clear();
            _registrations.Clear();
        }

        foreach (IInfiniFrameWindow window in windows) {
            try {
                if (window is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                else (window as IDisposable)?.Dispose();
            }
            catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException) {
                logger.LogWarning(ex, "Failed to asynchronously dispose an application window.");
            }
        }
    }

    private void RegisterWindowCore(string? id, Action<IInfiniFrameWindowBuilder> configure) {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        lock (_gate) {
            if (_built) throw new InvalidOperationException("Cannot register windows after the application has run.");
            if (id is not null && _registrations.Any(registration => registration.Id == id))
                throw new ArgumentException($"A window with id '{id}' is already registered.", nameof(id));
            _registrations.Add((id, configure));
        }
    }

    private void BuildAllWindows() {
        lock (_gate) {
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
            if (_built) return;

            var built = new List<(string Id, IInfiniFrameWindow Window)>();
            try {
                foreach ((string? id, Action<IInfiniFrameWindowBuilder> configure) in _registrations) {
                    var builder = InfiniFrameWindowBuilder.Create();
                    configure(builder);
                    string windowId = id ?? Guid.NewGuid().ToString("N");
                    built.Add((windowId, builder.Build()));
                }

                foreach ((string id, IInfiniFrameWindow window) in built) _windows.Add(id, window);
                _registrations.Clear();
                _built = true;
            }
            catch {
                foreach ((_, IInfiniFrameWindow window) in built) (window as IDisposable)?.Dispose();
                throw;
            }
        }
    }

    private void EnsureBuilt()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        if (!_built) throw new InvalidOperationException("Windows have not been built yet. Call Run() or RunAsync() first.");
    }
}
